using Bs.Calendar.Models;

namespace Bs.Calendar.Mvc.ViewModels.Rooms
{
    public class RoomFilterVm
    {
        public RoomFilter Map()
        {
            return new RoomFilter
                       {
                           SearchString = SearchString
                       };
        }

        public string SearchString { get; set; }
    }
}