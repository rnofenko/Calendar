using Bs.Calendar.Core;

namespace Bs.Calendar.DataAccess
{
    public class DiDataAccess
    {
        public static void Register()
        {
            Resolver.RegisterType<IUserRepository, UserRepository>();
            Resolver.RegisterType<IRoomRepository, RoomRepository>();
        }
    }
}
