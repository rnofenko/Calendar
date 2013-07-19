using System;

namespace Bs.Calendar.DataAccess.Bases
{
    public class RepoUnit : IDisposable
    {
        private CalendarContext _context;
        private UserRepository _user;
        private TeamRepository _team;

        public UserRepository User
        {
            get { return _user ?? (_user = new UserRepository(GetContext())); }
        }

        public TeamRepository Team
        {
            get { return _team ?? (_team = new TeamRepository(GetContext())); }
        }

        private CalendarContext GetContext()
        {
            return _context ?? (_context = new CalendarContext());
        }

        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
            }
        }
    }
}
