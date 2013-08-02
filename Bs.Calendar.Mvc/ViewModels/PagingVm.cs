namespace Bs.Calendar.Mvc.ViewModels
{
    public class PagingVm
    {
        public PagingVm()
        {
            Page = 1;
        }

        public PagingVm(string searchStr, string sortByStr, int totalPages, int page = 1)
        {
            SearchStr = searchStr;
            SortByStr = sortByStr;
            Page = page;
            TotalPages = totalPages;
        }

        public PagingVm(PagingVm pagingVm)
            : this(pagingVm.SearchStr, pagingVm.SortByStr, pagingVm.TotalPages, pagingVm.Page)
        {
        }

        public string SearchStr { get; set; }

        public string SortByStr { get; set; }

        public int Page { get; set; }

        public int TotalPages { get; set; }
    }
}