using System.ComponentModel;
using System.Web.Mvc;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels;
using Bs.Calendar.Mvc.ViewModels.Users;
using Bs.Calendar.Rules;

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

        private ActionResult passUserIntoTheView(string view, int id)
        {
            var user = _service.GetUser(id);
            return user != null ? (ActionResult)View(view, new UserEditVm(user)) : HttpNotFound();
        }

        public ActionResult Index()
        {
            var filter = new UserFilterVm();
            return View(filter);
        }

        public ActionResult Create()
        {
            return View("Edit", new UserEditVm());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Create(UserEditVm model)
        {
            ModelState.Remove("userId");
            if (!ModelState.IsValid)
            {                
                return View("Edit", model);
            }

            try
            {
                if (_service.CreateUser(model))
                {
                    return RedirectToAction("Index");
                }
                return View("Details", model);
            }
            catch (WarningException exception)
            {
                ModelState.AddModelError("", exception.Message);
                return View("Edit", model);
            }
        }

        public ActionResult RecoverUser(UserEditVm model)
        {
            _service.RecoverUser(model.Email);
            return RedirectToAction("Index");
        }

        public ActionResult ApproveUser(UserEditVm model)
        {
            _service.UpdateUserState(model.UserId, ApproveStates.Approved, LiveStatuses.Active);
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id)
        {
            return passUserIntoTheView("Edit", id);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Edit(UserEditVm model, bool deleted)
        {
           try
            {
                _service.EditUser(model, deleted);
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
            return passUserIntoTheView("Delete", id);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Delete(UserEditVm model)
        {
            _service.UpdateUserState(model.UserId, model.ApproveState, LiveStatuses.Deleted);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult List(PagingVm pagingVm)
        {
            var usersVm = _service.RetreiveList(pagingVm);
            Session["pagingVm"] = usersVm.PagingVm;

            return PartialView(usersVm);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0)]
        public ActionResult GetContactType(string contact)
        {
            return Json(ContactTypeParser.GetContactType(contact), JsonRequestBehavior.AllowGet);
        }
    }
}
