using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Threading;
using System.Diagnostics;
using System.Runtime.Serialization.Json;
using System.Web.SessionState;
using System.Runtime.Serialization;

using System.IO;
using System.Xml;
using System.Text;
using System.Text.RegularExpressions;

namespace Aphysoft.Share
{
    public class Provider
    {
        #region Fields
        
        private static Dictionary<int, ProviderRegister> resourceRegisters = new Dictionary<int, ProviderRegister>();

        private static Dictionary<string, ProviderRegister> serviceRegisters = new Dictionary<string, ProviderRegister>();

        #endregion

        internal static void Init()
        {
            Resource.Register("xhr_stream", ResourceTypes.Text, StreamBeginProcessRequest, StreamEndProcessRequest).NoBufferOutput().AllowOrigin("http" + (WebSettings.Secure ? "s" : "") + "://" + WebSettings.Domain).AllowCredentials();
            Resource.Register("xhr_provider", ResourceTypes.JSON, ProviderBeginProcessRequest, ProviderEndProcessRequest);
            Resource.Register("xhr_content_provider", ResourceTypes.JSON, Content.Begin, Content.End);
        }

        public static event ProviderClientDisconnectedEventHandler ClientDisconnected;
        
        internal static void StreamBeginProcessRequest(ResourceAsyncResult result)
        {
            HttpResponse httpResponse = result.Context.Response;
            HttpRequest httpRequest = result.Context.Request;

            try
            {
                httpResponse.Write("for(;;); ");

                StreamHeartbeatData hb = new StreamHeartbeatData();
                hb.serverTime = DateTime.Now;

                string heartbeat = WebUtilities.Serializer.Serialize(hb);

                StringBuilder padding = new StringBuilder();
                int ipadding = 1024 - heartbeat.Length;
                for (int i = 0; i < ipadding; i++) padding.Append(" ");
                httpResponse.Write(heartbeat + padding.ToString() + "\n");
            }
            catch
            {
                result.SetCompleted();
                return;
            }
            
            if (Web.Service.IsConnected)
            {
                string clientID = Params.GetValue("c", result.Context);
                string sessionId = Session.Id;
                bool clientStillConnected = true;

                if (sessionId == null || clientID == null)
                {
                    try
                    {
                        httpResponse.Write("{\"t\":\"unavailable\"}\n");
                    }
                    catch (HttpException hex)
                    {
                    }
                }
                else
                {

                    try
                    {
                        httpResponse.Write("{\"t\":\"available\"}\n");
                    }
                    catch (HttpException hex)
                    {
                        clientStillConnected = false;
                    }

                    if (clientStillConnected)
                    {
                        StreamMessage message = new StreamMessage(sessionId);

                        //message.httpRequest.UserHostAddress;
                        message.ClientID = clientID;

                        string requestHost = httpRequest.Headers["Host"];

                        if (requestHost != null)
                        {
                            string challengeRequestBaseHost = "base." + WebSettings.StreamDomain;
                            //if (Settings.StreamBasePort != 0)
                            //    challengeRequestBaseHost += ":" + Settings.StreamBasePort;

                            if (requestHost == challengeRequestBaseHost)
                                message.HostSessionIndex = -1;
                            else
                            {
                                for (int ido = 0; ido < WebSettings.MaxStream; ido++)
                                {
                                    string challengeRequestHost = "c-" + (ido + 1) + "." + WebSettings.StreamDomain;
                                    //if (Settings.StreamSubPorts[ido] != 0)
                                    //    challengeRequestHost += ":" + Settings.StreamSubPorts[ido];

                                    if (requestHost == challengeRequestHost)
                                    {
                                        message.HostSessionIndex = ido;
                                        break;
                                    }
                                }
                            }
                        }

                        if (Web.Service.Send(message))
                        {
                            do
                            {
                                /*StreamMessage response = OldService.Wait(message);

                                if (!response.IsAborted)
                                {
                                    if (response.MessageContinue)
                                    {
                                        try
                                        {
                                            httpResponse.Write("{\"t\":\"continue\"}\n");
                                        }
                                        catch (HttpException hex)
                                        {
                                            clientStillConnected = false;
                                        }

                                        break;
                                    }
                                    else
                                    {
                                        if (response.MessageType == "updatestreamdomain")
                                        {
                                            int hostIndex = ((int)response.MessageData) % Settings.StreamSubDomains.Length;
                                            string hostName;

                                            if (hostIndex < Settings.StreamSubDomains.Length)
                                            {
                                                hostName = Settings.StreamSubDomains[hostIndex] + "." + Settings.StreamDomain;
                                                if (Settings.StreamSubPorts[hostIndex] != 0)
                                                    hostName += ":" + Settings.StreamSubPorts[hostIndex];
                                            }
                                            else
                                            {
                                                hostName = Settings.StreamBaseSubDomain + "." + Settings.StreamDomain;
                                                if (Settings.StreamBasePort != 0)
                                                    hostName += ":" + Settings.StreamBasePort;
                                            }

                                            try
                                            {
                                                httpResponse.Write(string.Format("{{\"t\":\"updatestreamdomain\",\"d\":\"{0}\"}}\n", hostName));
                                            }
                                            catch (HttpException hex)
                                            {
                                                clientStillConnected = false;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            try
                                            {
                                                if (response.MessageData == null)
                                                    httpResponse.Write(string.Format("{{\"t\":\"{0}\"}}\n", response.MessageType));
                                                else
                                                    httpResponse.Write("{\"t\":\"" + response.MessageType + "\",\"d\":" + WebUtilities.Serializer.Serialize(response.MessageData) + "}\n");
                                            }
                                            catch (HttpException hex)
                                            {
                                                clientStillConnected = false;
                                                break;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    // two type                            
                                    if (OldService.IsConnected == false) // if server down
                                    {
                                        string hostName = Settings.StreamBaseSubDomain + "." + Settings.StreamDomain;

                                        if (Settings.StreamBasePort != 0)
                                            hostName += ":" + Settings.StreamBasePort;

                                        httpResponse.Write(string.Format("{{\"t\":\"updatestreamdomain\",\"d\":\"{0}\"}}\n", hostName));
                                        httpResponse.Write("{\"t\":\"disconnected\"}\n");
                                    }
                                    else // if page left
                                    {
                                        // tell server that page has been left
                                        OldService.Send(new ClientEndMessage(clientID, sessionId));
                                        httpResponse.Write("{\"t\":\"pageend\"}\n");
                                    }

                                    break;
                                }*/

                                break;
                            }
                            while (true);
                        }
                    }
                }
            }
            else
            {
                try
                {
                    httpResponse.Write("{\"t\":\"unavailable\"}\n");
                }
                catch (HttpException hex)
                {
                }
            }

            result.SetCompleted();
        }

