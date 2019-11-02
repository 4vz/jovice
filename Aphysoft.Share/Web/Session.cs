
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

using Aveezo;

namespace Aphysoft.Share
{
    internal sealed class SessionClient
    {
        public string ClientId { get; set; } = null;

        public string StreamSubDomain { get; set; } = null;

        public string StreamPort { get; set; } = null;
    }

    public sealed class Session : IDictionary<string, object>
    {
        #region Consts

        const string sessionIdCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        #endregion

        #region Fields

        private static Database database = Web.Database;

        public static Session Current { get; private set; } = null;

        public static string Id { get; private set; } = null;

        private Dictionary<string, object> data;

        public static string CookieSessionId { get; } = "ssid";

        public static string UserIPAddress { get; } = null;

        #endregion

        #region Constructors

        internal Session()
        {
            data = new Dictionary<string, object>();
        }

        #endregion

        #region Methods

        private static string CreateId()
        {
            StringBuilder sb = new StringBuilder();
            int avn = sessionIdCharacters.Length;
            for (int i = 0; i < 24; i++) { sb.Append(sessionIdCharacters[Rnd.Int(avn)]); }
            return sb.ToString();
        }
        
        internal static void Start(HttpContext context)
        {
            HttpRequest request = context.Request;
            HttpResponse response = context.Response;
            ExecutionType executionType = (ExecutionType)context.Items["provider"];

            bool sessionStart = false;

            if (request.Cookies[CookieSessionId] == null)
            {
                if (executionType == ExecutionType.Default)
                {
                    string sessionId = CreateId();

                    HttpCookie cookie = new HttpCookie(CookieSessionId);
                    cookie.Value = sessionId;
                    cookie.HttpOnly = true;
                    cookie.Path = "/";
                    cookie.Domain = WebSettings.Domain;

                    response.Cookies.Add(cookie);

                    Result newsession = database.ExecuteIdentity(@"
insert into 
Session(SS_SID, SS_Created, SS_Accessed, SS_UserAgent, SS_IPAddress)
values({0}, GETUTCDATE(), GETUTCDATE(), {1}, {2})
", sessionId, request.UserAgent, request.UserHostAddress);

                    context.Items["sessionId"] = sessionId;
                    sessionStart = true;
                }
                else if (executionType == ExecutionType.API)
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
                string sessionId = request.Cookies[CookieSessionId].Value;

                Result session = database.Query(@"
select SS_IPAddress from Session where SS_SID = {0}
", sessionId);
                if (session.Count == 1)
                {
                    string sessionIPAddress = session[0]["SS_IPAddress"].ToString();

                    if (sessionIPAddress != request.UserHostAddress)
                    {
                        database.Execute(@"
update Session set SS_Accessed = GETUTCDATE(), SS_IPAddress = {1} where SS_SID = {0}
", sessionId, request.UserHostAddress);
                    }
                    else
                    {
                        database.Execute(@"
update Session set SS_Accessed = GETUTCDATE() where SS_SID = {0}
", sessionId);
                    }

                    context.Items["sessionId"] = sessionId;
                    sessionStart = true;
                }
                else
                {
                    // maybe, browser masih open after this time, insert to database
                    Result newsession = database.ExecuteIdentity(@"
insert into 
Session(SS_SID, SS_Created, SS_Accessed, SS_UserAgent, SS_IPAddress)
values({0}, GETUTCDATE(), GETUTCDATE(), {1}, {2})
", sessionId, request.UserAgent, request.UserHostAddress);

                    context.Items["sessionId"] = sessionId;
                    sessionStart = true;
                }
            }

            if (sessionStart)
            {
                Current = new Session();
                Id = (string)context.Items["sessionId"];
            }
        }

        internal static SessionClient Client(HttpContext context)
        {
            SessionClient c = new SessionClient();
            c.ClientId = CreateId();

            if (Web.Service.IsConnected)
            {
                SessionClientMessage m = new SessionClientMessage(Id);
                m.Length = WebSettings.MaxStream;

                if (Web.Service.Send(m, out SessionClientMessage mr))
                {
                    c.StreamSubDomain = $"c-{mr.Index + 1}";

                    //if (Settings.StreamSubPorts[mr.Index] != 0)
                    //    c.StreamPort = ":" + Settings.StreamSubPorts[mr.Index];
                }
            }

            if (c.StreamSubDomain == null)
            {
                c.StreamSubDomain = "base";

                //if (Settings.StreamBasePort != 0)
                //    c.StreamPort = ":" + Settings.StreamBasePort;
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
    

}
