using System.Collections.Generic;
using System.Linq;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.ViewModels;

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

        public bool IsValid(BookEditVm book)
        {
            bool result = true;
            result = result && (book.Title != string.Empty);
            result = result && (book.Author != string.Empty);
            return result;
        }

        public void Delete(int id)
        {
            _repoUnit.Book.Delete(_repoUnit.Book.Get(id));
        }

        public void Save(BookEditVm bookModel)
        {
            var book = Get(bookModel.BookId) ?? new Book();
            book.Title = bookModel.Title;
            book.Author = bookModel.Author;
            _repoUnit.Book.Save(book);
        }
    }
}