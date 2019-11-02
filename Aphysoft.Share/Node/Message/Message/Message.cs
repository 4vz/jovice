using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    [Serializable]
    public abstract class Message : BaseMessage
    {
        #region Fields

        public string Data { get; set; } = null;

        #endregion

        #region Constructors

        public Message(string data) => Data = data;

        #endregion
    }
}
