using System.Collections.Generic;
using System.Linq;
using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Models;

namespace Bs.Calendar.Mvc.Services
{
    public class HomeService
    {
        public IEnumerable<User> LoadUsers()
        {
            using (var unit = new RepoUnit())
            {
                var users = unit.User.Load().ToList();

                if (!users.Any())
                {
                    unit.User.Save(new User {Email = "rnofenko@gmail.com", Role = Roles.Admin, 
                        FristName = "Roman", LastName = "Nofenko"});
                    users = unit.User.Load().ToList();
                }

                return users;
            }
        }
    }
}