using System.Configuration;
using System.Data.Entity;
using Bs.Calendar.Models;

namespace Bs.Calendar.DataAccess.Bases
{
    public class CalendarContext : DbContext
    {
        public IDbSet<User> Users { get; set; }
        public IDbSet<Room> Rooms { get; set; }

        public CalendarContext()
            : base(getConnectionName())
        {
            Configuration.LazyLoadingEnabled = false;
        }

        protected static string getConnectionName()
        {
            return "name=" + ConfigurationManager.AppSettings["ConnectionName"];
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
