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
                    unit.User.Save(new User {Email = "rnofenko@gmail.com", Role = Roles.Admin, 
                        FirstName = "Roman", LastName = "Nofenko", PasswordKeccakHash = "50071663808AB77374A5A26BDE4D48379442BF7755A7A3A5281EFF7CCFA5DAF7DE59F1145EDCDF39AC525FA2DFAE64088A097952033A70A378FD1E29A45226F7",PasswordMd5Hash = "C10ED385C509CC2C7BA59B2EB4C4947A"});
                    users = unit.User.Load().ToList();
                }

                return users;
            }
        }
    }
}