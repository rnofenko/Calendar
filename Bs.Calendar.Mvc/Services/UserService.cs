using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.ViewModels;
using Bs.Calendar.Rules;
using Bs.Calendar.Rules.Emails;

namespace Bs.Calendar.Mvc.Services
{
    public class UserService
    {
        private readonly RepoUnit _unit;
        private readonly ContactService _contactService;

        public UserService(RepoUnit unit, ContactService contactService)
        {
            _contactService = contactService;
            _unit = unit;
        }

        public User GetUser(int userId)
        {
            var user = _unit.User.Get(userId);
            return user;
        }

        public IEnumerable<User> GetAllUsers()
        {
            return _unit.User.Load().ToList();
        }

        public void SaveUser(UserEditVm userModel)
        {
            if (!EmailSender.IsValidEmailAddress(userModel.Email))
            {
                throw new WarningException(string.Format("{0} - is not valid email address", userModel.Email));
            }
            if (_unit.User.Get(u => u.Email == userModel.Email) != null)
            {
                throw new WarningException(string.Format("User with email {0} already exists", userModel.Email));
            }

            _contactService.UpdateContacts(userModel.Contacts);
            _unit.User.Save(userModel.Map());
        }

        public void UpdateUserState(int userModelId, ApproveStates approveState = ApproveStates.NotApproved, LiveStatuses liveState = LiveStatuses.Active)
        {
            var user = _unit.User.Get(userModelId);
            
            user.Live = liveState;
            user.ApproveState = approveState;

            _unit.User.Save(user);
        }

        public void DeleteUser(int id)
        {
            _unit.User.Delete(_unit.User.Get(id));
        }

        public bool CreateUser(UserEditVm userEditVm)
        {
            var dbUser = _unit.User.Get(u => u.Email == userEditVm.Email);

            if (dbUser != null)
            {
                userEditVm.FirstName = dbUser.FirstName;
                userEditVm.LastName = dbUser.LastName;
                userEditVm.Role = dbUser.Role;

                if (dbUser.Live == LiveStatuses.Deleted)
                {
                    userEditVm.Live = dbUser.Live;
                }

                return false;
            }

            userEditVm.ApproveState = ApproveStates.NotApproved;
            userEditVm.Live = LiveStatuses.Active;

            SaveUser(userEditVm);

            dbUser = _unit.User.Get(u => u.Email == userEditVm.Email);
            dbUser.PasswordHash = userEditVm.Email;

            _unit.User.Save(dbUser);

            return true;
        }

        public void EditUser(UserEditVm userModel, bool deleted)
        {
            var userToEdit = GetUser(userModel.UserId);

            if (!EmailSender.IsValidEmailAddress(userModel.Email))
            {
                throw new WarningException(string.Format("{0} - is not valid email address", userModel.Email));
            }
            if (userToEdit.Email != userModel.Email && _unit.User.Get(u => u.Email == userModel.Email) != null)
            {
                throw new WarningException(string.Format("User with email {0} already exists", userModel.Email));
            }
            if (userModel.Email != userToEdit.Email)
            {
                SendMsgToUser(userToEdit);
            }

            _contactService.UpdateContacts(userModel.Contacts);

            userToEdit = userModel.Map();
            userToEdit.Live = deleted ? LiveStatuses.Deleted : userToEdit.Live;

            _unit.User.Save(userToEdit);
        }

        private static void SendMsgToUser(User user)
        {
            var sender = Ioc.Resolve<EmailSender>();
            var body = string.Format("Hi, {0}!\nYour account's status is {1} and {2} now.", user.FullName, user.Live.GetDescription(), user.ApproveState.GetDescription());

            sender.Send("Status has been changed", body, user.Email);
        }

        public UsersVm RetrieveList(UserFilterVm filterVm)
        {
            return new UsersVm(_unit.User.Load(filterVm.Map()), filterVm);
        }

        public UsersVm RetreiveList(PagingVm pagingVm)
        {
            var users = _unit.User.Load();

            //users = searchByStr(users, pagingVm.SearchStr);
            //users = searchByRoleAndState(users, pagingVm.RolesFilter, pagingVm.LiveStatusFilter, pagingVm.ApproveStateFilter,
            //                             pagingVm.ShowUnknownRole, pagingVm.ShowUnknownLiveStatus, pagingVm.ShowUnknownApproveState);

            //users = sortByStr(users, pagingVm.SortByStr);

            int pageSize = Config.Instance.PageSize;
            pagingVm = updatePagingVm(pagingVm, users, pageSize);

            return new UsersVm
            {
                Users = users.Skip((pagingVm.Page - 1) * pageSize).Take(pageSize).ToList(),
                PagingVm = pagingVm
            };
        }

        private PagingVm updatePagingVm(PagingVm pagingVm, IQueryable<User> users, int pageSize)
        {
            var newPagingVm = new PagingVm(pagingVm);

            newPagingVm.TotalPages = PageCounter.GetTotalPages(users.Count(), pageSize);
            newPagingVm.Page = PageCounter.GetRangedPage(pagingVm.Page, newPagingVm.TotalPages);

            return newPagingVm;
        }

        public void RecoverUser(string email)
        {
            var userToRecover = _unit.User.Get(u => u.Email == email);

            userToRecover.Live = LiveStatuses.Active;
            userToRecover.ApproveState = ApproveStates.NotApproved;

            _unit.User.Save(userToRecover);
        }
    }
}
