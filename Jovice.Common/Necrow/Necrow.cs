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

        #region Virtualization 

        private static List<Tuple<string, List<Tuple<string, string, string, string, string, string>>>> pePhysicalInterfaces = null;

        internal static List<Tuple<string, List<Tuple<string, string, string, string, string, string>>>> PEPhysicalInterfaces
        {
            get { return pePhysicalInterfaces; }
        }

        private static List<Tuple<string, List<Tuple<string, string, string, string>>>> mePhysicalInterfaces = null;

        internal static List<Tuple<string, List<Tuple<string, string, string, string>>>> MEPhysicalInterfaces
        {
            get { return mePhysicalInterfaces; }
        }

        private static List<Tuple<string, List<Tuple<string, string>>>> nnPhysicalInterfaces = null;

        internal static List<Tuple<string, List<Tuple<string, string>>>> NNPhysicalInterfaces
        {
            get { return nnPhysicalInterfaces; }
        }

        private static Dictionary<string, string> nodeNeighbors = null;

        internal static Dictionary<string, string> NodeNeighbors
        {
            get { return nodeNeighbors; }
        }

        private static Dictionary<string, string> nnUnspecifiedInterfaces = null;

        internal static Dictionary<string, string> NNUnspecifiedInterfaces
        {
            get { return nnUnspecifiedInterfaces; }
        }

        private static Dictionary<string, List<string>> aliases = null;

        internal static Dictionary<string, List<string>> Aliases
        {
            get { return aliases; }
        }

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
        public static void Debug()
        {
            //string description = "TRUNK TO MSAN02-D1-SNT-MRC-1 PORT 5";
            //string descriptionPart;

            //string node = Probe.FindNode(description, out descriptionPart);

            //System.Console.WriteLine("d:" + description + ". n:" + node + ". p:" + descriptionPart);
        }

        public static void Test(string name)
        {
            prioritize.Enqueue(name + "*");
        }
