using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Aphysoft.Share
{
    /// <summary>
    /// DateTime Extensions
    /// </summary>
    public static class DateTimeExtensions
    {
        public static DateTime ConvertOffset(this DateTime value, TimeSpan offset)
        {
            if (value == DateTime.MinValue) return DateTime.MinValue;
            return value + offset;
        }

        public static DateTime ConvertOffset(this DateTime value, int hourOffset)
        {
            return value.ConvertOffset(new TimeSpan(hourOffset, 0, 0));
        }

        /// <summary>
        /// Converts the given date value to epoch time.
        /// </summary>
        public static long ToEpochTime(this DateTime dateTime)
        {
            var date = dateTime.ToUniversalTime();
            var ticks = date.Ticks - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).Ticks;
            var ts = ticks / TimeSpan.TicksPerSecond;
            return ts;
        }

        /// <summary>
        /// Converts the given epoch time to a <see cref="DateTime"/> with <see cref="DateTimeKind.Utc"/> kind.
        /// </summary>
        public static DateTime ToDateTimeFromEpoch(this long intDate)
        {
            var timeInTicks = intDate * TimeSpan.TicksPerSecond;
            return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddTicks(timeInTicks);
        }

        /// <summary>
        /// Week number according to the ISO-8601 standard. Weeks starting on Monday. The first week of the year is the week that contains that year's first Thursday (='First 4-day week'). The highest week number in a year is either 52 or 53. 2019 has 52 weeks.
        /// </summary>
        public static int GetWeekOfYear(this DateTime time)
        {
            // Seriously cheat.  If its Monday, Tuesday or Wednesday, then it'll 
            // be the same week# as whatever Thursday, Friday or Saturday are,
            // and we always get those right
            DayOfWeek day = CultureInfo.InvariantCulture.Calendar.GetDayOfWeek(time);
            if (day >= DayOfWeek.Monday && day <= DayOfWeek.Wednesday)
            {
                time = time.AddDays(3);
            }

            // Return the week of our adjusted day
            return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(time, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        }


        public static DateTime FirstDateInWeek(this DateTime dt)
        {
            while (dt.DayOfWeek != DayOfWeek.Monday)
                dt = dt.AddDays(-1);
            return new DateTime(dt.Year, dt.Month, dt.Day);
        }

        public static DateTime StartOfTheDay(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day);
        }
    }
}
