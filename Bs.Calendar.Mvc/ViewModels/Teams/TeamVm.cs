using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Bs.Calendar.Models;

namespace Bs.Calendar.Mvc.ViewModels.Teams
{
    public class TeamVm
    {
        public TeamVm(Team team)
        {
            Id = team.Id;
            Name = team.Name;
        }
        public TeamVm()
        {
        }

        public int Id { get; set; }
        public string Name { get; set; }
    }
}