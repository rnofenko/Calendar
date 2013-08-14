using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using Bs.Calendar.Models;

namespace Bs.Calendar.Mvc.ViewModels
{
    public class UserFilterVm
    {
        public UserFilterVm(UserFilter filter)
        {
            
        }
        
        public UserFilter Map()
        {
            return new UserFilter()
                       {
                           RoleFilter = OnlyAdmins ? Roles.Admin : Roles.All,
                           LiveStatusFilter = IncludeDeleted ? LiveStatuses.All : LiveStatuses.Active,
                           ApproveStateFilter = IncludeNotApproved ? ApproveStates.All : ApproveStates.Approved
                       };
        }

        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }

        public string SearchString { get; set; }

        public string SortBy { get; set; }

        public bool OnlyAdmins { get; set; }
        public bool IncludeDeleted { get; set; }
        public bool IncludeNotApproved { get; set; }
    }
}
