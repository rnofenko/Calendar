using System;
using System.Linq;
using Bs.Calendar.Core;
using Bs.Calendar.DataAccess.Bases;
using Bs.Calendar.Models;

namespace Bs.Calendar.DataAccess
{
    public class RoomRepository : BaseRepository<Room>, IRoomRepository
    {
        public IQueryable<Room> Load(RoomFilter filter)
        {
            IQueryable<Room> query = Load()
                .WhereIf(filter.SearchString.IsNotEmpty(), room => room.Name.Contains(filter.SearchString))
                .OrderByExpression(filter.SortByField);

            if(OnBeforePaging != null)
            {
                OnBeforePaging.Invoke(query);
            }

            query = query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize);

            return query;
        }

        public event Action<IQueryable<Room>> OnBeforePaging;
    }
}
