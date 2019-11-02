using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    public class EdgeMessageEventArgs : EdgeEventArgs
    {
        #region Fields

        public int MessageID { get; internal set; }

        public BaseMessage Message { get; internal set; }

        public int ResponseMessageID { get; internal set; }

        #endregion

        #region Constructors

        public EdgeMessageEventArgs(Edge edge) : base(edge)
        {
        }

        #endregion

        #region Methods

        public bool Reply(BaseMessage message) => edge.Send(message, MessageID);

        public bool Reply<T>(BaseMessage message, out T response) where T : BaseMessage => edge.Send(message, out response, MessageID);

        public bool Return() => edge.Send(Message, MessageID);

        public bool ReplyOK() => edge.Send(new OKMessage(), MessageID);

        public bool ReplyNOK() => edge.Send(new NOKMessage(), MessageID);

        #endregion
    }
}
