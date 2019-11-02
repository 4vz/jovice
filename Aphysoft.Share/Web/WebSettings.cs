using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    public static class WebSettings
    {
        public static string Domain { get; private set; } = null;
        public static string StreamDomain { get; private set; } = null;
        public static string ApiDomain { get; private set; } = null;
        public static bool Secure { get; private set; } = false;
        public static string DefaultTitle { get; private set; } = null;
        public static string UrlPrefix { get; private set; } = null;

        public static string Color0 { get; private set; } = null;
        public static string Color100 { get; private set; } = null;
        public static string ColorAccent { get; private set; } = null;
        public static string ColorBackground { get; private set; } = null;
        public static string FontBody { get; private set; } = null;

        public static int MaxStream { get; private set; } = 0;

        public static void Init()
        {
            Domain = Apps.Config("DOMAIN");
            StreamDomain = Apps.Config("STREAMDOMAIN");            
            ApiDomain = Apps.Config("APIDOMAIN");
            Secure = Bool(Apps.Config("SECURE"), false);
            DefaultTitle = Apps.Config("DEFAULTTITLE", "SHARE");
            UrlPrefix = Apps.Config("URLPREFIX", "");

            Color0 = Apps.Config("COLOR0", "000000");
            Color100 = Apps.Config("COLOR100", "F5F5F5");
            ColorAccent = Apps.Config("COLORACCENT", "AA0000");
            ColorBackground = Apps.Config("COLORBACKGROUND", "E8E8E8");
            FontBody = Apps.Config("FONTBODY", "Roboto");

            MaxStream = Int(Apps.Config("MAXSTREAM"), 3);

            MaxStream = MaxStream < 1 ? 1 : MaxStream > 8 ? 8 : MaxStream;

        }

        private static bool Bool(string config, bool def)
        {
            if (config == null) return def;

            string l = config.ToLower();

            if (l == "1" || l == "true" || l == "yes")
            {
                return true;
            }
            else if (l == "0" || l == "false" || l == "no")
            {
                return false;
            }
            else
            {
                return def;
            }
        }

        private static int Int(string config, int def)
        {
            if (config == null) return def;

            if (int.TryParse(config, out int l))
            {
                return l;
            }
            else
            {
                return def;
            }
        }
    }
}
