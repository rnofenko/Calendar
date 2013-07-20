using System.ComponentModel;
using System.Web.Mvc;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels;

namespace Bs.Calendar.Mvc.Controllers
{
    public class UsersController : Controller
    {
        private readonly UserService _service;

        public UsersController(UserService service)
        {
            _service = service;
        }

        public ActionResult Index()
        {
            var users = _service.GetAllUsers();
            return View(new UsersVm { Users = users });
        }

        public ActionResult Details(int id)
        {
            var user = _service.GetUser(id);
            return user != null ? (ActionResult)View(new UserEditVm(user)) : HttpNotFound();
        }

        public ActionResult Create()
        {
            return View("Edit", null);
        }

        [HttpPost,
        ValidateAntiForgeryToken]
        public ActionResult Create(UserEditVm model)
        {
            try
            {
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
            var user = _service.GetUser(id);
            return user != null ? (ActionResult)View(new UserEditVm(user)) : HttpNotFound();
        }

        [HttpPost]
        public ActionResult Edit(UserEditVm model)
        {
            try
            {
                _service.EditUser(model);
                return RedirectToAction("Index");
            }
            catch (WarningException exception)
            {
                ModelState.AddModelError("", exception.Message);
                return View(model);
            }
        }

        public ActionResult Delete(int id)
        {
            try
            {
                var user = _service.GetUser(id);
                return View(new UserEditVm(user));
            }
            catch (WarningException exception)
            {
                ModelState.AddModelError("UserNotFound", exception.Message);
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public ActionResult Delete(UserEditVm model)
        {
            try
            {
                _service.DeleteUser(model.UserId);
                return RedirectToAction("Index");
            }
            catch
            {
                var user = _service.GetUser(model.UserId);
                return View(new UserEditVm(user));
            }
        }

        [HttpPost]
        public ActionResult Find(string searchStr)
        {
            return PartialView("UserList", _service.Find(searchStr));
        }
    }
}
