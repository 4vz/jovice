using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    [Serializable]
    internal class StreamMessage : BaseStreamMessage
    {
        #region Fields

        public int HostSessionIndex { get; set; } = -1;

        public bool MessageContinue { get; set; } = false;

        public string MessageType { get; set; } = null;

        public object MessageData { get; set; } = null;

        #endregion

        #region Constructors

        public StreamMessage(string sessionId) : base(sessionId)
        {

        }

        public StreamMessage() : base("")
        {

        }

        #endregion
    }
}
