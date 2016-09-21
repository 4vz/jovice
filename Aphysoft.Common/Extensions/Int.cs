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
    }
}
