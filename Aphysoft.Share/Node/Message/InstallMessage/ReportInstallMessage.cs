using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    [Serializable]
    public class ReportInstallMessage : BaseMessage
    {
        #region Fields

        public int[] RequestID { get; set; }

        #endregion

        #region Constructors

        public ReportInstallMessage()
        {
            RequestID = null;
        }

        #endregion
    }
}
