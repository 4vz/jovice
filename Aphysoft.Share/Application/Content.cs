using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

using Aphysoft.Share.Html;
using System.Resources;

namespace Aphysoft.Share
{
    /// <summary>
    /// Provide UI Content
    /// </summary>
    public static class Content
    {
        #region Fields

        private static Dictionary<string, ContentRegister> contentRegisterPerPages = new Dictionary<string, ContentRegister>();
        private static List<string> contentRegisterPerPagesKeySort = new List<string>();
        private static Dictionary<string, ContentRegister> contentRegisters = new Dictionary<string, ContentRegister>();
        private static bool contentLoaded = false;

        #endregion

        #region Internal Content

        internal static void InternalContentLoad()
        {
        }

        #endregion

        #region Core

        // we accept WITHOUT urlPrefix
        internal static void Begin(ResourceAsyncResult result)
        {
            HttpContext context = result.Context;
            HttpResponse response = context.Response;
            HttpRequest request = context.Request;

            string vkey = Params.GetValue("vk", context);

            if (vkey != null)
            {
                string path = Resource.GetPath(vkey);

                ContentScriptPacket packet = new ContentScriptPacket();
                packet.ScriptUrl = path;

                DataContractJsonSerializer serializer = new DataContractJsonSerializer(packet.GetType());
                serializer.WriteObject(response.OutputStream, packet);
            }
            else
            {
                string url = Params.GetValue("p", context);

                if (url != null)
                {
                    string rus = Params.GetValue("t", context);

                    if (rus == "1")
                    {
                        PageData cd = GetPageData(Settings.UrlPrefix + url, true);

                        ContentHeaderPacket packet = new ContentHeaderPacket();
                        packet.Family = cd.Family;
                        packet.CurrentTitle = cd.Title;

                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(packet.GetType());
                        serializer.WriteObject(response.OutputStream, packet);
                    }
                    else
                    {
                        PageData cd = GetPageData(Settings.UrlPrefix + url, false);

                        ContentBodyPacket packet = new ContentBodyPacket();
                        packet.Family = cd.Family;
                        packet.CurrentTitle = cd.Title;
                        packet.Html = cd.Html;
                        packet.ScriptUrl = cd.ScriptUrl == null ? "" : cd.ScriptUrl;
                        packet.CssUrl = cd.CssUrl;
                        packet.Titles = cd.Titles;
                        packet.Urls = cd.Urls;
                        packet.DynamicScriptData = cd.ScriptData;
                        packet.Data = cd.GetData();

                        DataContractJsonSerializer serializer = new DataContractJsonSerializer(packet.GetType());
                        serializer.WriteObject(response.OutputStream, packet);
                    }

                }
                else
                {
                    result.Response.Status = "404 Not Found";
                    result.Response.TrySkipIisCustomErrors = true;
                }
            }

            result.SetCompleted();
        }

        internal static void End(ResourceAsyncResult result)
        {
        }

        internal static void Init()
        {
            if (!contentLoaded)
            {
                contentLoaded = true;
                Content.InternalContentLoad();
            }
        }

        #endregion
        
        #region Content
        
        private static ContentRegister GetRegister(string path, out string contentPath, out string queryString, out string queryStringPath)
        {
            ContentRegister contentRegister = null;
            contentPath = null;
            queryString = null;
            queryStringPath = null;

            // recheck if path prefix is application prefix
            if (path.StartsWith(Settings.UrlPrefix))
            {
                // remove prefix
                path = path.Substring(Settings.UrlPrefix.Length);

                // split between query string
                string[] pathAndQueryString = path.Split(new char[] { '?' }, 2);

                path = pathAndQueryString[0];

                if (pathAndQueryString.Length == 2)
                    queryString = pathAndQueryString[1];

                // lookup through dictionary, for path, from long entry first
                foreach (string pageUrl in contentRegisterPerPagesKeySort)
                {

                    if (pageUrl.EndsWith("?"))
                    {
                        // variable path
                        var pageUrlClean = pageUrl.TrimEnd(new char[] { '?' });

                        if (path.StartsWith(pageUrlClean))
                        {
                            contentRegister = contentRegisterPerPages[pageUrlClean];
                            contentPath = pageUrlClean;

                            if (path.Length >= pageUrl.Length)
                            {
                                queryStringPath = path.Substring(pageUrl.Length - 1);
                            }
                            break;
                        }
                    }
                    else if (path == pageUrl)
                    {
                        contentRegister = contentRegisterPerPages[pageUrl];
                        contentPath = pageUrl;
                        queryStringPath = string.Empty;
                        break;
                    }
                }
            }

            return contentRegister;
        }

