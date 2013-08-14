using System;
using System.ComponentModel;

namespace Bs.Calendar.Models
{
    [Flags]
    public enum Roles
    {
        [Description("simple")]
        Simple = 1,
        [Description("admin")]
        Admin = 2,
        All = int.MaxValue
    }
}