using System;

namespace Bs.Calendar.Models
{
    [Flags]
    public enum LogTypes
    {
        Info = 1,
        Warning = 2,
        Error = 4,
        All = Info | Warning | Error
    }
}