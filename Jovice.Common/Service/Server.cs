using Aphysoft.Common;
using Aphysoft.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;

namespace Jovice
{
    public static class Server
    {
        #region Fields

        private static Database center = Center.Database;

        #endregion

        #region Methods

        public static void Init()
        {
            #region Necrow

            // Register bind to necrow
            Service.Register(typeof(ServerNecrowServiceMessage), ServerNecrowServiceMessageHandler);
            // Register bind to client
            Service.Register(typeof(ClientNecrowServiceMessage), ClientNecrowServiceMessageHandler);
            Service.Disconnected += NecrowConnectionDisconnected;
            // Callback if register necrowa being registered to the server
            Provider.RegisterCallback("necrow", NecrowRegistered);

            #endregion

            #region Service

            #endregion

            
        }

        #endregion

        #region Necrow

        private static Connection necrowConnection = null;

        private static bool IsNecrowConnected()
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
        
        private static void ServerNecrowServiceMessageHandler(MessageEventArgs e)
        {
            ServerNecrowServiceMessage m = (ServerNecrowServiceMessage)e.Message;

            if (m.Type == NecrowServiceMessageType.Hello)
            {
                necrowConnection = e.Connection;

                Service.Debug("Necrow Connected");

                Provider.SetActionByRegisterMatch("service_[ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=]+", "yayaya");
                //Provider.SetActionByRegisterMatch("service", "service", "yayaya");
                //Provider.SetActionByRegister("necrowavailability", "necrow", "online");
            }
        }

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
}
