using Bs.Calendar.Models;

namespace Bs.Calendar.Mvc.ViewModels.Users
{
    public class UserVm
    {
        public UserVm(User user) 
        {
            UserId = user.Id;
            FullName = string.Format("{0} {1}", user.LastName, user.FirstName);
        }

        public UserVm() 
        {
        }

        public int UserId { get; set; }
        public string FullName { get; set; }
    }
}