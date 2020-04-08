using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using Aphysoft.Share;

namespace Necrow
{
    #region To Database
    
    internal abstract class StatusToDatabase : Data2
    {
        public bool Status { get; set; }

        public bool Protocol { get; set; }

        public bool Enable { get; set; }

        public bool UpdateStatus { get; set; } = false;

        public bool UpdateProtocol { get; set; } = false;

        public bool UpdateEnable { get; set; } = false;
    }

    internal abstract class ServiceBaseToDatabase : StatusToDatabase
    {
        public string ServiceImmediateID { get; set; }

        public bool UpdateServiceImmediateID { get; set; } = false;

        public string ServiceVID { get; set; }

        public string Description { get; set; }

        public bool UpdateDescription { get; set; } = false;
    }

    internal abstract class InterfaceToDatabase : ServiceBaseToDatabase
    {
        public string InterfaceID { get; set; } = null;

        public bool UpdateInterfaceID { get; set; } = false;

        public string Name { get; set; }

        public bool UpdateName { get; set; } = false;

        public string EquipmentName { get; set; }

        public bool UpdateEquipmentName { get; set; }

        public string InterfaceType { get; set; } = null;

        public bool UpdateInterfaceType { get; set; } = false;

        public int Dot1Q { get; set; } = -1;

        public bool UpdateDot1Q { get; set; } = false;

        public DateTime? LastDown { get; set; } = null;

        public bool UpdateLastDown { get; set; } = false;

        public int RateInput { get; set; } = -1;

        public int RateOutput { get; set; } = -1;

        public bool UpdateRateInput { get; set; } = false;

        public bool UpdateRateOutput { get; set; } = false;

        public char Mode { get; set; } = '-';

        public bool UpdateMode { get; set; } = false;

        public char Encapsulation { get; set; } = '-';

        public bool UpdateEncapsulation { get; set; } = false;

        public int AdmMTU { get; set; } = -1;

        public bool UpdateAdmMTU { get; set; } = false;

        public int RunMTU { get; set; } = -1;

        public bool UpdateRunMTU { get; set; } = false;

        public int Aggr { get; set; } = -1;

        public bool UpdateAggr { get; set; } = false;

        public string ParentID { get; set; } = null;

        public bool UpdateParentID { get; set; } = false;

        public string TopologyMEInterfaceID { get; set; } = null;

        public bool UpdateTopologyMEInterfaceID { get; set; } = false;

        public string TopologyNBInterfaceID { get; set; } = null;

        public bool UpdateTopologyNBInterfaceID { get; set; } = false;

        public bool PhysicalNeighborChecked { get; set; } = false;

        public string AggrNeighborParentID { get; set; } = null;

        public Dictionary<int, Tuple<string, string, string>> ChildrenNeighbor { get; set; } = null;

        public long CirTotalInput { get; set; } = -1;

        public bool UpdateCirTotalInput { get; set; } = false;

        public long CirTotalOutput { get; set; } = -1;

        public bool UpdateCirTotalOutput { get; set; } = false;

        public int CirConfigTotalInput { get; set; } = -1;

        public bool UpdateCirConfigTotalInput { get; set; } = false;

        public int CirConfigTotalOutput { get; set; } = -1;

        public bool UpdateCirConfigTotalOutput { get; set; } = false;

        public int SubInterfaceCount { get; set; } = -1;

        public bool UpdateSubInterfaceCount { get; set; } = false;

        public float TrafficInput { get; set; } = -1;

        public bool UpdateTrafficInput { get; set; } = false;

        public float TrafficOutput { get; set; } = -1;

        public bool UpdateTrafficOutput { get; set; } = false;
    }

    internal abstract class MacToDatabase : Data2
    {
        public string MacAddress { get; set; }

        public bool UpdateMacAddress { get; set; } = false;

        public string InterfaceID { get; set; }

        public int Age { get; set; }

        public bool UpdateAge { get; set; } = false;
    }

    internal class CustomerToDatabase : Data2
    {
        public string Name { get; set; }

        public string CID { get; set; }
    }

    internal class ServiceToDatabase : Data2
    {
        public string VID { get; set; }

        public string Type { get; set; }
    }

    [DataAttribute2("NodeSlot", "NC_ID", "NC")]
    internal class NodeSlotData : Data2
    {
        [FieldAttribute2("{0}_Slot1", true)]
        public int? Slot1 { get; set; }

        [FieldAttribute2("{0}_Slot2", true)]
        public int? Slot2 { get; set; }

        [FieldAttribute2("{0}_Slot3", true)]
        public int? Slot3 { get; set; }

        [FieldAttribute2("{0}_Type", true)]
        public string Type { get; set; }

        [FieldAttribute2("{0}_Board", true)]
        public string Board { get; set; }

        [FieldAttribute2("{0}_Info", true)]
        public string Info { get; set; }

        [FieldAttribute2("{0}_Serial", true)]
        public string Serial { get; set; }

        [FieldAttribute2("{0}_LastStartUp", true)]
        public DateTime? LastStartUp { get; set; }

        [FieldAttribute2("{0}_Enable", true)]
        public bool? Enable { get; set; }

        [FieldAttribute2("{0}_Protocol", true)]
        public bool? Protocol { get; set; }

        [FieldAttribute2("{0}_Capacity", true)]
        public int? Capacity { get; set; }
    }

    #endregion

    internal static class ProbeExtensions
    {
        public static string NullableInfo(this object value, string format)
        {
            string strtype = value.ToString();
            return (strtype == "-1" || strtype == "-") ? null : (format != null && strtype != null) ? string.Format(format, strtype) : strtype;
        }

        public static string NullableInfo(this object value)
        {
            return value.NullableInfo(null);
        }
    }

    internal enum FailureTypes
    {
        None,
        Connection,
        Database,
        Request,
        ProbeStopped,
        ALURequest,
        ManufactureCorrected,
    }

    internal class ProbeProcessResult
    {
        #region Fields

        private FailureTypes failure;

        public FailureTypes FailureType
        {
            get { return failure; }
            set { failure = value; }
        }

        #endregion

        #region Constructors

        public ProbeProcessResult()
        {

        }

        #endregion
    }

    public enum ProbeTypes
    {
        Runner,
        Deep
    }

    public sealed partial class Probe : SshConnection
    {
        #region Enums

        private enum UpdateTypes
        {
            NecrowVersion,
            TimeStamp,
            TimeOffset,
            Remark,
            IP,
            Name,
            Terminal,
            ConnectType,
            Model,
            Version,
            SubVersion,
            LastConfiguration,
            StartUpTime,
        }

        private enum EventActions { Add, Remove, Delete, Update }
        private enum EventElements
        {
            ALUCustomer, QOS, SDP, Circuit, Interface, Peer, CircuitReference,
            VRFReference, VRF, VRFRouteTarget, InterfaceIP, Service, Customer, NodeReference, InterfaceReference,
            NodeAlias, NodeSummary, POPInterfaceReference, Routing, NBInterface,
            PrefixList, PrefixEntry, Mac,
            Slot
        }

        #endregion

        #region Fields

        private Necrow necrow = null;

        public string ProbeUserAccessID { get; private set; } = null;

        private Thread idleThread = null;

        private Dictionary<string, object> updates;
        private Dictionary<string, (string, bool)> summaries;

        public int Identifier { get; private set; } = 0;

        private string outputIdentifier = null;

        private Database2 j;

        private string nodeID;
        private List<string> nodeRules;
        private string nodeManufacture;
        private string nodeModel;
        private string nodeVersion;
        private string nodeSubVersion;
        private string nodeIP;
        private string nodeTerminal;
        private string nodeConnectType;
        private string nodeAreaID;
        private string nodeType;
        private int nodeNVER;
        private TimeSpan nodeTimeOffset;
        private DateTime nodeStartUp = DateTime.MinValue;

        private string probeProgressID = null;
        private bool updatingNecrow = false;

        public string NodeName { get; private set; }

        public string LastNodeName { get; private set; } = null;

        public DateTime NodeProbeStartTime { get; private set; } = DateTime.MinValue;

        public DateTime LastProbeEndTime { get; private set; } = DateTime.MinValue;

        private readonly string alu = "ALCATEL-LUCENT";
        private readonly string hwe = "HUAWEI";
        private readonly string cso = "CISCO";
        private readonly string jun = "JUNIPER";
        private readonly string xr = "XR";

        private readonly string fho = "FIBERHOME";

        private Dictionary<string, Row2> reserves;
        private Dictionary<string, Row2> popInterfaces;

        public bool IsProbing { get; private set; } = false;

        public DateTime SSHProbeStartTime { get; private set; } = DateTime.MinValue;

        private bool queueStop = false;

        public bool SessionStart { get; set; } = false;

        public ProbeTypes ProbeType { get; set; } = ProbeTypes.Runner;

        private string serverAddress = null;
        private string serverUser = null;
        private string serverPassword = null;
        private string tacacUser = null;
        private string tacacPassword = null;

        internal bool ProgressPending { get; set; } = false;
        internal bool ProgressQueueDeep { get; set; } = false;

        #endregion

        #region Constructors

        public Probe(Necrow necrow, int identifier)
        {
            this.necrow = necrow;

            j = Database2.Get("JOVICE");

            Identifier = identifier;
        }

        #endregion

        #region Event

        private void Event(string message)
        {
            string name = (ProbeType == ProbeTypes.Runner ? "RUNNER" : "DEEP") + Identifier;

            if (outputIdentifier == null)
                necrow.Event(message, name);
            else
                necrow.Event(message, name, outputIdentifier);
        }

        private void Event(Result2 result, EventActions action, EventElements element, bool reportzero)
        {
            int row = result.AffectedRows;
            if (row > 0 || (row >= 0 && reportzero))
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(row);
                sb.Append(' ');
                if (row == 1)
                {
                    switch (element)
                    {
                        case EventElements.ALUCustomer: sb.Append("alu-customer"); break;
                        case EventElements.Circuit: sb.Append("circuit"); break;
                        case EventElements.Interface: sb.Append("interface"); break;
                        case EventElements.QOS: sb.Append("QOS"); break;
                        case EventElements.SDP: sb.Append("SDP"); break;
                        case EventElements.Peer: sb.Append("peer"); break;
                        case EventElements.CircuitReference: sb.Append("circuit reference"); break;
                        case EventElements.InterfaceIP: sb.Append("interface IP"); break;
                        case EventElements.VRF: sb.Append("VRF"); break;
                        case EventElements.VRFReference: sb.Append("VRF reference"); break;
                        case EventElements.VRFRouteTarget: sb.Append("VRF route target"); break;
                        case EventElements.Customer: sb.Append("customer"); break;
                        case EventElements.Service: sb.Append("service"); break;
                        case EventElements.NodeReference: sb.Append("node reference"); break;
                        case EventElements.InterfaceReference: sb.Append("interface reference"); break;
                        case EventElements.NodeAlias: sb.Append("node alias"); break;
                        case EventElements.NodeSummary: sb.Append("node summary"); break;
                        case EventElements.POPInterfaceReference: sb.Append("POP interface reference"); break;
                        case EventElements.Routing: sb.Append("routing"); break;
                        case EventElements.NBInterface: sb.Append("neighbor interface"); break;
                        case EventElements.PrefixList: sb.Append("prefix-list"); break;
                        case EventElements.PrefixEntry: sb.Append("prefix-list entry"); break;
                        case EventElements.Mac: sb.Append("mac-address"); break;
                        case EventElements.Slot: sb.Append("chassis slot"); break;
                    }
                }
                else
                {
                    switch (element)
                    {
                        case EventElements.ALUCustomer: sb.Append("alu-customers"); break;
                        case EventElements.Circuit: sb.Append("circuits"); break;
                        case EventElements.Interface: sb.Append("interfaces"); break;
                        case EventElements.QOS: sb.Append("QOSes"); break;
                        case EventElements.SDP: sb.Append("SDPs"); break;
                        case EventElements.Peer: sb.Append("peers"); break;
                        case EventElements.CircuitReference: sb.Append("circuit references"); break;
                        case EventElements.InterfaceIP: sb.Append("interface IPs"); break;
                        case EventElements.VRF: sb.Append("VRFs"); break;
                        case EventElements.VRFReference: sb.Append("VRF references"); break;
                        case EventElements.VRFRouteTarget: sb.Append("VRF route targets"); break;
                        case EventElements.Customer: sb.Append("customers"); break;
                        case EventElements.Service: sb.Append("services"); break;
                        case EventElements.NodeReference: sb.Append("node references"); break;
                        case EventElements.InterfaceReference: sb.Append("interface references"); break;
                        case EventElements.NodeAlias: sb.Append("node aliases"); break;
                        case EventElements.NodeSummary: sb.Append("node summaries"); break;
                        case EventElements.POPInterfaceReference: sb.Append("POP interface references"); break;
                        case EventElements.Routing: sb.Append("routings"); break;
                        case EventElements.NBInterface: sb.Append("neighbor interfaces"); break;
                        case EventElements.PrefixList: sb.Append("prefix-lists"); break;
                        case EventElements.PrefixEntry: sb.Append("prefix-list entries"); break;
                        case EventElements.Mac: sb.Append("mac-addresses"); break;
                        case EventElements.Slot: sb.Append("chassis slots"); break;
                    }
                }
                if (row > 1) sb.Append(" have been ");
                else sb.Append(" has been ");
                if (action == EventActions.Add) sb.Append("added (");
                else if (action == EventActions.Delete) sb.Append("deleted (");
                else if (action == EventActions.Remove) sb.Append("removed (");
                else if (action == EventActions.Update) sb.Append("updated (");
                else sb.Append("affected (");
                sb.Append(string.Format("{0:0.###}", result.ExecutionTime.TotalSeconds));
                sb.Append("s)");

                Event(sb.ToString());
            }
        }

        #endregion

        #region Start Stop

        public void Start()
        {
            if (IsStarted) return;

            Dictionary<string, int> accessCount = new Dictionary<string, int>();

            lock (necrow.ProbeInstances)
            {
                foreach (Probe oprobe in necrow.ProbeInstances)
                {
                    if (oprobe.ProbeUserAccessID != null)
                    {
                        if (!accessCount.ContainsKey(oprobe.ProbeUserAccessID)) accessCount.Add(oprobe.ProbeUserAccessID, 1);
                        else accessCount[oprobe.ProbeUserAccessID] = accessCount[oprobe.ProbeUserAccessID] + 1;
                    }
                }
            }
            foreach (KeyValuePair<string, ProbeUserAccess> pair in necrow.ProbeUserAccess)
            {
                if (!accessCount.ContainsKey(pair.Key))
                {
                    accessCount.Add(pair.Key, 0);
                }
            }

            // pilih userAccess yang paling sedikit
            int min = int.MaxValue;
            string saccessid = null;

            foreach (KeyValuePair<string, int> pair in accessCount)
            {
                if (pair.Value < min)
                {
                    min = pair.Value;
                    saccessid = pair.Key;
                }
            }

            // saccessid is the selected
            ProbeUserAccessID = saccessid;

            serverAddress = necrow.ProbeUserAccess[ProbeUserAccessID].ServerAddress;
            serverUser = necrow.ProbeUserAccess[ProbeUserAccessID].ServerUser;
            serverPassword = necrow.ProbeUserAccess[ProbeUserAccessID].ServerPassword;
            tacacUser = necrow.ProbeUserAccess[ProbeUserAccessID].TacacUser;
            tacacPassword = necrow.ProbeUserAccess[ProbeUserAccessID].TacacPassword;

            Event("Starting... (" + serverUser + "@" + serverAddress + " [" + tacacUser + "])");
            Start(serverAddress, serverUser, serverPassword);
        }

        public void QueueStop()
        {
            queueStop = true;
        }

        #endregion

        #region Handlers

        protected override void OnStarting()
        {
            Event("Connecting...");
            Thread.Sleep(Rnd.Int(0, 500));
        }

        protected override void OnConnected()
        {
            Event("Connected!");
            SSHProbeStartTime = DateTime.UtcNow;
        }

