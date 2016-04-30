using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aphysoft.Common
{
    /// <summary>
    /// String Extensions
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Reports the zero-based index of the first occurrence of the specified string in this instance.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="values">The strings to seek.</param>
        /// <returns>The zero-based index position of value if that string is found, or -1 if it is not. If value is System.String.Empty, the return value is 0.</returns>
        public static int IndexOf(this string value, params string[] values)
        {
            int rv = -1;
            foreach (string v in values)
            {
                rv = value.IndexOf(v);
                if (rv > -1) break;
            }

            return rv;
        }
    }
}
