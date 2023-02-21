using REDAirCalculator.Models.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using REDAirCalculator.DAL;
using REDAirCalculator.Models.CalculationModels;
using REDAirCalculator.Models.ResultModels;
using Umbraco.Web;
using Umbraco.Web.PublishedModels;

namespace REDAirCalculator.Utilities
{
    public class QuantityCalculator
    {
        private readonly IUmbracoContextFactory _context;
        private readonly UnitsNames _unitsNames;
        private readonly string _lang;
        private readonly IProductsRepository _productsRepository;
        private readonly ISettingsRepository _settingsRepository;
        private readonly Constants _constants;

        public QuantityCalculator(IUmbracoContextFactory context, string lang)
        {
            _lang = lang;
            _context = context;
            _unitsNames = new UnitsNames(_context);
            _productsRepository = new ProductsRepository(context, lang);
            _settingsRepository = new SettingsRepository(context, lang);
            _constants = _settingsRepository.GetConstants();
        }

        public QuantityCalculationsModel GetQuantityResult(ProcessedFormDataDto processedData, FormDataDto formData)
        {
            var rail = _productsRepository.GetRail(formData);
            var screws = _productsRepository.GetScrews(processedData);
            var showAllResults = formData.ShowAllResults;
            var roundPartCalculation = new PartCalculationsModel()
            {
                BaseRail = (double)rail.Ibm
            };
            var presize = GetPresize(processedData, formData, roundPartCalculation);
            var partCalculations = GetPartCalculations(presize, roundPartCalculation);

            var result = new QuantityCalculationsModel();
            result.Rail = GetConvertedRail(rail, partCalculations, presize);

            result.MineralWool = GetConvertedMineralWool(processedData, formData, partCalculations, presize).ToList();
            result.Accessories = GetAccessories(formData, partCalculations, presize, showAllResults).ToList();
            result.Screws = screws == null ? null : GetConvertedScrews(screws, partCalculations, presize);

            return result;
        }



