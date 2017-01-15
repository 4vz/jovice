using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aphysoft.Share
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
        
        public static int InOf(this string value, params string[] values)
        {
            int rv = -1;
            int ix = -1;
            foreach (string v in values)
            {
                ix++;
                if (value == v)
                {
                    rv = ix;
                    break;
                }
            }
            return rv;
        }

        //public static int In(this string value, params string[])

        public static bool StartsWith(this string value, params string[] values)
        {
            bool ret = false;
            foreach (string v in values)
            {
                if (value.StartsWith(v)) { ret = true; break; }
            }
            return ret;
        }

        private static char[] TrimCharacters(char[] except)
        {
            List<char> trimChars = new List<char>();
            for (int i = 0; i < 255; i++) trimChars.Add((char)i);
            foreach (char c in except) trimChars.Remove(c);

            return trimChars.ToArray();
        }

        /// <summary>
        /// Removes all leading occurrences except of a set of characters especified in an array from the current System.String object.
        /// </summary>
        public static string TrimStartExcept(this string value, params char[] trimExceptChars)
        {
            if (trimExceptChars == null) return value.TrimStart(null);
            return value.TrimStart(TrimCharacters(trimExceptChars));
        }

        /// <summary>
        /// Removes all trailing occurrences except of a set of characters specified in an array from the current System.String object.
        /// </summary>
        public static string TrimEndExcept(this string value, params char[] trimExceptChars)
        {
            if (trimExceptChars == null) return value.TrimEnd(null);
            return value.TrimEnd(TrimCharacters(trimExceptChars));
        }

        /// <summary>
        /// Removes all leading and trailing occurrences of a set of characters specified in an array from the current System.String object.
        /// </summary>
        public static string TrimExcept(this string value, params char[] trimExceptChars)
        {
            if (trimExceptChars == null) return value.Trim(null);
            return value.Trim(TrimCharacters(trimExceptChars));
        }

        public static string ToNull(this string value, string nullIf)
        {
            return value == nullIf ? null : value;
        }






        public static string[] RemoveDuplicatedEntries(this string[] values)
        {
            if (values == null) return null;

            List<string> listValues = new List<string>();

            foreach (string value in values)
            {
                if (!listValues.Contains(value)) listValues.Add(value);
            }

            return listValues.ToArray();
        }

    }
}
