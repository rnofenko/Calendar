using System;

namespace Bs.Calendar.Models
{
    [Flags]
    public enum Roles
    {
        None = 0,
        Simple = 1,
        Admin = 2
    }
}