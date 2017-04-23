using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Aphysoft.Share
{
    public static class QueryString
    {
        public static string[] GetValues(string key)
        {
            return GetValues(key, HttpContext.Current);
        }

        public static string[] GetValues(string key, HttpContext httpContext)
        {
            NameValueCollection col = httpContext.Request.QueryString;

            foreach (string k in col.AllKeys)
            {
                if (k == key)
                {
                    return col.GetValues(k);
                }
            }

            return null;
        }

        public static string GetValue(string key)
        {
            return GetValue(key, HttpContext.Current);
        }

        public static string GetValue(string key, HttpContext httpContext)
        {
            string[] values = GetValues(key, httpContext);

            if (values == null) return null;
            else return values[0];
        }

        public static bool Exists(string key)
        {
            return Exists(key, HttpContext.Current);
        }

        public static bool Exists(string key, HttpContext httpContext)
        {
            return GetValue(key, httpContext) != null;
        }

        public static bool IsExist()
        {
            return IsExist(HttpContext.Current);
        }

        public static bool IsExist(HttpContext httpContext)
        {
            return httpContext.Request.QueryString.Count > 0;
        }

        public static int ValuesCount(string key)
        {
            return ValuesCount(key, HttpContext.Current);
        }

        public static int ValuesCount(string key, HttpContext httpContext)
        {
            string[] values = GetValues(key, httpContext);

            if (values == null) return 0;
            else return values.Length;
        }
    }

}
