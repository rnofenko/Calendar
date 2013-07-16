using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;
using Bs.Calendar.Mvc.Services;

namespace Bs.Calendar.Mvc.Controllers
{
    public class RoomController : Controller
    {
        //
        // GET: /Room/
        public ActionResult Index()
        {
            RoomService rsService = new RoomService();  /* Инициализация сервиса */

            return View(rsService.CreateView());
        }

    }
}