        #region Converting Products
        private ProductDto GetConvertedRail(Rail rail, PartCalculationsModel partCalculations, PartCalculationsModel presize)
        {
            var product = new ProductDto
            {
                Name = rail.Name,
                HasDbNumber = CalculatorHelper.HasDbNumber(rail.NumberDB, _lang),
                DBNumber = rail.NumberDB,
                DBLabel = rail.LabelDB,
                SAPNumber = rail.NumberSap,
                Units = _unitsNames.GetUnitsByNameInPlural(rail.Units),
                UnitInSingular = _unitsNames.GetUnitsByNameInSingular(rail.Units),
                Amount = (int)(partCalculations.BaseRail / (double)rail.Ibm),
                Nessasary = new UnitTypeModel(Math.Round(presize.BaseRail, 2), _unitsNames.M),
                InPackaging = new UnitTypeModel(partCalculations.BaseRail, _unitsNames.M),
                UnitsPerPackage = new UnitTypeModel((double)rail.Ibm, _unitsNames.Ibm)
            };
            return product;
        }
        private ProductDto GetConvertedScrews(Screws screws, PartCalculationsModel partCalculations, PartCalculationsModel presize)
        {
            var product = new ProductDto
            {
                Name = screws.Name,
                HasDbNumber = CalculatorHelper.HasDbNumber(screws.NumberDB, _lang),
                DBNumber = screws.NumberDB,
                DBLabel = screws.LabelDB,
                Units = _unitsNames.GetUnitsByNameInPlural(screws.Units),
                UnitInSingular = _unitsNames.GetUnitsByNameInSingular(screws.Units),
                SAPNumber = screws.NumberSap,
                Amount = (int)(partCalculations.Screws / (double)screws.ElementsPerPackage),
                Nessasary = new UnitTypeModel(Math.Round(presize.Screws, 2), _unitsNames.Stk),
                InPackaging = new UnitTypeModel(partCalculations.Screws, _unitsNames.Stk),
                UnitsPerPackage = new UnitTypeModel((double)screws.ElementsPerPackage, _unitsNames.Stk)
            };
            return product;
        }
        private IEnumerable<ProductDto> GetConvertedFlexAccessories(PartCalculationsModel partCalculations, PartCalculationsModel presize)
        {
            var products = _productsRepository.GetFlexAccessories();
            var result = new List<ProductDto>();
            foreach (var product in products)
            {
                var resultProduct = new ProductDto
                {
                    Name = product.Name
                };

                if (product.GetType() == typeof(FP))
                {
                    resultProduct.HasDbNumber = CalculatorHelper.HasDbNumber(((FP)product).NumberDB, _lang);
                    resultProduct.DBNumber = ((FP)product).NumberDB;
                    resultProduct.SAPNumber = ((FP)product).NumberSap;
                    resultProduct.DBLabel = ((FP)product).LabelDB;
                    resultProduct.Units = _unitsNames.GetUnitsByNameInPlural(((FP)product).Units);
                    resultProduct.UnitInSingular = _unitsNames.GetUnitsByNameInSingular(((FP)product).Units);
                    resultProduct.Amount = (int)(partCalculations.FP / ((FP)product).ElementsPerPackage);
                    resultProduct.Nessasary = new UnitTypeModel(Math.Round(presize.FP, 2), _unitsNames.Stk);
                    resultProduct.InPackaging = new UnitTypeModel(partCalculations.FP, _unitsNames.Stk);
                    resultProduct.UnitsPerPackage = new UnitTypeModel((double)((FP)product).ElementsPerPackage, _unitsNames.Stk);
                }
                else if (product.GetType() == typeof(Startkit))
                {
                    resultProduct.HasDbNumber = CalculatorHelper.HasDbNumber(((Startkit)product).NumberDB, _lang);
                    resultProduct.DBLabel = ((Startkit)product).LabelDB;
                    resultProduct.DBNumber = ((Startkit)product).NumberDB;
                    resultProduct.SAPNumber = ((Startkit)product).NumberSap;
                    resultProduct.Units = _unitsNames.GetUnitsByNameInPlural(((Startkit)product).Units);
                    resultProduct.UnitInSingular = _unitsNames.GetUnitsByNameInSingular(((Startkit)product).Units);
                    resultProduct.Amount = 1;
                    resultProduct.Nessasary = new UnitTypeModel(1, "");
                    resultProduct.InPackaging = new UnitTypeModel(1, "");
                    resultProduct.UnitsPerPackage = new UnitTypeModel(1, "");
                }
                result.Add(resultProduct);
            }
            return result;
        }

