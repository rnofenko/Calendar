using System;
using System.Linq;
using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Models;

namespace Bs.Calendar.DataAccess
{
    public interface IRoomRepository : IRepository<Room>
    {
        IQueryable<Room> Load(RoomFilter filter);
        event Action<IQueryable<Room>> OnBeforePaging;
    }
}