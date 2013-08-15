using System;

namespace Bs.Calendar.Models
{
    [Flags]
    public enum ApproveStates
    {
        NotApproved = 1,
        Approved = 2,
        All = NotApproved | Approved
    }
}
