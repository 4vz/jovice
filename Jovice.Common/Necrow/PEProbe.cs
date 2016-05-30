using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Aphysoft.Common;
using Aphysoft.Share;

namespace Jovice
{
    #region To Database

    class PERouteNameToDatabase : ToDatabase
    {        
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string rd;

        public string RD
        {
            get { return rd; }
            set { rd = value; }
        }

        private bool updateRd = false;

        public bool UpdateRD
        {
            get { return updateRd; }
            set { updateRd = value; }
        }

        private string rdIpv6;

        public string RDIPv6
        {
            get { return rdIpv6; }
            set { rdIpv6 = value; }
        }

        private bool updateRdIpv6 = false;

        public bool UpdateRDIPv6
        {
            get { return updateRdIpv6; }
            set { updateRdIpv6 = value; }
        }

        private string[] routeTargets;

        public string[] RouteTargets
        {
            get { return routeTargets; }
            set { routeTargets = value; }
        }

        private bool updateRouteTargets = false;

        public bool UpdateRouteTargets
        {
            get { return updateRouteTargets; }
            set { updateRouteTargets = value; }
        }        

        private string routeID;

        public string RouteID
        {
            get { return routeID; }
            set { routeID = value; }
        }        
    }

    class PEQOSToDatabase : ToDatabase
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string package;

        public string Package
        {
            get { return package; }
            set { package = value; }
        }

        private int bandwidth;

