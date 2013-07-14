using System.Web.Mvc;
using Bs.Calendar.Mvc.Services;

namespace Bs.Calendar.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly HomeService _service;

        public HomeController(HomeService service)
        {
            _service = service;
        }

        public ActionResult Index()
        {
            var users = _service.LoadUsers();
            return View(users);
        }
    }
}
