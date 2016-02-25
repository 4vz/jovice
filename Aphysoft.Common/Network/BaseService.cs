using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

namespace Aphysoft.Common
{
    public enum ServiceOutputTypes : int
    {
        Debug = 0,
        Default = 1,
        None = int.MaxValue
    }

    public enum ServiceRoles
    {
        Client, 
        Server
    }

    public abstract class BaseService
    {
        #region Const

        internal static readonly int HeaderSize = 16;
        internal static readonly byte[] HelloPacket = new byte[16] { (byte)'H', (byte)'L', (byte)'L', (byte)'O', 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
        internal static readonly byte[] MessageHeadPacket = new byte[4] { (byte)'M', (byte)'E', (byte)'S', (byte)'G' };
        
        #endregion

        #region Fields

        private int outputLevel = Int16.MaxValue;

        private ManualResetEvent allDone = new ManualResetEvent(false);

        private Socket serverSocket = null;

        private Socket clientSocket = null;

        private IPEndPoint endPoint = null;

        private Thread connectionThread = null;

        private Connection clientConnection = null;

        private bool isConnecting = false;

        private bool reconnectAfterDisconnection = true;

        private int reconnectDelay = 5000;

        private int bufferSize = 256;

        private bool isClient = false;

        private int clientCounter = 0;

        private Dictionary<int, Connection> connections = new Dictionary<int, Connection>();

        private object connectionsWaitSync = new object();

        private Dictionary<Type, OnReceivedCallback> onReceivedCallbacks = new Dictionary<Type, OnReceivedCallback>();

        private Dictionary<int, Thread> receivingThreads = new Dictionary<int, Thread>();

        private object receivingThreadsWaitSync = new object();

        private Dictionary<int, BaseServiceMessage> responses = new Dictionary<int, BaseServiceMessage>();

        private object responsesWaitSync = new object();

        private object waitWaitSync = new object();

        private List<int> abortedResponses = new List<int>();

        private object abortedResponsesWaitSync = new object();
        
        #endregion

        #region Properties
        
        /// <summary>
        /// Gets the number of current active connections.
        /// </summary>
        public int ActiveConnectionsCount
        {
            get 
            {
                if (IsServer)
                    return connections.Count;
                else if (IsClient)
                    return IsConnected ? 1 : 0;
                else
                    return 0;
            }
        }

        /// <summary>
        /// Gets the number of current active receiving threads.
        /// </summary>
        public int ActiveReceivingThreadsCount
        {
            get { return receivingThreads.Count; }
        }

        /// <summary>
        /// Returns a value indicating whether the current instance is a server or not.
        /// </summary>
        public bool IsServer
        {
            get { if (serverSocket != null) return true; else return false; }
        }

        /// <summary>
        /// Returns a value indicating whether the current instance is a client instance or not.
        /// </summary>
        public bool IsClient
        {
            get { return isClient; }
        }

        /// <summary>
        /// Gets current buffer size for incoming socket message.
        /// </summary>
        public int BufferSize
        {
            get { return bufferSize; }
        }

        /// <summary>
        /// Returns a value indicating whether the current instance is still connecting or not.
        /// </summary>
        public bool IsConnecting
        {
            get { return isConnecting; }
        }

        /// <summary>
        /// Returns a value indicating whether the current instance is connected to remote peer.
        /// </summary>
        public bool IsConnected
        {
            get
            {
                if (IsClient)
                {
                    if (clientSocket == null) return false;
                    else return clientSocket.Connected;
                }
                else return false;
            }
        }

        #endregion
        
        #region Constructors

        public BaseService()
        {

        }

        #endregion

        #region Overrides

        public virtual void OnConnected(ConnectionEventArgs e) { }
        public virtual void OnReceived(MessageEventArgs e) { }
        public virtual void OnDisconnected(ConnectionEventArgs e) { }
        public virtual void OnOutput(string output) { }

        #endregion

        #region Methods
        
        public void OutputType(ServiceOutputTypes type)
        {
            outputLevel = (int)type;
        }

        internal void Output(ServiceOutputTypes type, string output)
        {
            if ((int)type >= outputLevel)
                OnOutput(output);
        }

        internal void Debug(string output)
        {
            Output(ServiceOutputTypes.Debug, output);
        }

        /// <summary>
        /// Set current instance as server instance.
        /// </summary>
        /// <param name="bindingInterface"></param>
        /// <param name="bindingPort"></param>
        public void Server(IPAddress bindingInterface, int bindingPort)
        {
            if (serverSocket == null && isClient == false)
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

                Debug("Starting server");

                Debug("Prefered Binding Interface: " +
                    (bindingInterface == IPAddress.Any ? "Any Interface" : bindingInterface.ToString()) +
                    ":" +
                    (bindingPort == 0 ? "Any Port" : bindingPort.ToString()));

                IPEndPoint bindEndPoint = new IPEndPoint(bindingInterface, bindingPort);

                bool bindOk = false;
                try
                {
                    serverSocket.Bind(bindEndPoint);

                    EndPoint ep = serverSocket.LocalEndPoint;
                    Debug("Binding interface: " + serverSocket.LocalEndPoint.ToString());

                    endPoint = bindEndPoint;

                    bindOk = true;
                }
                catch (Exception ex)
                {
                    Debug("Binding failure: " + ex.Message);
                    
                    serverSocket.Close();
                    serverSocket = null;
                }

                if (bindOk)
                {
                    serverSocket.Listen(100);

                    connectionThread = new Thread(new ThreadStart(Accept));
                    connectionThread.Start();
                }
            }
        }

