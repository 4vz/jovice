using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aphysoft.Share
{
    public static class Characters
    {
        public static readonly char[] Alphanumeric = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        public static readonly char[] Numeric = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        public static readonly char[] WhiteSpace = { ' ' };
    }

    public static class StringExtensions
    {
        public static string[] Split(this string value, string[] separator, int minimumSize)
        {
            // AFIS_ICHAICHA_AIMEE_ARVEE
            // separator: _
            // minimumSize: 6
            // AFIS_ICHAICHA, AIMEE_ARVEE
            // minimumSize: 5
            // AFIS_ICHAICHA, AIMEE, ARVEE

            List<string> results = new List<string>();

            StringBuilder buffer = new StringBuilder();

            foreach (char c in value)
            {
                buffer.Append(c);
                string currentBuffer = buffer.ToString();
                foreach (string sep in separator)
                {
                    if (currentBuffer.EndsWith(sep))
                    {
                        int splitLength = currentBuffer.Length - sep.Length;

                        if (splitLength >= minimumSize)
                        {
                            results.Add(currentBuffer.Remove(splitLength));
                            buffer.Clear();
                            break;
                        }
                    }
                }            }

            results.Add(buffer.ToString());

            return results.ToArray();

        }

        public static string[] Split(this string value, char[] separator, int minimumSize)
        {
            List<string> separators = new List<string>();

            foreach (char sep in separator)
            {
                separators.Add(sep.ToString());
            }

            return value.Split(separators.ToArray(), minimumSize);
        }
        
        public static int IndexOf(this string value, params string[] values)
        {
            if (string.IsNullOrEmpty(value)) return -1;

            int rv = -1;
            foreach (string v in values)
            {
                rv = value.IndexOf(v);
                if (rv > -1) break;
            }
            return rv;
        }
        
        public static int ArgumentIndexOf(this string value, params string[] values)
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

        public static bool IsOnlyContainsCharacters(this string value, char[] characters)
        {
            if (characters == null) return true;

            bool pass = true;

            foreach (char c in value)
            {
                bool found = false;

                foreach (char s in characters)
                {                    
                    if (c == s)
                    {
                        found = true;
                        break;
                    }
                }
                if (found == false)
                {
                    pass = false;
                    break;
                }
            }

            return pass;
        }

        public static string Random(this string[] values)
        {
            if (values == null) return null;
            else
            {
                if (values.Length == 0) return null;
                else return values[Aphysoft.Share.Rnd.Int(values.Length)];               
            }
        }

        public static bool StartsWith(this string value, params string[] values)
        {
            bool ret = false;
            foreach (string v in values)
            {
                if (value.StartsWith(v)) { ret = true; break; }
            }
            return ret;
        }

        public static bool Bool(this string value)
        {
            if (value == "true") return true;
            else return false;
        }

        private static char[] ExceptTheseCharacters(char[] except)
        {
            List<char> trimChars = new List<char>();
            for (int i = 0; i < 255; i++) trimChars.Add((char)i);
            foreach (char c in except) trimChars.Remove(c);

            return trimChars.ToArray();
        }

        public static string TrimStartExcept(this string value, params char[] characters)
        {
            if (characters == null) return value.TrimStart(null);
            return value.TrimStart(ExceptTheseCharacters(characters));
        }

        public static string TrimEndExcept(this string value, params char[] characters)
        {
            if (characters == null) return value.TrimEnd(null);
            return value.TrimEnd(ExceptTheseCharacters(characters));
        }

        public static string TrimExcept(this string value, params char[] characters)
        {
            if (characters == null) return value.Trim(null);
            return value.Trim(ExceptTheseCharacters(characters));
        }

        public static string ToNull(this string value, string nullIf)
        {
            return value == nullIf ? null : value;
        }

        public static string NullIfEmpty(this string value)
        {
            return value.ToNull("");
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

        public static string[] Merge(this string[] values, params string[][] adds)
        {
            List<string> nvals = new List<string>(values);
            foreach (string[] add in adds)
            {
                nvals.AddRange(add);
            }
            return nvals.ToArray();
        }

        public static unsafe string RemoveCharactersExcept(this string value, params char[] characters)
        {
            char[] charactersToRemove = ExceptTheseCharacters(characters);

            int len = value.Length;
            char* newChars = stackalloc char[len];
            char* currentChar = newChars;

            for (int i = 0; i < len; ++i)
            {
                char c = value[i];

                bool cont = false;

                foreach (char rc in charactersToRemove)
                {
                    if (rc == c)
                    {
                        cont = true;
                        break;
                    }
                }

                if (cont)
                    continue;
                else
                    *currentChar++ = c;

            }
            return new string(newChars, 0, (int)(currentChar - newChars));
        }

        public static string RemoveCharactersExcept(this string value, params char[][] characters)
        {
            List<char> cl = new List<char>();

            foreach (char[] cc in characters) cl.AddRange(cc);

            return value.RemoveCharactersExcept(cl.ToArray());
        }

        public static string ReplaceWithWhiteSpace(this string value, int index, int length)
        {
            return (new StringBuilder(value).ReplaceWithWhiteSpace(index, length)).ToString();
        }

        public static string NormalizeWhiteSpace(this string value)
        {
            int len = value.Length,
                index = 0,
                i = 0;
            var src = value.ToCharArray();
            bool skip = false;
            char ch;
            for (; i < len; i++)
            {
                ch = src[i];
                switch (ch)
                {
                    case '\u0020':
                    case '\u00A0':
                    case '\u1680':
                    case '\u2000':
                    case '\u2001':
                    case '\u2002':
                    case '\u2003':
                    case '\u2004':
                    case '\u2005':
                    case '\u2006':
                    case '\u2007':
                    case '\u2008':
                    case '\u2009':
                    case '\u200A':
                    case '\u202F':
                    case '\u205F':
                    case '\u3000':
                    case '\u2028':
                    case '\u2029':
                    case '\u0009':
                    case '\u000A':
                    case '\u000B':
                    case '\u000C':
                    case '\u000D':
                    case '\u0085':
                        if (skip) continue;
                        src[index++] = ch;
                        skip = true;
                        continue;
                    default:
                        skip = false;
                        src[index++] = ch;
                        continue;
                }
            }

            return new string(src, 0, index);
        }

        public static string Remove(this string value, params string[] remove)
        {
            foreach (string r in remove) value = value.Replace(r, "");

            return value;
        }

        public static string Remove(this string value, params string[][] remove)
        {
            List<string> rl = new List<string>();

            foreach (string[] rs in remove) rl.AddRange(rs);

            return value.Remove(rl.ToArray());
        }

        
        public static bool IsIPv4Address(this string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return false;
            }

            string[] splitValues = value.Split('.');
            if (splitValues.Length != 4)
            {
                return false;
            }

            return splitValues.All(r => byte.TryParse(r, out byte tempForParsing));
        }
    }

    public static class StringBuilderExtensions
    {
        public static StringBuilder RemoveCharactersExcept(this StringBuilder value, params char[] characters)
        {
            string removed = value.ToString().RemoveCharactersExcept(characters);
            value.Clear();
            value.Append(removed);
            return value;
        }

        public static StringBuilder RemoveCharactersExcept(this StringBuilder value, params char[][] characters)
        {
            string removed = value.ToString().RemoveCharactersExcept(characters);
            value.Clear();
            value.Append(removed);
            return value;
        }

        public static StringBuilder ReplaceWithWhiteSpace(this StringBuilder value, int index, int length)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < length; i++) sb.Append(' ');

            value.Remove(index, length);
            value.Insert(index, sb.ToString());

            return value;
        }

        public static StringBuilder NormalizeWhiteSpace(this StringBuilder value)
        {
            string normalized = value.ToString().NormalizeWhiteSpace();
            value.Clear();
            value.Append(normalized);
            return value;
        }

        public static StringBuilder Remove(this StringBuilder value, params string[] remove)
        {
            string removed = value.ToString().Remove(remove);
            value.Clear();
            value.Append(removed);
            return value;
        }

        public static StringBuilder Remove(this StringBuilder value, params string[][] remove)
        {
            string removed = value.ToString().Remove(remove);
            value.Clear();
            value.Append(removed);
            return value;
        }
    }
}
