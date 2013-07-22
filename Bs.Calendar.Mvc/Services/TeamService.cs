using System;
using System.Collections.Generic;
using System.Linq;
using Bs.Calendar.DataAccess;
using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.ViewModels;

namespace Bs.Calendar.Mvc.Services
{
    public class TeamService
    {
        private readonly RepoUnit _repoUnit;

        public TeamService(RepoUnit repository)
        {
            _repoUnit = repository;
        }

        public TeamEditVm CreateViewModel()
        {
            return new TeamEditVm();
        }

        public void Save(TeamEditVm team)
        {
            _repoUnit.Team.Save(team);
        }

        public bool IsValid(TeamEditVm team)
        {
            return team.Name != string.Empty;
        }

        public TeamEditVm Load(int id)
        {
            return _repoUnit.Team.Get(id);
        }

        public void Delete(TeamEditVm team)
        {
            if (team == null)
            {
                throw new ArgumentNullException("reference to the deleted instance cannot be null");
            }

            _repoUnit.Team.Delete(team);
        }

        public void Delete(int id)
        {
            var team = _repoUnit.Team.Get(id);

            Delete(team);
        }

        public TeamsVm GetAllTeams()
        {
            var teams = _repoUnit.Team.Load().ToList();

            return new TeamsVm { Teams = teams };
        }
    }
}