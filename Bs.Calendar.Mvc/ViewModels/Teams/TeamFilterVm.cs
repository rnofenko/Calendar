using System.Text.RegularExpressions;
using Bs.Calendar.Core;
using Bs.Calendar.Models;
using Bs.Calendar.Rules;

namespace Bs.Calendar.Mvc.ViewModels.Teams
{
    public class TeamFilterVm
    {
        public string SearchString { get; set; }

        public string SortByField { get; set; }

        public int Page { get; set; }

        public int TotalPages { get; set; }

        public TeamFilter Map() 
        {
            var filter = new TeamFilter() 
            {
                PageSize = Config.Instance.PageSize,
                Page = Page,
                SortByField = SortByField.IsEmpty() ? "Id" : SortByField, 
            };

            if (SearchString.IsNotEmpty()) {
                SearchString = Regex.Replace(SearchString.Trim(), @"\s+", " ").ToLower();
                filter.Name = SearchString;
            }

            return filter;
        }
    }
}