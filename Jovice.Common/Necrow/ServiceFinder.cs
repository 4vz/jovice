using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Aphysoft.Common;
using Aphysoft.Share;

namespace Jovice
{
    internal static class ServiceFinder
    {
        #region Fields

        private static Thread mainLoop = null;

        private static bool stop = false;

        private static bool idle = false;

        #endregion

        #region Methods

        private static void Event(string message)
        {
            Necrow.Event(message, "SERV");
        }

        public static void Start()
        {
            if (mainLoop == null)
                mainLoop = new Thread(new ThreadStart(MainLoop));

            stop = false;
            idle = false;

            mainLoop.Start();
        }

        public static void Stop()
        {
            Event("Stopping...");

            stop = true;

            if (idle)
            {
                mainLoop.Abort();
            }
        }

        private static void MainLoop()
        {
            Culture.Default();

            Event("Started");

            Database j = Necrow.JoviceDatabase;

            while (!stop)
            {

                Result result = j.Query(@"
select top 300 PI_ID as EID, PI_Description as DE, 'P' as TY
from PEInterface where PI_Description is not null and PI_SE_Check is null
union all
select top 300 MI_ID as EID, MI_Description as DE, 'I' as TY
from MEInterface where MI_Description is not null and MI_SE_Check is null
union all
select top 700 MC_ID as EID, MC_Description as DE, 'M' as TY
from MECircuit where MC_Description is not null and MC_SE_Check is null
order by EID
" );

                if (result.Count > 0)
                {
                    foreach (Row row in result)
                    {
                        Thread.Sleep(100);

                        string desc = row["DE"].ToString();
                        string eid = row["EID"].ToString();
                        string type = row["TY"].ToString();
                        string servid = null;

                        if (type == "M")
                        {
                            if (desc.EndsWith("_GROUP") ||
                                desc.StartsWith("RNC-"))
                            {
                                desc = null;
                            }
                        }

                        if (desc != null)
                        {
                            ServiceDescriptions d = ServiceDescriptions.Parse(desc);

                            string sid = d.SID;
                            string cid = d.CustID;
                            string stype = d.ServiceType;
                            string ssubtype = d.ServiceSubType;
                            string cdesc = d.CleanDescription;

                            if (sid != null)
                            {
                                string servicetype = null;
                                string servicesubtype = null;

                                if (stype == "VPNIP")
                                {
                                    servicetype = "VP";
                                    if (ssubtype == "TRANS") servicesubtype = "TA";
                                }
                                else if (stype == "ASTINET") servicetype = "AS";
                                else if (stype == "ASTINETBB") servicetype = "AB";
                                else if (stype == "VPNINSTAN") servicetype = "VI";

                                string custid = null;
                                bool custnameset = false;
                                string custname = null;

                                bool nameprocessing = false;

                                if (cid != null)
                                {
                                    Result result3 = j.Query("select * from ServiceCustomer where SC_CID = {0}", cid);

                                    if (result3.Count == 1)
                                    {
                                        custid = result3[0]["SC_ID"].ToString();
                                        custname = result3[0]["SC_Name"].ToString();
                                        custnameset = result3[0]["SC_Name_Set"].ToBoolean(false);
                                    }
                                    else
                                    {
                                        custid = Database.ID();
                                        j.Execute("insert into ServiceCustomer(SC_ID, SC_CID) values({0}, {1})", custid, cid);
                                    }
                                }

                                Result result2 = j.Query("select SE_ID, SE_Type from Service where SE_SID = {0}", sid);

                                if (result2.Count == 1)
                                {
                                    Row rowserv = result2[0];
                                    servid = rowserv["SE_ID"].ToString();
                                    if (rowserv["SE_Type"].ToString() == null && servicetype != null)
                                        j.Execute("update Service set SE_Type = {0}, SE_SubType = {1} where SE_ID = {2}", servicetype, servicesubtype, servid);
                                }
                                else
                                {
                                    servid = Database.ID();
                                    j.Execute("insert into Service(SE_ID, SE_SID, SE_SC, SE_Type, SE_SubType, SE_Raw_Desc) values({0}, {1}, {2}, {3}, {4}, {5})",
                                        servid, sid, custid, servicetype, servicesubtype, cdesc);
                                }

                                if (custid != null)
                                {
                                    if (!custnameset)
                                    {
                                        nameprocessing = true;
                                    }
                                }

                                // name it
                                if (nameprocessing)
                                {
                                    #region Name Processing

                                    Result result3 = j.Query("select SE_Raw_Desc from Service where SE_SC = {0} and SE_Raw_Desc is not null", custid);

                                    List<string> nems = new List<string>();
                                    foreach (Row rownem in result3)
                                    {
                                        string[] rds = rownem["SE_Raw_Desc"].ToString()
                                            .Split(
                                            new string[] { ",", "U/", "JL.", "JL ", "(", "[", "]", ")", "LT.", " LT ", " LT",
                                            "GD.", " KM", " KOMP.", " BLOK ",
                                            " SID ", " SID:", " SID-", " SID=",
                                            " CID ", " CID:", " CID-", " CID=", " CID.", "EXCID", " JL", " EX ",
                                            " FAA:", " FAI:", " FAA-", " FAI-", " CINTA ",
                                            " EX-",
                                            " KK ", "TBK", "BANDWIDTH",  },
                                            StringSplitOptions.RemoveEmptyEntries);

                                        if (rds.Length > 0)
                                            nems.Add(rds[0].Trim());
                                    }

                                    //nems.Sort((a, b) => b.Length.CompareTo(a.Length));

                                    Dictionary<string, int> lexicals = new Dictionary<string, int>();

                                    int totaln = 0;
                                    foreach (string nem in nems)
                                    {
                                        string[] nemp = nem.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);

                                        for (int ni = 0; ni < nemp.Length; ni++)
                                        {
                                            for (int nj = 1; nj <= nemp.Length - ni; nj++)
                                            {
                                                string[] subn = new string[nj];
                                                Array.Copy(nemp, ni, subn, 0, nj);

                                                string sub = string.Join(" ", subn);

                                                if (lexicals.ContainsKey(sub))
                                                {
                                                    lexicals[sub] += 1;
                                                    totaln++;
                                                }
                                                else
                                                    lexicals.Add(sub, 1);


                                            }
                                        }
                                    }

                                    List<KeyValuePair<string, int>> lexicalList = lexicals.ToList();

                                    if (lexicalList.Count > 0)
                                    {
                                        lexicalList.Sort((a, b) =>
                                        {
                                            if (a.Value > b.Value) return -1;
                                            else if (a.Value < b.Value) return 1;
                                            else
                                            {
                                                if (a.Key.Length > b.Key.Length) return -1;
                                                else if (a.Key.Length < b.Key.Length) return 1;
                                                else return 0;
                                            }
                                        });

                                        string cname = lexicalList[0].Key;

                                        if (lexicalList[0].Value > 1)
                                        {


                                            for (int li = 0; li < (lexicalList.Count > 10 ? 10 : lexicalList.Count); li++)
                                            {
                                                KeyValuePair<string, int> lip = lexicalList[li];

                                                string likey = lip.Key;
                                                int lival = lip.Value;
                                                int likeylen = StringHelper.CountWord(likey);

                                                bool lolos = true;

                                                for (int ly = li + 1; ly < (lexicalList.Count > 10 ? 10 : lexicalList.Count); ly++)
                                                {
                                                    KeyValuePair<string, int> lyp = lexicalList[ly];

                                                    string lykey = lyp.Key;
                                                    int lyval = lyp.Value;
                                                    int lykeylen = StringHelper.CountWord(lykey);

                                                    if (lykey.Length > likey.Length)
                                                    {
                                                        if (likeylen > 1)
                                                        {
                                                            if (lykey.IndexOf(likey) > -1)
                                                            {
                                                                int distance = lival - lyval;

                                                                double dtotaln = (double)totaln;
                                                                double minx = Math.Pow((1 / Math.Log(0.08 * dtotaln + 3)), 1.75);
                                                                if (((double)distance / dtotaln) > minx)
                                                                { }
                                                                else
                                                                    lolos = false;
                                                                break;
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (lykey.IndexOf(likey) > -1)
                                                            {
                                                                if ((ly - li) < 4)
                                                                {
                                                                    int distance = lival - lyval;

                                                                    double dtotaln = (double)totaln;
                                                                    double minx = Math.Pow((1 / Math.Log(0.08 * dtotaln + 3)), 1.75);
                                                                    if (((double)distance / dtotaln) > minx)
                                                                    { }
                                                                    else
                                                                        lolos = false;
                                                                    break;
                                                                }
                                                                else
                                                                {
                                                                    lolos = false;
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (lykeylen == 1)
                                                        {

                                                        }
                                                        else
                                                        {
                                                            if (lykeylen < likeylen)
                                                            {
                                                                if (likey.IndexOf(lykey) > -1)
                                                                {
                                                                    break;
                                                                }
                                                            }
                                                            else
                                                                break;
                                                        }
                                                    }
                                                }

                                                if (lolos)
                                                {
                                                    cname = likey;
                                                    break;
                                                }
                                            }
                                        }

                                        if (cname != null)
                                        {
                                            cname = cname.Trim(new char[] { ' ', '\"', '\'', '&', '(', ')' });
                                            cname = cname.Replace("PT.", "PT");
                                            cname = cname.Replace(" PT", "");
                                            cname = cname.Replace("PT ", "");
                                        }

                                        if (cname != null && cname != custname)
                                            Event("Customer name updated: " + cname + " (" + cid + ")");

                                            //if (cust == "4700030")
                                            //{
                                            //    SFEvent("-->" + totaln + " -->" + cname);
                                            //    int id = 0;
                                            //    foreach (KeyValuePair<string, int> pair in lexicalList)
                                            //    {
                                            //        if (id < 10)
                                            //            SFEvent(pair.Key + "=" + pair.Value + "" + ((cname == pair.Key) ? "<-" : ""));
                                            //        else
                                            //            break;

                                            //        id++;
                                            //    }
                                            //}

                                        if (cname != custname)
                                            j.Execute("update ServiceCustomer set SC_Name = {0} where SC_ID = {1}", cname, custid);

                                    }

                                    #endregion
                                }
                            }
                        }

                        if (type == "P")
                            j.Execute("update PEInterface set PI_SE = {1}, PI_SE_Check = 1 where PI_ID = {0}", eid, servid);
                        else if (type == "M")
                            j.Execute("update MECircuit set MC_SE = {1}, MC_SE_Check = 1 where MC_ID = {0}", eid, servid);
                        else if (type == "I")
                            j.Execute("update MEInterface set MI_SE = {1}, MI_SE_Check = 1 where MI_ID = {0}", eid, servid);
                    }
                }
                else
                {
                    if (!stop)
                    {
                        idle = true;
                        Thread.Sleep(60000);
                        idle = false;
                    }
                }
            }

            Event("Stopped");
        }
        
        #endregion
    }

    internal class ServiceDescriptions
    {
        #region Constants

        private static string[] monthsEnglish = new string[] { "JANUARY", "FEBRUARY", "MARCH", "APRIL", "MAY", "JUNE", "JULY", "AUGUST", "SEPTEMBER", "OCTOBER", "NOVEMBER", "DECEMBER" };
        private static string[] monthsBahasa = new string[] { "JANUARI", "FEBRUARI", "MARET", "APRIL", "MEI", "JUNI", "JULI", "AGUSTUS", "SEPTEMBER", "OKTOBER", "NOVEMBER", "DESEMBER" };

        #endregion

        #region Properties

        private string sid;

        public string SID
        {
            get { return sid; }
            private set { sid = value; }
        }

        private string custid;

        public string CustID
        {
            get { return custid; }
            set { custid = value; }
        }

        private string serviceType;

        public string ServiceType
        {
            get { return serviceType; }
            private set { serviceType = value; }
        }

        private string serviceSubType;

        public string ServiceSubType
        {
            get { return serviceSubType; }
            set { serviceSubType = value; }
        }

        private string cleanDescription;

        public string CleanDescription
        {
            get { return cleanDescription; }
            private set { cleanDescription = value; }
        }

        private string rawDescription;

        public string RawDescription
        {
            get { return rawDescription; }
            private set { rawDescription = value; }
        }

        #endregion

        #region Methods

        public static ServiceDescriptions Parse(string desc)
        {
            ServiceDescriptions de = new ServiceDescriptions();
            de.RawDescription = desc;


            string[] s = desc.Split(new char[] { '_', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            string d = string.Join(" ", s).ToUpper();

            int rmv = -1;
            int rle = -1;

            //                         12345678901234567890
            if ((rmv = d.IndexOf("MM IPVPN INSTAN ")) > -1) { de.ServiceType = "VPNINSTAN"; rle = 15; }
            else if ((rmv = d.IndexOf("MM VPNIP INSTAN ")) > -1) { de.ServiceType = "VPNINSTAN"; rle = 15; }
            else if ((rmv = d.IndexOf("VPNIP INSTANT ")) > -1) { de.ServiceType = "VPNINSTAN"; rle = 13; }
            else if ((rmv = d.IndexOf("IPVPN INSTANT ")) > -1) { de.ServiceType = "VPNINSTAN"; rle = 13; }
            else if ((rmv = d.IndexOf("VPNIP INSTAN ")) > -1) { de.ServiceType = "VPNINSTAN"; rle = 12; }
            else if ((rmv = d.IndexOf("IPVPN INSTAN ")) > -1) { de.ServiceType = "VPNINSTAN"; rle = 12; }
            else if ((rmv = d.IndexOf("VPNIP VPN IP ")) > -1) { de.ServiceType = "VPNIP"; rle = 12; }
            else if ((rmv = d.IndexOf("VPNIP VPNIP ")) > -1) { de.ServiceType = "VPNIP"; rle = 11; }
            else if ((rmv = d.IndexOf("VPN INSTAN ")) > -1) { de.ServiceType = "VPNINSTAN"; rle = 10; }
            else if ((rmv = d.IndexOf("MM IPVPN ")) > -1) { de.ServiceType = "VPNIP"; rle = 8; }
            else if ((rmv = d.IndexOf("MM VPNIP ")) > -1) { de.ServiceType = "VPNIP"; rle = 8; }
            else if ((rmv = d.IndexOf("MM VPNIP ")) > -1) { de.ServiceType = "VPNIP"; rle = 8; }
            else if ((rmv = d.IndexOf("MM VPNIP ")) > -1) { de.ServiceType = "VPNIP"; rle = 8; }
            else if ((rmv = d.IndexOf("VPN IP ")) > -1) { de.ServiceType = "VPNIP"; rle = 6; }
            else if ((rmv = d.IndexOf("IP VPN ")) > -1) { de.ServiceType = "VPNIP"; rle = 6; }
            else if ((rmv = d.IndexOf("VPNIP ")) > -1) { de.ServiceType = "VPNIP"; rle = 5; }
            else if ((rmv = d.IndexOf("IPVPN ")) > -1) { de.ServiceType = "VPNIP"; rle = 5; }
            //                         12345678901234567890
            else if ((rmv = d.IndexOf("MM ASTINET BEDA BW ")) > -1) { de.ServiceType = "ASTINETBB"; rle = 18; }
            else if ((rmv = d.IndexOf("MM ASTINET BEDABW ")) > -1) { de.ServiceType = "ASTINETBB"; rle = 17; }
            else if ((rmv = d.IndexOf("ASTINET BEDA BW ")) > -1) { de.ServiceType = "ASTINETBB"; rle = 15; }
            else if ((rmv = d.IndexOf("ASTINET BEDABW ")) > -1) { de.ServiceType = "ASTINETBB"; rle = 14; }
            else if ((rmv = d.IndexOf("MM ASTINET BB ")) > -1) { de.ServiceType = "ASTINETBB"; rle = 13; }
            else if ((rmv = d.IndexOf("ASTINET BB ")) > -1) { de.ServiceType = "ASTINETBB"; rle = 10; }
            else if ((rmv = d.IndexOf("ASTINETBB ")) > -1) { de.ServiceType = "ASTINETBB"; rle = 9; }
            else if ((rmv = d.IndexOf("AST BEDA BW ")) > -1) { de.ServiceType = "ASTINETBB"; rle = 11; }
            else if ((rmv = d.IndexOf("AST BEDABW ")) > -1) { de.ServiceType = "ASTINETBB"; rle = 10; }
            else if ((rmv = d.IndexOf("AST BB ")) > -1) { de.ServiceType = "ASTINETBB"; rle = 7; }
            //                         12345678901234567890
            else if ((rmv = d.IndexOf("MM ASTINET ")) > -1) { de.ServiceType = "ASTINET"; rle = 10; }
            else if ((rmv = d.IndexOf("ASTINET ")) > -1) { de.ServiceType = "ASTINET"; rle = 7; }
            //                         12345678901234567890
            else rmv = -1;

            if (rmv > -1) d = d.Remove(rmv, rle);
            rmv = -1;
            rle = -1;

            if (de.ServiceType == null || de.ServiceType == "VPNIP")
            {
                //                         12345678901234567890
                if ((rmv = d.IndexOf("VPNIP TRANS ACCESS ")) > -1) { de.ServiceType = "VPNIP"; rle = 18; de.ServiceSubType = "TRANS"; }
                else if ((rmv = d.IndexOf("TRANSACTIONAL ACCESS ")) > -1) { de.ServiceType = "VPNIP"; rle = 20; de.ServiceSubType = "TRANS"; }
                else if ((rmv = d.IndexOf("MM TRANS ACCESS ")) > -1) { de.ServiceType = "VPNIP"; rle = 15; de.ServiceSubType = "TRANS"; }
                else if ((rmv = d.IndexOf("TRANSS ACCESS ")) > -1) { de.ServiceType = "VPNIP"; rle = 13; de.ServiceSubType = "TRANS"; }
                else if ((rmv = d.IndexOf("TRANS ACCESS ")) > -1) { de.ServiceType = "VPNIP"; rle = 12; de.ServiceSubType = "TRANS"; }
                else if ((rmv = d.IndexOf("TRANSACCESS ")) > -1) { de.ServiceType = "VPNIP"; rle = 11; de.ServiceSubType = "TRANS"; }
                else if ((rmv = d.IndexOf("TRANS ACCES ")) > -1) { de.ServiceType = "VPNIP"; rle = 11; de.ServiceSubType = "TRANS"; }
                else if ((rmv = d.IndexOf("TRANSACC ")) > -1) { de.ServiceType = "VPNIP"; rle = 8; de.ServiceSubType = "TRANS"; }
                else if ((rmv = d.IndexOf("TRAN ACC ")) > -1) { de.ServiceType = "VPNIP"; rle = 8; de.ServiceSubType = "TRANS"; }
                else if ((rmv = d.IndexOf("TRANS AC ")) > -1) { de.ServiceType = "VPNIP"; rle = 8; de.ServiceSubType = "TRANS"; }
                else if ((rmv = d.IndexOf("TRANSAC ")) > -1) { de.ServiceType = "VPNIP"; rle = 7; de.ServiceSubType = "TRANS"; }
                else if ((rmv = d.IndexOf("TRANSC ")) > -1) { de.ServiceType = "VPNIP"; rle = 6; de.ServiceSubType = "TRANS"; }
                else if ((rmv = d.IndexOf("TRANSS ")) > -1) { de.ServiceType = "VPNIP"; rle = 6; de.ServiceSubType = "TRANS"; }
                else if ((rmv = d.IndexOf("TRANS ")) > -1 && de.ServiceType == null) { de.ServiceType = "VPNIP"; rle = 5; de.ServiceSubType = "TRANS"; }
                else rmv = -1;

                if (rmv > -1) d = d.Remove(rmv, rle);
                rmv = -1;
                rle = -1;
            }

            rmv = -1;
            rle = -1;

            d = d.Trim();

            //                         12345678901234567890
            if ((rmv = d.IndexOf("(EX SID FEAS")) > -1) { rle = 12; }
            else if ((rmv = d.IndexOf("[EX SID FEAS")) > -1) { rle = 12; }
            else if ((rmv = d.IndexOf("EX SID FEAS")) > -1) { rle = 11; }
            else if ((rmv = d.IndexOf("EX SID FEAS")) > -1) { rle = 11; }
            else if ((rmv = d.IndexOf("SID (FEAS)")) > -1) { rle = 10; }
            else if ((rmv = d.IndexOf("SID [FEAS]")) > -1) { rle = 10; }
            else if ((rmv = d.IndexOf("SID <FEAS>")) > -1) { rle = 10; }
            else if ((rmv = d.IndexOf("(SID FEAS")) > -1) { rle = 9; }
            else if ((rmv = d.IndexOf("[SID FEAS")) > -1) { rle = 9; }
            else if ((rmv = d.IndexOf("SID FEAS")) > -1) { rle = 8; }
            else if ((rmv = d.IndexOf("(EX FEAS")) > -1) { rle = 8; }
            else if ((rmv = d.IndexOf("[EX FEAS")) > -1) { rle = 8; }
            else if ((rmv = d.IndexOf("(EX SID")) > -1) { rle = 7; }
            else if ((rmv = d.IndexOf("[EX SID")) > -1) { rle = 7; }
            else if ((rmv = d.IndexOf("EX FEAS")) > -1) { rle = 7; }
            else if ((rmv = d.IndexOf("EX-SID")) > -1) { rle = 6; }
            else if ((rmv = d.IndexOf("EX SID")) > -1) { rle = 6; }
            else if ((rmv = d.IndexOf("X-SID")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf("X SID")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf("FEAS ")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf("VLAN ")) > -1) { rle = 4; }

            if (rmv > -1)
            {
                int rmvn = rmv + rle;
                if (rmvn < d.Length)
                {
                    if (d[rmvn] == ' ') rmvn += 1;
                    else if (d[rmvn] == ':' || d[rmvn] == '-' || d[rmvn] == '=')
                    {
                        rmvn += 1;
                        if (rmvn < d.Length && d[rmvn] == ' ') rmvn += 1;
                    }
                }
                if (rmvn < d.Length)
                {
                    int end = d.IndexOfAny(new char[] { ' ', ')', '(', ']', '[', '.', '<', '>' }, rmvn);
                    if (end > -1) d = d.Remove(rmv, end - rmv);
                    else d = d.Remove(rmv);
                }
            }
            rmv = -1;
            rle = -1;

            //                         12345678901234567890
            if ((rmv = d.IndexOf("SID-TENOSS-")) > -1) { rle = 11; }
            else if ((rmv = d.IndexOf("SID-TENOSS:")) > -1) { rle = 11; }
            else if ((rmv = d.IndexOf("SID-TENOSS=")) > -1) { rle = 11; }
            else if ((rmv = d.IndexOf("SID-TENOSS ")) > -1) { rle = 11; }
            else if ((rmv = d.IndexOf("SID TENOSS:")) > -1) { rle = 11; }
            else if ((rmv = d.IndexOf("SID TENOSS=")) > -1) { rle = 11; }
            else if ((rmv = d.IndexOf("SID TENOSS ")) > -1) { rle = 11; }
            else if ((rmv = d.IndexOf("TENOSS-SID-")) > -1) { rle = 11; }
            else if ((rmv = d.IndexOf("TENOSS-SID:")) > -1) { rle = 11; }
            else if ((rmv = d.IndexOf("TENOSS-SID=")) > -1) { rle = 11; }
            else if ((rmv = d.IndexOf("TENOSS-SID ")) > -1) { rle = 11; }
            else if ((rmv = d.IndexOf("TENOSS SID:")) > -1) { rle = 11; }
            else if ((rmv = d.IndexOf("TENOSS SID=")) > -1) { rle = 11; }
            else if ((rmv = d.IndexOf("TENOSS SID ")) > -1) { rle = 11; }
            else if ((rmv = d.IndexOf("SID SID ")) > -1) { rle = 7; }
            else if ((rmv = d.IndexOf("-SOID-")) > -1) { rle = 6; }
            else if ((rmv = d.IndexOf("(SID-")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf("(SID:")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf("(SID=")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf("(SID%")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf("(SID ")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf("<SID-")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf("<SID:")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf("<SID=")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf("<SID%")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf("<SID ")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf("[SID-")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf("[SID:")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf("[SID=")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf("[SID%")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf("[SID ")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf(" SID-")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf(" SID:")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf(" SID=")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf(" SID%")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf(" SID ")) > -1) { rle = 5; }
            else if ((rmv = d.IndexOf(" SIDT")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf(" SID1")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf(" SID2")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf(" SID3")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf(" SID4")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf(" SID5")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf(" SID6")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf(" SID7")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf(" SID8")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf(" SID9")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf("SID-")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf("SID:")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf("SID=")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf("SID%")) > -1) { rle = 4; }
            else if ((rmv = d.IndexOf("SID ")) > -1) { rle = 4; }
            else rmv = -1;

            if (rmv > -1)
            {
                int rmvn = rmv + rle;
                if (rmvn < d.Length)
                {
                    if (d[rmvn] == ' ') rmvn += 1;
                    else if (d[rmvn] == ':' || d[rmvn] == '-' || d[rmvn] == '=' || d[rmvn] == '(' || d[rmvn] == '[')
                    {
                        rmvn += 1;
                        if (rmvn < d.Length && d[rmvn] == ' ') rmvn += 1;
                    }
                }
                if (rmvn < d.Length)
                {
                    int end = -1;
                    int nextend = rmvn;

                    while (true)
                    {

                        end = d.IndexOfAny(new char[] { ' ', ')', '(', ']', '[', '.', '<', '>' }, nextend);
                        if (end > -1 && end < d.Length && d[end] == ' ' && end - rmvn <= 8) nextend = end + 1;
                        else break;
                    }

                    if (end > -1)
                    {
                        de.SID = d.Substring(rmvn, end - rmv - rle).Trim();
                        d = d.Remove(rmv, end - rmv);
                    }
                    else
                    {
                        string imx = d.Substring(rmvn).Trim();
                        imx = imx.Replace(' ', '_');

                        if (imx.Length > 13)
                        {
                            StringBuilder nimx = new StringBuilder();
                            nimx.Append(imx.Substring(0, 13));
                            for (int imxi = 13; imxi < imx.Length; imxi++)
                            {
                                if (char.IsDigit(imx[imxi])) nimx.Append(imx[imxi]);
                                else break;
                            }

                            imx = nimx.ToString();
                        }

                        de.SID = imx;
                        d = d.Remove(rmv);
                    }
                }

                if (de.SID != null)
                {
                    int weirdc = de.SID.IndexOfAny(new char[] { ' ' });

                    if (weirdc > -1) de.SID = null;

                }
            }

            if (de.SID == null)
            {
                string[] ss = d.Split(new char[] { ' ', ':', '=' });

                List<string> sidc = new List<string>();
                foreach (string si in ss)
                {
                    int dig = 0;

                    string fsi = si.Trim(new char[] { '-', ')', '(', '[', ']', '>', '<' });


                    // count digit in si
                    foreach (char ci in fsi)
                        if (char.IsDigit(ci))
                            dig++;

                    double oc = (double)dig / (double)fsi.Length;

                    if (oc > 0.3 && fsi.Length > 8 &&
                        !fsi.StartsWith("FAA-") &&
                        !fsi.StartsWith("FAI-") &&
                        !fsi.StartsWith("FAD-") &&
                        !fsi.StartsWith("GI") &&
                        !fsi.StartsWith("TE") &&
                        !fsi.StartsWith("FA") &&
                        fsi.IndexOf("GBPS") == -1 &&
                        fsi.IndexOf("KBPS") == -1 &&
                        fsi.IndexOf("MBPS") == -1 &&
                        fsi.IndexOf("BPS") == -1 &&
                        fsi.IndexOfAny(new char[] { '/', '.', ';', '\'', '\"', '>', '<', '/' }) == -1)
                    {
                        int imin = fsi.LastIndexOf('-');

                        if (imin > -1)
                        {
                            string lastport = fsi.Substring(imin + 1);

                            if (lastport.Length < 5) fsi = null;
                            else
                            {
                                bool adadigit = false;
                                foreach (char lastportc in lastport)
                                {
                                    if (char.IsDigit(lastportc))
                                    {
                                        adadigit = true;
                                        break;
                                    }
                                }

                                if (adadigit == false)
                                    fsi = null;
                            }
                        }

                        if (fsi != null)
                        {
                            if (fsi.Length >= 6 && fsi.Length <= 16)
                            {
                                bool isDate = true;

                                string[] fsip = fsi.Split(new char[] { '-' });
                                if (fsip.Length == 3)
                                {
                                    string first = fsip[0];
                                    if (char.IsDigit(first[0]))
                                    {
                                        if (first.Length == 1 || first.Length == 2 && char.IsDigit(first[1])) { }
                                        else isDate = false;
                                    }
                                    if (isDate && !char.IsDigit(first[0]))
                                    {
                                        if (first.Length >= 3 && (
                                            List.StartsWith(monthsEnglish, first) > -1 ||
                                            List.StartsWith(monthsBahasa, first) > -1
                                            ))
                                        { }
                                        else isDate = false;
                                    }
                                    string second = fsip[1];
                                    if (isDate && char.IsDigit(second[0]))
                                    {
                                        if (second.Length == 1 || second.Length == 2 && char.IsDigit(second[1])) { }
                                        else isDate = false;
                                    }
                                    if (isDate && !char.IsDigit(second[0]))
                                    {
                                        if (second.Length >= 3 && (
                                            List.StartsWith(monthsEnglish, second) > -1 ||
                                            List.StartsWith(monthsBahasa, second) > -1
                                            ))
                                        { }
                                        else isDate = false;
                                    }
                                    string third = fsip[2];
                                    if (isDate && char.IsDigit(second[0]))
                                    {
                                        if (third.Length == 2 && char.IsDigit(third[1])) { }
                                        else if (third.Length == 4 && char.IsDigit(third[1]) && char.IsDigit(third[2]) && char.IsDigit(third[3])) { }
                                        else isDate = false;
                                    }
                                    else isDate = false;
                                }
                                else if (fsip.Length == 1)
                                {
                                    // 04APR2014
                                    // APR42014
                                    // 4APR2014
                                    // 04042014

                                    if (char.IsDigit(fsi[0]))
                                    {

                                    }
                                    else
                                    {
                                        int tlen = 1;
                                        for (int fi = 1; fi < fsi.Length; fi++)
                                        {
                                            if (char.IsDigit(fsi[fi])) break;
                                            else tlen++;
                                        }

                                        string t = fsi.Substring(0, tlen);

                                        if (List.StartsWith(monthsEnglish, t) > -1 ||
                                            List.StartsWith(monthsBahasa, t) > -1)
                                        { }
                                        else isDate = false;

                                        if (isDate && fsi.Length > tlen)
                                        {
                                            int remaining = fsi.Length - tlen;
                                            if (remaining >= 3 && remaining <= 6)
                                            {
                                                for (int ooi = 0; ooi < remaining; ooi++)
                                                {
                                                    char cc = fsi[tlen + ooi];
                                                    if (!char.IsDigit(cc))
                                                    {
                                                        isDate = false;
                                                        break;
                                                    }
                                                }
                                            }
                                            else isDate = false;
                                        }
                                    }
                                }
                                else isDate = false;

                                if (isDate) fsi = null;
                            }
                        }

                        if (fsi != null)
                            sidc.Add(fsi);
                    }
                }

                if (sidc.Count > 0)
                {
                    sidc.Sort((a, b) => b.Length.CompareTo(a.Length));

                    de.SID = sidc[0];
                    d = d.Remove(d.IndexOf(de.SID), de.SID.Length);
                }
            }

            if (de.SID != null)
            {
                if (de.SID.Length <= 8)
                    de.SID = null;
                else
                {
                    string fixsid = de.SID.Trim(new char[] { '-', ')', '(', '[', ']', '>', '<' });
                    fixsid = fixsid.Replace("--", "-");

                    string[] sids = fixsid.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (sids.Length > 0)
                        fixsid = sids[0];

                    de.SID = fixsid;

                    if (StringHelper.Count(de.SID, '-') > 3)
                    {
                        de.SID = null;
                    }

                    if (de.SID != null)
                    {
                        int lmin = de.SID.LastIndexOf('-');
                        if (lmin > -1)
                            de.CustID = de.SID.Substring(0, lmin);

                        if (de.CustID == null && lmin == -1)
                        {
                            if (de.SID.Length == 12 && de.SID[0] == '4')
                            {
                                bool alldigit = true;
                                foreach (char c in de.SID) { if (!char.IsDigit(c)) { alldigit = false; break; } }
                                if (alldigit)
                                {
                                    de.CustID = de.SID.Substring(0, 7);
                                }
                            }
                            if (de.SID.Length == 17 && (de.SID[0] == '4' || de.SID[0] == '3'))
                            {
                                bool alldigit = true;
                                foreach (char c in de.SID) { if (!char.IsDigit(c)) { alldigit = false; break; } }
                                if (alldigit)
                                {
                                    de.CustID = de.SID.Substring(0, 7);
                                }
                            }
                        }
                    }
                }
            }

            d = d.Trim();

            // if double, singlekan
            if (d.Length >= 2 && d.Length % 2 == 0)
            {
                int halflen = d.Length / 2;
                string leftside = d.Substring(0, halflen);
                string rightside = d.Substring(halflen, halflen);

                if (leftside == rightside)
                    d = leftside;
            }

            d = d.Replace("()", "");
            d = d.Replace("[]", "");
            d = string.Join(" ", d.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries));

            de.CleanDescription = d;

            return de;
        }

        #endregion
    }
}
