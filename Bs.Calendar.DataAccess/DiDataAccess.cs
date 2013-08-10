using Bs.Calendar.Core;

namespace Bs.Calendar.DataAccess
{
    public class DiDataAccess
    {
        public static void Register()
        {
            Ioc.RegisterType<IUserRepository, UserRepository>();
            Ioc.RegisterType<IRoomRepository, RoomRepository>();
            Ioc.RegisterType<ITeamRepository, TeamRepository>();
            Ioc.RegisterType<IBookRepository, BookRepository>();
            Ioc.RegisterType<IBookHistoryRepository, BookHistoryRepository>();
        }
    }
}
