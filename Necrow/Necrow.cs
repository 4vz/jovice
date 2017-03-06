using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Globalization;
using System.IO;

using Aphysoft.Share;


namespace Center
{
    public class ProbeProperties
    {
        #region Fields

        private TimeSpan timeStart;

        public TimeSpan TimeStart
        {
            get { return timeStart; }
            set { timeStart = value; }
        }

        private TimeSpan timeEnd;

        public TimeSpan TimeEnd
        {
            get { return timeEnd; }
            set { timeEnd = value; }
        }

        private string probeCase;

        public string Case
        {
            get { return probeCase; }
            set { probeCase = value; }
        }

        private string sshUser;

        public string SSHUser
        {
            get { return sshUser; }
            set { sshUser = value; }
        }

        private string sshPassword;

        public string SSHPassword
        {
            get { return sshPassword; }
            set { sshPassword = value; }
        }

        private string tacacUser;

        public string TacacUser
        {
            get { return tacacUser; }
            set { tacacUser = value; }
        }

        private string tacacPassword;

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

    public class ProbeRequestData
    {
        #region Fields

        private Connection connection;

        public Connection Connection
        {
            get { return connection; }
        }

        private ServerNecrowServiceMessage message;

        public ServerNecrowServiceMessage Message
        {
            get { return message; }
        }

        #endregion

        #region Constructors

        public ProbeRequestData(Connection connection, ServerNecrowServiceMessage message)
        {
            this.connection = connection;
            this.message = message;
        }

        #endregion
    }

    public static class Necrow
    {
        #region Fields

        internal readonly static int Version = 18;

        private static Database j = null;

#if DEBUG
        private static bool console = true;
#else
        private static bool console = false;
#endif
        private static Dictionary<string, Queue<Tuple<int, string>>> list = null;

        private static Queue<Tuple<string, ProbeRequestData>> prioritize = new Queue<Tuple<string, ProbeRequestData>>();

        private static List<Tuple<string, string, string>> supportedVersions = null;

        private static bool mainLoop = true;

        private static Dictionary<string, Probe> instances = null;
       
        internal static Dictionary<string, Dictionary<string, object>> keeperNode = null;

        private static Timer helloTimer;

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
            prioritize.Enqueue(new Tuple<string, ProbeRequestData>(name.ToUpper() + "*", null));
        }
#else
        internal static void Log(string source, string message, string stacktrace)
        {
            Insert insert = j.Insert("ProbeLog");
            insert.Value("XL_TimeStamp", DateTime.UtcNow);
            insert.Value("XL_Source", source);
            insert.Value("XL_Message", message);
            insert.Value("XL_StackTrace", stacktrace);
            insert.Execute();
        }

        internal static void Log(string source, string message)
        {
            Log(source, message, null);
        }
#endif

        public static bool InTime(ProbeProperties properties)
        {
            TimeSpan start = properties.TimeStart;
            TimeSpan end = properties.TimeEnd;
            TimeSpan time = DateTime.UtcNow.TimeOfDay;

            return (start < end && start < time && time < end) ||
                   (start > end && (time > start || time < end)) ||
                   (start == end);
        }

