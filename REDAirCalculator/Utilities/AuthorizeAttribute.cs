using System;
using System.Configuration;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace REDAirCalculator.Utilities
{
    public class AuthorizeUserAttribute : AuthorizeAttribute
    {
        // sync user authorize with auth0
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            //IPrincipal user = filterContext.HttpContext.User;

            //bool isDev = Convert.ToBoolean(ConfigurationManager.AppSettings["isDev"]);

            //if (isDev) return;

            //if (user.Identity.IsAuthenticated) return;

            //HttpCookie auth0Cookie = filterContext.RequestContext.HttpContext.Request.Cookies["Auth0Cookie"];

            //if (auth0Cookie == null || auth0Cookie.Value != "0")
            //{
            //    filterContext.Result = new RedirectToRouteResult(
            //        new RouteValueDictionary(new { controller = "Auth0", action = "Authenticate" })
            //    );
            //}
        }
    }
}