using System;
using Bs.Calendar.DataAccess;
using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Models;
using Bs.Calendar.Mvc.Server;
using NUnit.Framework;

namespace Bs.Calendar.Tests.Int.TestHelpers
{
    public class ClientGenerator
    {
        [Test]
        public void GenerateClients()
        {
            DiMvc.Register();

            const int COUNT = 100;
            
            using (var unit = new RepoUnit())
            {
                for (var i = 1; i < COUNT; i++)
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

                    unit.User.Save(user);
                }
            }
        }
    }
}