        public static void Start()
        {
            Thread start = new Thread(new ThreadStart(delegate ()
            {
                Batch batch;
                Result result;

                Culture.Default();
                Event("Necrow Starting...");

                Service.Client();
                Service.Connected += delegate (Connection connection)
                {                    
                    Event("Connecting to Service...");
                    helloTimer = new Timer(new TimerCallback(delegate (object state)
                    {
                        Service.Send(new ServerNecrowServiceMessage(NecrowServiceMessageType.Hello));
                    }), null, 0, 20000);
                    
                };
                Service.Register(typeof(ServerNecrowServiceMessage), NecrowServiceMessageHandler);

                Event("Checking Jovice Database connection... ");

                bool joviceDatabaseConnected = false;
                j = Jovice.Database;

                DatabaseExceptionEventHandler checkingDatabaseException = delegate (object sender, DatabaseExceptionEventArgs eventArgs)
                {
                    Event("Connection Failed: " + eventArgs.Message);
                };

                j.Exception += checkingDatabaseException;

                if (j.Test())
                {
                    joviceDatabaseConnected = true;
                    Event("Jovice Database OK");
                }

                j.Exception -= checkingDatabaseException;

                j.Exception += delegate (object sender, DatabaseExceptionEventArgs eventArgs)
                {
                    Event("Database exception has been caught: " + eventArgs.Message.Trim(new char[] { '\r', '\n', ' ' }));
                    //throw new Exception(eventArgs.Message.Trim(new char[] { '\r', '\n', ' ' }) + "\n" + eventArgs.Sql);
                };
                j.Retry += delegate (object sender, DatabaseExceptionEventArgs eventArgs)
                {
                    if (eventArgs.Exception == DatabaseException.Timeout)
                    {
                        Event("Database query has timed out, retrying");
                    }
                    else
                    {
                        eventArgs.NoRetry = true;
                    }
                };
                j.QueryAttempts = 5;
                j.Timeout = 300;

                if (joviceDatabaseConnected)
                {
                    batch = j.Batch();

                    #region Graph

                    //JoviceGraph.Update();

                    #endregion

                    #region Database Check

                    Event("Checking database...");

                    DatabaseCheck();

                    Event("Database checks completed");

                    #endregion

                    #region Virtualizations

                    Event("Starting database virtualizations...");

                    NecrowVirtualization.Load();

                    Event("Database virtualizations completed");

                    #endregion
                    
                    #region Etc

                    interfaceTestPrefixes = new Dictionary<string, string[]>();
                    interfaceTestPrefixes.Add("Hu", new string[] { "H", "HU", "GI", "GE" });
                    interfaceTestPrefixes.Add("Te", new string[] { "T", "TE", "TENGIGE", "GI", "GE", "XE" }); // kadang Te-gig direfer sebagai Gi dammit people
                    interfaceTestPrefixes.Add("Gi", new string[] { "G", "GI", "GE", "GIGAE", "GIGABITETHERNET", "TE" }); // kadang Te-gig direfer sebagai Gi dammit people
                    interfaceTestPrefixes.Add("Fa", new string[] { "F", "FA", "FE", "FASTE" });
                    interfaceTestPrefixes.Add("Et", new string[] { "E", "ET", "ETH" });
                    interfaceTestPrefixes.Add("Ag", new string[] { "LAG", "ETH-TRUNK", "BE" });

                    #endregion

                    #region Probe initialization

                    // PROBE LIST

                    Event("Loading probe list...");

                    list = new Dictionary<string, Queue<Tuple<int, string>>>();

                    list.Add("MAIN", new Queue<Tuple<int, string>>());
                    list.Add("M", new Queue<Tuple<int, string>>());

                    foreach (Row xp in j.Query("select * from ProbeProgress order by XP_ID asc"))
                    {
                        string c = xp["XP_Case"].ToString();
                        if (c == null) c = "MAIN";
                        if (list.ContainsKey(c)) list[c].Enqueue(new Tuple<int, string>(xp["XP_ID"].ToInt(), xp["XP_NO"].ToString()));
                    }

                    foreach (KeyValuePair<string, Queue<Tuple<int, string>>> pair in list)
                    {
                        if (pair.Value.Count == 0)
                        {
                            CreateNodeQueue(pair.Key);
                        }
                        else
                        {
                            Event("Case: " + pair.Key + " is using existing list, " + pair.Value.Count + " node" + (pair.Value.Count > 1 ? "s" : "") + " remaining");
                        }
                    }                    

                    j.Execute("update ProbeProgress set XP_StartTime = NULL, XP_Status = NULL");

                    // SUPPORTED VERSION

                    if (supportedVersions == null)
                    {
                        Result sver = j.Query("select * from NodeSupport");

                        supportedVersions = new List<Tuple<string, string, string>>();

                        foreach (Row sve in sver)
                        {
                            supportedVersions.Add(new Tuple<string, string, string>(sve["NT_Manufacture"].ToString(), sve["NT_Version"].ToString(), sve["NT_SubVersion"].ToString()));
                        }
                    }

                    #endregion

                    #region Database Keepers

                    #region Node Keeper

                    result = j.Query("select * from Node");
                    keeperNode = new Dictionary<string, Dictionary<string, object>>();

                    foreach (Row row in result)
                    {
                        Dictionary<string, object> values = new Dictionary<string, object>();
                        keeperNode.Add(row["NO_ID"].ToString(), values);

                        values.Add("NO_Name", row["NO_Name"].ToString());
                        values.Add("NO_Type", row["NO_Type"].ToString());
                        values.Add("NO_Manufacture", row["NO_Manufacture"].ToString());
                        values.Add("NO_IP", row["NO_IP"].ToString());
                    }

                    #endregion

                    #endregion

                    instances = new Dictionary<string, Probe>();

                    long loops = 0;

                    while (mainLoop)
                    {
                        #region Check Regularly
                        if (loops % 10 == 0)
                        {
                            result = j.Query("select * from Node");

                            batch.Begin();
                            foreach (Row row in result)
                            {
                                string id = row["NO_ID"].ToString();

                                if (keeperNode.ContainsKey(id))
                                {
                                    Dictionary<string, object> keeper = keeperNode[id];

                                    Update update = j.Update("Node");
                                    update.Where("NO_ID", id);

                                    if ((string)keeper["NO_Name"] != row["NO_Name"].ToString()) update.Set("NO_Name", (string)keeper["NO_Name"]);
                                    if ((string)keeper["NO_Type"] != row["NO_Type"].ToString()) update.Set("NO_Type", (string)keeper["NO_Type"]);
                                    if ((string)keeper["NO_Manufacture"] != row["NO_Manufacture"].ToString()) update.Set("NO_Manufacture", (string)keeper["NO_Manufacture"]);
                                    if ((string)keeper["NO_IP"] != row["NO_IP"].ToString()) update.Set("NO_IP", (string)keeper["NO_IP"]);

                                    batch.Execute(update);
                                }
                                else
                                {
                                    Dictionary<string, object> values = new Dictionary<string, object>();
                                    keeperNode.Add(row["NO_ID"].ToString(), values);

                                    string newNodeName = row["NO_Name"].ToString();
                                    values.Add("NO_Name", newNodeName);
                                    values.Add("NO_Type", row["NO_Type"].ToString());
                                    values.Add("NO_Manufacture", row["NO_Manufacture"].ToString());
                                    values.Add("NO_IP", row["NO_IP"].ToString());

                                    prioritize.Enqueue(new Tuple<string, ProbeRequestData>(newNodeName, null));

                                    Event("New Node Registered: " + newNodeName);
                                }
                            }
                            batch.Commit();

                            result = j.Query(@"
select XA_ID, XA_TimeStart, XA_TimeEnd, XA_Case, XU_ServerUser, XU_ServerPassword, XU_TacacUser, XU_TacacPassword, XS_Address, XS_ConsolePrefixFormat
from ProbeAccess, ProbeUser, ProbeServer where XA_XU = XU_ID and XU_XS = XS_ID");

                            foreach (Row row in result)
                            {
                                string id = row["XA_ID"].ToString();

                                if (!instances.ContainsKey(id))
                                {
                                    // NEW
                                    ProbeProperties prop = new ProbeProperties();
                                    prop.SSHUser = row["XU_ServerUser"].ToString();
                                    prop.SSHPassword = row["XU_ServerPassword"].ToString();
                                    prop.TacacUser = row["XU_TacacUser"].ToString();
                                    prop.TacacPassword = row["XU_TacacPassword"].ToString();
                                    prop.SSHServerAddress = row["XS_Address"].ToString();
                                    prop.SSHTerminal = string.Format(row["XS_ConsolePrefixFormat"].ToString(), prop.SSHUser);
                                    prop.TimeStart = row["XA_TimeStart"].ToTimeSpan(TimeSpan.MinValue);
                                    prop.TimeEnd = row["XA_TimeEnd"].ToTimeSpan(TimeSpan.MaxValue);
                                    string accessCase = row["XA_Case"].ToString();
                                    prop.Case = accessCase == null ? "MAIN" : accessCase;

                                    if (prop.Case.InOf("MAIN", "M") > -1)
                                    {
                                        Thread.Sleep(100);
                                        instances.Add(id, Probe.Create(prop, "PROBE" + id.Trim()));

                                        Event("ADD PROBE" + id.Trim() + ": " + prop.SSHUser + "@" + prop.SSHServerAddress + " [" + prop.TacacUser + "] " + ((prop.TimeStart == TimeSpan.Zero || prop.TimeEnd == TimeSpan.Zero) ? "" : (prop.TimeStart + "-" + prop.TimeEnd)));
                                    }
                                    else
                                    {
                                        Event("PROBE" + id.Trim() + " CASE INVALID");
                                    }
                                }
                                else
                                {
                                    // UPDATE
                                    ProbeProperties prop = instances[id].Properties;

                                    List<string> updateinfo = new List<string>();

                                    // change timestart
                                    TimeSpan newstart = row["XA_TimeStart"].ToTimeSpan(TimeSpan.MinValue);
                                    TimeSpan newend = row["XA_TimeEnd"].ToTimeSpan(TimeSpan.MaxValue);
                                    bool updatetime = false;

                                    if (newstart != prop.TimeStart)
                                    {
                                        updatetime = true;
                                        prop.TimeStart = newstart;
                                    }
                                    if (newend != prop.TimeEnd)
                                    {
                                        updatetime = true;
                                        prop.TimeEnd = newend;
                                    }
                                    if (updatetime) updateinfo.Add("time changed to " + ((prop.TimeStart == TimeSpan.Zero || prop.TimeEnd == TimeSpan.Zero) ? "" : (prop.TimeStart + "-" + prop.TimeEnd)));

                                    // change case
                                    string newCase = row["XA_Case"].ToString();
                                    newCase = newCase == null ? "MAIN" : newCase;

                                    if (newCase != prop.Case)
                                    {
                                        updateinfo.Add("case " + prop.Case + " -> " + newCase);
                                        prop.Case = newCase;
                                    }

                                    // change tacac
                                    string newtacacuser = row["XU_TacacUser"].ToString();
                                    string newtacacpassword = row["XU_TacacPassword"].ToString();

                                    if (newtacacuser != prop.TacacUser)
                                    {
                                        updateinfo.Add("tacacuser " + prop.TacacUser + " -> " + newtacacuser);
                                        prop.TacacUser = newtacacuser;
                                    }
                                    if (newtacacpassword != prop.TacacPassword)
                                    {
                                        updateinfo.Add("tacacpass changed");
                                        prop.TacacPassword = newtacacpassword;
                                    }

                                    // change ssh
                                    string newsshuser = row["XU_ServerUser"].ToString();
                                    string newsshpassword = row["XU_ServerPassword"].ToString();
                                    string newsshserveraddress = row["XS_Address"].ToString();

                                    bool updatessh = false;

                                    if (newsshuser != prop.SSHUser)
                                    {
                                        updatessh = true;
                                        updateinfo.Add("sshuser " + prop.SSHUser + " -> " + newsshuser);
                                        prop.SSHUser = newsshuser;
                                    }
                                    if (newsshpassword != prop.SSHPassword)
                                    {
                                        updatessh = true;
                                        updateinfo.Add("sshpass changed");
                                        prop.SSHPassword = newsshpassword;
                                    }
                                    if (newsshserveraddress != prop.SSHServerAddress)
                                    {
                                        updatessh = true;
                                        updateinfo.Add("sshaddress " + prop.SSHServerAddress + " -> " + newsshserveraddress);
                                        prop.SSHServerAddress = newsshserveraddress;
                                    }

                                    if (updateinfo.Count > 0)
                                    {
                                        Event("UPDATE PROBE" + id.Trim() + ": " + string.Join(", ", updateinfo.ToArray()));
                                    }

                                    if (updatessh)
                                    {
                                        prop.SSHTerminal = string.Format(row["XS_ConsolePrefixFormat"].ToString(), prop.SSHUser);
                                        instances[id].QueueStop();
                                    }
                                }
                            }

                            List<string> remove = new List<string>();
                            foreach (KeyValuePair<string, Probe> pair in instances)
                            {
                                bool found = false;
                                foreach (Row row in result)
                                {
                                    string id = row["XA_ID"].ToString();
                                    if (pair.Key == id)
                                    {
                                        found = true;
                                        break;
                                    }
                                }

                                if (!found)
                                {
                                    Event("DELETE PROBE" + pair.Key.Trim());
                                    pair.Value.QueueStop();
                                    remove.Add(pair.Key);
                                }
                            }
                            foreach (string key in remove)
                                instances.Remove(key);
                        }
                        #endregion

                        /* 
                         * STARTED = probe is started
                         * CONNECTED = probe is connected to SSH
                         * PROBING = probe is probing to Node
                         * 
                         */

                        foreach (KeyValuePair<string, Probe> pair in instances)
                        {
                            Probe probe = pair.Value;
                            
                            if (probe.IsStarted)
                            {
                                if (probe.IsConnected)
                                {
                                    if (probe.IsProbing)
                                    {
                                    }
                                    else
                                    {
                                        if (!InTime(probe.Properties))
                                        {
                                            Event("Probe session has ended");
                                            probe.SessionStart = false;
                                            probe.Stop();
                                        }
                                        else if ((DateTime.UtcNow - probe.SSHProbeStartTime).TotalHours > 3)
                                        {
                                            Event("Restarting the probe");
                                            probe.Stop();
                                        }
                                    }
                                }
                                else
                                {
                                }
                            }
                            else
                            {
                                if (InTime(probe.Properties))
                                {
                                    if (!probe.SessionStart)
                                    {
                                        Event("Probe session has started");
                                        probe.SessionStart = true;
                                    }
                                    probe.Start();
                                }
                            }
                        }

                        Thread.Sleep(1000);
                        loops++;
                    }
                }
            }));
            start.Start();
        }

        public static void Stop()
        {

        }

        private static void CreateNodeQueue(string queueCase)
        {
            lock (list)
            {
                if (list[queueCase].Count == 0)
                {
                    List<string> newIDs = new List<string>();
                    string dbCase = null;

                    int excluded = 0;

                    if (queueCase == "MAIN")
                    {
                        #region MAIN

                        Event("Preparing list for main list...");

                        Result nres = j.Query(@"
select NO_ID from Node where NO_Active = 1 and NO_Type in ('P', 'M') and NO_TimeStamp is null and NO_LastConfiguration is null                        
");
                        Result mres = j.Query(@"
select a.NO_ID, a.NO_Name, a.NO_Remark, a.NO_TimeStamp, CASE WHEN a.span < 0 then 0 else a.span end as span from (
select NO_ID, NO_Name, NO_Remark, NO_LastConfiguration, NO_TimeStamp, DateDiff(hour, NO_LastConfiguration, NO_TimeStamp) as span 
from Node where NO_Active = 1 and NO_Type in ('P', 'M') and NO_TimeStamp is not null and NO_LastConfiguration is not null
) a
order by span asc, a.NO_LastConfiguration asc
");
                        Result sres = j.Query(@"
select NO_ID from Node where NO_Active = 1 and NO_Type in ('P', 'M') and NO_TimeStamp is not null and NO_LastConfiguration is null                        
");

                        foreach (Row row in nres) newIDs.Add(row["NO_ID"].ToString());
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

                            newIDs.Add(row["NO_ID"].ToString());
                        }
                        foreach (Row row in sres) newIDs.Add(row["NO_ID"].ToString());

                        #endregion
                    }
                    else if (queueCase == "M")
                    {
                        #region M

                        Event("Preparing list for mac-address list...");

                        Result sres = j.Query("select * from Node where NO_Active = 1 and NO_Type = 'M'");

                        foreach (Row row in sres)
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

                            newIDs.Add(row["NO_ID"].ToString());
                        }

                        #endregion

                        dbCase = "M";
                    }

                    int total = newIDs.Count + excluded;
                    Event("Total " + total + " nodes available, " + newIDs.Count + " nodes eligible, " + excluded + " excluded in this list");

                    // check incompleted probeprogress
                    List<int> idExists = new List<int>();
                    Result result = j.Query("select XP_ID from ProbeProgress");
                    foreach (Row row in result) idExists.Add(row["XP_ID"].ToInt());
                    
                    Batch batch = j.Batch();

                    batch.Begin();
                    int id = 1;
                    foreach (string newID in newIDs)
                    {
                        Insert insert = j.Insert("ProbeProgress");

                        while (idExists.Contains(id)) id++; // if id contained in incompleted id, then increase

                        insert.Value("XP_ID", id++);
                        insert.Value("XP_NO", newID);
                        insert.Value("XP_Case", dbCase);
                        batch.Execute(insert);
                    }
                    result = batch.Commit();
                    if (result.Count > 0) Event("List created");

                    foreach (Row xp in j.Query("select XP_ID, XP_NO from ProbeProgress where XP_Case " + (dbCase == null ? "is" : "=") + " {0} order by XP_ID asc", dbCase))
                    {
                        list[queueCase].Enqueue(new Tuple<int, string>(xp["XP_ID"].ToInt(), xp["XP_NO"].ToString()));
                    }
                }
            }
        }

        private static void DatabaseCheck()
        {
            Database jovice = Necrow.j;
            Result result;
            Batch batch = jovice.Batch();

            #region Upper case node name

            result = jovice.Query("select * from Node");

            string[] nodeTypes = new string[] { "P", "M", "S", "H", "D" };
            string[] nodeManufactures = new string[] { "CISCO", "HUAWEI", "ALCATEL-LUCENT", "JUNIPER", "TELLABS" };

            batch.Begin();
            foreach (Row row in result)
            {
                string id = row["NO_ID"].ToString();                

                Update update = jovice.Update("Node");
                update.Where("NO_ID", id);

                string name = row["NO_Name"].ToString();
                if (name.ToUpper() != name) update.Set("NO_Name", name.ToUpper());

                string type = row["NO_Type"].ToString();
                if (type == "p") update.Set("NO_Type", "P");
                else if (type == "m") update.Set("NO_Type", "M");
                else if (type == "s") update.Set("NO_Type", "S");
                else if (type == "h") update.Set("NO_Type", "H");
                else if (type == "d") update.Set("NO_Type", "D");
                else if (type.InOf(nodeTypes) == -1) update.Set("NO_Active", false);

                string man = row["NO_Manufacture"].ToString();
                if (man == "alu" || man == "ALU") update.Set("NO_Manufacture", "ALCATEL-LUCENT");
                else if (man == "hwe" || man == "HWE") update.Set("NO_Manufacture", "HUAWEI");
                else if (man == "cso" || man == "CSO" || man == "csc" || man == "CSC") update.Set("NO_Manufacture", "CISCO");
                else if (man == "jun" || man == "JUN") update.Set("NO_Manufacture", "JUNIPER");
                else if (man.ToUpper().InOf(nodeManufactures) == -1) update.Set("NO_Active", false);
                else if (man.ToUpper() != man) update.Set("NO_Manufacture", man.ToUpper());

                if (!update.IsEmpty) batch.Execute(update);
            }

            if (batch.Count > 0) Event("Checking Node...");
            result = batch.Commit();

            if (result.AffectedRows > 0)
            {
                Event("Affected " + result.AffectedRows + " rows");
            }

            #endregion
            
            bool neighborAffected = false;

            #region Neighbor already exists in node

            result = jovice.Query("select NO_ID, NN_ID from Node left join NodeNeighbor on NN_Name = NO_Name where NN_ID is not null and NO_Type in ('M', 'P') and NO_Active = 1");

            if (result.Count > 0)
            {
                Event("Removing " + result.Count + " duplicated neighbor nodes...");

                batch.Begin();
                foreach (Row row in result)
                {
                    string nnid = row["NN_ID"].ToString();
                    batch.Execute("update MEInterface set MI_TO_NI = NULL where MI_TO_NI in (select NI_ID from NeighborInterface where NI_NN = {0})", nnid);
                    batch.Execute("update PEInterface set PI_TO_NI = NULL where PI_TO_NI in (select NI_ID from NeighborInterface where NI_NN = {0})", nnid);
                    batch.Execute("delete from NeighborInterface where NI_NN = {0}", nnid);
                    batch.Execute("delete from NodeNeighbor where NN_ID = {0}", nnid);
                }
                result = batch.Commit();
                Event("Affected " + result.AffectedRows + " rows");

                neighborAffected = true;
            }

            #endregion

            #region Removing unused interfaces on Node Neighbors

            result = jovice.Query(@"
select NI_ID from NeighborInterface 
left join MEInterface on MI_TO_NI = NI_ID 
left join PEInterface on PI_TO_NI = NI_ID
left join NodeNeighbor on NN_ID = NI_NN
where NI_Name <> 'UNSPECIFIED' and MI_ID is null and PI_ID is null
");
            if (result.Count > 0)
            {
                Event("Removing " + result.Count + " unused interfaces on Node Neighbors...");

                batch.Begin();
                foreach (Row row in result)
                {
                    string ni = row["NI_ID"].ToString();

                    batch.Execute("delete from NeighborInterface where NI_ID = {0}", ni);
                }
                result = batch.Commit();

                Event("Removed " + result.AffectedRows + " interfaces");

                neighborAffected = true;
            }

            #endregion

            if (NecrowVirtualization.IsReady && neighborAffected)
            {
                Event("Reloading neighbor virtualizations...");

                NecrowVirtualization.NeighborLoad();

                Event("Virtualization reloaded");
            }
        }

        internal static Tuple<int, string> NextNode(string probeCase)
        {
            Tuple<int, string> noded = null;

            lock (list[probeCase])
            {
                if (list[probeCase].Count == 0)
                {
                    // here were do things every loop
                    DatabaseCheck();

                    // create new list
                    CreateNodeQueue(probeCase);
                }

                noded = list[probeCase].Dequeue();
            }

            return noded;
        }

        internal static Tuple<string, ProbeRequestData> NextPrioritize()
        {
            Tuple<string, ProbeRequestData> node = null;

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

                    Insert insert = j.Insert("NodeSupport");
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
                        prioritize.Enqueue(new Tuple<string, ProbeRequestData>(nodename.ToUpper(), null));
                    }
                }
            }
        }

