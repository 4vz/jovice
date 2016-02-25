using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;

namespace Aphysoft.Share
{
    internal class LiteUIPage : UIPage
    {
        protected override void OnLoad(HttpContext context)
        {
            Add("script_jquery_lite");
            Add("script_share_lite");

            base.OnLoad(context);
        }

        protected override void OnRender(HttpContext context)
        {
            HttpResponse response = context.Response;

            // begin title
            response.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?><!DOCTYPE html PUBLIC \"-//WAPFORUM//DTD XHTML Mobile 1.0//EN\" \"http://www.wapforum.org/DTD/xhtml-mobile10.dtd\"><html xmlns=\"http://www.w3.org/1999/xhtml\"><head runat=\"server\"><title>");
            if (Title == null) response.Write(Settings.FullName);
            else
            {
                string fti = Settings.TitleFormat;
                fti = fti.Replace("{PAGETITLE}", Title);
                fti = fti.Replace("{FULLNAME}", Settings.FullName);
                response.Write(fti);
            }
            // end title
            response.Write("</title>");
            // meta
            response.Write("<meta charset=\"UTF-8\" />");
            response.Write("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0, user-scalable=0, maximum-scale=1.0\" />");
            response.Write("<meta name=\"apple-mobile-web-app-capable\" content=\"yes\" />");
            // include css resource
            foreach (string key in CssResources)
            {
                Resource resource = Resource.Get(key);
                string path = Resource.GetPath(key);
                response.Write("<link type=\"text/css\" rel=\"stylesheet\" href=\"" + path + "\" />");
            }
            // end head, begin body
            response.Write("</head><body>");
            // include script resources
            foreach (string key in ScriptResources)
            {
                Resource resource = Resource.Get(key);
                string path = Resource.GetPath(key);
                response.Write("<script type=\"text/javascript\" language=\"javascript\" src=\"" + path + "\"></script>");
            }
            // onload script
            if (OnloadScripts.Count > 0)
            {
                response.Write("<script type=\"text/javascript\">/*<![CDATA[*/$(function() {");
                foreach (string script in OnloadScripts)
                {
                    var clscript = script.Trim();
                    response.Write(clscript);
                    if (!clscript.EndsWith(";")) response.Write(";");
                }
                response.Write("});/*]]>*/</script>");
            }
            // end body
            response.Write("</body>");
            response.Write("</html>");

            base.OnRender(context);
        }
    }
}
