using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bs.Calendar.Mvc.ViewModels
{
    public class TeamsVm
    {
        public IEnumerable<Bs.Calendar.Models.Team> Teams { get; set; }
    }
}