using System.Web.Mvc;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels;

namespace Bs.Calendar.Mvc.Controllers
{
    public class TeamController : Controller
    {
        private readonly TeamService _service;

        public TeamController(TeamService service)
        {
            _service = service;
        }

        public ActionResult Index()
        {
            var teams = _service.GetAllTeams();

            return View(teams);
        }

        [HttpPost]
        public ActionResult Update(TeamEditVm team)
        {
            if (_service.IsValid(team))
            {
                _service.Save(team);
            }

            return RedirectToAction("Index");
        }

        public ActionResult AddPage()
        {
            var team = _service.CreateViewModel();

            team.Extra.ViewTitle = "Add team";
            team.Extra.CallAction = "Update";
            team.Extra.CallController = "Team";

            return View("Edit", team);
        }

        public ActionResult UpdatePage()
        {
            var team = _service.CreateViewModel();

            team.Extra.ViewTitle = "Update team";
            team.Extra.CallAction = "Update";
            team.Extra.CallController = "Team";

            team.Name = "Initial name";

            return View("Edit", team);
        }

        public ActionResult Save(TeamEditVm revView)
        {
            _service.Save(revView);

            return View("Index");
        }

        [HttpDelete]
        public ActionResult Delete(UserEditVm model)
        {
            _service.Delete(model.UserId);
            return View("Index");
        }
    }
}
