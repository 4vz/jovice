using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Globalization;
using System.IO;

using Aphysoft.Share;
using System.Net;
using Jovice;

namespace Necrow
{
    public class ProbeProperties
    {
        #region Fields

        private string days;

        public string Days
        {
            get { return days; }
            set { days = value; }
        }

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

        public string SSHUser { get; set; }

        public string SSHPassword { get; set; }

        public string TacacUser { get; set; }

        public string TacacPassword { get; set; }

        public string SSHServerAddress { get; set; }

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

    public class ProbeUserAccess
    {
        #region Fields

        public string ID { get; set; }
        public string ServerAddress { get; set; }
        public string ServerUser { get; set; }
        public string ServerPassword { get; set; }
        public string TacacUser { get; set; }
        public string TacacPassword { get; set; }

        #endregion

        #region Constructors

        public ProbeUserAccess()
        {

        }

        #endregion
    }

    public partial class Necrow : Node
    {
        #region Fields

        internal readonly static int Version = 51;

        private Database2 jovice;
        private Database2 oss;

        private Queue<Tuple<string, ProbeRequestData>> prioritize = new Queue<Tuple<string, ProbeRequestData>>();        

        public List<Probe> ProbeInstances { get; set; } = null;
        public Dictionary<string, ProbeUserAccess> ProbeUserAccess { get; set; } = null;

        private List<Tuple<string, string, string>> supportedVersions = null;

        private readonly Queue<Tuple<string, string>> queueRunner = new Queue<Tuple<string, string>>();
        private readonly Queue<Tuple<string, string>> queueDeep = new Queue<Tuple<string, string>>();
        private readonly Dictionary<string, Tuple<bool, DateTime>> pendingList = new Dictionary<string, Tuple<bool, DateTime>>();

        internal Dictionary<string, Dictionary<string, object>> KeeperNode { get; private set; } = null;
        internal Dictionary<string, string[]> InterfaceTestPrefixes { get; private set; } = null;

        public object ProbeLock = new object();

        public int RunnerProbeCount
        {
            get
            {
                int cr = 0;

                lock (ProbeInstances)
                {
                    foreach (Probe probe in ProbeInstances)
                    {
                        if (probe.ProbeType == ProbeTypes.Runner) cr++;
                    }
                }

                return cr;
            }
        }
        public int DeepProbeCount
        {
            get
            {
                int cr = 0;

                lock (ProbeInstances)
                {
                    foreach (Probe probe in ProbeInstances)
                    {
                        if (probe.ProbeType == ProbeTypes.Deep) cr++;
                    }
                }

                return cr;
            }
        }

        public int MaxRunnerProbeCount { get; private set; } = 0;
        public int MaxDeepProbeCount { get; private set; } = 0;

        private List<int> probeIDs = new List<int>();

        public string TestNode { get; private set; } = null;
        public ProbeTypes TestProbeType { get; private set; } = ProbeTypes.Runner;

        #endregion

        #region Constructors 

        public Necrow() : base("NECROW")
        {
        }

        public Necrow(string testNode, ProbeTypes testProbeType) : base("NECROW")
        {
            TestNode = testNode;
            TestProbeType = testProbeType;
        }

        #endregion

        #region Methods

        internal void PendingNode(Probe probe, string progressID, TimeSpan duration)
        {
            if (progressID != null)
            {
                DateTime until = DateTime.UtcNow + duration;

                lock (pendingList)
                {
                    pendingList.Add(progressID, new Tuple<bool, DateTime>(probe.ProbeType == ProbeTypes.Deep, until));
                }

                jovice.Execute("update ProbeProgress set XP_StartTime = NULL, XP_Pending = {0} where XP_ID = {1}", until, progressID);

                probe.ProgressPending = true;
            }
        }

        internal void DisableNode(string nodeID)
        {            
            Result2 result = jovice.Query("select NO_Name from Node where NO_ID = {0}", nodeID);

            if (result.Count == 1)
            {
                Row2 row = result[0];

                string nodeName = row["NO_Name"].ToString();

                Event(nodeName + " is become inactive");

                lock (KeeperNode)
                {
                    if (KeeperNode.ContainsKey(nodeID))
                    {
                        Dictionary<string, object> values = KeeperNode[nodeID];
                        values["NO_Active"] = false;
                    }

                    jovice.Execute("update Node set NO_Active = 0 where NO_ID = {0}", nodeID);
                }
                
                // remove from node virtualization
                NecrowVirtualization.RemovePhysicalInterfacesByNode(nodeName);
            }
        }

