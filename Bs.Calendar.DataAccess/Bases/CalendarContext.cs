using System.Configuration;
using System.Data.Entity;
using Bs.Calendar.Models;

namespace Bs.Calendar.DataAccess.Bases
{
    public class CalendarContext : DbContext
    {
        public IDbSet<User> Users { get; set; }
        public IDbSet<Room> Rooms { get; set; }
        public IDbSet<Book> Books { get; set; }
        public IDbSet<Team> Teams { get; set; }
        public IDbSet<BookHistoryItem> BookHistories { get; set; }
        public IDbSet<PersonalEventLink> PersonalEvents { get; set; }
        public IDbSet<TeamEventLink> TeamEvents { get; set; }
        public IDbSet<CalendarLog> CalendarLog { get; set; }
        public IDbSet<EmailOnEventHistory> EmailOnEventHistories { get; set; }
        
        public CalendarContext()
            : base(getConnectionName())
        {
            Configuration.LazyLoadingEnabled = true;
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
