using Aphysoft.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Center
{
    public static class JoviceRecovery
    {
        private static Database j = Jovice.Database;

        internal static Necrow instance = null;

        internal static void Event(string message)
        {
            instance.Event(message);
        }

        internal static void NodeDuplication()
        {
            if (Necrow.Version != 31) return;

            Result result, result2, result3, result4;

            Batch batchUpdate1 = j.Batch();
            Batch batchUpdate2 = j.Batch();

            Batch batchDelete1 = j.Batch();
            Batch batchDelete2 = j.Batch();

            result = j.Query("select * from (select NO_Name, COUNT(NO_Name) as co from Node group by NO_Name) a where co > 1 order by co desc");

            if (result.Count > 0)
            {
                Event("Jovice Recovery: Node Duplication initiated");

                foreach (Row row in result)
                {
                    string noname = row["NO_Name"].ToString();
                                       
                    result2 = j.Query("select * from Node where NO_Name = {0} order by NO_TimeStamp desc", noname);

                    if (result2.Count > 1)
                    {
                        // keep 1st, move/delete everyone else

                        // get no_id from 1st = REFERENCE
                        string noid = result2[0]["NO_ID"].ToString();

                        Event("Fixing " + noname + ", reference Node ID is " + noid);

                        #region NodeSummary, NodeSummaryArchive, ProbeProgress

                        Event("Merging ProbeProgress, NodeSummaryArchive...");

                        // create key reference
                        result3 = j.Query("select * from NodeSummary where NS_NO = {0} order by NS_Key", noid);

                        Dictionary<string, string> nsref = new Dictionary<string, string>();

                        foreach (Row row3 in result3)
                        {
                            string key = row3["NS_Key"].ToString();
                            string id = row3["NS_ID"].ToString();

                            if (!nsref.ContainsKey(key)) nsref.Add(key, id);
                        }

                        result3 = j.Query("SELECT ProbeHistory.* from ProbeHistory, Node where XH_NO = NO_ID and NO_Name = {0} order by XH_StartTime", noname);

                        DateTime rdt = DateTime.MinValue;

                        batchUpdate1.Begin();
                        batchUpdate2.Begin();
                        batchDelete1.Begin();
                        batchDelete2.Begin();

                        foreach (Row row3 in result3)
                        {
                            long id = row3["XH_ID"].ToLong();
                            DateTime st = row3["XH_StartTime"].ToDateTime();

                            if (st > rdt)
                            {
                                rdt = row3["XH_EndTime"].ToDateTime();

                                if (row3["XH_NO"].ToString() != noid)
                                {
                                    // masukkan ini
                                    batchUpdate1.Execute("update ProbeHistory set XH_NO = {0} where XH_ID = {1}", noid, id);
                                }

                                // cek semua nodesummaryarchive yg ngebind ProbeHistory ini, update dengan nskey
                                result4 = j.Query("select * from NodeSummaryArchive, NodeSummary where NSX_XH = {0} and NSX_NS = NS_ID", id);

                                foreach (Row row4 in result4)
                                {
                                    string nsno = row4["NS_NO"].ToString();

                                    if (nsno != noid)
                                    {
                                        long nsxid = row4["NSX_ID"].ToLong();

                                        string nskey = row4["NS_Key"].ToString();

                                        if (nsref.ContainsKey(nskey))
                                        {
                                            string nskeyid = nsref[nskey];

                                            // update to reference
                                            batchUpdate2.Execute("update NodeSummaryArchive set NSX_NS = {0} where NSX_ID = {1}", nskeyid, nsxid);
                                        }
                                        else
                                            batchDelete2.Execute("delete from NodeSummaryArchive where NSX_ID = {0}", nsxid);
                                    }
                                }
                            }
                            else
                            {
                                // delete ini
                                batchDelete2.Execute("delete from NodeSummaryArchive where NSX_XH = {0}", id);
                                batchDelete1.Execute("delete from ProbeHistory where XH_ID = {0}", id);
                            }
                        }

                        result3 = batchUpdate1.Commit();
                        if (result3.AffectedRows > 0) Event(result3.AffectedRows + " ProbeHistory redirected");

                        result3 = batchUpdate2.Commit();
                        if (result3.AffectedRows > 0) Event(result3.AffectedRows + " NodeSummaryArchive redirected");

                        result3 = batchDelete2.Commit();
                        if (result3.AffectedRows > 0) Event(result3.AffectedRows + " NodeSummaryArchive deleted");

                        result3 = batchDelete1.Commit();
                        if (result3.AffectedRows > 0) Event(result3.AffectedRows + " ProbeHistory deleted");

                        Event("Deleting NodeSummary...");

                        batchDelete1.Begin();
                        foreach (Row row2 in result2)
                        {
                            string cnoid = row2["NO_ID"].ToString();
                            if (cnoid == noid) continue;

                            Column nsco = j.Scalar("select count(*) from NodeSummary where NS_NO = {0}", cnoid);

                            if (nsco.ToInt() > 0)
                            {
                                batchDelete1.Execute("delete from NodeSummary where NS_ID in (select NS_ID from NodeSummary where NS_NO = {0})", cnoid);
                            }
                        }

                        result3 = batchDelete1.Commit();
                        if (result3.AffectedRows > 0) Event(result3.AffectedRows + " NodeSummary deleted");

                        #endregion

                        #region NodeSlot, NodeAlias, NodeAccessRule

                        Event("Deleting NodeSlot...");

                        batchDelete1.Begin();
                        foreach (Row row2 in result2)
                        {
                            string cnoid = row2["NO_ID"].ToString();
                            if (cnoid == noid) continue;

                            batchDelete1.Execute("delete from NodeSlot where NC_ID in (select NC_ID from NodeSlot where NC_NO = {0})", cnoid);
                        }

                        result3 = batchDelete1.Commit();
                        if (result3.AffectedRows > 0) Event(result3.AffectedRows + " NodeSlot deleted");

                        Event("Deleting NodeAlias...");

                        batchDelete1.Begin();
                        foreach (Row row2 in result2)
                        {
                            string cnoid = row2["NO_ID"].ToString();
                            if (cnoid == noid) continue;

                            batchDelete1.Execute("delete from NodeAlias where NA_ID in (select NA_ID from NodeAlias where NA_NO = {0})", cnoid);
                        }

                        result3 = batchDelete1.Commit();
                        if (result3.AffectedRows > 0) Event(result3.AffectedRows + " NodeAlias deleted");

                        Event("Deleting NodeAccessRule...");

                        batchDelete1.Begin();
                        foreach (Row row2 in result2)
                        {
                            string cnoid = row2["NO_ID"].ToString();
                            if (cnoid == noid) continue;

                            batchDelete1.Execute("delete from NodeAccessRule where NAR_ID in (select NAR_ID from NodeAccessRule where NAR_NO = {0})", cnoid);
                        }

                        result3 = batchDelete1.Commit();
                        if (result3.AffectedRows > 0) Event(result3.AffectedRows + " NodeAccessRule deleted");

                        #endregion

                        #region DerivedRouteNetworkNode, DerivedRouteNetwork

                        Event("Deleting DerivedRouteNetworkNode...");

                        List<string> drnref = new List<string>();

                        batchDelete1.Begin();
                        foreach (Row row2 in result2)
                        {
                            string cnoid = row2["NO_ID"].ToString();
                            if (cnoid == noid) continue;

                            result4 = j.Query("select DRN_ID from DerivedRouteNetworkNode, DerivedRouteNetwork where DRNO_NO = {0} and DRNO_DRN = DRN_ID", cnoid);

                            foreach (Row row4 in result4)
                            {
                                string drnid = row4["DRN_ID"].ToString();

                                if (!drnref.Contains(drnid)) drnref.Add(drnid);
                            }

                            batchDelete1.Execute("delete from DerivedRouteNetworkNode where DRNO_NO = {0}", cnoid);
                        }

                        result3 = batchDelete1.Commit();
                        if (result3.AffectedRows > 0) Event(result3.AffectedRows + " DerivedRouteNetworkNode deleted");

                        Event("Deleting empty DerivedRouteNetwork");

                        batchDelete1.Begin();
                        foreach (string drn in drnref)
                        {
                            Column co = j.Scalar("select count(*) as count from DerivedRouteNetworkNode where DRNO_DRN = {0}", drn);

                            if (co.ToInt() == 0)
                            {
                                // layak untuk di delete
                                batchDelete1.Execute("delete from DerivedRouteNetwork where DRN_ID = {0}", drn);
                            }
                        }

                        result3 = batchDelete1.Commit();
                        if (result3.AffectedRows > 0) Event(result3.AffectedRows + " DerivedRouteNetwork deleted");

                        #endregion

                        #region PEMac, MEMac

                        Event("Deleting PEMac, MEMac...");

                        batchDelete1.Begin();
                        foreach (Row row2 in result2)
                        {
                            string cnoid = row2["NO_ID"].ToString();
                            if (cnoid == noid) continue;

                            batchDelete1.Execute("delete from PEMac where PA_ID in (select PA_ID from PEMac where PA_NO = {0})", cnoid);
                            batchDelete1.Execute("delete from MEMac where MA_ID in (select MA_ID from MEMac where MA_NO = {0})", cnoid);
                        }

                        result3 = batchDelete1.Commit();
                        if (result3.AffectedRows > 0) Event(result3.AffectedRows + " MAC deleted");

                        #endregion

                        #region Reserve

                        Event("Deleting Reserve...");

                        batchDelete1.Begin();
                        foreach (Row row2 in result2)
                        {
                            string cnoid = row2["NO_ID"].ToString();
                            if (cnoid == noid) continue;

                            batchDelete1.Execute("delete from Reserve where RE_ID in (select RE_ID from Reserve where RE_NO = {0})", cnoid);
                        }

                        result3 = batchDelete1.Commit();
                        if (result3.AffectedRows > 0) Event(result3.AffectedRows + " Reserve deleted");

                        #endregion

                        #region Interface Null

                        Event("Release all references back and forth from PEInterface, MEInterface...");

                        batchUpdate1.Begin();
                        batchUpdate2.Begin();
                        foreach (Row row2 in result2)
                        {
                            string cnoid = row2["NO_ID"].ToString();
                            if (cnoid == noid) continue;

                            batchUpdate1.Execute("update PEInterface set PI_PN = NULL, PI_PQ_Input = NULL, PI_PQ_Output = NULL, PI_SE = NULL, PI_PI = NULL, PI_TO_MI = NULL, PI_TO_NI = NULL where PI_ID in (select PI_ID from PEInterface where PI_NO = {0})", cnoid);
                            batchUpdate2.Execute("update MEInterface set MI_MC = NULL, MI_MQ_Input = NULL, MI_MQ_Output = NULL, MI_SE = NULL, MI_MI = NULL, MI_TO_PI = NULL, MI_TO_MI = NULL, MI_TO_NI = NULL where MI_ID in (select MI_ID from MEInterface where MI_NO = {0})", cnoid);
                        }

                        result3 = batchUpdate1.Commit();
                        if (result3.AffectedRows > 0) Event(result3.AffectedRows + " PE Interfaces nulled");

                        result3 = batchUpdate2.Commit();
                        if (result3.AffectedRows > 0) Event(result3.AffectedRows + " ME interfaces nulled");

                        Event("Release all references from other interfaces to these interfaces...");

                        batchUpdate1.Begin();
                        foreach (Row row2 in result2)
                        {
                            string cnoid = row2["NO_ID"].ToString();
                            if (cnoid == noid) continue;

                            batchUpdate1.Execute("update PEInterface set PI_TO_MI = NULL where PI_TO_MI in (select MI_ID from MEInterface where MI_NO = {0})", cnoid);
                            batchUpdate1.Execute("update MEInterface set MI_TO_PI = NULL where MI_TO_PI in (select PI_ID from PEInterface where PI_NO = {0})", cnoid);
                            batchUpdate1.Execute("update MEInterface set MI_TO_MI = NULL where MI_TO_MI in (select MI_ID from MEInterface where MI_NO = {0})", cnoid);
                        }

                        result3 = batchUpdate1.Commit();
                        if (result3.AffectedRows > 0) Event(result3.AffectedRows + " referenced interfaces nulled");

                        #endregion

                        #region PERouteName, PERouteUse

                        batchDelete1.Begin();
                        batchDelete2.Begin();

                        foreach (Row row2 in result2)
                        {
                            string cnoid = row2["NO_ID"].ToString();
                            if (cnoid == noid) continue;

                            batchDelete2.Execute("delete from PERouteUse where PU_PN in (select PN_ID from PERouteName where PN_NO = {0})", cnoid);
                            batchDelete1.Execute("delete from PERouteName where PN_ID in (select PN_ID from PERouteName where PN_NO = {0})", cnoid);
                        }

                        Event("Deleting PERouteUse...");

                        result3 = batchDelete2.Commit();
                        if (result3.AffectedRows > 0) Event(result3.AffectedRows + " PERouteUse deleted");

                        Event("Deleting PERouteName...");

                        result3 = batchDelete1.Commit();
                        if (result3.AffectedRows > 0) Event(result3.AffectedRows + " PERouteName deleted");

                        #endregion

                        #region PEQOS, MEQOS

                        Event("Deleting PEQOS, MEQOS...");

                        batchDelete1.Begin();
                        foreach (Row row2 in result2)
                        {
                            string cnoid = row2["NO_ID"].ToString();
                            if (cnoid == noid) continue;

                            batchDelete1.Execute("delete from PEQOS where PQ_ID in (select PQ_ID from PEQOS where PQ_NO = {0})", cnoid);
                            batchDelete1.Execute("delete from MEQOS where MQ_ID in (select MQ_ID from MEQOS where MQ_NO = {0})", cnoid);
                        }

                        result3 = batchDelete1.Commit();
                        if (result3.AffectedRows > 0) Event(result3.AffectedRows + " QOS deleted");

                        #endregion

                        #region PEPrefixList, PEPrefixEntry

                        Event("Deleting PEPrefixList, PEPrefixEntry...");

                        batchDelete1.Begin();
                        batchDelete2.Begin();
                        foreach (Row row2 in result2)
                        {
                            string cnoid = row2["NO_ID"].ToString();
                            if (cnoid == noid) continue;

                            batchDelete1.Execute("delete from PEPrefixEntry where PY_PX in (select PX_ID from PEPrefixList where PX_NO = {0})", cnoid);
                            batchDelete2.Execute("delete from PEPrefixList where PX_ID in (select PX_ID from PEPrefixList where PX_NO = {0})", cnoid);
                        }

                        result3 = batchDelete1.Commit();
                        if (result3.AffectedRows > 0) Event(result3.AffectedRows + " PEPrefixEntry deleted");

                        result3 = batchDelete2.Commit();
                        if (result3.AffectedRows > 0) Event(result3.AffectedRows + " PEPrefixList deleted");

                        #endregion

                        #region PEInterfaceIP

                        Event("Deleting PEInterfaceIP...");

                        batchDelete1.Begin();
                        foreach (Row row2 in result2)
                        {
                            string cnoid = row2["NO_ID"].ToString();
                            if (cnoid == noid) continue;

                            batchDelete1.Execute("delete from PEInterfaceIP where PP_ID in (select PP_ID from PEInterfaceIP where PP_PI in (select PI_ID from PEInterface where PI_NO = {0}))", cnoid);
                        }

                        result3 = batchDelete1.Commit();
                        if (result3.AffectedRows > 0) Event(result3.AffectedRows + " PEInterfaceIP deleted");

                        #endregion

                        #region MEPeer

                        Event("Deleting MEPeer...");

                        batchDelete1.Begin();
                        foreach (Row row2 in result2)
                        {
                            string cnoid = row2["NO_ID"].ToString();
                            if (cnoid == noid) continue;

                            batchDelete1.Execute("delete from MEPeer where MP_ID in (select MP_ID from MEPeer where MP_MC in (select MC_ID from MECircuit where MC_NO = {0}))", cnoid);
                        }

                        result3 = batchDelete1.Commit();
                        if (result3.AffectedRows > 0) Event(result3.AffectedRows + " MEPeer deleted");

                        #endregion

                        #region MESDP

                        Event("Deleting MESDP...");

                        batchDelete1.Begin();
                        foreach (Row row2 in result2)
                        {
                            string cnoid = row2["NO_ID"].ToString();
                            if (cnoid == noid) continue;

                            batchDelete1.Execute("delete from MESDP where MS_ID in (select MS_ID from MESDP where MS_NO = {0})", cnoid);
                        }

                        result3 = batchDelete1.Commit();
                        if (result3.AffectedRows > 0) Event(result3.AffectedRows + " MESDP deleted");

                        Event("Redirect all references to Node from other MESDP to referenced node...");

                        batchUpdate1.Begin();

                        foreach (Row row2 in result2)
                        {
                            string cnoid = row2["NO_ID"].ToString();
                            if (cnoid == noid) continue;

                            batchUpdate1.Execute("update MESDP set MS_TO_NO = {0} where MS_TO_NO = {1}", noid, cnoid);
                        }

                        result3 = batchUpdate1.Commit();
                        if (result3.AffectedRows > 0) Event(result3.AffectedRows + " MESDP redirected");


                        #endregion

                        #region MECircuit

                        Event("Deleting MECircuit...");

                        batchUpdate1.Begin();
                        batchDelete1.Begin();
                        foreach (Row row2 in result2)
                        {
                            string cnoid = row2["NO_ID"].ToString();
                            if (cnoid == noid) continue;

                            batchUpdate1.Execute("update MEPeer set MP_TO_MC = NULL where MP_TO_MC in (select MC_ID from MECircuit where MC_NO = {0})", cnoid);
                            batchDelete1.Execute("delete from MECircuit where MC_ID in (select MC_ID from MECircuit where MC_NO = {0})", cnoid);
                        }

                        result3 = batchUpdate1.Commit();
                        if (result3.AffectedRows > 0) Event(result3.AffectedRows + " references to MECircuit nulled");

                        result3 = batchDelete1.Commit();
                        if (result3.AffectedRows > 0) Event(result3.AffectedRows + " MECircuit deleted");

                        #endregion

                        #region MECustomer

                        Event("Deleting MECustomer...");

                        batchDelete1.Begin();
                        foreach (Row row2 in result2)
                        {
                            string cnoid = row2["NO_ID"].ToString();
                            if (cnoid == noid) continue;

                            batchDelete1.Execute("delete from MECustomer where MU_ID in (select MU_ID from MECustomer where MU_NO = {0})", cnoid);
                        }

                        result3 = batchDelete1.Commit();
                        if (result3.AffectedRows > 0) Event(result3.AffectedRows + " MECustomer deleted");

                        #endregion

                        #region Interfaces

                        Event("Deleting PEInterfaces, MEInterfaces...");

                        batchDelete1.Begin();
                        batchDelete2.Begin();
                        foreach (Row row2 in result2)
                        {
                            string cnoid = row2["NO_ID"].ToString();
                            if (cnoid == noid) continue;

                            batchDelete1.Execute("delete from DerivedAreaConnection where DAC_MI_1 in (select MI_ID from MEInterface where MI_NO = {0})", cnoid);
                            batchDelete1.Execute("delete from DerivedAreaConnection where DAC_MI_2 in (select MI_ID from MEInterface where MI_NO = {0})", cnoid);

                            batchDelete2.Execute("delete from MEInterface where MI_ID in (select MI_ID from MEInterface where MI_NO = {0})", cnoid);
                            batchDelete2.Execute("delete from PEInterface where PI_ID in (select PI_ID from PEInterface where PI_NO = {0})", cnoid);
                        }

                        result3 = batchDelete1.Commit();
                        if (result3.AffectedRows > 0) Event(result3.AffectedRows + " DAC deleted");

                        result3 = batchDelete2.Commit();
                        if (result3.AffectedRows > 0) Event(result3.AffectedRows + " interfaces deleted");

                        #endregion

                        #region Node

                        Event("Deleting Nodes...");

                        foreach (Row row2 in result2)
                        {
                            string cnoid = row2["NO_ID"].ToString();
                            if (cnoid == noid) continue;

                            j.Execute("delete from Node where NO_ID = {0}", cnoid);

                            Event(row2["NO_Name"].ToString() + " deleted");
                        }
                        
                        #endregion
                    }
                }
            }
        }
    }
}
