﻿using Bs.Calendar.Models;

namespace Bs.Calendar.Mvc.ViewModels
{
    public class PagingVm
    {
        public PagingVm()
        {
            Page = 1;
            TotalPages = 1;
            ShowNotApproved = true;
        }

        public PagingVm(string searchStr, string sortByStr, int totalPages, int page = 1,
            bool showDeleted = false, bool showAdmins = false, bool showNotApproved = true)
        {
            SearchStr = searchStr;
            SortByStr = sortByStr;
            Page = page;
            TotalPages = totalPages;

            ShowDeleted = showDeleted;
            ShowAdmins = showAdmins;
            ShowNotApproved = showNotApproved;
        }

        public PagingVm(PagingVm pagingVm)
            : this(pagingVm.SearchStr, pagingVm.SortByStr, pagingVm.TotalPages, pagingVm.Page, pagingVm.ShowDeleted, pagingVm.ShowAdmins, pagingVm.ShowNotApproved)
        {
        }

        public Roles RolesFilter
        { 
            get
            {
                var role = Roles.Simple;
                return ShowAdmins ? role | Roles.Admin : role;
            }
        }

        public LiveStatuses LiveStatusFilter
        {   
            get
            {
                var liveStatus = LiveStatuses.Active;

                return ShowDeleted ? liveStatus | LiveStatuses.Deleted : liveStatus;
            }
        }

        public ApproveStates ApproveStateFilter
        {
            get
            {
                var approveState = ApproveStates.Approved;

                return ShowNotApproved ? approveState | ApproveStates.NotApproved : approveState;
            }
        }

        public string SearchStr { get; set; }

        public string SortByStr { get; set; }

        public int Page { get; set; }

        public int TotalPages { get; set; }

        public bool ShowAdmins { get; set; }
        public bool ShowNotApproved { get; set; }
        public bool ShowDeleted { get; set; }
    }
}