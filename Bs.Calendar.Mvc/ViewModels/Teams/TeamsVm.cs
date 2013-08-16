using System.Collections.Generic;
using Bs.Calendar.Models;

namespace Bs.Calendar.Mvc.ViewModels.Teams
{
    public class TeamsVm
    {
        public IEnumerable<Team> Teams { get; set; }

        public TeamFilterVm Filter { get; set; }
    }
}