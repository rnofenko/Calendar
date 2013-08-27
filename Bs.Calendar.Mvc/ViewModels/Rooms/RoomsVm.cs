using System.Collections.Generic;
using Bs.Calendar.Models;

namespace Bs.Calendar.Mvc.ViewModels.Rooms
{
    public class RoomsVm
    {
        public IEnumerable<Room> Rooms { get; set; }
        public RoomFilterVm Filter { get; set; }
    }
}