using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    [Serializable]
    public class InfoInstallMessage : BaseMessage
    {
        #region Fields

        public InstallationFile[] Files { get; set; }

        public InstallationFile Apps { get; set; }

        #endregion

        #region Constructors

        public InfoInstallMessage()
        {

        }

        #endregion
    }
}
