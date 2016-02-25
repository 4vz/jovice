
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Aphysoft.Common;
using Aphysoft.Share;

namespace Jovice
{
    public class Jovice : Share
    {
        #region Database

        private static Database jovice = null;

        public static new Database Database
        {
            get
            {
                if (jovice == null)
                {
                    string database = Configuration.Settings("database");
                    string connectionString = string.Format("Data Source={0};Initial Catalog=jovice;User ID=telkom.center;Password=t3lk0mdotc3nt3r;async=true", database);
                    jovice = new Database(connectionString, DatabaseType.SqlServer);
                }
                return jovice;
            }
        }

        #endregion

        #region Methods

        protected override void OnInit()
        {
            Client.Init();
        }

        #endregion

    }

    public class Center
    {
        #region Database

        private static Database center = null;

        public static Database Database
        {
            get
            {
                if (center == null)
                {
                    string database = Configuration.Settings("database");
                    string connectionString = string.Format("Data Source={0};Initial Catalog=center;User ID=telkom.center;Password=t3lk0mdotc3nt3r;async=true", database);
                    center = new Database(connectionString, DatabaseType.SqlServer);
                }
                return center;
            }
        }

        #endregion
    }
}