        private IEnumerable<ProductDto> GetConvertedMultiAccessories(PartCalculationsModel partCalculations, PartCalculationsModel presize, bool showAllResults)
        {
            var products = _productsRepository.GetMultiAccessories();
            var result = new List<ProductDto>();

            foreach (var product in products)
            {
                var resultProduct = new ProductDto
                {
                    Name = product.Name
                };

                if (product.GetType() == typeof(Rail) && showAllResults)
                {
                    resultProduct.Units = _unitsNames.GetUnitsByNameInPlural(((Rail)product).Units);
                    resultProduct.UnitInSingular = _unitsNames.GetUnitsByNameInSingular(((Rail)product).Units);
                    resultProduct.Amount = (int)(partCalculations.BaseRail / (double)((Rail)product).Ibm);
                    resultProduct.HasDbNumber = CalculatorHelper.HasDbNumber(((Rail)product).NumberDB, _lang);
                    resultProduct.DBNumber = ((Rail)product).NumberDB;
                    resultProduct.DBLabel = ((Rail)product).LabelDB;
                    resultProduct.SAPNumber = ((Rail)product).NumberSap;
                    resultProduct.Nessasary = new UnitTypeModel(Math.Round(presize.BaseRail, 2), _unitsNames.M);
                    resultProduct.InPackaging = new UnitTypeModel(partCalculations.BaseRail, _unitsNames.M);
                    resultProduct.UnitsPerPackage = new UnitTypeModel((double)((Rail)product).Ibm, _unitsNames.Ibm);
                    result.Add(resultProduct);
                }
                else if (product.GetType() == typeof(MultiAccessory))
                {
                    resultProduct.Units = _unitsNames.GetUnitsByNameInPlural(((MultiAccessory)product).Units);
                    resultProduct.UnitInSingular = _unitsNames.GetUnitsByNameInSingular(((MultiAccessory)product).Units);
                    resultProduct.HasDbNumber = CalculatorHelper.HasDbNumber(((MultiAccessory)product).NumberDB, _lang);
                    resultProduct.DBLabel = ((MultiAccessory)product).LabelDB;
                    resultProduct.DBNumber = ((MultiAccessory)product).NumberDB;
                    resultProduct.SAPNumber = ((MultiAccessory)product).NumberSap;
                    resultProduct.UnitsPerPackage = new UnitTypeModel((double)((MultiAccessory)product).ElementsPerPackage, _unitsNames.Stk);
                    switch (((MultiAccessory)product).Type)
                    {
                        case "SC":
                            resultProduct.Amount = (int)(partCalculations.Screws / (double)((MultiAccessory)product).ElementsPerPackage);
                            resultProduct.Nessasary = new UnitTypeModel(Math.Round(presize.Screws, 2), _unitsNames.Stk);
                            resultProduct.InPackaging = new UnitTypeModel(partCalculations.Screws, _unitsNames.Stk);
                            result.Add(resultProduct);
                            break;
                        case "SB" when showAllResults:
                            resultProduct.Amount = (int)(partCalculations.SBBrackets / (double)((MultiAccessory)product).ElementsPerPackage);
                            resultProduct.Nessasary = new UnitTypeModel(Math.Round(presize.SBBrackets, 2), _unitsNames.Stk);
                            resultProduct.InPackaging = new UnitTypeModel(partCalculations.SBBrackets, _unitsNames.Stk);
                            result.Add(resultProduct);
                            break;
                        case "FB" when showAllResults:
                            resultProduct.Amount = (int)(partCalculations.FBBrackets / (double)((MultiAccessory)product).ElementsPerPackage);
                            resultProduct.Nessasary = new UnitTypeModel(Math.Round(presize.FBBrackets, 2), _unitsNames.Stk);
                            resultProduct.InPackaging = new UnitTypeModel(partCalculations.FBBrackets, _unitsNames.Stk);
                            result.Add(resultProduct);
                            break;
                    }
                }

            }
            return result.OrderBy(x => x.SAPNumber);
        }

        private IEnumerable<ProductDto> GetAccessories(FormDataDto formData, PartCalculationsModel partCalculations, PartCalculationsModel presize, bool showAllResults)
        {
            switch (formData.SystemType)
            {
                case "FLEX":
                    return GetConvertedFlexAccessories(partCalculations, presize);
                case "MULTI":
                    return GetConvertedMultiAccessories(partCalculations, presize, showAllResults);
                default:
                    return null;
            }
        }

        private IEnumerable<MineralWoolProd> GetMinWoolProducts(ProcessedFormDataDto processedFormData, PartCalculationsModel presize, string layers)
        {
            if (processedFormData.IsCalculationThickness)
            {
                processedFormData.InsulationThickness = 110;
            }

            var mineralWool = _productsRepository.GetMinWoolByThickness(processedFormData);

            var result = new List<MineralWoolProd>();

            if (mineralWool.DisplayedItems == null || ((processedFormData.InsulationThickness == 250 || processedFormData.InsulationThickness == 200) && layers == "1-Layer"))
            {
                result.Add(mineralWool);
            }
            else
            {
                result.AddRange(mineralWool.DisplayedItems.Cast<MineralWoolProd>());
            }
            return result;
        }

