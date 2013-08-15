using System.Collections.Generic;
using Bs.Calendar.Models;

namespace Bs.Calendar.Mvc.ViewModels.Users
{
    public class UsersVm
    {
        public IEnumerable<User> Users { get; set; }

        public UserFilterVm Filter { get; set; }
    }
}