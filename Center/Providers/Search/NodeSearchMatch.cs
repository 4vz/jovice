using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Center.Providers
{
    public class NodeSearchMatch : SearchMatch
    {
        #region Constructors

        public NodeSearchMatch()
        {
            Root("node");
            Root("nodes");
            Root("network element");
            Root("network elements");
            Root("ne");
        }

        #endregion

        #region Methods

        public override void Process(SearchMatchResult matchResult, SearchMatchQuery matchQuery)
        {
            matchResult.Type = "node";

            matchResult.QueryCount = "select NO_ID from Node where NO_Active = 1 and NO_Type in ('M', 'P')";
            matchResult.Query = @"
select NO_ID, NO_Name, AR_Name, AW_Name, AG_Name,
(
CAST(ns1.NS_Value as float) / 100 +
CAST(ns3.NS_Value as float) / CAST(ns2.NS_Value as float) +
ISNULL(CAST(ns5.NS_Value as float) / CAST(ns4.NS_Value as float), 0) +
ISNULL(CAST(ns7.NS_Value as float) / CAST(ns6.NS_Value as float), 0) +
ISNULL(CAST(ns9.NS_Value as float) / CAST(ns8.NS_Value as float), 0) +

(CASE WHEN ns5.NS_Value IS NOT NULL AND ns10.NS_Value IS NOT NULL THEN 
CAST(ns10.NS_Value as float) / (500 * CAST(ns5.NS_Value as float))
ELSE 0 END) +
(CASE WHEN ns7.NS_Value IS NOT NULL AND ns11.NS_Value IS NOT NULL THEN 
CAST(ns11.NS_Value as float) / (500 * CAST(ns7.NS_Value as float))
ELSE 0 END) +
(CASE WHEN ns9.NS_Value IS NOT NULL AND ns11.NS_Value IS NOT NULL THEN 
CAST(ns12.NS_Value as float) / (500 * CAST(ns9.NS_Value as float))
ELSE 0 END)

) / (2 +
(CASE WHEN ns5.NS_Value IS NOT NULL THEN 1 ELSE 0 END) + 
(CASE WHEN ns7.NS_Value IS NOT NULL THEN 1 ELSE 0 END) + 
(CASE WHEN ns9.NS_Value IS NOT NULL THEN 1 ELSE 0 END) +
(CASE WHEN ns10.NS_Value IS NOT NULL THEN 1 ELSE 0 END) +
(CASE WHEN ns11.NS_Value IS NOT NULL THEN 1 ELSE 0 END) +
(CASE WHEN ns12.NS_Value IS NOT NULL THEN 1 ELSE 0 END)
) as UR,
ISNULL(CAST(ns1.NS_Value as int), -1) as CPU,
CAST(ns2.NS_Value as int) as MEMORY_TOTAL,
CAST(ns3.NS_Value as int) as MEMORY_USED,
CAST(ns4.NS_Value as int) as INTERFACE_COUNT_FA, 
CAST(ns5.NS_Value as int) as INTERFACE_COUNT_FA_UP, 
CAST(ns6.NS_Value as int) as INTERFACE_COUNT_GI, 
CAST(ns7.NS_Value as int) as INTERFACE_COUNT_GI_UP, 
CAST(ns8.NS_Value as int) as INTERFACE_COUNT_TE, 
CAST(ns9.NS_Value as int) as INTERFACE_COUNT_TE_UP,
CAST(ns10.NS_Value as int) as SUBINTERFACE_COUNT_FA,
CAST(ns11.NS_Value as int) as SUBINTERFACE_COUNT_GI,
CAST(ns12.NS_Value as int) as SUBINTERFACE_COUNT_TE,
ISNULL(CAST(ns4.NS_Value as int), 0) + ISNULL(CAST(ns6.NS_Value as int), 0) + ISNULL(CAST(ns8.NS_Value as int), 0) as INTERFACE_COUNT,
ISNULL(CAST(ns5.NS_Value as int), 0) + ISNULL(CAST(ns7.NS_Value as int), 0) + ISNULL(CAST(ns9.NS_Value as int), 0) as INTERFACE_COUNT_UP,
ISNULL(CAST(ns3.NS_Value as float) / CAST(ns2.NS_Value as float), 0) as MEMORY_PERCENTAGE
from Node
left join Area on NO_AR = AR_ID
left join AreaWitel on AR_AW = AW_ID
left join AreaGroup on AW_AG = AG_ID
left join NodeSummary ns1 on ns1.NS_NO = NO_ID and ns1.NS_Key = 'CPU'
left join NodeSummary ns2 on ns2.NS_NO = NO_ID and ns2.NS_Key = 'MEMORY_TOTAL'
left join NodeSummary ns3 on ns3.NS_NO = NO_ID and ns3.NS_Key = 'MEMORY_USED'
left join NodeSummary ns4 on ns4.NS_NO = NO_ID and ns4.NS_Key = 'INTERFACE_COUNT_FA' and ns4.NS_Value <> '0'
left join NodeSummary ns5 on ns5.NS_NO = NO_ID and ns5.NS_Key = 'INTERFACE_COUNT_FA_UP' and ns5.NS_Value <> '0'
left join NodeSummary ns6 on ns6.NS_NO = NO_ID and ns6.NS_Key = 'INTERFACE_COUNT_GI' and ns6.NS_Value <> '0'
left join NodeSummary ns7 on ns7.NS_NO = NO_ID and ns7.NS_Key = 'INTERFACE_COUNT_GI_UP' and ns7.NS_Value <> '0'
left join NodeSummary ns8 on ns8.NS_NO = NO_ID and ns8.NS_Key = 'INTERFACE_COUNT_TE' and ns8.NS_Value <> '0'
left join NodeSummary ns9 on ns9.NS_NO = NO_ID and ns9.NS_Key = 'INTERFACE_COUNT_TE_UP' and ns9.NS_Value <> '0'
left join NodeSummary ns10 on ns10.NS_NO = NO_ID and ns10.NS_Key = 'SUBINTERFACE_COUNT_FA' and ns10.NS_Value <> '0'
left join NodeSummary ns11 on ns11.NS_NO = NO_ID and ns11.NS_Key = 'SUBINTERFACE_COUNT_GI' and ns11.NS_Value <> '0'
left join NodeSummary ns12 on ns12.NS_NO = NO_ID and ns12.NS_Key = 'SUBINTERFACE_COUNT_TE' and ns12.NS_Value <> '0'

where NO_Active = 1 and NO_Type in ('M', 'P')
";
            matchResult.RowID = "NO_ID";
            matchResult.Hide("NO_ID");

            matchResult.Sort("NO_Name", "Name");
            matchResult.Sort("UR", "Usage Rating", true);
            matchResult.Sort("CPU", "CPU", true);
            matchResult.Sort("MEMORY_PERCENTAGE", "Memory", true);
        }

        public override void RowProcess(SearchMatchResult matchResult, List<object> objects)
        {
        }

        #endregion
    }
}