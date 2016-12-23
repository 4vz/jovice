using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;


namespace Aphysoft.Share
{
    internal class UIPage
    {
        #region Fields

        protected string Title = null;

        protected List<string> ScriptResources = new List<string>();
        protected List<string> CssResources = new List<string>();

        protected List<string> OnloadScripts = new List<string>();

        #endregion

        #region Constructor

        public UIPage()
        {
        }

        #endregion

        #region Methods
      
        public void Render(HttpContext context)
        {
            OnLoad(context);
            OnRender(context);
        }

        protected virtual void OnLoad(HttpContext context)
        {
        }

        protected virtual void OnRender(HttpContext context)
        {
            HttpResponse response = context.Response;

            response.AppendHeader("Cache-Control", "private, no-cache, no-store, must-revalidate");
            response.AppendHeader("Pragma", "no-cache");
            response.AppendHeader("Expires", "Sat, 01 Jan 2000 00:00:00 GMT");
            response.Headers["Content-Type"] = "text/html; charset=utf-8"; 
        }
        
        public void Add(string key)
        {
            Resource resource = Resource.Get(key);

            if (resource != null)
            {
                if (resource.ResourceType == ResourceType.JavaScript)
                    ScriptResources.Add(key);
                else if (resource.ResourceType == ResourceType.CSS)
                    CssResources.Add(key);
            }
        }

        public void AddOnloadScript(string script)
        {
            if (!string.IsNullOrEmpty(script))
                OnloadScripts.Add(script);
        }

        #endregion
    }
}
