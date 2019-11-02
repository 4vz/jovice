using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Cryptography;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Web;

namespace Aphysoft.Share
{
    internal enum NodeType
    {
        Default,
        Web
    }

    public abstract class Node
    {
        #region Const

        internal static readonly int AppsPort = 23470;

        // this port used for: resolver, events

        internal static readonly int NodeCoordinatorPort = 23471;

        internal static readonly int ServiceStartPort = 23472;

        internal static readonly int ServiceEndPort = 23500;



        internal static readonly int ServiceEventPort = 23472;

        internal static readonly int BufferSize = 8192;

        internal static readonly byte[] HandshakeHead = new byte[] { (byte)'A', (byte)'F', (byte)'I', (byte)'S' };

        internal static readonly byte[] ServiceEventHead = new byte[] { (byte)'E', (byte)'V', (byte)'N', (byte)'T' };

        #endregion

        #region Fields

        private NodeType Type { get; } = NodeType.Default;

        private bool DoEvent { get; } = false;

        private bool DoDebug { get; set; } = true;

        public string Name { get; } = null;

        public string Directory { get; private set; } = null;

        private bool ready = false;

        public bool IsRunning { get; private set; } = false;

        private bool coordinator = true;

        private string aphysoftDataDirectory = null;

        public string AphysoftDataDirectory
        {
            get
            {
                if (aphysoftDataDirectory == null)
                    aphysoftDataDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Aphysoft");
                return aphysoftDataDirectory;
            }
        }

        private string coordinatorDirectory = null;

        private string CoordinatorDirectory
        {
            get
            {
                if (coordinatorDirectory == null)
                    coordinatorDirectory = Path.Combine(AphysoftDataDirectory, "Apps", "Coordinator");
                return coordinatorDirectory;
            }
        }

        private string coordinatorPortsFile = null;

        private string CoordinatorPortsFile
        {
            get
            {
                if (coordinatorPortsFile == null)
                    coordinatorPortsFile = Path.Combine(CoordinatorDirectory, "ports");
                return coordinatorPortsFile;
            }
        }

        private TcpListener coordinatorListener = null;

        private Thread serviceEventThread = null;

        private TcpListener serviceEventListener = null;

        private Dictionary<TcpClient, Tuple<byte[], byte[]>> serviceEventClients = null;

        protected Thread instanceThread = null;

        private List<Edge> edges = new List<Edge>();

        public Edge[] Edges
        {
            get
            {
                List<Edge> connectedEdges = new List<Edge>();

                foreach (Edge e in edges)
                {
                    if (e.IsConnected) connectedEdges.Add(e);
                }

                return connectedEdges.ToArray();
            }
        }

        private IPAddress bindingAddress = null;

        private Socket serverSocket = null;

        private bool serverStarted = false;

        private Thread serverThread = null;

        private ManualResetEvent edgeWaitSignal = new ManualResetEvent(false);

        private ManualResetEvent serviceEventSignal = new ManualResetEvent(false);

        private List<InstallationFile> installing = null;

        private bool restarting = false;

        private string lastEventMessage = null;

        private int lastEventRepeat = 0;

        private CancellationTokenSource terminalCancel = new CancellationTokenSource();

        #endregion
         
        public Node(string name, bool doEvent)
        {
            string upperName = name.ToUpper();

            if (upperName.ArgumentIndexOf("APPS", "WEB") > -1)
            {
                throw new ArgumentException("Illegal Node name", "name");
            }
            else
            {
                Name = name;
            }

            DoEvent = doEvent;
        }

        public Node(string name) : this(name, true)
        {
        }

        internal Node()
        {
            if (this is Web)
            {
                Type = NodeType.Web;
                Name = "WEB";
            }
        }

        #region Virtual

        protected virtual void OnStart() { }

        protected virtual void OnStop() { }

        protected virtual void OnEvent(string message, bool newLine, int repeat)
        {
            if (repeat > 0)
            {
                Apps.Console(message, repeat);
            }
            else
            {
                Apps.Console(message, newLine);
            }
        }

        protected virtual void OnStartUpdate() { }

        protected virtual bool OnUpdated() { return true; }

        protected virtual void OnConnected(EdgeConnectedEventArgs e) { }

        protected virtual void OnDisconnected(EdgeDisconnectedEventArgs e, bool wasAuthenticated) { }

        protected virtual void OnMessage(EdgeMessageEventArgs e)
        {
        }

        protected virtual void OnFileStartReceive(EdgeFileEventArgs e) { }

        protected virtual void OnFileReceived(EdgeFileEventArgs e) { }

        protected virtual void OnTerminalCommand(string command) { }

        #endregion

        #region Methods

        public static implicit operator bool(Node node)
        {
            return node.IsRunning;
        }

        #region Coordinator Ports

