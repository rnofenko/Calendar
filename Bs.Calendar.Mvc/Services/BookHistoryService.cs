using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.ViewModels;

namespace Bs.Calendar.Mvc.Services
{
    public class BookHistoryService
    {
        private readonly RepoUnit _unit;

        public BookHistoryService(RepoUnit unit)
        {
            _unit = unit;
        }

        public List<BookHistoryVm> GetBookHistory(int bookId)
        {
            var book = _unit.Book.Get(bookId);
            var bookHistory = _unit.BookHistory.Load(h => h.BookId == bookId).ToList();
            return (from history in bookHistory let user = _unit.User.Get(history.UserId) select new BookHistoryVm(book, user, history)).ToList();
        }
    }
}