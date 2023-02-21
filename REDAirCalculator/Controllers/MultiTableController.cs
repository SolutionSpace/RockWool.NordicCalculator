using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using Umbraco.Web.PublishedModels;
using Umbraco.Web.Security;

namespace REDAirCalculator.Controllers
{
    public class MultiTableController : RenderMvcController
    {
        public ActionResult MultiTable(MultiTable model)
        {
            var auth = new HttpContextWrapper(System.Web.HttpContext.Current).GetUmbracoAuthTicket();
            return auth != null ? CurrentTemplate(model) : Redirect("/?ReturnUrl=" + HttpUtility.UrlEncode(model.Url));
        }
    }
}