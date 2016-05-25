using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.Hosting;
using Aphysoft.Common;
using System.Diagnostics;
using System.Threading;

namespace Aphysoft.Share
{
    public class Share : IHttpModule, IHttpAsyncHandler, IRequiresSessionState
    {
        #region Consts

        const string year2000 = "Sat, 01 Jan 2000 00:00:00 GMT";

        #endregion

        #region Database

        private static Database share = null;

        public static Database Database
        {
            get
            {
                if (share == null)
                {
                    string database = Configuration.Settings("database");
                    string connectionString = string.Format("Data Source={0};Initial Catalog=share;User ID=telkom.center;Password=t3lk0mdotc3nt3r;async=true", database);
                    share = new Database(connectionString, DatabaseType.SqlServer);
                }

                return share;
            }
        }

        #endregion

        #region Instance Properties

        #region Const

        internal const int ShareVersion = 30;
        
        const string PathSetup = "/setup";
        
        #endregion

        #region System

        private static Share current;

        public static Share Current
        {
            get { return current; }
        }

        private readonly static object initSync = new object();
        private static bool inited = false;
        private static bool onInitCompleted = false;

        private Request currentRequest = null;

        public Request Request
        {
            get
            {
                if (currentRequest == null)
                    currentRequest = new Request();
                return currentRequest;
            }
        }

        #endregion

        #region Version

        protected int Version = 0;

        #endregion

        #endregion

        #region Core

        public void Init(HttpApplication application)
        {
            if (!inited)
            {
                lock (initSync)
                {
                    if (!inited)
                    {
                        inited = true;

                        Share.current = this;

                        OnInit();

                        onInitCompleted = true;

                        Settings.ClientInit();

                        Service.Client();

                        UserSettings.Init();

                        //Identity.Init()

                        Resource.Init();

                        if (Settings.EnableUI)
                        {
                            Content.Init();
                        }

                        OnResourceLoad();
                    }
                }
            }

            application.Error += application_Error;
            application.BeginRequest += application_BeginRequest;
            application.AuthenticateRequest += application_AuthenticateRequest;
            application.PostAuthenticateRequest += application_PostAuthenticateRequest;
            application.AuthorizeRequest += application_AuthorizeRequest;
            application.PostAuthorizeRequest += application_PostAuthorizeRequest;
            application.AcquireRequestState += application_AcquireRequestState;
            application.PostAcquireRequestState += application_PostAcquireRequestState;
            application.PreRequestHandlerExecute += application_PreRequestHandlerExecute;
            application.EndRequest += application_EndRequest;
            application.PreSendRequestHeaders += application_PreSendRequestHeaders;
            application.PreSendRequestContent += application_PreSendRequestContent;
        }

        public void Dispose()
        {
        }

        protected Share()
        {
        }

        #endregion

        #region Events

        private void application_Error(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpContext context = HttpContext.Current;

            Service.Debug("error: " + context.Error.Message + ", stack trace: " + context.Error.StackTrace);
        }

        private void application_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpContext context = HttpContext.Current;
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;

            // design type here
            context.Items["designtype"] = Aphysoft.Share.DesignType.Full;

