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
        private readonly RepoUnit _unit;

        public BookService(RepoUnit repository)
        {
            _unit = repository;
        }

        public Book Get(int id)
        {
            var book = _unit.Book.Get(id);
            return book;
        }
        
        public Book Get(string code)
        {
            return _unit.Book.Get(b => b.Code == code);
        }

        public BookHistoryVm GetBookHistories(int bookId)
        {
            var book = _unit.Book.Get(bookId);
            if (book.Id == 0)
            {
                throw new WarningException();
            }
            var result = new BookHistoryVm(book)
            {
                BookHistoryList = new List<BookHistoryItemVm>()
            };
            foreach (var bookHistory in book.BookHistories.OrderByDescending(h => h.OrderDate).ThenBy(h => h.UserId).ThenByDescending(h => h.Action))
            {
                result.BookHistoryList.Add(new BookHistoryItemVm(bookHistory));
            }
            return result;
        }

        public IEnumerable<Book> Load(string orderby, string searchStr)
        {
            var books = _search(searchStr);
            books = _orderBy(books, orderby);
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
            if (orderby == "reader")
            {
                return asc ? books.OrderBy(book => book.ReaderName) : books.OrderByDescending(book => book.ReaderName);
            }
            return asc ? books.OrderBy(book => book.Id) : books.OrderByDescending(book => book.Id);
        }

        private IEnumerable<Book> _search(string @searchStr)
        {
            if (String.IsNullOrEmpty(searchStr))
            {
                return _unit.Book.Load();
            }

            var res = _unit.Book.Load().Where(book => book.Description != null && book.Description.ToLower().Contains(searchStr)).ToList();

            res.AddRange(_unit.Book.Load().Where(book => book.Code.ToLower().Contains(searchStr)
                || book.Author.ToLower().Contains(searchStr) 
                || book.Title.ToLower().Contains(searchStr)));

            return res;
        }

        public void Validate(BookEditVm book)
        {
            if (string.IsNullOrEmpty(book.Code))
            {
                throw new WarningException(string.Format("Code should be specified"));
            }
            if (string.IsNullOrEmpty(book.Title))
            {
                throw new WarningException(string.Format("Title should be specified"));
            }
            if (string.IsNullOrEmpty(book.Author))
            {
                throw new WarningException(string.Format("Author should be specified"));
            }
            if (_unit.Book.Load(b => b.Code == book.Code && b.Id != book.BookId).Any())
            {
                throw new WarningException(string.Format("Code should be unique"));
            }
        }

        public void Delete(int id)
        {
            _unit.Book.Delete(_unit.Book.Get(id));
        }

        public void Save(BookHistoryVm model)
        {
            Validate(new BookEditVm(model));
            var book = Get(model.BookId) ?? new Book();
            book.Code = model.BookCode;
            book.Title = model.BookTitle;
            book.Author = model.BookAuthor;
            book.Description = model.BookDescription;
            book.ReaderName = model.ReaderId == 0 ? "None" : _unit.User.Get(model.ReaderId).FullName;
            if (model.BookHistoryList != null)
            {
                UpdateHistory(model);
            }
            _unit.Book.Save(book);
        }

        private void UpdateHistory(BookHistoryVm model)
        {
            foreach (var historyRecord in model.BookHistoryList)
            {
                if (historyRecord.Deleted)
                {
                    var historyToDelete = _unit.BookHistory.Get(historyRecord.Id);
                    _unit.BookHistory.Delete(historyToDelete);
                }
                else
                {
                    _unit.BookHistory.Save(new BookHistoryItem
                    {
                        Id = historyRecord.Id,
                        BookId = historyRecord.BookId,
                        UserId = historyRecord.UserId,
                        OrderDate = historyRecord.OrderDate,
                        Action = historyRecord.Action
                    });
                }
            }
        }
    }
}