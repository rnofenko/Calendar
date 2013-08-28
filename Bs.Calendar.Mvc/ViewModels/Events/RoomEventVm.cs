using System;
using System.Collections.Generic;
using Bs.Calendar.Models;

namespace Bs.Calendar.Mvc.ViewModels.Events
{
    public class RoomEventVm
    {
        public RoomEventVm()
        {
        }

        public RoomEventVm(Room room)
        {
            Room = room;
        }

        public Room Room { get; set; }

        public List<Tuple<DateTime, DateTime>> TimePeriod { get; set; }
    }
}