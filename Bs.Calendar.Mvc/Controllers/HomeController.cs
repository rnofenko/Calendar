using System;
using System.ComponentModel;
using System.Web.Mvc;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels.Events;
using Bs.Calendar.Mvc.ViewModels.Home;
using Bs.Calendar.Rules;

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
            return View();
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            var calendarEvent = _service.GetEvent(id);
            if(calendarEvent == null)
            {
                Response.StatusCode = 404;
            }

            return Json(new {redirectToUrl = Url.Action("Edit", "Event", new {id = id})}, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult Delete(CalendarCellEventVm calendarEvent)
        {
            try
            {
                _service.Delete(calendarEvent);
            }
            catch (WarningException exception)
            {
                ModelState.AddModelError("", exception.Message);
            }

            return Json(new {});
        }

        [HttpPost]
        public ActionResult Create(CalendarCellEventVm calendarEvent)
        {
            int id = 0;

            try
            {
                id = _service.Save(calendarEvent, User.Identity.Name);
            }
            catch (WarningException exception)
            {
                ModelState.AddModelError("", exception.Message);
            }

            return Json(new { id = id});
        }

        public ActionResult List(EventFilterVm filter)
        {
            return Json(_service.RetreiveList(filter, User.Identity.Name), JsonRequestBehavior.AllowGet);
        }
    }
}
