using Bs.Calendar.Models;

namespace Bs.Calendar.Mvc.ViewModels.Teams
{
    public class TeamUserVm
    {
        public TeamUserVm(User user) 
        {
            UserId = user.Id;
            FullName = string.Format("{0} {1}", user.LastName, user.FirstName);
        }

        public TeamUserVm() 
        {
        }

        public int UserId { get; set; }
        public string FullName { get; set; }
    }
}