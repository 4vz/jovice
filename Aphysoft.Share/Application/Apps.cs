using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using System.Configuration;
using System.Diagnostics;
using System.ServiceProcess;
using System.ComponentModel;
using System.Threading;
using System.Configuration.Install;
using System.Management;
using System.IO.Pipes;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System.Windows.Forms;
using System.Security.Principal;

namespace Aphysoft.Share
{
    public delegate void StartCallback(string directory);

    public delegate void StopCallback();

    public static class Apps
    {
        private const string appsRequired = "apps.exe is required for Apps Service";

        private static string serviceDirectory = null;

        public static string ServiceDirectory
        {
            get
            {
                if (serviceDirectory == null)
                    serviceDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Aphysoft", "Apps", "Service");
                return serviceDirectory;
            }
        }

        public static bool Active { get; private set; } = false;

        private static bool? console = null;

        private static readonly byte[] configEncryptedHeader = { (byte)'A', (byte)'P', (byte)'P', (byte)'S' };

        public static bool IsConsoleAvailable
        {
            get
            {
                if (console == null)
                {
                    if (Environment.UserInteractive)
                    {
                        console = true;
                        try { int t = System.Console.WindowHeight; }
                        catch { console = false; }
                    }
                    else
                        console = false;
                }
                return console.Value;
            }
        }
                
        public static void Console(object message, bool newLine = true)
        {
            Console(message.ToString(), newLine);
        }

        public static void Console(object message, int repeat)
        {
            Console(message.ToString(), repeat);
        }

        public static void Console(string message, bool newLine = true)
        {
            if (IsConsoleAvailable)
            {
                if (newLine == false)
                {
                    System.Console.Write(message);
                    consoleNewLine = false;
                }
                else
                {
                    System.Console.WriteLine(message);
                    consoleNewLine = true;
                }
            }
            else
            {
                if (outputStream != null)
                {
                    if (newLine)
                        outputStream.WriteLine(message);
                    else
                        outputStream.Write(message);
                }
            }
        }

        public static void Console(string message, int repeat)
        {
            if (IsConsoleAvailable)
            {
                if (repeat == 0)
                {
                    Console(message, false);
                }
                else if (repeat > 0)
                {
                    if (consoleNewLine == true)
                    {
                        Terminal.Up();                        
                    }

                    Terminal.ClearCurrentLine();
                    Console(message + " (" + repeat + ")", false);
                }
            }
        }

        public static string ConsoleReadLine()
        {
            if (IsConsoleAvailable)
            {
                return Terminal.ReadLine(terminalCancel.Token);
            }
            else
                return null;
        }

        public static ConsoleKeyInfo ConsoleReadKey()
        {
            if (IsConsoleAvailable)
            {
                return Terminal.ReadKey(terminalCancel.Token);
            }
            else
                return new ConsoleKeyInfo();
        }

        public static void ConsoleCancel()
        {
            terminalCancel.Cancel();
        }

        private static bool consoleNewLine = false;

        private static CancellationTokenSource terminalCancel = new CancellationTokenSource();

        private static Dictionary<string, string> configs = null;

        internal static bool LoadConfig(string directory)
        {
            // check config
            string configFile = Path.Combine(directory, "config");
            FileInfo cfi = new FileInfo(configFile);

            if (!cfi.Exists)
            {
                File.WriteAllText(cfi.FullName, "EMPTY:EMPTY");
            }

            string arch = null;
            string capt = null;
            string fami = null;
            string proc = null;

            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");
                foreach (ManagementObject queryObj in searcher.Get())
                {
                    arch = queryObj["Architecture"].ToString();
                    capt = queryObj["Caption"].ToString();
                    fami = queryObj["Family"].ToString();
                    proc = queryObj["ProcessorId"].ToString();
                    break;
                }
            }
            catch
            {
                return false;
            }

            if (arch == null || capt == null || fami == null || proc == null) return false;

            byte[] data = File.ReadAllBytes(configFile);

            bool encrypted = false;

            if (data.Length > 0)
            {
                if (data.Length > 4)
                {
                    if (data.StartsWith(configEncryptedHeader)) encrypted = true;
                }

                if (!encrypted)
                {
                    // we will encrypt this, but before that we must understand the file encoding, and fix it when necessary

                    // TODO
                    string plainConfig = File.ReadAllText(configFile, Encoding.UTF8);
                    File.WriteAllText(configFile, plainConfig);
                    data = File.ReadAllBytes(configFile);
                }



            }
            else return false;

