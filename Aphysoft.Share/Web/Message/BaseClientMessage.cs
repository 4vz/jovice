using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    [Serializable]
    internal abstract class BaseClientMessage : BaseStreamMessage
    {
        #region Constructor

        public BaseClientMessage(string clientID, string sessionId) : base(sessionId)
        {
            ClientID = clientID;
        }

        #endregion
    }
}