        internal void UpdateManufacture(string nodeID, string manufacture)
        {
            Result2 result = jovice.Query("select NO_Name from Node where NO_ID = {0}", nodeID);

            if (result.Count == 1)
            {
                lock (KeeperNode)
                {
                    if (KeeperNode.ContainsKey(nodeID))
                    {
                        Dictionary<string, object> values = KeeperNode[nodeID];
                        values["NO_Manufacture"] = manufacture;
                    }

                    jovice.Execute("update Node set NO_Manufacture = {1} where NO_ID = {0}", nodeID, manufacture);
                }
            }
        }
     
        private void DatabaseCheck()
        {
            Result2 result;
            Batch batch = jovice.Batch();

            #region Upper case node name

            result = jovice.Query("select * from Node");

            string[] nodeTypes = new string[] { "P", "M", "T", "S", "H", "D" };
            string[] nodeManufactures = new string[] { "CISCO", "HUAWEI", "ALCATEL-LUCENT", "JUNIPER", "TELLABS" };

            batch.Begin();
            foreach (Row2 row in result)
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
                else if (type.ArgumentIndexOf(nodeTypes) == -1) update.Set("NO_Active", false);

                string man = row["NO_Manufacture"].ToString();

                if (man != null && man.ArgumentIndexOf(nodeManufactures) == -1)
                {
                    update.Set("NO_Manufacture", null);
                    man = null;
                }                

                if (man == null)
                {
                    if (row["NO_Model"].ToString() != null) update.Set("NO_Model", null);
                    if (row["NO_Version"].ToString() != null) update.Set("NO_Version", null);
                    if (row["NO_SubVersion"].ToString() != null) update.Set("NO_SubVersion", null);
                }

                if (!update.IsEmpty) batch.Add(update);
            }

            if (batch.Count > 0) Event("Checking Node...");
            result = batch.Commit();

            if (result.AffectedRows > 0)
            {
                Event("Affected " + result.AffectedRows + " rows");
            }

            #endregion

            #region Neighbor already exists in node

            bool neighborAffected = false;

            result = jovice.Query("select NO_ID, NN_ID from Node left join NodeNeighbor on NN_Name = NO_Name where NN_ID is not null and NO_Type in ('M', 'P') and NO_Active = 1");
            
            if (result > 0)
            {
                Event($"Removing {result} duplicated neighbor nodes...");

                batch.Begin();
                foreach (Row2 row in result)
                {
                    string nnid = row["NN_ID"].ToString();
                    batch.Add("update MEInterface set MI_TO_NI = NULL where MI_TO_NI in (select NI_ID from NBInterface where NI_NN = {0})", nnid);
                    batch.Add("update PEInterface set PI_TO_NI = NULL where PI_TO_NI in (select NI_ID from NBInterface where NI_NN = {0})", nnid);
                    batch.Add("delete from NBInterface where NI_NN = {0}", nnid);
                    batch.Add("delete from NodeNeighbor where NN_ID = {0}", nnid);
                }
                result = batch.Commit();
                Event("Affected " + result.AffectedRows + " rows");

                neighborAffected = true;
            }

            #endregion

            #region Removing unused interfaces on Node Neighbors

            result = jovice.Query(@"
select NI_ID from NBInterface 
left join MEInterface on MI_TO_NI = NI_ID 
left join PEInterface on PI_TO_NI = NI_ID
left join NodeNeighbor on NN_ID = NI_NN
where NI_Name <> 'UNSPECIFIED' and MI_ID is null and PI_ID is null
");
            if (result.Count > 0)
            {
                Event("Removing " + result.Count + " unused interfaces on Node Neighbors...");

                batch.Begin();
                foreach (Row2 row in result)
                {
                    string ni = row["NI_ID"].ToString();

                    batch.Add("delete from NBInterface where NI_ID = {0}", ni);
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

        internal void AcknowledgeNodeVersion(string manufacture, string version, string subVersion)
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

                    Insert insert = jovice.Insert("NodeSupport");
                    insert.Value("NT_ID", Database2.ID());
                    insert.Value("NT_Manufacture", manufacture);
                    insert.Value("NT_Version", version);
                    insert.Value("NT_SubVersion", subVersion);
                    insert.Execute();
                }
            }
        }

