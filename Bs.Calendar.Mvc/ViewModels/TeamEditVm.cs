using System;
using System.ComponentModel.DataAnnotations;
using Bs.Calendar.Models;

namespace Bs.Calendar.Mvc.ViewModels
{
    public class TeamEditVm
    {
        public class TeamEditVmExtra
        {
            public string ViewTitle { get; set; }
            public string CallAction { get; set; }
            public string CallController { get; set; }
        }

        public TeamEditVmExtra Extra { get; set; }

        public int Id { get; set; }

        [StringLength(Models.Bases.BaseEntity.LENGTH_NAME),
        Required(ErrorMessage = "The name of the room must be specified"),
        Display(Name = "Name")]
        public string Name { get; set; }

        private void Setup(
            int id,
            string name)
        {
            this.Id = id;
            this.Name = name;

            Extra = new TeamEditVmExtra();
        }

        public TeamEditVm(Team team)
        {
            if(team == null)
            {
                throw new ArgumentNullException("Model instance cannot be set to null");
            }

            Setup(
                team.Id,
                team.Name);
        }

        public TeamEditVm()
        {
            Setup(
                0,
                string.Empty);
        }

        public static implicit operator Team(TeamEditVm teamViewModel)
        {
            if (teamViewModel == null)
            {
                throw new ArgumentNullException("reference to the converted instance cannot be null");
            }

            return new Team()
            {
                Id = teamViewModel.Id,
                Name = teamViewModel.Name
            };
        }

        public static implicit operator TeamEditVm(Team team)
        {
            return new TeamEditVm(team);
        }
    }
}