        private Dictionary<int, Tuple<string, int>> CoordinatorPortsParseLines(string[] lines)
        {
            Dictionary<int, Tuple<string, int>> dict = new Dictionary<int, Tuple<string, int>>();

            foreach (string line in lines)
            {
                string[] x = line.Split(new[] { ':' });

                if (x.Length == 3 && x[0].Length > 0 && x[1].Length > 0 && x[2].Length > 0)
                {
                    if (int.TryParse(x[0], out int xport))
                    {
                        if (!dict.ContainsKey(xport))
                        {    
                            if (int.TryParse(x[2], out int pid))
                            {
                                Process pbyid = null;

                                try
                                {
                                    pbyid = Process.GetProcessById(pid);
                                }
                                catch (ArgumentException ex)
                                {
                                    // pid is not running, then release
                                }

                                if (pbyid != null)
                                {
                                    if (!Tcp.IsPortListenAvailable(xport))
                                    {
                                        dict.Add(xport, new Tuple<string, int>(x[1], pid));
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return dict;
        }

        private Dictionary<int, Tuple<string, int>> GetCoordinatorPorts()
        {
            string[] lines = null;

            while (lines == null)
            {
                try
                {
                    lines = File.ReadAllLines(CoordinatorPortsFile);
                }
                catch (Exception ex)
                {
                    Debug("Retrying reading coordinator ports file...");
                    Thread.Sleep(100);
                }
            }

            return CoordinatorPortsParseLines(lines);
        }

        internal int GetPort(string usage)
        {
            Dictionary<int, Tuple<string, int>> ports = GetCoordinatorPorts();

            string tn = Name + "-" + usage;

            int port = 0;

            foreach (KeyValuePair<int, Tuple<string, int>> pair in ports)
            {
                string name = pair.Value.Item1;

                if (name == tn)
                {
                    port = pair.Key;
                    break;
                }
            }

            return port;
        }

        private int ReservePort(string usage)
        {
            int selectedPort = 0;

            IO.ExclusiveFileOpen(CoordinatorPortsFile, delegate (List<string> lines)
            {
                Dictionary<int, Tuple<string, int>> ports = CoordinatorPortsParseLines(lines.ToArray());

                for (int inc = ServiceStartPort; inc < ServiceEndPort; inc++)
                {
                    if (!ports.ContainsKey(inc))
                    {
                        // ok, this port is probably not used
                        // lets check
                        if (Tcp.IsPortListenAvailable(inc))
                        {
                            selectedPort = inc;

                            ports.Add(inc, new Tuple<string, int>(Name + "-" + usage, Process.GetCurrentProcess().Id));

                            break;
                        }
                    }
                }

                lines.Clear();

                foreach (KeyValuePair<int, Tuple<string, int>> pair in ports)
                {
                    lines.Add(pair.Key + ":" + pair.Value.Item1 + ":" + pair.Value.Item2);
                }
            });

            if (selectedPort > 0)
                Debug("Reserving port " + selectedPort + " for " + usage);

            return selectedPort;
        }

        private void ReleasePort(int port)
        {
            Debug("Release port " + port);

            IO.ExclusiveFileOpen(CoordinatorPortsFile, delegate (List<string> lines)
            {
                Dictionary<int, Tuple<string, int>> ports = CoordinatorPortsParseLines(lines.ToArray());

                if (ports.ContainsKey(port))
                {
                    ports.Remove(port);

                    lines.Clear();

                    foreach (KeyValuePair<int, Tuple<string, int>> pair in ports)
                    {
                        lines.Add(pair.Key + ":" + pair.Value.Item1 + ":" + pair.Value.Item2);
                    }
                }
            });
        }
        
        private void BeginCoordinator()
        {
            DirectoryInfo dInfo = new DirectoryInfo(CoordinatorDirectory);
            if (!dInfo.Exists)
            {
                dInfo.Create();
                IO.AllowEveryoneAllAccess(AphysoftDataDirectory);
            }

            FileInfo pfi = new FileInfo(CoordinatorPortsFile);

            while (!pfi.Exists)
            {
                try
                {   
                    FileStream fs = pfi.Create();
                    fs.Close();

                    IO.AllowEveryoneAllAccess(CoordinatorPortsFile);
                }
                catch
                {
                }

                pfi.Refresh();
            }

            Thread coordinatorThread = new Thread(new ThreadStart(delegate ()
            {
                Thread.Sleep(100);

                coordinatorListener = new TcpListener(IPAddress.Any, NodeCoordinatorPort);

                while (coordinator)
                {
                    try
                    {
                        if (Tcp.IsPortListenAvailable(NodeCoordinatorPort))
                        {
                            coordinatorListener.Start();

                            Event("Activating Node Coordinator");

                            while (coordinator)
                            {
                                try
                                {
                                    TcpClient client = coordinatorListener.AcceptTcpClient();

                                    //Debug("Coordinator client incoming...");

                                    if (client != null)
                                    {
                                        if (client.Client.RemoteEndPoint is IPEndPoint ep)
                                        {
                                            IPAddress epa = ep.Address;

                                            // TODO safelisting address

                                            NetworkStream stream = client.GetStream();
                                            byte[] buffer = new byte[client.ReceiveBufferSize];
                                            int read = stream.Read(buffer, 0, client.ReceiveBufferSize);
                                            string request = Encoding.ASCII.GetString(buffer, 0, read);

                                            string heads = Encoding.ASCII.GetString(HandshakeHead) + "SERV";

                                            if (request.StartsWith(heads) && request.Length > heads.Length)
                                            {
                                                string service = request.Substring(heads.Length);

                                                //Debug("Requested: " + service);

                                                int port = 0;
                                                foreach (KeyValuePair<int, Tuple<string, int>> pair in GetCoordinatorPorts())
                                                {
                                                    if (pair.Value.Item1 == service)
                                                    {
                                                        port = pair.Key;
                                                        break;
                                                    }
                                                }

                                                if (port > 0)
                                                {
                                                    Debug($"Coordinator client requested: Service {service} Port {port}");

                                                    string response = port.ToString();
                                                    byte[] data = Encoding.ASCII.GetBytes(response);
                                                    stream.Write(data, 0, data.Length);

                                                    //Debug("Sent");
                                                }
                                                else
                                                {
                                                    //Debug("Cant find port information");
                                                }
                                            }

                                            client.Close();
                                        }

                                        client.Close();
                                    }
                                    else
                                        break;
                                }
                                catch (Exception ex)
                                {
                                    Debug(ex);
                                    break;
                                }
                            }

                            coordinatorListener.Stop();
                        }
                        else
                        {
                            Thread.Sleep(10);
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug(ex);
                    }
                }
            }));
            coordinatorThread.Start();
        }

        private void EndCoordinator()
        {
            Debug("Ending coodinator");

            coordinator = false;

            if (coordinatorListener != null)
                coordinatorListener.Stop();
        }

        #endregion

        public void BeginAcceptEdges()
        {
            BeginAcceptEdges(IPAddress.Any);
        }

        public void BeginAcceptEdges(IPAddress bindingAddress)
        {
            if (!ready) return;

            if (!serverStarted)
            {
                serverStarted = true;

                this.bindingAddress = bindingAddress;

                serverThread = new Thread(new ThreadStart(delegate ()
                {
                    while (true)
                    {
                        int selectedPort = ReservePort("EDGE");

                        if (selectedPort == 0)
                        {
                            Event("EDGE: No TCP port are available.");

                            Thread.Sleep(60000);
                            continue;
                        }

                        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                        Event("Starting accept edges: " + (bindingAddress == IPAddress.Any ? "Any Interface" : bindingAddress.ToString()) + ":" + selectedPort);

                        try
                        {
                            serverSocket.Bind(new IPEndPoint(bindingAddress, selectedPort));
                            Event("Binding interface: " + serverSocket.LocalEndPoint.ToString());
                            serverSocket.Listen(100);

                            while (serverStarted)
                            {
                                edgeWaitSignal.Reset();

                                Debug("Waiting for new edge...");

                                serverSocket.BeginAccept(new AsyncCallback(delegate (IAsyncResult ar)
                                {
                                    if (!serverStarted) return;
                                    if (serverSocket == null) return;

                                    edgeWaitSignal.Set();
                                    Debug("Edge incoming...");

                                    Socket socket = ((Socket)ar.AsyncState).EndAccept(ar);
                                    socket.NoDelay = true;

                                    Edge edge = new Edge(this, socket);
                                    edges.Add(edge);

                                }), serverSocket);

                                edgeWaitSignal.WaitOne();
                            }


                        }
                        catch (Exception ex)
                        {
                            Debug(ex);

                            serverSocket.Close();
                            serverSocket = null;

                            ReleasePort(selectedPort);
                        }

                        Thread.Sleep(1000);

                        if (serverStarted == false)
                        {
                            Debug("Ending accept socket...");

                            if (serverSocket != null)
                            {
                                serverSocket.Close();
                                serverSocket = null;
                            }

                            ReleasePort(selectedPort);
                            break;
                        }
                    }
                }));

                serverThread.Start();
            }
        }

        public void EndAcceptEdges()
        {
            if (serverStarted)
            {
                serverStarted = false;

                edgeWaitSignal.Set();

                while (serverSocket != null)
                {
                    Thread.Sleep(100);
                }

                Debug("Accept socket has ended");
            }
        }

        public Edge BeginEdge(string host, string service)
        {
            try
            {
                Debug("Resolving " + host);
                IPAddress[] ads = Dns.GetHostAddresses(host);

                if (ads.Length > 0)
                {
                    Debug("Resolved: " + ads[0].ToString());
                    return BeginEdge(ads[0], service);
                }
            }
            catch (Exception ex)
            {
                if (ex is SocketException)
                {
                    Debug(host + " unresolved");
                }

                return null;
            }
            return null;

        }

        public Edge BeginEdge(IPAddress remoteAddress, string service)
        {
            if (!ready) return null;

            bool exists = false;
            foreach (Edge edge in edges)
            {
                if (edge.RemoteAddress == remoteAddress)
                {
                    exists = true;
                    break;
                }
            }

            if (!exists)
            {
                Edge edge = new Edge(this, remoteAddress, service);
                edges.Add(edge);

                return edge;
            }
            else
                return null;
        }

        internal void RemoveEdge(Edge edge)
        {
            if (!ready) return;

            lock (edges)
            {
                if (edges.Contains(edge))
                {
                    edges.Remove(edge);
                }
            }
        }

        internal void Connected(EdgeConnectedEventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(delegate ()
            {
                OnConnected(e);
            }));
            thread.Start();
        }

        internal void Disconnected(EdgeDisconnectedEventArgs e, bool wasConnected)
        {
            Thread thread = new Thread(new ThreadStart(delegate ()
            {
                OnDisconnected(e, wasConnected);
            }));
            thread.Start();
        }

        internal void Message(EdgeMessageEventArgs e)
        {
            Thread thread = new Thread(new ThreadStart(delegate ()
            {
                if (e.Message is InfoInstallMessage iim)
                {
                    Event("Checking for Update...");

                    installing = new List<InstallationFile>();

                    bool addToInstall = true;

                    // delete directory _UPDATE if exists
                    DirectoryInfo updateDir = new DirectoryInfo(Path.Combine(Directory, "_UPDATE"));
                    if (updateDir.Exists) updateDir.Delete(true);

                    // check apps
                    if (Apps.Active)
                    {
                        InstallationFile appsFile = iim.Apps;

                        if (Environment.UserInteractive)
                        {
                            // delete previous apps.exe_UPDATE if exists
                            FileInfo updateApps = new FileInfo(Path.Combine(Directory, "apps.exe_UPDATE"));
                            if (updateApps.Exists) updateApps.Delete();

                            FileInfo appsFI = new FileInfo(Path.Combine(Directory, "apps.exe"));

                            if (appsFI.Exists)
                            {
                                if (appsFI.Length == appsFile.Size)
                                {
                                    if (IO.Hash(appsFI.FullName) == appsFile.Hash)
                                    {
                                        addToInstall = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            // check whether apps update, and update inprogress
                            Tuple<FileInfo, DateTime> appsUpdate = GetLatestUpdateAppsService();
                            
                            if (appsUpdate != null)
                            {
                                if (appsUpdate.Item2 == appsFile.TimeStamp)
                                {
                                    // the latest is already there, update pending
                                    addToInstall = false;
                                }
                            }
                            else
                            {
                                string appsFileName = Path.Combine(Apps.ServiceDirectory, "apps.exe");
                                if (File.Exists(appsFileName))
                                {
                                    FileInfo appsFI = new FileInfo(appsFileName);

                                    if (appsFI.Length == appsFile.Size)
                                    {
                                        if (IO.Hash(appsFI.FullName) == appsFile.Hash)
                                        {
                                            // the latest is already there, installed
                                            addToInstall = false;
                                        }
                                    }
                                }
                            }
                        }

                        if (addToInstall)
                        {
                            installing.Add(appsFile);
                        }
                    }

                    // check installations
                    foreach (InstallationFile file in iim.Files)
                    {
                        string fullName = Path.Combine(Directory, file.Name);

                        addToInstall = true;

                        FileInfo fi = new FileInfo(fullName);

                        if (fi.Exists)
                        {
                            long size = fi.Length;

                            if (size == file.Size)
                            {
                                if (IO.Hash(fullName) == file.Hash)
                                {
                                    addToInstall = false;
                                }
                            }
                        }

                        if (addToInstall)
                        {
                            installing.Add(file);
                        }

                    }

                    ReportInstallMessage rm = new ReportInstallMessage();

                    if (installing.Count > 0)
                    {
                        List<int> fileIDs = new List<int>();
                        foreach (InstallationFile file in installing) fileIDs.Add(file.ID);
                        rm.RequestID = fileIDs.ToArray();
                    }
                    
                    Event("Replying update information");

                    e.Reply(rm);

                    if (rm.RequestID != null)
                    {
                        // need update
                        if (Apps.Active)
                        {
                            // will update if an Apps
                            OnStartUpdate();

                            while (installing.Count > 0)
                            {
                                Thread.Sleep(1000);
                            }

                            if (OnUpdated())
                            {
                                Restart();
                            }
                        }
                    }

                    installing = null;
                }
                else if (e.Message is EventMessage em)
                {
                    Event($"EVENTMESSAGE|{e.Edge.Name}|{e.Edge.RemotePort}|{em.Data}");
                }
                else
                {
                    OnMessage(e);
                }                    
            }));
            thread.Start();
        }

        private Tuple<FileInfo, DateTime> GetLatestUpdateAppsService()
        {
            List<Tuple<FileInfo, DateTime>> appsUpdates = new List<Tuple<FileInfo, DateTime>>();

            foreach (FileInfo fid in (new DirectoryInfo(Apps.ServiceDirectory)).GetFiles())
            {
                string name = fid.Name;

                if (name.StartsWith("apps.exe_"))
                {
                    string[] parts = name.Split(new char[] { '_' });

                    if (parts.Length == 3 && parts[2] == "UPDATE")
                    {
                        string bin = parts[1];
                        if (long.TryParse(bin, out long binl))
                        {
                            DateTime dt = DateTime.FromBinary(binl);
                            appsUpdates.Add(new Tuple<FileInfo, DateTime>(fid, dt));
                        }
                    }
                }
            }

            if (appsUpdates.Count > 0)
            {
                // there are pending updates, check the latest one
                appsUpdates.Sort((x, y) => y.Item2.CompareTo(x.Item2));

                return appsUpdates[0];
            }
            else
                return null;
        }

        internal void FileReceived(EdgeFileEventArgs e, string filePath)
        {            
            Thread thread = new Thread(new ThreadStart(delegate ()
            {
                InstallationFile installFile = null;

                if (installing != null)
                {
                    foreach (InstallationFile file in installing)
                    {
                        if (file.ID == e.Reference)
                        {
                            installFile = file;
                            break;
                        }
                    }
                }

                if (installFile != null)
                {
                    lock (installing)
                    {
                        if (installFile.Name == "apps.exe")
                        {
                            if (Environment.UserInteractive)
                            {
                                File.Move(filePath, Path.Combine(Directory, installFile.Name) + "_UPDATE");
                            }
                            else
                            {
                                string targetLoc = Path.Combine(Apps.ServiceDirectory, installFile.Name) + "_" + installFile.TimeStamp.ToBinary() + "_UPDATE";

                                if (!File.Exists(targetLoc))
                                {
                                    File.Move(filePath, targetLoc);
                                }
                            }
                        }
                        else
                        {
                            if (Environment.UserInteractive)
                            {
                                string localPath = Path.Combine(Directory, installFile.Name);

                                if (File.Exists(localPath))
                                    File.Delete(localPath);

                                System.IO.Directory.CreateDirectory(new FileInfo(localPath).Directory.FullName);
                                File.Move(filePath, localPath);
                            }
                            else
                            {
                                string localPath = Path.Combine(Directory, "_UPDATE", installFile.Name);

                                if (File.Exists(localPath))
                                    File.Delete(localPath);

                                System.IO.Directory.CreateDirectory(new FileInfo(localPath).Directory.FullName);
                                File.Move(filePath, localPath);
                            }
                        }                        

                        installing.Remove(installFile);
                    }
                }
                else
                {
                    e.Stream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None);
                    OnFileReceived(e);
                    e.Stream.Close();

                    File.Delete(filePath);
                }

            }));
            thread.Start();
        }

        public void Start()
        {
            if (Type == NodeType.Web)
            {
                Apps.LoadConfig(Path.Combine(HttpRuntime.AppDomainAppPath, "bin\\"));

                Start(HttpRuntime.AppDomainAppPath);
            }
            else
            {
                Start(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
            }
        }

        public void Start(string directory)
        {
            if (instanceThread != null)
            {
                Debug("Start: Already started");

                return;
            }

#if DEBUG
            if (DoEvent)
            {
                Event("DEBUG BUILD");
            }
            else
            {
                Apps.Console("DEBUG BUILD");
            }
#endif

            instanceThread = new Thread(new ThreadStart(delegate ()
            {
                IsRunning = true;
                               
                Culture.Default();

                Directory = directory;

                Thread terminalListener = null, serviceKeeper = null;

                if (Type == NodeType.Web)
                {
                }
                else
                {
                    if (Apps.Active)
                    {
                        if (Apps.IsConsoleAvailable)
                        {
                            terminalListener = new Thread(new ThreadStart(delegate ()
                            {
                                AppDomain domain = AppDomain.CurrentDomain;

                                while (IsRunning)
                                {
                                    string command = null;
                                    while ((command = (string)domain.GetData("terminalCommand")) == null)
                                    {
                                        if (!IsRunning) break;
                                        else Thread.Sleep(10);
                                    }

                                    domain.SetData("terminalCommand", null);

                                    if (IsRunning)
                                    {
                                        if (command.ToLower() == "exit")
                                        {
                                            Stop();
                                            break;
                                        }
                                        else
                                        {
                                            Event($"COMMAND|{command}");
                                            OnTerminalCommand(command);
                                        }
                                    }
                                }
                            }));
                            terminalListener.Start();
                        }
                        if (!Environment.UserInteractive)
                        {
                            serviceKeeper = new Thread(new ThreadStart(delegate ()
                            {
                                ServiceController aps = null;

                                while (IsRunning)
                                {
                                    if (aps == null) aps = Apps.GetAppsService();
                                    else aps.Refresh();

                                    if (aps != null)
                                    {
                                        if (aps.Status == ServiceControllerStatus.Stopped)
                                        {
                                            aps.Start();
                                            aps.WaitForStatus(ServiceControllerStatus.Running);
                                        }
                                    }

                                    Thread.Sleep(5000);
                                }
                            }));
                            serviceKeeper.Start();
                        }
                    }

                    BeginCoordinator();

                    BeginServiceEvent();
                }         

                Event(Name + " started");

                ready = true;

                OnStart(); // ngeblok disini

                if (Type == NodeType.Web)
                {
                }
                else
                {
                    terminalCancel.Cancel();
                }

                IsRunning = false;

                // EndAcceptEdges (if started)
                EndAcceptEdges();

                // abort all edges
                Debug("Ending all edges...");

                List<Edge> toEnds = new List<Edge>(edges);

                foreach (Edge edge in toEnds) { edge.End(); }

                // wait until all edges are closed
                while (true)
                {
                    bool allDone = true;
                    
                    foreach (Edge edge in edges) { if (edge.IsActive) allDone = false; }

                    if (allDone) break;

                    Thread.Sleep(10);
                }

                Debug("All edges has been disconnected");

                ready = false;

                OnStop();

                if (Type == NodeType.Web)
                {

                }
                else
                {
                    EndServiceEvent();

                    EndCoordinator();

                    terminalListener?.Join();
                }

                Event("End of instance thread");
            }));
            instanceThread.Start();
            
            if (Type == NodeType.Web)
            {
            }
            else if (Environment.UserInteractive)
            {
                instanceThread.Join();
            }
        }
                
        public void Stop()
        {
            if (!IsRunning)
            {
                Debug("Stop: Already stopped");
                return;
            }

            IsRunning = false;

            // for windows service, wait here
            //if (!Environment.UserInteractive)
            //{
                instanceThread.Join();
            //}
        }

        public void Restart()
        {
            if (Apps.Active)
            {
                if (!restarting)
                {
                    restarting = true;
                    
                    if (Environment.UserInteractive)
                    {
                        if (File.Exists(Path.Combine(Directory, "apps.exe_UPDATE")))
                            Apps.SetExit(ExitType.RestartHost);
                        else
                            Apps.SetExit(ExitType.Restart);

                        Stop();
                    }
                    else
                    {
                        FileInfo fupdating = new FileInfo(Path.Combine(Apps.ServiceDirectory, "updating"));

                        // there's still updating, so block the process
                        if (fupdating.Exists)
                        {
                            while (fupdating.Exists)
                            {
                                Thread.Sleep(1000);
                                fupdating.Refresh();
                            }
                        }

                        Tuple<FileInfo, DateTime> appUpdate = GetLatestUpdateAppsService();

                        if (appUpdate != null)
                        {
                            // theres update, lets update

                            // create updating
                            File.Create(Path.Combine(Apps.ServiceDirectory, "updating")).Close();

                            ServiceController aps = Apps.GetAppsService();
                            
                            // stop running apps
                            if (aps.Status == ServiceControllerStatus.Running)
                            {
                                aps.Stop();
                                aps.WaitForStatus(ServiceControllerStatus.Stopped);
                            }

                            // update apps
                            File.Move(appUpdate.Item1.FullName, Path.Combine(Apps.ServiceDirectory, "apps.exe"));

                            // start apps
                            aps.Start();
                            aps.WaitForStatus(ServiceControllerStatus.Running);

                            // delete other obsoleted _UPDATE
                            foreach (FileInfo fi in (new DirectoryInfo(Apps.ServiceDirectory)).GetFiles())
                            {
                                if (fi.Name.EndsWith("_UPDATE"))
                                {
                                    fi.Delete();
                                }
                            }
                        }

                        // DirectoryInfo dis = new DirectoryInfo(Apps.ServiceDirectory);


                    }
                }
            }
        }

        public void StandBy()
        {
            while (IsRunning)
            {
                Thread.Sleep(1000);
            }
        }

        #region Output
        
        private void BeginServiceEvent()
        {
            if (DoEvent)
            {
                byte[] eventHead = new byte[8];

                Buffer.BlockCopy(HandshakeHead, 0, eventHead, 0, 4);
                Buffer.BlockCopy(ServiceEventHead, 0, eventHead, 4, 4);

                serviceEventClients = new Dictionary<TcpClient, Tuple<byte[], byte[]>>();
                serviceEventThread = new Thread(new ThreadStart(delegate ()
                {
                    int selectedPort = 0;

                    while (IsRunning)
                    {
                        serviceEventListener = null;

                        selectedPort = ReservePort("EVENT");

                        if (selectedPort == 0)
                        {
                            Debug("EVENT: No TCP port are available.");

                            Thread.Sleep(60000);
                            continue;
                        }

                        if (!IsRunning) break;

                    //Debug
                    serviceEventListener = new TcpListener(IPAddress.Any, selectedPort);

                        try
                        {
                            serviceEventListener.Start();

                            while (true)
                            {
                                try
                                {
                                    TcpClient client = serviceEventListener.AcceptTcpClient();

                                    if (client != null)
                                    {
                                        bool ok = false;

                                    // *  encrypted with client public key
                                    // ** encrypted with AES

                                    // CLIENT:  AFISEVNT<RSA client exponent length><RSA client exponent><RSA client modulus length><RSA client modulus>
                                    // SERVER: [<AES 16 bytes IV><AES 32 bytes key>]*
                                    // CLIENT: [<32 bytes IDENTITY>]**
                                    // then SERVER began transmit events 

                                    if (client.Client.RemoteEndPoint is IPEndPoint ep)
                                        {
                                            NetworkStream stream = client.GetStream();
                                            stream.ReadTimeout = 2000;

                                            try
                                            {
                                                byte[] buffer = new byte[client.ReceiveBufferSize];
                                                byte[] head = new byte[8];
                                                byte[] send = null;

                                                int index, read, explength, modlength;

                                                if ((read = stream.Read(buffer, 0, buffer.Length)) > 8)
                                                {
                                                    Buffer.BlockCopy(buffer, 0, head, 0, 8);
                                                    index = 8;

                                                    if (head.SequenceEqual(eventHead) && read > 17)
                                                    {
                                                        explength = BitConverter.ToInt32(buffer, index);
                                                        index += 4;

                                                        if (explength > 0 && explength < 2048)
                                                        {
                                                            byte[] clientExponent = new byte[explength];
                                                            Buffer.BlockCopy(buffer, index, clientExponent, 0, explength);
                                                            index += explength;

                                                            modlength = BitConverter.ToInt32(buffer, index);
                                                            index += 4;

                                                            if (modlength > 0 && modlength < 2048)
                                                            {
                                                                byte[] clientModulus = new byte[modlength];
                                                                Buffer.BlockCopy(buffer, index, clientModulus, 0, modlength);

                                                                RSACryptoServiceProvider clientCrypto = new RSACryptoServiceProvider(2048);
                                                                clientCrypto.ImportParameters(new RSAParameters
                                                                {
                                                                    Exponent = clientExponent,
                                                                    Modulus = clientModulus
                                                                });

                                                                send = new byte[16 + 32];

                                                                byte[] iv = new byte[16];
                                                                byte[] key = new byte[32];

                                                                iv.Random();
                                                                key.Random();

                                                                Buffer.BlockCopy(iv, 0, send, 0, 16);
                                                                Buffer.BlockCopy(key, 0, send, 16, 32);

                                                                send = clientCrypto.Encrypt(send, false);
                                                                stream.Write(send, 0, send.Length);

                                                                if ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                                                                {
                                                                // check identity
                                                                byte[] aesdata = new byte[read];
                                                                    Buffer.BlockCopy(buffer, 0, aesdata, 0, read);

                                                                    byte[] data = Aes.Decrypt(aesdata, iv, key);

                                                                    if (data.Length == 32)
                                                                    {
                                                                        int pprocid = BitConverter.ToInt32(data, 0);


                                                                        try
                                                                        {
                                                                            Process process = Process.GetProcessById(pprocid);
                                                                            if (process.ProcessName == Process.GetCurrentProcess().ProcessName)
                                                                            {
                                                                            // thats right
                                                                            ok = true;
                                                                            }
                                                                        }
                                                                        catch (Exception ex) { }

                                                                        if (!ok)
                                                                        {
                                                                        // unique identity
                                                                        string identst = Apps.Config("EVENT_IDENTITY");

                                                                            if (identst != null)
                                                                            {
                                                                                string[] idents = identst.Split(new char[] { '/' });

                                                                                foreach (string ident in idents)
                                                                                {
                                                                                    string[] hexas = ident.Split(new char[] { '-' });
                                                                                    if (hexas.Length == 32)
                                                                                    {
                                                                                        byte[] thisbyte = new byte[32];
                                                                                        int i = 0;
                                                                                        foreach (string hexa in hexas)
                                                                                        {
                                                                                            try
                                                                                            {
                                                                                                thisbyte[i] = Convert.ToByte(hexa.ToLower(), 16);
                                                                                            }
                                                                                            catch (Exception ex)
                                                                                            {
                                                                                                break;
                                                                                            }
                                                                                            i++;
                                                                                        }

                                                                                        if (i == 32)
                                                                                        {
                                                                                            if (thisbyte.SequenceEqual(data))
                                                                                            {
                                                                                                ok = true;
                                                                                            }
                                                                                        }
                                                                                    }
                                                                                }
                                                                            }
                                                                        }

                                                                        if (ok)
                                                                        {
                                                                            Debug("Service event client has started (" + ep + ")");

                                                                            send = new byte[] { 0 };
                                                                            stream.Write(send, 0, send.Length);

                                                                            Thread.Sleep(1000);
                                                                            serviceEventClients.Add(client, new Tuple<byte[], byte[]>(iv, key));


                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            catch (Exception ex)
                                            {
                                            }
                                        }

                                        if (!ok)
                                            client.Close();
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    break;
                                }
                            }

                            break;
                        }
                        catch (Exception ex)
                        {
                        }

                        ReleasePort(selectedPort);
                        selectedPort = 0;

                        serviceEventSignal.Set();
                    }

                    if (selectedPort != 0)
                        ReleasePort(selectedPort);

                }));
                serviceEventThread.Start();
            }
        }

        private void EndServiceEvent()
        {
            if (DoEvent)
            {
                if (serviceEventThread != null)
                {
                    if (serviceEventListener != null)
                    {
                        Debug("Ending service event");

                        serviceEventListener.Stop();

                        foreach (KeyValuePair<TcpClient, Tuple<byte[], byte[]>> pair in serviceEventClients)
                        {
                            TcpClient client = pair.Key;
                            client.Close();
                        }

                        serviceEventClients = null;
                    }

                    serviceEventThread.Join();
                }
            }
        }
        
        private void ProcessEvent(string message)
        {
            if (message == null) return;

            if (message == lastEventMessage) lastEventRepeat++;
            else
            {
                lastEventMessage = message;
                lastEventRepeat = 0;
            }

            string finalMessage = $"{DateTime.UtcNow.ToString("yyyy/MM/dd:HH:mm:ss.fff")}|{lastEventRepeat}|{message}";

            if (DoEvent)
            {
                if (serviceEventClients != null)
                {
                    List<TcpClient> end = new List<TcpClient>();

                    lock (serviceEventClients)
                    {
                        foreach (KeyValuePair<TcpClient, Tuple<byte[], byte[]>> pair in serviceEventClients)
                        {
                            TcpClient client = pair.Key;

                            try
                            {
                                if (!client.Connected)
                                {
                                    end.Add(client);
                                    continue;
                                }

                                NetworkStream nwStream = client.GetStream();

                                string data = finalMessage;

                                byte[] raw = Encoding.ASCII.GetBytes(data);
                                byte[] enc = Aes.Encrypt(raw, pair.Value.Item1, pair.Value.Item2);

                                byte[] buffer = new byte[enc.Length + 4];

                                byte[] lenBytes = BitConverter.GetBytes(enc.Length);

                                Buffer.BlockCopy(lenBytes, 0, buffer, 0, 4);
                                Buffer.BlockCopy(enc, 0, buffer, 4, enc.Length);

                                nwStream.Write(buffer, 0, buffer.Length);
                            }
                            catch (Exception ex)
                            {
                                end.Add(client);
                            }
                        }
                    }

                    foreach (TcpClient endx in end)
                    {
                        if (serviceEventClients.ContainsKey(endx))
                        {
                            IPEndPoint epx = (IPEndPoint)endx.Client.RemoteEndPoint;
                            serviceEventClients.Remove(endx);
                            endx.Close();
                        }
                    }
                }

                OnEvent(finalMessage, true, lastEventRepeat);
            }
        }

        public void Event(string message)
        {
            Event(message, null);
        }

        public void Event(string message, string label)
        {
            Event(message, label, null);
        }

        public void Event(string message, string label, string subLabel)
        {
            if (label == null)
                ProcessEvent("||" + message);
            else
            {
                if (subLabel == null)
                    ProcessEvent("|" + label + "|" + message);
                else
                    ProcessEvent("|" + label + ">" + subLabel + "|" + message);
            }
        }

        public void NoDebug()
        {
            DoDebug = false;
        }

        public void Debug(string message)
        {
#if DEBUG
            if (DoDebug)
            {
                Event("DEBUG|" + message);
            }
#endif
        }

        public void Debug(Exception ex)
        {
            Debug(ex.Message);
            Debug(ex.StackTrace);
        }

        #endregion

        #endregion
    }
}
