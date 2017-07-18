using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Aphysoft.Share
{
    public static class WebFont
    {    
        internal static Dictionary<string, string> webFontsUrl = new Dictionary<string, string>();

        public static void Register(string key, string url)
        {
            webFontsUrl.Add(key, url);
        }
    }
}
