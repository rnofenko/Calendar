using System.Linq;
using System.Text.RegularExpressions;
using Bs.Calendar.Core;
using Bs.Calendar.Models;
using Bs.Calendar.Rules;

namespace Bs.Calendar.Mvc.ViewModels.Rooms
{
    public class RoomFilterVm
    {
        public RoomFilter Map()
        {
            return new RoomFilter
                       {
                           SearchString = SearchString.IsEmpty() ? string.Empty : SearchString,
                           SortByField = SortByField.IsEmpty() ? "Id" : SortByField,

                           Page = Page < 1 ? 1 : Page,
                           PageSize = Config.Instance.PageSize,
                       };
        }

        public string SearchString { get; set; }

        public string SortByField { get; set; }

        public int Page { get; set; }

        public int TotalPages { get; set; }
    }
}