        protected override void OnProcess()
        {
            Event("Terminal prefix: " + terminalPrefix);

            string restartCurrentNodeID = null;
            int restartCount = 0;

            while (true)
            {
                string progressID = null;

                ProgressPending = false;
                ProgressQueueDeep = false;

                bool fromDeepNodeQueue = false;

                if (restartCurrentNodeID != null)
                {
                    nodeID = restartCurrentNodeID;
                    restartCount++;
                    restartCurrentNodeID = null;
                }
                else
                {
                    restartCount = 0;
                    if (ProbeType == ProbeTypes.Runner)
                        necrow.NextRunnerNode(out nodeID, out progressID);
                    else
                    {
                        necrow.NextDeepNode(out nodeID, out progressID);
                        fromDeepNodeQueue = true;
                    }
                }

                if (nodeID == null)
                {
                    necrow.DeleteProbe(this);
                    break;
                }
                else
                {
                    bool caughtError = false;
                    bool continueProcess = false;

                    // lets do this
                    Result2 resultNode = j.Query("select * from Node where NO_ID = {0}", nodeID);

                    if (resultNode == 1)
                    {
                        Row2 node = resultNode[0];

                        if (node == null)
                        {
                            Event("Cannot found NO_ID");
                            necrow.RemoveProgress(progressID);
                        }
                        else
                        {
                            ProbeProcessResult probe = null;
#if !DEBUG
                            string info = null;
                            try
                            {
#endif
                                probe = Enter(node, progressID, fromDeepNodeQueue, out continueProcess);

                                if (probe != null)
                                {
                                    if (probe.FailureType == FailureTypes.ALURequest)
                                    {
                                        // restart?
                                        Event("Probe error: ALU request has failed");

                                        if (restartCount < 2) restartCurrentNodeID = nodeID;
                                        else
                                        {
                                            Update(UpdateTypes.Remark, "PROBEFAILED");
                                            caughtError = true;

                                            necrow.PendingNode(this, progressID, TimeSpan.FromHours(4));
                                        }

                                        continueProcess = false;
                                    }
                                    else if (probe.FailureType == FailureTypes.ManufactureCorrected)
                                    {
                                        restartCurrentNodeID = nodeID;
                                        continueProcess = false;
                                    }
                                    else if (probe.FailureType != FailureTypes.None)
                                    {
                                        string errorMessage = null;
                                        if (probe.FailureType == FailureTypes.Connection) errorMessage = "Connection error";
                                        else if (probe.FailureType == FailureTypes.Database) errorMessage = "Database error: " + j.LastException.Message.Trim(new char[] { '\r', '\n', ' ' }) + ", SQL: " + j.LastException.Sql;
                                        else if (probe.FailureType == FailureTypes.Request) errorMessage = "Request error";
#if !DEBUG
                                        throw new Exception(errorMessage);
#else
                                    Event("Probe error: " + errorMessage);
                                    Update(UpdateTypes.Remark, "PROBEFAILED");
                                    caughtError = true;
                                    continueProcess = false;
#endif
                                    }
                                }

#if !DEBUG
                            }
                            catch (Exception ex)
                            {
                                if (ex.Message.IndexOf("was being aborted") == -1)
                                {
                                    info = ex.Message;
                                    necrow.Log(NodeName, ex.Message, ex.StackTrace);
                                    Update(UpdateTypes.Remark, "PROBEFAILED");
                                }

                                continueProcess = false;

                                Save();

                                if (IsProbing)
                                {
                                    if (info != null)
                                        Event("Caught error: " + info + ", exiting current node...");
                                    Exit();
                                }
                                else if (info != null)
                                    Event("Caught error: " + info);
                            }
#endif

                            if (continueProcess)
                            {
                                #region Continue Process                            

                                if (ProbeType == ProbeTypes.Runner)
                                {
                                    lock (necrow.ProbeLock)
                                    {
                                        if (necrow.DeepProbeCount >= necrow.MaxDeepProbeCount)
                                        {
                                            // masukkan queue
                                            Event("Queuing to deep probe");
                                            necrow.QueueDeep(nodeID, progressID);
                                            ProgressQueueDeep = true;
                                        }
                                        else
                                        {
                                            // I, myself, become DEEP
                                            Event($"CHANGE PROBE: RUNNER{Identifier} to DEEP{Identifier}");
                                            ProbeType = ProbeTypes.Deep;
                                        }
                                    }
                                }

                                if (!ProgressQueueDeep)
                                {
                                    j.Execute("update ProbeProgress set XP_Deep = 1 where XP_ID = {0}", progressID);

                                    idleThread = new Thread(new ThreadStart(delegate ()
                                    {
                                        while (true)
                                        {
                                            Thread.Sleep(30000);
                                            if (nodeManufacture == alu || nodeManufacture == cso || nodeManufacture == hwe) SendCharacter((char)27);

                                        }
                                    }));
                                    idleThread.Start();

                                    Batch batch = j.Batch();
                                    Result2 result;
                                    batch.Begin();

                                    //// RESERVES
                                    //reserves = j.QueryDictionary("select * from Reserve where RE_NO = {0}", delegate (Row row)
                                    //{
                                    //    return row["RE_By_Name"].ToString() + "=" + row["RE_By_SID"].ToString();
                                    //}, delegate (Row row)
                                    //{
                                    //// delete duplicated
                                    //batch.Add("delete from Reserve where RE_ID = {0}", row["RE_ID"].ToString());
                                    //}, nodeID);

                                    // POP
                                    if (nodeType == "P")
                                    {
                                        popInterfaces = new Dictionary<string, Row2>();
                                        result = j.Query("select * from POP where UPPER(OO_NO_Name) = {0}", NodeName);
                                        foreach (Row2 row in result)
                                        {
                                            string storedID = row["OO_ID"].ToString();
                                            string interfaceName = row["OO_PI_Name"].ToString();
                                            string type = row["OO_Type"].ToString();

                                            string key = interfaceName + "_" + type;

                                            bool ooPINULL = row["OO_PI"].IsNull;

                                            if (!popInterfaces.ContainsKey(key))
                                            {
                                                popInterfaces.Add(key, row);

                                                if (row["OO_NO_Name"].ToString() != NodeName)
                                                {
                                                    // fix incorrect name in POP
                                                    batch.Add("update POP set OO_NO_Name = {0} where OO_ID = {1}", NodeName, storedID);
                                                }
                                            }
                                            else
                                            {
                                                // delete duplicated OO_PI_Name per OO_NO_Name
                                                batch.Add("delete from POP where OO_ID = {0}", storedID);
                                            }
                                        }
                                    }
                                    else popInterfaces = null;

                                    batch.Commit();
#if !DEBUG
                                    try
                                    {
#endif
                                        if (nodeType == "P" || nodeType == "T") probe = PEProcess();
                                        else if (nodeType == "M") probe = MEProcess();

                                        if (probe != null)
                                        {
                                            if (probe.FailureType == FailureTypes.ALURequest)
                                            {
                                                // restart?
                                                Event("Probe error: ALU request has failed");

                                                if (restartCount < 2) restartCurrentNodeID = nodeID;
                                                else
                                                {
                                                    Update(UpdateTypes.Remark, "PROBEFAILED");
                                                    caughtError = true;

                                                    necrow.PendingNode(this, progressID, TimeSpan.FromHours(4));
                                                }
                                            }
                                            else if (probe.FailureType == FailureTypes.ProbeStopped)
                                            {
                                                Event("Probe error: Probe has been stopped in the middle of request");
                                                caughtError = true;

                                                necrow.PendingNode(this, progressID, TimeSpan.FromMinutes(30));
                                            }
                                            else if (probe.FailureType != FailureTypes.None)
                                            {
                                                string errorMessage = null;
                                                if (probe.FailureType == FailureTypes.Connection) errorMessage = "Connection error";
                                                else if (probe.FailureType == FailureTypes.Database) errorMessage = "Database error: " + j.LastException.Message.Trim(new char[] { '\r', '\n', ' ' }) + ", SQL: " + j.LastException.Sql;
                                                else if (probe.FailureType == FailureTypes.Request) errorMessage = "Request error";

#if !DEBUG
                                                throw new Exception(errorMessage);
#else
                                            Event("Probe error: " + errorMessage);
                                            Update(UpdateTypes.Remark, "PROBEFAILED");
                                            caughtError = true;
#endif
                                            }
                                        }

#if !DEBUG
                                        necrow.AcknowledgeNodeVersion(nodeManufacture, nodeVersion, nodeSubVersion);
                                    }
                                    catch (Exception ex)
                                    {
                                        if (ex.Message.IndexOf("was being aborted") == -1)
                                        {
                                            info = ex.Message;
                                            necrow.Log(NodeName, ex.Message, ex.StackTrace);
                                            Event("Caught error: " + ex.Message + ", exiting current node...");
                                            Update(UpdateTypes.Remark, "PROBEFAILED");
                                            caughtError = true;
                                        }
                                    }
#endif
                                    if (idleThread != null)
                                    {
                                        idleThread.Abort();
                                        idleThread = null;
                                    }
                                }
                                else
                                {
                                    j.Execute("update ProbeProgress set XP_Deep = 1, XP_StartTime = NULL where XP_ID = {0}", progressID);
                                }


                                #endregion
                            }

                            if (probe != null && probe.FailureType == FailureTypes.Connection)
                            {
                                Event("Connection failure");

                                Save();
                                SessionFailure();

                                Thread.Sleep(1000);
                            }
                            else if (restartCurrentNodeID != null)
                            {
                                Event("Probe process to the node is to be restarted");

                                if (IsProbing)
                                    Exit();

                                Thread.Sleep(5000);
                            }
                            else
                            {
                                Event("Save and Exit");

                                if (IsProbing)
                                    SaveExit();

                                Thread.Sleep(1000);
                            }

                            //if (probeRequestData != null)
                            //{
                            //    if (!continueProcess) { }
                            //    else if (caughtError)
                            //    {
                            //        probeRequestData.Message.Data = new object[] { NodeName, "ERROR", info };
                            //        probeRequestData.Connection.Reply(probeRequestData.Message);
                            //    }
                            //    else
                            //    {
                            //        probeRequestData.Message.Data = new object[] { NodeName, "FINISH" };
                            //        probeRequestData.Connection.Reply(probeRequestData.Message);
                            //    }

                        }
                    }
                }
            }

            Stop();
        }

        protected override void OnBeforeTerminate()
        {
            outputIdentifier = null;
        }

        protected override void OnTerminated()
        {
            IsProbing = false;
            SSHProbeStartTime = DateTime.MinValue;
            Event("Disconnected");
        }

        protected override void OnDisconnected()
        {
            if (idleThread != null)
            {
                Event("Destroying idle thread");
                idleThread.Abort();
                idleThread = null;
            }
        }

        protected override void OnSessionFailure()
        {
            Event("Session failure has occured");
            outputIdentifier = null;
            Thread.Sleep(5000);
        }

        protected override void OnBeforeStop()
        {
            queueStop = false;
        }

        #endregion

        private ProbeProcessResult DatabaseFailure(ProbeProcessResult probe)
        {
            Event("Database failure has occured");
            Thread.Sleep(5000);

            probe.FailureType = FailureTypes.Database;

            return probe;
        }

        private static void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Rnd.Int(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        private ExpectResult Expect(params string[] args)
        {
            Queue<string> lastOutputs = new Queue<string>();
            string expectOutput = null;

            if (args.Length == 0) return new ExpectResult(-1, null, false);

            int wait = 0;
            int expectReturn = -1;

            bool timeout = false;

            while (true)
            {
                string output = GetOutput();

                if (output != null)
                {
                    wait = 0;

                    lastOutputs.Enqueue(output);
                    if (lastOutputs.Count > 100) lastOutputs.Dequeue();

                    StringBuilder lastOutputSB = new StringBuilder();
                    foreach (string s in lastOutputs)
                        lastOutputSB.Append(s);

                    string lastOutput = lastOutputSB.ToString();

                    expectOutput = lastOutput;

                    bool found = false;
                    for (int i = 0; i < args.Length; i++)
                    {
                        string expect = args[i];
                        if (lastOutput.IndexOf(expect) > -1)
                        {
                            expectReturn = i;
                            found = true;
                            break;
                        }
                    }
                    if (found) break;
                }
                else
                {
                    Thread.Sleep(50);
                    wait += 1;
                    if (wait == 400)
                    {
                        timeout = true;
                        break;
                    }
                }
            }

            return new ExpectResult(expectReturn, expectOutput, timeout);
        }

        private string ResolveHostName(string hostname, bool reverse)
        {
            string cpeip = null;
            hostname = hostname.ToLower();

            if (Request("cat /etc/hosts", out string[] lines)) SessionFailure();

            Dictionary<string, string> greppair = new Dictionary<string, string>();
            foreach (string line in lines)
            {
                if (line.Length > 0 && char.IsDigit(line[0]))
                {
                    string[] linex = line.Trim().Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);

                    if (linex.Length == 2)
                    {
                        string gip = linex[0];
                        string ghn = linex[1].ToLower();

                        if (!reverse)
                        {
                            if (ghn == hostname)
                            {
                                if (!greppair.ContainsKey(ghn))
                                {
                                    greppair.Add(ghn, gip);
                                    break;
                                }
                            }
                        }
                        else
                        {
                            if (!greppair.ContainsKey(gip))
                            {
                                greppair.Add(gip, ghn);
                                break;
                            }
                        }
                    }
                }
            }

            if (greppair.ContainsKey(hostname.ToLower()))
                cpeip = greppair[hostname.ToLower()].ToUpper();
            return
                cpeip;
        }

        private void Update(UpdateTypes type, object value)
        {
            string key = null;

            switch (type)
            {
                case UpdateTypes.NecrowVersion: key = "NO_NVER"; break;
                case UpdateTypes.TimeStamp: key = "NO_TimeStamp"; break;
                case UpdateTypes.TimeOffset: key = "NO_TimeOffset"; break;
                case UpdateTypes.Remark: key = "NO_Remark"; break;
                case UpdateTypes.IP: key = "NO_IP"; break;
                case UpdateTypes.Name: key = "NO_Name"; break;
                case UpdateTypes.Terminal: key = "NO_Terminal"; break;
                case UpdateTypes.ConnectType: key = "NO_ConnectType"; break;
                case UpdateTypes.Model: key = "NO_Model"; break;
                case UpdateTypes.Version: key = "NO_Version"; break;
                case UpdateTypes.SubVersion: key = "NO_SubVersion"; break;
                case UpdateTypes.LastConfiguration: key = "NO_LastConfiguration"; break;
                case UpdateTypes.StartUpTime: key = "NO_LastStartUp"; break;
            }

            if (key != null)
            {
                if (updates.ContainsKey(key)) updates[key] = value;
                else updates.Add(key, value);
            }
        }

        private void Summary(string key, int value, bool archive = true)
        {
            Summary(key, value.ToString(), archive);
        }

        private void Summary(string key, float value, bool archive = true)
        {
            Summary(key, value.ToString(), archive);
        }

        private void Summary(string key, bool value, bool archive = true)
        {
            Summary(key, value ? 1 : 0, archive);
        }

        private void Summary(string key, string value, bool archive = true)
        {
            if (key != null && value != null)
            {
                if (summaries.ContainsKey(key))
                {
                    summaries[key] = (value, archive);
                }
                else
                {
                    summaries.Add(key, (value, archive));
                }
            }
        }

        private void Exit()
        {
            Event("Exiting...");
            if (outputIdentifier != null)
            {
                Exit(nodeManufacture);
            }
            outputIdentifier = null;
            Event("Exit!");

            IsProbing = false;
            LastNodeName = NodeName;
            LastProbeEndTime = DateTime.UtcNow;

            if (queueStop)
                Stop();
        }

        private void Exit(string manufacture)
        {
            if (idleThread != null)
            {
                idleThread.Abort();
                idleThread = null;
            }

            Thread.Sleep(200);
            SendLine("");
            Thread.Sleep(200);

            if (manufacture == alu) SendLine("logout");
            else if (manufacture == hwe) SendLine("quit");
            else if (manufacture == cso) SendLine("exit");
            else if (manufacture == jun) SendLine("exit");

            WaitUntilTerminalReady();
        }

        private void SaveExit()
        {
            Save();
            Exit();
        }

