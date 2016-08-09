using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using Aphysoft.Common;
using Aphysoft.Share;

namespace Jovice.Providers
{
    public static class Statistics
    {
        public static ProviderPacket ProviderRequest(ResourceAsyncResult result, int id)
        {
            Database jovice = Jovice.Database;

            if (id == 5001) // 2001 Main Statistics
            {
                Result res;

                ProviderStatistics r = new ProviderStatistics();
                
                res = jovice.Query("select count(*) from Node where NO_Active = 1");

                r.Counts.Add(res[0][0].ToInt());
                r.Descs.Add("NETWORK ELEMENTS");

                res = jovice.Query("select count(*) from Node where NO_Type = 'P'");

                r.Counts.Add(res[0][0].ToInt());
                r.Descs.Add("PE-ROUTERS");

                res = jovice.Query("select count(*) from Node where NO_Type = 'M'");

                r.Counts.Add(res[0][0].ToInt());
                r.Descs.Add("METROS");

                res = jovice.Query(@"select count(*) from (
select MI_ID from MEInterface, Node where MI_NO = NO_ID and MI_Type is not null and NO_Active = 1
union all
select PI_ID from PEInterface, Node where PI_NO = NO_ID and PI_Name not like '%.%' and NO_Active = 1
) as s");

                r.Counts.Add(res[0][0].ToInt());
                r.Descs.Add("INTERFACES");

                res = jovice.Query(@"select count(*) from (
select MI_ID from MEInterface, Node where MI_NO = NO_ID and NO_Active = 1
union all
select PI_ID from PEInterface, Node where PI_NO = NO_ID  and NO_Active = 1
) as s");

                r.Counts.Add(res[0][0].ToInt());
                r.Descs.Add("LOGICAL INTERFACES");

                res = jovice.Query("select count(distinct PN_PR) from PEInterface, PERouteName, Node where PI_PN = PN_ID and PI_NO = NO_ID and NO_Active = 1");

                r.Counts.Add(res[0][0].ToInt());
                r.Descs.Add("VRF");

                res = jovice.Query("select count(*) from MECircuit, Node where MC_NO = NO_ID and NO_Active = 1");

                r.Counts.Add(res[0][0].ToInt());
                r.Descs.Add("VCID");

                res = jovice.Query("select count(distinct PQ_Name) from PEQOS, Node where PQ_NO = NO_ID and NO_Active = 1");

                r.Counts.Add(res[0][0].ToInt());
                r.Descs.Add("QOS");

                res = jovice.Query("select count(*) from PEInterfaceIP, PEInterface, Node where PP_PI = PI_ID and PI_NO = NO_ID and NO_Active = 1");

                r.Counts.Add(res[0][0].ToInt());
                r.Descs.Add("IP");

                return r;
            }

            return ProviderPacket.Null();
        }
    }

    #region 2001 Main Statistics

    [DataContractAttribute]
    public class ProviderStatistics : ProviderPacket
    {
        private List<int> counts = new List<int>();

        [DataMemberAttribute(Name = "counts")]
        public List<int> Counts
        {
            get { return counts; }
            set { counts = value; }
        }

        private List<string> descs = new List<string>();

        [DataMemberAttribute(Name = "descs")]
        public List<string> Descs
        {
            get { return descs; }
            set { descs = value; }
        }




    }

    #endregion
}