using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.ViewModels.Users;
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
            Users = team.Users == null ? new List<UserVm>() : team.Users.Select(u => new UserVm(u)).ToList();
        }

        public TeamEditVm()
        {
        }

        public int TeamId { get; set; }

        [StringLength(50),
        Required(ErrorMessage = "Team name is required!"),
        Display(Name = "Team name")]
        public string Name { get; set; }

        [StringLength(50)]
        public string Description { get; set; }

        public List<UserVm> Users { get; set; } 

        public bool IsDeleted { get; set; }
    }
}