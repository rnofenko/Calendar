using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bs.Calendar.Models;

namespace Bs.Calendar.Mvc.ViewModels
{
    public class UsersVm
    {
        public UsersVm(IEnumerable<User> users, UserFilterVm userFilterVm)
        {
            Users = users ?? Enumerable.Empty<User>();
            UserFilterVm = userFilterVm;
        }

        public IEnumerable<User> Users { get; set; }
        public PagingVm PagingVm { get; set; }
        public UserFilterVm UserFilterVm { get; set; }
    }
}