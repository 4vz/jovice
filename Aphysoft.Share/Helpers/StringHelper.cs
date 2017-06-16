using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Drawing;
using System.Text;
using System.Collections;

namespace Aphysoft.Share
{
    public static class StringSplitTypes
    {
        public static readonly char[] Space = new char[] { ' ' };
        public static readonly char[] Comma = new char[] { ',' };
        public static readonly char[] Dot = new char[] { '.' };
        public static readonly char[] Underscore = new char[] { '_' };
        public static readonly char[] Slash = new char[] { '/' };
    }


    /// <summary>
    /// Provides a set of static methods and properties that provide string manipulation mechanism.
    /// </summary>
    public static class StringHelper
    {
        public static int Count(string source, char find)
        {
            int count = 0;
            foreach (char c in source)
                if (c == find) count++;

            return count;
        }

        public static string EscapeFormat(string str)
        {
            return str.Replace("{", "{{").Replace("}", "}}");
        }

        public static bool Find(string source, params string[] args)
        {
            int argIndex, sourceIndex, length;
            return Find(source, out argIndex, out sourceIndex, out length, args);
        }

        public static bool Find(string source, out int argIndex, params string[] args)
        {
            int sourceIndex, length;
            return Find(source, out argIndex, out sourceIndex, out length, args);
        }

        public static bool Find(string source, out int argIndex, out int sourceIndex, out int length, params string[] args)
        {
            int i = 0;

            List<string> argslist = new List<string>(args);

            ListHelper.Sort(argslist, SortMethods.LengthDescending);

            foreach (string arg in argslist)
            {
                int iof = source.IndexOf(arg);
                if (iof > -1)
                {
                    argIndex = i;
                    sourceIndex = iof;
                    length = arg.Length;
                    return true;
                }
                i++;
            }

            argIndex = -1;
            sourceIndex = -1;
            length = 0;
            return false;
        }

        public static bool IsAllDigit(string source)
        {
            if (source == null) return false;
            foreach (char c in source) if (!char.IsDigit(c)) return false;
            return true;
        }

        public static bool IsAllIsIn(string source, char[] chars)
        {
            if (source == null) return false;
            foreach (char c in source)
            {
                bool isin = false;
                foreach (char h in chars)
                {
                    if (c == h) { isin = true; break; }
                }
                if (isin == false) return false;
            }
            return true;
        }        

        public static int CountWord(string source)
        {
            string[] parts = source.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);
            return parts.Length;
        }

        public static string Join(string separator, int start, int length, string[] list)
        {
            StringBuilder sb = new StringBuilder();

            bool first = true;
            for (int i = start; i < length; i++)
            {
                if (first) first = false;
                else sb.Append(separator);
                sb.Append(list[i]);
            }

            return sb.ToString();
        }

        public static string Join(string separator, int start, string[] list)
        {
            return Join(separator, start, list.Length, list);
        }

        public static string ZeroPadding(string val, int length) 
        {
            int ls = length - val.Length;

            for (int i = 0; i < ls; i++)
            {
                val = "0" + val;
            }

            return val;
        }

        public static string ZeroPadding(int val, int length)
        {
            return ZeroPadding(val.ToString(), length);
        }

        public static string Create(params object[] objs)
        {
            return CreateToStringBuilder(objs).ToString();
        }
        
        public static StringBuilder CreateToStringBuilder(params object[] objs)
        {
            StringBuilder sb = new StringBuilder();

            foreach (object s in objs)
            {
                if (s != null) sb.Append(s);
            }

            return sb;
        }
        
        public static List<string> SplitLine(string str)
        {
            List<string> splits = new List<string>(str.Split(new string[] { "\r\n", "\n\r" }, StringSplitOptions.RemoveEmptyEntries));

            return splits;
        }
        /// <summary>
        /// Parses specified base64 string to a dictionary key value pairs. This options is used by IntelliComboBox.
        /// Example valid options (after decoded): key:value,key:value,key:value,key:value
        /// </summary>
        public static Dictionary<string, string> OptionsParser(string base64_options)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();

            if (base64_options == null || base64_options == "")
                return dict;

