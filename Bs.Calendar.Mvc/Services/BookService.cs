using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public IEnumerable<Book> Load(string orderby, string searchStr)
        {
            IEnumerable<Book> books = _repoUnit.Book.Load();
            books = _search(books, searchStr);
            books = _orderBy (books, orderby);
            return books;
        }

        private static IEnumerable<Book> _orderBy(IEnumerable<Book> books, string @orderby)
        {
            bool asc = true;
            if (!String.IsNullOrEmpty(orderby))
            {
                orderby = orderby.ToLower();
                if (orderby[0] == '-')
                {
                    asc = false;
                    orderby = orderby.Substring(1);
                }
            }
            if (orderby == "code")
            {
                return asc ? books.OrderBy(book => book.Code) : books.OrderByDescending(book => book.Code);
            }
            if (orderby == "title")
            {
                return asc ? books.OrderBy(book => book.Title) : books.OrderByDescending(book => book.Title);
            }
            if (orderby == "author")
            {
                return asc ? books.OrderBy(book => book.Author) : books.OrderByDescending(book => book.Author);
            }
            return asc ? books.OrderBy(book => book.Id) : books.OrderByDescending(book => book.Id);
        }

        private static IEnumerable<Book> _search(IEnumerable<Book> books, string @searchStr)
        {
            if (String.IsNullOrEmpty(searchStr))
            {
                return books;
            }
            return books.Where(book =>
                               book.Code.ToLower().Contains(searchStr)
                               || book.Author.ToLower().Contains(searchStr)
                               || book.Title.ToLower().Contains(searchStr));
        }


        public void Validate(BookEditVm book)
        {
            if (string.IsNullOrEmpty(book.Code))
            {
                throw new WarningException(string.Format("Code must be specified"));
            }
            if (string.IsNullOrEmpty(book.Title))
            {
                throw new WarningException(string.Format("Title must be specified"));
            }
            if (string.IsNullOrEmpty(book.Author))
            {
                throw new WarningException(string.Format("Author must be specified"));
            }
            if (_repoUnit.Book.Load(b => b.Code == book.Code).Any())
            {
                throw new WarningException(string.Format("Code must be unique"));
            }
        }

        public void Delete(int id)
        {
            _repoUnit.Book.Delete(_repoUnit.Book.Get(id));
        }

        public void Save(BookEditVm bookModel)
        {
            Validate(bookModel);
            var book = Get(bookModel.BookId) ?? new Book();
            book.Code = bookModel.Code;
            book.Title = bookModel.Title;
            book.Author = bookModel.Author;
            _repoUnit.Book.Save(book);
        }
    }
}