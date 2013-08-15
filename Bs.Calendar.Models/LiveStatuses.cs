using System;
using System.ComponentModel;

namespace Bs.Calendar.Models
{
    [Flags]
    public enum LiveStatuses
    {
        [Description("deleted")]
        Deleted = 1,
        [Description("active")]
        Active = 2,
        All = Deleted | Active
    }
}
