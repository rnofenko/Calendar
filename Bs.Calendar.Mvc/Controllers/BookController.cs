using System.Collections.Generic;
using System.Web.Http;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Services;

namespace Bs.Calendar.Mvc.Controllers
{
    public class BookController : ApiController
    {
        private readonly BookService _service;

        public BookController(BookService service)
        {
            _service = service;
        }

        // GET: /Api/Book/
        public List<Book> Get()
        {
            return _service.List();
        }

    }
}
