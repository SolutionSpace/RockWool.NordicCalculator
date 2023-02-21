using System.Web.Mvc;
using REDAirCalculator.DAL;
using REDAirCalculator.Models.DTO;
using REDAirCalculator.Utilities;
using Umbraco.Web;
using Umbraco.Web.Mvc;

namespace REDAirCalculator.Controllers
{
    public class TablesController : SurfaceController
    {
        private readonly IUmbracoContextFactory _context;

        public TablesController(IUmbracoContextFactory context)
        {
            _context = context;
        }

        [System.Web.Http.HttpPost]
        public ActionResult GetFlexTableData(FormDataDto formData)
        {
            string culture = Utilities.UrlHelper.GetUrlCulture(Request);

            string flexTableUrl = HttpContext.Request.UrlReferrer.AbsolutePath;

            var _formDataRepository = new FormDataRepository(_context, culture);
            var _settingsRepository = new SettingsRepository(_context, culture);

            var tableCalculator = new TableCalculator(_context, _settingsRepository, _formDataRepository, culture);
          
            var data = tableCalculator.GetDataValue(formData);
            var tableResults = tableCalculator.GenerateAdminFlexTable(data);

            TempData["data"] = tableResults;
            TempData["defaultValues"] = formData;

            return Redirect(flexTableUrl);
        }

        [System.Web.Http.HttpPost]
        public ActionResult GetMultiTableData(FormDataDto formData)
        {
            string culture = Utilities.UrlHelper.GetUrlCulture(Request);

            string multiTableUrl = HttpContext.Request.UrlReferrer.AbsolutePath;

            var _formDataRepository = new FormDataRepository(_context, culture);
            var _settingsRepository = new SettingsRepository(_context, culture);

            var tableCalculator = new TableCalculator(_context, _settingsRepository, _formDataRepository, culture);
            
            var data = tableCalculator.GetDataValue(formData);
            var tableResults = tableCalculator.GenerateAdminMultiTable(data);
            
            TempData["data"] = tableResults;
            TempData["defaultValues"] = formData;

            return Redirect(multiTableUrl);
        }
    }
}