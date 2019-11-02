using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    [Serializable]
    internal abstract class BaseStreamMessage : SessionMessage
    {
        #region Constructor

        public BaseStreamMessage(string sessionId) : base(sessionId)
        {

        }
        #endregion
    }
}