        private IEnumerable<ProductDto> GetConvertedMineralWool(
            ProcessedFormDataDto processedFormData,
            FormDataDto formData,
            PartCalculationsModel partCalculations,
            PartCalculationsModel presize
            )
        {
            var result = new List<ProductDto>();
            var minWoolProducts = GetMinWoolProducts(processedFormData, presize, formData.NumberOfLayers);
            foreach (var item in minWoolProducts)
            {
                var product = new ProductDto
                {
                    Name = item.Name,
                    HasDbNumber = CalculatorHelper.HasDbNumber(item.NumberDB, _lang),
                    DBNumber = item.NumberDB,
                    DBLabel = item.LabelDB,
                    SAPNumber = item.NumberSap,
                    Units = _unitsNames.GetUnitsByNameInPlural(item.Units),
                    UnitInSingular = _unitsNames.GetUnitsByNameInSingular(item.Units),
                    Nessasary = new UnitTypeModel(Math.Round(presize.Batts, 2), _unitsNames.M2),
                    Amount = (int)Math.Ceiling(Math.Round(Ceiling(partCalculations.Batts, (double)item.Volume) / (double)item.Volume, 4))
                };
                if (processedFormData.InsulationThickness == 300 || (processedFormData.InsulationThickness == 200 && formData.NumberOfLayers == "2-Layers" ))
                {
                    product.Amount *= 2;
                    product.Nessasary = new UnitTypeModel(Math.Round(presize.Batts * 2, 2), _unitsNames.M2);
                    product.InPackaging = new UnitTypeModel(partCalculations.Batts * 2, _unitsNames.M2);
                }
                product.UnitsPerPackage = new UnitTypeModel((double)item.Volume, _unitsNames.M2);
                product.InPackaging = new UnitTypeModel(Math.Round(product.Amount * product.UnitsPerPackage.Value, 2), _unitsNames.M2);
                result.Add(product);
            }

            return result;
        }

        #endregion

        #region Settings Calculations

        private PartCalculationsModel GetPresize(ProcessedFormDataDto processedData, FormDataDto formData, PartCalculationsModel roundMeasuresParam)
        {
            var presize = new PartCalculationsModel
            {
                BaseRail = (double)formData.Area / ((double)processedData.BaseRailSpacing / (double)1000) +
                           formData.LengthCorners + formData.LengthDoorsWindowsSide
            };

            presize.Screws = Ceiling(Math.Ceiling(presize.BaseRail), roundMeasuresParam.BaseRail) / 3 * processedData.NumberOfScrews;
            presize.Batts = Math.Ceiling(formData.Area * (1 + (double)_constants.LenghtWaste / 100));
            presize.FP = Math.Ceiling(presize.BaseRail);
            if (formData.SystemType == "MULTI")
            {
                var bracketFactor = GetBracketFactor(processedData);
                presize.BracketsTotal = presize.BaseRail * 1000 / (double)processedData.MaxDistance;
                presize.FBBrackets = presize.BracketsTotal * bracketFactor.FBFactor;
                presize.SBBrackets = presize.BracketsTotal * bracketFactor.SBFactor;
            }

            return presize;
        }
        private PartCalculationsModel GetPartCalculations(PartCalculationsModel presize, PartCalculationsModel roundMeasuresParam)
        {
            var result = new PartCalculationsModel
            {
                BaseRail = Ceiling(Math.Ceiling(presize.BaseRail), roundMeasuresParam.BaseRail),
                Screws = Ceiling(Math.Ceiling(presize.Screws), 25),
                Batts = presize.Batts,
                FP = Ceiling(presize.FP, 16),
                BracketsTotal = Math.Ceiling(presize.BracketsTotal),
                FBBrackets = Ceiling(Math.Ceiling(presize.FBBrackets), 15),
                SBBrackets = Ceiling(Math.Ceiling(presize.SBBrackets), 15)
            };


            return result;
        }

        private BracketFactorModel GetBracketFactor(ProcessedFormDataDto processedData)
        {
            var result = new BracketFactorModel
            {
                Selected = (double)3000 / (double)processedData.MaxDistance
            };
            result.Factor = 1 / result.Selected;
            result.FBFactor = result.Factor * 1;
            result.SBFactor = (result.Selected - 1) * result.Factor;
            return result;

        }

        private double Ceiling(double value, double significance)
        {
            decimal accValue = Convert.ToDecimal(value);
            decimal accSignificance = Convert.ToDecimal(significance);

            if ((accValue % accSignificance) != 0)
            {
                decimal res = ((int)(accValue / accSignificance) * accSignificance) + accSignificance;
                return Convert.ToDouble(res);
            }
            return value;
        }
        #endregion

    }
}