        private bool Request(string command, out string[] lines, ProbeProcessResult probe)
        {
            Event($"Sending command [{command}] (Length: {command.Length})");

            bool requestLoop = true;
            bool requestFailed = false;
            lines = null;

            int aluError = 0;

            while (requestLoop)
            {
                ClearOutput();
                SendLine(command);

                Stopwatch stopwatch = new Stopwatch();
                StringBuilder lineBuilder = new StringBuilder();
                List<string> listLines = new List<string>();

                stopwatch.Start();

                int wait = 0;
                int skip = 0;
                bool ending = false;

                while (true)
                {
                    if (OutputCount > 0)
                    {
                        ending = false;

                        wait = 0;
                        string output = GetOutput();

                        for (int i = 0; i < output.Length; i++)
                        {
                            byte b = (byte)output[i];

                            if (skip > 0) skip--;
                            else if (b >= 32) lineBuilder.Append((char)b);

                            string line = lineBuilder.ToString();
                            bool pressingMore = false;

                            if (line.EndsWith(nodeTerminal))
                            {
                                ending = true;
                                break;
                            }
                            else if (nodeManufacture == hwe && line.EndsWith("---- More ----"))
                            {
                                pressingMore = true;
                                lineBuilder.Clear();
                                SendSpace();
                                skip = 42;
                            }
                            else if (nodeManufacture == alu && line.EndsWith("Press any key to continue (Q to quit)"))
                            {
                                pressingMore = true;
                                lineBuilder.Clear();
                                SendSpace();
                                skip = 40;
                            }
                            else if (b == 10)
                            {
                                lineBuilder.Clear();
                                //if (nodeManufacture == hwe && nodeVersion == "5.160" && line.Length > 80)
                                //{
                                //    int looptimes = (int)Math.Ceiling((float)line.Length / 80);

                                //    for (int loop = 0; loop < looptimes; loop++)
                                //    {
                                //        int sisa = 80;
                                //        if (loop == looptimes - 1) sisa = line.Length - (loop * 80);
                                //        string curline = line.Substring(loop * 80, sisa);
                                //        listLines.Add(curline);
                                //    }
                                //}
                                //else
                                //{
                                //    listLines.Add(line);
                                //}

                                listLines.Add(line);
                            }

                            if (pressingMore)
                            {
                                //Event("<WAITING-2>" + listlines[listlines.Count - 1]);
                                //Event("<WAITING-1>" + line);
                                //Event("Sending space command for waiting message");
                            }
                        }
                    }
                    else
                    {
                        if (ending)
                            break;

                        if (!IsProbing)
                        {
                            requestFailed = true;
                            probe.FailureType = FailureTypes.ProbeStopped;
                            break;
                        }

                        wait++;

                        // 10 = 1 detik, 100 = 10 detik, 600 = 1 menit, 1200 = 2 menit, 3000 = 5 menit, 6000 = 10 menit

                        if (wait % 200 == 0 && wait < 6000) // setiap 20 detik, notifikasi waiting
                        {
                            Event($"Waiting for output ({wait / 10}s)");
                            SendLine("");

#if DEBUG
                            Event("Last Output: " + (LastOutput.Length > 200 ? LastOutput.Substring(LastOutput.Length - 200) : LastOutput));
#endif
                        }
                        Thread.Sleep(100);
                        if (wait == 6000)
                        {
                            Event("Output timeout, cancel the reading");
                            requestFailed = true;
                        }
                        else if (wait >= 6000 && wait % 50 == 0)
                        {
                            SendControlC();
                        }
                        else if (wait == 7200) // setelah 12 menit, request failed
                        {
                            if (probe != null)
                            {
                                requestFailed = false;
                                probe.FailureType = FailureTypes.Connection;
                            }
                        }
                    }
                }

                stopwatch.Stop();

                if (!requestFailed)
                {
                    lines = listLines.ToArray();
                    bool improperCommand = false;
                    if (lines.Length < 10)
                    {
                        improperCommand = true;
                        foreach (string line in lines)
                        {
                            if (line.Contains(command.Length <= 20 ? command : command.Substring(0, 20)))
                            {
                                improperCommand = false;
                                break;
                            }
                        }
                    }

                    if (improperCommand)
                    {
                        Event("Command truncated, resending the command");
                        ClearOutput();
                        Thread.Sleep(500);
                    }
                    else
                    {
                        if (listLines.Count > 0)
                        {
                            if (listLines[0].StartsWith(command) && listLines[0].Length > command.Length) listLines[0] = listLines[0].Substring(command.Length);
                        }

                        lines = listLines.ToArray();

                        requestLoop = false;

                        bool completed = true;

                        if (nodeManufacture == alu)
                        {
                            // alu: check for Command not allowed for this user
                            foreach (string line in lines)
                            {
                                if (line.IndexOf("Command not allowed for this user") > -1)
                                {
                                    aluError++;
                                    completed = false;
                                    break;
                                }
                            }
                        }

                        if (completed)
                        {
                            Event($"Output completed ({lines.Length} lines in {string.Format("{0:0.###}", stopwatch.Elapsed.TotalSeconds)}s)");
                            requestLoop = false;
                        }
                        else
                        {
                            if (aluError > 0)
                            {
                                if (aluError < 5)
                                {
                                    Event("Output not completed: ALU command error, resending the command");
                                    Thread.Sleep(5000);
                                    requestLoop = true;
                                }
                                else
                                {
                                    Event("Output not completed: ALU command error");
                                    requestLoop = false;
                                    requestFailed = true;
                                    probe.FailureType = FailureTypes.ALURequest;
                                }
                            }
                            else
                                requestLoop = false;
                        }
                    }
                }
                else requestLoop = false;
            }

            if (!requestFailed)
            {
                probe.FailureType = FailureTypes.None;
                return false;
            }
            else
            {
                if (probe != null && probe.FailureType == FailureTypes.None)
                    probe.FailureType = FailureTypes.Request;
                return true;
            }
        }

        private bool ConnectByTelnet(string host, string manufacture)
        {
            bool connectSuccess = false;

            string user, pass, thost;

            if (nodeType == "F")
            {
                user = "740289";
                pass = "1992Readone";
                thost = nodeIP;
            }
            else
            {
                user = tacacUser;
                pass = tacacPassword;
                thost = host;
            }

            Event("Connecting with Telnet... (" + user + "@" + host + ")");
            SendLine("telnet " + thost);

            ExpectResult expect = Expect("ogin:", "sername:");

            if (expect.Index > -1)
            {
                // arrived at login
                if (manufacture == null && LastOutput.IndexOf("ALCATEL") > -1)
                {
                    manufacture = alu;
                    nodeManufacture = alu;
                }

                Event("Authenticating: User");
                SendLine(user);

                expect = Expect("assword:");
                if (expect.Index > -1)
                {
                    Event("Authenticating: Password");
                    SendLine(pass);

                    expect = Expect("#", ">");
                    if (expect.Index > -1)
                    {
                        connectSuccess = true;
                    }
                    else
                    {
                        SendControlRightBracket();
                        SendControlC();
                    }
                }
                else
                {
                    Event("Cannot find password console prefix");
                    SendControlRightBracket();
                    SendControlC();
                }
            }
            else
            {
                if (expect.IsTimeout)
                {
                    // timeout
                }
            }

            return connectSuccess;
        }

        private bool ConnectBySSH(string host, string manufacture)
        {
            bool connectSuccess = false;

            string user = tacacUser;
            string pass = tacacPassword;

            // ssh-keygen -R <node>

            Event("Connecting with SSH... (" + user + "@" + host + ")");
            SendLine("ssh -o StrictHostKeyChecking=no " + user + "@" + host);

            bool looppass = false;
            do
            {
                looppass = false;

                ExpectResult expect = Expect("assword:", "Connection refused", "No route to host", "Not replacing existing known_hosts", "Host key verification failed", "Permission denied (publickey");
                if (expect.Index == 0)
                {
                    Event("Authenticating: Password");
                    SendLine(pass);

                    expect = Expect(">", "#");

                    if (expect.Index > -1)
                    {
                        connectSuccess = true;
                    }
                    else
                    {
                        SendControlC();
                    }
                }
                else if (expect.Index >= 3)
                {
                    SendControlC();
                    // fail to ssh-keygen, just remove the known_hosts
                    Event("Removing known_hosts file because an error...");
                    SendLine("rm -f ~/.ssh/known_hosts");

                    Thread.Sleep(500);
                    SendLine("ssh -o StrictHostKeyChecking=no " + user + "@" + host);

                    looppass = true;
                }
                else
                {
                    if (expect.Index == -1)
                    {
                        SendControlC();

                        if (expect.IsTimeout)
                        {
                            // timeout
                        }
                        else
                        {
                            if (expect.Output == null)
                            {
                                Event("Expect output null");
                            }
                            else
                            {
                                int llength = expect.Output.Length - LastSendLine.Length;

                                if (llength == 2)
                                {
                                    // connect fail for some reason
                                }
                                else
                                {
                                    // the other error probaby ssh key expired or failing
                                    Event("Trying to regenerate new ssh key...");
                                    SendLine("ssh-keygen -R " + host);

                                    expect = Expect("Not replacing existing known_hosts", "Host key verification failed", "Permission denied (publickey");
                                    if (expect.Index >= 0)
                                    {
                                        // fail to ssh-keygen, just remove the known_hosts
                                        Event("Removing known_hosts file because an error...");
                                        SendLine("rm -f ~/.ssh/known_hosts");
                                    }

                                    Thread.Sleep(500);
                                    SendLine("ssh -o StrictHostKeyChecking=no " + user + "@" + host);

                                    looppass = true;
                                }
                            }
                        }
                    }
                    else if (expect.Index == 1 || expect.Index == 2)
                    {
                        // connection refuse
                    }
                }
            }
            while (looppass);

            return connectSuccess;
        }

        private void Save()
        {
#if !DEBUG
            try
            {
#endif
                Insert insert;
                Update update;
                Result2 result;
                Batch batch = j.Batch();

                update = j.Update("Node");
                foreach (KeyValuePair<string, object> pair in updates)
                {
                    if (necrow.KeeperNode.ContainsKey(nodeID))
                    {
                        Dictionary<string, object> keeper = necrow.KeeperNode[nodeID];
                        if (pair.Key == "NO_IP")
                        {
                            keeper["NO_IP"] = pair.Value;
                        }
                    }
                    update.Set(pair.Key, pair.Value);
                }
                update.Where("NO_ID", nodeID);
                update.Execute();

                // end node
                result = j.ExecuteIdentity("insert into ProbeHistory(XH_NO, XH_StartTime, XH_EndTime) values({0}, {1}, {2})", nodeID, NodeProbeStartTime, DateTime.UtcNow);
                long probeHistoryID = result.Identity;

                // nodesummary
                result = j.Query("select * from NodeSummary where NS_NO = {0}", nodeID);
                //if (!result) return DatabaseFailure(probe);
                Dictionary<string, Tuple<string, string>> dbsummaries = new Dictionary<string, Tuple<string, string>>();

                batch.Begin();
                foreach (Row2 row in result)
                {
                    string key = row["NS_Key"].ToString();
                    string id = row["NS_ID"].ToString();
                    string value = row["NS_Value"].ToString();

                    if (dbsummaries.ContainsKey(key)) batch.Add("delete from NodeSummary where NS_ID = {0}", id); // Duplicated summary key, remove this
                    else dbsummaries.Add(key, new Tuple<string, string>(id, value));
                }
                batch.Commit();

                batch.Begin();
                foreach (KeyValuePair<string, (string, bool)> pair in summaries)
                {
                    (string value, bool archive) = pair.Value;

                    Tuple<string, string> db = null;
                    if (dbsummaries.ContainsKey(pair.Key)) db = dbsummaries[pair.Key];

                    if (db == null)
                    {
                        string id = Database2.ID();

                        insert = j.Insert("NodeSummary");
                        insert.Value("NS_ID", id);
                        insert.Value("NS_NO", nodeID);
                        insert.Value("NS_Key", pair.Key);
                        insert.Value("NS_Value", value);
                        batch.Add(insert);

                        if (archive)
                        {
                            insert = j.Insert("NodeSummaryArchive");
                            insert.Value("NSX_XH", probeHistoryID);
                            insert.Value("NSX_NS", id);
                            insert.Value("NSX_Value", value);
                            batch.Add(insert);
                        }

                        Event("Summary " + pair.Key + " NEW: " + pair.Value);
                    }
                    else
                    {
                        string id = db.Item1;
                        string existingValue = db.Item2;

                        if (value != existingValue)
                        {
                            // summary has changed

                            if (archive)
                            {
                                // insert archive
                                insert = j.Insert("NodeSummaryArchive");
                                insert.Value("NSX_XH", probeHistoryID);
                                insert.Value("NSX_NS", id);
                                insert.Value("NSX_Value", value);
                                batch.Add(insert);
                            }

                            // update nodesummary
                            update = j.Update("NodeSummary");
                            update.Set("NS_Value", value);
                            update.Where("NS_ID", id);
                            batch.Add(update);

                            Event("Summary " + pair.Key + " CHANGED: " + existingValue + " -> " + value);
                        }
                    }
                }
                batch.Commit();

#if !DEBUG
            }
            catch (Exception ex)
            {
                necrow.Log(NodeName, ex.Message, ex.StackTrace);
                Event("Caught error on Save(): " + ex.Message + ", try again");
                Save();
            }
#endif

            if (probeProgressID != null)
            {
                if (!ProgressPending && !ProgressQueueDeep)
                {
                    // delete progress whenever only not pending and not queueing deep
                    j.Execute("delete from ProbeProgress where XP_ID = {0}", probeProgressID);
                }

                probeProgressID = null;
            }
        }

        private void CheckNodeManufacture()
        {
            if (nodeManufacture == null)
            {
                if (nodeType == "F")
                {
                    nodeManufacture = fho;
                }
                else
                {
                    SendLine("show clock");

                    WaitUntilEndsWith(new string[] { "#", ">" });

                    if (LastOutput.IndexOf("syntax error, expecting") > -1) nodeManufacture = jun;
                    else if (LastOutput.IndexOf("Unrecognized command") > -1) nodeManufacture = hwe;
                    else if (LastOutput.IndexOf("Invalid parameter") > -1) nodeManufacture = alu;

                    if (nodeManufacture == null)
                    {
                        nodeManufacture = cso;
                    }
                }
            }

            Event("Discovered Manufacture: " + nodeManufacture);
            necrow.UpdateManufacture(nodeID, nodeManufacture);
        }

        private bool ParseHuaweiUptime(string uptimeString, out TimeSpan uptime)
        {
            string[] tokens = uptimeString.Split(StringSplitTypes.Comma, StringSplitOptions.RemoveEmptyEntries);

            int day = 0;
            int hour = 0;
            int minute = 0;

            foreach (string token in tokens)
            {
                string otoken = token.Trim().ToLower();
                string[] tokens2 = otoken.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);

                if (tokens2.Length == 2)
                {
                    int dal = int.Parse(tokens2[0]);

                    if (tokens2[1].StartsWith("day")) day = dal;
                    else if (tokens2[1].StartsWith("hour")) hour = dal;
                    else if (tokens2[1].StartsWith("minute")) minute = dal;
                }
            }

            uptime = new TimeSpan(day, hour, minute, 0);

            return true;
        }

        private ProbeProcessResult Enter(Row2 row, string probeProgressID, bool fromDeepNodeQueue, out bool continueProcess)
        {
            bool prioritizeProcess = false;
            ProbeProcessResult probe = new ProbeProcessResult();
            string[] lines = null;

            Batch batch = j.Batch();
            Result2 result = null;

            continueProcess = false;

            WaitUntilTerminalReady();

            updates = new Dictionary<string, object>();
            summaries = new Dictionary<string, (string, bool)>();

            nodeID = row["NO_ID"].ToString();
            NodeName = row["NO_Name"].ToString();
            nodeManufacture = row["NO_Manufacture"].ToString();
            nodeModel = row["NO_Model"].ToString();
            nodeVersion = row["NO_Version"].ToString();
            nodeSubVersion = row["NO_SubVersion"].ToString();
            nodeIP = row["NO_IP"].ToString();
            nodeTerminal = row["NO_Terminal"].ToString();
            nodeConnectType = row["NO_ConnectType"].ToString();
            nodeAreaID = row["NO_AR"].ToString();
            nodeType = row["NO_Type"].ToString();
            nodeNVER = row["NO_NVER"].ToInt(0);
            nodeStartUp = row["NO_LastStartUp"].ToDateTime(DateTime.MinValue);

            this.probeProgressID = probeProgressID;

            string previousRemark = row["NO_Remark"].ToString();
            DateTime? previousTimeStamp = row["NO_TimeStamp"].ToNullableDateTime();

            TimeSpan deltaTimeStamp = TimeSpan.Zero;
            if (previousTimeStamp != null)
            {
                deltaTimeStamp = DateTime.UtcNow - previousTimeStamp.Value;
            }

            NodeProbeStartTime = DateTime.UtcNow;

            if (probeProgressID != null)
                j.Execute("update ProbeProgress set XP_StartTime = {0} where XP_ID = {1}", DateTime.UtcNow, this.probeProgressID);

            Event("Begin probing into " + NodeName);

            Update(UpdateTypes.Remark, null);
            Update(UpdateTypes.TimeStamp, DateTime.UtcNow);

            bool checkChassis = false;
            updatingNecrow = false;
            if (nodeNVER < Necrow.Version)
            {
                updatingNecrow = true;
                Event("Updated to newer Necrow version");
                Update(UpdateTypes.NecrowVersion, Necrow.Version);
            }

            #region CHECK ACCESS RULE

            nodeRules = j.QueryList("select * from NodeAccessRule where NAR_NO = {0}", "NAR_Rule", nodeID);

            #endregion

            #region CHECK IP

            if (nodeIP != null) Event("Host IP: " + nodeIP);

            Event("Checking host IP");

            if (nodeType == "P" || nodeType == "M" || nodeType == "T")
            {
                string resolvedIP = ResolveHostName(NodeName, false);

                if (nodeIP == null)
                {
                    if (resolvedIP == null)
                    {
                        Event("Hostname is unresolved");

                        if (previousRemark == "UNRESOLVED1" && deltaTimeStamp > TimeSpan.FromHours(2))
                        {
                            Update(UpdateTypes.Remark, "UNRESOLVED2");
                            necrow.PendingNode(this, probeProgressID, TimeSpan.FromHours(22));
                        }
                        else if (previousRemark == "UNRESOLVED2" && deltaTimeStamp > TimeSpan.FromHours(24))
                        {
                            Update(UpdateTypes.Remark, "UNRESOLVED3");
                            necrow.PendingNode(this, probeProgressID, TimeSpan.FromHours(24));
                        }
                        else if (previousRemark == "UNRESOLVED3" && deltaTimeStamp > TimeSpan.FromHours(48))
                        {
                            Update(UpdateTypes.Remark, "UNRESOLVED4");
                            necrow.PendingNode(this, probeProgressID, TimeSpan.FromHours(24));
                        }
                        else if (previousRemark == "UNRESOLVED4" && deltaTimeStamp > TimeSpan.FromHours(72))
                        {
                            Update(UpdateTypes.Remark, "UNRESOLVED5");
                            necrow.PendingNode(this, probeProgressID, TimeSpan.FromHours(24));
                        }
                        else if (previousRemark == "UNRESOLVED5" && deltaTimeStamp > TimeSpan.FromHours(96))
                        {
                            Update(UpdateTypes.Remark, "UNRESOLVED6");
                            necrow.PendingNode(this, probeProgressID, TimeSpan.FromHours(72));
                        }
                        else if (previousRemark == "UNRESOLVED6" && deltaTimeStamp > TimeSpan.FromHours(168))
                        {
                            Update(UpdateTypes.Remark, "UNRESOLVED");
                            necrow.DisableNode(nodeID);
                        }
                        else if (previousRemark == null || !previousRemark.StartsWith("UNRESOLVED"))
                        {
                            Update(UpdateTypes.Remark, "UNRESOLVED1");
                            necrow.PendingNode(this, probeProgressID, TimeSpan.FromHours(2));
                        }
                        else Update(UpdateTypes.Remark, previousRemark);

                        Save();
                        return probe;
                    }
                    else
                    {
                        Event("Resolved Host IP: " + resolvedIP);
                        nodeIP = resolvedIP;
                        Update(UpdateTypes.IP, nodeIP);
                    }
                }
                else
                {
                    if (resolvedIP == null)
                    {
                        Event("Hostname is unresolved");

                        // reverse ip?
                        Event("Resolving by reverse host name");
                        string hostName = ResolveHostName(nodeIP, true);

                        if (hostName != null)
                        {
                            result = j.Query("select * from Node where NO_Name = {0}", hostName);

                            if (result.Count == 0)
                            {
                                Event("Node " + NodeName + " has changed to " + hostName);
                                if (!NecrowVirtualization.AliasExists(NodeName))
                                {
                                    j.Execute("insert into NodeAlias(NA_ID, NA_NO, NA_Name) values({0}, {1}, {2})", Database2.ID(), nodeID, NodeName);
                                    NecrowVirtualization.AliasLoad();
                                }

                                Update(UpdateTypes.Name, hostName);

                                // Update interface virtualizations
                                if (nodeType == "P" || nodeType == "T")
                                {
                                    Tuple<string, List<Tuple<string, string, string, string, string, string>>> changeThis = null;
                                    List<Tuple<string, string, string, string, string, string>> interfaces = null;
                                    lock (NecrowVirtualization.PESync)
                                    {
                                        foreach (Tuple<string, List<Tuple<string, string, string, string, string, string>>> entry in NecrowVirtualization.PEPhysicalInterfaces)
                                        {
                                            if (entry.Item1 == NodeName)
                                            {
                                                changeThis = entry;
                                                break;
                                            }
                                        }
                                        if (changeThis != null)
                                        {
                                            NecrowVirtualization.PEPhysicalInterfaces.Remove(changeThis);
                                            interfaces = changeThis.Item2;
                                        }
                                        else interfaces = new List<Tuple<string, string, string, string, string, string>>();

                                        NecrowVirtualization.PEPhysicalInterfaces.Add(new Tuple<string, List<Tuple<string, string, string, string, string, string>>>(hostName, interfaces));
                                        NecrowVirtualization.PEPhysicalInterfacesSort();
                                    }
                                }
                                else if (nodeType == "M")
                                {
                                    Tuple<string, List<Tuple<string, string, string, string, string, string, string>>> changeThis = null;
                                    List<Tuple<string, string, string, string, string, string, string>> interfaces = null;
                                    lock (NecrowVirtualization.MESync)
                                    {
                                        foreach (Tuple<string, List<Tuple<string, string, string, string, string, string, string>>> entry in NecrowVirtualization.MEPhysicalInterfaces)
                                        {
                                            if (entry.Item1 == NodeName)
                                            {
                                                changeThis = entry;
                                                break;
                                            }
                                        }
                                        if (changeThis != null)
                                        {
                                            NecrowVirtualization.MEPhysicalInterfaces.Remove(changeThis);
                                            interfaces = changeThis.Item2;
                                        }
                                        else interfaces = new List<Tuple<string, string, string, string, string, string, string>>();

                                        NecrowVirtualization.MEPhysicalInterfaces.Add(new Tuple<string, List<Tuple<string, string, string, string, string, string, string>>>(hostName, interfaces));
                                        NecrowVirtualization.MEPhysicalInterfacesSort();
                                    }
                                }

                                NodeName = hostName;
                            }
                            else
                            {
                                Event("Node " + NodeName + " has new name " + hostName + ". " + hostName + " is already exists.");
                                Event("Mark this node as inactive");

                                Update(UpdateTypes.Remark, "NAMECONFLICTED");
                                necrow.DisableNode(nodeID);

                                Save();
                                return probe;
                            }
                        }
                        else
                        {
                            Event("Hostname has become unresolved");

                            if (previousRemark == "UNRESOLVED1" && deltaTimeStamp > TimeSpan.FromHours(2))
                            {
                                Update(UpdateTypes.Remark, "UNRESOLVED2");
                                necrow.PendingNode(this, probeProgressID, TimeSpan.FromHours(22));
                            }
                            else if (previousRemark == "UNRESOLVED2" && deltaTimeStamp > TimeSpan.FromHours(24))
                            {
                                Update(UpdateTypes.Remark, "UNRESOLVED3");
                                necrow.PendingNode(this, probeProgressID, TimeSpan.FromHours(24));
                            }
                            else if (previousRemark == "UNRESOLVED3" && deltaTimeStamp > TimeSpan.FromHours(48))
                            {
                                Update(UpdateTypes.Remark, "UNRESOLVED4");
                                necrow.PendingNode(this, probeProgressID, TimeSpan.FromHours(24));
                            }
                            else if (previousRemark == "UNRESOLVED4" && deltaTimeStamp > TimeSpan.FromHours(72))
                            {
                                Update(UpdateTypes.Remark, "UNRESOLVED5");
                                necrow.PendingNode(this, probeProgressID, TimeSpan.FromHours(24));
                            }
                            else if (previousRemark == "UNRESOLVED5" && deltaTimeStamp > TimeSpan.FromHours(96))
                            {
                                Update(UpdateTypes.Remark, "UNRESOLVED6");
                                necrow.PendingNode(this, probeProgressID, TimeSpan.FromHours(72));
                            }
                            else if (previousRemark == "UNRESOLVED6" && deltaTimeStamp > TimeSpan.FromHours(168))
                            {
                                Update(UpdateTypes.Remark, "UNRESOLVED");
                                necrow.DisableNode(nodeID);
                            }
                            else if (previousRemark == null || !previousRemark.StartsWith("UNRESOLVED"))
                            {
                                Update(UpdateTypes.Remark, "UNRESOLVED1");
                                necrow.PendingNode(this, probeProgressID, TimeSpan.FromHours(2));
                            }
                            else Update(UpdateTypes.Remark, previousRemark);

                            Save();
                            return probe;
                        }
                    }
                    else if (nodeIP != resolvedIP)
                    {
                        Event("Host IP has changed to: " + resolvedIP);
                        Update(UpdateTypes.IP, resolvedIP);
                        nodeIP = resolvedIP;
                    }
                }
            }
            else if (nodeType == "F")
            {
                if (nodeIP == null)
                {
                    Update(UpdateTypes.Remark, "UNRESOLVED");
                    necrow.DisableNode(nodeID);
                    Save();
                    return probe;
                }
            }

            Event("Host identified");

            outputIdentifier = NodeName;

            #endregion

            #region MANUFACTURE

            // check node manufacture
            bool checkNodeManufacture = false;

            if (nodeManufacture == alu || nodeManufacture == cso || nodeManufacture == hwe || nodeManufacture == jun)
            {
                Event("Manufacture: " + nodeManufacture + "");
                if (nodeModel != null) Event("Model: " + nodeModel);
            }
            else
            {
                nodeManufacture = null;
                checkNodeManufacture = true;

                Event("Manufacture: Unknown");
            }

            #endregion

            #region CONNECT

            string connectType = null;
            string connectBy = null;

            while (true)
            {
                int connectSequence = 0;
                connectType = nodeConnectType;

                // Prepare connect preference
                if (connectType == null)
                {
                    if (nodeManufacture == alu || nodeManufacture == cso) connectType = "T";
                    else if (nodeManufacture == hwe || nodeManufacture == jun) connectType = "S";
                    else connectType = "T";
                }

                bool connectSuccess = false;

                while (true)
                {
                    WaitUntilTerminalReady();

                    string currentConnectType = null;

                    if (connectSequence == 0)
                    {
                        if (connectType == "T") currentConnectType = "T";
                        else if (connectType == "S") currentConnectType = "S";
                    }
                    else if (connectSequence == 1)
                    {
                        if (connectType == "T") currentConnectType = "S";
                        else if (connectType == "S") currentConnectType = "T";
                    }

                    if (currentConnectType == "T")
                    {
                        connectSuccess = ConnectByTelnet(NodeName, nodeManufacture);
                        if (connectSuccess) connectBy = "T";
                        else Event("Telnet failed");
                    }
                    else if (currentConnectType == "S")
                    {
                        connectSuccess = ConnectBySSH(NodeName, nodeManufacture);
                        if (connectSuccess) connectBy = "S";
                        else Event("SSH failed");
                    }

                    if (connectSuccess || connectSequence == 1) break;
                    else connectSequence++;
                }

                if (connectSuccess == false)
                {
                    Event("Connection failed");

                    bool tacacWasNotOkay = false;

                    int loop = 0;
                    while (true)
                    {
                        WaitUntilTerminalReady();

                        string testOtherNode;

                        if (NodeName == "PE2-D2-CKA-VPN") testOtherNode = "PE-D2-CKA-VPN";
                        else testOtherNode = "PE2-D2-CKA-VPN";

                        Event("Trying to connect to other node...(" + testOtherNode + ")");

                        bool testConnected = ConnectByTelnet(testOtherNode, cso);

                        if (testConnected)
                        {
                            Exit(cso);
                            outputIdentifier = null;

                            if (tacacWasNotOkay)
                                Event("Connected! TACAC server connection is restored");
                            else
                                Event("Connected! TACAC server is OK.");

                            break;
                        }
                        else
                        {
                            tacacWasNotOkay = true;

                            outputIdentifier = null;
                            Event("Connection failed, TACAC server is possible overloaded/error/not responding.");

                            int time = 1;
                            #region time
                            if (loop == 0)
                            {
                                Event("Try again in 1 minute");
                            }
                            else if (loop == 1)
                            {
                                Event("Try again in 5 minutes");
                                time = 5;
                            }
                            else if (loop == 2)
                            {
                                Event("Try again in 15 minutes");
                                time = 15;
                            }
                            else if (loop == 3)
                            {
                                Event("Try again in 30 minutes");
                                time = 30;
                            }
                            else if (loop >= 4)
                            {
                                Event("Try again in 1 hour");
                                time = 60;
                            }
                            #endregion

                            Thread.Sleep(60000 * time);
                            loop++;
                        }
                    }

                    if (tacacWasNotOkay)
                    {
                        Event("Retrying...");
                    }
                    else
                    {
                        if (previousRemark == "CONNECTFAIL1" && deltaTimeStamp > TimeSpan.FromHours(2))
                        {
                            Update(UpdateTypes.Remark, "CONNECTFAIL2");
                            necrow.PendingNode(this, probeProgressID, TimeSpan.FromHours(4));
                        }
                        else if (previousRemark == "CONNECTFAIL2" && deltaTimeStamp > TimeSpan.FromHours(6))
                        {
                            Update(UpdateTypes.Remark, "CONNECTFAIL3");
                            necrow.PendingNode(this, probeProgressID, TimeSpan.FromHours(6));
                        }
                        else if (previousRemark == "CONNECTFAIL3" && deltaTimeStamp > TimeSpan.FromHours(12))
                        {
                            Update(UpdateTypes.Remark, "CONNECTFAIL4");
                            necrow.PendingNode(this, probeProgressID, TimeSpan.FromHours(12));
                        }
                        else if (previousRemark == "CONNECTFAIL4" && deltaTimeStamp > TimeSpan.FromHours(24))
                        {
                            Update(UpdateTypes.Remark, "CONNECTFAIL5");
                            necrow.PendingNode(this, probeProgressID, TimeSpan.FromHours(24));
                        }
                        else if (previousRemark == "CONNECTFAIL5" && deltaTimeStamp > TimeSpan.FromHours(48))
                        {
                            Update(UpdateTypes.Remark, "CONNECTFAIL6");
                            necrow.PendingNode(this, probeProgressID, TimeSpan.FromHours(24));
                        }
                        else if (previousRemark == "CONNECTFAIL6" && deltaTimeStamp > TimeSpan.FromHours(72))
                        {
                            Update(UpdateTypes.Remark, "CONNECTFAIL7");
                            necrow.PendingNode(this, probeProgressID, TimeSpan.FromHours(24));
                        }
                        else if (previousRemark == "CONNECTFAIL7" && deltaTimeStamp > TimeSpan.FromHours(96))
                        {
                            Update(UpdateTypes.Remark, "CONNECTFAIL8");
                            necrow.PendingNode(this, probeProgressID, TimeSpan.FromHours(24));
                        }
                        else if (previousRemark == "CONNECTFAIL8" && deltaTimeStamp > TimeSpan.FromHours(120))
                        {
                            Update(UpdateTypes.Remark, "CONNECTFAIL9");
                            necrow.PendingNode(this, probeProgressID, TimeSpan.FromHours(24));
                        }
                        else if (previousRemark == "CONNECTFAIL9" && deltaTimeStamp > TimeSpan.FromHours(144))
                        {
                            Update(UpdateTypes.Remark, "CONNECTFAIL10");
                            necrow.PendingNode(this, probeProgressID, TimeSpan.FromHours(24));
                        }
                        else if (previousRemark == "CONNECTFAIL10" && deltaTimeStamp > TimeSpan.FromHours(168))
                        {
                            Update(UpdateTypes.Remark, "CONNECTFAIL");
                            necrow.DisableNode(nodeID);
                        }
                        else if (previousRemark == null || !previousRemark.StartsWith("CONNECTFAIL"))
                        {
                            Update(UpdateTypes.Remark, "CONNECTFAIL1");
                            necrow.PendingNode(this, probeProgressID, TimeSpan.FromHours(2));
                        }
                        else Update(UpdateTypes.Remark, previousRemark);

                        Save();

                        return probe;
                    }
                }
                else break;
            }

            if (nodeConnectType == null || connectBy != connectType)
                Update(UpdateTypes.ConnectType, connectBy);

            if (checkNodeManufacture)
            {
                CheckNodeManufacture();
            }

            if (nodeManufacture == null)
            {
                Event("Unknown manufacture");
                Save();
                return probe;
            }

            Event("Connected!");

            IsProbing = true;

            string terminal = null;

            if (nodeManufacture == alu)
            {
                lines = LastOutput.Split('\n');
                string lastLine = lines[lines.Length - 1];

                int titik2 = lastLine.LastIndexOf('*');
                terminal = lastLine.Substring(titik2 + 1);
            }
            else if (nodeManufacture == hwe)
            {
                lines = LastOutput.Split('\n');
                string lastLine = lines[lines.Length - 1];
                terminal = lastLine;
            }
            else if (nodeManufacture == cso)
            {
                lines = LastOutput.Split('\n');
                string lastLine = lines[lines.Length - 1];
                terminal = lastLine;

                if (terminal.EndsWith(">"))
                {
                    Event("This CISCO node is not in previledge mode. Clearing node manufacture...");

                    nodeManufacture = null;
                    CheckNodeManufacture();

                    if (nodeManufacture == null)
                    {
                        Event("Unknown manufacture");
                        Save();
                    }
                    else
                    {
                        probe.FailureType = FailureTypes.ManufactureCorrected;
                    }

                    return probe;
                }
            }
            else if (nodeManufacture == jun)
            {
                lines = LastOutput.Split('\n');
                string lastLine = lines[lines.Length - 1];

                string[] linex = lastLine.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                terminal = linex[1];
            }
            else if (nodeManufacture == fho)
            {
                lines = LastOutput.Split('\n');
                string lastLine = lines[lines.Length - 1];
                terminal = lastLine;
            }

            terminal = terminal.Trim();

            Event("Terminal: " + terminal + "");

            if (terminal != nodeTerminal) Update(UpdateTypes.Terminal, terminal);
            nodeTerminal = terminal;

            #endregion

            #region TIME

            Event("Checking Time");

            bool nodeTimeRetrieved = false;
            DateTime utcTime = DateTime.UtcNow;
            DateTime nodeTime = DateTime.MinValue;
            TimeSpan uptime = TimeSpan.Zero;

            DateTime currentNodeStartUp = DateTime.MinValue;
            bool ignoreSecond = false;
            bool ignoreUptime = false;

            string[] junShowSystemUptimeLines = null;

            bool nodeStartTimeRetrieved = false;

            if (nodeManufacture == alu)
            {
                #region alu

                if (Request("show system time", out lines, probe)) return probe;

                foreach (string line in lines)
                {
                    if (line.StartsWith("Current Date"))
                    {
                        //Current Date & Time : 2016/05/06 23:38:39
                        //01234567890123456789012345678901234567890
                        string ps = line.Substring(22, 19);
                        if (DateTime.TryParseExact(ps, "yyyy/MM/dd HH:mm:ss", null, DateTimeStyles.None, out nodeTime)) { nodeTimeRetrieved = true; }
                        break;
                    }
                }

                if (Request("show uptime", out lines, probe)) return probe;

                foreach (string line in lines)
                {
                    if (line.StartsWith("System Up Time"))
                    {
                        //System Up Time         : 215 days, 21:04:59.62 (hr:min:sec)
                        string[] tokens = line.Split(new char[] { ':', '(' }, StringSplitOptions.RemoveEmptyEntries);
                        if (tokens.Length > 2)
                        {
                            string daytime = tokens[1];
                            string[] tokens2 = daytime.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            if (tokens2.Length == 2)
                            {
                                int day = 0;

                                string[] daytoken = tokens2[0].Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);
                                if (daytoken.Length == 2)
                                {
                                    day = int.Parse(daytoken[0]);
                                }

                                int hour = 0;
                                int minute = 0;
                                int second = 0;

                                string[] timetoken = tokens2[1].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                                if (timetoken.Length == 3)
                                {
                                    hour = int.Parse(timetoken[0]);
                                    minute = int.Parse(timetoken[1]);
                                    string[] tokens3 = timetoken[2].Split(StringSplitTypes.Dot, StringSplitOptions.RemoveEmptyEntries);

                                    second = int.Parse(tokens3[0]);

                                    if (tokens3.Length == 2)
                                    {
                                        int milisecond = (int)Math.Round(1000 / (double)int.Parse(tokens3[1]));
                                        if (milisecond >= 500) second++;
                                    }
                                }

                                uptime = new TimeSpan(day, hour, minute, second);
                                nodeStartTimeRetrieved = true;
                            }
                        }
                        break;
                    }
                }

                #endregion
            }
            else if (nodeManufacture == cso)
            {
                #region cso
                if (Request("show clock", out lines, probe)) return probe;

                foreach (string line in lines)
                {
                    string lineTrim = line.Trim(new char[] { '*', ' ', '.' });
                    if (lineTrim.Length > 0 && char.IsDigit(lineTrim[0]))
                    {
                        string[] ps = lineTrim.Split('.');
                        //00:26:20.139 WIB Sat May 7 2016
                        string[] pt = ps[1].Split(' ');
                        string form = string.Format("{0} {1} {2} {3}", ps[0], pt[3], pt[4], pt[5]);
                        if (DateTime.TryParseExact(form, "HH:mm:ss MMM d yyyy", null, DateTimeStyles.None, out nodeTime)) { nodeTimeRetrieved = true; }
                        break;
                    }
                }

                if (nodeVersion == null)
                {
                    if (Request("show version | in IOS", out lines, probe)) return probe;

                    string sl = string.Join("", lines.ToArray());

                    if (sl.IndexOf("Cisco IOS XR Software") > -1) nodeVersion = "XR";
                    else nodeVersion = "IOS";
                }

                if (nodeVersion == xr)
                {
                    #region xr
                    if (Request("sh ver brief | in uptime", out lines, probe)) return probe;

                    //sh ver brief | in uptime
                    foreach (string line in lines)
                    {
                        //PE2-D2-JT2-VPN uptime is 39 weeks, 6 days, 14 hours, 20 minutes
                        int uptimeIs = line.IndexOf("uptime is");
                        if (uptimeIs > -1)
                        {
                            string[] tokens = line.Substring(uptimeIs + 9).Trim().Split(StringSplitTypes.Comma, StringSplitOptions.RemoveEmptyEntries);

                            int day = 0;
                            int hour = 0;
                            int minute = 0;

                            foreach (string token in tokens)
                            {
                                string otoken = token.Trim().ToLower();
                                string[] tokens2 = otoken.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);

                                if (tokens2.Length == 2)
                                {
                                    int dal = int.Parse(tokens2[0]);

                                    if (tokens2[1].StartsWith("year")) day += (dal * 365);
                                    else if (tokens2[1].StartsWith("week")) day += (dal * 7);
                                    else if (tokens2[1].StartsWith("day")) day += dal;
                                    else if (tokens2[1].StartsWith("hour")) hour = dal;
                                    else if (tokens2[1].StartsWith("minute")) minute = dal;
                                }
                            }

                            uptime = new TimeSpan(day, hour, minute, 0);
                            nodeStartTimeRetrieved = true;
                            ignoreSecond = true;
                        }
                    }
                    #endregion
                }
                else
                {
                    #region ios
                    if (Request("show version | in uptime", out lines, probe)) return probe;

                    //sh ver brief | in uptime
                    foreach (string line in lines)
                    {
                        //PE2-D2-JT2-VPN uptime is 39 weeks, 6 days, 14 hours, 20 minutes
                        int uptimeIs = line.IndexOf("uptime is");
                        if (uptimeIs > -1)
                        {
                            string[] tokens = line.Substring(uptimeIs + 9).Trim().Split(StringSplitTypes.Comma, StringSplitOptions.RemoveEmptyEntries);

                            int day = 0;
                            int hour = 0;
                            int minute = 0;

                            foreach (string token in tokens)
                            {
                                string otoken = token.Trim().ToLower();
                                string[] tokens2 = otoken.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);

                                if (tokens2.Length == 2)
                                {
                                    int dal = int.Parse(tokens2[0]);

                                    if (tokens2[1].StartsWith("year")) day += (dal * 365);
                                    else if (tokens2[1].StartsWith("week")) day += (dal * 7);
                                    else if (tokens2[1].StartsWith("day")) day += dal;
                                    else if (tokens2[1].StartsWith("hour")) hour = dal;
                                    else if (tokens2[1].StartsWith("minute")) minute = dal;
                                }
                            }

                            uptime = new TimeSpan(day, hour, minute, 0);
                            nodeStartTimeRetrieved = true;
                            ignoreSecond = true;
                        }
                    }

                    #endregion
                }

