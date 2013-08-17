using System.ComponentModel;
using System.Web.Mvc;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels.Teams;

namespace Bs.Calendar.Mvc.Controllers
{
    [Authorize(Roles = "Admin, Simple")]
    public class TeamController : Controller
    {
        private readonly TeamService _service;

        public TeamController(TeamService service)
        {
            _service = service;
        }

        private ActionResult PassUserIntoTheView(string view, int id)
        {
            var team = _service.GetTeam(id);
            return team != null ? (ActionResult)View(view, new TeamEditVm(team)) : HttpNotFound();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create() 
        {
            return View("Edit", new TeamEditVm());
        }

        [HttpPost]
        [ValidateAjax]
        public ActionResult Create(TeamEditVm model)
        {
            if (!ModelState.IsValid) return View("Edit", model);            
            try 
            {
                _service.CreateTeam(model);
            } 
            catch (WarningException exception) 
            {
                ModelState.AddModelError("", exception.Message);
            }
            return Json(new { redirectToUrl = Url.Action("Index") });
        }

        public ActionResult Edit(int id) 
        {
            return PassUserIntoTheView("Edit", id);
        }

        [HttpPost]
        [ValidateAjax]
        public ActionResult Edit(TeamEditVm model) 
        {
            if (!ModelState.IsValid) return View(model);
            try
            {
                _service.EditTeam(model);
            } 
            catch (WarningException exception) 
            {
                ModelState.AddModelError("", exception.Message);
            }
            return Json(new { redirectToUrl = Url.Action("Index") });
        }

        public ActionResult Delete(int id)
        {
            return PassUserIntoTheView("Delete", id);
        }

        [HttpPost]
        public ActionResult Delete(TeamEditVm model)
        {
            _service.DeleteTeam(model.TeamId);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult List(TeamFilterVm filter)
        {
            return PartialView(_service.RetreiveList(filter));
        }

        [HttpGet]
        public JsonResult GetAllUsers(int teamId)
        {
            return Json(_service.GetAllUsers(teamId), JsonRequestBehavior.AllowGet);
        }
    }
}
