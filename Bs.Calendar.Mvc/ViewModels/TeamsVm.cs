using System.Collections.Generic;
using Bs.Calendar.Models;

namespace Bs.Calendar.Mvc.ViewModels
{
    public class TeamsVm
    {
        public IEnumerable<Team> Teams { get; set; }

        public PagingVm PagingVm { get; set; }
    }
}