        /// <summary>
        /// Set current instance as client instance.
        /// </summary>
        /// <param name="serverAddress"></param>
        public void Client(IPEndPoint serverAddress)
        {
            if (serverSocket == null && isClient == false)
            {
                isClient = true;

                Debug("Starting client");

                SetServerAddress(serverAddress);

                Debug("Server to " + endPoint.ToString());

                Connect();
            }
        }

        /// <summary>
        /// Client: Set server address.
        /// </summary>
        /// <param name="serverAddress"></param>
        public void SetServerAddress(IPEndPoint serverAddress)
        {
            if (!IsClient)
                return;

            if (IsConnecting || IsConnected)
                return;

            endPoint = serverAddress;
        }
 
        /// <summary>
        /// Client: Connect to server.
        /// </summary>
        public void Connect()
        {
            if (!IsClient || IsConnecting || IsConnected)
                return;

            try
            {
                isConnecting = true;
                Debug("Connecting...");

                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.BeginConnect(endPoint, new AsyncCallback(EndConnect), this);                
            }
            catch (Exception ex)
            {
                // connection failure
                Debug("Connect failed: " + ex.Message);

                clientSocket.Close();
                clientSocket = null;

                isConnecting = false;
            }
        }

        private void EndConnect(IAsyncResult ar)
        {
            bool connectionOk = false;

            try
            {                
                clientSocket.EndConnect(ar);
                isConnecting = false;

                Debug("Connected to " + clientSocket.RemoteEndPoint.ToString());

                connectionOk = true;
            }
            catch (Exception ex)
            {
                Debug("EndConnect failed: " + ex.Message);

                clientSocket.Close();
                clientSocket = null;

                isConnecting = false;
            }

            if (connectionOk)
            {
                clientConnection = new Connection(this, clientSocket);
                clientConnection.Start();

                clientConnection.Send(HelloPacket);
            }
            else
            {
                Reconnect();
            }
        }

        /// <summary>
        /// Client: Disconnect current connection to server.
        /// </summary>
        public void Disconnect()
        {
            if (!IsClient)
                return;

            Debug("Disconnecting...");

            if (IsConnected) // aps (atas permintaan sendiri)
            {
                clientSocket.BeginDisconnect(false, new AsyncCallback(EndDisconnect), this);
            }
            else
            {
                Debug("Not currently connected");

                if (IsConnecting)
                {
                    Debug("Cancel connecting...");

                    clientSocket.Close();

                    isConnecting = false;
                }
                else
                {
                    // disconnected non-aps 
                    ConnectionDisconnected(null);

                    clientSocket = null;

                    // reconnect?
                    Reconnect();
                }
            }
        }

