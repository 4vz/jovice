using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    [Serializable]
    internal class SessionClientMessage : SessionMessage
    {
        #region Fields

        public int Length { get; set; }

        public int Index { get; set; }

        #endregion

        #region Constructor

        public SessionClientMessage(string sessionId)
            : base(sessionId)
        {
        }

        public SessionClientMessage() : base()
        {
        }

        #endregion
    }
}
