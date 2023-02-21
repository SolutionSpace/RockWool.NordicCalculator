using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using REDAirCalculator.DAL;
using REDAirCalculator.Models.CalculationModels;
using REDAirCalculator.Utilities;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Forms.Core.Models;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Web.PublishedModels;

namespace REDAirCalculator.Controllers
{
    [AuthorizeUser]
    public class WindSpeedAreaController : SurfaceController
    {
        private readonly IUmbracoContextFactory _context;
        private readonly IFormRepository _formRepository;

        public WindSpeedAreaController(IUmbracoContextFactory context, IFormRepository formRepository)
        {
            _context = context;
            _formRepository = formRepository;
        }

        [HttpGet]
        public ActionResult Get(string area, string city)
        {
            string culture = Utilities.UrlHelper.GetUrlCulture(Request);

            IEnumerable<IPublishedContent> rootNodes = UmbracoContext.ContentCache.GetAtRoot().First().Children();
            FlexMultiCalculator calculatorPage = (FlexMultiCalculator)rootNodes.FirstOrDefault(d => d.ContentType.Alias == "flexMultiCalculator");

            // get current culture form
            string formGuid = calculatorPage.Value("selectForm", culture, fallback: Fallback.ToDefaultValue).ToString();

            Form form = FormHelper.GetForm(new Guid(formGuid));
            List<WindSpeedData> windSpeedData = WindSpeedParser.GetWindSpeedData(_context, Server, culture, form);

            double result = 0;
            
            if (string.IsNullOrEmpty(city))
            {
                result = windSpeedData.First(x => x.Area == area).WindSpeed;
            }

            if (!string.IsNullOrEmpty(area) && !string.IsNullOrEmpty(city))
            {
                result = windSpeedData.First(x => (x.Area == area) && (x.City == city)).WindSpeed;
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}