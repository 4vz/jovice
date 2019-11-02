using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    public abstract class EdgeEventArgs : EventArgs
    {
        #region Fields

        protected Edge edge = null;

        public Edge Edge { get => edge; }

        #endregion

        #region Constructors

        public EdgeEventArgs(Edge edge)
        {
            this.edge = edge;
        }

        #endregion
    }
}
