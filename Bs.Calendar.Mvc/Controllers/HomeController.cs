using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels.Home;
using Bs.Calendar.Mvc.ViewModels.Users;
using Bs.Calendar.Rules.Logs;

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

        [HttpGet]
        public ActionResult CreateEvent()
        {
            return View(new CalendarEventVm());
        }

        [HttpPost]
        public ActionResult CreateEvent(CalendarEventVm calendarEvent)
        {
            _service.SaveEvent(calendarEvent, User.Identity.Name);
            return null;
        }

        [HttpGet]
        public JsonResult GetEvents(DateTime from, DateTime to)
        {
            return Json(_service.GetEvents(from, to), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetTeams()
        {
            return Json(_service.GetTeams(), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetAllUsers()
        {
            return Json(_service.GetAllUsers(), JsonRequestBehavior.AllowGet);
        }
    }
}
