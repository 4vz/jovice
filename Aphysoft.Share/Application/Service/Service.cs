using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Web;


namespace Aphysoft.Share
{
    public class Service : BaseService
    {
        #region Fields

        private static Database share = Share.Database;

        private static Service instance = null;

        private static Service Instance
        {
            get
            {
                if (instance == null) instance = new Service();
                return instance;
            }
        }

        public static event ConsoleInputEventHandler ConsoleInput;

        public static event ConnectedEventHandler Connected;

        public static event DisconnectedEventHandler Disconnected;

        private static Thread consoleThread = null;

        #endregion

        #region Constructors

        public Service()
        {
        }

        #endregion

        #region Basic Methods

        public static void Server()
        {
            Server(ServiceOutputTypes.None);
        }

        public static void Server(ServiceOutputTypes outputType)
        {
            if (!IsServer && !IsClient)
            {
                if (share.Test())
                {
                    if ((int)outputType < (int)ServiceOutputTypes.None)
                    {
                        Instance.OutputType(outputType);

                        consoleThread = new Thread(new ThreadStart(ConsoleMainLoop));
                        consoleThread.Start();
                    }

                    Instance.Server(IPAddress.Any, 23474);

                    Provider.ServerInit();

                    Register(typeof(SessionClientServiceMessage), SessionClientServiceMessageHandler);
                }
            }
        }

        public static void Client()
        {
            if (!IsServer && !IsClient)
            {
                Instance.Client(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 23474));
            }
        }

        private static void ConsoleMainLoop()
        {
            do
            {
                string line = Console.ReadLine();

                ConsoleInput cs = new ConsoleInput(line);

                if (cs.IsCommand("exit")) break;
                else if (cs.IsCommand("connections")) Console.WriteLine("Currently active connections: " + instance.ActiveConnectionsCount);
                else if (cs.IsCommand("threads")) Console.WriteLine("Currently receiving threads: " + instance.ActiveReceivingThreadsCount);
                else if (cs.IsCommand("clients")) Console.WriteLine(string.Format("We have {0} client instances", Provider.clientInstances.Count));
                else if (cs.IsCommand("streamsendbyregister"))
                {
                    if (cs.Clauses.Count > 3)
                    {
                        string register = cs.Clauses[1];
                        string type = cs.Clauses[2];
                        string data = cs.ClausesFrom(3);

                        Console.WriteLine("Sending stream data to clients by register: " + register + " type: " + type + " data: " + data);
                        int clientaffected = Provider.SetActionByRegister(register, type, data);
                        Console.WriteLine(string.Format("Sent to {0} client instances", clientaffected));
                    }
                    else Console.WriteLine("Usage: streamsendbyregister <register> <type> <data>");
                }
                else if (cs.IsCommand("streamsendbyregistermatch"))
                {
                    if (cs.Clauses.Count > 3)
                    {
                        string pattern = cs.Clauses[1];
                        string type = cs.Clauses[2];
                        string data = cs.ClausesFrom(3);

                        Console.WriteLine("Sending stream data to clients by register pattern: " + pattern + " type: " + type + " data: " + data);
                        int clientaffected = Provider.SetActionByRegisterMatch(pattern, type, data);
                        Console.WriteLine(string.Format("Sent to {0} client instances", clientaffected));
                    }
                    else Console.WriteLine("Usage: streamsendbyregistermatch <pattern> <type> <data>");
                }
                else if (line.StartsWith("ping"))
                {
                    //Provider.SetAction("ping", null);
                }
                else if (ConsoleInput != null) ConsoleInput(cs);
            }
            while (true);

            Stop();
        }

        private static void SessionClientServiceMessageHandler(MessageEventArgs e)
        {
            SessionClientServiceMessage m = (SessionClientServiceMessage)e.Message;

            if (IsServer)
            {
                string sessionID = m.SessionID;
                StreamSessionInstance sessionInstance = Provider.GetSessionInstance(sessionID);

                if (sessionInstance == null) m.StreamSubDomainIndex = 0;
                else
                {
                    // we have this session before, maybe this a new window (client),
                    int index = sessionInstance.GetAvailableIndex();
                    m.StreamSubDomainIndex = index % m.StreamSubDomainLength;               
                }

                e.Connection.Reply(m);
            }

        }        

        #endregion

        #region Handlers

        public override void OnOutput(string output)
        {
            Console.WriteLine(output);
        }

        public override void OnConnected(ConnectionEventArgs e)
        {
            if (Connected != null) Connected(e.Connection);
        }

        public override void OnDisconnected(ConnectionEventArgs e)
        {
            if (Disconnected != null) Disconnected(e.Connection);
        }

        #endregion

        #region BaseService Shorthand

        public static new bool IsServer
        {
            get
            {
                BaseService instance = Service.Instance;

                return instance.IsServer;
            }
        }

        public static new bool IsClient
        {
            get
            {
                BaseService instance = Service.Instance;

                return instance.IsClient;
            }
        }

        public static new bool IsConnected
        {
            get
            {
                BaseService instance = Service.Instance;

                return instance.IsConnected;
            }
        }

        public static new void Stop()
        {
            BaseService instance = Service.Instance;

            instance.Stop();
        }

        public static new void Register(Type messageType, OnReceivedCallback method)
        {
            BaseService instance = Service.Instance;

            instance.Register(messageType, method);
        }

        public static new void Debug(string message)
        {
            BaseService instance = Service.Instance;

            instance.EventMessage(message);
        }

        public static void Debug(object message)
        {
            Debug(message.ToString());
        }

        public static void Debug(object[] message)
        {
            StringBuilder sb = new StringBuilder();
            bool next = false;
            foreach (object o in message)
            {
                if (next) sb.Append(", ");
                else next = true;
                sb.Append(o.ToString());
            }
            Debug(sb.ToString());
        }

        public static new bool Send(BaseServiceMessage message)
        {
            BaseService instance = Service.Instance;

            return instance.Send(message);
        }

        public static new T Wait<T>(T originalMessage) where T : BaseServiceMessage, new()
        {
            BaseService instance = Service.Instance;

            return instance.Wait<T>(originalMessage, new MessageWaitCallback(ClientConnectedCallback), HttpContext.Current);
        }

        private static bool ClientConnectedCallback(object obj)
        {
            HttpContext context = (HttpContext)obj;
            HttpResponse response = context.Response;

            if (response.IsClientConnected)
                return false;
            else
                return true;
        }

        #endregion 
    }

    public delegate void ConnectedEventHandler(Connection connection);

    public delegate void DisconnectedEventHandler(Connection connection);

}
