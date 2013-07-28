using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.Security;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels;
using Bs.Calendar.Rules;
using Roles = Bs.Calendar.Models.Roles;

namespace Bs.Calendar.Mvc.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login(bool recover = false)
        {
            ViewBag.Recover = recover;
            return View();
        }

        [HttpPost]
        public ActionResult Login(AccountVm model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var membershipProvider = new CalendarMembershipProvider();
                if (membershipProvider.ValidateUser(model.Email, model.Password))
                {
                    using (var unit = new RepoUnit())
                    {
                        var user = unit.User.Get(u => u.Email == model.Email);
                        user.LiveState = user.LiveState == LiveState.Deleted ? LiveState.NotApproved : user.LiveState;
                        unit.User.Save(user);
                    }
                    FormsAuthentication.SetAuthCookie(model.Email, true);                    
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "The user name or password provided is incorrect.");
            }
            return View(model);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(AccountVm account, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                var calendarMembershipProvider = new CalendarMembershipProvider();
                MembershipCreateStatus status;
                calendarMembershipProvider.CreateUser("", account.Password, account.Email, "", "", true, null,
                    out status);
                if (status == MembershipCreateStatus.Success)
                {
                    SendMsgToAdmins(account.Email);
                    FormsAuthentication.SetAuthCookie(account.Email, false);
                    return RedirectToAction("Index", "Home");
                }
                if (status == MembershipCreateStatus.DuplicateEmail)
                {
                    using (var unit = new RepoUnit())
                    {
                        var user = unit.User.Get(u => u.Email == account.Email);
                        if (user.LiveState == LiveState.Deleted)
                        {
                            return RedirectToAction("Login", new { recover = true });
                        }
                    }
                }
                ModelState.AddModelError("", ErrorCodeToString(status));
            }
            return View(account);
        }

        private static void SendMsgToAdmins(string emailAddress)
        {
            var sender = new EmailSender();
            const string subject = "New user registration";
            var body = string.Format("Hi there!\nA new user with email {0} has been added to the calendar!",
                emailAddress);
            using (var unit = new RepoUnit())
            {
                foreach (var admin in unit.User.Load(u => u.Role == Roles.Admin).ToList())
                {
                    var msg = new MailMessage(emailAddress, admin.Email)
                    {
                        Subject = subject,
                        Body = body
                    };
                    sender.SendEmail(msg);
                }
            }
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
    }
}
