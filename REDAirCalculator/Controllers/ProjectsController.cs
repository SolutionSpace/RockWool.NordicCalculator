using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;
using PagedList;
using REDAirCalculator.DAL;
using REDAirCalculator.Models.DTO;
using Umbraco.Web.Mvc;
using REDAirCalculator.Utilities;
using Umbraco.Core.Models.PublishedContent;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Web.Models;
using Umbraco.Web;
using Umbraco.Web.PublishedModels;
using System.Configuration;
using System.IO;
using System.Security.Principal;
using System.Web;
using MailKit.Net.Smtp;
using Umbraco.Core.Services;
using MimeKit;

namespace REDAirCalculator.Controllers
{
    [AuthorizeUser]
    public class ProjectsController : RenderMvcController
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectsController(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        private readonly int _projectsPerPage = Convert.ToInt32(ConfigurationManager.AppSettings["projectsPerPage"]);

        [HttpGet]
        public ActionResult Projects(Projects model, int p = 1, int t = 0)
        {
            IPrincipal user = HttpContext.User;

            // projects authentication check
            if (!user.Identity.IsAuthenticated)
            {
                string langRedirectUrl = "/?ReturnUrl=" + HttpUtility.UrlEncode($"{model.Url}?p={p}&t={t}");
                return Redirect(langRedirectUrl);
            }

            IEnumerable<IPublishedContent> rootNodes = UmbracoContext.ContentCache.GetAtRoot().First().Children();

            dynamic calculatorPage = ContentFinder.GetCurrentCalculatorPage(t, rootNodes);

            if (calculatorPage == null) return CurrentTemplate(model);

            string guid = calculatorPage.SelectForm.ToString();
            Form form = FormHelper.GetForm(new Guid(guid));

            string lang = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
            List<ProjectDto> projects = _projectRepository.GetAll(form.Id, lang);

            IPagedList<ProjectDto> pageData = projects.ToPagedList(p, _projectsPerPage);

            int previousPage = p - 1;
            if (pageData.Count == 0 && p > 1 && previousPage == pageData.PageCount)
            {
                return Redirect(Request.Path + "?p=" + (p - 1));
            }

            // loads projects for specific page
            TempData["pageData"] = pageData;
            TempData["projectsType"] = t;

            return CurrentTemplate(model);
        }
    }
    public class ProjectController : SurfaceController
    {
        private readonly bool _isDev = Convert.ToBoolean(ConfigurationManager.AppSettings["isDev"]);

        private readonly IProjectRepository _projectRepository;

