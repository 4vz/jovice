using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    public class StreamInstanceData
    {
        #region Fields

        public string Type { get; set; }

        public object Data { get; set; }

        #endregion

        #region Constructor

        public StreamInstanceData(string type, object data)
        {
            Type = type;
            Data = data;
        }

        #endregion
    }
}
