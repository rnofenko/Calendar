using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Bs.Calendar.DataAccess;
using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.ViewModels;

namespace Bs.Calendar.Mvc.Services
{
    public class TeamService
    {
        private readonly RepoUnit _unit;
        public int PageSize { get; set; }

        public TeamService(RepoUnit repoUnit)
        {
            _unit = repoUnit;
            PageSize = 7;
        }

        public Team GetTeam(int teamId) {
            var team = _unit.Team.Get(teamId);
            return team;
        }

        public IEnumerable<Team> GetAllTeams() 
        {
            return _unit.Team.Load().ToList();
        }

        public void SaveTeam(TeamEditVm teamModel) 
        {
            if (_unit.Team.Get(t => t.Name.Equals(teamModel.Name, StringComparison.OrdinalIgnoreCase)) != null) 
            {
                throw new WarningException(string.Format("Team with name {0} already exists", teamModel.Name));
            }

            _unit.Team.Save(new Team
            {
                Name = teamModel.Name,
                Description = teamModel.Description
            });
        }

        public void DeleteTeam(int id) 
        {
            _unit.Team.Delete(_unit.Team.Get(id));
        }

        public void EditTeam(TeamEditVm teamModel) 
        {
            var teamToEdit = GetTeam(teamModel.TeamId);
            var comparisonType = StringComparison.OrdinalIgnoreCase;

            if (!teamToEdit.Name.Equals(teamModel.Name, comparisonType) &&
                _unit.Team.Get(t => t.Name.Equals(teamModel.Name, comparisonType)) != null) 
            {
                throw new WarningException(string.Format("Team with name {0} already exists", teamModel.Name));
            }

            teamToEdit.Name = teamModel.Name;
            teamToEdit.Description = teamModel.Description;

            _unit.Team.Save(teamToEdit);
        }

        public TeamsVm RetreiveList(PagingVm pagingVm)
        {
            var teams = _unit.Team.Load();

            teams = teams.WhereIf(!string.IsNullOrEmpty(pagingVm.SearchStr), 
                        team => team.Name.ToLower().Contains(pagingVm.SearchStr.ToLower()));

            teams = sortByStr(teams, pagingVm.SortByStr);

            var totalPages = getTotalPages(teams.Count(), PageSize);
            var currentPage = getRangedPage(pagingVm.Page, totalPages);

            return new TeamsVm 
            {
                Teams = teams.Skip((currentPage - 1) * PageSize).Take(PageSize).ToList(),
                PagingVm = new PagingVm(pagingVm.SearchStr, pagingVm.SortByStr, totalPages, currentPage)
            };
        }

        private IQueryable<Team> sortByStr(IQueryable<Team> teams, string sortByStr) 
        {    
            teams = teams.OrderByIf(!string.IsNullOrEmpty(sortByStr),
                        team => team.Name);

            teams = teams.OrderByIf(string.IsNullOrEmpty(sortByStr),
                        team => team.Id);
            return teams;
        }

        private int getTotalPages(int count, int pageSize)
        {
            return (int) Math.Ceiling((decimal) count/pageSize);
        }

        private int getRangedPage(int page, int totalPages)
        {
            return page <= 1 ? 1 : page > totalPages ? totalPages : page;
        }
    }
}