        private void EndDisconnect(IAsyncResult ar)
        {
            clientSocket.EndDisconnect(ar);

            Debug("Disconnected");

            clientSocket.Close();
            clientSocket = null;

            isConnecting = false;

            ConnectionDisconnected(null);
        }

        private void Reconnect()
        {
            if (reconnectAfterDisconnection)
            {
                connectionThread = new Thread(new ThreadStart(EndReconnect));
                connectionThread.Start();
            }
        }

        private void EndReconnect()
        {
            Debug("Preparing to reconnect");

            Thread.Sleep(reconnectDelay);

            Connect();
        }

        private void Accept()
        {
            while (true) // wait for incoming connection
            {
                allDone.Reset();

                // waiting for connection....
                Debug("Waiting for new client connection...");

                serverSocket.BeginAccept(new AsyncCallback(EndAccept), serverSocket);
                
                allDone.WaitOne();
            }
        }

        private void EndAccept(IAsyncResult ar)
        {            
            // Signal the main thread to continue.
            allDone.Set();

            // Get the socket that handles the client request.
            Socket socket = ((Socket)ar.AsyncState).EndAccept(ar);
            socket.NoDelay = true;

            Connection connection = new Connection(this, socket);            

            lock (connectionsWaitSync)
            {
                connection.connectionID = clientCounter++;

                Debug("New client connected: " + socket.RemoteEndPoint.ToString() + " connection ID: " + connection.connectionID);

                connections.Add(connection.connectionID, connection);                
            }

            connection.Start();

            ThreadPool.QueueUserWorkItem(new WaitCallback(VerifyConnection), connection);
        }

        private void VerifyConnection(object state)
        {
            Thread.Sleep(5000);

            Connection connection = (Connection)state;

            if (connection.verified == false)
            {
                Debug("Cant verify the client, disconnecting...");

                connection.Disconnect();
            }
        }

        internal void ConnectionDisconnected(Connection connection)
        {
            if (IsServer)
            {
                Debug("Client disconnected: " + connection.socket.RemoteEndPoint.ToString());

                lock (connectionsWaitSync)
                {
                    connections.Remove(connection.ConnectionID);
                }

                OnDisconnected(new ConnectionEventArgs(connection));
            }
            else
            {
                Debug("Disconnected from server");

                OnDisconnected(new ConnectionEventArgs(connection));
            }
        }

