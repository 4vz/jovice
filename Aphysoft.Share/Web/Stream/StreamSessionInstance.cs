using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    public class StreamSessionInstance
    {
        #region Fields

        public string SessionId { get; }

        public Dictionary<string, StreamClientInstance> ClientInstances { get; }

        public string IPAddress { get; set; }

        #endregion

        #region Constructors

        public StreamSessionInstance(string sessionId)
        {
            ClientInstances = new Dictionary<string, StreamClientInstance>();

            SessionId = sessionId;
        }

        #endregion

        #region Methods

        public int GetAvailableIndex()
        {
            int lind = -1;

            foreach (KeyValuePair<string, StreamClientInstance> pair in ClientInstances)
            {
                StreamClientInstance clientInstance = pair.Value;

                int nind = clientInstance.SessionIndex;
                if ((nind - lind) > 1)
                    break;

                lind = nind;
            }

            return (lind + 1);
        }

        #endregion
    }

}
