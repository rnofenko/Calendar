using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.ViewModels.Teams;
using Bs.Calendar.Mvc.ViewModels.Users;
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

        public IEnumerable<UserVm> GetAllUsers(int teamId)
        {
            var users = _unit.User.Load(u => u.Teams.All(t => t.Id != teamId) && u.Live == LiveStatuses.Active).ToList();
            return users.Select(u => new UserVm(u)).ToList();
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
            team.Live = teamVm.IsDeleted ? LiveStatuses.Deleted : LiveStatuses.Active;

            addUsersToTeam(team, teamVm.Users);
            _unit.Team.Save(team);
        }

        private void addUsersToTeam(Team team, IEnumerable<UserVm> users)
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
            updatePagingData(filterVm);
            var filter = filterVm.Map();
            var teams = _unit.Team.Load(filter);

            return new TeamsVm 
            {
                Teams = teams,
                Filter = filterVm
            };
        }

        private void updatePagingData(TeamFilterVm filter)
        {
            var teams = _unit.Team.Load()
                             .WhereIf(filter.SearchString.IsNotEmpty(), x => x.Name.Contains(filter.SearchString))
                             .Where(x => x.Live == LiveStatuses.Active);

            filter.TotalPages = PageCounter.GetTotalPages(teams.Count(), Config.Instance.PageSize);
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