        public ProjectController(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        [HttpGet]
        public ActionResult Get(string entryGuid, int t = 0)
        {
            bool isLocal = Request.IsLocal;

            string culture = Utilities.UrlHelper.GetUrlCulture(Request);

            IEnumerable<IPublishedContent> rootNodes = UmbracoContext.ContentCache.GetAtRoot().First().Children();
            IPublishedContent calculatorPage = ContentFinder.GetCurrentCalculatorPage(t, rootNodes);

            IReadOnlyDictionary<string, PublishedCultureInfo> cultures = calculatorPage.Cultures;
            PublishedCultureInfo current = cultures[culture];

            // get current language redirect url
            string calculatorUrl = ((!_isDev || isLocal) && culture != "en") ? $"/{current.UrlSegment}" : $"/{culture}/{current.UrlSegment}";

            TempData["Forms_Current_Record_id"] = entryGuid;
            TempData["projectsType"] = t;

            return Redirect(calculatorUrl);
        }

        [HttpPost]
        public ActionResult Update(FormViewModel model, int t = 0)
        {
            NameValueCollection nameValueCollection = Request.Form;
            Dictionary<string, string> formDataDictionary = nameValueCollection.AllKeys.ToDictionary(key => key, key => nameValueCollection[key]);

            string calculatorUrl = Request.UrlReferrer.AbsolutePath;

            Form form = _projectRepository.Get(model.FormId);
            Record entry = FormHelper.GetEntry(model.RecordId, form);

            string showAllResultsGuid = string.Empty;
            string advancedFieldGuid = string.Empty;
            string precutPlanksGuid = string.Empty;

            IEnumerable<IPublishedContent> rootNodes = UmbracoContext.ContentCache.GetAtRoot().First().Children();
            IPublishedContent calculatorPage = ContentFinder.GetCurrentCalculatorPage(t, rootNodes);

            //Link form
            if (calculatorPage.GetType() == typeof(LinkCalculator))
            {
                foreach (Field field in form.AllFields)
                {
                    if (field.Alias != "precutPlanks") continue;
                    precutPlanksGuid = field.Id.ToString();
                }
            }
            else if (calculatorPage.GetType() == typeof(FlexMultiCalculator))
            {
                foreach (Field field in form.AllFields)
                {
                    if (field.Alias == "showAllResults")
                        showAllResultsGuid = field.Id.ToString();

                    if (field.Alias == "advancedField") 
                        advancedFieldGuid = field.Id.ToString();
                    
                    continue;
                    
                }
            }


            // updates old form values with new ones
            foreach (KeyValuePair<string, string> formPair in formDataDictionary)
            {
                Guid recordFieldGuid = new Guid();
                bool isValidGuid;

                try
                {
                    isValidGuid = Guid.TryParse(formPair.Key, out recordFieldGuid);
                }
                catch (FormatException)
                {
                    isValidGuid = false;
                }

                if (!isValidGuid) continue;

                if (entry.RecordFields.ContainsKey(recordFieldGuid))
                {
                    entry.RecordFields[recordFieldGuid].Values = new List<object>() { formPair.Value };
                }
            }

            // made for getting result if the checkbox (show all results) was unchecked 
            if (!nameValueCollection.AllKeys.Contains(showAllResultsGuid) && !string.IsNullOrEmpty(showAllResultsGuid))
            {
                entry.RecordFields[new Guid(showAllResultsGuid)].Values = new List<object>() { false };
            }

            if (!nameValueCollection.AllKeys.Contains(advancedFieldGuid) && !string.IsNullOrEmpty(advancedFieldGuid))
            {
                entry.RecordFields[new Guid(advancedFieldGuid)].Values = new List<object>() { false };
            }

            if (!nameValueCollection.AllKeys.Contains(precutPlanksGuid) && !string.IsNullOrEmpty(precutPlanksGuid))
            {
                entry.RecordFields[new Guid(precutPlanksGuid)].Values = new List<object>() { false };
            }

            _projectRepository.Update(entry, form);

            TempData["Forms_Current_Record_id"] = model.RecordId;
            return Redirect(calculatorUrl);
        }

        [HttpGet]
        public ActionResult Delete(string entryGuid, int t = 0)
        {
            string projectUrlWithQuery = Request.UrlReferrer.PathAndQuery;

            string culture = Utilities.UrlHelper.GetUrlCulture(Request);

            IEnumerable<IPublishedContent> rootNodes = UmbracoContext.ContentCache.GetAtRoot().First().Children();
            IPublishedContent calculatorPage = ContentFinder.GetCurrentCalculatorPage(t, rootNodes);

            // get current culture form
            string formGuid = calculatorPage.Value("selectForm", culture, fallback: Fallback.ToDefaultValue).ToString();

            Form form = _projectRepository.Get(new Guid(formGuid));
            Record entry = FormHelper.GetEntry(new Guid(entryGuid), form);

            _projectRepository.Delete(entry, form);

            TempData["Project_Delete_Success"] = true;

            return Redirect(projectUrlWithQuery);
        }

        [HttpPost]
        public void Send(Models.RegistrationModels.Email userEmail, string projectName)
        {
            IMemberService memberService = Services.MemberService;
            string smtpHost = ConfigurationManager.AppSettings["smtpHost"];
            int smtpPort = Convert.ToInt32(ConfigurationManager.AppSettings["smtpPort"]);
            bool smtpUseSSL = Convert.ToBoolean(ConfigurationManager.AppSettings["smtpUseSSL"]);
            string smtpUserName = ConfigurationManager.AppSettings["smtpUserName"];
            string smtpPassword = ConfigurationManager.AppSettings["smtpPassword"];

            string memberEmail = Umbraco.MembershipHelper.CurrentUserName;

            IPublishedContent member = Members.GetCurrentMember();
            string userCompany = member.GetProperty("company").GetValue().ToString();
            if (userCompany == null || userCompany == "")
            {
                var memberCompnayChangeModel = memberService.GetByEmail(memberEmail);
                memberCompnayChangeModel.SetValue("company", userEmail.Company);
                memberService.Save(memberCompnayChangeModel);
            }

            // get project send page
            IEnumerable<IPublishedContent> rootNodes = UmbracoContext.ContentCache.GetAtRoot().First().Children();
            Email sendProjectMail = (Email)rootNodes.FirstOrDefault(d => d.ContentType.Alias == "email");

            string culture = Utilities.UrlHelper.GetUrlCulture(Request);

            string subject = sendProjectMail.Value("subject", culture, fallback: Fallback.ToDefaultValue).ToString();
            string body = sendProjectMail.Value("body", culture, fallback: Fallback.ToDefaultValue).ToString();

            MimeMessage message = new MimeMessage();

            string address = _isDev ? "no-reply@test.com" : "no-reply@redair.rockwool.dk";

            MailboxAddress from = new MailboxAddress("no-reply", address);
            message.From.Add(from);

            MailboxAddress to = new MailboxAddress("User",
            userEmail.EmailAddress);
            message.To.Add(to);

            message.Subject = subject.Replace("[ProjectName]", projectName);

            BodyBuilder bodyBuilder = new BodyBuilder();

            

            // generate body
            bodyBuilder.HtmlBody = body;
            bodyBuilder.HtmlBody = MailTexts.CreateMessageForSendProject(memberEmail, userEmail.EmailAddress, userEmail.Company, bodyBuilder.HtmlBody);

            // add attachment
            string file = Path.Combine(Server.MapPath("~/Downloads"), userEmail.FormGuId + ".pdf");
            bodyBuilder.Attachments.Add(file);

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
        }
    }
}