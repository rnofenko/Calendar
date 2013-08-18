using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Bs.Calendar.Models;
using Bs.Calendar.Rules;

namespace Bs.Calendar.Mvc.ViewModels.Teams
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

        public string HeaderPattern { get; set; }

        public bool IsDeleted { get; set; }
    }
}