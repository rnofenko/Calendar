using System.Collections.Generic;
using System.Linq;
using Bs.Calendar.DataAccess;
using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Models;

namespace Bs.Calendar.Mvc.Services
{
    public class TeamService
    {

        public IEnumerable<Team> LoadTeams()
        {
            using (var team = new RepoUnit())
            {
                var teams = team.Team.Load().ToList();

                if (!teams.Any())
                {
                    team.Team.Save(new Team { Name = "Team #1" });
                    teams = team.Team.Load().ToList();
                }

                return teams;
            }
        }

    }
}