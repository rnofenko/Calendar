using System.ComponentModel;
using System.Web.Mvc;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels;

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
            return View("Edit", null);
        }

        [HttpPost,
        ValidateAntiForgeryToken]
        public ActionResult Create(TeamEditVm model)
        {
            ModelState.Remove("TeamId");
            if (!ModelState.IsValid)
                return View("Edit", model);

            try 
            {
                _service.SaveTeam(model);
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
        public ActionResult Edit(TeamEditVm model) 
        {
            ModelState.Remove("TeamId");
            if (!ModelState.IsValid)
                return View("Edit", model);

            try
            {
                _service.EditTeam(model);
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
        public ActionResult Delete(TeamEditVm model)
        {
            _service.DeleteTeam(model.TeamId);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult List(PagingVm pagingVm) 
        {
            return PartialView(_service.RetreiveList(pagingVm));
        }
    }
}
