using System;
using System.ComponentModel;

namespace Bs.Calendar.Models
{
    [Flags]
    public enum EventType
    {
        Personal = 1, 
        Meeting = 2,
        Company = 4,
        All = Personal | Meeting | Company
    }
}
