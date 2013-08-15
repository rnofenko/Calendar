using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.ViewModels.Users;
using Bs.Calendar.Rules;
using Bs.Calendar.Rules.Emails;

namespace Bs.Calendar.Mvc.Services
{
    public class UserService
    {
        private readonly RepoUnit _unit;
        private readonly ContactService _contactService;
        public int PageSize { get; set; }

        public UserService(RepoUnit unit, ContactService contactService)
        {
            PageSize = 7;
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

            var contacts = _contactService.UpdateContacts(userModel.Contacts);

            var user = new User
            {
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                Email = userModel.Email,
                Role = userModel.Role,
                BirthDate = userModel.BirthDate,
                Contacts = contacts,

                Live = userModel.Live,
                ApproveState = userModel.ApproveState
            };
            _unit.User.Save(user);
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
                sendMsgToUser(userToEdit);
            }

            userToEdit.FirstName = userModel.FirstName;
            userToEdit.LastName = userModel.LastName;
            userToEdit.Email = userModel.Email;
            userToEdit.Role = userModel.Role;
            userToEdit.Live = deleted ? LiveStatuses.Deleted : userToEdit.Live;
            userToEdit.BirthDate = userModel.BirthDate;

            var contacts = _contactService.UpdateContacts(userModel.Contacts);
            userToEdit.Contacts.Clear();
            userToEdit.Contacts = contacts;

            _unit.User.Save(userToEdit);
        }

        private static void sendMsgToUser(User user)
        {
            var sender = Ioc.Resolve<EmailSender>();
            var body = string.Format("Hi, {0}!\nYour account's status is {1} and {2} now.",
                user.FullName,
                user.Live == LiveStatuses.Active ? "active" : "deleted",
                user.ApproveState == ApproveStates.Approved ? "approved" : "not approved");

            sender.Send("Status has been changed", body, user.Email);
        }

        public UsersVm RetreiveList(UserFilterVm filterVm)
        {
            var filter = filterVm.Map();
            var users = _unit.User.Load(filter);
            updatePagingData(filterVm, users);

            return new UsersVm
            {
                Users = users,
                Filter = filterVm
            };
        }

        private void updatePagingData(UserFilterVm filter, IQueryable<User> users)
        {
            filter.TotalPages = PageCounter.GetTotalPages(users.Count(), PageSize);
            filter.Page = PageCounter.GetRangedPage(filter.Page, filter.TotalPages);
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
