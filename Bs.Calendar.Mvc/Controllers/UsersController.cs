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
            return View(users);
        }

        public ActionResult Details(int id)
        {
            var user = _service.GetUser(id);
            return View(user);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost,
        ValidateAntiForgeryToken]
        public ActionResult Create(UserEditVm model)
        {
            try
            {
                if (_service.IsValidEmailAddress(model.Email))
                {
                    _service.SaveUser(model.FirstName, model.LastName, model.Email, model.Role);
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            catch
            {
                return View(model);
            }
        }

        
        public ActionResult Edit(int id)
        {
            var user = _service.GetUser(id);
            return user != null
                       ? (ActionResult)
                         View(new UserEditVm(user.Id, user.FirstName, user.LastName, user.Email, user.Role))
                       : HttpNotFound();
        }

        [HttpPost]
        public ActionResult Edit(UserEditVm model)
        {
            try
            {
                if (_service.IsValidEmailAddress(model.Email))
                {
                    _service.EditUser(model.FirstName, model.LastName,model.Email,model.Role, model.UserId);
                    return RedirectToAction("Index");
                }
                return View(model);
            }
            catch
            {
                return View(model);
            }
        }

        public ActionResult Delete(int id)
        {
            var user = _service.GetUser(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(new UserEditVm(user.Id, user.FirstName, user.LastName, user.Email, user.Role));
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
                return View(new UserEditVm(user.Id, user.FirstName, user.LastName, user.Email, user.Role));
            }
        }
    }
}