using System.Collections.Generic;
using System.Linq;
using Bs.Calendar.Models;

namespace Bs.Calendar.Mvc.ViewModels
{
    public class BookTagVm
    {
        public int Id { get; set; }
        public string ReaderName { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public bool HasCover { get; set; }
        public List<string> BookTags { get; set; }

        public BookTagVm(Book book)
        {
            Id = book.Id;
            Code = book.Code;
            Title = book.Title;
            Author = book.Author;
            Description = book.Description;
            HasCover = book.HasCover;
            BookTags = book.Tags == null ? null : book.Tags.Select(x => x.Name).ToList();
        }

        public BookTagVm()
        {

        }
    }
}