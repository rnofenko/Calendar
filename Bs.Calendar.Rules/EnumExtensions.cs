using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Bs.Calendar.Rules
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum enumValue)
        {
            return enumValue.GetDescription(enumValue.ToString());
        }

        public static string GetDescription(this Enum enumValue, string returnIfNoDescription)
        {
            var description = enumValue.GetType().GetMember(enumValue.ToString()).First().GetCustomAttribute<DescriptionAttribute>(false);

            return description == null ? returnIfNoDescription : description.Description;
        }
    }
}
