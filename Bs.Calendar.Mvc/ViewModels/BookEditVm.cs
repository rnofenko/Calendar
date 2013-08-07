using System.ComponentModel.DataAnnotations;
using Bs.Calendar.Models;
using Bs.Calendar.Models.Bases;

namespace Bs.Calendar.Mvc.ViewModels
{
    public class BookEditVm
    {
        public int BookId { get; set; }

        public BookEditVm()
        {
        }

        public BookEditVm(Book book)
        {
            BookId = book.Id;
            Code = book.Code;
            Title = book.Title;
            Author = book.Author;
        }

        [StringLength(BaseEntity.LENGTH_NAME),
        Required(ErrorMessage = "The code of the book must be specified"),
        Display(Name = "Code")]
        public string Code { get; set; }

        [StringLength(BaseEntity.LENGTH_NAME),
        Required(ErrorMessage = "The title of the book must be specified"),
        Display(Name = "Title")]
        public string Title { get; set; }

        [Display(Name = "Author"),
        Required(ErrorMessage = "Author should be specified"),
        StringLength(BaseEntity.LENGTH_NAME)]
        public string Author { get; set; }
    }
}