using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    public class EdgeDisconnectedEventArgs : EdgeEventArgs
    {
        #region Fields

        #endregion

        #region Constructors

        public EdgeDisconnectedEventArgs(Edge edge) : base(edge)
        {
        }

        #endregion
    }
}
