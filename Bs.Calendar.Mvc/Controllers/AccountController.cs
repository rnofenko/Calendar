using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels;

namespace Bs.Calendar.Mvc.Controllers
{
    public class AccountController : Controller
    {
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(AccountVm model, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                var membershipProvider = new CalendarMembershipProvider();
                if (membershipProvider.ValidateUser(model.Email, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.Email, true);
                    if (Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
            }
            return View(model);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Manage()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View(new UserEditVm(new RepoUnit().User.Get(u => u.Email == User.Identity.Name)));
            }
            FormsAuthentication.RedirectToLoginPage();
            return null;
        }
    }
}
