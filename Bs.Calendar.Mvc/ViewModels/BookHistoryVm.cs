using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Bs.Calendar.Models;
using Bs.Calendar.Models.Bases;

namespace Bs.Calendar.Mvc.ViewModels
{
    public class BookHistoryVm
    {
        public List<BookHistoryItemVm> BookHistoryList { get; set; }
        public int BookId { get; set; }

        public int ReaderId { get; set; }
        public string ReaderName { get; set; }

        [Display(Name = "Description")]
        public string BookDescription { get; set; }

        [Required(ErrorMessage = "The code of the book must be specified"),
        RegularExpression(@"^[a-zA-Zа-яА-Я0-9\-]+$", ErrorMessage = "Code should contain only letters or digits, or dash"),
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

        public bool HasCover { get; set; }

        public BookHistoryVm(Book book)
        {
            BookId = book.Id;
            BookCode = book.Code;
            BookTitle = book.Title;
            BookAuthor = book.Author;
            BookDescription = book.Description;
            BookHistoryList = new List<BookHistoryItemVm>();
        }

        public BookHistoryVm()
        {
            
        }
    }
}