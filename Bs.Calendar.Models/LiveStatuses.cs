using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bs.Calendar.Models
{
    [Flags]
    public enum LiveStatuses
    {
        [Description("deleted")]
        Deleted = 1,
        [Description("active")]
        Active = 2,
        All = int.MaxValue
    }
}
