
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Aphysoft.Share;

using System.Text;
using System.Net;

namespace Center.Providers
{
    public class ServiceSearchMatch : SearchMatch
    {
        #region Constructors

        public ServiceSearchMatch() : base()
        {
            Root("services");
            Root("service");            

            Language("sid", "SID");
            Language("service id", "SID");
            Language("services id", "SID");
            Language("cid", "CID");
            Language("customer id", "CID");
            Language("customers id", "CID");
            Language("customer", "CUSTOMER");
            Language("customers", "CUSTOMER");
            Language("name", "NAME");
            Language("names", "NAME");
            Language("vcid", "VCID");
            Language("vpnip", "VPNIP");
            Language("vpn ip", "VPNIP");
            Language("ipvpn", "VPNIP");
            Language("ip vpn", "VPNIP");
            Language("transaccess", "TRANSACC");
            Language("transacc", "TRANSACC");
            Language("transac", "TRANSACC");
            Language("transaces", "TRANSACC");
            Language("transacess", "TRANSACC");
            Language("astinet", "ASTINET");
            Language("astinetbb", "ASTINETBB");
            Language("astinet bedabandwidth", "ASTINETBB");
            Language("astinet bedabw", "ASTINETBB");
            Language("astinet beda bandwidth", "ASTINETBB");
            Language("astinet different bandwidth", "ASTINETBB");
            Language("astinet multi bandwidth", "ASTINETBB");
            Language("metro-e", "METRO");
            Language("metro-ethernet", "METRO");
            Language("metro ethernet", "METRO");
            Language("metro e", "METRO");
            Language("metro", "METRO");
        }

        #endregion

        #region Methods

