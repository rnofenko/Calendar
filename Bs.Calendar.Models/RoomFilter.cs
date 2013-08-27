using System.Collections.Generic;

namespace Bs.Calendar.Models
{
    public class RoomFilter
    {
        public string SearchString { get; set; }

        public string SortByField { get; set; }

        public List<int> Colors { get; set; }

        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}