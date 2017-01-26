
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using Aphysoft.Share;

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
                    string database = ConfigurationHelper.Settings("database");
                    if (database == null)
                    {
                        database = "localhost";
#if DEBUG
                        database = "localhost\\SQLEXPRESS";
#endif
                    }
                    string connectionString = string.Format("Data Source={0};Initial Catalog=jovice;User ID=telkom.center;Password=t3lk0mdotc3nt3r;async=true", database);
                    jovice = new Database(connectionString, DatabaseType.SqlServer);
                }
                return jovice;
            }
        }

        #endregion
    }
}
