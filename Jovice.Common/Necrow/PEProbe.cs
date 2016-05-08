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

    class PEInterfaceToDatabase : ElementToDatabase
    {
        private int aggr = -1;

        public int Aggr
        {
            get { return aggr; }
            set { aggr = value; }
        }

        private bool updateAggr = false;

        public bool UpdateAggr
        {
            get { return updateAggr; }
            set { updateAggr = value; }
        }

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

        private int rateLimitInput = -1;

        public int RateLimitInput
        {
            get { return rateLimitInput; }
            set { rateLimitInput = value; }
        }

        private int rateLimitOutput = -1;

        public int RateLimitOutput
        {
            get { return rateLimitOutput; }
            set { rateLimitOutput = value; }
        }

        private bool updateRateLimitInput = false;

        public bool UpdateRateLimitInput
        {
            get { return updateRateLimitInput; }
            set { updateRateLimitInput = value; }
        }

        private bool updateRateLimitOutput = false;

        public bool UpdateRateLimitOutput
        {
            get { return updateRateLimitOutput; }
            set { updateRateLimitOutput = value; }
        }

        private long cirTotalInput = -1;

        public long CirTotalInput
        {
            get { return cirTotalInput; }
            set { cirTotalInput = value; }
        }

        private bool updateCirTotalInput = false;

        public bool UpdateCirTotalInput
        {
            get { return updateCirTotalInput; }
            set { updateCirTotalInput = value; }
        }

        private long cirTotalOutput = -1;

        public long CirTotalOutput
        {
            get { return cirTotalOutput; }
            set { cirTotalOutput = value; }
        }

        private bool updateCirTotalOutput = false;

        public bool UpdateCirTotalOutput
        {
            get { return updateCirTotalOutput; }
            set { updateCirTotalOutput = value; }
        }

        private int cirConfigTotalInput = -1;

        public int CirConfigTotalInput
        {
            get { return cirConfigTotalInput; }
            set { cirConfigTotalInput = value; }
        }

        private bool updateCirConfigTotalInput = false;

        public bool UpdateCirConfigTotalInput
        {
            get { return updateCirConfigTotalInput; }
            set { updateCirConfigTotalInput = value; }
        }

        private int cirConfigTotalOutput = -1;

        public int CirConfigTotalOutput
        {
            get { return cirConfigTotalOutput; }
            set { cirConfigTotalOutput = value; }
        }

        private bool updateCirConfigTotalOutput = false;

        public bool UpdateCirConfigTotalOutput
        {
            get { return updateCirConfigTotalOutput; }
            set { updateCirConfigTotalOutput = value; }
        }

        private int subInterfaceCount = -1;

        public int SubInterfaceCount
        {
            get { return subInterfaceCount; }
            set { subInterfaceCount = value; }
        }

        private bool updateSubInterfaceCount = false;

        public bool UpdateSubInterfaceCount
        {
            get { return updateSubInterfaceCount; }
            set { updateSubInterfaceCount = value; }
        }

        private string parentID = null;

        public string ParentID
        {
            get { return parentID; }
            set { parentID = value; }
        }

        private bool updateParentID = false;

        public bool UpdateParentID
        {
            get { return updateParentID; }
            set { updateParentID = value; }
        }

        private string adjacentID = null;

        public string AdjacentID
        {
            get { return adjacentID; }
            set { adjacentID = value; }
        }

        private bool updateAdjacentID = false;

        public bool UpdateAdjacentID
        {
            get { return updateAdjacentID; }
            set { updateAdjacentID = value; }
        }

        private Dictionary<string, string> adjacentSubifID = null;

        public Dictionary<string, string> AdjacentSubifID
        {
            get { return adjacentSubifID; }
            set { adjacentSubifID = value; }
        }
    }

    internal sealed partial class Probe
    {
        #region Methods

        private void PEProcess()
        {
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
                string ipv6 = row["PT_IPv6"].ToBoolean() == false ? "0" : "1";
                string type = row["PT_Type"].ToBoolean() == false ? "0" : "1";
                string routeTarget = row["PT_Community"].ToString();
                routeTargets.Add(ipv6 + type + routeTarget);
            }

            Dictionary<string, string[]> routetargetinsert = new Dictionary<string, string[]>();
            List<PERouteNameToDatabase> routenameinsert = new List<PERouteNameToDatabase>();
            List<PERouteNameToDatabase> routenameupdate = new List<PERouteNameToDatabase>();

            List<string> hweDisplayIPVPNInstanceVerboseLines = null;
                   
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

                    SendLine("show vrf all");
                    bool timeout;
                    List<string> lines = Read(out timeout);
                    if (timeout) { SaveExit(); return; }

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

                    SendLine("show ip vrf detail | in RD|Export VPN|Import VPN|RT");
                    bool timeout;
                    List<string> lines = Read(out timeout);
                    if (timeout) { SaveExit(); return; }

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
                SendLine("display ip vpn-instance verbose");
                bool timeout;
                hweDisplayIPVPNInstanceVerboseLines = Read(out timeout);
                if (timeout) { SaveExit(); return; }

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
                    int ipv6 = routeTarget[0] == '1' ? 1 : 0;
                    int type = routeTarget[1] == '1' ? 1 : 0;
                    string community = routeTarget.Substring(2);
                    batch.Execute("insert into PERouteTarget(PT_ID, PT_PR, PT_Type, PT_Community, PT_IPv6) values({0}, {1}, {2}, {3}, {4})", Database.ID(), pair.Key, type, community, ipv6);
                }
            }
            result = batch.Commit();
            Event(result, EventActions.Add, EventElements.VRFRouteTarget, false);

            // Route Name
            batch.Begin();
            foreach (PERouteNameToDatabase s in routenameinsert)
            {
                batch.Execute("insert into PERouteName(PN_ID, PN_PR, PN_NO, PN_Name, PN_RD, PN_RDv6) values({0}, {1}, {2}, {3}, {4}, {5})",
                    s.ID, s.RouteID, nodeID, s.Name, s.RD, s.RDIPv6
                    );
            }
            result = batch.Commit();
            Event(result, EventActions.Add, EventElements.VRFReference, false);

            // UPDATE
            batch.Begin();
            foreach (PERouteNameToDatabase s in routenameupdate)
            {
                List<string> v = new List<string>();
                if (s.UpdateRD) v.Add(Format("PN_RD = {0}", s.RD));
                if (s.UpdateRDIPv6) v.Add(Format("PN_RDv6 = {0}", s.RDIPv6));
                if (s.UpdateRouteTargets) v.Add(Format("PN_PR = {0}", s.RouteID));

                if (v.Count > 0) batch.Execute("update PERouteName set " + StringHelper.EscapeFormat(string.Join(",", v.ToArray())) + " where PN_ID = {0}", s.ID);
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

                    SendLine("show policy-map list | in PolicyMap");
                    bool timeout;
                    List<string> lines = Read(out timeout);
                    if (timeout) { SaveExit(); return; }

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

                    SendLine("show policy-map | in Policy Map");
                    bool timeout;
                    List<string> lines = Read(out timeout);
                    if (timeout) { SaveExit(); return; }

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
                SendLine("display qos-profile configuration");
                bool timeout;
                List<string> lines = Read(out timeout);
                if (timeout) { SaveExit(); return; }

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
                // PQ_ID, PQ_NO, PQ_Name, PQ_Bandwidth, PQ_Package
                if (s.Bandwidth == -1) batch.Execute("insert into PEQOS(PQ_ID, PQ_NO, PQ_Name, PQ_Bandwidth, PQ_Package) values({0}, {1}, {2}, null, {3})", s.ID, nodeID, s.Name, s.Package);
                else batch.Execute("insert into PEQOS(PQ_ID, PQ_NO, PQ_Name, PQ_Bandwidth, PQ_Package) values({0}, {1}, {2}, {3}, {4})", s.ID, nodeID, s.Name, s.Bandwidth, s.Package);
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

                SendLine("show int desc");
                bool timeout;
                List<string> lines = Read(out timeout);
                if (timeout) { SaveExit(); return; }

                if (nodeVersion == xr)
                {
                    #region asr

                    // interface
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
                                i.Status = status == "up" ? 1 : 0;
                                i.Protocol = protocol == "up" ? 1 : 0;
                                i.Description = description == String.Empty ? null : description;
                                interfacelive.Add(i.Name, i);
                            }
                        }
                    }

                    // vrf to interface
                    SendLine("show vrf all detail");
                    lines = Read(out timeout);
                    if (timeout) { SaveExit(); return; }

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
                    SendLine("show policy-map targets");
                    lines = Read(out timeout);
                    if (timeout) { SaveExit(); return; }

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
                    SendLine("show ipv4 vrf all interface | in \"Internet address|Secondary address|ipv4\"");
                    lines = Read(out timeout);
                    if (timeout) { SaveExit(); return; }

                    #endregion
                }
                else
                {
                    #region !asr

                    // interface
                    foreach (string line in lines)
                    {
                        //Gi0/1.825                      up             up       VPNIP LIPPO BANK TBK JL.ABDUL LATIF RAU SERANG CID 2205825 SID 4700032-78367
                        //012345678901234567890123456789012345678901234567890123456789
                        //          1         2         3         4         5    5
                        //123456789012345678901234567890123456789012345678901234567890
                        //         1         2         3         4         5         6
                        string lineTrim = line.TrimStart();
                        int length = lineTrim.Length;

                        if (!lineTrim.StartsWith("Interface") && !lineTrim.StartsWith("show"))
                        {
                            string ifname = length >= 55 ? lineTrim.Substring(0, 31).Trim() : null;
                            string status = length >= 55 ? lineTrim.Substring(31, 10).Trim() : null;
                            string protocol = length >= 55 ? lineTrim.Substring(46, 4).Trim() : null;
                            string description = length >= 56 ? lineTrim.Substring(55).Trim() : null;

                            NodeInterface nodeinterface = NodeInterface.Parse(ifname);
                            if (nodeinterface != null)
                            {
                                PEInterfaceToDatabase i = new PEInterfaceToDatabase();
                                i.Name = nodeinterface.GetShort();
                                i.Status = status == "up" ? 1 : 0;
                                i.Protocol = protocol == "up" ? 1 : 0;
                                i.Description = description == String.Empty ? null : description;
                                interfacelive.Add(i.Name, i);
                            }
                        }
                    }

                    // vrf to interface
                    SendLine("show ip vrf interfaces");
                    lines = Read(out timeout);
                    if (timeout) { SaveExit(); return; }

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
                    SendLine("show policy-map interface input brief");
                    lines = Read(out timeout);
                    if (timeout) { SaveExit(); return; }

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
                        SendLine("show policy-map interface | in Service-policy_input|Service-policy_output|Ethernet|Serial");
                        lines = Read(out timeout);
                        if (timeout) { SaveExit(); return; }

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
                        SendLine("show policy-map interface output brief");
                        List<string> tlines = Read(out timeout);
                        if (tlines != null) lines.AddRange(tlines);
                        if (timeout) { SaveExit(); return; }

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
                    SendLine("show interface rate-limit");
                    lines = Read(out timeout);
                    if (timeout) { SaveExit(); return; }

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
                                                    pid.RateLimitInput = kbps;

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
                                                    pid.RateLimitOutput = kbps;

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
                    SendLine("show ip interface | in Internet address|Secondary address|line protocol");
                    lines = Read(out timeout);
                    if (timeout) { SaveExit(); return; }

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

                SendLine("display interface description");
                bool timeout;
                List<string> lines = Read(out timeout);
                if (timeout) { SaveExit(); return; }

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
                                        pid.Status = (status == "up" || status == "up(s)") ? 1 : 0;
                                        pid.Protocol = (status == "up" || status == "up(s)") ? 1 : 0;
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
                        pid.Status = (status == "up" || status == "up(s)") ? 1 : 0;
                        pid.Protocol = (status == "up" || status == "up(s)") ? 1 : 0;
                        interfacelive.Add(port, pid);
                    }
                }

                SendLine("display interface brief");
                lines = Read(out timeout);
                if (timeout) { SaveExit(); return; }

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
                        if (li.Status == 1)
                        {
                            ssubinfup++;
                            if (li.Protocol == 1) ssubinfupup++;
                        }
                        if (inftype == "Hu") { ssubinfhu++; if (li.Status == 1) { ssubinfhuup++; if (li.Protocol == 1) ssubinfhuupup++; } }
                        if (inftype == "Te") { ssubinfte++; if (li.Status == 1) { ssubinfteup++; if (li.Protocol == 1) ssubinfteupup++; } }
                        if (inftype == "Gi") { ssubinfgi++; if (li.Status == 1) { ssubinfgiup++; if (li.Protocol == 1) ssubinfgiupup++; } }
                        if (inftype == "Fa") { ssubinffa++; if (li.Status == 1) { ssubinffaup++; if (li.Protocol == 1) ssubinffaupup++; } }
                        if (inftype == "Et") { ssubinfet++; if (li.Status == 1) { ssubinfetup++; if (li.Protocol == 1) ssubinfetupup++; } }
                        if (inftype == "Ag") { ssubinfag++; if (li.Status == 1) { ssubinfagup++; if (li.Protocol == 1) ssubinfagupup++; } }
                    }
                    else if (li.Aggr != -1)
                    {
                        parentPort = "Ag" + li.Aggr;
                        sinfag++;
                    }
                    else
                    {
                        sinf++;
                        if (li.Status == 1) sinfup++;
                        if (inftype == "Hu") { sinfhu++; if (li.Status == 1) sinfhuup++; }
                        if (inftype == "Te") { sinfte++; if (li.Status == 1) sinfteup++; }
                        if (inftype == "Gi") { sinfgi++; if (li.Status == 1) sinfgiup++; }
                        if (inftype == "Fa") { sinffa++; if (li.Status == 1) sinffaup++; }
                        if (inftype == "Et") { sinfet++; if (li.Status == 1) sinfetup++; }
                        if (inftype == "Se") { sinfse++; if (li.Status == 1) sinfseup++; }
                    }

                    if (parentPort != null)
                    {
                        if (interfacedb.ContainsKey(parentPort)) // cek di existing db
                        {
                            li.ParentID = interfacedb[parentPort]["PI_ID"].ToString();
                            int dot = li.Name.IndexOf('.');
                            if (dot > -1 && li.Name.Length > (dot + 1))
                            {
                                string sifname = li.Name.Substring(dot + 1);

                                if (interfacelive.ContainsKey(parentPort))
                                {
                                    PEInterfaceToDatabase parent = interfacelive[parentPort];
                                    if (parent.AdjacentSubifID != null)
                                    {
                                        if (parent.AdjacentSubifID.ContainsKey(sifname))
                                        {
                                            li.AdjacentID = parent.AdjacentSubifID[sifname];
                                        }
                                    }
                                }
                            }
                        }
                        else if (interfaceinsert.ContainsKey(parentPort)) // cek di interface yg baru
                            li.ParentID = interfaceinsert[parentPort].ID;                        
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

                    bool update = false;
                    StringBuilder updateinfo = new StringBuilder();

                    if (li.ParentID == null)
                    {
                        string adjID = db["PI_TO_MI"].ToString();
                        if (adjID != null)
                        {
                            li.AdjacentSubifID = new Dictionary<string, string>();

                            result = Query("select MI_Name, MI_ID from MEInterface where MI_MI = {0}", adjID);

                            foreach (Row row in result)
                            {
                                string spiid = row["MI_ID"].ToString();
                                string spiname = row["MI_Name"].ToString();

                                int dot = spiname.IndexOf('.');
                                if (dot > -1 && spiname.Length > (dot + 1))
                                {
                                    string sifname = spiname.Substring(dot + 1);
                                    if (!li.AdjacentSubifID.ContainsKey(sifname))
                                    {
                                        li.AdjacentSubifID.Add(sifname, spiid);
                                    }
                                }
                            }
                        }
                        else FindNodeCandidate(li.Description);
                    }
                    if (db["PI_PI"].ToString() != li.ParentID)
                    {
                        update = true;
                        u.UpdateParentID = true;
                        u.ParentID = li.ParentID;
                        updateinfo.Append(li.ParentID == null ? "parent " : "child ");
                    }
                    if (db["PI_TO_MI"].ToString() != li.AdjacentID)
                    {
                        update = true;
                        u.UpdateAdjacentID = true;
                        u.AdjacentID = li.AdjacentID;
                        updateinfo.Append("adj ");
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
                    if ((db["PI_Status"].ToBoolean() ? 1 : 0) != li.Status)
                    {
                        update = true;
                        u.UpdateStatus = true;
                        u.Status = li.Status;
                        updateinfo.Append("stat ");
                    }
                    if ((db["PI_Protocol"].ToBoolean() ? 1 : 0) != li.Protocol)
                    {
                        update = true;
                        u.UpdateProtocol = true;
                        u.Protocol = li.Protocol;
                        updateinfo.Append("prot ");
                    }
                    if (db["PI_Aggregator"].ToInt(-1) != li.Aggr)
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
                    if (db["PI_Rate_Input"].ToInt(-1) != li.RateLimitInput)
                    {
                        update = true;
                        u.UpdateRateLimitInput = true;
                        u.RateLimitInput = li.RateLimitInput;
                        updateinfo.Append("rin ");
                    }
                    if (db["PI_Rate_Output"].ToInt(-1) != li.RateLimitOutput)
                    {
                        update = true;
                        u.UpdateRateLimitOutput = true;
                        u.RateLimitOutput = li.RateLimitOutput;
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
                    if (db["PI_Summary_CIRTotalInput"].ToInt(-1) != li.CirTotalInput)
                    {
                        update = true;
                        u.UpdateCirTotalInput = true;
                        u.CirTotalInput = li.CirTotalInput;
                        updateinfo.Append("cirin ");
                    }
                    if (db["PI_Summary_CIRTotalOutput"].ToInt(-1) != li.CirTotalOutput)
                    {
                        update = true;
                        u.UpdateCirTotalOutput = true;
                        u.CirTotalOutput = li.CirTotalOutput;
                        updateinfo.Append("cirout ");
                    }
                    if (db["PI_Summary_SubInterfaceCount"].ToSmall(-1) != li.SubInterfaceCount)
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
                batch.Execute("insert into PEInterface(PI_ID, PI_NO, PI_Name, PI_Status, PI_Protocol, PI_Aggregator, PI_Description, PI_PN, PI_PQ_Input, PI_PQ_Output, PI_SE, PI_PI, PI_TO_MI, PI_Rate_Input, PI_Rate_Output, PI_Summary_CIRConfigTotalInput, PI_Summary_CIRConfigTotalOutput, PI_Summary_CIRTotalInput, PI_Summary_CIRTotalOutput, PI_Summary_SubInterfaceCount) values({0}, {1}, {2}, {3}, {4}, " + (s.Aggr == -1 ? "NULL" : s.Aggr + "") + ", {5}, {6}, {7}, {8}, {9}, {10}, {11}, " + ((s.RateLimitInput == -1) ? "NULL" : (s.RateLimitInput + "")) + ", " + ((s.RateLimitOutput == -1) ? "NULL" : (s.RateLimitOutput + "")) +
                    ", " + (s.CirConfigTotalInput == -1 ? "NULL" : (s.CirConfigTotalInput + "")) + ", " + (s.CirConfigTotalOutput == -1 ? "NULL" : (s.CirConfigTotalOutput + "")) + ", " + (s.CirTotalInput == -1 ? "NULL" : (s.CirTotalInput + "")) + ", " + (s.CirTotalOutput == -1 ? "NULL" : (s.CirTotalOutput + "")) +
                    ", " + (s.SubInterfaceCount == -1 ? "NULL" : (s.SubInterfaceCount + "")) +
                    ")",
                    s.ID, nodeID, s.Name, s.Status, s.Protocol, s.Description, s.RouteID, s.InputQOSID, s.OutputQOSID, s.ServiceID, s.ParentID, s.AdjacentID
                    );
                interfacereferenceupdate.Add(new Tuple<string, string>(s.AdjacentID, s.ID));
                if (s.IP != null) ipinsert.Add(s.ID, s.IP);
            }
            result = batch.Commit();
            Event(result, EventActions.Add, EventElements.Interface, false);

            // UPDATE
            batch.Begin();
            foreach (PEInterfaceToDatabase s in interfaceupdate)
            {
                List<string> v = new List<string>();
                if (s.UpdateParentID) v.Add(Format("PI_PI = {0}", s.ParentID));
                if (s.UpdateAdjacentID)
                {
                    v.Add(Format("PI_TO_MI = {0}", s.AdjacentID));
                    interfacereferenceupdate.Add(new Tuple<string, string>(s.AdjacentID, s.ID));
                }
                if (s.UpdateDescription)
                {
                    v.Add(Format("PI_Description = {0}", s.Description));
                    v.Add(Format("PI_SE = {0}", s.ServiceID));
                }
                if (s.UpdateStatus) v.Add("PI_Status = " + s.Status);
                if (s.UpdateProtocol) v.Add("PI_Protocol = " + s.Protocol);
                if (s.UpdateAggr)
                {
                    if (s.Aggr == -1)
                        v.Add("PI_Aggregator = NULL");
                    else
                        v.Add("PI_Aggregator = " + s.Aggr);
                }
                if (s.UpdateRouteID) v.Add(Format("PI_PN = {0}", s.RouteID));
                if (s.UpdateInputQOSID) v.Add(Format("PI_PQ_Input = {0}", s.InputQOSID));
                if (s.UpdateOutputQOSID) v.Add(Format("PI_PQ_Output = {0}", s.OutputQOSID));
                if (s.UpdateRateLimitInput)
                {
                    if (s.RateLimitInput > -1) v.Add("PI_Rate_Input = " + s.RateLimitInput);
                    else v.Add("PI_Rate_Input = NULL");
                }
                if (s.UpdateRateLimitOutput)
                {
                    if (s.RateLimitOutput > -1) v.Add("PI_Rate_Output = " + s.RateLimitOutput);
                    else v.Add("PI_Rate_Output = NULL");
                }
                if (s.UpdateCirConfigTotalInput)
                {
                    if (s.CirConfigTotalInput > -1) v.Add("PI_Summary_CIRConfigTotalInput = " + s.CirConfigTotalInput);
                    else v.Add("PI_Summary_CIRConfigTotalInput = NULL");
                }
                if (s.UpdateCirConfigTotalOutput)
                {
                    if (s.CirConfigTotalOutput > -1) v.Add("PI_Summary_CIRConfigTotalOutput = " + s.CirConfigTotalOutput);
                    else v.Add("PI_Summary_CIRConfigTotalOutput = NULL");
                }
                if (s.UpdateCirTotalInput)
                {
                    if (s.CirTotalInput > -1) v.Add("PI_Summary_CIRTotalInput = " + s.CirTotalInput);
                    else v.Add("PI_Summary_CIRTotalInput = NULL");
                }
                if (s.UpdateCirTotalOutput)
                {
                    if (s.CirTotalOutput > -1) v.Add("PI_Summary_CIRTotalOutput = " + s.CirTotalOutput);
                    else v.Add("PI_Summary_CIRTotalOutput = NULL");
                }
                if (s.UpdateSubInterfaceCount)
                {
                    if (s.SubInterfaceCount > -1) v.Add("PI_Summary_SubInterfaceCount = " + s.SubInterfaceCount);
                    else v.Add("PI_Summary_SubInterfaceCount = NULL");
                }

                if (s.IP != null)
                    ipinsert.Add(s.ID, s.IP);
                if (s.DeleteIP != null)
                    ipdelete.Add(s.ID, s.DeleteIPID);

                if (v.Count > 0) batch.Execute("update PEInterface set " + StringHelper.EscapeFormat(string.Join(",", v.ToArray())) + " where PI_ID = {0}", s.ID);
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
                    batch.Execute("insert into PEInterfaceIP(PP_ID, PP_PI, PP_Type, PP_IP) values({0}, {1}, {2}, {3})", Database.ID(), pair.Key, ip.Substring(0, 1), ip.Substring(2));
                }               
            }
            result = batch.Commit();
            Event(result, EventActions.Add, EventElements.InterfaceIP, false);

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
                batch.Execute("delete from PEInterface where PI_ID = {0}", id);
            }
            result = batch.Commit();
            Event(result, EventActions.Delete, EventElements.Interface, false);

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
