using System.Collections.Generic;
using System.Web.Mvc;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Services;

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
    }
}
