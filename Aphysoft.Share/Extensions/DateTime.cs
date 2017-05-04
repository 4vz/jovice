using System;
using System.Collections.Generic;
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
    }
}
