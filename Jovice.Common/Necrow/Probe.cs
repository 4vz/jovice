using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Aphysoft.Common;
using Aphysoft.Share;

namespace Jovice
{
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
        private int status;

        public int Status
        {
            get { return status; }
            set { status = value; }
        }

        private int protocol;

        public int Protocol
        {
            get { return protocol; }
            set { protocol = value; }
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
    }
    
    internal abstract class ElementToDatabase : StatusToDatabase
    {
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
    }

    internal enum ProbeMode
    {
        Default,
        StandBy
    }
    
    internal sealed partial class Probe : SshConnection
    {
        #region Enums
        
        protected enum NodeUpdateTypes
        {
            TimeStamp,
            Remark,
            RemarkUser,
            IP,
            Name,
            Active,
            Terminal,
            ConnectType,
            Model,
            Version,
            SubVersion,
            VersionTime,
            LastConfiguration
        }
        
        protected enum NodeExitReasons
        {
            None,
            Timeout
        }

        #endregion

        #region Fields

        private Thread mainLoop = null;

        private static bool stop = false;

        private ProbeMode mode;

        private bool endProbe = false;

        private Queue<string> outputs = new Queue<string>();
        private Dictionary<string, object> updates;
        private string defaultOutputIdentifier = null;
        private string outputIdentifier = null;

        protected Database j;

        private string sshServer = null;
        private string sshUser = null;
        private string sshPassword = null;
        private string sshTerminal = null;
        private string tacacUser = null;
        private string tacacPassword = null;

        private string nodeID;
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

        private bool noMore = false;

        private string alu = "ALCATEL-LUCENT";
        private string hwe = "HUAWEI";
        private string cso = "CISCO";
        private string jun = "JUNIPER";

        private string xr = "XR";

        private string feature = null;

        private Queue<string> prioritize = new Queue<string>();

        #endregion

        #region Constructors

        public Probe(string server, string user, string password, string cli, string tacacUser, string tacacPassword)
        {
            this.sshServer = server;
            this.sshUser = user;
            this.sshPassword = password;
            this.sshTerminal = cli;
            this.tacacUser = tacacUser;
            this.tacacPassword = tacacPassword;

            j = Necrow.JoviceDatabase;
        }

        #endregion

        #region Static

        private static Probe probeInstance = null;

        private static Probe standbyProbeInstance = null;

        public static void Start(ProbeProperties properties)
        {
            probeInstance = new Probe(properties.SSHServerAddress, properties.SSHUser, properties.SSHPassword, properties.SSHTerminal, properties.TacacUser, properties.TacacPassword);

            probeInstance.defaultOutputIdentifier = "PROB";

            probeInstance.Start(ProbeMode.Default);
        }

        public static void Start(StandByProbeProperties properties)
        {
            standbyProbeInstance = new Probe(properties.SSHServerAddress, properties.SSHUser, properties.SSHPassword, properties.SSHTerminal, properties.TacacUser, properties.TacacPassword);

            standbyProbeInstance.defaultOutputIdentifier = "STAN";

            standbyProbeInstance.Start(ProbeMode.StandBy);
        }

        public static void Stop(ProbeMode mode)
        {
            if (mode == ProbeMode.Default && probeInstance != null)
            {
                probeInstance.Stop();
            }
        }

        public static void Prioritize(string nodeName)
        {
            if (probeInstance != null)
            {
                probeInstance.PrioritizeQueue(nodeName);
            }
        }

        #endregion

        #region Methods

        private void Event(string message)
        {
            string oi;

            if (outputIdentifier == null)
                oi = defaultOutputIdentifier;
            else
                oi = outputIdentifier;

            Necrow.Event(message, oi);
        }

        private void Start(ProbeMode mode)
        {
            this.mode = mode;

            Event("Connecting... (" + sshUser + "@" + sshServer + ")");
            Start(sshServer, sshUser, sshPassword);
        }

        private void Restart()
        {
            Event("Reconnecting... (" + sshUser + "@" + sshServer + ")");
            Start(sshServer, sshUser, sshPassword);
        }

        private new void Stop()
        {
            outputIdentifier = null;

            Event("Disconnecting...");

            stop = true;

            mainLoop.Abort();
            mainLoop = null;

            base.Stop();
        }

        private void StopThenRestart()
        {
            outputIdentifier = null;

            Event("Disconnecting...");

            mainLoop.Abort();
            mainLoop = null;

            base.Stop();
        }

        #region SSHConnection

        protected override void CantConnect(string message)
        {
            if (message.IndexOf("Auth fail") > -1)
            {
                Event("Connection failed: Authentication failed");
            }
            else
            {
                Event("Connection failed: Server Not Responding / No Route To Server / Connection Blocked");
                Event("Reconnecting after 50 seconds");

                Thread.Sleep(50000);

                Restart();
            }
        }
        
        protected override void Connected()
        {
            Event("Connected");

            if (mainLoop == null)
                mainLoop = new Thread(new ThreadStart(MainLoop));

            stop = false;

            mainLoop.Start();
        }

        protected override void Disconnected()
        {
            if (mainLoop != null)
                mainLoop.Abort();

            outputIdentifier = null;

            Event("Disconnected");

            Thread.Sleep(5000);

            if (stop == false)
                Restart();
        }

        public override void OnResponse(string output)
        {
            if (output != null && output != "")
            {
                lock (outputs) 
                { 
                    outputs.Enqueue(output); 
                }
            }
        }

        #endregion
        
