using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    public class EdgeProgessEventArgs : EdgeEventArgs
    {
        #region Fields

        public int Length { get; }

        public int Current { get; }

        #endregion

        #region Constructors

        public EdgeProgessEventArgs(Edge edge, int length, int current) : base(edge)
        {
            Length = length;
            Current = current;
        }

        #endregion

        #region Methods

        #endregion
    }
}
