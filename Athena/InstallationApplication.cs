using Aphysoft.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Athena
{
    public class InstallationApplication
    {
        #region Fields

        private string name;

        public string Name { get => name; }

        private int version;

        public int Version { get => version; }

        private InstallationFile[] files;

        public InstallationFile[] Files { get => files; }

        private bool active;

        public bool Active { get => active; }

        #endregion

        #region Constructors

        public InstallationApplication(string name, int version, InstallationFile[] files, bool active)
        {
            this.name = name;
            this.version = version;
            this.files = files;
            this.active = active;
        }

        #endregion
    }
}