        internal static void StreamEndProcessRequest(ResourceAsyncResult result)
        {
        }

        internal static void ProviderBeginProcessRequest(ResourceAsyncResult result)
        {
            HttpContext context = result.Context;
            HttpResponse response = context.Response;
            HttpRequest request = context.Request;

            //Identity.Start();

            result.Tag = 0;

            // get requested provider id
            string appids = QueryString.GetValue("i");
            string cid = QueryString.GetValue("c");

            if (cid == null) // or cid not registered
            {
                response.Status = "403 Forbidden";
            }
            else if (appids != null)
            {
                // parse to int
                int appid;
                if (int.TryParse(appids, out appid))
                {
                    // succeeded
                    ProviderPacket packet = null;
                    result.Tag = appid;

                    if (appid >= 0 && appid <= 50) // system appid
                    {
                        #region System APPID
                        if (appid == 1)
                        {
                            #region Debug
                            string message = Params.GetValue("m");

                            packet = new ProviderPacket();
                            #endregion
                        }
                        else if (appid == 10 || appid == 11)
                        {
                            #region Change Register
                            string x = Params.GetValue("x");
                            string c = Params.GetValue("c");

                            if (x != null && c != null)
                            {
                                RegisterMessage m = new RegisterMessage(c, x.Split(StringSplitTypes.Comma, StringSplitOptions.RemoveEmptyEntries));
                                if (appid == 11) m.Force = true;
                                //OldService.Send(m);
                            }
                            #endregion
                        }

                        #endregion
                    }
                    else if (resourceRegisters.ContainsKey(appid))
                    {
                        ProviderRegister register = resourceRegisters[appid];
                        ResourceRequest proc = register.ResourceHandler;

                        packet = proc(result, appid);
                    }

                    if (packet == null)
                        packet = ProviderPacket.Null();
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(packet.GetType());
                    serializer.WriteObject(response.OutputStream, packet);
                }
            }
            result.SetCompleted();
        }

        internal static void ProviderEndProcessRequest(ResourceAsyncResult result)
        {
        }

        internal static void OnClientDisconnected(ProviderClientDisconnectedEventArgs e)
        {
            ClientDisconnected?.Invoke(e);
        }
        
        public static void Register(int[] ids, ResourceRequest handler)
        {
            ProviderRegister register = null;

            foreach (int id in ids)
            {
                if (!resourceRegisters.ContainsKey(id))
                {
                    if (register == null)
                        register = new ProviderRegister(handler);

                    lock (resourceRegisters)
                    {
                        resourceRegisters.Add(id, register);
                    }
                }
            }
        }

        public static void Register(int id, ResourceRequest handler)
        {
            if (!resourceRegisters.ContainsKey(id))
            {
                lock (resourceRegisters)
                {
                    resourceRegisters.Add(id, new ProviderRegister(handler));
                }
            }
        }
    }
    
    public class ProviderClientDisconnectedEventArgs : EventArgs
    {
        public string SessionId { get; }

        public ProviderClientDisconnectedEventArgs(string sessionId)
        {
            SessionId = sessionId;
        }
    }

    public delegate void ProviderRequestHandler(string data);

    public delegate void ProviderClientDisconnectedEventHandler(ProviderClientDisconnectedEventArgs e);
    



    internal class StreamHeartbeatData
    {
        #region Fields

        public string t = "heartbeat";
        public string d = "";
        public DateTime serverTime;
        public string v = Web.Version();

        #endregion
    }









    
    internal class ProviderRegister
    {
        private ResourceRequest resourceHandler;

        public ResourceRequest ResourceHandler
        {
            get { return resourceHandler; }
        }

        public ProviderRegister(ResourceRequest handler)
        {
            resourceHandler = handler;
        }
    }

    public delegate ProviderPacket ResourceRequest(ResourceAsyncResult result, int id);

    [DataContract]
    public class ProviderPacket
    {
        private string _data = null;

        [DataMember(Name = "data")]
        internal string _Data
        {
            get { return _data; }
            set { _data = value; }
        }

        public ProviderPacket()
        {
        }

        public static ProviderPacket Null()
        {
            return new ProviderPacket();
        }

        public static ProviderPacket Data(string data)
        {
            ProviderPacket p = new ProviderPacket();
            p._Data = data;
            return p;
        }
    }
}
