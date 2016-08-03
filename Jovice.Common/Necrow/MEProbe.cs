using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Aphysoft.Common;
using Aphysoft.Share;

namespace Jovice
{
    #region To Database

    class MECustomerToDatabase : ToDatabase
    {
        private string customerID;

        public string CustomerID
        {
            get { return customerID; }
            set { customerID = value; }
        }
    }

    class MEInterfaceToDatabase : InterfaceToDatabase
    {
        private MEInterfaceToDatabase[] aggrChilds = null;

        public MEInterfaceToDatabase[] AggrChilds
        {
            get { return aggrChilds; }
            set { aggrChilds = value; }
        }

        private string aggrAdjacentParentID = null;

        public string AggrAdjacentParentID
        {
            get { return aggrAdjacentParentID; }
            set { aggrAdjacentParentID = value; }
        }

        private string circuitID;

        public string CircuitID
        {
            get { return circuitID; }
            set { circuitID = value; }
        }

        private bool updateCircuit = false;

        public bool UpdateCircuit
        {
            get { return updateCircuit; }
            set { updateCircuit = value; }
        }

        private string interfaceType = null;

        public string InterfaceType
        {
            get { return interfaceType; }
            set { interfaceType = value; }
        }

        private bool updateInterfaceType = false;

        public bool UpdateInterfaceType
        {
            get { return updateInterfaceType; }
            set { updateInterfaceType = value; }
        }

        private string ingressID = null;

        public string IngressID
        {
            get { return ingressID; }
            set { ingressID = value; }
        }

        private bool updateIngressID = false;

        public bool UpdateIngressID
        {
            get { return updateIngressID; }
            set { updateIngressID = value; }
        }

        private string egressID = null;

        public string EgressID
        {
            get { return egressID; }
            set { egressID = value; }
        }

        private bool updateEgressID = false;

        public bool UpdateEgressID
        {
            get { return updateEgressID; }
            set { updateEgressID = value; }
        }

        private bool? used = null;

        public bool? Used
        {
            get { return used; }
            set { used = value; }
        }

        private bool updateUsed = false;

        public bool UpdateUsed
        {
            get { return updateUsed; }
            set { updateUsed = value; }
        }

        private string info = null;

        public string Info
        {
            get { return info; }
            set { info = value; }
        }

        private bool updateInfo = false;

        public bool UpdateInfo
        {
            get { return updateInfo; }
            set { updateInfo = value; }
        }

        private bool adjacentIDChecked = false;

        public bool AdjacentIDChecked
        {
            get { return adjacentIDChecked; }
            set { adjacentIDChecked = value; }
        }
    }

    class MEQOSToDatabase : StatusToDatabase
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private int type;

        /// <summary>
        /// 0: ingress 1: egress
        /// </summary>
        public int Type
        {
            get { return type; }
            set { type = value; }
        }

        private int bandwidth;

        public int Bandwidth
        {
            get { return bandwidth; }
            set { bandwidth = value; }
        }

        private bool updateBandwidth = false;

