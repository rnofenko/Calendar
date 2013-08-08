using Bs.Calendar.Models;

namespace Bs.Calendar.Mvc.ViewModels
{
    public class PagingVm
    {
        public PagingVm()
        {
            Page = 1;
            TotalPages = 1;
        }

        public PagingVm(string searchStr, string sortByStr, int totalPages, int page = 1,
            bool showDeleted = false, bool showAdmins = false, bool showNotApproved = true)
        {
            SearchStr = searchStr;
            SortByStr = sortByStr;
            Page = page;
            TotalPages = totalPages;

            ShowDeleted = showDeleted;
            ShowAdmins = showAdmins;
            ShowNotApproved = showNotApproved;
        }

        public PagingVm(PagingVm pagingVm)
            : this(pagingVm.SearchStr, pagingVm.SortByStr, pagingVm.TotalPages, pagingVm.Page, pagingVm.ShowDeleted, pagingVm.ShowAdmins, pagingVm.ShowNotApproved)
        {
        }

        public Roles RolesFilter
        { 
            get
            {
                var role = Roles.Admin;
                return ShowAdmins ? role : Roles.Simple | role;
            }
        }

        public LiveState StateFilter
        {
            get
            {
                var state = LiveState.Deleted | LiveState.NotApproved;

                if(!ShowNotApproved && !ShowDeleted)
                {
                    return state | LiveState.Active;
                }
                
                state = !ShowNotApproved ? state ^ LiveState.NotApproved : state;
                state = !ShowDeleted ? state ^ LiveState.Deleted : state;

                return state;
            }
        }

        public string SearchStr { get; set; }

        public string SortByStr { get; set; }

        public int Page { get; set; }

        public int TotalPages { get; set; }

        public bool ShowAdmins { get; set; }
        public bool ShowNotApproved { get; set; }
        public bool ShowDeleted { get; set; }
    }
}