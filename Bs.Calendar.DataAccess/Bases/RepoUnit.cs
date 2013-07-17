using System;

namespace Bs.Calendar.DataAccess.Bases
{
    public class RepoUnit : IDisposable
    {
        private CalendarContext _context;
        private UserRepository _user;

        public UserRepository User
        {
            get { return _user ?? (_user = new UserRepository(getContext())); }
        }

        private CalendarContext getContext()
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
