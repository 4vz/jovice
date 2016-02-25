using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aphysoft.Common;

namespace Aphysoft.Share
{
    public partial class Settings
    {
        #region Application Settings

        private static Dictionary<string, string> applicationSettings = new Dictionary<string, string>();

        public static string Get(string setting)
        {
            string settingLower = setting.ToLower();

            if (applicationSettings.ContainsKey(settingLower))
                return applicationSettings[settingLower];
            else
                return null;
        }

        private static List<string> notInApplicationSettings = new List<string>();

        #endregion

        #region Settings Setup

        #region Properties and Methods

        private static bool applicationSettingsLoaded = false;
        private static void ReadApplicationSettings()
        {
            // read all
            string sql = "SELECT S_Name, S_Value FROM Setting";

            Result r = Share.Database.Query(sql);

            if (r.Count > 0)
            {
                foreach (Row row in r)
                {
                    string name = row["S_Name"].ToString();
                    string value = row["S_Value"].ToString();

                    if (!notInApplicationSettings.Contains(name))
                    {
                        if (applicationSettings.ContainsKey(name.ToLower()))
                            applicationSettings[name.ToLower()] = value;
                        else
                            applicationSettings.Add(name.ToLower(), value);
                    }
                }
            }
        }

        private static bool ReadString(string key, ref string target)
        {
            notInApplicationSettings.Add(key);

            Result r = Share.Database.Query("select S_Value FROM Setting where S_Name = {0}", key);

            if (r.Count > 0)
            {
                target = r[0]["S_Value"].ToString();
                return true;
            }
            else return false;
        }
        private static bool ReadArrayString(string key, ref string[] target)
        {
            string arrayString = "";

            if (ReadString(key, ref arrayString))
            {
                target = arrayString.Split(new char[] { ',' });
                return true;
            }
            else return false;
        }
        private static bool ReadArrayInteger(string key, ref int[] target)
        {
            string[] arrayString = null;

            if (ReadArrayString(key, ref arrayString))
            {
                List<int> arrayTarget = new List<int>();

                foreach (string t in arrayString)
                {
                    int parsedInt;

                    if (int.TryParse(t, out parsedInt))
                    {
                        arrayTarget.Add(parsedInt);
                    }
                    else arrayTarget.Add(0);
                }

                target = arrayTarget.ToArray();
                return true;
            }
            else return false;
        }
        private static bool ReadBoolean(string key, ref bool target)
        {
            string boolstring = "false";
            //target = false;

            if (ReadString(key, ref boolstring))
            {
                target = (boolstring.ToLower() == "true") ? true : false;
                return true;
            }
            else return false;
        }
        private static bool ReadInteger(string key, ref int target)
        {
            string integerstring = "0";

            if (ReadString(key, ref integerstring))
            {
                return int.TryParse(integerstring, out target);
            }
            else return false;
        }
        private static bool ReadDouble(string key, ref double target)
        {
            string doublestring = "0";

            if (ReadString(key, ref doublestring))
            {
                return double.TryParse(doublestring, out target);
            }
            else return false;
        }
        private static bool ReadDefaultBool(string key, ref DefaultBool target)
        {
            string boolstring = "false";

            if (ReadString(key, ref boolstring))
            {
                if (boolstring == "true")
                    target = DefaultBool.True;
                else if (boolstring == "false")
                    target = DefaultBool.False;
                else
                    target = DefaultBool.Default;
                return true;
            }
            else return false;
        }
        
        #endregion

        #region Init

        private static void SettingsInit()
        {
            Database s = Share.Database;

            if (s.Test())
            {
                ReadSettings();
                ReadApplicationSettings();
            }
        }

        internal static void ClientInit()
        {
            if (!applicationSettingsLoaded)
            {
                applicationSettingsLoaded = true;
                physicalApplicationPath = System.Web.HttpRuntime.AppDomainAppPath;
                SettingsInit();
            }
        }

        internal static void ServerInit()
        {
            if (!applicationSettingsLoaded)
            {
                applicationSettingsLoaded = true;
                physicalApplicationPath = "";
                SettingsInit();
            }
        }

        #endregion

        #endregion
    }
}
