using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    public delegate void OnReceivedCallback(EdgeMessageEventArgs e);

    public abstract class WebService : Node
    {
        #region Fields

        private static object instancesWaitSync = new object();

        public static Database2 Database { get; private set; } = null;

        public static Dictionary<string, StreamClientInstance> ClientInstances { get; private set; }

        public static Dictionary<string, StreamSessionInstance> SessionInstances { get; private set; }

        internal static Dictionary<string, OnReceivedCallback> registerCallbacks;

        #endregion

        #region Constructors

        public WebService() : base("WEBSERVICE")
        {
            NoDebug();

            ClientInstances = new Dictionary<string, StreamClientInstance>();
            SessionInstances = new Dictionary<string, StreamSessionInstance>();

            registerCallbacks = new Dictionary<string, OnReceivedCallback>();
        }

        #endregion

        #region Handlers

        protected override void OnStart()
        {
            Database = Database2.Web();
            Database.QueryAttempts = 3;

            Database.Execute("update Session set SS_ClientsCount = 0 where SS_ClientsCount > 0");

            BeginAcceptEdges();

            while (IsRunning)
            {
                Thread.Sleep(2000);
            }
        }

        protected override void OnStop()
        {

            
        }

        protected override void OnConnected(EdgeConnectedEventArgs e)
        {
        }

        protected override void OnMessage(EdgeMessageEventArgs e)
        {
            if (e.Message is BaseStreamMessage bsm)
            {
                string sessionId = bsm.SessionID;

                if (sessionId == null)
                {
                    e.Reply(bsm);
                }

                if (bsm is StreamMessage sm)
                {
                    string clientID = sm.ClientID;

                    StreamClientInstance clientInstance;

                    bool newClientInstance = false;

                    lock (instancesWaitSync)
                    {
                        if (ClientInstances.ContainsKey(clientID))
                            clientInstance = ClientInstances[clientID];
                        else
                        {
                            clientInstance = new StreamClientInstance(clientID);
                            ClientInstances.Add(clientID, clientInstance);
                            newClientInstance = true;

                            // add to sessionInstance, if not found, create new
                            StreamSessionInstance sessionInstance;

                            if (SessionInstances.ContainsKey(sessionId))
                            {
                                sessionInstance = SessionInstances[sessionId];

                                // existing session,
                                // the session has another client instance
                                clientInstance.SessionIndex = sessionInstance.GetAvailableIndex();
                            }
                            else
                            {
                                sessionInstance = new StreamSessionInstance(sessionId);
                                SessionInstances.Add(sessionId, sessionInstance);

                                // yes, this is new session,
                                // it means new client
                                clientInstance.SessionIndex = 0;
                            }

                            clientInstance.SessionInstance = sessionInstance;
                            sessionInstance.ClientInstances.Add(clientID, clientInstance);

                            Database.Execute("update Session set SS_ClientsCount = {0} where SS_SID = {1}", sessionInstance.ClientInstances.Count, sessionId);
                        }
                    }

                    bool updateDomain = false;

                    if (newClientInstance)
                    {
                        if (sm.HostSessionIndex != clientInstance.SessionIndex)
                        {
                            sm.MessageContinue = false;
                            sm.MessageType = "updatestreamdomain";
                            sm.MessageData = clientInstance.SessionIndex;

                            e.Reply(sm);

                            updateDomain = true;
                        }
                    }

                    if (updateDomain == false)
                    {
                        Stopwatch stopwatch = new Stopwatch();
                        stopwatch.Start();

                        do
                        {
                            do
                            {
                                if (clientInstance.DataQueue.Count > 0) break;
                                else if (stopwatch.Elapsed.TotalSeconds >= 50) break;

                                clientInstance.resetEvent.WaitOne(1000);
                            }
                            while (true);

                            if (clientInstance.DataQueue.Count > 0)
                            {
                                lock (clientInstance.dataQueueWaitSync)
                                {
                                    if (clientInstance.DataQueue.Count > 0)
                                    {
                                        StreamInstanceData data = clientInstance.DataQueue.Dequeue();

                                        sm.MessageContinue = false;
                                        sm.MessageType = data.Type;
                                        sm.MessageData = data.Data;

                                        e.Reply(sm);

                                        if (clientInstance.DataQueue.Count == 0)
                                            clientInstance.resetEvent.Reset();
                                    }
                                }
                            }
                            else break;

                        }
                        while (true);
                    }

                    sm.MessageContinue = true;
                    sm.MessageType = null;
                    sm.MessageData = null;

                    e.Reply(sm);
                }
                else if (bsm is ClientEndMessage cem)
                {
                    string clientID = cem.ClientID;

                    lock (instancesWaitSync)
                    {
                        if (ClientInstances.ContainsKey(clientID))
                        {
                            StreamClientInstance clientInstance = ClientInstances[clientID];

                            ClientInstances.Remove(clientID);

                            StreamSessionInstance sessionInstance = clientInstance.SessionInstance;

                            sessionInstance.ClientInstances.Remove(clientID);

                            Database.Execute("update Session set SS_ClientsCount = {0} where SS_SID = {1}", sessionInstance.ClientInstances.Count, sessionId);

                            if (sessionInstance.ClientInstances.Count == 0)
                            {
                                // dont have client instances anymore, remove this
                                if (SessionInstances.ContainsKey(sessionInstance.SessionId))
                                {
                                    SessionInstances.Remove(sessionInstance.SessionId);
                                }
                            }
                        }
                    }
                }
                else if (bsm is RegisterMessage rm)
                {
                    string clientID = rm.ClientID;
                    string[] registers = rm.Registers;
                    bool force = rm.Force;

                    lock (instancesWaitSync)
                    {
                        if (ClientInstances.ContainsKey(clientID))
                        {
                            StreamClientInstance instance = ClientInstances[clientID];

                            if (force)
                            {
                                instance.RemoveAllRegisters();
                            }
                            foreach (string register in registers)
                            {
                                if (register.StartsWith("-"))
                                {
                                    string registerName = register.Substring(1);
                                    if (registerName.Length > 0) instance.RemoveRegister(registerName);
                                }
                                else
                                {
                                    instance.Register(register);

                                    if (registerCallbacks.ContainsKey(register))
                                    {
                                        registerCallbacks[register](e);
                                    }
                                }
                            }

                        }
                    }
                }
            }
        }

        protected override void OnDisconnected(EdgeDisconnectedEventArgs e, bool wasAuthenticated)
        {
            if (wasAuthenticated)
            {
            }
        }

        #endregion

        #region Methods

        internal static StreamSessionInstance GetSessionInstance(string sessionId)
        {
            if (sessionId != null && SessionInstances.ContainsKey(sessionId))
                return SessionInstances[sessionId];
            else
                return null;
        }

        public static string[] GetClientsByRegister(string register)
        {
            if (OldService.IsServer)
            {
                List<string> clients = new List<string>();

                foreach (KeyValuePair<string, StreamClientInstance> pair in ClientInstances)
                {
                    if (pair.Value.IsRegistered(register))
                    {
                        clients.Add(pair.Key);
                    }
                }

                return clients.ToArray();
            }
            else return null;
        }

        public static KeyValuePair<string, string[]>[] GetClientsByRegisterMatch(string registerMatch)
        {
            if (OldService.IsServer)
            {
                List<KeyValuePair<string, string[]>> clients = new List<KeyValuePair<string, string[]>>();

                foreach (KeyValuePair<string, StreamClientInstance> pair in ClientInstances)
                {
                    string[] matchTo;
                    if (pair.Value.IsRegisteredMatch(registerMatch, out matchTo))
                    {
                        KeyValuePair<string, string[]> kvp = new KeyValuePair<string, string[]>(pair.Key, matchTo);
                        clients.Add(kvp);
                    }
                }

                return clients.ToArray();
            }
            else return null;
        }

        public static int SetActionByRegister(string register, string type, object data)
        {
            string[] clientIDs = GetClientsByRegister(register);

            foreach (string clientID in clientIDs)
            {
                StreamClientInstance clientInstance = ClientInstances[clientID];
                clientInstance.SetAction(type, data);
            }

            return clientIDs.Length;
        }

        public static void SetActionByRegister(string register, object data)
        {
            SetActionByRegister(register, register, data);
        }

        public static int SetActionByRegisterMatch(string registerMatch, string type, object data)
        {
            KeyValuePair<string, string[]>[] clientIDs = GetClientsByRegisterMatch(registerMatch);

            foreach (KeyValuePair<string, string[]> pair in clientIDs)
            {
                string clientID = pair.Key;
                string[] matches = pair.Value;

                StreamClientInstance clientInstance = ClientInstances[clientID];
                clientInstance.SetAction(type, data);
            }

            return clientIDs.Length;
        }

        public static void SetActionByRegisterMatch(string registerMatch, object data)
        {
            KeyValuePair<string, string[]>[] clientIDs = GetClientsByRegisterMatch(registerMatch);

            foreach (KeyValuePair<string, string[]> pair in clientIDs)
            {
                string clientID = pair.Key;
                string[] matches = pair.Value;

                StreamClientInstance clientInstance = ClientInstances[clientID];

                foreach (string match in matches)
                {
                    clientInstance.SetAction(match, data);
                }
            }
        }

        public static void RegisterCallback(string register, OnReceivedCallback method)
        {
            if (!registerCallbacks.ContainsKey(register))
                registerCallbacks.Add(register, method);
        }

        #endregion
    }
}