            byte[] shaarch = Hash.SHA256(arch);
            byte[] shacapt = Hash.SHA256(capt);
            byte[] shafami = Hash.SHA256(fami);
            byte[] shaproc = Hash.SHA256(proc);

            byte[] iv = new byte[16];
            byte[] key = new byte[32];

            Buffer.BlockCopy(shaarch, 0, iv, 0, 8);
            Buffer.BlockCopy(shacapt, 0, iv, 8, 8);
            Buffer.BlockCopy(shafami, 0, key, 0, 16);
            Buffer.BlockCopy(shaproc, 0, key, 16, 16);

            // if plain, encrypt
            if (!encrypted)
            {
                using (AesManaged rm = new AesManaged())
                {
                    rm.IV = iv;
                    rm.Key = key;

                    using (FileStream ms = new FileStream(configFile, FileMode.Create))
                    {
                        ms.Write(configEncryptedHeader, 0, 4);

                        using (CryptoStream cs = new CryptoStream(ms, rm.CreateEncryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(data, 0, data.Length);
                            encrypted = true;
                        }
                    }

                    if (encrypted)
                        data = File.ReadAllBytes(configFile);
                }
            }

            if (encrypted)
            {
                byte[] configBytes = null;
                byte[] encryptedData = new byte[data.Length - 4];

                Buffer.BlockCopy(data, 4, encryptedData, 0, data.Length - 4);

                // read the encrypted file
                using (AesManaged rm = new AesManaged())
                {
                    rm.IV = iv;
                    rm.Key = key;

                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, rm.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(encryptedData, 0, encryptedData.Length);
                        }
                        configBytes = ms.ToArray();
                    }
                }

