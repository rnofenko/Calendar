using System;

namespace Bs.Calendar.DataAccess.Bases
{
    public class RepoTeam : IDisposable
    {
        private CalendarContext _context;
        private TeamRepository _team;

        public TeamRepository Team
        {
            get { return _team ?? (_team = new TeamRepository(getContext())); }
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
