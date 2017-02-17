using Aphysoft.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;

namespace Center
{
    public static class Server
    {
        #region Methods

        public static void Init()
        {
            Settings.ServerInit();
            TelegramBot.Init();
                        
            Service.Register(typeof(ServerNecrowServiceMessage), ServerNecrowServiceMessageHandler); // Register bind to necrow            
            Service.Register(typeof(ClientNecrowServiceMessage), ClientNecrowServiceMessageHandler); // Register bind to client
            Service.Disconnected += NecrowConnectionDisconnected;

            // Callback if register necrowa being registered to the server
            Provider.RegisterCallback("necrow", NecrowRegistered);
           
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