        public bool UpdateBandwidth
        {
            get { return updateBandwidth; }
            set { updateBandwidth = value; }
        }
    }

    class MESDPToDatabase : StatusToDatabase
    {
        private string sdp;

        public string SDP
        {
            get { return sdp; }
            set { sdp = value; }
        }

        private string farEnd;

        public string FarEnd
        {
            get { return farEnd; }
            set { farEnd = value; }
        }

        private bool updateFarEnd = false;

        public bool UpdateFarEnd
        {
            get { return updateFarEnd; }
            set { updateFarEnd = value; }
        }

        private string farEndNodeID;

        public string FarEndNodeID
        {
            get { return farEndNodeID; }
            set { farEndNodeID = value; }
        }

        private bool updateFarEndNodeID = false;

        public bool UpdateFarEndNodeID
        {
            get { return updateFarEndNodeID; }
            set { updateFarEndNodeID = value; }
        }

        private int admMTU;

        public int AdmMTU
        {
            get { return admMTU; }
            set { admMTU = value; }
        }

        private bool updateAdmMTU = false;

        public bool UpdateAdmMTU
        {
            get { return updateAdmMTU; }
            set { updateAdmMTU = value; }
        }

        private string type;

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        private bool updateType = false;

        public bool UpdateType
        {
            get { return updateType; }
            set { updateType = value; }
        }

        private string lsp;

        public string LSP
        {
            get { return lsp; }
            set { lsp = value; }
        }

        private bool updateLSP = false;

        public bool UpdateLSP
        {
            get { return updateLSP; }
            set { updateLSP = value; }
        }
    }

    class MECircuitToDatabase : ServiceBaseToDatabase
    {
        private string vcid;

        public string VCID
        {
            get { return vcid; }
            set { vcid = value; }
        }

        private string type;

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        private bool updateType = false;

        public bool UpdateType
        {
            get { return updateType; }
            set { updateType = value; }
        }

        private string customerID;

        public string CustomerID
        {
            get { return customerID; }
            set { customerID = value; }
        }

        private bool updateCustomer = false;

        public bool UpdateCustomer
        {
            get { return updateCustomer; }
            set { updateCustomer = value; }
        }

        private int admMTU;

        public int AdmMTU
        {
            get { return admMTU; }
            set { admMTU = value; }
        }

        private bool updateAdmMTU = false;

        public bool UpdateAdmMTU
        {
            get { return updateAdmMTU; }
            set { updateAdmMTU = value; }
        }

        private string description;

        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        private bool updateDescription = false;

        public bool UpdateDescription
        {
            get { return updateDescription; }
            set { updateDescription = value; }
        }

        private List<string> adjacentPeers = null;

        public List<string> AdjacentPeers
        {
            get { return adjacentPeers; }
            set { adjacentPeers = value; }
        }
    }

    class MEPeerToDatabase : StatusToDatabase
    {
        private string circuitID;

        public string CircuitID
        {
            get { return circuitID; }
            set { circuitID = value; }
        }

        private string sdpID;

        public string SDPID
        {
            get { return sdpID; }
            set { sdpID = value; }
        }

        private string sdp;

        public string SDP
        {
            get { return sdp; }
            set { sdp = value; }
        }

        private string vcid;

        public string VCID
        {
            get { return vcid; }
            set { vcid = value; }
        }

        private string type;

        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        private bool updateType = false;

        public bool UpdateType
        {
            get { return updateType; }
            set { updateType = value; }
        }

        private string toCircuitID;

        public string ToCircuitID
        {
            get { return toCircuitID; }
            set { toCircuitID = value; }
        }

        private bool updateToCircuitID = false;

        public bool UpdateToCircuitID
        {
            get { return updateToCircuitID; }
            set { updateToCircuitID = value; }
        }
    }

    #endregion

    internal sealed partial class Probe
    {
        private void MEProcess()
        {
            string[] lines = null;
            Batch batch = Batch();
            Result result;

            #region ALU-CUSTOMER

            Dictionary<string, MECustomerToDatabase> alucustlive = new Dictionary<string, MECustomerToDatabase>();
            Dictionary<string, Row> alucustdb = null;
            List<MECustomerToDatabase> alucustinsert = new List<MECustomerToDatabase>();
            List<MECustomerToDatabase> alucustupdate = new List<MECustomerToDatabase>();

            if (nodeManufacture == alu)
            {
                Event("Checking Circuit Customer");

                alucustdb = QueryDictionary("select * from MECustomer where MU_NO = {0}", "MU_UID", nodeID);

                #region Live

                if (Request("show service customer | match \"Customer-ID\"", out lines)) return;

                foreach (string line in lines)
                {
                    string[] linex = line.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                    if (linex.Length == 2 && linex[0].Trim() == "Customer-ID")
                    {
                        MECustomerToDatabase c = new MECustomerToDatabase();
                        c.CustomerID = linex[1].Trim();
                        alucustlive.Add(c.CustomerID, c);
                    }
                }

                #endregion

                #region Check

                foreach (KeyValuePair<string, MECustomerToDatabase> pair in alucustlive)
                {
                    MECustomerToDatabase li = pair.Value;
                    if (!alucustdb.ContainsKey(pair.Key))
                    {
                        Event("ALU-Customer ADD: " + li.CustomerID);
                        li.ID = Database.ID();
                        alucustinsert.Add(li);
                    }
                    else
                    {
                        Row db = alucustdb[pair.Key];

                        MECustomerToDatabase u = new MECustomerToDatabase();
                        u.ID = db["MU_ID"].ToString();

                        bool update = false;
                        StringBuilder updateinfo = new StringBuilder();

                        if (update)
                        {
                            Event("ALU-Customer UPDATE: " + pair.Key + " " + updateinfo.ToString());
                            alucustupdate.Add(u);
                        }
                    }
                }

                #endregion

                #region Execute

                // ADD
                batch.Begin();
                foreach (MECustomerToDatabase s in alucustinsert)
                {
                    Insert insert = Insert("MECustomer");
                    insert.Value("MU_ID", s.ID);
                    insert.Value("MU_NO", nodeID);
                    insert.Value("MU_UID", s.CustomerID);
                    batch.Execute(insert);
                }
                result = batch.Commit();
                Event(result, EventActions.Add, EventElements.ALUCustomer, false);

                // UPDATE
                batch.Begin();
                foreach (MECustomerToDatabase s in alucustupdate)
                {
                    //List<string> v = new List<string>();
                    // ...
                    //if (v.Count > 0) batch.Execute("update MECustomer set " + StringHelper.EscapeFormat(string.Join(",", v.ToArray())) + " where MU_ID = {0}", s.ID);
                }
                result = batch.Commit();
                Event(result, EventActions.Update, EventElements.ALUCustomer, false);

                #endregion

                alucustdb = QueryDictionary("select * from MECustomer where MU_NO = {0}", "MU_UID", nodeID);
            }

            #endregion

            #region QOS

            Event("Checking QOS");

            Dictionary<string, MEQOSToDatabase> qoslive = new Dictionary<string, MEQOSToDatabase>();
            //debug1:
            Dictionary<string, Row> qosdb = QueryDictionary("select * from MEQOS where MQ_NO = {0}", delegate (Row row) { return (row["MQ_Type"].ToBool() ? "1" : "0") + "_" + row["MQ_Name"].ToString(); }, nodeID);
            //goto debug2;
            List<MEQOSToDatabase> qosinsert = new List<MEQOSToDatabase>();
            List<MEQOSToDatabase> qosupdate = new List<MEQOSToDatabase>();
            
            #region Live

            if (nodeManufacture == alu)
            {
                #region alu

                if (Request("show qos sap-ingress", out lines)) return;

                foreach (string line in lines)
                {
                    if (line.Length > 0 && char.IsDigit(line[0]))
                    {
                        string[] linex = line.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);
                        string qos = linex[0];
                        MEQOSToDatabase q = new MEQOSToDatabase();
                        q.Name = qos;
                        q.Type = 0;
                        qoslive.Add("0_" + qos, q);
                    }
                }

                if (Request("show qos sap-egress", out lines)) return;

                foreach (string line in lines)
                {
                    if (line.Length > 0 && char.IsDigit(line[0]))
                    {
                        string[] linex = line.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);
                        string qos = linex[0];
                        MEQOSToDatabase q = new MEQOSToDatabase();
                        q.Name = qos;
                        q.Type = 1;
                        qoslive.Add("1_" + qos, q);
                    }
                }

                foreach (KeyValuePair<string, MEQOSToDatabase> pair in qoslive)
                {
                    MEQOSToDatabase li = pair.Value;
                    NodeQOSALU ni = NodeQOSALU.Parse(li.Name);
                    li.Bandwidth = ni.Bandwidth;
                }

                #endregion
            }
            else if (nodeManufacture == hwe)
            {
                #region hwe

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
                            string qosName = linex[0];
                            string qosIsUsed = linex[1];

                            MEQOSToDatabase q = new MEQOSToDatabase();
                            q.Name = qosName;
                            q.Type = 0;
                            qoslive.Add("0_" + qosName, q);
                        }
                    }
                }

                foreach (KeyValuePair<string, MEQOSToDatabase> pair in qoslive)
                {
                    MEQOSToDatabase li = pair.Value;
                    NodeQOSHWE ni = NodeQOSHWE.Parse(li.Name);
                    li.Bandwidth = ni.Bandwidth;
                }

                #endregion
            }

            #endregion

            #region Check

            foreach (KeyValuePair<string, MEQOSToDatabase> pair in qoslive)
            {
                MEQOSToDatabase li = pair.Value;
                if (!qosdb.ContainsKey(pair.Key))
                {
                    Event("QOS ADD: " + pair.Key + ((pair.Value.Bandwidth == -1) ? "" : ("(" + pair.Value.Bandwidth + "K)")));
                    li.ID = Database.ID();
                    qosinsert.Add(li);
                }
                else
                {
                    Row db = qosdb[pair.Key];

                    MEQOSToDatabase u = new MEQOSToDatabase();
                    u.ID = db["MQ_ID"].ToString();

                    bool update = false;
                    StringBuilder updateinfo = new StringBuilder();

                    int oldbw = -1;
                    if (!db["MQ_Bandwidth"].IsNull) oldbw = db["MQ_Bandwidth"].ToInt();
                    if (oldbw != li.Bandwidth)
                    {
                        update = true;
                        u.UpdateBandwidth = true;
                        u.Bandwidth = li.Bandwidth;
                        updateinfo.Append("bw" + ((li.Bandwidth == -1) ? "" : ("(" + li.Bandwidth + "K)")) + " ");
                    }
                    if (update)
                    {
                        Event("QOS UPDATE: " + pair.Key + " " + updateinfo.ToString());
                        qosupdate.Add(u);
                    }
                }
            }
            #endregion

            #region Execute

            // ADD
            batch.Begin();
            foreach (MEQOSToDatabase s in qosinsert)
            {
                Insert insert = Insert("MEQOS");
                insert.Value("MQ_ID", s.ID);
                insert.Value("MQ_NO", nodeID);
                insert.Value("MQ_Name", s.Name);
                insert.Value("MQ_Type", s.Type.Nullable(-1));
                insert.Value("MQ_Bandwidth", s.Bandwidth.Nullable(-1));
                batch.Execute(insert);
            }
            result = batch.Commit();
            Event(result, EventActions.Add, EventElements.QOS, false);

            // UPDATE
            batch.Begin();
            foreach (MEQOSToDatabase s in qosupdate)
            {
                Update update = Update("MEQOS");
                update.Set("MQ_Bandwidth", s.Bandwidth.Nullable(-1), s.UpdateBandwidth);
                update.Where("MQ_ID", s.ID);
                batch.Execute(update);
            }
            result = batch.Commit();
            Event(result, EventActions.Update, EventElements.QOS, false);

            #endregion

            qosdb = QueryDictionary("select * from MEQOS where MQ_NO = {0}", delegate (Row row) { return (row["MQ_Type"].ToBool() ? "1" : "0") + "_" + row["MQ_Name"].ToString(); }, nodeID);
            
            #endregion

            #region SDP

            Event("Checking SDP");
            
            Dictionary<string, MESDPToDatabase> sdplive = new Dictionary<string, MESDPToDatabase>();
            Dictionary<string, Row> sdpdb = QueryDictionary("select * from MESDP where MS_NO = {0}", "MS_SDP", nodeID);
            Dictionary<string, Row> ipnodedb = QueryDictionary("select NO_IP, NO_ID from Node where NO_IP is not null", "NO_IP");
            List<MESDPToDatabase> sdpinsert = new List<MESDPToDatabase>();
            List<MESDPToDatabase> sdpupdate = new List<MESDPToDatabase>();
            
            #region Live

            if (nodeManufacture == alu)
            {
                #region alu

                if (Request("show service sdp", out lines)) return;

                foreach (string line in lines)
                {
                    if (line.Length > 0)
                    {
                        if (char.IsDigit(line[0]))
                        {
                            string[] linex = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                            string sdp = linex[0];
                            string amtu = linex[1];
                            string omtu = linex[2];

                            string forth = linex[3];
                            string farend = null;
                            bool status, protocol;

                            string type = null;
                            string lsp = null;

                            if (forth == "Down")
                            {
                                status = false;
                                protocol = false;
                                type = linex[5].Trim()[0] + "";
                                lsp = linex[6].Trim()[0] + "";
                            }
                            else
                            {
                                farend = forth;
                                status = linex[4] == "Up" ? true : false;
                                protocol = linex[5] == "Up" ? true : false;
                                type = linex[6].Trim()[0] + "";
                                lsp = linex[7].Trim()[0] + "";
                            }

                            MESDPToDatabase d = new MESDPToDatabase();
                            d.SDP = sdp;
                            int iamtu;
                            if (int.TryParse(amtu, out iamtu)) d.AdmMTU = iamtu;
                            else d.AdmMTU = 0;
                            d.FarEnd = farend;
                            d.Status = status;
                            d.Protocol = protocol;
                            d.Type = type;
                            d.LSP = lsp;

                            sdplive.Add(sdp, d);
                        }
                    }
                }

                #endregion
            }
            else if (nodeManufacture == hwe)
            {
                #region hwe

                // dari mpls
                if (Request("display mpls ldp remote-peer", out lines)) return;

                string farend = null;
                bool active = false;

                foreach (string line in lines)
                {
                    if (line.Length > 0)
                    {
                        string lineTrim = line.Trim();

                        if (lineTrim.StartsWith("Remote Peer IP"))
                        {
                            string[] linex = lineTrim.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (linex.Length >= 5) farend = linex[4];
                        }
                        else if (farend != null)
                        {
                            if (lineTrim.IndexOf("Active") > -1) active = true;
                            else active = false;

                            MESDPToDatabase d = new MESDPToDatabase();
                            d.SDP = farend;
                            d.AdmMTU = 0;
                            d.FarEnd = farend;
                            d.Status = active;
                            d.Protocol = active;
                            d.Type = "M";
                            d.LSP = "L";

                            sdplive.Add(farend, d);
                            farend = null;
                        }
                    }
                }

                // dari vsi
                if (Request("display vsi verbose | in Peer Router ID", out lines)) return;

                foreach (string line in lines)
                {
                    string lineTrim = line.Trim();
                    if (lineTrim.Length > 0)
                    {
                        string[] linex = lineTrim.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        if (linex.Length == 2)
                        {
                            farend = linex[1].Trim();

                            if (!sdplive.ContainsKey(farend))
                            {
                                MESDPToDatabase d = new MESDPToDatabase();
                                d.SDP = farend;
                                d.AdmMTU = 0;
                                d.FarEnd = farend;
                                d.Status = true;
                                d.Protocol = true;
                                d.Type = "V";
                                d.LSP = "L";

                                sdplive.Add(farend, d);
                            }
                        }
                    }
                }

                // dari mpls
                //
                if (Request("display mpls l2vc | in destination", out lines)) return;

                foreach (string line in lines)
                {
                    string lineTrim = line.Trim();
                    if (lineTrim.Length > 0 && lineTrim.StartsWith("destination"))
                    {
                        string[] linex = lineTrim.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        if (linex.Length == 2)
                        {
                            farend = linex[1].Trim();

                            if (!sdplive.ContainsKey(farend))
                            {
                                MESDPToDatabase d = new MESDPToDatabase();
                                d.SDP = farend;
                                d.AdmMTU = 0;
                                d.FarEnd = farend;
                                d.Status = true;
                                d.Protocol = true;
                                d.Type = "E";
                                d.LSP = "L";

                                sdplive.Add(farend, d);
                            }
                        }
                    }
                }

                #endregion
            }

            #endregion
            
            #region Check

            foreach (KeyValuePair<string, MESDPToDatabase> pair in sdplive)
            {
                MESDPToDatabase li = pair.Value;

                if (li.FarEnd != null)
                {
                    if (ipnodedb.ContainsKey(li.FarEnd))
                        li.FarEndNodeID = ipnodedb[li.FarEnd]["NO_ID"].ToString();
                }

                if (!sdpdb.ContainsKey(pair.Key))
                {
                    Event("SDP ADD: " + pair.Key);
                    li.ID = Database.ID();
                    sdpinsert.Add(li);                    
                }
                else
                {
                    Row db = sdpdb[pair.Key];

                    MESDPToDatabase u = new MESDPToDatabase();
                    u.ID = db["MS_ID"].ToString();

                    bool update = false;
                    StringBuilder updateinfo = new StringBuilder();                    

                    if (db["MS_Status"].ToBool() != li.Status)
                    {
                        update = true;
                        u.UpdateStatus = true;
                        u.Status = li.Status;
                        updateinfo.Append("stat ");
                    }
                    if (db["MS_Protocol"].ToBool() != li.Protocol)
                    {
                        update = true;
                        u.UpdateProtocol = true;
                        u.Protocol = li.Protocol;
                        updateinfo.Append("prot ");
                    }
                    if (db["MS_Type"].ToString() != li.Type)
                    {
                        update = true;
                        u.UpdateType = true;
                        u.Type = li.Type;
                        updateinfo.Append("type ");
                    }
                    if (db["MS_LSP"].ToString() != li.LSP)
                    {
                        update = true;
                        u.UpdateLSP = true;
                        u.LSP = li.LSP;
                        updateinfo.Append("lsp ");
                    }
                    if ((db["MS_MTU"].IsNull ? 0 : db["MS_MTU"].ToShort()) != li.AdmMTU)
                    {
                        update = true;
                        u.UpdateAdmMTU = true;
                        u.AdmMTU = li.AdmMTU;
                        updateinfo.Append("mtu ");
                    }
                    if (db["MS_IP"].ToString() != li.FarEnd)
                    {
                        update = true;
                        u.UpdateFarEnd = true;
                        u.FarEnd = li.FarEnd;
                        updateinfo.Append("ip ");
                    }
                    if (db["MS_TO_NO"].ToString() != li.FarEndNodeID)
                    {
                        update = true;
                        u.UpdateFarEndNodeID = true;
                        u.FarEndNodeID = li.FarEndNodeID;
                        updateinfo.Append("iptono ");
                    }
                    if (update)
                    {
                        Event("SDP UPDATE: " + pair.Key + " " + updateinfo.ToString());
                        sdpupdate.Add(u);
                    }
                }
            }

            #endregion

            #region Execute

            // ADD
            batch.Begin();
            foreach (MESDPToDatabase s in sdpinsert)
            {
                Insert insert = Insert("MESDP");
                insert.Value("MS_ID", s.ID);
                insert.Value("MS_NO", nodeID);
                insert.Value("MS_SDP", s.SDP);
                insert.Value("MS_Status", s.Status);
                insert.Value("MS_Protocol", s.Protocol);
                insert.Value("MS_IP", s.FarEnd);
                insert.Value("MS_MTU", s.AdmMTU);
                insert.Value("MS_Type", s.Type);
                insert.Value("MS_LSP", s.LSP);
                insert.Value("MS_TO_NO", s.FarEndNodeID);
                batch.Execute(insert);
            }
            result = batch.Commit();
            Event(result, EventActions.Add, EventElements.SDP, false);

            // UPDATE
            batch.Begin();
            foreach (MESDPToDatabase s in sdpupdate)
            {
                Update update = Update("MESDP");
                update.Set("MS_Status", s.Status, s.UpdateStatus);
                update.Set("MS_Protocol", s.Protocol, s.UpdateProtocol);
                update.Set("MS_Type", s.Type, s.UpdateType);
                update.Set("MS_LSP", s.LSP, s.UpdateLSP);
                update.Set("MS_MTU", s.AdmMTU.Nullable(0), s.UpdateAdmMTU);
                update.Set("MS_IP", s.FarEnd, s.UpdateFarEnd);
                update.Set("MS_TO_NO", s.FarEndNodeID, s.UpdateFarEndNodeID);
                update.Where("MS_ID", s.ID);
                batch.Execute(update);
            }
            result = batch.Commit();
            Event(result, EventActions.Update, EventElements.SDP, false);

            #endregion

            sdpdb = QueryDictionary("select * from MESDP where MS_NO = {0}", "MS_SDP", nodeID);
            
            #endregion

            #region CIRCUIT

            Event("Checking Circuit");

            Dictionary<string, MECircuitToDatabase> circuitlive = new Dictionary<string, MECircuitToDatabase>();
            Dictionary<string, Row> circuitdb = null;
            List<MECircuitToDatabase> circuitinsert = new List<MECircuitToDatabase>();
            List<MECircuitToDatabase> circuitupdate = new List<MECircuitToDatabase>();
            List<string[]> hwecircuitdetail = null;
            ServiceReference circuitservicereference = new ServiceReference();
                               
            //circuitdb = QueryDictionary("select * from MECircuit where MC_NO = {0}", "MC_VCID", nodeID);
            //goto debug3;

            #region Live

            if (nodeManufacture == alu)
            {
                #region alu
                
                // PRESTEP, fix duplicated vcid in alu metro that might happen if probe fail sometime ago.
                result = Query("select MC_VCID from (select MC_VCID, COUNT(MC_VCID) as c from MECircuit where MC_NO = {0} group by MC_VCID) a where c >= 2", nodeID);
                if (result.Count > 0)
                {
                    Event(result.Count + " circuit(s) are found duplicated, began deleting...");
                    List<string> duplicatedvcids = new List<string>();
                    foreach (Row row in result)  duplicatedvcids.Add(row["MC_VCID"].ToString());
                    string duplicatedvcidstr = "'" + string.Join("', '", duplicatedvcids.ToArray()) + "'";
                    Execute("update MEInterface set MI_MC = NULL where MI_MC in (select MC_ID from MECircuit where MC_VCID in (" + duplicatedvcidstr + ") and MC_NO = {0})", nodeID);
                    Execute("update MEPeer set MP_TO_MC = NULL where MP_TO_MC in (select MC_ID from MECircuit where MC_VCID in (" + duplicatedvcidstr + ") and MC_NO = {0})", nodeID);
                    result = Execute("delete from MEPeer where MP_MC in (select MC_ID from MECircuit where MC_VCID in (" + duplicatedvcidstr + ") and MC_NO = {0})", nodeID);
                    Event(result.AffectedRows + " peer(s) have been deleted");
                    result = Execute("delete from MECircuit where MC_ID in (select MC_ID from MECircuit where MC_VCID in (" + duplicatedvcidstr + ") and MC_NO = {0})", nodeID);
                    Event(result.AffectedRows + " circuits(s) have been deleted");
                }
                
                circuitdb = QueryDictionary("select * from MECircuit where MC_NO = {0}", "MC_VCID", nodeID);

                //goto debug3;
                
                // STEP 1, dari display config untuk epipe dan vpls, biar dapet mtu dan deskripsinya
                if (Request("admin display-config | match customer context children | match \"epipe|vpls|description|service-mtu\" expression", out lines)) return;

                MECircuitToDatabase cservice = null;
                foreach (string line in lines)
                {
                    if (line == null) continue;
                    string oline = line.Trim();
                    if (oline == "") continue;
                    if (oline.StartsWith("epipe") || oline.StartsWith("vpls"))
                    {
                        if (cservice != null)
                        {
                            circuitlive.Add(cservice.VCID, cservice);
                            cservice = null;
                        }
                                                
                        string[] olinex = oline.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (olinex.Length >= 5 && alucustdb.ContainsKey(olinex[3]))
                        {
                            cservice = new MECircuitToDatabase();
                            cservice.Type = (olinex[0][0] + "").ToUpper();
                            cservice.VCID = olinex[1];
                            cservice.CustomerID = alucustdb[olinex[3]]["MU_ID"].ToString();
                        }
                    }
                    else if (cservice != null)
                    {
                        if (oline.StartsWith("description"))
                        {
                            string desc = oline.Substring(11).Trim();
                            if (desc.StartsWith("\"")) desc = desc.Substring(1);
                            if (desc.EndsWith("\"")) desc = desc.Substring(0, desc.Length - 1);
                            cservice.Description = desc;
                        }
                        if (oline.StartsWith("service-mtu"))
                        {
                            string mtu = oline.Substring(11).Trim();
                            int amtu;
                            if (int.TryParse(mtu, out amtu)) cservice.AdmMTU = amtu;
                            else cservice.AdmMTU = 0;
                        }
                    }
                }
                if (cservice != null)
                {
                    circuitlive.Add(cservice.VCID, cservice);
                    cservice = null;
                }

                // STEP 2, dari service-using, sisanya
                if (Request("show service service-using", out lines)) return;

                foreach (string line in lines)
                {
                    if (line.Length > 0 && char.IsDigit(line[0]))
                    {
                        string[] linex = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        if (linex.Length == 5)
                        {
                            MECircuitToDatabase service = null;
                            if (circuitlive.ContainsKey(linex[0])) service = circuitlive[linex[0]];
                            else
                            {
                                service = new MECircuitToDatabase();
                                service.VCID = linex[0];
                                service.Type = (linex[1][0] + "").ToUpper();
                                if (alucustdb.ContainsKey(linex[4])) service.CustomerID = alucustdb[linex[4]]["MU_ID"].ToString();
                                else service.CustomerID = null;
                                circuitlive.Add(linex[0], service);
                            }
                            service.Status = linex[2] == "Up";
                            service.Protocol = linex[3] == "Up";
                        }
                    }
                }
                #endregion
            }
            else if (nodeManufacture == hwe)
            {
                #region hwe
                
                circuitdb = QueryDictionary("select * from MECircuit where MC_NO = {0}", "MC_Description", nodeID);
                
                // display vsi verbose | in VSI
                // display mpls l2vc brief

                // STEP 1, VSI Name dan VSI ID
                if (Request("display vsi verbose | in VSI Name|VSI ID", out lines)) return;

                MECircuitToDatabase cservice = null;
                foreach (string line in lines)
                {
                    string lineTrim = line.Trim();
                    if (lineTrim.StartsWith("***VSI Name"))
                    {
                        if (cservice != null)
                        {
                            circuitlive.Add(cservice.Description, cservice);
                            cservice = null;
                        }

                        //***VSI Name               : IPTV_Unicast_1098
                        //01234567890123456789012345678901234567890
                        //          1         2    2

                        if (lineTrim.Length >= 29)
                        {
                            cservice = new MECircuitToDatabase();
                            cservice.Description = lineTrim.Substring(28).Trim();
                            cservice.Type = "V";
                            cservice.Status = true;
                            cservice.Protocol = true;
                        }
                    }
                    else if (lineTrim.StartsWith("VSI ID"))
                    {
                        if (cservice != null)
                        {
                            string[] linex = lineTrim.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                            if (linex.Length >= 2) cservice.VCID = linex[1].Trim();
                        }
                    }
                }
                if (cservice != null)
                {
                    circuitlive.Add(cservice.Description, cservice);
                    cservice = null;
                }

                // STEP 2, VSI Name and VSI Detail
                if (Request("display vsi", out lines)) return;

                foreach (string line in lines)
                {
                    if (line.Length >= 33)
                    {
                        string[] linex = line.Substring(32).Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        if (linex.Length == 6 && !(linex[0] == "Mem" && linex[1] == "PW") && !(linex[0] == "Disc" && linex[1] == "Type"))
                        {
                            string vsiName = line.Substring(0, 31).Trim();

                            if (circuitlive.ContainsKey(vsiName))
                            {
                                MECircuitToDatabase cu = circuitlive[vsiName];
                                cu.CustomerID = null;
                                bool state = linex[5] == "up";
                                cu.Status = state;
                                cu.Protocol = state;
                                int amtu;
                                if (int.TryParse(linex[4], out amtu)) cu.AdmMTU = amtu;
                                else cu.AdmMTU = 0;
                            }
                        }
                    }
                }

                // STEP 3, dari MPLS L2VC
                hwecircuitdetail = new List<string[]>();

                //display mpls l2vc | in client interface|VC ID|local VC MTU|destination
                if (Request("display mpls l2vc | in client interface|VC ID|local VC MTU|destination", out lines)) return;

                string cinterface = null;
                string cinterfaceVCID = null;
                string cinterfaceSDP = null;
                bool cinterfacestate = false;
                int cmtu = 0;

                foreach (string line in lines)
                {
                    string lineTrim = line.Trim();

                    if (lineTrim.Length > 0)
                    {
                        string[] linex = lineTrim.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                        if (linex.Length >= 2)
                        {
                            string lineKey = linex[0].Trim();
                            string lineValue = linex[1].Trim();

                            if (lineKey == "*client interface")
                            {
                                if (cinterface != null)
                                {
                                    string vcidname = cinterfaceVCID + "_GROUP";

                                    if (!circuitlive.ContainsKey(vcidname))
                                    {
                                        MECircuitToDatabase cu = new MECircuitToDatabase();
                                        cu.Type = "E";
                                        cu.Description = vcidname;
                                        cu.VCID = cinterfaceVCID;
                                        cu.CustomerID = null;
                                        cu.Status = cinterfacestate;
                                        cu.Protocol = cinterfacestate;
                                        cu.AdmMTU = cmtu;

                                        circuitlive.Add(vcidname, cu);
                                    }

                                    hwecircuitdetail.Add(new string[] { cinterface, cinterfaceSDP, cinterfacestate.ToString(), vcidname, cinterfaceVCID });

                                    cinterface = null;
                                    cinterfaceVCID = null;
                                    cinterfaceSDP = null;
                                    cinterfacestate = false;
                                    cmtu = 0;
                                }

                                if (lineValue.Length > 0)
                                {
                                    string[] linex2 = lineValue.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                    if (linex2.Length >= 3)
                                    {
                                        if (linex2[0].StartsWith("Eth-Trunk")) cinterface = "Ag" + linex2[0].Substring(9);
                                        else
                                        {
                                            NodeInterface inf = NodeInterface.Parse(linex2[0]);
                                            if (inf != null) cinterface = inf.GetShort();
                                        }
                                        if (linex2[2] == "up") cinterfacestate = true;
                                    }
                                }
                            }
                            else if (lineKey == "VC ID") cinterfaceVCID = lineValue;
                            else if (lineKey == "destination") cinterfaceSDP = lineValue;
                            else if (lineKey == "local VC MTU")
                            {
                                if (lineValue.Length > 0)
                                {
                                    string[] linex2 = lineValue.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                    int amtu;
                                    if (int.TryParse(linex2[0], out amtu)) cmtu = amtu;
                                    else cmtu = 0;
                                }
                            }
                        }
                    }
                }
                if (cinterface != null)
                {
                    string vcidname = cinterfaceVCID + "_GROUP";

                    if (!circuitlive.ContainsKey(vcidname))
                    {
                        MECircuitToDatabase cu = new MECircuitToDatabase();
                        cu.Type = "E";
                        cu.Description = vcidname;
                        cu.VCID = cinterfaceVCID;
                        cu.CustomerID = null;
                        cu.Status = cinterfacestate;
                        cu.Protocol = cinterfacestate;
                        cu.AdmMTU = cmtu;

                        circuitlive.Add(vcidname, cu);
                    }

                    hwecircuitdetail.Add(new string[] { cinterface, cinterfaceSDP, cinterfacestate.ToString(), vcidname, cinterfaceVCID });
                }

                #endregion
            }

            #endregion
            
            #region Check
            
            Dictionary<string, List<string>> adjacentPeers = null;
            foreach (KeyValuePair<string, MECircuitToDatabase> pair in circuitlive)
            {
                MECircuitToDatabase li = pair.Value;

                if (!circuitdb.ContainsKey(pair.Key))
                {
                    if (adjacentPeers == null)
                    {
                        result = Query("select MP_VCID, MP_ID from MEPeer, MESDP, Node where MP_MS = MS_ID and MS_IP = NO_IP and NO_ID = {0}", nodeID);
                        adjacentPeers = new Dictionary<string, List<string>>();
                        if (result.OK)
                        {
                            foreach (Row row in result)
                            {
                                string vcid = row["MP_VCID"].ToString();
                                string mp = row["MP_ID"].ToString();

                                if (vcid != null)
                                {
                                    if (adjacentPeers.ContainsKey(vcid)) adjacentPeers[vcid].Add(mp);
                                    else
                                    {
                                        List<string> mps = new List<string>();
                                        mps.Add(mp);
                                        adjacentPeers[vcid] = mps;
                                    }
                                }
                            }
                        }
                    }

                    Event("Circuit ADD: " + pair.Key);
                    li.ID = Database.ID();
                    if (li.Description != null) circuitservicereference.Add(li, li.Description);
                    if (li.VCID != null)
                    {
                        if (adjacentPeers.ContainsKey(li.VCID))
                            li.AdjacentPeers = adjacentPeers[li.VCID];
                    }
                    else li.AdjacentPeers = new List<string>();
                    circuitinsert.Add(li);
                }
                else
                {
                    Row db = circuitdb[pair.Key];

                    MECircuitToDatabase u = new MECircuitToDatabase();
                    u.ID = db["MC_ID"].ToString();

                    bool update = false;
                    StringBuilder updateinfo = new StringBuilder();

                    if (db["MC_Status"].ToBool() != li.Status)
                    {
                        update = true;
                        u.UpdateStatus = true;
                        u.Status = li.Status;
                        updateinfo.Append("stat ");
                    }
                    if (db["MC_Protocol"].ToBool() != li.Protocol)
                    {
                        update = true;
                        u.UpdateProtocol = true;
                        u.Protocol = li.Protocol;
                        updateinfo.Append("prot ");
                    }
                    if (db["MC_Type"].ToString() != li.Type)
                    {
                        update = true;
                        u.UpdateType = true;
                        u.Type = li.Type;
                        updateinfo.Append("type ");
                    }
                    if (db["MC_MU"].ToString() != li.CustomerID)
                    {
                        update = true;
                        u.UpdateCustomer = true;
                        u.CustomerID = li.CustomerID;
                        updateinfo.Append("cust ");
                    }
                    if (db["MC_Description"].ToString() != li.Description)
                    {
                        update = true;
                        u.UpdateDescription = true;
                        u.Description = li.Description;
                        updateinfo.Append("desc ");

                        u.ServiceID = null;

                        if (u.Description != null) circuitservicereference.Add(u, u.Description);
                    }
                    if ((db["MC_MTU"].IsNull ? 0 : db["MC_MTU"].ToShort()) != li.AdmMTU)
                    {
                        update = true;
                        u.UpdateAdmMTU = true;
                        u.AdmMTU = li.AdmMTU;
                        updateinfo.Append("mtu ");
                    }
                    if (update)
                    {
                        Event("Circuit UPDATE: " + pair.Key + " " + updateinfo.ToString());
                        circuitupdate.Add(u);
                    }
                }
            }

            #endregion

            #region Execute

            // SERVICE REFERENCE
            ServiceExecute(circuitservicereference);

            // ADD
            batch.Begin();
            foreach (MECircuitToDatabase s in circuitinsert)
            {
                Insert insert = Insert("MECircuit");
                insert.Value("MC_ID", s.ID);
                insert.Value("MC_NO", nodeID);
                insert.Value("MC_VCID", s.VCID);
                insert.Value("MC_Type", s.Type);
                insert.Value("MC_Status", s.Status);
                insert.Value("MC_Protocol", s.Protocol);
                insert.Value("MC_MU", s.CustomerID);
                insert.Value("MC_Description", s.Description);
                insert.Value("MC_MTU", s.AdmMTU.Nullable(0));
                insert.Value("MC_SE", s.ServiceID);
                batch.Execute(insert);
            }
            result = batch.Commit();
            Event(result, EventActions.Add, EventElements.Circuit, false);

            batch.Begin();
            foreach (MECircuitToDatabase s in circuitinsert)
            {
                if (s.AdjacentPeers != null)
                {
                    string[] peers = s.AdjacentPeers.ToArray();
                    foreach (string peer in peers) batch.Execute("update MEPeer set MP_TO_MC = {0} where MP_ID = {1}", s.ID, peer);
                }
            }
            result = batch.Commit();
            Event(result, EventActions.Update, EventElements.CircuitReference, false);

            // UPDATE
            batch.Begin();
            foreach (MECircuitToDatabase s in circuitupdate)
            {
                Update update = Update("MECircuit");
                update.Set("MC_Status", s.Status, s.UpdateStatus);
                update.Set("MC_Protocol", s.Protocol, s.UpdateProtocol);
                update.Set("MC_Type", s.Type, s.UpdateType);
                if (s.UpdateDescription)
                {
                    update.Set("MC_Description", s.Description);
                    update.Set("MC_SE", s.ServiceID);
                }
                update.Set("MC_MTU", s.AdmMTU.Nullable(0), s.UpdateAdmMTU);
                update.Set("MC_MU", s.CustomerID, s.UpdateCustomer);
                update.Where("MC_ID", s.ID);
                batch.Execute(update);
            }
            result = batch.Commit();
            Event(result, EventActions.Update, EventElements.Circuit, false);

            #endregion

            if (nodeManufacture == alu) circuitdb = QueryDictionary("select * from MECircuit where MC_NO = {0}", "MC_VCID", nodeID);
            else if (nodeManufacture == hwe) circuitdb = QueryDictionary("select * from MECircuit where MC_NO = {0}", "MC_Description", nodeID);
            
            #endregion

            #region PEER

            Event("Checking Peer");
            
            Dictionary<string, MEPeerToDatabase> peerlive = new Dictionary<string, MEPeerToDatabase>();
            List<string> duplicatedpeers = new List<string>();
            Dictionary<string, Row> peerdb = QueryDictionary("select * from MEPeer, MECircuit, MESDP where MP_MC = MC_ID and MP_MS = MS_ID and MC_NO = {0}", delegate (Row row) { return row["MS_SDP"].ToString() + ":" + row["MP_VCID"].ToString(); }, delegate (Row row) { duplicatedpeers.Add(row["MP_ID"].ToString()); }, nodeID);
            List<MEPeerToDatabase> peerinsert = new List<MEPeerToDatabase>();
            List<MEPeerToDatabase> peerupdate = new List<MEPeerToDatabase>();

            if (duplicatedpeers.Count > 0)
            {
                Event(duplicatedpeers.Count + " peer-per-circuit(s) are found duplicated, began deleting...");
                string duplicatedpeerstr = "'" + string.Join("', '", duplicatedpeers.ToArray()) + "'";
                result = Execute("delete from MEPeer where MP_ID in (" + duplicatedpeerstr + ")");
                Event(result, EventActions.Delete, EventElements.Peer, true);
            }
            
            #region Live

            if (nodeManufacture == alu)
            {
                #region alu

                if (Request("show service sdp-using", out lines)) return;

                foreach (string line in lines)
                {
                    if (line != null)
                    {
                        if (line.Length > 0)
                        {
                            if (char.IsDigit(line[0]))
                            {
                                string[] linex = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                                if (linex.Length > 1)
                                {
                                    MEPeerToDatabase c = new MEPeerToDatabase();

                                    if (circuitdb.ContainsKey(linex[0])) c.CircuitID = circuitdb[linex[0]]["MC_ID"].ToString();
                                    else c.CircuitID = null;

                                    string[] sdpvcid = linex[1].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                                    if (sdpvcid.Length > 1)
                                    {
                                        c.SDP = sdpvcid[0];
                                        c.VCID = sdpvcid[1];

                                        if (sdpdb.ContainsKey(sdpvcid[0])) c.SDPID = sdpdb[c.SDP]["MS_ID"].ToString();
                                        else c.SDPID = null;

                                        c.Type = linex[2][0] + "";
                                        c.Protocol = linex[4] == "Up";

                                        if (c.CircuitID != null && c.SDPID != null)
                                        {
                                            string keyn = c.SDP + ":" + c.VCID;
                                            if (!peerlive.ContainsKey(keyn))
                                                peerlive.Add(keyn, c);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                #endregion
            }
            else if (nodeManufacture == hwe)
            {
                #region hwe

                // peernya vsi
                if (Request("display vsi peer-info", out lines)) return;

                string cvsi = null;
                foreach (string line in lines)
                {
                    if (line != null && line.Length > 0)
                    {
                        if (line.StartsWith("VSI Name: "))
                        {
                            string vsiname = line.Substring(10, 31).Trim();

                            if (circuitdb.ContainsKey(vsiname)) cvsi = circuitdb[vsiname]["MC_ID"].ToString();
                            else cvsi = null;
                        }
                        else if (char.IsDigit(line[0]) && cvsi != null)
                        {
                            string[] linex = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                            if (linex.Length == 5)
                            {
                                MEPeerToDatabase c = new MEPeerToDatabase();
                                c.CircuitID = cvsi;

                                c.SDP = linex[0];
                                c.VCID = linex[1];

                                if (sdpdb.ContainsKey(linex[0])) c.SDPID = sdpdb[c.SDP]["MS_ID"].ToString();
                                else c.SDPID = null;

                                c.Type = "M";
                                c.Protocol = linex[4] == "up";

                                if (c.CircuitID != null && c.SDPID != null)
                                {
                                    string keyn = c.SDP + ":" + c.VCID;
                                    if (!peerlive.ContainsKey(keyn))
                                        peerlive.Add(keyn, c);
                                }
                            }
                        }
                    }
                }

                // peernya mpls l2vc
                foreach (string[] strs in hwecircuitdetail)
                {
                    // cinterface, cinterfaceSDP, cinterfacestate.ToString(), vcidname, cinterfaceVCID
                    //  0            1              2                          3         4

                    string vcidname = strs[3];
                    string vcid = strs[4];
                    string sdp = strs[1];

                    //string[] strs = pair.Value;

                    if (circuitdb.ContainsKey(vcidname))
                    {
                        string cid = circuitdb[vcidname]["MC_ID"].ToString();
                        MEPeerToDatabase c = new MEPeerToDatabase();
                        c.CircuitID = cid;

                        c.SDP = sdp;
                        c.VCID = vcid;

                        if (sdpdb.ContainsKey(sdp)) c.SDPID = sdpdb[sdp]["MS_ID"].ToString();
                        else c.SDPID = null;

                        c.Type = "S";
                        c.Protocol = strs[2] == "True";

                        if (c.CircuitID != null && c.SDPID != null)
                        {
                            string keyn = c.SDP + ":" + c.VCID;
                            if (!peerlive.ContainsKey(keyn))
                                peerlive.Add(keyn, c);
                        }
                    }
                }

                #endregion
            }

            #endregion
            
            #region Check

            foreach (KeyValuePair<string, MEPeerToDatabase> pair in peerlive)
            {
                MEPeerToDatabase li = pair.Value;

                result = Query("select MC_ID from MESDP, MECircuit where MS_TO_NO = MC_NO and MS_ID = {0} and MC_VCID = {1}", li.SDPID, li.VCID);
                if (result.OK && result.Count > 0) li.ToCircuitID = result[0]["MC_ID"].ToString();

                if (!peerdb.ContainsKey(pair.Key))
                {
                    Event("Peer ADD: " + pair.Key);
                    li.ID = Database.ID();
                    peerinsert.Add(li);
                }
                else
                {
                    Row db = peerdb[pair.Key];

                    MEPeerToDatabase u = new MEPeerToDatabase();
                    u.ID = db["MP_ID"].ToString();

                    bool update = false;
                    StringBuilder updateinfo = new StringBuilder();

                    if (db["MP_Protocol"].ToBool() != li.Protocol)
                    {
                        update = true;
                        u.UpdateProtocol = true;
                        u.Protocol = li.Protocol;
                        updateinfo.Append("prot ");
                    }
                    if (db["MP_Type"].ToString() != li.Type)
                    {
                        update = true;
                        u.UpdateType = true;
                        u.Type = li.Type;
                        updateinfo.Append("type ");
                    }
                    if (db["MP_TO_MC"].ToString() != li.ToCircuitID)
                    {
                        update = true;
                        u.UpdateToCircuitID = true;
                        u.ToCircuitID = li.ToCircuitID;
                        updateinfo.Append("ref ");
                    }
                    if (update)
                    {
                        Event("Peer UPDATE: " + pair.Key + " " + updateinfo.ToString());
                        peerupdate.Add(u);
                    }
                }
            }

            #endregion

            #region Execute

            // ADD
            batch.Begin();
            foreach (MEPeerToDatabase s in peerinsert)
            {
                Insert insert = Insert("MEPeer");
                insert.Value("MP_ID", s.ID);
                insert.Value("MP_MC", s.CircuitID);
                insert.Value("MP_MS", s.SDPID);
                insert.Value("MP_VCID", s.VCID);
                insert.Value("MP_Protocol", s.Protocol);
                insert.Value("MP_Type", s.Type);
                insert.Value("MP_TO_MC", s.ToCircuitID);
                batch.Execute(insert);
            }
            result = batch.Commit();
            Event(result, EventActions.Add, EventElements.Peer, false);

            // UPDATE
            batch.Begin();
            foreach (MEPeerToDatabase s in peerupdate)
            {
                Update update = Update("MEPeer");
                update.Set("MP_Protocol", s.Protocol, s.UpdateProtocol);
                update.Set("MP_Type", s.Type, s.UpdateType);
                update.Set("MP_TO_MC", s.ToCircuitID, s.UpdateToCircuitID);
                update.Where("MP_ID", s.ID);
                batch.Execute(update);
            }
            result = batch.Commit();
            Event(result, EventActions.Update, EventElements.Peer, false);

            // DELETE
            batch.Begin();
            foreach (KeyValuePair<string, Row> pair in peerdb)
            {
                if (!peerlive.ContainsKey(pair.Key) || pair.Value["MP_MC"].ToString() != peerlive[pair.Key].CircuitID)
                {
                    Event("Peer DELETE: " + pair.Key);
                    batch.Execute("delete from MEPeer where MP_ID = {0}", pair.Value["MP_ID"].ToString());                    
                }
            }
            result = batch.Commit();
            Event(result, EventActions.Delete, EventElements.Peer, false);

            #endregion

            #endregion

            #region INTERFACE

            Event("Checking Interface");
            
            SortedDictionary<string, MEInterfaceToDatabase> interfacelive = new SortedDictionary<string, MEInterfaceToDatabase>();
            List<string> duplicatedinterfaces = new List<string>();
            Dictionary<string, Row> interfacedb = QueryDictionary("select * from MEInterface where MI_NO = {0}", "MI_Name", delegate(Row row) { duplicatedinterfaces.Add(row["MI_ID"].ToString()); }, nodeID);
            SortedDictionary<string, MEInterfaceToDatabase> interfaceinsert = new SortedDictionary<string, MEInterfaceToDatabase>();
            List<MEInterfaceToDatabase> interfaceupdate = new List<MEInterfaceToDatabase>();

            if (duplicatedinterfaces.Count > 0)
            {
                Event(duplicatedinterfaces.Count + " interface(s) are found duplicated, began deleting...");
                string duplicatedinterfacestr = "'" + string.Join("', '", duplicatedinterfaces.ToArray()) + "'";
                Execute("update PEInterface set PI_TO_MI = NULL where PI_TO_MI in (" + duplicatedinterfacestr + ")");
                Execute("update MEInterface set MI_MI = NULL where MI_MI in (" + duplicatedinterfacestr + ")");
                result = Execute("delete from MEInterface where MI_ID in (" + duplicatedinterfacestr + ")");
                Event(result, EventActions.Delete, EventElements.Interface, true);
            }

            ServiceReference interfaceservicereference = new ServiceReference();
            
            #region Live

            if (nodeManufacture == alu)
            {
                #region alu
                if (Request("show port description", out lines)) return;

                string port = null;
                StringBuilder description = new StringBuilder();
                foreach (string line in lines)
                {
                    if (line.Length > 0)
                    {
                        if (char.IsDigit(line[0]))
                        {
                            if (port != null)
                            {
                                if (!interfacelive.ContainsKey(port))
                                {
                                    MEInterfaceToDatabase mid = new MEInterfaceToDatabase();
                                    mid.Name = port;
                                    mid.Description = description.ToString();
                                    interfacelive.Add(port, mid);
                                }
                            }

                            int fipo = line.IndexOf(' ');
                            port = "Ex" + line.Substring(0, fipo);
                            port = port.Replace('.', ':');
                            description.Clear();
                            description.Append(line.Substring(fipo + 1).TrimStart());
                        }
                        else if (line[0] == ' ')
                        {
                            if (port != null) description.Append(line.TrimStart());
                        }
                        else
                        {
                            if (port != null)
                            {
                                if (!interfacelive.ContainsKey(port))
                                {
                                    MEInterfaceToDatabase mid = new MEInterfaceToDatabase();
                                    mid.Name = port;
                                    mid.Description = description.ToString();
                                    interfacelive.Add(port, mid);
                                }
                                port = null;
                            }
                        }
                    }
                }
                if (port != null)
                {
                    if (!interfacelive.ContainsKey(port))
                    {
                        MEInterfaceToDatabase mid = new MEInterfaceToDatabase();
                        mid.Name = port;
                        mid.Description = description.ToString();

                        interfacelive.Add(port, mid);
                    }
                }

                if (Request("show port", out lines)) return;

                foreach (string line in lines)
                {
                    if (line.Length > 0)
                    {
                        if (char.IsDigit(line[0]))
                        {
                            string[] linex = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                            if (linex.Length >= 4)
                            {
                                string portex = "Ex" + linex[0].Trim();
                                portex = portex.Replace('.', ':');

                                if (interfacelive.ContainsKey(portex))
                                {
                                    interfacelive[portex].Status = linex[1].Trim() == "Up";

                                    string il3 = linex[3].Trim();
                                    if (il3 == "Link") il3 = "Up";

                                    interfacelive[portex].Protocol = il3 == "Up";

                                    if (interfacelive[portex].Status && interfacelive[portex].Protocol)
                                        interfacelive[portex].Used = true; // 1 1
                                    else
                                    {
                                        string desc = interfacelive[portex].Description.Trim();
                                        if (desc != null && (
                                                    desc.ToUpper().StartsWith("RESERVED") ||
                                                    desc.ToUpper().StartsWith("TRUNK") ||
                                                    desc.ToUpper().StartsWith("REQUEST") ||
                                                    desc.ToUpper().StartsWith("BOOK") ||
                                                    desc.ToUpper().StartsWith("TO")
                                                    ))
                                            interfacelive[portex].Used = true;
                                        else
                                            interfacelive[portex].Used = false;
                                    }

                                    if (linex.Length >= 7)                                    {
                                        string agr = linex[6].Trim();
                                        if (agr == "-") interfacelive[portex].Aggr = -1;
                                        else
                                        {
                                            int agri;
                                            if (!int.TryParse(agr, out agri)) agri = -1;
                                            interfacelive[portex].Aggr = agri;
                                        }

                                        if (agr != "-")
                                        {
                                            if (!interfacelive.ContainsKey("Ag" + agr))
                                            {
                                                MEInterfaceToDatabase mid = new MEInterfaceToDatabase();
                                                mid.Name = "Ag" + agr;
                                                interfacelive.Add("Ag" + agr, mid);
                                            }
                                        }

                                        if (linex.Length >= 10)
                                        {
                                            string typ = linex[9].Trim().ToLower();

                                            string ity = null;
                                            if (typ == "faste") ity = "Fa";
                                            else if (typ == "xcme") ity = "Gi";
                                            else if (typ == "xgige") ity = "Te";
                                            else if (typ == "tdm") ity = "Se";

                                            if (ity != null)
                                            {
                                                interfacelive[portex].InterfaceType = ity;
                                            }


                                            //3/1/1       Up    Yes  Up      9212 9212    - accs dotq xcme   GIGE-LX  10KM
                                            //1234567890123456789012345678901234567890123456789012345678901234567890123456789
                                            //         1         2         3         4         5         6   
                                            if (line.Length >= 64)
                                            {
                                                string endinfo = line.Substring(63).Trim();

                                                if (endinfo.Length > 0)
                                                    interfacelive[portex].Info = endinfo;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                
                if (nodeVersion.StartsWith("TiMOS-B")) // sementara TiMOS-B ga bisa dapet deskripsi
                {
                    if (Request("show service sap-using", out lines)) return;

                    foreach (string line in lines)
                    {
                        if (line.Length > 0)
                        {
                            if (char.IsDigit(line[0]) || line.StartsWith("lag"))
                            {
                                string[] linex = line.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);

                                string name = null, vlan = null;
                                string thisport = null;
                                string circuitID = null;

                                string[] portx = linex[0].Split(new char[] { ':' });
                                if (portx[0].StartsWith("lag-")) name = "Ag" + portx[0].Substring(4);
                                else name = "Ex" + portx[0];

                                name = name.Replace('.', ':');

                                if (portx.Length > 1) vlan = portx[1];
                                if (vlan == null) thisport = name + ".DIRECT";
                                else thisport = name + "." + vlan;

                                if (name.StartsWith("Ag"))
                                {
                                    if (!interfacelive.ContainsKey(name))
                                    {
                                        MEInterfaceToDatabase mid = new MEInterfaceToDatabase();
                                        mid.Name = name;
                                        mid.Status = true;
                                        mid.Protocol = true;
                                        mid.Enable = true;
                                        interfacelive.Add(name, mid);
                                    }
                                }

                                if (circuitdb.ContainsKey(linex[1])) circuitID = circuitdb[linex[1]]["MC_ID"].ToString();
                                else circuitID = null;

                                string status = linex[6];
                                string protocol = linex[7];

                                string ingressID = null;
                                if (qosdb.ContainsKey("0_" + linex[2])) ingressID = qosdb["0_" + linex[2]]["MQ_ID"].ToString();
                                string egressID = null;
                                if (qosdb.ContainsKey("1_" + linex[4])) egressID = qosdb["1_" + linex[4]]["MQ_ID"].ToString();


                                if (!interfacelive.ContainsKey(thisport))
                                {
                                    MEInterfaceToDatabase mid = new MEInterfaceToDatabase();
                                    mid.Name = thisport;
                                    mid.Status = status == "Up";
                                    mid.Protocol = protocol == "Up";
                                    mid.CircuitID = circuitID;
                                    mid.IngressID = ingressID;
                                    mid.EgressID = egressID;

                                    interfacelive.Add(thisport, mid);

                                    if (interfacelive.ContainsKey(name))
                                        interfacelive[name].Used = true;
                                }
                            }
                        }
                    }

                }
                else
                {
                    // make lag list
                    if (Request("show lag description", out lines)) return;

                    /* lag
                /*
===============================================================================
Lag-id Port-id   Adm   Act/Stdby Opr   Description
-------------------------------------------------------------------------------
01234567890123456789012345678901234567890123456789
          1         2         3         4
1(e)             up              up    AKSES_PE-D2-CKA-TRANSIT/ae1_TO_ME-A-JKT-
                                       CKA/lag-1_No1_3xGi (Downlink)
       3/1/4     up    active    up    AKSES_PE TO_PE Transit
       3/1/10    up    active    up    AKSES_TO_PE2-D2-CKA-VPN
       3/1/14    up    active    up    AKSES_TO_PE-D2-CKA-VPN_Gi0/0/0/2
       3/1/15    up    active    up    AKSES_TO_PE-D2-CKA-TRANSIT LAG-1 port
                                       ge-5/0/1

2(e)             up              up    TRUNK_to me2-d2-cka (80G)
       5/1/6     up    active    up    TRUNK_to-me2-d2-cka port 3/1/2
       5/2/3     up    active    up    TRUNK_to-me2-d2-cka port 8/1/4
       */
                    MEInterfaceToDatabase current = null;

                    foreach (string line in lines)
                    {
                        if (line.Length > 1)
                        {
                            if (char.IsDigit(line[0]))
                            {
                                if (description != null && current != null) current.Description = description.ToString();
                                description = null;
                                current = null;

                                int lag = -1;
                                bool lagok = false;

                                int fpar = line.IndexOf('(');
                                if (fpar > -1)
                                {
                                    lagok = int.TryParse(line.Substring(0, fpar), out lag);
                                }
                                else
                                {
                                    lagok = int.TryParse(line.Substring(0, line.IndexOf(' ')), out lag);
                                }

                                if (lagok)
                                {
                                    if (interfacelive.ContainsKey("Ag" + lag))
                                    {
                                        current = interfacelive["Ag" + lag];
                                        bool enup = line.Substring(17, 2) == "up" ? true : false;
                                        bool prot = line.Substring(33, 2) == "up" ? true : false;
                                        current.Status = enup;
                                        current.Enable = enup;
                                        current.Protocol = prot;
                                        if (line.Length >= 40)
                                        {
                                            description = new StringBuilder();
                                            description.Append(line.Substring(39));
                                        }
                                    }
                                }
                            }
                            else if (description != null && line.Length >= 40)
                            {
                                if (line.Substring(0, 39).Trim() == "") description.Append(line.Substring(39));
                            }
                        }
                    }

                    if (description != null && current != null) current.Description = description.ToString();

                    if (Request("show service sap-using description", out lines)) return;

                    port = null;
                    description = new StringBuilder();
                    string status = null;
                    string protocol = null;
                    string circuitID = null;
                    int dot1q = -1;
                    foreach (string line in lines)
                    {
                        if (line.Length > 0)
                        {
                            if (char.IsDigit(line[0]) || line.StartsWith("lag"))
                            {
                                if (port != null)
                                {
                                    if (!interfacelive.ContainsKey(port))
                                    {
                                        string desc = description.ToString();
                                        if (desc == "(Not Specified)") desc = null;

                                        MEInterfaceToDatabase mid = new MEInterfaceToDatabase();
                                        mid.Name = port;
                                        mid.Description = desc;
                                        mid.Status = status == "Up";
                                        mid.Protocol = protocol == "Up";
                                        mid.Enable = mid.Status;
                                        mid.CircuitID = circuitID;
                                        mid.Dot1Q = dot1q;

                                        interfacelive.Add(port, mid);
                                    }
                                }

                                string[] linex = line.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);

                                string name = null, vlan = null;
                                string[] portx = linex[0].Split(new char[] { ':' });
                                if (portx[0].StartsWith("lag-")) name = "Ag" + portx[0].Substring(4);
                                else name = "Ex" + portx[0];
                                
                                name = name.Replace('.', ':');

                                if (portx.Length > 1) vlan = portx[1];

                                if (name.StartsWith("Ag"))
                                {
                                    if (!interfacelive.ContainsKey(name))
                                    {
                                        // Unexpected Lag, not found in Lag Description
                                        MEInterfaceToDatabase mid = new MEInterfaceToDatabase();
                                        mid.Name = name;
                                        mid.Status = true;
                                        mid.Protocol = true;
                                        mid.Enable = true;
                                        interfacelive.Add(name, mid);
                                    }
                                }

                                if (circuitdb.ContainsKey(linex[1])) circuitID = circuitdb[linex[1]]["MC_ID"].ToString();
                                else circuitID = null;

                                status = linex[2];
                                protocol = linex[3];

                                description.Clear();
                                if (line.Length > 58)
                                    description.Append(line.Substring(58).TrimStart());

                                // 3/2/20:875                         202340875    Up   Down (Not Specified)
                                // 1/2/1:3081                         1310013081   Up   Up   (Not Specified)
                                // 0123456789012345678901234567890123456789012345678901234567890123456789
                                //           1         2         3         4         5

                                dot1q = -1;
                                if (vlan == null) port = name + ".DIRECT";
                                else
                                {
                                    port = name + "." + vlan;
                                    string[] svlan = vlan.Split(StringSplitTypes.Dot);
                                    int.TryParse(svlan[0], out dot1q);
                                }
                            }
                            else if (line[0] == ' ')
                            {
                                if (port != null)
                                {
                                    description.Append(line.TrimStart());
                                }
                            }
                            else
                            {
                                if (port != null)
                                {
                                    if (!interfacelive.ContainsKey(port))
                                    {
                                        string desc = description.ToString();
                                        if (desc == "(Not Specified)") desc = null;

                                        MEInterfaceToDatabase mid = new MEInterfaceToDatabase();
                                        mid.Name = port;
                                        mid.Description = desc;
                                        mid.Status = status == "Up";
                                        mid.Protocol = protocol == "Up";
                                        mid.CircuitID = circuitID;

                                        interfacelive.Add(port, mid);
                                    }
                                    port = null;
                                }
                            }
                        }
                    }
                    if (port != null)
                    {
                        if (!interfacelive.ContainsKey(port))
                        {
                            string desc = description.ToString();
                            if (desc == "(Not Specified)") desc = null;

                            MEInterfaceToDatabase mid = new MEInterfaceToDatabase();
                            mid.Name = port;
                            mid.Description = desc;
                            mid.Status = status == "Up";
                            mid.Protocol = protocol == "Up";
                            mid.Enable = mid.Status;
                            mid.CircuitID = circuitID;
                            mid.Dot1Q = dot1q;

                            interfacelive.Add(port, mid);
                        }
                    }

                    if (Request("show service sap-using", out lines)) return;

                    foreach (string line in lines)
                    {
                        if (line.Length > 0)
                        {
                            if (char.IsDigit(line[0]) || line.StartsWith("lag"))
                            {
                                string[] linex = line.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);

                                string name = null, vlan = null;
                                string thisport = null;

                                string[] portx = linex[0].Split(new char[] { ':' });
                                if (portx[0].StartsWith("lag-")) name = "Ag" + portx[0].Substring(4);
                                else name = "Ex" + portx[0];

                                name = name.Replace('.', ':');

                                if (portx.Length > 1) vlan = portx[1];
                                if (vlan == null) thisport = name + ".DIRECT";
                                else thisport = name + "." + vlan;

                                string ingressID = null;
                                if (qosdb.ContainsKey("0_" + linex[2])) ingressID = qosdb["0_" + linex[2]]["MQ_ID"].ToString();
                                string egressID = null;
                                if (qosdb.ContainsKey("1_" + linex[4])) egressID = qosdb["1_" + linex[4]]["MQ_ID"].ToString();

                                if (interfacelive.ContainsKey(thisport))
                                {
                                    if (ingressID != null)
                                        interfacelive[thisport].IngressID = ingressID;
                                    if (egressID != null)
                                        interfacelive[thisport].EgressID = egressID;

                                    if (interfacelive.ContainsKey(name))
                                        interfacelive[name].Used = true;
                                }
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
                                        MEInterfaceToDatabase mid = new MEInterfaceToDatabase();
                                        mid.Name = port;
                                        mid.Description = description.ToString().Trim();

                                        interfacelive.Add(port, mid);
                                    }

                                    description.Clear();
                                    port = null;
                                }

                                // 5.90
                                //GigabitEthernet0/0/0          H
                                //0123456789012345678901234567890
                                //          1         2         3

                                // 5.120
                                //Aux0/0/1                      *down   down     H
                                //012345678901234567890123456789012345678901234567
                                //          1         2         3         4

                                // 5.160
                                //Aux0/0/1                      down    down     HUAWEI, Aux0/0/1 Interface

                                // 8.x
                                //Eth-Trunk1????                up      up       AGGR_PE2-D1-PBR-TRANSIT/ETH-TRUNK1_TO_T-D1-PBR/BE5_5x10G
                                //012345678901234567890123456789012345678901234567
                                //          1         2         3         4
                                string inf = line.Substring(0, 30).Trim();
                                if (inf.StartsWith("Eth-Trunk")) port = "Ag" + inf.Substring(9);
                                else
                                {
                                    NodeInterface nif = NodeInterface.Parse(inf);
                                    if (nif != null) port = nif.GetShort();
                                }

                                if (port != null)
                                {
                                    string descarea = null;
                                    if (nodeVersion == "5.90")
                                        descarea = line.Substring(30).TrimStart();
                                    else
                                        descarea = line.Substring(47).TrimStart();

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
                        MEInterfaceToDatabase mid = new MEInterfaceToDatabase();
                        mid.Name = port;
                        mid.Description = description.ToString().Trim();

                        interfacelive.Add(port, mid);
                    }

                    description = new StringBuilder();
                    port = null;
                }

                if (Request("display interface brief", out lines)) return;

                begin = false;
                string aggre = null;

                foreach (string line in lines)
                {
                    if (begin)
                    {
                        string[] linex2 = line.Trim().Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);

                        if (linex2.Length >= 3)
                        {
                            string pstat = linex2[1].IndexOf("up") > -1 ? "Up" : "Down";
                            string pprot = linex2[2].IndexOf("up") > -1 ? "Up" : "Down";
                            string[] linex3 = linex2[0].Split(new char[] { '(', ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            string poe = null;
                            string pot = null;
                            bool issif = false;

                            if (linex3[0].StartsWith("Eth-Trunk")) poe = "Ag" + linex3[0].Substring(9);
                            else
                            {
                                NodeInterface nif = NodeInterface.Parse(linex3[0]);
                                if (nif != null)
                                {
                                    poe = nif.GetShort();
                                    pot = nif.ShortType;
                                    issif = nif.IsSubInterface;
                                }
                            }

                            if (poe != null)
                            {
                                if (interfacelive.ContainsKey(poe))
                                {
                                    interfacelive[poe].Status = pstat == "Up";
                                    interfacelive[poe].Protocol = pprot == "Up";

                                    if (issif == false)
                                    {
                                        if (pot == "Gi" && line.IndexOf("(10G)") > -1) pot = "Te";
                                        interfacelive[poe].InterfaceType = pot;
                                    }
                                }
                            }

                            if (line[0] != ' ')
                            {
                                string[] linex = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                                if (linex[0].StartsWith("Eth-Trunk") && linex[0].IndexOf(".") == -1)
                                    aggre = linex[0].Substring(9).Trim();
                                else
                                    aggre = null;
                            }
                            else
                            {
                                string[] linex = line.Trim().Split(new char[] { '(', ' ' }, StringSplitOptions.RemoveEmptyEntries);

                                NodeInterface nif = NodeInterface.Parse(linex[0]);
                                if (nif != null)
                                {
                                    string portnif = nif.GetShort();
                                    if (interfacelive.ContainsKey(portnif))
                                    {
                                        int agr;
                                        if (!int.TryParse(aggre, out agr)) agr = -1;
                                        interfacelive[portnif].Aggr = agr;
                                    }
                                }
                            }

                        }
                    }
                    else
                    {
                        if (line.StartsWith("Interface")) begin = true;
                    }
                }
                
                // mpls l2vc ke port
                foreach (string[] strs in hwecircuitdetail)
                {
                    // cinterface, cinterfaceSDP, cinterfacestate.ToString(), vcidname, cinterfaceVCID
                    //  0            1              2                          3         4

                    string vcidname = strs[3];
                    string vcid = strs[4];
                    string sdp = strs[1];
                    string inf = strs[0];

                    if (circuitdb.ContainsKey(vcidname))
                    {
                        if (interfacelive.ContainsKey(inf))
                        {
                            string cid = circuitdb[vcidname]["MC_ID"].ToString();
                            interfacelive[inf].CircuitID = cid;
                        }
                    }
                }

                // vsi ke port (l2 binding vsi)
                if (Request("display vsi services all", out lines)) return;

                //GigabitEthernet7/0/3.2999           "ZZZ ZZZ"                       up
                //GigabitEthernet7/0/10.20            OAMN-MSAN-PWT02
                //0123456789012345678901234567890123456789012345678901234567890123456789
                //          1         2         3         4         5         6
                //                                    1234567890123456789012345678901

                foreach (string line in lines)
                {
                    if (line.Length >= 68)
                    {
                        string se = line.Substring(0, 36).Trim();
                        NodeInterface nif = NodeInterface.Parse(se);

                        if (nif != null)
                        {
                            string portnif = nif.GetShort();
                            string vsi = line.Substring(36, 31).Trim();

                            if (interfacelive.ContainsKey(portnif))
                            {
                                if (circuitdb.ContainsKey(vsi))
                                {
                                    string cid = circuitdb[vsi]["MC_ID"].ToString();
                                    interfacelive[portnif].CircuitID = cid;
                                }
                            }
                        }
                    }
                }

                // qos
                if (Request("display cur int | in interface |qos-profile |user-queue |vlan-type\\ dot1q", out lines)) return;

                string qosInterface = null;
                foreach (string line in lines)
                {
                    string lineTrim = line.Trim();

                    if (lineTrim.StartsWith("interface "))
                    {
                        string nifc = lineTrim.Substring(10);

                        string nifs = null;
                        if (nifc.StartsWith("Eth-Trunk")) nifs = "Ag" + nifc.Substring(9);
                        else
                        {
                            NodeInterface nif = NodeInterface.Parse(nifc);
                            if (nif != null) nifs = nif.GetShort();
                        }

                        if (nifs != null && interfacelive.ContainsKey(nifs))
                            qosInterface = nifs;
                    }
                    else if (qosInterface != null)
                    {
                        if (lineTrim.StartsWith("qos-profile"))
                        {
                            string[] linex = lineTrim.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);

                            if (linex.Length >= 3)
                            {
                                string qosName = linex[1];
                                string qosDir = linex[2];

                                if (qosdb.ContainsKey("0_" + qosName))
                                {
                                    string qosID = qosdb["0_" + qosName]["MQ_ID"].ToString();

                                    if (qosDir == "inbound") interfacelive[qosInterface].IngressID = qosID;
                                    else if (qosDir == "outbound") interfacelive[qosInterface].EgressID = qosID;
                                }
                            }
                        }
                        else if (lineTrim.StartsWith("user-queue"))
                        {
                            string[] linex = lineTrim.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);

                            if (linex.Length >= 6)
                            {
                                string rSize = linex[2];
                                string rDir = linex[5];

                                int size;
                                if (!int.TryParse(rSize, out size)) size = -1;

                                if (rDir == "inbound") interfacelive[qosInterface].RateInput = size;
                                else if (rDir == "outbound") interfacelive[qosInterface].RateOutput = size;
                            }
                        }
                        else if (lineTrim.StartsWith("vlan-type"))
                        {
                            //vlan-type dot1q 110
                            //012345678901234567890123456789
                            int dot1q;
                            if (int.TryParse(lineTrim.Substring(16), out dot1q)) interfacelive[qosInterface].Dot1Q = dot1q;
                        }
                    }
                }

                #endregion
            }

            #endregion

            #region Check

            foreach (KeyValuePair<string, MEInterfaceToDatabase> pair in interfacelive)
            {
                MEInterfaceToDatabase li = pair.Value;
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
                    else li.SubInterfaceCount = 0;
                }
            }

            int sinf = 0, sinfup = 0, sinfag = 0, sinfhu = 0, sinfhuup = 0, sinfte = 0, sinfteup = 0, sinfgi = 0, sinfgiup = 0, sinffa = 0, sinffaup = 0, sinfet = 0, sinfetup = 0,
                ssubinf = 0, ssubinfup = 0, ssubinfupup = 0, ssubinfag = 0, ssubinfagup = 0, ssubinfagupup = 0, ssubinfhu = 0, ssubinfhuup = 0, ssubinfhuupup = 0, ssubinfte = 0, ssubinfteup = 0, ssubinfteupup = 0, ssubinfgi = 0, ssubinfgiup = 0, ssubinfgiupup = 0, ssubinffa = 0, ssubinffaup = 0, ssubinffaupup = 0, ssubinfet = 0, ssubinfetup = 0, ssubinfetupup = 0;


            foreach (KeyValuePair<string, MEInterfaceToDatabase> pair in interfacelive)
            {
                MEInterfaceToDatabase li = pair.Value;
                NodeInterface inf = NodeInterface.Parse(li.Name);
                string parentPort = null;

                // TOPOLOGY
                if (inf != null)
                {
                    string inftype = inf.ShortType;
                    if (inftype == "Ex") inftype = li.InterfaceType;

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
                        if (inftype == "Ag") { sinfag++; }
                        if (li.Aggr != -1) parentPort = "Ag" + li.Aggr;    
                    }

                    if (parentPort != null)
                    {
                        if (interfacedb.ContainsKey(parentPort)) // cek di existing db
                            li.ParentID = interfacedb[parentPort]["MI_ID"].ToString();
                        else if (interfaceinsert.ContainsKey(parentPort)) // cek di interface yg baru
                            li.ParentID = interfaceinsert[parentPort].ID;
                    }

                    if (!inf.IsSubInterface)
                    {
                        if (inftype == "Ag")
                        {
                            // we need support with child
                            int myaggr;
                            if (int.TryParse(inf.Port, out myaggr))
                            {
                                // cari child di interfacelive yg aggr-nya myaggr
                                List<MEInterfaceToDatabase> agPhysicals = new List<MEInterfaceToDatabase>();
                                foreach (KeyValuePair<string, MEInterfaceToDatabase> spair in interfacelive) { if (spair.Value.Aggr == myaggr) agPhysicals.Add(spair.Value); }
                                li.AggrChilds = agPhysicals.ToArray();

                                // anaknya duluan ya
                                foreach (MEInterfaceToDatabase mi in li.AggrChilds)
                                    FindMEPhysicalAdjacent(mi);
                            }
                        }
                        FindMEPhysicalAdjacent(li);

                        if (inftype == "Ag" && li.AdjacentID == null) // jika ini ag, dan ga punya adjacent id, maka...
                        {
                            string adjacentParentID = null; // cek apakah setiap anak punya adjacentParentID jika tidak = null
                            foreach (MEInterfaceToDatabase mi in li.AggrChilds)
                            {
                                if (mi.AggrAdjacentParentID == null)
                                {
                                    adjacentParentID = null;
                                    break;
                                }
                                else if (adjacentParentID == null) adjacentParentID = mi.AggrAdjacentParentID;
                                else if (adjacentParentID != mi.AggrAdjacentParentID)
                                {
                                    adjacentParentID = null;
                                    break;
                                }
                            }

                            // adjacentParentID adalah parentID (aggr) dari lawannya interface aggr ini di PE.
                            if (adjacentParentID != null)
                            {
                                li.AdjacentID = adjacentParentID;

                                // query lawan
                                li.AdjacentIDList = new Dictionary<int, string>();
                                result = Query("select PI_ID, PI_DOT1Q from PEInterface where PI_PI = {0}", li.AdjacentID);
                                foreach (Row row in result)
                                {
                                    if (!row["PI_DOT1Q"].IsNull)
                                    {
                                        int dot1q = row["PI_DOT1Q"].ToShort();
                                        if (!li.AdjacentIDList.ContainsKey(dot1q)) li.AdjacentIDList.Add(dot1q, row["PI_ID"].ToString());
                                    }

                                    //string spiname = row["PI_Name"].ToString();
                                    //int dot = spiname.IndexOf('.');
                                    //if (dot > -1 && spiname.Length > (dot + 1))
                                    //{
                                    //    string sifname = spiname.Substring(dot + 1);
                                    //    if (!li.AdjacentSubifID.ContainsKey(sifname)) li.AdjacentSubifID.Add(sifname, row["PI_ID"].ToString());
                                    //}
                                }
                            }                            
                        }
                    }
                    else if (li.ParentID != null)
                    {
                        int dot1q = li.Dot1Q;
                        if (dot1q > -1)
                        {
                            if (interfacelive.ContainsKey(parentPort))
                            {
                                MEInterfaceToDatabase parent = interfacelive[parentPort];
                                if (parent.AdjacentIDList != null)
                                {
                                    if (parent.AdjacentIDList.ContainsKey(dot1q))
                                        li.AdjacentID = parent.AdjacentIDList[dot1q];
                                }
                            }
                        }

                        //int dot = li.Name.IndexOf('.');
                        //if (dot > -1 && li.Name.Length > (dot + 1))
                        //{
                        //    string sifname = li.Name.Substring(dot + 1);
                        //    if (sifname != "DIRECT")
                        //    {
                        //        MEInterfaceToDatabase parent = interfacelive[parentPort];
                        //        if (parent.AdjacentSubifID != null)
                        //        {
                        //            if (parent.AdjacentSubifID.ContainsKey(sifname))
                        //                li.AdjacentID = parent.AdjacentSubifID[sifname];
                        //        }
                        //    }
                        //}
                    }

                }


                if (!interfacedb.ContainsKey(pair.Key))
                {
                    Event("Interface ADD: " + pair.Key);

                    // Service
                    if (li.Description != null) interfaceservicereference.Add(li, li.Description);

                    li.ID = Database.ID();
                    interfaceinsert.Add(li.Name, li);
                }
                else
                {
                    Row db = interfacedb[pair.Key];

                    MEInterfaceToDatabase u = new MEInterfaceToDatabase();
                    u.ID = db["MI_ID"].ToString();

                    bool update = false;
                    StringBuilder updateinfo = new StringBuilder();

                    if (db["MI_MI"].ToString() != li.ParentID)
                    {
                        update = true;
                        u.UpdateParentID = true;
                        u.ParentID = li.ParentID;
                        updateinfo.Append(li.ParentID == null ? "parent " : "child ");
                    }
                    if (db["MI_TO_PI"].ToString() != li.AdjacentID)
                    {
                        update = true;
                        u.UpdateAdjacentID = true;
                        u.AdjacentID = li.AdjacentID;
                        updateinfo.Append("adj ");
                    }
                    if (db["MI_Description"].ToString() != li.Description)
                    {
                        update = true;
                        u.UpdateDescription = true;
                        u.Description = li.Description;
                        updateinfo.Append("desc ");

                        u.ServiceID = null;
                        if (u.Description != null) interfaceservicereference.Add(u, u.Description);
                    }
                    if (db["MI_Status"].ToBool() != li.Status)
                    {
                        update = true;
                        u.UpdateStatus = true;
                        u.Status = li.Status;
                        updateinfo.Append("stat ");
                    }
                    if (db["MI_Protocol"].ToBool() != li.Protocol)
                    {
                        update = true;
                        u.UpdateProtocol = true;
                        u.Protocol = li.Protocol;
                        updateinfo.Append("prot ");
                    }
                    if (db["MI_Enable"].ToBool() != li.Enable)
                    {
                        update = true;
                        u.UpdateEnable = true;
                        u.Enable = li.Enable;
                        updateinfo.Append("ena ");
                    }
                    if (db["MI_DOT1Q"].ToShort(-1) != li.Dot1Q)
                    {
                        update = true;
                        u.UpdateDot1Q = true;
                        u.Dot1Q = li.Dot1Q;
                        updateinfo.Append("dot1q ");
                    }
                    if (db["MI_Aggregator"].ToShort(-1) != li.Aggr)
                    {
                        update = true;
                        u.UpdateAggr = true;
                        u.Aggr = li.Aggr;
                        updateinfo.Append("aggr ");
                    }
                    if (db["MI_MC"].ToString() != li.CircuitID)
                    {
                        update = true;
                        u.UpdateCircuit = true;
                        u.CircuitID = li.CircuitID;
                        updateinfo.Append("vcid ");
                    }
                    if (db["MI_Type"].ToString() != li.InterfaceType)
                    {
                        update = true;
                        u.UpdateInterfaceType = true;
                        u.InterfaceType = li.InterfaceType;
                        updateinfo.Append("type ");
                    }
                    if (db["MI_MQ_Input"].ToString() != li.IngressID)
                    {
                        update = true;
                        u.UpdateIngressID = true;
                        u.IngressID = li.IngressID;
                        updateinfo.Append("ingress ");
                    }
                    if (db["MI_MQ_Output"].ToString() != li.EgressID)
                    {
                        update = true;
                        u.UpdateEgressID = true;
                        u.EgressID = li.EgressID;
                        updateinfo.Append("egress ");
                    }
                    if (db["MI_Used"].ToNullableBool() != li.Used)
                    {
                        update = true;
                        u.UpdateUsed = true;
                        u.Used = li.Used;
                        updateinfo.Append("used ");
                    }
                    if (db["MI_Rate_Input"].ToInt(-1) != li.RateInput)
                    {
                        update = true;
                        u.UpdateRateInput = true;
                        u.RateInput = li.RateInput;
                        updateinfo.Append("rin ");
                    }
                    if (db["MI_Rate_Output"].ToInt(-1) != li.RateOutput)
                    {
                        update = true;
                        u.UpdateRateOutput = true;
                        u.RateOutput = li.RateOutput;
                        updateinfo.Append("rout ");
                    }
                    if (db["MI_Info"].ToString() != li.Info)
                    {
                        update = true;
                        u.UpdateInfo = true;
                        u.Info = li.Info;
                        updateinfo.Append("info ");
                    }
                    if (db["MI_Summary_SubInterfaceCount"].ToShort(-1) != li.SubInterfaceCount)
                    {
                        update = true;
                        u.UpdateSubInterfaceCount = true;
                        u.SubInterfaceCount = li.SubInterfaceCount;
                        updateinfo.Append("subifc ");
                    }

                    if (update)
                    {
                        Event("Interface UPDATE: " + pair.Key + " " + updateinfo.ToString());
                        interfaceupdate.Add(u);
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
            foreach (KeyValuePair<string, MEInterfaceToDatabase> pair in interfaceinsert)
            {
                MEInterfaceToDatabase s = pair.Value;

                Insert insert = Insert("MEInterface");
                insert.Value("MI_ID", s.ID);
                insert.Value("MI_NO", nodeID);
                insert.Value("MI_Name", s.Name);
                insert.Value("MI_Status", s.Status);
                insert.Value("MI_Protocol", s.Protocol);
                insert.Value("MI_Enable", s.Enable);
                insert.Value("MI_DOT1Q", s.Dot1Q.Nullable(-1));
                insert.Value("MI_Aggregator", s.Aggr.Nullable(-1));
                insert.Value("MI_Description", s.Description);
                insert.Value("MI_MC", s.CircuitID);
                insert.Value("MI_Type", s.InterfaceType);
                insert.Value("MI_MQ_Input", s.IngressID);
                insert.Value("MI_MQ_Output", s.EgressID);
                insert.Value("MI_Rate_Input", s.RateInput.Nullable(-1));
                insert.Value("MI_Rate_Output", s.RateOutput.Nullable(-1));
                insert.Value("MI_Used", s.Used);
                insert.Value("MI_Info", s.Info);
                insert.Value("MI_SE", s.ServiceID);
                insert.Value("MI_MI", s.ParentID);
                insert.Value("MI_TO_PI", s.AdjacentID);
                insert.Value("MI_Summary_SubInterfaceCount", s.SubInterfaceCount.Nullable(-1));
                batch.Execute(insert);

                interfacereferenceupdate.Add(new Tuple<string, string>(s.AdjacentID, s.ID));
            }
            result = batch.Commit();
            Event(result, EventActions.Add, EventElements.Interface, false);

            // UPDATE       
            batch.Begin();
            foreach (MEInterfaceToDatabase s in interfaceupdate)
            {
                Update update = Update("MEInterface");
                update.Set("MI_MI", s.ParentID, s.UpdateParentID);
                if (s.UpdateAdjacentID)
                {
                    update.Set("MI_TO_PI", s.AdjacentID);
                    interfacereferenceupdate.Add(new Tuple<string, string>(s.AdjacentID, s.ID));
                }
                if (s.UpdateDescription)
                {
                    update.Set("MI_Description", s.Description);
                    update.Set("MI_SE", s.ServiceID);
                }
                update.Set("MI_Status", s.Status, s.UpdateStatus);
                update.Set("MI_Protocol", s.Protocol, s.UpdateProtocol);
                update.Set("MI_Enable", s.Enable, s.UpdateEnable);
                update.Set("MI_DOT1Q", s.Dot1Q.Nullable(-1), s.UpdateDot1Q);
                update.Set("MI_Aggregator", s.Aggr.Nullable(-1), s.UpdateAggr);
                update.Set("MI_MC", s.CircuitID, s.UpdateCircuit);
                update.Set("MI_Type", s.InterfaceType, s.UpdateInterfaceType);
                update.Set("MI_MQ_Input", s.IngressID, s.UpdateIngressID);
                update.Set("MI_MQ_Output", s.EgressID, s.UpdateEgressID);
                update.Set("MI_Rate_Input", s.RateInput.Nullable(-1), s.UpdateRateInput);
                update.Set("MI_Rate_Output", s.RateOutput.Nullable(-1), s.UpdateRateOutput);
                update.Set("MI_Used", s.Used, s.UpdateUsed);
                update.Set("MI_Info", s.Info, s.UpdateInfo);
                update.Set("MI_Summary_SubInterfaceCount", s.SubInterfaceCount, s.UpdateSubInterfaceCount);
                update.Where("MI_ID", s.ID);
                batch.Execute(update);
            }
            result = batch.Commit();
            Event(result, EventActions.Update, EventElements.Interface, false);

            batch.Begin();
            foreach (Tuple<string, string> tuple in interfacereferenceupdate)
            {
                if (tuple.Item1 != null) batch.Execute("update PEInterface set PI_TO_MI = {0} where PI_ID = {1}", tuple.Item2, tuple.Item1);
                else batch.Execute("update PEInterface set PI_TO_MI = NULL where PI_TO_MI = {0}", tuple.Item2);
            }
            result = batch.Commit();

            // DELETE
            batch.Begin();
            List<string> interfacedelete = new List<string>();
            foreach (KeyValuePair<string, Row> pair in interfacedb)
            {
                if (!interfacelive.ContainsKey(pair.Key))
                {
                    Event("Interface DELETE: " + pair.Key);
                    string id = pair.Value["MI_ID"].ToString();
                    batch.Execute("update PEInterface set PI_TO_MI = NULL where PI_TO_MI = {0}", id);
                    batch.Execute("update MEInterface set MI_MI = NULL where MI_MI = {0}", id);
                    interfacedelete.Add(id);            
                }
            }
            batch.Commit();
            batch.Begin();
            foreach (string id in interfacedelete)
            {
                batch.Execute("delete from MEInterface where MI_ID = {0}", id);
            }
            result = batch.Commit();
            Event(result, EventActions.Delete, EventElements.Interface, false);

            // RESERVED INTERFACES
            batch.Begin();
            foreach (KeyValuePair<string, MEInterfaceToDatabase> pair in interfacelive)
            {
                if (reservedInterfaces.ContainsKey(pair.Key)) batch.Execute("delete from ReservedInterface where RI_ID = {0}", reservedInterfaces[pair.Key]["RI_ID"].ToString());
            }
            result = batch.Commit();
            if (result.AffectedRows > 0) Event(result.AffectedRows + " reserved interface" + (result.AffectedRows > 1 ? "s have " : " has ") + " been found");

            #endregion

            #endregion

            #region LATE DELETE

            // CIRCUIT DELETE
            batch.Begin();
            List<string> circuitdelete = new List<string>();
            foreach (KeyValuePair<string, Row> pair in circuitdb)
            {
                if (!circuitlive.ContainsKey(pair.Key))
                {
                    Event("Circuit DELETE: " + pair.Key);
                    string id = pair.Value["MC_ID"].ToString();
                    batch.Execute("update MEInterface set MI_MC = NULL where MI_MC = {0}", id);
                    batch.Execute("update MEPeer set MP_TO_MC = NULL where MP_TO_MC = {0}", id);
                    circuitdelete.Add(id);
                }
            }
            batch.Commit();
            batch.Begin();
            foreach (string id in circuitdelete)
            {
                batch.Execute("delete from MECircuit where MC_ID = {0}", id);
            }
            result = batch.Commit();
            Event(result, EventActions.Delete, EventElements.Circuit, false);

            // SDP DELETE
            batch.Begin();
            foreach (KeyValuePair<string, Row> pair in sdpdb)
            {
                if (!sdplive.ContainsKey(pair.Key))
                {
                    Event("SDP DELETE: " + pair.Key);
                    batch.Execute("delete from MESDP where MS_ID = {0}", pair.Value["MS_ID"].ToString());                    
                }
            }
            result = batch.Commit();
            Event(result, EventActions.Delete, EventElements.SDP, false);

            // ALU CUSTOMER DELETE            
            if (nodeManufacture == alu)
            {
                batch.Begin();
                foreach (KeyValuePair<string, Row> pair in alucustdb)
                {
                    Row row = pair.Value;
                    if (!alucustlive.ContainsKey(pair.Key))
                    {
                        Event("ALU-Customer DELETE: " + pair.Key);
                        batch.Execute("delete from MECustomer where MU_ID = {0}", row["MU_ID"].ToString());
                    }
                }
                result = batch.Commit();
                Event(result, EventActions.Delete, EventElements.ALUCustomer, false);
            }

            // QOS DELETE
            batch.Begin();
            foreach (KeyValuePair<string, Row> pair in qosdb)
            {
                Row row = pair.Value;
                if (!qoslive.ContainsKey(pair.Key))
                {
                    Event("QOS DELETE: " + pair.Key);
                    batch.Execute("delete from MEQOS where MQ_ID = {0}", row["MQ_ID"].ToString());
                }
            }
            result = batch.Commit();
            Event(result, EventActions.Delete, EventElements.QOS, false);

            #endregion

            SaveExit();            
        }
    }
}
