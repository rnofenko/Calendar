using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;
using Bs.Calendar.DataAccess.Bases;
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
        public ActionResult List()
        {
            var rooms = _service.List();

            return View("List", rooms);
        }

        //
        // GET: /Room/
        public ActionResult Index()
        {
            return List();
        }

        public ActionResult AddRoom()
        {
            var room = new RoomEditVm();
            return View("Room", room);
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