                #endregion
            }
            else if (nodeManufacture == hwe)
            {
                #region hwe
                if (Request("display clock", out lines, probe)) return probe;

                foreach (string line in lines)
                {
                    if (line.Length > 0 && line[0] == '2')
                    {
                        //2016-05-06 16:52:51+08:00
                        //0123456789012345678901234
                        string[] ps = line.Split('+');
                        string date = ps[0].Trim();
                        if (DateTime.TryParseExact(date, "yyyy-MM-dd HH:mm:ss", null, DateTimeStyles.None, out nodeTime))
                        {
                            nodeTimeRetrieved = true;
                        }
                        break;
                    }
                }

                if (Request("display version | in Master", out lines, probe)) return probe;

                //MPU(Master) 9  : uptime is 128 days, 16 hours, 12 minutes
                foreach (string line in lines)
                {
                    int uptimeIs = line.IndexOf("uptime is");
                    if (uptimeIs > -1 && line.IndexOf("Master") > -1)
                    {
                        if (ParseHuaweiUptime(line.Substring(uptimeIs + 9).Trim(), out uptime))
                        {
                            nodeStartTimeRetrieved = true;
                            ignoreSecond = true;
                        }

                        break;
                    }
                }

                if (!nodeStartTimeRetrieved)
                {
                    if (Request("display version", out lines, probe)) return probe;

                    // old device cant "display version | in Master"
                    //Quidway CX300A uptime is 1476 days, 5 hours, 33 minutes
                    foreach (string line in lines)
                    {
                        int uptimeIs = line.IndexOf("uptime is");
                        if (uptimeIs > -1)
                        {
                            if (ParseHuaweiUptime(line.Substring(uptimeIs + 9).Trim(), out uptime))
                            {
                                nodeStartTimeRetrieved = true;
                                ignoreSecond = true;
                            }

                            break;
                        }
                    }
                }

                #endregion
            }
            else if (nodeManufacture == jun)
            {
                #region jun
                if (Request("show system uptime", out junShowSystemUptimeLines, probe)) return probe;

                foreach (string line in junShowSystemUptimeLines)
                {
                    if (line.StartsWith("Current time: "))
                    {
                        //Current time: 2016-05-07 00:02:48
                        //0123456789012345678901234567890123456789
                        string ps = line.Substring(14, 19);
                        if (DateTime.TryParseExact(ps, "yyyy-MM-dd HH:mm:ss", null, DateTimeStyles.None, out nodeTime)) { nodeTimeRetrieved = true; }
                    }
                    else if (line.StartsWith("System booted:"))
                    {
                        //System booted: 2013-04-03 16:14:08 WIT (236w0d 22:30 ago)
                        //0123456789012345
                        string ps = line.Substring(15, 19);
                        if (DateTime.TryParseExact(ps, "yyyy-MM-dd HH:mm:ss", null, DateTimeStyles.None, out currentNodeStartUp))
                        {
                            ignoreUptime = true;
                            nodeStartTimeRetrieved = true;
                        }
                        break;
                    }
                }

                #endregion
            }
            else if (nodeManufacture == fho)
            {
                #region fho

                #endregion
            }

            if (!nodeTimeRetrieved)
            {
                throw new Exception("Failure on node time retrieval");
            }
            if (!nodeStartTimeRetrieved)
            {
                throw new Exception("Failure on node start up time retrieval");
            }

            utcTime = new DateTime(
                utcTime.Ticks - (utcTime.Ticks % TimeSpan.TicksPerSecond),
                utcTime.Kind
                ); // cut millisecond section

            nodeTimeOffset = nodeTime - utcTime;
            Event("Local time: " + nodeTime.ToString("yyyy/MM/dd HH:mm:ss"));
            Event("UTC offset: " + nodeTimeOffset.TotalHours + "h");
            Update(UpdateTypes.TimeOffset, nodeTimeOffset.TotalHours);

            if (!ignoreUptime)
            {
                currentNodeStartUp = utcTime - uptime;
                if (ignoreSecond)
                    currentNodeStartUp = new DateTime(currentNodeStartUp.Year, currentNodeStartUp.Month, currentNodeStartUp.Day, currentNodeStartUp.Hour, currentNodeStartUp.Minute, 0, 0);
            }
            else
            {
                // convert to UTC
                currentNodeStartUp = currentNodeStartUp - nodeTimeOffset;
            }

            if (nodeStartUp < currentNodeStartUp)
            {
                Event("Last node start up: " + nodeStartUp.ToString("yyyy/MM/dd HH:mm:ss") + " UTC");
                Event("Current node start up: " + currentNodeStartUp.ToString("yyyy/MM/dd HH:mm:ss") + " UTC");

                Update(UpdateTypes.StartUpTime, currentNodeStartUp);

                checkChassis = true; // node probably has been updated, check chassis configuration (versions/modul etc)
            }
            else
            {
                Event("Node is still up since last time, last start up: " + currentNodeStartUp.ToString("yyyy/MM/dd HH:mm:ss") + " UTC");
            }

            #endregion

            #region TERMINAL SETUP

            Event("Setup terminal");

            bool setNoPagePartition = !nodeRules.Contains("ENABLEPAGEPARTITION");
            if (!setNoPagePartition)
                Event("Enable Page Partition set (by ENABLEPAGEPARTITION rule)");

