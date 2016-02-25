using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using Aphysoft.Common;
using Aphysoft.Common.Html;

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
            #region Setup

            Content.Register("system_setup",
                new ContentPage[] {                    
                    new ContentPage("/setup", "Setup")
                },
                new ContentPackage(
                    Aphysoft.Share.UI.System.Setup.Setup.PageLoad,
                    Resource.Register("system_setup_scripts", ResourceType.JavaScript, Aphysoft.Share.UI.System.Setup.Resources.ResourceManager, "setup")
                    )
                );

            #endregion
        }

        #endregion

        #region Core

        // we accept WITHOUT urlPrefix
        internal static void Begin(ResourceResult result)
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

        internal static void End(ResourceResult result)
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

            if (design == DesignType.Full) package = register.Full;
            else if (design == DesignType.Touch) package = register.Touch;
            else package = register.Lite;

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
                                    data.Html = Content.HtmlProcess(data);
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

        internal static TileData GetTileData(string family, TileParams param)
        {
            TileData data = new TileData();

            ContentRegister contentRegister = GetRegister(family);

            if (contentRegister != null)
            {
                ContentPackage targetPackage = GetContentPackage(contentRegister);

                if (targetPackage != null)
                {
                    List<string> urls = new List<string>();

                    if (contentRegister.Pages != null)
                    {
                        foreach (ContentPage contentPage in contentRegister.Pages)
                        {
                            urls.Add(contentPage.Url + (contentPage.VariablePath ? "?" : ""));
                        }
                    }

                    if (targetPackage.TileHtml != null)
                        data.Html = targetPackage.TileHtml.GetString();
                    if (targetPackage.TileScript != null)
                        data.ScriptUrl = Resource.GetPath(targetPackage.TileScript.Key);
                    if (targetPackage.TileCss != null)
                        data.CssUrl = Resource.GetPath(targetPackage.TileCss.Key);

                    data.Url = "";
                    data.Urls = urls.ToArray();
                    data.TileLoad = targetPackage.TileInit;

                    data.Height = param.Height;
                    data.Width = param.Width;
                    data.Height = param.Height;

                    if (data.TileLoad != null)
                        data.TileLoad(data);

                    data.Html = Content.HtmlProcess(data);
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

        public static void Register(string family, ContentPage[] pages, ContentPackage full, ContentPackage touch, ContentPackage lite)
        {
            //if (!Share.Current.EnableUI) throw new Exception("EnableUI Required");

            if (!contentRegisters.ContainsKey(family))
            {
                ContentRegister register = new ContentRegister(family, pages, full, touch, lite);
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

        public static void Register(string family, ContentPage[] pages, ContentPackage contentPackage)
        {
            Register(family, pages, contentPackage, null, null);
        }


        private static string HtmlProcess(PageData page)
        {
            HtmlDocument document = page.Document;
            HtmlNode node = document.DocumentNode;

            Transverse(page, node);

            return node.OuterHtml;
        }

        private static string HtmlProcess(TileData tile)
        {
            HtmlDocument document = tile.HtmlDocument;
            HtmlNode node = document.DocumentNode;

            Transverse(tile, node);

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
                        bool parsed = false;

                        string id = node.Attributes["id"].Value;
                        string tagname = node.Name.Substring(6);

                        ///if (tagname == "
                        

                        int controlIndex = 0;
                        foreach (IUIControls control in page.Controls)
                        {
                            if (control.ID == id && tagname.ToLower() == control.GetType().Name.ToLower())
                            {
                                // create new id
                                string newID = id + "_" + control.GetType().Name + "" + controlIndex;

                                control.Process(node, newID);

                                node.Remove();

                                parsed = true;

                                break;
                            }
                            controlIndex++;
                        }
                    }
                    else
                    {
                        node.Attributes.Remove("runat");
                    }
                }
                else Transverse(page, node);
            }
        }

        private static void Transverse(TileData tile, HtmlNode parent)
        {
            List<HtmlNode> tobeRemoved = new List<HtmlNode>();

            foreach (HtmlNode node in parent.ChildNodes)
            {
                bool beenRemoved = false;

                bool thisNodetobeRemoved = false;
                if (node.Attributes.Contains("visible"))
                {
                    string visible = node.Attributes["visible"].Value;
                    thisNodetobeRemoved = true;
                }
                else
                    thisNodetobeRemoved = false;

                if (node.VisibleSet == true)
                    thisNodetobeRemoved = !node.Visible;

                if (thisNodetobeRemoved)
                    tobeRemoved.Add(node);

                if (beenRemoved == false)
                {
                    Transverse(tile, node);
                }
            }

            foreach (HtmlNode node in tobeRemoved)
            {
                parent.ChildNodes.Remove(node);
            }
        }

        // TODO
        private static void RenderTile(HtmlNode node, string family, TileParams param)
        {
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
    public delegate void TileInit(TileData tile);


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

        private List<IUIControls> controls = new List<IUIControls>();

        public List<IUIControls> Controls
        {
            get { return controls; }
        }

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

    public class TileData
    {
        private string html;

        public string Html
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

        private HtmlDocument htmlDocument;

        public HtmlDocument HtmlDocument
        {
            get
            {
                if (htmlDocument == null || htmlChanged == true)
                {
                    htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(html);

                    htmlChanged = false;
                }

                return htmlDocument;
            }
        }

        private string scriptUrl;

        public string ScriptUrl
        {
            get { return scriptUrl; }
            set { scriptUrl = value; }
        }

        private string cssUrl;

        public string CssUrl
        {
            get { return cssUrl; }
            set { cssUrl = value; }
        }

        private TileInit tileLoad;

        public TileInit TileLoad
        {
            get { return tileLoad; }
            set { tileLoad = value; }
        }

        private string[] urls;

        public string[] Urls
        {
            get { return urls; }
            set { urls = value; }
        }

        private string url = null;

        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        private string tag;

        public string Tag
        {
            get { return tag; }
            set { tag = value; }
        }

        private bool isRemoved = false;

        public bool IsRemoved
        {
            get { return isRemoved; }
            set { isRemoved = value; }
        }

        private string data = null;

        public string Data
        {
            get { return data; }
            set { data = value; }
        }

        private int heightIndex = 1;

        public int HeightIndex
        {
            get { return heightIndex; }
            set
            {
                int ih = value;

                if (ih <= 0 && ih > 3)
                    ih = 1;

                heightIndex = ih;
            }
        }

        private bool usingAccentBackground = false;

        public bool UsingAccentBackground
        {
            get { return usingAccentBackground; }
            set { usingAccentBackground = value; }
        }

        private string title;

        public string Title
        {
            get { return title; }
            set { title = value; }
        }

        private int height;

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        private int width;

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        private int maxWidth;

        public int MaxWidth
        {
            get { return maxWidth; }
            set { maxWidth = value; }
        }

        private int minWidth;

        public int MinWidth
        {
            get { return minWidth; }
            set { minWidth = value; }
        }

        public TileData()
        {

        }
    }

    public class TileParams
    {
        #region Fields

        private int height;

        public int Height
        {
            get { return height; }
            set { height = value; }
        }

        private int width;

        public int Width
        {
            get { return width; }
            set { width = value; }
        }

        private string data;

        public string Data
        {
            get { return data; }
            set { data = value; }
        }

        #endregion

        #region Constructor

        public TileParams()
        {

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

        private TileInit tileInit;

        public TileInit TileInit
        {
            get { return tileInit; }
            set { tileInit = value; }
        }

        private Resource tileHtml;

        public Resource TileHtml
        {
            get { return tileHtml; }
            set { tileHtml = value; }
        }

        private Resource tileScript;

        public Resource TileScript
        {
            get { return tileScript; }
            set { tileScript = value; }
        }

        private Resource tileCss;

        public Resource TileCss
        {
            get { return tileCss; }
            set { tileCss = value; }
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
            TileInit tileInit, Resource tileHtml, Resource tileScript, Resource tileCss,
            ScriptDataBinding scriptDataBinding
            ) 
        {
            this.pageInit = pageInit;
            this.pageHtml = pageHtml;
            this.pageScript = pageScript;
            this.pageCss = pageCss;
            this.tileInit = tileInit;
            this.tileHtml = tileHtml;
            this.tileScript = tileScript;
            this.tileCss = tileCss;
            this.scriptDataBinding = scriptDataBinding;
        }

        public ContentPackage(
            PageInit init, Resource html, Resource script, Resource css,
            ScriptDataBinding scriptDataBinding
            )
        {
            this.pageInit = init;
            this.pageHtml = html;
            this.pageScript = script;
            this.pageCss = css;
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

        private ContentPackage full;

        public ContentPackage Full
        {
            get { return full; }
            set { full = value; }
        }
        
        private ContentPackage touch;

        public ContentPackage Touch
        {
            get { return touch; }
            set { touch = value; }
        }
        
        private ContentPackage lite;

        public ContentPackage Lite
        {
            get { return lite; }
            set { lite = value; }
        }

        public ContentRegister(string family, ContentPage[] pages, ContentPackage full, ContentPackage touch, ContentPackage lite)
        {
            this.family = family;
            this.pages = pages;
            this.full = full;
            this.touch = touch;
            this.lite = lite;
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
