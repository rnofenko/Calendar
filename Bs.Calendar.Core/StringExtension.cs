using System;

namespace Bs.Calendar.Core
{
    public static class StringExtension
    {
        public static bool IsEmpty(this string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static bool IsNotEmpty(this string str)
        {
            return !IsEmpty(str);
        }

        public static bool Contains(this string str, string strToCheck, StringComparison comparisonType)
        {
            return str.IndexOf(strToCheck, comparisonType) > -1;
        }
    }
}
