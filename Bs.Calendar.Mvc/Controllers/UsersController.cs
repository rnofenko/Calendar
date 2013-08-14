using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels;
using Bs.Calendar.Mvc.ViewModels.Users;
using Bs.Calendar.Rules;

namespace Bs.Calendar.Mvc.Controllers
{    
    public class UsersController : Controller
    {
        private readonly UserService _service;

        public UsersController(UserService service)
        {
            _service = service;
        }

        public JsonResult GetAllUsers()
        {
            var users = _service.GetAllUsers().ToDictionary(user => user.Id.ToString(), user => user.FullName);
            return Json(users, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetCurrentUserId()
        {
            var user = _service.GetCurrentUser();
            if (user != null)
            {
                return Json(user.Id, JsonRequestBehavior.AllowGet);
            }
            return HttpNotFound();
        }

        public ActionResult GetUserFullName(int id)
        {
            var user = _service.GetUser(id);
            if (user != null)
            {
                return Json(user.FullName, JsonRequestBehavior.AllowGet);
            }
            return HttpNotFound();
        }

        [Authorize(Roles = "Admin")]
        private ActionResult passUserIntoTheView(string view, int id)
        {
            var user = _service.GetUser(id);
            return user != null ? (ActionResult)View(view, new UserEditVm(user)) : HttpNotFound();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var filter = new UserFilterVm();
            return View(filter);
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            return View("Edit", new UserEditVm());
        }

        [HttpPost,
        ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        public ActionResult RecoverUser(UserEditVm model)
        {
            _service.RecoverUser(model.Email);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult ApproveUser(UserEditVm model)
        {
            _service.UpdateUserState(model.UserId, ApproveStates.Approved, LiveStatuses.Active);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            return passUserIntoTheView("Edit", id);
        }

        [HttpPost,
        ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
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

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            return passUserIntoTheView("Delete", id);
        }

        [HttpPost,
        ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(UserEditVm model)
        {
            _service.UpdateUserState(model.UserId, model.ApproveState, LiveStatuses.Deleted);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public ActionResult List(PagingVm pagingVm)
        {
            var usersVm = _service.RetreiveList(pagingVm);
            Session["pagingVm"] = usersVm.PagingVm;

            return PartialView(usersVm);
        }

        [HttpGet]
        [OutputCache(NoStore = true, Duration = 0)]
        [Authorize(Roles = "Admin")]
        public ActionResult GetContactType(string contact) 
        {
            return Json(ContactTypeParser.GetContactType(contact), JsonRequestBehavior.AllowGet);
        }
    }
}