        public int Bandwidth
        {
            get { return bandwidth; }
            set { bandwidth = value; }
        }
    }

    class PEInterfaceToDatabase : InterfaceToDatabase
    {
        private string routeID;

        public string RouteID
        {
            get { return routeID; }
            set { routeID = value; }
        }

        private bool updateRouteID = false;

        public bool UpdateRouteID
        {
            get { return updateRouteID; }
            set { updateRouteID = value; }
        }

        private string inputQOSID;

        public string InputQOSID
        {
            get { return inputQOSID; }
            set { inputQOSID = value; }
        }

        private bool updateInputQOSID = false;

        public bool UpdateInputQOSID
        {
            get { return updateInputQOSID; }
            set { updateInputQOSID = value; }
        }

        private string outputQOSID;

        public string OutputQOSID
        {
            get { return outputQOSID; }
            set { outputQOSID = value; }
        }

        private bool updateOutputQOSID = false;

        public bool UpdateOutputQOSID
        {
            get { return updateOutputQOSID; }
            set { updateOutputQOSID = value; }
        }

        private List<string> ip = null;

        public List<string> IP
        {
            get { return ip; }
            set { ip = value; }
        }

        private List<string> deleteIP = null;

        public List<string> DeleteIP
        {
            get { return deleteIP; }
            set { deleteIP = value; }
        }

        private List<string> deleteIPID = null;

        public List<string> DeleteIPID
        {
            get { return deleteIPID; }
            set { deleteIPID = value; }
        }

    }

    #endregion

    internal sealed partial class Probe
    {
        #region Methods

        private void PEProcess()
        {
            string[] lines = null;
            Batch batch = Batch();
            Result result;

            #region VRF
            
            Dictionary<string, PERouteNameToDatabase> routelive = new Dictionary<string, PERouteNameToDatabase>();
            Dictionary<string, Row> routenamedb = QueryDictionary("select * from PERouteName where PN_NO = {0}", "PN_Name", nodeID);
            Dictionary<string, List<string>> routetargetlocaldb = new Dictionary<string, List<string>>();

            result = Query("select * from PERouteName, PERouteTarget where PN_PR = PT_PR and PN_NO = {0}", nodeID);
            foreach (Row row in result)
            {
                string name = row["PN_Name"].ToString();
                List<string> routeTargets;
                if (!routetargetlocaldb.ContainsKey(name))
                {
                    routeTargets = new List<string>();
                    routetargetlocaldb.Add(name, routeTargets);
                }
                else routeTargets = routetargetlocaldb[name];
                string ipv6 = row["PT_IPv6"].ToBool(false) == false ? "0" : "1";
                string type = row["PT_Type"].ToBool() == false ? "0" : "1";
                string routeTarget = row["PT_Community"].ToString();
                routeTargets.Add(ipv6 + type + routeTarget);
            }

            Dictionary<string, string[]> routetargetinsert = new Dictionary<string, string[]>();
            List<PERouteNameToDatabase> routenameinsert = new List<PERouteNameToDatabase>();
            List<PERouteNameToDatabase> routenameupdate = new List<PERouteNameToDatabase>();

            string[] hweDisplayIPVPNInstanceVerboseLines = null;
                   
            Event("Checking VRF");

            #region Live

            if (nodeManufacture == cso)
            {
                #region cso

                string name = null;
                string RD = null;
                List<string> routeTargets = new List<string>();

                if (nodeVersion == xr)
                {
                    #region xr
                    if (Request("show vrf all", out lines)) return;

                    int linen = 0;
                    foreach (string line in lines)
                    {
                        linen++;
                        if (linen <= 3) continue;

                        string lineTrim = line.Trim();
                        if (lineTrim.StartsWith("VRF") || lineTrim.StartsWith("*")) continue;
                        else if (lineTrim.EndsWith("#")) break;
                        else if (name != null && lineTrim.StartsWith("import"))
                        {
                            string[] s = lineTrim.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            string rt = s[1].Trim();
                            routeTargets.Add("01" + rt);
                        }
                        else if (name != null && lineTrim.StartsWith("export"))
                        {
                            string[] s = lineTrim.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            string rt = s[1].Trim();
                            routeTargets.Add("00" + rt);
                        }
                        else
                        {
                            if (name != null)
                            {
                                PERouteNameToDatabase i = new PERouteNameToDatabase();
                                i.Name = name;
                                i.RD = RD;
                                i.RouteTargets = routeTargets.ToArray();
                                routelive.Add(name, i);
                                name = null;
                                routeTargets.Clear();
                            }

                            string[] vrfline = lineTrim.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                            if (vrfline.Length >= 2)
                            {
                                name = vrfline[0].Trim();
                                if (vrfline[1] == "not") RD = null;
                                else RD = vrfline[1].Trim();
                            }
                        }
                    }
                    if (name != null)
                    {
                        PERouteNameToDatabase i = new PERouteNameToDatabase();
                        i.Name = name;
                        i.RD = RD;
                        i.RouteTargets = routeTargets.ToArray();
                        routelive.Add(name, i);
                    }

                    #endregion
                }
                else
                {
                    #region !xr

                    if (Request("show ip vrf detail | in RD|Export VPN|Import VPN|RT", out lines)) return;

                    int stage = 0;

                    foreach (string line in lines)
                    {
                        string lineTrim = line.Trim();
                        if (lineTrim.StartsWith("VRF "))
                        {
                            if (stage >= 1)
                            {
                                PERouteNameToDatabase i = new PERouteNameToDatabase();
                                i.Name = name;
                                i.RD = RD;
                                i.RouteTargets = routeTargets.ToArray();
                                routelive.Add(name, i);
                            }

                            routeTargets.Clear();

                            string[] linevrf = lineTrim.Split(';');
                            string vrfcc = linevrf[0].Substring(4);
                            string[] vrfccs = vrfcc.Split(' ');
                            name = vrfccs[0];
                            RD = linevrf[1].Substring(12);
                            stage = 1;
                        }
                        else if (lineTrim.StartsWith("Export VPN")) stage = 2;
                        else if (lineTrim.StartsWith("Import VPN")) stage = 3;
                        else if (lineTrim.StartsWith("RT:") && stage >= 2)
                        {
                            string[] rts = lineTrim.Split(new string[] { "RT:" }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string rt in rts)
                            {
                                if (stage == 2) routeTargets.Add("00" + rt.Trim());
                                else if (stage == 3) routeTargets.Add("01" + rt.Trim());
                            }
                        }
                    }
                    if (stage >= 1)
                    {
                        if (name != "__Platform_iVRF:_ID00_" &&
                            name != "__Platform_iVRF:_ID0" &&
                            name != "Default"
                            )
                        {
                            PERouteNameToDatabase i = new PERouteNameToDatabase();
                            i.Name = name;
                            i.RD = RD;
                            i.RouteTargets = routeTargets.ToArray();
                            routelive.Add(name, i);
                        }
                    }
                    #endregion
                }
                #endregion
            }
            else if (nodeManufacture == jun)
            {
                #region jun




                #endregion
            }
            else if (nodeManufacture == hwe)
            {
                #region hwe

                string name = null;
                string ipv = null;
                string RD = null;
                string RDIPv6 = null;
                List<string> routeTargets = new List<string>();

                // | include VPN-Instance Name and ID|Address family|Export VPN Targets|Import VPN Targets|Route Distinguisher
                if (Request("display ip vpn-instance verbose", out hweDisplayIPVPNInstanceVerboseLines)) return;

                foreach (string line in hweDisplayIPVPNInstanceVerboseLines)
                {
                    string linetrim = line.Trim();

                    if (linetrim.StartsWith("VPN-Instance Name and ID"))
                    {
                        if (name != null)
                        {
                            PERouteNameToDatabase i = new PERouteNameToDatabase();
                            i.Name = name;
                            i.RD = RD;
                            i.RDIPv6 = RDIPv6;
                            i.RouteTargets = routeTargets.ToArray();

                            routelive.Add(name, i);

                            name = null;
                            ipv = null;
                            RD = null;
                            RDIPv6 = null;
                            routeTargets.Clear();
                        }

                        string[] linenameid = linetrim.Substring(27).Split(StringSplitTypes.Comma, StringSplitOptions.RemoveEmptyEntries);
                        name = linenameid[0].Trim();
                    }
                    else if (linetrim == "Address family ipv4") ipv = "IPv4";
                    else if (linetrim == "Address family ipv6") ipv = "IPv6";
                    else if (linetrim.StartsWith("Route Distinguisher") && linetrim.Length > 21)
                    {
                        if (ipv == "IPv4")
                            RD = linetrim.Substring(22);
                        else
                            RDIPv6 = linetrim.Substring(22);
                    }
                    else if (linetrim.StartsWith("Export VPN Targets") && linetrim.Length > 20)
                    {
                        string comt = linetrim.Substring(21).Trim();
                        string[] comts = comt.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string comtx in comts)
                        {
                            routeTargets.Add((ipv == "IPv4" ? "0" : "1") + "0" + comtx.Trim());
                        }
                    }
                    else if (linetrim.StartsWith("Import VPN Targets") && linetrim.Length > 20)
                    {
                        string comt = linetrim.Substring(21).Trim();
                        string[] comts = comt.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string comtx in comts)
                        {
                            routeTargets.Add((ipv == "IPv4" ? "0" : "1") + "1" + comtx.Trim());
                        }
                    }
                }

                if (name != null)
                {
                    PERouteNameToDatabase i = new PERouteNameToDatabase();
                    i.Name = name;
                    i.RD = RD;
                    i.RDIPv6 = RDIPv6;
                    i.RouteTargets = routeTargets.ToArray();

                    routelive.Add(name, i);
                }

                #endregion
            }

            #endregion
                       
            #region Check

            foreach (KeyValuePair<string, PERouteNameToDatabase> pair in routelive)
            {
                PERouteNameToDatabase li = pair.Value;
                if (!routenamedb.ContainsKey(pair.Key))
                {
                    Event("VRF Name ADD: " + pair.Key);
                    li.ID = Database.ID();
                    routenameinsert.Add(li);
                }
                else
                {
                    Row db = routenamedb[pair.Key];
                    List<string> routeTargets = null;
                    if (routetargetlocaldb.ContainsKey(pair.Key)) routeTargets = routetargetlocaldb[pair.Key];

                    PERouteNameToDatabase u = new PERouteNameToDatabase();

                    bool update = false;
                    StringBuilder updateinfo = new StringBuilder();

                    if (db["PN_RD"].ToString() != li.RD)
                    {
                        update = true;
                        u.UpdateRD = true;
                        u.RD = li.RD;
                        updateinfo.Append("RD ");
                    }
                    if (db["PN_RDv6"].ToString() != li.RDIPv6)
                    {
                        update = true;
                        u.UpdateRDIPv6 = true;
                        u.RDIPv6 = li.RDIPv6;
                        updateinfo.Append("RDv6 ");
                    }
                    if (routeTargets != null)
                    {
                        List<string> temp = new List<string>(routeTargets);
                        foreach (string routeTarget in li.RouteTargets) { if (temp.Contains(routeTarget)) temp.Remove(routeTarget); }
                        List<string> temp2 = new List<string>(li.RouteTargets);
                        foreach (string routeTarget in routeTargets) { if (temp2.Contains(routeTarget)) temp2.Remove(routeTarget); }

                        if (temp.Count != 0 || temp2.Count != 0)
                        {
                            update = true;
                            u.UpdateRouteTargets = true;
                            u.RouteTargets = li.RouteTargets;
                            updateinfo.Append("RT ");
                        }
                    }

                    if (update)
                    {
                        u.ID = db["PN_ID"].ToString();
                        u.Name = pair.Key;
                        routenameupdate.Add(u);
                        Event("VRF Name UPDATE: " + pair.Key + " " + updateinfo.ToString());
                    }
                }
            }

            Summary("VRF_COUNT", routelive.Count);

            // Search/set route targets
            List<PERouteNameToDatabase> routetargetsearch = new List<PERouteNameToDatabase>(routenameinsert);
            foreach (PERouteNameToDatabase u in routenameupdate) { if (u.UpdateRouteTargets) routetargetsearch.Add(u); }            
            Dictionary<int, Result> routesearch = new Dictionary<int, Result>();

            foreach (PERouteNameToDatabase s in routetargetsearch)
            {
                string[] routeTargets = s.RouteTargets;
                int length = routeTargets.Length;
                string routeID = null;

                Result r;

                if (!routesearch.ContainsKey(length))
                {
                    // search route target for 
                    r = Query("select a.PT_PR, a.PT_TC from " +
                        "(select PT_PR, STR(CASE WHEN (PT_IPv6 IS NULL) THEN '0' ELSE '1' END) + LTRIM(STR(PT_Type)) + PT_Community as 'PT_TC' from PERouteTarget) a, " +
                        "(select PT_PR, COUNT(PT_PR) as 'COUNT' from PERouteTarget group by PT_PR) b " +
                        "where a.PT_PR = b.PT_PR and COUNT = {0} order by a.PT_PR ", length);
                    routesearch.Add(length, r);
                }
                else r = routesearch[length];

                int i = 0;
                while (i < r.Count)
                {
                    int matched = 0;
                    for (int ii = 0; ii < length; ii++)
                    {
                        foreach (string srt in routeTargets) { if (r[i + ii]["PT_TC"].ToString() == srt) matched++; }
                    }

                    if (matched == length)
                    {
                        routeID = r[i]["PT_PR"].ToString();
                        break;
                    }

                    i += length;
                }

                if (routeID == null)
                {
                    Event("VRF Route Targets ADD: " + s.Name);
                    routeID = Database.ID();
                    routetargetinsert.Add(routeID, routeTargets);
                }
                s.RouteID = routeID;
            }

            #endregion

            #region Execute

            // ADD
            
            // Route
            batch.Begin();
            foreach (KeyValuePair<string, string[]> pair in routetargetinsert)
            {
                batch.Execute("insert into PERoute(PR_ID) values({0})", pair.Key);
            }
            result = batch.Commit();
            Event(result, EventActions.Add, EventElements.VRF, false);
            
            // Route Target
            batch.Begin();
            foreach (KeyValuePair<string, string[]> pair in routetargetinsert)
            {
                foreach (string routeTarget in pair.Value)
                {
                    Insert insert = Insert("PERouteTarget");
                    insert.Value("PT_ID", Database.ID());
                    insert.Value("PT_PR", pair.Key);
                    insert.Value("PT_Type", routeTarget[1] == '1');
                    insert.Value("PT_Community", routeTarget.Substring(2));
                    insert.Value("PT_IPv6", routeTarget[0] == '1');
                    batch.Execute(insert);
                }
            }
            result = batch.Commit();
            Event(result, EventActions.Add, EventElements.VRFRouteTarget, false);

            // Route Name
            batch.Begin();
            foreach (PERouteNameToDatabase s in routenameinsert)
            {
                Insert insert = Insert("PERouteName");
                insert.Value("PN_ID", s.ID);
                insert.Value("PN_PR", s.RouteID);
                insert.Value("PN_NO", nodeID);
                insert.Value("PN_Name", s.Name);
                insert.Value("PN_RD", s.RD);
                insert.Value("PN_RDv6", s.RDIPv6);
                batch.Execute(insert);
            }
            result = batch.Commit();
            Event(result, EventActions.Add, EventElements.VRFReference, false);

            // UPDATE
            batch.Begin();
            foreach (PERouteNameToDatabase s in routenameupdate)
            {
                Update update = Update("PERouteName");
                update.Set("PN_RD", s.RD, s.UpdateRD);
                update.Set("PN_RDv6", s.RDIPv6, s.UpdateRDIPv6);
                update.Set("PN_PR", s.RouteID, s.UpdateRouteTargets);
                update.Where("PN_ID", s.ID);
                batch.Execute(update);
            }
            result = batch.Commit();
            Event(result, EventActions.Update, EventElements.VRFReference, false);

            #endregion

            routenamedb = QueryDictionary("select * from PERouteName where PN_NO = {0}", "PN_Name", nodeID);
            
            #endregion

            #region QOS
            
            Dictionary<string, PEQOSToDatabase> qoslive = new Dictionary<string, PEQOSToDatabase>();
            Dictionary<string, Row> qosdb = QueryDictionary("select * from PEQOS where PQ_NO = {0}", "PQ_Name", nodeID);
            List<PEQOSToDatabase> qosinsert = new List<PEQOSToDatabase>();
                      
            Event("Checking QOS");

            #region Live

            if (nodeManufacture == cso)
            {
                #region cso

                if (nodeVersion == xr)
                {
                    #region xr

                    if (Request("show policy-map list | in PolicyMap", out lines)) return;

                    foreach (string line in lines)
                    {
                        int imf = line.IndexOf("PolicyMap: ");
                        if (imf > -1)
                        {
                            int sta = imf + 11;
                            int spaceAfterName = line.IndexOf(' ', sta);
                            string name = line.Substring(sta, spaceAfterName - sta);

                            PEQOSToDatabase li = new PEQOSToDatabase();
                            li.Name = name;
                            NodeQOSCSO qos = NodeQOSCSO.Parse(li.Name);
                            li.Package = qos.Package;
                            li.Bandwidth = qos.Bandwidth;
                            qoslive.Add(name, li);
                        }
                    }

                    #endregion
                }
                else
                {
                    #region !xr
                    if (Request("show policy-map | in Policy Map", out lines)) return;

                    foreach (string line in lines)
                    {
                        string lineTrim = line.Trim();

                        if (lineTrim.StartsWith("Policy Map "))
                        {
                            string name = lineTrim.Substring(11);

                            PEQOSToDatabase li = new PEQOSToDatabase();
                            li.Name = name;
                            NodeQOSCSO qos = NodeQOSCSO.Parse(li.Name);
                            li.Package = qos.Package;
                            li.Bandwidth = qos.Bandwidth;
                            qoslive.Add(name, li);
                        }
                    }
                    #endregion
                }

                #endregion
            }
            else if (nodeManufacture == hwe)
            {
                #region hwe

                // not tested, pe hwe still dont use this
                if (Request("display qos-profile configuration", out lines)) return;

                bool qosCollect = false;
                foreach (string line in lines)
                {
                    string lineTrim = line.Trim();

                    if (qosCollect == false)
                    {
                        if (lineTrim.StartsWith("qos-profile-name")) qosCollect = true;
                    }
                    else
                    {
                        string[] linex = lineTrim.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);

                        if (linex.Length == 2)
                        {
                            string name = linex[0];
                            string used = linex[1];

                            PEQOSToDatabase li = new PEQOSToDatabase();
                            li.Name = name;
                            NodeQOSHWE qos = NodeQOSHWE.Parse(li.Name);
                            li.Bandwidth = qos.Bandwidth;
                            qoslive.Add(name, li);
                        }
                    }
                }
                #endregion
            }

            #endregion
                     
            #region Check

            foreach (KeyValuePair<string, PEQOSToDatabase> pair in qoslive)
            {
                PEQOSToDatabase li = pair.Value;

                if (!qosdb.ContainsKey(pair.Key))
                {
                    Event("QOS ADD: " + li.Name + " (" + (li.Package == null ? "-" : li.Package) + ", " +
                        (li.Bandwidth == -1 ? "-" : (li.Bandwidth + "K")) + ")");

                    li.ID = Database.ID();
                    qosinsert.Add(li);
                }
            }

            Summary("QOS_COUNT", qoslive.Count);

            #endregion

            #region Execute

            // ADD
            batch.Begin();
            foreach (PEQOSToDatabase s in qosinsert)
            {
                Insert insert = Insert("PEQOS");
                insert.Value("PQ_ID", s.ID);
                insert.Value("PQ_NO", nodeID);
                insert.Value("PQ_Name", s.Name);
                insert.Value("PQ_Bandwidth", s.Bandwidth.Nullable(-1));
                insert.Value("PQ_Package", s.Package);
                batch.Execute(insert);
            }
            result = batch.Commit();
            Event(result, EventActions.Add, EventElements.QOS, false);

            #endregion

            qosdb = QueryDictionary("select * from PEQOS where PQ_NO = {0}", "PQ_Name", nodeID);
            
            #endregion

            #region INTERFACE
            
            SortedDictionary<string, PEInterfaceToDatabase> interfacelive = new SortedDictionary<string, PEInterfaceToDatabase>();
            Dictionary<string, Row> interfacedb = QueryDictionary("select * from PEInterface where PI_NO = {0}", "PI_Name", nodeID);
            Dictionary<string, List<string[]>> ipdb = new Dictionary<string, List<string[]>>();

            result = Query("select PI_Name, PP_ID, PP_Type + ':' + PP_IP as IP from PEInterface, PEInterfaceIP where PP_PI = PI_ID and PI_NO = {0} order by PI_Name asc", nodeID);
            foreach (Row row in result)
            {
                string name = row["PI_Name"].ToString();
                List<string[]> ip = null;
                if (ipdb.ContainsKey(name)) ip = ipdb[name];
                else
                {
                    ip = new List<string[]>();
                    ipdb.Add(name, ip);
                }
                ip.Add(new string[] { row["IP"].ToString(), row["PP_ID"].ToString() });
            }

            SortedDictionary<string, PEInterfaceToDatabase> interfaceinsert = new SortedDictionary<string, PEInterfaceToDatabase>();
            List<PEInterfaceToDatabase> interfaceupdate = new List<PEInterfaceToDatabase>();
            Dictionary<string, List<string>> ipinsert = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> ipdelete = new Dictionary<string, List<string>>();

            ServiceReference interfaceservicereference = new ServiceReference();
            
            Event("Checking Interface");

            #region Live

            if (nodeManufacture == cso)
            {
                #region cso    

                if (nodeVersion == xr)
                {
                    #region xr

                    // interface
                    if (Request("show int desc", out lines)) return;

                    foreach (string line in lines)
                    {
                        //Gi0/0/0/0          up          up          TRUNK_PE-D2-JT2-VPN_Gi0/0/0/0_TO_ME4-D2-JT_4/1/1_1G_MAIN_VPN_GB
                        //012345678901234567890123456789012345678901234567890123456789
                        //          1         2         3         4         5    5
                        //123456789012345678901234567890123456789012345678901234567890
                        //         1         2         3         4         5         6
                        string lineTrim = line.TrimStart();
                        int length = lineTrim.Length;

                        if (!lineTrim.StartsWith("Interface") && !lineTrim.StartsWith("show") && !lineTrim.StartsWith("-"))
                        {
                            string ifname = length >= 43 ? lineTrim.Substring(0, 19).Trim() : null;
                            string status = length >= 43 ? lineTrim.Substring(19, 12).Trim() : null;
                            string protocol = length >= 43 ? lineTrim.Substring(31, 12).Trim() : null;
                            string description = length >= 44 ? lineTrim.Substring(43).Trim() : null;

                            NodeInterface nodeinterface = NodeInterface.Parse(ifname);
                            if (nodeinterface != null)
                            {
                                PEInterfaceToDatabase i = new PEInterfaceToDatabase();
                                i.Name = nodeinterface.GetShort();
                                i.Status = status == "up";
                                i.Protocol = protocol == "up";
                                i.Enable = !status.StartsWith("admin");
                                i.Description = description == String.Empty ? null : description;
                                interfacelive.Add(i.Name, i);
                            }
                        }
                    }

                    // dot1q
                    if (Request("show run int | in \"interface|encapsulation\"", out lines)) return;

                    //interface GigabitEthernet0/0/0/0.852
                    // encapsulation dot1q 852
                    // 012345678901234567890123456789
                    //interface GigabitEthernet0/0/0/0.855
                    //interface GigabitEthernet0/0/0/0.877
                    // encapsulation dot1q 877

                    string currentPIName = null;
                    foreach (string line in lines)
                    {
                        if (line.StartsWith("interface"))
                        {
                            currentPIName = null;
                            string[] linex = line.Split(StringSplitTypes.Space);
                            if (linex.Length == 2)
                            {
                                NodeInterface nodeinterface = NodeInterface.Parse(linex[1]);
                                if (nodeinterface != null) currentPIName = nodeinterface.GetShort();
                            }
                        }
                        else if (currentPIName != null)
                        {
                            string linetrim = line.Trim();
                            if (linetrim.StartsWith("encapsulation dot1q"))
                            {
                                if (interfacelive.ContainsKey(currentPIName))
                                {
                                    int dot1q;
                                    if (int.TryParse(linetrim.Substring(20), out dot1q))
                                    {
                                        interfacelive[currentPIName].Dot1Q = dot1q;
                                        currentPIName = null; // null after consumed by encap dot1q
                                    }
                                }
                            }
                        }
                    }

                    // vrf to interface
                    if (Request("show vrf all detail", out lines)) return;

                    string currentVrf = null;
                    string currentVrfName = null;
                    bool collectingInterface = false;

                    foreach (string line in lines)
                    {
                        string lineTrim = line.Trim();

                        if (currentVrf == null)
                        {
                            if (lineTrim.StartsWith("VRF ") && !lineTrim.StartsWith("VRF mode: "))
                            {
                                string name = lineTrim.Substring(4).Trim().Split(new char[] { ';' })[0].Trim();

                                if (name.StartsWith("*")) name = null;
                                else if (routenamedb.ContainsKey(name))
                                {
                                    currentVrf = routenamedb[name]["PN_ID"].ToString();
                                    currentVrfName = name;
                                }
                            }
                        }
                        else
                        {
                            if (collectingInterface)
                            {
                                if (lineTrim.StartsWith("Address family IPV4 Unicast"))
                                {
                                    currentVrf = null;
                                    currentVrfName = null;
                                    collectingInterface = false;
                                }
                                else
                                {
                                    NodeInterface nodeInterface = NodeInterface.Parse(lineTrim);
                                    if (nodeInterface != null)
                                    {
                                        string name = nodeInterface.GetShort();

                                        if (interfacelive.ContainsKey(name))
                                        {
                                            interfacelive[name].RouteID = currentVrf;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (lineTrim.StartsWith("Interfaces:")) collectingInterface = true;
                                else if (lineTrim.StartsWith("Address family IPV4 Unicast"))
                                {
                                    currentVrf = null;
                                    collectingInterface = false;
                                }
                            }
                        }
                    }

                    // policy map
                    if (Request("show policy-map targets", out lines)) return;

                    string currentpolicy = null;
                    int currentconfigrate = -1;
                    collectingInterface = false;

                    foreach (string line in lines)
                    {
                        string lineTrim = line.Trim();
                        int nameindex = lineTrim.IndexOf("Policymap: ");
                        if (nameindex > -1)
                        {
                            string name = lineTrim.Substring(nameindex + 10, lineTrim.IndexOf(' ', nameindex + 11) - (nameindex + 10)).Trim();
                            if (qosdb.ContainsKey(name))
                            {
                                currentpolicy = qosdb[name]["PQ_ID"].ToString();
                                if (!qosdb[name]["PQ_Bandwidth"].IsNull)
                                {
                                    currentconfigrate = qosdb[name]["PQ_Bandwidth"].ToInt();
                                }

                            }
                        }
                        else if (lineTrim.IndexOf("main policy") > -1)
                            collectingInterface = true;
                        else if (lineTrim.IndexOf("Total targets") > -1)
                        {
                            collectingInterface = false;
                            currentpolicy = null;
                            currentconfigrate = -1;
                        }
                        else if (collectingInterface && currentpolicy != null)
                        {
                            string[] linex = lineTrim.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);
                            if (linex.Length == 2)
                            {
                                NodeInterface nodeInterface = NodeInterface.Parse(linex[0]);
                                if (nodeInterface != null)
                                {
                                    string name = nodeInterface.GetShort();
                                    string type = nodeInterface.ShortType;
                                    int typerate = -1;
                                    if (type == "Te") typerate = 10485760;
                                    else if (type == "Ge") typerate = 1048576;
                                    else if (type == "Fa") typerate = 102400;
                                    else if (type == "Et") typerate = 10240;

                                    if (interfacelive.ContainsKey(name))
                                    {
                                        string parentPort = null;
                                        if (nodeInterface.IsSubInterface)
                                        {
                                            string bport = nodeInterface.GetBase();
                                            if (interfacelive.ContainsKey(bport))
                                                parentPort = bport;
                                        }

                                        if (linex[1] == "input")
                                        {
                                            interfacelive[name].InputQOSID = currentpolicy;

                                            if (parentPort != null)
                                            {
                                                if (currentconfigrate > -1)
                                                {
                                                    int cur = interfacelive[parentPort].CirConfigTotalInput;
                                                    if (cur == -1) cur = 0;
                                                    interfacelive[parentPort].CirConfigTotalInput = cur + currentconfigrate;

                                                    long curR = interfacelive[parentPort].CirTotalInput;
                                                    if (curR == -1) curR = 0;
                                                    interfacelive[parentPort].CirTotalInput = curR + currentconfigrate;
                                                }
                                                else if (typerate > -1)
                                                {
                                                    long curR = interfacelive[parentPort].CirTotalInput;
                                                    if (curR == -1) curR = 0;
                                                    interfacelive[parentPort].CirTotalInput = curR + typerate;
                                                }
                                            }
                                        }
                                        else if (linex[1] == "output")
                                        {
                                            interfacelive[name].OutputQOSID = currentpolicy;

                                            if (parentPort != null)
                                            {
                                                if (currentconfigrate > -1)
                                                {
                                                    int cur = interfacelive[parentPort].CirConfigTotalOutput;
                                                    if (cur == -1) cur = 0;
                                                    interfacelive[parentPort].CirConfigTotalOutput = cur + currentconfigrate;

                                                    long curR = interfacelive[parentPort].CirTotalOutput;
                                                    if (curR == -1) curR = 0;
                                                    interfacelive[parentPort].CirTotalOutput = curR + currentconfigrate;
                                                }
                                                else if (typerate > -1)
                                                {
                                                    long curR = interfacelive[parentPort].CirTotalOutput;
                                                    if (curR == -1) curR = 0;
                                                    interfacelive[parentPort].CirTotalOutput = curR + typerate;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // ip
                    if (Request("show ipv4 vrf all interface | in \"Internet address|Secondary address|ipv4\"", out lines)) return;

                    #endregion
                }
                else
                {
                    #region !xr

                    // interface
                    if (Request("show interface | in line protocol|Description|802.1Q", out lines)) return;

                    /*
GigabitEthernet0/1.3546 is administratively down, line protocol is down
  Description: ASTINET PEMDA TK I PAPUA SID 4703328-23028 MOVE TO PE-D7-JAP-INET
  Encapsulation 802.1Q Virtual LAN, Vlan ID  3546.
                    */

                    PEInterfaceToDatabase current = null;
                    StringBuilder descriptionBuffer = null;

                    foreach (string line in lines)
                    {
                        if (line.Length > 0)
                        {
                            string[] firstLineTokens = line.Split(new string[] { " is " }, StringSplitOptions.None);
                            if (firstLineTokens.Length == 3)
                            {
                                string name = firstLineTokens[0].Trim();
                                NodeInterface inf = NodeInterface.Parse(name);
                                if (inf != null)
                                {
                                    string mid = firstLineTokens[1].Trim();
                                    string last = firstLineTokens[2].Trim();

                                    if (current != null && descriptionBuffer != null) current.Description = descriptionBuffer.ToString().Trim();
                                    descriptionBuffer = null;

                                    if (!mid.StartsWith("delete")) // skip deleted interface
                                    {
                                        current = new PEInterfaceToDatabase();
                                        current.Name = inf.GetShort();

                                        if (mid.StartsWith("up")) current.Status = true;
                                        else current.Status = false;
                                        if (mid.StartsWith("admin")) current.Enable = false;
                                        else current.Enable = true;

                                        if (last.StartsWith("up")) current.Protocol = true;
                                        else current.Protocol = false;

                                        interfacelive.Add(current.Name, current);
                                    }
                                    else current = null;
                                }
                            }
                            else if (current != null)
                            {
                                string linets = line.TrimStart();
                                if (linets.StartsWith("Description: "))
                                    descriptionBuffer = new StringBuilder(line.Substring(line.IndexOf("Description: ") + 13).TrimStart());
                                else if (linets.StartsWith("Encapsulation "))
                                {
                                    int dot1q;
                                    //  Encapsulation 802.1Q Virtual LAN, Vlan ID  3546.
                                    string vlanid = line.Substring(line.IndexOf("Vlan ID") + 7).Trim('.', ' ');
                                    if (int.TryParse(vlanid, out dot1q)) current.Dot1Q = dot1q;
                                }
                                else if (descriptionBuffer != null) descriptionBuffer.Append(line);
                            }
                        }
                    }
                    if (current != null && descriptionBuffer != null) current.Description = descriptionBuffer.ToString().Trim();

                    // vrf to interface
                    if (Request("show ip vrf interfaces", out lines)) return;

                    foreach (string line in lines)
                    {
                        string lineTrim = line.Trim();
                        if (!lineTrim.StartsWith("Interface") && !lineTrim.StartsWith("show"))
                        {
                            string[] parts = lineTrim.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (parts.Length == 4)
                            {
                                if (routenamedb.ContainsKey(parts[2]) && interfacelive.ContainsKey(parts[0]))
                                    interfacelive[parts[0]].RouteID = routenamedb[parts[2]]["PN_ID"].ToString();
                            }
                        }
                    }

                    // policy map
                    if (Request("show policy-map interface input brief", out lines)) return;

                    bool policyMapUsingBrief = true;
                    foreach (string line in lines)
                    {
                        if (line != null)
                        {
                            if (line.ToUpper().Contains("INVALID INPUT"))
                            {
                                policyMapUsingBrief = false;
                                break;
                            }
                        }
                    }

                    if (!policyMapUsingBrief)
                    {
                        #region !Brief
                        if (Request("show policy-map interface | in Service-policy_input|Service-policy_output|Ethernet|Serial", out lines)) return;

                        string currentif = null;
                        string parentPort = null;
                        int typerate = -1;

                        foreach (string line in lines)
                        {
                            string lineTrim = line.Trim();
                            if (!lineTrim.StartsWith("show"))
                            {
                                if (!lineTrim.StartsWith("Service-policy"))
                                {
                                    currentif = null;
                                    parentPort = null;
                                    typerate = -1;

                                    string ifcandidate = lineTrim;

                                    NodeInterface nodeinterface = NodeInterface.Parse(ifcandidate);
                                    if (nodeinterface != null)
                                    {
                                        string name = nodeinterface.GetShort();
                                        if (interfacelive.ContainsKey(name))
                                        {
                                            currentif = name;

                                            if (nodeinterface.IsSubInterface)
                                            {
                                                string bport = nodeinterface.GetBase();
                                                if (interfacelive.ContainsKey(bport))
                                                {
                                                    parentPort = bport;
                                                    typerate = nodeinterface.GetTypeRate();
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if (lineTrim.StartsWith("Service-policy input:"))
                                    {
                                        int titikdua = lineTrim.IndexOf(':');
                                        if (titikdua > -1)
                                        {
                                            string pmap = lineTrim.Substring(titikdua + 1).Trim();

                                            string policyid = null;
                                            if (qosdb.ContainsKey(pmap)) policyid = qosdb[pmap]["PQ_ID"].ToString();

                                            if (currentif != null && policyid != null)
                                            {
                                                interfacelive[currentif].InputQOSID = policyid;

                                                if (parentPort != null)
                                                {
                                                    int currentconfigrate = qosdb[pmap]["PQ_Bandwidth"].ToInt(-1);

                                                    if (currentconfigrate > -1)
                                                    {
                                                        int cur = interfacelive[parentPort].CirConfigTotalInput;
                                                        if (cur == -1) cur = 0;
                                                        interfacelive[parentPort].CirConfigTotalInput = cur + currentconfigrate;

                                                        long curR = interfacelive[parentPort].CirTotalInput;
                                                        if (curR == -1) curR = 0;
                                                        interfacelive[parentPort].CirTotalInput = curR + currentconfigrate;
                                                    }
                                                    else if (typerate > -1)
                                                    {
                                                        long curR = interfacelive[parentPort].CirTotalInput;
                                                        if (curR == -1) curR = 0;
                                                        interfacelive[parentPort].CirTotalInput = curR + typerate;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else if (lineTrim.StartsWith("Service-policy output:"))
                                    {
                                        int titikdua = lineTrim.IndexOf(':');
                                        if (titikdua > -1)
                                        {
                                            string pmap = lineTrim.Substring(titikdua + 1).Trim();

                                            string policyid = null;
                                            if (qosdb.ContainsKey(pmap)) policyid = qosdb[pmap]["PQ_ID"].ToString();

                                            if (currentif != null && policyid != null)
                                            {
                                                interfacelive[currentif].OutputQOSID = policyid;

                                                if (parentPort != null)
                                                {
                                                    int currentconfigrate = qosdb[pmap]["PQ_Bandwidth"].ToInt(-1);

                                                    if (currentconfigrate > -1)
                                                    {
                                                        int cur = interfacelive[parentPort].CirConfigTotalOutput;
                                                        if (cur == -1) cur = 0;
                                                        interfacelive[parentPort].CirConfigTotalOutput = cur + currentconfigrate;

                                                        long curR = interfacelive[parentPort].CirTotalOutput;
                                                        if (curR == -1) curR = 0;
                                                        interfacelive[parentPort].CirTotalOutput = curR + currentconfigrate;
                                                    }
                                                    else if (typerate > -1)
                                                    {
                                                        long curR = interfacelive[parentPort].CirTotalOutput;
                                                        if (curR == -1) curR = 0;
                                                        interfacelive[parentPort].CirTotalOutput = curR + typerate;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }


                        #endregion
                    }
                    else
                    {
                        #region Brief
                        string[] tlines;
                        if (Request("show policy-map interface output brief", out tlines)) return;
                        List<string> mlines = new List<string>(lines);
                        mlines.AddRange(tlines);
                        lines = mlines.ToArray();

                        string currentpolicy = null;
                        string type = "input";
                        int currentconfigrate = -1;

                        foreach (string line in lines)
                        {
                            string lineTrim = line.Trim();

                            if (lineTrim.StartsWith("Service-policy "))
                            {
                                string[] linex = line.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                                if (linex.Length == 2)
                                {
                                    if (linex[0].IndexOf("input") > -1) type = "input";
                                    else type = "output";
                                    string pmap = linex[1].Trim();
                                    if (qosdb.ContainsKey(pmap))
                                    {
                                        currentpolicy = qosdb[pmap]["PQ_ID"].ToString();
                                        currentconfigrate = qosdb[pmap]["PQ_Bandwidth"].ToInt(-1);
                                    }
                                }
                            }
                            else if (currentpolicy != null)
                            {
                                NodeInterface nodeinterface = NodeInterface.Parse(lineTrim);
                                if (nodeinterface != null)
                                {
                                    string name = nodeinterface.GetShort();


                                    if (interfacelive.ContainsKey(name))
                                    {
                                        string typeif = nodeinterface.ShortType;
                                        int typerate = nodeinterface.GetTypeRate();

                                        string parentPort = null;

                                        if (nodeinterface.IsSubInterface)
                                        {
                                            string bport = nodeinterface.GetBase();
                                            if (interfacelive.ContainsKey(bport)) parentPort = bport;
                                        }

                                        PEInterfaceToDatabase i = interfacelive[name];
                                        if (type == "input")
                                        {
                                            i.InputQOSID = currentpolicy;

                                            if (parentPort != null)
                                            {
                                                if (currentconfigrate > -1)
                                                {
                                                    int cur = interfacelive[parentPort].CirConfigTotalInput;
                                                    if (cur == -1) cur = 0;
                                                    interfacelive[parentPort].CirConfigTotalInput = cur + currentconfigrate;

                                                    long curR = interfacelive[parentPort].CirTotalInput;
                                                    if (curR == -1) curR = 0;
                                                    interfacelive[parentPort].CirTotalInput = curR + currentconfigrate;
                                                }
                                                else if (typerate > -1)
                                                {
                                                    long curR = interfacelive[parentPort].CirTotalInput;
                                                    if (curR == -1) curR = 0;
                                                    interfacelive[parentPort].CirTotalInput = curR + typerate;
                                                }
                                            }
                                        }
                                        else if (type == "output")
                                        {
                                            i.OutputQOSID = currentpolicy;

                                            if (parentPort != null)
                                            {
                                                if (currentconfigrate > -1)
                                                {
                                                    int cur = interfacelive[parentPort].CirConfigTotalOutput;
                                                    if (cur == -1) cur = 0;
                                                    interfacelive[parentPort].CirConfigTotalOutput = cur + currentconfigrate;

                                                    long curR = interfacelive[parentPort].CirTotalOutput;
                                                    if (curR == -1) curR = 0;
                                                    interfacelive[parentPort].CirTotalOutput = curR + currentconfigrate;
                                                }
                                                else if (typerate > -1)
                                                {
                                                    long curR = interfacelive[parentPort].CirTotalOutput;
                                                    if (curR == -1) curR = 0;
                                                    interfacelive[parentPort].CirTotalOutput = curR + typerate;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                    }

                    // rate-limit
                    if (Request("show interface rate-limit", out lines)) return;

                    string currentinterface = null;
                    string currentmode = null;
                    string parentPort2 = null;
                    int typerate2 = -1;

                    foreach (string line in lines)
                    {
                        if (line.Length > 0)
                        {
                            string lineTrim = line.Trim();

                            if (lineTrim == "Input") currentmode = "input";
                            else if (lineTrim == "Output") currentmode = "output";
                            else if (lineTrim.StartsWith("params:"))
                            {
                                if (currentmode != null && currentinterface != null)
                                {
                                    string[] linex = lineTrim.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);

                                    if (linex.Length > 2)
                                    {
                                        int bps = -1;
                                        if (int.TryParse(linex[1], out bps))
                                        {
                                            int kbps = bps / 1000;

                                            if (interfacelive.ContainsKey(currentinterface))
                                            {
                                                PEInterfaceToDatabase pid = interfacelive[currentinterface];
                                                if (currentmode == "input")
                                                {
                                                    pid.RateInput = kbps;

                                                    if (parentPort2 != null)
                                                    {
                                                        if (kbps > 0)
                                                        {
                                                            int cur = interfacelive[parentPort2].CirConfigTotalInput;
                                                            if (cur == -1) cur = 0;
                                                            interfacelive[parentPort2].CirConfigTotalInput = cur + kbps;

                                                            long curR = interfacelive[parentPort2].CirTotalInput;
                                                            if (curR == -1) curR = 0;
                                                            interfacelive[parentPort2].CirTotalInput = curR + kbps;
                                                        }
                                                        else if (typerate2 > -1)
                                                        {
                                                            long curR = interfacelive[parentPort2].CirTotalInput;
                                                            if (curR == -1) curR = 0;
                                                            interfacelive[parentPort2].CirTotalInput = curR + typerate2;
                                                        }
                                                    }
                                                }
                                                else if (currentmode == "output")
                                                {
                                                    pid.RateOutput = kbps;

                                                    if (parentPort2 != null)
                                                    {
                                                        if (kbps > -1)
                                                        {
                                                            int cur = interfacelive[parentPort2].CirConfigTotalOutput;
                                                            if (cur == -1) cur = 0;
                                                            interfacelive[parentPort2].CirConfigTotalOutput = cur + kbps;

                                                            long curR = interfacelive[parentPort2].CirTotalOutput;
                                                            if (curR == -1) curR = 0;
                                                            interfacelive[parentPort2].CirTotalOutput = curR + kbps;
                                                        }
                                                        else if (typerate2 > -1)
                                                        {
                                                            long curR = interfacelive[parentPort2].CirTotalOutput;
                                                            if (curR == -1) curR = 0;
                                                            interfacelive[parentPort2].CirTotalOutput = curR + kbps;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    currentmode = null;
                                }
                            }
                            else if (!lineTrim.StartsWith("matches:")
                                && !lineTrim.StartsWith("conformed")
                                && !lineTrim.StartsWith("exceeded")
                                && !lineTrim.StartsWith("last")
                                && !lineTrim.StartsWith("Interface"))
                            {
                                int space = lineTrim.IndexOf(' ');

                                if (space > -1)
                                {
                                    NodeInterface nodeinterface = NodeInterface.Parse(lineTrim.Substring(0, space));
                                    if (nodeinterface != null)
                                    {
                                        parentPort2 = null;
                                        currentinterface = nodeinterface.GetShort();

                                        if (nodeinterface.IsSubInterface)
                                        {
                                            string bport = nodeinterface.GetBase();
                                            if (interfacelive.ContainsKey(bport))
                                            {
                                                parentPort2 = bport;
                                                typerate2 = nodeinterface.GetTypeRate();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // ip
                    if (Request("show ip interface | in Internet address|Secondary address|line protocol", out lines)) return;

                    #endregion
                }

                PEInterfaceToDatabase currentInterface = null;
                int linen = 0;
                foreach (string line in lines)
                {
                    linen++;
                    if (linen <= (nodeVersion == xr ? 2 : 1)) continue;

                    string linex = line.Trim();

                    if (currentInterface != null && linex.StartsWith("Internet address"))
                    {
                        string ip = linex.Substring(20);
                        if (currentInterface.IP == null) currentInterface.IP = new List<string>();
                        currentInterface.IP.Add("1:" + ip);
                    }
                    else if (currentInterface != null && linex.StartsWith("Secondary address"))
                    {
                        string ip = linex.Substring(18);
                        if (currentInterface.IP == null) currentInterface.IP = new List<string>();
                        currentInterface.IP.Add("2:" + ip);
                    }
                    else
                    {
                        currentInterface = null;

                        if (linex.IndexOf(' ') > -1)
                        {
                            string name = linex.Substring(0, linex.IndexOf(' '));
                            NodeInterface nodeInterface = NodeInterface.Parse(name);
                            if (nodeInterface != null)
                            {
                                string shortName = nodeInterface.GetShort();
                                if (interfacelive.ContainsKey(shortName))
                                    currentInterface = interfacelive[shortName];
                            }
                        }
                    }
                }

                #endregion
            }
            else if (nodeManufacture == hwe)
            {
                #region hwe
                if (Request("display interface description", out lines)) return;

                bool begin = false;
                string port = null;
                string status = null;
                string protocol = null;
                StringBuilder description = new StringBuilder();

                foreach (string line in lines)
                {
                    if (line.Length > 0)
                    {
                        if (begin)
                        {
                            if (line[0] != ' ' && line.Length >= 30)
                            {
                                if (port != null)
                                {
                                    if (!interfacelive.ContainsKey(port))
                                    {
                                        PEInterfaceToDatabase pid = new PEInterfaceToDatabase();
                                        pid.Name = port;
                                        pid.Description = description.ToString();
                                        pid.Status = (status == "up" || status == "up(s)");
                                        pid.Protocol = (status == "up" || status == "up(s)");
                                        interfacelive.Add(port, pid);
                                    }

                                    description.Clear();
                                    port = null;
                                    status = null;
                                    protocol = null;
                                }

                                // 8.x
                                //Eth-Trunk1                    up      up       AGGR_PE2-D1-PBR-TRANSIT/ETH-TRUNK1_TO_T-D1-PBR/BE5_5x10G
                                //GE1/0/7(10G)                  up      up       TRUNK_PE2-D1-PBR-TRANSIT_GE1/0/7_TO_T-D1-PBR_Te0/3/0/2_No4_10G_Eth-Trunk1
                                //GE1/0/8(10G)                  down    down
                                //012345678901234567890123456789012345678901234567
                                //          1         2         3         4
                                string inf = line.Substring(0, 30).Trim();
                                status = line.Substring(30, 7).Trim();
                                protocol = line.Substring(38, 7).Trim();

                                if (inf.StartsWith("Eth-Trunk")) port = "Ag" + inf.Substring(9);
                                else
                                {
                                    NodeInterface nif = NodeInterface.Parse(inf);
                                    if (nif != null) port = nif.GetShort();
                                }

                                if (port != null)
                                {
                                    string descarea = null;
                                    if (line.Length > 47) descarea = line.Substring(47).TrimStart();

                                    description.Append(descarea);
                                }
                            }
                            else if (port != null) description.Append(line.TrimStart());
                        }
                        else if (line.StartsWith("Interface")) begin = true;
                    }
                }
                if (port != null)
                {
                    if (!interfacelive.ContainsKey(port))
                    {
                        PEInterfaceToDatabase pid = new PEInterfaceToDatabase();
                        pid.Name = port;
                        pid.Description = description.ToString();
                        pid.Status = (status == "up" || status == "up(s)");
                        pid.Protocol = (status == "up" || status == "up(s)");
                        interfacelive.Add(port, pid);
                    }
                }

                if (Request("display interface brief", out lines)) return;

                begin = false;
                int aggr = -1;

                foreach (string line in lines)
                {
                    if (begin)
                    {
                        string poe = line.Trim().Split(StringSplitTypes.Space)[0].Trim();

                        if (aggr > -1 && line.StartsWith(" "))
                        {
                            if (!poe.StartsWith("Eth-Trunk"))
                            {
                                NodeInterface inf = NodeInterface.Parse(poe);
                                if (inf != null) interfacelive[inf.GetShort()].Aggr = aggr;
                            }
                        }
                        else
                        {
                            aggr = -1;
                            if (poe.StartsWith("Eth-Trunk") && poe.IndexOf('.') == -1)
                            {
                                if (!int.TryParse(poe.Substring(9), out aggr)) aggr = -1;
                            }
                        }
                    }
                    else
                    {
                        if (line.StartsWith("Interface")) begin = true;
                    }
                }

                if (Request("disp cur int | in interface|vlan-type\\ dot1q", out lines)) return;

                //interface Eth-Trunk25.3648
                //01234567890123456
                // vlan-type dot1q 3648
                // 01234567890123456789
                //interface Eth-Trunk25.3649
                // vlan-type dot1q 3649

                PEInterfaceToDatabase current = null;

                foreach (string line in lines)
                {
                    if (line.StartsWith("interface "))
                    {
                        current = null;
                        string ifname = line.Substring(10).Trim();
                        // Eth-Trunk1234.5678
                        // 0123456789
                        if (ifname.StartsWith("Eth-Trunk")) ifname = "Ag" + ifname.Substring(9);
                        NodeInterface inf = NodeInterface.Parse(ifname);
                        if (inf != null)
                        {
                            string sn = inf.GetShort();
                            if (interfacelive.ContainsKey(sn)) current = interfacelive[sn];
                        }
                    }
                    else if (current != null)
                    {
                        string linetrim = line.Trim();
                        if (linetrim.StartsWith("vlan-type dot1q"))
                        {
                            int dot1q;
                            if (int.TryParse(linetrim.Substring(16), out dot1q))
                            {
                                current.Dot1Q = dot1q;
                                current = null;
                            }
                        }
                    }
                }
                
                string cvrf = null;
                foreach (string line in hweDisplayIPVPNInstanceVerboseLines)
                {
                    //VPN-Instance Name and ID : asdasds,
                    //0123456789012345678901234567
                    if (line.Trim().StartsWith("VPN-Instance Name and ID"))
                    {
                        string[] linenameid = line.Substring(27).Split(StringSplitTypes.Comma, StringSplitOptions.RemoveEmptyEntries);
                        cvrf = linenameid[0].Trim();
                    }
                    else if (line.Trim().StartsWith("Address family")) cvrf = null;
                    else if (cvrf != null && line.Length > 15)
                    {
                        //  Interfaces : Eth-Trunk25.3648,
                        //0123456789012345678901234567
                        string inf = line.Substring(15).TrimEnd(',', ' ');
                        if (inf.StartsWith("Eth-Trunk")) inf = "Ag" + inf.Substring(9);
                        if (routenamedb.ContainsKey(cvrf) && interfacelive.ContainsKey(inf))
                            interfacelive[inf].RouteID = routenamedb[cvrf]["PN_ID"].ToString();
                    }
                }

                #endregion
            }

            #endregion

            #region Check

            foreach (KeyValuePair<string, PEInterfaceToDatabase> pair in interfacelive)
            {
                PEInterfaceToDatabase li = pair.Value;
                NodeInterface inf = NodeInterface.Parse(li.Name);

                if (inf != null)
                {
                    if (inf.IsSubInterface)
                    {
                        string parent = inf.GetBase();
                        if (interfacelive.ContainsKey(parent))
                        {
                            int count = interfacelive[parent].SubInterfaceCount;
                            if (count == -1) count = 0;
                            interfacelive[parent].SubInterfaceCount = count + 1;
                        }
                    }
                    else
                    {
                        li.SubInterfaceCount = 0;
                    }
                }
            }

            int sinf = 0, sinfup = 0, sinfhu = 0, sinfag = 0, sinfhuup = 0, sinfte = 0, sinfteup = 0, sinfgi = 0, sinfgiup = 0, sinffa = 0, sinffaup = 0, sinfet = 0, sinfetup = 0, sinfse = 0, sinfseup = 0,
                ssubinf = 0, ssubinfup = 0, ssubinfupup = 0, ssubinfag = 0, ssubinfagup = 0, ssubinfagupup = 0, ssubinfhu = 0, ssubinfhuup = 0, ssubinfhuupup = 0, ssubinfte = 0, ssubinfteup = 0, ssubinfteupup = 0, ssubinfgi = 0, ssubinfgiup = 0, ssubinfgiupup = 0, ssubinffa = 0, ssubinffaup = 0, ssubinffaupup = 0, ssubinfet = 0, ssubinfetup = 0, ssubinfetupup = 0;

            foreach (KeyValuePair<string, PEInterfaceToDatabase> pair in interfacelive)
            {
                PEInterfaceToDatabase li = pair.Value;
                NodeInterface inf = NodeInterface.Parse(li.Name);
                string parentPort = null;

                // TOPOLOGY
                if (inf != null)
                {
                    string inftype = inf.ShortType;

                    if (inf.IsSubInterface)
                    {
                        parentPort = inf.GetBase();

                        ssubinf++;
                        if (li.Status)
                        {
                            ssubinfup++;
                            if (li.Protocol) ssubinfupup++;
                        }
                        if (inftype == "Hu") { ssubinfhu++; if (li.Status) { ssubinfhuup++; if (li.Protocol) ssubinfhuupup++; } }
                        if (inftype == "Te") { ssubinfte++; if (li.Status) { ssubinfteup++; if (li.Protocol) ssubinfteupup++; } }
                        if (inftype == "Gi") { ssubinfgi++; if (li.Status) { ssubinfgiup++; if (li.Protocol) ssubinfgiupup++; } }
                        if (inftype == "Fa") { ssubinffa++; if (li.Status) { ssubinffaup++; if (li.Protocol) ssubinffaupup++; } }
                        if (inftype == "Et") { ssubinfet++; if (li.Status) { ssubinfetup++; if (li.Protocol) ssubinfetupup++; } }
                        if (inftype == "Ag") { ssubinfag++; if (li.Status) { ssubinfagup++; if (li.Protocol) ssubinfagupup++; } }
                    }
                    else
                    {
                        sinf++;
                        if (li.Status) sinfup++;
                        if (inftype == "Hu") { sinfhu++; if (li.Status) sinfhuup++; }
                        if (inftype == "Te") { sinfte++; if (li.Status) sinfteup++; }
                        if (inftype == "Gi") { sinfgi++; if (li.Status) sinfgiup++; }
                        if (inftype == "Fa") { sinffa++; if (li.Status) sinffaup++; }
                        if (inftype == "Et") { sinfet++; if (li.Status) sinfetup++; }
                        if (inftype == "Se") { sinfse++; if (li.Status) sinfseup++; }
                        if (inftype == "Ag") { sinfag++; }
                        if (li.Aggr != -1) parentPort = "Ag" + li.Aggr;
                    }

                    if (parentPort != null)
                    {
                        if (interfacedb.ContainsKey(parentPort)) // cek di existing db
                            li.ParentID = interfacedb[parentPort]["PI_ID"].ToString();
                        else if (interfaceinsert.ContainsKey(parentPort)) // cek di interface yg baru
                            li.ParentID = interfaceinsert[parentPort].ID;
                    }


                    if (li.ParentID == null) // aka jika physical interface, dan bukan anak aggregator
                    {
                        if (interfacedb.ContainsKey(pair.Key)) // cek di existing db
                        {
                            string adjID = interfacedb[pair.Key]["PI_TO_MI"].ToString();
                            if (adjID != null)
                            {
                                li.AdjacentIDList = new Dictionary<int, string>();
                                result = Query("select MI_ID, MI_DOT1Q from MEInterface where MI_MI = {0}", adjID);
                                foreach (Row row in result)
                                {
                                    if (!row["MI_DOT1Q"].IsNull)
                                    {
                                        string spiid = row["MI_ID"].ToString();
                                        int dot1q = row["MI_DOT1Q"].ToShort();
                                        if (!li.AdjacentIDList.ContainsKey(dot1q)) li.AdjacentIDList.Add(dot1q, spiid);
                                    }
                                    //string spiname = row["MI_Name"].ToString();
                                    //int dot = spiname.IndexOf('.');
                                    //if (dot > -1 && spiname.Length > (dot + 1))
                                    //{
                                    //    string sifname = spiname.Substring(dot + 1);
                                    //    if (!li.AdjacentSubifID.ContainsKey(sifname)) li.AdjacentSubifID.Add(sifname, spiid);
                                    //}
                                }
                            }
                            else FindNodeCandidate(li.Description);
                        }
                    }
                    else if (inf.IsSubInterface) // subinterface
                    {
                        int dot1q = li.Dot1Q;
                        if (dot1q > -1)
                        {
                            PEInterfaceToDatabase parent = interfacelive[parentPort];
                            if (parent.AdjacentIDList != null)
                            {
                                if (parent.AdjacentIDList.ContainsKey(dot1q))
                                    li.AdjacentID = parent.AdjacentIDList[dot1q];
                            }
                        }

                        //int dot = li.Name.IndexOf('.');
                        //if (dot > -1 && li.Name.Length > (dot + 1))
                        //{
                        //    string sifname = li.Name.Substring(dot + 1);
                        //    PEInterfaceToDatabase parent = interfacelive[parentPort];
                        //    if (parent.AdjacentSubifID != null)
                        //    {
                        //        if (parent.AdjacentSubifID.ContainsKey(sifname))
                        //            li.AdjacentID = parent.AdjacentSubifID[sifname];
                        //    }
                        //}
                    }
                }


                if (!interfacedb.ContainsKey(pair.Key))
                {
                    Event("Interface ADD: " + pair.Key);

                    // IP
                    if (li.IP != null)
                        Event("       IP ADD: " + string.Join(",", li.IP.ToArray()));

                    // Service
                    if (li.Description != null) interfaceservicereference.Add(li, li.Description);
                    
                    li.ID = Database.ID();
                    interfaceinsert.Add(li.Name, li);
                }
                else
                {
                    Row db = interfacedb[pair.Key];

                    PEInterfaceToDatabase u = new PEInterfaceToDatabase();
                    u.ID = db["PI_ID"].ToString();
                    li.ID = u.ID;

                    bool update = false;
                    StringBuilder updateinfo = new StringBuilder();

                    if (db["PI_PI"].ToString() != li.ParentID)
                    {
                        update = true;
                        u.UpdateParentID = true;
                        u.ParentID = li.ParentID;
                        updateinfo.Append(li.ParentID == null ? "parent " : "child ");
                    }
                    if (li.ParentID != null && li.Aggr == -1) // update adjacent ID jika berupa subinterface dan bukan anak aggregator
                    {
                        if (db["PI_TO_MI"].ToString() != li.AdjacentID)
                        {
                            update = true;
                            u.UpdateAdjacentID = true;
                            u.AdjacentID = li.AdjacentID;
                            updateinfo.Append("adj ");
                        }
                    }
                    if (db["PI_Description"].ToString() != li.Description)
                    {
                        update = true;
                        u.UpdateDescription = true;
                        u.Description = li.Description;
                        updateinfo.Append("desc ");

                        u.ServiceID = null;
                        if (u.Description != null) interfaceservicereference.Add(u, u.Description);
                    }
                    if (db["PI_Status"].ToBool() != li.Status)
                    {
                        update = true;
                        u.UpdateStatus = true;
                        u.Status = li.Status;
                        updateinfo.Append("stat ");
                    }
                    if (db["PI_Protocol"].ToBool() != li.Protocol)
                    {
                        update = true;
                        u.UpdateProtocol = true;
                        u.Protocol = li.Protocol;
                        updateinfo.Append("prot ");
                    }
                    if (db["PI_Enable"].ToBool() != li.Enable)
                    {
                        update = true;
                        u.UpdateEnable = true;
                        u.Enable = li.Enable;
                        updateinfo.Append("ena ");
                    }
                    if (db["PI_DOT1Q"].ToShort(-1) != li.Dot1Q)
                    {
                        update = true;
                        u.UpdateDot1Q = true;
                        u.Dot1Q = li.Dot1Q;
                        updateinfo.Append("dot1q ");                        
                    }
                    if (db["PI_Aggregator"].ToShort(-1) != li.Aggr)
                    {
                        update = true;
                        u.UpdateAggr = true;
                        u.Aggr = li.Aggr;
                        updateinfo.Append("aggr ");
                    }
                    if (db["PI_PN"].ToString() != li.RouteID)
                    {
                        update = true;
                        u.UpdateRouteID = true;
                        u.RouteID = li.RouteID;
                        updateinfo.Append("rout ");
                    }
                    if (db["PI_PQ_Input"].ToString() != li.InputQOSID)
                    {
                        update = true;
                        u.UpdateInputQOSID = true;
                        u.InputQOSID = li.InputQOSID;
                        updateinfo.Append("qin ");
                    }
                    if (db["PI_PQ_Output"].ToString() != li.OutputQOSID)
                    {
                        update = true;
                        u.UpdateOutputQOSID = true;
                        u.OutputQOSID = li.OutputQOSID;
                        updateinfo.Append("qout ");
                    }
                    if (db["PI_Rate_Input"].ToInt(-1) != li.RateInput)
                    {
                        update = true;
                        u.UpdateRateInput = true;
                        u.RateInput = li.RateInput;
                        updateinfo.Append("rin ");
                    }
                    if (db["PI_Rate_Output"].ToInt(-1) != li.RateOutput)
                    {
                        update = true;
                        u.UpdateRateOutput = true;
                        u.RateOutput = li.RateOutput;
                        updateinfo.Append("rout ");
                    }
                    if (db["PI_Summary_CIRConfigTotalInput"].ToInt(-1) != li.CirConfigTotalInput)
                    {
                        update = true;
                        u.UpdateCirConfigTotalInput = true;
                        u.CirConfigTotalInput = li.CirConfigTotalInput;
                        updateinfo.Append("circonfin ");
                    }
                    if (db["PI_Summary_CIRConfigTotalOutput"].ToInt(-1) != li.CirConfigTotalOutput)
                    {
                        update = true;
                        u.UpdateCirConfigTotalOutput = true;
                        u.CirConfigTotalOutput = li.CirConfigTotalOutput;
                        updateinfo.Append("circonfout ");
                    }
                    if (db["PI_Summary_CIRTotalInput"].ToLong(-1) != li.CirTotalInput)
                    {
                        update = true;
                        u.UpdateCirTotalInput = true;
                        u.CirTotalInput = li.CirTotalInput;
                        updateinfo.Append("cirin ");
                    }
                    if (db["PI_Summary_CIRTotalOutput"].ToLong(-1) != li.CirTotalOutput)
                    {
                        update = true;
                        u.UpdateCirTotalOutput = true;
                        u.CirTotalOutput = li.CirTotalOutput;
                        updateinfo.Append("cirout ");
                    }
                    if (db["PI_Summary_SubInterfaceCount"].ToShort(-1) != li.SubInterfaceCount)
                    {
                        update = true;
                        u.UpdateSubInterfaceCount = true;
                        u.SubInterfaceCount = li.SubInterfaceCount;
                        updateinfo.Append("subifc ");
                    }

                    // check ip
                    List<string[]> ipd = ipdb.ContainsKey(pair.Key) ? ipdb[pair.Key] : null;
                    List<string> ipl = li.IP;

                    if (ipl != null && ipd != null)
                    {
                        foreach (string ip in ipl)
                        {
                            bool contain = false;
                            foreach (string[] ipdp in ipd)
                            {
                                if (ipdp[0] == ip)
                                {
                                    contain = true;
                                    break;
                                }
                            }
                            if (!contain)
                            {
                                if (u.IP == null) u.IP = new List<string>();
                                u.IP.Add(ip);
                            }
                        }
                        foreach (string[] ip in ipd)
                        {
                            if (!ipl.Contains(ip[0]))
                            {
                                if (u.DeleteIP == null)
                                {
                                    u.DeleteIP = new List<string>();
                                    u.DeleteIPID = new List<string>();
                                }
                                u.DeleteIP.Add(ip[0]);
                                u.DeleteIPID.Add(ip[1]);
                            }
                        }
                    }
                    else
                    {
                        if (ipl == null && ipd != null)
                        {
                            // delete semua IP dari live
                            u.DeleteIP = new List<string>();
                            u.DeleteIPID = new List<string>();
                            foreach (string[] ip in ipd)
                            {
                                u.DeleteIP.Add(ip[0]);
                                u.DeleteIPID.Add(ip[1]);
                            }
                        }
                        else if (ipl != null && ipd == null) u.IP = new List<string>(ipl); // add semua IP
                    }

                    if (u.IP != null || u.DeleteIP != null)
                    {
                        updateinfo.Append("ip ");
                        update = true;
                    }

                    if (update)
                    {
                        interfaceupdate.Add(u);
                        Event("Interface UPDATE: " + pair.Key + " " + updateinfo.ToString());
                        if (u.IP != null) Event("          IP ADD: " + string.Join(",", u.IP.ToArray()));
                        if (u.DeleteIP != null) Event("       IP DELETE: " + string.Join(",", u.DeleteIP.ToArray()));
                    }
                }
            }

            Summary("INTERFACE_COUNT", sinf);
            Summary("INTERFACE_COUNT_UP", sinfup);
            Summary("INTERFACE_COUNT_HU", sinfhu);
            Summary("INTERFACE_COUNT_HU_UP", sinfhuup);
            Summary("INTERFACE_COUNT_TE", sinfte);
            Summary("INTERFACE_COUNT_TE_UP", sinfteup);
            Summary("INTERFACE_COUNT_GI", sinfgi);
            Summary("INTERFACE_COUNT_GI_UP", sinfgiup);
            Summary("INTERFACE_COUNT_FA", sinffa);
            Summary("INTERFACE_COUNT_FA_UP", sinffaup);
            Summary("INTERFACE_COUNT_SE", sinfse);
            Summary("INTERFACE_COUNT_SE_UP", sinfseup);
            Summary("INTERFACE_COUNT_AG", sinfag);
            Summary("SUBINTERFACE_COUNT", ssubinf);
            Summary("SUBINTERFACE_COUNT_UP", ssubinfup);
            Summary("SUBINTERFACE_COUNT_UP_UP", ssubinfupup);
            Summary("SUBINTERFACE_COUNT_AG", ssubinfag);
            Summary("SUBINTERFACE_COUNT_AG_UP", ssubinfagup);
            Summary("SUBINTERFACE_COUNT_AG_UP_UP", ssubinfagupup);
            Summary("SUBINTERFACE_COUNT_HU", ssubinfhu);
            Summary("SUBINTERFACE_COUNT_HU_UP", ssubinfhuup);
            Summary("SUBINTERFACE_COUNT_HU_UP_UP", ssubinfhuupup);
            Summary("SUBINTERFACE_COUNT_TE", ssubinfte);
            Summary("SUBINTERFACE_COUNT_TE_UP", ssubinfteup);
            Summary("SUBINTERFACE_COUNT_TE_UP_UP", ssubinfteupup);
            Summary("SUBINTERFACE_COUNT_GI", ssubinfgi);
            Summary("SUBINTERFACE_COUNT_GI_UP", ssubinfgiup);
            Summary("SUBINTERFACE_COUNT_GI_UP_UP", ssubinfgiupup);
            Summary("SUBINTERFACE_COUNT_FA", ssubinffa);
            Summary("SUBINTERFACE_COUNT_FA_UP", ssubinffaup);
            Summary("SUBINTERFACE_COUNT_FA_UP_UP", ssubinffaupup);

            #endregion

            #region Execute

            // SERVICE REFERENCE
            ServiceExecute(interfaceservicereference);

            // ADD
            batch.Begin();
            List<Tuple<string, string>> interfacereferenceupdate = new List<Tuple<string, string>>();
            foreach (KeyValuePair<string, PEInterfaceToDatabase> pair in interfaceinsert)
            {
                PEInterfaceToDatabase s = pair.Value;
                Insert insert = Insert("PEInterface");
                insert.Value("PI_ID", s.ID);
                insert.Value("PI_NO", nodeID);
                insert.Value("PI_Name", s.Name);
                insert.Value("PI_Status", s.Status);
                insert.Value("PI_Protocol", s.Protocol);
                insert.Value("PI_Enable", s.Enable);
                insert.Value("PI_DOT1Q", s.Dot1Q.Nullable(-1));
                insert.Value("PI_Aggregator", s.Aggr.Nullable(-1));
                insert.Value("PI_Description", s.Description);
                insert.Value("PI_PN", s.RouteID);
                insert.Value("PI_PQ_Input", s.InputQOSID);
                insert.Value("PI_PQ_Output", s.OutputQOSID);
                insert.Value("PI_SE", s.ServiceID);
                insert.Value("PI_PI", s.ParentID);
                insert.Value("PI_TO_MI", s.AdjacentID);
                insert.Value("PI_Rate_Input", s.RateInput.Nullable(-1));
                insert.Value("PI_Rate_Output", s.RateOutput.Nullable(-1));
                insert.Value("PI_Summary_CIRConfigTotalInput", s.CirConfigTotalInput.Nullable(-1));
                insert.Value("PI_Summary_CIRConfigTotalOutput", s.CirConfigTotalOutput.Nullable(-1));
                insert.Value("PI_Summary_CIRTotalInput", s.CirTotalInput.Nullable(-1));
                insert.Value("PI_Summary_CIRTotalOutput", s.CirTotalOutput.Nullable(-1));
                insert.Value("PI_Summary_SubInterfaceCount", s.SubInterfaceCount.Nullable(-1));
                batch.Execute(insert);

                interfacereferenceupdate.Add(new Tuple<string, string>(s.AdjacentID, s.ID));
                if (s.IP != null) ipinsert.Add(s.ID, s.IP);
            }
            result = batch.Commit();
            Event(result, EventActions.Add, EventElements.Interface, false);

            // UPDATE
            batch.Begin();
            foreach (PEInterfaceToDatabase s in interfaceupdate)
            {
                Update update = Update("PEInterface");
                update.Set("PI_PI", s.ParentID, s.UpdateParentID);
                if (s.UpdateAdjacentID)
                {
                    update.Set("PI_TO_MI", s.AdjacentID);
                    interfacereferenceupdate.Add(new Tuple<string, string>(s.AdjacentID, s.ID));
                }
                if (s.UpdateDescription)
                {
                    update.Set("PI_Description", s.Description);
                    update.Set("PI_SE", s.ServiceID);
                }
                update.Set("PI_Status", s.Status, s.UpdateStatus);
                update.Set("PI_Protocol", s.Protocol, s.UpdateProtocol);
                update.Set("PI_Enable", s.Enable, s.UpdateEnable);
                update.Set("PI_DOT1Q", s.Dot1Q.Nullable(-1), s.UpdateDot1Q);
                update.Set("PI_Aggregator", s.Aggr.Nullable(-1), s.UpdateAggr);
                update.Set("PI_PN", s.RouteID, s.UpdateRouteID);
                update.Set("PI_PQ_Input", s.InputQOSID, s.UpdateInputQOSID);
                update.Set("PI_PQ_Output", s.OutputQOSID, s.UpdateOutputQOSID);
                update.Set("PI_Rate_Input", s.RateInput.Nullable(-1), s.UpdateRateInput);
                update.Set("PI_Rate_Output", s.RateOutput.Nullable(-1), s.UpdateRateOutput);
                update.Set("PI_Summary_CIRConfigTotalInput", s.CirConfigTotalInput.Nullable(-1), s.UpdateCirConfigTotalInput);
                update.Set("PI_Summary_CIRConfigTotalOutput", s.CirConfigTotalOutput.Nullable(-1), s.UpdateCirConfigTotalOutput);
                update.Set("PI_Summary_CIRTotalInput", s.CirTotalInput.Nullable(-1), s.UpdateCirTotalInput);
                update.Set("PI_Summary_CIRTotalOutput", s.CirTotalOutput.Nullable(-1), s.UpdateCirTotalOutput);
                update.Set("PI_Summary_SubInterfaceCount", s.SubInterfaceCount.Nullable(-1), s.UpdateSubInterfaceCount);
                update.Where("PI_ID", s.ID);
                batch.Execute(update);

                if (s.IP != null)
                    ipinsert.Add(s.ID, s.IP);
                if (s.DeleteIP != null)
                    ipdelete.Add(s.ID, s.DeleteIPID);
            }
            result = batch.Commit();
            Event(result, EventActions.Update, EventElements.Interface, false);

            batch.Begin();
            foreach (Tuple<string, string> tuple in interfacereferenceupdate)
            {
                if (tuple.Item1 != null)
                    batch.Execute("update MEInterface set MI_TO_PI = {0} where MI_ID = {1}", tuple.Item2, tuple.Item1);
                else
                    batch.Execute("update MEInterface set MI_TO_PI = NULL where MI_TO_PI = {0}", tuple.Item2);
            }
            result = batch.Commit();

            // IP ADD
            batch.Begin();
            foreach (KeyValuePair<string, List<string>> pair in ipinsert)
            {
                List<string> ips = pair.Value;
                foreach (string ip in ips)
                {
                    Insert insert = Insert("PEInterfaceIP");
                    insert.Value("PP_ID", Database.ID());
                    insert.Value("PP_PI", pair.Key);
                    insert.Value("PP_Type", ip.Substring(0, 1));
                    insert.Value("PP_IP", ip.Substring(2));
                    batch.Execute(insert);
                }               
            }
            result = batch.Commit();
            Event(result, EventActions.Add, EventElements.InterfaceIP, false);

            // IP DELETE
            batch.Begin();
            foreach (KeyValuePair<string, List<string>> pair in ipdelete)
            {
                foreach (string id in pair.Value)
                {
                    batch.Execute("delete from PEInterfaceIP where PP_ID = {0}", id);
                }
            }
            result = batch.Commit();
            Event(result, EventActions.Delete, EventElements.InterfaceIP, false);

            // DELETE
            batch.Begin();
            List<string> interfacedelete = new List<string>();
            foreach (KeyValuePair<string, Row> pair in interfacedb)
            {
                if (!interfacelive.ContainsKey(pair.Key))
                {
                    Event("Interface DELETE: " + pair.Key);
                    string id = pair.Value["PI_ID"].ToString();
                    batch.Execute("update MEInterface set MI_TO_PI = NULL where MI_TO_PI = {0}", id);
                    batch.Execute("update PERoute set PR_PI = NULL where PR_PI = {0}", id);
                    batch.Execute("update PEInterface set PI_PI = NULL where PI_PI = {0}", id);
                    interfacedelete.Add(id);
                }
            }
            batch.Commit();
            batch.Begin();
            foreach (string id in interfacedelete)
            {
                batch.Execute("delete from PEInterfaceIP where PP_PI = {0}", id);
            }
            result = batch.Commit();
            Event(result, EventActions.Delete, EventElements.InterfaceIP, false);

            batch.Begin();
            foreach (string id in interfacedelete)
            {
                batch.Execute("update POP set OO_PI = NULL where OO_PI = {0}", id);
                batch.Execute("delete from PEInterface where PI_ID = {0}", id);
            }
            result = batch.Commit();
            Event(result, EventActions.Delete, EventElements.Interface, false);

            // RESERVED INTERFACES
            batch.Begin();
            foreach (KeyValuePair<string, PEInterfaceToDatabase> pair in interfacelive)
            {
                if (reservedInterfaces.ContainsKey(pair.Key)) batch.Execute("delete from ReservedInterface where RI_ID = {0}", reservedInterfaces[pair.Key]["RI_ID"].ToString());
            }
            result = batch.Commit();
            if (result.AffectedRows > 0) Event(result.AffectedRows + " reserved interface" + (result.AffectedRows > 1 ? "s have " : " has ") + " been found");

            // POP
            batch.Begin();
            foreach (KeyValuePair<string, Row> pair in popInterfaces)
            {
                Row row = pair.Value;
                if (row["OO_PI"].IsNull)
                {
                    if (interfacelive.ContainsKey(pair.Key))
                    {
                        batch.Execute("update POP set OO_PI = {0} where OO_ID = {1}", interfacelive[pair.Key].ID, row["OO_ID"].ToString());
                    }
                }
                else
                {
                    if (!interfacelive.ContainsKey(pair.Key))
                    {
                        batch.Execute("update POP set OO_PI = NULL where OO_ID = {0}", row["OO_ID"].ToString());
                    }
                }
            }
            result = batch.Commit();
            Event(result, EventActions.Update, EventElements.POPInterfaceReference, false);

            #endregion

            #endregion

            #region LATE DELETE

            // DELETE QOS
            batch.Begin();
            foreach (KeyValuePair<string, Row> pair in qosdb)
            {
                if (!qoslive.ContainsKey(pair.Key))
                {
                    Event("QOS DELETE: " + pair.Key);
                    batch.Execute("delete from PEQOS where PQ_ID = {0}", pair.Value["PQ_ID"].ToString());                    
                }
            }
            result = batch.Commit();
            Event(result, EventActions.Delete, EventElements.QOS, false);

            // DELETE ROUTE
            batch.Begin();
            List<string> routenamedelete = new List<string>();
            foreach (KeyValuePair<string, Row> pair in routenamedb)
            {
                if (!routelive.ContainsKey(pair.Key))
                {
                    Event("Route Name DELETE: " + pair.Key);
                    string id = pair.Value["PN_ID"].ToString();
                    batch.Execute("update PEInterface set PI_PN = NULL where PI_PN = {0}", id);
                    routenamedelete.Add(id);
                }
            }
            batch.Commit();
            batch.Begin();
            foreach (string id in routenamedelete)
            {
                batch.Execute("delete from PERouteName where PN_ID = {0}", id);
            }
            result = batch.Commit();
            Event(result, EventActions.Delete, EventElements.VRFReference, false);

            result = Execute("delete from PERouteTarget where PT_PR in (select PR_ID from PERoute left join PERouteName on PN_PR = PR_ID where PN_ID is null)");
            Event(result, EventActions.Delete, EventElements.VRFRouteTarget, false);

            result = Execute("delete from PERoute where PR_ID in (select PR_ID from PERoute left join PERouteName on PN_PR = PR_ID where PN_ID is null)");
            Event(result, EventActions.Delete, EventElements.VRF, false);

            #endregion

            SaveExit();
        }

        #endregion
    }
}
