using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bs.Calendar.Models
{
    [Flags]
    public enum ApproveStates
    {
        [Description("not approved")]
        NotApproved = 1,
        [Description("approved")]
        Approved = 2,
        All = 3
    }
}
