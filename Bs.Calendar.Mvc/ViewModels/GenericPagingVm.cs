using System.Collections.Generic;
using System.Linq;

namespace Bs.Calendar.Mvc.ViewModels
{
    public class GenericPagingVm<T>
    {
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<T> Data { get; set; }

        public GenericPagingVm(IEnumerable<T> data, int page)
        {
            PageSize = 7;
            CurrentPage = page;
            var count = data.Count();
            TotalPages = Rules.PageCounter.GetTotalPages(count, PageSize);
            var skip = (CurrentPage - 1) * PageSize;
            Data = data.Skip(skip).Take(PageSize);
        }
    }
}