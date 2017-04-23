using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml;
using System.IO;
using System.Configuration;
using System.Web.Configuration;


namespace Aphysoft.Share
{
    /// <summary>
    /// Provides application configuration properties.
    /// </summary>
    public partial class Settings
    {
        #region Developer Settings

        private static string developmentModeAuthentication = "share:share";

        internal static string DevelopmentModeAuthentication
        {
            get { return developmentModeAuthentication; }
        }

        private static string autoLoginUser = null;

        internal static string AutoLoginUser
        {
            get { return autoLoginUser; }
        }

        #endregion

        #region Main

        private static string physicalApplicationPath = "";
        /// <summary>
        /// Gets application path.
        /// </summary>
        public static string PhysicalApplicationPath
        {
            get { return physicalApplicationPath; }
        }

        private static string name = "Share";
        /// <summary>
        /// Gets the name of current application.
        /// </summary>
        public static string Name
        {
            get { return Settings.name; }
        }
        private static string fullName = "Share";
        /// <summary>
        /// Gets the full name of current application.
        /// </summary>
        public static string FullName
        {
            get { return Settings.fullName; }
        }
        /// <summary>
        /// the real path will be: "resourceDomain/resourceProviderPath.extension
        /// </summary>
        private static string resourceProviderPath = "resources";
        public static string ResourceProviderPath
        {
            get { return resourceProviderPath; }
        }

        private static string serviceProviderPath = "services";
        public static string ServiceProviderPath
        {
            get { return serviceProviderPath; }
        }

        #endregion

        #region Domain

        private static bool useDomain = false;

        public static bool UseDomain
        {
            get { return useDomain; }
        }

        private static bool sslAvailable = false;

        public static bool SSLAvailable
        {
            get { return sslAvailable; }
        }

        private static string baseDomain = "dummy.com";

        public static string BaseDomain
        {
            get { return baseDomain; }
        }

        private static string pageDomain = "sub.dummy.com";

        public static string PageDomain
        {
            get { return pageDomain; }
        }

        #endregion

        #region API

        private static string apiDomain = "api.dummy.com";

        public static string APIDomain
        {
            get { return apiDomain; }
        }

        #endregion

        #region Live

        private static bool enableLive = false;

        public static bool EnableLive
        {
            get { return enableLive; }
        }


        private static string streamDomain = "channel.dummy.com";

        public static string StreamDomain
        {
            get { return streamDomain; }
        }

        private static string streamBaseSubDomain = "base";

        public static string StreamBaseSubDomain
        {
            get { return streamBaseSubDomain; }
        }

        private static int streamBasePort = 0;

        public static int StreamBasePort
        {
            get { return streamBasePort; }
        }

        private static string[] streamSubDomains = new string[] { "c-1", "c-2", "c-3" };

        public static string[] StreamSubDomains
        {
            get { return streamSubDomains; }
        }

        private static int[] streamSubPorts = null;

        public static int[] StreamSubPorts
        {
            get { return streamSubPorts; }
        }

        #endregion

        #region Page

        private static string defaultPage = "";

        public static string DefaultPage
        {
            get
            {
                string dp = "";
                if (!defaultPage.StartsWith("/"))
                {
                    if (Settings.EnableUI) dp = UrlPrefix + "/" + defaultPage;
                    else dp = "/" + defaultPage;
                }
                else
                {
                    if (Settings.EnableUI) dp = UrlPrefix + defaultPage;
                    else dp = defaultPage;
                }

                return dp; 
            }
        }

        private static string titleFormat = "{TITLE}";

        public static string TitleFormat
        {
            get { return titleFormat; }
        }

        private static string titleEmpty = "";

        public static string TitleEmpty
        {
            get { return titleEmpty; }
        }

        private static string shortcutIcon = null;

        public static string ShortcutIcon
        {
            get { return shortcutIcon; }            
        }

        #endregion

        #region Resolution Group

        private static int[] sizeGroups = new int[] { 640, 960 };

        public static int[] SizeGroups
        {
            get { return sizeGroups; }
        }

        #endregion

        #region UI

        private static bool enableUI = false;

        public static bool EnableUI
        {
            get { return enableUI; }
        }

        private static string urlPrefix = "";

        public static string UrlPrefix
        {
            get
            {
                if (urlPrefix == "") return "";
                else if (!urlPrefix.StartsWith("/")) return "/" + urlPrefix;
                else return urlPrefix;
            }
        }

        private static bool ajaxify = true;

        public static bool Ajaxify
        {
            get { return ajaxify; }
        }

        public const string FontHeadingsDefault = "Avenir";        

