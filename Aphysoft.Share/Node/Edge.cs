using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    public enum EdgeTypes
    {
        Client,
        Server
    }

    public sealed class Edge
    {
        #region Fields

        private Node node;

        private Thread clientConnectionThread = null;

        private bool clientRunning = true;

        public EdgeTypes EdgeType { get; }

        public Socket Socket { get; private set; } = null;

        public IPEndPoint RemoteEndPoint { get => Socket != null ? (IPEndPoint)Socket.RemoteEndPoint : null; }

        public IPAddress RemoteAddress => RemoteEndPoint?.Address;

        public int RemotePort => RemoteEndPoint != null ? RemoteEndPoint.Port : 0;

        private bool connectingDisconnecting = false;

        private bool connectionEstablishing = false;

        public bool IsConnected { get; private set; } = false;

        public bool IsActive { get => (Socket != null || connectionEstablishing || receiving); }

        public bool IsApps { get; private set; } = false;

        private ManualResetEvent clientConnectionSignal = new ManualResetEvent(false);

        private ManualResetEvent receivingSignal = new ManualResetEvent(false);

        private Thread receivingThread = null;

        private bool maintainConnection = false;

        private Thread maintainConnectionThread = null;

        private DateTime lastPing = DateTime.MinValue;

        private bool pongRetrieved = false;

        private bool receiving = false;

        private int handshakingStep = 0;

        public string Name { get; private set; } = null;

        private RSACryptoServiceProvider localCrypto = null, remoteCrypto = null;

        private object incomingResponseSync = new object();

        private Dictionary<int, BaseMessage> responses = new Dictionary<int, BaseMessage>();

        private Dictionary<int, string> receivedFiles = new Dictionary<int, string>();

        private Dictionary<int, int> receivedFilesReferences = new Dictionary<int, int>();

        #endregion

        #region Delegates

        public delegate bool EdgeResponseWaitBreakCallback(object o);

        public delegate void EdgeSendProgressCallback(EdgeProgessEventArgs e);

        #endregion

        #region Constructors

        private Edge(Node node, EdgeTypes type)
        {
            this.node = node;
            EdgeType = type;

            //debug = false;
        }

        internal Edge(Node node, IPAddress remoteAddress, string service) : this(node, EdgeTypes.Server)
        {
            clientConnectionThread = new Thread(new ThreadStart(delegate ()
            {
                Debug("Starting client connection thread");

                while (clientRunning)
                {
                    clientConnectionSignal.Reset();

                    if (!connectionEstablishing && !connectingDisconnecting)
                    {
                        // query port
                        int servicePort = 0;

                        TcpClient coordinatorClient = new TcpClient();

                        try
                        {
                            Debug($"Retriving service port for {service}...");

                            coordinatorClient.Connect(remoteAddress, Node.NodeCoordinatorPort);

                            NetworkStream stream = coordinatorClient.GetStream();

                            stream.Write(Node.HandshakeHead, 0, Node.HandshakeHead.Length);
                            byte[] send = Encoding.ASCII.GetBytes("SERV" + service + "-EDGE");
                            stream.Write(send, 0, send.Length);

                            byte[] buffer = new byte[coordinatorClient.ReceiveBufferSize];
                            int read = stream.Read(buffer, 0, coordinatorClient.ReceiveBufferSize);

                            if (read > 0)
                            {
                                string resp = Encoding.ASCII.GetString(buffer, 0, read);

                                if (int.TryParse(resp, out int rport))
                                {
                                    Debug("Service port: " + rport);

                                    servicePort = rport;
                                }
                            }

                            stream.Close();
                        }
                        catch (Exception ex)
                        {
                        }

                        coordinatorClient.Close();

                        if (servicePort > 0)
                        {
                            try
                            {
                                handshakingStep = 0;

                                Debug("Connecting... (" + remoteAddress + ":" + servicePort + ")");

                                connectingDisconnecting = true;

                                Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                                Socket.BeginConnect(new IPEndPoint(remoteAddress, servicePort), new AsyncCallback(delegate (IAsyncResult ar)
                                {
                                    connectingDisconnecting = false;

                                    try
                                    {
                                        Socket.EndConnect(ar);

                                        Debug("Connected");

                                        if (clientRunning == false)
                                        {
                                            // whenever Edge ended, but after connected
                                            Debug("Edge is not running, disconnecting...");
                                        }
                                        else
                                        {
                                            connectionEstablishing = true;

                                            Receiving();

                                            Thread.Sleep(100);

                                            Send("HLLO", Encoding.ASCII.GetBytes(node.Name));
                                        }


                                    }
                                    catch (Exception ex)
                                    {
                                        Debug(ex);

                                        if (Socket != null)
                                        {
                                            Socket.Close();
                                            Socket = null;
                                        }
                                    }

                                }), this);
                            }
                            catch (Exception ex)
                            {
                                Debug(ex);

                                if (Socket != null)
                                {
                                    Socket.Close();
                                    Socket = null;
                                }

                                connectingDisconnecting = false;
                            }
                        }
                        else
                        {
                            Debug("Failed to get the service port, reconnecting in 5 seconds...");
                            Thread.Sleep(5000);
                            continue;
                        }
                    }
                    else
                    {
                        if (connectionEstablishing && IsConnected)
                        {
                            // ping, if still alive?
                            lastPing = DateTime.UtcNow;
                            pongRetrieved = false;

                            Send("PING");

                            while (true)
                            {
                                TimeSpan delta = DateTime.UtcNow - lastPing;

                                if (pongRetrieved)
                                {
                                    Debug("Pong retrieved");
                                    break;
                                }
                                else if (delta.TotalSeconds > 15)
                                {
                                    Debug("Server is lost");
                                    EndConnection();
                                    break;
                                }
                                Thread.Sleep(10);
                            }
                        }
                    }

                    clientConnectionSignal.WaitOne(30000);
                }

                Debug("Client connection thread ended");
            }));

            clientConnectionThread.Start();
        }

        internal Edge(Node node, Socket socket) : this(node, EdgeTypes.Client)
        {
            Socket = socket;

            connectionEstablishing = true;

            Receiving();
        }

        #endregion

        #region Debug

        private void Debug(string message)
        {
            node.Debug(message);
        }

        private void Debug(Exception ex)
        {
            node.Debug(ex);
        }

        #endregion

        #region Methods

        internal void End()
        {
            Debug("Ending edge");

            if (EdgeType == EdgeTypes.Server)
            {
                clientRunning = false;
            }

            EndConnection();
        }

        internal void EndConnection()
        {
            node.RemoveEdge(this);

            if (connectionEstablishing)
            {
                Debug("Disconnecting...");

                bool wasConnected = IsConnected;

                connectionEstablishing = false;
                IsConnected = false;
                maintainConnection = false;
                IsApps = false;

                if (localCrypto != null)
                {
                    localCrypto.Dispose();
                    localCrypto = null;
                }
                if (remoteCrypto != null)
                {
                    remoteCrypto.Dispose();
                    remoteCrypto = null;
                }

                connectingDisconnecting = true;

                Socket.Shutdown(SocketShutdown.Both);
                Socket.BeginDisconnect(false, new AsyncCallback(delegate (IAsyncResult ar)
                {
                    try
                    {
                        Socket.EndDisconnect(ar);

                        Debug("Disconnected");

                        node.Disconnected(new EdgeDisconnectedEventArgs(this), wasConnected);

                        connectingDisconnecting = false;

                        if (Socket != null)
                        {
                            Socket.Close();
                            Socket = null;
                        }

                        if (EdgeType == EdgeTypes.Server)
                        {
                            // if connection to server disconnected, only end socket but let the client to try to connect again if still running
                            clientConnectionSignal.Set();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug(ex);
                    }
                }), this);
            }
        }

        private string CheckType(byte[] checkType)
        {
            string parsedHead = Encoding.ASCII.GetString(checkType);

            if (parsedHead.IndexOf("HLLO", "OKOK", "ENCY", "WELC", "THNK", "PING", "PONG", "MESG") > -1) return parsedHead;
            else return null;
        }

        private void Receiving()
        {
            receivingThread = new Thread(new ThreadStart(delegate ()
            {
                Debug("Starting receiving thread");

                receiving = true;

                byte[] fourBytes = new byte[4];
                byte[] message = null;
                byte[] encryptedData = null;
                byte[] iv = null, key = null;

                if (localCrypto != null)
                {
                    localCrypto.Dispose();
                    localCrypto = null;
                }
                if (remoteCrypto != null)
                {
                    remoteCrypto.Dispose();
                    remoteCrypto = null;
                }

                string type = null;
                int packetSeek = 0;
                int headLength = 0, dataLength = 0, decryptedDataLength = 0;

                int fileStreamID = 0;
                int fileReference = 0;
                byte[] hash = null;
                string filePath = null;
                FileStream fileStream = null;

                while (receiving)
                {
                    receivingSignal.Reset();
                    try
                    {
                        if (Socket.Connected)
                        {
                            byte[] buffer = new byte[Node.BufferSize];

                            Socket.BeginReceive(buffer, 0, Node.BufferSize, SocketFlags.None, new AsyncCallback(delegate (IAsyncResult result)
                            {
                                if (Socket == null)
                                {
                                    Debug("Socket lost before EndReceive callback");
                                    receiving = false;

                                    EndConnection();

                                    receivingSignal.Set();
                                    return;
                                }

                                SocketError error = SocketError.Success;

                                int bytesRead = -1;


                                try
                                {
                                    bytesRead = Socket.EndReceive(result, out error);
                                }
                                catch (Exception ex)
                                {
                                    Debug(ex);
                                }

                                if (bytesRead > 0)
                                {
                                    Debug($"Received: {bytesRead} bytes");

                                    int bufferSeek = 0;
                                    int bytesLeft = bytesRead;

                                    do
                                    {
                                        bool packetEnd = false;

                                        int seek = 0, partLength = 0;
                                        byte[] dest = null;

                                        bool encrypted = localCrypto != null ? true : false;

                                        if (!encrypted) // NOT ENCRYPTED
                                        {
                                            if (packetSeek < 4) seek = packetSeek; // AFIS
                                            else if (packetSeek < 8) seek = packetSeek - 4; // HEAD
                                            else if (packetSeek < 12) seek = packetSeek - 8; // MESSAGE LENGTH
                                            else seek = packetSeek - 12; // DATA

                                            if (packetSeek < 12)
                                            {
                                                partLength = 4;
                                                dest = fourBytes;
                                            }
                                            else
                                            {
                                                partLength = dataLength;
                                                dest = message;
                                            }
                                        }
                                        else // ENCRYPTED
                                        {
                                            if (packetSeek < 4) // RSA-ENCRYPTED LENGTH
                                            {
                                                seek = packetSeek;
                                                partLength = 4;
                                                dest = fourBytes;
                                            }
                                            else if (packetSeek < (4 + headLength)) // RSA-ENCRYPTED-HEAD
                                            {
                                                seek = packetSeek - 4;
                                                partLength = headLength;
                                                dest = encryptedData;
                                            }
                                            else if (packetSeek < (4 + headLength + dataLength)) // AES-ENCRYPTED-DATA OR STREAM
                                            {
                                                seek = packetSeek - (4 + headLength);
                                                partLength = dataLength;
                                                dest = encryptedData;
                                            }
                                        }

                                        //0123456789012345678901234567890123456789
                                        //01234567890123456789 BUFFER 20
                                        //HHHHDDDDDDDDDDDDDDDDDDD PACKET
                                        //    01234567890123456 SEEK
                                        //                    012

                                        // SEEK = 0
                                        // PARTLENGTH = 23
                                        // BYTESLEFT = 16

                                        int length = (partLength - seek) < bytesLeft ? (partLength - seek) : bytesLeft;

                                        if (fileStreamID != 0)
                                        {
                                            dest = new byte[length];
                                            seek = 0;
                                        }

                                        Buffer.BlockCopy(buffer, bufferSeek, dest, seek, length);

                                        if (fileStreamID != 0)
                                        {
                                            fileStream.Write(dest, 0, dest.Length);
                                        }

                                        packetSeek += length;
                                        bytesLeft -= length;
                                        bufferSeek += length;

                                        if (!encrypted)
                                        {
                                            if (packetSeek == 4) // at the end of AFIS
                                            {
                                                if (!fourBytes.SequenceEqual(Node.HandshakeHead)) EndConnection();
                                            }
                                            else if (packetSeek == 8) // at the end of HEAD
                                            {
                                                type = CheckType(fourBytes);
                                                if (type == null) EndConnection();
                                            }
                                            else if (packetSeek == 12) // at the end of DATA LENGTH
                                            {
                                                dataLength = BitConverter.ToInt32(fourBytes, 0);
                                                Debug("Data Length: " + dataLength);

                                                if (dataLength == 0)
                                                    EndConnection();
                                                else
                                                    message = new byte[dataLength];
                                            }
                                            else if (packetSeek == (12 + dataLength)) // at the end of DATA
                                            {
                                                packetEnd = true;
                                                Debug("Packet is completed");
                                            }
                                        }
                                        else
                                        {
                                            if (packetSeek == 4) // at the end of RSA-ENCRYPTED LENGTH
                                            {
                                                headLength = BitConverter.ToInt32(fourBytes, 0);
                                                Debug("RSA-Encrypted-Head Length: " + headLength);

                                                if (headLength == 0)
                                                    EndConnection();
                                                else
                                                    encryptedData = new byte[headLength];
                                            }
                                            else if (packetSeek == (4 + headLength)) // at the end of RSA-ENCRYPTED-HEAD
                                            {
                                                Debug("RSA-Encrypted-Head is completed, begin decrypting...");

                                                byte[] head = localCrypto.Decrypt(encryptedData, false);

                                                // 52 bytes: info         0
                                                // 4  bytes: data length  52
                                                // 4  bytes: streamid;    56

                                                dataLength = BitConverter.ToInt32(head, 52);
                                                fileStreamID = BitConverter.ToInt32(head, 56);

                                                if (dataLength == 0)
                                                    EndConnection();
                                                else
                                                {
                                                    if (fileStreamID == 0)
                                                    {
                                                        Debug("AES-Encrypted Data");

                                                        // aes-encrypted data

                                                        // 16 bytes: iv                    0
                                                        // 32 bytes: key                   16
                                                        // 4  bytes: original data length  48

                                                        iv = Bytes.BlockCopy(head, 0, 16);
                                                        key = Bytes.BlockCopy(head, 16, 32);

                                                        Debug("IV: " + BitConverter.ToString(iv));
                                                        Debug("Key: " + BitConverter.ToString(key));

                                                        decryptedDataLength = BitConverter.ToInt32(head, 48);

                                                        encryptedData = new byte[dataLength];
                                                    }
                                                    else
                                                    {
                                                        hash = Bytes.BlockCopy(head, 0, 16);
                                                        fileReference = BitConverter.ToInt32(head, 16);

                                                        Debug($"Stream Data: {fileStreamID}");
                                                        Debug($"Checksum: {BitConverter.ToString(hash)}");

                                                        // stream
                                                        filePath = Path.GetTempFileName();
                                                        fileStream = new FileStream(filePath, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
                                                    }
                                                }
                                            }
                                            else if (packetSeek == (4 + headLength + dataLength)) // at the end of DATA
                                            {
                                                Debug("Data is completed");

                                                packetEnd = true;

                                                if (fileStreamID == 0)
                                                {
                                                    Debug("Decrypting AES-Encrypted Data...");

                                                    using (AesManaged rm = new AesManaged())
                                                    {
                                                        rm.Padding = PaddingMode.PKCS7;
                                                        rm.Key = key;
                                                        rm.IV = iv;

                                                        byte[] decryptedData = null;

                                                        using (MemoryStream ms = new MemoryStream())
                                                        {
                                                            using (CryptoStream cs = new CryptoStream(ms, rm.CreateDecryptor(), CryptoStreamMode.Write))
                                                            {
                                                                cs.Write(encryptedData, 0, encryptedData.Length);
                                                            }
                                                            decryptedData = ms.ToArray();
                                                        }

                                                        // get head
                                                        Buffer.BlockCopy(decryptedData, 0, fourBytes, 0, 4);
                                                        type = CheckType(fourBytes);

                                                        if (type == null)
                                                            EndConnection();

                                                        // get message
                                                        int messageLength = decryptedData.Length - 4;

                                                        if (messageLength > 0)
                                                        {
                                                            message = new byte[messageLength];
                                                            Buffer.BlockCopy(decryptedData, 4, message, 0, messageLength);
                                                        }
                                                        else
                                                            message = null;

                                                        Debug("Message completed");
                                                    }
                                                }
                                                else
                                                {
                                                    byte[] chash = null;
                                                    using (MD5 md5 = MD5.Create())
                                                    {
                                                        fileStream.Position = 0;
                                                        chash = md5.ComputeHash(fileStream);
                                                    }

                                                    Debug($"File stream {fileStreamID} is completed.");
                                                    Debug($"Path: {filePath}");

                                                    fileStream.Close();

                                                    if (chash.SequenceEqual(hash))
                                                    {
                                                        Debug("Checksum matched");

                                                        EdgeFileEventArgs ea = new EdgeFileEventArgs(this);
                                                        ea.Reference = fileReference;
                                                        ea.Checksum = chash;

                                                        node.FileReceived(ea, filePath);
                                                    }
                                                    else
                                                    {
                                                        Debug("Checksum fail");
                                                        File.Delete(filePath);
                                                    }
                                                }
                                            }
                                        }

                                        if (packetEnd)
                                        {
                                            try
                                            {
                                                if (EdgeType == EdgeTypes.Client) // IM A SERVER, AND EDGES ARE CLIENTS
                                                {
                                                    if (handshakingStep == 0 && type == "HLLO")
                                                    {
                                                        Name = Encoding.ASCII.GetString(message);
                                                        Send("OKOK", Encoding.ASCII.GetBytes(node.Name));
                                                        handshakingStep = 1;
                                                    }
                                                    else if (handshakingStep == 1 && type == "ENCY")
                                                    {
                                                        int ioffset = 0;
                                                        byte[] exponent, modulus;

                                                        Buffer.BlockCopy(message, ioffset, fourBytes, 0, 4);
                                                        int exponentLength = BitConverter.ToInt32(fourBytes, 0);

                                                        ioffset += 4;

                                                        exponent = new byte[exponentLength];
                                                        Buffer.BlockCopy(message, ioffset, exponent, 0, exponentLength);

                                                        ioffset += exponentLength;

                                                        Buffer.BlockCopy(message, ioffset, fourBytes, 0, 4);
                                                        int modulusLength = BitConverter.ToInt32(fourBytes, 0);

                                                        ioffset += 4;

                                                        modulus = new byte[modulusLength];
                                                        Buffer.BlockCopy(message, ioffset, modulus, 0, modulusLength);

                                                        Debug("Got authentication keys!");

                                                        remoteCrypto = new RSACryptoServiceProvider(2048);
                                                        RSAParameters remotePublicKey = new RSAParameters();
                                                        remotePublicKey.Exponent = exponent;
                                                        remotePublicKey.Modulus = modulus;
                                                        remoteCrypto.ImportParameters(remotePublicKey);

                                                        Debug("Generating authentication keys...");

                                                        localCrypto = new RSACryptoServiceProvider(2048);
                                                        RSAParameters localPublicKey = localCrypto.ExportParameters(false);

                                                        exponent = localPublicKey.Exponent;
                                                        modulus = localPublicKey.Modulus;

                                                        Send("WELC", BitConverter.GetBytes(exponentLength), exponent, BitConverter.GetBytes(modulusLength), modulus, Apps.Active ? Bytes.One : Bytes.Zero);

                                                        handshakingStep = 2;
                                                    }
                                                    else if (handshakingStep == 2 && type == "THNK")
                                                    {
                                                        Debug("SERVER Handshaking completed!");

                                                        IsConnected = true;

                                                        byte apps = message[0];

                                                        this.IsApps = apps == 1;

                                                        EdgeConnectedEventArgs e = new EdgeConnectedEventArgs(this);

                                                        handshakingStep = 3;

                                                        maintainConnection = true;
                                                        maintainConnectionThread = new Thread(new ThreadStart(delegate ()
                                                        {
                                                            lastPing = DateTime.UtcNow;
                                                            while (maintainConnection)
                                                            {
                                                                TimeSpan ts = DateTime.UtcNow - lastPing;

                                                                if (ts.Seconds > 60)
                                                                {
                                                                    // exceeded,
                                                                    // disconnect 
                                                                    Debug("No ping message in last 60 seconds, disconnected");
                                                                    EndConnection();
                                                                }

                                                                Thread.Sleep(1000);
                                                            }
                                                        }));
                                                        maintainConnectionThread.Start();

                                                        node.Connected(e);
                                                    }
                                                    else if (handshakingStep == 3)
                                                    {
                                                        if (type == "PING")
                                                        {
                                                            lastPing = DateTime.UtcNow;
                                                            Send("PONG", BitConverter.GetBytes(DateTime.UtcNow.ToBinary()));
                                                        }
                                                        else if (type == "MESG")
                                                        {
                                                            MessageHandler(message);
                                                        }
                                                    }
                                                }
                                                else if (EdgeType == EdgeTypes.Server) // IM A CLIENT, AND EDGE IS SERVER
                                                {
                                                    if (handshakingStep == 0 && type == "OKOK")
                                                    {
                                                        Name = Encoding.ASCII.GetString(message);

                                                        Debug("Generating authentication keys...");

                                                        localCrypto = new RSACryptoServiceProvider(2048);
                                                        RSAParameters localPublicKey = localCrypto.ExportParameters(false);

                                                        byte[] exponent = localPublicKey.Exponent;
                                                        byte[] modulus = localPublicKey.Modulus;

                                                        int exponentLength = exponent.Length;
                                                        int modulusLength = modulus.Length;

                                                        Send("ENCY", BitConverter.GetBytes(exponentLength), exponent, BitConverter.GetBytes(modulusLength), modulus);

                                                        handshakingStep = 1;
                                                    }
                                                    else if (handshakingStep == 1 && type == "WELC")
                                                    {
                                                        int ioffset = 0;
                                                        byte[] exponent, modulus;

                                                        Buffer.BlockCopy(message, ioffset, fourBytes, 0, 4);
                                                        int exponentLength = BitConverter.ToInt32(fourBytes, 0);

                                                        ioffset += 4;

                                                        exponent = new byte[exponentLength];
                                                        Buffer.BlockCopy(message, ioffset, exponent, 0, exponentLength);

                                                        ioffset += exponentLength;

                                                        Buffer.BlockCopy(message, ioffset, fourBytes, 0, 4);
                                                        int modulusLength = BitConverter.ToInt32(fourBytes, 0);

                                                        ioffset += 4;

                                                        modulus = new byte[modulusLength];
                                                        Buffer.BlockCopy(message, ioffset, modulus, 0, modulusLength);

                                                        ioffset += modulusLength;

                                                        byte apps = message[ioffset];

                                                        Debug("Got authentication keys!");

                                                        remoteCrypto = new RSACryptoServiceProvider(2048);
                                                        RSAParameters remotePublicKey = new RSAParameters();
                                                        remotePublicKey.Exponent = exponent;
                                                        remotePublicKey.Modulus = modulus;
                                                        remoteCrypto.ImportParameters(remotePublicKey);

                                                        Debug("CLIENT Handshaking completed!");

                                                        IsConnected = true;

                                                        Send("THNK", Apps.Active ? Bytes.One : Bytes.Zero);

                                                        this.IsApps = apps == 1;

                                                        EdgeConnectedEventArgs e = new EdgeConnectedEventArgs(this);

                                                        handshakingStep = 3;

                                                        node.Connected(e);
                                                    }
                                                    else if (handshakingStep == 3)
                                                    {
                                                        if (type == "PONG")
                                                        {
                                                            // my ping is responded
                                                            // im okay
                                                            pongRetrieved = true;
                                                            long serverTime = BitConverter.ToInt64(message, 0);
                                                            DateTime serverDateTime = DateTime.FromBinary(serverTime);
                                                        }
                                                        else if (type == "MESG")
                                                        {
                                                            MessageHandler(message);
                                                        }
                                                    }
                                                }
                                            }
                                            catch (ArgumentOutOfRangeException ex)
                                            {
                                                Debug("Message bytes format is not correct");
                                            }

                                            packetSeek = 0;
                                            fileStreamID = 0;
                                            fileReference = 0;
                                            headLength = 0;
                                            dataLength = 0;
                                            decryptedDataLength = 0;

                                            hash = null;
                                            iv = null;
                                            key = null;
                                            filePath = null;
                                            fileStream = null;
                                            message = null;
                                            type = null;
                                        }
                                    }
                                    while (bytesLeft > 0);

                                }
                                else
                                {
                                    Debug($"SocketError: {error}");


                                    if (bytesRead == 0) Debug("Socket zero-packet signal received");
                                    else Debug("Socket exception");

                                    receiving = false;

                                    EndConnection();
                                }

                                receivingSignal.Set();

                            }), this);
                        }
                        else
                        {
                            Debug("Socket has been closed");

                            receiving = false;
                            receivingSignal.Set();

                            EndConnection();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug(ex);
                    }
                    receivingSignal.WaitOne();
                }

                Debug("Receiving thread ended");

            }));
            receivingThread.Start();
        }

        private void MessageHandler(byte[] message)
        {
            int messageID = BitConverter.ToInt32(message, 0);
            int responseMessageID = BitConverter.ToInt32(message, 4);
            int dataLength = BitConverter.ToInt32(message, 8);

            byte[] data = new byte[dataLength];
            Buffer.BlockCopy(message, 12, data, 0, dataLength);

            BaseMessage ins = BaseMessage.Deserialize(data);

            EdgeMessageEventArgs e = new EdgeMessageEventArgs(this)
            {
                MessageID = messageID,
                Message = ins
            };

            if (responseMessageID != 0)
            {
                e.ResponseMessageID = responseMessageID;

                lock (incomingResponseSync)
                {
                    lock (responses)
                    {
                        if (!responses.ContainsKey(responseMessageID))
                        {
                            responses.Add(responseMessageID, ins);
                        }
                    }

                    Monitor.PulseAll(incomingResponseSync);
                }
            }

            node.Message(e);
        }

        #region Send

        public bool Send(FileStream fileStream)
        {
            return Send(fileStream, out int id, 0, null);
        }

        public bool Send(FileStream fileStream, EdgeSendProgressCallback progressCallback)
        {
            return Send(fileStream, out int id, 0, progressCallback);
        }

        public bool Send(FileStream fileStream, int reference, EdgeSendProgressCallback progressCallback)
        {
            return Send(fileStream, out int id, reference, progressCallback);
        }

        public bool Send(FileStream fileStream, out int id)
        {
            return Send(fileStream, out id, 0, null);
        }

        public bool Send(FileStream fileStream, int reference)
        {
            return Send(fileStream, out int id, reference, null);
        }

        public bool Send(FileStream fileStream, out int id, int reference, EdgeSendProgressCallback progressCallback)
        {
            bool returnValue = false;
            id = 0;

            if (connectionEstablishing && IsConnected && remoteCrypto != null)
            {
                id = Rnd.Int();

                Debug($"Sending file stream {id}...");

                byte[] hash = null;
                using (MD5 md5 = MD5.Create())
                {
                    fileStream.Position = 0;
                    hash = md5.ComputeHash(fileStream);
                    fileStream.Position = 0;
                }

                Debug($"Checksum: {BitConverter.ToString(hash)}");

                int length = (int)fileStream.Length;

                byte[] head = new byte[52 + 4 + 4];

                int state = 0;

                Buffer.BlockCopy(hash, 0, head, 0, 16); // HASH
                Buffer.BlockCopy(BitConverter.GetBytes(reference), 0, head, 16, 4); // REFERENCE
                Buffer.BlockCopy(BitConverter.GetBytes(length), 0, head, 52, 4); // ORIGINAL DATA LENGTH
                Buffer.BlockCopy(BitConverter.GetBytes(id), 0, head, 56, 4); // ID

                byte[] encryptedHead = remoteCrypto.Encrypt(head, false);
                byte[] encryptedHeadLength = BitConverter.GetBytes(encryptedHead.Length);

                byte[] buffer = new byte[4 + encryptedHead.Length];

                Buffer.BlockCopy(encryptedHeadLength, 0, buffer, 0, 4);
                Buffer.BlockCopy(encryptedHead, 0, buffer, 4, encryptedHead.Length);

                Socket.BeginSend(buffer, 0, buffer.Length, 0, new AsyncCallback(delegate (IAsyncResult ar)
                {
                    SocketError error = SocketError.Success;

                    int sent = 0;
                    try
                    {
                        sent = Socket.EndSend(ar, out error);
                    }
                    catch (Exception ex)
                    {
                        Debug(ex);
                    }
                    if (error == SocketError.Success)
                    {
                        Debug("Header sent (" + sent + " bytes)");

                        byte[] sendBuffer = new byte[Socket.SendBufferSize];
                        int total = 0;

                        using (NetworkStream ns = new NetworkStream(Socket))
                        {
                            int read;
                            while ((read = fileStream.Read(sendBuffer, 0, sendBuffer.Length)) > 0)
                            {
                                total += read;
                                progressCallback?.Invoke(new EdgeProgessEventArgs(this, (int)fileStream.Length, total));
                                ns.Write(sendBuffer, 0, read);
                            }
                        }

                        Debug($"Data sent ({fileStream.Length} bytes)");

                        state = 1;
                    }
                    else
                    {
                        Debug("Fail " + error);
                        state = -1;
                    }
                }), this);

                // wait until, beginsend invokes callback
                do
                {
                    Thread.Sleep(5);
                }
                while (state == 0);

                if (state == 1)
                {
                    Debug("Data sent");
                    returnValue = true;
                }
            }

            return returnValue;
        }

        private bool Send(string type, params byte[][] data)
        {
            bool returnValue = false;

            if (connectionEstablishing && type != null && type.Length == 4)
            {
                int length = 0;
                foreach (byte[] d in data) length += d.Length;

                byte[] finalData = null;
                byte[] headBytes = Encoding.ASCII.GetBytes(type);

                if (remoteCrypto != null)
                {
                    Debug($"Sending {type} with encryption...");

                    // ORIGINAL DATA = HEAD + MESSAGE
                    byte[] originalData = new byte[length + 4];

                    Buffer.BlockCopy(headBytes, 0, originalData, 0, 4);
                    int offset = 4;
                    foreach (byte[] d in data)
                    {
                        Buffer.BlockCopy(d, 0, originalData, offset, d.Length);
                        offset += d.Length;
                    }
                    byte[] originalDataLength = BitConverter.GetBytes(originalData.Length);

                    // ORIGINAL DATA ENCRYPTED BY AES
                    byte[] aesEncryptedData = null, iv = null, key = null;

                    using (AesManaged rm = new AesManaged())
                    {
                        rm.Padding = PaddingMode.PKCS7;
                        rm.GenerateIV();
                        rm.GenerateKey();

                        iv = rm.IV; // 16 Bytes
                        key = rm.Key; // 32 Bytes

                        Debug("IV " + BitConverter.ToString(iv));
                        Debug("Key " + BitConverter.ToString(key));
                        Debug("Data length " + originalData.Length);

                        using (MemoryStream ms = new MemoryStream())
                        {
                            using (CryptoStream cs = new CryptoStream(ms, rm.CreateEncryptor(), CryptoStreamMode.Write))
                            {
                                cs.Write(originalData, 0, originalData.Length);
                            }

                            aesEncryptedData = ms.ToArray();
                        }
                    }

                    byte[] aesEncryptedDataLength = BitConverter.GetBytes(aesEncryptedData.Length);

                    Debug("Encrypted-data length " + aesEncryptedData.Length);

                    byte[] aesKeys = new byte[16 + 32 + 4 + 4 + 4];

                    Buffer.BlockCopy(iv, 0, aesKeys, 0, 16); // IV
                    Buffer.BlockCopy(key, 0, aesKeys, 16, 32); // KEY
                    Buffer.BlockCopy(originalDataLength, 0, aesKeys, 48, 4); // AES-ENCRYPTED LENGTH
                    Buffer.BlockCopy(aesEncryptedDataLength, 0, aesKeys, 52, 4); // ORIGINAL DATA LENGTH

                    byte[] rsaEncryptedAesKeys = remoteCrypto.Encrypt(aesKeys, false);
                    byte[] rsaEncryptedAesKeysLength = BitConverter.GetBytes(rsaEncryptedAesKeys.Length);

                    finalData = new byte[4 + rsaEncryptedAesKeys.Length + aesEncryptedData.Length];

                    Buffer.BlockCopy(rsaEncryptedAesKeysLength, 0, finalData, 0, 4);
                    Buffer.BlockCopy(rsaEncryptedAesKeys, 0, finalData, 4, rsaEncryptedAesKeys.Length);
                    Buffer.BlockCopy(aesEncryptedData, 0, finalData, 4 + rsaEncryptedAesKeys.Length, aesEncryptedData.Length);
                }
                else
                {
                    Debug($"Sending {type} without encryption...");

                    finalData = new byte[length + 12];

                    Buffer.BlockCopy(Node.HandshakeHead, 0, finalData, 0, 4);
                    Buffer.BlockCopy(headBytes, 0, finalData, 4, 4);
                    Buffer.BlockCopy(BitConverter.GetBytes(length), 0, finalData, 8, 4);

                    int offset = 12;
                    foreach (byte[] d in data)
                    {
                        Buffer.BlockCopy(d, 0, finalData, offset, d.Length);
                        offset += d.Length;
                    }
                }

                if (finalData != null)
                {
                    try
                    {
                        Debug("Sending " + finalData.Length + " bytes...");

                        int state = 0;
                        int sent = 0;

                        Socket.BeginSend(finalData, 0, finalData.Length, 0, new AsyncCallback(delegate (IAsyncResult ar)
                        {
                            SocketError error = SocketError.Success;

                            int thisSent = 0;
                            try
                            {
                                thisSent = Socket.EndSend(ar, out error);
                            }
                            catch (Exception ex)
                            {
                                Debug(ex);
                            }

                            if (error == SocketError.Success)
                            {
                                Debug("Sent (" + thisSent + " bytes)");

                                sent += thisSent;

                                if (sent == finalData.Length)
                                    state = 1;
                            }
                            else
                            {
                                Debug("Fail " + error);
                                state = -1;
                            }

                        }), this);

                        if (type == "MESG")
                        {
                            // wait until, beginsend invokes callback
                            do
                            {
                                Thread.Sleep(5);
                            }
                            while (state == 0);

                            if (state == 1)
                            {
                                Debug("Message sent");
                                returnValue = true;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug(ex);
                    }
                }
            }

            return returnValue;
        }

        public bool Send(BaseMessage message)
        {
            return Send(message, false, out BaseMessage empty, 0, null, null);
        }

        internal bool Send(BaseMessage message, int responseMessageID)
        {
            return Send(message, false, out BaseMessage response, responseMessageID, null, null);
        }

        public bool Send<T>(BaseMessage message, out T response) where T : BaseMessage
        {
            return Send(message, true, out response, 0, null, null);
        }

        internal bool Send<T>(BaseMessage message, out T response, int responseMessageID) where T : BaseMessage
        {
            return Send(message, true, out response, responseMessageID, null, null);
        }

        private bool Send<T>(BaseMessage message, bool waitForResponse, out T response, int responseMessageID, EdgeResponseWaitBreakCallback waitBreak, object waitBreakObject) where T : BaseMessage
        {
            response = default;

            if (IsConnected)
            {
                byte[] data = message.Serialize();

                Debug("Data is null? " + (data == null));

                //Debug($"Data length to be send: {data.Length}");

                if (data != null)
                {
                    int messageID = Rnd.Int();

                    Debug($"Sending message (message ID: {messageID})");

                    if (Send("MESG", BitConverter.GetBytes(messageID), BitConverter.GetBytes(responseMessageID), BitConverter.GetBytes(data.Length), data))
                    {
                        if (waitForResponse)
                        {
                            bool gotResponse = false;

                            while (true)
                            {
                                if (!connectionEstablishing) break;

                                if (responses.ContainsKey(messageID))
                                {
                                    gotResponse = true;
                                    break;
                                }

                                lock (incomingResponseSync)
                                {
                                    Monitor.Wait(incomingResponseSync, 5000);
                                }

                                if (waitBreak != null)
                                {
                                    bool isBreak = waitBreak(waitBreakObject);
                                    if (isBreak) break;
                                }
                            }

                            if (gotResponse)
                            {
                                lock (responses)
                                {
                                    response = responses[messageID] as T;
                                    responses.Remove(messageID);
                                }

                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                            return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void Event(string message)
        {
            Send(new EventMessage(message));
        }

        public void Event(string message, string context)
        {
            Event($"{context}:{message}");
        }

        #endregion

        #endregion
    }
}
