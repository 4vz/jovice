using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aphysoft.Share;

namespace Center
{
    public class Center : Web
    {
        protected override void OnInit()
        {
            //Client.Init();
        }

        protected override void OnResourceLoad()
        {
            #region API

            API.Register("tselsites", APIs.TselSites.APIRequest);

            #endregion
        }

        protected override void OnScriptDataBinding(HttpContext context, ScriptData data)
        {
        }
    }
}