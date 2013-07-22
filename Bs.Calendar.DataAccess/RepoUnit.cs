using System;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess.Bases;

namespace Bs.Calendar.DataAccess
{
    public class RepoUnit : IDisposable
    {
        private CalendarContext _context;
        private IUserRepository _user;
		private IRoomRepository _room;
		private TeamRepository _team;

        public IUserRepository User
        {
            get { return _user ?? (_user = getRepository<IUserRepository>()); }
        }

        public IRoomRepository Room
        {
            get { return _room ?? (_room = getRepository<IRoomRepository>()); }
        }

		public TeamRepository Team
        {
            get { return _team ?? (_team = getRepository<TeamRepository>()); }
        }    
		
        private CalendarContext getContext()
		{
            return _context ?? (_context = new CalendarContext());
        }

        private T getRepository<T>() where T : IContexable
        {
            var repository = Resolver.Resolve<T>();
            repository.SetContext(getContext());

            return repository;
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
