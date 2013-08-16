using System.Collections.Generic;

namespace Bs.Calendar.Models
{
    public class UserFilter
    {
        public string Email { get; set; }

        public string Name { get; set; }

        public string SecondName { get; set; }

        public int PageSize { get; set; }

        public int Page { get; set; }

        public string SortByField { get; set; }

        public Roles Roles { get; set; }

        public ApproveStates ApproveStates { get; set; }

        public LiveStatuses LiveStatuses { get; set; }

        public List<string> EmailOrFullName { get; set; }
    }
}