#else
        internal static void Log(string source, string log)
        {
            Insert insert = JoviceDatabase.Insert("Log");
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
                Result result;
                Batch batch;
                Insert insert;

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
                    Event("Database exception: " + eventArgs.Message + ", SQL: " + eventArgs.Sql, "JOVICE");
                };
                jovice.QueryFailed += delegate (object sender, QueryFailedEventArgs e)
                {
                    Event("Query failed, retrying...");
                    Thread.Sleep(2000);
                };
                jovice.Attempts = 3;

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

                        string currentNode;
                        int count;

                        result = jovice.Query(@"
select NO_Name, LEN(NO_Name) as NO_LEN, PI_Name, LEN(PI_Name) as PI_LEN, PI_Type, PI_ID, PI_Description, PI_PI, PI_TO_MI from (
select NO_Name, NO_ID from Node where NO_Type = 'P'
union
select NA_Name, NA_NO from NodeAlias, Node where NA_NO = NO_ID and NO_Type = 'P'
) n, PEInterface
where NO_ID = PI_NO and PI_Description is not null and ltrim(rtrim(PI_Description)) <> '' and PI_Type in ('Hu', 'Te', 'Gi', 'Fa', 'Et')
order by NO_LEN desc, NO_Name, PI_LEN desc, PI_Name
");
                        pePhysicalInterfaces = new List<Tuple<string, List<Tuple<string, string, string, string, string, string>>>>();
                        List<Tuple<string, string, string, string, string, string>> currentPEInterfaces = new List<Tuple<string, string, string, string, string, string>>();

                        currentNode = null;
                        count = 0;

                        foreach (Row row in result)
                        {
                            string node = row["NO_Name"].ToString();

                            if (currentNode != node)
                            {
                                if (currentNode != null)
                                {
                                    pePhysicalInterfaces.Add(new Tuple<string, List<Tuple<string, string, string, string, string, string>>>(currentNode,
                                        new List<Tuple<string, string, string, string, string, string>>(currentPEInterfaces)));
                                    currentPEInterfaces.Clear();
                                }
                                currentNode = node;
                            }

                            currentPEInterfaces.Add(new Tuple<string, string, string, string, string, string>(
                                row["PI_Name"].ToString(), row["PI_Description"].ToString(), row["PI_ID"].ToString(), row["PI_Type"].ToString(), row["PI_PI"].ToString(), row["PI_TO_MI"].ToString()));
                            count++;
                        }
                        pePhysicalInterfaces.Add(new Tuple<string, List<Tuple<string, string, string, string, string, string>>>(currentNode, currentPEInterfaces));

                        Event("Loaded " + count + " PE physical interfaces");

                        result = jovice.Query(@"
select NO_Name, LEN(NO_Name) as NO_LEN, MI_Name, LEN(MI_Name) as MI_LEN, MI_Type, MI_ID, MI_Description from (
select NO_Name, NO_ID from Node where NO_Type = 'M'
union
select NA_Name, NA_NO from NodeAlias, Node where NA_NO = NO_ID and NO_Type = 'M'
) n, MEInterface
where NO_ID = MI_NO and MI_Description is not null and ltrim(rtrim(MI_Description)) <> '' and MI_Type in ('Hu', 'Te', 'Gi', 'Fa', 'Et')
order by NO_LEN desc, NO_Name, MI_LEN desc, MI_Name
");

                        mePhysicalInterfaces = new List<Tuple<string, List<Tuple<string, string, string, string>>>>();
                        List<Tuple<string, string, string, string>> currentMEInterfaces = new List<Tuple<string, string, string, string>>();

                        currentNode = null;
                        count = 0;

                        foreach (Row row in result)
                        {
                            string node = row["NO_Name"].ToString();

                            if (currentNode != node)
                            {
                                if (currentNode != null)
                                {
                                    mePhysicalInterfaces.Add(new Tuple<string, List<Tuple<string, string, string, string>>>(currentNode,
                                        new List<Tuple<string, string, string, string>>(currentMEInterfaces)));
                                    currentMEInterfaces.Clear();
                                }
                                currentNode = node;
                            }

                            currentMEInterfaces.Add(new Tuple<string, string, string, string>(
                                row["MI_Name"].ToString(), row["MI_Description"].ToString(), row["MI_ID"].ToString(), row["MI_Type"].ToString()));
                            count++;
                        }
                        mePhysicalInterfaces.Add(new Tuple<string, List<Tuple<string, string, string, string>>>(currentNode, currentMEInterfaces));

                        Event("Loaded " + count + " ME physical interfaces");

                        result = jovice.Query(@"
select NO_Name, NA_Name from Node, NodeAlias where NA_NO = NO_ID
");
                        aliases = new Dictionary<string, List<string>>();

                        count = 0;

                        foreach (Row row in result)
                        {
                            string noName = row["NO_Name"].ToString();

                            if (!aliases.ContainsKey(noName))
                                aliases.Add(noName, new List<string>());

                            aliases[noName].Add(row["NA_Name"].ToString());
                            count++;
                        }

                        Event("Loaded " + count + " node aliases");

                        result = jovice.Query("select * from NodeNeighbor");

                        nodeNeighbors = new Dictionary<string, string>();

                        foreach (Row row in result)
                        {
                            nodeNeighbors.Add(row["NN_Name"].ToString(), row["NN_ID"].ToString());
                        }

                        Event("Loaded " + nodeNeighbors.Count + " neighbors");

                        result = jovice.Query(@"
select NN_Name, LEN(NN_Name) as NN_LEN, NI_Name, LEN(NI_Name) as NI_LEN, NI_ID from 
(select NN_Name, NN_ID from NodeNeighbor
) n left join NeighborInterface on NI_NN = NN_ID and NI_Name <> 'UNSPECIFIED'
order by NN_LEN desc, NN_Name, NI_LEN desc, NI_Name
");

                        nnPhysicalInterfaces = new List<Tuple<string, List<Tuple<string, string>>>>();
                        List<Tuple<string, string>> currentNNInterfaces = new List<Tuple<string, string>>();

                        currentNode = null;
                        count = 0;

                        foreach (Row row in result)
                        {
                            string node = row["NN_Name"].ToString();

                            if (currentNode != node)
                            {
                                if (currentNode != null)
                                {
                                    nnPhysicalInterfaces.Add(new Tuple<string, List<Tuple<string, string>>>(currentNode,
                                        new List<Tuple<string, string>>(currentNNInterfaces)));
                                    currentNNInterfaces.Clear();
                                }
                                currentNode = node;
                            }

                            string niName = row["NI_Name"].ToString();

                            if (niName != null)
                            {
                                currentNNInterfaces.Add(new Tuple<string, string>(
                                    niName, row["NI_ID"].ToString()));
                                count++;
                            }
                        }
                        nnPhysicalInterfaces.Add(new Tuple<string, List<Tuple<string, string>>>(currentNode, currentNNInterfaces));

                        Event("Loaded " + count + " neighbor interfaces");

                        result = jovice.Query(@"
select NN_ID, NN_Name, NI_ID from NodeNeighbor left join NeighborInterface on NI_NN = NN_ID and NI_Name = 'UNSPECIFIED'
");
                        batch.Begin();

                        nnUnspecifiedInterfaces = new Dictionary<string, string>();

                        foreach (Row row in result)
                        {
                            string id = row["NN_ID"].ToString();
                            string node = row["NN_Name"].ToString();
                            string unid = row["NI_ID"].ToString();

                            if (unid != null)
                            {
                                if (!nnUnspecifiedInterfaces.ContainsKey(node))
                                    nnUnspecifiedInterfaces.Add(node, unid);
                                else
                                {
                                    batch.Execute("update PEInterface set PI_TO_NI = NULL where PI_TO_NI in (select NI_ID from NeighborInterface where NI_NN = {0})", id);
                                    batch.Execute("update MEInterface set MI_TO_NI = NULL where PI_TO_NI in (select NI_ID from NeighborInterface where NI_NN = {0})", id);
                                    batch.Execute("delete from NeighborInterface where NI_NN = {0}", id);
                                    batch.Execute("delete from NodeNeighbor where NN_ID = {0}", id);
                                    Event("Removed duplicated neighbor key: " + node);
                                }
                            }
                            else
                            {
                                unid = Database.ID();

                                insert = jovice.Insert("NeighborInterface");
                                insert.Value("NI_ID", unid);
                                insert.Value("NI_NN", id);
                                insert.Value("NI_Name", "UNSPECIFIED");

                                batch.Execute(insert);
                                nnUnspecifiedInterfaces.Add(node, unid);

                                Event("Added missing UNSPECIFIED interface to neighbor node " + node);
                            }
                        }

                        batch.Commit();

                        Event("Loaded " + nnUnspecifiedInterfaces.Count + " neighbor references");


                        Event("Database virtualizations completed");

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