        private static void NecrowServiceMessageHandler(MessageEventArgs e)
        {
            ServerNecrowServiceMessage m = (ServerNecrowServiceMessage)e.Message;

            if (m.Type == NecrowServiceMessageType.HelloResponse)
            {
                helloTimer.Dispose();
                Event("Service Connected");
            }
            else if (m.Type == NecrowServiceMessageType.Request)
            {
                //Event("We got request from server! = " + m.RequestID);
            }
            else if (m.Type == NecrowServiceMessageType.ProbeStatus)
            {
                List<object> objects = new List<object>();

                if (instances != null)
                {
                    foreach (KeyValuePair<string, Probe> ins in instances)
                    {
                        if (ins.Value.IsProbing)
                        {
                            objects.Add(new Tuple<string, string, string, DateTime, string>("A", ins.Key.Trim(), ins.Value.NodeName, ins.Value.NodeProbeStartTime, ins.Value.Properties.TacacUser));
                        }
                        else if (InTime(ins.Value.Properties))
                        {
                            objects.Add(new Tuple<string, string, string, DateTime, string>("I", ins.Key.Trim(), ins.Value.LastNodeName, ins.Value.LastProbeEndTime, null));
                        }
                    }
                }

                m.Data = objects.ToArray();
                e.Connection.Reply(m);
            }
            else if (m.Type == NecrowServiceMessageType.Probe)
            {
                string node = (string)m.Data[0];
                string option = "?";
                if (m.Data.Length > 1 && (string)m.Data[1] == "FORCE") option = "*";
                prioritize.Enqueue(new Tuple<string, ProbeRequestData>(node + option, new ProbeRequestData(e.Connection, m)));
            }
        }

        #endregion
    }
}
