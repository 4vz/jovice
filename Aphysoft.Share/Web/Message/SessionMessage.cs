using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Web;

namespace Aphysoft.Share
{
    [Serializable]
    public class SessionMessage : BaseMessage
    {
        #region Fields

        public string SessionID { get; set; }

        public string ClientID { get; set; }

        public string Data { get; set; }

        #endregion

        #region Constructor

        internal SessionMessage(string sessionId)
            : base()
        {
            this.SessionID = sessionId;
        }

        public SessionMessage(HttpContext context)
            : base()
        {
            SessionID = (string)context.Items["sessionId"];
        }

        public SessionMessage()
        {
            SessionID = null;
        }

        #endregion
    }
}
