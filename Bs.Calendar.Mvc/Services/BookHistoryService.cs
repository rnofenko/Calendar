using System.Collections.Generic;
using System.ComponentModel;
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

        public BookHistoryVm GetBookHistories(int bookId)
        {
            var book = _unit.Book.Get(bookId);
            if (book.Id == 0)
            {
                throw new WarningException();
            }
            var bookHistories = _unit.BookHistory.Load(h => h.BookId == bookId).OrderByDescending(h => h.OrderDate).ThenByDescending(h => h.Action);
            foreach (var bookHistory in bookHistories)
            {
                bookHistory.OrderDate = bookHistory.OrderDate.Date.AddDays(1);
            }
            var result = new BookHistoryVm(book) {BookHistoryList = new List<BookHistory>(bookHistories)};
            return result;
        }

        public void AddRecord(BookHistoryVm bookHistoryVm)
        {
            var bookHistoryItem = bookHistoryVm.BookHistoryList.LastOrDefault();
            bookHistoryItem.BookId = bookHistoryVm.BookId;
            _unit.BookHistory.Save(bookHistoryItem);
        }
    }
}