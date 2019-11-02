using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aphysoft.Share
{
    /// <summary>
    /// Integer Extensions
    /// </summary>
    public static class IntExtensions
    {
        public static int? Nullable(this int value, int returnNullIf)
        {
            if (value == returnNullIf) return (int?)null;
            else return new int?(value);
        }
        public static long? Nullable(this long value, long returnNullIf)
        {
            if (value == returnNullIf) return (long?)null;
            else return new long?(value);
        }
        
        public static bool Between(this int value, int min, int max)
        {
            if (min > max) return false;
            else if (min <= value && value <= max) return true;
            else return false;
        }

        public static string PluralCase(this int value, string singular, string plural)
        {
            if (value == 0 || value == 1) return singular;
            else return plural;
        }

        /// <summary>
        /// s or not s
        /// </summary>
        public static string PluralCase(this int value)
        {
            return value.PluralCase("", "s");
        }

        /// <summary>
        /// es or not es
        /// </summary>
        public static string PluralCase2(this int value)
        {
            return value.PluralCase("", "es");
        }
    }
}
