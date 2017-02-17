using Aphysoft.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Center.Providers
{
    public static class Network
    {
        public static ProviderPacket ProviderRequest(ResourceAsyncResult result, int id)
        {
            Database jovice = Jovice.Database;

            if (id == 15001) // Request AR
            {
                string reqType = Params.GetValue("ia"); // 
                if (reqType == "G")
                {
                    Result r = jovice.Query("select * from AreaGroup");
                    NetworkProviderPacket p = new NetworkProviderPacket();
                    List<object[]> points = new List<object[]>();
                    foreach (Row row in r)
                    {
                        object[] ob = new object[5];

                        ob[0] = row["AG_ID"].ToString().Trim();                        
                        ob[1] = row["AG_Latitude"].ToFloat();
                        ob[2] = row["AG_Longitude"].ToFloat();
                        ob[3] = row["AG_Name"].ToString().Trim();
                        ob[4] = row["AG_ShortName"].ToString().Trim();

                        points.Add(ob);
                    }
                    p.Points = points.ToArray();
                    return p;
                }
                else if (reqType == "W")
                {
                    Result r = jovice.Query("select * from AreaWitel");
                    NetworkProviderPacket p = new NetworkProviderPacket();
                    List<object[]> points = new List<object[]>();
                    foreach (Row row in r)
                    {
                        object[] ob = new object[5];

                        ob[0] = row["AW_ID"].ToString().Trim();
                        ob[1] = row["AW_Latitude"].ToFloat();
                        ob[2] = row["AW_Longitude"].ToFloat();
                        ob[3] = row["AW_Name"].ToString().Trim();
                        ob[4] = row["AW_AG"].ToString().Trim();

                        points.Add(ob);
                    }
                    p.Points = points.ToArray();
                    return p;
                }
                else if (reqType == "R")
                {
                    string nreq = Params.GetValue("r");

                    string[] nreqs = nreq.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

                    NetworkProviderPacket p = new NetworkProviderPacket();
                    List<object[]> points = new List<object[]>();
                    List<object[]> edges = new List<object[]>();

                    foreach (string entry in nreqs)
                    {
                        string[] entrys = entry.Split(StringSplitTypes.Comma, StringSplitOptions.RemoveEmptyEntries);
                        if (entrys.Length == 2)
                        {
                            string sgridx = entrys[0];
                            string sgridy = entrys[1];

                            int gridx, gridy;
                            if (int.TryParse(sgridx, out gridx) && int.TryParse(sgridy, out gridy))
                            {
                                double lngMin = (gridx * 2) - 180;
                                double lngMax = lngMin + 2;
                                if (lngMax == 180) lngMax = 180.0001;
                                double latMin = (gridy * 2) - 90;
                                double latMax = latMin + 2;
                                if (latMax == 90) latMax = 90.0001;

                                Result r = jovice.Query("select * from Area where AR_Latitude >= {0} and AR_Latitude < {1} and AR_Longitude >= {2} and AR_Longitude < {3}", latMin, latMax, lngMin, lngMax);
                                                                
                                foreach (Row row in r)
                                {
                                    object[] ob = new object[6];

                                    ob[0] = row["AR_ID"].ToString().Trim();
                                    ob[1] = row["AR_Latitude"].ToFloat();
                                    ob[2] = row["AR_Longitude"].ToFloat();
                                    ob[3] = gridx;
                                    ob[4] = gridy;
                                    ob[5] = row["AR_Name"].ToString().Trim();

                                    points.Add(ob);
                                }

                                r = jovice.Query(@"
select distinct aa.AR_ID so, ba.AR_Latitude de_Latitude, ba.AR_Longitude de_Longitude, ba.AR_Name de_Name from Area aa, Node an, MEInterface ai, MEInterface bi, Node bn, Area ba
where an.NO_AR = aa.AR_ID and  ai.MI_NO = an.NO_ID and ai.MI_TO_MI = bi.MI_ID and bi.MI_NO = bn.NO_ID and bn.NO_AR = ba.AR_ID
and aa.AR_ID <> ba.AR_ID
and aa.AR_Latitude >= {0} and aa.AR_Latitude < {1} and aa.AR_Longitude >= {2} and aa.AR_Longitude < {3}
", latMin, latMax, lngMin, lngMax);

                                foreach (Row row in r)
                                {
                                    object[] ob = new object[4];

                                    ob[0] = row["so"].ToString().Trim();
                                    ob[1] = row["de_Latitude"].ToFloat();
                                    ob[2] = row["de_Longitude"].ToFloat();
                                    ob[3] = row["de_Name"].ToString();

                                    edges.Add(ob);
                                }

                            }
                        }
                    }
                    p.Points = points.ToArray();
                    p.Edges = edges.ToArray();

                    return p;


                }


                //

                //

                //

                //foreach (Row row in r)
                //{
                //    object[] ob = new object[4];
                //    ob[0] = row["AR_ID"].ToString().Trim();
                //    ob[1] = row["AR_Name"].ToString().Trim();
                //    ob[2] = row["AR_Latitude"].ToFloat();
                //    ob[3] = row["AR_Longitude"].ToFloat();

                //    Service.Debug(ob);

                //    areas.Add(ob);
                //}

                //p.Areas = areas.ToArray();

                //return p;
            }

            return ProviderPacket.Null();
        }
    }


    [DataContract]
    public class NetworkProviderPacket : ProviderPacket
    {
        #region Fields

        private object[][] points = null;

        [DataMember(Name = "po")]
        public object[][] Points
        {
            get { return points; }
            set { points = value; }
        }

        private object[][] edges = null;

        [DataMember(Name = "eg")]
        public object[][] Edges
        {
            get { return edges; }
            set { edges = value; }
        }

        #endregion

        #region Contructors

        public NetworkProviderPacket()
        {
        }

        #endregion
    }
}