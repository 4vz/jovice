
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Aphysoft.Share
{
    internal sealed class SessionClient
    {
        #region Fields

        private string clientID = null;

        public string ClientID
        {
            get { return clientID; }
            set { clientID = value; }
        }

        private string streamSubDomain = null;

        public string StreamSubDomain
        {
            get { return streamSubDomain; }
            set { streamSubDomain = value; }
        }

        private string streamPort = null;

        public string StreamPort
        {
            get { return streamPort; }
            set { streamPort = value; }
        }

        #endregion
    }

    public sealed class Session : IDictionary<string, object>
    {
        #region Consts

        const string cookieSessionID = "ssid";
        const string sessionIDCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        #endregion

        #region Fields

        private static Database share = Share.Database;

        private static Session instance = null;

        public static Session Current
        {
            get { return instance; }
        }

        private static string id = null;

        public static string ID
        {
            get { return id; }
        }

        private Dictionary<string, object> data;

        public static string CookieSessionID { get => cookieSessionID; }

        private static string userIPAddress = null;

        public static string UserIPAddress { get => userIPAddress; }

        #endregion

        #region Constructors

        internal Session()
        {
            data = new Dictionary<string, object>();
        }

        #endregion

        #region Methods

        private static string createID()
        {
            StringBuilder sb = new StringBuilder();
            int avn = sessionIDCharacters.Length;
            for (int i = 0; i < 24; i++) { sb.Append(sessionIDCharacters[RandomHelper.Next(avn)]); }
            return sb.ToString();
        }
        
        internal static void Start(HttpContext context)
        {
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;
            ExecutionTypes executionType = (ExecutionTypes)context.Items["provider"];

            bool sessionStart = false;

            if (request.Cookies[cookieSessionID] == null)
            {
                if (executionType == ExecutionTypes.Default)
                {
                    string sessionID = createID();

                    HttpCookie cookie = new HttpCookie(cookieSessionID);
                    cookie.Value = sessionID;
                    cookie.HttpOnly = true;
                    cookie.Path = "/";
                    if (Settings.UseDomain) cookie.Domain = Settings.BaseDomain;

                    response.Cookies.Add(cookie);

                    Result newsession = share.ExecuteIdentity(@"
insert into 
[Session](SS_SID, SS_Created, SS_Accessed, SS_UserAgent, SS_IPAddress)
values({0}, GETUTCDATE(), GETUTCDATE(), {1}, {2})
", sessionID, request.UserAgent, request.UserHostAddress);

                    context.Items["sessionID"] = sessionID;
                    sessionStart = true;
                }
                else if (executionType == ExecutionTypes.API)
                {

                }
                else
                {
                    response.Status = "404 Not Found";
                    response.TrySkipIisCustomErrors = true;
                    response.End();
                }
            }
            else
            {
                string sessionID = request.Cookies[cookieSessionID].Value;

                Result session = share.Query(@"
select SS_IPAddress from [Session] where SS_SID = {0}
", sessionID);
                if (session.Count == 1)
                {
                    string sessionIPAddress = session[0]["SS_IPAddress"].ToString();

                    if (sessionIPAddress != request.UserHostAddress)
                    {
                        share.Execute(@"
update [Session] set SS_Accessed = GETUTCDATE(), SS_IPAddress = {1} where SS_SID = {0}
", sessionID, request.UserHostAddress);
                    }
                    else
                    {
                        share.Execute(@"
update [Session] set SS_Accessed = GETUTCDATE() where SS_SID = {0}
", sessionID);
                    }

                    context.Items["sessionID"] = sessionID;
                    sessionStart = true;
                }
                else
                {
                    // maybe, browser masih open after this time, insert to database
                    Result newsession = share.ExecuteIdentity(@"
insert into 
[Session](SS_SID, SS_Created, SS_Accessed, SS_UserAgent, SS_IPAddress)
values({0}, GETUTCDATE(), GETUTCDATE(), {1}, {2})
", sessionID, request.UserAgent, request.UserHostAddress);

                    context.Items["sessionID"] = sessionID;
                    sessionStart = true;
                }
            }

            if (sessionStart)
            {
                instance = new Session();
                id = (string)context.Items["sessionID"];
            }
        }

        internal static SessionClient Client(HttpContext context)
        {
            SessionClient c = new SessionClient();
            c.ClientID = createID();

            if (Service.IsConnected)
            {
                SessionClientServiceMessage m = new SessionClientServiceMessage(ID);
                m.StreamSubDomainLength = Settings.StreamSubDomains.Length;

                if (Service.Send(m))
                {
                    SessionClientServiceMessage mr = Service.Wait(m);
                    c.StreamSubDomain = Settings.StreamSubDomains[mr.StreamSubDomainIndex];
                    if (Settings.StreamSubPorts[mr.StreamSubDomainIndex] != 0)
                        c.StreamPort = ":" + Settings.StreamSubPorts[mr.StreamSubDomainIndex];
                }
            }

            if (c.StreamSubDomain == null)
            {
                c.StreamSubDomain = Settings.StreamBaseSubDomain;
                if (Settings.StreamBasePort != 0)
                    c.StreamPort = ":" + Settings.StreamBasePort;
            }   

            return c;
        }
        

        public void Add(string key, object value)
        {
            data.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return data.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return data.Keys; }
        }

        public bool Remove(string key)
        {
            return data.Remove(key);
        }

        public bool TryGetValue(string key, out object value)
        {
            return data.TryGetValue(key, out value);
        }

        public ICollection<object> Values
        {
            get { return data.Values; }
        }

        public object this[int index]
        {
            get
            {
                if (index >= 0 && index < data.Count)
                {
                    int i = 0;
                    foreach (KeyValuePair<string, object> kvpc in data)
                    {
                        if (i == index) return kvpc.Value;
                        i++;
                    }
                    return null;
                }
                else return null;
            }
            set { }
        }

        public object this[string key]
        {
            get
            {
                return data[key];
            }
            set { }
        }

        public void Add(KeyValuePair<string, object> item)
        {
            data.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            data.Clear();
        }

        public bool Contains(KeyValuePair<string, object> item)
        {
            return data.ContainsKey(item.Key);
        }

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return data.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<string, object> item)
        {
            return data.Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
        {
            return data.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)data.GetEnumerator();
        }

        #endregion
    }
    
    [Serializable]
    internal class SessionClientServiceMessage : SessionServiceMessage
    {
        #region Fields

        private int streamSubDomainLength;

        public int StreamSubDomainLength
        {
            get { return streamSubDomainLength; }
            set { streamSubDomainLength = value; }
        }

        private int streamSubDomainIndex;

        public int StreamSubDomainIndex
        {
            get { return streamSubDomainIndex; }
            set { streamSubDomainIndex = value; }
        }

        #endregion

        #region Constructor

        public SessionClientServiceMessage(string sessionID)
            : base(sessionID)
        {
        }

        public SessionClientServiceMessage() : base()
        {
        }

        #endregion
    }
}
