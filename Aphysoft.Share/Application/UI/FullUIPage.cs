using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using Aphysoft.Common;

namespace Aphysoft.Share
{
    internal class FullUIPage : UIPage
    {
        private string html = "";

        protected override void OnLoad(HttpContext context)
        {
            Add(Resource.CommonResourceScript);
            Add(Resource.CommonResourceCSS);                    

            JavaScriptSerializer js = new JavaScriptSerializer();

            PageData cd = Content.GetPageData(HttpContext.Current.Request.RawUrl, false);

            if (cd.Family == "system_status_404")
            {
                context.Response.StatusCode = 404;
                context.Response.TrySkipIisCustomErrors = true;
            }

            Title = cd.Title;

            string initHtml;

            if (!Settings.Ajaxify)
            {
                html = cd.Html;
                initHtml = "null";
            }
            else initHtml = js.Serialize(cd.Html);                       

            AddOnloadScript(
                string.Format("ui(\"{0}\", \"{1}\", \"{2}\", \"{3}\", {4}, \"{5}\", {6}, {7}, {8}, {9});",
                /*0*/cd.Family,
                /*1*/HttpContext.Current.Request.RawUrl.Substring(Settings.UrlPrefix.Length),
                /*2*/cd.ScriptUrl,
                /*3*/cd.CssUrl,
                /*4*/initHtml,
                /*5*/cd.Title,
                /*6*/js.Serialize(cd.Titles),
                /*7*/js.Serialize(cd.Urls),
                /*8*/js.Serialize(cd.ScriptData),
                /*9*/js.Serialize(cd.GetData())
                ));

            base.OnLoad(context);
        }

