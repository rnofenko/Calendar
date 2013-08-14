using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web.Helpers;

using System.Reflection;

namespace Bs.Calendar.Models
{
    public class UserFilter
    {
        public string SearchString { get; set; }

        public string SortBy { get; set; }

        public Roles RoleFilter { get; set; }
        public ApproveStates ApproveStateFilter { get; set; }
        public LiveStatuses LiveStatusFilter { get; set; }
    }
}
