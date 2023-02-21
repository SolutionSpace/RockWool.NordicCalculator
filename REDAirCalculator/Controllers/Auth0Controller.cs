using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Builders;
using Auth0.AuthenticationApi.Models;
using Newtonsoft.Json.Linq;
using REDAirCalculator.DAL;
using Serilog;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web.Mvc;
using UrlHelper = REDAirCalculator.Utilities.UrlHelper;
using REDAirCalculator.Utilities;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Web.Models;
using Umbraco.Web;
using Umbraco.Web.PublishedModels;

namespace REDAirCalculator.Controllers
{
    public class Auth0Controller : SurfaceController
    {
        private readonly string _auth0Domain = ConfigurationManager.AppSettings["auth0:Domain"];
        private readonly string _auth0RedirectUri = ConfigurationManager.AppSettings["auth0:RedirectUri"];
        private readonly IProjectRepository _projectRepository;
        private readonly bool _isDev = Convert.ToBoolean(ConfigurationManager.AppSettings["isDev"]);

        public Auth0Controller(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        public ActionResult Authenticate(int projectsType = 0, string formGuId = null, string entryGuId = null, bool forceAuth0 = false)
        {
            string redirectUri = UrlHelper.GetRedirectUrl(Request.Url.ToString(), _auth0RedirectUri);

            string auth0ClientId = ConfigurationManager.AppSettings["auth0:ClientId"];

            string culture = UrlHelper.GetUrlCulture(Request);

            AuthenticationApiClient client = new AuthenticationApiClient(new Uri($"https://{_auth0Domain}"));

            AuthorizationUrlBuilder urlBuilder = client.BuildAuthorizationUrl()
                .WithResponseType(AuthorizationResponseType.Code)
                .WithClient(auth0ClientId)
                .WithRedirectUrl(redirectUri)
                .WithScope("openid profile email")
                .WithState($"Forms_Current_Record_id:{entryGuId},FormGuid:{formGuId},ProjectsType:{projectsType},Culture:{culture}");

            // used on load event
            if (!forceAuth0)
            {
                urlBuilder = urlBuilder.WithValue("prompt", "none");
            }


            // build /authorize url
            string authorizationUrl = urlBuilder
                .Build()
                .ToString();

            return Redirect(authorizationUrl);
        }

        public async Task<ActionResult> Authorize(
            string code = null, 
            string error = null, 
            string error_description = null,
            string state = null)
        {
            HttpCookie auth0Cookie = Request.Cookies["Auth0Cookie"];
            HttpCookie newAuth0Cookie = new HttpCookie("Auth0Cookie");

            if (!string.IsNullOrEmpty(error))
            {
                Log.Error(error_description);
            }

            // user is not authorized 
            if (code == null)
            {
                if (auth0Cookie == null)
                {
                    newAuth0Cookie.Expires = DateTime.Now.AddHours(1);
                    newAuth0Cookie.Value = "0";
                    Response.Cookies.Set(newAuth0Cookie);
                }
                else
                {
                    auth0Cookie.Value = "0";
                    Response.Cookies.Set(auth0Cookie);
                }

                return Redirect("/");
            }

            string redirectUri = UrlHelper.GetRedirectUrl(Request.Url.ToString(), _auth0RedirectUri);

            AuthenticationApiClient client = new AuthenticationApiClient(new Uri($"https://{_auth0Domain}"));

            // get current access token
            var token = await client.GetTokenAsync(new AuthorizationCodeTokenRequest
            {
                ClientId = ConfigurationManager.AppSettings["auth0:ClientId"],
                ClientSecret = ConfigurationManager.AppSettings["auth0:ClientSecret"],
                Code = code,
                RedirectUri = redirectUri,
            });

            // get the authentication information from identity server
            UserInfo profile = await client.GetUserInfoAsync(token.AccessToken);

            // get additional claims

            string authCompanyClaim = ConfigurationManager.AppSettings["auth0:CompanyClaim"];
            List<KeyValuePair<string, JToken>> additionalClaims = (profile.AdditionalClaims ?? new Dictionary<string, JToken>()).ToList();

            string companyClaim = additionalClaims.Where(x => x.Key == $"{authCompanyClaim}").Select(x => x.Value.ToString()).FirstOrDefault();

            IMemberService memberService = Services.MemberService;
            string auth0Connection = profile.UserId.Split('|')[0];

            string email = !string.IsNullOrEmpty(profile.Email) ? profile.Email : profile.FullName;

            IEnumerable<IMember> members = memberService.GetAllMembers();

            IMember checkedMember = members.FirstOrDefault(m => m.Email == email && m.GetValue<string>("auth0Connection") == auth0Connection);

            if (checkedMember == null)
            {
                IMember member = memberService.CreateMemberWithIdentity(profile.Email, profile.Email, profile.FullName, "member");
                member.SetValue("isAuth0", true);
                member.SetValue("auth0Connection", auth0Connection);
                member.SetValue("company", companyClaim);

                memberService.Save(member);
            }

            FormsAuthentication.SetAuthCookie(email, false);

            if (auth0Cookie == null)
            {
                newAuth0Cookie.Expires = DateTime.Now.AddHours(1);
                newAuth0Cookie.Value = "1";
                Response.Cookies.Set(newAuth0Cookie);
            }
            else
            {
                auth0Cookie.Value = "1";
                Response.Cookies.Set(auth0Cookie);
            }

            // save project for not logged members
            if (!string.IsNullOrEmpty(state))
            {
                state = HttpUtility.UrlDecode(state);
                string forms_Current_Record_id = state.Split(',')[0].Replace("Forms_Current_Record_id:", "");
                string formGuid = state.Split(',')[1].Replace("FormGuid:", "");
                int projectsType = Int32.Parse(state.Split(',')[2].Replace("ProjectsType:", ""));
                string culture = state.Split(',')[3].Replace("Culture:", "");
                if(String.IsNullOrEmpty(culture)) culture = UrlHelper.GetUrlCulture(Request);

                if (!string.IsNullOrEmpty(forms_Current_Record_id) && !string.IsNullOrEmpty(formGuid))
                {
                    var member = Members.GetByEmail(profile.Email);
                    _projectRepository.Save(forms_Current_Record_id, formGuid, member.Id);


                    bool isLocal = Request.IsLocal;


                    IEnumerable<IPublishedContent> rootNodes = UmbracoContext.ContentCache.GetAtRoot().First().Children();
                    IPublishedContent calculatorPage = ContentFinder.GetCurrentCalculatorPage(projectsType, rootNodes);

                    IReadOnlyDictionary<string, PublishedCultureInfo> cultures = calculatorPage.Cultures;

                    PublishedCultureInfo current;
                    try
                    {
                        current = cultures[culture];
                    }
                    catch(Exception)
                    {
                        culture = "en";
                        current = cultures[culture];
                    }

                    // get current language redirect url
                    string calculatorUrl = ((!_isDev || isLocal) && culture != "en") ? $"/{current.UrlSegment}" : $"/{culture}/{current.UrlSegment}";

                    TempData["Forms_Current_Record_id"] = forms_Current_Record_id;
                    TempData["projectsType"] = projectsType;

                    return Redirect(calculatorUrl);
                }
            }


            return Redirect("/");
        }

        // http://local.redair.rockwool.dk/umbraco/surface/auth0/profile?token=ruh8669gFFU7WU7uy7sTBryOKXRcXuDK
        public async Task<ActionResult> Profile(string token)
        {
            AuthenticationApiClient client = new AuthenticationApiClient(new Uri($"https://{_auth0Domain}"));
            UserInfo profile = await client.GetUserInfoAsync(token);

            return Json(profile, JsonRequestBehavior.AllowGet);
        }
    }
}