using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Web.Configuration;

namespace Aphysoft.Common
{
    public static class Configuration
    {
        public static string Settings(string key, string ifnull)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            string iva = null;
            if ((iva = ConfigurationManager.AppSettings[key]) != null)
                return iva;
            else
                return ifnull;
        }

        public static string Settings(string key)
        {
            return Settings(key, null);
        }

        public static string ConnectionString(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;

            ConnectionStringSettingsCollection connectionStrings = ConfigurationManager.ConnectionStrings;

            string connectionString = null;

            foreach (ConnectionStringSettings connection in connectionStrings)
            {
                string name = connection.Name;
                string provider = connection.ProviderName;
                string currentConnectionString = connection.ConnectionString;

                if (name == key)
                {
                    connectionString = currentConnectionString;
                    break;
                }
            }

            return connectionString;
        }
    }
}
