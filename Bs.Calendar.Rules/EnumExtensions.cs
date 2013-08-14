using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Bs.Calendar.Rules
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            return value.GetDescription(value.ToString());
        }

        public static string GetDescription(this Enum value, string noDescriptionReturn)
        {
            var description = value.GetType().GetMember(value.ToString()).First().GetCustomAttribute<DescriptionAttribute>(false);

            return description == null ? noDescriptionReturn : description.Description;
        }
    }
}
