using System;
using System.Collections.Generic;
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

    public class Edge
    {
        #region Fields

        private Node node;

        private Thread clientConnectionThread = null;

        private bool running = true;

        private EdgeTypes edgeType;

        public EdgeTypes EdgeType { get => edgeType; }

        private IPAddress remoteAddress = null;

        public IPAddress RemoteAddress { get => remoteAddress; }

        private Socket socket = null;

        private bool connectingDisconnecting = false;

        private bool connected = false;

        public bool IsActive { get => (socket != null || connected || receiving); }

        private ManualResetEvent clientConnectionSignal = new ManualResetEvent(false);

        private ManualResetEvent receivingSignal = new ManualResetEvent(false);

        private Thread receivingThread = null;

        private bool receiving = false;

        private int handshakingStep = 0;

        private string remoteName = null;

        private RSACryptoServiceProvider localCrypto = null, remoteCrypto = null;

        #endregion

        #region Constructor

        private Edge(Node node, EdgeTypes type)
        {
            this.node = node;
            edgeType = type;
        }

        internal Edge(Node node, IPAddress remoteAddress) : this(node, EdgeTypes.Server)
        {
            this.remoteAddress = remoteAddress;

            clientConnectionThread = new Thread(new ThreadStart(delegate ()
            {
                Debug("Starting client connection thread");

                while (running)
                {
                    clientConnectionSignal.Reset();

                    if (!connected && !connectingDisconnecting)
                    {
                        try
                        {
                            handshakingStep = 0;

                            Debug("Connecting...");
                            connectingDisconnecting = true;

                            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                            socket.BeginConnect(new IPEndPoint(remoteAddress, Node.Port), new AsyncCallback(delegate (IAsyncResult ar)
                            {
                                connectingDisconnecting = false;

                                try
                                {
                                    socket.EndConnect(ar);

                                    Debug("Connected");

                                    if (running == false)
                                    {
                                        // whenever Edge ended, but after connected
                                        Debug("Edge is not running, disconnecting...");
                                    }
                                    else
                                    {
                                        connected = true;
                                        Receiving();
                                        
                                        Send("HLLO", Encoding.ASCII.GetBytes(node.Name));
                                    }


                                }
                                catch (Exception ex)
                                {
                                    Debug(ex);

                                    if (socket != null)
                                    {
                                        socket.Close();
                                        socket = null;
                                    }
                                }

                            }), this);
                        }
                        catch (Exception ex)
                        {
                            Debug(ex);

                            if (socket != null)
                            {
                                socket.Close();
                                socket = null;
                            }

                            connectingDisconnecting = false;
                        }
                    }
                    else
                    {
                        if (connected)
                        {
                            // ping, if still alive?
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
            this.socket = socket;

            IPEndPoint ep = socket.RemoteEndPoint as IPEndPoint;
            this.remoteAddress = ep.Address;

            connected = true;

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

        internal void Abort()
        {
            if (edgeType == EdgeTypes.Server)
            {
                Debug("Aborted");

                running = false;

                if (socket != null && socket.Connected)
                {
                    socket.Shutdown(SocketShutdown.Both);
                }

                End();
            }
        }

        internal void End()
        {
            Debug("End is called");

            if (edgeType == EdgeTypes.Client)
            {
                // if connection to client disconnected, end edge session
                node.RemoveEdge(this);
            }
            
            if (connected)
            {
                connected = false;
                
                Debug("Disconnecting...");

                connectingDisconnecting = true;

                socket.Shutdown(SocketShutdown.Both);
                socket.BeginDisconnect(false, new AsyncCallback(delegate (IAsyncResult ar)
                {
                    try
                    {
                        socket.EndDisconnect(ar);
                        Debug("Disconnected");

                        connectingDisconnecting = false;

                        if (socket != null)
                        {
                            socket.Close();
                            socket = null;
                        }

                        if (edgeType == EdgeTypes.Server)
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

        private string Head(byte[] checkHead)
        {
            string parsedHead = Encoding.ASCII.GetString(checkHead);

            if (parsedHead.IndexOf("HLLO", "OKOK", "ENCY", "WELC", "THNK") > -1) return parsedHead;
            else return null;
        }

        private void Receiving()
        {
            receivingThread = new Thread(new ThreadStart(delegate()
            {
                Debug("Starting receiving thread");

                receiving = true;

                int packetSeek = 0;

                byte[] buffer = new byte[Node.BufferSize];
                byte[] fourBytes = new byte[4];
                byte[] message = null;

                byte[] encryptedMessage = null;
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

                string head = null;
                int determinedLength = 0, determinedLength2 = 0;
                int decryptedDataLength = 0;

                while (receiving)
                {
                    receivingSignal.Reset();
                    try
                    {
                        if (socket.Connected)
                        { 
                            socket.BeginReceive(buffer, 0, Node.BufferSize, SocketFlags.None, new AsyncCallback(delegate (IAsyncResult result)
                            {
                                if (socket == null)
                                {
                                    Debug("Socket lost before begin receive callback");
                                    receiving = false;

                                    if (edgeType == EdgeTypes.Server)
                                    {
                                        End();
                                    }

                                    receivingSignal.Set();
                                    return;
                                }

                                SocketError error = SocketError.Success;

                                int bytesRead = -1;
                                
                                try
                                {
                                    bytesRead = socket.EndReceive(result, out error);
                                }
                                catch (Exception ex)
                                {
                                    Debug(ex);
                                }

                                Debug("Received: " + bytesRead);

                                if (bytesRead > 0)
                                {
                                    int bufferSeek = 0;
                                    int bytesLeft = bytesRead;

                                    do
                                    {
                                        bool packetEnd = false;
                                        int seek = 0;
                                        int partLength = 0;
                                        byte[] dest = null;

                                        bool encrypted = false;
                                        if (localCrypto != null) encrypted = true;

                                        if (!encrypted) // NOT ENCRYPTED
                                        {
                                            if (packetSeek < 4) seek = packetSeek; // AFIS
                                            else if (packetSeek < 8) seek = packetSeek - 4; // HEAD
                                            else if (packetSeek < 12) seek = packetSeek - 8; // MESSAGE LENGTH
                                            else seek = packetSeek - 12; // THE MESSAGE
                                            
                                            if (packetSeek < 12)
                                            {
                                                partLength = 4;
                                                dest = fourBytes;
                                            }
                                            else
                                            {
                                                partLength = determinedLength;
                                                dest = message;
                                            }
                                        }
                                        else // ENCRYPTED
                                        {
                                            if (packetSeek < 4) // RSA-ENCRYPTED-AES-KEYS LENGTH
                                            {
                                                seek = packetSeek; 
                                                partLength = 4;
                                                dest = fourBytes;
                                            }
                                            else if (packetSeek < (4 + determinedLength)) // RSA-ENCRYPTED-AES-KEYS
                                            {
                                                seek = packetSeek - 4; 
                                                partLength = determinedLength;
                                                dest = encryptedMessage;
                                            }     
                                            else if (packetSeek < (4 + determinedLength + determinedLength2)) // THE AES-ENCRYPTED-MESSAGE
                                            {
                                                seek = packetSeek - (4 + determinedLength);
                                                partLength = determinedLength2;
                                                dest = encryptedMessage;
                                            }
                                        }

                                        int length = (partLength - seek) < bytesLeft ? (partLength - seek) : bytesLeft;
                                        Buffer.BlockCopy(buffer, bufferSeek, dest, seek, length);

                                        packetSeek += length;
                                        bytesLeft -= length;
                                        bufferSeek += length;

                                        bool messageCompleted = false;

                                        if (!encrypted)
                                        {
                                            if (packetSeek == 4) // at the end of AFIS
                                            {
                                                if (!fourBytes.SequenceEqual(Node.HandshakeHead)) End();
                                            }
                                            else if (packetSeek == 8) // at the end of HEAD
                                            {
                                                head = Head(fourBytes);
                                                if (head == null) End();
                                            }
                                            else if (packetSeek == 12) // at the end of MESSAGE LENGTH
                                            {
                                                determinedLength = BitConverter.ToInt32(fourBytes, 0);
                                                Debug("Message Length: " + determinedLength);

                                                if (determinedLength == 0) End();
                                                else
                                                    message = new byte[determinedLength];
                                            }
                                            else if (packetSeek == (12 + determinedLength))
                                            {
                                                packetEnd = true;
                                                messageCompleted = true;
                                                Debug("Message completed");
                                            }
                                        }
                                        else
                                        {
                                            if (packetSeek == 4) // at the end of RSA-ENCRYPTED-AES-KEYS LENGTH
                                            {
                                                determinedLength = BitConverter.ToInt32(fourBytes, 0);
                                                Debug("RSA-Encrypted-AES-Keys Length: " + determinedLength);

                                                if (determinedLength == 0) End();
                                                else
                                                    encryptedMessage = new byte[determinedLength];
                                            }
                                            else if (packetSeek == (4 + determinedLength)) // at the end of RSA-ENCRYPTED-AES-KEYS
                                            {
                                                Debug("RSA-Encrypted-AES-Keys completed, begin decrypting...");

                                                byte[] aesKeys = localCrypto.Decrypt(encryptedMessage, false);

                                                // 16 iv, 32 key, 4 aesEncrypted length
                                                iv = new byte[16];
                                                key = new byte[32];

                                                Buffer.BlockCopy(aesKeys, 0, iv, 0, 16);
                                                Buffer.BlockCopy(aesKeys, 16, key, 0, 32);
                                                Buffer.BlockCopy(aesKeys, 48, fourBytes, 0, 4);

                                                determinedLength2 = BitConverter.ToInt32(fourBytes, 0);

                                                Debug("AES-Keys retrived");

                                                Debug("IV " + BitConverter.ToString(iv));
                                                Debug("Key " + BitConverter.ToString(key));

                                                if (determinedLength2 == 0) End();
                                                else
                                                    encryptedMessage = new byte[determinedLength2];

                                                Buffer.BlockCopy(aesKeys, 52, fourBytes, 0, 4);

                                                decryptedDataLength = BitConverter.ToInt32(fourBytes, 0);

                                                Debug("Data length: " + decryptedDataLength);
                                            }
                                            else if (packetSeek == (4 + determinedLength + determinedLength2))
                                            {
                                                packetEnd = true;

                                                Debug("AES-Encrypted-Message completed, begin decrypting...");

                                                using (RijndaelManaged rm = new RijndaelManaged())
                                                {
                                                    //rm.KeySize = 256;
                                                    //rm.Mode = CipherMode.CBC;
                                                    rm.Padding = PaddingMode.PKCS7;

                                                    rm.Key = key;
                                                    rm.IV = iv;

                                                    byte[] rawMessage = null;

                                                    using (MemoryStream ms = new MemoryStream())
                                                    {
                                                        using (CryptoStream cs = new CryptoStream(ms, rm.CreateDecryptor(), CryptoStreamMode.Write))
                                                        {
                                                            cs.Write(encryptedMessage, 0, encryptedMessage.Length);
                                                        }
                                                        rawMessage = ms.ToArray();
                                                    }


                                                    // get head
                                                    Buffer.BlockCopy(rawMessage, 0, fourBytes, 0, 4);
                                                    head = Head(fourBytes);
                                                    if (head == null) End();

                                                    // get message
                                                    int messageLength = rawMessage.Length - 4;

                                                    if (messageLength > 0)
                                                    {
                                                        message = new byte[messageLength];
                                                        Buffer.BlockCopy(rawMessage, 4, message, 0, messageLength);
                                                    }
                                                    else
                                                        message = null;

                                                    Debug("Message completed");

                                                    messageCompleted = true;
                                                }
                                            }
                                            else
                                            {
                                                Debug("Fallback = " + packetSeek);
                                            }

                                        }
                                        
                                        if (messageCompleted)
                                        {
                                            if (edgeType == EdgeTypes.Client) // IM A SERVER, AND EDGES ARE CLIENTS
                                            {
                                                if (handshakingStep == 0 && head == "HLLO")
                                                {
                                                    remoteName = Encoding.ASCII.GetString(message);
                                                    Send("OKOK", BitConverter.GetBytes(node.Name.Length), Encoding.ASCII.GetBytes(node.Name));
                                                    handshakingStep = 1;
                                                }
                                                else if (handshakingStep == 1 && head == "ENCY")
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
                                                    RSAParameters localPrivateKey = localCrypto.ExportParameters(true);
                                                    RSAParameters localPublicKey = localCrypto.ExportParameters(false);

                                                    exponent = localPublicKey.Exponent;
                                                    modulus = localPublicKey.Modulus;

                                                    Send("WELC", BitConverter.GetBytes(exponentLength), exponent, BitConverter.GetBytes(modulusLength), modulus);

                                                    handshakingStep = 2;
                                                }
                                                else if (handshakingStep == 2 && head == "THNK")
                                                {


                                                    Debug("SERVER Handshaking completed!");

                                                    handshakingStep = 3;
                                                }
                                            }
                                            else if (edgeType == EdgeTypes.Server) // IM A CLIENT, AND EDGE IS SERVER
                                            {
                                                if (handshakingStep == 0 && head == "OKOK")
                                                {
                                                    remoteName = Encoding.ASCII.GetString(message);

                                                    Debug("Generating authentication keys...");

                                                    localCrypto = new RSACryptoServiceProvider(2048);
                                                    RSAParameters localPrivateKey = localCrypto.ExportParameters(true);
                                                    RSAParameters localPublicKey = localCrypto.ExportParameters(false);

                                                    byte[] exponent = localPublicKey.Exponent;
                                                    byte[] modulus = localPublicKey.Modulus;

                                                    int exponentLength = exponent.Length;
                                                    int modulusLength = modulus.Length;

                                                    Send("ENCY", BitConverter.GetBytes(exponentLength), exponent, BitConverter.GetBytes(modulusLength), modulus);

                                                    handshakingStep = 1;
                                                }
                                                else if (handshakingStep == 1 && head == "WELC")
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

                                                    Debug("CLIENT Handshaking completed!");

                                                    if (Apps.Active)
                                                        Send("THNK", new byte[] { 1 });
                                                    else
                                                        Send("THNK", new byte[] { 0 });

                                                    handshakingStep = 2;
                                                }
                                            }
                                        }

                                        if (packetEnd)
                                        {
                                            packetSeek = 0;
                                        }
                                    }
                                    while (bytesLeft > 0);

                                }
                                else
                                {
                                    Debug("Socket zero-packet signal received");
                                    receiving = false;

                                    if (edgeType == EdgeTypes.Server)
                                    {
                                        End();
                                    }
                                }

                                receivingSignal.Set();

                            }), this);
                        }
                        else
                        {
                            Debug("Socket has been closed");

                            receiving = false;
                            receivingSignal.Set();

                            if (edgeType == EdgeTypes.Server)
                            {
                                End();
                            }
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

        public void Send(string head, params byte[][] data)
        {
            if (socket != null && head != null && head.Length == 4)
            {
                if (remoteCrypto != null)
                {
                    Debug("Sending with encryption...");

                    byte[] originalData = null;

                    int length = 0;
                    foreach (byte[] d in data) length += d.Length;

                    // ORIGINAL DATA = HEAD + MESSAGE
                    originalData = new byte[length + 4];
                    byte[] headBytes = Encoding.ASCII.GetBytes(head);
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

                    using (RijndaelManaged rm = new RijndaelManaged())
                    {
                        //rm.KeySize = 256;
                        //rm.Mode = CipherMode.CBC;
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

                    byte[] aesKeys = new byte[16 + 32 + 4 + 4];

                    Buffer.BlockCopy(iv, 0, aesKeys, 0, 16); // IV
                    Buffer.BlockCopy(key, 0, aesKeys, 16, 32); // KEY
                    Buffer.BlockCopy(aesEncryptedDataLength, 0, aesKeys, 48, 4); // AES-ENCRYPTED LENGTH
                    Buffer.BlockCopy(originalDataLength, 0, aesKeys, 52, 4); // ORIGINAL DATA LENGTH

                    byte[] rsaEncryptedAesKeys = remoteCrypto.Encrypt(aesKeys, false);
                    byte[] rsaEncryptedAesKeysLength = BitConverter.GetBytes(rsaEncryptedAesKeys.Length);

                    byte[] finalData = new byte[4 + rsaEncryptedAesKeys.Length + aesEncryptedData.Length];

                    Buffer.BlockCopy(rsaEncryptedAesKeysLength, 0, finalData, 0, 4);
                    Buffer.BlockCopy(rsaEncryptedAesKeys, 0, finalData, 4, rsaEncryptedAesKeys.Length);
                    Buffer.BlockCopy(aesEncryptedData, 0, finalData, 4 + rsaEncryptedAesKeys.Length, aesEncryptedData.Length);

                    try
                    {
                        socket.BeginSend(finalData, 0, finalData.Length, 0, new AsyncCallback(delegate (IAsyncResult ar)
                        {
                            SocketError error = SocketError.Success;

                            int bytesSent = -1;
                            try
                            {
                                bytesSent = socket.EndSend(ar, out error);
                            }
                            catch (Exception ex)
                            {
                                Debug(ex);
                            }

                            if (error == SocketError.Success && bytesSent > 0)
                            {
                                Debug("Successfully sent " + bytesSent + " bytes");
                            }
                            else
                            {

                            }

                        }), this);
                    }
                    catch (Exception ex)
                    {
                        Debug(ex);
                    }

                }
                else
                {
                    byte[] finalData = null;

                    int length = 0;
                    foreach (byte[] d in data) length += d.Length;

                    if (length > 0)
                    {
                        finalData = new byte[length + 12];

                        Buffer.BlockCopy(Node.HandshakeHead, 0, finalData, 0, 4);

                        byte[] headBytes = Encoding.ASCII.GetBytes(head);

                        Buffer.BlockCopy(headBytes, 0, finalData, 4, 4);

                        Buffer.BlockCopy(BitConverter.GetBytes(length), 0, finalData, 8, 4);

                        int offset = 12;
                        foreach (byte[] d in data)
                        {
                            Buffer.BlockCopy(d, 0, finalData, offset, d.Length);
                            offset += d.Length;
                        }

                        try
                        {
                            socket.BeginSend(finalData, 0, finalData.Length, 0, new AsyncCallback(delegate (IAsyncResult ar)
                            {
                                SocketError error = SocketError.Success;

                                int bytesSent = -1;
                                try
                                {
                                    bytesSent = socket.EndSend(ar, out error);
                                }
                                catch (Exception ex)
                                {
                                    Debug(ex);
                                }

                                if (error == SocketError.Success && bytesSent > 0)
                                {

                                }
                                else
                                {

                                }

                            }), this);
                        }
                        catch (Exception ex)
                        {
                            Debug(ex);
                        }
                    }
                }
            }
        }

        #endregion

    }
}
