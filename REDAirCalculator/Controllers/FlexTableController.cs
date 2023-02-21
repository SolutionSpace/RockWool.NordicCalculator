using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using Umbraco.Web.Mvc;
using Umbraco.Web.PublishedModels;
using Umbraco.Web.Security;

namespace REDAirCalculator.Controllers
{
    public class FlexTableController : RenderMvcController
    {
        public ActionResult FlexTable(FlexTable model)
        {
            bool isDev = Convert.ToBoolean(ConfigurationManager.AppSettings["isDev"]);

            var auth = new HttpContextWrapper(System.Web.HttpContext.Current).GetUmbracoAuthTicket();

            if (auth != null)
            {
                return CurrentTemplate(model);
            }
            else
            {
                string langRedirectUrl = "/?ReturnUrl=" + HttpUtility.UrlEncode(model.Url);

                Home home = (Home)model.Parent;

                // if site is on local iis or is not dev environment
                if (isDev && !Request.Url.Host.Contains("local."))
                {
                    langRedirectUrl = $"{home.Url}{langRedirectUrl}";
                }

                return Redirect(langRedirectUrl);
            }
        }
    }
}