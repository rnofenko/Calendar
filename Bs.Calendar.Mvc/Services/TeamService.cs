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
            if (_unit.Team.Get(t => t.Name == teamModel.Name) != null) 
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
            var oldName = GetTeam(teamModel.TeamId).Name;
            var comparisonType = StringComparison.InvariantCultureIgnoreCase;

            if (!oldName.Equals(teamModel.Name, comparisonType) &&
                _unit.Team.Get(t => t.Name.Equals(teamModel.Name, comparisonType)) != null) 
            {
                throw new WarningException(string.Format("Team with name {0} already exists", teamModel.Name));
            }

            _unit.Team.Save(new Team {
                Id = teamModel.TeamId,
                Name = teamModel.Name,
                Description = teamModel.Description
            });
        }

        public TeamsVm RetreiveList(PagingVm pagingVm)
        {
            var teams = _unit.Team.Load();

            if (!string.IsNullOrEmpty(pagingVm.SearchStr)) 
            {
                teams = Find(teams, pagingVm.SearchStr);
            }
            if (!string.IsNullOrEmpty(pagingVm.SortByStr))
            {
                teams = teams.OrderBy(team => team.Name);
            } 
            else 
            {
                teams = teams.OrderBy(team => team.Id);
            }
            
            var totalPages = (int)Math.Ceiling((decimal)teams.Count() / PageSize);
            var currentPage = pagingVm.Page <= 1 ? 1 : pagingVm.Page > totalPages ? totalPages : pagingVm.Page;

            return new TeamsVm 
            {
                Teams = teams.Skip((currentPage - 1) * PageSize).Take(PageSize).ToList(),
                PagingVm = new PagingVm(pagingVm.SearchStr, pagingVm.SortByStr, totalPages, currentPage)
            };
        }

        private IQueryable<Team> Find(IQueryable<Team> teams, string searchStr)
        {
            var resultTeams = teams.Where(team => team.Name.Equals(searchStr, StringComparison.InvariantCultureIgnoreCase));
            return resultTeams;
        }
    }
}