                if (configBytes != null)
                {
                    string configData = Encoding.UTF8.GetString(configBytes);

                    configs = new Dictionary<string, string>();
                    string[] lines = configData.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                    foreach (string line in lines)
                    {
                        string[] pair = line.Split(new[] { ':' }, 2);

                        if (pair.Length == 2)
                        {
                            string pkey = pair[0];

                            if (!configs.ContainsKey(pkey))
                            {
                                configs.Add(pkey, pair[1]);
                            }
                        }
                    }

                    // check for required 
                    bool passed = true;
                    foreach (string requiredConfig in requiredConfigs)
                    {
                        if (!configs.ContainsKey(requiredConfig))
                        {
                            passed = false;
                            break;
                        }
                    }

                    if (!passed)
                    {
                        Error("One of more required configurations are not found in config file");
                    }

                    return passed;
                }
                else return false;
            }
            else return false;
        }

        private static ServiceController currentServiceController = null;

        public static ServiceController GetCurrentService()
        {
            return currentServiceController;
        }

        public static ServiceController GetService(string name)
        {
            return WindowsService.GetService($"Aphysoft.Apps.{name}");
        }

        public static ServiceController GetAppsService()
        {
            return WindowsService.GetService("Aphysoft.Apps.Service");
        }

        public static void Service(StartCallback start, StopCallback stop = null, Node node = null)
        {
            if (start == null) return;

            if (Environment.UserInteractive)
            {               
                if ((string)AppDomain.CurrentDomain.GetData("data") == null)
                {
                    Assembly caller = Assembly.GetEntryAssembly();
                    AssemblyName callerName = caller.GetName();
                    string directory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
                    
                    FileInfo fiapps = new FileInfo(Path.Combine(directory, "apps.exe"));
                    if (!fiapps.Exists)
                        throw new FileNotFoundException(appsRequired, fiapps.FullName);

                    ServiceController serviceController = GetService(callerName.Name);
                    string serviceDisplayName = WindowsService.GetServiceDisplayName(WindowsService.GetServiceManagementObject(serviceController));

                    if (serviceDisplayName != null)
                    {
                        currentServiceController = serviceController;

                        if (IsConsoleAvailable)
                        {
                            System.Console.WriteLine($"This service has been installed locally as a Windows Service ({serviceDisplayName}).");
                            System.Console.WriteLine("Please use apps.exe or Services Console to start or stop this service.");
                            System.Console.WriteLine("");

                            if (serviceController.Status == ServiceControllerStatus.Running && node != null)
                            {
                                if (IsConsoleAvailable)
                                {
                                    System.Console.Write("Do you want to track events from this service? (y/n)");

                                    bool listenEvents = false;
                                    while (true)
                                    {
                                        ConsoleKeyInfo k = System.Console.ReadKey(true);

                                        if (k.Key == ConsoleKey.Y || k.Key == ConsoleKey.N)
                                        {
                                            if (k.Key == ConsoleKey.Y)
                                            {
                                                System.Console.Clear();
                                                System.Console.Write("Connecting...");
                                                Thread.Sleep(1000);
                                                listenEvents = true;
                                            }
                                            break;
                                        }
                                    }

                                    if (listenEvents)
                                    {
                                        byte[] eventHead = new byte[8];

                                        Buffer.BlockCopy(Node.HandshakeHead, 0, eventHead, 0, 4);
                                        Buffer.BlockCopy(Node.ServiceEventHead, 0, eventHead, 4, 4);

                                        while (true)
                                        {
                                            int port = node.GetPort("EVENT");

                                            byte[] iv = null;
                                            byte[] key = null;

                                            TcpClient client = new TcpClient();
                                            try
                                            {
                                                client.Connect(IPAddress.Loopback, port);

                                                NetworkStream stream = client.GetStream();
                                                stream.ReadTimeout = 2000;

                                                System.Console.Clear();
                                                System.Console.WriteLine("Connected");

                                                byte[] hssend;
                                                byte[] hsbuffer = new byte[client.ReceiveBufferSize];

                                                Thread.Sleep(1000);
                                                System.Console.Write("Creating secure channel...");

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

                                                int read = stream.Read(hsbuffer, 0, hsbuffer.Length);

                                                if (read == 0)
                                                {
                                                    System.Console.Write(" Secure channel failed");
                                                    Thread.Sleep(2000);
                                                    client.Close();
                                                    System.Console.Clear();
                                                    System.Console.Write("Reconnecting...");
                                                    continue;
                                                }
                                                else
                                                    System.Console.WriteLine(" Done");

                                                System.Console.Write("Authenticating...");

                                                byte[] eaes = new byte[read];
                                                Buffer.BlockCopy(hsbuffer, 0, eaes, 0, read);

                                                byte[] aes = clientCrypto.Decrypt(eaes, false);
                                                iv = new byte[16];
                                                key = new byte[32];

                                                Buffer.BlockCopy(aes, 0, iv, 0, 16);
                                                Buffer.BlockCopy(aes, 16, key, 0, 32);

                                                // submit identity by aes
                                                byte[] identity = new byte[32];

                                                // this, is used Process ID
                                                int cprocessid = Process.GetCurrentProcess().Id;
                                                Buffer.BlockCopy(BitConverter.GetBytes(cprocessid), 0, identity, 0, 4);

                                                // using custom identity

                                                //string test = "AA-cB-CC-DD-EE-FF-AA-BB-CC-DD-EE-FF-00-11-22-33-AA-BB-CC-DD-EE-FF-AA-BB-CC-DD-EE-FF-00-11-22-33";
                                                //string[] testx = test.Split(new char[] { '-' });
                                                //List<byte> ids = new List<byte>();
                                                //foreach (string t in testx)
                                                //{
                                                //    ids.Add(Convert.ToByte(t, 16));
                                                //}
                                                //identity = ids.ToArray();

                                                hssend = Aes.Encrypt(identity, iv, key);

                                                stream.Write(hssend, 0, hssend.Length);

                                                Array.Clear(hsbuffer, 0, hsbuffer.Length);
                                                read = stream.Read(hsbuffer, 0, hsbuffer.Length);

                                                if (read == 0)
                                                {
                                                    System.Console.Write(" Authentication failed");
                                                    Thread.Sleep(2000);
                                                    client.Close();
                                                    System.Console.Clear();
                                                    System.Console.Write("Reconnecting...");
                                                    Thread.Sleep(1000);
                                                    continue;
                                                }
                                                else
                                                {
                                                    System.Console.Clear();
                                                }

                                            }
                                            catch (Exception ex)
                                            {
                                                System.Console.Write($" Connection failed: {ex.Message} {ex.StackTrace}");
                                                Thread.Sleep(2000);
                                                System.Console.Clear();
                                                System.Console.Write("Reconnecting...");
                                                Thread.Sleep(1000);
                                                continue;
                                            }

                                            if (iv != null && key != null)
                                            {
                                                NetworkStream ns = client.GetStream();
                                                ns.ReadTimeout = Timeout.Infinite;

                                                try
                                                {

                                                    int dataLength = 0;

                                                    Seek.Variable(2, delegate (int index)
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

                                                    }, delegate (int index, object[] data)
                                                    {
                                                        byte[] bytes = Array.ConvertAll(data, item => (byte)item);

                                                        if (index == 0)
                                                        {
                                                            dataLength = BitConverter.ToInt32(bytes, 0);
                                                        }
                                                        else if (index == 1)
                                                        {
                                                            byte[] raw = Aes.Decrypt(bytes, iv, key);
                                                            System.Console.WriteLine(Encoding.ASCII.GetString(raw));
                                                        }
                                                    });

                                                }
                                                catch (Exception ex)
                                                {
                                                    break;
                                                }

                                            }

                                            client.Close();
                                            System.Console.WriteLine("Disconnected");
                                            Thread.Sleep(1000);
                                            System.Console.Clear();
                                            System.Console.Write("Reconnecting...");
                                        }
                                    }
                                }
                                else if (serviceController.Status == ServiceControllerStatus.Stopped)
                                {
                                    System.Console.WriteLine("This service is currently stopped.");
                                    Thread.Sleep(3000);
                                }
                            }
                        }
                    }
                    else
                    {
                        // to test sandboxing environment in DEBUG, put ! in the front of DEBUG
                        // not for windows form interactive mode
#if DEBUG
                        Active = true;
                                                
                        bool config = LoadConfig(directory);
                        if (!config) return;

                        start(directory);
#else
                        Process process = new Process
                        {
                            EnableRaisingEvents = true,
                            StartInfo = new ProcessStartInfo()
                        };
                        process.StartInfo.Arguments = caller.Location + " " + callerName.Name + "/" + callerName.Version.ToString() + "/" + callerName.ProcessorArchitecture.ToString();
                        process.StartInfo.FileName = "apps.exe";
                        process.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
                        process.Start();
#endif
                        stop?.Invoke();
                    }
                }
                else
                {
                    Environment.ExitCode = 0;

                    string directory = (string)AppDomain.CurrentDomain.GetData("directory");

                    FileInfo fiapps = new FileInfo(Path.Combine(directory, "apps.exe"));
                    if (!fiapps.Exists)
                        throw new FileNotFoundException(appsRequired, fiapps.FullName);

                    Active = true;

                    bool config = LoadConfig(directory);
                    if (!config) return;

                    start(directory);

                    stop?.Invoke();
                }
            }
            else
            {
                if (stop == null)
                    throw new ArgumentNullException("stop", "stop argument is required in Windows Service environment.");

                ServiceController appsService = GetAppsService();
                if (appsService == null)
                    throw new ConfigurationErrorsException("Aphysoft Apps Service is required to be installed.");
                else if (appsService.Status == ServiceControllerStatus.Stopped)
                {
                    appsService.Start();
                    appsService.WaitForStatus(ServiceControllerStatus.Running);
                }
                
                string directory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

                Active = true;

                bool config = LoadConfig(directory);
                if (!config) return;

                Assembly caller = Assembly.GetEntryAssembly();
                AssemblyName callerName = caller.GetName();

                currentServiceController = GetService(callerName.Name);

                ServiceBase.Run(new ServiceBase[] { new AppsService(start, stop) });
            }
        }

        public static void Service(Node node)
        {
            Service(delegate (string directory)
            {
                node.Start(directory);
            }, delegate ()
            {
                node.Stop();
            }, node);
        }

        public static void Service(Type form, Node node = null)
        {
            Service(delegate (string directory)
            {
                string fconfi = Path.Combine(directory, "form.config");
                FileInfo fconf = new FileInfo(fconfi);
                if (fconf.Exists) fconf.Delete();
                FileStream fs = File.Create(fconfi);
                string fconfdata = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n<configuration>\r\n  <System.Windows.Forms.ApplicationConfigurationSection>\r\n    <add key=\"DpiAwareness\" value=\"PerMonitorV2\" />\r\n  </System.Windows.Forms.ApplicationConfigurationSection>\r\n</configuration>";
                byte[] fconfbytes = Encoding.UTF8.GetBytes(fconfdata);
                fs.Write(fconfbytes, 0, fconfbytes.Length);
                fs.Close();

                AppDomain.CurrentDomain.SetData("APP_CONFIG_FILE", fconfi);

                if (node != null)
                {
                    Thread thread = new Thread(new ThreadStart(delegate ()
                    {
                        node.Start(directory);
                    }));
                    thread.Start();
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run((Form)form.GetConstructors()[0].Invoke(new object[] { node }));

            }, delegate ()
            {
                if (node != null)
                    node.Stop();
            }, node);
        }

        public static string Config(string key, string def)
        {
            if (configs == null)
            {
                return null;
            }
            else
            {
                if (configs.ContainsKey(key))
                {
                    return configs[key];
                }
                else
                {
                    return def;
                }
            }
        }

        public static string Config(string key)
        {
            return Config(key, null);
        }

        private static List<string> requiredConfigs = new List<string>();

        public static void Require(params string[] keys)
        {
            foreach (string s in keys)
            {
                if (!requiredConfigs.Contains(s))
                {
                    requiredConfigs.Add(s);
                }
            }
        }

        public static void SetExit(ExitType type)
        {
            if (Active)
            {
                if (type == ExitType.RestartHost)
                {
                    Environment.ExitCode = 50;
                }
                else if (type == ExitType.Restart)
                {
                    Environment.ExitCode = 100;
                }
                else
                {
                    Environment.ExitCode = 0;
                }
            }
        }

        public static string ServiceCommand(ServiceCommandType command, string data)
        {
            StringBuilder sb = new StringBuilder();

            if (command == ServiceCommandType.Restart)
            {
                sb.Append('R');
            }

            if (data != null)
                sb.Append(data);

            if (sb.Length > 0)
            {
                NamedPipeClientStream pipe = new NamedPipeClientStream(".", "apps", PipeDirection.InOut);

                string ret = null;

                try
                {
                    pipe.Connect(1000);

                    byte[] sendBuffer = Encoding.ASCII.GetBytes(sb.ToString());
                    int slen = sendBuffer.Length;
                    if (slen > UInt16.MaxValue) slen = (int)UInt16.MaxValue;
                    pipe.WriteByte((byte)1);
                    pipe.WriteByte((byte)(slen / 256));
                    pipe.WriteByte((byte)(slen & 255));
                    pipe.Write(sendBuffer, 0, slen);
                    
                    int b1 = pipe.ReadByte();
                    int b2 = pipe.ReadByte();
                    int rlen = b1 * 256;
                    rlen += b2;

                    if (rlen > 0)
                    {
                        byte[] receiveBuffer = new byte[rlen];
                        pipe.Read(receiveBuffer, 0, rlen);
                        ret = Encoding.ASCII.GetString(receiveBuffer);
                    }

                    pipe.Close();
                }
                catch (Exception ex)
                {
                    ret = "FAILED:" + ex.Message;
                }

                return ret;
            }
            else
                return null;
        }

        public static void Error(string message)
        {
            if (Environment.UserInteractive)
            {
                if (IsConsoleAvailable)
                {
                    Console("Error: " + message);
                    Thread.Sleep(5000);
                }
                else
                {
                    MessageBox.Show(message, "Error");
                }
            }
            else
            {
                throw new Exception(message);
            }

            Environment.Exit(1);
        }

        private static StreamWriter outputStream = null;

        public static void SetFileOutput(string file)
        {
            outputStream = new StreamWriter(file, true, Encoding.Default);
        }
    }

    public enum ExitType
    {
        Exit,
        RestartHost,
        Restart
    }

    public enum ServiceCommandType
    {
        Restart
    }

    internal class AppsService : ServiceBase
    {
        private StartCallback start;
        private StopCallback stop;

        public AppsService(StartCallback start, StopCallback stop) : base()
        {
            this.start = start;
            this.stop = stop;
        }

        protected override void OnStart(string[] args)
        {
            start?.Invoke(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));
        }

        protected override void OnStop()
        {
            stop?.Invoke();
        }
    }

    public abstract class AppsInstaller : Installer
    {
        public AppsInstaller(string displayName, string description)
        {            
            ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller();
            ServiceInstaller serviceInstaller = new ServiceInstaller();

            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            serviceProcessInstaller.Password = null;
            serviceProcessInstaller.Username = null;

            serviceInstaller.Description = description;
            serviceInstaller.DisplayName = displayName;
            serviceInstaller.ServiceName = "Aphysoft.Apps." + Assembly.GetExecutingAssembly().GetName().Name;
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.DelayedAutoStart = true;

            Installers.AddRange(new Installer[] {
                serviceProcessInstaller,
                serviceInstaller
                });
        }
    }
}
