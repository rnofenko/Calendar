using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bs.Calendar.Models;

namespace Bs.Calendar.Mvc.ViewModels
{
    public class UserViewModel
    {
        public IEnumerable<User> Users { get; set; }
    }
}