        internal void ConnectionMessageReceived(Connection connection, int messageID, int messageReplyID, BaseServiceMessage message)
        {
            Type type = message.GetType();

            if (IsServer)
            {  
                if (type == typeof(EventServiceMessage))
                {
                    EventServiceMessage eventServiceMessage = (EventServiceMessage)message;

                    DateTime timeStamp = eventServiceMessage.Timestamp;

                    string eventTimeStamp = string.Format("{0,2}:{1,2}:{2,2}:{3,3}", timeStamp.Hour, timeStamp.Minute, timeStamp.Second, timeStamp.Millisecond);
                    string connectionInfo = string.Format("{0, 5}", connection.ConnectionID);

                    Output(ServiceOutputTypes.Default, "EVENT [" + eventTimeStamp + "] [" + connectionInfo + "] " + eventServiceMessage.Message);
                }
                else
                {
                    Debug("Message Received Type = " + type.Name);

                    if (type == typeof(CancelServiceMessage))
                    {
                        CancelServiceMessage csm = (CancelServiceMessage)message;

                        lock (receivingThreadsWaitSync)
                        {
                            Debug("Force remove receiving thread " + messageReplyID);
                            int threadID = messageReplyID;

                            if (receivingThreads.ContainsKey(threadID))
                            {
                                Thread tn = receivingThreads[threadID];
                                tn.Abort();
                                receivingThreads.Remove(threadID);
                            }
                            else
                            {
                                Debug("Thread ID " + threadID + " not found");
                            }
                        }
                    }
                    else
                    {
                        OnReceivedCallback callback = null;

                        foreach (KeyValuePair<Type, OnReceivedCallback> pair in onReceivedCallbacks)
                        {
                            Type ptype = pair.Key;
                            OnReceivedCallback pcallback = pair.Value;

                            if (type == ptype || type.IsSubclassOf(ptype))
                            {
                                callback = pcallback;
                                break;
                            }
                        }

                        Thread thread = new Thread(new ParameterizedThreadStart(OnReceivedThread));
                        ReceivedThreadObject rto = new ReceivedThreadObject(connection, callback, messageID, messageReplyID, message);

                        lock (receivingThreadsWaitSync)
                        {
                            Debug("Add receiving thread " + messageID);
                            receivingThreads.Add(messageID, thread);
                            thread.Start(rto);
                        }
                    }
                }
                
            }
            else if (IsClient)
            {
                if (false)
                {
                }
                else
                {
                    OnReceivedCallback callback = null;

                    foreach (KeyValuePair<Type, OnReceivedCallback> pair in onReceivedCallbacks)
                    {
                        Type ptype = pair.Key;
                        OnReceivedCallback pcallback = pair.Value;

                        if (type == ptype || type.IsSubclassOf(ptype))
                        {
                            callback = pcallback;
                            break; 
                        }
                    }

                    Thread thread = new Thread(new ParameterizedThreadStart(OnReceivedThread));
                    ReceivedThreadObject rto = new ReceivedThreadObject(connection, callback, messageID, messageReplyID, message);

                    lock (receivingThreadsWaitSync)
                    {
                        Debug("Add receiving thread " + messageID);
                        receivingThreads.Add(messageID, thread);
                        thread.Start(rto);
                    }
                }
            }
        }

        public T Wait<T>(T originalMessage) where T : BaseServiceMessage, new()
        {
            return Wait<T>(originalMessage, null, null);
        }

        public T Wait<T>(T originalMessage, MessageWaitCallback callback, object callbackObject) where T : BaseServiceMessage, new()
        {
            if (!IsClient)
                throw new Exception("Wait is Client function");

            int messageID = originalMessage.messageID;

            if (messageID != -1)
            {
                Debug("Now waiting message: " + messageID);

                bool responded = false;
                do
                {
                    if (IsConnected == false) break;

                    if (responses.ContainsKey(messageID))
                    {
                        responded = true;
                        break;
                    }

                    lock (waitWaitSync)
                    {
                        Debug("wait, waiter: " + messageID);
                        Monitor.Wait(waitWaitSync, 5000);
                    }

                    // if optional condition for break
                    if (callback != null)
                    {
                        bool isBreak = callback(callbackObject);
                        if (isBreak) break;
                    }
                    
                }
                while (responded == false);

                T responseServiceMessage = null;

                if (responded)
                {
                    Debug("Message responded");

                    lock (responsesWaitSync)
                    {
                        responseServiceMessage = responses[messageID] as T;
                        responses.Remove(messageID);
                    }

                    originalMessage.messageID = responseServiceMessage.messageID;
                }
                else
                {
                    Debug("Message wait cancel");

                    // break tapi responded masih false
                    // cancel thread di server
                    responseServiceMessage = new T();
                    responseServiceMessage.IsAborted = true;

                    if (IsConnected == false)
                    {
                        Debug("Server disconnected while waiting");
                    }
                    else
                    {
                        Debug("Callback breaks");
                        // note myself if incoming response messageID, to be removed immediately
                        lock (abortedResponsesWaitSync)
                        {
                            Debug("Add to-be ignored response by messageID: " + messageID);
                            abortedResponses.Add(messageID);
                            Thread art = new Thread(new ParameterizedThreadStart(RemoveAbortedResponseCallback));
                            art.Start(messageID);
                        }
                        // note server to abort message waiting callback, thread id is messageID???
                        CancelServiceMessage csm = new CancelServiceMessage();
                        clientConnection.Send(csm, messageID);
                    }
                }

                return responseServiceMessage;
            }
            else
                throw new Exception("cannot wait for a reply of unsend message");
        }

