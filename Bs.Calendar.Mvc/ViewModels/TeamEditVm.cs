using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Bs.Calendar.Models;
using Bs.Calendar.Rules;

namespace Bs.Calendar.Mvc.ViewModels
{
    public class TeamEditVm
    {
        public TeamEditVm(Team team)
        {
            TeamId = team.Id;
            Name = team.Name;
            Description = team.Description;
            HeaderPattern = Config.Instance.TeamHeaderPattern;
            Users = team.Users == null ? new List<TeamUserVm>() : team.Users.Select(u => new TeamUserVm(u)).ToList();
        }

        public TeamEditVm()
        {
            HeaderPattern = Config.Instance.TeamHeaderPattern;
        }

        public int TeamId { get; set; }

        [StringLength(50),
        Required(ErrorMessage = "Team name is required!"),
        Display(Name = "Team name")]
        public string Name { get; set; }

        [StringLength(50)]
        public string Description { get; set; }

        public List<TeamUserVm> Users { get; set; }
        public List<int> TeamUserIds { get; set; } 

        public string HeaderPattern { get; set; }
    }

    public class TeamUserVm
    {
        public TeamUserVm(User user)
        {
            UserId = user.Id;
            Name = string.Format("{0} {1}.", user.LastName, user.FirstName[0]);
        }

        public TeamUserVm()
        {
        }

        public int UserId { get; set; }
        public string Name { get; set; }
    }
}