        private static string fontHeadings = FontHeadingsDefault;

        public static string FontHeadings
        {
            get { return fontHeadings; }
        }

        public const string FontBodyDefault = "Segoe UI";

        private static string fontBody = FontBodyDefault;

        public static string FontBody
        {
            get { return fontBody; }
        }

        private static Color colorAccent = new Color("3f97bf");

        public static Color ColorAccent
        {
            get { return colorAccent; }
        }

        private static Color colorBackground = new Color(232, 232, 232);

        public static Color ColorBackground
        {
            get { return colorBackground; }           
        }

        private static Color color0 = new Color(0, 0, 0);

        public static Color Color0
        {
            get { return color0; }            
        }

        private static Color color100 = new Color(245, 245, 245);

        public static Color Color100
        {
            get { return color100; }
        }
        
        #endregion

        #region JS Library

        private static bool three = false;

        public static bool THREE
        {
            get { return three; }
        }

        private static bool raphael = true;

        public static bool Raphael
        {
            get { return raphael; }
        }

        private static bool fabric = true;

        public static bool Fabric
        {
            get { return fabric; }
        }

        #endregion

        #region Identity

        private static int sessionUserLogonLength = 10;
        /// <summary>
        /// Gets the amount of time, in minutes, allowed between user request per session before time out.
        /// </summary>
        public static int SessionUserLogonLength
        {
            get { return Settings.sessionUserLogonLength; }
        }

        private static int sessionUserInstanceLength = 2;
        /// <summary>
        /// Gets the amount of time, in minutes, allowed per user instance in worker process before time out.
        /// </summary>
        public static int SessionUserInstanceLength
        {
            get { return Settings.sessionUserInstanceLength; }
        }

        #endregion

        #region Localization

        private static Localization localization = new Localization();

        public static Localization Localization
        {
            get { return localization; }
        }

        #endregion

        #region Read Settings

        private static void ReadSettings()
        {
            // Developer Settings
            ReadString("DevelopmentModeAuthentication", ref developmentModeAuthentication);
            ReadString("AutoLoginUser", ref autoLoginUser);

            // Main
            ReadString("Name", ref name);
            ReadString("FullName", ref fullName);

            // Domain
            ReadBoolean("UseDomain", ref useDomain);
            ReadBoolean("SSLAvailable", ref sslAvailable);

            // Live
            ReadBoolean("EnableLive", ref enableLive);

            // Resolution Group
            ReadArrayInteger("ResolutionGroup", ref sizeGroups);

            // API
            ReadString("APIDomain", ref apiDomain);

            if (Settings.EnableLive)
            {
                useDomain = true;

                ReadString("StreamDomain", ref streamDomain);
                ReadString("StreamBaseSubDomain", ref streamBaseSubDomain);
                ReadInteger("StreamBasePort", ref streamBasePort);
                ReadArrayString("StreamSubDomains", ref streamSubDomains);
                if (!ReadArrayInteger("StreamSubPorts", ref streamSubPorts))
                {
                    streamSubPorts = new int[streamSubDomains.Length];
                    for (int i = 0; i < streamSubDomains.Length; i++) streamSubPorts[i] = 0;
                }
            }

            // Domain
            if (Settings.UseDomain)
            {
                ReadString("BaseDomain", ref baseDomain);
                ReadString("PageDomain", ref pageDomain);
            }

            // Page
            ReadString("DefaultPage", ref defaultPage);
            ReadString("TitleFormat", ref titleFormat);
            ReadString("TitleEmpty", ref titleEmpty);

            if (Settings.TitleEmpty == "")
            {
                titleEmpty = Settings.FullName;
            }

            ReadString("ShortcutIcon", ref shortcutIcon);

            // UI
            ReadBoolean("EnableUI", ref enableUI);
            if (Settings.EnableUI)
            {
                ReadString("UrlPrefix", ref urlPrefix);
                ReadBoolean("Ajaxify", ref ajaxify);
                ReadString("FontHeadings", ref fontHeadings);
                ReadString("FontBody", ref fontBody);

                string sc = "";
                if (ReadString("ColorAccent", ref sc)) colorAccent = new Color(sc);
                if (ReadString("ColorBackground", ref sc)) colorBackground = new Color(sc);
                if (ReadString("Color0", ref sc)) color0 = new Color(sc);
                if (ReadString("Color100", ref sc)) color100 = new Color(sc);
            }

            // Library
            ReadBoolean("THREE", ref three);
            ReadBoolean("Raphael", ref raphael);
            ReadBoolean("Fabric", ref fabric);
        }

        #endregion
    }
}

