using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels;

namespace Bs.Calendar.Mvc.Controllers
{
    public class UserController : Controller
    {
        private readonly HomeService _service;

        public UserController(HomeService service)
        {
            _service = service;
        }

        public ActionResult Index() {
            var users = _service.LoadUsers();
            return View(new UserViewModel {Users = users});
        }

        [HttpPost]
        public string Add(int userId) {
            return "NotYetImplemented";
        }


        public string Edit(int userId) {
            return "NotYetImplemented";
        }

        public string Delete(int userId) {
            return "NotYetImplemented";
        }
    }
}
