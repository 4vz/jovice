using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    [Serializable]
    internal class ClientEndMessage : BaseClientMessage
    {
        #region Constructors

        public ClientEndMessage(string clientID, string sessionId) : base(clientID, sessionId)
        {
        }

        public ClientEndMessage() : base(null, null)
        {

        }

        #endregion
    }
}