        private void RemoveAbortedResponseCallback(object obj)
        {
            int removeThisID = (int)obj;

            Thread.Sleep(5000);

            Debug("Remove to-be aborted response ID: " + removeThisID);

            if (abortedResponses.Contains(removeThisID))
            {
                lock (abortedResponses)
                {
                    if (abortedResponses.Contains(removeThisID))
                    {
                        Debug("Removed");
                        abortedResponses.Remove(removeThisID);
                    }
                }
            }
        }

        public void Stop()
        {
            if (IsServer)
            {
                if (connectionThread != null)
                {
                    allDone.Reset();
                    connectionThread.Abort();
                }

                serverSocket = null;
            }
            else if (IsClient)
            {

            }
        }

        /// <summary>
        /// Client: Send specified message to connected server.
        /// </summary>
        public bool Send(BaseServiceMessage message)
        {
            if (IsClient)
            {
                if (IsConnected)
                {
                    clientConnection.Send(message);
                    return true;
                }
            }

            return false;
        }

        public Connection GetConnection(int connectionID)
        {
            if (connections.ContainsKey(connectionID))
                return connections[connectionID];
            else
                return null;
        }

        /// <summary>
        /// Client: Output message to connected server OnOutput.
        /// </summary>
        public void EventMessage(string message)
        {
            if (IsClient)
            {
                EventServiceMessage sm = new EventServiceMessage(message);
                Send(sm);
            }
            else if (IsServer)
            {
                Output(ServiceOutputTypes.Default, message);
            }
        }

        /// <summary>
        /// Register message handler.
        /// </summary>
        public void Register(Type messageType, OnReceivedCallback receivedCallback)
        {
            lock (onReceivedCallbacks)
            {
                if (!onReceivedCallbacks.ContainsKey(messageType))
                {                    
                    onReceivedCallbacks.Add(messageType, receivedCallback);
                }
            }
        }

        private void OnReceivedThread(object obj)
        {
            ReceivedThreadObject rto = (ReceivedThreadObject)obj;
            int messageID = rto.MessageID;
            int messageReplyID = rto.MessageReplyID;
            
            // pasang message ID ke message
            rto.Message.messageID = messageID;

            MessageEventArgs eva = new MessageEventArgs(rto.Connection, messageID, messageReplyID, rto.Message);

            if (rto.Callback != null)
                rto.Callback(eva);

            if (eva.IsResponse)
            {
                Debug("A response for message ID = " + messageReplyID);

                if (abortedResponses.Contains(messageReplyID))
                {
                    Debug("Ignore that because contain in aborted response, remove the id");
                    // ignore this response
                    lock (abortedResponses)
                    {
                        Debug("Removed");
                        abortedResponses.Remove(messageReplyID);
                    }
                }
                else
                {
                    Debug("Ok Got it, pulse the waiter");
                    lock (waitWaitSync)
                    {
                        lock (responsesWaitSync)
                        {
                            responses.Add(messageReplyID, rto.Message);
                        }

                        Debug("Pulsing: " + messageReplyID);
                        Monitor.PulseAll(waitWaitSync);
                    }
                }
            }

            lock (receivingThreadsWaitSync)
            {
                Debug("Remove receiving thread " + messageID);
                receivingThreads.Remove(messageID);
            }
        }

        #endregion
    }

    public sealed class Connection
    {
        #region Fields

        internal Socket socket = null;

        private BaseService service = null;

        private byte[] buffer = null;
        private byte[] header = null;
        private byte[] message = null;

        private int headerSeek = 0;        
        private int messageID = -1;
        private int messageReplyID = -1;
        private int messageLength = 0;
        private int messageSeek = 0;

        internal bool verified = false;

        private bool disconnected = false;

        internal int connectionID = 0;
       
        #endregion

        #region Properties

        public int ConnectionID
        {
            get { return connectionID; }
        }

        public BaseService Service
        {
            get { return service; }
        }

