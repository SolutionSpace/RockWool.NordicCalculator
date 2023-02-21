using System;
using System.Web.Mvc;
using System.Web.Security;
using REDAirCalculator.DAL;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Models;
using Umbraco.Web.Mvc;

namespace REDAirCalculator.Controllers
{
    public class RegisterController : SurfaceController
    {
        private readonly IProjectRepository _projectRepository;

        public RegisterController(
          IUmbracoContextAccessor umbracoContextAccessor,
          IUmbracoDatabaseFactory databaseFactory,
          ServiceContext services,
          AppCaches appCaches,
          ILogger logger,
          IProfilingLogger profilingLogger,
          UmbracoHelper umbracoHelper,
          IProjectRepository projectRepository)
          : base(umbracoContextAccessor, databaseFactory, services, appCaches, logger, profilingLogger, umbracoHelper)
        {
            _projectRepository = projectRepository;
        }

        [HttpPost]
        public ActionResult HandleRegisterMember(
            [Bind(Prefix = "registerModel")] RegisterModel model,
            string entryGuid,
            string formGuid)
        {
            if (!string.IsNullOrEmpty(entryGuid))
            {
                TempData["Forms_Current_Record_id"] = entryGuid;
            }

            if (!this.ModelState.IsValid)
                return (ActionResult)this.CurrentUmbracoPage();
            if (string.IsNullOrEmpty(model.Name) && !string.IsNullOrEmpty(model.Email))
                model.Name = model.Email;
            MembershipCreateStatus status;
            Members.RegisterMember(model, out status, model.LoginOnSuccess);

            // validation
            string invalidUserName = Umbraco.GetDictionaryValue("Registration valid name");
            string invalidPassword = Umbraco.GetDictionaryValue("Registration password strength");
            string invalidEmail = Umbraco.GetDictionaryValue("Registration valid email");
            string duplicateUserName = Umbraco.GetDictionaryValue("Registration member name exist");
            string duplicateEmail = Umbraco.GetDictionaryValue("Registration member email exist");
            string generalError = Umbraco.GetDictionaryValue("Registration general error");

            switch (status)
            {
                case MembershipCreateStatus.Success:
                    this.TempData["FormSuccess"] = (object)true;

                    // save project for not logged members
                    if (!string.IsNullOrEmpty(entryGuid))
                    {
                        var member = Members.GetByEmail(model.Email);
                        _projectRepository.Save(entryGuid, formGuid, member.Id);
                    }

                    if (!model.RedirectUrl.IsNullOrWhiteSpace())
                        return (ActionResult)this.Redirect(model.RedirectUrl);
                    return (ActionResult)this.RedirectToCurrentUmbracoPage();
                case MembershipCreateStatus.InvalidUserName:
                    this.ModelState.AddModelError(model.UsernameIsEmail || model.Username == null ? "registerModel.Email" : "registerModel.Username", invalidUserName);
                    break;
                case MembershipCreateStatus.InvalidPassword:
                    this.ModelState.AddModelError("registerModel.Password", invalidPassword);
                    break;
                case MembershipCreateStatus.InvalidQuestion:
                case MembershipCreateStatus.InvalidAnswer:
                    throw new NotImplementedException(status.ToString());
                case MembershipCreateStatus.InvalidEmail:
                    this.ModelState.AddModelError("registerModel.Email", invalidEmail);
                    break;
                case MembershipCreateStatus.DuplicateUserName:
                    this.ModelState.AddModelError(model.UsernameIsEmail || model.Username == null ? "registerModel.Email" : "registerModel.Username", duplicateUserName);
                    break;
                case MembershipCreateStatus.DuplicateEmail:
                    this.ModelState.AddModelError("registerModel.Email", duplicateEmail);
                    break;
                case MembershipCreateStatus.UserRejected:
                case MembershipCreateStatus.InvalidProviderUserKey:
                case MembershipCreateStatus.DuplicateProviderUserKey:
                case MembershipCreateStatus.ProviderError:
                    this.ModelState.AddModelError("registerModel", generalError + " " + (object)status);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if(!this.ModelState.IsValid)
            {
                TempData["FailedRegisterValidation"] = true;
            }

            return CurrentUmbracoPage();
        }
    }
}