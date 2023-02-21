using System;
using System.Collections.Generic;
using System.Linq;
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
    public class LinkCalculatorController : RenderMvcController
    {
        private readonly IUmbracoContextFactory _context;
        private readonly IFormRepository _formRepository;
        private readonly string _lang;


        public LinkCalculatorController(IUmbracoContextFactory context, IFormRepository formRepository)
        {
            _context = context;
            _formRepository = formRepository;
            _lang = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
        }

        public ActionResult LinkCalculator(LinkCalculator model)
        {
            string guid = model.SelectForm.ToString();
            TempData["FormGuid"] = guid;
            TempData["Theme"] = "Calculator";
            TempData["IsLinkSystem"] = true;
            TempData["projectsType"] = 1;
            var currentEntryObjectGuid = TempData["Forms_Current_Record_id"];

            // check page state
            TempData["HasLinkCalculationError"] = false;
            TempData["IsResulter"] = false;

            // get windspeed excel
            if (_lang != "da" && _lang != "en" && _lang != "fi")
            {
                Form form = FormHelper.GetForm(new Guid(TempData["formGuid"].ToString()));
                List<WindSpeedData> windSpeedData = WindSpeedParser.GetWindSpeedData(_context, Server, _lang, form);
                Dictionary<string, List<string>> windSpeedDict = WindSpeedParser.GetWindSpeedDict(windSpeedData);
                TempData["WindSpeedData"] = windSpeedDict;
            }
            if (currentEntryObjectGuid != null)
            {
                // implementation for getting current submitted entry
                LinkFormDataDto currentEntry =
                    _formRepository.GetLinkSubmittedEntry(new Guid(guid), currentEntryObjectGuid);

                // get calculated data
                var linkCalculator = new LinkResultsCalculator(_context);
                var calculatedData = linkCalculator.GetResult(currentEntry);

                TempData["FormData"] = currentEntry;
                TempData["CalculatedLinkData"] = calculatedData;

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

                    // adding of print data from flex/multi page
                    FlexMultiCalculator flexMultiCalculatorPage =
                        (FlexMultiCalculator)rootPage.Children.FirstOrDefault(d =>
                           d.ContentType.Alias == "flexMultiCalculator");

                    LinkPdfGenerator pdfGenerator = new LinkPdfGenerator(
                        model,
                        flexMultiCalculatorPage,
                        guid,
                        currentEntryObjectGuid.ToString(),
                        currentEntry,
                        calculatedData,
                        logoPath,
                        downloadPath,
                        TempData["PdfName"].ToString().Replace(pdfExtension, ""),
                        languageDateTime,
                        _lang);
                    string pdfFile = pdfGenerator.Generate();

                    TempData["PdfLink"] = $"{downloadString}/{pdfFile}";
                }
                else
                {
                    TempData["PdfName"] = string.Empty;
                    TempData["PdfLink"] = string.Empty;
                }

                // for opening types
                TempData["openingTypesList"] = currentEntry.OpeningTypes;
                TempData["HasLinkCalculationError"] = calculatedData.HasCalculationError;
            }

            return CurrentTemplate(model);
        }

    }

}