        internal void Log(string source, string message, string stacktrace)
        {
            Insert insert = jovice.Insert("ProbeLog");
            insert.Value("XL_TimeStamp", DateTime.UtcNow);
            insert.Value("XL_Source", source);
            insert.Value("XL_Message", message);
            insert.Value("XL_StackTrace", stacktrace);
            insert.Execute();
        }

        internal void Log(string source, string message)
        {
            Log(source, message, null);
        }

        private void NextPending(bool deep, out string nodeID, out string progressID)
        {
            nodeID = null;
            progressID = null;

            lock (pendingList)
            {
                List<Tuple<string, DateTime>> qualifiedList = new List<Tuple<string, DateTime>>();

                foreach (KeyValuePair<string, Tuple<bool, DateTime>> pair in pendingList)
                {
                    if (pair.Value.Item1 == deep)
                    {
                        if (pair.Value.Item2 < DateTime.UtcNow)
                        {
                            qualifiedList.Add(new Tuple<string, DateTime>(pair.Key, pair.Value.Item2));
                        }
                    }
                }

                if (qualifiedList.Count > 0)
                {
                    qualifiedList.Sort((a, b) => a.Item2.CompareTo(b.Item2));

                    progressID = qualifiedList[0].Item1;

                    Column2 noc = jovice.Scalar(@"select XP_NO from ProbeProgress where XP_ID = {0}", progressID);

                    pendingList.Remove(progressID);

                    if (noc != null)
                    {
                        nodeID = noc.ToString();
                        
                    }
                    else
                    {
                        RemoveProgress(progressID);

                    }
                    
                    // we took from pending, remove from pendingList
                    
                }
            }
        }