            string options = Base64.Decode(base64_options);

            string[] option_pairs = options.Split(new char[] { ';' });
            foreach (string option in option_pairs)
            {
                string[] keyvaluepair = option.Split(new char[] { ':' }, 2);
                if (keyvaluepair.Length == 2)
                {
                    string key = keyvaluepair[0].Trim().ToLower();
                    string value = keyvaluepair[1].Trim();

                    if (dict.ContainsKey(key))
                        dict[key] = value;
                    else
                        dict.Add(key, value);
                }
            }

            return dict;
        }





        private static Random randomHelper = null;

        private static string javaScriptSafeVariableCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        /// <summary>
        /// Represents a set of characters that are safe to create a javascript name variable.
        /// </summary>
        public static string JavaScriptSafeVariableCharacters
        {
            get { return StringHelper.javaScriptSafeVariableCharacters; }
        }

        /// <summary>
        /// Generates a randomly javascript variable with specified length.
        /// </summary>
        public static string CreateJavaScriptSafeVariable(int length)
        {
            if (randomHelper == null)
                randomHelper = new Random();

            StringBuilder sb = new StringBuilder();
          
            int avn = javaScriptSafeVariableCharacters.Length;

            for (int i = 0; i < length; i++)
            {
                sb.Append(javaScriptSafeVariableCharacters[randomHelper.Next(avn)]);
            }

            return sb.ToString();
        }

        private static string safeRandomCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        /// <summary>
        /// Represents a set of characters that are safe to create random character string.
        /// </summary>
        public static string SafeRandomCharacters
        {
            get { return StringHelper.safeRandomCharacters; }
        }
        /// <summary>
        /// Generates a randomly string with specified length.
        /// </summary>
        public static string CreateSafeRandomCharacter(int length)
        {
            if (randomHelper == null)
                randomHelper = new Random();

            StringBuilder sb = new StringBuilder();

            int avn = safeRandomCharacters.Length;

            for (int i = 0; i < length; i++)
            {
                sb.Append(safeRandomCharacters[randomHelper.Next(avn)]);
            }

            return sb.ToString();
        }


        /// <summary>
        /// Generates an equivalent javascript object notation or array of specified object.
        /// </summary>
        public static string CreateSimpleJSON(object obj)
        {
            // TODO: array blum dihandle

            if (obj == null)
            {
                return "null";
            }
            else
            {
                Type objType = obj.GetType();
                StringBuilder sb = new StringBuilder();

                if (objType == typeof(string) || objType == typeof(char))
                {
                    sb.Append('\"');
                    sb.Append(obj.ToString());
                    sb.Append('\"');
                }
                else if (objType == typeof(int) || objType == typeof(double) || objType == typeof(float))
                {
                    sb.Append(obj.ToString());
                }                
                else if (objType == typeof(bool))
                {
                    bool value = (bool)obj;
                    if (value)
                        sb.Append("true");
                    else
                        sb.Append("false");
                }
                else if (objType.IsGenericType)
                {
                    Type[] genericTypeAttrs = objType.GetGenericArguments();
                    Type genericType = objType.GetGenericTypeDefinition();

                    if (genericType.Name == "List`1")
                    {
                        Type attr0 = genericTypeAttrs[0];

                        sb.Append('[');
                        
                        bool listFirst = true;

                        System.Reflection.PropertyInfo pitem = objType.GetProperty("Item");
                        System.Reflection.PropertyInfo pcount = objType.GetProperty("Count");

                        int count = (int)pcount.GetValue(obj, null);

                        for (int x = 0; x < count; x++)                        
                        {
                            if (!listFirst)
                                sb.Append(',');

                            object iObj = pitem.GetValue(obj, new object[] { x });
                            
                            sb.Append(CreateSimpleJSON(iObj));

                            listFirst = false;
                        } 

                        sb.Append(']');
                    }
                }
                else
                {
                    sb.Append('{');
                    System.Reflection.PropertyInfo[] props = objType.GetProperties();
                    bool first = true;

                    foreach (System.Reflection.PropertyInfo prop in props)
                    {
                        Type propType = prop.PropertyType;

                        if (!first) 
                            sb.Append(',');

                        sb.Append('\"');
                        sb.Append(prop.Name);
                        sb.Append('\"');
                        sb.Append(':');
                        sb.Append(CreateSimpleJSON(prop.GetValue(obj, null)));

                        first = false;
                    }

                    sb.Append('}');
                }

                return sb.ToString();
            }            
        }

        /// <summary>
        /// 
        /// </summary>
        public static bool TryParseIPv4(string inputraw, out string ipv4, out int bitmask)
        {
            ipv4 = null;
            bitmask = 0;

            string input = inputraw.Trim();

            int slashIndex = input.IndexOf('/');
            int spaceIndex = input.IndexOf(' ');

            string ip;
            string aip = null;

            if (slashIndex > -1)
            {
                ip = input.Substring(0, slashIndex);
                aip = input.Substring(slashIndex + 1);
            }
            else if (spaceIndex > -1)
            {
                ip = input.Substring(0, spaceIndex);
                aip = input.Substring(spaceIndex + 1);
            }
            else ip = input;

            if (ip == null)
            {
                return false;
            }

            bitmask = 32;

            string[] ipp = ip.Split('.');

            if (ipp.Length != 4)
            {                
                return false;
            }

            int ipp0 = -1, ipp1 = -1, ipp2 = -1, ipp3 = -1;

            if (int.TryParse(ipp[0], out ipp0) && 
                int.TryParse(ipp[1], out ipp1) && 
                int.TryParse(ipp[2], out ipp2) && 
                int.TryParse(ipp[3], out ipp3)
                )
            {
                if (ipp0 >= 0 && ipp0 <= 255 && ipp1 >= 0 && ipp1 <= 255 && ipp2 >= 0 && ipp2 <= 255 && ipp3 >= 0 && ipp3 <= 255)
                {
                    ipv4 = ip;
                }
            }

            if (ipv4 != null)
            {
                if (aip != null)
                {
                    int tbitmask;

                    if (int.TryParse(aip, out tbitmask))
                    {
                        if (tbitmask >= 0 && tbitmask <= 32)
                        {
                            bitmask = tbitmask;
                            return true;
                        }
                    }

                    string[] aipp = aip.Split('.');

                    if (aipp.Length == 4)
                    {
                        int aipp0 = -1, aipp1 = -1, aipp2 = -1, aipp3 = -1;

                        if (int.TryParse(aipp[0], out aipp0) &&
                            int.TryParse(aipp[1], out aipp1) &&
                            int.TryParse(aipp[2], out aipp2) &&
                            int.TryParse(aipp[3], out aipp3)
                            )
                        {
                            int pls = 0;

                            if (aipp0 == 255)
                            {
                                pls = 8;
                                if (aipp1 == 255)
                                {
                                    pls = 16;
                                    if (aipp2 == 255)
                                    {
                                        pls = 24;
                                        if (aipp3 == 255)
                                        {
                                            pls = 32;
                                        }
                                        else
                                        {
                                            double logres = Math.Log((aipp3 ^ 255) + 1, 2);
                                            if (logres % 1 == 0) pls += (8 - (int)logres);
                                            else return true;
                                        }
                                    }
                                    else if (aipp3 == 0)
                                    {
                                        double logres = Math.Log((aipp2 ^ 255) + 1, 2);
                                        if (logres % 1 == 0) pls += 8 - (int)logres;
                                        else return true;
                                    }
                                }
                                else if (aipp2 == 0 && aipp3 == 0)
                                {
                                    double logres = Math.Log((aipp1 ^ 255) + 1, 2);
                                    if (logres % 1 == 0) pls += 8 - (int)logres;
                                    else return true;
                                }
                            }
                            else if (aipp1 == 0 && aipp2 == 0 && aipp3 == 0)
                            {
                                double logres = Math.Log((aipp0 ^ 255) + 1, 2);
                                if (logres % 1 == 0) pls += 8 - (int)logres;
                                else return true;
                            }

                            bitmask = pls;
                        }
                    }

                    return true;
                }
                else
                {
                    return true;
                }
            }
            else 
                return false;
        }
    }
}
