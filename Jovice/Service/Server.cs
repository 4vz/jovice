using Aphysoft.Share;
using Microsoft.VisualBasic.Devices;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading;
using System.Web;

namespace Center
{
    public static class Server
    {
        #region Fields

        private readonly static PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        private readonly static PerformanceCounter ramCounter = new PerformanceCounter("Memory", "Available MBytes");
        private readonly static ComputerInfo computerInfo = new ComputerInfo();
        private readonly static ManagementObjectSearcher mos = new ManagementObjectSearcher("root\\CIMV2", "SELECT * FROM Win32_Processor");

        private static float cpu;
        private static double cpuUsage;
        private static float availableRam;
        private static float totalRam;
        private static float percentageRam;
        private static string cpuClass;

        public static float CPU { get { return cpu; } }
        public static double CPUUsage { get { return cpuUsage; } }
        public static float AvailableRam { get { return availableRam; } }
        public static float TotalRam { get { return totalRam; } }
        public static float PercentageRam { get { return percentageRam; } }
        public static string CPUClass { get { return cpuClass; } }

        private static Timer timer;

        #endregion

        #region Methods

        public static void Init()
        {
            Settings.ServerInit();
            //TelegramBot.Init();
            AutoCertRenew.Init();

            


            // service handler to WEB-CLIENT
            //Service.Register(typeof(ClientNecrowServiceMessage), ClientNecrowServiceMessageHandler);

            // service handler to NECROW
            //Service.Register(typeof(ServerNecrowServiceMessage), ServerNecrowServiceMessageHandler);     
            
            //Service.Disconnected += NecrowConnectionDisconnected;

            // Callback if register necrowa being registered to the server
            Provider.RegisterCallback("necrow", NecrowRegistered);

            totalRam = (float)Math.Round((double)(computerInfo.TotalPhysicalMemory / 1024000));

            #region Reading cpuClass
            string oname = null;
            int ocpu = 0;
            foreach (ManagementObject mo in mos.Get())
            {
                string cname = mo["Name"].ToString();
                ocpu += int.Parse(mo["NumberOfLogicalProcessors"].ToString());
                if (oname != null && cname != oname)
                {
                    cpuClass = cname + " " + ocpu + " Logical Processors";
                    oname = cname;
                    ocpu = 0;
                }
                else if (oname == null) oname = cname;
            }
            if (oname != null) cpuClass = oname + " " + ocpu + " Logical Processors";
            #endregion

            DateTime markCpuWarning = DateTime.MinValue;
            DateTime markCpuOkay = DateTime.MinValue;

            cpuCounter.NextValue(); // shift 1 cpu counter cycle

            timer = new Timer(new TimerCallback(delegate (object state)
            {                



                cpu = cpuCounter.NextValue();
                cpuCounter.NextValue(); // shift 1 cpu counter cycle
                cpuUsage = Math.Round(cpu, 2);
                availableRam = ramCounter.NextValue();
                percentageRam = 100 - (float)Math.Round(availableRam / totalRam * 100, 2);

                if (cpuUsage > 90)
                {
                    if (markCpuWarning == DateTime.MinValue)
                        markCpuWarning = DateTime.Now;
                    else if (DateTime.Now - markCpuWarning > TimeSpan.FromSeconds(60))
                    {
                        // this already existed for mor than 60 seconds, give warning, continuesly
                        //TelegramBot.CPUWarning();
                    }
                }
                else
                {
                    if (markCpuWarning > DateTime.MinValue)
                    {
                        if (markCpuOkay == DateTime.MinValue)
                            markCpuOkay = DateTime.Now;
                        else if (DateTime.Now - markCpuOkay > TimeSpan.FromSeconds(20))
                        {
                            // cpu is okay now
                            markCpuOkay = DateTime.MinValue;
                            markCpuWarning = DateTime.MinValue;
                            //TelegramBot.CPUOK();
                        }
                    }
                }

            }), null, 0, 10000);
        }

        #endregion

        #region Necrow

        private static Connection necrowConnection = null;

        internal static bool IsNecrowConnected()
        {
            return necrowConnection != null && necrowConnection.IsConnected;
        }