        private static ContentRegister GetRegister(string family)
        {
            ContentRegister contentRegister = null;
            
            if (contentRegisters.ContainsKey(family))
            {
                contentRegister = contentRegisters[family];
            }

            return contentRegister;
        }
        
        private static ContentPackage GetContentPackage(ContentRegister register)
        {
            ContentPackage package;
            DesignType design = Share.Current.Request.Design;

            package = register.Package;

            return package;
        }

        internal static PageData GetPageData(string path, bool headerOnly)
        {            
            PageData data = new PageData();

            if (path.StartsWith(Settings.UrlPrefix))
            {
                string contentPath, queryString, queryStringPath;

                ContentRegister contentRegister = GetRegister(path, out contentPath, out queryString, out queryStringPath);

                HttpContext.Current.Items["storedQueryString"] = queryString;

                bool ok = false;

                // if content register found, then serve
                #region Found
                if (contentRegister != null)
                { 
                    ContentPackage targetPackage = GetContentPackage(contentRegister);

                    if (targetPackage != null)
                    {
                        // look for contentOnPage for current page title
                        // create titles and urls
                        ContentPage contentOnPage = null;
                        List<string> titles = new List<string>();
                        List<string> urls = new List<string>();

                        List<ContentPage> sorter = new List<ContentPage>(contentRegister.Pages);
                        sorter.Sort(new ContentPageUrlSorter());

                        foreach (ContentPage contentPage in sorter)
                        {
                            if (contentPage.Url == contentPath)
                            {
                                contentOnPage = contentPage;
                            }

                            titles.Add(contentPage.Title);
                            urls.Add(contentPage.Url + (contentPage.VariablePath ? "?" : ""));
                        }

                        data.VariablePath = queryStringPath;
                        data.QueryString = queryString;
                        data.Url = contentPath;
                        data.Title = contentOnPage.Title;
                        data.Family = contentRegister.Family;
                        
                        #region complete data
                        if (headerOnly == false)
                        {
                            if (targetPackage.PageHtml != null)
                                data.Html = targetPackage.PageHtml.GetString();
                            if (targetPackage.PageScript != null)
                                data.ScriptUrl = Resource.GetPath(targetPackage.PageScript.Key);
                            if (targetPackage.PageCss != null)
                                data.CssUrl = Resource.GetPath(targetPackage.PageCss.Key);

                            data.Titles = titles.ToArray();
                            data.Urls = urls.ToArray();

                            // page dynamic script data
                            ScriptData scriptData = new ScriptData();

                            // add dynamic script
                            BindingParameters parameters = new BindingParameters();
                            parameters.RefererUrl = Settings.UrlPrefix + contentPath;

                            Content.ScriptDataBinding(parameters, scriptData);

                            // save dynamic script
                            data.ScriptData = scriptData.GetArrayObject();

                            if (targetPackage.PageInit != null)
                            {
                                try
                                {
                                    targetPackage.PageInit(data);
                                    data.Html = HtmlProcess(data);
                                }
                                catch (Exception ex)
                                {
                                    //data.Status = 501;
                                    //data.Message = ex.Message;
                                }
                            }
                        }
                        #endregion                        

                        ok = true;
                    }
                }
                #endregion

                if (ok == false)
                {
                    ContentRegister statusRegister = GetRegister("system_status");
                    ContentPackage targetPackage = GetContentPackage(statusRegister);

                    data.Title = "Page Not Found";
                    data.Family = "system_status_404";

                    #region complete data
                    if (headerOnly == false)
                    {
                        if (targetPackage.PageHtml != null)
                            data.Html = targetPackage.PageHtml.GetString();
                        if (targetPackage.PageScript != null)
                            data.ScriptUrl = Resource.GetPath(targetPackage.PageScript.Key);
                        if (targetPackage.PageCss != null)
                            data.CssUrl = Resource.GetPath(targetPackage.PageCss.Key);
                    }

                    #endregion
                }
            }
            

            return data;
        }

