using System.Web.Mvc;
using Bs.Calendar.Mvc.Services;

namespace Bs.Calendar.Mvc.Controllers
{
    public class TeamController : Controller
    {
        //
        // GET: /Team/

        private readonly TeamService _service;

        public TeamController(TeamService service)
        {
            _service = service;
        }

        public ActionResult Index()
        {
            var teams = _service.LoadTeams();
            return View(teams);
        }

    }
}
