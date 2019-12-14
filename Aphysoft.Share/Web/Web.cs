using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.Hosting;

using System.Diagnostics;
using System.Threading;
using System.Runtime.Serialization.Json;
using System.Configuration;
using System.Net;
using System.IO;



namespace Aphysoft.Share
{
    public abstract class Web : Node, IHttpModule, IHttpAsyncHandler, IRequiresSessionState
    {
        #region Consts

        const string year2000 = "Sat, 01 Jan 2000 00:00:00 GMT";

        #endregion

        #region Fields

        public static Database2 Database { get; private set; } = null;

        public static Edge Service { get; private set; } = null;

        public static Web Current { get; private set; } 
               
        private readonly static object initSync = new object();

        private static bool inited = false;        

        internal const int ShareVersion = 30;

        #endregion

        #region Core

        public void Init(HttpApplication application)
        {
            Current = this;

            application.Error += Application_Error;
            application.BeginRequest += Application_BeginRequest;
            application.AuthenticateRequest += Application_AuthenticateRequest;
            application.PostAuthenticateRequest += Application_PostAuthenticateRequest;
            application.AuthorizeRequest += Application_AuthorizeRequest;
            application.PostAuthorizeRequest += Application_PostAuthorizeRequest;
            application.ResolveRequestCache += Application_ResolveRequestCache;
            application.PostResolveRequestCache += Application_PostResolveRequestCache;
            application.MapRequestHandler += Application_MapRequestHandler;
            application.PostMapRequestHandler += Application_PostMapRequestHandler;
            application.AcquireRequestState += Application_AcquireRequestState;
            application.PostAcquireRequestState += Application_PostAcquireRequestState;
            application.PreRequestHandlerExecute += Application_PreRequestHandlerExecute;
            application.EndRequest += Application_EndRequest;
            application.PreSendRequestHeaders += Application_PreSendRequestHeaders;
            application.PreSendRequestContent += Application_PreSendRequestContent;

            Start();

            Database = Database2.Web();
            Database.Retry += delegate(object sender, DatabaseExceptionEventArgs2 e)
            {
                if (Service != null && Service.IsConnected)
                {
                    Service.Event($"{e.Message}|{e.Sql}", "DATABASE");
                }
            };
            Database.QueryAttempts = 3;
        }

        public void Dispose()
        {
        }

        protected Web() : base()
        {
        }

        #endregion

        #region Events

        private void Application_Error(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;

            string error = $"Error: {context.Error.Message}{(context.Error.InnerException != null ? ", " + context.Error.InnerException.Message : "")} stack trace: {context.Error.StackTrace}";

            Quick.Log(error);

            Event(error);
        }

