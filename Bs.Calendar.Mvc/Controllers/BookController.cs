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

        public JsonResult Get(int id)
        {
            var book = _service.Get(id);
            var response = new JsonResult();
            response.Data = book;
            response.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return response;
        }

        public JsonResult List()
        {
            var books = _service.Load();
            var response = new JsonResult();
            response.Data = books;
            response.JsonRequestBehavior = JsonRequestBehavior.AllowGet;
            return response;
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
