using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Models;

namespace Bs.Calendar.DataAccess
{
    public class RoomRepository : BaseRepository<Room>
    {
        public RoomRepository(CalendarContext context)
            : base(context)
        {

        }
    }
}
