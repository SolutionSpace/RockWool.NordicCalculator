using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using REDAirCalculator.Utilities;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Web;
using Umbraco.Web.Mvc;
using Umbraco.Web.PublishedModels;
using Umbraco.Web.Security;

namespace REDAirCalculator.Controllers
{
    [AuthorizeUser]
    public class HomeController : RenderMvcController
    {
        public ActionResult Home(Home model, string returnUrl = null)
        {
            if (string.IsNullOrEmpty(returnUrl)) return CurrentTemplate(model);

            IEnumerable<IPublishedContent> rootNodes = model.Children();
            FlexTable flexTable = (FlexTable)rootNodes.FirstOrDefault(d => d.ContentType.Alias == "flexTable");
            MultiTable multiTable = (MultiTable)rootNodes.FirstOrDefault(d => d.ContentType.Alias == "multiTable");

            Projects projects = (Projects)rootNodes.FirstOrDefault(d => d.ContentType.Alias == "projects");

            if (returnUrl != flexTable.Url && returnUrl != multiTable.Url && !returnUrl.Contains(projects.Url)) return CurrentTemplate(model);

            // projects authentication
            if (returnUrl.Contains(projects.Url))
            {
                IPrincipal user = HttpContext.User;

                if (user.Identity.IsAuthenticated)
                {
                    return Redirect(returnUrl);
                }
            }

            // flex/multi tables authentication
            var auth = new HttpContextWrapper(System.Web.HttpContext.Current).GetUmbracoAuthTicket();
            return auth != null ? Redirect(returnUrl) : CurrentTemplate(model);
        }
    }
}