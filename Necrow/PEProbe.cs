using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using System.Net;
using Aphysoft.Share;

namespace Center
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
        private PEInterfaceToDatabase[] aggrChilds = null;

        public PEInterfaceToDatabase[] AggrChilds
        {
            get { return aggrChilds; }
            set { aggrChilds = value; }
        }

        private string neighborCheckMITOPI = null;

        public string NeighborCheckMITOPI
        {
            get { return neighborCheckMITOPI; }
            set { neighborCheckMITOPI = value; }
        }

        private bool updateNeighborCheckMITOPI = false;

        public bool UpdateNeighborCheckMITOPI
        {
            get { return updateNeighborCheckMITOPI; }
            set { updateNeighborCheckMITOPI = value; }
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
    }

    class PERouteUseToDatabase : ToDatabase
    {
        private string type;

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        private string routeNameID;

        public string RouteNameID
        {
            get { return routeNameID; }
            set { routeNameID = value; }
        }

        private string network;

        public string Network
        {
            get { return network; }
            set { network = value; }
        }

        private string neighbor;

        public string Neighbor
        {
            get { return neighbor; }
            set { neighbor = value; }
        }

        private string interfaceID;

        public string InterfaceID
        {
            get { return interfaceID; }
            set { interfaceID = value; }
        }

        private string interfaceGone;

        public string InterfaceGone
        {
            get { return interfaceGone; }
            set { interfaceGone = value; }
        }

        private bool updateInterface = false;

        public bool UpdateInterface
        {
            get { return updateInterface; }
            set { updateInterface = value; }
        }

        private int area = -1;

        public int Area
        {
            get { return area; }
            set { area = value; }
        }

        private int process = -1;

        public int Process
        {
            get { return process; }
            set { process = value; }
        }

        private string wildcard = null;

        public string Wildcard
        {
            get { return wildcard; }
            set { wildcard = value; }
        }

        private int bgpAS;

        public int BGPAS
        {
            get { return bgpAS; }
            set { bgpAS = value; }
        }
        
        private int remoteAS = -1;

        public int RemoteAS
        {
            get { return remoteAS; }
            set { remoteAS = value; }
        }

        private bool updateRemoteAS = false;

        public bool UpdateRemoteAS
        {
            get { return updateRemoteAS; }
            set { updateRemoteAS = value; }
        }

        private string prefixListInID = null;

        public string PrefixListInID
        {
            get { return prefixListInID; }
            set { prefixListInID = value; }
        }

        private bool updatePrefixListInID = false;

        public bool UpdatePrefixListInID
        {
            get { return updatePrefixListInID; }
            set { updatePrefixListInID = value; }
        }

        private string prefixListInGone = null;

        public string PrefixListInGone
        {
            get { return prefixListInGone; }
            set { prefixListInGone = value; }
        }

        private bool updatePrefixListInGone = false;

        public bool UpdatePrefixListInGone
        {
            get { return updatePrefixListInGone; }
            set { updatePrefixListInGone = value; }
        }

        private string prefixListOutID = null;

        public string PrefixListOutID
        {
            get { return prefixListOutID; }
            set { prefixListOutID = value; }
        }

        private bool updatePrefixListOutID = false;

        public bool UpdatePrefixListOutID
        {
            get { return updatePrefixListOutID; }
            set { updatePrefixListOutID = value; }
        }

        private string prefixListOutGone = null;

        public string PrefixListOutGone
        {
            get { return prefixListOutGone; }
            set { prefixListOutGone = value; }
        }

        private bool updatePrefixListOutGone = false;

        public bool UpdatePrefixListOutGone
        {
            get { return updatePrefixListOutGone; }
            set { updatePrefixListOutGone = value; }
        }

        private int maximumPrefix = -1;

        public int MaximumPrefix
        {
            get { return maximumPrefix; }
            set { maximumPrefix = value; }
        }

        private bool updateMaximumPrefix = false;

        public bool UpdateMaximumPrefix
        {
            get { return updateMaximumPrefix; }
            set { updateMaximumPrefix = value; }
        }

        private int maximumPrefixThreshold = -1;

        public int MaximumPrefixThreshold
        {
            get { return maximumPrefixThreshold; }
            set { maximumPrefixThreshold = value; }
        }

        private bool updateMaximumPrefixThreshold = false;

        public bool UpdateMaximumPrefixThreshold
        {
            get { return updateMaximumPrefixThreshold; }
            set { updateMaximumPrefixThreshold = value; }
        }

        private bool? maximumPrefixWarningOnly;

        public bool? MaximumPrefixWarningOnly
        {
            get { return maximumPrefixWarningOnly; }
            set { maximumPrefixWarningOnly = value; }
        }

        private bool updateMaximumPrefixWarningOnly = false;

        public bool UpdateMaximumPrefixWarningOnly
        {
            get { return updateMaximumPrefixWarningOnly; }
            set { updateMaximumPrefixWarningOnly = value; }
        }

        private string routePolicyIn = null;

        public string RoutePolicyIn
        {
            get { return routePolicyIn; }
            set { routePolicyIn = value; }
        }

        private bool updateRoutePolicyIn = false;

        public bool UpdateRoutePolicyIn
        {
            get { return updateRoutePolicyIn; }
            set { updateRoutePolicyIn = value; }
        }

        private string routePolicyOut = null;

        public string RoutePolicyOut
        {
            get { return routePolicyOut; }
            set { routePolicyOut = value; }
        }

        private bool updateRoutePolicyOut = false;

        public bool UpdateRoutePolicyOut
        {
            get { return updateRoutePolicyOut; }
            set { updateRoutePolicyOut = value; }
        }

        private string messageDigestKey;

        public string MessageDigestKey
        {
            get { return messageDigestKey; }
            set { messageDigestKey = value; }
        }

        private bool updateMessageDigestKey = false;

        public bool UpdateMessageDigestKey
        {
            get { return updateMessageDigestKey; }
            set { updateMessageDigestKey = value; }
        }

        private string interfaceNetwork;

        public string InterfaceNetwork
        {
            get { return interfaceNetwork; }
            set { interfaceNetwork = value; }
        }

        private bool updateInterfaceNetwork = false;

        public bool UpdateInterfaceNetwork
        {
            get { return updateInterfaceNetwork; }
            set { updateInterfaceNetwork = value; }
        }
    }

    class PEPrefixListToDatabase : ToDatabase
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }

    class PEPrefixEntryToDatabase : ToDatabase
    {
        private string prefixListID;

        public string PrefixListID
        {
            get { return prefixListID; }
            set { prefixListID = value; }
        }
        
        private string network;

        public string Network
        {
            get { return network; }
            set { network = value; }
        }

        private int sequence;

        public int Sequence
        {
            get { return sequence; }
            set { sequence = value; }
        }

        private bool updateSequence = false;

        public bool UpdateSequence
        {
            get { return updateSequence; }
            set { updateSequence = value; }
        }

        private string access;

        public string Access
        {
            get { return access; }
            set { access = value; }
        }

        private bool updateAccess = false;

        public bool UpdateAccess
        {
            get { return updateAccess; }
            set { updateAccess = value; }
        }

        private int ge;

        public int GE
        {
            get { return ge; }
            set { ge = value; }
        }

        private bool updateGE = false;

        public bool UpdateGE
        {
            get { return updateGE; }
            set { updateGE = value; }
        }

        private int le;

        public int LE
        {
            get { return le; }
            set { le = value; }
        }

        private bool updateLE = false;

        public bool UpdateLE
        {
            get { return updateLE; }
            set { updateLE = value; }
        }
    }

    #endregion

    class NeighborGroup : PERouteUseToDatabase
    {
        #region Fields

        #endregion

        #region Constructors

        #endregion
    }

    internal sealed partial class Probe
    {
        private ProbeProcessResult PEProcess()
        {
            ProbeProcessResult probe = new ProbeProcessResult();

            string[] lines = null;
            Batch batch = Batch();            
            Result result;

            #region VRF

            Dictionary<string, PERouteNameToDatabase> routenamelive = new Dictionary<string, PERouteNameToDatabase>();
            Dictionary<string, Row> routenamedb = QueryDictionary("select * from PERouteName where PN_NO = {0}", "PN_Name", nodeID);
            if (routenamedb == null) return DatabaseFailure(probe);
            Dictionary<string, List<string>> routetargetlocaldb = new Dictionary<string, List<string>>();

            result = Query("select * from PERouteName, PERouteTarget where PN_PR = PT_PR and PN_NO = {0}", nodeID);
            if (!result.OK) return DatabaseFailure(probe);

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
            string[] junShowConfigurationRoutingInstancesLines = null;
            string[] junShowConfigurationPolicyOptions = null;

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
                    if (Request("show vrf all", out lines, probe)) return probe;

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
                                routenamelive.Add(name, i);
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
                        routenamelive.Add(name, i);
                    }

                    #endregion
                }
                else
                {
                    #region ios

                    if (Request("show ip vrf detail | in RD|Export VPN|Import VPN|RT", out lines, probe)) return probe;

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
                                routenamelive.Add(name, i);
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
                            routenamelive.Add(name, i);
                        }
                    }
                    #endregion
                }
                #endregion
            }
            else if (nodeManufacture == jun)
            {
                #region jun

                Dictionary<string, List<string>> communities = new Dictionary<string, List<string>>();
                if (Request("show configuration policy-options", out junShowConfigurationPolicyOptions, probe)) return probe;

                string currentStatement = null;
                foreach (string line in junShowConfigurationPolicyOptions)
                {
                    //policy-statement SMS_Hub_STI-export {
                    //0123456789012345678901234567890123456789
                    //           community add 17974:15094;
                    //           0123456789
                    //add 17974:12584;
                    //[ target:17974:14020 target:17974:14021 ];
                    //[ 17974:15632 17974:15633 ];

                    string lineTrim = line.Trim();
                    if (line.StartsWith("policy-statement"))
                    {
                        currentStatement = line.Substring(17, line.IndexOf(' ', 17) - 17);
                        communities.Add(currentStatement, new List<string>());
                    }
                    else if (lineTrim.StartsWith("community "))
                    {
                        string cle = lineTrim.Substring(9).Trim(new char[] { ' ', ';' });
                        if (cle.StartsWith("add "))
                        {
                            string ot = cle.Substring(4);
                            if (ot.StartsWith("target:")) ot = ot.Substring(7);
                            communities[currentStatement].Add(ot);
                        }
                        else if (cle.StartsWith("[ "))
                        {
                            string ot = cle.Substring(1, cle.Length - 3).Trim();
                            string[] ots = ot.Split(StringSplitTypes.Space);

                            foreach (string otx in ots)
                            {
                                string otxx;
                                if (otx.StartsWith("target:")) otxx = otx.Substring(7);
                                else otxx = otx;
                                communities[currentStatement].Add(otxx);
                            }
                        }
                    }
                }

                if (Request("show configuration routing-instances", out junShowConfigurationRoutingInstancesLines, probe)) return probe;
                /*
                0123456789
                EOC_Test {
                    instance-type vrf;
                    route-distinguisher 17974:13072;
                    012345678901234567890123456789
                    vrf-import EOC_Test-import;
                    vrf-export EOC_Test-export;
                */
                string currentVRF = null;
                string currentRD = null;
                List<string> currentRouteTargets = new List<string>();

                foreach (string line in junShowConfigurationRoutingInstancesLines)
                {
                    string lineTrim = line.Trim();
                    bool inactive = false;
                    if (lineTrim.StartsWith("inactive: "))
                    {
                        lineTrim = lineTrim.Substring(10);
                        inactive = true;
                    }

                    if (line.EndsWith("{") && !line.StartsWith(" "))
                    {
                        if (currentVRF != null)
                        {
                            PERouteNameToDatabase i = new PERouteNameToDatabase();
                            i.Name = currentVRF;
                            i.RD = currentRD;
                            i.RouteTargets = currentRouteTargets.ToArray();
                            routenamelive.Add(currentVRF, i);
                        }

                        currentRouteTargets.Clear();
                        currentVRF = lineTrim.Substring(0, lineTrim.LastIndexOf(' '));
                    }
                    else if (lineTrim.StartsWith("route-distinguisher "))
                    {
                        currentRD = lineTrim.Substring(20, lineTrim.IndexOf(';') - 20);
                    }
                    else if (lineTrim.StartsWith("vrf-import "))
                    {
                        string stat = lineTrim.Substring(11, lineTrim.IndexOf(';') - 11);
                        if (communities.ContainsKey(stat))
                            foreach (string statx in communities[stat])
                                currentRouteTargets.Add("01" + statx);
                    }
                    else if (lineTrim.StartsWith("vrf-export "))
                    {
                        string stat = lineTrim.Substring(11, lineTrim.IndexOf(';') - 11);
                        if (communities.ContainsKey(stat))
                            foreach (string statx in communities[stat])
                                currentRouteTargets.Add("00" + statx);
                    }
                }
                if (currentVRF != null)
                {
                    PERouteNameToDatabase i = new PERouteNameToDatabase();
                    i.Name = currentVRF;
                    i.RD = currentRD;
                    i.RouteTargets = currentRouteTargets.ToArray();
                    routenamelive.Add(currentVRF, i);
                }

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
                if (Request("display ip vpn-instance verbose", out hweDisplayIPVPNInstanceVerboseLines, probe)) return probe;

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

                            routenamelive.Add(name, i);

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

                    routenamelive.Add(name, i);
                }

                #endregion
            }

            #endregion

            #region Check

            foreach (KeyValuePair<string, PERouteNameToDatabase> pair in routenamelive)
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

                    u.ID = db["PN_ID"].ToString();
                    li.ID = u.ID;

                    bool update = false;
                    StringBuilder updateinfo = new StringBuilder();

                    if (db["PN_RD"].ToString() != li.RD)
                    {
                        update = true;
                        u.UpdateRD = true;
                        u.RD = li.RD;
                        UpdateInfo(updateinfo, "RD", db["PN_RD"].ToString(), li.RD);
                    }
                    if (db["PN_RDv6"].ToString() != li.RDIPv6)
                    {
                        update = true;
                        u.UpdateRDIPv6 = true;
                        u.RDIPv6 = li.RDIPv6;                        
                        UpdateInfo(updateinfo, "RDv6", db["PN_RDv6"].ToString(), li.RDIPv6);
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
                            UpdateInfo(updateinfo, "RT", routeTargets.Count.ToString() + " rts", li.RouteTargets.Length.ToString() + " rts");
                        }
                    }

                    if (update)
                    {
                        u.Name = pair.Key;
                        routenameupdate.Add(u);
                        Event("VRF Name UPDATE: " + pair.Key + " " + updateinfo.ToString());
                    }
                }
            }

            Summary("VRF_COUNT", routenamelive.Count);

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
                    if (!r.OK) return DatabaseFailure(probe);
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
            if (!result.OK) return DatabaseFailure(probe);
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
            if (!result.OK) return DatabaseFailure(probe);
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
            if (!result.OK) return DatabaseFailure(probe);
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
            if (!result.OK) return DatabaseFailure(probe);
            Event(result, EventActions.Update, EventElements.VRFReference, false);

            #endregion

            routenamedb = QueryDictionary("select * from PERouteName where PN_NO = {0}", "PN_Name", nodeID);
            if (routenamedb == null) return DatabaseFailure(probe);

            #endregion

            #region QOS

            Dictionary<string, PEQOSToDatabase> qoslive = new Dictionary<string, PEQOSToDatabase>();
            Dictionary<string, Row> qosdb = QueryDictionary("select * from PEQOS where PQ_NO = {0}", "PQ_Name", nodeID);
            if (qosdb == null) return DatabaseFailure(probe);
            List<PEQOSToDatabase> qosinsert = new List<PEQOSToDatabase>();

            Event("Checking QOS");

            #region Live

            if (nodeManufacture == cso)
            {
                #region cso

                if (nodeVersion == xr)
                {
                    #region xr

                    if (Request("show policy-map list | in PolicyMap", out lines, probe)) return probe;

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
                            CiscoQOS qos = CiscoQOS.Parse(li.Name);
                            li.Package = qos.Package;
                            li.Bandwidth = qos.Bandwidth;
                            qoslive.Add(name, li);
                        }
                    }

                    #endregion
                }
                else
                {
                    #region ios
                    if (Request("show policy-map | in Policy Map", out lines, probe)) return probe;

                    foreach (string line in lines)
                    {
                        string lineTrim = line.Trim();

                        if (lineTrim.StartsWith("Policy Map "))
                        {
                            string name = lineTrim.Substring(11);

                            PEQOSToDatabase li = new PEQOSToDatabase();
                            li.Name = name;
                            CiscoQOS qos = CiscoQOS.Parse(li.Name);
                            li.Package = qos.Package;
                            li.Bandwidth = qos.Bandwidth;
                            qoslive.Add(name, li);
                        }
                    }
                    #endregion
                }

                #endregion
            }
            else if (nodeManufacture == jun)
            {
                #region jun
                if (Request("show configuration firewall | match \"policer|if-exceeding|bandwidth-limit\"", out lines, probe)) return probe;

                string name = null;
                bool ifExceeding = false;
                string bl = null;

                foreach (string line in lines)
                {
                    string lineTrim = line.Trim();
                    //policer Limit_Rate_10M {
                    //012345678
                    if (line.StartsWith("policer ") && line.EndsWith("{"))
                    {
                        if (name != null)
                        {
                            PEQOSToDatabase li = new PEQOSToDatabase();
                            li.Name = name;
                            li.Bandwidth = (int)Math.Round(QOS.ParseBandwidth(bl, 1));
                            qoslive.Add(name, li);
                        }

                        name = line.Substring(8, line.IndexOf('{') - 9);
                        ifExceeding = false;
                        bl = null;
                    }
                    else if (lineTrim.StartsWith("if-exceeding") && line.EndsWith("{")) ifExceeding = true;
                    else if (ifExceeding)
                    {
                        //bandwidth-limit 20480000;
                        //burst-size-limit 3840000;
                        //0123456789012345678
                        if (lineTrim.StartsWith("bandwidth-limit")) bl = lineTrim.Substring(16, lineTrim.IndexOf(';') - 16);
                    }
                }
                if (name != null)
                {
                    PEQOSToDatabase li = new PEQOSToDatabase();
                    li.Name = name;
                    li.Bandwidth = (int)Math.Round(QOS.ParseBandwidth(bl, 1));
                    qoslive.Add(name, li);
                }

                #endregion
            }
            else if (nodeManufacture == hwe)
            {
                #region hwe

                // not tested, pe hwe still dont use this
                if (Request("display qos-profile configuration", out lines, probe)) return probe;

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
                            HuaweiQOS qos = HuaweiQOS.Parse(li.Name);
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
            if (!result.OK) return DatabaseFailure(probe);
            Event(result, EventActions.Add, EventElements.QOS, false);

            #endregion

            qosdb = QueryDictionary("select * from PEQOS where PQ_NO = {0}", "PQ_Name", nodeID);
            if (qosdb == null) return DatabaseFailure(probe);

            #endregion

            #region INTERFACE

            SortedDictionary<string, PEInterfaceToDatabase> interfacelive = new SortedDictionary<string, PEInterfaceToDatabase>();
            Dictionary<string, Row> interfacedb = QueryDictionary("select * from PEInterface where PI_NO = {0}", "PI_Name", nodeID);
            if (interfacedb == null) return DatabaseFailure(probe);
            Dictionary<string, List<string[]>> ipdb = new Dictionary<string, List<string[]>>();

            result = Query("select PI_Name, PP_ID, CAST(PP_IPv6 as varchar) + '_' + CAST(PP_Order as varchar) + '_' + PP_IP as IPKEY from PEInterface, PEInterfaceIP where PP_PI = PI_ID and PI_NO = {0} order by PI_Name asc", nodeID);
            if (!result.OK) return DatabaseFailure(probe);
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
                ip.Add(new string[] { row["IPKEY"].ToString(), row["PP_ID"].ToString() });
            }

            SortedDictionary<string, PEInterfaceToDatabase> interfaceinsert = new SortedDictionary<string, PEInterfaceToDatabase>();
            List<PEInterfaceToDatabase> interfaceupdate = new List<PEInterfaceToDatabase>();
            Dictionary<string, List<string>> ipinsert = new Dictionary<string, List<string>>();
            Dictionary<string, List<string>> ipdelete = new Dictionary<string, List<string>>();

            ServiceReference interfaceServiceReference = new ServiceReference();

            Event("Checking Interface");

            #region Live

            if (nodeManufacture == cso)
            {
                #region cso    

                /*
IOS
GigabitEthernet0/1.3546 is administratively down, line protocol is down
Description: ASTINET PEMDA TK I PAPUA SID 4703328-23028 MOVE TO PE-D7-JAP-INET
Encapsulation 802.1Q Virtual LAN, Vlan ID  3546.
Last input 0:00:00, output 0:00:00, output hang never
Last input never, output never, output hang never
Last input 3y0w, output 3y0w, output hang never
ASR
GigabitEthernet0/0/0/1.3131 is up, line protocol is up
Description: VPNIP BANK MANDIRI Gedung Pusri Jl. Taman Anggrek-Kemanggisan Jaya CID 21573500 SID 4700037-38261
Encapsulation 802.1Q Virtual LAN, VLAN Id 3131,  loopback not set,
Last input 00:00:00, output 00:00:00

*/

                if (nodeVersion == xr)
                {
                    #region xr

                    // interface
                    if (Request("show interface | in \"line protocol|Description|802.1Q|Last input\"", out lines, probe)) return probe;
                    
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
                                NetworkInterface inf = NetworkInterface.Parse(name);
                                if (inf != null)
                                {
                                    string mid = firstLineTokens[1].Trim();
                                    string last = firstLineTokens[2].Trim();

                                    if (current != null && descriptionBuffer != null) current.Description = descriptionBuffer.ToString().Trim();
                                    descriptionBuffer = null;

                                    if (!mid.StartsWith("delete")) // skip deleted interface
                                    {
                                        current = new PEInterfaceToDatabase();
                                        current.Name = inf.Name;

                                        if (!inf.IsSubInterface) current.InterfaceType = inf.Type;

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
                                    //  Encapsulation 802.1Q Virtual LAN, VLAN Id 55,  loopback not set,
                                    //                                    0123456789
                                    int vlanIdIndex = line.IndexOf("VLAN Id ");
                                    if (vlanIdIndex > -1)
                                    {
                                        int dot1q;
                                        string vlanid = line.Substring(vlanIdIndex + 7, line.IndexOf(',', vlanIdIndex) - (vlanIdIndex + 7)).Trim('.', ' ');
                                        if (int.TryParse(vlanid, out dot1q)) current.Dot1Q = dot1q;
                                    }
                                }
                                else if (linets.StartsWith("Last input"))
                                {
                                    // catet if physical dan protocol down
                                    if (current.Protocol == false && current.Name.IndexOf(".") == -1)
                                    {
                                        //  Last input 3y0w,
                                        //  01234567890123456
                                        current.LastDown = ParseLastInput(linets.Substring(11, linets.IndexOf(", ") - 11).Trim());
                                    }
                                }
                                else if (descriptionBuffer != null) descriptionBuffer.Append(line);
                            }
                        }
                    }
                    if (current != null && descriptionBuffer != null) current.Description = descriptionBuffer.ToString().Trim();

                    // vrf to interface
                    if (Request("show vrf all detail", out lines, probe)) return probe;

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
                                    NetworkInterface networkInterface = NetworkInterface.Parse(lineTrim);
                                    if (networkInterface != null)
                                    {
                                        string name = networkInterface.Name;

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
                    if (Request("show policy-map targets", out lines, probe)) return probe;

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
                                NetworkInterface networkInterface = NetworkInterface.Parse(linex[0]);
                                if (networkInterface != null)
                                {
                                    string name = networkInterface.Name;
                                    string type = networkInterface.Type;
                                    int typerate = -1;
                                    if (type == "Te") typerate = 10485760;
                                    else if (type == "Ge") typerate = 1048576;
                                    else if (type == "Fa") typerate = 102400;
                                    else if (type == "Et") typerate = 10240;

                                    if (interfacelive.ContainsKey(name))
                                    {
                                        string parentPort = null;
                                        if (networkInterface.IsSubInterface)
                                        {
                                            string bport = networkInterface.BaseName;
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

                    // ipv4
                    if (Request("show ipv4 vrf all interface | in \"Internet address|Secondary address|ipv4\"", out lines, probe)) return probe;

                    PEInterfaceToDatabase currentInterface = null;
                    int linen = 0;
                    int ipv4SecondaryAddressCtr = 2;
                    foreach (string line in lines)
                    {
                        linen++;
                        if (linen <= (nodeVersion == xr ? 2 : 1)) continue;

                        string linex = line.Trim();

                        if (currentInterface != null && linex.StartsWith("Internet address"))
                        {
                            string ip = linex.Substring(20);
                            if (currentInterface.IP == null) currentInterface.IP = new List<string>();
                            currentInterface.IP.Add("0_1_" + ip);
                        }
                        else if (currentInterface != null && linex.StartsWith("Secondary address"))
                        {
                            string ip = linex.Substring(18);
                            if (currentInterface.IP == null) currentInterface.IP = new List<string>();
                            currentInterface.IP.Add("0_" + ipv4SecondaryAddressCtr + "_" + ip);
                            ipv4SecondaryAddressCtr++;
                        }
                        else
                        {
                            currentInterface = null;
                            ipv4SecondaryAddressCtr = 2;

                            if (linex.IndexOf(' ') > -1)
                            {
                                string name = linex.Substring(0, linex.IndexOf(' '));
                                NetworkInterface networkInterface = NetworkInterface.Parse(name);
                                if (networkInterface != null)
                                {
                                    string shortName = networkInterface.Name;
                                    if (interfacelive.ContainsKey(shortName))
                                        currentInterface = interfacelive[shortName];
                                }
                            }
                        }
                    }

                    // ipv6
                    if (Request("show ipv6 vrf all interface | in \"ipv6 protocol|subnet is\"", out lines, probe)) return probe;

                    currentInterface = null;
                    int ipv6AddressCtr = 1;

                    foreach (string line in lines)
                    {
                        string linetrim = line.Trim();

                        if (line.IndexOf("ipv6 protocol") > -1)
                        {
                            currentInterface = null;
                            ipv6AddressCtr = 1;

                            string name = linetrim.Substring(0, linetrim.IndexOf(' '));
                            NetworkInterface networkInterface = NetworkInterface.Parse(name);
                            if (networkInterface != null)
                            {
                                string shortName = networkInterface.Name;
                                if (interfacelive.ContainsKey(shortName))
                                    currentInterface = interfacelive[shortName];
                            }
                        }
                        else if (currentInterface != null && line.IndexOf("subnet is") > -1)
                        {
                            //    2001:4488:0:94::1, subnet is 2001:4488:0:94::/64
                            string ip = linetrim.Substring(0, linetrim.IndexOf(','));
                            string nm = linetrim.Substring(linetrim.IndexOf('/') + 1);

                            if (currentInterface.IP == null) currentInterface.IP = new List<string>();
                            currentInterface.IP.Add("1_" + ipv6AddressCtr + "_" + ip + "/" + nm);
                            ipv6AddressCtr++;
                        }
                    }

                    #endregion
                }
                else
                {
                    #region ios
                    
                    // interface
                    if (Request("show interface | in line protocol|Description|802.1Q|Last input", out lines, probe)) return probe;
                    
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
                                NetworkInterface inf = NetworkInterface.Parse(name);
                                if (inf != null)
                                {
                                    string mid = firstLineTokens[1].Trim();
                                    string last = firstLineTokens[2].Trim();

                                    if (current != null && descriptionBuffer != null) current.Description = descriptionBuffer.ToString().Trim();
                                    descriptionBuffer = null;

                                    if (!mid.StartsWith("delete")) // skip deleted interface
                                    {
                                        current = new PEInterfaceToDatabase();
                                        current.Name = inf.Name;

                                        if (!inf.IsSubInterface) current.InterfaceType = inf.Type;

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
                                    //                                    0123456789
                                    string vlanid = line.Substring(line.IndexOf("Vlan ID") + 7).Trim('.', ' ');
                                    if (int.TryParse(vlanid, out dot1q)) current.Dot1Q = dot1q;
                                }
                                else if (linets.StartsWith("Last input"))
                                {
                                    // catet if physical dan protocol down
                                    if (current.Protocol == false && current.Name.IndexOf(".") == -1)
                                    {
                                        //  Last input 3y0w,
                                        //  01234567890123456
                                        current.LastDown = ParseLastInput(linets.Substring(11, linets.IndexOf(", ") - 11).Trim());
                                    }
                                }
                                else if (descriptionBuffer != null) descriptionBuffer.Append(line);
                            }
                        }
                    }
                    if (current != null && descriptionBuffer != null) current.Description = descriptionBuffer.ToString().Trim();

                    // vrf to interface
                    if (Request("show ip vrf interfaces", out lines, probe)) return probe;

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
                    if (Request("show policy-map interface input brief", out lines, probe)) return probe;

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
                        if (Request("show policy-map interface | in Service-policy_input|Service-policy_output|Ethernet|Serial", out lines, probe)) return probe;

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

                                    NetworkInterface networkInterface = NetworkInterface.Parse(ifcandidate);
                                    if (networkInterface != null)
                                    {
                                        string name = networkInterface.Name;
                                        if (interfacelive.ContainsKey(name))
                                        {
                                            currentif = name;

                                            if (networkInterface.IsSubInterface)
                                            {
                                                string bport = networkInterface.BaseName;
                                                if (interfacelive.ContainsKey(bport))
                                                {
                                                    parentPort = bport;
                                                    typerate = networkInterface.TypeRate;
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
                        if (Request("show policy-map interface output brief", out tlines, probe)) return probe;
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
                                NetworkInterface networkInterface = NetworkInterface.Parse(lineTrim);

                                if (networkInterface != null)
                                {
                                    string name = networkInterface.Name;

                                    if (interfacelive.ContainsKey(name))
                                    {
                                        string typeif = networkInterface.Type;
                                        int typerate = networkInterface.TypeRate;

                                        string parentPort = null;

                                        if (networkInterface.IsSubInterface)
                                        {
                                            string bport = networkInterface.BaseName;
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

                    // port-channel
                    if (Request("sh etherchannel detail | in Port:|Port-channel:", out lines, probe)) return probe;

                    //0123456789012345678
                    //Port: Gi1/3
                    //Port: Gi5/1
                    //Port-channel: Po1    (Primary Aggregator)
                    List<string> currentPorts = new List<string>();

                    foreach (string line in lines)
                    {
                        if (line.StartsWith("Port: "))
                        {
                            string port = line.Substring(6).Trim();
                            NetworkInterface nif = NetworkInterface.Parse(port);
                            if (nif != null)
                            {
                                currentPorts.Add(nif.Name);
                            }
                        }
                        else if (line.StartsWith("Port-channel: "))
                        {
                            string port = line.Substring(14).Split(StringSplitTypes.Space)[0];
                            NetworkInterface nif = NetworkInterface.Parse(port);
                            if (nif != null)
                            {
                                string portn = nif.PortName;
                                int porti;
                                if (int.TryParse(portn, out porti))
                                {
                                    foreach (string child in currentPorts)
                                    {
                                        if (interfacelive.ContainsKey(child))
                                        {
                                            interfacelive[child].Aggr = porti;
                                        }
                                    }
                                }
                            }
                            currentPorts.Clear();
                        }
                    }

                    // rate-limit
                    if (Request("show interface rate-limit", out lines, probe)) return probe;

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
                                    NetworkInterface networkInterface = NetworkInterface.Parse(lineTrim.Substring(0, space));
                                    if (networkInterface != null)
                                    {
                                        parentPort2 = null;
                                        currentinterface = networkInterface.Name;

                                        if (networkInterface.IsSubInterface)
                                        {
                                            string bport = networkInterface.BaseName;
                                            if (interfacelive.ContainsKey(bport))
                                            {
                                                parentPort2 = bport;
                                                typerate2 = networkInterface.TypeRate;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // ip
                    if (Request("show ip interface | in Internet address|Secondary address|line protocol", out lines, probe)) return probe;
                    
                    PEInterfaceToDatabase currentInterface = null;
                    int linen = 0;
                    int secondaryAddressCtr = 2;
                    foreach (string line in lines)
                    {
                        linen++;
                        if (linen <= (nodeVersion == xr ? 2 : 1)) continue;

                        string linex = line.Trim();

                        if (currentInterface != null && linex.StartsWith("Internet address"))
                        {
                            string ip = linex.Substring(20);
                            if (currentInterface.IP == null) currentInterface.IP = new List<string>();
                            currentInterface.IP.Add("0_1_" + ip);
                        }
                        else if (currentInterface != null && linex.StartsWith("Secondary address"))
                        {
                            string ip = linex.Substring(18);
                            if (currentInterface.IP == null) currentInterface.IP = new List<string>();
                            currentInterface.IP.Add("0_" + secondaryAddressCtr + "_" + ip);
                            secondaryAddressCtr++;
                        }
                        else
                        {
                            currentInterface = null;
                            secondaryAddressCtr = 2;

                            if (linex.IndexOf(' ') > -1)
                            {
                                string name = linex.Substring(0, linex.IndexOf(' '));
                                NetworkInterface networkInterface = NetworkInterface.Parse(name);
                                if (networkInterface != null)
                                {
                                    string shortName = networkInterface.Name;
                                    if (interfacelive.ContainsKey(shortName))
                                        currentInterface = interfacelive[shortName];
                                }
                            }
                        }
                    }

                    #endregion
                }

                #endregion
            }
            else if (nodeManufacture == jun)
            {
                #region jun

                Dictionary<string, Tuple<bool, bool>> subifStatProt = new Dictionary<string, Tuple<bool, bool>>();

                if (Request("show interfaces terse", out lines, probe)) return probe;
                foreach (string line in lines)
                {
                    string lineTrim = line.Trim();
                    //ge-6/1/0.1002           up    up   aenet    --> ae1.1002
                    string[] tokens = lineTrim.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);
                    if (tokens.Length >= 3)
                    {
                        NetworkInterface nif = NetworkInterface.Parse(tokens[0]);
                        if (nif != null && nif.IsSubInterface)
                        {
                            subifStatProt.Add(nif.Name, new Tuple<bool, bool>(tokens[1] == "up", tokens[2] == "up"));
                        }
                    }
                }

                if (Request("show interfaces detail | match \"^ *(Physical interface|Logical interface|Description:|Flags:|Last flapped|Link-level type|Addresses,|Destination:|Policer:)\"", out lines, probe)) return probe;

                //Physical interface: ge-6/0/0, Enabled, Physical link is Up
                //012345678901234567890123456789
                //  Logical interface ge-1/0/0.56 (Index 139) (SNMP ifIndex 669) (Generation 204)
                //  Last flapped   : 2016-05-13 13:38:23 WIT                 
                PEInterfaceToDatabase current = null;
                bool physicalInterface = false;
                bool collectAddress = false;
                int ipv4SecondaryAddressCtr = 1;
                int ipv6SecondaryAddressCtr = 1;

                foreach (string line in lines)
                {
                    string lineTrim = line.Trim();
                    if (line.StartsWith("Physical interface: "))
                    {
                        string[] splits = line.Substring(19).Trim().Split(StringSplitTypes.Comma);

                        if (splits.Length == 3)
                        {
                            string ifname = splits[0];
                            NetworkInterface nif = NetworkInterface.Parse(ifname);
                            if (nif != null)
                            {
                                current = new PEInterfaceToDatabase();
                                current.Name = nif.Name;
                                current.Enable = splits[1].Trim() == "Enabled";
                                current.Status = current.Enable;
                                current.Protocol = splits[2].Trim() == "Physical link is Up";

                                if (nif.Name.StartsWith("Ag")) current.InterfaceType = "Ag";

                                physicalInterface = true;
                                collectAddress = false;
                                ipv4SecondaryAddressCtr = 1;

                                interfacelive.Add(current.Name, current);
                            }
                            else current = null;
                        }
                    }
                    else if (line.StartsWith("  Logical interface "))
                    {
                        string ifname = line.Substring(20, line.IndexOf(' ', 20) - 20);
                        NetworkInterface nif = NetworkInterface.Parse(ifname);
                        if (nif != null)
                        {
                            current = new PEInterfaceToDatabase();
                            current.Name = nif.Name;

                            if (subifStatProt.ContainsKey(nif.Name))
                            {
                                current.Enable = subifStatProt[nif.Name].Item1;
                                current.Status = current.Enable;
                                current.Protocol = subifStatProt[nif.Name].Item2;
                            }

                            physicalInterface = false;
                            collectAddress = false;
                            ipv4SecondaryAddressCtr = 1;

                            interfacelive.Add(current.Name, current);
                        }
                        else current = null;
                    }
                    else if (current != null)
                    { 
                        if (lineTrim.StartsWith("Description:"))
                        {
                            //Description: IPTV MULTICAST TRAFFIC
                            //01234567890123456789
                            current.Description = lineTrim.Substring(13);
                        }
                        else if (lineTrim.StartsWith("Flags:"))
                        {
                            //SNMP-Traps 0x4000 VLAN-Tag [ 0x8100.1002 ]  Encapsulation: ENET2
                            string lineValue = lineTrim.Substring(7);
                            int vtag = lineValue.IndexOf("VLAN-Tag");
                            if (vtag > -1)
                            {
                                string vtags = lineValue.Substring(vtag);
                                int vtagdot = vtags.IndexOf('.');
                                if (vtagdot > -1)
                                {
                                    string vlanid = vtags.Substring(vtagdot + 1, vtags.IndexOf(' ', vtagdot) - vtagdot - 1);
                                    int dot1q;
                                    if (int.TryParse(vlanid, out dot1q))
                                    {
                                        if (dot1q > 0)
                                            current.Dot1Q = dot1q;
                                    }
                                }
                            }
                        }
                        else if (lineTrim.StartsWith("Link-level type") && physicalInterface && !current.Name.StartsWith("Ag"))
                        {
                            // Link-level type: Ethernet, MTU: 9014, Speed: 10Gbps, BPDU Error: None
                            string[] tokens = lineTrim.Split(StringSplitTypes.Comma);
                            foreach (string token in tokens)
                            {
                                string[] pair = token.Trim().Split(new char[] { ':' });
                                if (pair.Length == 2)
                                {
                                    string key = pair[0].Trim();
                                    string value = pair[1].Trim().ToLower();

                                    if (key == "Speed")
                                    {
                                        if (value == "10gbps") current.InterfaceType = "Te";
                                        else if (value == "1g" || value == "1gbps" || value == "1000m" || value == "1000mbps") current.InterfaceType = "Gi";
                                        else if (value == "100m" || value == "100mbps") current.InterfaceType = "Fa";
                                        else if (value == "10m" || value == "10mbps") current.InterfaceType = "Et";
                                        break;
                                    }
                                }
                            }
                        }
                        else if (lineTrim.StartsWith("Addresses,")) collectAddress = true;
                        else if (collectAddress && lineTrim.StartsWith("Destination:"))
                        {
                            collectAddress = false;
                            string[] tokens = lineTrim.Split(StringSplitTypes.Comma, StringSplitOptions.RemoveEmptyEntries);
                            // Destination: 10.1.3.36/30, Local: 10.1.3.37
                            if (tokens.Length >= 2)
                            {
                                string dest = tokens[0].Trim();
                                string locl = tokens[1].Trim();

                                if (dest.IndexOf('/') > -1)
                                {
                                    string[] slashs = dest.Split(new char[] { '/' });
                                    if (slashs.Length == 2)
                                    {
                                        int slash = -1;
                                        if (!int.TryParse(slashs[1], out slash)) slash = -1;
                                        if (slash > -1)
                                        {
                                            string[] locals = locl.Split(new char[] { ':' }, 2);
                                            if (locals.Length == 2)
                                            {
                                                string iplocal = locals[1].Trim() + "/" + slash;
                                                if (current.IP == null) current.IP = new List<string>();
                                                if (iplocal.IndexOf(':') > -1)
                                                {
                                                    current.IP.Add("1_" + ipv6SecondaryAddressCtr + "_" + iplocal);
                                                    ipv6SecondaryAddressCtr++;
                                                }
                                                else
                                                {
                                                    current.IP.Add("0_" + ipv4SecondaryAddressCtr + "_" + iplocal);
                                                    ipv4SecondaryAddressCtr++;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (lineTrim.StartsWith("Policer: "))
                        {
                            //Input: Limit_Rate_10M-ge-0/0/0.140-inet-i, Output: Limit_Rate_10M-ge-0/0/0.140-inet-o
                            string lineValue = lineTrim.Substring(9);
                            string[] tokens = lineValue.Split(StringSplitTypes.Comma);
                            foreach (string token in tokens)
                            {
                                string tokenTrim = token.Trim();
                                if (tokenTrim.StartsWith("Input: "))
                                {
                                    string tokenValue = tokenTrim.Substring(7);
                                    foreach (KeyValuePair<string, Row> pair in qosdb)
                                    {
                                        if (tokenValue.StartsWith(pair.Key + "-"))
                                        {
                                            current.InputQOSID = pair.Value["PQ_ID"].ToString();
                                            break;
                                        }
                                    }
                                }
                                else if (tokenTrim.StartsWith("Output: "))
                                {
                                    string tokenValue = tokenTrim.Substring(8);
                                    foreach (KeyValuePair<string, Row> pair in qosdb)
                                    {
                                        if (tokenValue.StartsWith(pair.Key + "-"))
                                        {
                                            current.OutputQOSID = pair.Value["PQ_ID"].ToString();
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                        else if (lineTrim.StartsWith("Last flapped") && !current.Name.StartsWith("Ag"))
                        {
                            //Last flapped   : 2016-11-08 14:33:30 WIT (9w0d 09:00 ago)
                            //                 0123456789 01234567
                            //012345678901234567890123456789
                            string value = lineTrim.Substring(lineTrim.IndexOf(':') + 2);
                            string[] tokens = value.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);
                            string date = tokens[0];
                            string time = tokens[1];

                            if (current.Protocol == false)
                            {
                                int year, month, day;
                                int hour, min, sec;
                                if (int.TryParse(date.Substring(0, 4), out year) &&
                                    int.TryParse(date.Substring(5, 2), out month) &&
                                    int.TryParse(date.Substring(8, 2), out day) &&
                                    int.TryParse(time.Substring(0, 2), out hour) &&
                                    int.TryParse(time.Substring(3, 2), out min) &&
                                    int.TryParse(time.Substring(6, 2), out sec))
                                {
                                    current.LastDown = (new DateTime(year, month, day, hour, min, sec)) - nodeTimeOffset;
                                }
                            }
                            else
                                current.LastDown = null;

                        }
                    }
                }

                if (Request("show lacp interfaces", out lines, probe)) return probe;

                int aesid = -1;
                bool lacpif = false;
                foreach (string line in lines)
                {
                    //Aggregated interface: ae1
                    //012345678901234567890123456789
                    if (line.StartsWith("Aggregated interface: ae"))
                    {
                        string aes = line.Substring(24);
                        if (!int.TryParse(aes, out aesid)) aesid = -1;
                        lacpif = false;
                    }
                    else if (!lacpif && line.StartsWith("    LACP protocol:")) lacpif = true;
                    else if (lacpif)
                    {
                        string lineTrim = line.Trim();
                        string[] tokens = lineTrim.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);
                        if (tokens.Length >= 1)
                        {
                            NetworkInterface nif = NetworkInterface.Parse(tokens[0]);
                            if (nif != null)
                            {
                                string name = nif.Name;
                                if (interfacelive.ContainsKey(name)) interfacelive[name].Aggr = aesid;
                            }
                        }
                    }
                }

                // vrf
                string currentVRF = null;
                foreach (string line in junShowConfigurationRoutingInstancesLines)
                {
                    string lineTrim = line.Trim();
                    bool inactive = false;
                    if (lineTrim.StartsWith("inactive: "))
                    {
                        lineTrim = lineTrim.Substring(10);
                        inactive = true;
                    }
                    if (line.EndsWith("{") && !line.StartsWith(" "))
                    {
                        currentVRF = lineTrim.Substring(0, lineTrim.LastIndexOf(' '));
                    }
                    else if (line.StartsWith("    interface "))
                    {
                        //interface aaa
                        //01234567890
                        string ifname = lineTrim.Substring(10, lineTrim.IndexOf(';') - 10);
                        NetworkInterface nif = NetworkInterface.Parse(ifname);
                        if (nif != null)
                        {
                            string name = nif.Name;
                            if (interfacelive.ContainsKey(name) && routenamedb.ContainsKey(currentVRF))
                                interfacelive[name].RouteID = routenamedb[currentVRF]["PN_ID"].ToString();
                        }
                    }
                }

                #endregion
            }
            else if (nodeManufacture == hwe)
            {
                #region hwe

                if (Request("display interface description", out lines, probe)) return probe;

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
                                        pid.Description = description.Length > 0 ? description.ToString() : null;
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

                                //if (inf.StartsWith("Eth-Trunk"))
                                //    Event("test");

                                NetworkInterface nif = NetworkInterface.Parse(inf);
                                if (nif != null)
                                {
                                    if (nif.Type == "Hu" || nif.Type == "Te") port = "Gi" + nif.PortName;
                                    else port = nif.Name;
                                    
                                }
                                if (port != null) description.Append(line.Substring(47).TrimStart());
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
                        pid.Description = description.Length > 0 ? description.ToString() : null;
                        pid.Status = (status == "up" || status == "up(s)");
                        pid.Protocol = (status == "up" || status == "up(s)");
                        interfacelive.Add(port, pid);
                    }
                }

                //main interface
                if (Request("display interface main | in current state|down time|BW", out lines, probe)) return probe;

                PEInterfaceToDatabase currentInterfaceToDatabase = null;

                foreach (string line in lines)
                {
                    if (line.IndexOf("current state") > -1 && !line.StartsWith("Line protocol"))
                    {
                        currentInterfaceToDatabase = null;
                        string[] splits = line.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);
                        NetworkInterface nif = NetworkInterface.Parse(splits[0]);
                        if (nif != null)
                        {
                            string name;
                            if (nif.Type == "Hu" || nif.Type == "Te") name = "Gi" + nif.PortName;
                            else name = nif.Name;

                            if (interfacelive.ContainsKey(name))
                            {
                                currentInterfaceToDatabase = interfacelive[name];
                                currentInterfaceToDatabase.InterfaceType = nif.Type; // default
                            }
                        }
                    }
                    else if (currentInterfaceToDatabase != null && line.IndexOf("BW") > -1)
                    {
                        string itype = null;
                        int indx;
                        if (line.IndexOf("BW: 1000M") > -1 || line.IndexOf("Port BW: 1G") > -1) itype = "Gi";
                        else if (line.IndexOf("Port BW: 10G") > -1) itype = "Te";
                        else if ((indx = line.IndexOf("max BW: ")) > -1)
                        {
                            string lineTrim = line.Trim();
                            string range = lineTrim.Substring(indx + 8, lineTrim.IndexOf(',', indx) - (indx + 8));
                            if (range.IndexOf("~") > -1)
                            {
                                string[] toks = range.Split(new string[] { "~", "Mbps" }, StringSplitOptions.RemoveEmptyEntries);
                                if (toks.Length >= 2)
                                {
                                    int t1 = int.Parse(toks[0]);
                                    int t2 = int.Parse(toks[1]);
                                    int mix = t1 + ((t2 - t1) / 2);

                                    if (mix >= 50000 && mix < 500000) itype = "Hu";
                                    else if (mix >= 5000 && mix < 50000) itype = "Te";
                                    else if (mix < 5000 && mix >= 500) itype = "Gi";
                                    else if (mix < 500 && mix > 50) itype = "Fa";
                                }
                            }
                        }
                        if (itype != null) currentInterfaceToDatabase.InterfaceType = itype;
                    }
                    else if (currentInterfaceToDatabase != null && line.StartsWith("Last physical down time"))
                    {
                        //Last physical down time : 2013-11-07 01:05:24 UTC+07:00
                        //01234567890123456789012345678901234567890123456789
                        //                          1234567890123456789
                        //                          0123456789012345678
                        if (currentInterfaceToDatabase.Protocol == false)
                        {
                            string dtim = line.Substring(26, 19);
                            int year, month, day;
                            int hour, min, sec;
                            if (int.TryParse(dtim.Substring(0, 4), out year) &&
                                int.TryParse(dtim.Substring(5, 2), out month) &&
                                int.TryParse(dtim.Substring(8, 2), out day) &&
                                int.TryParse(dtim.Substring(11, 2), out hour) &&
                                int.TryParse(dtim.Substring(14, 2), out min) &&
                                int.TryParse(dtim.Substring(17, 2), out sec))
                            {
                                currentInterfaceToDatabase.LastDown = (new DateTime(year, month, day, hour, min, sec)) - nodeTimeOffset;
                            }
                        }
                        else
                            currentInterfaceToDatabase.LastDown = null;

                        currentInterfaceToDatabase = null;
                    }
                }
                
                if (Request("display interface brief", out lines, probe)) return probe;

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
                                NetworkInterface inf = NetworkInterface.Parse(poe);
                                if (inf != null)
                                {
                                    string normal = null;
                                    // fix missing Hu and Te on huawei devices
                                    if (inf.Type == "Hu") normal = "Gi" + inf.Interface;
                                    else if (inf.Type == "Te") normal = "Gi" + inf.Interface;
                                    else normal = inf.Name;
                                    interfacelive[normal].Aggr = aggr;
                                }
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

                if (Request(@"disp cur int | in interface|vlan-type\ dot1q|qos\ car\ cir|ip\ address|ipv6\ address", out lines, probe)) return probe;

                //interface Eth-Trunk25.3648
                //01234567890123456
                // vlan-type dot1q 3648
                // qos car cir 102400 cbs 18700000 green pass red discard inbound
                // qos car cir 102400 cbs 18700000 green pass red discard outbound
                // ip address 61.94.229.5 255.255.255.252
                // ipv6 enable
                // ipv6 address 2001:4488:1::6D/126
                // 01234567890123456789
                //interface Eth-Trunk25.3649
                // vlan-type dot1q 3649

                // ip address unnumbered interface LoopBack0


                PEInterfaceToDatabase current = null;
                PEInterfaceToDatabase currentParent = null;
                int typerate = -1;
                int ipv4SecondaryOrder = 1;
                int ipv6SecondaryOrder = 1;

                foreach (string line in lines)
                {
                    if (line.StartsWith("interface "))
                    {
                        current = null;
                        ipv4SecondaryOrder = 1;
                        ipv6SecondaryOrder = 1;

                        string ifname = line.Substring(10).Trim();
                        // Eth-Trunk1234.5678
                        // 0123456789
                        NetworkInterface inf = NetworkInterface.Parse(ifname);
                        if (inf != null)
                        {
                            string sn = inf.Name;
                            if (interfacelive.ContainsKey(sn))
                            {
                                current = interfacelive[sn];

                                if (inf.IsSubInterface)
                                {
                                    string bport = inf.BaseName;
                                    if (interfacelive.ContainsKey(bport))
                                    {
                                        currentParent = interfacelive[bport];
                                        typerate = inf.TypeRate;
                                    }
                                }
                                else currentParent = null;
                            }
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
                            }
                        }
                        else if (linetrim.StartsWith("ip address"))
                        {
                            string[] splits = linetrim.Split(StringSplitTypes.Space);
                            //ip address 61.94.229.5 255.255.255.252 sub
                            //ip address unnumbered interface LoopBack0
                            if (splits.Length >= 4)
                            {
                                string ip = splits[2];
                                string nm = splits[3];

                                if (ip != "unnumbered")
                                {
                                    IPAddress valid;
                                    if (IPAddress.TryParse(ip, out valid) && IPAddress.TryParse(nm, out valid))
                                    {
                                        if (current.IP == null) current.IP = new List<string>();
                                        int cidr = IPNetwork.ToCidr(IPAddress.Parse(nm));
                                        current.IP.Add("0_" + ipv4SecondaryOrder + "_" + ip + "/" + cidr);
                                        ipv4SecondaryOrder++;
                                    }
                                }
                            }
                        }
                        else if (linetrim.StartsWith("ipv6 address"))
                        {
                            string[] splits = linetrim.Split(StringSplitTypes.Space);
                            //ipv6 address 2001:4488:1::6D/126
                            if (splits.Length >= 3)
                            {
                                string ip = splits[2];
                                if (ip != "unnumbered" && ip != "auto")
                                {
                                    string[] ipparts = ip.Split(new char[] { '/' });
                                    IPAddress valid;
                                    if (IPAddress.TryParse(ipparts[0], out valid))
                                    {
                                        if (current.IP == null) current.IP = new List<string>();
                                        current.IP.Add("1_" + ipv6SecondaryOrder + "_" + ip);
                                        ipv6SecondaryOrder++;
                                    }
                                }
                            }
                        }
                        else if (linetrim.StartsWith("qos car cir"))
                        {
                            string[] splits = linetrim.Split(StringSplitTypes.Space);
                            if (splits.Length > 3)
                            {
                                bool inbound = false;
                                if (linetrim.EndsWith("inbound")) inbound = true;

                                int kbps;
                                if (int.TryParse(splits[3], out kbps))
                                {
                                    if (inbound) current.RateInput = kbps;
                                    else current.RateOutput = kbps;

                                    if (currentParent != null)
                                    {
                                        if (kbps > 0)
                                        {
                                            if (inbound)
                                            {
                                                int cur = currentParent.CirConfigTotalInput;
                                                if (cur == -1) cur = 0;
                                                currentParent.CirConfigTotalInput = cur + kbps;

                                                long curR = currentParent.CirTotalInput;
                                                if (curR == -1) curR = 0;
                                                currentParent.CirTotalInput = curR + kbps;
                                            }
                                            else
                                            {
                                                int cur = currentParent.CirConfigTotalOutput;
                                                if (cur == -1) cur = 0;
                                                currentParent.CirConfigTotalOutput = cur + kbps;

                                                long curR = currentParent.CirTotalOutput;
                                                if (curR == -1) curR = 0;
                                                currentParent.CirTotalOutput = curR + kbps;
                                            }
                                        }
                                        else if (typerate > -1)
                                        {
                                            if (inbound)
                                            {
                                                long curR = currentParent.CirTotalInput;
                                                if (curR == -1) curR = 0;
                                                currentParent.CirTotalInput = curR + typerate;
                                            }
                                            else
                                            {
                                                long curR = currentParent.CirTotalOutput;
                                                if (curR == -1) curR = 0;
                                                currentParent.CirTotalOutput = curR + typerate;
                                            }
                                        }
                                    }
                                }
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
                NetworkInterface inf = NetworkInterface.Parse(li.Name);

                if (inf != null)
                {
                    if (inf.IsSubInterface)
                    {
                        string parent = inf.BaseName;
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

            List<Tuple<string, string, string, string, string, string>> vPEPhysicalInterfaces = null;
            bool vExists = false;
            lock (NecrowVirtualization.PESync)
            {
                foreach (Tuple<string, List<Tuple<string, string, string, string, string, string>>> v in NecrowVirtualization.PEPhysicalInterfaces)
                {
                    if (v.Item1 == nodeName)
                    {
                        vPEPhysicalInterfaces = v.Item2;
                        vExists = true;
                    }
                }
                if (!vExists)
                {
                    vPEPhysicalInterfaces = new List<Tuple<string, string, string, string, string, string>>();
                    NecrowVirtualization.PEPhysicalInterfaces.Add(new Tuple<string, List<Tuple<string, string, string, string, string, string>>>(nodeName, vPEPhysicalInterfaces));
                    NecrowVirtualization.PEPhysicalInterfacesSort(true);
                }
            }

            int sinf = 0, sinfup = 0, sinfhu = 0, sinfag = 0, sinfhuup = 0, sinfte = 0, sinfteup = 0, sinfgi = 0, sinfgiup = 0, sinffa = 0, sinffaup = 0, sinfet = 0, sinfetup = 0, sinfse = 0, sinfseup = 0,
                ssubinf = 0, ssubinfup = 0, ssubinfupup = 0, ssubinfag = 0, ssubinfagup = 0, ssubinfagupup = 0, ssubinfhu = 0, ssubinfhuup = 0, ssubinfhuupup = 0, ssubinfte = 0, ssubinfteup = 0, ssubinfteupup = 0, ssubinfgi = 0, ssubinfgiup = 0, ssubinfgiupup = 0, ssubinffa = 0, ssubinffaup = 0, ssubinffaupup = 0, ssubinfet = 0, ssubinfetup = 0, ssubinfetupup = 0;

            foreach (KeyValuePair<string, PEInterfaceToDatabase> pair in interfacelive)
            {
                PEInterfaceToDatabase li = pair.Value;

                if (li.InterfaceType != null)
                {
                    // phyif
                    sinf++;
                    if (li.Status) sinfup++;
                    if (li.InterfaceType == "Hu") { sinfhu++; if (li.Status) sinfhuup++; }
                    if (li.InterfaceType == "Te") { sinfte++; if (li.Status) sinfteup++; }
                    if (li.InterfaceType == "Gi") { sinfgi++; if (li.Status) sinfgiup++; }
                    if (li.InterfaceType == "Fa") { sinffa++; if (li.Status) sinffaup++; }
                    if (li.InterfaceType == "Et") { sinfet++; if (li.Status) sinfetup++; }
                    if (li.InterfaceType == "Ag") { sinfag++; }
                }

                NetworkInterface inf = NetworkInterface.Parse(li.Name);
                string parentPort = null;

                // TOPOLOGY
                if (inf != null)
                {
                    string inftype = inf.Type;

                    if (inf.IsSubInterface)
                    {
                        parentPort = inf.BaseName;
                    }
                    else
                    {
                        if (li.Aggr != -1) parentPort = "Ag" + li.Aggr;
                    }

                    if (parentPort != null)
                    {
                        if (interfacedb.ContainsKey(parentPort)) // cek di existing db
                            li.ParentID = interfacedb[parentPort]["PI_ID"].ToString();
                        else if (interfaceinsert.ContainsKey(parentPort)) // cek di interface yg baru
                            li.ParentID = interfaceinsert[parentPort].ID;
                    }


                    if (!inf.IsSubInterface)
                    {
                        if (inftype == "Ag")
                        {
                            // we need support with child
                            int myaggr;
                            if (int.TryParse(inf.Interface, out myaggr))
                            {
                                // cari child di interfacelive yg aggr-nya myaggr
                                List<PEInterfaceToDatabase> agPhysicals = new List<PEInterfaceToDatabase>();
                                foreach (KeyValuePair<string, PEInterfaceToDatabase> spair in interfacelive)
                                {
                                    if (spair.Value.Aggr == myaggr)
                                        agPhysicals.Add(spair.Value);
                                }
                                li.AggrChilds = agPhysicals.ToArray();

                                // cari neighbor per anak
                                // jika setiap anak memiliki neighbor YANG PARENT IDNYA TIDAK NULL DAN SAMA, maka set ke adjacentParentID
                                string childNeighborParentID = null;
                                foreach (PEInterfaceToDatabase pi in li.AggrChilds)
                                {
                                    FindPhysicalNeighbor(pi); // find topology anaknya dulu

                                    if (pi.AggrNeighborParentID == null)
                                    {
                                        childNeighborParentID = null;
                                        break;
                                    }
                                    else if (childNeighborParentID == null) childNeighborParentID = pi.AggrNeighborParentID;
                                    else if (childNeighborParentID != pi.AggrNeighborParentID)
                                    {
                                        childNeighborParentID = null;
                                        break;
                                    }
                                }

                                // adjacentParentID adalah parentID (aggr) dari lawannya interface aggr ini di ME.
                                if (childNeighborParentID != null)
                                {
                                    li.TopologyMEInterfaceID = childNeighborParentID;

                                    // query lawan
                                    li.ChildrenNeighbor = new Dictionary<int, Tuple<string, string, string>>();
                                    result = Query("select MI_ID, MI_DOT1Q, MI_TO_MI, MI_TO_PI from MEInterface where MI_MI = {0}", li.TopologyMEInterfaceID);
                                    if (!result.OK) return DatabaseFailure(probe);
                                    foreach (Row row in result)
                                    {
                                        if (!row["MI_DOT1Q"].IsNull)
                                        {
                                            int dot1q = row["MI_DOT1Q"].ToIntShort();
                                            if (!li.ChildrenNeighbor.ContainsKey(dot1q)) li.ChildrenNeighbor.Add(dot1q, 
                                                new Tuple<string, string, string>(row["MI_ID"].ToString(), row["MI_TO_MI"].ToString(), row["MI_TO_PI"].ToString()));
                                        }
                                    }
                                }
                            }
                        }
                        else
                            FindPhysicalNeighbor(li);
                    }
                    else if (li.ParentID != null)  // subif dan anak aggregate
                    {
                        if (interfacelive.ContainsKey(parentPort))
                        {
                            PEInterfaceToDatabase parent = interfacelive[parentPort];

                            if (li.Aggr == -1)
                            {
                                // subif
                                ssubinf++;
                                if (li.Status)
                                {
                                    ssubinfup++;
                                    if (li.Protocol) ssubinfupup++;
                                }
                                if (parent.InterfaceType == "Hu") { ssubinfhu++; if (li.Status) { ssubinfhuup++; if (li.Protocol) ssubinfhuupup++; } }
                                if (parent.InterfaceType == "Te") { ssubinfte++; if (li.Status) { ssubinfteup++; if (li.Protocol) ssubinfteupup++; } }
                                if (parent.InterfaceType == "Gi") { ssubinfgi++; if (li.Status) { ssubinfgiup++; if (li.Protocol) ssubinfgiupup++; } }
                                if (parent.InterfaceType == "Fa") { ssubinffa++; if (li.Status) { ssubinffaup++; if (li.Protocol) ssubinffaupup++; } }
                                if (parent.InterfaceType == "Et") { ssubinfet++; if (li.Status) { ssubinfetup++; if (li.Protocol) ssubinfetupup++; } }
                                if (parent.InterfaceType == "Ag") { ssubinfag++; if (li.Status) { ssubinfagup++; if (li.Protocol) ssubinfagupup++; } }
                            }

                            int dot1q = li.Dot1Q;
                            if (dot1q > -1)
                            {
                                if (parent.ChildrenNeighbor != null)
                                {
                                    if (parent.ChildrenNeighbor.ContainsKey(dot1q))
                                    {
                                        Tuple<string, string, string> parentChildrenNeighbor = parent.ChildrenNeighbor[dot1q];

                                        li.TopologyMEInterfaceID = parentChildrenNeighbor.Item1;
                                        li.NeighborCheckMITOPI = parentChildrenNeighbor.Item3;
                                    }
                                }
                            }
                        }
                    }
                }
                
                if (!interfacedb.ContainsKey(pair.Key))
                {
                    Event("Interface ADD: " + pair.Key);

                    li.ID = Database.ID();
                    interfaceinsert.Add(li.Name, li);

                    // IP
                    if (li.IP != null)
                    {
                        foreach (string ip in li.IP.ToArray())
                        {
                            string[] ipx = ip.Split(StringSplitTypes.Underscore);
                            Event("+ " + (ipx[0] == "0" ? "IPv4" : "IPv6") + " " + (ipx[1] == "1" ? "" : "secondary ") + ipx[2]);
                        }
                    }

                    // Service
                    if (li.Description != null) interfaceServiceReference.Add(li, li.Description);
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
                        UpdateInfo(updateinfo, "parent", db["PI_PI"].ToString(), li.ParentID, true);
                    }
                    if (db["PI_TO_MI"].ToString() != li.TopologyMEInterfaceID)
                    {
                        update = true;
                        u.UpdateTopologyMEInterfaceID = true;
                        u.TopologyMEInterfaceID = li.TopologyMEInterfaceID;
                        UpdateInfo(updateinfo, "pi-to-mi", db["PI_TO_MI"].ToString(), li.TopologyMEInterfaceID, true);
                    }
                    else if (li.TopologyMEInterfaceID != null && li.NeighborCheckMITOPI != u.ID)
                    {
                        update = true;
                        u.UpdateNeighborCheckMITOPI = true;
                        u.TopologyMEInterfaceID = li.TopologyMEInterfaceID;
                        UpdateInfo(updateinfo, "neighbor-mi-to-pi", li.NeighborCheckMITOPI, u.ID, true);
                    }
                    if (db["PI_TO_NI"].ToString() != li.TopologyNeighborInterfaceID)
                    {
                        update = true;
                        u.UpdateTopologyNeighborInterfaceID = true;
                        u.TopologyNeighborInterfaceID = li.TopologyNeighborInterfaceID;
                        UpdateInfo(updateinfo, "pi-to-ni", db["PI_TO_NI"].ToString(), li.TopologyNeighborInterfaceID, true);
                    }
                    if (db["PI_Description"].ToString() != li.Description)
                    {
                        update = true;
                        u.UpdateDescription = true;
                        u.Description = li.Description;
                        UpdateInfo(updateinfo, "description", db["PI_Description"].ToString(), li.Description, true);

                        u.ServiceID = null;
                        if (u.Description != null) interfaceServiceReference.Add(u, u.Description);
                    }
                    if (db["PI_Status"].ToBool() != li.Status)
                    {
                        update = true;
                        u.UpdateStatus = true;
                        u.Status = li.Status;
                        UpdateInfo(updateinfo, "status", db["PI_Status"].ToBool().DescribeUpDown(), li.Status.DescribeUpDown());
                    }
                    if (db["PI_Protocol"].ToBool() != li.Protocol)
                    {
                        update = true;
                        u.UpdateProtocol = true;
                        u.Protocol = li.Protocol;
                        UpdateInfo(updateinfo, "protocol", db["PI_Protocol"].ToBool().DescribeUpDown(), li.Protocol.DescribeUpDown());
                    }
                    if (db["PI_Enable"].ToBool() != li.Enable)
                    {
                        update = true;
                        u.UpdateEnable = true;
                        u.Enable = li.Enable;
                        UpdateInfo(updateinfo, "enable", db["PI_Enable"].ToBool().DescribeTrueFalse(), li.Enable.DescribeTrueFalse());
                    }
                    if (db["PI_Type"].ToString() != li.InterfaceType)
                    {
                        update = true;
                        u.UpdateInterfaceType = true;
                        u.InterfaceType = li.InterfaceType;
                        UpdateInfo(updateinfo, "type", db["PI_Type"].ToString(), li.InterfaceType);
                    }
                    if (db["PI_DOT1Q"].ToIntShort(-1) != li.Dot1Q)
                    {
                        update = true;
                        u.UpdateDot1Q = true;
                        u.Dot1Q = li.Dot1Q;
                        UpdateInfo(updateinfo, "dot1q", db["PI_DOT1Q"].ToIntShort(-1).NullableInfo(), li.Dot1Q.NullableInfo());
                    }
                    if (db["PI_Aggregator"].ToIntShort(-1) != li.Aggr)
                    {
                        update = true;
                        u.UpdateAggr = true;
                        u.Aggr = li.Aggr;
                        UpdateInfo(updateinfo, "aggr", db["PI_Aggregator"].ToIntShort(-1).NullableInfo(), li.Aggr.NullableInfo());
                    }
                    if (db["PI_PN"].ToString() != li.RouteID)
                    {
                        update = true;
                        u.UpdateRouteID = true;
                        u.RouteID = li.RouteID;
                        UpdateInfo(updateinfo, "routename", db["PI_PN"].ToString(), li.RouteID, true);
                    }
                    if (db["PI_PQ_Input"].ToString() != li.InputQOSID)
                    {
                        update = true;
                        u.UpdateInputQOSID = true;
                        u.InputQOSID = li.InputQOSID;
                        UpdateInfo(updateinfo, "qos-input", db["PI_PQ_Input"].ToString(), li.InputQOSID, true);
                    }
                    if (db["PI_PQ_Output"].ToString() != li.OutputQOSID)
                    {
                        update = true;
                        u.UpdateOutputQOSID = true;
                        u.OutputQOSID = li.OutputQOSID;
                        UpdateInfo(updateinfo, "qos-output", db["PI_PQ_Output"].ToString(), li.OutputQOSID, true);
                    }
                    if (db["PI_Rate_Input"].ToInt(-1) != li.RateInput)
                    {
                        update = true;
                        u.UpdateRateInput = true;
                        u.RateInput = li.RateInput;
                        UpdateInfo(updateinfo, "rate-input", db["PI_Rate_Input"].ToInt(-1).NullableInfo(), li.RateInput.NullableInfo());
                    }
                    if (db["PI_Rate_Output"].ToInt(-1) != li.RateOutput)
                    {
                        update = true;
                        u.UpdateRateOutput = true;
                        u.RateOutput = li.RateOutput;
                        UpdateInfo(updateinfo, "rate-output", db["PI_Rate_Output"].ToInt(-1).NullableInfo(), li.RateOutput.NullableInfo());
                    }
                    if (db["PI_LastDown"].ToNullabelDateTime() != li.LastDown)
                    {
                        update = true;
                        u.UpdateLastDown = true;
                        u.LastDown = li.LastDown;
                        UpdateInfo(updateinfo, "lastdown", db["PI_LastDown"].ToNullabelDateTime().ToString(), li.LastDown.ToString(), true);
                    }
                    if (db["PI_Summary_CIRConfigTotalInput"].ToInt(-1) != li.CirConfigTotalInput)
                    {
                        update = true;
                        u.UpdateCirConfigTotalInput = true;
                        u.CirConfigTotalInput = li.CirConfigTotalInput;
                        UpdateInfo(updateinfo, "circonf-input", db["PI_Summary_CIRConfigTotalInput"].ToInt(-1).NullableInfo(), li.CirConfigTotalInput.NullableInfo());
                    }
                    if (db["PI_Summary_CIRConfigTotalOutput"].ToInt(-1) != li.CirConfigTotalOutput)
                    {
                        update = true;
                        u.UpdateCirConfigTotalOutput = true;
                        u.CirConfigTotalOutput = li.CirConfigTotalOutput;
                        UpdateInfo(updateinfo, "circonf-output", db["PI_Summary_CIRConfigTotalOutput"].ToInt(-1).NullableInfo(), li.CirConfigTotalOutput.NullableInfo());
                    }
                    if (db["PI_Summary_CIRTotalInput"].ToLong(-1) != li.CirTotalInput)
                    {
                        update = true;
                        u.UpdateCirTotalInput = true;
                        u.CirTotalInput = li.CirTotalInput;
                        UpdateInfo(updateinfo, "cir-input", db["PI_Summary_CIRTotalInput"].ToLong(-1).NullableInfo(), li.CirTotalInput.NullableInfo());
                    }
                    if (db["PI_Summary_CIRTotalOutput"].ToLong(-1) != li.CirTotalOutput)
                    {
                        update = true;
                        u.UpdateCirTotalOutput = true;
                        u.CirTotalOutput = li.CirTotalOutput;
                        UpdateInfo(updateinfo, "cir-output", db["PI_Summary_CIRTotalOutput"].ToLong(-1).NullableInfo(), li.CirTotalOutput.NullableInfo());
                    }
                    if (db["PI_Summary_SubInterfaceCount"].ToIntShort(-1) != li.SubInterfaceCount)
                    {
                        update = true;
                        u.UpdateSubInterfaceCount = true;
                        u.SubInterfaceCount = li.SubInterfaceCount;
                        UpdateInfo(updateinfo, "subif-count", db["PI_Summary_SubInterfaceCount"].ToIntShort(-1).NullableInfo(), li.SubInterfaceCount.NullableInfo());
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
                        UpdateInfo(updateinfo, "ip", null);
                        update = true;
                    }

                    if (update)
                    {                        
                        Event("Interface UPDATE: " + pair.Key + " " + updateinfo.ToString());
                        interfaceupdate.Add(u);
                        
                        if (u.IP != null)
                        {
                            foreach (string ip in u.IP.ToArray())
                            {
                                string[] ipx = ip.Split(StringSplitTypes.Underscore);
                                Event("+ " + (ipx[0] == "0" ? "IPv4" : "IPv6") + " " + (ipx[1] == "1" ? "" : "secondary ") + ipx[2]);
                            }
                        }
                        if (u.DeleteIP != null)
                        {
                            foreach (string ip in u.DeleteIP.ToArray())
                            {
                                string[] ipx = ip.Split(StringSplitTypes.Underscore);
                                Event("- " + (ipx[0] == "0" ? "IPv4" : "IPv6") + " " + (ipx[1] == "1" ? "" : "secondary ") + ipx[2]);
                            }
                        }
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
            ServiceDiscovery(interfaceServiceReference);

            // ADD
            batch.Begin();
            List<Tuple<string, string>> interfaceTopologyMIUpdate = new List<Tuple<string, string>>();
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
                insert.Value("PI_Type", s.InterfaceType);
                insert.Value("PI_DOT1Q", s.Dot1Q.Nullable(-1));
                insert.Value("PI_Aggregator", s.Aggr.Nullable(-1));
                insert.Value("PI_Description", s.Description);
                insert.Value("PI_PN", s.RouteID);
                insert.Value("PI_PQ_Input", s.InputQOSID);
                insert.Value("PI_PQ_Output", s.OutputQOSID);
                insert.Value("PI_Rate_Input", s.RateInput.Nullable(-1));
                insert.Value("PI_Rate_Output", s.RateOutput.Nullable(-1));
                insert.Value("PI_SE", s.ServiceID);
                insert.Value("PI_PI", s.ParentID);
                insert.Value("PI_TO_MI", s.TopologyMEInterfaceID);
                insert.Value("PI_TO_NI", s.TopologyNeighborInterfaceID);
                insert.Value("PI_LastDown", s.LastDown);
                insert.Value("PI_Summary_CIRConfigTotalInput", s.CirConfigTotalInput.Nullable(-1));
                insert.Value("PI_Summary_CIRConfigTotalOutput", s.CirConfigTotalOutput.Nullable(-1));
                insert.Value("PI_Summary_CIRTotalInput", s.CirTotalInput.Nullable(-1));
                insert.Value("PI_Summary_CIRTotalOutput", s.CirTotalOutput.Nullable(-1));
                insert.Value("PI_Summary_SubInterfaceCount", s.SubInterfaceCount.Nullable(-1));
                batch.Execute(insert);

                interfaceTopologyMIUpdate.Add(new Tuple<string, string>(s.TopologyMEInterfaceID, s.ID));
                if (s.IP != null) ipinsert.Add(s.ID, s.IP);
            }
            result = batch.Commit();
            if (!result.OK) return DatabaseFailure(probe);
            Event(result, EventActions.Add, EventElements.Interface, false);

            // UPDATE
            batch.Begin();
            foreach (PEInterfaceToDatabase s in interfaceupdate)
            {
                Update update = Update("PEInterface");
                update.Set("PI_PI", s.ParentID, s.UpdateParentID);
                if (s.UpdateTopologyMEInterfaceID)
                {
                    update.Set("PI_TO_MI", s.TopologyMEInterfaceID);
                    interfaceTopologyMIUpdate.Add(new Tuple<string, string>(s.TopologyMEInterfaceID, s.ID));
                }
                else if (s.UpdateNeighborCheckMITOPI)
                {
                    interfaceTopologyMIUpdate.Add(new Tuple<string, string>(s.TopologyMEInterfaceID, s.ID));
                }
                update.Set("PI_TO_NI", s.TopologyNeighborInterfaceID, s.UpdateTopologyNeighborInterfaceID);
                if (s.UpdateDescription)
                {
                    update.Set("PI_Description", s.Description);
                    update.Set("PI_SE", s.ServiceID);
                }
                update.Set("PI_Status", s.Status, s.UpdateStatus);
                update.Set("PI_Protocol", s.Protocol, s.UpdateProtocol);
                update.Set("PI_Enable", s.Enable, s.UpdateEnable);
                update.Set("PI_Type", s.InterfaceType, s.UpdateInterfaceType);
                update.Set("PI_DOT1Q", s.Dot1Q.Nullable(-1), s.UpdateDot1Q);
                update.Set("PI_Aggregator", s.Aggr.Nullable(-1), s.UpdateAggr);
                update.Set("PI_PN", s.RouteID, s.UpdateRouteID);
                update.Set("PI_PQ_Input", s.InputQOSID, s.UpdateInputQOSID);
                update.Set("PI_PQ_Output", s.OutputQOSID, s.UpdateOutputQOSID);
                update.Set("PI_Rate_Input", s.RateInput.Nullable(-1), s.UpdateRateInput);
                update.Set("PI_Rate_Output", s.RateOutput.Nullable(-1), s.UpdateRateOutput);
                update.Set("PI_LastDown", s.LastDown, s.UpdateLastDown);
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
            if (!result.OK) return DatabaseFailure(probe);
            Event(result, EventActions.Update, EventElements.Interface, false);

            batch.Begin();
            foreach (Tuple<string, string> tuple in interfaceTopologyMIUpdate)
            {
                if (tuple.Item1 != null)
                    batch.Execute("update MEInterface set MI_TO_PI = {0} where MI_ID = {1}", tuple.Item2, tuple.Item1);
                else
                    batch.Execute("update MEInterface set MI_TO_PI = NULL where MI_TO_PI = {0}", tuple.Item2);
            }
            result = batch.Commit();
            if (!result.OK) return DatabaseFailure(probe);

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
                    string[] ipx = ip.Split(StringSplitTypes.Underscore);
                    insert.Value("PP_IPv6", ipx[0] == "0" ? false : true);
                    insert.Value("PP_Order", int.Parse(ipx[1]));
                    insert.Value("PP_IP", ipx[2]);
                    batch.Execute(insert);
                }
            }
            result = batch.Commit();
            if (!result.OK) return DatabaseFailure(probe);
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
            if (!result.OK) return DatabaseFailure(probe);
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
            result = batch.Commit();
            if (!result.OK) return DatabaseFailure(probe);
            Event(result, EventActions.Delete, EventElements.Interface, false);

            // redone vPEPhysicalInterfaces
            vPEPhysicalInterfaces.Clear();
            foreach (KeyValuePair<string, PEInterfaceToDatabase> pair in interfacelive)
            {
                PEInterfaceToDatabase li = pair.Value;
                vPEPhysicalInterfaces.Add(new Tuple<string, string, string, string, string, string>(li.Name, li.Description, li.ID, li.InterfaceType, li.ParentID, li.TopologyMEInterfaceID));
            }
            NecrowVirtualization.PEPhysicalInterfacesSort(vPEPhysicalInterfaces);

            batch.Begin();
            foreach (string id in interfacedelete)
            {
                batch.Execute("delete from PEInterfaceIP where PP_PI = {0}", id);
            }
            result = batch.Commit();
            if (!result.OK) return DatabaseFailure(probe);
            Event(result, EventActions.Delete, EventElements.InterfaceIP, false);

            batch.Begin();
            foreach (string id in interfacedelete)
            {
                batch.Execute("update POP set OO_PI = NULL where OO_PI = {0}", id);
                batch.Execute("update PERouteUse set PU_PI = NULL, PU_PI_Gone = (select PI_Name from PEInterface where PI_ID = {0}) where PU_PI = {0}", id);
                batch.Execute("delete from PEInterface where PI_ID = {0}", id);
            }
            result = batch.Commit();
            if (!result.OK) return DatabaseFailure(probe);
            Event(result, EventActions.Delete, EventElements.Interface, false);

            // RESERVES
            batch.Begin();
            foreach (KeyValuePair<string, PEInterfaceToDatabase> pair in interfacelive)
            {
                foreach (KeyValuePair<string, Row> pair2 in reserves)
                {
                    string key2 = pair2.Key;
                    if (key2.StartsWith(pair.Value.Name + "-") || key2.EndsWith("-" + pair.Value.ServiceSID))
                    {
                        batch.Execute("delete from Reserve where RE_ID = {0}", pair2.Value["RE_ID"].ToString());
                    }
                }
            }
            result = batch.Commit();
            if (!result.OK) return DatabaseFailure(probe);
            if (result.AffectedRows > 0) Event(result.AffectedRows + " reserved entr" + (result.AffectedRows > 1 ? "ies have" : "y has") + " been found");

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
            if (!result.OK) return DatabaseFailure(probe);
            Event(result, EventActions.Update, EventElements.POPInterfaceReference, false);

            #endregion

            #endregion

            #region ROUTING

            Dictionary<string, PERouteUseToDatabase> routeuselive = new Dictionary<string, PERouteUseToDatabase>();
            Dictionary<string, Row> routeusedb = QueryDictionary("select PERouteUse.* from PERouteUse, PERouteName where PU_PN = PN_ID and PN_NO = {0}", delegate (Row row)
            {
                StringBuilder keysb = new StringBuilder();

                keysb.Append(row["PU_PN"].ToString());
                keysb.Append("_");

                string type = row["PU_Type"].ToString();
                keysb.Append(type);
                keysb.Append("_");

                if (type == "S")
                {
                    keysb.Append(row["PU_Network"].ToString());
                    keysb.Append("_");
                    keysb.Append(row["PU_Neighbor"].ToString(""));
                    keysb.Append("_");
                    string pi = row["PU_PI"].ToString();
                    string piGone = row["PU_PI_Gone"].ToString();
                    keysb.Append(pi != null ? pi : piGone != null ? piGone : "");
                }
                else if (type == "B")
                {
                    keysb.Append(row["PU_Neighbor"].ToString());
                    keysb.Append("_");
                    int BGPAS = row["PU_B_AS"].ToInt(-1);
                    keysb.Append("" + (BGPAS != -1 ? BGPAS + "" : ""));
                }
                else if (type == "O")
                {
                    int process = row["PU_O_Process"].ToInt(-1);
                    keysb.Append(process != -1 ? process + "_" : "_");
                    int area = row["PU_O_Area"].ToInt(-1);
                    keysb.Append(area != -1 ? area + "_" : "_");
                    string pi = row["PU_PI"].ToString();
                    string piGone = row["PU_PI_Gone"].ToString();
                    keysb.Append(pi != null ? pi : piGone != null ? piGone : "");
                    keysb.Append("_");
                    keysb.Append(row["PU_Network"].ToString());
                    keysb.Append("_");
                    keysb.Append(row["PU_O_Wildcard"].ToString());
                }
                else if (type == "R")
                {
                    string pi = row["PU_PI"].ToString();
                    string piGone = row["PU_PI_Gone"].ToString();
                    keysb.Append(pi != null ? pi : piGone != null ? piGone : "");
                    keysb.Append("_");
                    keysb.Append(row["PU_Network"].ToString());
                }
                else if (type == "E")
                {
                    string pi = row["PU_PI"].ToString();
                    string piGone = row["PU_PI_Gone"].ToString();
                    keysb.Append(pi != null ? pi : piGone != null ? piGone : "");
                }

                return keysb.ToString();

            }, nodeID);
            if (routeusedb == null) return DatabaseFailure(probe);
            List<PERouteUseToDatabase> routeuseinsert = new List<PERouteUseToDatabase>();
            List<PERouteUseToDatabase> routeuseupdate = new List<PERouteUseToDatabase>();

            Dictionary<string, Tuple<PEPrefixListToDatabase, List<PEPrefixEntryToDatabase>>> prefixlistlive = new Dictionary<string, Tuple<PEPrefixListToDatabase, List<PEPrefixEntryToDatabase>>>();
            Dictionary<string, Row> prefixlistdb = QueryDictionary("select * from PEPrefixList where PX_NO = {0}", "PX_Name", nodeID);
            if (prefixlistdb == null) return DatabaseFailure(probe);
            Dictionary<string, Row> prefixentrydb = QueryDictionary("select PX_Name, PEPrefixEntry.* from PEPrefixEntry, PEPrefixList where PY_PX = PX_ID and PX_NO = {0}", delegate (Row row)
            {
                return row["PX_Name"].ToString() + "_" + row["PY_Network"].ToString() + "_" + row["PY_Sequence"].ToIntShort(-1) + "_" + row["PY_Access"].ToString() + "_" + row["PY_Ge"].ToIntShort(-1) + "_" + row["PY_Le"].ToIntShort(-1);
            }, nodeID);
            if (prefixentrydb == null) return DatabaseFailure(probe);
            List<PEPrefixListToDatabase> prefixlistinsert = new List<PEPrefixListToDatabase>();
            List<PEPrefixEntryToDatabase> prefixentryinsert = new List<PEPrefixEntryToDatabase>();
            List<PEPrefixEntryToDatabase> prefixentryupdate = new List<PEPrefixEntryToDatabase>();
            List<string> prefixentrydelete = new List<string>();

            Event("Checking Routing");

            #region Live

            if (nodeManufacture == cso)
            {
                #region cso    

                if (nodeVersion == xr)
                {
                    #region xr

                    string currentRouteNameID = null;
                    string currentNeighbor = null;
                    string currentRemoteAS = null;
                    string currentProcess = null;
                    string currentArea = null;
                    string currentInterface = null;

                    #region STATIC
                    if (Request("sh run router static", out lines, probe)) return probe;

                    foreach (string line in lines)
                    {
                        string linetrim = line.Trim();
                        if (linetrim.Length > 0)
                        {
                            if (linetrim.StartsWith("vrf "))
                            {
                                string vrfname = linetrim.Substring(4);

                                if (routenamedb.ContainsKey(vrfname) && routenamelive.ContainsKey(vrfname))
                                    currentRouteNameID = routenamedb[vrfname]["PN_ID"].ToString();
                                else
                                    currentRouteNameID = null;
                            }
                            else if (currentRouteNameID != null)
                            {
                                if (char.IsDigit(linetrim[0]))
                                {
                                    string[] parts = linetrim.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);

                                    string network = null, neighbor = null, interfaceID = null;
                                    string ifname = null;
                                    if (parts.Length == 2)
                                    {
                                        network = parts[0];

                                        if (char.IsDigit(parts[1][0]))
                                            neighbor = parts[1];
                                        else
                                        {
                                            // probably interface
                                            NetworkInterface nif = NetworkInterface.Parse(parts[1]);
                                            if (nif != null)
                                            {
                                                ifname = nif.Name;
                                                if (interfacelive.ContainsKey(ifname))
                                                {
                                                    interfaceID = interfacelive[ifname].ID;
                                                }
                                            }

                                            neighbor = "INTERFACE";
                                        }
                                    }
                                    else if (parts.Length == 3)
                                    {
                                        network = parts[0];
                                        NetworkInterface nif = NetworkInterface.Parse(parts[1]);
                                        if (nif != null)
                                        {
                                            ifname = nif.Name;
                                            if (interfacelive.ContainsKey(ifname))
                                            {
                                                interfaceID = interfacelive[ifname].ID;
                                            }
                                        }
                                        neighbor = parts[2];
                                    }

                                    if (network != null)
                                    {
                                        PERouteUseToDatabase i = new PERouteUseToDatabase();
                                        i.RouteNameID = currentRouteNameID;
                                        i.Type = "S";
                                        i.Network = network;
                                        i.Neighbor = neighbor;
                                        i.InterfaceID = interfaceID;

                                        if (interfaceID == null && ifname != null)
                                        {
                                            i.InterfaceGone = ifname;
                                        }

                                        string key = currentRouteNameID + "_S_" + network + "_" + (neighbor != null ? neighbor : "") + "_" + (interfaceID != null ? interfaceID : ifname != null ? ifname : "");
                                        routeuselive.Add(key, i);
                                    }
                                }
                            }
                        }
                    }

                    #endregion

                    #region PREFIX-SET and ROUTE POLICY LANGUAGE

                    if (Request(@"show rpl prefix-set", out lines, probe)) return probe;
                    //Wed Sep 21 15:23:41.177 GMT
                    //Listing for all Prefix Set objects

                    //prefix-set OSS_JT2
                    //012345678901
                    //  61.94.111.0/24 ge 32 le 32
                    //      0           1  2 3  4
                    //end-set
                    //!
                    //prefix-set TO-GLOBAL
                    //  0.0.0.0/0
                    //end-set
                    //!
                    //prefix-set FROM-ASTINET-AJB
                    //  202.134.6.96/28
                    //end-set

                    string currentPrefixList = null;

                    foreach (string line in lines)
                    {
                        string linetrim = line.Trim();
                        if (linetrim.Length > 0)
                        {
                            if (linetrim.StartsWith("prefix-set"))
                            {
                                string name = linetrim.Substring(11);
                                if (!prefixlistlive.ContainsKey(name))
                                {
                                    PEPrefixListToDatabase pl = new PEPrefixListToDatabase();
                                    pl.Name = name;
                                    prefixlistlive.Add(name, new Tuple<PEPrefixListToDatabase, List<PEPrefixEntryToDatabase>>(pl, new List<PEPrefixEntryToDatabase>()));
                                }
                                currentPrefixList = name;
                            }
                            else if (linetrim.StartsWith("end-set"))
                            {
                                currentPrefixList = null;
                            }
                            else if (currentPrefixList != null)
                            {
                                string[] secs = linetrim.Split(new char[] { ',', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                string network = secs[0];
                                int ge = -1;
                                int le = -1;
                                if (secs.Length > 2)
                                {
                                    if (secs[1] == "ge")
                                    {
                                        int.TryParse(secs[2], out ge);
                                        if (secs.Length > 4 && secs[3] == "le") int.TryParse(secs[4], out le);
                                    }
                                    else if (secs[1] == "le")
                                    {
                                        int.TryParse(secs[2], out le);
                                        if (secs.Length > 4 && secs[3] == "ge") int.TryParse(secs[4], out ge); // probably no, since ge always before le, lol
                                    }
                                }

                                PEPrefixEntryToDatabase pe = new PEPrefixEntryToDatabase();
                                pe.Sequence = -1;
                                pe.Network = network;
                                pe.GE = ge;
                                pe.LE = le;

                                prefixlistlive[currentPrefixList].Item2.Add(pe);
                            }
                        }
                    }

                    //RP/0/RSP1/CPU0:PE2-D2-JT2-VPN#show rpl route-policy
                    //Wed Sep 21 19:34:30.619 GMT
                    //Listing for all Route Policy objects

                    //route-policy AIA
                    //01234567890123
                    //  if destination in AIA then
                    //    drop
                    //  else
                    //    pass
                    //  endif
                    //end-policy
                    //!
                    Dictionary<string, string> rpl = new Dictionary<string, string>();

                    if (Request(@"show rpl route-policy", out lines, probe)) return probe;

                    StringBuilder caps = new StringBuilder();
                    string currentRPL = null;

                    foreach (string line in lines)
                    {
                        if (currentRPL != null)
                        {
                            if (line.StartsWith("end-policy"))
                            {
                                rpl.Add(currentRPL, caps.ToString());
                                currentRPL = null;
                            }
                            else
                            {
                                caps.AppendLine(line);
                            }
                        }
                        else if (line.StartsWith("route-policy "))
                        {
                            currentRPL = line.Substring(13);
                            caps.Clear();
                        }
                    }

                    #endregion

                    #region BGP
                    if (Request("sh run router bgp", out lines, probe)) return probe;

                    currentRouteNameID = null;
                    currentNeighbor = null;
                    currentRemoteAS = null;

                    int currentBGPAS = -1;
                    string currentUseNeighborGroup = null;
                    string currentUseRPLIN = null;
                    string currentUseRPLOUT = null;

                    string currentNeighborGroup = null;
                    string currentMaximumPrefix = null;
                    string currentMaximumPrefixThres = null;
                    string currentMaximumPrefixWO = null;
                    string currentRPLIN = null;
                    string currentRPLOUT = null;

                    Dictionary<string, NeighborGroup> neighborGroups = new Dictionary<string, NeighborGroup>();

                    foreach (string line in lines)
                    {
                        string linetrim = line.Trim();
                        if (linetrim.Length > 0)
                        {
                            //neighbor-group AFIS
                            //01234567890123456789
                            if (linetrim.StartsWith("router bgp "))
                            {
                                //router bgp 17974
                                //012345678901
                                if (!int.TryParse(linetrim.Substring(11), out currentBGPAS)) currentBGPAS = 1;
                            }
                            if (linetrim.StartsWith("neighbor-group "))
                            {
                                currentNeighborGroup = linetrim.Substring(15);
                            }
                            else if (currentNeighborGroup != null)
                            {
                                if (linetrim.StartsWith("maximum-prefix "))
                                {
                                    string[] splits = linetrim.Split(StringSplitTypes.Space);
                                    //maximum-prefix 10 90 warning-only
                                    // 0              1  2   3
                                    currentMaximumPrefix = splits[1];
                                    if (splits.Length > 2)
                                    {
                                        if (splits[2] == "warning-only") currentMaximumPrefixWO = "warning-only";
                                        else currentMaximumPrefixThres = splits[2];
                                    }
                                    if (splits.Length > 3 && currentMaximumPrefixWO == null)
                                    {
                                        if (splits[3] == "warning-only") currentMaximumPrefixWO = "warning-only";
                                    }
                                }
                                if (linetrim.StartsWith("route-policy "))
                                {
                                    //route-policy TO-DOMESTIK out
                                    string[] rps = linetrim.Split(StringSplitTypes.Space);
                                    if (rpl.ContainsKey(rps[1]))
                                    {
                                        string dir = rps[2];
                                        if (dir == "in") currentRPLIN = rpl[rps[1]];
                                        else if (dir == "out") currentRPLOUT = rpl[rps[1]];
                                    }
                                }
                                if (linetrim == "!")
                                {
                                    NeighborGroup g = new NeighborGroup();

                                    // maximum prefix
                                    if (currentMaximumPrefix != null)
                                    {
                                        int mp;
                                        if (int.TryParse(currentMaximumPrefix, out mp)) g.MaximumPrefix = mp;
                                        if (currentMaximumPrefixThres != null && int.TryParse(currentMaximumPrefixThres, out mp)) g.MaximumPrefixThreshold = mp;
                                        if (currentMaximumPrefixWO == "warning-only") g.MaximumPrefixWarningOnly = true;
                                    }
                                    // rpl
                                    if (currentRPLIN != null) g.RoutePolicyIn = currentRPLIN;
                                    if (currentRPLOUT != null) g.RoutePolicyOut = currentRPLOUT;

                                    neighborGroups.Add(currentNeighborGroup, g);

                                    currentNeighborGroup = null;
                                    currentMaximumPrefix = null;
                                    currentMaximumPrefixThres = null;
                                    currentMaximumPrefixWO = null;
                                    currentRPLIN = null;
                                    currentRPLOUT = null;
                                }
                            }
                            else if (linetrim.StartsWith("vrf "))
                            {
                                string vrfname = linetrim.Substring(4);
                                if (routenamedb.ContainsKey(vrfname) && routenamelive.ContainsKey(vrfname))
                                    currentRouteNameID = routenamedb[vrfname]["PN_ID"].ToString();
                                else
                                    currentRouteNameID = null;

                                currentNeighbor = null;
                            }
                            else if (currentRouteNameID != null)
                            {
                                if (linetrim.StartsWith("neighbor "))
                                {
                                    currentNeighbor = linetrim.Substring(9);
                                    currentRemoteAS = null;
                                    currentUseNeighborGroup = null;
                                    currentUseRPLIN = null;
                                    currentUseRPLOUT = null;
                                }
                                if (linetrim.StartsWith("remote-as ")) currentRemoteAS = linetrim.Substring(10);
                                if (linetrim.StartsWith("use neighbor-group ")) currentUseNeighborGroup = linetrim.Substring(19);
                                if (linetrim.StartsWith("route-policy "))
                                {
                                    //route-policy TO-DOMESTIK out
                                    string[] rps = linetrim.Split(StringSplitTypes.Space);
                                    if (rpl.ContainsKey(rps[1]))
                                    {
                                        string dir = rps[2];
                                        if (dir == "in") currentUseRPLIN = rpl[rps[1]];
                                        else if (dir == "out") currentUseRPLOUT = rpl[rps[1]];
                                    }
                                }
                                if (linetrim == "!")
                                {
                                    if (currentNeighbor != null)
                                    {
                                        PERouteUseToDatabase i = new PERouteUseToDatabase();
                                        i.RouteNameID = currentRouteNameID;
                                        i.Type = "B";
                                        i.Neighbor = currentNeighbor;
                                        i.BGPAS = currentBGPAS;
                                        i.InterfaceID = FindInterfaceByNeighbor(currentNeighbor, currentRouteNameID, interfacelive);

                                        if (currentUseNeighborGroup != null && neighborGroups.ContainsKey(currentUseNeighborGroup))
                                        {
                                            NeighborGroup g = neighborGroups[currentUseNeighborGroup];

                                            i.MaximumPrefix = g.MaximumPrefix;
                                            i.MaximumPrefixThreshold = g.MaximumPrefixThreshold;
                                            i.MaximumPrefixWarningOnly = g.MaximumPrefixWarningOnly;
                                            i.RoutePolicyIn = g.RoutePolicyIn;
                                            i.RoutePolicyOut = g.RoutePolicyOut;
                                        }

                                        if (currentRemoteAS != null)
                                        {
                                            int ras = -1;
                                            if (int.TryParse(currentRemoteAS, out ras)) i.RemoteAS = ras;
                                        }

                                        if (currentUseRPLIN != null) i.RoutePolicyIn = currentUseRPLIN;
                                        if (currentUseRPLOUT != null) i.RoutePolicyOut = currentUseRPLOUT;

                                        string key = currentRouteNameID + "_B_" + currentNeighbor + "_" + (currentBGPAS != -1 ? currentBGPAS + "" : "");
                                        routeuselive.Add(key, i);
                                    }

                                    currentNeighbor = null;
                                }
                            }
                        }
                    }

                    #endregion

                    #region OSPF
                    if (Request("sh run router ospf", out lines, probe)) return probe;

                    currentRouteNameID = null;
                    currentProcess = null;
                    currentArea = null;
                    currentInterface = null;
                    currentNeighbor = null;

                    string currentMessageDigestKey = null;
                    string currentOspfInterfaceNetwork = null;

                    foreach (string line in lines)
                    {
                        string linetrim = line.Trim();
                        if (linetrim.Length > 0)
                        {
                            if (linetrim.StartsWith("router ospf "))
                            {
                                currentProcess = linetrim.Substring(12);

                                currentRouteNameID = null;
                                currentArea = null;
                            }
                            else if (currentProcess != null)
                            {
                                if (linetrim.StartsWith("vrf "))
                                {
                                    string vrfname = linetrim.Substring(4);
                                    if (routenamedb.ContainsKey(vrfname) && routenamelive.ContainsKey(vrfname))
                                        currentRouteNameID = routenamedb[vrfname]["PN_ID"].ToString();
                                    else
                                        currentRouteNameID = null;

                                    currentArea = null;
                                }
                                else if (currentRouteNameID != null)
                                {
                                    if (linetrim.StartsWith("area "))
                                    {
                                        currentArea = linetrim.Substring(5);
                                    }
                                    else if (currentArea != null)
                                    {
                                        if (linetrim.StartsWith("interface "))
                                        {
                                            NetworkInterface nif = NetworkInterface.Parse(linetrim.Substring(10));
                                            if (nif != null) currentInterface = nif.Name;

                                            currentNeighbor = null;
                                            currentMessageDigestKey = null;
                                            currentOspfInterfaceNetwork = null;
                                        }
                                        else if (currentInterface != null)
                                        {
                                            if (linetrim.StartsWith("neighbor ")) currentNeighbor = linetrim.Substring(9);
                                            else if (linetrim.StartsWith("message-digest-key ")) currentMessageDigestKey = linetrim.Substring(19);
                                            else if (linetrim.StartsWith("network ")) currentOspfInterfaceNetwork = linetrim.Substring(8);
                                            else if (linetrim == "!")
                                            {
                                                PERouteUseToDatabase i = new PERouteUseToDatabase();
                                                i.RouteNameID = currentRouteNameID;
                                                i.Type = "O";
                                                i.Neighbor = currentNeighbor;

                                                int oprocess, oarea;
                                                if (int.TryParse(currentProcess, out oprocess)) i.Process = oprocess;
                                                if (int.TryParse(currentArea, out oarea)) i.Area = oarea;

                                                string interfaceID = null;
                                                if (interfacelive.ContainsKey(currentInterface))
                                                {
                                                    interfaceID = interfacelive[currentInterface].ID;
                                                    if (interfaceID != null)
                                                    {
                                                        i.InterfaceID = interfaceID;
                                                    }
                                                }
                                                else if (currentInterface != null)
                                                    i.InterfaceGone = currentInterface;

                                                if (currentMessageDigestKey != null) i.MessageDigestKey = currentMessageDigestKey;
                                                if (currentOspfInterfaceNetwork != null)
                                                {
                                                    switch (currentOspfInterfaceNetwork)
                                                    {
                                                        case "broadcast": i.InterfaceNetwork = "B"; break;
                                                        case "non-broadcast": i.InterfaceNetwork = "N"; break;
                                                        case "point-to-multipoint": i.InterfaceNetwork = "M"; break;
                                                        case "point-to-point": i.InterfaceNetwork = "P"; break;
                                                    }
                                                }

                                                string key = currentRouteNameID + "_O_" + currentProcess + "_" + currentArea + "_" + (interfaceID != null ? interfaceID : currentInterface != null ? currentInterface : "") + "__";
                                                routeuselive.Add(key, i);

                                                currentInterface = null;
                                                currentNeighbor = null;
                                                currentMessageDigestKey = null;
                                                currentOspfInterfaceNetwork = null;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    #endregion

                    #region RIP
                    if (Request("sh run router rip", out lines, probe)) return probe;

                    currentRouteNameID = null;
                    currentInterface = null;

                    foreach (string line in lines)
                    {
                        string linetrim = line.Trim();
                        if (linetrim.Length > 0)
                        {
                            if (linetrim.StartsWith("vrf "))
                            {
                                string vrfname = linetrim.Substring(4);
                                if (routenamedb.ContainsKey(vrfname) && routenamelive.ContainsKey(vrfname))
                                    currentRouteNameID = routenamedb[vrfname]["PN_ID"].ToString();
                                else
                                    currentRouteNameID = null;

                                currentInterface = null;
                            }
                            else if (currentRouteNameID != null)
                            {
                                if (linetrim.StartsWith("interface "))
                                {
                                    NetworkInterface nif = NetworkInterface.Parse(linetrim.Substring(10));
                                    if (nif != null) currentInterface = nif.Name;
                                }
                                else if (currentInterface != null)
                                {
                                    if (linetrim == "!")
                                    {
                                        PERouteUseToDatabase i = new PERouteUseToDatabase();
                                        i.RouteNameID = currentRouteNameID;
                                        i.Type = "R";

                                        string interfaceID = null;
                                        if (interfacelive.ContainsKey(currentInterface))
                                        {
                                            interfaceID = interfacelive[currentInterface].ID;
                                            if (interfaceID != null)
                                            {
                                                i.InterfaceID = interfaceID;
                                            }
                                        }
                                        else if (currentInterface != null)
                                            i.InterfaceGone = currentInterface;

                                        string key = currentRouteNameID + "_R_" + (interfaceID != null ? interfaceID : currentInterface != null ? currentInterface : "") + "_";
                                        routeuselive.Add(key, i);

                                        currentInterface = null;
                                    }
                                }
                            }
                        }
                    }


                    #endregion

                    #region EIGRP
                    if (Request("sh run router eigrp", out lines, probe)) return probe;

                    currentRouteNameID = null;
                    currentInterface = null;

                    foreach (string line in lines)
                    {
                        string linetrim = line.Trim();
                        if (linetrim.Length > 0)
                        {
                            if (linetrim.StartsWith("vrf "))
                            {
                                string vrfname = linetrim.Substring(4);
                                if (routenamedb.ContainsKey(vrfname) && routenamelive.ContainsKey(vrfname))
                                    currentRouteNameID = routenamedb[vrfname]["PN_ID"].ToString();
                                else
                                    currentRouteNameID = null;

                                currentInterface = null;
                            }
                            else if (currentRouteNameID != null)
                            {
                                if (linetrim.StartsWith("interface "))
                                {
                                    NetworkInterface nif = NetworkInterface.Parse(linetrim.Substring(10));
                                    if (nif != null) currentInterface = nif.Name;
                                }
                                else if (currentInterface != null)
                                {
                                    if (linetrim == "!")
                                    {
                                        PERouteUseToDatabase i = new PERouteUseToDatabase();
                                        i.RouteNameID = currentRouteNameID;
                                        i.Type = "E";

                                        string interfaceID = null;
                                        if (interfacelive.ContainsKey(currentInterface))
                                        {
                                            interfaceID = interfacelive[currentInterface].ID;
                                            if (interfaceID != null)
                                            {
                                                i.InterfaceID = interfaceID;
                                            }
                                        }
                                        else if (currentInterface != null)
                                            i.InterfaceGone = currentInterface;

                                        string key = currentRouteNameID + "_E_" + (interfaceID != null ? interfaceID : currentInterface != null ? currentInterface : "");
                                        routeuselive.Add(key, i);

                                        currentInterface = null;
                                    }
                                }
                            }
                        }
                    }

                    #endregion

                    #endregion
                }
                else
                {
                    #region ios

                    #region STATIC

                    //ip route vrf Astinet 203.130.235.128 255.255.255.248 GigabitEthernet1/19.2185 192.168.3.218
                    //ip route vrf Astinet 203.130.235.128 255.255.255.248 192.168.3.218
                    //ip route vrf Astinet 203.130.235.128 255.255.255.248 GigabitEthernet1/19.2185
                    //01234567890123456
                    if (Request("sh run | in ip route vrf", out lines, probe)) return probe;

                    foreach (string line in lines)
                    {
                        string linetrim = line.Trim();
                        if (linetrim.Length > 0)
                        {
                            if (linetrim.StartsWith("ip route vrf"))
                            {
                                string lineleft = linetrim.Substring(13);
                                string[] linex = lineleft.Split(StringSplitTypes.Space);

                                string vrf = linex[0];
                                if (routenamedb.ContainsKey(vrf) && routenamelive.ContainsKey(vrf))
                                {
                                    string routeNameID = routenamedb[vrf]["PN_ID"].ToString();
                                    string network = linex[1];
                                    string netmask = linex[2];
                                    string ifname = null;
                                    string interfaceID = null;
                                    string neighbor = null;

                                    string thirdarg = linex[3];
                                    string fortharg = null;
                                    if (linex.Length > 4) fortharg = linex[4];

                                    int cidr = IPNetwork.ToCidr(IPAddress.Parse(netmask));
                                    network = network + "/" + cidr;

                                    if (char.IsLetter(thirdarg[0]))
                                    {
                                        // probably interface
                                        NetworkInterface nif = NetworkInterface.Parse(thirdarg);
                                        if (nif != null)
                                        {
                                            ifname = nif.Name;
                                            if (interfacelive.ContainsKey(ifname)) interfaceID = interfacelive[ifname].ID;
                                            if (fortharg != null) neighbor = fortharg;
                                            else neighbor = "INTERFACE";
                                        }
                                    }
                                    else neighbor = thirdarg;

                                    PERouteUseToDatabase i = new PERouteUseToDatabase();
                                    i.RouteNameID = routeNameID;
                                    i.Type = "S";
                                    i.Network = network;
                                    i.Neighbor = neighbor;
                                    i.InterfaceID = interfaceID;

                                    if (interfaceID == null && ifname != null)
                                    {
                                        i.InterfaceGone = ifname;
                                    }

                                    string key = routeNameID + "_S_" + network + "_" + (neighbor != null ? neighbor : "") + "_" + (interfaceID != null ? interfaceID : ifname != null ? ifname : "");
                                    routeuselive.Add(key, i);
                                }
                            }
                        }
                    }

                    #endregion

                    #region PREFIX-LIST

                    if (Request(@"show ip prefix-list", out lines, probe)) return probe;
                    //PE-D2-JT2-INET#show ip prefix-list
                    //ip prefix-list ADV-DEFAULT: 1 entries
                    //   seq 5 permit 0.0.0.0/0
                    //ip prefix-list ADV-NONE: 1 entries
                    //   seq 5 deny 0.0.0.0/0
                    //ip prefix-list ADV-TO-BRAS: 1 entries
                    //   seq 10 permit 0.0.0.0/0
                    //ip prefix-list AST-BB-TRIHAMAS: 3 entries
                    //0123456789012345678901234567890123456789
                    //   seq 5 permit 180.250.67.56/29
                    //   seq 10 permit 180.250.72.184/29 ge 1 le 32
                    //    0   1    2           3          4  5 6  7
                    //    1   2    3           4          5  6 7  8
                    //   seq 20 deny 0.0.0.0/0 le 32

                    string currentPrefixList = null;

                    foreach (string line in lines)
                    {
                        string linetrim = line.Trim();
                        if (linetrim.Length > 0)
                        {
                            if (linetrim.StartsWith("ip prefix-list"))
                            {
                                string name = linetrim.Substring(15, linetrim.IndexOf(':') - 15);
                                if (!prefixlistlive.ContainsKey(name))
                                {
                                    PEPrefixListToDatabase pl = new PEPrefixListToDatabase();
                                    pl.Name = name;
                                    prefixlistlive.Add(name, new Tuple<PEPrefixListToDatabase, List<PEPrefixEntryToDatabase>>(pl, new List<PEPrefixEntryToDatabase>()));
                                }
                                currentPrefixList = name;
                            }
                            else if (currentPrefixList != null && linetrim.StartsWith("seq"))
                            {
                                string[] secs = linetrim.Split(StringSplitTypes.Space);
                                int seq = -1;
                                if (!int.TryParse(secs[1], out seq)) seq = -1;
                                string access = secs[2];
                                string network = secs[3];
                                int ge = -1;
                                int le = -1;
                                if (secs.Length > 5)
                                {
                                    if (secs[4] == "ge")
                                    {
                                        int.TryParse(secs[5], out ge);
                                        if (secs.Length > 7 && secs[6] == "le") int.TryParse(secs[7], out le);
                                    }
                                    else if (secs[4] == "le")
                                    {
                                        int.TryParse(secs[5], out le);
                                        if (secs.Length > 7 && secs[6] == "ge") int.TryParse(secs[7], out ge); // probably no, since ge always before le, lol
                                    }
                                }

                                PEPrefixEntryToDatabase pe = new PEPrefixEntryToDatabase();
                                pe.Sequence = seq;
                                pe.Access = access == "permit" ? "P" : "D";
                                pe.Network = network;
                                pe.GE = ge;
                                pe.LE = le;

                                prefixlistlive[currentPrefixList].Item2.Add(pe);
                            }
                        }
                    }

                    #endregion

                    #region BGP, RIP, OSPF, EIGRP                    

                    string currentRouter = null;
                    string currentRouteNameID = null;
                    string currentNeighbor = null;
                    string currentRemoteAS = null;
                    string currentProcess = null;
                    string currentPrefixListIN = null;
                    string currentPrefixListOUT = null;
                    string currentMaximumPrefix = null;
                    string currentMaximumPrefixThres = null;
                    string currentMaximumPrefixWO = null;

                    int currentBGPAS = -1;

                    //sh run | in \ address-family|\ \ neighbor|\ exit-address-family
                    if (Request(@"sh run | in router\ bgp|router\ rip|router\ ospf|router\ eigrp|\ address-family\ ipv4\ vrf|\ \ neighbor|\ exit-address-family|\ network\ ", out lines, probe)) return probe;

                    foreach (string line in lines)
                    {
                        string linetrim = line.Trim();
                        if (linetrim.Length > 0)
                        {
                            if (linetrim.StartsWith("router eigrp "))
                            {
                                //router eigrp 777
                                //router bgp 17974
                                //01234567890123
                                currentRouter = "E";
                            }
                            else if (linetrim.StartsWith("router bgp "))
                            {
                                currentRouter = "B";
                                if (!int.TryParse(linetrim.Substring(11), out currentBGPAS)) currentBGPAS = 1;
                            }
                            else if (linetrim.StartsWith("router rip"))
                            {
                                currentRouter = "R";
                            }
                            else if (linetrim.StartsWith("router ospf "))
                            {
                                //router ospf 777 vrf V2399:Ditjen_Imigrasi
                                //0      1    2   3   4
                                currentRouter = "O";
                                string[] routerx = linetrim.Split(StringSplitTypes.Space);

                                if (routerx.Length == 5 && routerx[3] == "vrf")
                                {
                                    currentProcess = routerx[2];
                                    string vrfname = routerx[4];
                                    if (routenamedb.ContainsKey(vrfname) && routenamelive.ContainsKey(vrfname))
                                        currentRouteNameID = routenamedb[vrfname]["PN_ID"].ToString();
                                    else
                                        currentRouteNameID = null;
                                }
                            }
                            else if (linetrim.StartsWith("network "))
                            {
                                string[] networkx = linetrim.Split(StringSplitTypes.Space);
                                if (currentRouter == "E")
                                {
                                    #region EIGRP
                                    //network 10.0.0.0
                                    //0123456789
                                    //currentNetwork = linetrim.Substring(8);
                                    #endregion
                                }
                                else if (currentRouter == "R" && currentRouteNameID != null)
                                {
                                    #region RIP
                                    if (networkx.Length > 1)
                                    {
                                        //network 172.30.0.0
                                        //012345678
                                        string network = networkx[1] + "/0"; // set network to /0

                                        PERouteUseToDatabase i = new PERouteUseToDatabase();
                                        i.RouteNameID = currentRouteNameID;
                                        i.Type = "R";
                                        i.Network = network;

                                        string key = currentRouteNameID + "_R__" + network;
                                        routeuselive.Add(key, i);
                                    }
                                    #endregion
                                }
                                else if (currentRouter == "O" && currentRouteNameID != null)
                                {
                                    #region OSPF
                                    //network 172.20.243.96 0.0.0.3 area 0
                                    //0       1             2       3    4
                                    if (networkx.Length == 5 && networkx[3] == "area")
                                    {
                                        string network = networkx[1] + "/0"; // using /0 because we're using wildcard mask
                                        string wcmask = networkx[2];
                                        string area = networkx[4];

                                        PERouteUseToDatabase i = new PERouteUseToDatabase();
                                        i.RouteNameID = currentRouteNameID;
                                        i.Type = "O";
                                        i.Network = network;
                                        i.Wildcard = wcmask;

                                        int oprocess, oarea;
                                        if (int.TryParse(currentProcess, out oprocess)) i.Process = oprocess;
                                        if (int.TryParse(area, out oarea)) i.Area = oarea;

                                        string key = currentRouteNameID + "_O_" + currentProcess + "_" + area + "__" + network + "_" + wcmask;
                                        routeuselive.Add(key, i);
                                    }
                                    #endregion
                                }
                            }
                            else if (linetrim.StartsWith("neighbor ") && currentRouteNameID != null)
                            {
                                string[] neighborx = linetrim.Split(StringSplitTypes.Space);
                                if (currentRouter == "E")
                                {
                                    #region EIGRP
                                    //neighbor 10.98.1.10 GigabitEthernet2/2/0.2989
                                    NetworkInterface nif = NetworkInterface.Parse(neighborx[2]);
                                    if (nif != null)
                                    {
                                        string dif = nif.Name;
                                        string neighbor = neighborx[1];

                                        PERouteUseToDatabase i = new PERouteUseToDatabase();
                                        i.RouteNameID = currentRouteNameID;
                                        i.Type = "E";
                                        //i.Network = currentNetwork;
                                        //i.Neighbor = neighbor;

                                        string interfaceID = null;
                                        if (interfacelive.ContainsKey(dif))
                                        {
                                            interfaceID = interfacelive[dif].ID;
                                            if (interfaceID != null) i.InterfaceID = interfaceID;
                                        }
                                        else if (dif != null) i.InterfaceGone = dif;

                                        string key = currentRouteNameID + "_E_" + (interfaceID != null ? interfaceID : dif != null ? dif : "");
                                        routeuselive.Add(key, i);
                                    }
                                    #endregion
                                }
                                else if (currentRouter == "B")
                                {
                                    #region BGP
                                    string thisneighbor = neighborx[1];

                                    if (currentNeighbor != null && thisneighbor != currentNeighbor)
                                    {
                                        // save current neighbor
                                        PERouteUseToDatabase i = new PERouteUseToDatabase();
                                        i.RouteNameID = currentRouteNameID;
                                        i.Type = "B";
                                        i.Neighbor = currentNeighbor;
                                        i.BGPAS = currentBGPAS;
                                        i.PrefixListInID = currentPrefixListIN;
                                        i.PrefixListOutID = currentPrefixListOUT;

                                        if (currentMaximumPrefix != null)
                                        {
                                            int mp;
                                            if (int.TryParse(currentMaximumPrefix, out mp))
                                            {
                                                i.MaximumPrefix = mp;
                                                if (currentMaximumPrefixThres != null)
                                                {
                                                    if (int.TryParse(currentMaximumPrefixThres, out mp)) i.MaximumPrefixThreshold = mp;
                                                    if (currentMaximumPrefixWO == "warning-only") i.MaximumPrefixWarningOnly = true;
                                                }
                                            }
                                        }


                                        int ras = -1;
                                        if (int.TryParse(currentRemoteAS, out ras)) i.RemoteAS = ras;

                                        string key = currentRouteNameID + "_B_" + currentNeighbor + "_" + currentBGPAS;
                                        routeuselive.Add(key, i);

                                        currentNeighbor = thisneighbor;
                                        currentRemoteAS = null;
                                        currentPrefixListIN = null;
                                        currentPrefixListOUT = null;
                                        currentMaximumPrefix = null;
                                        currentMaximumPrefixThres = null;
                                        currentMaximumPrefixWO = null;
                                    }
                                    else currentNeighbor = thisneighbor;

                                    if (neighborx[2] == "remote-as") currentRemoteAS = neighborx[3];
                                    else if (neighborx[2] == "prefix-list")
                                    {
                                        if (neighborx[4] == "in") currentPrefixListIN = neighborx[3];
                                        else if (neighborx[4] == "out") currentPrefixListOUT = neighborx[3];
                                    }
                                    else if (neighborx[2] == "maximum-prefix")
                                    {
                                        currentMaximumPrefix = neighborx[3];
                                        if (neighborx.Length > 4)
                                        {
                                            if (neighborx[4] == "warning-only") currentMaximumPrefixWO = "warning-only";
                                            else currentMaximumPrefixThres = neighborx[4];
                                        }
                                        if (neighborx.Length > 5 && currentMaximumPrefixWO == null)
                                        {
                                            if (neighborx[5] == "warning-only") currentMaximumPrefixWO = "warning-only";
                                        }
                                    }
                                    #endregion
                                }
                            }
                            else if (linetrim.StartsWith("address-family ipv4 vrf"))
                            {
                                //address-family ipv4 vrf WIFI-ID
                                //012345678901234567890123456789
                                string vrfname = linetrim.Substring(24);
                                if (routenamedb.ContainsKey(vrfname) && routenamelive.ContainsKey(vrfname))
                                    currentRouteNameID = routenamedb[vrfname]["PN_ID"].ToString();
                                else
                                    currentRouteNameID = null;

                                if (currentRouter == "B")
                                {
                                    currentNeighbor = null;
                                    currentRemoteAS = null;
                                    currentPrefixListIN = null;
                                    currentPrefixListOUT = null;
                                    currentMaximumPrefix = null;
                                    currentMaximumPrefixThres = null;
                                    currentMaximumPrefixWO = null;
                                }
                            }
                            else if (linetrim.StartsWith("exit-address-family") && currentRouteNameID != null)
                            {
                                if (currentRouter == "B")
                                {
                                    #region BGP

                                    if (currentNeighbor != null)
                                    {
                                        PERouteUseToDatabase i = new PERouteUseToDatabase();
                                        i.RouteNameID = currentRouteNameID;
                                        i.Type = "B";
                                        i.Neighbor = currentNeighbor;
                                        i.BGPAS = currentBGPAS;

                                        if (i.Neighbor != null)
                                        {
                                            IPAddress neighborIP = IPAddress.Parse(i.Neighbor);

                                            foreach (KeyValuePair<string, PEInterfaceToDatabase> pair in interfacelive)
                                            {
                                                PEInterfaceToDatabase li = pair.Value;

                                                if (li.RouteID == i.RouteNameID)
                                                {
                                                    if (li.IP != null)
                                                    {
                                                        foreach (string cip in li.IP)
                                                        {
                                                            string[] cips = cip.Split(StringSplitTypes.Underscore);

                                                            if (cips.Length == 3 && cips[0] == "0")
                                                            {
                                                                string ipcidr = cips[2];
                                                                string thip = ipcidr.Split(new char[] { '/' })[0];

                                                                IPNetwork net = IPNetwork.Parse(ipcidr);

                                                                if (neighborIP.IsInSameSubnet(net.FirstUsable, net.Netmask))
                                                                {
                                                                    // satu subnet, bisa jadi neighbor tembakan
                                                                    i.InterfaceID = li.ID;
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }

                                        i.PrefixListInID = currentPrefixListIN;
                                        i.PrefixListOutID = currentPrefixListOUT;

                                        if (currentMaximumPrefix != null)
                                        {
                                            int mp;
                                            if (int.TryParse(currentMaximumPrefix, out mp))
                                            {
                                                i.MaximumPrefix = mp;
                                                if (currentMaximumPrefixThres != null)
                                                {
                                                    if (int.TryParse(currentMaximumPrefixThres, out mp)) i.MaximumPrefixThreshold = mp;
                                                    if (currentMaximumPrefixWO == "warning-only") i.MaximumPrefixWarningOnly = true;
                                                }
                                            }
                                        }

                                        int ras = -1;
                                        if (int.TryParse(currentRemoteAS, out ras)) i.RemoteAS = ras;

                                        string key = currentRouteNameID + "_B_" + currentNeighbor + "_" + currentBGPAS;
                                        routeuselive.Add(key, i);
                                    }

                                    #endregion
                                }

                                currentRouteNameID = null;
                            }
                        }
                    }

                    #endregion

                    #endregion
                }

                #endregion
            }
            else if (nodeManufacture == jun)
            {
                #region jun

                #region PREFIX-LIST POLICY-STATEMENT

                string currentPrefixList = null;
                StringBuilder caps = new StringBuilder();
                string currentRPL = null;

                Dictionary<string, string> rpl = new Dictionary<string, string>();

                foreach (string line in junShowConfigurationPolicyOptions)
                {
                    string lineTrim = line.Trim();

                    //prefix-list FROM-UNIFIED-MLS2 {
                    //01234567890123456789
                    if (line.StartsWith("prefix-list "))
                    {
                        currentPrefixList = line.Substring(12, line.Length - 14);
                        PEPrefixListToDatabase pl = new PEPrefixListToDatabase();
                        pl.Name = currentPrefixList;
                        prefixlistlive.Add(currentPrefixList, new Tuple<PEPrefixListToDatabase, List<PEPrefixEntryToDatabase>>(pl, new List<PEPrefixEntryToDatabase>()));
                    }
                    else if (currentPrefixList != null && line.Length >= 14 && char.IsDigit(line[4]) && line.EndsWith(";"))
                    {
                        //    1.1.1.1/1;
                        //012345678901234
                        string prefix = line.Substring(4, line.Length - 5);
                        PEPrefixEntryToDatabase pe = new PEPrefixEntryToDatabase();
                        pe.Network = prefix.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries)[0];

                        prefixlistlive[currentPrefixList].Item2.Add(pe);
                    }
                    else if (currentRPL != null)
                    {
                        if (line == "}")
                        {
                            rpl.Add(currentRPL, caps.ToString());
                            currentRPL = null;
                        }
                        else
                        {
                            caps.AppendLine(line);
                        }
                    }
                    else if (line.StartsWith("policy-statement "))
                    {
                        //policy-statement IMSlink-BB-JKT-import {
                        //01234567890123456789
                        currentRPL = line.Substring(17, line.Length - 19);
                        caps.Clear();
                    }

                }

                #endregion

                PERouteUseToDatabase current = null;
                string currentRouteNameID = null;
                string currentRouter = null;
                string currentGroup = null;                
                string currentRemoteAS = null;
                string currentRPLIN = null;
                string currentRPLOUT = null;
                string currentNetwork = null;
                bool currentPrefixLimit = false;
                int currentMaximumPrefix = -1;
                int currentBGPAS = -1;
                               
                foreach (string line in junShowConfigurationRoutingInstancesLines)
                {
                    string lineTrim = line.Trim();
                    bool inactive = false;
                    if (lineTrim.StartsWith("inactive: "))
                    {
                        lineTrim = lineTrim.Substring(10);
                        inactive = true;
                    }
                    if (line.EndsWith("{") && !line.StartsWith(" "))
                    {
                        currentRouter = null;
                        currentGroup = null;

                        current = null;
                        currentRemoteAS = null;
                        currentRPLOUT = null;
                        currentRPLIN = null;
                        currentNetwork = null;

                        string vrfname = lineTrim.Substring(0, lineTrim.LastIndexOf(' '));
                        if (vrfname != null)
                        {
                            if (routenamedb.ContainsKey(vrfname) && routenamelive.ContainsKey(vrfname))
                            {
                                currentRouteNameID = routenamedb[vrfname]["PN_ID"].ToString();
                                string rd = routenamedb[vrfname]["PN_RD"].ToString();
                                if (rd != null && rd.Length > 1)
                                {
                                    string[] rds = rd.Split(new char[] { ':' });
                                    int bgpas;
                                    if (rds.Length == 2 && rds[0].Length <= 5 && int.TryParse(rds[0], out bgpas)) currentBGPAS = bgpas;
                                }
                            }
                            else
                                currentRouteNameID = null;
                        }
                    }
                    else if (currentRouteNameID != null)
                    {
                        //01234567890123456789
                        //route 10.200.0.0/24 next-hop 172.31.24.14;
                        //route 172.27.6.208/28 next-hop [ 172.27.6.198 172.27.6.228 ];
                        //route 172.27.6.16/28 next-hop [ 172.27.6.6 172.27.6.236 ];
                        //route 172.27.6.48/28 next-hop [ 172.27.6.36 172.27.6.244 ];
                        //route 172.28.18.32/27 next-hop 172.28.18.2;

                        //route 172.27.2.96/28 {
                        //    qualified-next-hop 172.27.2.52;
                        //    qualified-next-hop 172.27.2.60;
                        //    qualified-next-hop 172.27.2.68;
                        //    qualified-next-hop 172.27.2.76;
                        //    qualified-next-hop 172.27.2.84;
                        //    qualified-next-hop 172.27.2.92;
                        //    01234567890123456789
                        //}

                        if (lineTrim.StartsWith("route ") && lineTrim.IndexOf("next-hop") > -1 && lineTrim.EndsWith(";"))
                        {
                            string network = lineTrim.Substring(6, lineTrim.IndexOf(' ', 6) - 6);
                            string nexthopline = lineTrim.Substring(lineTrim.IndexOf("next-hop") + 9).Trim(new char[] { '[', ']', ' ', ';' });

                            string[] nexthops = nexthopline.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries).RemoveDuplicatedEntries();

                            foreach (string nexthop in nexthops)
                            {
                                PERouteUseToDatabase i = new PERouteUseToDatabase();
                                i.RouteNameID = currentRouteNameID;
                                i.Type = "S";
                                i.Network = network;
                                i.Neighbor = nexthop;
                                i.InterfaceID = FindInterfaceByNeighbor(i.Neighbor, currentRouteNameID, interfacelive);

                                string key = currentRouteNameID + "_S_" + network + "_" + (i.Neighbor != null ? i.Neighbor : "") + "_" + (i.InterfaceID != null ? i.InterfaceID : "");
                                routeuselive.Add(key, i);
                            }
                        }
                        else if (lineTrim.StartsWith("route ") && lineTrim.EndsWith(" {"))
                        {
                            currentNetwork = lineTrim.Substring(6, lineTrim.IndexOf(' ', 6) - 6);
                        }
                        else if (lineTrim.StartsWith("qualified-next-hop ") && currentNetwork != null)
                        {
                            string nexthopline = lineTrim.Substring(19).Trim(new char[] { '[', ']', ' ', ';' });

                            string[] nexthops = nexthopline.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries).RemoveDuplicatedEntries();

                            foreach (string nexthop in nexthops)
                            {
                                PERouteUseToDatabase i = new PERouteUseToDatabase();
                                i.RouteNameID = currentRouteNameID;
                                i.Type = "S";
                                i.Network = currentNetwork;
                                i.Neighbor = nexthop;
                                i.InterfaceID = FindInterfaceByNeighbor(i.Neighbor, currentRouteNameID, interfacelive);

                                string key = currentRouteNameID + "_S_" + currentNetwork + "_" + (i.Neighbor != null ? i.Neighbor : "") + "_" + (i.InterfaceID != null ? i.InterfaceID : "");
                                routeuselive.Add(key, i);
                            }
                        }
                        else if (line.EndsWith("{") && lineTrim.StartsWith("bgp "))
                        {
                            currentRouter = "B";

                            current = null;
                            currentRemoteAS = null;
                            currentRPLOUT = null;
                            currentRPLIN = null;
                            currentNetwork = null;
                        }
                        else if (line.EndsWith("{") && lineTrim.StartsWith("ospf "))
                        {
                            currentRouter = "O";

                            current = null;
                            currentRemoteAS = null;
                            currentRPLOUT = null;
                            currentRPLIN = null;
                            currentNetwork = null;
                        }
                        else if (currentRouter == "B")
                        {
                            #region BGP

                            if (line.EndsWith("{") && lineTrim.StartsWith("group ")) currentGroup = lineTrim.Substring(6, lineTrim.Length - 8);
                            else if (currentGroup != null)
                            {
                                if (lineTrim.StartsWith("peer-as "))
                                {
                                    string value = lineTrim.Substring(8, lineTrim.Length - 9);
                                    if (current != null)
                                    {
                                        int ras = -1;
                                        if (int.TryParse(value, out ras)) current.RemoteAS = ras;
                                    }
                                    else currentRemoteAS = value;
                                }
                                else if (lineTrim.StartsWith("export "))
                                {
                                    string value = null;
                                    string rplname = lineTrim.Substring(7, lineTrim.Length - 8);
                                    if (rpl.ContainsKey(rplname)) value = rpl[rplname];
                                    if (current != null) current.RoutePolicyOut = value;
                                    else currentRPLOUT = value;
                                }
                                else if (lineTrim.StartsWith("import "))
                                {
                                    string value = null;
                                    string rplname = lineTrim.Substring(7, lineTrim.Length - 8);
                                    if (rpl.ContainsKey(rplname)) value = rpl[rplname];
                                    if (current != null) current.RoutePolicyIn = value;
                                    else currentRPLIN = value;
                                }
                                else if (lineTrim.StartsWith("prefix-limit {"))
                                {
                                    currentPrefixLimit = true;
                                }
                                else if (lineTrim.StartsWith("maximum ") && currentPrefixLimit)
                                {
                                    string mxm = lineTrim.Substring(8, lineTrim.Length - 9);
                                    int mxmi;
                                    if (int.TryParse(mxm, out mxmi))
                                    {
                                        if (current != null) current.MaximumPrefix = mxmi;
                                        else currentMaximumPrefix = mxmi;
                                    }
                                }
                                else if (lineTrim.StartsWith("neighbor "))
                                {
                                    string value = lineTrim.Substring(9);
                                    string neighbor = value.Substring(0, value.IndexOfAny(new char[] { ' ', ';' }));

                                    current = new PERouteUseToDatabase();
                                    current.RouteNameID = currentRouteNameID;
                                    current.Type = "B";
                                    current.Neighbor = neighbor;
                                    current.BGPAS = currentBGPAS;
                                    current.InterfaceID = FindInterfaceByNeighbor(neighbor, currentRouteNameID, interfacelive);

                                    string key = currentRouteNameID + "_B_" + neighbor + "_" + currentBGPAS;
                                    routeuselive.Add(key, current);

                                    int ras = -1;
                                    if (currentRemoteAS != null && int.TryParse(currentRemoteAS, out ras)) current.RemoteAS = ras;
                                    current.RoutePolicyOut = currentRPLOUT;
                                    current.RoutePolicyIn = currentRPLIN;
                                    current.MaximumPrefix = currentMaximumPrefix;
                                }
                            }

                            #endregion
                        }
                        else if (currentRouter == "O")
                        {
                            #region OSPF
                            #endregion
                        }
                    }
                }

                #endregion
            }
            else if (nodeManufacture == hwe)
            {
                #region hwe

                #region STATIC
                //display cur | in ip route-static
                //ip route-static vpn-instance Transit_Mix 118.98.26.0 255.255.255.0 NULL0
                //ip route-static vpn-instance Transit_Mix 118.98.90.0 255.255.255.0 NULL0 1.1.1.1
                //012345678901234567890123456789

                //ip route vrf Astinet 203.130.235.128 255.255.255.248 192.168.3.218
                //ip route vrf Astinet 203.130.235.128 255.255.255.248 GigabitEthernet1/19.2185
                //01234567890123456
                if (Request("display cur | in ip route-static vpn-instance", out lines, probe)) return probe;

                foreach (string line in lines)
                {
                    string linetrim = line.Trim();
                    if (linetrim.Length > 0)
                    {
                        if (linetrim.StartsWith("ip route-static vpn-instance"))
                        {
                            string lineleft = linetrim.Substring(29);
                            string[] linex = lineleft.Split(StringSplitTypes.Space);

                            string vrf = linex[0];
                            if (routenamedb.ContainsKey(vrf) && routenamelive.ContainsKey(vrf))
                            {
                                string routeNameID = routenamedb[vrf]["PN_ID"].ToString();
                                string network = linex[1];
                                string netmask = linex[2];
                                string ifname = null;
                                string interfaceID = null;
                                string neighbor = null;

                                string thirdarg = linex[3];
                                string fortharg = null;
                                if (linex.Length > 4) fortharg = linex[4];

                                int cidr = IPNetwork.ToCidr(IPAddress.Parse(netmask));
                                network = network + "/" + cidr;

                                if (char.IsLetter(thirdarg[0]))
                                {
                                    // probably interface
                                    NetworkInterface nif = NetworkInterface.Parse(thirdarg);
                                    if (nif != null)
                                    {
                                        ifname = nif.Name;
                                        if (interfacelive.ContainsKey(ifname)) interfaceID = interfacelive[ifname].ID;
                                        if (fortharg != null) neighbor = fortharg;
                                        else neighbor = "INTERFACE";
                                    }
                                    else if (thirdarg.StartsWith("NULL"))
                                    {
                                        if (fortharg != null) neighbor = fortharg;
                                    }
                                }
                                else neighbor = thirdarg;

                                PERouteUseToDatabase i = new PERouteUseToDatabase();
                                i.RouteNameID = routeNameID;
                                i.Type = "S";
                                i.Network = network;
                                i.Neighbor = neighbor;
                                i.InterfaceID = interfaceID;

                                if (interfaceID == null && ifname != null)
                                {
                                    i.InterfaceGone = ifname;
                                }

                                string key = routeNameID + "_S_" + network + "_" + (neighbor != null ? neighbor : "") + "_" + (interfaceID != null ? interfaceID : ifname != null ? ifname : "");
                                routeuselive.Add(key, i);
                            }
                        }
                    }
                }

                #endregion

                #region PREFIX-LIST

                //>display ip ip-prefix
                if (Request(@"display ip ip-prefix | in Prefix-list|index:", out lines, probe)) return probe;
                //Prefix-list bogons
                //0123456789012
                //Permitted 668
                //Denied 2622212578
                //    index: 5             permit     0.0.0.0/0   match-network
                //    01234567
                //    index: 10            permit     127.0.0.0/8           ge 8    le 32
                //    index: 15            permit     10.0.0.0/8            ge 8    le 32
                //           0             1          2                     3  4    5  6

                string currentPrefixList = null;

                foreach (string line in lines)
                {
                    string linetrim = line.Trim();
                    if (linetrim.Length > 0)
                    {
                        if (linetrim.StartsWith("Prefix-list"))
                        {
                            string name = linetrim.Substring(12);
                            if (!prefixlistlive.ContainsKey(name))
                            {
                                PEPrefixListToDatabase pl = new PEPrefixListToDatabase();
                                pl.Name = name;
                                prefixlistlive.Add(name, new Tuple<PEPrefixListToDatabase, List<PEPrefixEntryToDatabase>>(pl, new List<PEPrefixEntryToDatabase>()));
                            }
                            currentPrefixList = name;
                        }
                        else if (currentPrefixList != null && linetrim.StartsWith("index:"))
                        {
                            string[] secs = linetrim.Substring(7).Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);
                            int seq = -1;
                            if (!int.TryParse(secs[0], out seq)) seq = -1;
                            string access = secs[1];
                            string network = secs[2];
                            int ge = -1;
                            int le = -1;
                            if (secs.Length > 4)
                            {
                                if (secs[3] == "ge")
                                {
                                    int.TryParse(secs[4], out ge);
                                    if (secs.Length > 6 && secs[5] == "le") int.TryParse(secs[6], out le);
                                }
                                else if (secs[3] == "le")
                                {
                                    int.TryParse(secs[4], out le);
                                    if (secs.Length > 6 && secs[5] == "ge") int.TryParse(secs[6], out ge); // probably no, since ge always before le, lol
                                }
                            }

                            PEPrefixEntryToDatabase pe = new PEPrefixEntryToDatabase();
                            pe.Sequence = seq;
                            pe.Access = access == "permit" ? "P" : "D";
                            pe.Network = network;
                            pe.GE = ge;
                            pe.LE = le;

                            prefixlistlive[currentPrefixList].Item2.Add(pe);
                        }
                    }
                }




                #endregion

                #region BGP

                if (Request("disp cur | in \"bgp\\ |\\ ipv4-family\\ vpn-instance\\ |\\ ipv6-family\\ vpn-instance\\ |\\ \\ peer\\ |\\ #\"", out lines, probe)) return probe;

                string currentRouter = null;
                string currentRouteNameID = null;
                int currentBGPAS = -1;

                foreach (string line in lines)
                {
                    string linetrim = line.Trim();
                    if (linetrim.Length > 0)
                    {
                        if (linetrim.StartsWith("bgp "))
                        {
                            //bgp 7713
                            //01234
                            currentRouter = "B";
                            if (!int.TryParse(linetrim.Substring(4), out currentBGPAS)) currentBGPAS = 1;
                        }
                        else if (linetrim.StartsWith("peer "))
                        {
                            string[] splits = linetrim.Split(StringSplitTypes.Space);
                            if (currentRouter == "B" && currentRouteNameID != null)
                            {
                                string peer = splits[1];

                                // peer should be IP
                                IPAddress valid;
                                if (IPAddress.TryParse(peer, out valid))
                                {
                                    string key = currentRouteNameID + "_B_" + peer + "_" + (currentBGPAS != -1 ? currentBGPAS + "" : "");

                                    PERouteUseToDatabase i = null;

                                    if (routeuselive.ContainsKey(key)) i = routeuselive[key];
                                    else
                                    {
                                        i = new PERouteUseToDatabase();
                                        i.RouteNameID = currentRouteNameID;
                                        i.Type = "B";
                                        i.Neighbor = peer;
                                        i.BGPAS = currentBGPAS;

                                        routeuselive.Add(key, i);
                                    }

                                    if (splits.Length == 4 && splits[2] == "as-number")
                                    {
                                        int ras = -1;
                                        if (int.TryParse(splits[3], out ras)) i.RemoteAS = ras;
                                    }
                                    else if (splits.Length >= 4 && splits[2] == "route-limit")
                                    {
                                        int ras = -1;
                                        if (int.TryParse(splits[3], out ras)) i.MaximumPrefix = ras;
                                    }
                                    else if (splits.Length == 4 && splits[2] == "connect-interface")
                                    {
                                        NetworkInterface nif = NetworkInterface.Parse(splits[3]);
                                        if (nif != null)
                                        {
                                            string interfaceName = nif.Name;
                                            if (interfacelive.ContainsKey(interfaceName))
                                            {
                                                string interfaceID = interfacelive[interfaceName].ID;
                                                if (interfaceID != null) i.InterfaceID = interfaceID;
                                            }
                                            else if (interfaceName != null) i.InterfaceGone = interfaceName;
                                        }
                                    }
                                }
                            }
                        }
                        else if (linetrim.StartsWith("ipv4-family vpn-instance") || linetrim.StartsWith("ipv6-family vpn-instance"))
                        {
                            //ipv4-family vpn-instance Transit_Domestik
                            //012345678901234567890123456789
                            string vrfname = linetrim.Substring(25);
                            if (routenamedb.ContainsKey(vrfname) && routenamelive.ContainsKey(vrfname))
                                currentRouteNameID = routenamedb[vrfname]["PN_ID"].ToString();
                            else
                                currentRouteNameID = null;
                        }
                        else if (linetrim.StartsWith("#"))
                        {
                            currentRouteNameID = null;
                        }
                    }
                }

                #endregion

                #endregion
            }

            #endregion

            #region Check

            foreach (KeyValuePair<string, Tuple<PEPrefixListToDatabase, List<PEPrefixEntryToDatabase>>> pair in prefixlistlive)
            {
                PEPrefixListToDatabase li = pair.Value.Item1;

                if (!prefixlistdb.ContainsKey(pair.Key))
                {
                    Event("Prefix-List ADD: " + pair.Key);

                    li.ID = Database.ID();
                    prefixlistinsert.Add(li);
                }
                else
                {
                    Row db = prefixlistdb[pair.Key];

                    PEPrefixListToDatabase u = new PEPrefixListToDatabase();
                    u.ID = db["PX_ID"].ToString();
                    li.ID = u.ID;

                    // theres no update for PrefixList atm
                }
            }

            foreach (KeyValuePair<string, Tuple<PEPrefixListToDatabase, List<PEPrefixEntryToDatabase>>> pair in prefixlistlive)
            {
                PEPrefixListToDatabase li = pair.Value.Item1;
                List<PEPrefixEntryToDatabase> len = pair.Value.Item2;

                bool addEntry = false;

                // ADD / UPDATE
                foreach (PEPrefixEntryToDatabase en in len)
                {
                    string key = li.Name + "_" + en.Network + "_" + en.Sequence + "_" + en.Access + "_" + en.GE + "_" + en.LE;

                    if (!prefixentrydb.ContainsKey(key))
                    {
                        if (!addEntry)
                        {
                            Event("Prefix-List " + li.Name + " ENTRIES:");
                            addEntry = true;
                        }

                        Event("+ " + en.Network + (en.Access != null ? " " + (en.Access == "P" ? "permit" : "deny") : ""));

                        en.ID = Database.ID();
                        en.PrefixListID = li.ID;
                        prefixentryinsert.Add(en);
                    }
                    else
                    {
                        Row db = prefixentrydb[key];

                        PEPrefixEntryToDatabase u = new PEPrefixEntryToDatabase();
                        u.ID = db["PY_ID"].ToString();
                        en.ID = u.ID;

                        bool update = false;
                        StringBuilder updateinfo = new StringBuilder();

                        if (db["PY_Sequence"].ToIntShort(-1) != en.Sequence)
                        {
                            update = true;
                            u.UpdateSequence = true;
                            u.Sequence = en.Sequence;
                            UpdateInfo(updateinfo, "seq", db["PY_Sequence"].ToIntShort(-1).NullableInfo(), en.Sequence.NullableInfo());
                        }
                        if (db["PY_Access"].ToString() != en.Access)
                        {
                            update = true;
                            u.UpdateAccess = true;
                            u.Access = en.Access;
                            UpdateInfo(updateinfo, "access", db["PY_Access"].ToString() == "D" ? "deny" : db["PY_Access"].ToString() != null ? "permit" : null, en.Access == "D" ? "deny" : en.Access != null ? "permit" : null);
                        }
                        if (db["PY_Ge"].ToIntShort(-1) != en.GE)
                        {
                            update = true;
                            u.UpdateGE = true;
                            u.GE = en.GE;
                            UpdateInfo(updateinfo, "ge", db["PY_Ge"].ToIntShort(-1).NullableInfo(), en.GE.NullableInfo());
                        }
                        if (db["PY_Le"].ToIntShort(-1) != en.LE)
                        {
                            update = true;
                            u.UpdateLE = true;
                            u.LE = en.LE;
                            UpdateInfo(updateinfo, "le", db["PY_Le"].ToIntShort(-1).NullableInfo(), en.LE.NullableInfo());
                        }

                        if (update)
                        {
                            prefixentryupdate.Add(u);
                            Event("U " + en.Network + " " + updateinfo.ToString());
                        }
                    }
                }

                foreach (KeyValuePair<string, Row> pair2 in prefixentrydb)
                {
                    string key = pair2.Key;

                    if (pair2.Value["PX_Name"].ToString() == li.Name)
                    {
                        bool find = false;
                        foreach (PEPrefixEntryToDatabase en in len)
                        {
                            if (key == li.Name + "_" + en.Network + "_" + en.Sequence + "_" + en.Access + "_" + en.GE + "_" + en.LE)
                            {
                                find = true;
                                break;
                            }
                        }

                        if (!find)
                        {
                            if (!addEntry)
                            {
                                Event("Prefix-List " + li.Name + " ENTRIES:");
                                addEntry = true;
                            }

                            string access = pair2.Value["PY_Access"].ToString();

                            Event("- " + pair2.Value["PY_Network"].ToString() + (access != null ? " " + (access == "P" ? "permit" : "deny") : ""));
                            prefixentrydelete.Add(pair2.Value["PY_ID"].ToString());
                        }
                    }
                }
            }

            foreach (KeyValuePair<string, PERouteUseToDatabase> pair in routeuselive)
            {
                PERouteUseToDatabase li = pair.Value;

                string info = "UNKNOWN ";
                foreach (KeyValuePair<string, Row> pair2 in routenamedb)
                {
                    if (pair2.Value["PN_ID"].ToString() == li.RouteNameID)
                    {
                        info = pair2.Key + " ";
                        break;
                    }
                }

                if (!routeusedb.ContainsKey(pair.Key))
                {
                    string referencedinterface = null;

                    if (li.InterfaceID != null)
                    {
                        foreach (KeyValuePair<string, PEInterfaceToDatabase> pair2 in interfacelive)
                        {
                            if (pair2.Value.ID == li.InterfaceID)
                            {
                                referencedinterface = pair2.Key;
                                break;
                            }
                        }
                    }
                    else if (li.InterfaceGone != null)
                        referencedinterface = li.InterfaceGone + " NOTEXISTS";

                    if (li.Type == "S") info += "static " + li.Network + (li.Neighbor != null ? (" to " + li.Neighbor + (referencedinterface != null ? " (" + referencedinterface + ")" : "")) : "");
                    else if (li.Type == "B")
                    {
                        info += "bgp " + li.BGPAS + " to " + li.Neighbor;

                        // setup prefix list in and out ID
                        if (li.PrefixListInID != null)
                        {
                            if (prefixlistlive.ContainsKey(li.PrefixListInID)) li.PrefixListInID = prefixlistlive[li.PrefixListInID].Item1.ID;
                            else
                            {
                                li.PrefixListInGone = li.PrefixListInID = null;
                                li.PrefixListInID = null;
                            }
                        }
                        if (li.PrefixListOutID != null)
                        {
                            if (prefixlistlive.ContainsKey(li.PrefixListOutID)) li.PrefixListOutID = prefixlistlive[li.PrefixListOutID].Item1.ID;
                            else
                            {
                                li.PrefixListOutGone = li.PrefixListOutID = null;
                                li.PrefixListOutID = null;
                            }
                        }
                    }
                    else if (li.Type == "O") info += "ospf area " + li.Area + (referencedinterface != null ? " (" + referencedinterface + ")" : "");
                    else if (li.Type == "R") info += "rip" + (referencedinterface != null ? " (" + referencedinterface + ")" : "");
                    else if (li.Type == "E") info += "eigrp" + (referencedinterface != null ? " (" + referencedinterface + ")" : "");

                    Event("Routing ADD: " + info);

                    li.ID = Database.ID();
                    routeuseinsert.Add(li);
                }
                else
                {
                    Row db = routeusedb[pair.Key];

                    PERouteUseToDatabase u = new PERouteUseToDatabase();
                    u.ID = db["PU_ID"].ToString();
                    li.ID = u.ID;

                    bool update = false;
                    StringBuilder updateinfo = new StringBuilder();

                    if (db["PU_PI"].ToString() != li.InterfaceID || db["PU_PI_Gone"].ToString() != li.InterfaceGone)
                    {
                        update = true;
                        u.UpdateInterface = true;
                        u.InterfaceID = li.InterfaceID;
                        u.InterfaceGone = li.InterfaceGone;
                        UpdateInfo(updateinfo, "interface", null);
                    }

                    if (li.Type == "B")
                    {
                        // setup prefix list in and out ID
                        if (li.PrefixListInID != null)
                        {
                            if (prefixlistlive.ContainsKey(li.PrefixListInID)) li.PrefixListInID = prefixlistlive[li.PrefixListInID].Item1.ID;
                            else
                            {
                                li.PrefixListInGone = li.PrefixListInID;
                                li.PrefixListInID = null;
                            }
                        }
                        if (li.PrefixListOutID != null)
                        {
                            if (prefixlistlive.ContainsKey(li.PrefixListOutID)) li.PrefixListOutID = prefixlistlive[li.PrefixListOutID].Item1.ID;
                            else
                            {
                                li.PrefixListOutGone = li.PrefixListOutID;
                                li.PrefixListOutID = null;
                            }
                        }

                        if (db["PU_B_RemoteAS"].ToInt(-1) != li.RemoteAS)
                        {
                            update = true;
                            u.UpdateRemoteAS = true;
                            u.RemoteAS = li.RemoteAS;
                            UpdateInfo(updateinfo, "remote-as", db["PU_B_RemoteAS"].ToInt(-1).NullableInfo(), li.RemoteAS.NullableInfo());
                        }
                        if (db["PU_B_PX_In"].ToString() != li.PrefixListInID)
                        {
                            update = true;
                            u.UpdatePrefixListInID = true;
                            u.PrefixListInID = li.PrefixListInID;
                            if (li.PrefixListInGone == null) UpdateInfo(updateinfo, "pl-input", db["PU_B_PX_In"].ToString(), li.PrefixListInID, true);
                        }
                        if (db["PU_B_PX_In_Gone"].ToString() != li.PrefixListInGone)
                        {
                            update = true;
                            u.UpdatePrefixListInGone = true;
                            u.PrefixListInGone = li.PrefixListInGone;
                            if (li.PrefixListInID == null && li.PrefixListInGone != null) UpdateInfo(updateinfo, "pl-input", "notexists");
                        }
                        if (db["PU_B_PX_Out"].ToString() != li.PrefixListOutID)
                        {
                            update = true;
                            u.UpdatePrefixListOutID = true;
                            u.PrefixListOutID = li.PrefixListOutID;
                            if (li.PrefixListOutGone == null) UpdateInfo(updateinfo, "pl-output", db["PU_B_PX_Out"].ToString(), li.PrefixListOutID, true);
                        }
                        if (db["PU_B_PX_Out_Gone"].ToString() != li.PrefixListOutGone)
                        {
                            update = true;
                            u.UpdatePrefixListOutGone = true;
                            u.PrefixListOutGone = li.PrefixListOutGone;
                            if (li.PrefixListOutID == null && li.PrefixListOutGone != null) UpdateInfo(updateinfo, "pl-output", "notexists");
                        }
                        if (db["PU_B_RPL_In"].ToString() != li.RoutePolicyIn)
                        {
                            update = true;
                            u.UpdateRoutePolicyIn = true;
                            u.RoutePolicyIn = li.RoutePolicyIn;
                            UpdateInfo(updateinfo, "rpl-input", db["PU_B_RPL_In"].ToString(), li.RoutePolicyIn);
                        }
                        if (db["PU_B_RPL_Out"].ToString() != li.RoutePolicyOut)
                        {
                            update = true;
                            u.UpdateRoutePolicyOut = true;
                            u.RoutePolicyOut = li.RoutePolicyOut;
                            UpdateInfo(updateinfo, "rpl-output", db["PU_B_RPL_Out"].ToString(), li.RoutePolicyOut);
                        }
                        if (db["PU_B_MaximumPrefix"].ToInt(-1) != li.MaximumPrefix)
                        {
                            update = true;
                            u.UpdateMaximumPrefix = true;
                            u.MaximumPrefix = li.MaximumPrefix;
                            UpdateInfo(updateinfo, "max-prefix", db["PU_B_MaximumPrefix"].ToInt(-1).NullableInfo(), li.MaximumPrefix.NullableInfo());
                        }
                        if (db["PU_B_MaximumPrefix_Threshold"].ToIntShort(-1) != li.MaximumPrefixThreshold)
                        {
                            update = true;
                            u.UpdateMaximumPrefixThreshold = true;
                            u.MaximumPrefixThreshold = li.MaximumPrefixThreshold;
                            UpdateInfo(updateinfo, "max-prefix-threshold", db["PU_B_MaximumPrefix_Threshold"].ToIntShort(-1).NullableInfo(), li.MaximumPrefixThreshold.NullableInfo());
                        }
                        if (db["PU_B_MaximumPrefix_WarningOnly"].ToNullableBool() != li.MaximumPrefixWarningOnly)
                        {
                            update = true;
                            u.UpdateMaximumPrefixWarningOnly = true;
                            u.MaximumPrefixWarningOnly = li.MaximumPrefixWarningOnly;
                            UpdateInfo(updateinfo, "max-prefix-warning-only", db["PU_B_MaximumPrefix_WarningOnly"].ToNullableBool().ToString(), li.MaximumPrefixWarningOnly.ToString());
                        }
                    }
                    else if (li.Type == "O")
                    {
                        if (db["PU_O_MessageDigestKey"].ToString() != li.MessageDigestKey)
                        {
                            update = true;
                            u.UpdateMessageDigestKey = true;
                            u.MessageDigestKey = li.MessageDigestKey;
                            UpdateInfo(updateinfo, "message-digest-key", db["PU_O_MessageDigestKey"].ToString(), li.MessageDigestKey, true);
                        }
                        if (db["PU_O_InterfaceNetwork"].ToString() != li.InterfaceNetwork)
                        {
                            update = true;
                            u.UpdateInterfaceNetwork = true;
                            u.InterfaceNetwork = li.InterfaceNetwork;
                            UpdateInfo(updateinfo, "interface-network", db["PU_O_InterfaceNetwork"].ToString(), li.InterfaceNetwork);
                        }
                    }

                    if (update)
                    {
                        routeuseupdate.Add(u);
                        Event("Routing UPDATE: " + info + updateinfo.ToString());
                    }
                }
            }

            Summary("PREFIX_LIST_COUNT", prefixlistlive.Count);

            #endregion

            #region Execute

            // PREFIX-LIST
            // ADD
            batch.Begin();
            foreach (PEPrefixListToDatabase s in prefixlistinsert)
            {
                Insert insert = Insert("PEPrefixList");
                insert.Value("PX_ID", s.ID);
                insert.Value("PX_NO", nodeID);
                insert.Value("PX_Name", s.Name);

                batch.Execute(insert);
            }
            result = batch.Commit();
            if (!result.OK) return DatabaseFailure(probe);
            Event(result, EventActions.Add, EventElements.PrefixList, false);

            // PREFIX-ENTRY
            // ADD
            batch.Begin();
            foreach (PEPrefixEntryToDatabase s in prefixentryinsert)
            {
                Insert insert = Insert("PEPrefixEntry");
                insert.Value("PY_ID", s.ID);
                insert.Value("PY_PX", s.PrefixListID);
                insert.Value("PY_Network", s.Network);
                insert.Value("PY_Sequence", s.Sequence.Nullable(-1));
                insert.Value("PY_Access", s.Access);
                insert.Value("PY_Ge", s.GE.Nullable(-1));
                insert.Value("PY_Le", s.LE.Nullable(-1));

                batch.Execute(insert);
            }
            result = batch.Commit();
            if (!result.OK) return DatabaseFailure(probe);
            Event(result, EventActions.Add, EventElements.PrefixEntry, false);

            // UPDATE
            batch.Begin();
            foreach (PEPrefixEntryToDatabase s in prefixentryupdate)
            {
                Update update = Update("PEPrefixEntry");
                update.Set("PY_Sequence", s.Sequence, s.UpdateSequence);
                update.Set("PY_Access", s.Access, s.UpdateAccess);
                update.Set("PY_Ge", s.GE.Nullable(-1), s.UpdateGE);
                update.Set("PY_Le", s.LE.Nullable(-1), s.UpdateLE);
                update.Where("PY_ID", s.ID);
                batch.Execute(update);
            }
            result = batch.Commit();
            if (!result.OK) return DatabaseFailure(probe);
            Event(result, EventActions.Update, EventElements.PrefixEntry, false);

            // DELETE
            batch.Begin();
            foreach (string s in prefixentrydelete)
            {
                batch.Execute("delete from PEPrefixEntry where PY_ID = {0}", s);
            }
            result = batch.Commit();
            if (!result.OK) return DatabaseFailure(probe);
            Event(result, EventActions.Delete, EventElements.PrefixEntry, false);

            // ROUTEUSE
            // ADD
            batch.Begin();
            foreach (PERouteUseToDatabase s in routeuseinsert)
            {
                Insert insert = Insert("PERouteUse");
                insert.Value("PU_ID", s.ID);
                insert.Value("PU_PN", s.RouteNameID);
                insert.Value("PU_Type", s.Type);

                insert.Value("PU_PI", s.InterfaceID);
                insert.Value("PU_PI_Gone", s.InterfaceGone);
                insert.Value("PU_Network", s.Network);
                insert.Value("PU_Neighbor", s.Neighbor);

                if (s.Type == "S")
                {

                }
                else if (s.Type == "B")
                {
                    insert.Value("PU_B_AS", s.BGPAS);
                    insert.Value("PU_B_RemoteAS", s.RemoteAS.Nullable(-1));
                    insert.Value("PU_B_PX_In", s.PrefixListInID);
                    insert.Value("PU_B_PX_In_Gone", s.PrefixListInGone);
                    insert.Value("PU_B_PX_Out", s.PrefixListOutID);
                    insert.Value("PU_B_PX_Out_Gone", s.PrefixListOutGone);
                    insert.Value("PU_B_RPL_In", s.RoutePolicyIn);
                    insert.Value("PU_B_RPL_Out", s.RoutePolicyOut);
                    insert.Value("PU_B_MaximumPrefix", s.MaximumPrefix.Nullable(-1));
                    insert.Value("PU_B_MaximumPrefix_Threshold", s.MaximumPrefixThreshold.Nullable(-1));
                    insert.Value("PU_B_MaximumPrefix_WarningOnly", s.MaximumPrefixWarningOnly);                    
                }
                else if (s.Type == "O")
                {
                    insert.Value("PU_O_Process", s.Process);
                    insert.Value("PU_O_Area", s.Area);
                    insert.Value("PU_O_Wildcard", s.Wildcard);
                    insert.Value("PU_O_MessageDigestKey", s.MessageDigestKey);
                    insert.Value("PU_O_InterfaceNetwork", s.InterfaceNetwork);
                }
                else if (s.Type == "R")
                {

                }
                else if (s.Type == "E")
                {

                }

                batch.Execute(insert);
            }
            result = batch.Commit();
            if (!result.OK) return DatabaseFailure(probe);
            Event(result, EventActions.Add, EventElements.Routing, false);

            // UPDATE
            batch.Begin();
            foreach (PERouteUseToDatabase s in routeuseupdate)
            {
                Update update = Update("PERouteUse");
                update.Set("PU_PI", s.InterfaceID, s.UpdateInterface);
                update.Set("PU_PI_Gone", s.InterfaceGone, s.UpdateInterface);
                update.Set("PU_B_RemoteAS", s.RemoteAS.Nullable(-1), s.UpdateRemoteAS);
                update.Set("PU_B_PX_In", s.PrefixListInID, s.UpdatePrefixListInID);
                update.Set("PU_B_PX_In_Gone", s.PrefixListInGone, s.UpdatePrefixListInGone);
                update.Set("PU_B_PX_Out", s.PrefixListOutID, s.UpdatePrefixListOutID);
                update.Set("PU_B_PX_Out_Gone", s.PrefixListOutGone, s.UpdatePrefixListOutGone);
                update.Set("PU_B_RPL_In", s.RoutePolicyIn, s.UpdateRoutePolicyIn);
                update.Set("PU_B_RPL_Out", s.RoutePolicyOut, s.UpdateRoutePolicyOut);
                update.Set("PU_B_MaximumPrefix", s.MaximumPrefix.Nullable(-1), s.UpdateMaximumPrefix);
                update.Set("PU_B_MaximumPrefix_Threshold", s.MaximumPrefixThreshold.Nullable(-1), s.UpdateMaximumPrefixThreshold);
                update.Set("PU_B_MaximumPrefix_WarningOnly", s.MaximumPrefixWarningOnly, s.UpdateMaximumPrefixWarningOnly);
                update.Set("PU_O_MessageDigestKey", s.MessageDigestKey, s.UpdateMessageDigestKey);
                update.Set("PU_O_InterfaceNetwork", s.InterfaceNetwork, s.UpdateInterfaceNetwork);
                update.Where("PU_ID", s.ID);
                batch.Execute(update);
            }
            result = batch.Commit();
            if (!result.OK) return DatabaseFailure(probe);
            Event(result, EventActions.Update, EventElements.Routing, false);

            // DELETE
            batch.Begin();
            foreach (KeyValuePair<string, Row> pair in routeusedb)
            {
                if (!routeuselive.ContainsKey(pair.Key))
                {
                    string info = "UNKNOWN ";
                    foreach (KeyValuePair<string, Row> pair2 in routenamedb)
                    {
                        if (pair2.Value["PN_ID"].ToString() == pair.Value["PU_PN"].ToString())
                        {
                            info = pair2.Key + " ";
                            break;
                        }
                    }

                    string referencedinterface = null;
                    string interfaceID = pair.Value["PU_PI"].ToString();
                    string interfaceGone = pair.Value["PU_PI_Gone"].ToString();

                    if (interfaceID != null)
                    {
                        foreach (KeyValuePair<string, PEInterfaceToDatabase> pair2 in interfacelive)
                        {
                            if (pair2.Value.ID == interfaceID)
                            {
                                referencedinterface = pair2.Key;
                                break;
                            }
                        }
                    }
                    else if (interfaceGone != null && interfaceGone != "")
                        referencedinterface = interfaceGone + " NOTEXISTS";

                    string type = pair.Value["PU_Type"].ToString();

                    if (type == "S") info += "static " + pair.Value["PU_Network"].ToString() + " to " + pair.Value["PU_Neighbor"].ToString() + (referencedinterface != null ? " (" + referencedinterface + ")" : "");
                    else if (type == "B") info += "bgp " + pair.Value["PU_B_AS"].ToString() + " to " + pair.Value["PU_Neighbor"].ToString();
                    else if (type == "O") info += "ospf area " + pair.Value["PU_O_Area"].ToString() + (referencedinterface != null ? " (" + referencedinterface + ")" : "");
                    else if (type == "R") info += "rip" + (referencedinterface != null ? " (" + referencedinterface + ")" : "");
                    else if (type == "E") info += "eigrp" + (referencedinterface != null ? " (" + referencedinterface + ")" : "");
                    
                    Event("Routing DELETE: " + info);
                    batch.Execute("delete from PERouteUse where PU_ID = {0}", pair.Value["PU_ID"].ToString());
                }
            }
            result = batch.Commit();
            if (!result.OK) return DatabaseFailure(probe);
            Event(result, EventActions.Delete, EventElements.Routing, false);

            #endregion

            #endregion

            #region LATE DELETE
            
            // DELETE Prefix-LIST
            batch.Begin();
            foreach (KeyValuePair<string, Row> pair in prefixlistdb)
            {
                if (!prefixlistlive.ContainsKey(pair.Key))
                {
                    Event("Prefix-List DELETE: " + pair.Key);
                    // delete left over PEPrefixEntry
                    batch.Execute("delete from PEPrefixEntry where PY_PX = {0}", pair.Value["PX_ID"].ToString());
                    batch.Execute("delete from PEPrefixList where PX_ID = {0}", pair.Value["PX_ID"].ToString());
                }
            }
            result = batch.Commit();
            if (!result.OK) return DatabaseFailure(probe);
            Event(result, EventActions.Delete, EventElements.PrefixList, false);

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
            if (!result.OK) return DatabaseFailure(probe);
            Event(result, EventActions.Delete, EventElements.QOS, false);

            // DELETE ROUTE
            batch.Begin();
            List<string> routenamedelete = new List<string>();
            foreach (KeyValuePair<string, Row> pair in routenamedb)
            {
                if (!routenamelive.ContainsKey(pair.Key))
                {
                    Event("Route Name DELETE: " + pair.Key);
                    string id = pair.Value["PN_ID"].ToString();
                    batch.Execute("update PEInterface set PI_PN = NULL where PI_PN = {0}", id);
                    batch.Execute("delete from PERouteUse where PU_PN = {0}", id);
                    routenamedelete.Add(id);
                }
            }
            result = batch.Commit();
            if (!result.OK) return DatabaseFailure(probe);
            batch.Begin();
            foreach (string id in routenamedelete)
            {
                batch.Execute("delete from PERouteName where PN_ID = {0}", id);
            }
            result = batch.Commit();
            if (!result.OK) return DatabaseFailure(probe);
            Event(result, EventActions.Delete, EventElements.VRFReference, false);

            result = Execute("delete from PERouteTarget where PT_PR in (select PR_ID from PERoute left join PERouteName on PN_PR = PR_ID where PN_ID is null)");
            if (!result.OK) return DatabaseFailure(probe);
            Event(result, EventActions.Delete, EventElements.VRFRouteTarget, false);

            result = Execute("delete from PERoute where PR_ID in (select PR_ID from PERoute left join PERouteName on PN_PR = PR_ID where PN_ID is null)");
            if (!result.OK) return DatabaseFailure(probe);
            Event(result, EventActions.Delete, EventElements.VRF, false);

            #endregion

            return probe;
        }

        private DateTime? ParseLastInput(string lastInput)
        {
            if (lastInput == "never") return null;
            else if (lastInput.IndexOf(":") > -1)
            {
                //0:00:00
                string[] hpc = lastInput.Split(new char[] { ':' });
                int hourago = int.Parse(hpc[0]);
                int minuteago = int.Parse(hpc[1]);
                int secondago = int.Parse(hpc[2]);
                return DateTime.UtcNow - new TimeSpan(hourago, minuteago, secondago);
            }
            else if (lastInput.IndexOf("w") > -1)
            {
                //0w2d
                //12w3d
                //1y51w
                TimeSpan yearsago = TimeSpan.Zero;
                TimeSpan weeksago = TimeSpan.Zero;
                TimeSpan daysago = TimeSpan.Zero;

                StringBuilder sb = new StringBuilder();
                foreach (char c in lastInput)
                {
                    if (char.IsDigit(c)) sb.Append(c);
                    else
                    {
                        string sbs = sb.ToString();
                        sb.Clear();
                        int sbi = int.Parse(sbs);
                        if (c == 'y') yearsago = new TimeSpan(sbi * 365, 0, 0, 0);
                        else if (c == 'w') weeksago = new TimeSpan(sbi * 7, 0, 0, 0);
                        else if (c == 'd') daysago = new TimeSpan(sbi, 0, 0, 0);
                    }
                }

                DateTime lastDown = (DateTime.UtcNow - yearsago - weeksago - daysago).Date;
                if (daysago == TimeSpan.Zero) lastDown = new DateTime(lastDown.Year, lastDown.Month, 1);
                return lastDown;
            }
            else return null;
        }

        private string FindInterfaceByNeighbor(string neighbor, string routeNameID, SortedDictionary<string, PEInterfaceToDatabase> interfacelive)
        {
            if (neighbor == null) return null;

            IPAddress neighborIP;

            string interfaceID = null;

            if (IPAddress.TryParse(neighbor, out neighborIP))
            {
                bool ipv4 = true;
                if (neighbor.IndexOf(':') > -1) ipv4 = false;

                foreach (KeyValuePair<string, PEInterfaceToDatabase> pair in interfacelive)
                {
                    PEInterfaceToDatabase li = pair.Value;

                    if (li.RouteID == routeNameID)
                    {
                        if (li.IP != null)
                        {
                            foreach (string cip in li.IP)
                            {
                                string[] cips = cip.Split(StringSplitTypes.Underscore);

                                if (cips.Length == 3 && cips[0] == (ipv4 ? "0" : "1"))
                                {
                                    string ipcidr = cips[2];
                                    string thip = ipcidr.Split(new char[] { '/' })[0];

                                    IPNetwork net;
                                    if (IPNetwork.TryParse(ipcidr, out net))
                                    {
                                        if (neighborIP.IsInSameSubnet(net.FirstUsable, net.Netmask))
                                        {
                                            // satu subnet, bisa jadi neighbor tembakan
                                            interfaceID = li.ID;
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return interfaceID;
        }
    }
}
