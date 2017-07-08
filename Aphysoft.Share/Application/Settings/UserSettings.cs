using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Aphysoft.Share
{
    public sealed class UserSettings : Ageable
    {
        #region User Settings Statics

        private static Dictionary<string, int> registers = new Dictionary<string, int>();
        private static Dictionary<string, string> defaultValues = new Dictionary<string, string>();
        private static Dictionary<string, UserSettings> savedSettings = new Dictionary<string, UserSettings>();
        private static UserSettings emptySettings = new UserSettings();

        private static object _sync = new object();

        private static List<string> _keys = null;
        public static List<string> Keys
        {
            get
            {
                bool createNew = false;

                if (_keys == null)
                {
                    createNew = true;
                }
                else
                {
                    if (_keys.Count != registers.Count)
                    {
                        createNew = true;

                        lock (_keys)
                        {
                            _keys.Clear();
                            _keys = null;
                        }
                    }
                }

                if (createNew)
                {
                    _keys = new List<string>();

                    foreach (KeyValuePair<string, int> vp in registers)
                    {
                        _keys.Add(vp.Key);
                    }
                }

                return _keys;
            }
        }

        public static void Register(string name, int settingID, string defaultValue)
        {
            if (!registers.ContainsKey(name) && !registers.ContainsValue(settingID))
            {
                lock (registers)
                {
                    registers.Add(name, settingID);
                    defaultValues.Add(name, defaultValue);
                }
            }
        }

        public static UserSettings Load(string userID)
        {
            if (userID == null)
                return emptySettings;
            else if (savedSettings.ContainsKey(userID))
            {
                UserSettings us = savedSettings[userID];
                us.Touch();
                return savedSettings[userID];
            }
            else
            {
                UserSettings u = new UserSettings();
                u.userID = userID;

                lock (_sync)
                {
                    // clean up aging properties                    
                    List<string> tobeRemoved = new List<string>();
                    foreach (KeyValuePair<string, UserSettings> kvp in savedSettings)
                        if (DateTime.Now.Subtract(kvp.Value.lastTouch).TotalSeconds >= 600) tobeRemoved.Add(kvp.Key);
                    foreach (string t in tobeRemoved) savedSettings.Remove(t);
                    
                    if (!savedSettings.ContainsKey(userID))
                        savedSettings.Add(userID, u);
                }

                return u;
            }
        }

        internal static void Init()
        {
            if (Settings.EnableUI)
            {
                Register("COLORACCENT", 101, Settings.ColorAccent.Hex());
                Register("COLORBACKGROUND", 102, Settings.ColorBackground.Hex());
                Register("COLOR0", 103, Settings.Color0.Hex());
                Register("COLOR100", 104, Settings.Color100.Hex());
            }
        }

        #endregion

        #region Constructors

        internal UserSettings() // TODO: make internal
        {
            values = new Dictionary<string, UserSettingValue>();
        }

        #endregion

        #region Fields

        private Dictionary<string, UserSettingValue> values = null;

        private string userID = null;

        public string UserID
        {
            get { return userID; }
        }

        #endregion

        #region Methods

        public string Get(string key)
        {
            int version;
            return Get(key, out version); 
        }

        public string Get(string key, out int version)
        {
            string returnValue = null;
            version = -1;

            if (registers.ContainsKey(key))
            {
                if (userID == null)
                    returnValue = defaultValues[key];
                else
                {
                    Database dc = Share.Database;
                    int keyID = registers[key];

                    // get database's user setting version   
                    Column settingCol = dc.Scalar("select US_V from [UserSetting] where US_U = {0} and US_SID = {1}", userID, keyID);

                    int settingV = settingCol.ToInt();
                    
                    bool proceedSyncFromDatabase = false;   
                    UserSettingValue usv = null;

                    if (values.ContainsKey(key))
                    {
                        usv = values[key];

                        if (usv.Version < settingV) // outdated
                            proceedSyncFromDatabase = true;
                        else // updated
                        {                            
                            returnValue = usv.Value;
                            version = usv.Version;
                        }
                    }
                    else // not exists
                        proceedSyncFromDatabase = true;

                    if (proceedSyncFromDatabase)
                    {
                        int newVersion;

                        Result rows = dc.Query("select US_Value, US_V from [UserSetting] where US_U = {0} AND US_SID = {1}",
                            userID, keyID);

                        if (rows.Count >= 1)
                        {
                            Row rowt = rows[0];
                            returnValue = rowt["US_Value"].ToString();
                            newVersion = rowt["US_V"].ToInt();
                        }
                        else
                        {
                            returnValue = defaultValues[key];
                            newVersion = 0;
                            // insert this returnValue to database
                            dc.Execute("insert into [UserSetting](US_ID, US_U, US_SID, US_Value, US_V) values({0}, {1}, {2}, {3}, {4})",
                                Database.ID(), userID, keyID, returnValue, newVersion);
                        }
                                                    
                        if (usv == null)
                        {
                            // new setting to values
                            usv = new UserSettingValue(returnValue, newVersion);

                            lock (values)
                            {
                                values.Add(key, usv);
                            }
                        }
                        else
                        {
                            // update outdated setting
                            usv.Value = returnValue;
                            usv.Version = newVersion;                            
                        }
                        version = newVersion;
                    }
                }
            }

            return returnValue;
        }

        public void Set(string key, string value)
        {
            if (registers.ContainsKey(key))
            {
                if (userID != null)
                {
                    Database dc = Share.Database;
                    int keyID = registers[key];

                    Column settingVersionColumn = dc.Scalar("select US_V from [UserSetting] where US_U = {0} and US_SID = {1}",
                        userID, keyID);
                    
                    bool isNoSettingInDatabase = false;
                    int settingVersion;
                    int newVersion;

                    if (settingVersionColumn == null)
                    {
                        isNoSettingInDatabase = true;
                        settingVersion = 0;
                        newVersion = 1;
                    }
                    else
                    {
                        settingVersion = settingVersionColumn.ToInt();
                    }                    

                    UserSettingValue usv = null;

                    if (values.ContainsKey(key))
                    {
                        usv = values[key];

                        if (usv.Version < settingVersion)
                        {
                            // outdated
                            // update setting                            
                            newVersion = settingVersion + 1;
                            usv.Version = newVersion;
                        }
                        else
                        {
                            // updated 
                            newVersion = usv.Version + 1;
                            usv.Version = newVersion;
                        }

                        usv.Value = value;
                    }
                    else
                    {
                        // didnt exists
                        newVersion = settingVersion + 1;
                        usv = new UserSettingValue(value, newVersion);

                        lock (values)
                        {
                            values.Add(key, usv);
                        }
                    }

                    if (isNoSettingInDatabase)
                    {
                        // new, insert to database
                        // insert this returnValue to database
                        dc.Execute("insert into [UserSetting](US_ID, US_U, US_SID, US_Value, US_V) values({0}, {1}, {2}, {3}, {4})",
                            Database.ID(), userID, keyID, value, newVersion);
                    }
                    else
                    {
                        // update
                        dc.Execute("update [UserSetting] set US_Value = {0}, US_V = {1} where US_U = {2} and US_SID = {3}",
                            value, newVersion, userID, keyID);
                    }
                }
            }            
        }
               
        #endregion
    }

    public class UserSettingValue
    {
        private string value;

        public string Value
        {
            get { return value; }
            set { this.value = value; }
        }

        private int version = 0;

        public int Version
        {
            get { return version; }
            internal set { version = value; }
        }

        internal UserSettingValue(string value, int version)
        {
            this.value = value;
            this.version = version;
        }
    }
}
