using System;

namespace Bs.Calendar.Core.Utilities
{
    public static class DateTimeUtilities
    {
        public static DateTime FirstDayOfWeek(this DateTime today)
        {
            int offset = DayOfWeek.Monday - today.DayOfWeek;

            return offset != 1 ? today.AddDays(offset) : today.AddDays(-6);
        }

        public static DateTime LastDayOfWeek(this DateTime today)
        {
            int offset = DayOfWeek.Monday - today.DayOfWeek;

            return offset != 1 ? today.AddDays(offset + 6) : today;
        }
    }
}