        public bool IsConnected
        {
            get { return !disconnected; }
        }

        #endregion

        #region Constructors

        internal Connection(BaseService service, Socket socket)
        {
            this.service = service;
            this.socket = socket;

            buffer = new byte[service.BufferSize];
            header = new byte[BaseService.HeaderSize];
        }

        #endregion

        #region Methods

        internal void Start()
        {
            Receive();
        }

        private void Output(ServiceOutputTypes outputType, string output)
        {
            service.Output(outputType, output);
        }

        private void Debug(string output)
        {
            service.Debug(output);
        }

        private void Receive()
        {
            Debug("Begin Receiving...");

            socket.BeginReceive(buffer, 0, service.BufferSize, SocketFlags.None, new AsyncCallback(EndReceive), this);
        }

        public static bool ByteArrayCompare(byte[] a, byte[] b)
        {
            if (a.Length != b.Length) return false;
            int len = a.Length;
            unsafe
            {
                fixed (byte* ap = a, bp = b)
                {
                    long* alp = (long*)ap, blp = (long*)bp;
                    for (; len >= 8; len -= 8)
                    {
                        if (*alp != *blp) return false;
                        alp++;
                        blp++;
                    }
                    byte* ap2 = (byte*)alp, bp2 = (byte*)blp;
                    for (; len > 0; len--)
                    {
                        if (*ap2 != *bp2) return false;
                        ap2++;
                        bp2++;
                    }
                }
            }
            return true;
        }

        private void EndReceive(IAsyncResult ar)
        {
            Debug("Received");

            SocketError error = SocketError.Success;

            int bytesRead = -1;

            try
            {
                bytesRead = socket.EndReceive(ar, out error);
            }
            catch (Exception ex)
            {
                Debug("Receive failed: " + ex.Message);
                bytesRead = -1;
            }


            if (error == SocketError.Success && bytesRead > 0)
            {
                Debug("Incoming " + bytesRead + " bytes");

                // <command 4 bytes>
                
                // MESSAGE
                // MESG<message length 4 bytes><message id 4 bytes><reply to message id 4 bytes>
                // reply to hanya untuk server send, client receive

                int bufferSeek = 0;
                int bufferBytesLeft = bytesRead;

                do
                {
                    if (bufferBytesLeft > 0)
                    {
                        if (headerSeek < BaseService.HeaderSize)  // waiting for header
                        {
                            Debug("Colleting header bytes");

                            int headerCopyCount = (BaseService.HeaderSize - headerSeek) < bufferBytesLeft ? (BaseService.HeaderSize - headerSeek) : bufferBytesLeft;

                            Buffer.BlockCopy(buffer, bufferSeek, header, headerSeek, headerCopyCount);

                            headerSeek += headerCopyCount;
                            bufferBytesLeft -= headerCopyCount;
                            bufferSeek += headerCopyCount;

                            if (headerSeek == BaseService.HeaderSize) // start checking header
                            {
                                // parse header
                                byte[] headerHead = new byte[4];
                                Buffer.BlockCopy(header, 0, headerHead, 0, 4);

                                if (ByteArrayCompare(headerHead, BaseService.MessageHeadPacket))
                                {
                                    // go on
                                    messageLength = BitConverter.ToInt32(header, 4);
                                    messageID = BitConverter.ToInt32(header, 8);
                                    messageReplyID = BitConverter.ToInt32(header, 12);
                                    

                                    Debug("MESG");
                                    Debug("Message Length: " + messageLength);
                                    Debug("Message ID: " + messageID);
                                    Debug("Message Reply ID: " + messageReplyID);

                                    messageSeek = 0;
                                    message = new byte[messageLength];
                                }
                                else if (service.IsServer)
                                {
                                    if (verified == false && ByteArrayCompare(header, BaseService.HelloPacket))
                                    {
                                        verified = true;
                                        Debug("Client verified");
                                        Send(BaseService.HelloPacket); // Reply to client
                                        service.OnConnected(new ConnectionEventArgs(this));
                                    }

                                    headerSeek = 0;
                                }
                                else if (service.IsClient)
                                {
                                    if (verified == false && ByteArrayCompare(header, BaseService.HelloPacket))
                                    {
                                        verified = true;
                                        Debug("Server verified");
                                        service.OnConnected(new ConnectionEventArgs(this));
                                    }

                                    headerSeek = 0;
                                }
                                //else if (headerStr.StartsWith("DISC"))
                                //{
                                //    Output("DISC");
                                //    bufferSeek = -1;
                                //    Disconnect();
                                //    break;
                                //}
                            }

                            continue;
                        }

                        if (headerSeek == BaseService.HeaderSize) // waiting for message
                        {
                            Debug("Colleting message bytes");

                            int messageCopyCount = (messageLength - messageSeek) < bufferBytesLeft ? (messageLength - messageSeek) : bufferBytesLeft;

                            Buffer.BlockCopy(buffer, bufferSeek, message, messageSeek, messageCopyCount);

                            messageSeek += messageCopyCount;
                            bufferBytesLeft -= messageCopyCount;
                            bufferSeek += messageCopyCount;

                            if (messageSeek == messageLength)
                            {
                                Debug("Message finished");

                                service.ConnectionMessageReceived(this, messageID, messageReplyID, BaseServiceMessage.Deserialize(message));

                                headerSeek = 0;
                            }
                        }
                    }
                }
                while (bufferBytesLeft > 0);

                if (bufferSeek != -1)
                    Receive();
            }
            else if (bytesRead == 0)
            {
                Debug("Zero-length bytes: " + error);

                if (error == SocketError.Success)
                {
                    if (socket.Poll(100, SelectMode.SelectRead))
                    {
                        Disconnect();
                    }
                }
                else
                    Disconnect();
            }
            else
            {

            }
        }

