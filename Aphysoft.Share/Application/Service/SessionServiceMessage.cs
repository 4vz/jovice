using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aphysoft.Common;
using System.Web;

namespace Aphysoft.Share
{
    [Serializable]
    public class SessionServiceMessage : BaseServiceMessage
    {
        #region Fields

        private string sessionID;

        public string SessionID
        {
            get { return sessionID; }
            set { sessionID = value; }
        }

        private string clientID;

        public string ClientID
        {
            get { return clientID; }
            set { clientID = value; }
        }

        private string data;

        public string Data
        {
            get { return data; }
            set { data = value; }
        }

        #endregion

        #region Constructor

        internal SessionServiceMessage(string sessionID)
            : base()
        {
            this.sessionID = sessionID;
        }

        public SessionServiceMessage(HttpContext context)
            : base()
        {
            sessionID = (string)context.Items["sessionID"];
        }

        public SessionServiceMessage()
        {
            sessionID = null;
        }

        #endregion
    }
}
