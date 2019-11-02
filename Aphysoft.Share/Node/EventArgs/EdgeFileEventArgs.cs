using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    public class EdgeFileEventArgs : EdgeEventArgs
    {
        #region Fields

        public int Reference { get; internal set; }

        public FileStream Stream { get; internal set; }

        public byte[] Checksum { get; internal set; }

        #endregion

        #region Constructors

        public EdgeFileEventArgs(Edge edge) : base(edge)
        {
        }

        #endregion

        #region Methods

        #endregion
    }
}