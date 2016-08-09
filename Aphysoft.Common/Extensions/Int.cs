using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aphysoft.Common
{
    /// <summary>
    /// Integer Extensions
    /// </summary>
    public static class IntExtensions
    {
        public static int? Nullable(this int value, int ifNull)
        {
            if (value == ifNull) return (int?)null;
            else return new int?(value);
        }
        public static long? Nullable(this long value, long ifNull)
        {
            if (value == ifNull) return (long?)null;
            else return new long?(value);
        }
    }
}