#if DEBUG
            if (!setNoPagePartition)
            {
                Event("DEBUG: Canceled Enable Page Partition rule");
                setNoPagePartition = true;
            }
#endif

            if (nodeManufacture == alu)
            {
                if (Request("environment no saved-ind-prompt", out lines, probe)) return probe;
                if (setNoPagePartition)
                {
                    if (Request("environment no more", out lines, probe)) return probe;
                }
            }
            else if (nodeManufacture == hwe)
            {
                if (setNoPagePartition)
                {
                    if (Request("screen-length 0 temporary", out lines, probe)) return probe;
                }
            }
            else if (nodeManufacture == cso)
            {
                if (Request("terminal length 0", out lines, probe)) return probe;
            }
            else if (nodeManufacture == jun)
            {
                if (Request("set cli screen-length 0", out lines, probe)) return probe;
            }

            #endregion

            #region CHASSIS

            if (nodeVersion == null || nodeModel == null || updatingNecrow || prioritizeProcess)
                checkChassis = true;

            if (checkChassis)
            {
                Event("Checking Chassis");

                string version = null;
                string subVersion = null;
                string model = null;


                Dictionary<string, NodeSlotData> slotlive = new Dictionary<string, NodeSlotData>();
                Dictionary<string, Row2> slotdb = j.QueryDictionary("select * from NodeSlot where NC_NO = {0}", delegate (Row2 slotr)
                {

                    return slotr["NC_Slot1"].ToString() + "-" + slotr["NC_Slot2"].ToString("") + "-" + slotr["NC_Slot3"].ToString("");
                }, nodeID);
                if (slotdb == null) return DatabaseFailure(probe);
                List<NodeSlotData> slotinsert = new List<NodeSlotData>();
                List<NodeSlotData> slotupdate = new List<NodeSlotData>();

                int maxSlot = 0;
                int curSlot = 0;

                if (nodeManufacture == alu)
                {
                    #region alu
                    if (Request("show version | match TiMOS", out lines, probe)) return probe;

                    foreach (string line in lines)
                    {
                        //TiMOS-C-11.0.R6 cp
                        //0123456789012345
                        if (line.StartsWith("TiMOS"))
                        {
                            version = line.Substring(0, line.IndexOf(' ')).Trim();
                            break;
                        }
                    }

                    if (Request("show chassis", out lines, probe)) return probe;

                    int chassisSection = 0;

                    foreach (string line in lines)
                    {
                        string lineTrim = line.Trim();

                        if (line.StartsWith("------------")) chassisSection++;

                        if (chassisSection == 0)
                        {
                            if (lineTrim.StartsWith("Type"))
                            {
                                string[] tokens = lineTrim.Split(new char[] { ':' });
                                if (tokens.Length == 2)
                                    model = tokens[1].Trim();
                            }
                            else if (lineTrim.StartsWith("Number of slots"))
                            {
                                string[] tokens = lineTrim.Split(new char[] { ':' });
                                if (tokens.Length == 2)
                                {
                                    string slots = tokens[1].Trim();
                                    if (int.TryParse(slots, out int c))
                                    {
                                        if (c > 2)
                                            maxSlot = c - 2;
                                    }
                                }
                            }
                        }
                    }

                    // hardcoded maxSlot
                    if (model == "7750 SR-a8")
                        maxSlot = 4;

                    if (Request("show card state", out lines, probe)) return probe;

                    int nc1 = -1, nc2 = -1;

                    bool capturing = false;

                    NodeSlotData current = null;

                    foreach (string line in lines)
                    {
                        if (line.Length > 0)
                        {
                            string lineTrim = line.Trim();

                            if (!capturing && char.IsDigit(line[0]))
                            {
                                capturing = true;
                            }
                            else if (capturing && line[0] == '=')
                            {
                                capturing = false;
                            }

                            if (capturing)
                            {
                                if (line[0] == ' ')
                                {
                                    current.Board = lineTrim;
                                }
                                else
                                {
                                    nc2 = -1;

                                    string[] tokens = lineTrim.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);

                                    string ix = tokens[0];
                                    if (ix.IndexOf('/') > -1)
                                    {
                                        string[] ox = ix.Split(StringSplitTypes.Slash);

                                        if (!int.TryParse(ox[0], out nc1))
                                        {
                                            nc1 = 0;
                                        }
                                        if (!int.TryParse(ox[1], out nc2))
                                        {
                                            nc2 = 0;
                                        }
                                    }
                                    else
                                    {
                                        if (char.IsDigit(ix[0]))
                                        {
                                            nc1 = int.Parse(ix);
                                            curSlot++;
                                        }
                                        else
                                        {
                                            nc1 = 101 + (ix[0] - 65);
                                        }
                                    }

                                    // save
                                    NodeSlotData li = new NodeSlotData();

                                    li.Slot1 = nc1;

                                    if (nc2 > -1)
                                        li.Slot2 = nc2;

                                    li.Type = tokens[1];
                                    li.Board = tokens[1];
                                    li.Enable = tokens[2] == "up" ? true : false;
                                    li.Protocol = tokens[3] == "up" ? true : false;

                                    if (tokens.Length > 4)
                                    {
                                        if (int.TryParse(tokens[4], out int capacity))
                                        {
                                            li.Capacity = capacity;
                                        }
                                    }

                                    current = li;

                                    string key = nc1 + "-" + (nc2 > -1 ? nc2 + "-" : "-");

                                    slotlive.Add(key, li);
                                }
                                /*

1      iom3-xp                           up    up                  2
1/1    m2-10gb-xp-xfp                    up    up            2
1/2    m10-1gb-xp-sfp                    up    up            10
2      iom3-xp                           up    up                  2
2/1    m2-10gb-xp-xfp                    up    up            2
2/2    m10-1gb-xp-sfp                    up    up            10
3      iom-20g-b                         up    up                  2
3/1    m20-1gb-xp-sfp                    up    up            20
3/2    m2-10gb-xp-xfp                    up    up            2
4      iom-20g-b                         up    up                  2
4/1    m2-10gb-xp-xfp                    up    up            2
4/2    m2-10gb-xp-xfp                    down  provisioned   2
           (not equipped)
A      sfm-80g                           up    up                      Active
B      sfm-80g                           up    up                      Standby


                                */
                            }
                        }
                    }

                    #endregion
                }
                else if (nodeManufacture == hwe)
                {
                    #region hwe
                    if (Request("display version | in VRP|HUAWEI", out lines, probe)) return probe;

                    foreach (string line in lines)
                    {
                        if (line.ToLower().StartsWith("vrp (r) software"))
                            version = line.Substring(26, line.IndexOf(' ', 26) - 26).Trim();
                        if (line.StartsWith("HUAWEI "))
                            model = line.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries)[1];
                        if (version != null && model != null) break;
                    }

                    if (Request("display version | in LPU|StartupTime|Version", out lines, probe)) return probe;

                    int nc1 = -1, nc2 = -1;

                    DateTime? startuptime = null;

                    foreach (string line in lines)
                    {
                        if (line.Length > 0)
                        {

                            string lineTrim = line.Trim();

                            if (lineTrim.StartsWith("LPU  Slot  Quantity"))
                            {
                                string[] tokens = lineTrim.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                                if (tokens.Length == 2 && int.TryParse(tokens[1].Trim(), out int c)) maxSlot = c;
                            }
                            else if (line.StartsWith("LPU "))
                            {
                                curSlot++;

                                string[] tokens = lineTrim.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);

                                if (tokens.Length > 1)
                                {
                                    string s = tokens[1].Trim();
                                    if (int.TryParse(s, out int c))
                                    {
                                        nc1 = c;
                                        nc2 = -1;
                                        startuptime = null;
                                    }
                                }
                            }
                            else if (nc1 != -1)
                            {
                                if (lineTrim.StartsWith("StartupTime"))
                                {
                                    string[] tokens = lineTrim.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);
                                    if (tokens.Length == 3)
                                    {
                                        string date = tokens[1];
                                        string time = tokens[2];
                                        if (DateTime.TryParseExact(date + " " + time, "yyyy/MM/dd HH:mm:ss", null, DateTimeStyles.None, out DateTime startupDateTime))
                                        {
                                            startuptime = startupDateTime;
                                        }
                                    }
                                }
                                else if (line.IndexOf("PCB") > -1 && line.IndexOf("Version :") > -1)
                                {
                                    string[] tokens = lineTrim.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                                    if (tokens.Length == 2)
                                    {
                                        NodeSlotData li = new NodeSlotData();

                                        li.Slot1 = nc1;

                                        li.Info = tokens[1].Trim();
                                        li.LastStartUp = startuptime;

                                        slotlive.Add(nc1 + "--", li);

                                        nc1 = -1;
                                    }
                                }
                            }
                        }
                    }

                    if (Request("display elabel brief", out lines, probe)) return probe;


                    /*
          1         2         3         4         5         6
0123456789012345678901234567890123456789012345678901234567890123456789
LPU 1      CR57LPUF101A0       210305358910E2000051            LPUF-100
  PIC 0    CR57L5XXA0          030MXM10E2000053                P100-A-5*10GBase LAN/WAN-XFP-A
  PIC 2    CR57L5XXA0          030MXM10E2000049                P100-A-5*10GBase LAN/WAN-XFP-A
LPU 2      CR57LPUF240A00      210305469010K7000259            LPUF-240
  PIC 0    CR57E1NCB1          030QDFW0K7000001                P240-A-1x100GBase LAN-CFP
LPU 15     CR53LPUFC0          03846110DB000532
  PIC 0    CR53C2CF0           030KBB10E8000067                2-Port Channelized OC-3c/STM-1c P
                                                               OS-SFP
LPU 2      CR57LPUF240A00      210305469010K7000259            LPUF-240
  PIC 0    CR57E1NCB1          030QDFW0K7000001                P240-A-1x100GBase LAN-CFP


5.120
          1         2         3         4         5         6
0123456789012345678901234567890123456789012345678901234567890123456789
LPU 5    CX61LPUK21   210305253310F4000012    LPUF-21-A
  PIC 0  CR52L1XXB0   030KKP10F2000168
  PIC 1  CR52L1XXB0   030KKP10F2000090
LPU 6    CX61EFFFG0   2103051580109A000176
  PIC 0  CR52EFFFT0   030GES109A000033
LPU 8    CR52EFGEG0   030FNF1086000201
  PIC 0  CR52EFGE     0362291086000263
MPU 9    CR52SRUA1    030GED1086000058
MPU 10   CR52SRUA1    030GED1086000052
SFU 11   CR52SFUD0    030AUG1085001920
SFU 12   CR52SFUD0    030AUG1085001924

8.100/8.150
          1         2         3         4         5         6
0123456789012345678901234567890123456789012345678901234567890123456789
LPU 1           CR57LPUF240A00  210305469010H7000016    LPUF-240
  PIC 0         CR57LBXF1       030QDDW0H6000141        P240-12x10GBase LAN/WAN-SFP+ -A
LPU 8           CR57LPUF120A03  210305518910HA000016    LPUF-51-E
  PIC 0         CR57EFGFB0      030PMA10H9001062        P51-24xFE/GE-SFP-A
MPU 9           CR57SRUA480A81  210305725710H7000101    SRUA8
MPU 10          CR57SRUA480A81  210305725710H7000103    SRUA8
SFU 11          CR57SFU480C00   210305609510H7000122    SFUI-480-C
SFU 12          CR57SFU480C00   210305609510H7000121    SFUI-480-C
PWR 17

8.180
          1         2         3         4         5         6         7         8
012345678901234567890123456789012345678901234567890123456789012345678901234567890123456789
LPU 1           CR58LPUF2TA00                            210305770210JB000007    NE9000 LPUF-2T
  PIC 0         CR58EANBC0                               032VDR10JA000001        NP-10x100GBase-QSFP28
LPU 2           CR58LPUF2TA00                            210305770210JC000010    NE9000 LPUF-2T
  PIC 0         CR58EANBC0                               032VDR10JB000016        NP-10x100GBase-QSFP28
LPU 3           CR58LPUF480C01                           210305753210JB000005    NE9000 LPUF-480
  PIC 0         CR58LFXF0                                032JVE10JA000010        NP-24x1000M/10GBase LAN/WAN-SFP+
  PIC 1         CR58LFXF0                                032JVE10JA000004        NP-24x1000M/10GBase LAN/WAN-SFP+
LPU 4           CR58LPUF480C01                           210305753210JB000007    NE9000 LPUF-480
  PIC 0         CR58LFXF0                                032JVE10JA000001        NP-24x1000M/10GBase LAN/WAN-SFP+
MPU 9           CR58MPUP10                               210305753110JB000077    NE9000 MPUP1


                    */

                    nc1 = -1;
                    nc2 = -2;

                    NodeSlotData current = null;
                    StringBuilder desc = new StringBuilder();

                    foreach (string line in lines)
                    {
                        if (line.Length > 0)
                        {
                            string[] t;

                            if (nodeVersion == "5.120")
                            {
                                t = line.Tokenize(4, StringOperations.None, 0, 9, 22, 46);
                            }
                            else if (nodeVersion == "8.100" || nodeVersion == "8.150")
                            {
                                t = line.Tokenize(4, StringOperations.None, 0, 16, 32, 56);
                            }
                            else if (nodeVersion == "8.180")
                            {
                                t = line.Tokenize(4, StringOperations.None, 0, 16, 57, 81);
                            }
                            else
                            {
                                t = line.Tokenize(4, StringOperations.None, 0, 11, 31, 63);
                            }
                            string[] tt = t.Trim();

                            if (t.Length == 4)
                            {
                                if (tt[0] == "")
                                {
                                    desc.Append(t[3]);

                                    if (current != null)
                                    {
                                        current.Type = desc.ToString().Trim();
                                    }
                                }
                                else
                                {
                                    if (tt[0].StartsWith("LPU"))
                                    {
                                        if (!int.TryParse(tt[0].Tokenize(2, StringOperations.None)[1], out nc1)) nc1 = -1;
                                        nc2 = -1;

                                        desc.Clear();
                                    }
                                    else if (tt[0].StartsWith("PIC"))
                                    {
                                        if (!int.TryParse(tt[0].Tokenize(2, StringOperations.None)[1], out nc2)) nc2 = -1;

                                        desc.Clear();
                                    }
                                    else
                                    {
                                        nc1 = -1;
                                    }

                                    if (nc1 > -1)
                                    {
                                        string key = nc1 + "-" + (nc2 > -1 ? nc2 + "-" : "-");

                                        if (slotlive.ContainsKey(key))
                                        {
                                            current = slotlive[key];
                                        }
                                        else
                                        {
                                            current = new NodeSlotData();

                                            current.Slot1 = nc1;
                                            current.Slot2 = nc2.Nullable(-1);

                                            slotlive.Add(key, current);
                                        }

                                        desc.Append(t[3]);

                                        current.Board = tt[1];
                                        current.Serial = tt[2];

                                        string type = desc.ToString().Trim();

                                        if (string.IsNullOrEmpty(type))
                                        {
                                            current.Type = null;
                                            current.Enable = true;
                                            current.Protocol = true;
                                        }
                                        else
                                        {
                                            current.Type = type;
                                            current.Enable = true;
                                            current.Protocol = true;
                                        }

                                    }

                                }
                            }
                        }
                    }


                    if (version == null)
                    {
                        // yes probably, those nodes that cant pipe display version :( -> QUIDWAY?
                        if (Request("display version", out lines, probe)) return probe;

                        foreach (string line in lines)
                        {
                            if (line.ToLower().StartsWith("vrp (r) software"))
                                version = line.Substring(26, line.IndexOf(' ', 26) - 26).Trim();
                            if (line.StartsWith("Quidway "))
                                model = "QUIDWAY " + line.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries)[1];
                            if (version != null && model != null) break;
                        }
                    }

                    #endregion
                }
                else if (nodeManufacture == cso)
                {
                    #region cso
                    if (Request("show version | in IOS", out lines, probe)) return probe;

                    string sl = string.Join("", lines.ToArray());

                    if (sl.IndexOf("Cisco IOS XR Software") > -1)
                    {
                        // ASR
                        model = "ASR";
                        version = "XR";
                        int iv = sl.IndexOf("Version");
                        if (iv > -1)
                        {
                            string ivonw = sl.Substring(iv + 7).Trim();
                            if (ivonw.Length > 0)
                            {
                                iv = ivonw.IndexOf('[');
                                if (iv == -1) iv = ivonw.IndexOf(',');
                                if (iv == -1) iv = ivonw.IndexOf(' ');
                                if (iv > -1)
                                {
                                    subVersion = ivonw.Substring(0, iv);
                                }
                            }
                        }
                    }
                    else
                    {
                        // IOS
                        version = "IOS";

                        // software
                        int iver = sl.IndexOf("Version ");
                        if (iver > -1)
                        {
                            string imod = sl.Substring(iver + 8, sl.IndexOf(',', iver + 8) - (iver + 8));
                            subVersion = imod;
                        }

                        // model
                        if (Request("show version | in bytes of memory", out lines, probe)) return probe;

                        sl = string.Join("", lines.ToArray());
                        string slo = sl.ToLower();

                        int cisc = slo.IndexOf("cisco ");
                        if (cisc > -1)
                        {
                            string imod = sl.Substring(cisc + 6, sl.IndexOf(' ', cisc + 6) - (cisc + 6));

                            int iod = 0;
                            foreach (char imodc in imod)
                            {
                                if (char.IsDigit(imodc)) { break; }
                                iod++;
                            }
                            model = imod.Substring(iod);
                        }
                    }
                    #endregion
                }
                else if (nodeManufacture == jun)
                {
                    #region jun
                    //Model: mx960
                    //Junos: 16.1R4-S1.3

                    if (Request("show version | match \"Junos:|JUNOS Base OS boot|Model\"", out lines, probe)) return probe;

                    foreach (string line in lines)
                    {
                        if (line.StartsWith("JUNOS Base OS boot"))
                        {
                            string[] linex = line.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                            if (linex.Length >= 2) version = linex[1];
                        }
                        else if (line.StartsWith("Junos: "))
                        {
                            version = line.Substring(7).Trim();
                        }
                        else if (line.IndexOf("Model: ") > -1)
                        {
                            model = line.Substring(line.IndexOf("Model: ") + 7).Trim();
                        }
                    }
                    #endregion
                }

                if (nodeModel == null && model != null)
                {
                    nodeModel = model;
                    Update(UpdateTypes.Model, model);
                    Event("Model discovered: " + model);
                }
                else if (nodeModel != null && model != nodeModel)
                {
                    nodeModel = model;
                    Update(UpdateTypes.Model, model);
                    Event("Model changed: " + model);
                }
                if (version != nodeVersion)
                {
                    nodeVersion = version;
                    Update(UpdateTypes.Version, version);
                    Event("Version updated: " + version);
                }
                if (nodeVersion == null)
                {
                    throw new Exception("Cant determined node ver" +
                        "sion");
                }
                if (subVersion != nodeSubVersion)
                {
                    nodeSubVersion = subVersion;
                    Update(UpdateTypes.SubVersion, subVersion);
                    Event("SubVersion updated: " + subVersion);
                }

                #region Check

                foreach (KeyValuePair<string, NodeSlotData> pair in slotlive)
                {
                    NodeSlotData li = pair.Value;

                    if (!slotdb.ContainsKey(pair.Key))
                    {
                        Event("Slot ADD: " + pair.Key);

                        li.Id = Database2.ID();
                        slotinsert.Add(li);
                    }
                    else
                    {
                        Row2 db = slotdb[pair.Key];

                        if (li.Update(out NodeSlotData u, db))
                        {
                            Event("Slot UPDATE: " + pair.Key);
                            slotupdate.Add(u);
                        }
                    }
                }

                #endregion

                #region Execute

                // ADD
                batch.Begin();
                foreach (NodeSlotData s in slotinsert)
                {
                    Insert insert = j.Insert("NodeSlot");
                    insert.Value("NC_ID", s.Id);
                    insert.Value("NC_NO", nodeID);
                    insert.Value("NC_Slot1", s.Slot1);
                    insert.Value("NC_Slot2", s.Slot2);
                    insert.Value("NC_Slot3", s.Slot3);
                    insert.Value("NC_Type", s.Type);
                    insert.Value("NC_Board", s.Board);
                    insert.Value("NC_Info", s.Info);
                    insert.Value("NC_Serial", s.Serial);
                    insert.Value("NC_LastStartUp", s.LastStartUp);
                    insert.Value("NC_Enable", s.Enable);
                    insert.Value("NC_Protocol", s.Protocol);
                    insert.Value("NC_Capacity", s.Capacity);

                    batch.Add(insert);
                }

                result = batch.Commit();
                if (!result) return DatabaseFailure(probe);
                Event(result, EventActions.Add, EventElements.Slot, false);

                // UPDATE
                batch.Begin();

                foreach (NodeSlotData s in slotupdate)
                {
                    Update update = j.Update("NodeSlot");

                    update.Set("NC_Type", s.Type, s.IsUpdated("Type"));
                    update.Set("NC_Board", s.Board, s.IsUpdated("Board"));
                    update.Set("NC_Info", s.Info, s.IsUpdated("Info"));
                    update.Set("NC_Serial", s.Serial, s.IsUpdated("Serial"));
                    update.Set("NC_LastStartUp", s.LastStartUp, s.IsUpdated("LastStartUp"));
                    update.Set("NC_Enable", s.Enable, s.IsUpdated("Enable"));
                    update.Set("NC_Protocol", s.Protocol, s.IsUpdated("Protocol"));
                    update.Set("NC_Capacity", s.Capacity, s.IsUpdated("Capacity"));
                    update.Where("NC_ID", s.Id);

                    batch.Add(update);
                }
                result = batch.Commit();
                if (!result) return DatabaseFailure(probe);
                Event(result, EventActions.Update, EventElements.Slot, false);

                // SDP DELETE
                batch.Begin();
                foreach (KeyValuePair<string, Row2> pair in slotdb)
                {
                    if (!slotlive.ContainsKey(pair.Key))
                    {
                        Event("Slot DELETE: " + pair.Key);
                        batch.Add("delete from NodeSlot where NC_ID = {0}", pair.Value["NC_ID"].ToString());
                    }
                }
                result = batch.Commit();
                if (!result) return DatabaseFailure(probe);
                Event(result, EventActions.Delete, EventElements.Slot, false);

                #endregion

                Summary("CHASSIS_PROCESS_SLOT_MAX", maxSlot);
                Summary("CHASSIS_PROCESS_SLOT_COUNT", curSlot);
            }

            Event("Version: " + nodeVersion + ((nodeSubVersion != null) ? ":" + nodeSubVersion : ""));

            #endregion

            #region LAST CONFIGURATION

            Event("Checking Last Configuration");

            bool configurationHasChanged = false;
            bool lastConfLiveRetrieved = false;
            DateTime lastConfLive = DateTime.MinValue;

            if (nodeManufacture == alu)
            {
                #region alu
                if (Request("show system information | match \"Time Last Modified\"", out lines, probe)) return probe;

                bool lastModified = false;

                foreach (string line in lines)
                {
                    if (line.StartsWith("Time Last Modified"))
                    {
                        lastModified = true;
                        //Time Last Modified     : 2
                        //Time Last Modified        : 2016/11/07 17:35:36
                        //01234567890123456789012345
                        string datetime = line.Split(new char[] { ':' }, 2)[1].Trim();
                        if (datetime == "N/A")
                        {
                            lastConfLive = new DateTime(2000, 1, 1, 0, 0, 0);
                            lastConfLiveRetrieved = true;
                            break;
                        }
                        else
                        {
                            lastConfLive = DateTime.Parse(datetime);
                            lastConfLive = new DateTime(
                                lastConfLive.Ticks - (lastConfLive.Ticks % TimeSpan.TicksPerSecond),
                                lastConfLive.Kind
                                );
                            lastConfLiveRetrieved = true;
                            break;
                        }
                    }
                }

                if (lastModified == false)
                {
                    if (Request("show system information | match \"Time Last Saved\"", out lines, probe)) return probe;

                    foreach (string line in lines)
                    {
                        if (line.StartsWith("Time Last Saved"))
                        {
                            lastModified = true;
                            //Time Last Saved        : 2015/01/13 01:13:56
                            //Time Last Saved           : 2017/04/10 12:06:46
                            //01234567890123456789012345
                            string datetime = line.Split(new char[] { ':' }, 2)[1].Trim();
                            if (datetime == "N/A")
                            {
                                lastConfLive = new DateTime(2000, 1, 1, 0, 0, 0);
                                lastConfLiveRetrieved = true;
                                break;
                            }
                            else if (DateTime.TryParse(datetime, out lastConfLive))
                            {
                                lastConfLive = new DateTime(
                                    lastConfLive.Ticks - (lastConfLive.Ticks % TimeSpan.TicksPerSecond),
                                    lastConfLive.Kind
                                    );
                                lastConfLiveRetrieved = true;
                                break;
                            }
                        }
                    }
                }
                #endregion
            }
            else if (nodeManufacture == hwe)
            {
                #region hwe

                if (nodeVersion.StartsWith("8"))
                {
                    if (Request("display configuration commit list 1", out lines, probe)) return probe;

                    foreach (string line in lines)
                    {
                        if (line.Length > 0 && char.IsDigit(line[0]))
                        {
                            //1    1000000583    -                    850106          2016-05-04 16:32:38+07:00
                            //0123456789012345678901234567890123456789012345678901234567890123456789
                            //          1         2         3         4         5         6
                            string[] tokens = line.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);

                            if (tokens.Length == 6)
                            {
                                string ps2 = tokens[4] + " " + tokens[5];
                                string[] tokens2 = ps2.Split(new char[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
                                string ps = tokens2[0];

                                if (DateTime.TryParseExact(ps, "yyyy-MM-dd HH:mm:ss", null, DateTimeStyles.None, out lastConfLive)) lastConfLiveRetrieved = true;
                                break;
                            }
                        }
                    }
                }
                else if (nodeVersion == "5.70")
                {
                    if (Request("display saved-configuration time", out lines, probe)) return probe;

                    foreach (string line in lines)
                    {
                        //02:38:04 WIB 2017/02/13
                        string[] tokens = line.Split(StringSplitTypes.Space);
                        if (tokens.Length == 3)
                        {
                            string date = string.Join(" ", tokens[0], tokens[2]);
                            if (DateTime.TryParseExact(date, "HH:mm:ss yyyy/MM/dd", null, DateTimeStyles.None, out lastConfLive))
                            {
                                lastConfLiveRetrieved = true;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    if (Request("display changed-configuration time", out lines, probe)) return probe;

                    StringBuilder datesection = null;
                    foreach (string line in lines)
                    {
                        //The time when system configuration has been changed lastly is:May 06 2016 03:28:39
                        if (line.StartsWith("The time"))
                        {
                            int colon = line.IndexOf(':');
                            if (colon < line.Length - 1)
                            {
                                datesection = new StringBuilder();
                                datesection.Append(line.Substring(colon + 1));
                            }
                        }
                        else if (datesection != null)
                        {
                            if (!line.StartsWith(terminal))
                                datesection.Append(line);
                        }
                    }

                    if (datesection != null)
                    {
                        if (DateTime.TryParseExact(datesection.ToString(), "MMM dd yyyy HH:mm:ss", null, DateTimeStyles.None, out lastConfLive)) lastConfLiveRetrieved = true;
                    }
                }
                #endregion
            }
            else if (nodeManufacture == cso)
            {
                #region cso
                if (nodeVersion == xr)
                {
                    #region xr
                    if (Request("show configuration history commit last 1 | in commit", out lines, probe)) return probe;

                    foreach (string line in lines)
                    {
                        if (line.Length > 0 && line[0] == '1')
                        {
                            //1     commit     id 1000000807                  Fri
                            //01234567890123456789012345678901234567890123456789
                            string dateparts = line.Substring(48).Trim();

                            if (dateparts != null)
                            {
                                //Fri Jan 16 11:18:46 2015
                                string[] dates = dateparts.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);

                                if (DateTime.TryParse(dates[1] + " " + dates[2] + " " + dates[4] + " " + dates[3], out lastConfLive))
                                {
                                    lastConfLive = new DateTime(
                                        lastConfLive.Ticks - (lastConfLive.Ticks % TimeSpan.TicksPerSecond),
                                        lastConfLive.Kind
                                        );
                                    lastConfLiveRetrieved = true;
                                    break;
                                }
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region ios

                    // because so many differences between IOS version, we might try every possible command
                    bool passed = false;

                    // most of ios version will work this way
                    if (Request("show configuration id detail", out lines, probe)) return probe;

                    foreach (string line in lines)
                    {
                        if (line.StartsWith("Last change time"))
                        {
                            //Last change time             : 2015-01-17T02:33:01.553Z
                            //01234567890123456789012345678901234567890123456789
                            //          1         2         3
                            string dateparts = line.Substring(31).Trim();
                            if (dateparts != null)
                            {
                                if (DateTime.TryParse(dateparts, out lastConfLive))
                                {
                                    Event("Using configuration id");
                                    passed = true;
                                    lastConfLive = new DateTime(
                                        lastConfLive.Ticks - (lastConfLive.Ticks % TimeSpan.TicksPerSecond),
                                        lastConfLive.Kind
                                        );
                                    lastConfLiveRetrieved = true;
                                    break;
                                }
                            }
                            break;
                        }
                    }

                    if (passed == false)
                    {
                        // using xr-like command history
                        //show configuration history
                        if (Request("show configuration history", out lines, probe)) return probe;

                        string lastline = null;
                        foreach (string line in lines)
                        {
                            if (line.IndexOf("Invalid input detected") > -1) break;
                            else if (line == "") break;
                            else if (char.IsDigit(line[0])) lastline = line;
                        }

                        if (lastline != null)
                        {
                            string[] linex = lastline.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);
                            //5    23:33:21 WIB Tue Mar 1 2016User : ffm-myarham
                            string datetime = linex[1];
                            string datemonth = linex[4].ToUpper();
                            string datedate = linex[5];
                            string dateyear = linex[6];
                            if (dateyear.Length > 4) dateyear = dateyear.Substring(0, 4);

                            string datestr = datemonth + " " + datedate + " " + dateyear + " " + datetime;

                            DateTime parsedDT = DateTime.MinValue;
                            if (DateTime.TryParse(datestr, out parsedDT))
                            {
                                Event("Using configuration history");
                                passed = true;
                                lastConfLive = parsedDT;
                                lastConfLiveRetrieved = true;
                            }
                        }
                    }

                    if (passed == false)
                    {
                        // and here we are, using forbidden command ever
                        if (Request("show log | in CONFIG_I", out lines, probe)) return probe;

                        string lastline = null;
                        foreach (string line in lines)
                        {
                            if (line.IndexOf("*") > -1) lastline = line;
                        }

                        if (lastline != null)
                        {
                            lastline = lastline.Substring(lastline.IndexOf("*"));

                            string[] lastlines = lastline.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);

                            string datemonth = lastlines[0].TrimStart(new char[] { '*' }).ToUpper();
                            string datedate = lastlines[1];
                            string datetime = lastlines[2];

                            string datestr = datemonth + " " + datedate + " " + DateTime.Now.Year + " " + datetime;

                            DateTime parsedDT = DateTime.MinValue;
                            if (DateTime.TryParse(datestr, out parsedDT))
                            {
                                Event("Using log config");
                                passed = true;

                                if (parsedDT > DateTime.Now)
                                {
                                    // probably year is wrong, so use last year
                                    string datestrrev = datemonth + " " + datedate + " " + (DateTime.Now.Year - 1) + " " + datetime;
                                    parsedDT = DateTime.Parse(datestrrev);
                                }

                                lastConfLive = parsedDT;
                                lastConfLiveRetrieved = true;
                            }
                        }
                    }

                    if (passed == false)
                    {
                        // and... if everything fail, we will use this slowlest command ever
                        if (Request("show run | in Last config", out lines, probe)) return probe;

                        foreach (string line in lines)
                        {
                            //! Last configuration change at 1
                            //01234567890123456789012345678901
                            if (line.StartsWith("! Last config"))
                            {
                                string eline = line.Substring(31);
                                string[] linex = eline.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);
                                //17:27:19 WIB Tue Mar 1 2016 
                                //0        1   2   3   4 5
                                string datetime = linex[0];
                                string datemonth = linex[3].ToUpper();
                                string datedate = linex[4];
                                string dateyear = linex[5];
                                string datestr = datemonth + " " + datedate + " " + dateyear + " " + datetime;

                                DateTime parsedDT = DateTime.MinValue;
                                if (DateTime.TryParse(datestr, out parsedDT))
                                {
                                    Event("Using running configuration");
                                    passed = true;
                                    lastConfLive = parsedDT;
                                    lastConfLiveRetrieved = true;
                                }
                                break;
                            }
                        }
                    }

                    #endregion
                }
                #endregion
            }
            else if (nodeManufacture == jun)
            {
                #region jun
                foreach (string line in junShowSystemUptimeLines)
                {
                    if (line.StartsWith("Last configured: "))
                    {
                        //Last configured: 2015-01-20 09:53:54
                        //0123456789012345678901234567890123456789
                        string ps = line.Substring(17, 19);
                        if (DateTime.TryParseExact(ps, "yyyy-MM-dd HH:mm:ss", null, DateTimeStyles.None, out lastConfLive)) { lastConfLiveRetrieved = true; }
                        break;
                    }
                }
                #endregion
            }

            DateTime lastConfDB = row["NO_LastConfiguration"].ToDateTime();

            if (!lastConfLiveRetrieved)
            {
                throw new Exception("Failure on last configuration retrieval");
            }

            lastConfLive = lastConfLive - nodeTimeOffset;
            TimeSpan difference = lastConfLive - lastConfDB;

            if (Math.Abs(difference.TotalSeconds) > 1)
            {
                Event("Saved: " + lastConfDB.ToString("yyyy/MM/dd HH:mm:ss") + " UTC");
                Event("Actual: " + lastConfLive.ToString("yyyy/MM/dd HH:mm:ss") + " UTC");
                Event("Configuration has changed!");
                Update(UpdateTypes.LastConfiguration, lastConfLive);
                configurationHasChanged = true;
            }
            else
            {
                Event("Saved: " + lastConfLive.ToString("yyyy/MM/dd HH:mm:ss") + " UTC");
                Event("Actual: " + lastConfLive.ToString("yyyy/MM/dd HH:mm:ss") + " UTC");
                Event("Configuration is up to date");
            }

            #endregion

            #region CPU / MEMORY

            Event("Checking CPU / Memory"); //mengecek memory dan CPU 

            int cpu = -1;
            int mtotal = -1;
            int mused = -1;

            #region Live

            if (nodeManufacture == cso)
            {
                #region cso
                if (Request("show processes cpu | in CPU", out lines, probe)) return probe;

                foreach (string line in lines)
                {
                    if (line.StartsWith("CPU "))
                    {
                        int oid = line.Trim().LastIndexOf(' ');
                        if (oid > -1)
                        {
                            string okx = line.Substring(oid + 1).Trim();
                            string perc = okx.Substring(0, okx.IndexOf('%'));
                            if (!int.TryParse(perc, out cpu)) cpu = -1;
                        }
                    }
                }

                if (nodeVersion == xr)
                {
                    //show memory summary | in Physical Memory
                    if (Request("show memory summary | in Physical Memory", out lines, probe)) return probe;

                    foreach (string line in lines)
                    {
                        string lint = line.Trim();
                        if (lint.StartsWith("Physical Memory: "))
                        {
                            string[] linex = lint.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);

                            string ltot = linex[2];
                            string lfree = linex[4];

                            ltot = ltot.Substring(0, ltot.Length - 1);
                            lfree = lfree.Substring(1, lfree.Length - 2);

                            int ltots;
                            if (int.TryParse(ltot, out ltots))
                                mtotal = ltots * 1000;
                            else
                                mtotal = -1;

                            if (mtotal > -1)
                            {
                                int lfrees;
                                if (int.TryParse(lfree, out lfrees))
                                    mused = mtotal - (lfrees * 1000);
                                else
                                    mused = -1;
                            }

                        }

                    }
                }
                else
                {
                    //show process memory  | in Processor Pool
                    if (Request("show process memory | in Total:", out lines, probe)) return probe;

                    foreach (string line in lines)
                    {
                        string lint = line.Trim();
                        if (lint.StartsWith("Processor Pool"))
                        {
                            string[] linex = lint.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);

                            string ltot = linex[3];
                            string luse = linex[5];

                            double ltots;
                            if (!double.TryParse(ltot, out ltots)) ltots = -1;

                            double luses;
                            if (!double.TryParse(luse, out luses)) luses = -1;

                            if (ltots >= 0) mtotal = (int)Math.Round(ltots / 1000);
                            if (luses >= 0) mused = (int)Math.Round(luses / 1000);
                            break;
                        }
                        else if (lint.StartsWith("Total:"))
                        {
                            string[] linex = lint.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                            string ltot = linex[0].Trim().Split(StringSplitTypes.Space)[1];
                            string luse = linex[1].Trim().Split(StringSplitTypes.Space)[1];

                            double ltots;
                            if (!double.TryParse(ltot, out ltots)) ltots = -1;

                            double luses;
                            if (!double.TryParse(luse, out luses)) luses = -1;

                            if (ltots >= 0) mtotal = (int)Math.Round(ltots / 1000);
                            if (luses >= 0) mused = (int)Math.Round(luses / 1000);
                            break;
                        }
                    }
                }

                #endregion
            }
            else if (nodeManufacture == alu)
            {
                #region alu
                if (Request("show system cpu | match \"Busiest Core\"", out lines, probe)) return probe;

                foreach (string line in lines)
                {
                    string lint = line.Trim();

                    if (lint.StartsWith("Busiest Core "))
                    {
                        int oid = lint.LastIndexOf(' ');
                        if (oid > -1)
                        {
                            string okx = lint.Substring(oid + 1).Trim();
                            string perc = okx.Substring(0, okx.IndexOf('%'));

                            float cpuf;
                            if (!float.TryParse(perc, out cpuf)) cpuf = -1;

                            if (cpuf == -1) cpu = -1;
                            else cpu = (int)Math.Round(cpuf);
                        }
                    }
                }

                //show system memory-pools | match bytes
                if (Request("show system memory-pools | match bytes", out lines, probe)) return probe;

                foreach (string line in lines)
                {
                    string lint = line.Trim();

                    if (lint.StartsWith("Total In Use") || lint.StartsWith("Available Memory"))
                    {
                        string[] linex = lint.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        if (linex.Length >= 2)
                        {
                            string ibytes = linex[1].Trim();
                            ibytes = ibytes.Substring(0, ibytes.IndexOf(' '));
                            string[] ibytesx = ibytes.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            string cbytes = string.Join("", ibytesx);

                            double dbytes;
                            if (!double.TryParse(cbytes, out dbytes)) dbytes = -1;

                            if (lint.StartsWith("Total In Use") && dbytes > -1)
                            {
                                mused = (int)Math.Round(dbytes / 1000);
                            }
                            else if (mused > -1 && lint.StartsWith("Available Memory") && dbytes > -1)
                            {
                                mtotal = mused + (int)Math.Round(dbytes / 1000);
                            }
                        }
                    }
                }

                #endregion
            }
            else if (nodeManufacture == hwe)
            {
                #region hwe
                if (Request("display cpu-usage", out lines, probe)) return probe;

                foreach (string line in lines)
                {
                    string lint = line.Trim();

                    if (nodeVersion.StartsWith("8"))
                    {
                        //System cpu use rate is : 10%
                        //012345678901234567890123456789
                        if (lint.StartsWith("System cpu use rate is"))
                        {
                            string okx = line.Substring(25).Trim();
                            string perc = okx.Substring(0, okx.IndexOf('%'));
                            if (!int.TryParse(perc, out cpu)) cpu = -1;
                            break;
                        }
                    }
                    else
                    {
                        if (lint.StartsWith("CPU utilization for"))
                        {
                            int oid = lint.LastIndexOf(' ');
                            if (oid > -1)
                            {
                                string okx = line.Substring(oid + 1).Trim();
                                string perc = okx.Substring(0, okx.IndexOf('%'));
                                if (!int.TryParse(perc, out cpu)) cpu = -1;
                            }
                            break;
                        }
                    }
                }

                if (Request("display memory-usage", out lines, probe)) return probe;

                foreach (string line in lines)
                {
                    string lint = line.Trim();

                    if (lint.StartsWith("System Total") || lint.StartsWith("Total Memory"))
                    {
                        string[] linex = lint.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                        if (linex.Length >= 2)
                        {
                            string odata = linex[1].Trim();
                            odata = odata.Substring(0, odata.IndexOf(' '));

                            double dbytes;
                            if (!double.TryParse(odata, out dbytes)) dbytes = -1;

                            if (lint.StartsWith("System Total"))
                            {
                                if (nodeVersion == "8.80") // already in Kbytes
                                    mtotal = (int)Math.Round(dbytes);
                                else
                                    mtotal = (int)Math.Round(dbytes / 1000);
                            }
                            else if (lint.StartsWith("Total Memory"))
                            {
                                if (nodeVersion == "8.80") // already in Kbytes
                                    mused = (int)Math.Round(dbytes);
                                else
                                    mused = (int)Math.Round(dbytes / 1000);
                            }
                        }
                    }
                }
                #endregion
            }
            else if (nodeManufacture == jun)
            {
                #region jun

                //show chassis routing-engine | match Idle
                if (Request("show chassis routing-engine | match Idle", out lines, probe)) return probe;

                foreach (string line in lines)
                {
                    string lint = line.Trim();

                    if (lint.StartsWith("Idle"))
                    {
                        string[] linex = lint.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);
                        if (linex.Length == 3)
                        {
                            string perc = linex[1];
                            int idlecpu;
                            if (int.TryParse(perc, out idlecpu))
                            {
                                cpu = 100 - idlecpu;
                            }
                            else
                            {
                                cpu = -1;
                            }
                        }
                        break;
                    }
                }

                //show task memory
                if (Request("show task memory", out lines, probe)) return probe;

                foreach (string line in lines)
                {
                    string lint = line.Trim();

                    if (lint.StartsWith("Currently In Use:") || lint.StartsWith("Available:"))
                    {
                        string[] linex = lint.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                        if (linex.Length >= 2)
                        {
                            string rightside = linex[1].Trim();

                            linex = rightside.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);

                            if (linex.Length >= 1)
                            {
                                string odata = linex[0].Trim();
                                int dbytes;
                                if (!int.TryParse(odata, out dbytes)) dbytes = -1;

                                if (lint.StartsWith("Currently In Use:"))
                                {
                                    mused = dbytes;
                                }
                                else if (mused > -1 && lint.StartsWith("Available:"))
                                {
                                    mtotal = mused + dbytes;
                                }
                            }
                        }
                    }
                }

                #endregion
            }

            #endregion

            #region Check and Update

            if (cpu > -1 && cpu < 100)
            {
                Event("CPU = " + cpu + "%");
                Summary("CPU", cpu, false);
            }
            else
            {
                Event("CPU load is unavailable (" + cpu + ")");
            }
            if (mtotal > -1)
            {
                Event("Memory total = " + mtotal + "KB");
                Summary("MEMORY_TOTAL", mtotal, false);
            }
            if (mused > -1)
            {
                Event("Memory used = " + mused + "KB");
                Summary("MEMORY_USED", mused, false);
            }

            #endregion

            #endregion

            if (configurationHasChanged || fromDeepNodeQueue)
            {
                continueProcess = true;
            }
            else if (necrow.TestNode == nodeID && necrow.TestProbeType == ProbeTypes.Deep)
            {
                Event("Deep Test Node, continuing process");
                continueProcess = true;
            }
            else if (updatingNecrow)
            {
                continueProcess = true;
            }
            else
            {
                SaveExit();
            }

            return probe;
        }

        private void UpdateInfo(StringBuilder updateInfo, string title, string info)
        {
            if (updateInfo.Length > 0) updateInfo.Append(", ");
            if (info != null)
            {
                if (info.Length > 0) info = " " + info;
            }
            else info = "";
            updateInfo.Append(title + info);
        }

        private void UpdateInfo(StringBuilder updateInfo, string title, string from, string to)
        {
            if (updateInfo.Length > 0) updateInfo.Append(", ");
            if (from != null)
            {
                if (from.Length > 10) from = from.Substring(0, 10) + "...";
                else if (from.Length == 0) from = "<empty>";
            }
            else from = "NULL";
            if (to != null)
            {
                if (to.Length > 10) to = to.Substring(0, 10) + "...";
                else if (to.Length == 0) to = "<empty>";
            }
            else to = "NULL";
            updateInfo.Append(title + " " + from + " -> " + to);
        }

        private void UpdateInfo(StringBuilder updateInfo, string title, string from, string to, bool changeInfo)
        {
            if (changeInfo)
            {
                if (updateInfo.Length > 0) updateInfo.Append(", ");
                if (from == null)
                {
                    updateInfo.Append(title + " assigned");
                }
                else if (to == null)
                {
                    updateInfo.Append(title + " removed");
                }
                else
                {
                    updateInfo.Append(title + " changed");
                }
            }
            else
                UpdateInfo(updateInfo, title, from, to);
        }

        private void ServiceImmediateDiscovery<T>(SortedDictionary<string, T> liveEntries) where T : ServiceBaseToDatabase
        {
            Batch batch = j.Batch();
            Result2 result;

            List<ServiceToDatabase> serviceinsert = new List<ServiceToDatabase>();
            List<ServiceToDatabase> serviceupdate = new List<ServiceToDatabase>();

            foreach (KeyValuePair<string, T> pair in liveEntries)
            {
                if (pair.Value is ServiceBaseToDatabase)
                {
                    ServiceBaseToDatabase li = pair.Value as ServiceBaseToDatabase;

                    if (li.Description != null)
                    {
                        Service service = Service.Parse(li.Description);

                        string vid = service.VID;

                        if (vid != null)
                        {
                            string stype = service.ServiceType;

                            string s_type = null;
                            if (stype == "VPNIP") s_type = "VP";
                            else if (stype == "TRANS") s_type = "TA";
                            else if (stype == "ASTINET") s_type = "AS";
                            else if (stype == "ASTINETBB") s_type = "AB";
                            else if (stype == "VPNINSTAN") s_type = "VI";
                            else if (stype == "IPTRANSIT") s_type = "IT";

                            else if (stype == "METRO") s_type = "ME";
                            else if (stype == "TELKOMSELSITES") s_type = "TS";

                            string s_id = null;

                            #region se

                            li.ServiceVID = vid;

                            Result2 rsi = j.Query("select * from ServiceImmediate where SI_VID = {0} order by SI_SE_Check desc, SI_SE desc, SI_ID desc", vid);

                            if (rsi > 0)
                            {
                                s_id = rsi[0]["SI_ID"];

                                if (s_type != null)
                                {
                                    if (rsi[0]["SI_Type"] == null || (s_type != "ME" && s_type != "TS"))
                                    {
                                        // update if assign new, OR s_type is not ME or TS
                                        Event("Service Immediate UPDATE: " + vid + " " + s_type);
                                        ServiceToDatabase c = new ServiceToDatabase
                                        {
                                            Id = s_id,
                                            Type = s_type
                                        };
                                        serviceupdate.Add(c);
                                    }
                                }
                            }
                            else
                            {
                                s_id = Database2.ID();

                                Event("Service ADD: " + vid);
                                ServiceToDatabase c = new ServiceToDatabase
                                {
                                    Id = s_id,
                                    VID = vid,
                                    Type = s_type
                                };
                                serviceinsert.Add(c);
                            }

                            li.ServiceImmediateID = s_id;
                        }

                        #endregion
                    }
                }
            }
            
            // SERVICE ADD
            batch.Begin();
            foreach (ServiceToDatabase s in serviceinsert)
            {
                Insert insert = j.Insert("ServiceImmediate");
                insert.Value("SI_ID", s.Id);
                insert.Value("SI_VID", s.VID);
                insert.Value("SI_Type", s.Type);
                batch.Add(insert);
            }
            result = batch.Commit();
            Event(result, EventActions.Add, EventElements.Service, false);

            // SERVICE UPDATE
            batch.Begin();
            foreach (ServiceToDatabase s in serviceupdate)
            {
                Update update = j.Update("ServiceImmediate");
                update.Set("SI_Type", s.Type);
                update.Where("SI_ID", s.Id);
                batch.Add(update);
            }
            result = batch.Commit();
            Event(result, EventActions.Update, EventElements.Service, false);
        }

        public string FindNeighborPart(string description, string name)
        {
            if (description == null || name == null) return null;

            int find = description.IndexOf(name);
            int findLength = name.Length;

            if (find == -1)
            {
                if (name.StartsWith("ME-"))
                {
                    // coba pake ME1-
                    string nameAlternate = name.Replace("ME-", "ME1-");
                    find = description.IndexOf(nameAlternate);
                    if (find > -1) findLength = nameAlternate.Length;
                }
                else if (name.StartsWith("PE-"))
                {
                    // coba pake PE1-
                    string nameAlternate = name.Replace("PE-", "PE1-");
                    find = description.IndexOf(nameAlternate);
                    if (find > -1) findLength = nameAlternate.Length;
                }
            }
            if (find == -1)
            {
                Match m = captureNodeTypeNumberLocation.Match(name);

                if (m.Success && m.Groups[0].Value == name && m.Groups.Count == 4)
                {
                    string peme = m.Groups[1].Value;
                    string number = m.Groups[2].Value;
                    string loc = m.Groups[3].Value;

                    // ME9-CKA
                    string pemeNumberStripLoc = " " + peme + number + "-" + loc + " ";
                    find = description.IndexOf(pemeNumberStripLoc);
                    if (find > -1) findLength = pemeNumberStripLoc.Length;

                    // ME-CKA9
                    if (find == -1)
                    {
                        string pemeStripLocNumber = " " + peme + "-" + loc + number + " ";
                        find = description.IndexOf(pemeStripLocNumber);
                        if (find > -1) findLength = pemeStripLocNumber.Length;
                    }

                    // CKA-9
                    if (find == -1)
                    {
                        string locStripNumber = " " + loc + "-" + number + " ";
                        find = description.IndexOf(locStripNumber);
                        if (find > -1) findLength = locStripNumber.Length;
                    }

                    // CKA9
                    if (find == -1)
                    {
                        string locNumber = " " + loc + number + " ";
                        find = description.IndexOf(locNumber);
                        if (find > -1) findLength = locNumber.Length;
                    }

                }
            }

            if (find > -1) return description.Substring(find + findLength);
            else return null;
        }

        private string FindNeighborPartUsingAlias(string description, string name)
        {
            int find = -1;
            int findLength = name.Length;

            if (NecrowVirtualization.Aliases.ContainsKey(name))
            {
                // search with alias
                foreach (string alias in NecrowVirtualization.Aliases[name])
                {
                    find = description.IndexOf(alias);
                    if (find > -1)
                    {
                        findLength = alias.Length;
                        break;
                    }
                }
            }

            if (find > -1) return description.Substring(find + findLength);
            else return null;
        }

        private string[] GenerateTestInterface(string type, string port)
        {
            List<string> testInterfaces = new List<string>();

            if (type != null && necrow.InterfaceTestPrefixes.ContainsKey(type))
            {
                foreach (string prefix in necrow.InterfaceTestPrefixes[type])
                {
                    testInterfaces.Add(port);
                    testInterfaces.Add(prefix + port);
                    testInterfaces.Add(prefix + "-" + port);
                    testInterfaces.Add(prefix + " " + port);
                    testInterfaces.Add(prefix + "/" + port);
                }
            }
            else testInterfaces.Add(port);

            testInterfaces = ListHelper.Sort(testInterfaces, SortMethods.LengthDescending);

            return testInterfaces.ToArray();
        }

        private void FindPhysicalNeighbor(InterfaceToDatabase li)
        {
            if (li.PhysicalNeighborChecked) return;
            li.PhysicalNeighborChecked = true;

            string description = CleanDescription(li.Description, NodeName);
            if (description == null) return;

            string interfaceName = li.Name;
            string interfaceType = li.InterfaceType;
            string interfacePort = interfaceName.Substring(2);

            bool done = false;

            Result2 result;
            Batch batch = j.Batch();
            Insert insert;

            #region TO_PI

            if (li.GetType() == typeof(MEInterfaceToDatabase))
            {
                MEInterfaceToDatabase mi = (MEInterfaceToDatabase)li;

                string neighborPEName = null;
                string neighborPEPart = null;
                List<Tuple<string, string, string, string, string, string>> currentNeighborPEInterfaces = null;

                lock (NecrowVirtualization.PESync)
                {
                    foreach (Tuple<string, List<Tuple<string, string, string, string, string, string>>> pe in NecrowVirtualization.PEPhysicalInterfaces)
                    {
                        neighborPEName = pe.Item1;
                        currentNeighborPEInterfaces = pe.Item2;
                        neighborPEPart = FindNeighborPart(description, neighborPEName);
                        if (neighborPEPart != null) break;
                    }
                    if (neighborPEPart == null)
                    {
                        foreach (Tuple<string, List<Tuple<string, string, string, string, string, string>>> pe in NecrowVirtualization.PEPhysicalInterfaces)
                        {
                            neighborPEName = pe.Item1;
                            currentNeighborPEInterfaces = pe.Item2;
                            neighborPEPart = FindNeighborPartUsingAlias(description, neighborPEName);
                            if (neighborPEPart != null) break;
                        }
                    }
                }
                if (neighborPEPart != null)
                {
                    Tuple<string, string, string, string, string, string> matchedInterface = null;

                    #region Find Interface

                    int leftMostFinding = neighborPEPart.Length;

                    foreach (Tuple<string, string, string, string, string, string> currentNBInterface in currentNeighborPEInterfaces)
                    {
                        string neighborInterfaceName = currentNBInterface.Item1;
                        string neighborInterfaceType = currentNBInterface.Item4;
                        string neighborInterfacePort = neighborInterfaceName.Substring(2);

                        foreach (string test in GenerateTestInterface(neighborInterfaceType, neighborInterfacePort))
                        {
                            int pos = neighborPEPart.IndexOf(test);

                            if (pos > -1 && pos < leftMostFinding)
                            {
                                leftMostFinding = pos;
                                matchedInterface = currentNBInterface;
                            }
                        }
                    }

                    #endregion

                    if (matchedInterface != null)
                    {
                        #region Crosscheck with matched interface description

                        string matchedDescription = CleanDescription(matchedInterface.Item2, neighborPEName);

                        if (matchedDescription != null)
                        {
                            string matchedNeighborPart = FindNeighborPart(matchedDescription, NodeName);

                            if (matchedNeighborPart != null) // at least we can find me name or the alias in pi description
                            {
                                foreach (string test in GenerateTestInterface(interfaceType, interfacePort))
                                {
                                    if (matchedNeighborPart.IndexOf(test) > -1)
                                    {
                                        mi.TopologyPEInterfaceID = matchedInterface.Item3;
                                        mi.NeighborCheckPITOMI = matchedInterface.Item6;

                                        // anak agregator ga mgkn punya anak sendiri
                                        // daftar parentnya juga untuk ditangkap di aggr pencari anak
                                        if (li.Aggr != -1) li.AggrNeighborParentID = matchedInterface.Item5;
                                        else
                                        {
                                            // find pi child
                                            li.ChildrenNeighbor = new Dictionary<int, Tuple<string, string, string>>();
                                            result = j.Query("select PI_ID, PI_DOT1Q, PI_TO_MI from PEInterface where PI_PI = {0}", mi.TopologyPEInterfaceID);
                                            foreach (Row2 row in result)
                                            {
                                                if (!row["PI_DOT1Q"].IsNull)
                                                {
                                                    int dot1q = row["PI_DOT1Q"].ToIntShort();
                                                    if (!li.ChildrenNeighbor.ContainsKey(dot1q)) li.ChildrenNeighbor.Add(dot1q, 
                                                        new Tuple<string, string, string>(row["PI_ID"].ToString(), row["PI_TO_MI"].ToString(), null));
                                                }
                                            }
                                        }

                                        break;
                                    }
                                }
                            }
                        }

                        #endregion
                    }

                    done = true;
                }

                if (done) return;
            }

            #endregion

            #region TO_MI

            string neighborMEName = null;
            string neighborMEPart = null;
            List<Tuple<string, string, string, string, string, string, string>> currentNeighborMEInterfaces = null;

            lock (NecrowVirtualization.MESync)
            {
                foreach (Tuple<string, List<Tuple<string, string, string, string, string, string, string>>> me in NecrowVirtualization.MEPhysicalInterfaces)
                {
                    neighborMEName = me.Item1;
                    currentNeighborMEInterfaces = me.Item2;
                    neighborMEPart = FindNeighborPart(description, neighborMEName);
                    if (neighborMEPart != null) break;
                }
                if (neighborMEPart == null)
                {
                    foreach (Tuple<string, List<Tuple<string, string, string, string, string, string, string>>> me in NecrowVirtualization.MEPhysicalInterfaces)
                    {
                        neighborMEName = me.Item1;
                        currentNeighborMEInterfaces = me.Item2;
                        neighborMEPart = FindNeighborPartUsingAlias(description, neighborMEName);
                        if (neighborMEPart != null) break;
                    }
                }
            }

            if (neighborMEPart != null)
            {
                Tuple<string, string, string, string, string, string, string> matchedInterface = null;

                #region Find Interface

                int leftMostFinding = neighborMEPart.Length;

                foreach (Tuple<string, string, string, string, string, string, string> currentNBInterface in currentNeighborMEInterfaces)
                {
                    string neighborInterfaceName = currentNBInterface.Item1;
                    string neighborInterfaceType = currentNBInterface.Item4;
                    string neighborInterfacePort = neighborInterfaceName.Substring(2);

                    foreach (string test in GenerateTestInterface(neighborInterfaceType, neighborInterfacePort))
                    {
                        int pos = neighborMEPart.IndexOf(test);

                        if (pos > -1 && pos < leftMostFinding)
                        {
                            leftMostFinding = pos;
                            matchedInterface = currentNBInterface;
                        }
                    }
                }

                #endregion

                if (matchedInterface != null)
                {
                    #region Crosscheck with matched interface description

                    string matchedDescription = CleanDescription(matchedInterface.Item2, neighborMEName);

                    if (matchedDescription != null)
                    {
                        string matchedNeighborPart = FindNeighborPart(matchedDescription, NodeName);

                        if (matchedNeighborPart != null)
                        {
                            foreach (string test in GenerateTestInterface(interfaceType, interfacePort))
                            {
                                if (matchedNeighborPart.IndexOf(test) > -1)
                                {
                                    li.TopologyMEInterfaceID = matchedInterface.Item3;
                                    if (li.GetType() == typeof(PEInterfaceToDatabase))
                                        ((PEInterfaceToDatabase)li).NeighborCheckMITOPI = matchedInterface.Item7;
                                    else
                                        ((MEInterfaceToDatabase)li).NeighborCheckMITOMI = matchedInterface.Item6;

                                    // anak agregator ga mgkn punya anak sendiri
                                    // daftar parentnya juga untuk ditangkap di aggr pencari anak
                                    if (li.Aggr != -1) li.AggrNeighborParentID = matchedInterface.Item5;
                                    else
                                    {
                                        // find mi child
                                        li.ChildrenNeighbor = new Dictionary<int, Tuple<string, string, string>>();
                                        result = j.Query("select MI_ID, MI_DOT1Q, MI_TO_MI, MI_TO_PI from MEInterface where MI_MI = {0}", li.TopologyMEInterfaceID);
                                        foreach (Row2 row in result)
                                        {
                                            if (!row["MI_DOT1Q"].IsNull)
                                            {
                                                int dot1q = row["MI_DOT1Q"].ToIntShort();
                                                if (!li.ChildrenNeighbor.ContainsKey(dot1q)) li.ChildrenNeighbor.Add(dot1q, 
                                                    new Tuple<string, string, string>(row["MI_ID"].ToString(), row["MI_TO_MI"].ToString(), row["MI_TO_PI"].ToString()));
                                            }
                                        }
                                    }

                                    break;
                                }
                            }
                        }
                    }

                    #endregion
                }

                done = true;
            }

            if (done) return;

            #endregion

            #region TO_NI

            string findNeighborNode = null;
            string findNeighborPart = null;

            lock (NecrowVirtualization.NNSync)
            {
                foreach (Tuple<string, List<Tuple<string, string>>> nn in NecrowVirtualization.NNPhysicalInterfaces)
                {
                    string neighborName = nn.Item1;
                    List<Tuple<string, string>> currentNBInterfaces = nn.Item2;

                    string neighborPart = FindNeighborPart(description, neighborName);

                    if (neighborPart != null)
                    {
                        findNeighborNode = neighborName;
                        findNeighborPart = neighborPart;

                        Tuple<string, string> matchedInterface = null;

                        #region Find Interface

                        int leftMostFinding = neighborPart.Length;

                        foreach (Tuple<string, string> currentNBInterface in currentNBInterfaces)
                        {
                            string neighborInterfaceName = currentNBInterface.Item1;

                            foreach (string test in GenerateTestInterface(null, neighborInterfaceName))
                            {
                                int pos = neighborPart.IndexOf(test);

                                if (pos > -1 && pos < leftMostFinding)
                                {
                                    leftMostFinding = pos;
                                    matchedInterface = currentNBInterface;
                                }
                            }
                        }

                        #endregion

                        if (matchedInterface != null)
                        {
                            li.TopologyNBInterfaceID = matchedInterface.Item2;
                            done = true;
                        }

                        break;
                    }
                }
            }

            if (!done)
            {
                if (findNeighborNode == null)
                {
                    // find neighbor node in description
                    findNeighborNode = FindNode(description, out findNeighborPart);
                }

                if (findNeighborNode != null)
                {
                    string neighborNodeID = null;

                    if (NecrowVirtualization.NodeNeighbors.ContainsKey(findNeighborNode))
                    {
                        neighborNodeID = NecrowVirtualization.NodeNeighbors[findNeighborNode];
                    }
                    else
                    {
                        batch.Begin();

                        // insert neighbor node
                        neighborNodeID = Database2.ID();
                        insert = j.Insert("NodeNeighbor");
                        insert.Value("NN_ID", neighborNodeID);
                        insert.Value("NN_Name", findNeighborNode);
                        batch.Add(insert);

                        // insert neighbor interface unspecified
                        string unspecifiedID = Database2.ID();
                        insert = j.Insert("NBInterface");
                        insert.Value("NI_ID", unspecifiedID);
                        insert.Value("NI_NN", neighborNodeID);
                        insert.Value("NI_Name", "UNSPECIFIED");
                        batch.Add(insert);

                        batch.Commit();

                        // tambah ke collection neighbors
                        lock (NecrowVirtualization.NNSync)
                        {
                            NecrowVirtualization.NodeNeighbors.Add(findNeighborNode, neighborNodeID);
                            NecrowVirtualization.NNPhysicalInterfaces.Add(new Tuple<string, List<Tuple<string, string>>>(findNeighborNode, new List<Tuple<string, string>>()));
                            NecrowVirtualization.NNUnspecifiedInterfaces.Add(findNeighborNode, unspecifiedID);

                            NecrowVirtualization.NNPhysicalInterfaces.Sort((a, b) => b.Item1.Length.CompareTo(a.Item1.Length));
                        }
                    }

                    // find interface
                    string neighborInterface = FindInterface(findNeighborPart);

                    if (neighborInterface == null)
                    {
                        // pake unspecified
                        li.TopologyNBInterfaceID = NecrowVirtualization.NNUnspecifiedInterfaces[findNeighborNode];
                    }
                    else
                    {
                        List<Tuple<string, string>> interfaces = null;

                        lock (NecrowVirtualization.NNSync)
                        {
                            foreach (Tuple<string, List<Tuple<string, string>>> tuple in NecrowVirtualization.NNPhysicalInterfaces)
                            {
                                if (tuple.Item1 == findNeighborNode)
                                {
                                    interfaces = tuple.Item2;
                                    break;
                                }
                            }
                        }

                        bool exists = false;
                        foreach (Tuple<string, string> ni in interfaces)
                        {
                            if (ni.Item1 == neighborInterface)
                            {
                                exists = true;
                                li.TopologyNBInterfaceID = ni.Item2;
                                break;
                            }
                        }

                        if (!exists)
                        {
                            // new interface under neighborNode
                            string interfaceID = Database2.ID();
                            insert = j.Insert("NBInterface");
                            insert.Value("NI_ID", interfaceID);
                            insert.Value("NI_NN", neighborNodeID);
                            insert.Value("NI_Name", neighborInterface);
                            insert.Execute();

                            li.TopologyNBInterfaceID = interfaceID;
                            interfaces.Add(new Tuple<string, string>(neighborInterface, interfaceID));
                            
                            interfaces.Sort((a, b) => b.Item1.Length.CompareTo(a.Item1.Length));
                        }
                    }
                }
            }

            #endregion
        }

        private readonly char[] cleanDescriptionAllowedCharacterForTrimStart = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
        private readonly char[] cleanDescriptionAllowedCharacterForTrimEnd = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

        private readonly string[] cleanDescriptionStartsWith = { "EKS ", "EKS.", "EXS ", "EXS. ", "EX ", "EX-", "BEKAS ", "MIGRASI", "MIGRATED", "PINDAH", "GANTI" };
        private readonly string[] cleanDescriptionUnnecessaryInfo = { "(EX", "(EKS", "[EX", "[EKS", " EX-", " EKS-", " EXS.", " EKS.", " BEKAS ", " PINDAHAN ", " MOVE " };

        private readonly Regex findNodeRegex = new Regex(@"^(?:(?:T)\d{0,2}-D[1-7]|(?:GPON|MSAN|DSL(?:AM)?|ME|PE|SW(?:C)?|BRAS|DCN|SBC|HRB|WAC|WAG)\d{0,2})(?:-(?:\d[A-Z\d]+|[A-Z][A-Z\d]*)){1,4}$");
        private readonly Regex findInterfaceRegex = new Regex(@"^(?:(?:\/)*(?:(?:F(?:A(?:ST)?)?|(?:(?:TE(?:NGIG(?:ABIT)?)?|HU(?:NDRED)?){0,1}(?:G(?:I(?:GABIT)?)?)?)){0,1}(?:E(?:T(?:HERNET)?)?)?|XE)?(?:\/|-)*(?:[0-9]{1,2})(?:\/[0-9]{1,2}){1,3}|PKT[0-9])$");
        private readonly Regex captureNodeTypeNumberLocation = new Regex(@"^(ME|PE)(\d)-D[1-7]-([A-Z]{3})$");

        //4700146-0029928203

        private string CleanDescription(string description, string originNodeName)
        {
            // ends if null or empty
            if (string.IsNullOrEmpty(description)) return null;

            // make all description upper case, remove leading and trailing characters except allowed characters, and replace underscores with space
            description = description.ToUpper().TrimStartExcept(cleanDescriptionAllowedCharacterForTrimStart).TrimEndExcept(cleanDescriptionAllowedCharacterForTrimEnd).Replace('_', ' ');

            // ends if migrated or moved somewhere elese
            if (description.StartsWith(cleanDescriptionStartsWith)) return null;

            // removes unnecessary info after specified point
            int unid = description.IndexOf(cleanDescriptionUnnecessaryInfo);
            if (unid > -1) description = description.Remove(unid);

            // removes current node name
            description = description.Replace(originNodeName, "");

            return description;
        }

        private string FindNode(string description, out string nodePart)
        {
            string[] splits = description.Split(new string[] {
                " ", "(", ")", "_", "[", "]", ";", ".", "=", ":", "@", "/", "\\",
                " L2-", " TO-", "-SID-", "-SOID-", "-TENOSS-", "-SID4", "-SOID4", "-TO-", "-IP-",
                "-PORT",
                "-GE-", "-GI-", "-TE-", "-XE-", "-FA-",
                "-GI0", "-GI1", "-GI2", "-GI3", "-GI4", "-GI5", "-GI6", "-GI7", "-GI8", "-GI9",
                "-GE0", "-GE1", "-GE2", "-GE3", "-GE4", "-GE5", "-GE6", "-GE7", "-GE8", "-GE9",
                "-TE0", "-TE1", "-TE2", "-TE3", "-TE4", "-TE5", "-TE6", "-TE7", "-TE8", "-TE9",
                "-XE0", "-XE1", "-XE2", "-XE3", "-XE4", "-XE5", "-XE6", "-XE7", "-XE8", "-XE9",
                "-FA0", "-FA1", "-FA2", "-FA3", "-FA4", "-FA5", "-FA6", "-FA7", "-FA8", "-FA9",
            }, StringSplitOptions.RemoveEmptyEntries);

            nodePart = null;

            foreach (string split in splits)
            {
                if (findNodeRegex.IsMatch(split))
                {
                    int isp = description.IndexOf(split);
                    nodePart = description.Substring(isp + split.Length, description.Length - isp - split.Length);
                    return split;
                }
                else
                {
                    string[] splitstrip = split.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    
                    StringBuilder nsplit = new StringBuilder();

                    bool first = true, anumber = false;
                    foreach (string split2s in splitstrip)
                    {
                        if (!first && !anumber)
                        {
                            nsplit.Append("-");
                            anumber = false;
                        }

                        int spliti;
                        if (int.TryParse(split2s, out spliti)) anumber = true;

                        nsplit.Append(split2s);

                        first = false;
                    }
                    string split2 = nsplit.ToString();

                    if (findNodeRegex.IsMatch(split2))
                    {
                        int isp = description.IndexOf(split);
                        nodePart = description.Substring(isp + split.Length, description.Length - isp - split.Length);
                        return split2;
                    }
                    else
                    {
                        if (anumber && split2.IndexOf('-') > -1) // so its ended with number
                        {
                            // remove last strip and join with the previous token
                            string split3 = split2.Substring(0, split2.LastIndexOf('-')) + split2.Substring(split2.LastIndexOf('-') + 1);

                            if (findNodeRegex.IsMatch(split3))
                            {
                                int isp = description.IndexOf(split);
                                nodePart = description.Substring(isp + split.Length, description.Length - isp - split.Length);
                                return split3;
                            }
                        }
                    }
                }
            }

            return null;
        }

        private string FindInterface(string description)
        {
            string[] splits = description.Split(new string[] {
                " ", "(", ")", "_", "[", "]", ";", "=", ":", "@", "\\"
            }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string split in splits)
            {
                if (findInterfaceRegex.IsMatch(split))
                {
                    string iclean = split.TrimStart(new char[] { '/' }).Replace("-", "").Replace("XE", "TE");

                    int ic = 0;
                    foreach (char c in iclean)
                    {
                        if (char.IsDigit(c)) break;
                        ic++;
                    }

                    if (ic == 3 & iclean.StartsWith("PKT")) return iclean;
                    
                    string port = iclean.Substring(ic);
                    string utype = iclean.Substring(0, 1);

                    string gtype;
                    if (utype == "H") gtype = "Hu";
                    else if (utype == "T") gtype = "Te";
                    else if (utype == "G") gtype = "Gi";
                    else if (utype == "F") gtype = "Fa";
                    else if (utype == "E") gtype = "Et";
                    else gtype = "Ex";

                    return gtype + port;
                }
            }

            return null;
        }

        private string ConvertALUPort(string input)
        {
            // 1/2/3.CHANNEL:VLAN.SUBVLAN
            // to
            // Ex1/2/3:CHANNEL.VLAN.SUBVLAN
            //
            // 1/2/3.CHANNEL to 1/2/3:CHANNEL.DIRECT
            //

            string[] tokens = input.Split(new char[] { ':' });

            string port;

            string[] portTokens = tokens[0].Split(new char[] { '.' });
            if (portTokens.Length == 1) port = portTokens[0].StartsWith("lag-") ? ("Ag" + portTokens[0].Substring(4)) : ("Ex" + portTokens[0]);
            else port = (portTokens[0].StartsWith("lag-") ? ("Ag" + portTokens[0].Substring(4)) : ("Ex" + portTokens[0])) + ":" + portTokens[1];

            string vlan;

            if (tokens.Length > 1) vlan = tokens[1];
            else vlan = "DIRECT";

            return port + "." + vlan;
        }

        private string FixDescription(string input)
        {
            if (input == null) return null;

            string[] tokens = input.Split(new string[] { "                                               " }, StringSplitOptions.RemoveEmptyEntries);

            StringBuilder sb = new StringBuilder();
            foreach (string token in tokens)
            {
                sb.Append(token);
            }

            return sb.ToString();
        }
    }

    internal class ExpectResult
    {
        private int index;

        public int Index
        {
            get { return index; }
        }

        private string output;

        public string Output
        {
            get { return output; }
        }

        private bool timeout = false;

        public bool IsTimeout { get => timeout; }

        public ExpectResult(int index, string output, bool timeout)
        {
            this.index = index;
            this.output = output;
            this.timeout = timeout;
        }
    }
}
