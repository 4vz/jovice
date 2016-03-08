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
    internal class InterfaceDescription
    {
        #region Fields

        private bool ready = true;

        public bool Ready
        {
            get { return ready; }
            set { ready = value; }
        }

        private string interfaceID;

        public string InterfaceID
        {
            get { return interfaceID; }
            set { interfaceID = value; }
        }

        private string interfaceName;

        public string InterfaceName
        {
            get { return interfaceName; }
            set { interfaceName = value; }
        }

        private string nodeID;

        public string NodeID
        {
            get { return nodeID; }
            set { nodeID = value; }
        }

        private string nodeName;

        public string NodeName
        {
            get { return nodeName; }
            set { nodeName = value; }
        }

        #endregion

        #region Constructor

        public InterfaceDescription()
        {

        }

        #endregion

    }

    internal static class TopologyFinder
    {
        #region Fields

        private static Thread mainLoop = null;

        private static bool stop = false;

        private static bool idle = false;

        #endregion

        #region Methods

        private static void Event(string message)
        {
            Necrow.Event(message, "TOPO");
        }

        public static bool IsStop()
        {
            if (mainLoop == null) return true;
            else return !mainLoop.IsAlive;
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
                Result result;

                Batch batch = j.Batch();
                Dictionary<string, string> nodes = new Dictionary<string, string>();
                Dictionary<string, string> nodeAliases = new Dictionary<string, string>();
                List<string> nodeCandidates = new List<string>();

                #region Populate Nodes

                result = j.Query(@"
select NC_Name from NodeCandidate
");

                foreach (Row row in result)
                {
                    string name = row["NC_Name"].ToString();
                    if (!nodeCandidates.Contains(name)) nodeCandidates.Add(name);
                }
                
                result = j.Query(@"
select *, LEN(Name) as Len from (
select NO_ID as ID, NO_Type as Type, NO_Name as Name
from Node where NO_Active = 1
union 
select NO_ID as ID, NO_Type as Type, NA_Name as Name
from NodeAlias, Node where NO_Active = 1 and NA_NO = NO_ID
) source
order by Len desc
");
                foreach (Row row in result)
                {
                    string name = row["Name"].ToString();
                    if (!nodes.ContainsKey(name)) nodes.Add(name, row["Type"].ToString() + row["ID"].ToString());
                }

                #endregion

                #region MI_TO_PI and PI_TO_MI Physical

                batch.Clear();

                result = j.Query(@"
select 
top 500 MI_ID, MI_Name, MI_Description, NO_Name
from 
MEInterface with (NOLOCK), Node
where 
MI_Description is not null and MI_Name not like '%.%' and MI_TO_Check is null and MI_NO = NO_ID
");

                foreach (Row row in result)
                {
                    string miID = row["MI_ID"].ToString();
                    string nodeName = row["NO_Name"].ToString();
                    string name = row["MI_Name"].ToString();
                    string desc = row["MI_Description"].ToString();

                    if (desc != null)
                    {
                        InterfaceDescription[] ds = ParseDescription(desc, nodeName, nodes, nodeCandidates);

                        if (ds.Length > 0)
                        {
                            bool readyExists = false;
                            List<Tuple<string, bool>> mutualPeers = new List<Tuple<string, bool>>();

                            foreach (InterfaceDescription d in ds)
                            {
                                if (d.Ready)
                                {
                                    readyExists = true;

                                    // cek dari arah PE
                                    Result result2 = j.Query("select PI_ID, PI_Description, PI_Status from PEInterface where PI_ID = {0}", d.InterfaceID);

                                    Row row2 = result2[0];
                                    string pidesc = row2["PI_Description"].ToString();
                                    bool pistatus = row2["PI_Status"].ToBoolean();

                                    if (pidesc != null)
                                    {
                                        InterfaceDescription[] ds2 = ParseDescription(pidesc, d.NodeName, nodes, nodeCandidates);

                                        if (ds2.Length > 0)
                                        {
                                            foreach (InterfaceDescription d2 in ds2)
                                            {
                                                if (d2.Ready)
                                                {
                                                    if (d2.InterfaceID == miID)
                                                    {
                                                        //TFEvent("    Mutually Connected!");
                                                        Event("Updated: " + nodeName + " " + name.ToUpper() + " <--> " + d.NodeName + " " + d.InterfaceName);

                                                        mutualPeers.Add(new Tuple<string, bool>(d.InterfaceID, pistatus));
                                                        break;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }

                            if (readyExists == false)
                            {
                                // ada referensi ke PE, tapi PE itu belum siap
                                batch.Execute("update MEInterface set MI_TO_PI = null, MI_TO_Check = 0 where MI_ID = {0}", miID);

                                // 
                                batch.Execute("update MEInterface set MI_TO_PI = null, MI_TO_Check = null where MI_MI = {0}", miID);
                            }
                            else
                            {
                                // ada referensi ke PE, dan PE telah siap
                                if (mutualPeers.Count > 0)
                                {
                                    // ada yg mutual
                                    bool upExists = false;
                                    foreach (Tuple<string, bool> item in mutualPeers)
                                    {
                                        if (item.Item2) // cari yg status up
                                        {
                                            upExists = true;
                                            batch.Execute("update MEInterface set MI_TO_PI = {1}, MI_TO_Check = 1 where MI_ID = {0}", miID, item.Item1);
                                            batch.Execute("update PEInterface set PI_TO_MI = {1} where PI_ID = {0}", item.Item1, miID);
                                            break;
                                        }
                                    }

                                    if (upExists == false)
                                    {
                                        // gada yg up, pake yg pertama saja
                                        Tuple<string, bool> item = mutualPeers[0];
                                        batch.Execute("update MEInterface set MI_TO_PI = {1}, MI_TO_Check = 1 where MI_ID = {0}", miID, item.Item1);
                                        batch.Execute("update PEInterface set PI_TO_MI = {1} where PI_ID = {0}", item.Item1, miID);
                                    }
                                }
                                else
                                {
                                    // gak ada yg mutual
                                    batch.Execute("update MEInterface set MI_TO_PI = null, MI_TO_Check = 1 where MI_ID = {0}", miID);
                                }
                            }
                        }
                        else
                        {
                            // lets find some new node here
                            string[] descs = desc.Split(new char[] { ' ', '(', ')', '_', '[', ']', ';', '.', '=', ':', '@', '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);

                            bool found = false;
                            foreach (string des in descs)
                            {
                                if (des.Length >= 8 && des.IndexOf('-') > -1 && (des.StartsWith("PE") || des.StartsWith("ME") || des.StartsWith("CES")) && !des.EndsWith("-"))
                                {
                                    string[] dess = des.Split(new char[] { '-' });
                                    if (dess.Length >= 3)
                                    {
                                        if (dess[0].StartsWith("ME") && dess[0].Length > 2 && !char.IsDigit(dess[0][2])) continue;
                                        else if (dess[0].StartsWith("PE") && dess[0].Length > 2 && !char.IsDigit(dess[0][2])) continue;
                                        else if (dess[0].StartsWith("CES") && dess[0].Length > 3 && !char.IsDigit(dess[0][3])) continue;

                                        bool illegal = false;
                                        foreach (char c in des)
                                        {
                                            if (c == '-' || char.IsDigit(c) || (char.IsLetter(c) && char.IsUpper(c))) { }
                                            else { illegal = true; break; }
                                        }

                                        if (illegal == false)
                                        {
                                            string thisdes = des.ToUpper();

                                            if (!nodes.ContainsKey(thisdes))
                                            {
                                                if (!nodeCandidates.Contains(thisdes))
                                                {
                                                    Event("Node Candidate: " + thisdes);
                                                    nodeCandidates.Add(thisdes);
                                                    batch.Execute("insert into NodeCandidate values({0}, {1})", Database.ID(), thisdes);
                                                }

                                                found = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            
                            batch.Execute("update MEInterface set MI_TO_PI = null, MI_TO_Check = {1} where MI_ID = {0}", miID, found ? 0 : 1);                            
                        }

                        batch.Execute("update PEInterface set PI_TO_MI = null where PI_TO_MI in (select MI_ID from MEInterface where MI_MI = {0})", miID);
                        batch.Execute("update MEInterface set MI_TO_PI = null, MI_TO_Check = null where MI_MI = {0}", miID);
                    }
                }

                result = batch.Commit();

                int resultMI = result.AffectedRows;

                #endregion

                #region MI_MI

                batch.Clear();

                result = j.Query(@"
select top 500 a.MI_ID, b.MI_ID as MI_MI
from 
MEInterface a, MEInterface b
where a.MI_Name like '%.%' and a.MI_MI is null and a.MI_NO = b.MI_NO and 
b.MI_Name not like '%.%' and a.MI_Name like b.MI_Name + '.%'
");

                foreach (Row row in result)
                {
                    string miID = row["MI_ID"].ToString();
                    string miMI = row["MI_MI"].ToString();

                    batch.Execute("update MEInterface set MI_MI = {0} where MI_ID = {1}", miMI, miID);
                }

                result = batch.Commit();

                int resultMIMI = result.AffectedRows;

                #endregion

                #region MI_MI Aggregator

                batch.Clear();

                result = j.Query(@"
select top 500 a.MI_ID, b.MI_ID as MI_MI
from 
MEInterface a, MEInterface b
where 
a.MI_Aggregator is not null and a.MI_MI is null and a.MI_NO = b.MI_NO and
b.MI_Name = 'Ag' + CONVERT(varchar(2), a.MI_Aggregator)
");
                foreach (Row row in result)
                {
                    string miID = row["MI_ID"].ToString();
                    string miMI = row["MI_MI"].ToString();

                    batch.Execute("update MEInterface set MI_MI = {0} where MI_ID = {1}", miMI, miID);
                }

                result = batch.Commit();

                int resultMIMIAgg = result.AffectedRows;

                #endregion

                #region PI_PI

                batch.Clear();

                result = j.Query(@"
select top 500 a.PI_ID, b.PI_ID as PI_PI
from 
PEInterface a, PEInterface b
where a.PI_Name like '%.%' and a.PI_PI is null and a.PI_NO = b.PI_NO and 
b.PI_Name not like '%.%' and a.PI_Name like b.PI_Name + '.%'
");

                foreach (Row row in result)
                {
                    string piID = row["PI_ID"].ToString();
                    string piPI = row["PI_PI"].ToString();

                    batch.Execute("update PEInterface set PI_PI = {0} where PI_ID = {1}", piPI, piID);
                }

                result = batch.Commit();

                int resultPI = result.AffectedRows;

                #endregion

                #region MI_TO_PI and PI_TO_MI

                batch.Clear();

                result = j.Query(@"
select top 500 a.MI_ID, a.MI_Name, c.PI_ID, c.PI_Name, c.PI_NO
from
MEInterface a, MEInterface b, PEInterface c
where a.MI_MI = b.MI_ID and b.MI_TO_PI = c.PI_ID
and a.MI_TO_Check is null
");

                foreach (Row row in result)
                {
                    string miID = row["MI_ID"].ToString();
                    string miName = row["MI_Name"].ToString();
                    string piID = row["PI_ID"].ToString();
                    string piName = row["PI_Name"].ToString();
                    string piNO = row["PI_NO"].ToString();
                    
                    string vlan = null;
                    string piSubID = null;

                    int dotin = miName.IndexOf('.');
                    if (dotin > -1) vlan = miName.Substring(dotin + 1);

                    int dotinvlan = vlan.IndexOf('.');

                    if (dotin > -1 && dotinvlan == -1)
                    {
                        string piSub = piName + "." + vlan;
                        Result result2 = j.Query("select PI_ID from PEInterface where PI_NO = {0} and PI_Name = {1}", piNO, piSub);
                        if (result2.Count == 1) piSubID = result2[0]["PI_ID"].ToString();
                    }

                    batch.Execute("update MEInterface set MI_TO_PI = {0}, MI_TO_Check = 1 where MI_ID = {1}", piSubID, miID);
                    batch.Execute("update PEInterface set PI_TO_MI = {0} where PI_ID = {1}", miID, piSubID);
                }

                result = batch.Commit();

                int resultMITOPI = result.AffectedRows;

                #endregion

                #region MP

                batch.Clear();

                result = j.Query(@"
select top 500 MP_ID, MP_VCID, NO_ID from MEPeer, MESDP, Node
where MP_MS = MS_ID and MS_IP = NO_IP and MP_TO_Check is null
");

                foreach (Row row in result)
                {
                    string mpID = row["MP_ID"].ToString();
                    string mpVcid = row["MP_VCID"].ToString();
                    string noID = row["NO_ID"].ToString();

                    Result result2 = j.Query("select MC_ID from MECircuit where MC_NO = {0} and MC_VCID = {1}", noID, mpVcid);
                    if (result2.Count == 1)
                    {
                        string mcid = result2[0]["MC_ID"].ToString();
                        batch.Execute("update MEPeer set MP_TO_MC = {0}, MP_TO_Check = 1 where MP_ID = {1}", mcid, mpID);
                    }
                    else
                        batch.Execute("update MEPeer set MP_TO_Check = 1 where MP_ID = {0}", mpID);
                }

                result = batch.Commit();

                int resultMP = result.AffectedRows;

                #endregion

                if (resultMI == 0 && resultMIMI == 0 && resultPI == 0 && resultMITOPI == 0 && resultMP == 0 && resultMIMIAgg == 0)
                {
                    if (!stop)
                    {
                        idle = true;
                        Thread.Sleep(60000);
                        idle = false;
                    }
                }
                else
                {
                    Event("Commit: MI " + resultMI + ", MIMI " + resultMIMI + ", MIMIAGG " + resultMIMIAgg + ", PI " + resultPI + ", MITOPI " + resultMITOPI + ", MP " + resultMP);
                }
            }

            Event("Stopped");
        }
        
        private static InterfaceDescription[] ParseDescription(string description, string nodeName, Dictionary<string, string> nodes, List<string> nodeCandidates)
        {
            List<InterfaceDescription> idescs = new List<InterfaceDescription>();

            Database j = Necrow.JoviceDatabase;

            if (nodes.ContainsKey(nodeName))
            {
                string odesc = description.ToUpper();

                string thisType = nodes[nodeName][0] + "";

                int index = odesc.IndexOf(" EX ");
                if (index == -1) index = odesc.IndexOf("(EX ");
                if (index == -1) index = odesc.IndexOf("[EX ");
                if (index == -1) index = odesc.IndexOf(" EKS ");
                if (index == -1) index = odesc.IndexOf("(EKS ");
                if (index == -1) index = odesc.IndexOf(" EXS ");
                if (index == -1) index = odesc.IndexOf("(EXS ");
                if (index == -1) index = odesc.IndexOf("[EXS ");
                if (index > -1) odesc = odesc.Remove(index);

                List<Tuple<int, string>> ixs = new List<Tuple<int, string>>();

                List<string> found = new List<string>();

                foreach (KeyValuePair<string, string> pair in nodes)
                {
                    if (pair.Key != nodeName)
                    {
                        int ix = odesc.IndexOf(pair.Key);
                        if (ix > -1)
                        {
                            bool existsbefore = false;
                            foreach (string f in found)
                            {
                                if (f.IndexOf(pair.Key) > -1)
                                {
                                    existsbefore = true;
                                    break;
                                }
                            }

                            if (existsbefore == false)
                            {
                                found.Add(pair.Key);
                                ixs.Add(new Tuple<int, string>(ix, pair.Key));
                            }
                        }
                    }
                }

                if (ixs.Count == 0)
                {
                    foreach (string nodeCandidate in nodeCandidates)
                    {
                        int ix = odesc.IndexOf(nodeCandidate);
                        if (ix > -1)
                        {
                            InterfaceDescription idesc = new InterfaceDescription();
                            idesc.Ready = false;
                            idescs.Add(idesc);
                            break;
                        }
                    }
                }
                else
                {

                    ixs.Sort(delegate(Tuple<int, string> o1, Tuple<int, string> o2)
                    {
                        return o1.Item1.CompareTo(o2.Item1);
                    }
                    );

                    for (int ix = 0; ix < ixs.Count; ix++)
                    {
                        Tuple<int, string> ux = ixs[ix];
                        Tuple<int, string> nx = null;
                        if (ix < ixs.Count - 1) nx = ixs[ix + 1];

                        string span = null;
                        if (nx != null) span = odesc.Substring(ux.Item1, nx.Item1 - ux.Item1);
                        else span = odesc.Substring(ux.Item1);

                        string remoteName = ux.Item2;
                        string remoteValue = nodes[remoteName];
                        string remoteType = remoteValue[0] + "";
                        string remoteID = remoteValue.Substring(1);                        

                        string actualSpan = span.Substring(remoteName.Length);

                        if (thisType == "M" && remoteType == "P")
                        {
                            Result result = j.Query(@"
select PI_ID, PI_Name, LEN(PI_Name) as Len from PEInterface where PI_NO = {0}
and PI_Name not like '%.%'
order by Len desc", remoteID);

                            if (result.Count == 0)
                            {
                                // neighbor remote is not ready
                                InterfaceDescription idesc = new InterfaceDescription();

                                idesc.Ready = false;
                                idesc.NodeID = remoteID;
                                idesc.NodeName = remoteName;

                                idescs.Add(idesc);
                            }
                            else
                            {

                                foreach (Row row in result)
                                {
                                    string name = row["PI_Name"].ToString().ToUpper();
                                    string trailname = name.Substring(2);

                                    string referenceTo = null;

                                    List<string> testnames = new List<string>();
                                    testnames.Add(name);

                                    if (name.StartsWith("GI"))
                                    {
                                        testnames.Add("GI-" + trailname);
                                        testnames.Add("GE-" + trailname);
                                        testnames.Add("GIGABITETHERNET" + trailname);
                                        testnames.Add("GI " + trailname);
                                        testnames.Add("GE " + trailname);
                                        testnames.Add("GE" + trailname);
                                        testnames.Add("G" + trailname);
                                    }
                                    else if (name.StartsWith("TE"))
                                    {
                                        testnames.Add("TE-" + trailname);
                                        testnames.Add("TENGIGABITETHERNET" + trailname);
                                        testnames.Add("TE " + trailname);
                                        testnames.Add("T" + trailname);
                                    }
                                    else if (name.StartsWith("HU"))
                                    {
                                        testnames.Add("HU-" + trailname);
                                        testnames.Add("HUNDREDGIGABITETHERNET" + trailname);
                                        testnames.Add("HU " + trailname);
                                        testnames.Add("H" + trailname);
                                    }
                                    else if (name.StartsWith("FA"))
                                    {
                                        testnames.Add("FA-" + trailname);
                                        testnames.Add("FASTETHERNET" + trailname);
                                        testnames.Add("FA " + trailname);
                                        testnames.Add("FE-" + trailname);
                                        testnames.Add("FE" + trailname);
                                        testnames.Add("F" + trailname);
                                    }
                                    else if (name.StartsWith("ET"))
                                    {
                                        testnames.Add("ET-" + trailname);
                                        testnames.Add("ETHERNET" + trailname);
                                        testnames.Add("ET " + trailname);
                                        testnames.Add("ETH" + trailname);
                                        testnames.Add("ETH " + trailname);
                                        testnames.Add("ETH-" + trailname);
                                        testnames.Add("E" + trailname);
                                    }
                                    else if (name.StartsWith("SE"))
                                    {
                                        testnames.Add("SE-" + trailname);
                                        testnames.Add("SERIAL" + trailname);
                                        testnames.Add("SE " + trailname);
                                        testnames.Add("S" + trailname);
                                    }

                                    foreach (string testname in testnames)
                                    {
                                        if (actualSpan.IndexOf(testname) > -1)
                                        {
                                            referenceTo = row["PI_ID"].ToString();
                                            break;
                                        }
                                    }

                                    if (referenceTo != null)
                                    {
                                        InterfaceDescription idesc = new InterfaceDescription();

                                        idesc.InterfaceID = referenceTo;
                                        idesc.InterfaceName = name;
                                        idesc.NodeID = remoteID;
                                        idesc.NodeName = remoteName;

                                        idescs.Add(idesc);

                                        break;
                                    }
                                }
                            }
                        }
                        else if (thisType == "P" && remoteType == "M")
                        {
                            Result result = j.Query(@"
select MI_ID, MI_Name, LEN(MI_Name) as Len from MEInterface where MI_NO = {0}
and MI_Name not like '%.%'
order by Len desc", remoteID);

                            if (result.Count == 0)
                            {
                                // neighbor remote is not ready
                                InterfaceDescription idesc = new InterfaceDescription();

                                idesc.Ready = false;
                                idesc.NodeID = remoteID;
                                idesc.NodeName = remoteName;

                                idescs.Add(idesc);
                            }
                            else
                            {
                                foreach (Row row in result)
                                {
                                    string name = row["MI_Name"].ToString().ToUpper();
                                    string trailname = name.Substring(2);

                                    string referenceTo = null;

                                    List<string> testnames = new List<string>();



                                    if (name.StartsWith("EX"))
                                    {
                                        testnames.Add(trailname);
                                    }
                                    else
                                    {
                                        testnames.Add(name);

                                        if (name.StartsWith("GI"))
                                        {
                                            testnames.Add("GI-" + trailname);
                                            testnames.Add("GE-" + trailname);
                                            testnames.Add("GIGABITETHERNET" + trailname);
                                            testnames.Add("GI " + trailname);
                                            testnames.Add("GE " + trailname);
                                            testnames.Add("GE" + trailname);
                                            testnames.Add("G" + trailname);
                                        }
                                        else if (name.StartsWith("TE"))
                                        {
                                            testnames.Add("TE-" + trailname);
                                            testnames.Add("TENGIGABITETHERNET" + trailname);
                                            testnames.Add("TE " + trailname);
                                            testnames.Add("T" + trailname);
                                        }
                                        else if (name.StartsWith("HU"))
                                        {
                                            testnames.Add("HU-" + trailname);
                                            testnames.Add("HUNDREDGIGABITETHERNET" + trailname);
                                            testnames.Add("HU " + trailname);
                                        }
                                        else if (name.StartsWith("FA"))
                                        {
                                            testnames.Add("FA-" + trailname);
                                            testnames.Add("FASTETHERNET" + trailname);
                                            testnames.Add("FA " + trailname);
                                            testnames.Add("FE" + trailname);
                                            testnames.Add("F" + trailname);
                                        }
                                        else if (name.StartsWith("ET"))
                                        {
                                            testnames.Add("ET-" + trailname);
                                            testnames.Add("ETHERNET" + trailname);
                                            testnames.Add("ETH" + trailname);
                                            testnames.Add("ETH " + trailname);
                                            testnames.Add("ETH-" + trailname);
                                            testnames.Add("ET " + trailname);
                                        }
                                        else if (name.StartsWith("SE"))
                                        {
                                            testnames.Add("SE-" + trailname);
                                            testnames.Add("SERIAL" + trailname);
                                            testnames.Add("SE " + trailname);
                                            testnames.Add("S" + trailname);
                                        }
                                    }

                                    foreach (string testname in testnames)
                                    {
                                        if (actualSpan.IndexOf(testname) > -1)
                                        {
                                            referenceTo = row["MI_ID"].ToString();
                                            break;
                                        }
                                    }

                                    if (referenceTo != null)
                                    {
                                        InterfaceDescription idesc = new InterfaceDescription();

                                        idesc.InterfaceID = referenceTo;
                                        idesc.InterfaceName = name;
                                        idesc.NodeID = remoteID;
                                        idesc.NodeName = remoteName;

                                        idescs.Add(idesc);

                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return idescs.ToArray();
        }

        #endregion
    }
}
