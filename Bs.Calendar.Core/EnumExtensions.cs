using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Bs.Calendar.Core
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
