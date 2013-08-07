using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels;

namespace Bs.Calendar.Mvc.Controllers
{
    public class BookController : Controller
    {
        private readonly BookService _service;

        public BookController(BookService service)
        {
            _service = service;
        }

        public ActionResult Get(int id)
        {
            var book = _service.Get(id);
            if (book != null)
            {
                return Json(book, JsonRequestBehavior.AllowGet);
            }
            return HttpNotFound();
        }

        public JsonResult List()
        {
            var orderby = Request["orderby"];
            var searchStr = Request["search"];
            var books = _service.Load(orderby, searchStr);
            return Json(books, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View("Edit", null);
        }

        public ActionResult Edit(int id)
        {
            var book = _service.Get(id);
            return book != null ? (ActionResult) View("Edit", new BookEditVm(book)) : HttpNotFound();
        }

        public ActionResult Delete(int id)
        {
            try
            {
                _service.Delete(id);
            }
            catch (ArgumentException)
            {
                return HttpNotFound();
            }

            return RedirectToAction("Index");
        }

        [HttpPost,
         ValidateAntiForgeryToken]
        public ActionResult Create(BookEditVm book)
        {
            ModelState.Remove("BookId");
            if (ModelState.IsValid && _service.IsValid(book))
            {
                _service.Save(book);
                return RedirectToAction("Index");
            }

            return View("Edit", book);
        }

        [HttpPost,
         ValidateAntiForgeryToken]
        public ActionResult Edit(BookEditVm book)
        {
            if (ModelState.IsValid && _service.IsValid(book))
            {
                _service.Save(book);
                return RedirectToAction("Index");
            }

            return View("Edit", book);
        }
    }
}
