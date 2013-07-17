using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;
using Bs.Calendar.Mvc.Services;

using Bs.Calendar.Mvc.ViewModels;

namespace Bs.Calendar.Mvc.Controllers
{
    public class RoomController : Controller
    {
        private RoomService _rsService;

        //
        // GET: /Room/
        public ActionResult Index()
        {
            _rsService = new RoomService();  /* Инициализация сервиса */

            return View(_rsService.Room);
        }

        //
        // GET: /Room/Save
        public ActionResult Save(RoomEditVm revView)
        {
            _rsService.Save(revView);

            return View("Index");
        }

    }
}