        internal void Send(byte[] data)
        {
            try
            {
                socket.BeginSend(data, 0, data.Length, 0, new AsyncCallback(EndSend), this);
            }
            catch (Exception ex)
            {
                Disconnect();
            }
        }

        private void EndSend(IAsyncResult ar)
        {
            SocketError error;
            try
            {
                int bytesSent = socket.EndSend(ar, out error);

                Debug(error + " Sent " + bytesSent + " bytes");
            }
            catch (Exception ex)
            {
                Debug("EndSend failed: " + ex.Message);
            }
        }

        public void Disconnect()
        {
            if (disconnected)
                return;
            else
                disconnected = true;

            if (service.IsServer)
            {
                if (socket.Connected)
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.BeginDisconnect(true, new AsyncCallback(EndDisconnect), this);
                }
                else
                    service.ConnectionDisconnected(this);
            }
            else if (service.IsClient)
            {
                service.Disconnect();
            }
        }

        private void EndDisconnect(IAsyncResult ar)
        {
            socket.EndDisconnect(ar);
            service.ConnectionDisconnected(this);
            socket.Close();            
        }

        public void Send(BaseServiceMessage message)
        {
            Send(message, -1);
        }

        public void Reply(BaseServiceMessage message)
        {
            Send(message, message.messageID);
        }

        internal void Send(BaseServiceMessage message, int messageReplyID)
        {
            byte[] bytes = message.Serialize();

            if (bytes != null)
            {
                int messageLength = bytes.Length;

                int messageID = RandomHelper.Next(); // TODO use helper 0 <= x <= int.Max

                message.messageID = messageID;

                Debug("Sending message, message ID = " + messageID + ", messageReplyID = " + messageReplyID);

                byte[] buffer = new byte[messageLength + BaseService.HeaderSize]; // header, message

                Buffer.BlockCopy(BaseService.MessageHeadPacket, 0, buffer, 0, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(messageLength), 0, buffer, 4, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(messageID), 0, buffer, 8, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(messageReplyID), 0, buffer, 12, 4);
                Buffer.BlockCopy(bytes, 0, buffer, 16, messageLength);

                Send(buffer);
            }
            else
            {
                Debug("Not sending: serialization failed");
            }
        }

