using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.IO;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Security.Principal;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Threading;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting;
using System.Diagnostics;
using System.IO.Pipes;
using System.Net;
using System.Net.Sockets;
using System.Security;
using System.Runtime.InteropServices;
using System.Security.AccessControl;

namespace Aphysoft.Apps
{
    internal class Program
    {
        private static bool? console = null;

        private static bool IsConsoleAvailable
        {
            get
            {
                if (console == null)
                {
                    if (Environment.UserInteractive)
                    {
                        console = true;
                        try { int t = Console.WindowHeight; }
                        catch { console = false; }
                    }
                    else
                        console = false;
                }
                return console.Value;
            }
        }

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern bool FreeConsole();

        [STAThread]
        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                string programFilesDir = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
                string appsServiceDir = Path.Combine(programFilesDir, "Aphysoft", "Apps", "Service");
                string appsServiceFile = Path.Combine(appsServiceDir, AppDomain.CurrentDomain.FriendlyName);

                if (appsServiceFile != Assembly.GetEntryAssembly().Location)
                {
                    if (args.Length == 2)
                    {
                        string assembly = args[0];
                        string assemblyData = args[1];

                        if (assembly == "-i" || assembly == "-u")
                        {
                            //Console.SetWindowSize(1, 1);
                            FreeConsole();

                            FileInfo fi = new FileInfo(assemblyData);
                            if (fi.Exists && fi.Extension == ".exe")
                            {
                                if (assembly == "-i")
                                {
                                    ManagedInstallerClass.InstallHelper(new string[] { "/LogFile=", "/LogToConsole=false", "/InstallStateDir=" + Path.GetTempPath(), assemblyData });
                                }
                                else
                                {
                                    ManagedInstallerClass.InstallHelper(new string[] { "/u", "/LogFile=", "/LogToConsole=false", assemblyData });
                                }
                            }

                            return;
                        }

                        string name = Path.GetFileNameWithoutExtension(assembly);
                        string directory = Path.GetDirectoryName(assembly);

                        // check appsl and _UPDATE
                        string appsl = Path.Combine(directory, "appsl.exe");
                        if (File.Exists(appsl)) File.Delete(appsl);

                        string appsu = Path.Combine(directory, "apps.exe_UPDATE");
                        if (File.Exists(appsu)) File.Delete(appsu);

                        AppDomainSetup setup = new AppDomainSetup { ApplicationName = name, ShadowCopyFiles = "true" };

                        Thread terminalThread = null;
                        AppDomain domain = null;
                        bool running = true;

                        CancellationTokenSource terminalCancel = new CancellationTokenSource();

                        if (IsConsoleAvailable)
                        {                            
                            terminalThread = new Thread(new ThreadStart(delegate ()
                            {
                                while (true)
                                {
                                    if (domain != null)
                                    {
                                        string readLine = Terminal.ReadLine(terminalCancel.Token);
                                        if (readLine == null) break;
                                        if (!running) break;

                                        string line = readLine.Trim();

                                        if (line != "")
                                        {
                                            domain.SetData("terminalCommand", line);
                                            if (line.ToLower() == "exit") break;
                                        }
                                    }
                                    else Thread.Sleep(10);
                                }
                            }));
                            terminalThread.Start();
                        }

                        while (true)
                        {
                            domain = AppDomain.CreateDomain(name, AppDomain.CurrentDomain.Evidence, setup);
                            
                            if (assemblyData == null) assemblyData = "";
                            domain.SetData("data", assemblyData);
                            domain.SetData("directory", directory); 

                            try
                            {
                                domain.ExecuteAssembly(assembly); // should block until application exited
                                int exitcode = Environment.ExitCode;
                                AppDomain.Unload(domain);

                                if (exitcode == 0) break;
                                else if (exitcode == 50)
                                {
                                    File.Copy(Path.Combine(directory, "apps.exe"), Path.Combine(directory, "appsl.exe"));

                                    Process current = Process.GetCurrentProcess();

                                    Process process = new Process { StartInfo = new ProcessStartInfo() };
                                    process.StartInfo.Arguments = "-r \"" + assembly + "\" " + current.Id;
                                    process.StartInfo.FileName = "appsl.exe";
                                    process.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
                                    process.StartInfo.CreateNoWindow = true;
                                    process.Start();

                                    break;

                                    // apps updated, shutdown and start apps
                                }
                                else if (exitcode == 100)
                                {
                                    // only restart domain, let the looper do his job
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("Exception: " + ex.Message);
                                break;
                            }
                        }

                        running = false;

                        if (terminalThread != null)
                        {
                            terminalCancel.Cancel();

                            Console.WriteLine("Apps instance terminated");
                            Thread.Sleep(1000);
                        }
                    }
                    else if (args.Length == 3)
                    {
                        string retok = args[0];
                        string assembly = args[1];
                        string pids = args[2];

                        if (retok == "-r")
                        {
                            if (int.TryParse(pids, out int pid))
                            {
                                // wait for pid until closed
                                while (true)
                                {
                                    try
                                    {
                                        Process pr = Process.GetProcessById(pid);
                                        Thread.Sleep(500);
                                    }
                                    catch
                                    {
                                        // pid is not running, then release
                                        break;
                                    }
                                }
                            }

                            string directory = Path.GetDirectoryName(assembly);
                            string apps = Path.Combine(directory, "apps.exe");
                            string appsUpdate = Path.Combine(directory, "apps.exe_UPDATE");

                            if (File.Exists(appsUpdate))
                            {
                                // replace old with this
                                File.Delete(apps);
                                File.Move(appsUpdate, apps);
                                Thread.Sleep(500);
                            }
                            else
                            {
                            }

                            Process process = new Process { StartInfo = new ProcessStartInfo(assembly) };
                            process.Start();

                        }
                    }
                    else if (args.Length == 0)
                    {
                        // check, is this exe run as administrator
                        WindowsIdentity identity = WindowsIdentity.GetCurrent();
                        WindowsPrincipal principal = new WindowsPrincipal(identity);
                        bool isAdmin = principal.IsInRole(WindowsBuiltInRole.Administrator);

                        if (!isAdmin)
                        {
                            Console.WriteLine("Services configuration requires elevated permission.");
                            Console.WriteLine("Please <Run> this program <as administrator> to continue...");
                            Console.ReadKey();
                        }
                        else
                        {
                            DirectoryInfo di = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));

                            FileInfo sourceProgFile = null;
                            string selectedAppsService = null;

                            CheckAppsService(out ServiceController appsService, out Dictionary<string, Tuple<ServiceController, string, string>> appsMemberServices);

                            List<FileInfo> fis = new List<FileInfo>();
                            foreach (FileInfo fi in di.GetFiles("*.exe")) if (fi.Name != "apps.exe") fis.Add(fi);

                            if (fis.Count == 1)
                            {
                                sourceProgFile = fis[0];
                                foreach (KeyValuePair<string, Tuple<ServiceController, string, string>> pair in appsMemberServices)
                                {
                                    // jika nama sama, file ini adalah nama service yg uda keiinstall
                                    if ((new FileInfo(pair.Value.Item2).Name) == fis[0].Name)
                                    {
                                        selectedAppsService = pair.Key;
                                        break;
                                    }
                                }
                            }

                            if (sourceProgFile == null) return;

                            if (appsMemberServices.Count > 0)
                                AppsService();
                            else
                                DeleteAppsService();

                            string installName = null;

                            int action = -1;
                            bool toInstall = false;

                            if (selectedAppsService != null)
                            {
                                ServiceController service = appsMemberServices[selectedAppsService].Item1;

                                bool disabled = false;
                                string wmiQuery = @"SELECT * FROM Win32_Service WHERE Name='" + service.ServiceName + @"'";
                                ManagementObjectSearcher searcher = new ManagementObjectSearcher(wmiQuery);
                                ManagementObjectCollection results = searcher.Get();
                                foreach (ManagementObject mo in results)
                                {
                                    if (mo["StartMode"].ToString() == "Disabled")
                                    {
                                        disabled = true;
                                        break;
                                    }
                                }

                                if (disabled)
                                {
                                    Console.WriteLine("[" + appsMemberServices[selectedAppsService].Item3 + "]");
                                    Console.WriteLine("The service is currently waiting for uninstallation (or else being disabled).");
                                    Console.WriteLine("If it is still waiting for uninstallation, you may wait and or close everything (like Services Management Console) that left opened.");
                                    Console.WriteLine("If it is being disabled, please set start up type to either automatic or manual (by using Services Management Console), then try this again.");
                                }
                                else
                                {
                                    Console.WriteLine("[" + appsMemberServices[selectedAppsService].Item3 + "] [" + service.Status.ToString() + "]");
                                    Console.WriteLine("Select the action: ");
                                    Console.WriteLine("(1) Uninstall Service");
                                    Console.WriteLine("(2) Push new config file");

                                    if (service.Status == ServiceControllerStatus.Running)
                                    {
                                        Console.WriteLine("(3) Stop Service");
                                        Console.WriteLine("(4) Restart Service");
                                        Console.WriteLine("(5) Exit");
                                        action = ReadSelect(5);
                                        if (action == 4) return;
                                    }
                                    else
                                    {
                                        Console.WriteLine("(3) Start Service");
                                        Console.WriteLine("(4) Exit");
                                        action = ReadSelect(4);
                                        if (action == 3) return;
                                    }
                                }
                            }
                            else
                            {
                                installName = Path.GetFileNameWithoutExtension(sourceProgFile.Name);

                                Console.WriteLine("[" + installName + "]");
                                Console.WriteLine("(1) Install as Windows Service");
                                Console.WriteLine("(2) Exit");
                                action = ReadSelect(2);
                                toInstall = true;

                                if (action == 1) return;
                            }

                            CheckAppsService(out appsService, out appsMemberServices);

                            if (action == 0)
                            {
                                if (toInstall)
                                {
                                    Console.WriteLine("> Install as Windows Service");

                                    BeginProgress();

                                    AppsService();

                                    string appsDirectory = GetAppsServiceDirectory();
                                    string progDir = Path.Combine(appsDirectory, installName);

                                    if (Directory.Exists(progDir)) Directory.Delete(progDir, true);
                                    Directory.CreateDirectory(progDir);

                                    CopyInstallation(di.FullName, progDir, true);

                                    string progPath = Path.Combine(progDir, sourceProgFile.Name);

                                    Install(progPath);

                                    CheckAppsService(out appsService, out appsMemberServices);

                                    ServiceController progService = null;
                                    string progDisplayName = null;

                                    foreach (KeyValuePair<string, Tuple<ServiceController, string, string>> pair in appsMemberServices)
                                    {
                                        Tuple<ServiceController, string, string> v = pair.Value;

                                        if (v.Item2 == progPath)
                                        {
                                            progService = v.Item1;
                                            progDisplayName = v.Item3;
                                            break;
                                        }
                                    }

                                    if (progService != null) Start(progService);

                                    EndProgress();
                                }
                                else
                                {
                                    Console.WriteLine("> Uninstall Service");

                                    BeginProgress();

                                    ServiceController progService = appsMemberServices[selectedAppsService].Item1;
                                    string progPath = appsMemberServices[selectedAppsService].Item2;
                                    string dispName = appsMemberServices[selectedAppsService].Item3;
                                    string progDir = (new FileInfo(progPath)).Directory.FullName;

                                    Stop(progService);

                                    if (!File.Exists(progPath))
                                    {
                                        if (!Directory.Exists(progDir)) Directory.CreateDirectory(progDir);
                                        File.Copy(sourceProgFile.FullName, progPath);
                                    }

                                    Uninstall(progPath);

                                    Directory.Delete(progDir, true);

                                    CheckAppsService(out appsService, out appsMemberServices);

                                    if (appsMemberServices.Count == 0)
                                    {
                                        DeleteAppsService();
                                    }

                                    EndProgress();
                                }
                            }
                            else if (action == 1)
                            {
                                Console.WriteLine("> Push new config file");

                                BeginProgress();

                                ServiceController progService = appsMemberServices[selectedAppsService].Item1;
                                string progPath = appsMemberServices[selectedAppsService].Item2;
                                string dispName = appsMemberServices[selectedAppsService].Item3;
                                string progDir = (new FileInfo(progPath)).Directory.FullName;
                                
                                string progConfig = Path.Combine(progDir, "config");
                                string sourceConfig = Path.Combine(sourceProgFile.Directory.FullName, "config");

                                if (File.Exists(sourceConfig))
                                {
                                    Stop(progService);
                                    File.Copy(sourceConfig, progConfig, true);
                                    Start(progService);
                                }
                                else
                                {
                                    Console.WriteLine("Source config is not exist");
                                }

                                EndProgress();

                            }
                            else if (action == 2)
                            {
                                BeginProgress();

                                ServiceController progService = appsMemberServices[selectedAppsService].Item1;

                                if (progService.Status == ServiceControllerStatus.Running)
                                {
                                    Console.WriteLine("> Stop service");
                                    Stop(progService);
                                }
                                else if (progService.Status == ServiceControllerStatus.Stopped)
                                {
                                    Console.WriteLine("> Start service");
                                    Start(progService);
                                }

                                EndProgress();
                            }
                            else if (action == 3)
                            {
                                BeginProgress();

                                ServiceController progService = appsMemberServices[selectedAppsService].Item1;

                                Console.WriteLine("> Restart service");

                                Stop(progService);
                                Start(progService);

                                EndProgress();
                            }
                        }
                    }
                }
                else
                {
                    // select
                    //int i = 0;
                    //string[] selections = new string[appsMemberServices.Count];
                    //foreach (KeyValuePair<string, Tuple<ServiceController, string, string>> pair in appsMemberServices)
                    //{
                    //    Tuple<ServiceController, string, string> t = pair.Value;
                    //    Console.WriteLine("(" + (i + 1) + ") " + t.Item3);
                    //   selections[i] = pair.Key;
                    //   i++;
                    //}
                    //selectedAppsService = selections[ReadSelect(i)];
                }

