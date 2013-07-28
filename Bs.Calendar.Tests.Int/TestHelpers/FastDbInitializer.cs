using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Models;

namespace Bs.Calendar.Tests.Int.TestHelpers
{
    public class FastDbInitializer : DropCreateDatabaseIfModelChanges<CalendarContext>
    {
        private readonly int _userCount;

        public FastDbInitializer(int userCount)
        {
            _userCount = userCount;
        }


        protected override void Seed(CalendarContext context) {
            if (context == null)
                context = new CalendarContext();

            context.Configuration.AutoDetectChangesEnabled = false;
            context.Configuration.ValidateOnSaveEnabled = false;
            var randomizer = new Random();

            try
            {
                for (int i = 1; i < _userCount; i++)
                {
                    var user = new User
                    {
                        Email = "test" + randomizer.Next(_userCount) + "@gmail.com",
                        FullName = "Test Test",
                        FirstName = "Test",
                        LastName = "Test",
                        Role = Roles.None,
                        LiveState = LiveState.Active
                    };

                    context = AddToContext(context, user, i, 1000, true);
                }

                context.SaveChanges();
            }
            catch
            {
                //do nothing
            }
            finally
            {
                if (context != null)
                    context.Dispose();
            }         
        }

        private CalendarContext AddToContext(CalendarContext context,
                                         User entity, int count, int commitCount, bool recreateContext)
        {
            context.Users.Add(entity);

            if (count%commitCount == 0)
            {
                context.SaveChanges();
                if (recreateContext)
                {
                    context.Dispose();
                    context = new CalendarContext();
                    context.Configuration.AutoDetectChangesEnabled = false;
                    context.Configuration.ValidateOnSaveEnabled = false;
                }
            }

            return context;
        }
    }
}
