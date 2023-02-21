using System;
using System.Web.Mvc;
using System.Web.Security;
using Umbraco.Web.Mvc;
using MailKit.Net.Smtp;
using MimeKit;
using REDAirCalculator.DAL;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Auth0.AuthenticationApi;
using REDAirCalculator.Models.RegistrationModels;
using REDAirCalculator.Utilities;
using Umbraco.Core.Models;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.PublishedModels;
using UrlHelper = System.Web.Mvc.UrlHelper;
using CustomUrlHelper = REDAirCalculator.Utilities.UrlHelper;

namespace REDAirCalculator.Controllers
{
    public class MembersController : SurfaceController
    {
        private readonly bool _isDev = Convert.ToBoolean(ConfigurationManager.AppSettings["isDev"]);

        private readonly IProjectRepository _projectRepository;

        public MembersController(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult RestorePassword(RestorePasswordModel userEmail, string entryGuid)
        {
            string userId;
            try
            {
                userId = Umbraco.MembershipHelper.GetByEmail(userEmail.EmailAddress).Id.ToString();
            }
            catch
            {
                return RedirectToCurrentUmbracoPage();
            }

            string smtpHost = ConfigurationManager.AppSettings["smtpHost"];
            int smtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["smtpPort"]);
            bool smtpUseSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["smtpUseSSL"]);
            string smtpUserName = ConfigurationManager.AppSettings["smtpUserName"];
            string smtpPassword = ConfigurationManager.AppSettings["smtpPassword"];

            // get change password page
            IEnumerable<IPublishedContent> rootNodes = UmbracoContext.ContentCache.GetAtRoot().First().Children();
            ChangePasswordEmail changePasswordMail = (ChangePasswordEmail)rootNodes.FirstOrDefault(d => d.ContentType.Alias == "changePasswordEmail");

            string culture = Utilities.UrlHelper.GetUrlCulture(Request);

            string subject = changePasswordMail.Value("subject", culture, fallback: Fallback.ToDefaultValue).ToString();
            string body = changePasswordMail.Value("body", culture, fallback: Fallback.ToDefaultValue).ToString();

            if (!string.IsNullOrEmpty(entryGuid))
            {
                TempData["Forms_Current_Record_id"] = entryGuid;
            }

            MimeMessage message = new MimeMessage();

            string address = _isDev ? "no-reply@test.com" : "no-reply@redair.rockwool.dk";

            MailboxAddress from = new MailboxAddress("no-reply", address);
            message.From.Add(from);

            MailboxAddress to = new MailboxAddress("User", userEmail.EmailAddress);
            message.To.Add(to);

            message.Subject = subject;

            BodyBuilder bodyBuilder = new BodyBuilder();

            string hashedGuId = userEmail.EmailAddress.Crypt();

            string recoveryLink = Request.Url.ToString()
                .Replace("RenderMvc", $"surface/Members/ForgetPassword?memberGuId=" + hashedGuId + culture);

            // generate body
            bodyBuilder.HtmlBody = body;
            bodyBuilder.HtmlBody = MailTexts.CreateMessageForChangePassword(recoveryLink, body);

            message.Body = bodyBuilder.ToMessageBody();

            SmtpClient client = new SmtpClient();

            client.Connect(smtpHost, smtpPort, smtpUseSSL);

            if (_isDev)
            {
                client.Authenticate(smtpUserName, smtpPassword);
            }

            client.Send(message);
            client.Disconnect(true);
            client.Dispose();

            TempData["Notification"] = "succes:pass-recovery:5000";

            return CurrentUmbracoPage();
        }

        public ActionResult ForgetPassword(string memberGuId)
        {
            bool isLocal = Request.IsLocal;

            TempData["ChangePassword"] = true;
            string culture = memberGuId.Substring(memberGuId.Length - 2);
            TempData["memberGuId"] = Hash.Decrypt(memberGuId.Substring(0, memberGuId.Length - 2));

            string redirectUrl = ((!_isDev || isLocal) && culture != "en") ? "/" : $"/{culture}/";

            return Redirect(redirectUrl);
        }


        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                var member = Services.MemberService.GetByUsername(model.MemberGuId);
                member.IsLockedOut = false;
                Services.MemberService.SavePassword(member, model.NewPassword);
                Services.MemberService.Save(member);
            }
            if (!ModelState.IsValid)
            {
                TempData["FailedValidation"] = true;
            }
            return RedirectToCurrentUmbracoPage();
        }

        public ActionResult RenderLogin()
        {
            return PartialView("_Login", new LoginModel());
        }

        [HttpPost]
        public ActionResult SubmitLogin(
            LoginModel model,
            string returnUrl,
            string entryGuid,
            string formGuid)
        {
            if (!string.IsNullOrEmpty(entryGuid))
            {
                TempData["Forms_Current_Record_id"] = entryGuid;
            }

            if (!ModelState.IsValid) return CurrentUmbracoPage();

            if (Membership.ValidateUser(model.Username, model.Password))
            {
                FormsAuthentication.SetAuthCookie(model.Username, model.RememberMe != null ? true : false);
                UrlHelper myHelper = new UrlHelper(HttpContext.Request.RequestContext);

                // save project for not logged members
                if (!string.IsNullOrEmpty(entryGuid))
                {
                    var member = Members.GetByEmail(model.Username);
                    _projectRepository.Save(entryGuid, formGuid, member.Id);
                }

                if (myHelper.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                else
                {
                    return RedirectToCurrentUmbracoPage();
                }
            }
            else
            {
                TempData["FailedLoginValidation"] = true;
                string incorrectCredentials = Umbraco.GetDictionaryValue("Login incorrect credentials");
                ModelState.AddModelError("", incorrectCredentials);
            }

            return CurrentUmbracoPage();
        }

        public ActionResult RenderLogout()
        {
            return PartialView("_Logout", null);
        }

        [HttpPost]
        public ActionResult SubmitLogout()
        {
            IIdentity user = User.Identity;

            IMemberService memberService = Services.MemberService;
            IMember member = memberService.GetByEmail(user.Name);

            if (member == null)
            {
                return CurrentUmbracoPage();
            }

            TempData.Clear();
            Session.Clear();
            FormsAuthentication.SignOut();

            string isAuth0Property = member.GetValue<string>("isAuth0");
            bool isAuth0 = isAuth0Property != null && isAuth0Property.Equals("1");

            if (!isAuth0) return RedirectToCurrentUmbracoPage();

            string auth0Domain = ConfigurationManager.AppSettings["auth0:Domain"];
            string auth0ClientId = ConfigurationManager.AppSettings["auth0:ClientId"];
            string auth0PostLogoutRedirectUri = CustomUrlHelper.GetDomainUrl(Request.Url.ToString());

            AuthenticationApiClient apiClient = new AuthenticationApiClient(new Uri($"https://{auth0Domain}"));

            var logoutUrl = apiClient.BuildLogoutUrl()
                .WithClientId(auth0ClientId)
                .WithReturnUrl(auth0PostLogoutRedirectUri)
                .Build()
                .ToString();
            
            HttpCookie auth0Cookie = Request.Cookies["Auth0Cookie"];

            if (auth0Cookie == null) return Redirect(logoutUrl);

            // remove auth0 cookie
            Response.Cookies.Remove("auth0Cookie");
            auth0Cookie.Expires = DateTime.Now.AddHours(-10);
            auth0Cookie.Value = null;
            Response.Cookies.Set(auth0Cookie);

            return Redirect(logoutUrl);
        }
    }
}