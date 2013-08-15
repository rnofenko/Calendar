using System;

namespace Bs.Calendar.Models
{
    [Flags]
    public enum LiveStatuses
    {
        Deleted = 1,
        Active = 2,
        All = Deleted | Active
    }
}
