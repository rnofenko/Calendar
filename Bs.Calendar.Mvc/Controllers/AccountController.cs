using System.Web.Mvc;
using System.Web.Security;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels;

namespace Bs.Calendar.Mvc.Controllers
{
    public class AccountController : Controller
    {
        private readonly RepoUnit _unit;

        public AccountController(RepoUnit unit)
        {
            _unit = unit;
        }

        public ActionResult Login()
        {
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
                    FormsAuthentication.SetAuthCookie(model.Email, true);
                    if (Url.IsLocalUrl(returnUrl))
                    {
                        return Redirect(returnUrl);
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
                var userEmail = User.Identity.Name;
                return View(new UserEditVm(_unit.User.Get(u=>u.Email == userEmail)));
            }
            FormsAuthentication.RedirectToLoginPage();
            return null;
        }
    }
}
