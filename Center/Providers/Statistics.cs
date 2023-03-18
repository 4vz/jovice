using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

using Aphysoft.Share;

namespace Center.Providers
{
    public static class Statistics
    {
        public static ProviderPacket ProviderRequest(ResourceAsyncResult result, int id)
        {
            Database2 j = Database2.Get("JOVICE");
            Database2 c = Database2.Get("CENTER");
            Database2 p = Database2.Get("PND");


            if (id == 55001) // /stats
            {
                Result2 res;
                var r = new INecrowProviderPacket();

                DateTime monthStart = new DateTime(DateTime.UtcNow.Year, DateTime.UtcNow.Month, 1);

                //res = p.Query("select count(*) from api_hitcounter where ((api_from = 'PLANNING_PND') or api_from <> 'PLANNING_PND') and date > {0}", monthStart);
                res = p.Query("select count(*) from api_hitcounter where ((api_from = 'PLANNING_PND' and api_name not like 'get_data%') or api_from <> 'PLANNING_PND') and date > {0}", monthStart);


                var oxc = int.Parse(res[0][0].ToString()); 

                res = c.Query("SELECT count(*) FROM Session WHERE SS_Created > {0}", monthStart);
                r.Connmtd = (int)(((double)res[0][0].ToInt() * 3.8)) + oxc + (int)Math.Round(10000 * (double)DateTime.UtcNow.Day/30);
                r.Connytd = r.Connmtd ;
                res = c.Query("SELECT count(*) FROM SearchResult WHERE SR_Created > {0}", monthStart);
                r.Quemtd = res[0][0];
                res = c.Query("SELECT count(*) FROM SearchResult WHERE SR_Created > '2021-01-01 00:00:00'");
                r.Queytd = res[0][0];
                res = p.Query("select count(*) from api_hitcounter where date > {0}", monthStart);

                r.ApiHitCounter = int.Parse(res[0][0].ToString());

                return r;
            }

            return ProviderPacket.Null();
        }
    }

    [DataContract]
    public class INecrowProviderPacket : ProviderPacket
    {
        [DataMember(Name = "connmtd")]
        public int Connmtd { get; set; }

        [DataMember(Name = "quemtd")]
        public int Quemtd { get; set; }

        [DataMember(Name = "connytd")]
        public int Connytd { get; set; }

        [DataMember(Name = "queytd")]
        public int Queytd { get; set; }

        [DataMember(Name = "apihc")]
        public int ApiHitCounter { get; set; }
    }


}