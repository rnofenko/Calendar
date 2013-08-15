using System;
using System.ComponentModel;

namespace Bs.Calendar.Models
{
    [Flags]
    public enum ApproveStates
    {
        [Description("not approved")]
        NotApproved = 1,
        [Description("approve")]
        Approved = 2,
        All = NotApproved | Approved
    }
}
