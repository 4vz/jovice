using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    [Serializable]
    public class EventMessage : Message
    {
        #region Constructors

        public EventMessage(string message) : base(message)
        {
        }

        #endregion
    }
}
