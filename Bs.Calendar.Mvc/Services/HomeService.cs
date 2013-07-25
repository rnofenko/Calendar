using System.Collections.Generic;
using System.Linq;
using Bs.Calendar.DataAccess;
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
                    unit.User.Save(new User {Email = "rnofenko@gmail.com", Role = Roles.Admin, State = State.Ok,
                                             FirstName = "Roman",
                                             LastName = "Nofenko",
                                             PasswordKeccakHash = "E9447A0B454AA39752445D6DCD2619F25C83F6453BA463C614820239CDC7CAB811F0C75D27776E119836523CF839C90596F2C0B07A45023741C200B51B6944D4"
                    });
                    users = unit.User.Load().ToList();
                }

                return users;
            }
        }
    }
}