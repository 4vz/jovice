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

        internal readonly static int Version = 12;

        private static Database joviceDatabase = null;

        internal static Database JoviceDatabase
        {
            get { return joviceDatabase; }
        }

        private static bool console = false;

        private static Queue<Tuple<long, string>> list = null;

        private static Queue<string> prioritize = new Queue<string>();

        private static List<Tuple<string, string, string>> supportedVersions = null;

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

#if DEBUG
        public static void Test(string name)
        {
            prioritize.Enqueue(name + "*");
        }
#endif

        internal static void CreateNodeQueue()
        {
            lock (list)
            {
                if (list.Count == 0)
                {
                    Event("Preparing node list...");

                    Result nres = JoviceDatabase.Query(@"
select NO_ID from Node where NO_Active = 1 and NO_Type in ('P', 'M') and NO_TimeStamp is null and NO_LastConfiguration is null                        
");
                    Result mres = JoviceDatabase.Query(@"
select a.NO_ID, a.NO_Name, a.NO_Remark, a.NO_TimeStamp, CASE WHEN a.span < 0 then 0 else a.span end as span from (
select NO_ID, NO_Name, NO_Remark, NO_LastConfiguration, NO_TimeStamp, DateDiff(hour, NO_LastConfiguration, NO_TimeStamp) as span 
from Node where NO_Active = 1 and NO_Type in ('P', 'M') and NO_TimeStamp is not null and NO_LastConfiguration is not null
) a
order by span asc, a.NO_LastConfiguration asc
");
                    Result sres = JoviceDatabase.Query(@"
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

                    Batch batch = JoviceDatabase.Batch();

                    batch.Begin();
                    foreach (string nid in nids)
                    {
                        Insert insert = JoviceDatabase.Insert("NodeProgress");
                        insert.Value("NP_NO", nid);
                        batch.Execute(insert);
                    }
                    Result result = batch.Commit();
                    if (result.Count > 0) Event("List created");

                    Result npr = JoviceDatabase.Query("select NP_ID, NP_NO from NodeProgress where NP_EndTime is null order by NP_ID asc");

                    foreach (Row np in npr)
                    {
                        long np_ID = np["NP_ID"].ToLong();
                        string npNO = np["NP_NO"].ToString();
                        list.Enqueue(new Tuple<long, string>(np_ID, npNO));
                    }
                }
            }
        }

        internal static Tuple<long, string> GetNode()
        {
            Tuple<long, string> noded = null;

            lock (list)
            {
                if (list.Count == 0)
                {
                    CreateNodeQueue();
                }

                noded = list.Dequeue();

            }

            return noded;
        }

        internal static string GetPrioritize()
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

        internal static void SupportedNodeVersion(string manufacture, string version, string subVersion)
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

                    Insert insert = JoviceDatabase.Insert("NodeSupport");
                    insert.Value("NT_ID", Database.ID());
                    insert.Value("NT_Manufacture", manufacture);
                    insert.Value("NT_Version", version);
                    insert.Value("NT_SubVersion", subVersion);
                    insert.Execute();
                }
            }
        }

        internal static void Log(string source, string log)
        {
            Insert insert = JoviceDatabase.Insert("Log");
            insert.Value("LO_TimeStamp", DateTime.UtcNow);
            insert.Value("LO_Source", source);
            insert.Value("LO_Log", log);
            insert.Execute();
        }

        public static void Start()
        {
            Thread start = new Thread(new ThreadStart(delegate ()
            {
                Culture.Default();
                Event("Necrow Starting...");

                Service.Client();
                Service.Connected += delegate (Connection connection)
                {
                    Event("Service Connected");
                    Service.Send(new ServerNecrowServiceMessage(NecrowServiceMessageType.Hello));
                };
                Service.Register(typeof(ServerNecrowServiceMessage), NecrowServiceMessageHandler);

                Event("Checking Jovice Database...");

                bool joviceDatabaseConnected = false;
                Database jovice = Jovice.Database;

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

                joviceDatabase = jovice;

                jovice.Exception += Jovice_Exception;
                jovice.QueryFailed += Jovice_QueryFailed;
                jovice.Attempts = 3;

                if (joviceDatabaseConnected)
                {
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

                        list = new Queue<Tuple<long, string>>();

                        Result npr = jovice.Query("select NP_ID, NP_NO from NodeProgress where NP_EndTime is null order by NP_ID asc");

                        foreach (Row np in npr)
                        {
                            long npID = np["NP_ID"].ToLong();
                            string npNO = np["NP_NO"].ToString();
                            list.Enqueue(new Tuple<long, string>(npID, npNO));
                        }

                        if (list.Count == 0)
                        {
                            CreateNodeQueue();
                        }
                        else
                        {
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

        public static void Prioritize(string nodeName)
        {
            lock (prioritize)
            {
                prioritize.Enqueue(nodeName);
            }
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
                        Necrow.Prioritize(nodename);
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
