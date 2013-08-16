using System.Linq;
using System.Text.RegularExpressions;
using Bs.Calendar.Models;
using Bs.Calendar.Core;
using Bs.Calendar.Rules;
using System.Linq;

namespace Bs.Calendar.Mvc.ViewModels.Users
{
public class UserFilterVm
    {
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

            if (SearchString.IsNotEmpty())
            {
                SearchString = Regex.Replace(SearchString.Trim(), @"\s+", " ").ToLower();
                filter.EmailOrFullName = SearchString.Split().ToList();
            }

            filter.Roles = OnlyAdmins ? Roles.Admin : Roles.All;
            filter.ApproveStates = NotApproved ? ApproveStates.All : ApproveStates.Approved;
            filter.LiveStatuses = Deleted ? LiveStatuses.All : LiveStatuses.Active;

            return filter;
        }
    }
}