using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aphysoft.Event
{
    class Program
    {
        static void Main(string[] args)
        {
            string name = "aev";

            if (Process.GetCurrentProcess().ProcessName != name)
            {
                Console.WriteLine("Rename this to " + name + ", then try again");
                return;
            }

            string type = null;
            IPAddress t = IPAddress.None;
            string s = null;
            byte[] id = null;

            if (args.Length == 1)
            {
                if (args[0] == "/?" || args[0] == "--help" || args[0] == "-h")
                {
                    type = "print-help";
                }
            }
            else if (args.Length == 3)
            {
                string target = args[0];
                string service = args[1];
                string identity = args[2];

                bool ok = true;
                
                if (target != null && IPAddress.TryParse(target, out t)) { }
                else
                {
                    ok = false;
                    Console.WriteLine("Target IP Address is invalid");
                }

                if (service != null && (service.Length > 0 && service.Length < 32))
                {
                    s = service;
                }
                else
                {
                    ok = false;
                    Console.WriteLine("Service name is invalid");
                }

                if (identity != null)
                {
                    ok = false;

                    string[] splits = identity.Split(new char[] { '-' });
                    if (splits.Length == 32)
                    {
                        List<byte> bytes = new List<byte>();

                        foreach (string split in splits)
                        {
                            if (split.Length == 2)
                            {
                                try
                                {
                                    byte b = Convert.ToByte(split, 16);
                                    bytes.Add(b);
                                }
                                catch (Exception ex) { }
                            }
                        }

                        if (bytes.Count == 32)
                        {
                            ok = true;
                            id = bytes.ToArray();
                        }
                    }

                    if (!ok)
                    {
                        Console.WriteLine("The identity is invalid");
                    }
                }

                if (ok)
                {
                    type = "execute";
                }
            }

            if (type != null)
            {
                if (type == "print-help")
                {
                    Version thisAppsVersion = Assembly.GetExecutingAssembly().GetName().Version;
                    Console.WriteLine("");
                    Console.WriteLine("Aphysoft.Event " + thisAppsVersion.ToString());
                    Console.WriteLine("Copyright (c) Afis Herman Reza Devara\n");
                    Console.WriteLine("USAGE: " + name + " <TARGET> <NAME> <IDENTITY>");
                    Console.WriteLine("   <TARGET> The IP address of the service's host");
                    Console.WriteLine("     <NAME> The name of the service");
                    Console.WriteLine(" <IDENTITY> 32-bytes hexadecimals to identify this connection");
                    Console.WriteLine("            (format: aa-bb-cc-dd-ee-ff ... )");
                }
                else if (type == "execute")
                {
                    // query service port
                    bool executeOK = false;

                    TcpClient client = null;

                    try
                    {
                        client = new TcpClient();
                        client.Connect(t, 23471);

                        NetworkStream stream = client.GetStream();
                        byte[] buffer = new byte[client.ReceiveBufferSize];

                        stream.Write(Encoding.ASCII.GetBytes("AFISSERV"), 0, 8);
                        stream.Write(Encoding.ASCII.GetBytes(s + "-EVENT"), 0, s.Length + 6);

                        int read = stream.Read(buffer, 0, buffer.Length);

                        if (read > 0)
                        {
                            byte[] reads = new byte[read];
                            Buffer.BlockCopy(buffer, 0, reads, 0, read);

                            string portstring = Encoding.ASCII.GetString(reads);

                            if (int.TryParse(portstring, out int port))
                            {
                                client.Close();

                                byte[] eventHead = new byte[] { (byte)'A', (byte)'F', (byte)'I', (byte)'S', (byte)'E', (byte)'V', (byte)'N', (byte)'T' };

                                /// copy paste from Apps.cs
                                /// 
                                while (true)
                                {
                                    byte[] iv = null;
                                    byte[] key = null;

                                    client = new TcpClient();
                                    try
                                    {
                                        Console.Write("Connecting to " + t + ":" + port + "...");
                                        client.Connect(t, port);

                                        stream = client.GetStream();
                                        stream.ReadTimeout = 2000;

                                        Console.WriteLine(" Connected");

                                        byte[] hssend;
                                        byte[] hsbuffer = new byte[client.ReceiveBufferSize];

                                        Thread.Sleep(1000);
                                        Console.Write("Creating secure channel...");

                                        RSACryptoServiceProvider clientCrypto = new RSACryptoServiceProvider(2048);
                                        RSAParameters clientPublicKey = clientCrypto.ExportParameters(false);
                                        byte[] exponent = clientPublicKey.Exponent;
                                        byte[] modulus = clientPublicKey.Modulus;

                                        hssend = new byte[8 + 4 + exponent.Length + 4 + modulus.Length];
                                        Buffer.BlockCopy(eventHead, 0, hssend, 0, 8);
                                        Buffer.BlockCopy(BitConverter.GetBytes(exponent.Length), 0, hssend, 8, 4);
                                        Buffer.BlockCopy(exponent, 0, hssend, 12, exponent.Length);
                                        Buffer.BlockCopy(BitConverter.GetBytes(modulus.Length), 0, hssend, 12 + exponent.Length, 4);
                                        Buffer.BlockCopy(modulus, 0, hssend, 16 + exponent.Length, modulus.Length);

                                        stream.Write(hssend, 0, hssend.Length);

                                        read = stream.Read(hsbuffer, 0, hsbuffer.Length);

                                        if (read == 0)
                                        {
                                            Console.Write(" Secure channel failed");
                                            break;
                                        }
                                        else
                                            Console.WriteLine(" Done");

                                        Console.Write("Authenticating...");

                                        byte[] eaes = new byte[read];
                                        Buffer.BlockCopy(hsbuffer, 0, eaes, 0, read);

                                        byte[] aes = clientCrypto.Decrypt(eaes, false);
                                        iv = new byte[16];
                                        key = new byte[32];

                                        Buffer.BlockCopy(aes, 0, iv, 0, 16);
                                        Buffer.BlockCopy(aes, 16, key, 0, 32);

                                        // submit identity by aes
                                        byte[] identity = new byte[32];

                                        // using custom identity
                                        identity = id;

                                        hssend = Aes.Encrypt(identity, iv, key);

                                        stream.Write(hssend, 0, hssend.Length);

                                        Array.Clear(hsbuffer, 0, hsbuffer.Length);
                                        read = stream.Read(hsbuffer, 0, hsbuffer.Length);

                                        if (read == 0)
                                        {
                                            Console.Write(" Authentication failed");
                                            break;
                                        }
                                        else
                                        {
                                            Console.WriteLine(" Accepted");
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(" Connection failed");
                                        break;
                                    }

                                    if (iv != null && key != null)
                                    {
                                        NetworkStream ns = client.GetStream();
                                        ns.ReadTimeout = Timeout.Infinite;

                                        try
                                        {

                                            int dataLength = 0;

                                            Seek.Variable(2, delegate(int index)
                                            {
                                                if (index == 0)
                                                    return 4;
                                                else
                                                    return dataLength;
                                            }, delegate ()
                                            {                                                
                                                byte[] rec = new byte[client.ReceiveBufferSize];
                                                byte[] des = new byte[ns.Read(rec, 0, rec.Length)];
                                                Array.Copy(rec, des, des.Length);

                                                return Array.ConvertAll(des, item => (object)item);

                                            }, delegate(int index, object[] data)
                                            {
                                                byte[] bytes = Array.ConvertAll(data, item => (byte)item);

                                                if (index == 0)
                                                {                                                    
                                                    dataLength = BitConverter.ToInt32(bytes, 0);
                                                }
                                                else if (index == 1)
                                                {
                                                    byte[] raw = Aes.Decrypt(bytes, iv, key);
                                                    Console.WriteLine(Encoding.ASCII.GetString(raw));
                                                }
                                            });

                                        }
                                        catch (Exception ex)
                                        {
                                            break;
                                        }

                                    }

                                    client.Close();
                                    break;
                                }

                                /// end here
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("ERROR: " + ex.Message);
                    }
                    finally
                    {
                        if (client != null)
                            client.Close();
                    }

                    if (!executeOK)
                    {
                        Console.WriteLine("");
                        Console.WriteLine(name + " has terminated");
                    }
                }
            }
            else
            {
                Console.WriteLine("USAGE: " + Assembly.GetExecutingAssembly().GetName().Name + " <TARGET> <NAME> <IDENTITY>");
            }
        }
    }

    // Share/Security/Aes.cs
    public static class Aes
    {
        private static byte[] EncryptProcess(byte[] data, AesManaged am)
        {
            byte[] edata = null;

            using (MemoryStream ms = new MemoryStream())
            {
                using (CryptoStream cs = new CryptoStream(ms, am.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    cs.Write(data, 0, data.Length);
                }

                edata = ms.ToArray();
            }

            return edata;
        }

        public static byte[] Encrypt(byte[] data, byte[] iv, byte[] key)
        {
            byte[] edata = null;

            using (AesManaged am = new AesManaged())
            {
                am.IV = iv;
                am.Key = key;

                edata = EncryptProcess(data, am);
            }

            return edata;
        }

        public static byte[] Encrypt(byte[] data, out byte[] iv, out byte[] key)
        {
            byte[] edata = null;

            using (AesManaged am = new AesManaged())
            {
                am.Padding = PaddingMode.PKCS7;

                am.GenerateIV();
                am.GenerateKey();

                iv = am.IV;
                key = am.Key;

                edata = EncryptProcess(data, am);
            }

            return edata;
        }

        public static byte[] Decrypt(byte[] edata, byte[] iv, byte[] key)
        {
            byte[] data = null;

            using (AesManaged am = new AesManaged())
            {
                am.IV = iv;
                am.Key = key;
                am.Padding = PaddingMode.PKCS7;

                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, am.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(edata, 0, edata.Length);
                    }

                    data = ms.ToArray();
                }
            }

            return data;
        }

        public static int EncryptedLength(int length)
        {
            return ((int)Math.Floor((double)length / 16) + 1) * 16;
        }
    }

    // Share/Util/Seek.cs
    public static class Seek
    {
        public delegate int SizeHandler(int index);
        public delegate object[] BufferRetrieve();
        public delegate void GroupCallback(int index, object[] data);

        public static void Variable(int groupCount, SizeHandler groupSize, BufferRetrieve retrieve, GroupCallback callback)
        {
            int order = 0;
            int orderSeek = 0;

            object[] orderBuffer = null;

            while (true)
            {
                object[] buffer = retrieve();

                int available = buffer.Length;

                if (available > 0)
                {
                    int bufferSeek = 0;

                    while (available > 0)
                    {
                        int orderCountRead = 0;

                        if (orderSeek == 0)
                        {
                            int orderSize = groupSize(order);

                            if (orderSize > 0)
                            {
                                orderCountRead = orderSize;

                                if (orderCountRead > 100000000) throw new OverflowException();
                                orderBuffer = new object[orderCountRead];
                            }
                            else
                            {
                                order++;
                            }
                        }

                        if (orderSeek < orderCountRead)
                        {
                            int orderLeft = orderCountRead - orderSeek;
                            int retrieved = (available < orderLeft) ? available : orderLeft;

                            Array.Copy(buffer, bufferSeek, orderBuffer, orderSeek, retrieved);

                            available -= retrieved;
                            orderSeek += retrieved;
                            bufferSeek += retrieved;

                            if (orderSeek == orderCountRead)
                            {
                                callback(order, orderBuffer);

                                order++;
                                orderSeek = 0;
                            }
                        }

                        if (order >= groupCount)
                        {
                            order = 0;
                        }
                    }
                }
                else
                {
                    break;
                }


            }
        }
    }
}
