using System;
using System.ComponentModel;

namespace Bs.Calendar.Models
{
    [Flags]
    public enum EventType
    {
        Personal = 1, 
        Meeting,
        Company,
        All = Personal | Meeting | Company
    }
}