        private void MainLoop()
        {
            Culture.Default();

            if (mode == ProbeMode.Default)
            {
                #region Default

                List<string> ids = null;
                int index = -1;

                List<string> ispop = null;

                while (true)
                {
                    if (index == -1)
                    {
                        Event("Preparing node list...");

                        #region Preparing node list

                        Result pres = j.Query("select PO_NO from PEPOP");
                        ispop = new List<string>();
                        foreach (Row prow in pres) { ispop.Add(prow["PO_NO"].ToString()); }

                        Result nres = j.Query(@"
select NO_ID, NO_Name, NO_TimeStamp, NO_Remark, NO_Type, NO_Version, AG_Index
from Node
left join Area on NO_AR = AR_ID
left join AreaWitel on AR_AW = AW_ID
left join AreaGroup on AW_AG = AG_ID
where NO_Active = 1
");
                        ids = new List<string>();

                        Event(nres.Count + " active nodes");

                        int eligibleNodes = 0;
                        foreach (Row nrow in nres)
                        {
                            string remark = nrow["NO_Remark"].ToString();

                            if (remark != null)
                            {
                                DateTime timestamp = nrow["NO_TimeStamp"].ToDateTime();
                                TimeSpan span = DateTime.Now - timestamp;

                                if (remark == "CONNECTFAIL" && span.TotalHours <= 3)
                                {
                                    Event("Excluded: " + nrow["NO_Name"].ToString() + " Remark: " + remark);
                                    continue;
                                }
                                else if (remark == "UNRESOLVED" && span.TotalDays <= 1)
                                {
                                    Event("Excluded: " + nrow["NO_Name"].ToString() + " Remark: " + remark);
                                    continue;
                                }
                            }

                            string noid = nrow["NO_ID"].ToString();
                            string notype = nrow["NO_Type"].ToString();
                            int noag = nrow["AG_Index"].ToInt();

                            if (notype == "P")
                            {
                                if (noag == 2)
                                {
                                    if (ispop.Contains(noid)) for (int i = 0; i < 6; i++) { ids.Add(noid); }
                                    else for (int i = 0; i < 3; i++) { ids.Add(noid); }
                                }
                                else if (noag == 5 || noag == 6)
                                {
                                    if (ispop.Contains(noid)) for (int i = 0; i < 5; i++) { ids.Add(noid); }
                                    else for (int i = 0; i < 2; i++) { ids.Add(noid); }
                                }
                                else if (noag == 1 || noag == 3 || noag == 4)
                                {
                                    if (ispop.Contains(noid)) for (int i = 0; i < 4; i++) { ids.Add(noid); }
                                    else for (int i = 0; i < 2; i++) { ids.Add(noid); }
                                }
                                else if (noag == 7)
                                {
                                    if (ispop.Contains(noid)) for (int i = 0; i < 3; i++) { ids.Add(noid); }
                                    else for (int i = 0; i < 1; i++) { ids.Add(noid); }
                                }
                                else
                                {
                                    for (int i = 0; i < 1; i++) { ids.Add(noid); }
                                }
                            }
                            else if (notype == "M")
                            {
                                if (noag == 2) for (int i = 0; i < 5; i++) { ids.Add(noid); }
                                else if (noag == 5 || noag == 6) for (int i = 0; i < 4; i++) { ids.Add(noid); }
                                else if (noag == 1 || noag == 3 || noag == 4) for (int i = 0; i < 3; i++) { ids.Add(noid); }
                                else if (noag == 7) for (int i = 0; i < 2; i++) { ids.Add(noid); }
                                else for (int i = 0; i < 1; i++) { ids.Add(noid); }
                            }
                            eligibleNodes++;
                        }

                        Event(eligibleNodes + " eligible nodes");

                        Shuffle<string>(ids);
                        index = 0;

                        #endregion
                    }

                    #region Prioritize

                    if (prioritize.Count > 0)
                    {
                        string nodeName = prioritize.Dequeue();
                        Result rnode = j.Query("select * from Node where lower(NO_Name) = {0}", nodeName.ToLower());

                        Row node = rnode[0];
                        bool goFurther = false;

                        if (NodeEnter(node, out goFurther))
                        {
                            Thread.Sleep(10000);
                            continue;
                        }

                        PeekProcess();

                        if (goFurther)
                        {
                            if (nodeType == "P") PEProcess();
                            else if (nodeType == "M") MEProcess();
                        }
                        else NodeEnd();

                        Thread.Sleep(10000);
                        continue;
                    }

                    #endregion

                    if (index < ids.Count)
                    {
                        string id = ids[index++];
                        Result rnode = j.Query("select * from Node where NO_ID = {0}", id);

                        Row node = rnode[0];
                        bool goFurther = false;

                        if (NodeEnter(node, out goFurther))
                        {
                            Thread.Sleep(10000);
                            continue;
                        }

                        PeekProcess();

                        if (goFurther)
                        {
                            if (nodeType == "P") PEProcess();
                            else if (nodeType == "M") MEProcess();
                        }
                        else NodeEnd();

                        Thread.Sleep(10000);
                    }
                    else index = -1;
                }

                #endregion
            }
            else if (mode == ProbeMode.StandBy)
            {
                #region Stand By

                while (true)
                {
                    int wait = 0;
                    while (SSHWait())
                    {
                        Event("Waiting");
                        wait++;

                        if (wait == 3)
                        {
                            StopThenRestart();
                        }
                    }

                    Event("Idle");

                    Thread.Sleep(5000);
                }

                #endregion
            }
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

        private void PrioritizeQueue(string nodeName)
        {
            prioritize.Enqueue(nodeName);
        }

        #region Send

        private void SendControlRightBracket()
        {
            Request((char)29);
            Thread.Sleep(250);
        }

        private void SendControlC()
        {
            Request((char)3);
            Thread.Sleep(250);
        }

        private void SendControlZ()
        {
            Request((char)26);
            Thread.Sleep(250);
        }

        private void SendSpace()
        {
            Request((char)32);
        }

        private bool Send(string command)
        {
            Request(command);
            return false;
        }

        #endregion

        #region SSH

        private bool SSHWait()
        {
            int wait = 0;
            bool continueWait = true;
            while (wait < 100)
            {
                while (outputs.Count > 0) lock (outputs) outputs.Dequeue();
                if (LastOutput != null && LastOutput.TrimEnd().EndsWith(sshTerminal))
                {
                    continueWait = false;
                    break;
                }
                Thread.Sleep(100);
                wait++;
            }
            return continueWait;
        }
        
        private List<string> SSHRead(string request)
        {
            Request(request);
            Request("echo end\\ request");

            StringBuilder lineBuilder = new StringBuilder();
            List<string> lines = new List<string>();

            StringBuilder lastOutputSB = new StringBuilder();

            int wait = 0;
            while (true)
            {
                if (outputs.Count > 0)
                {
                    lock (outputs)
                    {
                        wait = 0;
                        string output = outputs.Dequeue();
                        if (output == null) continue;

                        lastOutputSB.Append(output);

                        for (int i = 0; i < output.Length; i++)
                        {
                            byte b = (byte)output[i];
                            if (b == 10)
                            {
                                string line = lineBuilder.ToString().Trim();
                                lines.Add(line);
                                lineBuilder.Clear();
                            }
                            else if (b == 9) lineBuilder.Append(' ');
                            else if (b >= 32) lineBuilder.Append((char)b);
                        }
                        if (lastOutputSB.ToString().IndexOf("end request") > -1) break;
                    }
                }
                else
                {
                    wait++;
                    Thread.Sleep(100);
                    if (wait == 200)
                    {
                        SendControlC();
                        break;
                    }
                }
            }
            if (lineBuilder.Length > 0) lines.Add(lineBuilder.ToString().Trim());
            return lines;
        }

        private int SSHExpect(params string[] args)
        {
            if (args.Length == 0) return -1;

            int wait = 0;
            int expectReturn = -1;

            Queue<string> lastOutputs = new Queue<string>();

            while (true)
            {
                if (outputs.Count > 0)
                {
                    lock (outputs)
                    {
                        wait = 0;

                        string output = outputs.Dequeue();
                        if (output != null)
                        {
                            lastOutputs.Enqueue(output);
                            if (lastOutputs.Count > 100) lastOutputs.Dequeue();

                            StringBuilder lastOutputSB = new StringBuilder();
                            foreach (string s in lastOutputs)
                                lastOutputSB.Append(s);

                            string lastOutput = lastOutputSB.ToString();

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
                    }
                }
                else
                {
                    Thread.Sleep(50);
                    wait += 1;
                    if (wait == 100) break;
                }

            }

            return expectReturn;
        }

        private string SSHCheckNodeIP(string hostname)
        {
            return SSHCheckNodeIP(hostname, false);
        }

        private string SSHCheckNodeIP(string hostname, bool reverse)
        {
            string cpeip = null;

            List<string> lines = SSHRead("cat /etc/hosts | grep -i " + hostname);

            Dictionary<string, string> greppair = new Dictionary<string, string>();
            foreach (string line in lines)
            {
                if (line.Length > 0 && char.IsDigit(line[0]))
                {
                    string[] linex = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (linex.Length == 2)
                    {
                        IPAddress address;
                        if (IPAddress.TryParse(linex[0], out address))
                        {
                            if (!reverse && !greppair.ContainsKey(linex[1].ToLower()))
                            {
                                greppair.Add(linex[1].ToLower(), linex[0]);
                            }
                            else if (!greppair.ContainsKey(linex[0]))
                            {
                                greppair.Add(linex[0], linex[1]);
                            }
                        }
                    }
                }
            }
            if (greppair.ContainsKey(hostname.ToLower())) cpeip = greppair[hostname.ToLower()].ToUpper();
            return cpeip;
        }

        #endregion

        #region Node

        private void NodeUpdate(NodeUpdateTypes type, object value)
        {
            string key = null;

            switch (type)
            {
                case NodeUpdateTypes.TimeStamp:
                    key = "NO_TimeStamp";
                    break;
                case NodeUpdateTypes.Remark:
                    key = "NO_Remark";
                    break;
                case NodeUpdateTypes.RemarkUser:
                    key = "NO_RemarkUser";
                    break;
                case NodeUpdateTypes.IP:
                    key = "NO_IP";
                    break;
                case NodeUpdateTypes.Name:
                    key = "NO_Name";
                    break;
                case NodeUpdateTypes.Active:
                    key = "NO_Active";
                    break;
                case NodeUpdateTypes.Terminal:
                    key = "NO_Terminal";
                    break;
                case NodeUpdateTypes.ConnectType:
                    key = "NO_ConnectType";
                    break;
                case NodeUpdateTypes.Model:
                    key = "NO_Model";
                    break;
                case NodeUpdateTypes.Version:
                    key = "NO_Version";
                    break;
                case NodeUpdateTypes.SubVersion:
                    key = "NO_SubVersion";
                    break;
                case NodeUpdateTypes.VersionTime:
                    key = "NO_VersionTime";
                    break;
                case NodeUpdateTypes.LastConfiguration:
                    key = "NO_LastConfiguration";
                    break;
            }


            if (key != null)
            {
                if (updates.ContainsKey(key)) updates[key] = value;
                else updates.Add(key, value);
            }
        }

        protected bool NodeEnter(Row row, out bool goFurther)
        {
            goFurther = false;

            int wait = 0;
            while (SSHWait())
            {
                wait++;
                Event("MCE Waiting... (" + wait + ")");
                Request((char)13);
                SendControlRightBracket();
                SendControlC();
                Thread.Sleep(1000);
                if (wait == 3)
                {
                    StopThenRestart();
                    return true;
                }
            }

            updates = new Dictionary<string, object>();

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

            string previousRemark = row["NO_Remark"].ToString();

            string nodeUser = tacacUser;
            string nodePass = tacacPassword;

            Event("Begin probing into " + nodeName);
            if (nodeIP != null) Event("Host IP: " + nodeIP);
            Event("Manufacture: " + nodeManufacture + "");
            if (nodeModel != null) Event("Model: " + nodeModel);

            DateTime now = DateTime.Now;

            NodeUpdate(NodeUpdateTypes.Remark, null);
            NodeUpdate(NodeUpdateTypes.RemarkUser, null);
            NodeUpdate(NodeUpdateTypes.TimeStamp, now);

            #region CHECK COMPATIBILITY
            if (nodeManufacture == alu || nodeManufacture == cso || nodeManufacture == hwe || nodeManufacture == jun) ;
            else
            {
                Event("Unsupported node manufacture");
                NodeStop();
                return true;
            }
            #endregion

            #region CHECK IP

            Event("Checking host IP");
            string resolvedIP = SSHCheckNodeIP(nodeName);

            if (resolvedIP == null) Event("Hostname is unresolved");

            if (nodeIP == null)
            {
                if (resolvedIP == null)
                {
                    #region null, null
                    if (previousRemark == "UNRESOLVED")
                        NodeUpdate(NodeUpdateTypes.Active, 0);
                    else
                        NodeUpdate(NodeUpdateTypes.Remark, "UNRESOLVED");

                    NodeStop();
                    return true;
                    #endregion
                }
                else
                {
                    #region null, RESOLVED!
                    Event("Host IP Resolved: " + resolvedIP);
                    nodeIP = resolvedIP;
                    NodeUpdate(NodeUpdateTypes.IP, nodeIP);
                    #endregion
                }
            }
            else
            {
                if (resolvedIP == null)
                {
                    #region RESOLVED!, null
                    // reverse ip?
                    Event("Resolving by reverse host name");
                    string hostName = SSHCheckNodeIP(nodeIP, true);

                    if (hostName != null)
                    {
                        #region RESOLVED!, null, RESOLVED!
                        Event("Hostname has changed to: " + hostName);

                        Result result = j.Query("select * from Node where NO_Name = {0}", hostName);

                        if (result.Count == 1)
                        {
                            #region CHANGED to existing???

                            string existingtype = result[0]["NO_Type"].ToString();
                            string existingnodeid = result[0]["NO_ID"].ToString();

                            if (existingtype == "P")
                            {
                                // cek interface
                                Column checkInterface = j.Scalar("select count(*) from PEInterface where PI_NO = {0}", existingnodeid);

                                string deletethisnode;
                                string keepthisnode;

                                int ci = checkInterface.ToInt();

                                if (ci > 0)
                                {
                                    Event("Existing node has found, delete this node");
                                    // yg existing sudah punya interface, yg ini dihapus aja
                                    deletethisnode = nodeID;
                                    keepthisnode = existingnodeid;

                                    Event("Creating alias");
                                    j.Execute("insert into NodeAlias(NA_ID, NA_NO, NA_Name) values({0}, {1}, {2})",
                                        Database.ID(), existingnodeid, nodeName);
                                }
                                else
                                {
                                    Event("Delete/update existing node properties");
                                    // yg existing kosong, pake yg ini, rename ini jadi existing, hapus existing
                                    deletethisnode = existingnodeid;
                                    keepthisnode = nodeID;

                                    NodeUpdate(NodeUpdateTypes.Name, hostName);
                                }

                                int n;
                                // update POP
                                n = j.Execute("update PEPOP set PO_NO = {0} where PO_NO = {1}", keepthisnode, deletethisnode).AffectedRows;
                                if (n == 1) Event("Update PoP OK");
                                // update ME_TO_PI
                                n = j.Execute("update MEInterface set MI_TO_PI = null where MI_TO_PI in (select PI_ID from PEInterface where PI_NO = {0})", deletethisnode).AffectedRows;
                                if (n > 0) Event("Update ME interface to PI: " + n + " entries");
                                // hapus interface IP
                                n = j.Execute("delete from PEInterfaceIP where PP_PI in (select PI_ID from PEInterface where PI_NO = {0})", deletethisnode).AffectedRows;
                                if (n > 0) Event("Delete interface IP: " + n + " entries");
                                // hapus interface
                                n = j.Execute("delete from PEInterface where PI_NO = {0}", deletethisnode).AffectedRows;
                                if (n > 0) Event("Delete interface: " + n + " entries");
                                // hapus QOS
                                n = j.Execute("delete from PEQOS where PQ_NO = {0}", deletethisnode).AffectedRows;
                                if (n > 0) Event("Delete QOS: " + n + " entries");
                                // hapus Route Name
                                n = j.Execute("delete from PERouteName where PN_NO = {0}", deletethisnode).AffectedRows;
                                if (n > 0) Event("Delete route name: " + n + " entries");
                                // hapus Node
                                n = j.Execute("delete from Node where NO_ID = {0}", deletethisnode).AffectedRows;
                                if (n > 0) Event("Node deleted");
                            }
                            else if (existingnodeid == "M")
                            {
                                // cek interface
                                Column checkInterface = j.Scalar("select count(*) from MEInterface where MI_NO = {0}", existingnodeid);

                                string deletethisnode;
                                string keepthisnode;

                                int ci = checkInterface.ToInt();

                                if (ci > 0)
                                {
                                    Event("Existing node has found, delete this node");
                                    // yg existing sudah punya interface, yg ini dihapus aja
                                    deletethisnode = nodeID;
                                    keepthisnode = existingnodeid;

                                    Event("Creating alias");
                                    j.Execute("insert into NodeAlias(NA_ID, NA_NO, NA_Name) values({0}, {1}, {2})",
                                        Database.ID(), existingnodeid, nodeName);
                                }
                                else
                                {
                                    Event("Delete/update existing node properties");
                                    // yg existing kosong, pake yg ini, rename ini jadi existing, hapus existing
                                    deletethisnode = existingnodeid;
                                    keepthisnode = nodeID;

                                    NodeUpdate(NodeUpdateTypes.Name, hostName);
                                }

                                int n;
                                // hapus customer
                                n = j.Execute("delete from MECustomer where MU_NO = {0}", deletethisnode).AffectedRows;
                                if (n > 0) Event("Delete customer: " + n + " entries");
                                // hapus service peer
                                n = j.Execute("delete from MEPeer where MP_MC in (select MC_ID from MECircuit where MC_NO = {0})", deletethisnode).AffectedRows;
                                if (n > 0) Event("Delete service peer: " + n + " entries");
                                // hapus interface
                                n = j.Execute("delete from MEInterface where MI_NO = {0}", deletethisnode).AffectedRows;
                                if (n > 0) Event("Delete interface: " + n + " entries");
                                // hapus circuit
                                n = j.Execute("delete from MECircuit where MC_NO = {0}", deletethisnode).AffectedRows;
                                if (n > 0) Event("Delete service: " + n + " entries");
                                // hapus sdp
                                n = j.Execute("delete from MESDP where MS_NO = {0}", deletethisnode).AffectedRows;
                                if (n > 0) Event("Delete peer: " + n + " entries");
                                // hapus Node
                                n = j.Execute("delete from Node where NO_ID = {0}", deletethisnode).AffectedRows;
                                if (n > 0) Event("Node deleted");
                            }

                            #endregion
                        }
                        else
                        {
                            #region NO PROBLEM

                            // simply change to the new one
                            NodeUpdate(NodeUpdateTypes.Name, hostName);

                            // insert old name alias
                            Event("Creating alias");
                            j.Execute("insert into NodeAlias(NA_ID, NA_NO, NA_Name) values({0}, {1}, {2})",
                                Database.ID(), nodeID, nodeName);

                            #endregion
                        }

                        NodeStop();
                        return true;
                        #endregion
                    }
                    else
                    {
                        #region RESOLVED!, null, null

                        if (previousRemark == "UNRESOLVED")
                            NodeUpdate(NodeUpdateTypes.Active, 0);
                        else
                            NodeUpdate(NodeUpdateTypes.Remark, "UNRESOLVED");

                        NodeStop();
                        return true;
                        #endregion                            
                    }
                    #endregion
                }
                else if (nodeIP != resolvedIP)
                {
                    #region IP HAS CHANGED

                    Event("IP has changed to: " + resolvedIP + "");

                    NodeUpdate(NodeUpdateTypes.Remark, "IPHASCHANGED");
                    NodeUpdate(NodeUpdateTypes.Active, 0);

                    NodeStop();
                    return true;

                    #endregion
                }
            }

            Event("Host identified");

            #endregion

            outputIdentifier = nodeName;

            #region CONNECT

            NodeUpdate(NodeUpdateTypes.RemarkUser, nodeUser);

            string connectType = nodeConnectType;
            int connectSequence = 0;
            string connectBy = null;

            if (connectType == null)
            {
                if (nodeManufacture == alu || nodeManufacture == cso) connectType = "T";
                else if (nodeManufacture == hwe || nodeManufacture == jun) connectType = "S";
            }

            bool connectSuccess = false;
            while (true)
            {
                wait = 0;
                while (SSHWait())
                {
                    wait++;
                    Event("MCE Waiting... (" + wait + ")");
                    Request((char)13);
                    SendControlRightBracket();
                    SendControlC();
                    Thread.Sleep(1000);
                    if (wait == 3)
                    {
                        StopThenRestart();
                        return true;
                    }
                }

                string currentConnectType = null;

                #region Determine connect type
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
                #endregion

                if (currentConnectType == "T")
                {
                    connectSuccess = NodeConnectByTelnet(nodeName, nodeManufacture, nodeUser, nodePass);
                    if (connectSuccess) connectBy = "T";
                    else
                    {
                        Event("Telnet failed");
                    }
                }
                else if (currentConnectType == "S")
                {
                    connectSuccess = NodeConnectBySSH(nodeName, nodeManufacture, nodeUser, nodePass);
                    if (connectSuccess) connectBy = "S";
                    else
                    {
                        Event("SSH failed");
                    }
                }

                if (connectSuccess || connectSequence == 1) break;
                else connectSequence++;
            }

            if (connectSuccess == false)
            {
                Event("Connection failed");

                bool tacacError = false;
                int loop = 0;
                while (true)
                {
                    wait = 0;
                    while (SSHWait())
                    {
                        wait++;
                        Event("MCE Waiting... (" + wait + ")");
                        Request((char)13);
                        SendControlRightBracket();
                        SendControlC();
                    }

                    string testOtherNode;

                    if (nodeName == "PE-D2-JT2-MGT") testOtherNode = "PE-D2-CKA";
                    else testOtherNode = "PE-D2-JT2-MGT";

                    Event("Trying to connect to other node...(" + testOtherNode + ")");

                    bool testConnected = NodeConnectByTelnet(testOtherNode, cso, nodeUser, nodePass);

                    if (testConnected)
                    {
                        NodeExit(cso);
                        Event("Connected! TACAC server is OK.");
                        break;
                    }
                    else
                    {
                        tacacError = true;
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


                if (tacacError)
                {
                    // this node is innocent
                    // TODO: try again?
                }
                else
                {
                    if (previousRemark == "CONNECTFAIL")
                        NodeUpdate(NodeUpdateTypes.Active, 0);
                    else
                        NodeUpdate(NodeUpdateTypes.Remark, "CONNECTFAIL");

                }

                NodeStop();
                return true;
            }

            if (nodeConnectType == null || connectBy != connectType)
                NodeUpdate(NodeUpdateTypes.ConnectType, connectBy);

            Event("Connected!");

            string terminal = null;

            if (nodeManufacture == alu)
            {
                string[] lines = LastOutput.Split('\n');
                string lastLine = lines[lines.Length - 1];

                int titik2 = lastLine.LastIndexOf(':');
                terminal = lastLine.Substring(titik2 + 1);
            }
            else if (nodeManufacture == hwe)
            {
                string[] lines = LastOutput.Split('\n');
                string lastLine = lines[lines.Length - 1];
                terminal = lastLine;
            }
            else if (nodeManufacture == cso)
            {
                string[] lines = LastOutput.Split('\n');
                string lastLine = lines[lines.Length - 1];
                terminal = lastLine;

                if (terminal.EndsWith(">"))
                {
                    Event("Error: Not In Privileged EXEC mode");
                    NodeEnd();
                    return true;
                }
            }
            else if (nodeManufacture == jun)
            {
                string[] lines = LastOutput.Split('\n');
                string lastLine = lines[lines.Length - 1];

                string[] linex = lastLine.Split(new char[] { '@' }, StringSplitOptions.RemoveEmptyEntries);
                terminal = linex[1];
            }

            Event("Terminal: " + terminal + "");

            if (terminal != nodeTerminal) NodeUpdate(NodeUpdateTypes.Terminal, terminal);
            nodeTerminal = terminal;

            #endregion

            #region TERMINAL SETUP

            bool timeout;
            noMore = true; // by default, we can no more

            if (nodeManufacture == alu)
            {
                if (Send("environment no saved-ind-prompt")) { NodeStop(); return true; }
                NodeRead(out timeout);
                if (timeout) { NodeStop(); return true; }

                if (Send("environment no more")) { NodeStop(); return true; }
                List<string> lines = NodeRead(out timeout);
                if (timeout) { NodeStop(); return true; }

                string oline = string.Join(" ", lines);
                if (oline.IndexOf("CLI Command not allowed for this user.") > -1)
                    noMore = false;
                else
                    noMore = true;
            }
            else if (nodeManufacture == hwe)
            {
                if (Send("screen-length 0 temporary")) { NodeStop(); return true; }
                NodeRead(out timeout);
                if (timeout) { NodeStop(); return true; }
            }
            else if (nodeManufacture == cso)
            {
                if (Send("terminal length 0")) { NodeStop(); return true; }
                NodeRead(out timeout);
                if (timeout) { NodeStop(); return true; }
            }
            else if (nodeManufacture == jun)
            {
                if (Send("set cli screen-length 0")) { NodeStop(); return true; }
                NodeRead(out timeout);
                if (timeout) { NodeStop(); return true; }
            }

            #endregion

            #region VERSION

            bool checkVersion = false;

            if (nodeVersion == null) checkVersion = true;
            else
            {
                DateTime versionTime = row["NO_VersionTime"].ToDateTime();
                TimeSpan span = now - versionTime;
                if (span.TotalDays >= 7) checkVersion = true;
            }

            if (checkVersion)
            {
                Event("Checking Version");

                string version = null;
                string subVersion = null;
                string model = null;

                if (nodeManufacture == alu)
                {
                    #region alu
                    if (Send("show version | match TiMOS")) { NodeStop(); return true; }
                    List<string> lines = NodeRead(out timeout);
                    if (timeout) { NodeStop(); return true; }

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
                    #endregion
                }
                else if (nodeManufacture == hwe)
                {
                    #region hwe
                    if (Send("display version")) { NodeStop(); return true; }
                    List<string> lines = NodeRead(out timeout);
                    if (timeout) { NodeStop(); return true; }

                    foreach (string line in lines)
                    {
                        if (line.StartsWith("VRP (R) software"))
                        {
                            version = line.Substring(26, line.IndexOf(' ', 26) - 26).Trim();
                            break;
                        }
                    }
                    #endregion
                }
                else if (nodeManufacture == cso)
                {
                    #region cso
                    if (Send("show version | in IOS")) { NodeStop(); return true; }
                    List<string> lines = NodeRead(out timeout);
                    if (timeout) { NodeStop(); return true; }

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
                        if (Send("show version | in bytes of memory")) { NodeStop(); return true; }
                        lines = NodeRead(out timeout);
                        if (timeout) { NodeStop(); return true; }
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
                    if (Send("show version | match \"JUNOS Base OS boot\"")) { NodeStop(); return true; }
                    List<string> lines = NodeRead(out timeout);
                    if (timeout) { NodeStop(); return true; }

                    foreach (string line in lines)
                    {
                        if (line.StartsWith("JUNOS Base OS boot"))
                        {
                            string[] linex = line.Split(new char[] { '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
                            if (linex.Length >= 2) version = linex[1];
                            break;
                        }
                    }
                }

                if (model != nodeModel)
                {
                    nodeModel = model;
                    NodeUpdate(NodeUpdateTypes.Model, model);
                    Event("Model discovered: " + model);
                }
                if (version != nodeVersion)
                {
                    nodeVersion = version;
                    NodeUpdate(NodeUpdateTypes.Version, version);
                    Event("Version updated: " + version);
                }
                if (subVersion != nodeSubVersion)
                {
                    nodeSubVersion = subVersion;
                    NodeUpdate(NodeUpdateTypes.SubVersion, subVersion);
                    Event("SubVersion updated: " + subVersion);
                }

                NodeUpdate(NodeUpdateTypes.VersionTime, now);
            }

            if (nodeVersion == null)
            {
                Event("Cant determined node version.");
                NodeEnd();
                return true;
            }

            Event("Version: " + nodeVersion + ((nodeSubVersion != null) ? ":" + nodeSubVersion : ""));

            #endregion

            #region LAST CONFIGURATION

            Event("Checking Last Configuration");

            bool lastconfliveisnull = true;

            DateTime lastconflive = DateTime.MinValue;

            if (nodeManufacture == alu)
            {
                #region alu
                if (Send("show system information | match \"Time Last Modified\"")) { NodeStop(); return true; }
                List<string> lines = NodeRead(out timeout);
                if (timeout) { NodeStop(NodeExitReasons.Timeout); return true; }

                bool lastModified = false;

                foreach (string line in lines)
                {
                    if (line.StartsWith("Time Last Modified"))
                    {
                        lastModified = true;
                        //Time Last Modified     : 2
                        //01234567890123456789012345
                        string datetime = line.Substring(25).Trim();
                        lastconflive = DateTime.Parse(datetime);
                        lastconflive = new DateTime(
                            lastconflive.Ticks - (lastconflive.Ticks % TimeSpan.TicksPerSecond),
                            lastconflive.Kind
                            );
                        lastconfliveisnull = false;
                    }
                }

                if (lastModified == false)
                {
                    if (Send("show system information | match \"Time Last Saved\"")) { NodeStop(); return true; }
                    lines = NodeRead(out timeout);
                    if (timeout) { NodeStop(NodeExitReasons.Timeout); return true; }

                    foreach (string line in lines)
                    {
                        if (line.StartsWith("Time Last Saved"))
                        {
                            lastModified = true;
                            //Time Last Saved        : 2015/01/13 01:13:56
                            //01234567890123456789012345
                            string datetime = line.Substring(25).Trim();
                            if (DateTime.TryParse(datetime, out lastconflive))
                            {
                                lastconflive = new DateTime(
                                    lastconflive.Ticks - (lastconflive.Ticks % TimeSpan.TicksPerSecond),
                                    lastconflive.Kind
                                    );
                                lastconfliveisnull = false;
                            }
                        }
                    }
                }
                #endregion
            }
            else if (nodeManufacture == hwe)
            {
                #region hwe
                if (Send("display changed-configuration time")) { NodeStop(); return true; }
                List<string> lines = NodeRead(out timeout);
                if (timeout) { NodeStop(NodeExitReasons.Timeout); return true; }

                foreach (string line in lines)
                {
                    if (line.StartsWith("The time"))
                    {
                        string dateparts = line.Substring(line.IndexOf(':') + 1);

                        if (dateparts != null)
                        {
                            if (DateTime.TryParse(dateparts, out lastconflive))
                            {
                                lastconflive = new DateTime(
                                    lastconflive.Ticks - (lastconflive.Ticks % TimeSpan.TicksPerSecond),
                                    lastconflive.Kind
                                    );
                                lastconfliveisnull = false;
                            }
                        }
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
                    if (Send("show configuration history commit last 1 | in commit")) { NodeStop(); return true; }
                    List<string> lines = NodeRead(out timeout);
                    if (timeout) { NodeStop(NodeExitReasons.Timeout); return true; }

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

                                if (DateTime.TryParse(dates[1] + " " + dates[2] + " " + dates[4] + " " + dates[3], out lastconflive))
                                {
                                    lastconflive = new DateTime(
                                        lastconflive.Ticks - (lastconflive.Ticks % TimeSpan.TicksPerSecond),
                                        lastconflive.Kind
                                        );
                                    lastconfliveisnull = false;
                                }
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    #region !xr
                    if (Send("show configuration id detail")) { NodeStop(); return true; }
                    List<string> lines = NodeRead(out timeout);
                    if (timeout) { NodeStop(NodeExitReasons.Timeout); return true; }

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
                                if (DateTime.TryParse(dateparts, out lastconflive))
                                {
                                    lastconflive = new DateTime(
                                        lastconflive.Ticks - (lastconflive.Ticks % TimeSpan.TicksPerSecond),
                                        lastconflive.Kind
                                        );
                                    lastconfliveisnull = false;
                                }
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
                if (Send("show system uptime | match \"Last configured\"")) { NodeStop(); return true; }
                List<string> lines = NodeRead(out timeout);
                if (timeout) { NodeStop(NodeExitReasons.Timeout); return true; }

                foreach (string line in lines)
                {
                    if (line.StartsWith("Last configured: "))
                    {
                        //Last configured: 2015-01-20 09:53:54
                        //0123456789012345678901234567890123456789
                        string lineTrim = line.Substring(17).Trim();
                        string[] linex = lineTrim.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);

                        if (DateTime.TryParse(linex[0] + " " + linex[1], out lastconflive))
                        {
                            lastconflive = new DateTime(
                                lastconflive.Ticks - (lastconflive.Ticks % TimeSpan.TicksPerSecond),
                                lastconflive.Kind
                                );
                            lastconfliveisnull = false;
                        }
                        break;
                    }
                }
                #endregion
            }

            DateTime lastconfdb = row["NO_LastConfiguration"].ToDateTime();

            if (lastconfliveisnull == false && lastconflive != lastconfdb)
            {
                Event("Configuration has changed!");
                NodeUpdate(NodeUpdateTypes.LastConfiguration, lastconflive);

                goFurther = true;
            }
            else
            {
            }

            #endregion

            return false;
        }

        private bool NodeConnectByTelnet(string host, string manufacture, string user, string pass)
        {
            int expect = -1;
            bool connectSuccess = false;

            Event("Connecting with Telnet... (" + user + "@" + host + ")");
            Request("telnet " + host);

            if (manufacture == alu)
            {
                #region alu
                expect = SSHExpect("ogin:");
                if (expect == 0)
                {
                    Event("Authenticating: User");
                    Request(user);
                    expect = SSHExpect("assword:");
                    if (expect == 0)
                    {
                        Event("Authenticating: Password");
                        Request(pass);
                        expect = SSHExpect("#", "ogin:", "closed by foreign");
                        if (expect == 0) connectSuccess = true;
                        else SendControlZ();
                    }
                    else
                    {
                        Event("Cannot find password console prefix");
                        SendControlZ();
                    }
                }
                else
                {
                    Event("Cannot find login console prefix");
                    SendControlC();
                }
                #endregion
            }
            else if (manufacture == hwe)
            {
                #region hwe
                expect = SSHExpect("sername:", "closed by foreign");
                if (expect == 0)
                {
                    Event("Authenticating: User");
                    Request(user);
                    expect = SSHExpect("assword:");
                    if (expect == 0)
                    {
                        Event("Authenticating: Password");
                        Request(pass);
                        expect = SSHExpect(">", "sername:", "Tacacs server reject");
                        if (expect == 0) connectSuccess = true;
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
                    Event("Cannot find username console prefix");
                    SendControlC();
                }
                #endregion
            }
            else if (manufacture == cso)
            {
                #region cso
                expect = SSHExpect("sername:");
                if (expect == 0)
                {
                    Event("Authenticating: User");
                    Request(user);
                    expect = SSHExpect("assword:");
                    if (expect == 0)
                    {
                        Event("Authenticating: Password");
                        Request(pass);
                        expect = SSHExpect("#", "sername:", "closed by foreign", "cation failed");
                        if (expect == 0) connectSuccess = true;
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
                    Event("Cannot find username console prefix");
                    SendControlC();
                }
                #endregion
            }
            else SendControlC();

            return connectSuccess;
        }

        private bool NodeConnectBySSH(string host, string manufacture, string user, string pass)
        {
            int expect = -1;
            bool connectSuccess = false;

            Event("Connecting with SSH... (" + user + "@" + host + ")");
            Request("ssh -o StrictHostKeyChecking=no " + user + "@" + host);

            if (manufacture == hwe)
            {
                #region hwe
                expect = SSHExpect("assword:", "Connection refused");
                if (expect == 0)
                {
                    Event("Authenticating: Password");
                    Request(pass);
                    expect = SSHExpect(">", "assword:");
                    if (expect == 0) connectSuccess = true;
                    else SendControlC();
                }
                else SendControlC();
                #endregion
            }
            else if (manufacture == cso)
            {
                #region cso
                expect = SSHExpect("assword:", "Connection refused");
                if (expect == 0)
                {
                    Event("Authenticating: Password");
                    Request(pass);
                    expect = SSHExpect("#", "assword:");
                    if (expect == 0) connectSuccess = true;
                    else SendControlC();
                }
                else SendControlC();
                #endregion
            }
            else if (manufacture == jun)
            {
                #region jun
                expect = SSHExpect("password:");
                if (expect == 0)
                {
                    Event("Authenticating: Password");
                    Request(pass);
                    expect = SSHExpect(">", "assword:");
                    if (expect == 0) connectSuccess = true;
                    else SendControlC();
                }
                else SendControlC();
                #endregion
            }
            else SendControlC();

            return connectSuccess;
        }

        protected List<string> NodeRead(out bool timeout)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            // Event("Reading...");

            timeout = false;
            if (nodeTerminal == null) return null;

            StringBuilder lineBuilder = new StringBuilder();
            List<string> lines = new List<string>();
            int wait = 0;

            StringBuilder lastOutputSB = new StringBuilder();
                        
            while (true)
            {
                if (outputs.Count > 0)
                {
                    lock (outputs)
                    {
                        wait = 0;
                        string output = outputs.Dequeue();

                        if (output == null) continue;

                        lastOutputSB.Append(output);

                        for (int i = 0; i < output.Length; i++)
                        {
                            byte b = (byte)output[i];
                            if (b == 10)
                            {
                                string line = lineBuilder.ToString();
                                lines.Add(line);
                                lineBuilder.Clear();
                            }
                            else if (b >= 32) lineBuilder.Append((char)b);
                        }

                        string losb = lastOutputSB.ToString();

                        if (noMore == false)
                        {
                            if (nodeManufacture == alu)
                            {
                                string aluMORE = "Press any key to continue (Q to quit)";
                                if (losb.Contains(aluMORE))
                                {
                                    List<string> newlines = new List<string>();

                                    foreach (string line in lines)
                                    {
                                        string newline;
                                        if (line.IndexOf(aluMORE) > -1)
                                            newline = line.Replace(aluMORE + "                                      ", "");
                                        else newline = line;

                                        newlines.Add(newline);
                                    }

                                    lines = newlines;

                                    SendSpace();
                                }
                            }
                        }

                        if (losb.TrimEnd().EndsWith(nodeTerminal.Trim())) break;
                    }
                }
                else
                {
                    wait++;

                    if (wait % 50 == 0)
                    {
                        Event("Waiting...");
                    }


                    Thread.Sleep(100);
                    if (wait == 400)
                    {
                        timeout = true;
                        Thread.Sleep(1000);
                        SendControlC();
                        break;
                    }
                }
            }
            if (lineBuilder.Length > 0) lines.Add(lineBuilder.ToString().Trim());

            stopwatch.Stop();

            //Event("Reading completed in " + stopwatch.ElapsedMilliseconds + " ms");

            return lines;
        }

        private void NodeEnd()
        {
            NodeExit();
            NodeSave();
        }

        private void NodeStop(NodeExitReasons because)
        {
            if (because == NodeExitReasons.Timeout)
            {
                Event("Reading timeout");
                NodeExit();
            }
            else
            {
                NodeSave();
                outputIdentifier = null;
            }
        }

        private void NodeStop()
        {
            NodeStop(NodeExitReasons.None);
        }
        
        private void NodeExit()
        {
            Event("Exiting...");
            if (outputIdentifier != null)
            {
                NodeExit(nodeManufacture);
            }
            outputIdentifier = null;
            Event("Exit!");
        }

        private void NodeExit(string manufacture)
        {
            Thread.Sleep(100);
            if (manufacture == alu && Send("logout")) { NodeStop(); return; }
            else if (manufacture == hwe && Send("quit")) { NodeStop(); return; }
            else if (manufacture == cso && Send("exit")) { NodeStop(); return; }
            else if (manufacture == jun && Send("exit")) { NodeStop(); return; }

            int wait = 0;
            while (SSHWait())
            {
                wait++;
                Event("MCE Wait " + wait + ": exit node");
                Request((char)13);
                SendControlRightBracket();
                SendControlC();
                Thread.Sleep(1000);
                if (wait == 3)
                {
                    StopThenRestart();
                    break;
                }
            }
        }

        private void NodeSave()
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, object> pair in updates)
            {
                if (sb.Length > 0) sb.Append(", ");
                sb.Append(j.Format(pair.Key + " = {0}", pair.Value));
            }

            if (sb.Length > 0)
            {
                string sql = "update Node set " +
                    StringHelper.EscapeFormat(sb.ToString()) +
                    " where NO_ID = {0}";
                j.Scalar(sql, nodeID);
            }
        }

        #endregion

        #endregion
    }
}
