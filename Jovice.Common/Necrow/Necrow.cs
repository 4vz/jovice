using Aphysoft.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Aphysoft.Common;
using System.Threading;
using System.Globalization;
using System.IO;

namespace Jovice
{
    public class ProbeProperties
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

        public bool IsComplete
        {
            get
            {
                return sshUser != null && sshPassword != null && sshServerAddress != null && sshTerminal != null && tacacUser != null && tacacPassword != null;
            }
        }

        #endregion
    }

    public static class Necrow
    {
        #region Fields

        internal readonly static int Version = 15;

        private static Database jovice = null;

        internal static Database Jovice
        {
            get { return jovice; }
        }

        private static bool console = false;

        private static Queue<Tuple<int, string>> list = null;

        private static Queue<string> prioritize = new Queue<string>();

        private static List<Tuple<string, string, string>> supportedVersions = null;

        #endregion

        #region Helpers

        private static Dictionary<string, string[]> interfaceTestPrefixes = null;

        internal static Dictionary<string, string[]> InterfaceTestPrefixes
        {
            get { return interfaceTestPrefixes; }
        }

        #endregion

        #region Methods

        internal static void Event(string message, string subsystem)
        {
            if (console)
            {
                //yyyy/MM/dd 
                if (subsystem == null)
                    System.Console.WriteLine(DateTime.UtcNow.ToString("HH:mm:ss.fff") + "|" + message);
                else
                    System.Console.WriteLine(DateTime.UtcNow.ToString("HH:mm:ss.fff") + "|" + subsystem + "|" + message);
            }
        }

        internal static void Event(string message)
        {
            Event(message, null);
        }

#if DEBUG
        public static bool Debug()
        {
            return true;
        }

        public static void Test(string name)
        {
            prioritize.Enqueue(name + "*");
        }
#else
        internal static void Log(string source, string log)
        {
            Insert insert = Necrow.Jovice.Insert("Log");
            insert.Value("LO_TimeStamp", DateTime.UtcNow);
            insert.Value("LO_Source", source);
            insert.Value("LO_Log", log);
            insert.Execute();
        }
#endif

        public static void Start()
        {
            Thread start = new Thread(new ThreadStart(delegate ()
            {
                Batch batch;

                Culture.Default();
                Event("Necrow Starting...");

                Service.Client();
                Service.Connected += delegate (Connection connection)
                {
                    Event("Service Connected");
                    Service.Send(new ServerNecrowServiceMessage(NecrowServiceMessageType.Hello));
                };
                Service.Register(typeof(ServerNecrowServiceMessage), NecrowServiceMessageHandler);

                Event("Checking Jovice Database connection...");

                bool joviceDatabaseConnected = false;
                jovice = global::Jovice.Jovice.Database;

                DatabaseExceptionEventHandler checkingDatabaseException = delegate (object sender, DatabaseExceptionEventArgs eventArgs)
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

                jovice.Exception += delegate (object sender, DatabaseExceptionEventArgs eventArgs)
                {
                    Event("Database exception: " + eventArgs.Message, "JOVICE");                    
                    throw new Exception(eventArgs.Message + "\r\nSQL:\r\n" + eventArgs.Sql);          
                };
                jovice.QueryFailed += delegate (object sender, QueryFailedEventArgs e)
                {
                    Event("#" + (e.AttemptNumber + 1) + " database query has failed, retry in 10 seconds");
                    Thread.Sleep(10000);
                };
                jovice.Attempts = 5;

                if (joviceDatabaseConnected)
                {
                    batch = jovice.Batch();

                    Event("Loading accounts...");

                    int account;
                    if (!int.TryParse(Configuration.Settings("account", "0"), out account)) account = 0;

                    List<ProbeProperties> accounts = new List<ProbeProperties>();

                    for (int ai = 1; ai <= account; ai++)
                    {
                        string accconf = Configuration.Settings("account" + ai);

                        if (accconf != null)
                        {
                            string[] confs = accconf.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                            ProbeProperties prop = new ProbeProperties();

                            foreach (string conf in confs)
                            {
                                string[] pair = conf.Split(new char[] { '=' });

                                if (pair.Length == 2)
                                {
                                    if (pair[0] == "ssh")
                                    {
                                        string[] sshx = pair[1].Split(new char[] { '@', ':' }, 3);
                                        if (sshx.Length == 3)
                                        {
                                            prop.SSHUser = sshx[0];
                                            prop.SSHServerAddress = sshx[1];
                                            prop.SSHPassword = sshx[2];
                                        }
                                    }
                                    else if (pair[0] == "terminal")
                                    {
                                        prop.SSHTerminal = pair[1];
                                    }
                                    else if (pair[0] == "tacac")
                                    {
                                        string[] tacacx = pair[1].Split(new char[] { ':' }, 2);
                                        if (tacacx.Length == 2)
                                        {
                                            prop.TacacUser = tacacx[0];
                                            prop.TacacPassword = tacacx[1];
                                        }
                                    }
                                }
                            }

                            if (prop.IsComplete)
                            {
                                accounts.Add(prop);
                            }
                            else Event("Account #" + ai + " configuration is incomplete");
                        }
                        else Event("Cannot find Account #" + ai + " configuration");
                    }

                    if (accounts.Count > 0)
                    {
                        Event("Loaded " + accounts.Count + " account" + ((accounts.Count > 1) ? "s" : ""));

                        #region Virtualizations

                        Event("Starting database virtualizations...");

                        NecrowVirtualization.Load();

                        Event("Database virtualizations completed");

                        NecrowVirtualization.FlushNeighborInterface();

                        #endregion

                        #region Etc

                        interfaceTestPrefixes = new Dictionary<string, string[]>();
                        interfaceTestPrefixes.Add("Hu", new string[] { "H", "HU" });
                        interfaceTestPrefixes.Add("Te", new string[] { "T", "TE", "TENGIGE", "GI", "XE" }); // kadang Te-gig direfer sebagai Gi dammit people
                        interfaceTestPrefixes.Add("Gi", new string[] { "G", "GI", "GE", "GIGAE", "GIGABITETHERNET", "TE" }); // kadang Te-gig direfer sebagai Gi dammit people
                        interfaceTestPrefixes.Add("Fa", new string[] { "F", "FA", "FE", "FASTE" });
                        interfaceTestPrefixes.Add("Et", new string[] { "E", "ET", "ETH" });
                        interfaceTestPrefixes.Add("Ag", new string[] { "LAG", "ETH-TRUNK", "BE" });

                        #endregion

                        #region Probe initialization

                        Event("Loading probe list...");

                        list = new Queue<Tuple<int, string>>();

                        foreach (Row xp in jovice.Query("select XP_ID, XP_NO from ProbeProgress order by XP_ID asc"))
                        {
                            list.Enqueue(new Tuple<int, string>(xp["XP_ID"].ToInt(), xp["XP_NO"].ToString()));
                        }

                        if (list.Count == 0)
                        {
                            CreateNodeQueue();
                        }
                        else
                        {
                            // set all starttime and status to null
                            jovice.Execute("update ProbeProgress set XP_StartTime = NULL, XP_Status = NULL");
                            Event("Using existing list, " + list.Count + " node" + (list.Count > 1 ? "s" : "") + " remaining");
                        }

                        if (supportedVersions == null)
                        {
                            Result sver = jovice.Query("select * from NodeSupport");

                            supportedVersions = new List<Tuple<string, string, string>>();

                            foreach (Row sve in sver)
                            {
                                supportedVersions.Add(new Tuple<string, string, string>(sve["NT_Manufacture"].ToString(), sve["NT_Version"].ToString(), sve["NT_SubVersion"].ToString()));
                            }
                        }

                        #endregion

                        #region Starting instances

                        int instance;
                        if (!int.TryParse(Configuration.Settings("instance", "1"), out instance)) instance = 1;
                        if (instance < 1) instance = 1;
                        Event("Creating " + instance + " probe instance" + (instance > 1 ? "s..." : "..."));

                        for (int ins = 0; ins < instance; ins++)
                        {
                            int acn = ins % accounts.Count;
                            Event("Starting Instance #" + (ins + 1) + " using Account #" + (acn + 1));
                            Probe.Start(accounts[acn], "INST" + (ins + 1));
                        }

                        #endregion
                    }
                    else Event("There's no account configured in the configuration file, please check account configuration in the *.config file");

                }
                else
                {
                    Event("Problem with database, please check database data source in the *.config file");
                }
            }));
            start.Start();
        }

        public static void Stop()
        {

        }

        private static void CreateNodeQueue()
        {
            lock (list)
            {
                if (list.Count == 0)
                {
                    Event("Preparing node list...");

                    Result nres = Jovice.Query(@"
select NO_ID from Node where NO_Active = 1 and NO_Type in ('P', 'M') and NO_TimeStamp is null and NO_LastConfiguration is null                        
");
                    Result mres = Jovice.Query(@"
select a.NO_ID, a.NO_Name, a.NO_Remark, a.NO_TimeStamp, CASE WHEN a.span < 0 then 0 else a.span end as span from (
select NO_ID, NO_Name, NO_Remark, NO_LastConfiguration, NO_TimeStamp, DateDiff(hour, NO_LastConfiguration, NO_TimeStamp) as span 
from Node where NO_Active = 1 and NO_Type in ('P', 'M') and NO_TimeStamp is not null and NO_LastConfiguration is not null
) a
order by span asc, a.NO_LastConfiguration asc
");
                    Result sres = Jovice.Query(@"
select NO_ID from Node where NO_Active = 1 and NO_Type in ('P', 'M') and NO_TimeStamp is not null and NO_LastConfiguration is null                        
");

                    List<string> nids = new List<string>();

                    int excluded = 0;

                    foreach (Row row in nres) nids.Add(row["NO_ID"].ToString());
                    foreach (Row row in mres)
                    {
                        string remark = row["NO_Remark"].ToString();
                        if (remark != null)
                        {
                            DateTime timestamp = row["NO_TimeStamp"].ToDateTime();
                            TimeSpan span = DateTime.Now - timestamp;

                            if (
                                (remark == "CONNECTFAIL" && span.TotalHours <= 3) ||
                                (remark == "UNRESOLVED" && span.TotalDays <= 1)
                            )
                            {
                                excluded++;
                                Event("Excluded: " + row["NO_Name"].ToString() + " Remark: " + remark);
                                continue;
                            }
                        }

                        nids.Add(row["NO_ID"].ToString());
                    }
                    foreach (Row row in sres) nids.Add(row["NO_ID"].ToString());
                    int total = nids.Count + excluded;
                    Event("Total " + total + " nodes available, " + nids.Count + " nodes eligible, " + excluded + " excluded in this list");

                    Batch batch = Jovice.Batch();

                    batch.Begin();
                    int id = 1;
                    foreach (string nid in nids)
                    {
                        Insert insert = Jovice.Insert("ProbeProgress");
                        insert.Value("XP_ID", id++);
                        insert.Value("XP_NO", nid);
                        batch.Execute(insert);
                    }
                    Result result = batch.Commit();
                    if (result.Count > 0) Event("List created");

                    foreach (Row xp in Jovice.Query("select XP_ID, XP_NO from ProbeProgress order by XP_ID asc"))
                    {
                        list.Enqueue(new Tuple<int, string>(xp["XP_ID"].ToInt(), xp["XP_NO"].ToString()));
                    }
                }
            }
        }

        internal static Tuple<int, string> NextNode()
        {
            Tuple<int, string> noded = null;

            lock (list)
            {
                if (list.Count == 0)
                {
                    NecrowVirtualization.FlushNeighborInterface();

                    CreateNodeQueue();
                }

                noded = list.Dequeue();

            }

            return noded;
        }

        internal static string NextPrioritize()
        {
            string node = null;

            lock (prioritize)
            {
                if (prioritize.Count > 0)
                {
                    node = prioritize.Dequeue();
                }
            }

            return node;
        }

        internal static void AcknowledgeNodeVersion(string manufacture, string version, string subVersion)
        {
            bool exists = false;

            foreach (Tuple<string, string, string> sve in supportedVersions)
            {
                if (sve.Item1 == manufacture && sve.Item2 == version && sve.Item3 == subVersion)
                {
                    exists = true;
                    break;
                }
            }

            if (!exists)
            {
                lock (supportedVersions)
                {
                    supportedVersions.Add(new Tuple<string, string, string>(manufacture, version, subVersion));

                    Insert insert = Jovice.Insert("NodeSupport");
                    insert.Value("NT_ID", Database.ID());
                    insert.Value("NT_Manufacture", manufacture);
                    insert.Value("NT_Version", version);
                    insert.Value("NT_SubVersion", subVersion);
                    insert.Execute();
                }
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
                        prioritize.Enqueue(nodename);
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
