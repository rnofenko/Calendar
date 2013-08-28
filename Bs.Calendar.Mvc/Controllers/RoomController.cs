using System.Web.Mvc;
using Bs.Calendar.Mvc.Services;

using Bs.Calendar.Mvc.ViewModels;
using System;
using Bs.Calendar.Mvc.ViewModels.Rooms;

namespace Bs.Calendar.Mvc.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RoomController : Controller
    {
        private readonly RoomService _service;

        public RoomController(RoomService service)
        {
            _service = service;
        }

        public ActionResult Index()
        {
            return View(new RoomFilterVm());
        }

        [HttpGet]
        public ActionResult List(RoomFilterVm filter)
        {
            return PartialView(_service.RetreiveList(filter));
        }

        public ActionResult Delete(int id)
        {
            try
            {
                _service.Delete(id);
            }
            catch(ArgumentException)
            {
                return HttpNotFound();
            }

            return RedirectToAction("Index");
        }

        [HttpPost,
         ValidateAntiForgeryToken]
        public ActionResult Create(RoomEditVm room)
        {
            ModelState.Remove("RoomId");
            if (ModelState.IsValid && _service.IsValid(room))
            {
                _service.Save(room);
                return RedirectToAction("Index");
            }

            return View("Edit", room);
        }

        [HttpPost,
         ValidateAntiForgeryToken]
        public ActionResult Edit(RoomEditVm room)
        {
            if (ModelState.IsValid && _service.IsValid(room))
            {
                _service.Save(room);
                return RedirectToAction("Index");
            }

            return View("Edit", room);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            return passRoomIntoTheView("Edit", id);
        }

        [HttpGet]
        public ActionResult Create()
        {
            return View("Edit", null);
        }

        [HttpGet]
        public JsonResult GetAllRooms()
        {
            return Json(_service.GetAllRooms(), JsonRequestBehavior.AllowGet);
        }

        private ActionResult passRoomIntoTheView(string view, int id)
        {
            var room = _service.Get(id);
            return room != null ? (ActionResult)View(view, new RoomEditVm(room)) : HttpNotFound();
        }
    }
}
