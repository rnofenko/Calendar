using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bs.Calendar.Models
{
    [Flags]
    public enum EventStatus
    {
        Involved = 1,
        NotInvolved,
        All = Involved | NotInvolved
    }
}
