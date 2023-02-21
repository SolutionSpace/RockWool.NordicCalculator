using REDAirCalculator.Models.ResultViewModels;
using REDAirCalculator.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Web;
using REDAirCalculator.DAL;
using Umbraco.Web.PublishedModels;
using REDAirCalculator.Models.ResultModels;
using Umbraco.Core.Models.PublishedContent;

namespace REDAirCalculator.Utilities
{
    public class LinkResultsCalculator 
    {
        private readonly IUmbracoContextFactory _context;
        private readonly ILinkDataRepository _linkDataRepository;
        private readonly LinkConstants _constants;
        private readonly string _lang;

        public LinkResultsCalculator(IUmbracoContextFactory context)
        {
            _context = context;
            _lang = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            _linkDataRepository = new LinkDataRepository(context, _lang);
            _constants = _linkDataRepository.GetLinkConstants();
        }

        public LinkResultsViewModel GetResult(LinkFormDataDto formData)
        {
            try { 
            //data
            var products = _linkDataRepository.GetLinkProducts();
            var profiles = _linkDataRepository.GetProfileTypes();


            //calculations
            var resultCalculations = new LinkResultsViewModel();
            resultCalculations.InstalationInstractions =  GetInstallationInstructions(formData, profiles);
            resultCalculations.LinkCalculations = GetCalculationsResult(formData, products);
            resultCalculations.LinkCalculations.SumOfNeededLBM = Math.Round(resultCalculations.LinkCalculations.SumOfNeededLBM, 2);

                
                foreach (var b in formData.OpeningTypes)
                {
                    if(!b.isWindow)
                    b.PerItemJoiner += 2;
                }

                resultCalculations.LinkResultsPart = GetMainResults(formData, resultCalculations.LinkCalculations, products, profiles);
            resultCalculations.PrecutPlanks = formData.PrecutPlanks;
            return resultCalculations;
            }
            catch (InvalidOperationException)
            {
                return new LinkResultsViewModel
                {
                    HasCalculationError = true
                };
            }
        }


        //Calculations
        private List<OpeningTypeDto> GetInstallationInstructions(LinkFormDataDto formData, IEnumerable<IPublishedContent> profiles)
        {
            foreach(var t in formData.OpeningTypes)
            {
                var joinerWidth = t.Width > _constants.JoinerBracketRequirement ?
                                  Math.Ceiling(t.Width / _constants.JoinerBracketDistance - 3) : 0;
                var joinerHeight = t.Height > _constants.JoinerBracketRequirement ?
                                  Math.Ceiling(t.Height / _constants.JoinerBracketDistance - 3) : 0;
                
                if (t.isWindow)
                {                   
                    var angleBracketsParameterLargeHeight = formData.PlankDepth > _constants.AngleBracketThreshold ?
                                                Math.Ceiling((t.Width - 2 * _constants.CornerAngleBracketDistance )/ _constants.LBund_Max201_350mm) - 1 :
                                                Math.Ceiling((t.Width - 2 * _constants.CornerAngleBracketDistance )/ _constants.LBund_Max0200mm) - 1;

                    var angleBracketsParameterLowHeight = formData.PlankDepth > _constants.AngleBracketThreshold ?
                                                Math.Ceiling(t.Width / _constants.LBund_Max201_350mm) - 1 :
                                                Math.Ceiling(t.Width / _constants.LBund_Max0200mm) - 1;

                    t.PerItemAngleBrackets = t.Height > _constants.CornerAngleBracketCriterium ?
                                             angleBracketsParameterLargeHeight + 2 : angleBracketsParameterLowHeight;

                    t.PerItemLBM = ((t.Width + _constants.CutWaste + _constants.BoardThickness * 2) * 2 + (t.Height + _constants.CutWaste) * 2) / 1000;
                    t.PerItemJoiner = joinerWidth * 2 + joinerHeight * 2;
                    t.PerItemCorner = _constants.WindowCornersPerWindow;
                }
                else
                {
                    t.PerItemLBM = ((t.Width + _constants.CutWaste + _constants.BoardThickness * 2) + (t.Height + _constants.CutWaste) * 2) / 1000;
                    t.PerItemJoiner = joinerWidth + joinerHeight * 2;
                    t.PerItemCorner = _constants.DoorCornersPerDoor;
                }
                t.LBM = t.PerItemLBM * t.Amount;
                t.AngleBrackets = t.PerItemAngleBrackets * t.Amount;
                t.Joiner = t.PerItemJoiner * t.Amount;
                t.Corner = t.PerItemCorner * t.Amount;
                t.SelfWeight = Math.Round(t.PerItemLBM * formData.PlankDepth / 1000 * _constants.BoardThickness / 1000 * _constants.Density,1);

                t.AdditionalCornerScrewsPerCorner = 
                    Math.Floor(
                        (formData.PlankDepth - 
                            (formData.PlankDepth > _constants.BracketThicknessThreshold 
                            ? profiles.Select(x => (ProfileType)x).First(x => x.Name.Contains("CL")).Length
                            : profiles.Select(x => (ProfileType)x).First(x => x.Name.Contains("CS")).Length)
                        - _constants.AdditionalCornerScrewMinimumDistance) / _constants.AdditionalCornerScrewSpacing
                    );

                if(t.AdditionalCornerScrewsPerCorner < 0)
                {
                    t.AdditionalCornerScrewsPerCorner += 1;
                }

                t.AdditionalCornerScrews = t.AdditionalCornerScrewsPerCorner * t.Corner;

                //Rounding
                t.PerItemLBM = Math.Round(t.PerItemLBM, 2);
            }

            return formData.OpeningTypes;
        }

