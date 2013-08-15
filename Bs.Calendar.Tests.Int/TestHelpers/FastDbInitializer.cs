using System;
using System.Data.Entity;
using System.Diagnostics;
using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Models;

namespace Bs.Calendar.Tests.Int.TestHelpers
{
    public class FastDbInitializer : DropCreateDatabaseAlways<CalendarContext>
    {
        private readonly int UserCount;

        public FastDbInitializer(int userCount)
        {
            UserCount = userCount;
        }


        protected override void Seed(CalendarContext context) {

            if (context == null)
                context = new CalendarContext();

            context.Configuration.AutoDetectChangesEnabled = false;
            context.Configuration.ValidateOnSaveEnabled = false;

            var randomizer = new Random();

            try
            {
                for (int i = 1; i < UserCount; i++)
                {
                    var user = new User
                    {
                        Email = string.Format("test{0}@gmail.com", randomizer.Next(UserCount)),
                        FullName = "Test Test",
                        FirstName = "Test",
                        LastName = "Test",
                        Role = Roles.Simple,

                        Live = LiveStatuses.Active,
                        ApproveState = ApproveStates.Approved
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
                //if (context != null)
                //    context.Dispose();
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
