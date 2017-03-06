using Aphysoft.Share;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Center
{
    #region To Database
    
    class MEMACToDatabase : ToDatabase
    {
        private string address;

        public string Address
        {
            get { return address; }
            set { address = value; }
        }

        private string circuitID;

        public string CircuitID
        {
            get { return circuitID; }
            set { circuitID = value; }
        }

        private string peerID;

        public string PeerID
        {
            get { return peerID; }
            set { peerID = value; }
        }

        private string interfaceID;

        public string InterfaceID
        {
            get { return interfaceID; }
            set { interfaceID = value; }
        }

        private int age;

        public int Age
        {
            get { return age; }
            set { age = value; }
        }

        private bool updateAge = false;

        public bool UpdateAge
        {
            get { return updateAge; }
            set { updateAge = value; }
        }

        private DateTime? lastChange;

        public DateTime? LastChange
        {
            get { return lastChange; }
            set { lastChange = value; }
        }

        private bool updateLastChange = false;

        public bool UpdateLastChange
        {
            get { return updateLastChange; }
            set { updateLastChange = value; }
        }

    }
    
    #endregion

    internal sealed partial class Probe
    {
        private ProbeProcessResult MEMacProcess()
        {
            ProbeProcessResult probe = new ProbeProcessResult();

            string[] lines = null;
            Batch batch = Batch();
            Result result;

            #region MAC

            Event("Checking Mac");

            Dictionary<string, MEMACToDatabase> maclive = new Dictionary<string, MEMACToDatabase>();
            Dictionary<string, Row> macdb = QueryDictionary("select * from MEMac where MA_NO = {0}", delegate (Row row) {
                string peerID = row["MA_MP"].ToString();
                string interfaceID = row["MA_MI"].ToString();
                if (peerID != null) return row["MA_MAC"].ToString() + "_" + peerID;
                else return row["MA_MAC"].ToString() + "_" + interfaceID;
            }, nodeID);
            List<MEMACToDatabase> macinsert = new List<MEMACToDatabase>();
            List<MEMACToDatabase> macupdate = new List<MEMACToDatabase>();
            
            // circuits
            Dictionary<string, Row> circuitdb = null;
            if (nodeManufacture == alu) circuitdb = QueryDictionary("select * from MECircuit where MC_NO = {0}", "MC_VCID", nodeID);
            else if (nodeManufacture == hwe) circuitdb = QueryDictionary("select * from MECircuit where MC_NO = {0}", "MC_Description", nodeID);
            if (circuitdb == null) return DatabaseFailure(probe);

            // peers
            List<string> duplicatedpeers = new List<string>();
            Dictionary<string, Row> peerdb = QueryDictionary("select * from MEPeer, MECircuit, MESDP where MP_MC = MC_ID and MP_MS = MS_ID and MC_NO = {0}", delegate (Row row) {
                return row["MS_SDP"].ToString() + ":" + row["MP_VCID"].ToString();
            }, delegate (Row row) { duplicatedpeers.Add(row["MP_ID"].ToString()); }, nodeID);
            if (peerdb == null) return DatabaseFailure(probe);
            if (duplicatedpeers.Count > 0)
            {
                Event(duplicatedpeers.Count + " peer-per-circuit(s) are found duplicated, began deleting...");
                string duplicatedpeerstr = "'" + string.Join("', '", duplicatedpeers.ToArray()) + "'";
                result = Execute("delete from MEPeer where MP_ID in (" + duplicatedpeerstr + ")");
                if (!result.OK) return DatabaseFailure(probe);
                Event(result, EventActions.Delete, EventElements.Peer, true);
            }

            // interfaces
            List<string> duplicatedinterfaces = new List<string>();
            Dictionary<string, Row> interfacedb = QueryDictionary("select * from MEInterface where MI_NO = {0}", "MI_Name", delegate (Row row) { duplicatedinterfaces.Add(row["MI_ID"].ToString()); }, nodeID);
            if (interfacedb == null) return DatabaseFailure(probe);
            if (duplicatedinterfaces.Count > 0)
            {
                Event(duplicatedinterfaces.Count + " interface(s) are found duplicated, began deleting...");
                string duplicatedinterfacestr = "'" + string.Join("', '", duplicatedinterfaces.ToArray()) + "'";
                result = Execute("update PEInterface set PI_TO_MI = NULL where PI_TO_MI in (" + duplicatedinterfacestr + ")");
                if (!result.OK) return DatabaseFailure(probe);
                result = Execute("update MEInterface set MI_MI = NULL where MI_MI in (" + duplicatedinterfacestr + ")");
                if (!result.OK) return DatabaseFailure(probe);
                result = Execute("delete from MEInterface where MI_ID in (" + duplicatedinterfacestr + ")");
                if (!result.OK) return DatabaseFailure(probe);
                Event(result, EventActions.Delete, EventElements.Interface, true);
            }

            #region Live

            if (nodeManufacture == alu)
            {
                #region alu

                if (Request("show service fdb-mac", out lines, probe)) return probe;

                bool start = false;

                foreach (string line in lines)
                {
                    string lineTrim = line.Trim();

                    if (!start && lineTrim.StartsWith("-------------------------------------------------")) start = true;
                    else
                    {
                        string[] tokens = lineTrim.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);

                        if (tokens.Length == 6)
                        {
                            MEMACToDatabase li = new MEMACToDatabase();

                            string circuitID = null;
                            string keydis = null;

                            if (tokens[2].StartsWith("sdp:"))
                            {
                                string sdpt = tokens[2].Substring(4);
                                if (peerdb.ContainsKey(sdpt))
                                {
                                    li.PeerID = peerdb[sdpt]["MP_ID"].ToString();
                                    keydis = li.PeerID;
                                    circuitID = peerdb[sdpt]["MP_MC"].ToString();
                                }
                            }
                            else if (tokens[2].StartsWith("sap:"))
                            {
                                string sapt = ConvertALUPort(tokens[2].Substring(4));
                                if (interfacedb.ContainsKey(sapt))
                                {
                                    li.InterfaceID = interfacedb[sapt]["MI_ID"].ToString();
                                    keydis = li.InterfaceID;
                                    circuitID = interfacedb[sapt]["MI_MC"].ToString();
                                }
                            }

                            if (circuitID != null)
                            {
                                li.Address = tokens[1];
                                li.CircuitID = circuitID;

                                if (tokens[3].Length >= 3)
                                {
                                    string[] typeAges = tokens[3].Split(new char[] { '/' });
                                    int age;
                                    if (int.TryParse(typeAges[1], out age)) li.Age = age;
                                }

                                DateTime dparse;
                                if (DateTime.TryParseExact(tokens[4] + " " + tokens[5], "MM/dd/yy HH:mm:ss", null, DateTimeStyles.None, out dparse))
                                    li.LastChange = dparse - nodeTimeOffset;

                                maclive.Add(li.Address + "_" + keydis, li);
                            }
                        }
                    }
                }

                #endregion
            }
            else if (nodeManufacture == hwe)
            {
                #region hwe

                if (Request("display mac-address dynamic verbose | in MAC Address:|Port       :|Peer IP    :|Aging time", out lines, probe)) return probe;
              
                //MAC Address: 4c16-f13d-5577     VLAN/VSI/SI   : IPTV_Unicast_1065
                //Port       : GE8/0/14.111       Type          : dynamic
                //Peer IP    : 172.31.70.1        VC-ID         : 1936653802
                //Aging time : 300                LSP/MAC_Tunnel: 1/0

                bool start = false;

                MEMACToDatabase current = null;

                foreach (string line in lines)
                {
                    string lineTrim = line.Trim();

                    if (!start && lineTrim.StartsWith("-------------------------------------------------")) start = true;
                    else
                    {
                        if (lineTrim.Length > 0)
                        {
                            string[] tokens = lineTrim.Split(new char[] { ':' });
                            if (tokens.Length == 3)
                            {
                                string token0 = tokens[0].Trim();
                                string token1 = tokens[1].Trim();

                                string[] midTokens = token1.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);
                                string token10 = midTokens[0].Trim();
                                string token11 = null;

                                if (midTokens.Length == 2) token11 = midTokens[1];

                                string token2 = tokens[2].Trim();

                                if (token0 == "MAC Address")
                                {
                                    current = new MEMACToDatabase();

                                    string[] addressSplit = token10.Split(new char[] { '-' });
                                    current.Address = addressSplit[0].Substring(0, 2) + ":" + addressSplit[0].Substring(2, 2) + ":" +
                                        addressSplit[1].Substring(0, 2) + ":" + addressSplit[1].Substring(2, 2) + ":" +
                                        addressSplit[2].Substring(0, 2) + ":" + addressSplit[2].Substring(2, 2);

                                    if (circuitdb.ContainsKey(token2))
                                    {
                                        current.CircuitID = circuitdb[token2]["MC_ID"].ToString();
                                    }
                                }
                                else if (current != null)
                                {
                                    if (token0 == "Port")
                                    {
                                        if (token10 != "-")
                                        {
                                            NetworkInterface nif = NetworkInterface.Parse(token10);

                                            if (nif != null)
                                            {
                                                string ifName = nif.Name;
                                                if (interfacedb.ContainsKey(ifName))
                                                {
                                                    current.InterfaceID = interfacedb[ifName]["MI_ID"].ToString();
                                                    //current.CircuitID = interfacelive[ifName].CircuitID;
                                                }
                                            }
                                        }
                                    }
                                    else if (token0 == "Peer IP")
                                    {
                                        if (token10 != "-")
                                        {
                                            string peerKey = token10 + ":" + token2;
                                            if (peerdb.ContainsKey(peerKey))
                                            {
                                                current.PeerID = peerdb[peerKey]["MP_ID"].ToString();
                                                //current.CircuitID = peerlive[peerKey].CircuitID;

                                                string key = current.Address + "_" + current.PeerID;
                                                if (!maclive.ContainsKey(key) && current.CircuitID != null) maclive.Add(key, current);
                                            }
                                        }
                                        else if (current.InterfaceID != null)
                                        {
                                            string key = current.Address + "_" + current.InterfaceID;
                                            if (!maclive.ContainsKey(key) && current.CircuitID != null) maclive.Add(key, current);
                                        }
                                    }
                                    else if (token0 == "Aging time")
                                    {
                                        int age;
                                        if (int.TryParse(token10, out age)) current.Age = age;
                                    }
                                }
                            }
                        }
                    }
                }

                #endregion
            }

            #endregion

            #region Check

            foreach (KeyValuePair<string, MEMACToDatabase> pair in maclive)
            {
                MEMACToDatabase li = pair.Value;

                if (!macdb.ContainsKey(pair.Key))
                {
                    li.ID = Database.ID();
                    macinsert.Add(li);
                }
                else
                {
                    Row db = macdb[pair.Key];

                    MEMACToDatabase u = new MEMACToDatabase();
                    u.ID = db["MA_ID"].ToString();
                    li.ID = u.ID;

                    bool update = false;
                    StringBuilder updateinfo = new StringBuilder();

                    if (db["MA_Age"].ToInt() != li.Age)
                    {
                        update = true;
                        u.UpdateAge = true;
                        u.Age = li.Age;
                    }
                    if (db["MA_LastChange"].ToNullableDateTime() != li.LastChange)
                    {
                        update = true;
                        u.UpdateLastChange = true;
                        u.LastChange = li.LastChange;
                    }

                    if (update)
                    {
                        macupdate.Add(u);
                    }
                }
            }

            #endregion

            #region Execute

            // ADD
            batch.Begin();
            foreach (MEMACToDatabase s in macinsert)
            {
                Insert insert = Insert("MEMac");
                insert.Value("MA_ID", s.ID);
                insert.Value("MA_NO", nodeID);
                insert.Value("MA_MI", s.InterfaceID);
                insert.Value("MA_MP", s.PeerID);
                insert.Value("MA_MC", s.CircuitID);
                insert.Value("MA_MAC", s.Address);
                insert.Value("MA_Age", s.Age);
                insert.Value("MA_LastChange", s.LastChange);
                batch.Execute(insert);
            }
            result = batch.Commit();
            if (!result.OK) return DatabaseFailure(probe);
            Event(result, EventActions.Add, EventElements.MAC, false);

            // UPDATE
            batch.Begin();
            foreach (MEMACToDatabase s in macupdate)
            {
                Update update = Update("MEMac");
                update.Set("MA_Age", s.Age, s.UpdateAge);
                update.Set("MA_LastChange", s.LastChange, s.UpdateLastChange);
                update.Where("MA_ID", s.ID);
                batch.Execute(update);
            }
            result = batch.Commit();
            if (!result.OK) return DatabaseFailure(probe);
            Event(result, EventActions.Update, EventElements.MAC, false);

            // DELETE
            batch.Begin();
            foreach (KeyValuePair<string, Row> pair in macdb)
            {
                if (!maclive.ContainsKey(pair.Key))
                {
                    batch.Execute("delete from MEMac where MA_ID = {0}", pair.Value["MA_ID"].ToString());
                }
            }
            result = batch.Commit();
            if (!result.OK) return DatabaseFailure(probe);
            Event(result, EventActions.Delete, EventElements.MAC, false);

            #endregion

            #endregion


            return probe;
        }
    }
}