        private static void NecrowRegistered(MessageEventArgs e)
        {
            // Tell registerer about necrow availability status
            //Provider.SetActionByRegister("necrow");
        }

        private static void NecrowConnectionDisconnected(Connection connection)
        {
            if (necrowConnection != null)
            {
                if (connection == necrowConnection)
                {
                    necrowConnection = null;
                    Provider.SetActionByRegister("necrowavailability", "necrow", "offline");

                    TelegramBot.NecrowOffline();
                }
            }
        }

        #region Server to Necrow

        private static void ServerNecrowServiceMessageHandler(MessageEventArgs e)
        {
            ServerNecrowServiceMessage m = (ServerNecrowServiceMessage)e.Message;

            if (m.Type == NecrowServiceMessageType.Hello)
            {
                necrowConnection = e.Connection;
                NecrowSend(new ServerNecrowServiceMessage(NecrowServiceMessageType.HelloResponse));
                TelegramBot.NecrowOnline();

                //Provider.SetActionByRegisterMatch("service_[ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=]+", "yayaya");
                //Provider.SetActionByRegisterMatch("service", "service", "yayaya");
                //Provider.SetActionByRegister("necrowavailability", "necrow", "online");
            }
            else if (m.Type == NecrowServiceMessageType.ProbeStatus)
            {
                TelegramBot.NecrowProbeStatus(m);
            }
            else if (m.Type == NecrowServiceMessageType.Probe)
            {
                TelegramBot.NecrowProbe(m);
            }
        }

        public static void NecrowSend(ServerNecrowServiceMessage message)
        {
            if (IsNecrowConnected())
            {
                necrowConnection.Send(message);
            }
        }
        
        #endregion

        private static void ClientNecrowServiceMessageHandler(MessageEventArgs e)
        {
            ClientNecrowServiceMessage m = (ClientNecrowServiceMessage)e.Message;

            if (m.Type == ClientNecrowMessageTypes.IsNecrowAvailable)
            {
                bool av = IsNecrowConnected();
                if (av) m.Data = "online";
                else m.Data = "offline";
            }
            else if (m.Type == ClientNecrowMessageTypes.Ping)
            { 
            }

            e.Connection.Reply(m);
        }

        #endregion
    }

    [Serializable]
    public class NecrowStreamData
    {
        #region Fields


        #endregion
    }

    [Serializable]
    public enum ClientNecrowMessageTypes
    {
        Probe,



        IsNecrowAvailable,
        Ping
    }
    
    [Serializable]
    public class ClientNecrowServiceMessage : SessionServiceMessage
    {
        #region Fields

        private ClientNecrowMessageTypes type;

        public ClientNecrowMessageTypes Type
        {
            get { return type; }
            set { type = value; }
        }

        private string pingDestionation;

        public string PingDestination
        {
            get { return pingDestionation; }
            set { pingDestionation = value; }
        }

        #endregion

        #region Constructors

        public ClientNecrowServiceMessage(HttpContext context) : base(context)
        {
        }

        public ClientNecrowServiceMessage() : base()
        {

        }

        #endregion
    }


    [Serializable]
    public enum NecrowServiceMessageType
    {
        Hello,
        HelloResponse,
        ProbeStatus,
        Probe,


        Request
    }

    [Serializable]
    public class ServerNecrowServiceMessage : BaseServiceMessage
    {
        #region Fields

        private NecrowServiceMessageType type;

        public NecrowServiceMessageType Type
        {
            get { return type; }
            set { type = value; }
        }

        private int requestID;

        public int RequestID
        {
            get { return requestID; }
            set { requestID = value; }
        }

        private object identifierData1;

        public object IdentifierData1
        {
            get { return identifierData1; }
            set { identifierData1 = value; }
        }

        private object identifierData2;

        public object IdentifierData2
        {
            get { return identifierData2; }
            set { identifierData2 = value; }
        }

        private object identifierData3;

        public object IdentifierData3
        {
            get { return identifierData3; }
            set { identifierData3 = value; }
        }

        private object[] data;

        public object[] Data
        {
            get { return data; }
            set { data = value; }
        }

        #endregion

        #region Constructors

        public ServerNecrowServiceMessage(NecrowServiceMessageType type)
        {
            this.type = type;
        }

        #endregion
    }
}
