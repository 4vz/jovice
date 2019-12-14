using Aphysoft.Share;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Athena
{
    public class Athena : Node
    {
        #region Static

        public static Athena Instance { get; set; }

        public Database2 Database { get; private set; } = null;

        private Dictionary<string, InstallationFile[]> installations = null;

        private Dictionary<string, Tuple<DateTime>> installationInformations = null;

        private InstallationFile appsInstallationFile = null;

        private string deployApps = null;

        private int deployReference = -1;

        private Dictionary<string, int> onProgressInstallation = null;
        
        #endregion

        #region Constructors

        /// <summary>
        /// Whats Athena Do:
        /// 1. Communication to chat services: Telegram, WhatsApp, etc
        /// 2. Give auto update to connected Apps
        /// 3. Monitor apps status
        /// </summary>
        public Athena() : base("ATHENA")
        {            
        }

        #endregion

        protected override void OnStart()
        {
            Instance = this;

            Event("Checking database connection...");
            Database = Database2.Web();

            if (Database)
            {
                Event("Database connection OK");
                
                string store = Path.Combine(Directory, "store");

                DirectoryInfo di = new DirectoryInfo(store);
                if (!di.Exists) di.Create();

                DirectoryInfo[] dis = di.GetDirectories();
                installations = new Dictionary<string, InstallationFile[]>();
                installationInformations = new Dictionary<string, Tuple<DateTime>>();
                onProgressInstallation = new Dictionary<string, int>();

                foreach (DirectoryInfo dix in dis)
                {
                    DoVersion(dix.Name);
                }
                
                BeginAcceptEdges();

                //Messenger.Init();






                //Event("RETURN:" + Apps.ServiceCommand(ServiceCommandType.Restart, "Aphysoft.Apps.test"));

                StandBy();
            }
        }

        protected override void OnStop()
        {            
        }
        
        private void SendUpdateInformation(Edge edge)
        {
            string name = edge.Name;

            if (deployApps == name) return;

            if (installations.ContainsKey(name))
            {
                Event("Sending update information...");

                if (edge.Send(new InfoInstallMessage { Files = installations[name], Apps = appsInstallationFile }, out ReportInstallMessage resp))
                {
                    if (resp.RequestID != null)
                    {
                        Event("Edge is requesting updates");

                        if (edge.IsApps)
                        {
                            foreach (int rid in resp.RequestID)
                            {
                                if (rid == appsInstallationFile.ID)
                                {
                                    // request update for apps.ID
                                    string pathToFile = Path.Combine(Directory, "store", "APPS", "apps.exe");

                                    FileStream fs = new FileStream(pathToFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                                    edge.Send(fs, appsInstallationFile.ID);
                                    fs.Close();
                                }
                                else
                                {
                                    foreach (InstallationFile file in installations[name])
                                    {
                                        if (file.ID == rid)
                                        {
                                            string pathToFile = Path.Combine(Directory, "store", name, file.Name);

                                            FileStream fs = new FileStream(pathToFile, FileMode.Open, FileAccess.Read, FileShare.Read);
                                            edge.Send(fs, file.ID);
                                            fs.Close();

                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            Event("Edge is not an apps");
                        }

                    }
                    else
                    {
                        Event("Edge up to date");
                    }
                }
            }
        }

        protected override void OnConnected(EdgeConnectedEventArgs e)
        {
            Event("Edge Connected");

            Edge edge = e.Edge;

            SendUpdateInformation(edge);

            if (edge.Name == "ATHENADEPLOY")
            {
                Event("Sending configuration information...");

                e.Edge.Send(new AthenaConfigurationMessage { Installations = installations, Informations = installationInformations });
            }
        }

        protected override void OnMessage(EdgeMessageEventArgs e)
        {
            if (e.Edge.Name == "ATHENADEPLOY")
            {
                if (e.Message is AthenaDeployMessage)
                {
                    AthenaDeployMessage dm = e.Message as AthenaDeployMessage;

                    deployApps = dm.Apps;
                    deployReference = dm.Reference;

                    string dirpath = Path.Combine(Directory, "store", dm.Apps);

                    e.ReplyOK();
                }
            }
        }

        protected override void OnFileReceived(EdgeFileEventArgs e)
        {
            if (deployApps != null && e.Reference == deployReference)
            {
                Event($"Incoming Deploy Archive: {deployApps}");

                string isi = Path.Combine(Directory, "incoming.zip");

                FileStream fs = File.Create(isi);
                e.Stream.CopyTo(fs);
                fs.Close();

                string appi = Path.Combine(Directory, "store", deployApps);
                DirectoryInfo di = new DirectoryInfo(appi);

                if (di.Exists)
                {
                    foreach (FileInfo fi in di.GetFiles())
                    {
                        fi.Delete();
                    }
                }
                else
                {
                    di.Create();
                }

                ZipFile.ExtractToDirectory(isi, appi);
                File.Delete(isi);

                string deployData = $"{DateTime.UtcNow.ToBinary()}";
                StreamWriter sw = File.CreateText(Path.Combine(appi, "deploy"));
                sw.Write(deployData);
                sw.Close();

                DoVersion(deployApps);

                string deployedApps = deployApps;
                deployApps = null;

                Event($"Deployed. Notice connected edges for new update.");

                int nedge = 0;

                foreach (Edge edge in Edges)
                {
                    if (edge.Name == deployedApps)
                    {
                        nedge++;
                        SendUpdateInformation(edge);
                    }
                }
                
                Event($"{nedge} edges noticed");
            }
        }

        private void DoVersion(string name)
        {
            string dirpath = Path.Combine(Directory, "store", name);

            if (name.ToUpper() == "APPS")
            {
                if (name != "APPS") // not all uppercase
                {
                    // fix this shit
                    string appsExeThere = Path.Combine(dirpath, "apps.exe");

                    string returnFileFromTemp = null;

                    if (File.Exists(appsExeThere))
                    {
                        returnFileFromTemp = Path.GetTempFileName();
                        File.Copy(appsExeThere, returnFileFromTemp, true);
                    }

                    System.IO.Directory.Delete(dirpath, true);

                    dirpath = Path.Combine(Directory, "store", "APPS");
                    System.IO.Directory.CreateDirectory(dirpath);

                    appsExeThere = Path.Combine(dirpath, "apps.exe");

                    if (returnFileFromTemp != null)
                    {
                        File.Move(returnFileFromTemp, appsExeThere);
                    }

                    name = "APPS";
                }

                string appsExe = Path.Combine(dirpath, "apps.exe");
                FileInfo appsFI = new FileInfo(appsExe);

                if (appsFI.Exists)
                {
                    appsInstallationFile = new InstallationFile("apps.exe", appsFI.Length, IO.Hash(appsExe), appsFI.LastWriteTimeUtc);
                }
            }
            else
            {
                List<InstallationFile> files = new List<InstallationFile>();

                string appsDir = Path.Combine(Directory, "store", "APPS");

                if (!System.IO.Directory.Exists(appsDir))
                    System.IO.Directory.CreateDirectory(appsDir);

                string appsExe = Path.Combine(Directory, "store", "APPS", "apps.exe");
                string configFile = Path.Combine(dirpath, "config");

                string deployData = null;

                foreach (FileInfo fi in IO.EnumerateFiles(dirpath))
                {
                    if (fi.Extension == ".pdb" ||
                        fi.Extension == ".apps" ||
                        fi.Name == "apps.exe_UPDATE" ||
                        fi.Name == "appsl.exe" ||
                        fi.FullName == configFile)
                    {
                        fi.Delete();
                        continue;
                    }

                    string hash = IO.Hash(fi.FullName);

                    if (fi.Name == "apps.exe")
                    {
                        bool gogogo = false;

                        if (appsInstallationFile != null)
                        {
                            // compare with existing
                            if (appsInstallationFile.Hash != hash)
                            {
                                if (appsInstallationFile.TimeStamp < fi.LastWriteTimeUtc)
                                {
                                    // ok update
                                    gogogo = true;
                                }
                            }
                        }
                        else
                            gogogo = true;

                        if (gogogo)
                        {
                            File.Copy(fi.FullName, appsExe, true);

                            FileInfo appsFI = new FileInfo(appsExe);
                            appsInstallationFile = new InstallationFile("apps.exe", appsFI.Length, IO.Hash(appsExe), appsFI.LastWriteTimeUtc);
                        }

                        File.Delete(fi.FullName);
                    }
                    else
                    {
                        if (fi.Name == "deploy")
                        {
                            // deploy informations
                            deployData = File.ReadAllText(fi.FullName);
                        }
                        else
                        {
                            files.Add(new InstallationFile(fi.FullName.Substring(dirpath.Length + 1), fi.Length, hash, fi.LastWriteTimeUtc));
                        }
                    }
                }

                if (deployData == null)
                {
                    deployData = $"{DateTime.MinValue.ToBinary()}";

                    StreamWriter sw = File.CreateText(Path.Combine(dirpath, "deploy"));
                    sw.Write(deployData);
                    sw.Close();
                }
                
                if (deployData != null)
                {
                    string[] sss = deployData.Split(new[] { ';' });

                    if (sss.Length == 1)
                    {
                        DateTime xa = DateTime.MinValue;

                        if (long.TryParse(sss[0], out long dbin))
                        {
                            xa = DateTime.FromBinary(dbin);
                        }
                        else
                            Debug($"{name}: Fail to parse binary deploy time (string: {sss[0]})");

                        if (installationInformations.ContainsKey(name)) installationInformations.Remove(name);
                        installationInformations.Add(name, new Tuple<DateTime>(xa));
                    }
                }

                if (installations.ContainsKey(name)) installations.Remove(name);
                installations.Add(name, files.ToArray());
            }
        }
    }

    public class AthenaDeploy : Node
    {
        #region Fields

        private Edge athenaEdge = null;

        private string mode = null;

        private string configurationPath = null;

        private Dictionary<string, string> appsLocalDeploy = new Dictionary<string, string>();
        private Dictionary<string, (string Url, string Server, string Dir, string User, string Password)> appsGitDeploy = new Dictionary<string, (string Url, string Server, string Dir, string User, string Password)>();

        private string solutionDir = null;

        #endregion

        #region Constructors

        public AthenaDeploy(string configurationPath) : base("ATHENADEPLOY", false)
        {
            this.configurationPath = configurationPath;
        }

        #endregion

        #region Methods

        protected override void OnStart()
        {
            Apps.Console("Athena.Deploy");

            string server = null;

#if DEBUG
            mode = "Debug";
#else
            mode = "Release";
#endif


            Apps.Console($"Configuration: {configurationPath}");

            FileInfo fcp = new FileInfo(configurationPath);

            if (fcp.Exists)
            {
                solutionDir = fcp.DirectoryName;

                Apps.Console($"Solution Directory: {solutionDir}");

                string[] clines = File.ReadAllLines(configurationPath);

                foreach (string cline in clines)
                {
                    if (cline.StartsWith("ATHENA=") && server == null) server = cline.Substring(7);

                    int colon;
                    if ((colon = cline.IndexOf(':')) > -1)
                    {
                        string name = cline.Substring(0, colon);

                        if (cline.Length > (colon + 1))
                        {
                            string value = cline.Substring(colon + 1);

                            if ((colon = value.IndexOf(':')) > -1)
                            {
                                string dename = value.Substring(0, colon);

                                if (value.Length > (colon + 1))
                                {
                                    Dictionary<string, string> devalues = new Dictionary<string, string>();

                                    value = value.Substring(colon + 1);

                                    string[] pa = value.Split(StringSplitTypes.SemiColon, StringSplitOptions.RemoveEmptyEntries);

                                    foreach (string pax in pa)
                                    {
                                        int eq;

                                        if ((eq = pax.IndexOf('=')) > -1)
                                        {
                                            string paxkey = pax.Substring(0, eq);

                                            if (pax.Length > (eq + 1))
                                            {
                                                string paxvalue = pax.Substring(eq + 1);

                                                if (!devalues.ContainsKey(paxkey)) devalues.Add(paxkey, paxvalue);
                                                else devalues[paxkey] = paxvalue;                                                
                                            }
                                        }
                                    }

                                    if (dename == "ATHENA")
                                    {
                                        if (devalues.ContainsKey("LOCAL"))
                                        {
                                            string path = devalues["LOCAL"] + mode + "/";

                                            if (!appsLocalDeploy.ContainsKey(name))
                                            {
                                                appsLocalDeploy.Add(name, path);
                                            }
                                        }
                                    }
                                    else if (dename == "GITFTP")
                                    {
                                        if (!appsGitDeploy.ContainsKey(name))
                                        {
                                            string xgit = devalues.ContainsKey("GIT") ? devalues["GIT"] : null;
                                            string xserver = devalues.ContainsKey("SERVER") ? devalues["SERVER"] : null;
                                            string xdir = devalues.ContainsKey("DIR") ? devalues["DIR"] : null;
                                            string xuser = devalues.ContainsKey("USER") ? devalues["USER"] : null;
                                            string xpass = devalues.ContainsKey("PASS") ? devalues["PASS"] : null;

                                            if (!Is.NullOrEmpty(xgit, xserver, xdir, xuser, xpass))
                                                appsGitDeploy.Add(name, (xgit, xserver, xdir, xuser, xpass));
                                        }
                                    }

                                }
                            }




                        }
                    }



                }

                Apps.Console($"ATHENA Server: {server}");
                if (server != null)
                    Apps.Console($"{appsLocalDeploy.Count} ATHENA deployments");
                Apps.Console($"{appsGitDeploy.Count} GIT deployments");
                                
                athenaEdge = BeginEdge(server, "ATHENA");
                Apps.Console("Connecting...");
                StandBy();
            }
            else
            {

            }
        }

        protected override void OnStop()
        {
        }

        protected override void OnMessage(EdgeMessageEventArgs e)
        {
            if (e.Message is AthenaConfigurationMessage)
            {
                AthenaConfigurationMessage cm = e.Message as AthenaConfigurationMessage;

                int index = 1;

                List<string> appsIndex = new List<string>();

                foreach (KeyValuePair<string, InstallationFile[]> pair in cm.Installations)
                {
                    if (cm.Informations.ContainsKey(pair.Key))
                    {
                        DateTime dtimestamp = cm.Informations[pair.Key].Item1;
                        Apps.Console($"({index++}) {pair.Key}: {dtimestamp.ToLongDateString()} {dtimestamp.ToLongTimeString()}");

                        appsIndex.Add(pair.Key);

                        if (appsLocalDeploy.ContainsKey(pair.Key))
                        {
                            Apps.Console($"Local Directory: {appsLocalDeploy[pair.Key]}");
                        }
                        else
                        {
                            Apps.Console("Local Directory: none");
                        }
                    }
                }
                foreach (KeyValuePair<string, string> pair in appsLocalDeploy)
                {
                    if (!appsIndex.Contains(pair.Key))
                    {
                        appsIndex.Add(pair.Key);

                        Apps.Console($"({index++}) {pair.Key}: NOT DEPLOYED YET");
                        Apps.Console($"Local Directory: {pair.Value}");
                    }
                }
                foreach (KeyValuePair<string, (string Url, string Server, string Dir, string User, string Password)> pair in appsGitDeploy)
                {
                    appsIndex.Add(pair.Key);

                    Apps.Console($"({index++}) {pair.Key}");
                    Apps.Console($"{pair.Value.Url} to {pair.Value.Server}/{pair.Value.Dir}");
                }

                int selectedIndex = -1;

                while (IsRunning)
                {
                    Apps.Console($"Please enter the apps index you want to deploy (1-{index - 1}): ", false);
                    if (int.TryParse(Apps.ConsoleReadLine(), out int rel))
                    {
                        if (rel >= 1 && rel <= (index - 1))
                        {
                            selectedIndex = rel;
                            break;
                        }
                    }
                }

                if (selectedIndex > -1)
                {
                    string selectedApps = appsIndex[selectedIndex - 1];

                    if (appsLocalDeploy.ContainsKey(selectedApps))
                    {
                        Apps.Console($"Deploying {selectedApps}");

                        string deployDir = Path.Combine(solutionDir, appsLocalDeploy[selectedApps].Replace('/','\\'));
                        string zip = Path.Combine(solutionDir, selectedApps + ".zip");

                        if (File.Exists(zip)) File.Delete(zip);

                        Apps.Console($"Creating archive... {deployDir}");
                        ZipFile.CreateFromDirectory(deployDir, zip);

                        int reference = Rnd.Int();

                        AthenaDeployMessage dm = new AthenaDeployMessage(selectedApps, reference);

                        if (e.Edge.Send(dm, out Message rm))
                        {
                            if (rm is OKMessage)
                            {
                                FileStream fs = new FileStream(zip, FileMode.Open, FileAccess.Read, FileShare.Read);
                                Apps.Console($"Sending the archive to Athena... {fs.Length} bytes");

                                e.Edge.Send(fs, reference, delegate (EdgeProgessEventArgs ed)
                                {
                                    if (ed.Current == ed.Length && ed.Length > 0)
                                    {
                                        Apps.Console($"Sent.");
                                    }
                                });

                                fs.Close();
                            }
                        }

                        File.Delete(zip);
                    }
                    else if (appsGitDeploy.ContainsKey(selectedApps))
                    {
                        Apps.Console($"Deploying {selectedApps}");

                        (string Url, string Server, string Dir, string User, string Password) = appsGitDeploy[selectedApps];

                        Process.Start("git", "clone " + Url + " D:\\Temp").WaitForExit();

                        Apps.Console("Exited");

                        while (true)
                        {
                            Thread.Sleep(1000);
                        }


                        Apps.Console($"Done");
                    }
                    else
                    {
                        Apps.Console($"Please configure the local directory for this apps first.");
                        End(); 
                    }
                }
            
            }
        }

        protected override void OnConnected(EdgeConnectedEventArgs e)
        {
            Apps.Console("Connected");
        }

        protected override void OnDisconnected(EdgeDisconnectedEventArgs e, bool wasAuthenticated)
        {
            Apps.Console("Disconnected");
            End();
        }

        private void End()
        {
            Apps.ConsoleCancel();
            //Thread.Sleep(1000);
            //Console("Press Enter to exit");
            //while (ConsoleReadKey().Key != ConsoleKey.Enter);
            Stop();
        }

        #endregion
    }

    [Serializable]
    public class AthenaConfigurationMessage : BaseMessage
    {
        #region Fields

        public Dictionary<string, InstallationFile[]> Installations { get; set; }

        public Dictionary<string, Tuple<DateTime>> Informations { get; set; }

        #endregion

        #region Constructors

        public AthenaConfigurationMessage()
        {
            
        }

        #endregion

        #region Methods

        #endregion
    }

    [Serializable]
    public class AthenaDeployMessage : BaseMessage
    {
        #region Fields

        public string Apps { get; set; }

        public int Reference { get; set; }

        #endregion

        #region Constructors

        public AthenaDeployMessage(string apps, int reference)
        {
            Apps = apps;
            Reference = reference;
        }

        #endregion
    }
}
