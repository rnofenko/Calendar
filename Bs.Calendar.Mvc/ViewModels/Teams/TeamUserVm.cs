using Bs.Calendar.Models;

namespace Bs.Calendar.Mvc.ViewModels.Teams
{
    public class TeamUserVm
    {
        public TeamUserVm(User user) 
        {
            UserId = user.Id;
            Name = string.Format("{0} {1}.", user.LastName, user.FirstName[0]);
        }

        public TeamUserVm() 
        {
        }

        public int UserId { get; set; }
        public string Name { get; set; }
    }
}