using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Aphysoft.Share
{
    /// <summary>
    /// Provides a set of static methods that provide support for Base64 encoding.
    /// </summary>
    public static class Base64
    {
        /// <summary>
        /// Converts the specified System.String to its equivalent System.String representation encoded with base 64 digits.
        /// </summary>
        /// <param name="toEncode">A System.String.</param>
        /// <returns>System.String encoded with base 64 digits.</returns>
        public static string Encode(string toEncode)
        {
            if (toEncode == null)
                return null;

            byte[] toEncodeAsBytes = ASCIIEncoding.ASCII.GetBytes(toEncode.Trim());
            string returnValue = Convert.ToBase64String(toEncodeAsBytes);

            return returnValue;
        }

        /// <summary>
        /// Converts the specified System.String, which encodes binary data as base 64 digits, to an equivalent System.String.
        /// </summary>
        /// <param name="encodedData">A System.String, which encodes binary data as base 64 digits.</param>
        /// <returns>System.String</returns>
        public static string Decode(string encodedData)
        {
            if (encodedData == null)
                return null;

            byte[] encodedDataAsBytes = Convert.FromBase64String(encodedData.Trim());
            string returnValue = System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
            return returnValue;
        }
        /// <summary>
        /// Gets a value indicating whether the specified System.String is valid base 64 digits, 
        /// if valid then returns the decoded data using specified parameter variable.
        /// </summary>
        /// <param name="encodedData">System.String encoded with base 64 digits.</param>
        /// <param name="decodedData">Decoded System.String if encodedData is valid base 64 digits encoded System.String.</param>
        public static bool TryDecode(string encodedData, out string decodedData)
        {
            byte[] encodedDataAsBytes;
            try
            {
                encodedDataAsBytes = Convert.FromBase64String(encodedData.Trim());
                decodedData = System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
            }
            catch (Exception ex)
            {
                decodedData = null;
                return false;
            }
            return true;
        }

        /// <summary>
        /// Converts the specified System.String to its equivalent URL-safe System.String representation encoded with base 64 digits.
        /// </summary>
        /// <param name="toEncode">A System.String.</param>
        /// <returns>URL-safe System.String encoded with base 64 digits.</returns>
        public static string UrlEncode(string toEncode)
        {
            return HttpUtility.UrlEncode(Encode(toEncode));
        }

        /// <summary>
        /// Converts the specified URL-safe System.String, which encodes binary data as base 64 digits, to an equivalent System.String.
        /// </summary>
        /// <param name="encodedData">A URL-safe System.String, which encodes binary data as base 64 digits.</param>
        /// <returns>System.String</returns>
        public static string UrlDecode(string encodedData)
        {
            return Decode(HttpUtility.UrlDecode(encodedData));
        }

        public static bool TryUrlDecode(string encodedData, out string decodedData)
        {
            byte[] encodedDataAsBytes;
            try
            {
                string encodedDataUnescape = HttpUtility.UrlDecode(encodedData);

                encodedDataAsBytes = Convert.FromBase64String(encodedDataUnescape.Trim());
                decodedData = System.Text.ASCIIEncoding.ASCII.GetString(encodedDataAsBytes);
            }
            catch (Exception ex)
            {
                decodedData = null;
                return false;
            }
            return true;
        }
    }
}
