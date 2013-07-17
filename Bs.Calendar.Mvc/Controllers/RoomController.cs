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
        private RoomService Service { get; set; }

        public RoomController()
        {
            Service = new RoomService();  /* Инициализация сервиса */
        }

        //
        // GET: /Room/
        public ActionResult Index()
        {
            return View(Service.CreateView());
        }
        
        public ActionResult List()
        {
            return View(Service.List());
        }
    }
}
