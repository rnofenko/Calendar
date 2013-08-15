using System;

namespace Bs.Calendar.Models
{
    [Flags]
    public enum Roles
    {
        Simple = 1,
        Admin = 2,
        All = Simple | Admin
    }
}