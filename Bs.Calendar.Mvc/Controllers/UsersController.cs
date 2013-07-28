using System;
using System.ComponentModel;
using System.Web.Mvc;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels;

namespace Bs.Calendar.Mvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserService _service;

        public UsersController(UserService service)
        {
            _service = service;
        }

        private ActionResult PassUserIntoTheView(string view, int id)
        {
            var user = _service.GetUser(id);
            return user != null ? (ActionResult)View(view, new UserEditVm(user)) : HttpNotFound();
        }

        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult Details(int id)
        {
            return PassUserIntoTheView("Details", id);
        }

        public ActionResult Create()
        {
            return View("Edit", null);
        }

        [HttpPost,
        ValidateAntiForgeryToken]
        public ActionResult Create(UserEditVm model)
        {
            ModelState.Remove("userId");
            if (!ModelState.IsValid)
            {                
                return View("Edit", model);
            }

            try
            {
                model.LiveState = LiveState.Active;
                //model.BirthDate = new DateTime((new Random()).Next(1970, 1992), (new Random()).Next(1, 12), (new Random()).Next(1, 28)).Date;
                _service.SaveUser(model);
                return RedirectToAction("Index");
            }
            catch (WarningException exception)
            {
                ModelState.AddModelError("", exception.Message);
                return View("Edit", model);
            }
        }

        public ActionResult Edit(int id)
        {
            return PassUserIntoTheView("Edit", id);
        }

        [HttpPost]
        public ActionResult Edit(UserEditVm model, bool delete)
        {
            ModelState.Remove("userId");
            if (!ModelState.IsValid)
                return View("Edit", model);

            try
            {
                _service.EditUser(model, delete);
                return delete ? RedirectToAction("Logout", "Account") : RedirectToAction("Index");
            }
            catch (WarningException exception)
            {
                ModelState.AddModelError("", exception.Message);
                return View(model);
            }
        }

        public ActionResult Delete(int id)
        {
            return PassUserIntoTheView("Delete", id);
        }

        [HttpPost]
        public ActionResult Delete(UserEditVm model)
        {
            _service.UpdateUserState(model.UserId, LiveState.Deleted);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult List(PagingVm pagingVm)
        {
            var usersVm = _service.RetreiveList(pagingVm);
            Session["pagingVm"] = usersVm.PagingVm;

            return PartialView(usersVm);
        }
    }
}
