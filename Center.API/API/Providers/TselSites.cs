using Aphysoft.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Center.APIs
{
    public static class TselSites
    {
        public static APIPacket APIRequest(APIAsyncResult result, string[] paths, string apiAccessID)
        {
            Database j = Jovice.Database;
            int count = paths.Length;

            if (paths[1] == "search")
            {
                // search
                if (count > 3)
                {
                    string searchby = null;
                    if (paths[2].ArgumentIndexOf("siteid", "mac") > -1) searchby = paths[2];
                    else return new ErrorAPIPacket(APIErrors.BadRequestFormat);

                    int offset = 0;
                    int limit = 20;

                    string offsets = QueryString.GetValue("offset");
                    string limits = QueryString.GetValue("limit");
                    if (offsets != null && int.TryParse(offsets, out offset)) { }
                    if (offset < 0) offset = 0;
                    if (limits != null && int.TryParse(limits, out limit)) { }
                    if (limit < 1) limit = 1;
                    else if (limit > 50) limit = 50;

                    string searchpattern = paths[3].Replace('*', '%');

                    TselSitesResultAPIPacket packet = null;
                    packet = new TselSitesResultAPIPacket();
                    packet.SearchType = searchby;
                    packet.SearchPattern = paths[3];
                    packet.Results = new TselSitesResultItemAPIPacket[0];

                    packet.ResultOffset = offset;
                    packet.ResultLimit = limit;

                    if (searchby == "siteid")
                    {
                        Result r = j.Query(@"
select * from (select ROW_NUMBER() over (order by MI_ID) as ROWNUM, MI_ID as ROWID, * from (
select MI_ID, MI_Name, NO_Name, NO_Manufacture, MI_Description, NO_TimeStamp, SE_SID, MI_Type
from MEInterface, Node, Service, ServiceCustomer 
where SE_SC = SC_ID and SC_Name = 'TELKOMSELSITES' and SE_SID like {0} and MI_SE = SE_ID and MI_NO = NO_ID and NO_Active = 1 and MI_Type is not null
) source) source where ROWNUM > {1} AND ROWNUM <= {2}
", searchpattern, offset, offset + limit);

                        Result rc = j.Query(@"
select count(*) from (
select MI_ID
from MEInterface, Node, Service, ServiceCustomer 
where SE_SC = SC_ID and SC_Name = 'TELKOMSELSITES' and SE_SID like {0} and MI_SE = SE_ID and MI_NO = NO_ID and NO_Active = 1 and MI_Type is not null
) source
", searchpattern);

                        packet.ResultCount = rc[0][0].ToInt();

                        List<TselSitesResultItemAPIPacket> items = new List<TselSitesResultItemAPIPacket>();

                        foreach (Row row in r)
                        {
                            TselSitesResultItemAPIPacket item = new TselSitesResultItemAPIPacket();

                            item.Index = row["ROWNUM"].ToLong();
                            item.Interface = row["MI_Name"].ToString().Substring(2);
                            item.InterfaceDescription = row["MI_Description"].ToString();
                            item.InterfaceType = row["MI_Type"].ToString();
                            item.LastUpdated =  row["NO_TimeStamp"].ToDateTime();
                            item.SiteID = row["SE_SID"].ToString();
                            item.Node = row["NO_Name"].ToString();
                            item.NodeManufacture = row["NO_Manufacture"].ToString();

                            items.Add(item);
                        }

                        packet.Results = items.ToArray();
                    }
                    else
                    {
                        //mac
                        Result r = j.Query(@"
select * from (
select ROW_NUMBER() over (order by infmi) as ROWNUM, * from (
select distinct inf.MI_Name as infmi, inf.SE_SID, inf.NO_Name, MA_Mac, inf.NO_Manufacture, inf.MI_Description, inf.NO_TimeStamp, inf.MI_Type 
from MEMac, MEInterface m, (
select MI_ID, MI_Name, SE_SID, NO_Name, NO_Manufacture, MI_Description, NO_TimeStamp, MI_Type
from MEInterface, Node, Service, ServiceCustomer 
where SE_SC = SC_ID and SC_Name = 'TELKOMSELSITES' and MI_SE = SE_ID and MI_NO = NO_ID and NO_Active = 1 and MI_Type is not null
) inf 
where MA_MI = m.MI_ID and m.MI_MI = inf.MI_ID and MA_Mac like {0}
) 
source) 
source where ROWNUM > {1} AND ROWNUM <= {2}
", searchpattern.ToLower(), offset, offset + limit);

                        Result rc = j.Query(@"
select count(*) from (
select distinct inf.MI_Name, inf.SE_SID, inf.NO_Name, MA_Mac from MEMac, MEInterface m, (
select MI_ID, MI_Name, SE_SID, NO_Name
from MEInterface, Node, Service, ServiceCustomer 
where SE_SC = SC_ID and SC_Name = 'TELKOMSELSITES' and MI_SE = SE_ID and MI_NO = NO_ID and NO_Active = 1 and MI_Type is not null
) inf where MA_MI = m.MI_ID and m.MI_MI = inf.MI_ID and MA_Mac like {0}
) source
", searchpattern);

                        List<TselSitesResultItemAPIPacket> items = new List<TselSitesResultItemAPIPacket>();

                        foreach (Row row in r)
                        {
                            TselSitesResultItemAPIPacket item = new TselSitesResultItemAPIPacket();

                            item.Index = row["ROWNUM"].ToLong();
                            item.Interface = row["infmi"].ToString().Substring(2);
                            item.InterfaceDescription = row["MI_Description"].ToString();
                            item.InterfaceType = row["MI_Type"].ToString();
                            item.LastUpdated = row["NO_TimeStamp"].ToDateTime();
                            item.SiteID = row["SE_SID"].ToString();
                            item.Mac = row["MA_Mac"].ToString();
                            item.Node = row["NO_Name"].ToString();
                            item.NodeManufacture = row["NO_Manufacture"].ToString();

                            items.Add(item);
                        }

                        packet.Results = items.ToArray();

                        packet.ResultCount = rc[0][0].ToInt();

                    }

                    return packet;
                }
                else return new ErrorAPIPacket(APIErrors.BadRequestFormat);
            }
            else
            {
                // show
                string siteID = paths[1];

                TselSiteAPIPacket packet = new TselSiteAPIPacket();

                Result r = j.Query(@"
select MI_ID, MI_Name, NO_Name, NO_Manufacture, MI_Description, NO_TimeStamp, SE_SID, MI_Type
from MEInterface, Node, Service, ServiceCustomer 
where SE_SC = SC_ID and SC_Name = 'TELKOMSELSITES' and SE_SID = {0}
and MI_SE = SE_ID and MI_NO = NO_ID and NO_Active = 1 and MI_Type is not null
", siteID);

                if (r.Count == 1)
                {
                    Row row = r[0];

                    packet.SiteID = row["SE_SID"].ToString();
                    packet.Node = row["NO_Name"].ToString();
                    packet.NodeManufacture = row["NO_Manufacture"].ToString();
                    packet.LastUpdated = row["NO_TimeStamp"].ToDateTime();
                    packet.Interface = row["MI_Name"].ToString().Substring(2);
                    packet.InterfaceDescription = row["MI_Description"].ToString();
                    packet.InterfaceType = row["MI_Type"].ToString();
                    packet.Topology = "(ERROR CYCLIC CONNECTION FOUND)";

                    return packet;
                }
                else
                {
                    return new ErrorAPIPacket(APIErrors.NotFound);
                }
            }
        }
    }

    [DataContract]
    public class TselSiteAPIPacket : APIPacket
    {
        #region Fields

        private string siteID;

        [DataMember(Name = "siteid")]
        public string SiteID { get => siteID; set => siteID = value; }
        
        private DateTime lastUpdated;

        [DataMember(Name = "lastupdated")]
        public DateTime LastUpdated { get => lastUpdated; set => lastUpdated = value; }

        private string node;

        [DataMember(Name = "node")]
        public string Node
        {
            get { return node; }
            set { node = value; }
        }

        private string nodeManufacture;

        [DataMember(Name = "nodemanufacture")]
        public string NodeManufacture
        {
            get { return nodeManufacture; }
            set { nodeManufacture = value; }
        }

        private string inf;

        [DataMember(Name = "interface")]
        public string Interface
        {
            get { return inf; }
            set { inf = value; }
        }

        private string infDescription;

        [DataMember(Name = "interfacedescription")]
        public string InterfaceDescription
        {
            get { return infDescription; }
            set { infDescription = value; }
        }

        private string infType;

        [DataMember(Name = "interfacetype")]
        public string InterfaceType
        {
            get { return infType; }
            set { infType = value; }
        }

        private string topology;

        [DataMember(Name = "topology")]
        public string Topology { get => topology; set => topology = value; }
        
        #endregion
    }

    [DataContract]
    public class TselSitesResultAPIPacket : ResultAPIPacket
    {
        #region Fields

        private string searchType;

        [DataMember(Name = "searchtype")]
        public string SearchType
        {
            get { return searchType; }
            set { searchType = value; }
        }

        private string searchPattern;

        [DataMember(Name = "searchpattern")]
        public string SearchPattern
        {
            get { return searchPattern; }
            set { searchPattern = value; }
        }

        private TselSitesResultItemAPIPacket[] results;

        [DataMember(Name = "results")]
        public TselSitesResultItemAPIPacket[] Results
        {
            get { return results; }
            set { results = value; }
        }

        #endregion

        #region Constructors

        public TselSitesResultAPIPacket()
        {

        }

        #endregion
    }

    [DataContract]
    public class TselSitesResultItemAPIPacket : ResultItemAPIPacket
    {
        #region Fields

        private DateTime lastUpdated;

        [DataMember(Name = "lastupdated")]
        public DateTime LastUpdated
        {
            get { return lastUpdated; }
            set { lastUpdated = value; }
        }

        private string siteID;

        [DataMember(Name = "siteid")]
        public string SiteID
        {
            get { return siteID; }
            set { siteID = value; }
        }

        private string mac;

        [DataMember(Name = "mac")]
        public string Mac
        {
            get { return mac; }
            set { mac = value; }
        }

        private string node;

        [DataMember(Name = "node")]
        public string Node
        {
            get { return node; }
            set { node = value; }
        }

        private string nodeManufacture;

        [DataMember(Name = "nodemanufacture")]
        public string NodeManufacture
        {
            get { return nodeManufacture; }
            set { nodeManufacture = value; }
        }

        private string inf;

        [DataMember(Name = "interface")]
        public string Interface
        {
            get { return inf; }
            set { inf = value; }
        }

        private string infDescription;

        [DataMember(Name = "interfacedescription")]
        public string InterfaceDescription
        {
            get { return infDescription; }
            set { infDescription = value; }
        }

        private string infType;

        [DataMember(Name = "interfacetype")]
        public string InterfaceType
        {
            get { return infType; }
            set { infType = value; }
        }

        #endregion

        public TselSitesResultItemAPIPacket()
        {

        }

    }
}