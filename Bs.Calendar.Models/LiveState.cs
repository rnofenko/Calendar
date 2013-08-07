using System;

namespace Bs.Calendar.Models
{
    [Flags]
    public enum LiveState
    {
        Active = 1,
        Deleted = 2,
        NotApproved = 4
    }
}