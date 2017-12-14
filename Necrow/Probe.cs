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


namespace Center
{
    #region To Database

    internal abstract class ToDatabase
    {
        private string id;

        public string ID
        {
            get { return id; }
            set { id = value; }
        }

    }

    internal abstract class StatusToDatabase : ToDatabase
    {
        private bool status;

        public bool Status
        {
            get { return status; }
            set { status = value; }
        }

        private bool protocol;

        public bool Protocol
        {
            get { return protocol; }
            set { protocol = value; }
        }

        private bool enable;

        public bool Enable
        {
            get { return enable; }
            set { enable = value; }
        }

        private bool updateStatus = false;

        public bool UpdateStatus
        {
            get { return updateStatus; }
            set { updateStatus = value; }
        }

        private bool updateProtocol = false;

        public bool UpdateProtocol
        {
            get { return updateProtocol; }
            set { updateProtocol = value; }
        }

        private bool updateEnable = false;

        public bool UpdateEnable
        {
            get { return updateEnable; }
            set { updateEnable = value; }
        }
    }

    internal abstract class ServiceBaseToDatabase : StatusToDatabase
    {
        private string serviceID;

        public string ServiceID
        {
            get { return serviceID; }
            set { serviceID = value; }
        }

        private string serviceSID;

        public string ServiceSID
        {
            get { return serviceSID; }
            set { serviceSID = value; }
        }
    }

    internal abstract class InterfaceToDatabase : ServiceBaseToDatabase
    {
        #region Basic

        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private bool updateName = false;

        public bool UpdateName
        {
            get { return updateName; }
            set { updateName = value; }
        }

        private string description;

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        private bool updateDescription = false;

        public bool UpdateDescription
        {
            get { return updateDescription; }
            set { updateDescription = value; }
        }
        
        private string interfaceType = null;

        public string InterfaceType
        {
            get { return interfaceType; }
            set { interfaceType = value; }
        }

        private bool updateInterfaceType = false;

        public bool UpdateInterfaceType
        {
            get { return updateInterfaceType; }
            set { updateInterfaceType = value; }
        }

        private int dot1q = -1;

        public int Dot1Q
        {
            get { return dot1q; }
            set { dot1q = value; }
        }

        private bool updateDot1Q = false;

        public bool UpdateDot1Q
        {
            get { return updateDot1Q; }
            set { updateDot1Q = value; }
        }

        private DateTime? lastDown = null;

        public DateTime? LastDown
        {
            get { return lastDown; }
            set { lastDown = value; }
        }

        private bool updateLastDown = false;

        public bool UpdateLastDown
        {
            get { return updateLastDown; }
            set { updateLastDown = value; }
        }

        #endregion

        #region QOS

        private int rateInput = -1;

        public int RateInput
        {
            get { return rateInput; }
            set { rateInput = value; }
        }

        private int rateOutput = -1;

        public int RateOutput
        {
            get { return rateOutput; }
            set { rateOutput = value; }
        }

        private bool updateRateInput = false;

        public bool UpdateRateInput
        {
            get { return updateRateInput; }
            set { updateRateInput = value; }
        }

        private bool updateRateOutput = false;

        public bool UpdateRateOutput
        {
            get { return updateRateOutput; }
            set { updateRateOutput = value; }
        }

        #endregion

        #region Usage

        private char mode = '-';

        public char Mode { get => mode; set => mode = value; }

        private bool updateMode = false;

        public bool UpdateMode { get => updateMode; set => updateMode = value; }

        private char encapsulation = '-';

        public char Encapsulation { get => encapsulation; set => encapsulation = value; }

        private bool updateEncapsulation = false;

        public bool UpdateEncapsulation { get => updateEncapsulation; set => updateEncapsulation = value; }

        #endregion

        #region MTU

        private int admMTU = -1;

        public int AdmMTU { get => admMTU; set => admMTU = value; }

        private bool updateAdmMTU = false;

        public bool UpdateAdmMTU { get => updateAdmMTU; set => updateAdmMTU = value; }

        private int runMTU = -1;

        public int RunMTU { get => runMTU; set => runMTU = value; }

        private bool updateRunMTU = false;

        public bool UpdateRunMTU { get => updateRunMTU; set => updateRunMTU = value; }

        #endregion

        #region Topology

        private int aggr = -1;

        public int Aggr
        {
            get { return aggr; }
            set { aggr = value; }
        }

        private bool updateAggr = false;

        public bool UpdateAggr
        {
            get { return updateAggr; }
            set { updateAggr = value; }
        }

        private string parentID = null;

        public string ParentID
        {
            get { return parentID; }
            set { parentID = value; }
        }

        private bool updateParentID = false;

        public bool UpdateParentID
        {
            get { return updateParentID; }
            set { updateParentID = value; }
        }

        private string topologyMEInterfaceID = null;

        public string TopologyMEInterfaceID
        {
            get { return topologyMEInterfaceID; }
            set { topologyMEInterfaceID = value; }
        }

        private bool updateTopologyMEInterfaceID = false;

        public bool UpdateTopologyMEInterfaceID
        {
            get { return updateTopologyMEInterfaceID; }
            set { updateTopologyMEInterfaceID = value; }
        }

        private string topologyNBInterfaceID = null;

        public string TopologyNBInterfaceID
        {
            get { return topologyNBInterfaceID; }
            set { topologyNBInterfaceID = value; }
        }

        private bool updateTopologyNBInterfaceID = false;

        public bool UpdateTopologyNBInterfaceID
        {
            get { return updateTopologyNBInterfaceID; }
            set { updateTopologyNBInterfaceID = value; }
        }

        private bool physicalNeighborChecked = false;

        public bool PhysicalNeighborChecked
        {
            get { return physicalNeighborChecked; }
            set { physicalNeighborChecked = value; }
        }

        private string aggrNeighborParentID = null;

        public string AggrNeighborParentID
        {
            get { return aggrNeighborParentID; }
            set { aggrNeighborParentID = value; }
        }

        private Dictionary<int, Tuple<string, string, string>> childrenNeighbor = null;

        public Dictionary<int, Tuple<string, string, string>> ChildrenNeighbor
        {
            get { return childrenNeighbor; }
            set { childrenNeighbor = value; }
        }

        #endregion

        #region Summary

        private long cirTotalInput = -1;

        public long CirTotalInput
        {
            get { return cirTotalInput; }
            set { cirTotalInput = value; }
        }

        private bool updateCirTotalInput = false;

        public bool UpdateCirTotalInput
        {
            get { return updateCirTotalInput; }
            set { updateCirTotalInput = value; }
        }

        private long cirTotalOutput = -1;

        public long CirTotalOutput
        {
            get { return cirTotalOutput; }
            set { cirTotalOutput = value; }
        }

        private bool updateCirTotalOutput = false;

        public bool UpdateCirTotalOutput
        {
            get { return updateCirTotalOutput; }
            set { updateCirTotalOutput = value; }
        }

        private int cirConfigTotalInput = -1;

        public int CirConfigTotalInput
        {
            get { return cirConfigTotalInput; }
            set { cirConfigTotalInput = value; }
        }

        private bool updateCirConfigTotalInput = false;

        public bool UpdateCirConfigTotalInput
        {
            get { return updateCirConfigTotalInput; }
            set { updateCirConfigTotalInput = value; }
        }

        private int cirConfigTotalOutput = -1;

        public int CirConfigTotalOutput
        {
            get { return cirConfigTotalOutput; }
            set { cirConfigTotalOutput = value; }
        }

        private bool updateCirConfigTotalOutput = false;

        public bool UpdateCirConfigTotalOutput
        {
            get { return updateCirConfigTotalOutput; }
            set { updateCirConfigTotalOutput = value; }
        }

        private int subInterfaceCount = -1;

        public int SubInterfaceCount
        {
            get { return subInterfaceCount; }
            set { subInterfaceCount = value; }
        }

        private bool updateSubInterfaceCount = false;

        public bool UpdateSubInterfaceCount
        {
            get { return updateSubInterfaceCount; }
            set { updateSubInterfaceCount = value; }
        }

        #endregion

        #region Percentage

        private float trafficInput = -1;

        public float TrafficInput { get => trafficInput; set => trafficInput = value; }

        private bool updateTrafficInput = false;

        public bool UpdateTrafficInput { get => updateTrafficInput; set => updateTrafficInput = value; }

        private float trafficOutput = -1;

        public float TrafficOutput { get => trafficOutput; set => trafficOutput = value; }

        private bool updateTrafficOutput = false;

        public bool UpdateTrafficOutput { get => updateTrafficOutput; set => updateTrafficOutput = value; }

        #endregion
    }

    internal abstract class MacToDatabase : ToDatabase
    {
        private string macAddress;

        public string MacAddress { get => macAddress; set => macAddress = value; }

        private bool updateMacAddress = false;

        public bool UpdateMacAddress { get => updateMacAddress; set => updateMacAddress = value; }

        private string interfaceID;

        public string InterfaceID { get => interfaceID; set => interfaceID = value; }

        private int age;

        public int Age { get => age; set => age = value; }

        private bool updateAge = false;

        public bool UpdateAge { get => updateAge; set => updateAge = value; }
    }

    internal class CustomerToDatabase : ToDatabase
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string cid;

