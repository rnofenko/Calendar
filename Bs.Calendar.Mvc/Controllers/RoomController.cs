﻿using System;
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

        /// <summary>
        /// Action is used both to create and to edit room records
        /// </summary>
        [HttpPost,
        ValidateAntiForgeryToken]
        public ActionResult Edit(RoomEditVm room)
        {
            if (ModelState.IsValid && _service.IsValid(room))
            {
                _service.Save(room);
                return RedirectToAction("Edit", room.Id);
            }

            return View("EditRoom", room);
        }

        public ActionResult Create()
        {
            var room = _service.CreateViewModel(new RoomEditVm.RoomEditVmExtra()
            {
                ViewTitle = "Create room",
                CallAction = "Edit",
                CallController = "Room"
            });

            return View("EditRoom", room);
        }

        public ActionResult Edit(int id)
        {
            var room = _service.Load(id);
            room.Extra = new RoomEditVm.RoomEditVmExtra()
             {
                 ViewTitle = "Edit room",
                 CallAction = "Edit",
                 CallController = "Room"
             };

            return View("EditRoom", room);
        }

        //
        // GET: /Room/Save
        public ActionResult Save(RoomEditVm roomViewModel)
        {
            _service.Save(roomViewModel);

            return View("Index");
        }
    }
}
