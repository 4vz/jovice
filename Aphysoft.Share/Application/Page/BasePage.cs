using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;

using System.Diagnostics;

using System.Collections.Generic;
using System.Text;
using System.Web.Script.Serialization;

// TODO
namespace Aphysoft.Share
{
    /// <summary>
    /// TODO: Not Done
    /// Provides the base class for pages in Aphysoft Share application environment that are used to bind every 
    /// application services and simplify the application structure. This class is a partial type class which
    /// is mean you can add other functionality without change this class.
    /// </summary>
    public partial class BasePage : System.Web.UI.Page
    {
        #region Fields

        private DesignType design = DesignType.Full;

        public DesignType Design
        {
            get { return design; }
            set { design = value; }
        }

        protected List<string> IncludeResources = new List<string>();

        protected List<string> OnloadScripts = new List<string>();

        #endregion

        #region Constructor

        public BasePage()
        {
        }

        #endregion

        #region Methods

        protected override void OnPreInit(EventArgs e)
        {
            // Instancing
            HttpContext.Current.Items.Add("current_page", this);

            base.OnPreInit(e);
        }
        
        protected override void OnInit(EventArgs e)
        {
            if (Design == DesignType.Full)
            {
                Add(Resource.CommonResourceScript);
                Add(Resource.CommonResourceCSS);    
            }
            else if (Design == DesignType.Lite)
            {
                //Add("script_jquery_lite");
                //Add("script_share_lite");
            }
            
            base.OnInit(e);
        }
        
        protected override void OnPreRenderComplete(EventArgs e)
        {
            // begin title
            if (string.IsNullOrEmpty(Title)) Title = Settings.FullName;
            else
            {
                string fti = Settings.TitleFormat;
                fti = fti.Replace("{PAGETITLE}", Title);
                fti = fti.Replace("{FULLNAME}", Settings.FullName);
                Title = fti;
            }

            // include resource
            foreach (string key in IncludeResources)
            {
                Resource resource = Resource.Get(key);
                string path = Resource.GetPath(key);

                if (resource.FileExtension == ".js")
                {
                    HtmlGenericControl scriptTag = new HtmlGenericControl();
                    scriptTag.TagName = "script";
                    scriptTag.Attributes.Add("type", "text/javascript");
                    scriptTag.Attributes.Add("language", "javascript");
                    scriptTag.Attributes.Add("src", path);
                    Page.Header.Controls.Add(scriptTag);
                }
                else if (resource.FileExtension == ".css")
                {
                    HtmlLink cssTag = new HtmlLink();
                    cssTag.EnableViewState = false;
                    cssTag.Attributes.Add("type", "text/css");
                    cssTag.Attributes.Add("rel", "stylesheet");
                    cssTag.Href = path;
                    Page.Header.Controls.Add(cssTag);
                }
            }

            ClientScriptManager cs = Page.ClientScript;

            HttpContext context = Context;
            HttpResponse response = context.Response;
            HttpRequest request = context.Request;

            ScriptData bridgeScript = new ScriptData();
            Share.Current.ScriptDataBinding(context, bridgeScript);

            StringBuilder sb = new StringBuilder();

            sb.Append("share.data({ " + string.Join(", ", bridgeScript.GetArrayString()) + " });");
            
            // content
            if (OnloadScripts.Count > 0)
            {
                StringBuilder calls = new StringBuilder();
                foreach (string script in OnloadScripts)
                {
                    calls.Append(script);
                    calls.Append("\r\n");
                }

                StringBuilder callScript = new StringBuilder();

                callScript.Append("$(function() {\r\n");
                callScript.Append(calls.ToString());
                callScript.Append("});\r\n");

                cs.RegisterClientScriptBlock(GetType(), "startup_calls", callScript.ToString(), true);
            }

            cs.RegisterClientScriptBlock(GetType(), "startup_data", WebUtilities.Minifier.MinifyJavaScript(sb.ToString()), true);
            
            // TODO
            Response.AppendHeader("Cache-Control", "private, no-cache, no-store, must-revalidate");
            Response.AppendHeader("Pragma", "no-cache");
            Response.AppendHeader("Expires", "Sat, 01 Jan 2000 00:00:00 GMT");
            Response.Headers["Content-Type"] = "text/html; charset=utf-8"; 

            base.OnPreRenderComplete(e);
        }

        public void Add(string key)
        {
            Resource resource = Resource.Get(key);

            if (resource != null)
            {
                IncludeResources.Add(key);
            }
        }

        public void AddOnloadScript(string script)
        {
            if (!string.IsNullOrEmpty(script))
            {
                OnloadScripts.Add(script);
            }
        }

        #endregion
    }

}
