using System;
using System.Collections.Generic;
using System.Text;
using System.Web;

namespace Aphysoft.Share
{
    /// <summary>
    /// Provides a set of static methods to simplify query string detection from an URL.
    /// </summary>
    public static class Params
    {
        /// <summary>
        /// Gets first value by specified key of current context HttpRequest, returns null is not exist.
        /// </summary>
        public static string GetValue(string key)
        {            
            return GetValue(key, HttpContext.Current, null);
        }

        public static string GetValue(string key, HttpContext context)
        {
            return GetValue(key, context, null);
        }

        public static string GetValue(string key, RequestType? type)
        {
            return GetValue(key, HttpContext.Current, type);
        }

        public static string GetValue(string key, HttpContext context, RequestType? type)
        {
            string[] values = GetValues(key, context, type);

            if (values != null)
                return values[0];
            else
                return null;
        }

        /// <summary>
        /// Gets values by specified key of current context HttpRequest, returns null is not exist.
        /// </summary>
        public static string[] GetValues(string key)
        {
            return GetValues(key, HttpContext.Current, null);
        }

        public static string[] GetValues(string key, RequestType? type)
        {
            return GetValues(key, HttpContext.Current, type);
        }

        public static string[] GetValues(string key, HttpContext context)
        {
            return GetValues(key, context, null);
        }

        public static string[] GetValues(string key, HttpContext context, RequestType? type)
        {
            HttpRequest request = context.Request;
            string[] vals = null;

            if (vals == null && (type == null || type == RequestType.QueryString) && request.QueryString.HasKeys())
            {
                // QUERY STRING
                if (request.QueryString[key] != null)
                {
                    vals = request.QueryString.GetValues(key);
                }
            }
            if (vals == null && (type == null || type == RequestType.Form) && request.Form.HasKeys())
            {
                // FORM
                if (request.Form[key] != null)
                {
                    vals = request.Form.GetValues(key);
                }
            }
            if (vals == null)
            {
                string storedQueryString = (string)context.Items["storedQueryString"];

                if (!string.IsNullOrEmpty(storedQueryString))
                {
                    string[] splits = storedQueryString.Split(new char[] { '&' });
                    
                    List<string> values = new List<string>();

                    foreach (string sp in splits)
                    {
                        string[] splits2 = sp.Split(new char[] { '=' });

                        if (splits2.Length == 2)
                        {
                            if (splits2[0] == key)
                                values.Add(splits2[1]);
                        }
                    }

                    if (values.Count > 0)
                        vals = values.ToArray();
                }
            }

            return vals;
        }

        public static bool ValueExists(string key, string valueCheck)
        {
            string[] values = GetValues(key);

            if (values != null)
            {
                foreach (string value in values)
                {
                    if (value == valueCheck)
                        return true;
                }
            }

            return false;
        }
    }

    public enum RequestType
    {
        Form,
        QueryString
    }
}
