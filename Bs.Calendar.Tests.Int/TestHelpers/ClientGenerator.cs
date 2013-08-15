using Bs.Calendar.DataAccess;
using Bs.Calendar.Models;

namespace Bs.Calendar.Tests.Int.TestHelpers
{
    public class ClientGenerator
    {
        public void GenerateClients(RepoUnit repoUnit, int count = 1000)
        {
            for (var i = 1; i < count; i++)
            {
                var user = new User
                               {
                                   Email = string.Format("test{0}@gmail.com", i),
                                   FirstName = "Test" + i,
                                   LastName = "Test",

                                   Role = Roles.Simple,
                                   Live = LiveStatuses.Active,
                                   ApproveState = ApproveStates.Approved
                               };

                repoUnit.User.Save(user);
            }
        }
    }
}
