using Aphysoft.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Aphysoft.Common;
using System.Threading;
using System.Globalization;

namespace Jovice
{
    [Flags]
    public enum NecrowServices
    {
        None            = 0x00000,
        Probe           = 0x00001,
        StandbyProbe    = 0x10000,
        All             = 0x11111,
        AllExceptProbe  = 0x01110
    }

    public enum TestProbeServices
    {
        MECustomer = 1,
        MEQOS = 2,
        MEPeer = 4,
        MEService = 8,
        MEServicePeer = 16,
        MEInterface = 32,
        MEAll = 63,
        PERoute = 64,
        PEQOS = 128,
        PEInterface = 256,
        PERP = 512,
        PEAll = 960,
        All = 1023
    }
      
    public abstract class BaseProbeProperties
    {
        #region Fields

        string sshUser;

        public string SSHUser
        {
            get { return sshUser; }
            set { sshUser = value; }
        }

        string sshPassword;

        public string SSHPassword
        {
            get { return sshPassword; }
            set { sshPassword = value; }
        }

        string tacacUser;

        public string TacacUser
        {
            get { return tacacUser; }
            set { tacacUser = value; }
        }

        string tacacPassword;

        public string TacacPassword
        {
            get { return tacacPassword; }
            set { tacacPassword = value; }
        }

        private string sshTerminal;

        public string SSHTerminal
        {
            get { return sshTerminal; }
            set { sshTerminal = value; }
        }

        private string sshServerAddress;

        public string SSHServerAddress
        {
            get { return sshServerAddress; }
            set { sshServerAddress = value; }
        }

        #endregion
    }

    public class ProbeProperties : BaseProbeProperties
    {
        #region Fields

        private string testProbeNode;

        public string TestProbeNode
        {
            get { return testProbeNode; }
            set { testProbeNode = value; }
        }

        private TestProbeServices testServices;

        public TestProbeServices TestProbeServices
        {
            get { return testServices; }
            set { testServices = value; }
        }

        #endregion
    }

    public class StandByProbeProperties : BaseProbeProperties
    {

    }

    public static class Necrow
    {
        #region Fields

        internal readonly static int Version = 10;

        private static Database joviceDatabase = null;

        internal static Database JoviceDatabase
        {
            get { return joviceDatabase; }
        }

        private static NecrowServices services;

        private static bool console = false;
                
        private static ProbeProperties probeProperties = null;

        public static ProbeProperties ProbeProperties
        {
            get { return probeProperties; }
        }

        private static StandByProbeProperties standbyProbeProperties = null;

        public static StandByProbeProperties StandbyProbeProperties
        {
            get { return standbyProbeProperties; }
        }

        #endregion

        #region Methods

        internal static void Event(string message, string subsystem)
        {
            if (console)
            {
                if (subsystem == null)
                    System.Console.WriteLine(DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss.fff") + "|" + message);
                else
                    System.Console.WriteLine(DateTime.UtcNow.ToString("yyyy/MM/dd HH:mm:ss.fff") + "|" + subsystem + "|" + message);
            }
        }

        internal static void Event(string message)
        {
            Event(message, null);
        }

        public static void Set(ProbeProperties properties)
        {
            probeProperties = properties;
        }

        public static void Set(StandByProbeProperties properties)
        {
            standbyProbeProperties = properties;
        }

        public static bool IsServiceSet(NecrowServices service)
        {
            return services.IsFlagSet<NecrowServices>(service);
        }

        public static void Start(NecrowServices services)
        {
            Culture.Default();
            Necrow.services = services;
            Thread start = new Thread(new ThreadStart(delegate()
            {
                Culture.Default();

                Thread.Sleep(100);

                Event("Necrow Starting...");

                Event("Initialize Service Client");
                Service.Client();
                Service.Connected += delegate (Connection connection)
                {
                    Event("Service Connected");
                    Service.Send(new ServerNecrowServiceMessage(NecrowServiceMessageType.Hello));
                };
                Service.Register(typeof(ServerNecrowServiceMessage), NecrowServiceMessageHandler);
                
                Event("Checking Database...");

                bool joviceDatabaseConnected = false;
                Database jovice = Jovice.Database;

                DatabaseExceptionEventHandler checkingDatabaseException = delegate(object sender, DatabaseExceptionEventArgs eventArgs)
                {
                    Event("Connection Failed: " + eventArgs.Message);
                };

                jovice.Exception += checkingDatabaseException;

                if (jovice.Test())
                {
                    joviceDatabaseConnected = true;
                    Event("Jovice Database OK");
                }

                jovice.Exception -= checkingDatabaseException;

                joviceDatabase = jovice;
                
                jovice.Exception += Jovice_Exception;
                jovice.QueryFailed += Jovice_QueryFailed;
                jovice.Attempts = 3;

                if (joviceDatabaseConnected)
                {
                    Event("Necrow Started");

                    if (services.IsFlagSet(NecrowServices.Probe) && probeProperties != null)
                    {
                        Probe.Start(probeProperties);
                    }
                    if (services.IsFlagSet(NecrowServices.StandbyProbe) && standbyProbeProperties != null)
                    {
                        Probe.Start(standbyProbeProperties);
                    }
                }
            }));
            start.Start();
        }

        private static void Jovice_QueryFailed(object sender, EventArgs e)
        {
            Event("Query failed, retrying...");
            Thread.Sleep(2000);
        }

        private static void Jovice_Exception(object sender, DatabaseExceptionEventArgs eventArgs)
        {
            Event("Database exception: " + eventArgs.Message + ", SQL: " + eventArgs.Sql, "JOVICE");
        }

        public static void Stop()
        {
            if (services.IsFlagSet(NecrowServices.Probe))
            {
                Probe.Stop(ProbeMode.Default);
            }
        }

        public static void Console()
        {
            console = true;

            bool consoleLoop = true;

            while (consoleLoop)
            {
                string line = System.Console.ReadLine();
                ConsoleInput cs = new ConsoleInput(line);

                if (cs.IsCommand("exit"))
                {
                    Stop();
                    consoleLoop = false;
                }
                else if (cs.IsCommand("probe"))
                {
                    if (cs.Clauses.Count == 2)
                    {
                        string nodename = cs.Clauses[1];
                        Probe.Prioritize(nodename);
                    }
                }
            }
        }

        private static void NecrowServiceMessageHandler(MessageEventArgs e)
        {
            ServerNecrowServiceMessage m = (ServerNecrowServiceMessage)e.Message;

            if (m.Type == NecrowServiceMessageType.Request)
            {
                Event("We got request from server! = " + m.RequestID);

            }

        }

        #endregion
    }

    [Serializable]
    public enum NecrowServiceMessageType
    {
        Hello,
        Request
    }

    [Serializable]
    public class ServerNecrowServiceMessage : BaseServiceMessage
    {
        #region Fields

        private NecrowServiceMessageType type;

        public NecrowServiceMessageType Type
        {
            get { return type; }
            set { type = value; }
        }
        
        private int requestID;

        public int RequestID
        {
            get { return requestID; }
            set { requestID = value; }
        }

        #endregion

        #region Constructors

        public ServerNecrowServiceMessage(NecrowServiceMessageType type)
        {
            this.type = type;
        }

        #endregion
    }
}
