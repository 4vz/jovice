using System;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aphysoft.Share
{
    /// <summary>
    /// Provides a set of static methods that provide hashing function.
    /// </summary>
    public static class Hash
    {
        public static string MD5(string input)
        {
            if (input == null)
                input = string.Empty;

            return MD5(Encoding.UTF8.GetBytes(input));
        }

        public static string MD5(byte[] input)
        {
            if (input == null)
                return null;

            StringBuilder sb = new StringBuilder();
            MD5 md5 = new MD5CryptoServiceProvider();
            

            byte[] result = md5.ComputeHash(input);

            for (int i = 0; i < result.Length; i++)
                sb.Append(result[i].ToString("x2"));

            return sb.ToString();
        }

        public static string Basic(string input)
        {
            string s = MD5(input);

            if (s == null)
                return null;
            else
                return s.Substring(21);
        }

        public static byte[] SHA256(byte[] input)
        {
            SHA256Managed hashstring = new SHA256Managed();
            return hashstring.ComputeHash(input);
        }

        public static byte[] SHA256(string input)
        {
            return SHA256(Encoding.UTF8.GetBytes(input));
        }
    }
}