        private LinkCalculationsModel GetCalculationsResult(LinkFormDataDto formData, IEnumerable<IPublishedContent> products)
        {
            var result = new LinkCalculationsModel();
            result.SumOfNeededLBM = formData.OpeningTypes.Sum(t => t.LBM) * (1 + (double)_constants.LengthWaste / 100);
            var selectedProduct = formData.PrecutPlanks == true ?
                                      products.Where(x => x.Id == 6144).Select(x => (LinkProduct)x).First() :
                                      products.Where(x => x.Id == 6146).Select(x => (LinkProduct)x).First();
            result.OnePlateLayerLBM = selectedProduct.Length * Math.Floor((selectedProduct.Width + _constants.CutWaste) / ((double)formData.PlankDepth + _constants.CutWaste))/1000;
            result.FullBoards = Math.Ceiling(result.SumOfNeededLBM / result.OnePlateLayerLBM);
            return result;
        }
        
        private List<LinkResultsItemModel> GetMainResults(LinkFormDataDto formData, LinkCalculationsModel calculations, IEnumerable<IPublishedContent> products, IEnumerable<IPublishedContent> profiles)
        {
            var resulterProducts = new List<LinkResultsItemModel>();

            #region Products from Excel
            //Products which setted up in Umbraco with ids : 6144, 6146, 6147
            //6144 - REDAir Link Board 48x_x3000
            //6146 - REDAir Link Board 48x1200x1500, Palle med 8 stk.
            //6147 - REDAir Link Board 48x1200x1500, Palle med 2 stk.
            var board1 = products.Where(x => x.Id == 6144).Select(x => (LinkProduct)x).First();
            var board2 = products.Where(x => x.Id == 6146).Select(x => (LinkProduct)x).First();
            var board3 = products.Where(x => x.Id == 6147).Select(x => (LinkProduct)x).First();
            #endregion

            #region Products calculations

            var unitsDictionary = new UnitsNames(_context);
            // resulter product 1
            var product1 = new LinkResultsItemModel();
            product1.Description = board1.Name.Substring(0, board1.Name.IndexOf("_")) + formData.PlankDepth + board1.Name.Substring(board1.Name.IndexOf("_") + 1);
            product1.Units = formData.PrecutPlanks == true ? Math.Ceiling(calculations.SumOfNeededLBM / (board1.Length / 1000)) : 0;
            product1.UnitsInPlural = unitsDictionary.GetUnitsByNameInPlural( board1.UnitsLabel );
            product1.UnitsInSingular = unitsDictionary.GetUnitsByNameInSingular(board1.UnitsLabel);
            product1.DeliveredLBM = product1.Units != 0 ? product1.Units * board1.Length / 1000 : 0;
            product1.PlankDepth = product1.Units != 0 ? formData.PlankDepth : 0;
            product1.DeliveredM2 = product1.Units != 0 ? product1.Units * board1.Length / 1000 * product1.PlankDepth / 1000 : 0;
            product1.DBNumber = board1.DBnumber;
            product1.DBLabel = board1.DBlabel;
            product1.SAPNumber = board1.SapNumber;

            // resulter product 2
            var product2 = new LinkResultsItemModel();
            product2.Description = board2.Name;
            product2.Units = formData.PrecutPlanks == false ? Math.Floor( Math.Ceiling(calculations.FullBoards / (double)board3.AmountInPallet) / (board2.AmountInPallet / (double)board3.AmountInPallet)) : 0;
            product2.UnitsInPlural = unitsDictionary.GetUnitsByNameInPlural( board2.UnitsLabel );
            product2.UnitsInSingular = unitsDictionary.GetUnitsByNameInSingular(board2.UnitsLabel);
            product2.DeliveredM2 = product2.Units != 0 ? Math.Round(product2.Units * (double)board2.SqrInPallet,2) : 0;
            product2.DBNumber = board2.DBnumber;
            product2.DBLabel = board2.DBlabel;
            product2.SAPNumber = board2.SapNumber;

            // resulter product 3
            var product3 = new LinkResultsItemModel();
            product3.Description = board3.Name;
            product3.Units = formData.PrecutPlanks == false ? Math.Ceiling(Math.Max(calculations.FullBoards - product2.Units*board2.AmountInPallet,0)/(double)board3.AmountInPallet ) : 0;
            product3.UnitsInPlural = unitsDictionary.GetUnitsByNameInPlural( board3.UnitsLabel );
            product3.UnitsInSingular = unitsDictionary.GetUnitsByNameInSingular(board3.UnitsLabel);
            product3.DeliveredM2 = product3.Units != 0 ? Math.Round(product3.Units * (double)board3.SqrInPallet, 2) : 0;
            product3.DBNumber = board3.DBnumber;
            product3.DBLabel = board3.DBlabel;
            product3.SAPNumber = board3.SapNumber;


            //Corner profile - hjørneprofil
            var cornerProfile = new LinkResultsItemModel();
            var selectedProfile = formData.PlankDepth > _constants.BracketThicknessThreshold ? profiles.Select(x => (ProfileType)x).First(x => x.Name.Contains("CL")):
                                                                                               profiles.Select(x => (ProfileType)x).First(x => x.Name.Contains("CS"));
            cornerProfile.Description = selectedProfile.Name;
            cornerProfile.UsedBrackets = formData.OpeningTypes.Sum(x => x.Corner);
            cornerProfile.Units = Math.Ceiling(cornerProfile.UsedBrackets / selectedProfile.AmountInPackage);
            cornerProfile.DeliveredBrackets = cornerProfile.Units * selectedProfile.AmountInPackage;
            cornerProfile.UnitsInPlural = unitsDictionary.GetUnitsByNameInPlural(selectedProfile.UnitsLabel);
            cornerProfile.UnitsInSingular = unitsDictionary.GetUnitsByNameInSingular(selectedProfile.UnitsLabel);
            cornerProfile.DBNumber = selectedProfile.DBnumber;
            cornerProfile.DBLabel = selectedProfile.DBlabel;
            cornerProfile.SAPNumber = selectedProfile.SapNumber;


            //Assembly profile - samleprofil
            var assemblyProfile = new LinkResultsItemModel();
            selectedProfile = formData.PlankDepth > _constants.BracketThicknessThreshold ? profiles.Select(x => (ProfileType)x).First(x => x.Name.Contains("EL")) :
                                                                                               profiles.Select(x => (ProfileType)x).First(x => x.Name.Contains("ES"));
            assemblyProfile.Description = selectedProfile.Name;
            assemblyProfile.UsedBrackets = formData.OpeningTypes.Sum(x => x.Joiner);
            assemblyProfile.Units = Math.Ceiling(assemblyProfile.UsedBrackets / selectedProfile.AmountInPackage);
            assemblyProfile.DeliveredBrackets = assemblyProfile.Units * selectedProfile.AmountInPackage;
            assemblyProfile.UnitsInPlural = unitsDictionary.GetUnitsByNameInPlural(selectedProfile.UnitsLabel);
            assemblyProfile.UnitsInSingular = unitsDictionary.GetUnitsByNameInSingular(selectedProfile.UnitsLabel);
            assemblyProfile.DBNumber = selectedProfile.DBnumber;
            assemblyProfile.DBLabel = selectedProfile.DBlabel;
            assemblyProfile.SAPNumber = selectedProfile.SapNumber;

            //Additional corner screws - REDAir Link 90 Screws
            var additionalCornerScrews = new LinkResultsItemModel();
            selectedProfile = profiles.Select(x => (ProfileType)x).Last(x => x.SapNumber == 306785);

            additionalCornerScrews.Description = selectedProfile.Name;
            additionalCornerScrews.UsedScrews = formData.OpeningTypes.Sum(x => x.AdditionalCornerScrews);
            additionalCornerScrews.Units = Math.Ceiling(additionalCornerScrews.UsedScrews / selectedProfile.AmountInPackage);
            additionalCornerScrews.DeliveredScrews = additionalCornerScrews.Units * selectedProfile.AmountInPackage;
            additionalCornerScrews.UnitsInPlural = unitsDictionary.GetUnitsByNameInPlural(selectedProfile.UnitsLabel);
            additionalCornerScrews.UnitsInSingular = unitsDictionary.GetUnitsByNameInSingular(selectedProfile.UnitsLabel);
            additionalCornerScrews.DBLabel = selectedProfile.DBlabel;
            additionalCornerScrews.DBNumber = selectedProfile.DBnumber;
            additionalCornerScrews.SAPNumber = selectedProfile.SapNumber;
            additionalCornerScrews.HasEmptyMessage = selectedProfile.HasEmptyMessage;

            //Brackets - Vinkelbeslag (2 kN, længde til under vindue)
            var selectedBracket = profiles.Select(x => (ProfileType)x).First(x => x.AmountInPackage == 0);

            var bracket = new LinkResultsItemModel();
            bracket.Description = selectedBracket.Name;
            bracket.UsedBrackets = formData.OpeningTypes.Sum(x => x.AngleBrackets);
            bracket.Units = bracket.UsedBrackets;
            bracket.UnitsInPlural = unitsDictionary.GetUnitsByNameInPlural(selectedBracket.UnitsLabel);
            bracket.UnitsInSingular = unitsDictionary.GetUnitsByNameInSingular(selectedBracket.UnitsLabel);
            bracket.DBLabel = selectedBracket.DBlabel;
            bracket.DBNumber = selectedBracket.DBnumber;
            bracket.SAPNumber = selectedBracket.SapNumber;
            bracket.HasEmptyMessage = selectedBracket.HasEmptyMessage;

            #endregion


            product1.Units = Math.Round(product1.Units, 2);
            product2.Units = Math.Round(product2.Units, 2);
            product3.Units = Math.Round(product3.Units, 2);
            cornerProfile.Units = Math.Round(cornerProfile.Units, 2);
            assemblyProfile.Units = Math.Round(assemblyProfile.Units, 2);
            bracket.Units = Math.Round(bracket.Units, 2);

            resulterProducts.Add(product1);
            resulterProducts.Add(product2);
            resulterProducts.Add(product3);
            resulterProducts.Add(cornerProfile);
            resulterProducts.Add(assemblyProfile);
            resulterProducts.Add(additionalCornerScrews);
            resulterProducts.Add(bracket);

            return resulterProducts;

        }
    }
}