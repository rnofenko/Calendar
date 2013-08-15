using System;
using System.ComponentModel;
using System.Web.Mvc;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Services;
using Bs.Calendar.Mvc.ViewModels;

namespace Bs.Calendar.Mvc.Controllers
{
    public class BookController : Controller
    {
        private readonly BookService _service;
        private readonly BookHistoryService _bookHistoryService;

        public BookController(BookService service, BookHistoryService bookHistoryService)
        {
            _service = service;
            _bookHistoryService = bookHistoryService;
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
            var page = Request["page"];
            var pageNumber = 0;
            try
            {
                pageNumber = Convert.ToInt32(page);
            }
            catch
            {
                pageNumber = 0;
            }
            if (pageNumber < 1)
            {
                return Json(books, JsonRequestBehavior.AllowGet);
            }
            var pager = new GenericPagingVm<Book>(books, pageNumber);
            return Json(pager, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View("Edit", null);
        }

        [HttpPost,
         ValidateAntiForgeryToken]
        public ActionResult Create(BookHistoryVm book)
        {
            ModelState.Remove("BookId");
            if (ModelState.IsValid)
            {
                _service.Save(book);
                return RedirectToAction("Index");
            }

            return View("Edit", new BookHistoryVm());
        }

        public ActionResult Edit(int id)
        {
            try
            {
                return View("Edit", _bookHistoryService.GetBookHistories(id));
            }
            catch (WarningException)
            {
                return HttpNotFound();
            }
        }

        [HttpPost,
         ValidateAntiForgeryToken]
        public ActionResult Edit(BookHistoryVm book)
        {
            if (ModelState.IsValid)
            {
                _service.Save(book);
                _service.AddRecord(book);
                //return RedirectToAction("Index");
            }

            return View("Edit", _bookHistoryService.GetBookHistories(book.BookId));
        }

        public ActionResult Save(BookHistoryVm history)
        {
            return View("Index");
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
    }
}
