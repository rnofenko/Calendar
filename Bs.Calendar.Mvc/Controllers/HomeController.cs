using System;
using System.ComponentModel;
using System.Web.Mvc;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels.Events;
using Bs.Calendar.Mvc.ViewModels.Home;

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
        [ValidateAjax]
        public ActionResult Edit(int id, EventType type)
        {
            var calendarEvent = _service.GetEvent(id, type);
            return calendarEvent != null ? (ActionResult)RedirectToAction("Edit", "Event", new {id = id, type = calendarEvent.EventType }) : HttpNotFound();
        }

        [HttpPost]
        [ValidateAjax]
        public ActionResult Edit(CalendarCellEventVm calendarEvent)
        {
            if(calendarEvent.Id == 0)
            {
                try
                {
                    _service.Save(calendarEvent, User.Identity.Name);
                    //Присобачить в репозиторий событие OnSave, получить сохраннную запись и вернуть отсюда Id-шник
                }
                catch (WarningException exception)
                {
                    ModelState.AddModelError("", exception.Message);
                }

                return Json(new { Id = "" });
            }
            else
            {
                return Json(new { redirectToUrl = Url.Action("Index", "Home") });
            }
        }

        public ActionResult List(EventFilterVm filter)
        {
            return Json(_service.RetreiveList(filter), JsonRequestBehavior.AllowGet);
        }
    }
}
