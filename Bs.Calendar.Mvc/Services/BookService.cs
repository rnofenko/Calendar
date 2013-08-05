using System.Collections.Generic;
using System.Linq;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;

namespace Bs.Calendar.Mvc.Services
{
    public class BookService
    {
        private readonly RepoUnit _repoUnit;

        public BookService(RepoUnit repository)
        {
            _repoUnit = repository;
        }

        public Book Get(int id)
        {
            var book = _repoUnit.Book.Get(id);
            return book;
        }

        public List<Book> Load()
        {
            var books = _repoUnit.Book.Load();
            return books.ToList();
        }

    }
}