using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.ViewModels.Teams;
using Bs.Calendar.Rules;

namespace Bs.Calendar.Mvc.Services
{
    public class TeamService
    {
        private readonly RepoUnit _unit;

        public TeamService(RepoUnit repoUnit)
        {
            _unit = repoUnit;
        }

        public IEnumerable<TeamUserVm> GetAllUsers(int teamId)
        {
            var users = _unit.User.Load(u => u.Teams.All(t => t.Id != teamId)).ToList();
            return users.Select(u => new TeamUserVm(u)).ToList();
        }

        public Team GetTeam(int teamId) 
        {
            var team = _unit.Team.Get(teamId);
            return team;
        }

        public IEnumerable<Team> GetAllTeams() 
        {
            return _unit.Team.Load().ToList();
        }

        public void CreateTeam(TeamEditVm teamModel) 
        {
            validateTeam(teamModel);

            saveTeam(teamModel);
        }

        public void DeleteTeam(int id) 
        {
            _unit.Team.Delete(_unit.Team.Get(id));
        }

        public void EditTeam(TeamEditVm teamModel) 
        {
            var teamToEdit = GetTeam(teamModel.TeamId);

            validateTeam(teamModel, teamToEdit);

            saveTeam(teamModel, teamToEdit);
        }

        private void saveTeam(TeamEditVm teamVm, Team editedTeam = null)
        {
            var team = editedTeam ?? new Team();
            team.Name = teamVm.Name;
            team.Description = teamVm.Description;

            addUsersToTeam(team, teamVm.Users);
            _unit.Team.Save(team);
        }

        private void addUsersToTeam(Team team, IEnumerable<TeamUserVm> users)
        {
            if (team.Users != null) team.Users.Clear();
            if (users == null) return;

            var userIds = users.Select(u => u.UserId);
            team.Users = _unit.User.Load(u => userIds.Contains(u.Id)).ToList();
        }

        private void validateTeam(TeamEditVm teamVm, Team editedTeam = null)
        {
            var comparisonType = StringComparison.OrdinalIgnoreCase;

            if (editedTeam != null && editedTeam.Name.Equals(teamVm.Name, comparisonType))
            {
                return;
            }

            if (_unit.Team.Get(t => t.Name.Equals(teamVm.Name, comparisonType)) != null)
            {
                throw new WarningException(string.Format("Team with name {0} already exists", teamVm.Name));
            }
        }

        public TeamsVm RetreiveList(TeamFilterVm filterVm) 
        {
            var filter = filterVm.Map();
            var teams = _unit.Team.Load(filter);
            updatePagingData(filterVm, teams);

            return new TeamsVm 
            {
                Teams = teams,
                Filter = filterVm
            };
        }

        private void updatePagingData(TeamFilterVm filter, IQueryable<Team> teams)
        {
            var pageSize = Config.Instance.PageSize;

            filter.TotalPages = PageCounter.GetTotalPages(teams.Count(), pageSize);
            filter.Page = PageCounter.GetRangedPage(filter.Page, filter.TotalPages);
        }

        private IQueryable<Team> sortByStr(IQueryable<Team> teams, string sortByStr) 
        {
            teams = teams.OrderByIf(!string.IsNullOrEmpty(sortByStr),
                        team => team.Name);

            teams = teams.OrderByIf(string.IsNullOrEmpty(sortByStr),
                        team => team.Id);
            return teams;
        }
    }
}