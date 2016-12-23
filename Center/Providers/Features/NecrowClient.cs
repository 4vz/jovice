
using Aphysoft.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Center.Providers
{
    public static class NecrowClient
    {
        public static ProviderPacket ProviderRequest(ResourceAsyncResult result, int id)
        {
            Database center = Center.Database;
            Database jovice = Jovice.Database;

            ClientNecrowServiceMessage m = new ClientNecrowServiceMessage();

            if (id == 50001)
            {
                #region Is Necrow Available

                m.Type = ClientNecrowMessageTypes.IsNecrowAvailable;

                if (Service.Send(m))
                {
                    ClientNecrowServiceMessage mr = Service.Wait(m);
                    return ProviderPacket.Data(mr.Data);
                }

                #endregion
            }
            else if (id == 50005)
            {
                #region PING Request

                m.Type = ClientNecrowMessageTypes.Ping;
                m.PingDestination = Params.GetValue("ip");

                string piid = Params.GetValue("pi");
                if (piid != null)
                {
                    
                }

                Service.Debug("PING:" + piid + " " + m.PingDestination);


                //if (Service.Send(m))
                //{
                //    ClientNecrowServiceMessage mr = Service.Wait(m);

                //}

                #endregion
            }

            



            /*

            m.IsNecrowAvailable = true;

            if (Service.IsConnected && Service.Send(m))
            {
                ClientNecrowServiceMessage mr = Service.Wait(m);

                if (mr.IsNecrowAvailable)
                {
                    if (id == 50001)
                    {
                        return ProviderPacket.SetData("NECROWAVAILABLE");
                    }
                    else if (id == 50002)
                    {
                        string node = Params.GetValue("n");
                        string vrf = Params.GetValue("v");
                        string dip = Params.GetValue("dip"); // destination ip
                        int count;
                        if (!int.TryParse(Params.GetValue("cou"), out count)) count = 5;
                        int size;
                        if (!int.TryParse(Params.GetValue("sze"), out size)) size = 100;

                        string inf = Params.GetValue("f");

                        if (node != null && vrf != null && dip != null)
                        {

                            Result r;
                            Row row;

                            r = jovice.Query("select NO_ID, NO_Name from Node where LOWER(NO_Name) = {0}", node.ToLower());

                            if (r.Count > 0)
                            {
                                row = r[0];
                                string nodeID = row["NO_ID"].ToString();

                                r = jovice.Query("select PN_ID, PN_Name from PERouteName where LOWER(PN_Name) = {0} and PN_NO = {1}", vrf.ToLower(), nodeID);

                                if (r.Count > 0)
                                {
                                    row = r[0];

                                    // validate dip
                                    IPAddress ipa = null;
                                    if (IPAddress.TryParse(dip, out ipa))
                                    {
                                        dip = ipa.ToString();

                                        if (count < 1) count = 1;
                                        else if (count > 1000) count = 1000;

                                        if (size < 32) size = 32;
                                        else if (size > 1000) size = 1000;
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    return ProviderPacket.SetData("NECROWNOTAVAILABLE");
                }
            }*/

            return ProviderPacket.Null();
        }
    }
}