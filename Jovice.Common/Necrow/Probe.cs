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

        private StopState stop;

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
        private readonly int batchmax = 20;

        private string xr = "XR";

        private string feature = null;//"interface";

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
            Event("Stop requested");

            outputIdentifier = null;
            Event("Disconnecting...");

            stop = StopState.Stop;

            mainLoop.Abort();
            mainLoop = null;

            base.Stop();
        }

        private void Failure()
        {
            Event("Connection failure has occured");

            outputIdentifier = null;
            Event("Disconnecting...");

            stop = StopState.Failure;

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
                    bool forceProcess = false;

                    if (prioritize.Count > 0)
                    {
                        string nodeName = prioritize.Dequeue();

                        if (nodeName.EndsWith("*"))
                        {
                            nodeName = nodeName.TrimEnd(new char[] { '*' });
                            forceProcess = true;
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

                        Enter(node, out continueProcess, forceProcess);

                        if (continueProcess)
                        {
                            Event("Continue process...");
                            if (nodeType == "P") PEProcess();
                            else if (nodeType == "M") MEProcess();
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
                    Thread.Sleep(100);
                    wait++;
                } 
                if (continueWait == false) break;

                // else continue wait...
                loop++;
                if (loop == 3) Failure(); // loop 3 times, its a failure

                // print message where we are waiting (or why)
                Event(waitMessage + "... (" + loop + ")");

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
            Thread.Sleep(100);

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

        private List<string> Read()
        {
            bool timeout;
            return Read(out timeout);
        }

        private List<string> Read(out bool timeout)
        {
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

                    if (wait % 50 == 0)
                    {
                        Event("Waiting...");
                    }

                    Thread.Sleep(100);
                    if (wait == 400)
                    {
                        timeout = true;
                        Event("Reading timeout");

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
        
        private bool ConnectByTelnet(string host, string manufacture, string user, string pass)
        {
            int expect = -1;
            bool connectSuccess = false;

            Event("Connecting with Telnet... (" + user + "@" + host + ")");

            if (WriteLine("telnet " + host)) { requestFailure = true; return false; }

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

            Result nsr = Query("select * from NodeSummary where NS_NO = {0}", nodeID);
            Dictionary<string, string[]> nsd = new Dictionary<string, string[]>();
            foreach (Row ns in nsr)
            {
                string nsk = ns["NS_Key"].ToString();
                string nsid = ns["NS_ID"].ToString();
                string nsv = ns["NS_Value"].ToString();

                nsd.Add(nsk, new string[] { nsid, nsv });
            }

            sb = new StringBuilder();
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

        private void Enter(Row row, out bool continueProcess, bool forceProcess)
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

                    if (nodeName == "PE-D2-TAN") testOtherNode = "PE2-D2-JT2-MGT";
                    else testOtherNode = "PE-D2-TAN";

                    Event("Trying to connect to other node...(" + testOtherNode + ")");

                    bool testConnected = ConnectByTelnet(testOtherNode, cso, nodeUser, nodePass);

                    if (testConnected)
                    {
                        Exit(cso);
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
                Event("Last configuration on " + lastconflive.ToString("yy/MM/dd HH:mm:ss.fff"));
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
                if (nodeNVER < Necrow.Version) Update(UpdateTypes.NecrowVersion, Necrow.Version);
            }
            else if (forceProcess)
            {
                Event("Process forced to continue");
                continueProcess = true;
            }
            else
            {
                SaveExit();
            }
        }

        #endregion

        #endregion
    }
}