            // When using UI, we're not using internal session state.
            if (Settings.EnableUI)
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Disabled);
            else
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);

            string appExecutionPath = request.AppRelativeCurrentExecutionFilePath;

            if (appExecutionPath.StartsWith("~/" + Settings.ResourceProviderPath)) context.Items["provider"] = ExecutionTypes.Resources;
            else if (appExecutionPath.StartsWith("~/" + Settings.ServiceProviderPath)) context.Items["provider"] = ExecutionTypes.Services;
            else if (appExecutionPath.ToLower() == "~/favicon.ico") context.Items["provider"] = ExecutionTypes.Favicon;
            else
            {
                context.Items["provider"] = ExecutionTypes.Default;

                string host = request.Headers["Host"];
                string cExUrl = request.CurrentExecutionFilePath.ToLower();

                // if hostname differ, then redirect to hsotname
                if (Settings.UseDomain && host != Settings.PageDomain)
                {
                    response.Redirect("http://" + Settings.PageDomain + request.RawUrl);
                    response.End();
                }
                else
                {
                    if (Settings.EnableUI)
                    {
                        // redirect to url prefix
                        bool toUIPage = false;

                        if (!string.IsNullOrEmpty(Settings.UrlPrefix))
                        {
                            if (cExUrl == "/" || cExUrl == Settings.UrlPrefix)
                                response.Redirect(string.Format("{0}/", Settings.UrlPrefix), true);
                            else if (cExUrl.StartsWith(Settings.UrlPrefix))
                                toUIPage = true;
                        }
                        else
                            toUIPage = true;

                        if (toUIPage)
                        {
                            HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Disabled);
                            UIPage page;
                            if (Share.Current.Request.Design == Aphysoft.Share.DesignType.Full) page = new FullUIPage();
                            else if (Share.Current.Request.Design == Aphysoft.Share.DesignType.Touch) page = new TouchUIPage();
                            else page = new LiteUIPage();
                            context.Items["uipage"] = page;
                        }
                        else
                        {
                            // to non ASP.NET page.
                            // eg. html, htm, etc
                            // HttpSessionState is NOT AVAILABLE
                            if (cExUrl.EndsWith(".aspx")) // sorry, we had to disable asp.net pages
                            {
                                response.Headers.Add("Content-Type", "text/html; charset=utf-8");
                                response.Write("UI is Enabled, we can't serve ASP.NET pages since HttpSessionState has been disabled.");
                                response.End();
                            }
                        }
                    }
                    else
                    {
                        // to ASP.NET page
                        // HttpSessionState is AVAILABLE
                        OnUrlRewrite();
                    }
                }
            }
        }

        private void application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        private void application_PostAuthenticateRequest(object sender, EventArgs e)
        {
        }

        private void application_AuthorizeRequest(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpContext context = HttpContext.Current;

            #region Basic Authorization for Development Mode

            if (Settings.DevelopmentMode)
            {
                HttpRequest request = context.Request;
                HttpResponse response = context.Response;

                //string authhash = request.Headers["Authorization"];

                //if (authhash == null || authhash != "Basic " + Base64.Encode(Settings.DevelopmentModeAuthentication))
                //{
                //    response.Status = "401 Authorization Required";
                //    response.Headers.Add("WWW-Authenticate", "Basic realm=\"This site is currently under development mode. Please enter developer user and password to continue.\"");

                //    if (context.Items.Contains("resource"))
                //        response.Write("/* Developer Mode: Authorization Required */");
                //    else  // uipage or asp.net page                
                //        response.Write("<h1>Developer Mode: Authorization Required</h1>");
                //    response.End();

                //    return;
                //}
            }
            #endregion
        }

        private void application_PostAuthorizeRequest(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;
        }

        private void application_AcquireRequestState(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;

            ExecutionTypes executionType = (ExecutionTypes)context.Items["provider"];
            string requestHostName = request.Headers["Host"];

            if (Settings.EnableUI)
            {
                Session.Start(context);

                #region Setup

                if (executionType == ExecutionTypes.Default)
                {
                    string cp = Path.Base();

                    if (request.Cookies["vers"] == null)
                    {
                        if (cp != Settings.UrlPrefix + PathSetup)
                        {
                            response.Redirect(Settings.UrlPrefix + PathSetup + "?done=" + Base64.UrlEncode(request.RawUrl));
                            response.End();
                        }
                        else
                        {
                            #region Setup

                            HttpCookie cookie = new HttpCookie("shts");
                            cookie.Value = "shts";
                            cookie.HttpOnly = false;
                            cookie.Path = "/";
                            if (Settings.UseDomain) cookie.Domain = Settings.BaseDomain;

                            response.Cookies.Add(cookie);

                            #endregion
                        }
                    }
                    else
                    {
                        if (cp != Settings.UrlPrefix + PathSetup)
                        {
                            string verss = request.Cookies["vers"].Value;

                            int vers;

                            if (int.TryParse(verss, out vers)) ;
                            else vers = 0;

                            if (vers < ShareVersion)
                            {
                                response.Redirect(Settings.UrlPrefix + PathSetup + "?done=" + Base64.UrlEncode(request.RawUrl));
                                response.End();
                            }
                        }
                    }
                }
                #endregion
            }
            else
            {
                // WITHOUT UI
                if (context.Session != null)
                {
                    context.Items["sessionID"] = context.Session.SessionID;
                }
            }
        }

        private void application_PostAcquireRequestState(object sender, EventArgs e)
        {
        }

        private void application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;

            if (Settings.EnableUI)
            {
                if (context.Items.Contains("uipage"))
                {
                    UIPage page = (UIPage)context.Items["uipage"];
                    page.Render(context);
                    Compress(context);
                    context.Response.End();
                }
            }
        }

        private void application_EndRequest(object sender, EventArgs e)
        {
        }

        private void application_PreSendRequestHeaders(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;
            HttpResponse response = context.Response;
            HttpRequest request = context.Request;

            response.Headers.Remove("Server");
            response.Headers.Remove("X-AspNet-Version");
            response.Headers.Remove("X-Powered-By");

            // send debugging information
            //response.Headers.Add("Server", string.Format("{0} {1} {2}",
            //    context.Server.MachineName,
            //    Process.GetCurrentProcess().Id.ToString(),
            //    ""
            //    ));
        }

        private void application_PreSendRequestContent(object sender, EventArgs e)
        {
        }

        #endregion

        #region Handlers

        public bool IsReusable { get { return true; } }

        public void ProcessRequest(HttpContext context) { }

        public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        {
            HttpResponse response = context.Response;
            HttpRequest request = context.Request;

            string currentExecutionPath = string.Format("~{0}", context.Request.RawUrl);
            string resourcesProviderExecutionPath = string.Format("~/{0}", Settings.ResourceProviderPath).ToLower();
            string servicesProviderExecutionPath = string.Format("~/{0}", Settings.ServiceProviderPath).ToLower();

            AsyncResult result = null;

            bool notFound = true;
            
            if (currentExecutionPath.StartsWith(resourcesProviderExecutionPath.ToLower()))
            {
                #region Resource

                string resourceRawTarget = currentExecutionPath.Substring(resourcesProviderExecutionPath.Length);

                // rawtarget: /resourcekey/hash.filetype(?querystrings)
                if (!string.IsNullOrEmpty(resourceRawTarget))
                {
                    string[] rawTargets = resourceRawTarget.Split(new char[] { '/' });

                    if (rawTargets.Length >= 3)
                    {
                        string resourceKeyHash = rawTargets[1];
                        string resourceRest = rawTargets[2];

                        Resource resource = Resource.Get(resourceKeyHash);

                        if (resource != null)
                        {
                            string ext = resource.FileExtension;
                            string[] resourceRestParts = resourceRest.Split(new char[] { '?' }, 2);
                            string resourceHash = resourceRestParts[0];

                            if (resourceHash.EndsWith(ext))
                            {
                                notFound = false;
                                ResourceAsyncResult resourceResult = new ResourceAsyncResult(context, cb, extraData);
                                result = resourceResult;

                                string ifModified = request.Headers["If-Modified-Since"];

                                if (resource.Cache && !string.IsNullOrEmpty(ifModified) && ifModified == year2000)
                                {
                                    TimeSpan oneYear = TimeSpan.FromDays(365);
                                    DateTime expires = DateTime.Now.Add(oneYear);
                                    string expiresStr = expires.ToString("r");

                                    response.Status = "304 Not Modified";
                                    response.AppendHeader("Cache-Control", "public, max-age=" + oneYear.TotalSeconds + "");
                                    response.AppendHeader("Last-Modified", year2000);
                                    response.AppendHeader("Expires", expiresStr);

                                    response.AppendHeader("Content-Type", resource.MimeType);
                                    response.AppendHeader("Vary", "Accept-Encoding");

                                    resourceResult.SetCompleted();
                                }
                                else
                                {
                                    if (resource.Cache && resource.BeginHandler == null)
                                    {
                                        //response.Cache.SetCacheability(HttpCacheability.Public);
                                        //response.Cache.SetLastModified(new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc));
                                        //response.Expires = 518400;
                                        TimeSpan oneYear = TimeSpan.FromDays(365);
                                        DateTime expires = DateTime.Now.Add(oneYear);

                                        string expiresStr = expires.ToString("r");

                                        response.AppendHeader("Cache-Control", "public, max-age=" + oneYear.TotalSeconds + "");
                                        response.AppendHeader("Last-Modified", year2000);
                                        response.AppendHeader("Expires", expiresStr);
                                        //response.AppendHeader("Pragma", "no-cache");
                                    }
                                    else
                                    {
                                        //response.Cache.SetCacheability(HttpCacheability.NoCache);
                                        response.AppendHeader("Cache-Control", "private, no-store, no-cache, must-revalidate, post-check=0, pre-check=0");
                                        response.AppendHeader("Pragma", "no-cache");

                                        DateTime expires = DateTime.Now.Add(TimeSpan.FromDays(365));

                                        string expiresStr = expires.ToString("r");

                                        response.AppendHeader("Expires", expiresStr);
                                    }

                                    if (resource.AccessControlAllowOrigin != null)
                                        response.AppendHeader("Access-Control-Allow-Origin", resource.AccessControlAllowOrigin);
                                    if (resource.AccessControlAllowCredentials)
                                        response.AppendHeader("Access-Control-Allow-Credentials", "true");
                                    
                                    response.AppendHeader("Content-Type", resource.MimeType);

                                    if (resource.Compressed) Compress(context);

                                    if (resource.BeginHandler != null)
                                    {
                                        if (resource.BufferOutput == false)
                                            response.BufferOutput = false;

                                        resourceResult.ResourceOutput = new ResourceOutput();
                                        resourceResult.Resource = resource;
                                        resource.BeginHandler(resourceResult);
                                    }
                                    else
                                    {
                                        response.Status = "200 OK";

                                        if (resource.OriginalData != null || resource.groupSources != null)
                                        {
                                            byte[] data = resource.Data;
                                            int bufferLength = 4096;
                                            int length = data.Length;
                                            int position = 0;
                                            do
                                            {
                                                int left = (length - position) > bufferLength ? bufferLength : (length - position);
                                                response.OutputStream.Write(data, position, left);
                                                position += left;
                                            }
                                            while (position < length);
                                        }

                                        resourceResult.SetCompleted();
                                    }
                                }
                            }
                        }
                    }
                }

                #endregion
            }
            
            if (notFound)
            {
                result = new AsyncResult(context, cb, extraData);
                response.Status = "404 Not Found";
                response.TrySkipIisCustomErrors = true;
                result.SetCompleted();
            }            

            return result;
        }

        public void EndProcessRequest(IAsyncResult result)
        {
            if (result.GetType() == typeof(ResourceAsyncResult))
            {
                ResourceAsyncResult resourceResult = (ResourceAsyncResult)result;
                HttpContext context = resourceResult.Context;
                HttpResponse response = context.Response;
                Resource resource = resourceResult.Resource;

                if (resource != null)
                {
                    if (resource.EndHandler != null)
                    {
                        ResourceEndProcessRequest proc = resource.EndHandler;
                        if (proc != null) proc(resourceResult);
                    }
                    if (resource.BeginHandler != null)
                    {
                        ResourceOutput resourceOutput = resourceResult.ResourceOutput;
                        if (resourceOutput.Data != null)
                        {
                            byte[] data = resourceOutput.GetData(resource);

                            int bufferLength = 4096;
                            int length = data.Length;
                            int position = 0;
                            do
                            {
                                int left = (length - position) > bufferLength ? bufferLength : (length - position);
                                response.OutputStream.Write(data, position, left);
                                position += left;
                            }
                            while (position < length);
                        }
                    }
                }
            }
        }

        #endregion

        #region Application Handler

        internal void ScriptDataBinding(HttpContext context, ScriptData data)
        {
            SessionClient client = Session.Client(context);

            HttpRequest request = context.Request;

            UserSettings us = new UserSettings();

            data.System("serverTime", DateTime.UtcNow);
            data.System("clientID", client.ClientID);
            data.System("protocol", request.IsSecureConnection ? "https" : "http");
            data.System("titleFormat", Settings.TitleFormat);
            data.System("titleEmpty", Settings.TitleEmpty);

            data.System("sizeGroups", Settings.SizeGroups);

            if (Settings.EnableUI)
            {
                data.System("urlPrefix", Settings.UrlPrefix);
                data.System("ajaxify", Settings.Ajaxify);
                data.System("contentProviderUrl", Resource.GetPath("xhr_content_provider"));

                Color c0 = new Color(us.Get("COLOR0"));
                Color c100 = new Color(us.Get("COLOR100"));

                data.System("colorAccent", string.Format("#{0}", us.Get("COLORACCENT")));
                data.System("color0", string.Format("#{0}", us.Get("COLOR0")));
                data.System("color100", string.Format("#{0}", us.Get("COLOR100")));

                data.System("fontHeadings", Settings.FontHeadings);
                data.System("fontBody", Settings.FontBody);
            }

            if (Settings.UseDomain)
            {
                data.System("baseDomain", Settings.BaseDomain);
                data.System("pageDomain", Settings.PageDomain);

                if (Settings.EnableLive)
                {
                    data.System("streamDomain", client.StreamSubDomain + "." + Settings.StreamDomain);
                    data.System("streamPath", Resource.GetPath("xhr_stream"));
                }
            }

            data.System("providerPath", Resource.GetPath("xhr_provider"));

            OnScriptDataBinding(context, data);
        }

        internal void StyleSheetDataBinding(HttpContext context, StyleSheetData data)
        {
            UserSettings us = new UserSettings(); // TODO: from identity

            if (Settings.EnableUI)
            {
                Color c0 = new Color(us.Get("COLOR0"));
                Color c100 = new Color(us.Get("COLOR100"));

                data.Add("._FH", "font-family: \"" + Settings.FontHeadings + "\"");
                data.Add("._FB", "font-family: \"" + Settings.FontBody + "\"; color: #" + c0.Hex() + "");

                //int ro = 20;
                //int roa = 100 / ro;

                //float rr = ((float)(c100.Red - c0.Red) / ro);
                //float rg = ((float)(c100.Green - c0.Green) / ro);
                //float rb = ((float)(c100.Blue - c0.Blue) / ro);

                //Service.Event("" + rr);

                //for (int i = 0; i <= ro; i++)
                //{
                //    int r = (int)Math.Ceiling((float)c0.Red + (rr * (float)i));
                //    int g = (int)Math.Ceiling((float)c0.Green + (rg * (float)i));
                //    int b = (int)Math.Ceiling((float)c0.Blue + (rb * (float)i));

                //    Color com = new Color(r, g, b);
                //    string comh = com.Hex();
                //    data.Add("._C" + (i * roa), "color: #" + comh);
                //    data.Add("._CB" + (i * roa), "background-color: #" + comh);
                //}

                //Color ca = new Color(us.Get("COLORACCENT"));
                //string cah = ca.Hex();

                //data.Add("._CA", "color: #" + cah);
                //data.Add("._CBA", "background-color: #" + cah);

                //-webkit-text-stroke: 0.2px; -webkit-text-shadow: 0 0 1px rgba(51,51,51,0.2); 
                data.Add("body", "background-color: #" + us.Get("COLORBACKGROUND"));
            }


            OnStyleSheetDataBinding(context, data);
        }

        protected virtual void OnInit() { }

        protected virtual void OnUrlRewrite() { }

        protected virtual void OnResourceLoad() { }

        protected virtual void OnScriptDataBinding(HttpContext context, ScriptData data) { }

        protected virtual void OnStyleSheetDataBinding(HttpContext context, StyleSheetData data) { }

        #endregion

        #region Methods

        internal static void Compress(HttpContext context)
        {
            // Gzip or Deflate if available
            HttpResponse response = context.Response;
            HttpRequest request = context.Request;

            bool gzipped = false;
            bool deflated = false;

            string acceptEncoding = request.Headers["Accept-Encoding"];
            if (!string.IsNullOrEmpty(acceptEncoding) && acceptEncoding.Contains("gzip"))
                gzipped = true;
            else if (!string.IsNullOrEmpty(acceptEncoding) && acceptEncoding.Contains("deflate"))
                deflated = true;

            if (gzipped || deflated)
            {
                if (gzipped)
                {
                    response.Filter = new System.IO.Compression.GZipStream(response.Filter, System.IO.Compression.CompressionMode.Compress);
                    response.AppendHeader("Content-Encoding", "gzip");
                }
                else if (deflated)
                {
                    response.Filter = new System.IO.Compression.DeflateStream(response.Filter, System.IO.Compression.CompressionMode.Compress);
                    response.AppendHeader("Content-Encoding", "deflate");
                }

                response.AppendHeader("Vary", "Content-Encoding");
            }
        }
        #endregion
    }

    public class Request
    {
        public DesignType Design
        {
            get
            {
                if (Settings.EnableUI)
                {
                    HttpContext context = HttpContext.Current;
                    if (context.Items["designtype"] == null)
                        throw new Exception("DesignType is still not available");
                    else
                    {
                        Aphysoft.Share.DesignType designType = (Aphysoft.Share.DesignType)context.Items["designtype"];
                        return designType;
                    }
                }
                else return DesignType.Lite;
            }
        }

        public Request()
        {

        }
    }

    public class AsyncResult : IAsyncResult
    {
        #region Fields

        protected HttpContext context;

        protected AsyncCallback callback;

        protected object asyncState;

        protected bool isCompleted = false;

        protected object responseObject;

        #endregion

        #region Constructors

        public AsyncResult(HttpContext context, AsyncCallback callback, object asyncState)
        {
            this.context = context;
            this.callback = callback;
            this.asyncState = asyncState;
        }

        #endregion

        #region IAsyncResult Members

        public object AsyncState
        {
            get { return asyncState; }
        }

        public WaitHandle AsyncWaitHandle
        {
            get { throw new InvalidOperationException("ASP.NET Should never use this property"); }
        }

        public bool CompletedSynchronously
        {
            get { return false; }
        }

        public bool IsCompleted
        {
            get { return isCompleted; }
        }

        #endregion

        public HttpContext Context
        {
            get { return context; }
        }

        public HttpResponse Response
        {
            get { return context.Response; }
        }

        public HttpRequest Request
        {
            get { return context.Request; }
        }

        public object ResponseObject
        {
            get { return responseObject; }
            set { responseObject = value; }
        }

        public void SetCompleted()
        {
            isCompleted = true;

            if (callback != null)
                callback(this);
        }

        private int tag;

        public int Tag
        {
            get { return tag; }
            set { tag = value; }
        }
    }

    internal enum ExecutionTypes
    {
        Default,
        Resources,
        Services,
        Favicon
    }
}
