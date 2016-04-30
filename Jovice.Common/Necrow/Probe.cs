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

    internal abstract class ServiceBaseToDatabase : StatusToDatabase
    {
        private string serviceID;

        public string ServiceID
        {
            get { return serviceID; }
            set { serviceID = value; }
        }

        private bool updateServiceID = false;

        public bool UpdateServiceID
        {
            get { return updateServiceID; }
            set { updateServiceID = value; }
        }
    }

    internal abstract class ElementToDatabase : ServiceBaseToDatabase
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

        private enum UpdateTypes
        {
            NecrowVersion,
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

        private enum StopState
        {
            Stop,
            Failure
        }

        #endregion

        #region Fields

        private Thread mainLoop = null;
        private Thread idleThread = null;

        private ProbeMode mode;

        private Queue<string> outputs = new Queue<string>();
        private Dictionary<string, object> updates;
        private Dictionary<string, string> summaries;
        private string defaultOutputIdentifier = null;
        private string outputIdentifier = null;

        private Database j;

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
        private int nodeNVER;

        private bool noMore = false;

        private readonly string alu = "ALCATEL-LUCENT";
        private readonly string hwe = "HUAWEI";
        private readonly string cso = "CISCO";
        private readonly string jun = "JUNIPER";
        private readonly string xr = "XR";

        private char[] newline = new char[] { (char)13, (char)10 };

        private Queue<string> prioritize = new Queue<string>();

        public bool IsConnected
        {
            get
            {
                return mainLoop != null;
            }
        }

        public bool requestFailure = false;

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

        #region Database

        public Batch Batch()
        {
            return j.Batch();
        }

        public Result Query(string sql)
        {
            Result result = null;
            while (true)
            {
                result = j.Query(sql);
                if (result.OK) break;
                else
                {
                    Event("Exception while executing query, retrying...");
                    Thread.Sleep(5000);
                }
            }

            return result;
        }

        public Dictionary<string, Row> QueryDictionary(string sql, string key)
        {
            return QueryDictionary(sql, key, delegate (Row row) { }, null);
        }

        public Dictionary<string, Row> QueryDictionary(string sql, string key, params object[] args)
        {
            return QueryDictionary(sql, key, delegate(Row row) { }, args);
        }

        public Dictionary<string, Row> QueryDictionary(string sql, string key, QueryDictionaryDuplicateCallback duplicate, params object[] args)
        {
            Dictionary<string, Row> result = null;
            while (true)
            {
                result = j.QueryDictionary(sql, key, duplicate, args);
                if (result != null) break;
                else
                {
                    Event("Exception while executing query, retrying...");
                    Thread.Sleep(5000);
                }
            }

            return result;
        }

        public Dictionary<string, Row> QueryDictionary(string sql, QueryDictionaryKeyCallback callback, params object[] args)
        {
            return QueryDictionary(sql, callback, delegate (Row row) { }, args);
        }

        public Dictionary<string, Row> QueryDictionary(string sql, QueryDictionaryKeyCallback callback, QueryDictionaryDuplicateCallback duplicate, params object[] args)
        {
            Dictionary<string, Row> result = null;
            while (true)
            {
                result = j.QueryDictionary(sql, callback, duplicate, args);
                if (result != null) break;
                else
                {
                    Event("Exception while executing query, retrying...");
                    Thread.Sleep(5000);
                }
            }

            return result;
        }

        public string Format(string sql, params object[] args)
        {
            return j.Format(sql, args);
        }

        public Result Query(string sql, params object[] args)
        {
            Result result = null;
            while (true)
            {
                result = j.Query(sql, args);
                if (result.OK) break;
                else
                {
                    Event("Exception while executing query, retrying...");
                    Thread.Sleep(5000);
                }
            }

            return result;
        }

        public Column Scalar(string sql)
        {
            Column result = null;
            while (true)
            {
                Column c = j.Scalar(sql);
                if (c != null) break;
                else
                {
                    Event("Exception while executing scalar, retrying...");
                    Thread.Sleep(5000);
                }
            }

            return result;
        }

        public Column Scalar(string sql, params object[] args)
        {
            Column result = null;
            while (true)
            {
                Column c = j.Scalar(sql, args);
                if (c != null) break;
                else
                {
                    Event("Exception while executing scalar, retrying...");
                    Thread.Sleep(5000);
                }
            }

            return result;
        }

        public Result Execute(string sql)
        {
            Result result = null;
            while (true)
            {
                result = j.Execute(sql);
                if (result.OK) break;
                else
                {
                    Event("Exception while executing execute, retrying...");
                    Thread.Sleep(5000);
                }
            }

            return result;
        }

        public Result Execute(string sql, params object[] args)
        {
            Result result = null;
            while (true)
            {
                result = j.Execute(sql, args);
                if (result.OK) break;
                else
                {
                    if (result.AffectedRows > 0) break; // if there are affected rows, break anyway
                    else
                    {
                        Event("Exception while executing execute, retrying...");
                        Thread.Sleep(5000);
                    }
                }
            }

            return result;
        }

        public Result ExecuteIdentity(string sql)
        {
            Result result = null;
            while (true)
            {
                result = j.ExecuteIdentity(sql);
                if (result.OK) break;
                else
                {
                    Event("Exception while executing execute identity, retrying...");
                    Thread.Sleep(5000);
                }
            }

            return result;
        }

        public Result ExecuteIdentity(string sql, params object[] args)
        {
            Result result = null;
            while (true)
            {
                result = j.ExecuteIdentity(sql, args);
                if (result.OK) break;
                else
                {
                    Event("Exception while executing execute identity, retrying...");
                    Thread.Sleep(5000);
                }
            }

            return result;
        }

        #endregion
        
        #region Static

        private static Probe probeInstance = null;

        private static Probe standbyProbeInstance = null;

        public static void Start(ProbeProperties properties)
        {
            probeInstance = new Probe(properties.SSHServerAddress, properties.SSHUser, properties.SSHPassword, properties.SSHTerminal, properties.TacacUser, properties.TacacPassword);

            probeInstance.defaultOutputIdentifier = "PROB";

            if (properties.TestProbeNode != null)
            {
                probeInstance.PrioritizeQueue(properties.TestProbeNode + "*");
            }

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

            //Event("Connecting... (" + sshUser + "@" + sshServer + ")");
            Start(sshServer, sshUser, sshPassword);

            // TEST GOES HERE
            //MEInterfaceToDatabase li = new MEInterfaceToDatabase();
            //li.Description = "TRUNK_ME-D7-PTR/Gi6/0/19_No8_1G_To_PE-D7-PTR-VPN Gi0/7/0/12_EX_PE-D2-PTK";
            //li.Name = "Gi6/0/19";
            //nodeName = "ME-D7-PTR";

            //SortedDictionary<string, MEInterfaceToDatabase> interfacelive = new SortedDictionary<string, MEInterfaceToDatabase>();
            //MEInterfaceToDatabase mi = new MEInterfaceToDatabase();
            //interfacelive.Add("Gi6/0/19", mi);

            //FindPhysicalAdjacent(li);

            //Event("li:" + li.AdjacentID);
        }

        private void Restart()
        {
            Event("Reconnecting... (" + sshUser + "@" + sshServer + ")");
            Start(sshServer, sshUser, sshPassword);
        }

        private void End()
        {
            outputIdentifier = null;

            Event("Disconnecting...");

            mainLoop.Abort();
            mainLoop = null;

            if (idleThread != null)
            {
                idleThread.Abort();
                mainLoop = null;
            }

            base.Stop();
        }

        private new void Stop()
        {
            Event("Stop requested");
            End();
        }

        private void Failure()
        {
            Event("Connection failure has occured");
            End();
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
            Event("Connected!");

            if (mainLoop != null)
            {
                mainLoop.Abort();
                mainLoop = null;
            }

            mainLoop = new Thread(new ThreadStart(MainLoop));

            mainLoop.Start();
        }

        protected override void Disconnected()
        {
            if (mainLoop != null)
            {
                mainLoop.Abort();
                mainLoop = null;
            }

            outputIdentifier = null;

            Event("Disconnected");

            Thread.Sleep(5000);
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

                while (true)
                {
                    if (index == -1)
                    {
                        Event("Preparing node list...");

                        #region Preparing node list

                        Result nres = Query(@"
select NO_ID from Node where NO_Active = 1 and NO_Type in ('P', 'M') and NO_TimeStamp is null and NO_LastConfiguration is null                        
");
                        Result mres = Query(@"
select a.NO_ID, a.NO_Name, a.NO_Remark, a.NO_TimeStamp, CASE WHEN a.span < 0 then 0 else a.span end as span from (
select NO_ID, NO_Name, NO_Remark, NO_LastConfiguration, NO_TimeStamp, DateDiff(hour, NO_LastConfiguration, NO_TimeStamp) as span 
from Node where NO_Active = 1 and NO_Type in ('P', 'M') and NO_TimeStamp is not null and NO_LastConfiguration is not null
) a
order by span asc, a.NO_LastConfiguration asc
");
                        Result sres = Query(@"
select NO_ID from Node where NO_Active = 1 and NO_Type in ('P', 'M') and NO_TimeStamp is not null and NO_LastConfiguration is null                        
");

                        ids = new List<string>();

                        int excluded = 0;

                        foreach (Row row in nres) ids.Add(row["NO_ID"].ToString());
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

                            ids.Add(row["NO_ID"].ToString());
                        }
                        foreach (Row row in sres) ids.Add(row["NO_ID"].ToString());
                        int total = ids.Count + excluded;
                        Event("Total " + total + " nodes available, " + ids.Count + " nodes eligible, " + excluded + " excluded in this iteration.");

                        #endregion

                        index = 0;
                    }

                    Row node = null;
                    bool prioritizeProcess = false;

                    if (prioritize.Count > 0)
                    {
                        string nodeName = prioritize.Dequeue();

                        if (nodeName.EndsWith("*"))
                        {
                            nodeName = nodeName.TrimEnd(new char[] { '*' });
                            prioritizeProcess = true;
                        }

                        Event("Prioritizing Probe: " + nodeName);
                        Result rnode = Query("select * from Node where lower(NO_Name) = {0}", nodeName.ToLower());

                        if (rnode.Count == 1)
                        {
                            node = rnode[0];
                        }
                        else
                        {
                            Event("Failed, not exists in the database.");
                            continue;
                        }
                    }
                    else if (index < ids.Count)
                    {
                        index++;
                        string id = ids[index];
                        Result rnode = Query("select * from Node where NO_ID = {0}", id);
                        node = rnode[0];
                    }
                    else index = -1;

                    if (node != null)
                    {
                        bool continueProcess = false;

                        Enter(node, out continueProcess, prioritizeProcess);

                        if (continueProcess)
                        {
                            idleThread = new Thread(new ThreadStart(delegate ()
                            {
                                while (true)
                                {
                                    Thread.Sleep(30000);
                                    if (nodeManufacture == alu || nodeManufacture == cso || nodeManufacture == hwe) SendCharacter((char)27);

                                }
                            }));
                            idleThread.Start();

                            if (nodeType == "P")
                            {
                                PEProcess();
                                findMEPhysicalAdjacentLoaded = false;
                            }
                            else if (nodeType == "M")
                            {
                                MEProcess();
                            }

                            idleThread.Abort();
                            idleThread = null;
                        }

                        // delay after probing finished
                        Event("Next node in 5 seconds...");
                        Thread.Sleep(5000);
                    }
                }

                #endregion
            }
            else if (mode == ProbeMode.StandBy)
            {
                #region Stand By

                while (true)
                {
                    int wait = 0;
                    /*while (SSHWait())
                    {
                        Event("Waiting");
                        wait++;

                        if (wait == 3)
                        {
                            Failure();
                        }
                    }*/

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

        #region Methods

        private void SendLine(string command)
        {
            SendLine(command, false);
        }

        private void SendLine(string command, bool saveOnFailure)
        {
            if (IsConnected)
            {
                Thread.Sleep(250);
                bool res = WriteLine(command);
                if (res == false)
                {
                    if (saveOnFailure) Save();
                    Failure();
                }
            }
        }

        private void Send(string line)
        {
            if (IsConnected)
            {
                Thread.Sleep(250);
                bool res = Write(line);
                if (res == false) Failure();
            }
        }

        private void SendCharacter(char character)
        {
            Send(character.ToString());
        }

        private void SendSpace()
        {
            SendCharacter((char)32);
        }

        private void SendControlRightBracket()
        {
            SendCharacter((char)29);
        }

        private void SendControlC()
        {
            SendCharacter((char)3);
        }

        private void SendControlZ()
        {
            SendCharacter((char)26);
        }

        private void WaitUntilMCETerminalReady(string waitMessage)
        {
            int loop = 0;

            while (true)
            {
                // check output, break when terminal is ready
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
                    //Event(LastOutput);
                    Thread.Sleep(100);
                    wait++;
                }
                if (continueWait == false) break;

                // else continue wait...
                loop++;
                if (loop == 3) Failure(); // loop 3 times, its a failure

                // print message where we are waiting (or why)
                Event(waitMessage + "... (" + loop + ")");

                Event("Last Reading Output: ");
                int lp = LastOutput.Length - 200;
                if (lp < 0) lp = 0;
                string lop = LastOutput.Substring(lp);
                lop = lop.Replace("\r", "<CR>");
                lop = lop.Replace("\n", "<NL>");
                Event(lop);

                // try sending exit characters
                SendCharacter((char)13);
                SendControlRightBracket();
                SendControlC();

                Thread.Sleep(1000);
            }

        }

        private List<string> MCESendLine(string command)
        {
            SendLine(command);
            SendLine("echo end\\ request");

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

        private int MCEExpect(params string[] args)
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

        private string MCECheckNodeIP(string hostname)
        {
            return MCECheckNodeIP(hostname, false);
        }

        private string MCECheckNodeIP(string hostname, bool reverse)
        {
            string cpeip = null;

            List<string> lines = MCESendLine("cat /etc/hosts | grep -i " + hostname);
            if (lines == null) return null;

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

        private void Update(UpdateTypes type, object value)
        {
            string key = null;

            switch (type)
            {
                case UpdateTypes.NecrowVersion:
                    key = "NO_NVER";
                    break;
                case UpdateTypes.TimeStamp:
                    key = "NO_TimeStamp";
                    break;
                case UpdateTypes.Remark:
                    key = "NO_Remark";
                    break;
                case UpdateTypes.RemarkUser:
                    key = "NO_RemarkUser";
                    break;
                case UpdateTypes.IP:
                    key = "NO_IP";
                    break;
                case UpdateTypes.Name:
                    key = "NO_Name";
                    break;
                case UpdateTypes.Active:
                    key = "NO_Active";
                    break;
                case UpdateTypes.Terminal:
                    key = "NO_Terminal";
                    break;
                case UpdateTypes.ConnectType:
                    key = "NO_ConnectType";
                    break;
                case UpdateTypes.Model:
                    key = "NO_Model";
                    break;
                case UpdateTypes.Version:
                    key = "NO_Version";
                    break;
                case UpdateTypes.SubVersion:
                    key = "NO_SubVersion";
                    break;
                case UpdateTypes.VersionTime:
                    key = "NO_VersionTime";
                    break;
                case UpdateTypes.LastConfiguration:
                    key = "NO_LastConfiguration";
                    break;
            }


            if (key != null)
            {
                if (updates.ContainsKey(key)) updates[key] = value;
                else updates.Add(key, value);
            }
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
            if (key != null)
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

            WaitUntilMCETerminalReady("MCE Waiting on Exit");
        }

        private void SaveExit()
        {
            Save();
            Exit();
        }

        private List<string> Read(out bool timeout)
        {
            Event("Reading...");

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

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

                                if (nodeManufacture == hwe && nodeVersion == "5.160" && line.Length > 80)
                                {
                                    int looptimes = (int)Math.Ceiling((float)line.Length / 80);

                                    for (int loop = 0; loop < looptimes; loop++)
                                    {
                                        int sisa = 80;
                                        if (loop == looptimes - 1) sisa = line.Length - (loop * 80);
                                        string curline = line.Substring(loop * 80, sisa);
                                        lines.Add(curline);
                                    }
                                }
                                else
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

                    if (wait % 50 == 0 && wait < 400)
                    {
                        Event("Waiting...");

                        SendLine("");

                        Event("Last Reading Output: ");
                        int lp = LastOutput.Length - 200;
                        if (lp < 0) lp = 0;
                        string lop = LastOutput.Substring(lp);
                        lop = lop.Replace("\r", "<CR>");
                        lop = lop.Replace("\n", "<NL>");
                        Event(lop);
                    }

                    Thread.Sleep(100);
                    if (wait == 400)
                    {
                        timeout = true;
                        Event("Reading timeout, cancel the reading...");
                    }

                    if (wait >= 400 && wait % 50 == 0)
                    {
                        SendControlC();
                    }
                    if (wait == 800)
                    {
                        Event("Cancel has failed, restarting the probe...");
                        Failure();
                    }
                }
            }
            if (lineBuilder.Length > 0) lines.Add(lineBuilder.ToString().Trim());
            stopwatch.Stop();

            if (!timeout)
                Event("Reading completed (" + stopwatch.Elapsed + ")");

            return lines;
        }

        private bool ConnectByTelnet(string host, string manufacture, string user, string pass)
        {
            int expect = -1;
            bool connectSuccess = false;

            Event("Connecting with Telnet... (" + user + "@" + host + ")");
            SendLine("telnet " + host);

            if (manufacture == alu)
            {
                #region alu
                expect = MCEExpect("ogin:");
                if (expect == 0)
                {
                    Event("Authenticating: User");
                    SendLine(user);

                    expect = MCEExpect("assword:");
                    if (expect == 0)
                    {
                        Event("Authenticating: Password");
                        SendLine(pass);

                        expect = MCEExpect("#", "ogin:", "closed by foreign");
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
                expect = MCEExpect("sername:", "closed by foreign");
                if (expect == 0)
                {
                    Event("Authenticating: User");
                    SendLine(user);

                    expect = MCEExpect("assword:");
                    if (expect == 0)
                    {
                        Event("Authenticating: Password");
                        SendLine(pass);

                        expect = MCEExpect(">", "sername:", "Tacacs server reject");
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
                expect = MCEExpect("sername:");
                if (expect == 0)
                {
                    Event("Authenticating: User");
                    SendLine(user);

                    expect = MCEExpect("assword:");
                    if (expect == 0)
                    {
                        Event("Authenticating: Password");
                        SendLine(pass);

                        expect = MCEExpect("#", "sername:", "closed by foreign", "cation failed");
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

        private bool ConnectBySSH(string host, string manufacture, string user, string pass)
        {
            int expect = -1;
            bool connectSuccess = false;

            Event("Connecting with SSH... (" + user + "@" + host + ")");
            SendLine("ssh -o StrictHostKeyChecking=no " + user + "@" + host);

            if (manufacture == hwe)
            {
                #region hwe

                expect = MCEExpect("assword:", "Connection refused");
                if (expect == 0)
                {
                    Event("Authenticating: Password");
                    SendLine(pass);

                    expect = MCEExpect(">", "assword:");
                    if (expect == 0) connectSuccess = true;
                    else SendControlC();
                }
                else SendControlC();

                #endregion
            }
            else if (manufacture == cso)
            {
                #region cso

                expect = MCEExpect("assword:", "Connection refused");
                if (expect == 0)
                {
                    Event("Authenticating: Password");
                    SendLine(pass);

                    expect = MCEExpect("#", "assword:");
                    if (expect == 0) connectSuccess = true;
                    else SendControlC();
                }
                else SendControlC();

                #endregion
            }
            else if (manufacture == jun)
            {
                #region jun

                expect = MCEExpect("password:");
                if (expect == 0)
                {
                    Event("Authenticating: Password");
                    SendLine(pass);

                    expect = MCEExpect(">", "assword:");
                    if (expect == 0) connectSuccess = true;
                    else SendControlC();
                }
                else SendControlC();

                #endregion
            }
            else SendControlC();

            return connectSuccess;
        }

        private void Save()
        {
            StringBuilder sb;

            sb = new StringBuilder();
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

                Result r = j.Execute(sql, nodeID);
            }

            sb = new StringBuilder();

            Result nsr = Query("select * from NodeSummary where NS_NO = {0}", nodeID);
            Dictionary<string, string[]> nsd = new Dictionary<string, string[]>();
            foreach (Row ns in nsr)
            {
                string nsk = ns["NS_Key"].ToString();
                string nsid = ns["NS_ID"].ToString();
                string nsv = ns["NS_Value"].ToString();

                if (nsd.ContainsKey(nsk))
                {
                    // Duplicated summary key, remove this
                    sb.Append(j.Format("delete from NodeSummary where NS_ID = {0};", nsid));
                }
                else
                    nsd.Add(nsk, new string[] { nsid, nsv });
            }
                        
            foreach (KeyValuePair<string, string> pair in summaries)
            {
                string[] db = null;
                if (nsd.ContainsKey(pair.Key)) db = nsd[pair.Key];

                if (db == null)
                {
                    if (pair.Value == null)
                    {

                    }
                    else
                    {
                        // insert
                        sb.Append(j.Format("insert into NodeSummary(NS_ID, NS_NO, NS_Key, NS_Value) values({0}, {1}, {2}, {3});",
                            Database.ID(), nodeID, pair.Key, pair.Value));
                    }
                }
                else
                {
                    string dbi = db[0];
                    string dbv = db[1];

                    if (pair.Value == null) sb.Append(j.Format("delete from NodeSummary where NS_ID = {0};", dbi));
                    else if (pair.Value != dbv) sb.Append(j.Format("update NodeSummary set NS_Value = {0} where NS_ID = {1};", pair.Value, dbi));
                }
            }

            if (sb.Length > 0)
            {
                j.Execute(sb.ToString());
            }
        }

        private void Enter(Row row, out bool continueProcess, bool prioritizeProcess)
        {
            continueProcess = false;

            WaitUntilMCETerminalReady("MCE Waiting I");

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

            string previousRemark = row["NO_Remark"].ToString();

            bool timeout;
            string nodeUser = tacacUser;
            string nodePass = tacacPassword;

            Event("Begin probing into " + nodeName);
            if (nodeIP != null) Event("Host IP: " + nodeIP);
            Event("Manufacture: " + nodeManufacture + "");
            if (nodeModel != null) Event("Model: " + nodeModel);

            DateTime now = DateTime.Now;

            Update(UpdateTypes.Remark, null);
            Update(UpdateTypes.RemarkUser, null);
            Update(UpdateTypes.TimeStamp, now);

            // check node manufacture
            if (nodeManufacture == alu || nodeManufacture == cso || nodeManufacture == hwe || nodeManufacture == jun) ;
            else
            {
                Event("Unsupported node manufacture");
                Save();
                return;
            }

            #region CHECK IP

            Event("Checking host IP");
            string resolvedIP = MCECheckNodeIP(nodeName);

            if (resolvedIP == null) Event("Hostname is unresolved");
            if (nodeIP == null)
            {
                if (resolvedIP == null)
                {
                    #region null, null
                    if (previousRemark == "UNRESOLVED")
                        Update(UpdateTypes.Active, 0);
                    else
                        Update(UpdateTypes.Remark, "UNRESOLVED");

                    Save();
                    return;
                    #endregion
                }
                else
                {
                    #region null, RESOLVED!
                    Event("Host IP Resolved: " + resolvedIP);
                    nodeIP = resolvedIP;
                    Update(UpdateTypes.IP, nodeIP);
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
                    string hostName = MCECheckNodeIP(nodeIP, true);

                    if (hostName != null)
                    {
                        #region RESOLVED!, null, RESOLVED!
                        Event("Hostname has probably changed to: " + hostName);

                        Result result = Query("select * from Node where NO_Name = {0}", hostName);

                        if (result.Count == 1)
                        {
                            #region CHANGED to existing???

                            string existingtype = result[0]["NO_Type"].ToString();
                            string existingnodeid = result[0]["NO_ID"].ToString();

                            if (existingtype == "P")
                            {
                                // cek interface count
                                Column interfaceCount = j.Scalar("select count(PI_ID) from PEInterface where PI_NO = {0}", existingnodeid);

                                string deletethisnode;
                                string keepthisnode;

                                int ci = interfaceCount.ToInt();

                                if (ci > 0)
                                {
                                    Event("Existing node has found, delete this node");
                                    // yg existing sudah punya interface, yg ini dihapus aja
                                    deletethisnode = nodeID;
                                    keepthisnode = existingnodeid;

                                    Event("Creating alias");
                                    Execute("insert into NodeAlias(NA_ID, NA_NO, NA_Name) values({0}, {1}, {2})",
                                        Database.ID(), existingnodeid, nodeName);
                                }
                                else
                                {
                                    Event("Delete/update existing node properties");
                                    // yg existing kosong, pake yg ini, rename ini jadi existing, hapus existing
                                    deletethisnode = existingnodeid;
                                    keepthisnode = nodeID;

                                    Update(UpdateTypes.Name, hostName);
                                }

                                int n;
                                // update POP
                                n = Execute("update PEPOP set PO_NO = {0} where PO_NO = {1}", keepthisnode, deletethisnode).AffectedRows;
                                if (n == 1) Event("Update PoP OK");
                                // update ME_TO_PI
                                n = Execute("update MEInterface set MI_TO_PI = null where MI_TO_PI in (select PI_ID from PEInterface where PI_NO = {0})", deletethisnode).AffectedRows;
                                if (n > 0) Event("Update ME interface to PI: " + n + " entries");
                                // hapus interface IP
                                n = Execute("delete from PEInterfaceIP where PP_PI in (select PI_ID from PEInterface where PI_NO = {0})", deletethisnode).AffectedRows;
                                if (n > 0) Event("Delete interface IP: " + n + " entries");
                                // hapus interface
                                n = Execute("delete from PEInterface where PI_NO = {0}", deletethisnode).AffectedRows;
                                if (n > 0) Event("Delete interface: " + n + " entries");
                                // hapus QOS
                                n = Execute("delete from PEQOS where PQ_NO = {0}", deletethisnode).AffectedRows;
                                if (n > 0) Event("Delete QOS: " + n + " entries");
                                // hapus Route Name
                                n = Execute("delete from PERouteName where PN_NO = {0}", deletethisnode).AffectedRows;
                                if (n > 0) Event("Delete route name: " + n + " entries");
                                // hapus Node
                                n = Execute("delete from Node where NO_ID = {0}", deletethisnode).AffectedRows;
                                if (n > 0) Event("Node deleted");
                            }
                            else if (existingnodeid == "M")
                            {
                                // cek interface
                                Column interfaceCount = Scalar("select count(MI_ID) from MEInterface where MI_NO = {0}", existingnodeid);

                                string deletethisnode;
                                string keepthisnode;

                                int ci = interfaceCount.ToInt();

                                if (ci > 0)
                                {
                                    Event("Existing node has found, delete this node");
                                    // yg existing sudah punya interface, yg ini dihapus aja
                                    deletethisnode = nodeID;
                                    keepthisnode = existingnodeid;

                                    Event("Creating alias");
                                    Execute("insert into NodeAlias(NA_ID, NA_NO, NA_Name) values({0}, {1}, {2})",
                                        Database.ID(), existingnodeid, nodeName);
                                }
                                else
                                {
                                    Event("Delete/update existing node properties");
                                    // yg existing kosong, pake yg ini, rename ini jadi existing, hapus existing
                                    deletethisnode = existingnodeid;
                                    keepthisnode = nodeID;

                                    Update(UpdateTypes.Name, hostName);
                                }

                                int n;
                                // hapus customer
                                n = Execute("delete from MECustomer where MU_NO = {0}", deletethisnode).AffectedRows;
                                if (n > 0) Event("Delete customer: " + n + " entries");
                                // hapus service peer
                                n = Execute("delete from MEPeer where MP_MC in (select MC_ID from MECircuit where MC_NO = {0})", deletethisnode).AffectedRows;
                                if (n > 0) Event("Delete service peer: " + n + " entries");
                                // hapus interface
                                n = Execute("delete from MEInterface where MI_NO = {0}", deletethisnode).AffectedRows;
                                if (n > 0) Event("Delete interface: " + n + " entries");
                                // hapus circuit
                                n = Execute("delete from MECircuit where MC_NO = {0}", deletethisnode).AffectedRows;
                                if (n > 0) Event("Delete service: " + n + " entries");
                                // hapus sdp
                                n = Execute("delete from MESDP where MS_NO = {0}", deletethisnode).AffectedRows;
                                if (n > 0) Event("Delete peer: " + n + " entries");
                                // hapus Node
                                n = Execute("delete from Node where NO_ID = {0}", deletethisnode).AffectedRows;
                                if (n > 0) Event("Node deleted");
                            }

                            #endregion
                        }
                        else
                        {
                            #region NO PROBLEM

                            // simply change to the new one
                            Update(UpdateTypes.Name, hostName);

                            // insert old name alias
                            Event("Creating alias");
                            Execute("insert into NodeAlias(NA_ID, NA_NO, NA_Name) values({0}, {1}, {2})",
                                Database.ID(), nodeID, nodeName);

                            #endregion
                        }

                        Save();
                        return;
                        #endregion
                    }
                    else
                    {
                        #region RESOLVED!, null, null

                        if (previousRemark == "UNRESOLVED")
                            Update(UpdateTypes.Active, 0);
                        else
                            Update(UpdateTypes.Remark, "UNRESOLVED");

                        Save();
                        return;
                        #endregion                            
                    }
                    #endregion
                }
                else if (nodeIP != resolvedIP)
                {
                    #region IP HAS CHANGED

                    Event("IP has changed to: " + resolvedIP + "");

                    Update(UpdateTypes.Remark, "IPHASCHANGED");
                    Update(UpdateTypes.Active, 0);

                    Save();
                    return;

                    #endregion
                }
            }

            Event("Host identified");

            outputIdentifier = nodeName;

            #endregion

            #region CONNECT

            Update(UpdateTypes.RemarkUser, nodeUser);

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
                WaitUntilMCETerminalReady("MCE Waiting II");

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
                    connectSuccess = ConnectByTelnet(nodeName, nodeManufacture, nodeUser, nodePass);
                    if (connectSuccess) connectBy = "T";
                    else Event("Telnet failed");
                }
                else if (currentConnectType == "S")
                {
                    connectSuccess = ConnectBySSH(nodeName, nodeManufacture, nodeUser, nodePass);
                    if (connectSuccess) connectBy = "S";
                    else Event("SSH failed");
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
                    WaitUntilMCETerminalReady("MCE Waiting III");

                    string testOtherNode;

                    if (nodeName == "PE2-D2-CKA-VPN") testOtherNode = "PE-D2-CKA-VPN";
                    else testOtherNode = "PE2-D2-CKA-VPN";

                    Event("Trying to connect to other node...(" + testOtherNode + ")");

                    bool testConnected = ConnectByTelnet(testOtherNode, cso, nodeUser, nodePass);

                    if (testConnected)
                    {
                        Exit(cso);
                        outputIdentifier = null;
                        Event("Connected! TACAC server is OK.");
                        break;
                    }
                    else
                    {
                        tacacError = true;
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

                if (tacacError)
                {
                    // this node is innocent
                    // TODO: try again?
                }
                else
                {
                    if (previousRemark == "CONNECTFAIL")
                        Update(UpdateTypes.Active, 0);
                    else
                        Update(UpdateTypes.Remark, "CONNECTFAIL");

                }

                Save();
                return;
            }

            if (nodeConnectType == null || connectBy != connectType)
                Update(UpdateTypes.ConnectType, connectBy);

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
                    Save();
                    Exit();
                    return;
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

            if (terminal != nodeTerminal) Update(UpdateTypes.Terminal, terminal);
            nodeTerminal = terminal;

            #endregion

            #region TERMINAL SETUP

            Event("Setup terminal");

            noMore = true; // by default, we can no more

            if (nodeManufacture == alu)
            {

                SendLine("environment no saved-ind-prompt", true);
                Read(out timeout);
                if (timeout) { SaveExit(); return; }

                SendLine("environment no more");
                List<string> lines = Read(out timeout);
                if (timeout) { SaveExit(); return; }

                string oline = string.Join(" ", lines);
                if (oline.IndexOf("CLI Command not allowed for this user.") > -1)
                    noMore = false;
                else
                    noMore = true;
            }
            else if (nodeManufacture == hwe)
            {
                SendLine("screen-length 0 temporary");
                Read(out timeout);
                if (timeout) { SaveExit(); return; }
            }
            else if (nodeManufacture == cso)
            {
                SendLine("terminal length 0");
                Read(out timeout);
                if (timeout) { SaveExit(); return; }
            }
            else if (nodeManufacture == jun)
            {
                SendLine("set cli screen-length 0");
                Read(out timeout);
                if (timeout) { SaveExit(); return; }
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
                    SendLine("show version | match TiMOS");
                    List<string> lines = Read(out timeout);
                    if (timeout) { SaveExit(); return; }

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
                    SendLine("display version");
                    List<string> lines = Read(out timeout);
                    if (timeout) { SaveExit(); return; }

                    foreach (string line in lines)
                    {
                        if (line.StartsWith("VRP (R) software"))
                        {
                            version = line.Substring(26, line.IndexOf(' ', 26) - 26).Trim();
                            break;
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
                    SendLine("show version | in IOS");
                    List<string> lines = Read(out timeout);
                    if (timeout) { SaveExit(); return; }

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
                        SendLine("show version | in bytes of memory");
                        lines = Read(out timeout);
                        if (timeout) { SaveExit(); return; }

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
                    SendLine("show version | match \"JUNOS Base OS boot\"");
                    List<string> lines = Read(out timeout);
                    if (timeout) { SaveExit(); return; }

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
                    Update(UpdateTypes.Model, model);
                    Event("Model discovered: " + model);
                }
                if (version != nodeVersion)
                {
                    nodeVersion = version;
                    Update(UpdateTypes.Version, version);
                    Event("Version updated: " + version);
                }
                if (subVersion != nodeSubVersion)
                {
                    nodeSubVersion = subVersion;
                    Update(UpdateTypes.SubVersion, subVersion);
                    Event("SubVersion updated: " + subVersion);
                }

                Update(UpdateTypes.VersionTime, now);
            }

            if (nodeVersion == null)
            {
                Event("Cant determined node version.");
                SaveExit();
                return;
            }

            Event("Version: " + nodeVersion + ((nodeSubVersion != null) ? ":" + nodeSubVersion : ""));

            #endregion

            #region LAST CONFIGURATION

            Event("Checking Last Configuration");

            bool configurationHasChanged = false;
            bool lastconfliveisnull = true;

            DateTime lastconflive = DateTime.MinValue;

            if (nodeManufacture == alu)
            {
                #region alu
                SendLine("show system information | match \"Time Last Modified\"");
                List<string> lines = Read(out timeout);
                if (timeout) { SaveExit(); return; }

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
                    SendLine("show system information | match \"Time Last Saved\"");
                    lines = Read(out timeout);
                    if (timeout) { SaveExit(); return; }

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
                SendLine("display changed-configuration time");
                List<string> lines = Read(out timeout);
                if (timeout) { SaveExit(); return; }

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
                    SendLine("show configuration history commit last 1 | in commit");
                    List<string> lines = Read(out timeout);
                    if (timeout) { SaveExit(); return; }

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

                    // because so many differences between IOS version, we might try every possible command
                    bool passed = false;

                    // most of ios version will work this way
                    SendLine("show configuration id detail");
                    List<string> lines = Read(out timeout);
                    if (timeout) { SaveExit(); return; }

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
                                    Event("Using configuration id");
                                    passed = true;
                                    lastconflive = new DateTime(
                                        lastconflive.Ticks - (lastconflive.Ticks % TimeSpan.TicksPerSecond),
                                        lastconflive.Kind
                                        );
                                    lastconfliveisnull = false;
                                }
                            }
                            break;
                        }
                    }

                    if (passed == false)
                    {
                        // using xr-like command history
                        //show configuration history
                        SendLine("show configuration history");
                        lines = Read(out timeout);
                        if (timeout) { SaveExit(); return; }

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
                                lastconflive = parsedDT;
                                lastconfliveisnull = false;
                            }
                        }
                    }

                    if (passed == false)
                    {
                        // and here we are, using forbidden command ever
                        SendLine("show log | in CONFIG_I");
                        lines = Read(out timeout);
                        if (timeout) { SaveExit(); return; }

                        string lastline = null;
                        foreach (string line in lines)
                        {
                            if (line.StartsWith("*")) lastline = line;
                        }

                        if (lastline != null)
                        {
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

                                lastconflive = parsedDT;
                                lastconfliveisnull = false;
                            }
                        }
                    }

                    if (passed == false)
                    {
                        // and... if everything fail, we will use this slowlest command ever
                        SendLine("show run | in Last config");
                        lines = Read(out timeout);

                        if (timeout) { SaveExit(); return; }

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
                                    lastconflive = parsedDT;
                                    lastconfliveisnull = false;
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
                SendLine("show system uptime | match \"Last configured\"");
                List<string> lines = Read(out timeout);
                if (timeout) { SaveExit(); return; }

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

            if (lastconfliveisnull == false)
            {
                Event("Saved last configuration on " + lastconfdb.ToString("yy/MM/dd HH:mm:ss.fff"));
                Event("Actual last configuration on " + lastconflive.ToString("yy/MM/dd HH:mm:ss.fff"));
            }

            if (lastconfliveisnull == false && lastconflive != lastconfdb)
            {
                Event("Configuration has changed!");
                Update(UpdateTypes.LastConfiguration, lastconflive);

                //continueProcess = true;
                configurationHasChanged = true;
            }
            else
            {
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

                SendLine("show processes cpu | in CPU");
                List<string> lines = Read(out timeout);
                if (timeout) { SaveExit(); return; }

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
                    SendLine("show memory summary | in Physical Memory");
                    lines = Read(out timeout);
                    if (timeout) { SaveExit(); return; }

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
                    SendLine("show process memory | in Total:");
                    lines = Read(out timeout);

                    if (timeout) { SaveExit(); return; }

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

                SendLine("show system cpu | match \"Busiest Core\"");
                List<string> lines = Read(out timeout);
                if (timeout) { SaveExit(); return; }

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
                SendLine("show system memory-pools | match bytes");
                lines = Read(out timeout);
                if (timeout) { SaveExit(); return; }

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

                SendLine("display cpu-usage");
                List<string> lines = Read(out timeout);
                if (timeout) { SaveExit(); return; }


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

                SendLine("display memory-usage");
                lines = Read(out timeout);

                if (timeout) { SaveExit(); return; }

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
                SendLine("show chassis routing-engine | match Idle");
                List<string> lines = Read(out timeout);
                if (timeout) { SaveExit(); return; }

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
                SendLine("show task memory");
                lines = Read(out timeout);

                if (timeout) { SaveExit(); return; }

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

            if (configurationHasChanged || nodeNVER < Necrow.Version)
            {
                continueProcess = true;
                if (nodeNVER < Necrow.Version)
                {
                    Event("Updated to newer Necrow version");
                    Update(UpdateTypes.NecrowVersion, Necrow.Version);
                }
            }
            else if (prioritizeProcess)
            {
                Event("Prioritized node, continuing process");
                continueProcess = true;
            }
            else
            {
                SaveExit();
            }
        }

        #endregion

        private void ServiceExecute(ServiceReference reference)
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

            Result customerresult = Query("select * from ServiceCustomer where SC_CID in ('" + string.Join("','", customerid.ToArray()) + "')");
            foreach (Row row in customerresult)
            {
                customerdb.Add(row["SC_CID"].ToString(), row);
                if (!row["SC_Name_Set"].ToBoolean(false)) servicebycustomerid.Add(row["SC_ID"].ToString());
            }
            Result serviceresult = Query("select * from Service where SE_SID in ('" + string.Join("','", serviceid.ToArray()) + "') or SE_SC in ('" + string.Join("','", servicebycustomerid.ToArray()) + "')");
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

                if (c_id != null)
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
                batch.Execute("insert into ServiceCustomer(SC_ID, SC_CID, SC_Name) values({0}, {1}, {2})", s.ID, s.CID, s.Name);
            }
            result = batch.Commit();
            if (result.AffectedRows > 0) Event(result.AffectedRows + " customer(s) have been added");

            // CUSTOMER UPDATE
            batch.Begin();
            foreach (CustomerToDatabase s in customerupdate)
            {
                batch.Execute("update ServiceCustomer set SC_Name = {0} where SC_ID = {1}", s.Name, s.ID);
            }
            result = batch.Commit();
            if (result.AffectedRows > 0) Event(result.AffectedRows + " customer(s) have been updated");

            // SERVICE ADD
            batch.Begin();
            foreach (ServiceToDatabase s in serviceinsert)
            {
                batch.Execute("insert into Service(SE_ID, SE_SID, SE_SC, SE_Type, SE_SubType, SE_Raw_Desc) values({0}, {1}, {2}, {3}, {4}, {5})", s.ID, s.SID, s.CustomerID, s.Type, s.SubType, s.RawDesc);
            }
            result = batch.Commit();
            if (result.AffectedRows > 0) Event(result.AffectedRows + " service(s) have been added");

            // SERVICE UPDATE
            batch.Begin();
            foreach (ServiceToDatabase s in serviceupdate)
            {
                batch.Execute("update Service set SE_Type = {0}, SE_SubType = {1} where SE_ID = {2}", s.Type, s.SubType, s.ID);
            }
            result = batch.Commit();
            if (result.AffectedRows > 0) Event(result.AffectedRows + " service(s) have been updated");
        }

        private bool findMEPhysicalAdjacentLoaded = false;
        private List<Tuple<string, List<Tuple<string, string, string>>>> MEPEAdjacent = null;
        private Dictionary<string, List<string>> meAlias = null;
        private Dictionary<string, string[]> MEInterfaceTestPrefix = null;

        private void FindMEPhysicalAdjacent(MEInterfaceToDatabase li)
        {
            int exid;
            string description = li.Description;
            if (description == null) return;
            description = description.ToUpper().Replace('_', ' ');

            #region Loader

            if (!findMEPhysicalAdjacentLoaded)
            {
                Result result = Query(@"
select NO_Name, LEN(NO_Name) as NO_LEN, PI_Name, LEN(PI_Name) as PI_LEN, PI_ID, PI_Description from (
select NO_Name, NO_ID from Node where NO_Type = 'P'
union
select NA_Name, NA_NO from NodeAlias, Node where NA_NO = NO_ID and NO_Type = 'P'
) n, PEInterface
where NO_ID = PI_NO and PI_Description is not null and ltrim(rtrim(PI_Description)) <> '' and PI_Name not like '%.%' and
(PI_Name like 'Te%' or PI_Name like 'Gi%' or PI_Name like 'Fa%' or PI_Name like 'Et%')
order by NO_LEN desc, NO_Name, PI_LEN desc, PI_Name
");

                MEPEAdjacent = new List<Tuple<string, List<Tuple<string, string, string>>>>();
                List<Tuple<string, string, string>> curlist = new List<Tuple<string, string, string>>();
                string curnoname = null;
                foreach (Row row in result)
                {
                    string noname = row["NO_Name"].ToString();
                    string piname = row["PI_Name"].ToString();
                    string pidesc = row["PI_Description"].ToString();
                    string piid = row["PI_ID"].ToString();

                    if (curnoname != noname)
                    {
                        if (curnoname != null)
                        {
                            MEPEAdjacent.Add(new Tuple<string, List<Tuple<string, string, string>>>(curnoname, new List<Tuple<string, string, string>>(curlist)));
                            curlist.Clear();
                        }
                        curnoname = noname;                  
                    }

                    curlist.Add(new Tuple<string, string, string>(piname, pidesc, piid));
                }
                MEPEAdjacent.Add(new Tuple<string, List<Tuple<string, string, string>>>(curnoname, curlist));

                result = Query(@"
select NO_ID, NA_Name from Node, NodeAlias where NA_NO = NO_ID and NO_Type = 'M'
order by NO_ID asc
");
                meAlias = new Dictionary<string, List<string>>();

                foreach (Row row in result)
                {
                    string noid = row["NO_ID"].ToString();
                    if (!meAlias.ContainsKey(noid)) meAlias.Add(noid, new List<string>());
                    meAlias[noid].Add(row["NA_Name"].ToString());
                }

                MEInterfaceTestPrefix = new Dictionary<string, string[]>();
                MEInterfaceTestPrefix.Add("Hu", new string[] { "H", "HU" });
                MEInterfaceTestPrefix.Add("Te", new string[] { "T", "TE", "TENGIGE" });
                MEInterfaceTestPrefix.Add("Gi", new string[] { "G", "GI", "GE", "GIGAE", "GIGABITETHERNET" });
                MEInterfaceTestPrefix.Add("Fa", new string[] { "F", "FA", "FE", "FASTE" });
                MEInterfaceTestPrefix.Add("Et", new string[] { "E", "ET", "ETH" });

                findMEPhysicalAdjacentLoaded = true;
            }

            #endregion
            
            #region Bekas/Ex/X dsb, remove hingga akhir

            exid = description.IndexOf(" EX ", " EKS ", "(EX", "(EKS", "[EX", "[EKS", " EX-", " EKS-", " BEKAS ");
            if (exid > -1) description = description.Remove(exid);

            #endregion

            bool foundnode = false;

            foreach (Tuple<string, List<Tuple<string, string, string>>> pe in MEPEAdjacent)
            {
                string peName = pe.Item1;
                List<Tuple<string, string, string>> pis = pe.Item2;

                int peNamePart = description.IndexOf(peName);
                if (peNamePart > -1)
                {
                    foundnode = true;
                    string descPEPart = description.Substring(peNamePart);
                    Tuple<string, string, string> matchedPI = null;

                    #region Find in currently available PI

                    int locPI = descPEPart.Length;
                    foreach (Tuple<string, string, string> pi in pis)
                    {
                        string piName = pi.Item1;
                        string piType = piName.Substring(0, 2);
                        string piDetail = piName.Substring(2);

                        List<string> testIf = new List<string>();
                        string[] prefixs = MEInterfaceTestPrefix[piType];

                        foreach (string prefix in prefixs)
                        {
                            testIf.Add(prefix + piDetail);
                            testIf.Add(prefix + "-" + piDetail);
                            testIf.Add(prefix + " " + piDetail);
                            testIf.Add(prefix + "/" + piDetail);
                        }

                        testIf = List.Sort(testIf, SortMethods.LengthDescending);

                        foreach (string test in testIf)
                        {
                            int pos = descPEPart.IndexOf(test);
                            if (pos > -1 && pos < locPI)
                            {
                                locPI = pos;
                                matchedPI = pi;
                            }
                        }
                    }

                    #endregion

                    if (matchedPI != null)
                    {
                        #region Crosscheck matched PI description

                        string piDesc = matchedPI.Item2.ToUpper();
                        string miName = li.Name;
                        string miType = miName.Substring(0, 2);
                        if (miType == "Ex") miType = li.InterfaceType;
                        string miDetail = miName.Substring(2);

                        #region Bekas/Ex/X dsb, remove hingga akhir
                        exid = piDesc.IndexOf(" EX ", " EKS ", "(EX", "(EKS", "[EX", "[EKS", " EX-", " EKS-", " BEKAS ");
                        if (exid > -1) piDesc = piDesc.Remove(exid);
                        #endregion

                        int meNamePart = piDesc.IndexOf(nodeName);

                        if (meNamePart == -1)
                        {
                            // if -1, test with alias
                            if (meAlias.ContainsKey(nodeID))
                            {
                                List<string> aliases = meAlias[nodeID];
                                foreach (string alias in aliases)
                                {
                                    meNamePart = piDesc.IndexOf(alias);
                                    if (meNamePart > -1) break;
                                }
                            }
                        }

                        if (meNamePart > -1) // at least we can find me name or the alias in pi description
                        {
                            string descMEPart = piDesc.Substring(meNamePart);

                            List<string> testIf = new List<string>();
                            string[] prefixs = MEInterfaceTestPrefix[miType];

                            foreach (string prefix in prefixs)
                            {
                                testIf.Add(miDetail);
                                testIf.Add(prefix + miDetail);
                                testIf.Add(prefix + "-" + miDetail);
                                testIf.Add(prefix + " " + miDetail);
                                testIf.Add(prefix + "/" + miDetail);
                            }
                            testIf = List.Sort(testIf, SortMethods.LengthDescending);

                            bool foundinterface = false;
                            foreach (string test in testIf)
                            {
                                if (descMEPart.IndexOf(test) > -1)
                                {
                                    foundinterface = true;
                                    break;
                                }
                            }

                            if (foundinterface)
                            {
                                li.AdjacentID = matchedPI.Item3;

                                li.AdjacentSubifID = new Dictionary<string, string>();

                                // find pi child
                                Result result = Query("select PI_ID, PI_Name from PEInterface where PI_PI = {0}", li.AdjacentID);
                                foreach (Row row in result)
                                {
                                    string spiid = row["PI_ID"].ToString();
                                    string spiname = row["PI_Name"].ToString();

                                    int dot = spiname.IndexOf('.');
                                    if (dot > -1 && spiname.Length > (dot + 1))
                                    {
                                        string sifname = spiname.Substring(dot + 1);
                                        if (!li.AdjacentSubifID.ContainsKey(sifname))
                                        {
                                            li.AdjacentSubifID.Add(sifname, spiid);
                                        }
                                    }
                                }

                            }
                        }

                        #endregion

                        break;
                    }
                }
            }

            if (foundnode == false)
            {
                FindNodeCandidate(description);
            }
        }

        private Dictionary<string, string> nodeCandidates = null;
        private List<string> nodes = null;

        private void FindNodeCandidate(string description)
        {
            if (description == null) return;

            if (nodeCandidates == null)
            {
                nodeCandidates = new Dictionary<string, string>();

                Result result = Query("select * from NodeCandidate");
                foreach (Row row in result)
                {
                    string ncid = row["NC_ID"].ToString();
                    string ncname = row["NC_Name"].ToString();
                    if (!nodeCandidates.ContainsKey(ncname)) nodeCandidates.Add(ncname, ncid);
                }

                nodes = new List<string>();

                result = Query("select NO_Name from Node");
                foreach (Row row in result)
                {
                    string noname = row["NO_Name"].ToString();
                    nodes.Add(noname);
                }
            }

            // lets find some new node here
            string[] descs = description.Split(new char[] { ' ', '(', ')', '_', '[', ']', ';', '.', '=', ':', '@', '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string des in descs)
            {
                if (des.Length >= 8 && des.IndexOf('-') > -1 && (des.StartsWith("PE") || des.StartsWith("ME") || des.StartsWith("CES")) && !des.EndsWith("-"))
                {
                    string[] dess = des.Split(new char[] { '-' });
                    if (dess.Length >= 3)
                    {
                        if (dess[0].StartsWith("ME") && dess[0].Length > 2 && !char.IsDigit(dess[0][2])) continue;
                        else if (dess[0].StartsWith("PE") && dess[0].Length > 2 && !char.IsDigit(dess[0][2])) continue;
                        else if (dess[0].StartsWith("CES") && dess[0].Length > 3 && !char.IsDigit(dess[0][3])) continue;

                        bool illegal = false;
                        foreach (char c in des)
                        {
                            if (c == '-' || char.IsDigit(c) || (char.IsLetter(c) && char.IsUpper(c))) { }
                            else { illegal = true; break; }
                        }

                        if (illegal == false)
                        {
                            string thisdes = des.ToUpper();

                            // last check if ending with -<numeric> its fail
                            string[] lxcp = thisdes.Split('-');
                            if (!char.IsDigit(lxcp[lxcp.Length - 1][0]))
                            {
                                if (!nodes.Contains(thisdes))
                                {
                                    if (!nodeCandidates.ContainsKey(thisdes))
                                    {
                                        Event("Node Candidate: " + thisdes);

                                        string ncid = Database.ID();
                                        nodeCandidates.Add(thisdes, ncid);
                                        Execute("insert into NodeCandidate values({0}, {1})", ncid, thisdes);
                                    }
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion
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

        private static string[] monthsEnglish = new string[] { "JANUARY", "FEBRUARY", "MARCH", "APRIL", "MAY", "JUNE", "JULY", "AUGUST", "SEPTEMBER", "OCTOBER", "NOVEMBER", "DECEMBER" };
        private static string[] monthsBahasa = new string[] { "JANUARI", "FEBRUARI", "MARET", "APRIL", "MEI", "JUNI", "JULI", "AGUSTUS", "SEPTEMBER", "OKTOBER", "NOVEMBER", "DESEMBER" };

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
                        de.SID = d.Substring(rmvn, end - rmv - rle).Trim();
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
                                            List.StartsWith(monthsEnglish, first) > -1 ||
                                            List.StartsWith(monthsBahasa, first) > -1
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
                                            List.StartsWith(monthsEnglish, second) > -1 ||
                                            List.StartsWith(monthsBahasa, second) > -1
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

                                        if (List.StartsWith(monthsEnglish, t) > -1 ||
                                            List.StartsWith(monthsBahasa, t) > -1)
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
                    string fixsid = de.SID.Trim(new char[] { '-', ')', '(', '[', ']', '>', '<' });
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

            return de;
        }

        #endregion
    }
}
