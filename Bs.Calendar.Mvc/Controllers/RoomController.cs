using System.Web.Mvc;
using Bs.Calendar.Mvc.Services;

using Bs.Calendar.Mvc.ViewModels;

namespace Bs.Calendar.Mvc.Controllers
{
    public class RoomController : Controller
    {
        private readonly RoomService _service;

        public RoomController(RoomService service)
        {
            _service = service;
        }

        //
        // GET: /Room/
        public ActionResult Index()
        {
            var rooms = _service.GetAllRooms();

            return View(rooms);
        }

        /// <summary>
        /// Method is used both to create and to update room records
        /// </summary>
        [HttpPost]
        public ActionResult Update(RoomEditVm room)
        {
            if (_service.IsValid(room))
            {
                _service.Save(room);
            }

            return RedirectToAction("Index");
        }

        public ActionResult AddPage()
        {
            var room = _service.CreateViewModel();

            room.Extra.ViewTitle = "Add room";
            room.Extra.CallAction = "Update";
            room.Extra.CallController = "Room";

            return View("EditRoom", room);
        }

        public ActionResult UpdatePage()
        {
            var room = _service.CreateViewModel();

            room.Extra.ViewTitle = "Update room";
            room.Extra.CallAction = "Update";
            room.Extra.CallController = "Room";

            room.NumberOfPlaces = 11;
            room.Name = "Initial name";
            room.Color = System.Drawing.Color.Blue;

            return View("EditRoom", room);
        }

        //
        // GET: /Room/Save
        public ActionResult Save(RoomEditVm revView)
        {
            _service.Save(revView);

            return View("Index");
        }
    }
}
