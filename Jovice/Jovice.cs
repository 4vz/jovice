
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using Aphysoft.Share;
using System.Configuration;

namespace Center
{
    public class Jovice
    {
        #region Database

        private static Database jovice = null;

        public static Database Database
        {
            get
            {
                if (jovice == null)
                {
                    string key = "jovice";
#if DEBUG
                    key = "jovice_debug";
#endif
                    jovice = new Database(ConfigurationManager.AppSettings[key], DatabaseType.SqlServer);
                }
                return jovice;
            }
        }

        #endregion
    }
}
