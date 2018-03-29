using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    public abstract class Node
    {
        #region Const

        internal static readonly int Port = 2347;

        internal static readonly int BufferSize = 4096;

        internal static readonly byte[] HandshakeHead = new byte[] { (byte)'A', (byte)'F', (byte)'I', (byte)'S' };

        #endregion

        #region Fields

        private string name = null;

        public string Name { get => name; }

        private bool running = false;

        public bool IsRunning { get => running; }

        //private static Timer connecting;

        protected Thread instanceThread = null;

        private Thread consoleThread = null;

        public bool IsConsole { get => (consoleThread != null); }        

        private ConsoleInput consoleInput = null;

        private List<Edge> edges = new List<Edge>();

        private IPAddress bindingAddress = null;

        private Socket serverSocket = null;

        private bool serverStarted = false;

        private Thread serverThread = null;

        private ManualResetEvent edgeWaiter = new ManualResetEvent(false);

        private Thread updaterThread = null;

        #endregion

        public Node(string name)
        {
            this.name = name;
        }

        #region Virtual

        protected virtual void OnStart() { }

        protected virtual void OnStop() { }

        protected virtual void OnEvent(string message) { }
        
        protected virtual void OnUpdating() { }

        protected virtual void OnUpdated() { }

        protected virtual void OnConnected(Edge edge) { }

        protected virtual void OnDisconnected(Edge edge) { }

        #endregion

        #region Methods

        public void BeginAcceptEdges()
        {
            BeginAcceptEdges(IPAddress.Any);
        }

        public void BeginAcceptEdges(IPAddress bindingAddress)
        {
            if (!serverStarted)
            {
                serverStarted = true;

                this.bindingAddress = bindingAddress;

                serverThread = new Thread(new ThreadStart(delegate ()
                {
                    while (true)
                    {
                        serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                        Event("Starting accept edges: " + (bindingAddress == IPAddress.Any ? "Any Interface" : bindingAddress.ToString()) + ":" + Port);

                        try
                        {
                            serverSocket.Bind(new IPEndPoint(bindingAddress, Port));
                            Event("Binding interface: " + serverSocket.LocalEndPoint.ToString());
                            serverSocket.Listen(100);

                            while (serverStarted)
                            {
                                edgeWaiter.Reset();

                                Debug("Waiting for new edge...");

                                serverSocket.BeginAccept(new AsyncCallback(delegate(IAsyncResult ar)
                                {
                                    edgeWaiter.Set();
                                    Debug("Edge incoming...");                                    

                                    Socket socket = ((Socket)ar.AsyncState).EndAccept(ar);
                                    socket.NoDelay = true;

                                    Edge edge = new Edge(this, socket);
                                    edges.Add(edge);

                                }), serverSocket);

                                edgeWaiter.WaitOne();
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug(ex);
                            serverSocket.Close();
                            serverSocket = null;
                        }

                        Thread.Sleep(1000);

                        if (serverStarted == false)
                            break;
                    }
                }));

                serverThread.Start();
            }
        }

        public Edge BeginEdge(IPAddress remoteAddress)
        {
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
                Edge edge = new Edge(this, remoteAddress);
                edges.Add(edge);

                return edge;
            }
            else
                return null;
        }
        
        internal void RemoveEdge(Edge edge)
        {
            lock (edges)
            {
                if (edges.Contains(edge))
                {
                    edges.Remove(edge);
                }
            }
        }

        protected void EndConnect()
        {
            //connecting.Dispose();
        }

        public void Start()
        {
            Start(false);
        }

        public void Start(bool console)
        {
            instanceThread = new Thread(new ThreadStart(delegate ()
            {
                running = true;
                Culture.Default();
                OnStart();                
            }));
            instanceThread.Start();

            updaterThread = new Thread(new ThreadStart(delegate ()
            {
            }));
            updaterThread.Start();

            if (console)
            {
                consoleThread = new Thread(new ThreadStart(delegate ()
                {
                    while (true)
                    {
                        string line = Console.ReadLine();
                        if (!instanceThread.IsAlive) break;
                        string trimLower = line.Trim().ToLower();

                        if (trimLower == "exit")
                        {
                            running = false;
                            break;
                        }
                    }
                }));
                consoleThread.Start();
            }

            instanceThread.Join();

            // abort all edges
            Debug("Ending all edges...");
            foreach (Edge edge in edges)
            {
                edge.Abort();
            }

            // wait until all edges are closed
            while (true)
            {
                bool allDone = true;

                foreach (Edge edge in edges)
                {
                    if (edge.IsActive) allDone = false;
                }

                if (allDone) break;
                Thread.Sleep(10);
            }
            Debug("All edges has been disconnected");

            OnStop();

            if (console)
            {                   
                if (consoleThread.IsAlive)
                    consoleThread.Join();

                Console.WriteLine("Application terminated, it is safe to close the console window now.");
                Console.WriteLine("Press any key to close");
                Console.ReadKey();
            }
        }

        public void Stop()
        {
            running = false;
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
                OnEvent(DateTime.UtcNow.ToString("HH:mm:ss.fff") + "||" + message);
            else
            {
                if (subLabel == null)
                    OnEvent(DateTime.UtcNow.ToString("HH:mm:ss.fff") + "|" + label + "|" + message);
                else
                    OnEvent(DateTime.UtcNow.ToString("HH:mm:ss.fff") + "|" + label + ">" + subLabel + "|" + message);
            }
        }

        public void Debug(string message)
        {
#if DEBUG
            Event("DEBUG|" + message);
#endif
        }

        public void Debug(Exception ex)
        {
            Debug(ex.Message);
            Debug(ex.StackTrace);
        }
        
        public int ConsoleWait(string[] options, int timeout)
        {
            int ret = -1;
            int c = 0;
            int secincounter = timeout * 100;

            while (true)
            {
                if (consoleInput != null)
                {
                    int ci = -1;
                    foreach (string option in options)
                    {
                        ci++;
                        if (consoleInput.IsCommand(option))
                        {
                            ret = ci;
                            break;
                        }
                    }

                    if (ret > -1)
                    {
                        break;
                    }

                }
                Thread.Sleep(10);
                c++;

                if (c >= secincounter) break;
            }

            return ret;
        }

        #endregion
    }
}
