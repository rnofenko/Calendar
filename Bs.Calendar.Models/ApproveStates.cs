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

    public static class ApproveStatesExtension
    {
        public static bool IsNotAllOrNull(this ApproveStates states)
        {
            return states > 0 && states != ApproveStates.All;
        }
    }
}