                if (progressThread != null)
                    progressThread.Join();
            }
            else
            {
                 ServiceBase.Run(new ServiceBase[] { new AppsService() });
            }
        }

        private static double progress = 0;
        private static double rate = 0;
        private static bool ending = false;
        private static string lastProgress = null;

        private static Thread progressThread = null;

        private static void ClearLine()
        {
            Console.SetCursorPosition(0, Console.CursorTop);
            for (int i = 0; i < Console.BufferWidth - 1; i++) Console.Write(' ');
            Console.SetCursorPosition(0, Console.CursorTop);
        }

        private static void BeginProgress()
        {
            ending = false;
            progress = 0;
            lastProgress = null;

            progressThread = new Thread(new ThreadStart(delegate ()
            {
                rate = 5;
                while (progress < 100)
                {
                    progress += rate;
                    if (progress > 100) progress = 100;
                    
                    if (!ending)
                        rate = ((double)(100 - progress) / 10);

                    int rprog = (int)Math.Floor(progress);
                    int jx = (int)Math.Floor((float)rprog / 5);

                    string sprog = rprog + "%";
                    if (Math.Round(progress, 3) == 99.999) sprog = "99.999~%";
                    else if (rprog == 99) sprog = Math.Round(progress, 3) + "%";

                    if (sprog == "100%") sprog = "99.999999~%";

                    if (sprog != lastProgress)
                    {
                        StringBuilder sb = new StringBuilder();
                        for (int j = 0; j < jx; j++) sb.Append("#");
                        for (int j = 0; j < (20 - jx); j++) sb.Append(" ");

                        sb.Append(" ");
                        sb.Append(sprog);
                     

                        ClearLine();
                        Console.Write(sb.ToString());

                        lastProgress = sprog;
                    }

                    Thread.Sleep(100);
                }

                ClearLine();
                Console.WriteLine("> Done");
            }));
            progressThread.Start();
        }

        private static void EndProgress()
        {
            rate = 10;
            ending = true;
        }

        public static string GetAppsServiceDirectory()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles), "Aphysoft", "Apps", "Service");
        }

        private static ServiceController GetAppsService()
        {
            foreach (ServiceController svc in ServiceController.GetServices()) if (svc.ServiceName == "Aphysoft.Apps.Service") return svc;
            return null;
        }

        private static void AppsService()
        {
            // file apps
            string sofile = Assembly.GetEntryAssembly().Location;

            // service directory
            string asdir = GetAppsServiceDirectory();
            if (!Directory.Exists(asdir)) Directory.CreateDirectory(asdir);

            int update = 0;

            // check apps.exe file in service directory
            string asfile = Path.Combine(asdir, "apps.exe");
            if (!File.Exists(asfile)) File.Copy(sofile, asfile, true);
            else
            {
                FileInfo asfi = new FileInfo(asfile);
                FileInfo sofi = new FileInfo(sofile);

                if (IO.Hash(asfile) != IO.Hash(sofile))
                {
                    if (asfi.LastWriteTimeUtc < sofi.LastWriteTimeUtc) update = -1;
                    else if (asfi.LastWriteTimeUtc > sofi.LastWriteTimeUtc) update = 1;
                }
            }

            // check service installation
            ServiceController appsService = GetAppsService();
            if (appsService != null)
            {
                string sepath = GetServiceExecutablePath(GetManagementObject(appsService));
                if (sepath != asfile)
                {
                    Stop(appsService);
                    Uninstall(sepath);
                    appsService = null;
                }
            }
            if (appsService == null)
            {
                if (update == -1) File.Copy(sofile, asfile, true);
                Install(asfile);
                appsService = GetAppsService();
            }
            else
            {
                if (update == -1)
                {
                    Stop(appsService);
                    File.Copy(sofile, asfile, true);
                }
            }

            // update local apps
            if (update == 1)
            {
                File.Copy(asfile, sofile + "_UPDATE", true);

                // TODO
            }

            Start(appsService);
        }    

        private static void DeleteAppsService()
        {
            // file apps
            string sofile = Assembly.GetEntryAssembly().Location;

            string asdir = GetAppsServiceDirectory();
            string asfile = Path.Combine(asdir, "apps.exe");

            ServiceController appsService = GetAppsService();

            if (appsService != null)
            {
                Stop(appsService);

                string sepath = GetServiceExecutablePath(GetManagementObject(appsService));
                if (sepath != asfile) Uninstall(sepath);
                else
                {
                    if (!File.Exists(asfile))
                    {
                        if (!Directory.Exists(asdir)) Directory.CreateDirectory(asdir);
                        File.Copy(sofile, asfile);
                    }
                    Uninstall(asfile);
                }
            }

            if (File.Exists(asfile)) File.Delete(asfile);
        }

        private static void Stop(ServiceController service)
        {
            if (service.Status == ServiceControllerStatus.Running)
            {
                //Console.WriteLine(" Stopping " + service.DisplayName);
                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped);
                //Console.WriteLine(" " + service.DisplayName + " Stopped");
            }
        }

        private static void Start(ServiceController service)
        {
            if (service.Status == ServiceControllerStatus.Stopped)
            {
                //Console.WriteLine(" Starting " + service.DisplayName);
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running);
                //Console.WriteLine(" " + service.DisplayName + " Started");
            }
        }
        
        public static ManagementObject GetManagementObject(ServiceController service)
        {
            ManagementObject wmiService = new ManagementObject("Win32_Service.Name='" + service.ServiceName + "'");
            wmiService.Get();

            return wmiService;
        }

        public static string GetServiceExecutablePath(ManagementObject wmiService)
        {
            string call = wmiService["PathName"].ToString();
            string callFile = null;

            if (!string.IsNullOrEmpty(call))
            {
                if (call[0] == '"')
                {
                    string[] callTokens = call.Split('"');
                    callFile = callTokens[1];
                }
                else
                    callFile = call;               
            }

            return callFile;
        }        

        private static int ReadSelect(int max)
        {
            ConsoleKeyInfo key;
            int select = -1;

            while (true)
            {
                key = Console.ReadKey();
                Console.SetCursorPosition(0, Console.CursorTop);
                for (int i = 0; i < Console.BufferWidth - 1; i++) Console.Write(' ');
                Console.SetCursorPosition(0, Console.CursorTop);

                if (char.IsDigit(key.KeyChar))
                {
                    int dig = int.Parse(key.KeyChar + "");

                    if (dig > 0 && dig <= max)
                    {
                        select = dig - 1;
                        break;
                    }
                }
            }

            return select;
        }

        public static void CheckAppsService(out ServiceController appsService, out Dictionary<string, Tuple<ServiceController, string, string>> appsMemberServices)
        {
            appsService = null;
            appsMemberServices = new Dictionary<string, Tuple<ServiceController, string, string>>();

            foreach (ServiceController svc in ServiceController.GetServices())
            {
                if (svc.ServiceName == "Aphysoft.Apps.Service")
                    appsService = svc;
                else if (svc.ServiceName.StartsWith("Aphysoft.Apps."))
                {
                    ManagementObject wmiService = GetManagementObject(svc);

                    appsMemberServices.Add(svc.ServiceName, new Tuple<ServiceController, string, string>(
                        svc, GetServiceExecutablePath(wmiService), svc.DisplayName));
                }
            }
        }

        private static void CopyInstallation(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                return;
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                // skip here
                if (file.Name == "apps.exe" ||
                    file.Name.EndsWith(".pdb")) continue;

                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    CopyInstallation(subdir.FullName, temppath, copySubDirs);
                }
            }
        }

        private static void Uninstall(string path)
        {
            Process process = new Process { StartInfo = new ProcessStartInfo() };
            process.StartInfo.Arguments = "-u \"" + path + "\"";
            process.StartInfo.FileName = "apps.exe";
            process.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.WaitForExit();
        }

        private static void Install(string path)
        {
            Process process = new Process { StartInfo = new ProcessStartInfo() };
            process.StartInfo.Arguments = "-i \"" + path + "\"";
            process.StartInfo.FileName = "apps.exe";
            process.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            process.WaitForExit();
        }
    }

    internal class AppsService : ServiceBase
    {
        private NamedPipeServerStream pipe = null;
        private Thread thread = null;

        public AppsService()
        {
        }

        protected override void OnStart(string[] args)
        {
            thread = new Thread(new ThreadStart(delegate ()
            {
                PipeSecurity ps = new PipeSecurity();
                SecurityIdentifier sid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
                PipeAccessRule par = new PipeAccessRule(sid, PipeAccessRights.ReadWrite, AccessControlType.Allow);
                ps.AddAccessRule(par);

                pipe = new NamedPipeServerStream("apps", PipeDirection.InOut, 1, PipeTransmissionMode.Byte, PipeOptions.Asynchronous, 8192, 8192, ps);

                while (true)
                {
                    try
                    {
                        pipe.WaitForConnection();

                        int fb = pipe.ReadByte();
                        if (fb == 0) break;

                        int len = pipe.ReadByte() * 256;
                        len += pipe.ReadByte();

                        if (len > 0)
                        {
                            byte[] buffer = new byte[len];
                            pipe.Read(buffer, 0, len);

                            string data = Encoding.ASCII.GetString(buffer);
                            string resp = "NONE";

                            if (data.Length > 1)
                            {
                                char t = data[0];
                                string point = data.Substring(1);

                                if (t == 'R')
                                {
                                    Program.CheckAppsService(out ServiceController appsService, out Dictionary<string, Tuple<ServiceController, string, string>> appsMemberServices);

                                    if (appsMemberServices.ContainsKey(point))
                                    {
                                        ServiceController svc = appsMemberServices[point].Item1;
                                        string exepath = appsMemberServices[point].Item2;

                                        string servpath = Program.GetAppsServiceDirectory();
                                        string dirpath = (new FileInfo(exepath)).Directory.FullName;
                                        string updatepath = Path.Combine(dirpath, "_UPDATE");
                                        string tempupdatepath = Path.Combine(servpath, point + "_UPDATE");
                                        string existingconfig = Path.Combine(dirpath, "config");
                                        string tempupdateconfig = Path.Combine(servpath, point + "_config");

                                        if (svc.Status == ServiceControllerStatus.Running)
                                        {
                                            svc.Stop();
                                            svc.WaitForStatus(ServiceControllerStatus.Stopped);
                                            Thread.Sleep(3000);
                                        }

                                        bool restart = false;

                                        if (Directory.Exists(updatepath))
                                        {
                                            try
                                            {
                                                // were updating
                                                Directory.Move(updatepath, tempupdatepath);
                                                // copy existing config
                                                File.Move(existingconfig, tempupdateconfig);
                                                // delete existing directory
                                                Directory.Delete(dirpath, true);
                                                // rename update
                                                Directory.Move(tempupdatepath, dirpath);
                                                // put back config
                                                File.Move(tempupdateconfig, existingconfig);

                                                resp = "UPDATED";
                                            }
                                            catch (Exception ex)
                                            {
                                                resp = "ERROR";
                                            }
                                        }
                                        else
                                        {
                                            restart = true;
                                        }

                                        svc.Start();
                                        svc.WaitForStatus(ServiceControllerStatus.Running);

                                        if (restart)
                                            resp = "RESTARTED";

                                    }
                                    else
                                        resp = "NOTFOUND";
                                }
                            }

                            byte[] respBuffer = Encoding.ASCII.GetBytes(resp);
                            int slen = respBuffer.Length;
                            if (slen > UInt16.MaxValue) slen = (int)UInt16.MaxValue;

                            StreamWriter fs = File.CreateText("C:\\log");
                            fs.WriteLine($"{DateTime.Now.ToLongTimeString()} Incoming {data[0]} and data {data.Substring(1)}, with resp {resp}");
                            fs.WriteLine($"Meanwhile: slen {slen}");
                            fs.Close();

                            pipe.WriteByte((byte)(slen / 256));
                            pipe.WriteByte((byte)(slen & 255));
                            pipe.Write(respBuffer, 0, slen);
                        }

                        pipe.Disconnect();
                    }
                    catch (Exception ex)
                    {
                        break;
                    }
                }

            }));
            thread.Start();
        }

        protected override void OnStop()
        {
            NamedPipeClientStream spipe = new NamedPipeClientStream(".", "apps", PipeDirection.InOut);
            spipe.Connect();
            byte[] buffer = new byte[1]{ (byte)0 };
            spipe.Write(buffer, 0, 1);
            spipe.Flush();
            spipe.Close();
            
            pipe.Close();
            thread.Join();
        }
    }

    [RunInstaller(true)]
    public class ProjectInstaller : Installer
    {
        public ProjectInstaller()
        {
            ServiceProcessInstaller serviceProcessInstaller = new ServiceProcessInstaller();
            ServiceInstaller serviceInstaller = new ServiceInstaller();

            serviceProcessInstaller.Account = ServiceAccount.LocalSystem;
            serviceProcessInstaller.Password = null;
            serviceProcessInstaller.Username = null;

            serviceInstaller.Description = "Aphysoft Apps Service provides support for Aphysoft Apps powered services. If this service is stopped, most of Aphysoft Apps services functionality in this machine will not work.";
            serviceInstaller.DisplayName = "Aphysoft Apps Service";
            serviceInstaller.ServiceName = "Aphysoft.Apps.Service";
            serviceInstaller.StartType = ServiceStartMode.Automatic;
            serviceInstaller.DelayedAutoStart = true;

            Installers.AddRange(new Installer[] {
                    serviceProcessInstaller,
                    serviceInstaller
                    });
        }
    }

    // Share/Util/IO.cs
    public static class IO
    {
        public delegate void FileStreamEventHandler(FileStream stream);

        public delegate void FileStreamLinesEventHandler(List<string> lines);

        public static bool HasWriteAccessToDirectory(string directoryPath)
        {
            try
            {
                // TODO

                System.Security.AccessControl.DirectorySecurity ds = Directory.GetAccessControl(directoryPath);
                return true;

            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        public static void AllowEveryoneAllAccess(string path)
        {
            if (IsDirectory(path))
            {
                DirectoryInfo dInfo = new DirectoryInfo(path);

                DirectorySecurity dSecurity = dInfo.GetAccessControl();
                dSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.ObjectInherit | InheritanceFlags.ContainerInherit, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                dInfo.SetAccessControl(dSecurity);

                foreach (DirectoryInfo di in dInfo.GetDirectories())
                {
                    AllowEveryoneAllAccess(di.FullName);
                }
            }
            else
            {
                FileInfo fInfo = new FileInfo(path);

                FileSecurity fSecurity = fInfo.GetAccessControl();
                fSecurity.AddAccessRule(new FileSystemAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), FileSystemRights.FullControl, InheritanceFlags.None, PropagationFlags.NoPropagateInherit, AccessControlType.Allow));
                fInfo.SetAccessControl(fSecurity);
            }
        }

        public static bool IsDirectory(string path)
        {
            FileAttributes attr = File.GetAttributes(path);

            if (attr.HasFlag(FileAttributes.Directory))
                return true;
            else
                return false;
        }

        public static bool IsFile(string path)
        {
            return !IsDirectory(path);
        }

        public static void ExclusiveFileOpen(string path, FileStreamEventHandler handler)
        {
            FileStream stream = null;

            while (stream == null)
            {
                try
                {
                    stream = File.Open(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                }
                catch (IOException ex)
                {
                    Thread.Sleep(100);
                }
            }

            if (stream != null)
            {
                handler(stream);

                stream.Close();
                stream.Dispose();
            }
        }

        public static void ExclusiveFileOpen(string path, FileStreamLinesEventHandler handler)
        {
            ExclusiveFileOpen(path, delegate (FileStream fs)
            {
                byte[] array = new byte[fs.Length];
                fs.Read(array, 0, array.Length);

                int position = 0;

                List<string> lines = new List<string>();
                StringBuilder sb = new StringBuilder();

                int lineEnding = -1;

                // 0: CRLN
                // 1: CR
                // 2: LN

                while (position < array.Length)
                {
                    byte cb = array[position];
                    if (cb == '\r')
                    {
                        if ((position + 1) < array.Length)
                        {
                            byte nb = array[position + 1];
                            if (nb == '\n')
                            {
                                lines.Add(sb.ToString());
                                sb.Clear();

                                position++;

                                lineEnding = 0;
                            }
                            else
                            {
                                lineEnding = 1;
                            }
                        }
                    }
                    else if (cb == '\n')
                    {
                        lines.Add(sb.ToString());
                        sb.Clear();
                        lineEnding = 2;
                    }
                    else
                    {
                        sb.Append(Encoding.UTF8.GetString(cb.ToArray()));
                    }

                    position++;
                }

                if (sb.Length > 0)
                {
                    lines.Add(sb.ToString());
                }

                //List<string> original = new List<string>(lines);

                handler(lines);

                fs.SetLength(0);
                fs.Seek(0, SeekOrigin.Begin);

                string lineEndingString = null;

                if (lineEnding == 0) lineEndingString = "\r\n";
                else if (lineEnding == 1) lineEndingString = "\r";
                else lineEndingString = "\n";

                int lineIndex = 0;
                foreach (string line in lines)
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(line + (lineIndex < lines.Count - 1 ? lineEndingString : ""));

                    fs.Write(bytes, 0, bytes.Length);
                }
            });
        }

        public static FileInfo[] EnumerateFiles(DirectoryInfo parent)
        {
            List<FileInfo> list = new List<FileInfo>();

            foreach (DirectoryInfo di in parent.GetDirectories())
            {
                list.AddRange(EnumerateFiles(di));
            }

            foreach (FileInfo fi in parent.GetFiles())
            {
                list.Add(fi);
            }

            return list.ToArray();
        }

        public static FileInfo[] EnumerateFiles(string path)
        {
            return EnumerateFiles(new DirectoryInfo(path));
        }

        private static MD5 md5 = null;

        public static string Hash(string path)
        {
            if (md5 == null) md5 = MD5.Create();

            string hashstring = null;
            using (FileStream stream = File.OpenRead(path))
            {
                hashstring = md5.ComputeHash(stream).ToHex();
            }

            return hashstring;
        }

        public static string Hash(FileStream stream)
        {
            if (md5 == null) md5 = MD5.Create();
            if (stream == null) return null;

            return md5.ComputeHash(stream).ToHex();
        }
    }

    // Share/Extensions/Byte.cs
    public static class ByteExtensions
    {
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int memcmp(byte[] b1, byte[] b2, UIntPtr count);

        public static bool SequenceEqual(this byte[] b1, byte[] to)
        {
            if (b1 == to) return true; //reference equality check

            if (b1 == null || to == null || b1.Length != to.Length) return false;

            return memcmp(b1, to, new UIntPtr((uint)b1.Length)) == 0;
        }

        public static bool StartsWith(this byte[] b1, byte[] with)
        {
            byte[] compare = new byte[with.Length];
            Buffer.BlockCopy(b1, 0, compare, 0, with.Length);

            return compare.SequenceEqual(with);
        }

        public static string ToHex(this byte[] value)
        {
            var hex = new StringBuilder(value.Length * 2);
            foreach (byte b in value)
            {
                hex.AppendFormat("{0:x2}", b);
            }

            return hex.ToString();
        }

        public static void Random(this byte[] b)
        {
            Rnd.Bytes(b);
        }

        public static byte[] ToArray(this byte b)
        {
            return new[] { b };
        }
    }

    // Share/Util/Rnd.cs
    public static class Rnd
    {
        [ThreadStatic]
        private static System.Random random;

        private static System.Random Seed
        {
            get { return random ?? (random = new System.Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
        }

        public static int Int()
        {

            return Seed.Next();
        }

        public static int Natural()
        {
            return Seed.Next(0, int.MaxValue);
        }

        public static int Int(int maxValue)
        {
            return Seed.Next(maxValue);
        }

        public static int Int(int minValue, int maxValue)
        {
            return Seed.Next(minValue, maxValue);
        }

        public static double Double()
        {
            return Seed.NextDouble();
        }

        public static void Bytes(byte[] buffer)
        {
            Seed.NextBytes(buffer);
        }
    }

    // Share/Util/Terminal.cs
    public static class Terminal
    {
        public static string ReadLine(CancellationToken cancellationToken)
        {
            StringBuilder stringBuilder = new StringBuilder();
            Task.Run(() =>
            {
                try
                {
                    ConsoleKeyInfo keyInfo;
                    var startingLeft = System.Console.CursorLeft;
                    var startingTop = System.Console.CursorTop;
                    var currentIndex = 0;
                    do
                    {
                        var previousLeft = System.Console.CursorLeft;
                        var previousTop = System.Console.CursorTop;
                        while (!System.Console.KeyAvailable)
                        {
                            cancellationToken.ThrowIfCancellationRequested();
                            Thread.Sleep(50);
                        }
                        keyInfo = System.Console.ReadKey();
                        switch (keyInfo.Key)
                        {
                            case ConsoleKey.A:
                            case ConsoleKey.B:
                            case ConsoleKey.C:
                            case ConsoleKey.D:
                            case ConsoleKey.E:
                            case ConsoleKey.F:
                            case ConsoleKey.G:
                            case ConsoleKey.H:
                            case ConsoleKey.I:
                            case ConsoleKey.J:
                            case ConsoleKey.K:
                            case ConsoleKey.L:
                            case ConsoleKey.M:
                            case ConsoleKey.N:
                            case ConsoleKey.O:
                            case ConsoleKey.P:
                            case ConsoleKey.Q:
                            case ConsoleKey.R:
                            case ConsoleKey.S:
                            case ConsoleKey.T:
                            case ConsoleKey.U:
                            case ConsoleKey.V:
                            case ConsoleKey.W:
                            case ConsoleKey.X:
                            case ConsoleKey.Y:
                            case ConsoleKey.Z:
                            case ConsoleKey.Spacebar:
                            case ConsoleKey.Decimal:
                            case ConsoleKey.Add:
                            case ConsoleKey.Subtract:
                            case ConsoleKey.Multiply:
                            case ConsoleKey.Divide:
                            case ConsoleKey.D0:
                            case ConsoleKey.D1:
                            case ConsoleKey.D2:
                            case ConsoleKey.D3:
                            case ConsoleKey.D4:
                            case ConsoleKey.D5:
                            case ConsoleKey.D6:
                            case ConsoleKey.D7:
                            case ConsoleKey.D8:
                            case ConsoleKey.D9:
                            case ConsoleKey.NumPad0:
                            case ConsoleKey.NumPad1:
                            case ConsoleKey.NumPad2:
                            case ConsoleKey.NumPad3:
                            case ConsoleKey.NumPad4:
                            case ConsoleKey.NumPad5:
                            case ConsoleKey.NumPad6:
                            case ConsoleKey.NumPad7:
                            case ConsoleKey.NumPad8:
                            case ConsoleKey.NumPad9:
                            case ConsoleKey.Oem1:
                            case ConsoleKey.Oem102:
                            case ConsoleKey.Oem2:
                            case ConsoleKey.Oem3:
                            case ConsoleKey.Oem4:
                            case ConsoleKey.Oem5:
                            case ConsoleKey.Oem6:
                            case ConsoleKey.Oem7:
                            case ConsoleKey.Oem8:
                            case ConsoleKey.OemComma:
                            case ConsoleKey.OemMinus:
                            case ConsoleKey.OemPeriod:
                            case ConsoleKey.OemPlus:
                                stringBuilder.Insert(currentIndex, keyInfo.KeyChar);
                                currentIndex++;
                                if (currentIndex < stringBuilder.Length)
                                {
                                    var left = System.Console.CursorLeft;
                                    var top = System.Console.CursorTop;
                                    System.Console.Write(stringBuilder.ToString().Substring(currentIndex));
                                    System.Console.SetCursorPosition(left, top);
                                }
                                break;
                            case ConsoleKey.Backspace:
                                if (currentIndex > 0)
                                {
                                    currentIndex--;
                                    stringBuilder.Remove(currentIndex, 1);
                                    var left = System.Console.CursorLeft;
                                    var top = System.Console.CursorTop;
                                    if (left == previousLeft)
                                    {
                                        left = System.Console.BufferWidth - 1;
                                        top--;
                                        System.Console.SetCursorPosition(left, top);
                                    }
                                    System.Console.Write(stringBuilder.ToString().Substring(currentIndex) + " ");
                                    System.Console.SetCursorPosition(left, top);
                                }
                                else
                                {
                                    //System.Console.SetCursorPosition(startingLeft, startingTop);
                                }
                                break;
                            case ConsoleKey.Delete:
                                if (stringBuilder.Length > currentIndex)
                                {
                                    stringBuilder.Remove(currentIndex, 1);
                                    System.Console.SetCursorPosition(previousLeft, previousTop);
                                    System.Console.Write(stringBuilder.ToString().Substring(currentIndex) + " ");
                                    System.Console.SetCursorPosition(previousLeft, previousTop);
                                }
                                else
                                    System.Console.SetCursorPosition(previousLeft, previousTop);
                                break;
                            case ConsoleKey.LeftArrow:
                                if (currentIndex > 0)
                                {
                                    currentIndex--;
                                    var left = System.Console.CursorLeft - 2;
                                    var top = System.Console.CursorTop;
                                    if (left < 0)
                                    {
                                        left = System.Console.BufferWidth + left;
                                        top--;
                                    }
                                    System.Console.SetCursorPosition(left, top);
                                    if (currentIndex < stringBuilder.Length - 1)
                                    {
                                        System.Console.Write(stringBuilder[currentIndex].ToString() + stringBuilder[currentIndex + 1]);
                                        System.Console.SetCursorPosition(left, top);
                                    }
                                }
                                else
                                {
                                    System.Console.SetCursorPosition(startingLeft, startingTop);
                                    if (stringBuilder.Length > 0)
                                        System.Console.Write(stringBuilder[0]);
                                    System.Console.SetCursorPosition(startingLeft, startingTop);
                                }
                                break;
                            case ConsoleKey.RightArrow:
                                if (currentIndex < stringBuilder.Length)
                                {
                                    System.Console.SetCursorPosition(previousLeft, previousTop);
                                    System.Console.Write(stringBuilder[currentIndex]);
                                    currentIndex++;
                                }
                                else
                                {
                                    System.Console.SetCursorPosition(previousLeft, previousTop);
                                }
                                break;
                            case ConsoleKey.Home:
                                if (stringBuilder.Length > 0 && currentIndex != stringBuilder.Length)
                                {
                                    System.Console.SetCursorPosition(previousLeft, previousTop);
                                    System.Console.Write(stringBuilder[currentIndex]);
                                }
                                System.Console.SetCursorPosition(startingLeft, startingTop);
                                currentIndex = 0;
                                break;
                            case ConsoleKey.End:
                                if (currentIndex < stringBuilder.Length)
                                {
                                    System.Console.SetCursorPosition(previousLeft, previousTop);
                                    System.Console.Write(stringBuilder[currentIndex]);
                                    var left = previousLeft + stringBuilder.Length - currentIndex;
                                    var top = previousTop;
                                    while (left > System.Console.BufferWidth)
                                    {
                                        left -= System.Console.BufferWidth;
                                        top++;
                                    }
                                    currentIndex = stringBuilder.Length;
                                    System.Console.SetCursorPosition(left, top);
                                }
                                else
                                    System.Console.SetCursorPosition(previousLeft, previousTop);
                                break;
                            default:
                                System.Console.SetCursorPosition(previousLeft, previousTop);
                                break;
                        }
                    } while (keyInfo.Key != ConsoleKey.Enter);
                    System.Console.WriteLine();
                }
                catch
                {
                    //MARK: Change this based on your need. See description below.
                    stringBuilder.Clear();
                }
            }).Wait();
            return stringBuilder.ToString();
        }
    }
}
