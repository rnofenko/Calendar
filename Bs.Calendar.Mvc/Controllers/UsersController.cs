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
                return View("Edit", model);

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
            return PassUserIntoTheView("Edit", id);
        }

        [HttpPost]
        public ActionResult Edit(UserEditVm model)
        {
            ModelState.Remove("userId");
            if (!ModelState.IsValid)
                return View("Edit", model);

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
            return PassUserIntoTheView("Delete", id);
        }

        [HttpPost]
        public ActionResult Delete(UserEditVm model)
        {
            _service.DeleteUser(model.UserId);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult List(string searchStr, string sortByStr, int page = 1)
        {
            return PartialView(_service.RetreiveList(searchStr, sortByStr, page));
        }
    }
}
