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
            var bookHistories = _unit.BookHistory.Load(h => h.BookId == bookId).OrderByDescending(h => h.OrderDate).ThenByDescending(h => h.Action);//.ToList();
            var result = new BookHistoryVm(book)
            {
                BookHistoryList = new List<BookHistoryItemVm>()
            };
            foreach (var bookHistory in bookHistories)
            {
                result.BookHistoryList.Add(new BookHistoryItemVm(bookHistory));
            }
            return result;
        }

        public void AddRecord(List<BookHistoryItemVm> bookHistoryItemsList)
        {
            foreach (var bookHistoryItem in bookHistoryItemsList)
            {
                _unit.BookHistory.Save(new BookHistoryItem
                {
                    Id = bookHistoryItem.Id,
                    BookId = bookHistoryItem.BookId,
                    UserId = bookHistoryItem.UserId,
                    OrderDate = bookHistoryItem.OrderDate,
                    Action = bookHistoryItem.Action
                });
            }
        }

        public void ClearHistory(int bookId)
        {
            _unit.Book.Get(bookId).BookHistories.Clear();

            var bookHistory  = _unit.BookHistory.Load(h => h.BookId == bookId);
            foreach (var history in bookHistory)
            {
                _unit.BookHistory.Delete(history);
            }
        }
    }
}