        private static void ScriptDataBinding(BindingParameters param, ScriptData script)
        {
            string contentPath, queryString, queryStringPath;
            string refererUrl = param.RefererUrl;

            ContentRegister contentRegister = GetRegister(refererUrl, out contentPath, out queryString, out queryStringPath);

            if (contentRegister != null)
            {
                ContentPackage package = GetContentPackage(contentRegister);

                if (package.ScriptDataBinding != null)
                    package.ScriptDataBinding(param, script);
            }
        }
                
        public static void Register(string key, ResourceManager resourceManager, string objectName, string resourcePath, params string[] pages)
        {
            List<ContentPage> lpages = new List<ContentPage>();

            foreach (string page in pages)
            {
                string[] tokens = page.Split(new char[] { ':' });
                string url = tokens[0];
                bool var = false;
                string title = null;

                if (tokens.Length >= 2) var = tokens[1] == "true";
                if (tokens.Length >= 3) title = tokens[2];

                lpages.Add(new ContentPage(url, var, title));
            }

            Register(key, lpages.ToArray(), new ContentPackage(Resource.Register(key, ResourceTypes.JavaScript, resourceManager, objectName, resourcePath)));
        }

        public static void Register(string family, ContentPage[] pages, ContentPackage contentPackage)
        {
            //if (!Share.Current.EnableUI) throw new Exception("EnableUI Required");

            if (!contentRegisters.ContainsKey(family))
            {
                ContentRegister register = new ContentRegister(family, pages, contentPackage);
                contentRegisters.Add(family, register);
                if (pages != null)
                {
                    foreach (ContentPage page in pages)
                    {
                        if (!contentRegisterPerPages.ContainsKey(page.Url) &&
                            !page.Url.StartsWith("~/" + Settings.ResourceProviderPath + "/")
                            )
                        {
                            contentRegisterPerPages.Add(page.Url, register);
                            contentRegisterPerPagesKeySort.Add(page.Url + (page.VariablePath ? "?" : ""));
                        }
                    }
                    if (pages.Length > 0) contentRegisterPerPagesKeySort.Sort(new KeyLengthSorter());
                }
            }

        }
        
        private static string HtmlProcess(PageData page)
        {
            HtmlDocument document = page.Document;
            HtmlNode node = document.DocumentNode;

            Transverse(page, node);

            return node.OuterHtml;
        }

        private static void Transverse(PageData page, HtmlNode parent)
        {   
            List<HtmlNode> childNodes = new List<HtmlNode>();

            foreach (HtmlNode node in parent.ChildNodes)
            {
                childNodes.Add(node);
            }

            foreach (HtmlNode node in childNodes)
            {
                if (node.Attributes.Contains("runat") && ((string)node.Attributes["runat"].Value) == "server")
                {
                    if (node.Name.StartsWith("share:") && node.Attributes.Contains("id"))
                    {
                        string id = node.Attributes["id"].Value;
                        string tagname = node.Name.Substring(6);
                    }
                    else
                    {
                        node.Attributes.Remove("runat");
                    }
                }
                else Transverse(page, node);
            }
        }

        public static void Render(HttpContext context)
        {
            JavaScriptSerializer js = new JavaScriptSerializer();

            PageData cd = Content.GetPageData(HttpContext.Current.Request.RawUrl, false);

            if (cd.Family == "system_status_404")
            {
                context.Response.StatusCode = 404;
                context.Response.TrySkipIisCustomErrors = true;
            }

            string title = cd.Title;

            string html;
            string initHtml;

            if (!Settings.Ajaxify)
            {
                html = cd.Html;
                initHtml = "null";
            }
            else
            {
                html = "";
                initHtml = js.Serialize(cd.Html);
            }

            HttpResponse response = context.Response;
            HttpRequest request = context.Request;

            // begin title
            response.Write("<!doctype html>");
            response.Write("<!-- By Afis Herman Reza Devara. Open the browser's developer console for more information about this. -->");
            response.Write("<html><head><title>");
            if (title == null) response.Write(Settings.FullName);
            else
            {
                string fti = Settings.TitleFormat;
                fti = fti.Replace("{TITLE}", title);
                response.Write(fti);
            }
            // end title
            response.Write("</title>");
            // meta
            response.Write("<meta charset=\"utf-8\" />");
            // make this to some mechanism to add meta from descendant page
            // only touch page use this.
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
                    if (rs.ResourceType == ResourceTypes.JPEG || rs.ResourceType == ResourceTypes.PNG)
                        shortcutIconKey = Settings.ShortcutIcon;
                    else
                        shortcutIconKey = "image_shortcuticon";
                }
                else shortcutIconKey = "image_shortcuticon";
            }

