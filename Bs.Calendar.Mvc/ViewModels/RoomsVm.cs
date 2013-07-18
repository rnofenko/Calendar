using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bs.Calendar.Mvc.ViewModels
{
    public class RoomsVm
    {
        public IEnumerable<Bs.Calendar.Models.Room> Rooms { get; set; }
    }
}