using System.Collections.Generic;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;

namespace Bs.Calendar.Mvc.ViewModels
{
    public class BookHistoryVm
    {
        public List<BookHistory> BookHistoryList { get; set; }

        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public string BookAuthor { get; set; }

        public BookHistoryVm(Book book)
        {
            var repoUnit = Ioc.Resolve<RepoUnit>();

            BookId = book.Id;
            BookTitle = book.Title;
            BookAuthor = book.Author;

            BookHistoryList = new List<BookHistory>();
            var bookHistories = repoUnit.BookHistory.Load(h => h.BookId == book.Id);
            foreach (var bookHistory in bookHistories)
            {
                BookHistoryList.Add(bookHistory);
            }
        }

        public BookHistoryVm()
        {
            
        }
    }
}