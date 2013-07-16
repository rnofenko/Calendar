using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels;

namespace Bs.Calendar.Mvc.Controllers
{
    public class UsersController : Controller
    {
        private readonly HomeService _service;

        public UsersController(HomeService service)
        {
            _service = service;
        }

        public ActionResult Index() 
        {
            var users = _service.LoadUsers();
            return View(new UsersVm {Users = users});
        }

        public string Add() 
        {
            return "Not Yet Implemented";
        }

        [HttpPost]
        public string Edit(int userId)
        {
            return "Not Yet Implemented";
        }

        [HttpPost]
        public string Delete(int userId) 
        {
            return "Not Yet Implemented";
        }

        [HttpPost]
        public ActionResult Find(string text)
        {
            if (string.IsNullOrEmpty(text))
                return RedirectToAction("Index");

            //Delete extra whitespaces
            text = Regex.Replace(text.Trim(), @"\s+", " ");     
            
            var users = _service.LoadUsers();
            if (text.Contains('@'))
            {
                users = users.Where(user => user.Email.Equals(text, StringComparison.InvariantCulture));
            }
            else
            {
                var arrName = text.Split();
                users = users.Where(
                    user => user.FirstName.Equals(arrName[0], StringComparison.InvariantCulture));

                if (arrName.Length == 2)
                    users = users.Where(
                        user => user.LastName.Equals(arrName[1], StringComparison.InvariantCulture));
            }
            return View("Index", new UsersVm { Users = users.ToList() });
        }
    }
}
