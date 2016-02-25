using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using Aphysoft.Common;
using Aphysoft.Share;

namespace Jovice.Providers
{
    public sealed class Portal
    {
        public static ProviderPacket ProviderRequest(ResourceResult result, int id)
        {
            if (id == 30001)
            {
                #region Portal Authentication

                string userName = Params.GetValue("u");
                string password = Params.GetValue("p");

                PortalAuthResult packet = new PortalAuthResult();
                packet.UserName = userName;

                WebRequest request = WebRequest.Create("http://esshr.telkom.co.id/esshr/presensi/index.php");

                string comb = Base64.Encode(userName + ":" + password);
                //Service.Event("-->" + comb);                

                request.Headers.Add("Authorization", "Basic " + comb);

                bool isok = false;
                try
                {
                    WebResponse response = request.GetResponse();
                    isok = true;
                }
                catch (WebException ex)
                {

                }
                packet.Accepted = isok;
                
                
                return packet;

                #endregion
            }

            return ProviderPacket.Null();
        }
    }

    #region 30001 Portal Authentication

    [DataContractAttribute]
    public class PortalAuthResult : ProviderPacket
    {
        #region Fields

        private string userName;

        [DataMemberAttribute(Name = "u")]
        public string UserName
        {
            get { return userName; }
            set { userName = value; }
        }

        private bool accepted;

        [DataMemberAttribute(Name = "a")]
        public bool Accepted
        {
            get { return accepted; }
            set { accepted = value; }
        }

        #endregion

        #region Constructors

        public PortalAuthResult()
        {

        }

        #endregion
    }

    #endregion
}