            Resource defaultShortcutIcon = Resource.Get(shortcutIconKey);
            response.Write("<link rel=\"icon\" type=\"" + defaultShortcutIcon.MimeType + "\" href=\"" + Resource.GetPath(shortcutIconKey) + "?v=2\" />");

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

                sbstyle.Append(string.Format("@font-face {{ font-family: '{0}'; {2}src: url({1}) format('truetype'), url({3}) format('woff'); }}\r\n",
                        w.FontFamily,
                        Resource.GetPath(w.TtfKey),
                        weightClause,
                        Resource.GetPath(w.WoffKey)
                    ));
                if (!string.IsNullOrEmpty(w.AltFontFamily))
                    sbstyle.Append(string.Format("@font-face {{ font-family: '{0}'; src: url({1}) format('truetype'), url({3}) format('woff'); }}\r\n",
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
            response.Write("<link type=\"text/css\" rel=\"stylesheet\" href=\"" + Resource.GetPath(Resource.CommonResourceCSS) + "\" />");

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
            response.Write("<script type=\"text/javascript\" language=\"javascript\" src=\"" + Resource.GetPath(Resource.CommonResourceScript) + "\"></script>");

            // onload script
            response.Write("<script type=\"text/javascript\">/*<![CDATA[*/");

            StringBuilder sb = new StringBuilder();

            ScriptData scriptData = new ScriptData();
            Share.Current.ScriptDataBinding(context, scriptData);
            sb.Append("share.data({ " + string.Join(", ", scriptData.GetArrayString()) + " });");

            sb.Append("$(function() {");
            sb.Append(string.Format("ui(\"{0}\", \"{1}\", \"{2}\", \"{3}\", {4}, \"{5}\", {6}, {7}, {8}, {9});",
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
            sb.Append("});");

            response.Write(WebUtilities.Minifier.MinifyJavaScript(sb.ToString()));

            response.Write("/*]]>*/</script>");
            response.Write("</body>");
            response.Write("</html>");

            response.AppendHeader("Cache-Control", "private, no-cache, no-store, must-revalidate");
            response.AppendHeader("Pragma", "no-cache");
            response.AppendHeader("Expires", "Sat, 01 Jan 2000 00:00:00 GMT");
            response.Headers["Content-Type"] = "text/html; charset=utf-8";
        }

        #endregion

        #region Application

        public static void Redirect(string family, int index, string querystring)
        {
            if (contentRegisters.ContainsKey(family))
            {
                ContentRegister register = contentRegisters[family];

                if (register.Pages != null)
                {
                    if (index < register.Pages.Length)
                    {
                        HttpContext.Current.Response.Redirect(Settings.UrlPrefix + register.Pages[index].Url + (string.IsNullOrEmpty(querystring) ? string.Empty : "?" + querystring), true);
                        HttpContext.Current.Response.End();
                    }
                }
            }
        }

        public static void Redirect(string family)
        {
            Redirect(family, 0, null);
        }

        public static void Redirect(string family, int index)
        {
            Redirect(family, index, null);
        }

        public static void Redirect(string family, string querystring)
        {
            Redirect(family, 0, querystring);
        }

        #endregion
    }

    public delegate void ScriptDataBinding(BindingParameters param, ScriptData script);
    public delegate void PageInit(PageData page);

    [DataContractAttribute]
    class ContentHeaderPacket
    {
        private string family;

        [DataMemberAttribute(Name = "f")]
        public string Family
        {
            get { return family; }
            set { family = value; }
        }

        private string currentTitle;

        [DataMemberAttribute(Name = "t")]
        public string CurrentTitle
        {
            get { return currentTitle; }
            set { currentTitle = value; }
        }

        public ContentHeaderPacket()
        {

        }
    }
    
    [DataContractAttribute]
    class ContentBodyPacket : ContentHeaderPacket
    {
        private string html = "";

        [DataMemberAttribute(Name = "c")]
        public string Html
        {
            get { return html; }
            set { html = value; }
        }

        private string scriptUrl;

        [DataMemberAttribute(Name = "s")]
        public string ScriptUrl
        {
            get { return scriptUrl; }
            set { scriptUrl = value; }
        }

        private string cssUrl;

        [DataMemberAttribute(Name = "u")]
        public string CssUrl
        {
            get { return cssUrl; }
            set { cssUrl = value; }
        }

        private string[] titles;

        [DataMemberAttribute(Name = "y")]
        public string[] Titles
        {
            get { return titles; }
            set { titles = value; }
        }

        private string[] urls;

        [DataMemberAttribute(Name = "z")]
        public string[] Urls
        {
            get { return urls; }
            set { urls = value; }
        }

        private object[] dynamicScriptData;

        [DataMemberAttribute(Name = "d")]
        public object[] DynamicScriptData
        {
            get { return dynamicScriptData; }
            set { dynamicScriptData = value; }
        }

        private object[] data;

        [DataMemberAttribute(Name = "x")]
        public object[] Data
        {
            get { return data; }
            set { data = value; }
        }

        public ContentBodyPacket()
        {
        }
    }

    [DataContractAttribute]
    class ContentScriptPacket
    {
        private string scriptUrl;

        [DataMemberAttribute(Name = "s")]
        public string ScriptUrl
        {
            get { return scriptUrl; }
            set { scriptUrl = value; }
        }
    }

    public class PageData
    {
        #region Fields

        #region Internal

        private string html;

        /// <summary>
        /// Gets or Sets Page Html.
        /// </summary>
        internal string Html
        {
            get { return html; }
            set
            {
                if (value != html)
                    htmlChanged = true;

                html = value;
            }
        }

        private bool htmlChanged = true;


        private string[] titles;

        /// <summary>
        /// Internal Use Only.
        /// </summary>
        internal string[] Titles
        {
            get { return titles; }
            set { titles = value; }
        }

        private string[] urls;
        /// <summary>
        /// Internal Use Only.
        /// </summary>
        internal string[] Urls
        {
            get { return urls; }
            set { urls = value; }
        }

        private string url;
        /// <summary>
        /// Internal Use Only.
        /// </summary>
        internal string Url
        {
            get { return url; }
            set { url = value; }
        }

        private string scriptUrl;
        /// <summary>
        /// Internal Use Only.
        /// </summary>
        internal string ScriptUrl
        {
            get { return scriptUrl; }
            set { scriptUrl = value; }
        }
        /// <summary>
        /// Internal Use Only.
        /// </summary>
        internal string cssUrl;
        /// <summary>
        /// Internal Use Only.
        /// </summary>
        internal string CssUrl
        {
            get { return cssUrl; }
            set { cssUrl = value; }
        }

        private string family;
        /// <summary>
        /// Internal Use Only.
        /// </summary>
        internal string Family
        {
            get { return family; }
            set { family = value; }
        }

        private object[] scriptData;
        /// <summary>
        /// Internal Use Only.
        /// </summary>
        internal object[] ScriptData
        {
            get { return scriptData; }
            set { scriptData = value; }
        }

        private Dictionary<string, object> data;

        #endregion

        private HtmlDocument document;

        /// <summary>
        /// Gets structured Html document.
        /// </summary>
        public HtmlDocument Document
        {
            get
            {
                if (document == null || htmlChanged == true)
                {
                    document = new HtmlDocument();

                    if (!string.IsNullOrEmpty(html))
                        document.LoadHtml(html);

                    htmlChanged = false;
                }

                return document;
            }
        }

        private string title = "";

        /// <summary>
        /// Gets or Sets Page Title.
        /// </summary>
        public string Title
        {
            get { return title; }
            set { title = value; }
        }
        
        private string queryStringPath;

        /// <summary>
        /// Gets page variable path.
        /// </summary>
        public string VariablePath
        {
            get { return queryStringPath; }
            internal set { queryStringPath = value; }
        }

        private string queryString;

        /// <summary>
        /// Gets page query string.
        /// </summary>
        public string QueryString
        {
            get { return queryString; }
            internal set { queryString = value; }
        }

        #endregion

        #region Constructor

        internal PageData()
        {
            data = new Dictionary<string, object>();
        }

        #endregion

        #region Methods

        internal object[] GetData()
        {
            List<object> o = new List<object>();

            foreach (KeyValuePair<string, object> kvp in data)
            {
                o.Add(kvp.Key);
                o.Add(kvp.Value);
            }

            return o.ToArray();
        }

        public object Data(string key)
        {
            if (data.ContainsKey(key))
                return data[key];
            else
                return null;
        }

        public void Data(string key, object value)
        {
            if (value == null)
            {
                if (data.ContainsKey(key))
                    data.Remove(key);
            }
            else
            {
                if (!data.ContainsKey(key))
                    data.Add(key, value);
                else
                    data[key] = value;
            }
        }

        #endregion
    }

    public class ContentPackage
    { 
        #region Fields
        
        private PageInit pageInit;

        public PageInit PageInit
        {
            get { return pageInit; }
            set { pageInit = value; }
        }

        private Resource pageHtml;

        public Resource PageHtml
        {
            get { return pageHtml; }
            set { pageHtml = value; }
        }

        private Resource pageScript;

        public Resource PageScript
        {
            get { return pageScript; }
            set { pageScript = value; }
        }

        private Resource pageCss;

        public Resource PageCss
        {
            get { return pageCss; }
            set { pageCss = value; }
        }

        private ScriptDataBinding scriptDataBinding;

        public ScriptDataBinding ScriptDataBinding
        {
            get { return scriptDataBinding; }
            set { scriptDataBinding = value; }
        }

        #endregion

        #region Constructor

        public ContentPackage(
            PageInit pageInit, Resource pageHtml, Resource pageScript, Resource pageCss,
            ScriptDataBinding scriptDataBinding
            ) 
        {
            this.pageInit = pageInit;
            this.pageHtml = pageHtml;
            this.pageScript = pageScript;
            this.pageCss = pageCss;
            this.scriptDataBinding = scriptDataBinding;
        }

        public ContentPackage(
            PageInit init, Resource html, Resource script, Resource css
            )
        {
            this.pageInit = init;
            this.pageHtml = html;
            this.pageScript = script;
            this.pageCss = css;
        }

        public ContentPackage(
            Resource script, Resource css
            )
        {
            this.pageScript = script;
            this.pageCss = css;
        }

        public ContentPackage(
            PageInit init, Resource script, Resource css
            )
        {
            this.pageInit = init;
            this.pageScript = script;
            this.pageCss = css;
        }

        public ContentPackage(
            PageInit init, Resource script
            )
        {
            this.pageInit = init;
            this.pageScript = script;
        }

        public ContentPackage(
            Resource script
            )
        {
            this.pageScript = script;
        }

        #endregion
    }

    public class ContentRegister
    {
        private string family;

        public string Family
        {
            get { return family; }
            set { family = value; }
        }

        private ContentPage[] pages;

        public ContentPage[] Pages
        {
            get { return pages; }
            set { pages = value; }
        }

        private ContentPackage package;

        public ContentPackage Package
        {
            get { return package; }
            set { package = value; }
        }

        public ContentRegister(string family, ContentPage[] pages, ContentPackage package)
        {
            this.family = family;
            this.pages = pages;
            this.package = package;
        }
    }

    public class ContentPage
    {
        private string url;

        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        private bool variablePath;

        public bool VariablePath
        {
            get { return variablePath; }
            set { variablePath = value; }
        }

        public ContentPage(string url)
        {
            this.url = url;
            this.title = "";
            this.variablePath = false;
        }

        public ContentPage(string url, bool variablePath)
        {
            this.url = url;
            this.variablePath = variablePath;
            this.title = "";
        }

        public ContentPage(string url, string title)
        {
            this.url = url;
            this.title = title;
            this.variablePath = false;
        }

        public ContentPage(string url, bool variablePath, string title)
        {
            this.url = url;
            this.variablePath = variablePath;
            this.title = title;
        }

    }

    internal class KeyLengthSorter : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x.Length > y.Length)
                return -1;
            else if (x.Length < y.Length)
                return 1;
            else
                return 0;
        }
    }

    internal class ContentPageUrlSorter : IComparer<ContentPage>
    {
        public int Compare(ContentPage x, ContentPage y)
        {
            if (x.Url.Length > y.Url.Length)
                return -1;
            else if (x.Url.Length < y.Url.Length)
                return 1;
            else
                return 0;
        }
    }
}
