using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Models.Bases;

namespace Bs.Calendar.Mvc.ViewModels
{
    public class BookHistoryVm
    {
        public List<BookHistory> BookHistoryList { get; set; }

        public int UserId { get; set; }
        public int BookId { get; set; }        
        public DateTime OrderDate { get; set; }
        public DirectionEnums Action { get; set; }

        [Display(Name = "Description")]
        public string BookDescription { get; set; }

        [Required(ErrorMessage = "The code of the book must be specified"),
        Display(Name = "Code")]
        public string BookCode { get; set; }

        [StringLength(BaseEntity.LENGTH_NAME),
        Required(ErrorMessage = "The title of the book must be specified"),
        Display(Name = "Title")]
        public string BookTitle { get; set; }

        [Display(Name = "Author"),
        Required(ErrorMessage = "Author should be specified"),
        StringLength(BaseEntity.LENGTH_NAME)]
        public string BookAuthor { get; set; }

        public BookHistoryVm(Book book)
        {
            var repoUnit = Ioc.Resolve<RepoUnit>();

            BookId = book.Id;
            BookCode = book.Code;
            BookTitle = book.Title;
            BookAuthor = book.Author;
            BookDescription = book.Description;

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