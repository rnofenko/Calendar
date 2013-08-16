using System.Collections.Generic;

namespace Bs.Calendar.Models
{
    public class UserFilter
    {
        public List<string> EmailOrFullName { get; set; }

        public int PageSize { get; set; }

        public int Page { get; set; }

        public string SortByField { get; set; }

        public Roles Roles { get; set; }

        public ApproveStates ApproveStates { get; set; }

        public LiveStatuses LiveStatuses { get; set; }
    }
}