using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Web.Configuration;
using System.IO;

namespace Aphysoft.Share
{
    public static class ApplicationConfiguration
    {
        private static Configuration webConfig = null;

        private static Configuration WebConfig
        {
            get
            {
                if (webConfig == null)
                    webConfig = WebConfigurationManager.OpenWebConfiguration("~");

                return webConfig;
            }
        }

        private static string applicationPath = string.Empty;

        public static string ApplicationPath
        {
            get
            {
                if (applicationPath == string.Empty)
                {
                    string confPath = WebConfig.FilePath;
                    
                    FileInfo f = new FileInfo(confPath);
                    applicationPath = string.Format("{0}\\", f.DirectoryName);                    
                }

                return applicationPath;
            }
        }

        public static string Read(string key)
        {

            int c = ConfigurationManager.AppSettings.Count;

            if (WebConfig.AppSettings.Settings.Count > 0)
            {
                KeyValueConfigurationElement customSetting = WebConfig.AppSettings.Settings[key];

                if (customSetting != null)
                    return customSetting.Value;
            }

            return "";
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