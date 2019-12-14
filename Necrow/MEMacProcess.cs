using Aphysoft.Share;
using Jovice;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Necrow
{
    #region To Database
    
    class MEMacToDatabase : MacToDatabase
    {
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

    public sealed partial class Probe
    {
        private ProbeProcessResult MEMacProcess()
        {
            ProbeProcessResult probe = new ProbeProcessResult();

            string[] lines = null;
            Batch batch = j.Batch();
            Result2 result;

            #region MAC

            Event("Checking Mac");

            Dictionary<string, MEMacToDatabase> maclive = new Dictionary<string, MEMacToDatabase>();
            Dictionary<string, Row2> macdb = j.QueryDictionary("select * from MEMac where MA_NO = {0}", delegate (Row2 row) {
                string peerID = row["MA_MP"].ToString();
                string interfaceID = row["MA_MI"].ToString();
                if (peerID != null) return row["MA_MAC"].ToString() + "_" + peerID;
                else return row["MA_MAC"].ToString() + "_" + interfaceID;
            }, nodeID);
            List<MEMacToDatabase> macinsert = new List<MEMacToDatabase>();
            List<MEMacToDatabase> macupdate = new List<MEMacToDatabase>();
            
            // circuits
            Dictionary<string, Row2> circuitdb = null;
            if (nodeManufacture == alu) circuitdb = j.QueryDictionary("select * from MECircuit where MC_NO = {0}", "MC_VCID", nodeID);
            else if (nodeManufacture == hwe) circuitdb = j.QueryDictionary("select * from MECircuit where MC_NO = {0}", "MC_Description", nodeID);
            if (circuitdb == null) return DatabaseFailure(probe);

            // peers
            List<string> duplicatedpeers = new List<string>();
            Dictionary<string, Row2> peerdb = j.QueryDictionary("select * from MEPeer, MECircuit, MESDP where MP_MC = MC_ID and MP_MS = MS_ID and MC_NO = {0}", delegate (Row2 row) {
                return row["MS_SDP"].ToString() + ":" + row["MP_VCID"].ToString();
            }, delegate (Row2 row) { duplicatedpeers.Add(row["MP_ID"].ToString()); }, nodeID);
            if (peerdb == null) return DatabaseFailure(probe);
            if (duplicatedpeers.Count > 0)
            {
                Event(duplicatedpeers.Count + " peer-per-circuit(s) are found duplicated, began deleting...");
                string duplicatedpeerstr = "'" + string.Join("', '", duplicatedpeers.ToArray()) + "'";
                result = j.Execute("delete from MEPeer where MP_ID in (" + duplicatedpeerstr + ")");
                if (!result) return DatabaseFailure(probe);
                Event(result, EventActions.Delete, EventElements.Peer, true);
            }

            // interfaces
            List<string> duplicatedinterfaces = new List<string>();
            Dictionary<string, Row2> interfacedb = j.QueryDictionary("select * from MEInterface where MI_NO = {0}", "MI_Name", delegate (Row2 row) { duplicatedinterfaces.Add(row["MI_ID"].ToString()); }, nodeID);
            if (interfacedb == null) return DatabaseFailure(probe);
            if (duplicatedinterfaces.Count > 0)
            {
                Event(duplicatedinterfaces.Count + " interface(s) are found duplicated, began deleting...");
                string duplicatedinterfacestr = "'" + string.Join("', '", duplicatedinterfaces.ToArray()) + "'";
                result = j.Execute("update PEInterface set PI_TO_MI = NULL where PI_TO_MI in (" + duplicatedinterfacestr + ")");
                if (!result) return DatabaseFailure(probe);
                result = j.Execute("update MEInterface set MI_MI = NULL where MI_MI in (" + duplicatedinterfacestr + ")");
                if (!result) return DatabaseFailure(probe);
                result = j.Execute("delete from MEInterface where MI_ID in (" + duplicatedinterfacestr + ")");
                if (!result) return DatabaseFailure(probe);
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
                            MEMacToDatabase li = new MEMacToDatabase();

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
                                li.MacAddress = tokens[1];
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

                                maclive.Add(li.MacAddress + "_" + keydis, li);
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

                MEMacToDatabase current = null;

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
                                    current = new MEMacToDatabase();

                                    string[] addressSplit = token10.Split(new char[] { '-' });
                                    current.MacAddress = addressSplit[0].Substring(0, 2) + ":" + addressSplit[0].Substring(2, 2) + ":" +
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

                                                string key = current.MacAddress + "_" + current.PeerID;
                                                if (!maclive.ContainsKey(key) && current.CircuitID != null) maclive.Add(key, current);
                                            }
                                        }
                                        else if (current.InterfaceID != null)
                                        {
                                            string key = current.MacAddress + "_" + current.InterfaceID;
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

            foreach (KeyValuePair<string, MEMacToDatabase> pair in maclive)
            {
                MEMacToDatabase li = pair.Value;

                if (!macdb.ContainsKey(pair.Key))
                {
                    li.Id = Database2.ID();
                    macinsert.Add(li);
                }
                else
                {
                    Row2 db = macdb[pair.Key];

                    MEMacToDatabase u = new MEMacToDatabase();
                    u.Id = db["MA_ID"].ToString();
                    li.Id = u.Id;

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
            foreach (MEMacToDatabase s in macinsert)
            {
                Insert insert = j.Insert("MEMac");
                insert.Value("MA_ID", s.Id);
                insert.Value("MA_NO", nodeID);
                insert.Value("MA_MI", s.InterfaceID);
                insert.Value("MA_MP", s.PeerID);
                insert.Value("MA_MC", s.CircuitID);
                insert.Value("MA_MAC", s.MacAddress);
                insert.Value("MA_Age", s.Age);
                insert.Value("MA_LastChange", s.LastChange);
                batch.Add(insert);
            }
            result = batch.Commit();
            if (!result) return DatabaseFailure(probe);
            Event(result, EventActions.Add, EventElements.Mac, false);

            // UPDATE
            batch.Begin();
            foreach (MEMacToDatabase s in macupdate)
            {
                Update update = j.Update("MEMac");
                update.Set("MA_Age", s.Age, s.UpdateAge);
                update.Set("MA_LastChange", s.LastChange, s.UpdateLastChange);
                update.Where("MA_ID", s.Id);
                batch.Add(update);
            }
            result = batch.Commit();
            if (!result) return DatabaseFailure(probe);
            Event(result, EventActions.Update, EventElements.Mac, false);

            // DELETE
            batch.Begin();
            foreach (KeyValuePair<string, Row2> pair in macdb)
            {
                if (!maclive.ContainsKey(pair.Key))
                {
                    batch.Add("delete from MEMac where MA_ID = {0}", pair.Value["MA_ID"].ToString());
                }
            }
            result = batch.Commit();
            if (!result) return DatabaseFailure(probe);
            Event(result, EventActions.Delete, EventElements.Mac, false);

            #endregion

            #endregion
            
            return probe;
        }
    }
}