        protected override void OnRender(HttpContext context)
        {
            HttpResponse response = context.Response;
            HttpRequest request = context.Request;

            // begin title
            response.Write("<!doctype html>");
            response.Write("<!-- created with love, by Afis Herman Reza Devara -->");
            response.Write("<html><head><title>");
            if (Title == null) response.Write(Settings.FullName);
            else
            {
                string fti = Settings.TitleFormat;
                fti = fti.Replace("{TITLE}", Title);
                response.Write(fti);
            }
            // end title
            response.Write("</title>");
            // meta
            response.Write("<meta charset=\"utf-8\" />");
            // TODO: make this to some mechanism to add meta from descendant page
            // TODO: only touch page use this.
            response.Write("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0, user-scalable=0, maximum-scale=1.0\" />");
            response.Write("<meta name=\"apple-mobile-web-app-capable\" content=\"yes\" />");
            response.Write("<meta name=\"apple-mobile-web-app-status-bar-style\" content=\"black-translucent\" />");
            // shortcut icon
            string shortcutIconKey;
            if (Settings.ShortcutIcon == null)
                shortcutIconKey = "image_shortcuticon";
            else
            {
                Resource rs = Resource.Get(Settings.ShortcutIcon);
                if (rs != null)
                {
                    if (rs.ResourceType == ResourceType.JPEG || rs.ResourceType == ResourceType.PNG)
                        shortcutIconKey = Settings.ShortcutIcon;
                    else
                        shortcutIconKey = "image_shortcuticon";
                }
                else shortcutIconKey = "image_shortcuticon";
            }
            
            Resource defaultShortcutIcon = Resource.Get(shortcutIconKey);
            response.Write("<link rel=\"icon\" type=\"" + defaultShortcutIcon.MimeType + "\" href=\"" + Resource.GetPath(shortcutIconKey) + "\" />");

            #region Internal StyleSheet

            response.Write("<style>");

            StringBuilder sbstyle = new StringBuilder();

            #region Webfont
            foreach (KeyValuePair<string, WebFontType> font in WebFont.webFonts)
            {
                WebFontType w = font.Value;

                string weightClause;

                if (w.FontWeight == WebFontWeight.Bold)
                    weightClause = "font-weight: bold; ";
                else if (w.FontWeight == WebFontWeight.Weight100)
                    weightClause = "font-weight: 100; ";
                else if (w.FontWeight == WebFontWeight.Weight200)
                    weightClause = "font-weight: 200; ";
                else if (w.FontWeight == WebFontWeight.Weight300)
                    weightClause = "font-weight: 300; ";
                else if (w.FontWeight == WebFontWeight.Weight500)
                    weightClause = "font-weight: 500; ";
                else if (w.FontWeight == WebFontWeight.Weight600)
                    weightClause = "font-weight: 600; ";
                else if (w.FontWeight == WebFontWeight.Weight800)
                    weightClause = "font-weight: 800; ";
                else if (w.FontWeight == WebFontWeight.Weight900)
                    weightClause = "font-weight: 900; ";
                else
                    weightClause = string.Empty;
                
                sbstyle.Append(String.Format("@font-face {{ font-family: '{0}'; {2}src: url({1}) format('truetype'), url({3}) format('woff'); }}\r\n",
                        w.FontFamily,
                        Resource.GetPath(w.TtfKey),
                        weightClause,
                        Resource.GetPath(w.WoffKey)
                    ));
                if (!string.IsNullOrEmpty(w.AltFontFamily))
                    sbstyle.Append(String.Format("@font-face {{ font-family: '{0}'; src: url({1}) format('truetype'), url({3}) format('woff'); }}\r\n",
                        w.AltFontFamily,
                        Resource.GetPath(w.TtfKey),
                        weightClause,
                        Resource.GetPath(w.WoffKey)
                    ));
            }
            #endregion

            #region Core

            sbstyle.Append("html { height: 100%; } body { height: 100%; overflow: hidden; } body, input { font-size: 15px } * { margin: 0; padding: 0; } a:link, a:hover, a:visited, a:active { text-decoration: none; }");

            #endregion

            #region CSS Bridge

            StyleSheetData css = new StyleSheetData();
            Share.Current.StyleSheetDataBinding(context, css);

            foreach (KeyValuePair<string, StyleSheetDataClass> pair in css.Css)
            {
                sbstyle.Append(string.Format("{0} {{ ", pair.Key));

                foreach (string line in pair.Value.Lines)
                    sbstyle.Append(string.Format("{0}; ", line));

                sbstyle.Append("} ");
            }

            #endregion

            //response.Write(Web.Minifier.MinifyStyleSheet(sbstyle.ToString()));
            response.Write(sbstyle.ToString());
            response.Write("</style>");

            #endregion

            // include css resource
            foreach (string key in CssResources)
            {
                Resource resource = Resource.Get(key);
                string path = Resource.GetPath(key);
                response.Write("<link type=\"text/css\" rel=\"stylesheet\" href=\"" + path + "\" />");
            }
            // end head, begin body
            response.Write("</head><body>");
            response.Write("<div id=\"main\" class=\"_MA\" style=\"display:none\">"); // start main
            response.Write("<div id=\"top\" class=\"_TO\"></div>");
            response.Write("<div id=\"bottom\" class=\"_BM\"></div>");
            response.Write("<div id=\"pages\" class=\"_PS\">");
            if (!Settings.Ajaxify)
            {
                response.Write("<div id=\"page\" class=\"_PG\">");
                response.Write(html); // TODO
                response.Write("</div>");
            }
            response.Write("</div>");
            response.Write("</div>"); // end main
            response.Write("<div id=\"nojs\" class=\"_NJ\"></div>");            
            // end body
            // include script resource
            foreach (string key in ScriptResources)
            {
                Resource resource = Resource.Get(key);
                string path = Resource.GetPath(key);
                response.Write("<script type=\"text/javascript\" language=\"javascript\" src=\"" + path + "\"></script>");
            }
            // onload script
            response.Write("<script type=\"text/javascript\">/*<![CDATA[*/");

            StringBuilder sb = new StringBuilder();

            ScriptData scriptData = new ScriptData();
            Share.Current.ScriptDataBinding(context, scriptData);
            sb.Append("share.data({ " + string.Join(", ", scriptData.GetArrayString()) + " });");

            if (OnloadScripts.Count > 0)
            {
                sb.Append("$(function() {");
                foreach (string script in OnloadScripts)
                {
                    var clscript = script.Trim();
                    sb.Append(clscript);
                    if (!clscript.EndsWith(";")) sb.Append(";");
                }
                sb.Append("});");
            }
            
            response.Write(WebUtilities.Minifier.MinifyJavaScript(sb.ToString()));

            response.Write("/*]]>*/</script>");
            response.Write("</body>");
            response.Write("</html>");

            base.OnRender(context);
        }
    }
}
