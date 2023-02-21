using System;
using System.Collections.Generic;
using System.Web.Mvc;
using REDAirCalculator.DAL;
using REDAirCalculator.Models.CalculationModels;
using REDAirCalculator.Models.DTO;
using Umbraco.Web.Mvc;
using REDAirCalculator.Utilities;
using Umbraco.Web;
using Umbraco.Web.PublishedModels;
using Umbraco.Core;
using Umbraco.Forms.Core.Models;

namespace REDAirCalculator.Controllers
{
    [AuthorizeUser]
    public class FlexMultiCalculatorController : RenderMvcController
    {
        private readonly IUmbracoContextFactory _context;
        private readonly IFormRepository _formRepository;
        private readonly string _lang;

        private readonly ISettingsRepository _settingsRepository;
        private readonly IFormDataRepository _formDataRepository;
        private readonly IProductsRepository _productsRepository;

        public FlexMultiCalculatorController(IUmbracoContextFactory context, IFormRepository formRepository)
        {
            _context = context;
            _formRepository = formRepository;
            _lang = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            _settingsRepository = new SettingsRepository(context, _lang);
            _formDataRepository = new FormDataRepository(context, _lang);
            _productsRepository = new ProductsRepository(context, _lang);
        }

        public ActionResult Calculator(FlexMultiCalculator model)
        {
            string guid = model.SelectForm.ToString();
            TempData["FormGuid"] = guid;
            TempData["Theme"] = "Calculator";
            TempData["IsLinkSystem"] = false;
            TempData["projectsType"] = 0;
            var currentEntryObjectGuid = TempData["Forms_Current_Record_id"];
            TempData["rootNodes"] = _context.EnsureUmbracoContext().UmbracoContext.ContentCache.GetAtRoot();

            // get windspeed excel
            Form form = FormHelper.GetForm(new Guid(TempData["formGuid"].ToString()));
            List<WindSpeedData> windSpeedData = WindSpeedParser.GetWindSpeedData(_context, Server, _lang, form);
            Dictionary<string, List<string>> windSpeedDict = WindSpeedParser.GetWindSpeedDict(windSpeedData);
            TempData["WindSpeedData"] = windSpeedDict;

            // check page state
            TempData["HasCalculationError"] = false;
            TempData["IsResulter"] = false;
            if (currentEntryObjectGuid != null)
            {
                // implementation for getting current submitted entry
                FormDataDto currentEntry = _formRepository.GetFlexMultiSubmittedEntry(new Guid(guid), currentEntryObjectGuid);
                if (currentEntry.SystemType == "FLEX" && currentEntry.InsulationThicknessMmFlex != null && _lang == "no")
                {
                    currentEntry.InsulationThickness = currentEntry.InsulationThicknessMmFlex;
                }
                else
                {
                    currentEntry.InsulationThicknessMmFlex = currentEntry.InsulationThickness;
                }

                // get calculated data
                ResultCalculator resultCalculator = new ResultCalculator(_context);
                CalculatedDataDto calculatedData = resultCalculator.GetResult(currentEntry, windSpeedData);

                TempData["FormData"] = currentEntry;
                TempData["CalculatedData"] = calculatedData;
                TempData["IsMulti"] = currentEntry.SystemType == "MULTI";
                TempData["ShowAllResults"] = currentEntry.ShowAllResults;
                TempData["AdvancedField"] = currentEntry.AdvancedField;

                if (!calculatedData.HasCalculationError)
                {
                    TempData["IsResulter"] = true;

                    // implementation for generating pdf
                    string pdfExtension = ".pdf";
                    Home rootPage = (Home)model.Parent;
                    TempData["PdfName"] = $"{currentEntry.ProjectName.ToFirstUpper()}-{rootPage.Title}{pdfExtension}";

                    string logoPath = Server.MapPath("/Content/images");
                    string downloadString = "/Downloads";
                    string downloadPath = Server.MapPath(downloadString);

                    // current language date time
                    string lang = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
                    DateTime languageDateTime = DateTimeLanguageConverter.Convert(DateTime.Now, lang);

                    PdfGenerator pdfGenerator = new PdfGenerator(
                        model,
                        guid,
                        currentEntryObjectGuid.ToString(),
                        currentEntry,
                        calculatedData,
                        logoPath,
                        downloadPath,
                        TempData["PdfName"].ToString().Replace(pdfExtension, ""),
                        languageDateTime,
                        _lang,
                        _context);
                    string pdfFile = pdfGenerator.Generate();

                    TempData["PdfLink"] = $"{downloadString}/{pdfFile}";
                }
                else
                {
                    TempData["PdfName"] = string.Empty;
                    TempData["PdfLink"] = string.Empty;
                }

                TempData["HasCalculationError"] = calculatedData.HasCalculationError;

                TempData["anchorScrewList"] = _settingsRepository.GetAnchorScrewDtos(currentEntry);
            }
            else
            {
                TempData["anchorScrewList"] = _settingsRepository.GetAnchorScrewDtos();
            }

            //for combinations
            TempData["anchorFrictionCombinations"] = _settingsRepository.GetAnchorFrictionCombinations();
            TempData["lcwThicknessCombinations"] = _productsRepository.GetLcwThicknessCombinations(_lang);
            TempData["baseRailSpacingList"] = _formDataRepository.GetBaseRailSpacingCombinationModels();

            return CurrentTemplate(model);
        }      

    }  
}