        public override void Process(SearchMatchResult matchResult, SearchMatchQuery matchQuery)
        {
            matchResult.Type = "jovice_service";

            Where whereVCID = SearchDescriptor.Build(matchQuery.Descriptors, delegate(SearchDescriptor descriptor)
            {
                SearchConstraints c = descriptor.Constraint;

                return descriptor.Build(delegate(int index, string value)
                {
                    string v = jovice.Escape(value);

                    if (descriptor.Descriptor == "VCID")
                    {
                        if (v.Length >= 4)
                        {
                            if (c == SearchConstraints.StartsWith) return "MC_VCID like '" + v + "%'";
                            else if (c == SearchConstraints.EndsWith) return "MC_VCID like '%" + v + "'";
                            else if (c == SearchConstraints.Like) return "MC_VCID like '%" + v + "%'";
                            else if (c == SearchConstraints.Equal) return "MC_VCID like '" + v + "'";
                        }
                        else return null;
                    }
                    return null;
                });
            });
            Where whereService = SearchDescriptor.Build(matchQuery.Descriptors, delegate (SearchDescriptor descriptor)
            {
                SearchConstraints c = descriptor.Constraint;
                string cv = Search.JoinValues(descriptor.Values);

                return descriptor.Build(delegate (int index, string value)
                {
                    string v = jovice.Escape(value);

                    if (descriptor.Descriptor == "SID")
                    {
                        if (c == SearchConstraints.StartsWith) return "SE_SID like '" + v + "%'";
                        else if (c == SearchConstraints.EndsWith) return "SE_SID like '%" + v + "'";
                        else if (c == SearchConstraints.Like) return "SE_SID like '%" + v + "%'";
                        else if (c == SearchConstraints.Equal) return "SE_SID like '" + v + "'";
                    }
                    else if ((descriptor.SuperDescriptor == "CUSTOMER" && descriptor.Descriptor == "NAME") || descriptor.Descriptor == "CUSTOMER")
                    {
                        if (c == SearchConstraints.StartsWith) return "SC_Name like '" + v + "%'";
                        else if (c == SearchConstraints.EndsWith) return "SC_Name like '%" + v + "'";
                        else if (c == SearchConstraints.Like) return "SC_Name like '%" + v + "%'";
                        else if (c == SearchConstraints.Equal) return "SC_Name like '" + v + "'";
                    }
                    else if (descriptor.Descriptor == "CID")
                    {
                        if (c == SearchConstraints.StartsWith) return "SC_CID like '" + v + "%'";
                        else if (c == SearchConstraints.EndsWith) return "SC_CID like '%" + v + "'";
                        else if (c == SearchConstraints.Like) return "SC_CID like '%" + v + "%'";
                        else if (c == SearchConstraints.Equal) return "SC_CID like '" + v + "'";
                    }
                    else if (descriptor.Descriptor == "ASTINET")
                    {
                        string res = "SE_Type = 'AS'";
                        if (v.Length > 0) res += " AND SC_Name like '" + v + "'";
                        return res;
                    }
                    else if (descriptor.Descriptor == "ASTINETBB")
                    {
                        string res = "SE_Type = 'AB'";
                        if (v.Length > 0) res += " AND SC_Name like '" + v + "'";
                        return res;
                    }
                    else if (descriptor.Descriptor == "VPNIP")
                    {
                        string res = "SE_Type = 'VP'";
                        if (v.Length > 0) res += " AND SC_Name like '" + v + "'";
                        return res;
                    }
                    else if (descriptor.Descriptor == "TRANSACC")
                    {
                        string res = "SE_Type = 'VP' AND SE_SubType = 'TA'";
                        if (v.Length > 0) res += " AND SC_Name like '" + v + "'";
                        return res;
                    }
                    else if (descriptor.Descriptor == "METRO")
                    {
                        string res = "SE_Type is null";
                        if (v.Length > 0) res += " AND SC_Name like '" + v + "'";
                        return res;
                    }

                    return null;
                });
            });

            if (whereVCID.Value != null)
            {
                #region VCID exists
                matchResult.QueryCount = @"
select distinct SO_ID
from (select SO_ID
from (
select MC_SE as SO_ID from 
MECircuit" + whereVCID.Format(" where ") + @"
union all
select MI_SE as SO_ID from
MEInterface, MECircuit
where MI_MC = MC_ID" + whereVCID.Format(" and ") + @"
union all
select PI_SE as SO_ID from
PEInterface, MEInterface, MECircuit
where PI_ID = MI_TO_PI and MI_MC = MC_ID" + whereVCID.Format(" and ") + @"
) source where SO_ID is not null
) source, Service
left join ServiceCustomer on SC_ID = SE_SC
where SE_ID = SO_ID" + whereService.Format(" and ");

                matchResult.Query = @"
select distinct SE_ID, SE_SID, SC_CID, SC_Name, SC_Name_Set, SE_Type, SE_SubType
from (
select SO_ID, ROW_NUMBER() OVER (order by SO_Rate desc, SO_Bandwidth desc
) AS SO_RN from (
select MC_SE as SO_ID, null as SO_Rate, null as SO_Bandwidth from 
MECircuit" + whereVCID.Format(" where ") + @"
union all
select MI_SE as SO_ID, MI_Rate_Output as SO_Rate, MQ_Bandwidth as SO_Bandwidth from
MECircuit, MEInterface left join MEQOS on MI_MQ_Output = MQ_ID
where MI_MC = MC_ID" + whereVCID.Format(" and ") + @"
union all
select PI_SE as SO_ID, 0 as SO_Rate, 0 as SO_Bandwidth from
PEInterface, MEInterface, MECircuit
where PI_ID = MI_TO_PI and MI_MC = MC_ID" + whereVCID.Format(" and ") + @"
) source where SO_ID is not null
) source, Service
left join ServiceCustomer on SC_ID = SE_SC
where SE_ID = SO_ID" + whereService.Format(" and ");
                #endregion
            }
            else
            {
                #region Service exists
                matchResult.QueryCount = @"
select SE_ID from Service left join ServiceCustomer on SC_ID = SE_SC" + whereService.Format(" where ");

                matchResult.Query = @"
select SE_ID, SE_SID, SC_CID, SC_Name, SC_Name_Set, SE_Type, SE_SubType
from Service left join ServiceCustomer on SC_ID = SE_SC" + whereService.Format(" where ");
                #endregion
            }

            matchResult.RowID = "SE_ID";

            matchResult.Hide("SE_ID");

            matchResult.Sort("SE_SID", "SID");
            matchResult.Sort("SC_Name", "Customer");

            Topology.Prepare(matchResult);

            matchResult.AddColumn("StreamServiceID"); // 21
        }

        public override void RowProcess(SearchMatchResult matchResult, List<object> objects)
        {
            string seID = (string)objects[2];

            string serviceType = (string)objects[7];
            string serviceSubType = (string)objects[8];

            Topology.Discovery(objects, TopologyDiscoveryTypes.ServiceID, seID, ref serviceType, ref serviceSubType);

            // modify serviceType dan serviceSubType
            objects[7] = serviceType;
            objects[8] = serviceSubType;

            objects.Add(Base64.Encode(seID));
        }

        private static string[] CommonString(string left, string right)
        {
            List<string> result = new List<string>();
            string[] rightArray = right.Split();
            string[] leftArray = left.Split();

            result.AddRange(rightArray.Where(r => leftArray.Any(l => l.StartsWith(r))));

            // must check other way in case left array contains smaller words than right array
            result.AddRange(leftArray.Where(l => rightArray.Any(r => r.StartsWith(l))));

            return result.Distinct().ToArray();
        }

        #endregion
    }
}