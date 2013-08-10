using System;
using Bs.Calendar.Models;

namespace Bs.Calendar.Mvc.ViewModels
{
    public class BookHistoryVm
    {
        public BookHistoryVm()
        {            
        }

        //public BookHistoryVm(Book book, User user, DateTime returnDate)
        //{
        //    BookId = book.Id;
        //    BookTitle = book.Title;
        //    BookAuthor = book.Author;

        //    UserId = user.Id;
        //    UserFullName = user.FullName;

        //    TakeDate = DateTime.Now;
        //    ReturnDate = returnDate;
        //}

        public BookHistoryVm(Book book, User user, BookHistory bookHistory )
        {
            BookId = book.Id;
            BookTitle = book.Title;
            BookAuthor = book.Author;

            UserId = user.Id;
            UserFullName = user.FullName;

            TakeDate = bookHistory.TakeDate;
            ReturnDate = bookHistory.ReturnDate;
        }

        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public string BookAuthor { get; set; }

        public int UserId { get; set; }
        public string UserFullName { get; set; }
        
        public DateTime TakeDate { get; set; }
        public DateTime ReturnDate { get; set; }
    }
}