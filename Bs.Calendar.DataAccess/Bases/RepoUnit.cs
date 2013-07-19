using System;

namespace Bs.Calendar.DataAccess.Bases
{
    public class RepoUnit : IDisposable
    {
        private CalendarContext _context;
        private UserRepository _user;
		private RoomRepository  _room;
		private TeamRepository _team;
        
        public UserRepository User
        {
            get { return _user ?? (_user = new UserRepository(getContext())); }
        }

        public RoomRepository Room
        {
            get { return _room ?? (_room = new RoomRepository(getContext())); }
        }

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
