using Aphysoft.Share;
using Jovice;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Necrow
{
    internal class ServiceCustomerToDatabase : Data2
    {
        public string Name { get; set; }

        public string AltName { get; set; }

        public string NAccountNumber { get; set; }

        public string TAbbr { get; set; }
    }

    public partial class Necrow
    {
        private Thread serviceDiscoveryThread = null;

        //private Dictionary<string, Row> serviceCustomerByAccountNumber = null;

        private void BeginServiceDiscovery()
        {
            serviceDiscoveryThread = new Thread(new ThreadStart(ProcessServiceDiscovery));
            serviceDiscoveryThread.Start();
        }

        private void ProcessServiceDiscovery()
        {
            Batch b = jovice.Batch();

            DateTime nossfSeLastCheck = DateTime.MinValue;
            DateTime seLastCheck = DateTime.MinValue;

            while (true)
            {
                bool nossfReady = nossf && nossf.Query("select AGREENUM from DWH_SALES.eas_ncrm_agree_order_line where ROWNUM = 1");
                //bool desdbnReady = desdbn && desdbn.Query("select PERIODE from NF_AKTIF_VPNIP_FINAL where ROWNUM = 1");
                bool dbwinsReady = dbwins && dbwins.Query("select PRODUCT_NAME from DOK_DWS.UIM_SERVICES where ROWNUM = 1");
                //
                // ADD SERVICEIMMEDIATE FOR SERVICE THAT HAVE NONE
                //
                if (jovice.Query(out Result2 r2, @"
select SE_ID, SE_SID
from Service
left join ServiceImmediate on SI_SE = SE_ID
where SI_ID is null
"))
                {
                    b.Begin();

                    foreach (Row2 row2 in r2)
                    {
                        Result2 r21 = jovice.Query("select * from ServiceImmediate where SI_VID = {0}", row2["SE_SID"]);

                        if (r21 > 0)
                        {
                            foreach (Row2 row21 in r21)
                            {
                                // so this one, update the ServiceImmediate with SI_VID
                                Update siu = jovice.Update("ServiceImmediate");
                                siu.Where("SI_ID", row21["SI_ID"]);
                                siu.Set("SI_SE", row2["SE_ID"]);
                                siu.Set("SI_SE_Check", true);

                                b.Add(siu);
                            }
                        }
                        else
                        {
                            Insert sii = jovice.Insert("ServiceImmediate");
                            sii.Key("SI_ID");
                            sii.Value("SI_VID", row2["SE_SID"]);
                            sii.Value("SI_SE", row2["SE_ID"]);
                            sii.Value("SI_SE_Check", true);

                            b.Add(sii);
                        }
                    }

                    b.Commit();
                }
                
                //
                // DELETE SERVICEIMMEDIATES THAT WERE NOT REFERENCED FROM INTERFACES/CIRCUITS
                //
                jovice.Execute(@"delete from ServiceImmediate where SI_ID in (
select SI_ID from ServiceImmediate where SI_ID not in (
select PI_SI from PEInterface where PI_SI is not null
union
select MI_SI from MEInterface where MI_SI is not null
union
select MC_SI from MECircuit where MC_SI is not null
) and SI_SE is null and SI_SE_Check = 1
)");
                
                //
                // DELETE MULTIPLE SERVICEIMMEDIATE VID
                //
                if (jovice.Query(out Result2 r3, @"
select SI_SE, SI_ID, RefID, RefTable
from (
select si2.SI_SE, si2.SI_ID, COUNT(si2.SI_ID) as refid_count from 
ServiceImmediate si2, 
(select SI_VID, COUNT(SI_VID) as sivid_count from ServiceImmediate where SI_SE is not null group by SI_VID) si1, 
(select PI_SI as ID, PI_ID as RefID, 'PI' as RefTable from PEInterface where PI_SI is not null union
select MI_SI as ID, MI_ID as RefID, 'MI' as RefTable from MEInterface where MI_SI is not null union
select MC_SI as ID, MC_ID as RefID, 'MC' as RefTable from MECircuit where MC_SI is not null) ref 
where si1.sivid_count > 1 
and si1.SI_VID = si2.SI_VID 
and ref.ID = si2.SI_ID 
group by si2.SI_ID, si2.SI_SE
) ord, (
select PI_SI as ID,  PI_ID as RefID, 'PI' as RefTable from PEInterface where PI_SI is not null union
select MI_SI as ID, MI_ID as RefID, 'MI' as RefTable from MEInterface where MI_SI is not null union
select MC_SI as ID, MC_ID as RefID, 'MC' as RefTable from MECircuit where MC_SI is not null) ref2
where SI_ID = ref2.ID
order by SI_SE, refid_count desc
"))
                {
                    string currentSE = null;
                    string currentToSI = null;

                    Batch bUpdate = jovice.Batch();
                    Batch bDelete = jovice.Batch();

                    foreach (Row2 row3 in r3)
                    {
                        string se = row3["SI_SE"];
                        string si = row3["SI_ID"];
                        string rid = row3["RefID"];
                        string tab = row3["RefTable"];

                        if (currentSE != se)
                        {
                            currentSE = se;
                            currentToSI = si;
                        }

                        if (currentToSI != si)
                        {
                            //tab.rid update _si to currentToSI
                            string table = null;

                            if (tab == "PI") table = "PEInterface";
                            else if (tab == "MI") table = "MEInterface";
                            else if (tab == "MC") table = "MECircuit";

                            Update riu = jovice.Update(table);
                            riu.Set($"{tab}_SI", currentToSI);
                            riu.Where($"{tab}_ID", rid);

                            bUpdate.Add(riu);
                            bDelete.Add("delete from ServiceImmediate where SI_ID = {0}", si);
                        }
                    }

                    bUpdate.Commit();
                    bDelete.Commit();
                }

                //
                // CHECK FOR NEW SERVICE FROM SERVICEIMMEDIATE
                //
                if (nossfReady && dbwinsReady)
                {
                    bool seCheck = false;
                    if ((DateTime.UtcNow - seLastCheck) > TimeSpan.FromMinutes(5))
                    {
                        seLastCheck = DateTime.UtcNow;
                        seCheck = true;
                    }

                    // get all types except TS (Telkomsel Sites)
                    if (jovice.Query(out Result2 r4, @"
select top 1000 SI_ID, SI_VID from ServiceImmediate where 
SI_SE is NULL and (SI_Type in ('AS', 'AB', 'VP', 'VI', 'TA', 'IT') or SI_Type is null) and SI_SE_Check = {0}
order by newid()
", seCheck))
                    { 
                        foreach (Row2 row4 in r4)
                        {
                            string siid = row4["SI_ID"].ToString();
                            string vid = row4["SI_VID"].ToString();

                            string seid = null;

                            if (jovice.Query(out Row2 serow, "select SE_ID from Service where SE_SID = {0}", vid))
                            {
                                // search from Service
                                seid = serow["SE_ID"];
                            }
                            else if (jovice.Query(out Row2 sorow, "select SE_ID, SE_SID from Service, ServiceOrder where SE_ID = SO_SE and SO_OID = {0}", vid))
                            {
                                // search from Order
                                seid = sorow["SE_ID"];
                            }
                            else
                            {
                                // USING NOSSF
                                string nossfsid = null;

                                if (nossf.Query(out Row2 norow, "select LI_SID from DWH_SALES.eas_ncrm_agree_order_line where ORDER_ID = {0} and LI_SID is not null and LI_STATUS = 'Complete'", vid))
                                    nossfsid = norow["LI_SID"];
                                else if (nossf.Query(out Row2 norow2, "select LI_SID from DWH_SALES.eas_ncrm_agree_order_line where li_sid = {0} and ROWNUM = 1", vid))
                                    nossfsid = vid;

                                if (nossfsid != null)
                                {
                                    if (jovice.Query(out Row2 serow2, "select * from Service where SE_SID = {0}", nossfsid))
                                        seid = serow2["SE_ID"]; // using existing seid from service
                                    else
                                    {
                                        seid = Database2.ID();

                                        if (!OSS.RefreshOrder(nossfsid, seid, "nossf", true)) // using new service, refresh from nossf db (dwhnas)
                                            seid = null;
                                    }
                                }

                                if (seid == null)
                                {
                                    // seid is still null, USING DBWINS
                                    if (dbwins.Query(out Row2 wirow, "select * from DOK_DWS.SERVICE_INFO_DATIN where CFS_ID = {0}", vid))
                                    {
                                        seid = Database2.ID();

                                        if (!OSS.RefreshOrder(vid, seid, "dbwins", true))
                                            seid = null;
                                    }




                                }
                            }


                            if (seid != null)
                            {
                                Update siu = jovice.Update("ServiceImmediate");
                                siu.Set("SI_SE_Check", true);
                                siu.Set("SI_SE", seid);
                                siu.Where("SI_ID", siid);
                                siu.Execute();
                            }
                        }
                    }
                }
                
                //                
                // REFRESH NEW SERVICES FROM OSS
                //
                if (nossfReady)
                {
                    if ((DateTime.UtcNow - nossfSeLastCheck) > TimeSpan.FromHours(6))
                    {
                        nossfSeLastCheck = DateTime.UtcNow;

                        if (nossf.Query(out Result2 r1, @"select distinct LI_SID from DWH_SALES.eas_ncrm_agree_order_line where LI_SID is not null order by DBMS_RANDOM.VALUE"))
                        {
                            foreach (Row2 row1 in r1)
                            {
                                string sesid = row1["LI_SID"];

                                Result2 r11 = jovice.Query("select SE_ID from Service where SE_SID = {0}", sesid);

                                string sid = null;
                                bool newService = false;

                                if (r11 == 0)
                                {
                                    sid = Database2.ID();
                                    newService = true;
                                }
                                else
                                {
                                    sid = r11[0]["SE_ID"];
                                    newService = false;
                                }

                                bool ok = OSS.RefreshOrder(sesid, sid, "nossf", newService);

                                if (!ok) break;
                            }
                        }
                        if (dbwins.Query(out Result2 dr1, @"select distinct LI_SID from NCX_WIB.DETAIL_WFM_ATTR_NCX_MYCARRIER where LI_SID is not null order by DBMS_RANDOM.VALUE"))
                        {
                            foreach (Row2 row1 in dr1)
                            {
                                string sesid = row1["LI_SID"];

                                Result2 r11 = jovice.Query("select SE_ID from Service where SE_SID = {0}", sesid);

                                string sid = null;
                                bool newService = false;

                                if (r11 == 0)
                                {
                                    sid = Database2.ID();
                                    newService = true;
                                }
                                else
                                {
                                    sid = r11[0]["SE_ID"];
                                    newService = false;
                                }

                                bool ok = OSS.RefreshOrder(sesid, sid, "dbwins", newService);

                                if (!ok) break;
                            }
                        }
                        
                    }
                }

                Thread.Sleep(20000);
            }
        }

        private void EndServiceDiscovery()
        {
            serviceDiscoveryThread.Abort();
        }

        private Regex nossfAlternateNameRegex = new Regex(@"\(([a-zA-Z\s\d]+)\)");

        private (string name, string altname) DetermineCustomerName(string name)
        {
            string un = null;
            string an = null;

            if (name != null)
            {
                StringBuilder nameSB = new StringBuilder(name.ToUpper().NormalizeWhiteSpace());

                nameSB.Remove("(PERSERO)");

                StringBuilder altnameSB = new StringBuilder();

                int id = 0;
                foreach (Match m in nossfAlternateNameRegex.Matches(nameSB.ToString()))
                {
                    if (m.Success)
                    {
                        Group g = m.Groups[1];
                        if (id == 0)
                        {
                            altnameSB.Append(g.Value.Trim());
                        }
                        id++;
                        nameSB.ReplaceWithWhiteSpace(g.Index - 1, g.Length + 2);
                    }
                }
                
                nameSB.RemoveCharactersExcept(Characters.Alphanumeric, Characters.WhiteSpace);
                altnameSB.RemoveCharactersExcept(Characters.Alphanumeric, Characters.WhiteSpace);

                nameSB.Remove("PT ", "CV ", "TBK", " PT", " CV");
                altnameSB.Remove("PT ", "CV ", "TBK", " PT", " CV");

                nameSB.NormalizeWhiteSpace();
                altnameSB.NormalizeWhiteSpace();

                un = nameSB.ToString().Trim();
                string anx = altnameSB.ToString().Trim();

                if (anx.Length > 0) an = anx;

                if (an != null)
                {
                    if (un.Length < an.Length)
                    {
                        string t = un;
                        un = an;
                        an = t;
                    }
                }
            }

            return (un, an);
        }
    }

}
