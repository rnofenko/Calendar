using System;
using System.ComponentModel;
using System.Web.Mvc;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.Services.Events;
using Bs.Calendar.Mvc.ViewModels.Events;

namespace Bs.Calendar.Mvc.Controllers
{
    public class EventController : Controller
    {
        private readonly EventService _service;

        public EventController(EventService eventService)
        {
            _service = eventService;
        }

        [HttpGet]
        public ActionResult Create() 
        {
            return View(new CalendarEventVm());
        }



        [HttpPost]
        [ValidateAjax]
        public ActionResult Create(CalendarEventVm calendarEvent)
        {
            try 
            {
                _service.Save(calendarEvent, User.Identity.Name);
            } 
            catch (WarningException exception) 
            {
                ModelState.AddModelError("", exception.Message);
            }

            return Json(new { redirectToUrl = Url.Action("Index", "Home") });
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

        [HttpGet]
        public JsonResult GetRooms(DateTime dateTime)
        {
            return Json(_service.GetRooms(dateTime), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Edit(int id, EventType type)
        {
            throw new NotImplementedException();
        }
    }
}
