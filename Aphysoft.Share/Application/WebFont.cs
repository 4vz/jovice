using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Aphysoft.Share
{
    public static class WebFont
    {    
        internal static Dictionary<string, WebFontType> webFonts = new Dictionary<string, WebFontType>();

        public static void Register(string key, string fontFamily, string altFontFamily, WebFontWeight fontWeight,
            Resource ttfResource, Resource woffResource, Resource eotResource)
        {
            lock (webFonts)
            {
                if (!webFonts.ContainsKey(key))
                {
                    WebFontType ft = new WebFontType();
                    ft.FontFamily = fontFamily;
                    ft.AltFontFamily = altFontFamily;
                    ft.FontWeight = fontWeight;
                    if (ttfResource != null) ft.TtfKey = ttfResource.Key;
                    if (woffResource != null) ft.WoffKey = woffResource.Key;
                    if (eotResource != null) ft.EotKey = eotResource.Key;

                    webFonts.Add(key, ft);
                }
            }
        }
    }

    internal class WebFontType
    {
        private string fontFamily = string.Empty;

        public string FontFamily
        {
            get { return fontFamily; }
            set { fontFamily = value; }
        }

        private string altFontFamily = string.Empty;

        public string AltFontFamily
        {
            get { return altFontFamily; }
            set { altFontFamily = value; }
        }

        private WebFontWeight fontWeight = WebFontWeight.Normal;

        public WebFontWeight FontWeight
        {
            get { return fontWeight; }
            set { fontWeight = value; }
        }

        private string ttfKey = string.Empty;

        public string TtfKey
        {
            get { return ttfKey; }
            set { ttfKey = value; }
        }

        private string woffKey = string.Empty;

        public string WoffKey
        {
            get { return woffKey; }
            set { woffKey = value; }
        }
        private string eotKey = string.Empty;

        public string EotKey
        {
            get { return eotKey; }
            set { eotKey = value; }
        }

        public WebFontType()
        {
        }
    }

    public enum WebFontWeight
    {
        Weight100,
        Weight200,    // Light
        Weight300,    // Semi Light
        Normal,       // 400, Normal 
        Weight500,    // Semi Bold
        Weight600,    
        Bold,         // 700, Bold
        Weight800,    
        Weight900,    // Black
    }
}