        #endregion
    }
    
    public delegate void OnReceivedCallback(MessageEventArgs e);

    public delegate bool MessageWaitCallback(object o);

    internal class ReceivedThreadObject
    {
        #region Fields

        private int messageID;
        private int messageReplyID;
        private OnReceivedCallback callback;
        private Connection connection;
        private BaseServiceMessage message;

        #endregion

        #region Properties

        public int MessageID
        {
            get { return messageID; }
        }

        public int MessageReplyID
        {
            get { return messageReplyID; }
        }

        public OnReceivedCallback Callback
        {
            get { return callback; }
        }

        public Connection Connection
        {
            get { return connection; }
        }

        public BaseServiceMessage Message
        {
            get { return message; }
        }

        #endregion

        #region Constructors

        public ReceivedThreadObject(Connection connection, OnReceivedCallback callback, int messageID, int messageReplyID, BaseServiceMessage message)
        {
            this.messageID = messageID;
            this.message = message;
            this.callback = callback;
            this.connection = connection;
            this.messageReplyID = messageReplyID;
        }

        #endregion
    }

    public class ConnectionEventArgs : EventArgs
    {
        #region Fields

        private Connection connection = null;

        public Connection Connection
        {
            get { return connection; }
        }

        public BaseService Service
        {
            get { return connection.Service; }
        }

        #endregion

        #region Constructors

        public ConnectionEventArgs(Connection connection)
        {
            this.connection = connection;
        }

        #endregion
    }

    public class MessageEventArgs : ConnectionEventArgs
    {
        #region Fields

        private BaseServiceMessage message = null;  
        private int messageID;
        private int messageReplyID;

        #endregion

        #region Properties

        public BaseServiceMessage Message
        {
            get { return message; }
        }

        public bool IsResponse
        {
            get { if (messageReplyID != -1) return true; else return false; }
        }

        #endregion

        #region Constructors

        public MessageEventArgs(Connection connection, int messageID, int messageReplyID, BaseServiceMessage message) : base(connection)
        {
            this.messageID = messageID;
            this.message = message;
            this.messageReplyID = messageReplyID;
        }

        #endregion
    }

    [Serializable]
    public abstract class BaseServiceMessage
    {
        #region Fields

        [NonSerialized]
        internal int messageID = -1;

        [NonSerialized]
        private bool isAborted = false;

        /// <summary>
        /// You require to check this properties if wait call is expected to take a long time.
        /// </summary>
        public bool IsAborted
        {
            get { return isAborted; }
            internal set { isAborted = value; }
        }

        #endregion

        #region Constructors

        public BaseServiceMessage()
        {

        }

        #endregion

        #region Serialize Deserialize

        internal byte[] Serialize()
        {
            byte[] bytes = null;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter bin = new BinaryFormatter();
                try
                {
                    bin.Serialize(memoryStream, this);
                    bytes = memoryStream.ToArray();
                }
                catch (Exception ex)
                {
                }
            }

            return bytes;
        }

        internal static BaseServiceMessage Deserialize(byte[] buffer)
        {
            BaseServiceMessage obj = null;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                memoryStream.Write(buffer, 0, buffer.Length);
                memoryStream.Position = 0;

                BinaryFormatter bin = new BinaryFormatter();

                try
                {
                    obj = (BaseServiceMessage)bin.Deserialize(memoryStream);
                }
                catch (Exception ex)
                {
                }
            }

            return obj;
        }

        #endregion
    }

    [Serializable]
    internal class EventServiceMessage : BaseServiceMessage
    {
        #region Fields

        private string message;

        public string Message
        {
            get { return message; }
        }

        private DateTime timestamp;

        public DateTime Timestamp
        {
            get { return timestamp; }
        }

        #endregion

        #region Constructors

        public EventServiceMessage(string message)
        {
            this.message = message;
            timestamp = DateTime.Now;
        }

        #endregion
    }
    
    [Serializable]
    internal class CancelServiceMessage : BaseServiceMessage
    {
        #region Constructors

        public CancelServiceMessage()
        {
        }

        #endregion
    }
}