        private void Application_BeginRequest(object sender, EventArgs e)
        {
            HttpApplication application = (HttpApplication)sender;
            HttpContext context = HttpContext.Current;
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;

            response.TrySkipIisCustomErrors = true;

            if (!inited)
            {
                lock (initSync)
                {
                    if (!inited)
                    {
                        if (Database)
                        {
                            WebSettings.Init();

                            inited = true;

                            OnInit();

                            Resource.Init();

                            Provider.Init();

                            Content.Init();

                            OnResourceLoad();
                        }
                    }
                }
            }

            if (Database)
            {
               // We're not using internal session state.
               HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Disabled);

               string host = request.Headers["Host"];
               string currentExecutionPathLower = request.CurrentExecutionFilePath.ToLower();
               string appExecutionPath = request.AppRelativeCurrentExecutionFilePath;

               if (host == WebSettings.ApiDomain)
               {
                   if (!request.IsSecureConnection && WebSettings.Secure)
                   {
                       response.Redirect("https://" + WebSettings.ApiDomain + request.RawUrl);
                       response.End();
                   }

                   if (appExecutionPath.ToLower() == "~/favicon.ico") context.Items["provider"] = ExecutionType.Favicon;
                   else
                   {
                       context.Items["provider"] = ExecutionType.API;
                   }
               }
               else
               {
                   if (appExecutionPath.StartsWith("~/resources")) context.Items["provider"] = ExecutionType.Resources;
                   else if (appExecutionPath.ToLower() == "~/favicon.ico") context.Items["provider"] = ExecutionType.Favicon;
                   else if (host != WebSettings.Domain)
                   {
                       response.Redirect("http://" + WebSettings.Domain + request.RawUrl);
                       response.End();
                   }
                   else
                   {
                       if (!request.IsSecureConnection && WebSettings.Secure)
                       {
                           response.Redirect("https://" + WebSettings.Domain + request.RawUrl);
                           response.End();
                       }

                       context.Items["provider"] = ExecutionType.Default;

                        // redirect to url prefix
                        bool toUIPage = false;

                        if (!string.IsNullOrEmpty(WebSettings.UrlPrefix))
                        {
                            if (currentExecutionPathLower == "/" || currentExecutionPathLower == WebSettings.UrlPrefix)
                                response.Redirect(string.Format("{0}/", WebSettings.UrlPrefix), true);
                            else if (currentExecutionPathLower.StartsWith(WebSettings.UrlPrefix))
                                toUIPage = true;
                        }
                        else
                            toUIPage = true;

                        if (toUIPage)
                        {
                            HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Disabled);
                        }
                        else
                        {
                            // to non ASP.NET page.
                            // eg. html, htm, etc
                            // HttpSessionState is NOT AVAILABLE
                            if (currentExecutionPathLower.EndsWith(".aspx")) // sorry, we had to disable asp.net pages
                            {
                                response.Headers.Add("Content-Type", "text/html; charset=utf-8");
                                response.Write("UI is Enabled, we can't serve ASP.NET pages since HttpSessionState has been disabled.");
                                response.End();
                            }
                        }
                    }
               }
            }
            else
            {
                response.Status = "503 Service Unavailable";
                response.End();
            }
        }

        private void Application_AuthenticateRequest(object sender, EventArgs e)
        {
        }

        private void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
        }

        private void Application_AuthorizeRequest(object sender, EventArgs e)
        {
        }

        private void Application_PostAuthorizeRequest(object sender, EventArgs e)
        {
        }
        
        private void Application_ResolveRequestCache(object sender, EventArgs e)
        {
        }

        private void Application_PostResolveRequestCache(object sender, EventArgs e)
        {
        }

        private void Application_MapRequestHandler(object sender, EventArgs e)
        {
        }

        private void Application_PostMapRequestHandler(object sender, EventArgs e)
        {
        }

        private void Application_AcquireRequestState(object sender, EventArgs e)
        {
            Session.Start(HttpContext.Current);
        }

        private void Application_PostAcquireRequestState(object sender, EventArgs e)
        {
        }

        private void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            HttpContext context = HttpContext.Current;
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;

            ExecutionType type = (ExecutionType)context.Items["provider"];

            if (type == ExecutionType.Default)
            {
                Content.Render(context);
                Compress(context);
                response.End();
            }
        }

        private void Application_EndRequest(object sender, EventArgs e)
        {
        }

        private void Application_PreSendRequestHeaders(object sender, EventArgs e)
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

        private void Application_PreSendRequestContent(object sender, EventArgs e)
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
            ExecutionType execution = (ExecutionType)context.Items["provider"];

            string currentExecutionPath = string.Format("~{0}", context.Request.RawUrl);
            string resourcesProviderExecutionPath = string.Format("~/resources").ToLower();

            AsyncResult result = null;

            int statusCode = 404;

            if (execution == ExecutionType.Resources) //   currentExecutionPath.StartsWith(resourcesProviderExecutionPath.ToLower()))
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
                                statusCode = -1;
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
            else if (execution == ExecutionType.API)
            {
                #region API
                                
                string[] queryAreas = currentExecutionPath.Split(new char[] { '?' }, 2);
                string[] paths = queryAreas[0].Split(StringSplitTypes.Slash);

                if (paths.Length >= 2)
                {
                    string api = paths[1].ToLower();

                    if (api.Length > 0)
                    {
                        APIRegister apiRegister = API.Get(api);

                        if (apiRegister != null)
                        {
                            if (paths.Length >= 3)
                            {
                                if (QueryString.IsExist())
                                {
                                    if (QueryString.Exists("key"))
                                    {
                                        if (QueryString.ValuesCount("key") > 1)
                                        {
                                            statusCode = -1;
                                            result = new AsyncResult(context, cb, extraData);
                                            response.Status = "400 Bad Request";
                                            result.SetCompleted();
                                        }
                                        else
                                        {
                                            string apiKey = QueryString.GetValue("key");

                                            List<string> pas = new List<string>();
                                            int ipath = 0;
                                            foreach (string path in paths)
                                            {
                                                if (ipath > 0) pas.Add(path);
                                                ipath++;
                                            }

                                            Result2 r = Database.Query(@"
select AA_ID from [ApiSubscription], [Api], [ApiAccess] where AS_AA = AA_ID and AS_AP = AP_ID and AA_Active = 1 and AP_Name = {0} and AA_Key = {1}", api, apiKey);

                                            if (r.Count == 1)
                                            {
                                                string aaID = r[0]["AA_ID"].ToString();

                                                statusCode = -1;
                                                APIAsyncResult resourceResult = new APIAsyncResult(context, cb, extraData);
                                                result = resourceResult;

                                                APIPacket packet = apiRegister.APIRequest(resourceResult, pas.ToArray(), aaID);

                                                if (packet != null)
                                                {
                                                    if (packet is ErrorAPIPacket)
                                                    {
                                                        response.StatusCode = ((ErrorAPIPacket)packet).HttpStatusCode;
                                                    }

                                                    if (QueryString.GetValue("nocompress") == "true") { }
                                                    else Compress(context);

                                                    response.AppendHeader("Cache-Control", "private, no-store, no-cache, must-revalidate, post-check=0, pre-check=0");
                                                    response.AppendHeader("Pragma", "no-cache");

                                                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(packet.GetType());
                                                    serializer.WriteObject(response.OutputStream, packet);

                                                    response.ContentType = "application/json";

                                                    result.SetCompleted();
                                                }
                                                else
                                                {
                                                    statusCode = -1;
                                                    result = new AsyncResult(context, cb, extraData);
                                                    response.Status = "500 Internal Server Error";
                                                    result.SetCompleted();
                                                }
                                            }
                                            else
                                            {
                                                statusCode = -1;
                                                result = new AsyncResult(context, cb, extraData);
                                                response.Status = "403 Forbidden";
                                                result.SetCompleted();
                                            }

                                        }
                                    }
                                    else
                                    {
                                        statusCode = -1;
                                        result = new AsyncResult(context, cb, extraData);
                                        response.Status = "403 Forbidden";
                                        result.SetCompleted();
                                    }
                                }
                                else
                                {
                                    statusCode = -1;
                                    result = new AsyncResult(context, cb, extraData);
                                    response.Status = "403 Forbidden";
                                    result.SetCompleted();
                                }
                            }
                            else
                            {
                                statusCode = -1;
                                result = new AsyncResult(context, cb, extraData);
                                response.Status = "400 Bad Request";
                                result.SetCompleted();
                            }
                        }
                        else
                        {
                            statusCode = -1;
                            result = new AsyncResult(context, cb, extraData);
                            response.Status = "501 Not Implemented";
                            result.SetCompleted();
                        }
                    }
                    else
                    {
                        if (Apps.Config("SSL", "false") == "true") response.Redirect("https://" + Apps.Config("DOMAIN"));
                        else response.Redirect("http://" + Apps.Config("DOMAIN"));
                        response.End();
                        result.SetCompleted();
                    }
                }
                
                #endregion
            }
            
            if (statusCode == 404)
            {
                result = new AsyncResult(context, cb, extraData);
                response.Status = "404 Not Found";                
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
                    resource.EndHandler?.Invoke(resourceResult);
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

        internal void ScriptDataBinding(HttpContext context, ScriptData scriptData)
        {
            SessionClient client = Session.Client(context);

            HttpRequest request = context.Request;

            scriptData.System("serverTime", DateTime.UtcNow);
            scriptData.System("clientID", client.ClientId);
            scriptData.System("protocol", request.IsSecureConnection ? "https" : "http");
            scriptData.System("defaultTitle", WebSettings.DefaultTitle);
            scriptData.System("urlPrefix", WebSettings.UrlPrefix);
            scriptData.System("ajaxify", true);
            scriptData.System("contentProviderUrl", Resource.GetPath("xhr_content_provider"));
            scriptData.System("colorAccent", string.Format("#{0}", WebSettings.ColorAccent));
            scriptData.System("color0", string.Format("#{0}", WebSettings.Color0));
            scriptData.System("color100", string.Format("#{0}", WebSettings.Color100));
            scriptData.System("fontBody", WebSettings.FontBody);
            scriptData.System("domain", WebSettings.Domain);
            scriptData.System("streamDomain", client.StreamSubDomain + "." + WebSettings.StreamDomain + client.StreamPort);
            scriptData.System("streamPath", Resource.GetPath("xhr_stream"));
            scriptData.System("providerPath", Resource.GetPath("xhr_provider"));

            OnScriptDataBinding(context, scriptData);
        }

        internal void StyleSheetDataBinding(HttpContext context, StyleSheetData styleSheetData)
        {
            ShareColor c0 = new ShareColor(Apps.Config("COLOR0", "000000"));

            styleSheetData.Add("._FB", "font-family: \"" + WebSettings.FontBody + "\"; color: #" + c0.Hex() + "");
            styleSheetData.Add("body", "background-color: #" + WebSettings.ColorBackground);

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



            OnStyleSheetDataBinding(context, styleSheetData);
        }

        protected virtual void OnInit() { }

        protected virtual void OnUrlRewrite() { }

        protected virtual void OnResourceLoad() { }

        protected virtual void OnScriptDataBinding(HttpContext context, ScriptData data) { }

        protected virtual void OnStyleSheetDataBinding(HttpContext context, StyleSheetData data) { }
        
        #endregion

        #region Methods

        public static string Version()
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Build.ToString() + "." + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.Revision;
        }

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

        #region Node

        protected override void OnStart()
        {
            Service = BeginEdge(IPAddress.Loopback, "WEBSERVICE");


            while (IsRunning)
            {
                Thread.Sleep(5000);
            }
        }

        public new void Event(string message, string context)
        {
            Service?.Event(message, context);
        }

        public new void Event(string message)
        {
            Service?.Event(message);
        }

        #endregion
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

    internal enum ExecutionType
    {
        Default,
        API,
        Resources,
        Favicon
    }
}