        public string CID
        {
            get { return cid; }
            set { cid = value; }
        }
    }

    internal class ServiceToDatabase : ToDatabase
    {
        private string sid;

        public string SID
        {
            get { return sid; }
            set { sid = value; }
        }

        private string type;

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        private string subType;

        public string SubType
        {
            get { return subType; }
            set { subType = value; }
        }

        private string rawDesc;

        public string RawDesc
        {
            get { return rawDesc; }
            set { rawDesc = value; }
        }

        private string customerID;

        public string CustomerID
        {
            get { return customerID; }
            set { customerID = value; }
        }
    }

    internal class SlotToDatabase : ToDatabase
    {
        private string slotID1 = null;

        public string SlotID1 { get => slotID1; set => slotID1 = value; }

        private string slotID2 = null;

        public string SlotID2 { get => slotID2; set => slotID2 = value; }

        private string info1 = null;

        public string Info1 { get => info1; set => info1 = value; }

        private string info2 = null;

        public string Info2 { get => info2; set => info2 = value; }

        private string info3 = null;

        public string Info3 { get => info3; set => info3 = value; }

        private string info4 = null;

        public string Info4 { get => info4; set => info4 = value; }        

        private bool updateInfo1 = false;

        public bool UpdateInfo1 { get => updateInfo1; set => updateInfo1 = value; }

        private bool updateInfo2 = false;

        public bool UpdateInfo2 { get => updateInfo2; set => updateInfo2 = value; }

        private bool updateInfo3 = false;

        public bool UpdateInfo3 { get => updateInfo3; set => updateInfo3 = value; }

        private bool updateInfo4 = false;

        public bool UpdateInfo4 { get => updateInfo4; set => updateInfo4 = value; }
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
        ALURequest
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

    internal sealed partial class Probe : SshConnection
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

        private Necrow instance = null;

        private ProbeProperties properties;
        internal ProbeProperties Properties { get { return properties; } }
        
        private Thread idleThread = null;
        
        private Dictionary<string, object> updates;
        private Dictionary<string, string> summaries;
        private string defaultOutputIdentifier = null;
        private string outputIdentifier = null;
        
        private Database j;

        private string lastNodeName = null;
        private DateTime lastProbeEndTime = DateTime.MinValue;

        private string nodeID;
        private List<string> nodeRules;
        private string nodeName;
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

        private DateTime nodeProbeStartTime = DateTime.MinValue;
        private DateTime sshProbeStartTime = DateTime.MinValue;

        private bool updatingNecrow = false;

        public string NodeName
        {
            get { return nodeName; }
        }

        public string LastNodeName
        {
            get { return lastNodeName; }
        }

        public DateTime NodeProbeStartTime
        {
            get { return nodeProbeStartTime; }
        }

        public DateTime LastProbeEndTime
        {
            get { return lastProbeEndTime; }
        }

        private readonly string alu = "ALCATEL-LUCENT";
        private readonly string hwe = "HUAWEI";
        private readonly string cso = "CISCO";
        private readonly string jun = "JUNIPER";
        private readonly string xr = "XR";

        private Dictionary<string, Row> reserves;
        private Dictionary<string, Row> popInterfaces;

        private ProbeRequestData probeRequestData = null;

        private bool probing = false;

        public bool IsProbing
        {
            get { return probing; }
        }

        public DateTime SSHProbeStartTime
        {
            get { return sshProbeStartTime; }
        }

        private bool queueStop = false;

        private bool sessionStart = false;

        public bool SessionStart
        {
            get { return sessionStart; }
            set { sessionStart = value; }
        }

        #endregion

        #region Constructors

        public Probe(ProbeProperties properties, Necrow instance, string identifier)
        {
            this.properties = properties;
            this.defaultOutputIdentifier = identifier;
            this.instance = instance;

            j = Jovice.Database;
        }

        #endregion

        #region Database

        public Batch Batch()
        {
            if (j == null) j = Jovice.Database;
            return j.Batch();
        }

        public Insert Insert(string table)
        {
            if (j == null) j = Jovice.Database;
            return j.Insert(table);
        }

        public Update Update(string table)
        {
            if (j == null) j = Jovice.Database;
            return j.Update(table);
        }

        public Dictionary<string, Row> QueryDictionary(string sql, string key, params object[] args)
        {
            if (j == null) j = Jovice.Database;
            return j.QueryDictionary(sql, key, args);
        }

        public Dictionary<string, Row> QueryDictionary(string sql, string key, QueryDictionaryDuplicateCallback duplicate, params object[] args)
        {
            if (j == null) j = Jovice.Database;
            return j.QueryDictionary(sql, key, duplicate, args);
        }

        public Dictionary<string, Row> QueryDictionary(string sql, QueryDictionaryKeyCallback callback, params object[] args)
        {
            if (j == null) j = Jovice.Database;
            return j.QueryDictionary(sql, callback, args);
        }

        public Dictionary<string, Row> QueryDictionary(string sql, QueryDictionaryKeyCallback callback, QueryDictionaryDuplicateCallback duplicate, params object[] args)
        {
            if (j == null) j = Jovice.Database;
            return j.QueryDictionary(sql, callback, duplicate, args);
        }

        public List<string> QueryList(string sql, string key, params object[] args)
        {
            if (j == null) j = Jovice.Database;
            return j.QueryList(sql, key, args);
        }

        public string Format(string sql, params object[] args)
        {
            if (j == null) j = Jovice.Database;
            return j.Format(sql, args);
        }

        public Result Query(string sql, params object[] args)
        {
            if (j == null) j = Jovice.Database;
            return j.Query(sql, args);
        }

        public Column Scalar(string sql, params object[] args)
        {
            if (j == null) j = Jovice.Database;
            return j.Scalar(sql, args);
        }

        public Result Execute(string sql, params object[] args)
        {
            if (j == null) j = Jovice.Database;
            return j.Execute(sql, args);
        }

        public Result ExecuteIdentity(string sql, params object[] args)
        {
            if (j == null) j = Jovice.Database;
            return j.ExecuteIdentity(sql, args);
        }

        #endregion

        #region Event

        private void Event(string message)
        {
            if (outputIdentifier == null)
                instance.Event(message, defaultOutputIdentifier);
            else
                instance.Event(message, defaultOutputIdentifier, outputIdentifier);
        }

        private void Event(Result result, EventActions action, EventElements element, bool reportzero)
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

        public void QueueStop()
        {
            queueStop = true;
        }

        public void Start()
        {
            Start(properties.SSHServerAddress, properties.SSHUser, properties.SSHPassword);
        }

        #endregion
        
        #region Handlers

        protected override void OnStarting()
        {
            Event("Connecting... (" + properties.SSHUser + "@" + properties.SSHServerAddress + " [" + properties.TacacUser + "])");
            Thread.Sleep(RandomHelper.Next(0, 500));
        }

        protected override void OnConnected()
        {
            Event("Connected!");

            sshProbeStartTime = DateTime.UtcNow;
        }

        protected override void OnProcess()
        {
            Event("Terminal prefix: " + terminalPrefix);

            Row restartCurrentNode = null;
            int restartCount = 0;

            while (true)
            {
                string xpID = null;
                Row node = null;
                bool continueProcess = false;
                bool prioritizeProcess = false;
                bool prioritizeAsk = false;
                probeRequestData = null;

                if (restartCurrentNode != null)
                {
                    node = restartCurrentNode;
                    restartCount++;
                    restartCurrentNode = null;
                }
                else
                {
                    Tuple<string, ProbeRequestData> prioritize = null;

                    if (properties.Case == "MAIN")
                    {
                        prioritize = instance.NextPrioritize();

                        if (prioritize != null)
                        {
                            string prioritizeNode = prioritize.Item1;

                            if (prioritizeNode.EndsWith("*"))
                            {
                                prioritizeNode = prioritizeNode.TrimEnd(new char[] { '*' });
                                prioritizeProcess = true;
                            }
                            else if (prioritizeNode.EndsWith("?"))
                            {
                                prioritizeNode = prioritizeNode.TrimEnd(new char[] { '?' });
                                prioritizeAsk = true;
                            }


                            Event("Prioritizing Probe: " + prioritizeNode);
                            Result rnode = Query("select * from Node where upper(NO_Name) = {0} and NO_Active = 1", prioritizeNode);

                            if (rnode.Count == 1)
                            {
                                node = rnode[0];

                                if (prioritize.Item2 != null)
                                {
                                    probeRequestData = prioritize.Item2;
                                    probeRequestData.Message.Data = new object[] { node["NO_Name"].ToString(), "STARTING" };
                                    probeRequestData.Connection.Reply(probeRequestData.Message);
                                }
                            }
                            else
                            {
                                Event("Failed. Theres no active probe for specified name");
                                continue;
                            }
                        }
                    }

                    if (prioritize == null)
                    {
                        Tuple<string, string> noded = instance.NextNode(properties.Case);

                        xpID = noded.Item1;
                        Result rnode = Query("select * from Node where NO_ID = {0}", noded.Item2);
                        
                        node = rnode[0];

                        if (node["NO_Active"].ToBool() == false)
                        {
                            // remove this from progress and moveon
                            Execute("delete from ProbeProgress where XP_ID = {0}", xpID);
                            continue;
                        }
                    }

                    restartCount = 0;
                }

                if (node != null)
                {
                    bool caughtError = false;
                    
                    string info = null;
                    ProbeProcessResult probe = null;
#if !DEBUG
                    try
                    {
#endif

                    probe = Enter(node, xpID, out continueProcess, prioritizeProcess, prioritizeAsk);

                    if (probe != null)
                    {
                        if (probe.FailureType == FailureTypes.ALURequest)
                        {
                            // restart?
                            Event("Probe error: ALU request has failed");

                            if (restartCount < 2) restartCurrentNode = node;
                            else
                            {
                                Update(UpdateTypes.Remark, "PROBEFAILED");
                                caughtError = true;

                                instance.PendingNode(properties.Case, xpID, nodeID, TimeSpan.FromHours(4));
                            }

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
                            instance.Log(nodeName, ex.Message, ex.StackTrace);
                            Update(UpdateTypes.Remark, "PROBEFAILED");
                        }

                        continueProcess = false;

                        Save();

                        if (probing)
                        {
                            if (info != null)
                                Event("Caught error: " + info + ", exiting current node...");
                            Exit();
                        }
                        else if (info != null)
                            Event("Caught error: " + info);

                        caughtError = true;
                    }
#endif

                    if (continueProcess)
                    {
                        #region Continue Process
                        idleThread = new Thread(new ThreadStart(delegate ()
                        {
                            while (true)
                            {
                                Thread.Sleep(30000);
                                if (nodeManufacture == alu || nodeManufacture == cso || nodeManufacture == hwe) SendCharacter((char)27);

                            }
                        }));
                        idleThread.Start();
                                                
                        if (properties.Case == "MAIN")
                        {
                            #region MAIN CASE preparations

                            Batch batch = Batch();
                            Result result;
                            batch.Begin();
                            // RESERVES
                            reserves = QueryDictionary("select * from Reserve where RE_NO = {0}", delegate (Row row)
                            {
                                return row["RE_By_Name"].ToString() + "-" + row["RE_By_SID"].ToString();
                            }, delegate (Row row)
                            {
                            // delete duplicated
                            batch.Execute("delete from Reserve where RE_ID = {0}", row["RE_ID"].ToString());
                            }, nodeID);

                            // POP
                            if (nodeType == "P")
                            {
                                popInterfaces = new Dictionary<string, Row>();
                                result = Query("select * from POP where UPPER(OO_NO_Name) = {0}", nodeName);
                                foreach (Row row in result)
                                {
                                    string storedID = row["OO_ID"].ToString();
                                    string interfaceName = row["OO_PI_Name"].ToString();
                                    string type = row["OO_Type"].ToString();

                                    string key = interfaceName + "_" + type;

                                    bool ooPINULL = row["OO_PI"].IsNull;

                                    if (!popInterfaces.ContainsKey(key))
                                    {
                                        popInterfaces.Add(key, row);

                                        if (row["OO_NO_Name"].ToString() != nodeName)
                                        {
                                            // fix incorrect name in POP
                                            batch.Execute("update POP set OO_NO_Name = {0} where OO_ID = {1}", nodeName, storedID);
                                        }
                                    }
                                    else
                                    {
                                        // delete duplicated OO_PI_Name per OO_NO_Name
                                        batch.Execute("delete from POP where OO_ID = {0}", storedID);
                                    }
                                }
                            }
                            else popInterfaces = null;

                            batch.Commit();

                            #endregion
                        }
#if !DEBUG
                        try
                        {
#endif

                        if (properties.Case == "MAIN")
                        {
                            if (nodeType == "P") probe = PEProcess();
                            else if (nodeType == "M") probe = MEProcess();
                        }
                        else
                            probe = MEMacProcess();

                        if (probe != null)
                        {
                            if (probe.FailureType == FailureTypes.ALURequest)
                            {
                                // restart?
                                Event("Probe error: ALU request has failed");

                                if (restartCount < 2) restartCurrentNode = node;
                                else
                                {
                                    Update(UpdateTypes.Remark, "PROBEFAILED");
                                    caughtError = true;

                                    instance.PendingNode(properties.Case, xpID, nodeID, TimeSpan.FromHours(4));
                                }
                            }
                            else if (probe.FailureType == FailureTypes.ProbeStopped)
                            {
                                Event("Probe error: Probe has been stopped in the middle of request");
                                caughtError = true;

                                instance.PendingNode(properties.Case, xpID, nodeID, TimeSpan.FromMinutes(30));
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
                            instance.AcknowledgeNodeVersion(nodeManufacture, nodeVersion, nodeSubVersion);
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.IndexOf("was being aborted") == -1)
                            {
                                info = ex.Message;
                                instance.Log(nodeName, ex.Message, ex.StackTrace);
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
                        #endregion
                    }

                    if (probe != null && probe.FailureType == FailureTypes.Connection)
                    {
                        Save();

                        ConnectionFailure();
                    }
                    else if (restartCurrentNode != null)
                    {
                        Event("Probe process to the node is to be restarted");

                        if (probing)
                            Exit();
                    }
                    else
                    {
                        if (probing)
                            SaveExit();
                    }

                    if (probeRequestData != null)
                    {
                        if (!continueProcess) { }
                        else if (caughtError)
                        {
                            probeRequestData.Message.Data = new object[] { nodeName, "ERROR", info };
                            probeRequestData.Connection.Reply(probeRequestData.Message);
                        }
                        else
                        {
                            probeRequestData.Message.Data = new object[] { nodeName, "FINISH" };
                            probeRequestData.Connection.Reply(probeRequestData.Message);
                        }
                        
                    }

                    Thread.Sleep(5000); // GRACE PERIOD BETWEEN NODES OR NECROW TO DO SOMETHING
                }
            }
        }

        protected override void OnBeforeTerminate()
        {
            outputIdentifier = null;
        }

        protected override void OnTerminated()
        {
            probing = false;
            sshProbeStartTime = DateTime.MinValue;
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

        protected override void OnConnectionFailure()
        {
            Event("Connection failure has occured");

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
                int k = RandomHelper.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        private ExpectResult Expect(params string[] args)
        {
            Queue<string> lastOutputs = new Queue<string>();
            string expectOutput = null;

            if (args.Length == 0) return new ExpectResult(-1, null);

            int wait = 0;
            int expectReturn = -1;

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
                        break;
                }
            }

            return new ExpectResult(expectReturn, expectOutput);
        }

        private string ResolveHostName(string hostname, bool reverse)
        {
            string cpeip = null;
            hostname = hostname.ToLower();

            if (Request2("cat /etc/hosts | grep -i " + hostname, out string[] lines)) ConnectionFailure();

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

        private void Status(string status)
        {
            if (probeProgressID != null) Execute("update ProbeProgress set XP_Status = {0} where XP_ID = {1}", status, probeProgressID);
        }

        private void Summary(string key, int value)
        {
            Summary(key, value.ToString());
        }

        private void Summary(string key, float value)
        {
            Summary(key, value.ToString());
        }

        private void Summary(string key, bool value)
        {
            Summary(key, value ? 1 : 0);
        }

        private void Summary(string key, string value)
        {
            if (key != null && value != null)
            {
                if (summaries.ContainsKey(key)) summaries[key] = value;
                else summaries.Add(key, value);
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

            probing = false;
            lastNodeName = nodeName;
            lastProbeEndTime = DateTime.UtcNow;

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
            Event("Request [" + command + "]...");

#if DEBUG
            Event("Command length: " + command.Length);
#endif

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
                                if (nodeManufacture == hwe && nodeVersion == "5.160" && line.Length > 80)
                                {
                                    int looptimes = (int)Math.Ceiling((float)line.Length / 80);

                                    for (int loop = 0; loop < looptimes; loop++)
                                    {
                                        int sisa = 80;
                                        if (loop == looptimes - 1) sisa = line.Length - (loop * 80);
                                        string curline = line.Substring(loop * 80, sisa);
                                        listLines.Add(curline);
                                    }
                                }
                                else
                                {
                                    listLines.Add(line);
                                }
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
                            Event("The probe is not probing anymore");
                            requestFailed = true;
                            probe.FailureType = FailureTypes.ProbeStopped;
                            break;
                        }
                        
                        wait++;
                        if (wait % 200 == 0 && wait < 1600)
                        {
                            Event("Waiting...");
                            SendLine("");

#if DEBUG
                            Event("Last Output: " + LastOutput);
#endif
                        }
                        Thread.Sleep(100);
                        if (wait == 1600)
                        {
                            Event("Reading timeout, cancel the reading...");
                            requestFailed = true;
                        }
                        else if (wait >= 1600 && wait % 50 == 0)
                        {
                            SendControlC();
                        }
                        else if (wait == 2000)
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
                        Event("Improper command, send request again...");
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
                            Event("Request completed (" + lines.Length + " lines in " + string.Format("{0:0.###}", stopwatch.Elapsed.TotalSeconds) + "s)");
                            requestLoop = false;
                        }
                        else
                        {
                            if (aluError > 0)
                            {
                                if (aluError < 5)
                                {
                                    Event("Request not completed: ALU Request Error. Try again");
                                    Thread.Sleep(5000);
                                    requestLoop = true;
                                }
                                else
                                {
                                    Event("Request not completed: ALU Request Error.");
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

            string user = properties.TacacUser;
            string pass = properties.TacacPassword;

            Event("Connecting with Telnet... (" + user + "@" + host + ")");
            SendLine("telnet " + host);

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

            return connectSuccess;
        }

        private bool ConnectBySSH(string host, string manufacture)
        {
            bool connectSuccess = false;

            string user = properties.TacacUser;
            string pass = properties.TacacPassword;

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
                    SendLine("rm ~/.ssh/known_hosts");

                    Thread.Sleep(500);
                    SendLine("ssh -o StrictHostKeyChecking=no " + user + "@" + host);

                    looppass = true;
                }
                else
                {
                    if (expect.Index == -1)
                    {
                        SendControlC();

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
                                    SendLine("rm ~/.ssh/known_hosts");
                                }

                                Thread.Sleep(500);
                                SendLine("ssh -o StrictHostKeyChecking=no " + user + "@" + host);

                                looppass = true;
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
            Result result;
            Batch batch = Batch();
            
            update = Update("Node");
            foreach (KeyValuePair<string, object> pair in updates)
            {
                if (instance.KeeperNode.ContainsKey(nodeID))
                {
                    Dictionary<string, object> keeper = instance.KeeperNode[nodeID];
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
            result = ExecuteIdentity("insert into ProbeHistory(XH_NO, XH_StartTime, XH_EndTime) values({0}, {1}, {2})", nodeID, nodeProbeStartTime, DateTime.UtcNow);
            long probeHistoryID = result.Identity;
            
            // nodesummary
            result = Query("select * from NodeSummary where NS_NO = {0}", nodeID);
            //if (!result.OK) return DatabaseFailure(probe);
            Dictionary<string, Tuple<string, string>> dbsummaries = new Dictionary<string, Tuple<string, string>>();

            batch.Begin();
            foreach (Row row in result)
            {
                string key = row["NS_Key"].ToString();
                string id = row["NS_ID"].ToString();
                string value = row["NS_Value"].ToString();

                if (dbsummaries.ContainsKey(key)) batch.Execute("delete from NodeSummary where NS_ID = {0}", id); // Duplicated summary key, remove this
                else dbsummaries.Add(key, new Tuple<string, string>(id, value));
            }
            batch.Commit();

            // old nodesummaryarchive (for migration purpose build 21)
            List<string> availablearchives = QueryList("select NS_Key from NodeSummaryArchive, NodeSummary where NSX_NS = NS_ID and NS_NO = {0}", "NS_Key", nodeID);

            batch.Begin();
            foreach (KeyValuePair<string, string> pair in summaries)
            {
                if (pair.Value == null) continue; // were not accepting null summary

                Tuple<string, string> db = null;
                if (dbsummaries.ContainsKey(pair.Key)) db = dbsummaries[pair.Key];

                if (db == null)
                {
                    string id = Database.ID();

                    insert = Insert("NodeSummary");
                    insert.Value("NS_ID", id);
                    insert.Value("NS_NO", nodeID);
                    insert.Value("NS_Key", pair.Key);
                    insert.Value("NS_Value", pair.Value);
                    batch.Execute(insert);

                    insert = Insert("NodeSummaryArchive");
                    insert.Value("NSX_XH", probeHistoryID);
                    insert.Value("NSX_NS", id);
                    insert.Value("NSX_Value", pair.Value);
                    batch.Execute(insert);

                    Event("Summary " + pair.Key + " NEW: " + pair.Value);
                }
                else
                {
                    string id = db.Item1;
                    string value = db.Item2;

                    if (pair.Value != value)
                    {
                        // summary has changed
                        // insert archive
                        insert = Insert("NodeSummaryArchive");
                        insert.Value("NSX_XH", probeHistoryID);
                        insert.Value("NSX_NS", id);
                        insert.Value("NSX_Value", pair.Value);
                        batch.Execute(insert);
                        // update nodesummary
                        update = Update("NodeSummary");
                        update.Set("NS_Value", pair.Value);
                        update.Where("NS_ID", id);
                        batch.Execute(update);

                        Event("Summary " + pair.Key + " CHANGED: " + value + " -> " + pair.Value);
                    }
                    else
                    {
                        // no change
                        // check if we dont have archive before, make one
                        if (availablearchives.IndexOf(pair.Key) == -1)
                        {
                            insert = Insert("NodeSummaryArchive");
                            insert.Value("NSX_XH", probeHistoryID); // current probehistory
                            insert.Value("NSX_NS", id);
                            insert.Value("NSX_Value", value); // current value
                            batch.Execute(insert);
                        }
                    }
                }
            }
            batch.Commit();

#if !DEBUG
            }
            catch (Exception ex)
            {
                instance.Log(nodeName, ex.Message, ex.StackTrace);
                Event("Caught error on Save(): " + ex.Message + ", try again");
                Save();
            }
#endif

            if (probeProgressID != null)
            {
                Execute("delete from ProbeProgress where XP_ID = {0}", probeProgressID);
                probeProgressID = null;
            }
        }

        private ProbeProcessResult Enter(Row row, string probeProgressID, out bool continueProcess, bool prioritizeProcess, bool prioritizeAsk)
        {
            ProbeProcessResult probe = new ProbeProcessResult();
            string[] lines = null;

            Batch batch = Batch();
            Result result = null;

            continueProcess = false;

            WaitUntilTerminalReady();

            updates = new Dictionary<string, object>();
            summaries = new Dictionary<string, string>();

            nodeID = row["NO_ID"].ToString();
            nodeName = row["NO_Name"].ToString();
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


            nodeProbeStartTime = DateTime.UtcNow;
            
            if (probeProgressID != null)
                Execute("update ProbeProgress set XP_StartTime = {0} where XP_ID = {1}", DateTime.UtcNow, this.probeProgressID);

            Event("Begin probing into " + nodeName);

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

            nodeRules = QueryList("select * from NodeAccessRule where NAR_NO = {0}", "NAR_Rule", nodeID);

            #endregion

            #region CHECK IP

            if (nodeIP != null) Event("Host IP: " + nodeIP);

            Event("Checking host IP");
            string resolvedIP = ResolveHostName(nodeName, false);

            if (nodeIP == null)
            {
                if (resolvedIP == null)
                {
                    Event("Hostname is unresolved");

                    if (previousRemark == "UNRESOLVED1" && deltaTimeStamp > TimeSpan.FromHours(2))
                    {
                        Update(UpdateTypes.Remark, "UNRESOLVED");
                        instance.DisableNode(nodeID);
                    }
                    else if (previousRemark == null || !previousRemark.StartsWith("UNRESOLVED"))
                    {
                        Update(UpdateTypes.Remark, "UNRESOLVED1");
                        instance.PendingNode(properties.Case, probeProgressID, nodeID, TimeSpan.FromHours(2));
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
                        result = Query("select * from Node where NO_Name = {0}", hostName);

                        if (result.Count == 0)
                        {
                            Event("Node " + nodeName + " has changed to " + hostName);
                            if (!NecrowVirtualization.AliasExists(nodeName))
                            {
                                Execute("insert into NodeAlias(NA_ID, NA_NO, NA_Name) values({0}, {1}, {2})", Database.ID(), nodeID, nodeName);
                                NecrowVirtualization.AliasLoad();
                            }

                            Update(UpdateTypes.Name, hostName);

                            // Update interface virtualizations
                            if (nodeType == "P")
                            {
                                Tuple<string, List<Tuple<string, string, string, string, string, string>>> changeThis = null;
                                List<Tuple<string, string, string, string, string, string>> interfaces = null;
                                lock (NecrowVirtualization.PESync)
                                {
                                    foreach (Tuple<string, List<Tuple<string, string, string, string, string, string>>> entry in NecrowVirtualization.PEPhysicalInterfaces)
                                    {
                                        if (entry.Item1 == nodeName)
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
                                        if (entry.Item1 == nodeName)
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
                            
                            nodeName = hostName;
                        }
                        else
                        {
                            Event("Node " + nodeName + " has new name " + hostName + ". " + hostName + " is already exists.");
                            Event("Mark this node as inactive");

                            Update(UpdateTypes.Remark, "NAMECONFLICTED");
                            instance.DisableNode(nodeID);

                            Save();
                            return probe;
                        }                        
                    }
                    else
                    {
                        Event("Hostname has become unresolved");

                        if (previousRemark == "UNRESOLVED1" && deltaTimeStamp > TimeSpan.FromHours(2))
                        {
                            Update(UpdateTypes.Remark, "UNRESOLVED");
                            instance.DisableNode(nodeID);
                        }
                        else if (previousRemark == null || !previousRemark.StartsWith("UNRESOLVED"))
                        {
                            Update(UpdateTypes.Remark, "UNRESOLVED1");
                            instance.PendingNode(properties.Case, probeProgressID, nodeID, TimeSpan.FromHours(2));
                        }
                        else Update(UpdateTypes.Remark, previousRemark);

                        Save();
                        return probe;
                    }
                }
                else if (nodeIP != resolvedIP)
                {
                    Event("Host IP has changed to: " + resolvedIP);

                    Update(UpdateTypes.Remark, "IPHASCHANGED");
                    instance.DisableNode(nodeID);

                    Save();

                    instance.CreateNode(nodeID);

                    return probe;
                }
            }

            Event("Host identified");

            outputIdentifier = nodeName;

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
                        connectSuccess = ConnectByTelnet(nodeName, nodeManufacture);
                        if (connectSuccess) connectBy = "T";
                        else Event("Telnet failed");
                    }
                    else if (currentConnectType == "S")
                    {
                        connectSuccess = ConnectBySSH(nodeName, nodeManufacture);
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

                        if (nodeName == "PE2-D2-CKA-VPN") testOtherNode = "PE-D2-CKA-VPN";
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
                            instance.PendingNode(properties.Case, probeProgressID, nodeID, TimeSpan.FromHours(6));
                        }
                        else if (previousRemark == "CONNECTFAIL2" && deltaTimeStamp > TimeSpan.FromHours(6))
                        {
                            Update(UpdateTypes.Remark, "CONNECTFAIL3");
                            instance.PendingNode(properties.Case, probeProgressID, nodeID, TimeSpan.FromHours(12));
                        }
                        else if (previousRemark == "CONNECTFAIL3" && deltaTimeStamp > TimeSpan.FromHours(12))
                        {
                            Update(UpdateTypes.Remark, "CONNECTFAIL");
                            instance.DisableNode(nodeID);
                        }
                        else if (previousRemark == null || !previousRemark.StartsWith("CONNECTFAIL"))
                        {
                            Update(UpdateTypes.Remark, "CONNECTFAIL1");
                            instance.PendingNode(properties.Case, probeProgressID, nodeID, TimeSpan.FromHours(2));
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
                if (nodeManufacture == null)
                {
                    SendLine("show clock");

                    WaitUntilEndsWith(new string[] { "#", ">" });

                    if (LastOutput.IndexOf("syntax error, expecting") > -1) nodeManufacture = jun;
                    else if (LastOutput.IndexOf("Unrecognized command") > -1) nodeManufacture = hwe;
                    else if (LastOutput.IndexOf("Invalid parameter") > -1)  nodeManufacture = alu;

                    if (nodeManufacture == null)
                    {
                        nodeManufacture = cso;
                    }
                }

                Event("Discovered Manufacture: " + nodeManufacture);
                instance.UpdateManufacture(nodeID, nodeManufacture);
            }

            if (nodeManufacture == null)
            {
                Event("Manufacture unknown");

                Save();

                return probe;
            }

            Event("Connected!");
            
            probing = true;

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
                    throw new Exception("This CISCO node is not in previledge mode");
                }
            }
            else if (nodeManufacture == jun)
            {
                lines = LastOutput.Split('\n');
                string lastLine = lines[lines.Length - 1];

                string[] linex = lastLine.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                terminal = linex[1];
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

                                if (tokens2[1].StartsWith("day")) day = dal;
                                else if (tokens2[1].StartsWith("hour")) hour = dal;
                                else if (tokens2[1].StartsWith("minute")) minute = dal;
                            }                            
                        }

                        uptime = new TimeSpan(day, hour, minute, 0);
                        nodeStartTimeRetrieved = true;
                        ignoreSecond = true;

                        break;
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

                Dictionary<string, SlotToDatabase> slotlive = new Dictionary<string, SlotToDatabase>();
                Dictionary<string, Row> slotdb = QueryDictionary("select * from NodeSlot where NC_NO = {0}", delegate (Row slotr)
                {
                    return slotr["NC_ID1"].ToString() + "-" + slotr["NC_ID2"].ToString("");
                }, nodeID);
                if (slotdb == null) return DatabaseFailure(probe);
                List<SlotToDatabase> slotinsert = new List<SlotToDatabase>();
                List<SlotToDatabase> slotupdate = new List<SlotToDatabase>();

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

                    if (Request("show chassis | match expression \"  Type|slots\"", out lines, probe)) return probe;

                    foreach (string line in lines)
                    {
                        string lineTrim = line.Trim();

                        if (line.StartsWith("    Type"))
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

                    if (Request("show mda", out lines, probe)) return probe;

                    string sid = null;
                    bool capturing = false;

                    foreach (string line in lines)
                    {
                        if (line.Length > 0)
                        {
                            if (char.IsDigit(line[0]))
                            {
                                capturing = true;
                            }

                            if (capturing)
                            {
                                if (line[0] != '=')
                                {
                                    string[] tokens = line.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);
                                    bool secondary = line[0] == ' ';

                                    string mda = null;
                                    string slotinfo = null;
                                    string slotadmin = null;
                                    string slotprotocol = null;

                                    if (!secondary)
                                    {
                                        sid = tokens[0];
                                        mda = tokens[1];
                                        slotinfo = tokens[2];
                                        slotadmin = tokens[3];
                                        slotprotocol = tokens[4];

                                        curSlot++;
                                    }
                                    else
                                    {
                                        mda = tokens[0];
                                        slotinfo = tokens[1];
                                        slotadmin = tokens[2];
                                        slotprotocol = tokens[3];
                                    }

                                    SlotToDatabase li = new SlotToDatabase();
                                    li.SlotID1 = sid;
                                    li.SlotID2 = mda;
                                    li.Info1 = slotinfo;
                                    li.Info2 = slotadmin;
                                    li.Info3 = slotprotocol;

                                    slotlive.Add(sid + "-" + mda, li);

                                }
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
                        if (line.StartsWith("VRP (R) software"))
                            version = line.Substring(26, line.IndexOf(' ', 26) - 26).Trim();
                        if (line.StartsWith("HUAWEI "))
                            model = line.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries)[1];
                        if (version != null && model != null) break;
                    }

                    if (Request("display version | in LPU|StartupTime|Version", out lines, probe)) return probe;

                    string lpu = null;
                    string startuptime = null;
                    string pcb = null;

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
                            else if (lineTrim.StartsWith("LPU "))
                            {
                                curSlot++;

                                string[] tokens = lineTrim.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);

                                if (tokens.Length > 1)
                                {
                                    string s = tokens[1].Trim();
                                    if (int.TryParse(s, out int c))
                                    {
                                        lpu = s;
                                        startuptime = null;
                                        pcb = null;
                                    }
                                }
                            }
                            else if (lpu != null)
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
                                            startuptime = startupDateTime.ToEpochTime() + "";
                                        }
                                    }
                                }
                                else if (line.IndexOf("PCB") > -1 && line.IndexOf("Version :") > -1)
                                {
                                    string[] tokens = lineTrim.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                                    if (tokens.Length == 2)
                                    {
                                        pcb = tokens[1].Trim();

                                        SlotToDatabase li = new SlotToDatabase();
                                        li.SlotID1 = lpu;
                                        li.Info1 = pcb;
                                        li.Info2 = startuptime;

                                        slotlive.Add(lpu + "-", li);

                                        lpu = null;
                                    }
                                }
                            }
                        }
                    }




                    // additional setup for huawei >5.90 for screen-width tweak (help problem with 5.160 auto text-wrap)
                    //if (version == "5.160")
                    //{
                    //    if (Send("screen-width 80" + (char)13 + "Y")) { NodeStop(); return true; }
                    //    NodeRead(out timeout);
                    //    if (timeout) { NodeStop(); return true; }
                    //}

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
                    throw new Exception("Cant determined node version");
                }
                if (subVersion != nodeSubVersion)
                {
                    nodeSubVersion = subVersion;
                    Update(UpdateTypes.SubVersion, subVersion);
                    Event("SubVersion updated: " + subVersion);
                }

                #region Check

                foreach (KeyValuePair<string, SlotToDatabase> pair in slotlive)
                {
                    SlotToDatabase li = pair.Value;

                    if (!slotdb.ContainsKey(pair.Key))
                    {
                        Event("Slot ADD: " + pair.Key);

                        li.ID = Database.ID();
                        slotinsert.Add(li);
                    }
                    else
                    {
                        Row db = slotdb[pair.Key];

                        SlotToDatabase u = new SlotToDatabase();
                        u.ID = db["NC_ID"].ToString();
                        li.ID = u.ID;

                        bool update = false;
                        StringBuilder updateinfo = new StringBuilder();

                        if (db["NC_Info1"].ToString() != li.Info1)
                        {
                            update = true;
                            u.UpdateInfo1 = true;
                            u.Info1 = li.Info1;
                            UpdateInfo(updateinfo, "info1", db["NC_Info1"].ToString(), li.Info1);
                        }
                        if (db["NC_Info2"].ToString() != li.Info2)
                        {
                            update = true;
                            u.UpdateInfo2 = true;
                            u.Info2 = li.Info2;
                            UpdateInfo(updateinfo, "info2", db["NC_Info2"].ToString(), li.Info2);
                        }
                        if (db["NC_Info3"].ToString() != li.Info3)
                        {
                            update = true;
                            u.UpdateInfo3 = true;
                            u.Info3 = li.Info3;
                            UpdateInfo(updateinfo, "info3", db["NC_Info3"].ToString(), li.Info3);
                        }
                        if (db["NC_Info4"].ToString() != li.Info4)
                        {
                            update = true;
                            u.UpdateInfo4= true;
                            u.Info4 = li.Info4;
                            UpdateInfo(updateinfo, "info4", db["NC_Info4"].ToString(), li.Info4);
                        }

                        if (update)
                        {
                            Event("Slot UPDATE: " + pair.Key + " " + updateinfo.ToString());
                            slotupdate.Add(u);
                        }
                    }
                }

                #endregion

                #region Execute

                // ADD
                batch.Begin();
                foreach (SlotToDatabase s in slotinsert)
                {
                    Insert insert = Insert("NodeSlot");
                    insert.Value("NC_ID", s.ID);
                    insert.Value("NC_NO", nodeID);
                    insert.Value("NC_ID1", s.SlotID1);
                    insert.Value("NC_ID2", s.SlotID2);
                    insert.Value("NC_Info1", s.Info1);
                    insert.Value("NC_Info2", s.Info2);
                    insert.Value("NC_Info3", s.Info3);
                    insert.Value("NC_Info4", s.Info4);
                    batch.Execute(insert);
                }
                result = batch.Commit();
                if (!result.OK) return DatabaseFailure(probe);
                Event(result, EventActions.Add, EventElements.Slot, false);

                // UPDATE
                batch.Begin();
                foreach (SlotToDatabase s in slotupdate)
                {
                    Update update = Update("NodeSlot");
                    update.Set("NC_Info1", s.Info1, s.UpdateInfo1);
                    update.Set("NC_Info2", s.Info2, s.UpdateInfo2);
                    update.Set("NC_Info3", s.Info3, s.UpdateInfo3);
                    update.Set("NC_Info4", s.Info4, s.UpdateInfo4);
                    update.Where("NC_ID", s.ID);
                    batch.Execute(update);
                }
                result = batch.Commit();
                if (!result.OK) return DatabaseFailure(probe);
                Event(result, EventActions.Update, EventElements.Slot, false);

                // SDP DELETE
                batch.Begin();
                foreach (KeyValuePair<string, Row> pair in slotdb)
                {
                    if (!slotlive.ContainsKey(pair.Key))
                    {
                        Event("Slot DELETE: " + pair.Key);
                        batch.Execute("delete from NodeSlot where NC_ID = {0}", pair.Value["NC_ID"].ToString());
                    }
                }
                result = batch.Commit();
                if (!result.OK) return DatabaseFailure(probe);
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
                Summary("CPU", cpu);
            }
            else
            {
                Event("CPU load is unavailable (" + cpu + ")");
                Summary("CPU", null);
            }
            if (mtotal > -1)
            {
                Event("Memory total = " + mtotal + "KB");
                Summary("MEMORY_TOTAL", mtotal);
            }
            else
            {
                Event("Memory total is unavailable");
                Summary("MEMORY_TOTAL", null);
            }
            if (mused > -1)
            {
                Event("Memory used = " + mused + "KB");
                Summary("MEMORY_USED", mused);
            }
            else
            {
                Event("Memory used is unavailable");
                Summary("MEMORY_USED", null);
            }

            #endregion

            #endregion

            if (properties.Case == "MAIN")
            {
                if (configurationHasChanged)
                {
                    continueProcess = true;
                }
                else if (prioritizeProcess)
                {
                    Event("Prioritized node, continuing process");
                    continueProcess = true;
                }
                else if (updatingNecrow)
                {
                    continueProcess = true;
                }
                else if (prioritizeAsk)
                {
                    probeRequestData.Message.Data = new object[] { nodeName, "UPTODATE", lastConfLive };
                    probeRequestData.Connection.Reply(probeRequestData.Message);

                    SaveExit();
                }
                else
                {
                    SaveExit();
                }
            }
            else if (properties.Case == "M")
            {
                continueProcess = true;
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

        private void ServiceDiscovery(ServiceReference reference)
        {
            Batch batch = Batch();
            Result result;

            List<Tuple<ServiceBaseToDatabase, ServiceMapping>> mappings = reference.Mappings;
            List<string> customerid = new List<string>();
            List<string> serviceid = new List<string>();
            List<string> servicebycustomerid = new List<string>();
            Dictionary<string, Row> customerdb = new Dictionary<string, Row>();
            Dictionary<string, Row> servicedb = new Dictionary<string, Row>();
            Dictionary<string, List<Row>> servicebycustomerdb = new Dictionary<string, List<Row>>();
            List<CustomerToDatabase> customerinsert = new List<CustomerToDatabase>();
            List<CustomerToDatabase> customerupdate = new List<CustomerToDatabase>();
            List<ServiceToDatabase> serviceinsert = new List<ServiceToDatabase>();
            List<ServiceToDatabase> serviceupdate = new List<ServiceToDatabase>();

            foreach (Tuple<ServiceBaseToDatabase, ServiceMapping> tuple in mappings)
            {
                string sid = tuple.Item2.SID;
                string cid = tuple.Item2.CID;
                if (serviceid.IndexOf(sid) == -1) serviceid.Add(sid);
                if (cid != null && customerid.IndexOf(cid) == -1) customerid.Add(cid);
            }

            Result customerresult = Query("select * from ServiceCustomer where SC_CID in {0}", customerid);
            foreach (Row row in customerresult)
            {
                string c_id = row["SC_ID"].ToString();

                customerdb.Add(row["SC_CID"].ToString(), row);
                if (!row["SC_Name_Set"].ToBool(false)) servicebycustomerid.Add(row["SC_ID"].ToString());

                if (!servicebycustomerdb.ContainsKey(c_id)) servicebycustomerdb.Add(c_id, new List<Row>());
            }
            Result serviceresult = Query("select * from Service where SE_SID in {0} or SE_SC in {1}", serviceid, servicebycustomerid);
            foreach (Row row in serviceresult)
            {
                servicedb.Add(row["SE_SID"].ToString(), row);
                string c_id = row["SE_SC"].ToString();
                if (c_id != null)
                {
                    if (servicebycustomerdb.ContainsKey(c_id)) servicebycustomerdb[c_id].Add(row);
                    else
                    {
                        List<Row> lrow = new List<Row>();
                        lrow.Add(row);
                        servicebycustomerdb.Add(c_id, lrow);
                    }
                }
            }
            foreach (Tuple<ServiceBaseToDatabase, ServiceMapping> tuple in mappings)
            {
                string sid = tuple.Item2.SID;
                string cid = tuple.Item2.CID;
                string stype = tuple.Item2.ServiceType;
                string ssubtype = tuple.Item2.ServiceSubType;
                string cdesc = tuple.Item2.CleanDescription;
                
                string s_type = null, s_subtype = null;
                if (stype == "VPNIP")
                {
                    s_type = "VP";
                    if (ssubtype == "TRANS") s_subtype = "TA";
                }
                else if (stype == "ASTINET") s_type = "AS";
                else if (stype == "ASTINETBB") s_type = "AB";
                else if (stype == "VPNINSTAN") s_type = "VI";
                else if (stype == "TELKOMSELSITES") { s_type = "TS"; s_subtype = "SI"; }

                string c_id = null, c_name = null, s_id = null;

                #region sc
                if (cid != null)
                {
                    if (customerdb.ContainsKey(cid))
                    {
                        c_id = customerdb[cid]["SC_ID"].ToString();
                        c_name = customerdb[cid]["SC_Name"].ToString();
                    }
                    else
                    {
                        c_id = Database.ID();
                        c_name = cdesc;

                        Row ncdb = new Row();
                        ncdb.Add("SC_ID", new Column("SC_ID", c_id, false));
                        ncdb.Add("SC_CID", new Column("SC_CID", cid, false));
                        ncdb.Add("SC_Name", new Column("SC_Name", c_name, false));
                        ncdb.Add("SC_Name_Set", new Column("SC_Name_Set", null, true));
                        customerdb.Add(cid, ncdb);

                        Event("Customer ADD: " + c_name + " (" + cid + ")");
                        CustomerToDatabase c = new CustomerToDatabase();
                        c.ID = c_id;
                        c.CID = cid;
                        c.Name = c_name;
                        customerinsert.Add(c);

                        servicebycustomerdb.Add(c_id, new List<Row>());
                    }
                }
                #endregion

                #region se

                tuple.Item1.ServiceSID = sid;

                if (servicedb.ContainsKey(sid))
                {
                    s_id = servicedb[sid]["SE_ID"].ToString();

                    if (servicedb[sid]["SE_Type"].ToString() == null && s_type != null)
                    {
                        Event("Service UPDATE: " + sid + " " + s_type + " " + s_subtype);
                        ServiceToDatabase c = new ServiceToDatabase();
                        c.ID = s_id;
                        c.Type = s_type;
                        c.SubType = s_subtype;
                        serviceupdate.Add(c);
                    }

                    tuple.Item1.ServiceID = s_id;
                    
                }
                else
                {
                    s_id = Database.ID();

                    Row ndb = new Row();
                    ndb.Add("SE_ID", new Column("SE_ID", s_id, false));
                    ndb.Add("SE_SID", new Column("SE_SID", sid, false));
                    ndb.Add("SE_SC", new Column("SE_SC", c_id, false));
                    ndb.Add("SE_Type", new Column("SE_Type", s_type, s_type == null ? true : false));
                    ndb.Add("SE_SubType", new Column("SE_SubType", s_subtype, s_subtype == null ? true : false));
                    ndb.Add("SE_Raw_Desc", new Column("SE_Raw_Desc", cdesc, cdesc == null ? true : false));
                    servicedb.Add(sid, ndb);

                    Event("Service ADD: " + sid + " (" + cid + ")");
                    ServiceToDatabase c = new ServiceToDatabase();
                    c.ID = s_id;
                    c.SID = sid;
                    c.CustomerID = c_id;
                    c.Type = s_type;
                    c.SubType = s_subtype;
                    c.RawDesc = cdesc;
                    serviceinsert.Add(c);

                    //set interface to this service
                    tuple.Item1.ServiceID = s_id;

                    if (c_id != null)
                        servicebycustomerdb[c_id].Add(ndb);
                }
                #endregion

                #region Name Processing

                if (c_id != null && c_name != "TELKOMSELSITES")
                {
                    List<Row> rownems = servicebycustomerdb[c_id];

                    if (rownems.Count > 1)
                    {
                        List<string> nems = new List<string>();

                        foreach (Row rownem in rownems)
                        {
                            string[] rds = rownem["SE_Raw_Desc"].ToString()
                                .Split(
                                new string[] { ",", "U/", "JL.", "JL ", "(", "[", "]", ")", "LT.", " LT ", " LT",
                                            "GD.", " KM", " KOMP.", " BLOK ",
                                            " SID ", " SID:", " SID-", " SID=",
                                            " CID ", " CID:", " CID-", " CID=", " CID.", "EXCID", " JL", " EX ",
                                            " FAA:", " FAI:", " FAA-", " FAI-", " CINTA ",
                                            " EX-",
                                            " KK ", "TBK", "BANDWIDTH",  },
                                StringSplitOptions.RemoveEmptyEntries);

                            if (rds.Length > 0)
                                nems.Add(rds[0].Trim());
                        }

                        Dictionary<string, int> lexicals = new Dictionary<string, int>();

                        int totaln = 0;
                        foreach (string nem in nems)
                        {
                            string[] nemp = nem.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);
                            for (int ni = 0; ni < nemp.Length; ni++)
                            {
                                for (int nj = 1; nj <= nemp.Length - ni; nj++)
                                {
                                    string[] subn = new string[nj];
                                    Array.Copy(nemp, ni, subn, 0, nj);
                                    string sub = string.Join(" ", subn);
                                    if (lexicals.ContainsKey(sub))
                                    {
                                        lexicals[sub] += 1;
                                        totaln++;
                                    }
                                    else
                                        lexicals.Add(sub, 1);
                                }
                            }
                        }

                        List<KeyValuePair<string, int>> lexicalList = lexicals.ToList();

                        if (lexicalList.Count > 0)
                        {
                            lexicalList.Sort((a, b) =>
                            {
                                if (a.Value > b.Value) return -1;
                                else if (a.Value < b.Value) return 1;
                                else
                                {
                                    if (a.Key.Length > b.Key.Length) return -1;
                                    else if (a.Key.Length < b.Key.Length) return 1;
                                    else return 0;
                                }
                            });

                            string cname = lexicalList[0].Key;

                            if (lexicalList[0].Value > 1)
                            {
                                for (int li = 0; li < (lexicalList.Count > 10 ? 10 : lexicalList.Count); li++)
                                {
                                    KeyValuePair<string, int> lip = lexicalList[li];

                                    string likey = lip.Key;
                                    int lival = lip.Value;
                                    int likeylen = StringHelper.CountWord(likey);
                                    bool lolos = true;
                                    for (int ly = li + 1; ly < (lexicalList.Count > 10 ? 10 : lexicalList.Count); ly++)
                                    {
                                        KeyValuePair<string, int> lyp = lexicalList[ly];

                                        string lykey = lyp.Key;
                                        int lyval = lyp.Value;
                                        int lykeylen = StringHelper.CountWord(lykey);

                                        if (lykey.Length > likey.Length)
                                        {
                                            if (likeylen > 1)
                                            {
                                                if (lykey.IndexOf(likey) > -1)
                                                {
                                                    int distance = lival - lyval;

                                                    double dtotaln = (double)totaln;
                                                    double minx = Math.Pow((1 / Math.Log(0.08 * dtotaln + 3)), 1.75);
                                                    if (((double)distance / dtotaln) > minx) { }
                                                    else lolos = false;
                                                    break;
                                                }
                                            }
                                            else
                                            {
                                                if (lykey.IndexOf(likey) > -1)
                                                {
                                                    if ((ly - li) < 4)
                                                    {
                                                        int distance = lival - lyval;

                                                        double dtotaln = (double)totaln;
                                                        double minx = Math.Pow((1 / Math.Log(0.08 * dtotaln + 3)), 1.75);
                                                        if (((double)distance / dtotaln) > minx) { }
                                                        else lolos = false;
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        lolos = false;
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (lykeylen == 1) { }
                                            else
                                            {
                                                if (lykeylen < likeylen) { if (likey.IndexOf(lykey) > -1) break; }
                                                else break;
                                            }
                                        }
                                    }

                                    if (lolos)
                                    {
                                        cname = likey;
                                        break;
                                    }
                                }
                            }

                            if (cname != null)
                            {
                                cname = cname.Trim(new char[] { ' ', '\"', '\'', '&', '(', ')', '-' });
                                cname = cname.Replace("PT.", "PT");
                                cname = cname.Replace(" PT", "");
                                cname = cname.Replace("PT ", "");

                                if (cname != c_name)
                                {
                                    Event("Customer UPDATE: " + cname + " (" + cid + ")");

                                    customerdb.Remove(cid);

                                    Row ncdb = new Row();
                                    ncdb.Add("SC_ID", new Column("SC_ID", c_id, false));
                                    ncdb.Add("SC_CID", new Column("SC_CID", cid, false));
                                    ncdb.Add("SC_Name", new Column("SC_Name", cname, false));
                                    ncdb.Add("SC_Name_Set", new Column("SC_Name_Set", null, true));

                                    customerdb.Add(cid, ncdb);

                                    CustomerToDatabase c = new CustomerToDatabase();
                                    c.ID = c_id;
                                    c.Name = cname;
                                    customerupdate.Add(c);
                                }
                            }

                        }
                    }
                }
                #endregion
            }

            // CUSTOMER ADD
            batch.Begin();
            foreach (CustomerToDatabase s in customerinsert)
            {
                Insert insert = Insert("ServiceCustomer");
                insert.Value("SC_ID", s.ID);
                insert.Value("SC_CID", s.CID);
                insert.Value("SC_Name", s.Name);
                batch.Execute(insert);
            }
            result = batch.Commit();
            Event(result, EventActions.Add, EventElements.Customer, false);

            // CUSTOMER UPDATE
            batch.Begin();
            foreach (CustomerToDatabase s in customerupdate)
            {
                batch.Execute("update ServiceCustomer set SC_Name = {0} where SC_ID = {1}", s.Name, s.ID);
            }
            result = batch.Commit();
            Event(result, EventActions.Update, EventElements.Customer, false);

            // SERVICE ADD
            batch.Begin();
            foreach (ServiceToDatabase s in serviceinsert)
            {
                Insert insert = Insert("Service");
                insert.Value("SE_ID", s.ID);
                insert.Value("SE_SID", s.SID);
                insert.Value("SE_SC", s.CustomerID);
                insert.Value("SE_Type", s.Type);
                insert.Value("SE_SubType", s.SubType);
                insert.Value("SE_Raw_Desc", s.RawDesc);
                batch.Execute(insert);
            }
            result = batch.Commit();
            Event(result, EventActions.Add, EventElements.Service, false);

            // SERVICE UPDATE
            batch.Begin();
            foreach (ServiceToDatabase s in serviceupdate)
            {
                Update update = Update("Service");
                update.Set("SE_Type", s.Type);
                update.Set("SE_SubType", s.SubType);
                update.Where("SE_ID", s.ID);
                batch.Execute(update);
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

            if (type != null && instance.InterfaceTestPrefixes.ContainsKey(type))
            {
                foreach (string prefix in instance.InterfaceTestPrefixes[type])
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

            string description = CleanDescription(li.Description, nodeName);
            if (description == null) return;

            string interfaceName = li.Name;
            string interfaceType = li.InterfaceType;
            string interfacePort = interfaceName.Substring(2);

            bool done = false;

            Result result;
            Batch batch = Batch();
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
                            string matchedNeighborPart = FindNeighborPart(matchedDescription, nodeName);

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
                                            result = Query("select PI_ID, PI_DOT1Q, PI_TO_MI from PEInterface where PI_PI = {0}", mi.TopologyPEInterfaceID);
                                            foreach (Row row in result)
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
                        string matchedNeighborPart = FindNeighborPart(matchedDescription, nodeName);

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
                                        result = Query("select MI_ID, MI_DOT1Q, MI_TO_MI, MI_TO_PI from MEInterface where MI_MI = {0}", li.TopologyMEInterfaceID);
                                        foreach (Row row in result)
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
                        neighborNodeID = Database.ID();
                        insert = Insert("NodeNeighbor");
                        insert.Value("NN_ID", neighborNodeID);
                        insert.Value("NN_Name", findNeighborNode);
                        batch.Execute(insert);

                        // insert neighbor interface unspecified
                        string unspecifiedID = Database.ID();
                        insert = Insert("NBInterface");
                        insert.Value("NI_ID", unspecifiedID);
                        insert.Value("NI_NN", neighborNodeID);
                        insert.Value("NI_Name", "UNSPECIFIED");
                        batch.Execute(insert);

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
                            string interfaceID = Database.ID();
                            insert = Insert("NBInterface");
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

            string[] tokens = input.Split(new string[] { "        " }, StringSplitOptions.RemoveEmptyEntries);

            StringBuilder sb = new StringBuilder();
            foreach (string token in tokens)
            {
                sb.Append(token.TrimStart());
            }

            return sb.ToString();
        }
    }

    internal class ServiceReference
    {
        #region Fields

        private List<Tuple<ServiceBaseToDatabase, ServiceMapping>> mappings;

        public List<Tuple<ServiceBaseToDatabase, ServiceMapping>> Mappings
        {
            get { return mappings; }
        }

        #endregion

        #region Constructors

        public ServiceReference()
        {
            mappings = new List<Tuple<ServiceBaseToDatabase, ServiceMapping>>();
        }

        #endregion

        #region Methods

        public void Add(ServiceBaseToDatabase reference, string description)
        {
            ServiceMapping servmap = ServiceMapping.Parse(description);
            if (servmap.SID != null)
                mappings.Add(new Tuple<ServiceBaseToDatabase, ServiceMapping>(reference, servmap));
        }

        #endregion
    }

    internal class ServiceMapping
    {
        #region Constants

        private static readonly string[] monthsEnglish = new string[] { "JANUARY", "FEBRUARY", "MARCH", "APRIL", "MAY", "JUNE", "JULY", "AUGUST", "SEPTEMBER", "OCTOBER", "NOVEMBER", "DECEMBER" };
        private static readonly string[] monthsBahasa = new string[] { "JANUARI", "FEBRUARI", "MARET", "APRIL", "MEI", "JUNI", "JULI", "AGUSTUS", "SEPTEMBER", "OKTOBER", "NOVEMBER", "DESEMBER" };
        private static readonly Regex findSiteID = new Regex(@"([A-Z][A-Z][A-Z][0-9][0-9][0-9])");

        #endregion

        #region Properties

        private string sid;

        public string SID
        {
            get { return sid; }
            private set { sid = value; }
        }

        private string cid;

        public string CID
        {
            get { return cid; }
            set { cid = value; }
        }

        private string serviceType;

        public string ServiceType
        {
            get { return serviceType; }
            private set { serviceType = value; }
        }

        private string serviceSubType;

        public string ServiceSubType
        {
            get { return serviceSubType; }
            set { serviceSubType = value; }
        }

        private string cleanDescription;

        public string CleanDescription
        {
            get { return cleanDescription; }
            private set { cleanDescription = value; }
        }

        private string rawDescription;

        public string RawDescription
        {
            get { return rawDescription; }
            private set { rawDescription = value; }
        }

        #endregion

        #region Methods

        public static ServiceMapping Parse(string desc)
        {
            ServiceMapping de = new ServiceMapping();
            de.RawDescription = desc;

            string[] s = desc.Split(new char[] { '_', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string d = string.Join(" ", s).ToUpper();

            #region Find SID

            int rmv = -1;
            int rle = -1;

            //                         12345678901234567890
            if ((rmv = d.IndexOf("MM IPVPN INSTAN ")) > -1) { de.ServiceType = "VPNINSTAN"; rle = 15; }
            else if ((rmv = d.IndexOf("MM VPNIP INSTAN ")) > -1) { de.ServiceType = "VPNINSTAN"; rle = 15; }
            else if ((rmv = d.IndexOf("VPNIP INSTANT ")) > -1) { de.ServiceType = "VPNINSTAN"; rle = 13; }
            else if ((rmv = d.IndexOf("IPVPN INSTANT ")) > -1) { de.ServiceType = "VPNINSTAN"; rle = 13; }
            else if ((rmv = d.IndexOf("VPNIP INSTAN ")) > -1) { de.ServiceType = "VPNINSTAN"; rle = 12; }
            else if ((rmv = d.IndexOf("IPVPN INSTAN ")) > -1) { de.ServiceType = "VPNINSTAN"; rle = 12; }
            else if ((rmv = d.IndexOf("VPNIP VPN IP ")) > -1) { de.ServiceType = "VPNIP"; rle = 12; }
            else if ((rmv = d.IndexOf("VPNIP VPNIP ")) > -1) { de.ServiceType = "VPNIP"; rle = 11; }
            else if ((rmv = d.IndexOf("VPN INSTAN ")) > -1) { de.ServiceType = "VPNINSTAN"; rle = 10; }
            else if ((rmv = d.IndexOf("MM IPVPN ")) > -1) { de.ServiceType = "VPNIP"; rle = 8; }
            else if ((rmv = d.IndexOf("MM VPNIP ")) > -1) { de.ServiceType = "VPNIP"; rle = 8; }
            else if ((rmv = d.IndexOf("MM VPNIP ")) > -1) { de.ServiceType = "VPNIP"; rle = 8; }
            else if ((rmv = d.IndexOf("MM VPNIP ")) > -1) { de.ServiceType = "VPNIP"; rle = 8; }
            else if ((rmv = d.IndexOf("VPN IP ")) > -1) { de.ServiceType = "VPNIP"; rle = 6; }
            else if ((rmv = d.IndexOf("IP VPN ")) > -1) { de.ServiceType = "VPNIP"; rle = 6; }
            else if ((rmv = d.IndexOf("VPNIP ")) > -1) { de.ServiceType = "VPNIP"; rle = 5; }
            else if ((rmv = d.IndexOf("IPVPN ")) > -1) { de.ServiceType = "VPNIP"; rle = 5; }
            //                         12345678901234567890
            else if ((rmv = d.IndexOf("MM ASTINET BEDA BW ")) > -1) { de.ServiceType = "ASTINETBB"; rle = 18; }
            else if ((rmv = d.IndexOf("MM ASTINET BEDABW ")) > -1) { de.ServiceType = "ASTINETBB"; rle = 17; }
            else if ((rmv = d.IndexOf("ASTINET BEDA BW ")) > -1) { de.ServiceType = "ASTINETBB"; rle = 15; }
            else if ((rmv = d.IndexOf("ASTINET BEDABW ")) > -1) { de.ServiceType = "ASTINETBB"; rle = 14; }
            else if ((rmv = d.IndexOf("MM ASTINET BB ")) > -1) { de.ServiceType = "ASTINETBB"; rle = 13; }
            else if ((rmv = d.IndexOf("ASTINET BB ")) > -1) { de.ServiceType = "ASTINETBB"; rle = 10; }
            else if ((rmv = d.IndexOf("ASTINETBB ")) > -1) { de.ServiceType = "ASTINETBB"; rle = 9; }
            else if ((rmv = d.IndexOf("AST BEDA BW ")) > -1) { de.ServiceType = "ASTINETBB"; rle = 11; }
            else if ((rmv = d.IndexOf("AST BEDABW ")) > -1) { de.ServiceType = "ASTINETBB"; rle = 10; }
            else if ((rmv = d.IndexOf("AST BB ")) > -1) { de.ServiceType = "ASTINETBB"; rle = 7; }
            //                         12345678901234567890
            else if ((rmv = d.IndexOf("MM ASTINET ")) > -1) { de.ServiceType = "ASTINET"; rle = 10; }
            else if ((rmv = d.IndexOf("ASTINET ")) > -1) { de.ServiceType = "ASTINET"; rle = 7; }
            //                         12345678901234567890
            else rmv = -1;

            if (rmv > -1) d = d.Remove(rmv, rle);
            rmv = -1;
            rle = -1;

            if (de.ServiceType == null || de.ServiceType == "VPNIP")
            {
                //                         12345678901234567890
                if ((rmv = d.IndexOf("VPNIP TRANS ACCESS ")) > -1) { de.ServiceType = "VPNIP"; rle = 18; de.ServiceSubType = "TRANS"; }
                else if ((rmv = d.IndexOf("TRANSACTIONAL ACCESS ")) > -1) { de.ServiceType = "VPNIP"; rle = 20; de.ServiceSubType = "TRANS"; }
                else if ((rmv = d.IndexOf("MM TRANS ACCESS ")) > -1) { de.ServiceType = "VPNIP"; rle = 15; de.ServiceSubType = "TRANS"; }
                else if ((rmv = d.IndexOf("TRANSS ACCESS ")) > -1) { de.ServiceType = "VPNIP"; rle = 13; de.ServiceSubType = "TRANS"; }
                else if ((rmv = d.IndexOf("TRANS ACCESS ")) > -1) { de.ServiceType = "VPNIP"; rle = 12; de.ServiceSubType = "TRANS"; }
                else if ((rmv = d.IndexOf("TRANSACCESS ")) > -1) { de.ServiceType = "VPNIP"; rle = 11; de.ServiceSubType = "TRANS"; }
                else if ((rmv = d.IndexOf("TRANS ACCES ")) > -1) { de.ServiceType = "VPNIP"; rle = 11; de.ServiceSubType = "TRANS"; }
                else if ((rmv = d.IndexOf("TRANSACC ")) > -1) { de.ServiceType = "VPNIP"; rle = 8; de.ServiceSubType = "TRANS"; }
                else if ((rmv = d.IndexOf("TRAN ACC ")) > -1) { de.ServiceType = "VPNIP"; rle = 8; de.ServiceSubType = "TRANS"; }
                else if ((rmv = d.IndexOf("TRANS AC ")) > -1) { de.ServiceType = "VPNIP"; rle = 8; de.ServiceSubType = "TRANS"; }
                else if ((rmv = d.IndexOf("TRANSAC ")) > -1) { de.ServiceType = "VPNIP"; rle = 7; de.ServiceSubType = "TRANS"; }
                else if ((rmv = d.IndexOf("TRANSC ")) > -1) { de.ServiceType = "VPNIP"; rle = 6; de.ServiceSubType = "TRANS"; }
                else if ((rmv = d.IndexOf("TRANSS ")) > -1) { de.ServiceType = "VPNIP"; rle = 6; de.ServiceSubType = "TRANS"; }
                else if ((rmv = d.IndexOf("TRANS ")) > -1 && de.ServiceType == null) { de.ServiceType = "VPNIP"; rle = 5; de.ServiceSubType = "TRANS"; }
                else rmv = -1;

                if (rmv > -1) d = d.Remove(rmv, rle);
                rmv = -1;
                rle = -1;
            }

            rmv = -1;
            rle = -1;

            d = d.Trim();

            //                         12345678901234567890
            if ((rmv = d.IndexOf("(EX SID FEAS")) > -1) { rle = 12; }
            else if ((rmv = d.IndexOf("[EX SID FEAS")) > -1) { rle = 12; }
            else if ((rmv = d.IndexOf("EX SID FEAS")) > -1) { rle = 11; }
            else if ((rmv = d.IndexOf("EX SID FEAS")) > -1) { rle = 11; }
            else if ((rmv = d.IndexOf("SID (FEAS)")) > -1) { rle = 10; }
            else if ((rmv = d.IndexOf("SID [FEAS]")) > -1) { rle = 10; }
            else if ((rmv = d.IndexOf("SID <FEAS>")) > -1) { rle = 10; }
            else if ((rmv = d.IndexOf("(SID FEAS")) > -1) { rle = 9; }
            else if ((rmv = d.IndexOf("[SID FEAS")) > -1) { rle = 9; }
            else if ((rmv = d.IndexOf("SID FEAS")) > -1) { rle = 8; }
            else if ((rmv = d.IndexOf("(EX FEAS")) > -1) { rle = 8; }
            else if ((rmv = d.IndexOf("[EX FEAS")) > -1) { rle = 8; }
            else if ((rmv = d.IndexOf("(EX SID")) > -1) { rle = 7; }
            else if ((rmv = d.IndexOf("[EX SID")) > -1) { rle = 7; }
            else if ((rmv = d.IndexOf("EX FEAS")) > -1) { rle = 7; }
            else if ((rmv = d.IndexOf("EX-SID")) > -1) { rle = 6; }
            else if ((rmv = d.IndexOf("EX SID")) > -1) { rle = 6; }
            else if ((rmv = d.IndexOf("X-SID")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf("X SID")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf("EXSID")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf("XSID3")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf("XSID4")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf("XSID ")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf("FEAS ")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf("VLAN ")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf(" EX3")) > -1) { rle = 3; }
            else if ((rmv = d.IndexOf(" EX4")) > -1) { rle = 3; }
            else if ((rmv = d.IndexOf("(EX3")) > -1) { rle = 3; }
            else if ((rmv = d.IndexOf("(EX4")) > -1) { rle = 3; }
            else if ((rmv = d.IndexOf("[EX3")) > -1) { rle = 3; }
            else if ((rmv = d.IndexOf("[EX4")) > -1) { rle = 3; }
            else if ((rmv = d.IndexOf("<EX3")) > -1) { rle = 3; }
            else if ((rmv = d.IndexOf("<EX4")) > -1) { rle = 3; }

            if (rmv > -1)
            {
                int rmvn = rmv + rle;
                if (rmvn < d.Length)
                {
                    if (d[rmvn] == ' ') rmvn += 1;
                    else if (d[rmvn] == ':' || d[rmvn] == '-' || d[rmvn] == '=')
                    {
                        rmvn += 1;
                        if (rmvn < d.Length && d[rmvn] == ' ') rmvn += 1;
                    }
                }
                if (rmvn < d.Length)
                {
                    int end = d.IndexOfAny(new char[] { ' ', ')', '(', ']', '[', '.', '<', '>' }, rmvn);
                    if (end > -1) d = d.Remove(rmv, end - rmv);
                    else d = d.Remove(rmv);
                }
            }
            rmv = -1;
            rle = -1;

            //                         12345678901234567890
            if ((rmv = d.IndexOf("SID-TENOSS-")) > -1) { rle = 11; }
            else if ((rmv = d.IndexOf("SID-TENOSS:")) > -1) { rle = 11; }
            else if ((rmv = d.IndexOf("SID-TENOSS=")) > -1) { rle = 11; }
            else if ((rmv = d.IndexOf("SID-TENOSS ")) > -1) { rle = 11; }
            else if ((rmv = d.IndexOf("SID TENOSS:")) > -1) { rle = 11; }
            else if ((rmv = d.IndexOf("SID TENOSS=")) > -1) { rle = 11; }
            else if ((rmv = d.IndexOf("SID TENOSS ")) > -1) { rle = 11; }
            else if ((rmv = d.IndexOf("TENOSS-SID-")) > -1) { rle = 11; }
            else if ((rmv = d.IndexOf("TENOSS-SID:")) > -1) { rle = 11; }
            else if ((rmv = d.IndexOf("TENOSS-SID=")) > -1) { rle = 11; }
            else if ((rmv = d.IndexOf("TENOSS-SID ")) > -1) { rle = 11; }
            else if ((rmv = d.IndexOf("TENOSS SID:")) > -1) { rle = 11; }
            else if ((rmv = d.IndexOf("TENOSS SID=")) > -1) { rle = 11; }
            else if ((rmv = d.IndexOf("TENOSS SID ")) > -1) { rle = 11; }
            else if ((rmv = d.IndexOf("SID SID ")) > -1) { rle = 7; }
            else if ((rmv = d.IndexOf("-SOID-")) > -1) { rle = 6; }
            else if ((rmv = d.IndexOf("(SID-")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf("(SID:")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf("(SID=")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf("(SID%")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf("(SID ")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf("<SID-")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf("<SID:")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf("<SID=")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf("<SID%")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf("<SID ")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf("[SID-")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf("[SID:")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf("[SID=")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf("[SID%")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf("[SID ")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf(" SID-")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf(" SID:")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf(" SID=")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf(" SID%")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf(" SID ")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf(" SIDT")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf(" SID0")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf(" SID1")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf(" SID2")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf(" SID3")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf(" SID4")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf(" SID5")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf(" SID6")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf(" SID7")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf(" SID8")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf(" SID9")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf("SID-")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf("SID:")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf("SID=")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf("SID%")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf("SID ")) > -1) { rle = 4; }
            else rmv = -1;

            if (rmv > -1)
            {
                int rmvn = rmv + rle;
                if (rmvn < d.Length)
                {
                    if (d[rmvn] == ' ') rmvn += 1;
                    else if (d[rmvn] == ':' || d[rmvn] == '-' || d[rmvn] == '=' || d[rmvn] == '(' || d[rmvn] == '[')
                    {
                        rmvn += 1;
                        if (rmvn < d.Length && d[rmvn] == ' ') rmvn += 1;
                    }
                }
                if (rmvn < d.Length)
                {
                    int end = -1;
                    int nextend = rmvn;

                    while (true)
                    {

                        end = d.IndexOfAny(new char[] { ' ', ')', '(', ']', '[', '.', '<', '>' }, nextend);
                        if (end > -1 && end < d.Length && d[end] == ' ' && end - rmvn <= 8) nextend = end + 1;
                        else break;
                    }

                    if (end > -1)
                    {
                        int len = end - rmv - rle;
                        if (len + rmvn > d.Length) de.SID = d.Substring(rmvn).Trim();
                        else de.SID = d.Substring(rmvn, len).Trim();
                        d = d.Remove(rmv, end - rmv);
                    }
                    else
                    {
                        string imx = d.Substring(rmvn).Trim();
                        imx = imx.Replace(' ', '_');

                        if (imx.Length > 13)
                        {
                            StringBuilder nimx = new StringBuilder();
                            nimx.Append(imx.Substring(0, 13));
                            for (int imxi = 13; imxi < imx.Length; imxi++)
                            {
                                if (char.IsDigit(imx[imxi])) nimx.Append(imx[imxi]);
                                else break;
                            }

                            imx = nimx.ToString();
                        }

                        de.SID = imx;
                        d = d.Remove(rmv);
                    }
                }

                if (de.SID != null)
                {
                    int weirdc = de.SID.IndexOfAny(new char[] { ' ' });

                    if (weirdc > -1) de.SID = null;
                }
            }

            if (de.SID == null)
            {
                string[] ss = d.Split(new char[] { ' ', ':', '=' });

                List<string> sidc = new List<string>();
                foreach (string si in ss)
                {
                    int dig = 0;

                    string fsi = si.Trim(new char[] { '-', ')', '(', '[', ']', '>', '<' });


                    // count digit in si
                    foreach (char ci in fsi)
                        if (char.IsDigit(ci))
                            dig++;

                    double oc = (double)dig / (double)fsi.Length;

                    if (oc > 0.3 && fsi.Length > 8 &&
                        !fsi.StartsWith("FAA-") &&
                        !fsi.StartsWith("FAI-") &&
                        !fsi.StartsWith("FAD-") &&
                        !fsi.StartsWith("GI") &&
                        !fsi.StartsWith("TE") &&
                        !fsi.StartsWith("FA") &&
                        fsi.IndexOf("GBPS") == -1 &&
                        fsi.IndexOf("KBPS") == -1 &&
                        fsi.IndexOf("MBPS") == -1 &&
                        fsi.IndexOf("BPS") == -1 &&
                        fsi.IndexOfAny(new char[] { '/', '.', ';', '\'', '\"', '>', '<', '/' }) == -1)
                    {
                        int imin = fsi.LastIndexOf('-');

                        if (imin > -1)
                        {
                            string lastport = fsi.Substring(imin + 1);

                            if (lastport.Length < 5) fsi = null;
                            else
                            {
                                bool adadigit = false;
                                foreach (char lastportc in lastport)
                                {
                                    if (char.IsDigit(lastportc))
                                    {
                                        adadigit = true;
                                        break;
                                    }
                                }

                                if (adadigit == false)
                                    fsi = null;
                            }
                        }

                        if (fsi != null)
                        {
                            if (fsi.Length >= 6 && fsi.Length <= 16)
                            {
                                bool isDate = true;

                                string[] fsip = fsi.Split(new char[] { '-' });
                                if (fsip.Length == 3)
                                {
                                    string first = fsip[0];
                                    if (char.IsDigit(first[0]))
                                    {
                                        if (first.Length == 1 || first.Length == 2 && char.IsDigit(first[1])) { }
                                        else isDate = false;
                                    }
                                    if (isDate && !char.IsDigit(first[0]))
                                    {
                                        if (first.Length >= 3 && (
                                            ListHelper.StartsWith(monthsEnglish, first) > -1 ||
                                            ListHelper.StartsWith(monthsBahasa, first) > -1
                                            ))
                                        { }
                                        else isDate = false;
                                    }
                                    string second = fsip[1];
                                    if (isDate && char.IsDigit(second[0]))
                                    {
                                        if (second.Length == 1 || second.Length == 2 && char.IsDigit(second[1])) { }
                                        else isDate = false;
                                    }
                                    if (isDate && !char.IsDigit(second[0]))
                                    {
                                        if (second.Length >= 3 && (
                                            ListHelper.StartsWith(monthsEnglish, second) > -1 ||
                                            ListHelper.StartsWith(monthsBahasa, second) > -1
                                            ))
                                        { }
                                        else isDate = false;
                                    }
                                    string third = fsip[2];
                                    if (isDate && char.IsDigit(second[0]))
                                    {
                                        if (third.Length == 2 && char.IsDigit(third[1])) { }
                                        else if (third.Length == 4 && char.IsDigit(third[1]) && char.IsDigit(third[2]) && char.IsDigit(third[3])) { }
                                        else isDate = false;
                                    }
                                    else isDate = false;
                                }
                                else if (fsip.Length == 1)
                                {
                                    // 04APR2014
                                    // APR42014
                                    // 4APR2014
                                    // 04042014

                                    if (char.IsDigit(fsi[0]))
                                    {

                                    }
                                    else
                                    {
                                        int tlen = 1;
                                        for (int fi = 1; fi < fsi.Length; fi++)
                                        {
                                            if (char.IsDigit(fsi[fi])) break;
                                            else tlen++;
                                        }

                                        string t = fsi.Substring(0, tlen);

                                        if (ListHelper.StartsWith(monthsEnglish, t) > -1 ||
                                            ListHelper.StartsWith(monthsBahasa, t) > -1)
                                        { }
                                        else isDate = false;

                                        if (isDate && fsi.Length > tlen)
                                        {
                                            int remaining = fsi.Length - tlen;
                                            if (remaining >= 3 && remaining <= 6)
                                            {
                                                for (int ooi = 0; ooi < remaining; ooi++)
                                                {
                                                    char cc = fsi[tlen + ooi];
                                                    if (!char.IsDigit(cc))
                                                    {
                                                        isDate = false;
                                                        break;
                                                    }
                                                }
                                            }
                                            else isDate = false;
                                        }
                                    }
                                }
                                else isDate = false;

                                if (isDate) fsi = null;
                            }
                        }

                        if (fsi != null)
                            sidc.Add(fsi);
                    }
                }

                if (sidc.Count > 0)
                {
                    sidc.Sort((a, b) => b.Length.CompareTo(a.Length));

                    de.SID = sidc[0];
                    d = d.Remove(d.IndexOf(de.SID), de.SID.Length);
                }
            }

            if (de.SID != null)
            {
                if (de.SID.Length <= 8)
                    de.SID = null;
                else
                {
                    string fixsid = de.SID.Trim(new char[] { '-', ')', '(', '[', ']', '>', '<', '\'', '\"' });
                    fixsid = fixsid.Replace("--", "-");

                    string[] sids = fixsid.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (sids.Length > 0)
                        fixsid = sids[0];

                    de.SID = fixsid;

                    if (StringHelper.Count(de.SID, '-') > 3)
                    {
                        de.SID = null;
                    }

                    if (de.SID != null)
                    {
                        int lmin = de.SID.LastIndexOf('-');
                        if (lmin > -1)
                            de.CID = de.SID.Substring(0, lmin);

                        if (de.CID == null && lmin == -1)
                        {
                            if (de.SID.Length == 12 && de.SID[0] == '4')
                            {
                                bool alldigit = true;
                                foreach (char c in de.SID) { if (!char.IsDigit(c)) { alldigit = false; break; } }
                                if (alldigit)
                                {
                                    de.CID = de.SID.Substring(0, 7);
                                }
                            }
                            if (de.SID.Length == 17 && (de.SID[0] == '4' || de.SID[0] == '3'))
                            {
                                bool alldigit = true;
                                foreach (char c in de.SID) { if (!char.IsDigit(c)) { alldigit = false; break; } }
                                if (alldigit)
                                {
                                    de.CID = de.SID.Substring(0, 7);
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            #region Find SITEID

            if (de.SID == null)
            {
                if (d.IndexOf("TELKOMSEL") > -1 || d.IndexOf("TSEL") > -1)
                {
                    Match m = findSiteID.Match(d);

                    if (m.Success)
                    {
                        string siteID = m.Groups[0].Value;

                        de.SID = siteID;
                        de.CID = "TELKOMSELSITES";
                        de.ServiceType = "TELKOMSELSITES";
                        de.ServiceSubType = "TELKOMSELSITES";
                    }
                }
            }

            #endregion

            if (de.CID != "TELKOMSELSITES")
            {
                d = d.Trim();

                // if double, singlekan
                if (d.Length >= 2 && d.Length % 2 == 0)
                {
                    int halflen = d.Length / 2;
                    string leftside = d.Substring(0, halflen);
                    string rightside = d.Substring(halflen, halflen);

                    if (leftside == rightside)
                        d = leftside;
                }

                d = d.Replace("()", "");
                d = d.Replace("[]", "");
                d = string.Join(" ", d.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries));

                de.CleanDescription = d;
            }
            else
            {
                de.CleanDescription = "TELKOMSELSITES";
            }

            return de;
        }

        #endregion
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

        public ExpectResult(int index, string output)
        {
            this.index = index;
            this.output = output;
        }
    }
}
