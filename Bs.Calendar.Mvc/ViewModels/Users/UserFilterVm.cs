using System.Text.RegularExpressions;
using Bs.Calendar.Models;
using Bs.Calendar.Core;
using Bs.Calendar.Rules;

namespace Bs.Calendar.Mvc.ViewModels.Users
{
    public class UserFilterVm
    {
        public UserFilterVm(string searchStr, string sortByStr, int totalPages, int page = 1,
            bool showDeleted = false, bool showAdmins = false, bool showNotApproved = true)
        {
            SearchString = searchStr;
            SortByField = sortByStr;
            Page = page;
            TotalPages = totalPages;

            Deleted = showDeleted;
            OnlyAdmins = showAdmins;
            NotApproved = showNotApproved;
        }

        public UserFilterVm(UserFilterVm pagingVm)
            : this(pagingVm.SearchString, pagingVm.SortByField, pagingVm.TotalPages, pagingVm.Page, pagingVm.Deleted, pagingVm.OnlyAdmins, pagingVm.NotApproved)
        {
        }

        public UserFilterVm()
        {
        }

        public string SearchString { get; set; }

        public string SortByField { get; set; }

        public int Page { get; set; }

        public int TotalPages { get; set; }

        public bool OnlyAdmins { get; set; }
        public bool NotApproved { get; set; }
        public bool Deleted { get; set; }

        public UserFilter Map()
        {
            var filter = new UserFilter
                {
                    PageSize = Config.Instance.PageSize,
                    Page = Page < 1 ? 1 : Page,
                    SortByField = SortByField.IsEmpty() ? "Id" : SortByField
                };

            if(SearchString.IsNotEmpty())
            {
                SearchString = Regex.Replace(SearchString.Trim(), @"\s+", " ").ToLower();
                filter.Email = SearchString;

                var splitedString = SearchString.Split();
                if (splitedString.Length>0)
                {
                    filter.Name = splitedString[0];
                }
                if (splitedString.Length > 1)
                {
                    filter.SecondName = splitedString[1];
                }
            }

            filter.Roles = OnlyAdmins ? Roles.Admin : Roles.All;
            filter.ApproveStates = NotApproved ? ApproveStates.All : ApproveStates.Approved;
            filter.LiveStatuses = Deleted ? LiveStatuses.All : LiveStatuses.Active;

            return filter;
        }
    }
}