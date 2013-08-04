using System;
using System.ComponentModel;
using System.Web.Mvc;
using System.Web.Security;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels;

namespace Bs.Calendar.Mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly AccountService _service;

        public AccountController(AccountService service)
        {
            _service = service;
        }

        public ActionResult Login(bool recover = false)
        {
            ViewBag.Recover = recover;
            return View();
        }

        [HttpPost]
        public ActionResult Login(AccountVm model, string returnUrl)
        {
            if (model.Email == "Admin")
            {
                model.Email = "admin@gmail.com";
            }
            if (ModelState.IsValid)
            {
                if(_service.LoginUser(model))
                {                    
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
        public ActionResult Register(RegisterVm account, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                MembershipCreateStatus status;
                var register = _service.RegisterUser(account, out status);
                if (register == true)
                {
                    return RedirectToAction("Index", "Home");
                }
                if (register == false)
                {
                    return RedirectToAction("Login", new { recover = true });
                }
                ModelState.AddModelError("", ErrorCodeToString(status));
            }
            return View(account);
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

        public ActionResult Edit()
        {
            var email = User.Identity.Name;

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Index", "Home");
            }

            return View(_service.GetUserEditVm(email));
        }

        [HttpPost,
        ValidateAntiForgeryToken]
        public ActionResult Edit(UserEditVm userEditVm)
        {
            ModelState.Remove("userId");
            if (!ModelState.IsValid)
            {
                return View(userEditVm);
            }

            try 
            {
                _service.EditUser(userEditVm);
                FormsAuthentication.SetAuthCookie(userEditVm.Email, true);
                return RedirectToAction("Index", "Home");
            } 
            catch (WarningException exception) 
            {
                ModelState.AddModelError("", exception.Message);
                return View(userEditVm);
            }
        }

        public ActionResult PasswordRecovery()
        {
            return View(); 
        }

        [HttpPost]
        public ActionResult PasswordRecovery(AccountVm model)
        {
            try
            {
                _service.PasswordRecovery(model.Email, Request.Url.AbsoluteUri);
            }
            catch (WarningException exception)
            {
                ModelState.Remove("Password");
                ModelState.AddModelError("", exception.Message);
                return View();
            }

            return View(model);
        }

        public ActionResult PasswordReset(int id, string token) 
        {
            try 
            {
                return View(_service.CheckToken(id, token));
            } 
            catch (WarningException exception) 
            {
                ModelState.AddModelError("", exception.Message);
                return View("PasswordRecovery");
            }
        }

        [HttpPost]
        public ActionResult PasswordReset(AccountVm model)
        {
            try
            {
                _service.ResetPassword(model);
                _service.LoginUser(model);
            }
            catch (WarningException exception)
            {
                ModelState.Remove("Password");
                ModelState.AddModelError("", exception.Message);
                return View("PasswordRecovery");
            }
            
            return RedirectToAction("Index", "Home");
        }

    }
}