        internal void NextRunnerNode(out string nodeID, out string progressID)
        {
            nodeID = null;
            progressID = null;

            if (TestNode != null)
            {
                nodeID = TestNode;
                return;
            }

            NextPending(false, out nodeID, out progressID);

            if (nodeID == null)
            {
                lock (queueRunner)
                {
                    if (queueRunner.Count == 0)
                    {
                        DatabaseCheck();

                        Event("Creating queue list...");

                        List<string> queueNodeIDs = new List<string>();

                        Result2 nres = jovice.Query(@"
select NO_ID from Node where NO_Active = 1 and NO_Type in ('P', 'M', 'T', 'F') and NO_TimeStamp is null and NO_LastConfiguration is null                        
");
                        Result2 mres = jovice.Query(@"
select a.NO_ID, a.NO_Name, CASE WHEN a.span < 0 then 0 else a.span end as span from (
select NO_ID, NO_Name, NO_Type, NO_LastConfiguration, DateDiff(hour, NO_LastConfiguration, NO_TimeStamp) as span, DATEDIFF(hour, NO_TimeStamp, GETUTCDATE()) as span_now
from Node where NO_Active = 1 and NO_Type in ('P', 'M', 'T', 'F') and NO_TimeStamp is not null and NO_LastConfiguration is not null
) a
order by span asc, a.NO_LastConfiguration asc
");
                        Result2 sres = jovice.Query(@"
select NO_ID from Node where NO_Active = 1 and NO_Type in ('P', 'M', 'T', 'F') and NO_TimeStamp is not null and NO_LastConfiguration is null                        
");

                        foreach (Row2 row in nres) queueNodeIDs.Add(row["NO_ID"].ToString());
                        foreach (Row2 row in mres)
                        {
                            string add = row["NO_ID"].ToString();
                            if (!queueNodeIDs.Contains(add))
                                queueNodeIDs.Add(add);
                        }
                        foreach (Row2 row in sres)
                        {
                            string add = row["NO_ID"].ToString();
                            if (!queueNodeIDs.Contains(add))
                                queueNodeIDs.Add(add);
                        }

                        long queueIndex;
                        
                        Result2 result = jovice.Query("select top 1 XP_Queue from ProbeProgress order by XP_Queue desc");
                        if (result.Count > 0) queueIndex = result[0]["XP_Queue"].ToLong() + 1;
                        else queueIndex = 1;

                        Batch batch = jovice.Batch();
                        batch.Begin();

                        foreach (string queueNodeID in queueNodeIDs)
                        {
                            bool existInDeep = false;

                            foreach (Tuple<string, string> tup in queueDeep)
                            {
                                if (tup.Item1 == queueNodeID)
                                {
                                    existInDeep = true;
                                    break;
                                }
                            }

                            if (!existInDeep)
                            {
                                string newProgressID = Database2.ID();

                                Insert insert = jovice.Insert("ProbeProgress");
                                insert.Value("XP_ID", newProgressID);
                                insert.Value("XP_NO", queueNodeID);
                                insert.Value("XP_Queue", queueIndex++);
                                insert.Value("XP_Deep", false);
                                batch.Add(insert);

                                queueRunner.Enqueue(new Tuple<string, string>(queueNodeID, newProgressID));
                            }
                        }
                        result = batch.Commit();
                    }

                    if (queueRunner.Count > 0)
                    {
                        Tuple<string, string> rtup = queueRunner.Dequeue();

                        nodeID = rtup.Item1;
                        progressID = rtup.Item2;
                    }
                }
            }
        }

        internal void NextDeepNode(out string nodeID, out string progressID)
        {
            nodeID = null;
            progressID = null;

            if (TestNode != null)
            {
                nodeID = TestNode;
                return;
            }

            NextPending(true, out nodeID, out progressID);

            if (nodeID == null)
            {
                lock (queueDeep)
                {
                    if (queueDeep.Count > 0)
                    {
                        Tuple<string, string> rtup = queueDeep.Dequeue();

                        nodeID = rtup.Item1;
                        progressID = rtup.Item2;
                    }
                }
            }
        }

        internal void QueueDeep(string nodeID, string progressID)
        {
            lock (queueDeep)
            {
                queueDeep.Enqueue(new Tuple<string, string>(nodeID, progressID));
            }
        }

        internal void RemoveProgress(string progressID)
        {
            jovice.Execute("delete from ProbeProgress where XP_ID = {0}", progressID);
        }

        private void NewProbe(ProbeTypes type, int tid)
        {
            string typed;
            if (type == ProbeTypes.Runner) typed = "RUNNER";
            else typed = "DEEP";

            Probe probe = new Probe(this, tid);
            probe.ProbeType = type;

            Event($"NEW PROBE: {typed}{tid}");

            probe.ConnectionFailed += delegate (object sender, Exception exception)
            {
                string message = exception.Message;
                string name = typed + tid;

                if (message.IndexOf("Auth fail") > -1) Event("Connection failed: Authentication failed", name);
                else if (message.IndexOf("unreachable") > -1) Event("Connection failed: Server unreachable", name);
                else if (message.IndexOf("respond after a period of time") > -1) Event("Connection failed: Connection time out", name);
                else
                {
                    Event("Connection failed", name);
#if DEBUG
                    Event("DEBUG " + message, name);
#endif
                }

                Event("Reconnecting in 5 seconds...", name);
                Thread.Sleep(5000);
                Event("Preparing to reconnect", name);
            };

            //probe.Start();
            lock (ProbeInstances)
            {
                ProbeInstances.Add(probe);
            }
        }

        private void DeleteProbe(ProbeTypes type, int count)
        {
            string typed;
            if (type == ProbeTypes.Runner) typed = "RUNNER";
            else typed = "DEEP";

            List<Probe> removedProbes = new List<Probe>();
            foreach (Probe probe in ProbeInstances)
            {
                if (probe.ProbeType == type)
                {
                    if (count > 0)
                    {
                        probe.QueueStop();
                        removedProbes.Add(probe);
                        count--;
                    }
                    else break;
                }
            }

            foreach (Probe probe in removedProbes)
            {
                ProbeInstances.Remove(probe);
                probeIDs.Remove(probe.Identifier);

                Event($"DELETE PROBE: {typed}{probe.Identifier}");
            }
        }

        internal void DeleteProbe(Probe probe)
        {
            string typed;
            if (probe.ProbeType == ProbeTypes.Runner) typed = "RUNNER";
            else typed = "DEEP";

            lock (ProbeInstances)
            {
                ProbeInstances.Remove(probe);
                probeIDs.Remove(probe.Identifier);

                Event($"DELETE PROBE: {typed}{probe.Identifier}");
            }
        }

        #endregion

        #region Handler

        private void NecrowServiceMessageHandler(MessageEventArgs e)
        {
            /*ServerNecrowServiceMessage m = (ServerNecrowServiceMessage)e.Message;

            if (m.Type == NecrowServiceMessageType.HelloResponse)
            {
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
                        if (ins.Value.IsStarted && ins.Value.IsProbing)
                        {
                            //objects.Add(new Tuple<string, string, string, DateTime, string>("A", ins.Key.Trim(), ins.Value.NodeName, ins.Value.NodeProbeStartTime, ins.Value.Properties.TacacUser));
                        }
                        //else if (InTime(ins.Value.Properties))
                        //{
                            //objects.Add(new Tuple<string, string, string, DateTime, string>("I", ins.Key.Trim(), ins.Value.LastNodeName, ins.Value.LastProbeEndTime, null));
                        //}
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
            }*/
        }

        protected override void OnStart()
        {
            Event("Necrow is Starting...");

            jovice = Jovice.Database;

            jovice.Retry += delegate (object sender, DatabaseExceptionEventArgs2 eventArgs)
            {
                if (eventArgs.Type == DatabaseExceptionType2.Timeout)
                    Event("Database query has timed out, retrying");
                else
                {
                    Event("Jovice Database Connection failed: " + eventArgs.Message);
                    eventArgs.NoRetry = true;
                    //Stop();
                }
            };
            jovice.QueryAttempts = 5;
            jovice.Timeout = 600;

            oss = OSS.Database;

            Batch batch;
            Result2 result;
            
            // Handlers
            //Service.Register(typeof(ServerNecrowServiceMessage), NecrowServiceMessageHandler);

            Event("Checking Jovice Database connection... ");

            if (jovice)
            {
                batch = jovice.Batch();

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

                NecrowVirtualization.Load(this);

                Event("Database virtualizations completed");

                #endregion

                #region Etc

                InterfaceTestPrefixes = new Dictionary<string, string[]>();
                InterfaceTestPrefixes.Add("Hu", new string[] { "H", "HU", "GI", "GE" });
                InterfaceTestPrefixes.Add("Te", new string[] { "T", "TE", "TENGIGE", "GI", "GE", "XE" }); // kadang Te-gig direfer sebagai Gi dammit people
                InterfaceTestPrefixes.Add("Gi", new string[] { "G", "GI", "GE", "GIGAE", "GIGABITETHERNET", "TE" }); // kadang Te-gig direfer sebagai Gi dammit people
                InterfaceTestPrefixes.Add("Fa", new string[] { "F", "FA", "FE", "FASTE" });
                InterfaceTestPrefixes.Add("Et", new string[] { "E", "ET", "ETH" });
                InterfaceTestPrefixes.Add("Ag", new string[] { "LAG", "ETH-TRUNK", "BE" });

                #endregion

                #region Queue initializations

                // PROBE LIST
                Event("Loading queue...");

                List<string> progressRemove = new List<string>();

                foreach (Row2 xp in jovice.Query("select * from ProbeProgress order by XP_Queue asc"))
                {
                    string progressID = xp["XP_ID"].ToString();
                    string noid = xp["XP_NO"].ToString();
                    bool deep = xp["XP_Deep"].ToBool();
                    DateTime? pending = xp["XP_Pending"].ToNullableDateTime();

                    bool removed = false;

                    foreach (Tuple<string, string> otu in queueRunner)
                    {
                        if (otu.Item1 == noid)
                        {
                            removed = true;
                            break;
                        }
                    }

                    if (!removed)
                    {
                        foreach (Tuple<string, string> otu in queueDeep)
                        {
                            if (otu.Item1 == noid)
                            {
                                removed = true;
                                break;
                            }
                        }
                    }


                    if (removed)
                    {
                        progressRemove.Add(progressID);
                        continue;
                    }
                    else
                    {
                        if (pending.HasValue)
                        {
                            pendingList.Add(progressID, new Tuple<bool, DateTime>(deep, pending.Value));
                        }
                        else
                        {
                            Tuple<string, string> queueEntry = new Tuple<string, string>(noid, progressID);

                            if (!deep)
                                queueRunner.Enqueue(queueEntry);
                            else
                                queueDeep.Enqueue(queueEntry);
                        }
                    }
                }

                if (progressRemove.Count > 0)
                {
                    batch.Begin();
                    foreach (string xp in progressRemove)
                    {
                        batch.Add("delete from ProbeProgress where XP_ID = {0}", xp);
                    }
                    batch.Commit();
                }

                jovice.Execute("update ProbeProgress set XP_StartTime = NULL");

                // SUPPORTED VERSION
                if (supportedVersions == null)
                {
                    Result2 sver = jovice.Query("select * from NodeSupport");

                    supportedVersions = new List<Tuple<string, string, string>>();

                    foreach (Row2 sve in sver)
                    {
                        supportedVersions.Add(new Tuple<string, string, string>(sve["NT_Manufacture"].ToString(), sve["NT_Version"].ToString(), sve["NT_SubVersion"].ToString()));
                    }
                }

                #endregion

                #region Database Keepers

                #region Node Keeper

                result = jovice.Query("select * from Node");
                KeeperNode = new Dictionary<string, Dictionary<string, object>>();

                foreach (Row2 row in result)
                {
                    Dictionary<string, object> values = new Dictionary<string, object>();
                    KeeperNode.Add(row["NO_ID"].ToString(), values);

                    values.Add("NO_Name", row["NO_Name"].ToString());
                    values.Add("NO_Type", row["NO_Type"].ToString());
                    values.Add("NO_Manufacture", row["NO_Manufacture"].ToString());
                    values.Add("NO_IP", row["NO_IP"].ToString());
                    values.Add("NO_Active", row["NO_Active"].ToBool());
                }

                #endregion

                #endregion

                ProbeInstances = new List<Probe>();
                ProbeUserAccess = new Dictionary<string, ProbeUserAccess>();

                long loops = 0;

                if (TestNode != null)
                {
                    result = jovice.Query("select * from Node where NO_Name = {0}", TestNode);

                    if (result.Count == 1)
                    {
                        Row2 r = result[0];
                        Event("TEST MODE ENABLED: " + r["NO_Name"].ToString());
                        TestNode = r["NO_ID"].ToString();
                    }
                    else
                    {
                        Event($"{TestNode} is not found in the Database");
                        TestNode = null;
                    }
                        
                }

                Event("Starting Service Discovery...");

                BeginServiceDiscovery();

                while (IsRunning)
                {
                    #region Check Regularly
                    if (loops % 10 == 0)
                    {
                        lock (KeeperNode)
                        {
                            result = jovice.Query("select * from Node");

                            batch.Begin();
                            foreach (Row2 row in result)
                            {
                                string id = row["NO_ID"].ToString();

                                if (KeeperNode.ContainsKey(id))
                                {
                                    Dictionary<string, object> keeper = KeeperNode[id];

                                    Update update = jovice.Update("Node");
                                    update.Where("NO_ID", id);

                                    if ((string)keeper["NO_Name"] != row["NO_Name"].ToString()) update.Set("NO_Name", (string)keeper["NO_Name"]);
                                    if ((string)keeper["NO_Type"] != row["NO_Type"].ToString()) update.Set("NO_Type", (string)keeper["NO_Type"]);
                                    if ((string)keeper["NO_Manufacture"] != row["NO_Manufacture"].ToString()) update.Set("NO_Manufacture", (string)keeper["NO_Manufacture"]);
                                    if ((string)keeper["NO_IP"] != row["NO_IP"].ToString()) update.Set("NO_IP", (string)keeper["NO_IP"]);
                                    if ((bool)keeper["NO_Active"] != row["NO_Active"].ToBool()) update.Set("NO_Active", (bool)keeper["NO_Active"]);

                                    batch.Add(update);
                                }
                                else
                                {
                                    Dictionary<string, object> values = new Dictionary<string, object>();
                                    KeeperNode.Add(row["NO_ID"].ToString(), values);

                                    string newNodeName = row["NO_Name"].ToString();
                                    values.Add("NO_Name", newNodeName);
                                    values.Add("NO_Type", row["NO_Type"].ToString());
                                    values.Add("NO_Manufacture", row["NO_Manufacture"].ToString());
                                    values.Add("NO_IP", row["NO_IP"].ToString());
                                    values.Add("NO_Active", row["NO_Active"].ToBool());

                                    prioritize.Enqueue(new Tuple<string, ProbeRequestData>(newNodeName, null));

                                    Event("New Node Registered: " + newNodeName);
                                }
                            }
                            batch.Commit();
                        }

                        result = jovice.Query("select * from ProbeConfiguration");

                        foreach (Row2 row in result)
                        {
                            string key = row["XC_Key"].ToString();
                            string value = row["XC_Value"].ToString();

                            if (key == "RUNNER_PROBE_COUNT" && int.TryParse(value, out int vrunner)) MaxRunnerProbeCount = vrunner;
                            if (key == "DEEP_PROBE_COUNT" && int.TryParse(value, out int vdeep)) MaxDeepProbeCount = vdeep;
                        }

                        result = jovice.Query("select * from ProbeUser");

                        List<string> userAccessIDs = new List<string>();

                        foreach (Row2 row in result)
                        {
                            string id = row["XU_ID"].ToString();

                            userAccessIDs.Add(id);

                            string serverAddress = row["XU_ServerAddress"].ToString();
                            string serverUser = row["XU_ServerUser"].ToString();
                            string serverPassword = row["XU_ServerPassword"].ToString();
                            string tacacUser = row["XU_TacacUser"].ToString();
                            string tacacPassword = row["XU_TacacPassword"].ToString();

                            if (ProbeUserAccess.ContainsKey(id))
                            {
                                // update
                                bool update = false;

                                ProbeUserAccess ua = ProbeUserAccess[id];

                                if (ua.ServerAddress != serverAddress)
                                {
                                    update = true;
                                    ua.ServerAddress = serverAddress;
                                }
                                if (ua.ServerUser != serverUser)
                                {
                                    update = true;
                                    ua.ServerUser = serverUser;
                                }
                                if (ua.ServerPassword != serverPassword)
                                {
                                    update = true;
                                    ua.ServerPassword = serverPassword;
                                }
                                if (ua.TacacUser != tacacUser)
                                {
                                    update = true;
                                    ua.TacacUser = tacacUser;
                                }
                                if (ua.TacacPassword != tacacPassword)
                                {
                                    update = true;
                                    ua.TacacPassword = tacacPassword;
                                }

                                if (update)
                                {
                                    lock (ProbeInstances)
                                    {
                                        foreach (Probe probe in ProbeInstances)
                                        {
                                            if (probe.ProbeUserAccessID == id)
                                            {
                                                probe.QueueStop();
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // add
                                ProbeUserAccess.Add(id, new ProbeUserAccess() { ID = id, ServerAddress = serverAddress, ServerUser = serverUser, ServerPassword = serverPassword, TacacUser = tacacUser, TacacPassword = tacacPassword });
                            }
                        }

                        foreach (KeyValuePair<string, ProbeUserAccess> pair in ProbeUserAccess)
                        {
                            if (!userAccessIDs.Contains(pair.Key))
                            {
                                // delete
                                string did = pair.Key;

                                ProbeUserAccess.Remove(did);

                                lock (ProbeInstances)
                                {
                                    List<Probe> probeRemoves = new List<Probe>();

                                    foreach (Probe probe in ProbeInstances)
                                    {
                                        if (probe.ProbeUserAccessID == did)
                                        {
                                            probe.QueueStop();
                                            probeRemoves.Add(probe);
                                        }
                                    }
                                    foreach (Probe probeRemove in probeRemoves)
                                        ProbeInstances.Remove(probeRemove);
                                }
                            }
                        }

                        int cr = RunnerProbeCount;
                        int cd = DeepProbeCount;

                        if (TestNode != null)
                        {
                            if (TestProbeType == ProbeTypes.Runner)
                            {
                                if (cr == 0)
                                {
                                    NewProbe(ProbeTypes.Runner, 1);
                                }
                            }
                            else
                            {
                                if (cd == 0)
                                {
                                    NewProbe(ProbeTypes.Deep, 1);
                                }
                            }
                        }
                        else
                        {
                            Event($"RUNNER NODES: {queueRunner.Count}");
                            Event($"DEEP NODES: {queueDeep.Count}");
                            Event($"PENDING NODES: {pendingList.Count}");
                            Event($"RUNNER PROBES: {cr}/{MaxRunnerProbeCount}");
                            Event($"DEEP PROBES: {cd}/{MaxDeepProbeCount}");

                            int newRunner = MaxRunnerProbeCount - cr;

                            if (newRunner < 0)
                            {
                                newRunner = Math.Abs(newRunner);
                                Event($"Reducing {newRunner} runner probe{newRunner.PluralCase()}");
                                DeleteProbe(ProbeTypes.Runner, newRunner);
                            }
                            else if (newRunner > 0)
                            {
                                Event($"Creating {newRunner} runner probe{newRunner.PluralCase()}");
                                for (int or = 0; or < newRunner; or++)
                                {
                                    int tid = 1;
                                    while (probeIDs.Contains(tid)) tid++;
                                    probeIDs.Add(tid);

                                    NewProbe(ProbeTypes.Runner, tid);
                                }
                            }

                            int createNewDeep = 0;

                            if (queueDeep.Count > cd && cd < MaxDeepProbeCount)
                            {
                                createNewDeep = queueDeep.Count < MaxDeepProbeCount ? queueDeep.Count : MaxDeepProbeCount;
                            }

                            if (createNewDeep > 0)
                            {
                                Event($"Creating {createNewDeep} deep probe{createNewDeep.PluralCase()}");

                                for (int or = 0; or < createNewDeep; or++)
                                {
                                    int tid = 1;
                                    while (probeIDs.Contains(tid)) tid++;
                                    probeIDs.Add(tid);

                                    NewProbe(ProbeTypes.Deep, tid);
                                }
                            }
                            else if (cd > MaxDeepProbeCount)
                            {
                                int deleteDeep = cd - MaxDeepProbeCount;

                                Event($"Reducing {deleteDeep} deep probe{deleteDeep.PluralCase()}");

                                DeleteProbe(ProbeTypes.Deep, newRunner);
                            }
                        }
                    }
                    #endregion

                    /* 
                     * STARTED = probe is started
                     * CONNECTED = probe is connected to SSH
                     * PROBING = probe is probing to Node
                     */

                    lock (ProbeInstances)
                    {
                        foreach (Probe probe in ProbeInstances)
                        {
                            if (probe.IsStarted)
                            {
                                if (probe.IsRunning)
                                {
                                    if (probe.IsProbing)
                                    {
                                    }
                                    else
                                    {
                                        if ((DateTime.UtcNow - probe.SSHProbeStartTime).TotalHours > 3)
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
                                probe.Start();
                            }
                        }
                    }

                    Thread.Sleep(1000);
                    loops++;
                }

                Event("Necrow terminated");
                foreach (Probe probe in ProbeInstances) probe.Stop(); // kill all instance in process
                jovice.Execute("update ProbeProgress set XP_StartTime = NULL");

                EndServiceDiscovery();
            }
            else
            {
                string failType;

                if (jovice.LastException == null)
                {
                    failType = "lastException is null";
                }
                else
                {
                    switch (jovice.LastException.Type)
                    {
                        case DatabaseExceptionType2.LoginFailed:
                            failType = "Login Failed";
                            break;
                        case DatabaseExceptionType2.Timeout:
                            failType = "Connection Timeout";
                            break;
                        default:
                            failType = $"Unspecified ({jovice.LastException.Message})";
                            break;
                    }
                }
                Event($"Connection failed: {failType}");
            }
        }

        protected override void OnStop()
        {
        }

        //protected override void OnServiceConnected()
        //{
        // Introduce ourselves
        //    Service.Send(new ServerNecrowServiceMessage(NecrowServiceMessageType.Hello));
        //}

        #endregion
    }
}
