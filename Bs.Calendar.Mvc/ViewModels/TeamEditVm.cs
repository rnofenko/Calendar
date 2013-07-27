using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Bs.Calendar.Models;

namespace Bs.Calendar.Mvc.ViewModels
{
    public class TeamEditVm
    {
        public int TeamId { get; set; }

        public TeamEditVm(Team team)
        {
            TeamId = team.Id;
            Name = team.Name;
            Description = team.Description;
        }

        public TeamEditVm()
        {
        }

        [StringLength(50),
        Required(ErrorMessage = "Team name is required!"),
        Display(Name = "Team name")]
        public string Name { get; set; }

        [StringLength(50),
        Required(ErrorMessage = "Description is required!")]
        public string Description { get; set; }
    }
}