using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Aphysoft.Common;
using Aphysoft.Share;

namespace Jovice
{
    class MEInterfaceToDatabase : ElementToDatabase
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

        private int used = -1;

        public int Used
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

    class MECircuitToDatabase : StatusToDatabase
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
    }

    class MECircuitSDPToDatabase : StatusToDatabase
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
    }

    internal sealed partial class Probe
    {
        private bool MEProcess()
        {
            #region Variables

            List<string> batchlist1 = new List<string>();
            List<string> batchlist2 = new List<string>();
            List<string> batchlist3 = new List<string>();
            List<string> batchlist4 = new List<string>();
            List<string> batchlist5 = new List<string>();
            StringBuilder batchstring = new StringBuilder();
            int batchline = 0;

            #endregion

            #region CUSTOMER

            string custinsertsql = "insert into MECustomer(MU_ID, MU_NO, MU_UID) values";
            string custdeletesql = "delete from MECustomer where ";
            Dictionary<string, string> custlive = new Dictionary<string, string>();
            Result custdbresult = j.Query("select * from MECustomer where MU_NO = {0}", nodeID);
            Dictionary<string, string> custdb = new Dictionary<string, string>();
            foreach (Row row in custdbresult) { custdb.Add(row["MU_UID"].ToString(), row["MU_ID"].ToString()); }

            if (feature == null || feature == "customer")
            {
                Event("Checking Service Customer");

                if (nodeManufacture == alu)
                {
                    #region Live

                    if (nodeManufacture == alu)
                    {
                        if (Send("show service customer | match \"Customer-ID\"")) { NodeSaveMainLoopRestart(); return true; }
                        bool timeout;
                        List<string> lines = NodeRead(out timeout);
                        if (requestFailure) { requestFailure = false; MainLoopRestart(); return true; }
                        if (timeout) { NodeReadTimeOutExit(); return true; }

                        foreach (string line in lines)
                        {
                            string[] linex = line.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                            if (linex.Length == 2 && linex[0].Trim() == "Customer-ID")
                            {
                                custlive.Add(linex[1].Trim(), null);
                            }
                        }
                    }

                    #endregion

                    #region Execute

                    // ADD
                    batchline = 0;
                    batchlist1.Clear();
                    batchstring.Clear();

                    int newcustomer = 0;
                    foreach (KeyValuePair<string, string> pair in custlive)
                    {
                        string uid = pair.Key;

                        if (!custdb.ContainsKey(uid))
                        {
                            Event("Customer ADD: " + uid);
                            string id = Database.ID();

                            batchlist1.Add(j.Format("({0}, {1}, {2})", id, nodeID, uid));

                            if (batchlist1.Count >= 50)
                            {
                                newcustomer += j.Execute(custinsertsql + string.Join(",", batchlist1.ToArray())).AffectedRows;
                                batchlist1.Clear();
                            }
                        }
                    }
                    if (batchlist1.Count > 0)
                        j.Execute(custinsertsql + string.Join(",", batchlist1.ToArray()));
                    if (newcustomer > 0)
                        Event(newcustomer + " customer(s) added");



                    #endregion
                }
            }

            #endregion

            #region QOS

            string qosinsertsql = "insert into MEQOS(MQ_ID, MQ_NO, MQ_Name, MQ_Type, MQ_Bandwidth) values";
            string qosdeletesql = "delete from MEQOS where ";
            Result qosdbresult = j.Query("select * from MEQOS where MQ_NO = {0}", nodeID);
            Dictionary<string, Row> qosdb = new Dictionary<string, Row>();
            foreach (Row sdbo in qosdbresult) { qosdb.Add((sdbo["MQ_Type"].ToBoolean() ? "1" : "0") + "_" + sdbo["MQ_Name"].ToString(), sdbo); }
            Dictionary<string, MEQOSToDatabase> qoslive = new Dictionary<string, MEQOSToDatabase>();
            List<MEQOSToDatabase> qosinsert = new List<MEQOSToDatabase>();
            List<MEQOSToDatabase> qosupdate = new List<MEQOSToDatabase>();

            if (feature == null || feature == "qos")
            {
                Event("Checking QOS");

                #region Live

                if (nodeManufacture == alu)
                {
                    #region alu

                    if (Send("show qos sap-ingress")) { NodeSaveMainLoopRestart(); return true; }
                    bool timeout;
                    List<string> lines = NodeRead(out timeout);
                    if (requestFailure) { requestFailure = false; MainLoopRestart(); return true; }
                    if (timeout) { NodeReadTimeOutExit(); return true; }

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

                    if (Send("show qos sap-egress")) { NodeSaveMainLoopRestart(); return true; }
                    lines = NodeRead(out timeout);
                    if (requestFailure) { requestFailure = false; MainLoopRestart(); return true; }
                    if (timeout) { NodeReadTimeOutExit(); return true; }

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

                        NodeQOSMEALU ni = NodeQOSMEALU.Parse(li.Name);
                        li.Bandwidth = ni.Bandwidth;
                    }

                    #endregion
                }
                else if (nodeManufacture == hwe)
                {
                    #region hwe

                    if (Send("display qos-profile configuration")) { NodeSaveMainLoopRestart(); return true; }
                    bool timeout;
                    List<string> lines = NodeRead(out timeout);
                    if (requestFailure) { requestFailure = false; MainLoopRestart(); return true; }
                    if (timeout) { NodeReadTimeOutExit(); return true; }

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

                        NodeQOSMEHWE ni = NodeQOSMEHWE.Parse(li.Name);
                        li.Bandwidth = ni.Bandwidth;
                    }

                    #endregion
                }

                #endregion

                #region Check

                foreach (KeyValuePair<string, MEQOSToDatabase> pair in qoslive)
                {
                    if (!qosdb.ContainsKey(pair.Key))
                    {
                        // ADD
                        qosinsert.Add(pair.Value);
                        Event("QOS ADD: " + pair.Key + ((pair.Value.Bandwidth == -1) ? "" : ("(" + pair.Value.Bandwidth + "K)")));
                    }
                    else
                    {
                        Row db = qosdb[pair.Key];

                        MEQOSToDatabase u = new MEQOSToDatabase();
                        u.ID = db["MQ_ID"].ToString();

                        MEQOSToDatabase li = pair.Value;
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
                            Event("QOS MODIFY: " + pair.Key + " " + updateinfo.ToString());
                            qosupdate.Add(u);
                        }
                    }
                }
                #endregion

                #region Execute

                // ADD
                batchline = 0;
                batchlist1.Clear();
                batchstring.Clear();

                int newqos = 0;
                foreach (MEQOSToDatabase s in qosinsert)
                {
                    string id = Database.ID();

                    if (s.Type == -1)
                    {
                        if (s.Bandwidth == -1) batchlist1.Add(j.Format("({0}, {1}, {2}, null, null)", id, nodeID, s.Name));
                        else batchlist1.Add(j.Format("({0}, {1}, {2}, null, {3})", id, nodeID, s.Name, s.Bandwidth));
                    }
                    else
                    {
                        if (s.Bandwidth == -1) batchlist1.Add(j.Format("({0}, {1}, {2}, {3}, null)", id, nodeID, s.Name, s.Type));
                        else batchlist1.Add(j.Format("({0}, {1}, {2}, {3}, {4})", id, nodeID, s.Name, s.Type, s.Bandwidth));
                    }

                    if (batchlist1.Count == 50)
                    {
                        newqos += j.Execute(qosinsertsql + string.Join(",", batchlist1.ToArray())).AffectedRows;
                        batchlist1.Clear();
                    }
                }
                if (batchlist1.Count > 0)
                    newqos += j.Execute(qosinsertsql + string.Join(",", batchlist1.ToArray())).AffectedRows;
                if (newqos > 0)
                    Event(newqos + " qos(s) added");

                // MODIFY
                batchline = 0;
                batchlist1.Clear();
                batchstring.Clear();

                int modifiedqos = 0;
                foreach (MEQOSToDatabase s in qosupdate)
                {
                    List<string> v = new List<string>();
                    if (s.UpdateBandwidth) v.Add(s.Bandwidth == -1 ? "MQ_Bandwidth = null" : j.Format("MQ_Bandwidth = {0}", s.Bandwidth));

                    if (v.Count > 0)
                    {
                        string q = j.Format("update MEQOS set " + StringHelper.EscapeFormat(string.Join(",", v.ToArray())) + " where MQ_ID = {0};", s.ID);
                        batchline++;
                        batchstring.AppendLine(q);

                        if (batchline == 50)
                        {
                            modifiedqos += j.Execute(batchstring.ToString()).AffectedRows;
                            batchstring.Clear();
                            batchline = 0;
                        }
                    }
                }
                if (batchline > 0)
                    modifiedqos += j.Execute(batchstring.ToString()).AffectedRows;
                if (modifiedqos > 0)
                    Event(modifiedqos + " qos(s) modified");

                #endregion
            }

            qosdbresult = j.Query("select * from MEQOS where MQ_NO = {0}", nodeID);
            qosdb = new Dictionary<string, Row>();
            foreach (Row sdbo in qosdbresult) { qosdb.Add((sdbo["MQ_Type"].ToBoolean() ? "1" : "0") + "_" + sdbo["MQ_Name"].ToString(), sdbo); }

            #endregion

            #region PEER
                        
            string peerinsertsql = "insert into MESDP(MS_ID, MS_NO, MS_SDP, MS_Status, MS_Protocol, MS_IP, MS_MTU, MS_Type, MS_LSP) values";
            string peerdeletesql = "delete from MESDP where ";
            Result peerdbresult = j.Query("select * from MESDP where MS_NO = {0}", nodeID);
            Dictionary<string, Row> peerdb = new Dictionary<string, Row>();                
            foreach (Row sdbo in peerdbresult) { peerdb.Add(sdbo["MS_SDP"].ToString(), sdbo); }
            Dictionary<string, MESDPToDatabase> peerlive = new Dictionary<string, MESDPToDatabase>();
            List<MESDPToDatabase> peerinsert = new List<MESDPToDatabase>();
            List<MESDPToDatabase> peerupdate = new List<MESDPToDatabase>();

            if (feature == null || feature == "peer")
            {
                Event("Checking Peer");

                #region Live

                if (nodeManufacture == alu)
                {
                    #region alu

                    if (Send("show service sdp")) { NodeSaveMainLoopRestart(); return true; }
                    bool timeout;
                    List<string> lines = NodeRead(out timeout);
                    if (requestFailure) { requestFailure = false; MainLoopRestart(); return true; }
                    if (timeout) { NodeReadTimeOutExit(); return true; }

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
                                d.Status = status == true ? 1 : 0;
                                d.Protocol = protocol == true ? 1 : 0;
                                d.Type = type;
                                d.LSP = lsp;

                                peerlive.Add(sdp, d);
                            }
                        }
                    }

                    #endregion
                }
                else if (nodeManufacture == hwe)
                {
                    #region hwe

                    // dari mpls
                    if (Send("display mpls ldp remote-peer")) { NodeSaveMainLoopRestart(); return true; }
                    bool timeout;
                    List<string> lines = NodeRead(out timeout);
                    if (requestFailure) { requestFailure = false; MainLoopRestart(); return true; }
                    if (timeout) { NodeReadTimeOutExit(); return true; }

                    string farend = null;
                    int active = -1;

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
                                if (lineTrim.IndexOf("Active") > -1) active = 1;
                                else active = 0;

                                MESDPToDatabase d = new MESDPToDatabase();
                                d.SDP = farend;
                                d.AdmMTU = 0;
                                d.FarEnd = farend;
                                d.Status = active;
                                d.Protocol = active;
                                d.Type = "M";
                                d.LSP = "L";

                                peerlive.Add(farend, d);

                                active = -1;
                                farend = null;
                            }
                        }
                    }

                    // dari vsi
                    if (Send("display vsi verbose | in Peer Router ID")) { NodeSaveMainLoopRestart(); return true; }
                    lines = NodeRead(out timeout);
                    if (requestFailure) { requestFailure = false; MainLoopRestart(); return true; }
                    if (timeout) { NodeReadTimeOutExit(); return true; }

                    foreach (string line in lines)
                    {
                        string lineTrim = line.Trim();
                        if (lineTrim.Length > 0)
                        {
                            string[] linex = lineTrim.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                            if (linex.Length == 2)
                            {
                                farend = linex[1].Trim();

                                if (!peerlive.ContainsKey(farend))
                                {
                                    MESDPToDatabase d = new MESDPToDatabase();
                                    d.SDP = farend;
                                    d.AdmMTU = 0;
                                    d.FarEnd = farend;
                                    d.Status = 1;
                                    d.Protocol = 1;
                                    d.Type = "V";
                                    d.LSP = "L";

                                    peerlive.Add(farend, d);
                                }
                            }
                        }
                    }

                    // dari mpls
                    //
                    if (Send("display mpls l2vc | in destination")) { NodeSaveMainLoopRestart(); return true; }
                    lines = NodeRead(out timeout);
                    if (requestFailure) { requestFailure = false; MainLoopRestart(); return true; }
                    if (timeout) { NodeReadTimeOutExit(); return true; }

                    foreach (string line in lines)
                    {
                        string lineTrim = line.Trim();
                        if (lineTrim.Length > 0 && lineTrim.StartsWith("destination"))
                        {
                            string[] linex = lineTrim.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                            if (linex.Length == 2)
                            {
                                farend = linex[1].Trim();

                                if (!peerlive.ContainsKey(farend))
                                {
                                    MESDPToDatabase d = new MESDPToDatabase();
                                    d.SDP = farend;
                                    d.AdmMTU = 0;
                                    d.FarEnd = farend;
                                    d.Status = 1;
                                    d.Protocol = 1;
                                    d.Type = "E";
                                    d.LSP = "L";

                                    peerlive.Add(farend, d);
                                }
                            }
                        }
                    }

                    #endregion
                }

                #endregion

                #region Check

                foreach (KeyValuePair<string, MESDPToDatabase> pair in peerlive)
                {
                    if (!peerdb.ContainsKey(pair.Key))
                    {
                        // ADD
                        peerinsert.Add(pair.Value);
                        Event("Peer ADD: " + pair.Key);
                    }
                    else
                    {
                        // MODIFY
                        Row db = peerdb[pair.Key];

                        MESDPToDatabase u = new MESDPToDatabase();
                        u.ID = db["MS_ID"].ToString();

                        MESDPToDatabase li = pair.Value;
                        bool update = false;

                        StringBuilder updateinfo = new StringBuilder();

                        if ((db["MS_Status"].ToBoolean() ? 1 : 0) != li.Status)
                        {
                            update = true;
                            u.UpdateStatus = true;
                            u.Status = li.Status;
                            updateinfo.Append("stat ");
                        }
                        if ((db["MS_Protocol"].ToBoolean() ? 1 : 0) != li.Protocol)
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
                        if ((db["MS_MTU"].IsNull ? 0 : db["MS_MTU"].ToSmall()) != li.AdmMTU)
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
                        if (update)
                        {
                            Event("Peer MODIFY: " + pair.Key + " " + updateinfo.ToString());
                            peerupdate.Add(u);
                        }
                    }
                }

                #endregion

                #region Execute

                // ADD
                batchline = 0;
                batchlist1.Clear();
                batchstring.Clear();

                int newpeer = 0;
                foreach (MESDPToDatabase s in peerinsert)
                {
                    string id = Database.ID();
                    batchlist1.Add(j.Format("({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})",
                        id, nodeID, s.SDP, s.Status, s.Protocol, s.FarEnd, s.AdmMTU, s.Type, s.LSP
                        ));

                    if (batchlist1.Count == 50)
                    {
                        newpeer += j.Execute(peerinsertsql + string.Join(",", batchlist1.ToArray())).AffectedRows;
                        batchlist1.Clear();
                    }
                }
                if (batchlist1.Count > 0)
                    newpeer += j.Execute(peerinsertsql + string.Join(",", batchlist1.ToArray())).AffectedRows;
                if (newpeer > 0)
                    Event(newpeer + " peer(s) added");

                // MODIFY
                batchline = 0;
                batchlist1.Clear();
                batchstring.Clear();

                int modifiedpeer = 0;
                foreach (MESDPToDatabase s in peerupdate)
                {
                    List<string> v = new List<string>();
                    if (s.UpdateStatus) v.Add("MS_Status = " + s.Status);
                    if (s.UpdateProtocol) v.Add("MS_Protocol = " + s.Protocol);
                    if (s.UpdateType) v.Add(j.Format("MS_Type = {0}", s.Type));
                    if (s.UpdateLSP) v.Add(j.Format("MS_LSP = {0}", s.LSP));
                    if (s.UpdateAdmMTU) v.Add(s.AdmMTU == 0 ? j.Format("MS_MTU = {0}", null) : ("MS_MTU = " + s.AdmMTU));
                    if (s.UpdateFarEnd) v.Add(j.Format("MS_IP = {0}", s.FarEnd));

                    if (v.Count > 0)
                    {
                        string q = j.Format("update MESDP set " + StringHelper.EscapeFormat(string.Join(",", v.ToArray())) + " where MS_ID = {0};", s.ID);
                        batchline++;
                        batchstring.AppendLine(q);

                        if (batchline == 50)
                        {
                            modifiedpeer += j.Execute(batchstring.ToString()).AffectedRows;
                            batchstring.Clear();
                            batchline = 0;
                        }
                    }
                }
                if (batchline > 0)
                    modifiedpeer += j.Execute(batchstring.ToString()).AffectedRows;
                if (modifiedpeer > 0)
                    Event(modifiedpeer + " peer(s) modified");

                #endregion
            }

            peerdbresult = j.Query("select * from MESDP where MS_NO = {0}", nodeID);
            peerdb = new Dictionary<string, Row>();
            foreach (Row sdbo in peerdbresult) { peerdb.Add(sdbo["MS_SDP"].ToString(), sdbo); }
                
            #endregion

            #region SERVICE
            
            string circuitinsertsql = "insert into MECircuit(MC_ID, MC_NO, MC_VCID, MC_Type, MC_Status, MC_Protocol, MC_MU, MC_Description, MC_MTU) values";
            string circuitdeletesql = "delete from MECircuit where ";
            string circuitremoverefsql = "update MEPeer set MP_TO_MC = null, MP_TO_Check = null where ";
            Result circuitdbresult = j.Query("select * from MECircuit where MC_NO = {0}", nodeID);
            Dictionary<string, Row> circuitdb = new Dictionary<string, Row>();                
            Dictionary<string, MECircuitToDatabase> circuitlive = new Dictionary<string, MECircuitToDatabase>();
            List<MECircuitToDatabase> circuitinsert = new List<MECircuitToDatabase>();
            List<MECircuitToDatabase> circuitupdate = new List<MECircuitToDatabase>();

            List<string> cccircuit = new List<string>();

            // hwe only
            List<string[]> circuitethernetdetail = null;

            if (feature == null || feature == "service" || feature == "interface")
            {
                Event("Checking Service");
                
                #region Live

                if (nodeManufacture == alu)
                {
                    #region alu

                    foreach (Row vdbo in circuitdbresult)
                    {
                        string vcid = vdbo["MC_VCID"].ToString();
                        string mcid = vdbo["MC_ID"].ToString();

                        if (circuitdb.ContainsKey(vcid))
                        {
                            Result orx = j.Query(@"
select MC_ID, MI_ID from MECircuit
left join MEInterface on MI_MC = MC_ID
where
MC_NO = {0} and MC_VCID = {1}
", nodeID, vcid);

                            if (orx.Count >= 2)
                            {
                                string insertedmcid = circuitdb[vcid]["MC_ID"].ToString();

                                bool ygSudahDiinsertBener = false;
                                foreach (Row orxr in orx)
                                {
                                    string cmcid = orxr["MC_ID"].ToString();
                                    string cmiid = orxr["MI_ID"].ToString();

                                    if (cmcid == insertedmcid)
                                    {
                                        if (cmiid != null) { ygSudahDiinsertBener = true; break; }
                                    }
                                    else if (cmcid == mcid)
                                    {
                                        if (cmiid != null) { break; }
                                    }
                                }

                                if (ygSudahDiinsertBener)
                                {
                                    // mcid has no mi, remove mcid
                                    cccircuit.Add(j.Format("{0}", mcid));
                                }
                                else
                                {
                                    circuitdb.Remove(vcid); // remove yg sudah ada, diganti mcid yg ini
                                    cccircuit.Add(j.Format("{0}", insertedmcid));
                                    circuitdb.Add(vcid, vdbo);
                                }
                            }
                        }
                        else
                            circuitdb.Add(vcid, vdbo);
                    }

                    if (cccircuit.Count > 0)
                    {
                        Event("Duplicate Service(s) found (" + cccircuit.Count + "), began deleting...");
                        string dsql = string.Format("delete from MECircuit where MC_ID in ({0})", string.Join(",", cccircuit));
                        Result rex = j.Execute(dsql);
                        Event("" + rex.AffectedRows + " entries deleted.");
                    }



                    Result vdbcr = j.Query("select * from MECustomer where MU_NO = {0}", nodeID);
                    Dictionary<string, string> vdbc = new Dictionary<string, string>();
                    foreach (Row vdbco in vdbcr) { vdbc.Add(vdbco["MU_UID"].ToString(), vdbco["MU_ID"].ToString()); }

                    // STEP 1, dari display config untuk epipe dan vpls, biar dapet mtu dan deskripsinya
                    if (Send("admin display-config | match customer context children")) { NodeSaveMainLoopRestart(); return true; }
                    bool timeout;
                    List<string> lines = NodeRead(out timeout);
                    if (requestFailure) { requestFailure = false; MainLoopRestart(); return true; }
                    if (timeout) { NodeReadTimeOutExit(); return true; }

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

                            cservice = new MECircuitToDatabase();
                            string[] olinex = oline.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                            if (olinex.Length >= 5 && vdbc.ContainsKey(olinex[3]))
                            {
                                cservice.Type = (olinex[0][0] + "").ToUpper();
                                cservice.VCID = olinex[1];
                                cservice.CustomerID = vdbc[olinex[3]];
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
                    if (Send("show service service-using")) { NodeSaveMainLoopRestart(); return true; }
                    lines = NodeRead(out timeout);
                    if (requestFailure) { requestFailure = false; MainLoopRestart(); return true; }
                    if (timeout) { NodeReadTimeOutExit(); return true; }

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
                                    if (vdbc.ContainsKey(linex[4])) service.CustomerID = vdbc[linex[4]];
                                    else service.CustomerID = null;
                                    circuitlive.Add(linex[0], service);
                                }
                                service.Status = linex[2] == "Up" ? 1 : 0;
                                service.Protocol = linex[3] == "Up" ? 1 : 0;
                            }
                        }
                    }

                    #endregion
                }
                else if (nodeManufacture == hwe)
                {
                    #region hwe

                    foreach (Row vdbo in circuitdbresult) { circuitdb.Add(vdbo["MC_Description"].ToString(), vdbo); }
                    // display vsi verbose | in VSI
                    // display mpls l2vc brief

                    // STEP 1, VSI Name dan VSI ID
                    if (Send("display vsi verbose | in VSI Name|VSI ID")) { NodeSaveMainLoopRestart(); return true; }
                    bool timeout;
                    List<string> lines = NodeRead(out timeout);
                    if (requestFailure) { requestFailure = false; MainLoopRestart(); return true; }
                    if (timeout) { NodeReadTimeOutExit(); return true; }

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
                                cservice.Status = 1;
                                cservice.Protocol = 1;
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
                    if (Send("display vsi")) { NodeSaveMainLoopRestart(); return true; }
                    lines = NodeRead(out timeout);
                    if (requestFailure) { requestFailure = false; MainLoopRestart(); return true; }
                    if (timeout) { NodeReadTimeOutExit(); return true; }

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
                                    int state = linex[5] == "up" ? 1 : 0;
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
                    circuitethernetdetail = new List<string[]>();

                    //display mpls l2vc | in client interface|VC ID|local VC MTU|destination
                    if (Send("display mpls l2vc | in client interface|VC ID|local VC MTU|destination")) { NodeSaveMainLoopRestart(); return true; }
                    lines = NodeRead(out timeout);
                    if (requestFailure) { requestFailure = false; MainLoopRestart(); return true; }
                    if (timeout) { NodeReadTimeOutExit(); return true; }

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
                                            int state = cinterfacestate ? 1 : 0;
                                            cu.Status = state;
                                            cu.Protocol = state;
                                            cu.AdmMTU = cmtu;

                                            circuitlive.Add(vcidname, cu);
                                        }

                                        circuitethernetdetail.Add(new string[] { cinterface, cinterfaceSDP, cinterfacestate.ToString(), vcidname, cinterfaceVCID });

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
                            int state = cinterfacestate ? 1 : 0;
                            cu.Status = state;
                            cu.Protocol = state;
                            cu.AdmMTU = cmtu;

                            circuitlive.Add(vcidname, cu);
                        }

                        circuitethernetdetail.Add(new string[] { cinterface, cinterfaceSDP, cinterfacestate.ToString(), vcidname, cinterfaceVCID });
                    }

                    #endregion
                }

                #endregion

                #region Check

                foreach (KeyValuePair<string, MECircuitToDatabase> pair in circuitlive)
                {
                    if (!circuitdb.ContainsKey(pair.Key))
                    {
                        // ADD
                        circuitinsert.Add(pair.Value);
                        Event("Service ADD: " + pair.Key);
                    }
                    else
                    {
                        // MODIFY
                        Row db = circuitdb[pair.Key];

                        MECircuitToDatabase u = new MECircuitToDatabase();
                        u.ID = db["MC_ID"].ToString();

                        MECircuitToDatabase li = pair.Value;
                        bool update = false;

                        StringBuilder updateinfo = new StringBuilder();

                        if ((db["MC_Status"].ToBoolean() ? 1 : 0) != li.Status)
                        {
                            update = true;
                            u.UpdateStatus = true;
                            u.Status = li.Status;
                            updateinfo.Append("stat ");
                        }
                        if ((db["MC_Protocol"].ToBoolean() ? 1 : 0) != li.Protocol)
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
                        }
                        if ((db["MC_MTU"].IsNull ? 0 : db["MC_MTU"].ToSmall()) != li.AdmMTU)
                        {
                            update = true;
                            u.UpdateAdmMTU = true;
                            u.AdmMTU = li.AdmMTU;
                            updateinfo.Append("mtu ");
                        }
                        if (update)
                        {
                            Event("Service MODIFY: " + pair.Key + " " + updateinfo.ToString());
                            circuitupdate.Add(u);
                        }
                    }
                }

                #endregion

                #region Execute

                // ADD
                batchline = 0;
                batchlist1.Clear();
                batchstring.Clear();

                int newservice = 0;
                foreach (MECircuitToDatabase s in circuitinsert)
                {
                    //(MC_ID, MC_NO, MC_VCID, MC_Type, MC_Status, MC_Protocol, MC_MU, MC_Description, MC_MTU)
                    string id = Database.ID();
                    batchlist1.Add(j.Format("({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8})",
                        id, nodeID, s.VCID, s.Type, s.Status, s.Protocol, s.CustomerID, s.Description, s.AdmMTU
                        ));

                    if (batchlist1.Count == 50)
                    {
                        newservice += j.Execute(circuitinsertsql + string.Join(",", batchlist1.ToArray())).AffectedRows;
                        batchlist1.Clear();
                    }
                }
                if (batchlist1.Count > 0)
                    newservice += j.Execute(circuitinsertsql + string.Join(",", batchlist1.ToArray())).AffectedRows;
                if (newservice > 0)
                    Event(newservice + " service(s) added");

                // MODIFY
                batchline = 0;
                batchlist1.Clear();
                batchstring.Clear();

                int modifiedservice = 0;
                foreach (MECircuitToDatabase s in circuitupdate)
                {
                    List<string> v = new List<string>();
                    if (s.UpdateStatus) v.Add("MC_Status = " + s.Status);
                    if (s.UpdateProtocol) v.Add("MC_Protocol = " + s.Protocol);
                    if (s.UpdateType) v.Add(j.Format("MC_Type = {0}", s.Type));
                    if (s.UpdateDescription)
                    {
                        v.Add(j.Format("MC_Description = {0}", s.Description));
                        v.Add("MC_SE = null");
                        v.Add("MC_SE_Check = null");
                    }
                    if (s.UpdateAdmMTU) v.Add(s.AdmMTU == 0 ? j.Format("MC_MTU = {0}", null) : ("MC_MTU = " + s.AdmMTU));
                    if (s.UpdateCustomer) v.Add(j.Format("MC_MU = {0}", s.CustomerID));

                    if (v.Count > 0)
                    {
                        string q = j.Format("update MECircuit set " + StringHelper.EscapeFormat(string.Join(",", v.ToArray())) + " where MC_ID = {0};", s.ID);
                        batchline++;
                        batchstring.AppendLine(q);

                        if (batchline == 50)
                        {
                            modifiedservice += j.Execute(batchstring.ToString()).AffectedRows;
                            batchstring.Clear();
                            batchline = 0;
                        }
                    }
                }
                if (batchline > 0)
                    modifiedservice += j.Execute(batchstring.ToString()).AffectedRows;
                if (modifiedservice > 0)
                    Event(modifiedservice + " service(s) modified");

                #endregion
            }

            circuitdbresult = j.Query("select * from MECircuit where MC_NO = {0}", nodeID);
            circuitdb = new Dictionary<string, Row>();

            if (nodeManufacture == alu) foreach (Row vdbo in circuitdbresult) { circuitdb.Add(vdbo["MC_VCID"].ToString(), vdbo); }
            else if (nodeManufacture == hwe) foreach (Row vdbo in circuitdbresult) { circuitdb.Add(vdbo["MC_Description"].ToString(), vdbo); }

            #endregion

            #region SERVICE PEER

            string servpeerinsertsql = "insert into MEPeer(MP_ID, MP_MC, MP_MS, MP_VCID, MP_Protocol, MP_Type) values";
            string servpeerdeletesql = "delete from MEPeer where ";
            Result servpeerdbresult = j.Query("select * from MEPeer, MECircuit, MESDP where MP_MC = MC_ID and MP_MS = MS_ID and MC_NO = {0}", nodeID);
            Dictionary<string, Row> servpeerdb = new Dictionary<string, Row>();

            List<string> sdpvcidduplicate = new List<string>();
            foreach (Row row in servpeerdbresult) 
            { 
                string sdpvcid = row["MS_SDP"].ToString() + ":" + row["MP_VCID"].ToString();
                if (servpeerdb.ContainsKey(sdpvcid))
                    sdpvcidduplicate.Add(row["MP_ID"].ToString());
                else servpeerdb.Add(sdpvcid, row);
            }

            if (sdpvcidduplicate.Count > 0)
            {
                Event("Duplicate SDP:VCID(s) found (" + sdpvcidduplicate.Count + "), began deleting...");

                int sdpvcidduplicatedeleted = 0;

                batchlist1.Clear();
                for (int i = 0; i < sdpvcidduplicate.Count; i++)
                {
                    batchlist1.Add(sdpvcidduplicate[i]);

                    if (batchlist1.Count >= 50)
                    {
                        sdpvcidduplicatedeleted += j.Execute("delete from MEPeer where MP_ID in ('" + string.Join("','", batchlist1.ToArray()) + "')").AffectedRows;
                        batchlist1.Clear();
                    }
                }
                if (batchlist1.Count > 0)
                    sdpvcidduplicatedeleted += j.Execute("delete from MEPeer where MP_ID in ('" + string.Join("','", batchlist1.ToArray()) + "')").AffectedRows;

                Event(sdpvcidduplicatedeleted + " entries deleted.");
            }

            Dictionary<string, MECircuitSDPToDatabase> servpeerlive = new Dictionary<string, MECircuitSDPToDatabase>();
            List<MECircuitSDPToDatabase> servpeerinsert = new List<MECircuitSDPToDatabase>();
            List<MECircuitSDPToDatabase> servpeerupdate = new List<MECircuitSDPToDatabase>();

            if (feature == null || feature == "servicepeer")
            {
                Event("Checking Service Peer");

                #region Live

                if (nodeManufacture == alu)
                {
                    #region alu

                    if (Send("show service sdp-using")) { NodeSaveMainLoopRestart(); return true; }
                    bool timeout;
                    List<string> lines = NodeRead(out timeout);
                    if (requestFailure) { requestFailure = false; MainLoopRestart(); return true; }
                    if (timeout) { NodeReadTimeOutExit(); return true; }

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
                                        MECircuitSDPToDatabase c = new MECircuitSDPToDatabase();

                                        if (circuitdb.ContainsKey(linex[0])) c.CircuitID = circuitdb[linex[0]]["MC_ID"].ToString();
                                        else c.CircuitID = null;

                                        string[] sdpvcid = linex[1].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                                        c.SDP = sdpvcid[0];
                                        c.VCID = sdpvcid[1];

                                        if (peerdb.ContainsKey(sdpvcid[0])) c.SDPID = peerdb[c.SDP]["MS_ID"].ToString();
                                        else c.SDPID = null;

                                        c.Type = linex[2][0] + "";
                                        c.Protocol = linex[4] == "Up" ? 1 : 0;

                                        if (c.CircuitID != null && c.SDPID != null)
                                            servpeerlive.Add(c.SDP + ":" + c.VCID, c);
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
                    if (Send("display vsi peer-info")) { NodeSaveMainLoopRestart(); return true; }
                    bool timeout;
                    List<string> lines = NodeRead(out timeout);
                    if (requestFailure) { requestFailure = false; MainLoopRestart(); return true; }
                    if (timeout) { NodeReadTimeOutExit(); return true; }

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
                                    MECircuitSDPToDatabase c = new MECircuitSDPToDatabase();
                                    c.CircuitID = cvsi;

                                    c.SDP = linex[0];
                                    c.VCID = linex[1];

                                    if (peerdb.ContainsKey(linex[0])) c.SDPID = peerdb[c.SDP]["MS_ID"].ToString();
                                    else c.SDPID = null;

                                    c.Type = "M";
                                    c.Protocol = linex[4] == "up" ? 1 : 0;

                                    if (c.CircuitID != null && c.SDPID != null)
                                    {
                                        string comb = c.SDP + ":" + c.VCID;
                                        if (!servpeerlive.ContainsKey(comb))
                                            servpeerlive.Add(comb, c);
                                    }
                                }
                            }
                        }
                    }

                    // peernya mpls l2vc
                    foreach (string[] strs in circuitethernetdetail)
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
                            MECircuitSDPToDatabase c = new MECircuitSDPToDatabase();
                            c.CircuitID = cid;

                            c.SDP = sdp;
                            c.VCID = vcid;

                            if (peerdb.ContainsKey(sdp)) c.SDPID = peerdb[sdp]["MS_ID"].ToString();
                            else c.SDPID = null;

                            c.Type = "S";
                            c.Protocol = strs[2] == "True" ? 1 : 0;

                            if (c.CircuitID != null && c.SDPID != null)
                            {
                                string comb = c.SDP + ":" + c.VCID;
                                if (!servpeerlive.ContainsKey(comb))
                                    servpeerlive.Add(comb, c);
                            }
                        }
                    }

                    #endregion
                }

                #endregion

                #region Check

                foreach (KeyValuePair<string, MECircuitSDPToDatabase> pair in servpeerlive)
                {
                    if (!servpeerdb.ContainsKey(pair.Key))
                    {
                        // ADD
                        servpeerinsert.Add(pair.Value);
                        Event("Service Peer ADD: " + pair.Key);
                    }
                    else
                    {
                        // MODIFY
                        Row db = servpeerdb[pair.Key];

                        MECircuitSDPToDatabase u = new MECircuitSDPToDatabase();
                        u.ID = db["MP_ID"].ToString();

                        MECircuitSDPToDatabase li = pair.Value;
                        bool update = false;

                        StringBuilder updateinfo = new StringBuilder();

                        if ((db["MP_Protocol"].ToBoolean() ? 1 : 0) != li.Protocol)
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
                        if (update)
                        {
                            Event("Service Peer MODIFY: " + pair.Key + " " + updateinfo.ToString());
                            servpeerupdate.Add(u);
                        }
                    }
                }

                #endregion

                #region Execute

                // ADD
                batchline = 0;
                batchlist1.Clear();
                batchstring.Clear();

                int newservpeer = 0;
                foreach (MECircuitSDPToDatabase s in servpeerinsert)
                {
                    string id = Database.ID();

                    //MP_ID, MP_MC, MP_MS, MP_VCID, MP_Protocol, MP_Type
                    batchlist1.Add(j.Format("({0}, {1}, {2}, {3}, {4}, {5})",
                        id, s.CircuitID, s.SDPID, s.VCID, s.Protocol, s.Type
                        ));

                    if (batchlist1.Count == 50)
                    {
                        newservpeer += j.Execute(servpeerinsertsql + string.Join(",", batchlist1.ToArray())).AffectedRows;
                        batchlist1.Clear();
                    }
                }
                if (batchlist1.Count > 0)
                    newservpeer += j.Execute(servpeerinsertsql + string.Join(",", batchlist1.ToArray())).AffectedRows;
                if (newservpeer > 0)
                    Event(newservpeer + " service peer(s) added");

                // MODIFY
                batchline = 0;
                batchlist1.Clear();
                batchstring.Clear();

                int modifiedservpeer = 0;
                foreach (MECircuitSDPToDatabase s in servpeerupdate)
                {
                    List<string> v = new List<string>();
                    if (s.UpdateProtocol) v.Add("MP_Protocol = " + s.Protocol);
                    if (s.UpdateType) v.Add(j.Format("MP_Type = {0}", s.Type));

                    if (v.Count > 0)
                    {
                        string q = j.Format("update MEPeer set " + StringHelper.EscapeFormat(string.Join(",", v.ToArray())) + " where MP_ID = {0};", s.ID);
                        batchline++;
                        batchstring.AppendLine(q);

                        if (batchline == 50)
                        {
                            modifiedservpeer += j.Execute(batchstring.ToString()).AffectedRows;
                            batchstring.Clear();
                            batchline = 0;
                        }
                    }
                }
                if (batchline > 0)
                    modifiedservpeer += j.Execute(batchstring.ToString()).AffectedRows;
                if (modifiedservpeer > 0)
                    Event(modifiedservpeer + " service peer(s) modified");

                // DELETE
                batchline = 0;
                batchlist1.Clear();
                batchstring.Clear();

                int deletedservpeer = 0;
                foreach (KeyValuePair<string, Row> pair in servpeerdb)
                {
                    bool delete = false;

                    if (!servpeerlive.ContainsKey(pair.Key)) delete = true;
                    else
                    {
                        string livecid = servpeerlive[pair.Key].CircuitID;
                        string dbcid = pair.Value["MP_MC"].ToString();

                        if (livecid != dbcid) delete = true;
                    }

                    if (delete)
                    {
                        // DELETE
                        batchlist1.Add(j.Format("MP_ID = {0}", pair.Value["MP_ID"].ToString()));
                        Event("Service Peer DELETE: " + pair.Key);
                    }
                }
                if (batchlist1.Count > 0)
                {
                    foreach (string s in batchlist1)
                    {
                        if (batchline % 20 == 0)
                        {
                            if (batchstring.Length > 0)
                            {
                                deletedservpeer += j.Execute(servpeerdeletesql + batchstring.ToString()).AffectedRows;
                                batchstring.Clear();
                            }
                            batchstring.Append(s);
                        }
                        else batchstring.Append(" or " + s);
                        batchline++;
                    }
                    if (batchstring.Length > 0)
                        deletedservpeer += j.Execute(servpeerdeletesql + batchstring.ToString()).AffectedRows;
                }
                if (deletedservpeer > 0)
                    Event(deletedservpeer + " service peer(s) deleted");

                #endregion
            }
            
            #region Late Execute

            // DELETE PEER
            if (feature == null || feature == "peer")
            {
                batchline = 0;
                batchlist1.Clear();
                batchstring.Clear();

                int deletedpeer = 0;
                foreach (KeyValuePair<string, Row> pair in peerdb)
                {
                    if (!peerlive.ContainsKey(pair.Key))
                    {
                        batchlist1.Add(j.Format("MS_ID = {0}", pair.Value["MS_ID"].ToString()));
                        Event("Peer DELETE: " + pair.Key);
                    }
                }
                if (batchlist1.Count > 0)
                {
                    foreach (string s in batchlist1)
                    {
                        if (batchline % 20 == 0)
                        {
                            if (batchstring.Length > 0)
                            {
                                deletedpeer += j.Execute(peerdeletesql + batchstring.ToString()).AffectedRows;
                                batchstring.Clear();
                            }
                            batchstring.Append(s);
                        }
                        else batchstring.Append(" or " + s);
                        batchline++;
                    }
                    if (batchstring.Length > 0)
                        deletedpeer += j.Execute(peerdeletesql + batchstring.ToString()).AffectedRows;
                }
                if (deletedpeer > 0)
                    Event(deletedpeer + " peer(s) deleted");
            }

            #endregion

            #endregion

            #region INTERFACE

            string interfacesinsertsql = "insert into MEInterface(MI_ID, MI_NO, MI_Name, MI_Status, MI_Protocol, MI_Aggregator, MI_Description, MI_MC, MI_Type, MI_MQ_Input, MI_MQ_Output, MI_Rate_Input, MI_Rate_Output, MI_Used, MI_Info) values";
            string interfacesdeletesql = "delete from MEInterface where ";
            string interfaceremoveref1sql = "update MEInterface set MI_MI = null where ";
            string interfaceremoveref2sql = "update PEInterface set PI_TO_MI = null where ";
            Result interfacedbresult = j.Query("select * from MEInterface where MI_NO = {0}", nodeID);
            Dictionary<string, Row> interfacedb = new Dictionary<string, Row>();
            List<string> interfaceduplicate = new List<string>();
            foreach (Row row in interfacedbresult)
            {
                string miname = row["MI_Name"].ToString();
                if (interfacedb.ContainsKey(miname))
                    interfaceduplicate.Add(row["MI_ID"].ToString());
                else
                    interfacedb.Add(miname, row);
            }
            if (interfaceduplicate.Count > 0)
            {
                Event("Duplicate interface found (" + interfaceduplicate.Count + "), began deleting...");

                int interfaceduplicatedeleted = 0;

                batchlist1.Clear();
                for (int i = 0; i < interfaceduplicate.Count; i++)
                {
                    batchlist1.Add(interfaceduplicate[i]);

                    if (batchlist1.Count >= 50)
                    {
                        interfaceduplicatedeleted += j.Execute("delete from MEInterface where MI_ID in ('" + string.Join("','", batchlist1.ToArray()) + "')").AffectedRows;
                        batchlist1.Clear();
                    }
                }
                if (batchlist1.Count > 0)
                    interfaceduplicatedeleted += j.Execute("delete from MEInterface where MI_ID in ('" + string.Join("','", batchlist1.ToArray()) + "')").AffectedRows;

                Event(interfaceduplicatedeleted + " entries deleted.");
            }
            Dictionary<string, MEInterfaceToDatabase> interfaceslive = new Dictionary<string, MEInterfaceToDatabase>();
            List<MEInterfaceToDatabase> interfacesinsert = new List<MEInterfaceToDatabase>();
            List<MEInterfaceToDatabase> interfacesupdate = new List<MEInterfaceToDatabase>();

            if (feature == null || feature == "interface")
            {
                Event("Checking Interface");

                #region Live

                if (nodeManufacture == alu)
                {
                    #region alu

                    if (Send("show port description")) { NodeSaveMainLoopRestart(); return true; }
                    bool timeout;
                    List<string> lines = NodeRead(out timeout);
                    if (requestFailure) { requestFailure = false; MainLoopRestart(); return true; }
                    if (timeout) { NodeReadTimeOutExit(); return true; }

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
                                    if (!interfaceslive.ContainsKey(port))
                                    {
                                        MEInterfaceToDatabase mid = new MEInterfaceToDatabase();
                                        mid.Name = port;
                                        mid.Description = description.ToString();
                                        interfaceslive.Add(port, mid);
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
                                if (port != null)
                                {
                                    description.Append(line.TrimStart());
                                }
                            }
                            else
                            {
                                if (port != null)
                                {
                                    if (!interfaceslive.ContainsKey(port))
                                    {
                                        MEInterfaceToDatabase mid = new MEInterfaceToDatabase();
                                        mid.Name = port;
                                        mid.Description = description.ToString();

                                        interfaceslive.Add(port, mid);
                                    }
                                    port = null;
                                }
                            }
                        }
                    }
                    if (port != null)
                    {
                        if (!interfaceslive.ContainsKey(port))
                        {
                            MEInterfaceToDatabase mid = new MEInterfaceToDatabase();
                            mid.Name = port;
                            mid.Description = description.ToString();

                            interfaceslive.Add(port, mid);
                        }
                    }

                    if (Send("show port")) { NodeSaveMainLoopRestart(); return true; }
                    lines = NodeRead(out timeout);
                    if (requestFailure) { requestFailure = false; MainLoopRestart(); return true; }
                    if (timeout) { NodeReadTimeOutExit(); return true; }

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

                                    if (interfaceslive.ContainsKey(portex))
                                    {
                                        interfaceslive[portex].Status = (linex[1].Trim() == "Up") ? 1 : 0;

                                        string il3 = linex[3].Trim();
                                        if (il3 == "Link") il3 = "Up";

                                        interfaceslive[portex].Protocol = (il3 == "Up") ? 1 : 0;

                                        if (interfaceslive[portex].Status == 1 && interfaceslive[portex].Protocol == 1)
                                            interfaceslive[portex].Used = 1; // 1 1
                                        else
                                        {
                                            string desc = interfaceslive[portex].Description.Trim();
                                            if (desc != null && (
                                                        desc.ToUpper().StartsWith("RESERVED") ||
                                                        desc.ToUpper().StartsWith("TRUNK") ||
                                                        desc.ToUpper().StartsWith("REQUEST") ||
                                                        desc.ToUpper().StartsWith("BOOK") ||
                                                        desc.ToUpper().StartsWith("TO")
                                                        ))
                                                interfaceslive[portex].Used = 1;
                                            else
                                                interfaceslive[portex].Used = 0;
                                        }

                                        if (linex.Length >= 7)
                                        {
                                            string agr = linex[6].Trim();
                                            if (agr == "-") interfaceslive[portex].Aggr = -1;
                                            else
                                            {
                                                int agri;
                                                if (!int.TryParse(agr, out agri)) agri = -1;
                                                interfaceslive[portex].Aggr = agri;
                                            }

                                            if (agr != "-")
                                            {
                                                if (!interfaceslive.ContainsKey("Ag" + agr))
                                                {
                                                    MEInterfaceToDatabase mid = new MEInterfaceToDatabase();
                                                    mid.Name = "Ag" + agr;
                                                    interfaceslive.Add("Ag" + agr, mid);
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
                                                    interfaceslive[portex].InterfaceType = ity;
                                                }


                                                //3/1/1       Up    Yes  Up      9212 9212    - accs dotq xcme   GIGE-LX  10KM
                                                //1234567890123456789012345678901234567890123456789012345678901234567890123456789
                                                //         1         2         3         4         5         6   
                                                if (line.Length >= 64)
                                                {
                                                    string endinfo = line.Substring(63).Trim();

                                                    if (endinfo.Length > 0)
                                                        interfaceslive[portex].Info = endinfo;
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
                        if (Send("show service sap-using")) { NodeSaveMainLoopRestart(); return true; }
                        lines = NodeRead(out timeout);
                        if (requestFailure) { requestFailure = false; MainLoopRestart(); return true; }
                        if (timeout) { NodeReadTimeOutExit(); return true; }

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
                                        if (!interfaceslive.ContainsKey(name))
                                        {
                                            MEInterfaceToDatabase mid = new MEInterfaceToDatabase();
                                            mid.Name = name;
                                            interfaceslive.Add(name, mid);
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


                                    if (!interfaceslive.ContainsKey(thisport))
                                    {
                                        MEInterfaceToDatabase mid = new MEInterfaceToDatabase();
                                        mid.Name = thisport;
                                        mid.Status = (status == "Up") ? 1 : 0;
                                        mid.Protocol = (protocol == "Up") ? 1 : 0;
                                        mid.CircuitID = circuitID;
                                        mid.IngressID = ingressID;
                                        mid.EgressID = egressID;

                                        interfaceslive.Add(thisport, mid);

                                        if (interfaceslive.ContainsKey(name))
                                            interfaceslive[name].Used = 1;
                                    }
                                }
                            }
                        }

                    }
                    else
                    {
                        if (Send("show service sap-using description")) { NodeSaveMainLoopRestart(); return true; }
                        lines = NodeRead(out timeout);
                        if (requestFailure) { requestFailure = false; MainLoopRestart(); return true; }
                        if (timeout) { NodeReadTimeOutExit(); return true; }

                        port = null;
                        description = new StringBuilder();
                        string status = null;
                        string protocol = null;
                        string circuitID = null;
                        foreach (string line in lines)
                        {
                            if (line.Length > 0)
                            {
                                if (char.IsDigit(line[0]) || line.StartsWith("lag"))
                                {
                                    if (port != null)
                                    {
                                        if (!interfaceslive.ContainsKey(port))
                                        {
                                            string desc = description.ToString();
                                            if (desc == "(Not Specified)") desc = null;

                                            MEInterfaceToDatabase mid = new MEInterfaceToDatabase();
                                            mid.Name = port;
                                            mid.Description = desc;
                                            mid.Status = (status == "Up") ? 1 : 0;
                                            mid.Protocol = (protocol == "Up") ? 1 : 0;
                                            mid.CircuitID = circuitID;

                                            interfaceslive.Add(port, mid);
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
                                        if (!interfaceslive.ContainsKey(name))
                                        {
                                            MEInterfaceToDatabase mid = new MEInterfaceToDatabase();
                                            mid.Name = name;
                                            interfaceslive.Add(name, mid);
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
                                    // 0123456789012345678901234567890123456789012345678901234567890123456789
                                    //           1         2         3         4         5

                                    if (vlan == null) port = name + ".DIRECT";
                                    else port = name + "." + vlan;
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
                                        if (!interfaceslive.ContainsKey(port))
                                        {
                                            string desc = description.ToString();
                                            if (desc == "(Not Specified)") desc = null;

                                            MEInterfaceToDatabase mid = new MEInterfaceToDatabase();
                                            mid.Name = port;
                                            mid.Description = desc;
                                            mid.Status = (status == "Up") ? 1 : 0;
                                            mid.Protocol = (protocol == "Up") ? 1 : 0;
                                            mid.CircuitID = circuitID;

                                            interfaceslive.Add(port, mid);
                                        }
                                        port = null;
                                    }
                                }
                            }
                        }
                        if (port != null)
                        {
                            if (!interfaceslive.ContainsKey(port))
                            {
                                string desc = description.ToString();
                                if (desc == "(Not Specified)") desc = null;

                                MEInterfaceToDatabase mid = new MEInterfaceToDatabase();
                                mid.Name = port;
                                mid.Description = desc;
                                mid.Status = (status == "Up") ? 1 : 0;
                                mid.Protocol = (protocol == "Up") ? 1 : 0;
                                mid.CircuitID = circuitID;

                                interfaceslive.Add(port, mid);
                            }
                        }

                        if (Send("show service sap-using")) { NodeSaveMainLoopRestart(); return true; }
                        lines = NodeRead(out timeout);
                        if (requestFailure) { requestFailure = false; MainLoopRestart(); return true; }
                        if (timeout) { NodeReadTimeOutExit(); return true; }

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

                                    if (interfaceslive.ContainsKey(thisport))
                                    {
                                        if (ingressID != null)
                                            interfaceslive[thisport].IngressID = ingressID;
                                        if (egressID != null)
                                            interfaceslive[thisport].EgressID = egressID;

                                        if (interfaceslive.ContainsKey(name))
                                            interfaceslive[name].Used = 1;
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

                    if (Send("display interface description")) { NodeSaveMainLoopRestart(); return true; }
                    bool timeout;
                    List<string> lines = NodeRead(out timeout);
                    if (requestFailure) { requestFailure = false; MainLoopRestart(); return true; }
                    if (timeout) { NodeReadTimeOutExit(); return true; }

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
                                        if (!interfaceslive.ContainsKey(port))
                                        {
                                            MEInterfaceToDatabase mid = new MEInterfaceToDatabase();
                                            mid.Name = port;
                                            mid.Description = description.ToString();

                                            interfaceslive.Add(port, mid);
                                        }

                                        description = new StringBuilder();
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
                                    //Eth-Trunk1                    up      up       AGGR_PE2-D1-PBR-TRANSIT/ETH-TRUNK1_TO_T-D1-PBR/BE5_5x10G
                                    //012345678901234567890123456789012345678901234567
                                    //          1         2         3         4


                                    string descarea = null;
                                    if (nodeVersion == "5.90")
                                        descarea = line.Substring(30).TrimStart();
                                    else
                                        descarea = line.Substring(47).TrimStart();

                                    //descarea = descarea.TrimEnd(newline);

                                    string inf = line.Substring(0, 30).Trim();

                                    if (inf.StartsWith("Eth-Trunk")) port = "Ag" + inf.Substring(9);
                                    else
                                    {
                                        NodeInterface nif = NodeInterface.Parse(inf);
                                        if (nif != null) port = nif.GetShort();
                                    }

                                    if (port != null)
                                    {
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
                        if (!interfaceslive.ContainsKey(port))
                        {
                            MEInterfaceToDatabase mid = new MEInterfaceToDatabase();
                            mid.Name = port;
                            mid.Description = description.ToString();

                            interfaceslive.Add(port, mid);
                        }

                        description = new StringBuilder();
                        port = null;
                    }

                    if (Send("display interface brief")) { NodeSaveMainLoopRestart(); return true; }
                    lines = NodeRead(out timeout);
                    if (requestFailure) { requestFailure = false; MainLoopRestart(); return true; }
                    if (timeout) { NodeReadTimeOutExit(); return true; }

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
                                        pot = nif.GetShortType();
                                        issif = nif.IsSubInterface;
                                    }
                                }

                                if (poe != null)
                                {
                                    if (interfaceslive.ContainsKey(poe))
                                    {
                                        interfaceslive[poe].Status = (pstat == "Up") ? 1 : 0;
                                        interfaceslive[poe].Protocol = (pprot == "Up") ? 1 : 0;

                                        if (issif == false)
                                        {
                                            if (pot == "Gi" && line.IndexOf("(10G)") > -1) pot = "Te";
                                            interfaceslive[poe].InterfaceType = pot;
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
                                        if (interfaceslive.ContainsKey(portnif))
                                        {
                                            int agr;
                                            if (!int.TryParse(aggre, out agr)) agr = -1;
                                            interfaceslive[portnif].Aggr = agr;
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
                    foreach (string[] strs in circuitethernetdetail)
                    {
                        // cinterface, cinterfaceSDP, cinterfacestate.ToString(), vcidname, cinterfaceVCID
                        //  0            1              2                          3         4

                        string vcidname = strs[3];
                        string vcid = strs[4];
                        string sdp = strs[1];
                        string inf = strs[0];

                        if (circuitdb.ContainsKey(vcidname))
                        {
                            if (interfaceslive.ContainsKey(inf))
                            {
                                string cid = circuitdb[vcidname]["MC_ID"].ToString();
                                interfaceslive[inf].CircuitID = cid;
                            }
                        }
                    }

                    // vsi ke port (l2 binding vsi)
                    if (Send("display vsi services all")) { NodeSaveMainLoopRestart(); return true; }
                    lines = NodeRead(out timeout);
                    if (requestFailure) { requestFailure = false; MainLoopRestart(); return true; }
                    if (timeout) { NodeReadTimeOutExit(); return true; }

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

                                if (interfaceslive.ContainsKey(portnif))
                                {
                                    if (circuitdb.ContainsKey(vsi))
                                    {
                                        string cid = circuitdb[vsi]["MC_ID"].ToString();
                                        interfaceslive[portnif].CircuitID = cid;
                                    }
                                }
                            }
                        }
                    }

                    // qos
                    if (Send("display cur int | in interface |qos-profile |user-queue")) { NodeSaveMainLoopRestart(); return true; }
                    lines = NodeRead(out timeout);
                    if (requestFailure) { requestFailure = false; MainLoopRestart(); return true; }
                    if (timeout) { NodeReadTimeOutExit(); return true; }

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

                            if (nifs != null && interfaceslive.ContainsKey(nifs))
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

                                        if (qosDir == "inbound") interfaceslive[qosInterface].IngressID = qosID;
                                        else if (qosDir == "outbound") interfaceslive[qosInterface].EgressID = qosID;
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

                                    if (rDir == "inbound") interfaceslive[qosInterface].RateLimitInput = size;
                                    else if (rDir == "outbound") interfaceslive[qosInterface].RateLimitOutput = size;
                                }
                            }
                        }

                    }

                    #endregion
                }

                #endregion

                #region Check

                foreach (KeyValuePair<string, MEInterfaceToDatabase> pair in interfaceslive)
                {
                    if (!interfacedb.ContainsKey(pair.Key))
                    {
                        // ADD
                        interfacesinsert.Add(pair.Value);
                        Event("Interface ADD: " + pair.Key);
                    }
                    else
                    {
                        Row db = interfacedb[pair.Key];

                        MEInterfaceToDatabase u = new MEInterfaceToDatabase();
                        u.ID = db["MI_ID"].ToString();

                        MEInterfaceToDatabase li = pair.Value;
                        bool update = false;

                        StringBuilder updateinfo = new StringBuilder();

                        if (db["MI_Description"].ToString() != li.Description)
                        {
                            update = true;
                            u.UpdateDescription = true;
                            u.Description = li.Description;
                            updateinfo.Append("desc ");
                        }
                        if ((db["MI_Status"].ToBoolean() ? 1 : 0) != li.Status)
                        {
                            update = true;
                            u.UpdateStatus = true;
                            u.Status = li.Status;
                            updateinfo.Append("stat ");
                        }
                        if ((db["MI_Protocol"].ToBoolean() ? 1 : 0) != li.Protocol)
                        {
                            update = true;
                            u.UpdateProtocol = true;
                            u.Protocol = li.Protocol;
                            updateinfo.Append("prot ");
                        }
                        if ((db["MI_Aggregator"].IsNull ? -1 : db["MI_Aggregator"].ToInt()) != li.Aggr)
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
                        if (db["MI_Used"].IsNull)
                        {
                            if (li.Used > -1)
                            {
                                update = true;
                                u.UpdateUsed = true;
                                u.Used = li.Used;
                                updateinfo.Append("used ");
                            }
                        }
                        else
                        {
                            if ((db["MI_Used"].ToBoolean() ? 1 : 0) != li.Used)
                            {
                                update = true;
                                u.UpdateUsed = true;
                                u.Used = li.Used;
                                updateinfo.Append("used ");
                            }
                        }
                        Column rateinput = db["MI_Rate_Input"];
                        if (rateinput.IsNull)
                        {
                            if (li.RateLimitInput > -1)
                            {
                                update = true;
                                u.UpdateRateLimitInput = true;
                                u.RateLimitInput = li.RateLimitInput;
                                updateinfo.Append("rin ");
                            }
                        }
                        else
                        {
                            if (li.RateLimitInput != rateinput.ToInt())
                            {
                                update = true;
                                u.UpdateRateLimitInput = true;
                                u.RateLimitInput = li.RateLimitInput;
                                updateinfo.Append("rin ");
                            }
                        }
                        Column rateoutput = db["MI_Rate_Output"];
                        if (rateoutput.IsNull)
                        {
                            if (li.RateLimitOutput > -1)
                            {
                                update = true;
                                u.UpdateRateLimitOutput = true;
                                u.RateLimitOutput = li.RateLimitOutput;
                                updateinfo.Append("rout ");
                            }
                        }
                        else
                        {
                            if (li.RateLimitOutput != rateoutput.ToInt())
                            {
                                update = true;
                                u.UpdateRateLimitOutput = true;
                                u.RateLimitOutput = li.RateLimitOutput;
                                updateinfo.Append("rout ");
                            }
                        }
                        if (db["MI_Info"].ToString() != li.Info)
                        {
                            update = true;
                            u.UpdateInfo = true;
                            u.Info = li.Info;
                            updateinfo.Append("info ");
                        }

                        if (update)
                        {
                            Event("Interface MODIFY: " + pair.Key + " " + updateinfo.ToString());
                            interfacesupdate.Add(u);
                        }
                    }
                }

                #endregion

                #region Execute

                // ADD
                batchline = 0;
                batchlist1.Clear();
                batchstring.Clear();

                int newinterface = 0;
                foreach (MEInterfaceToDatabase s in interfacesinsert)
                {
                    string miid = Database.ID();

                    batchlist1.Add(j.Format("({0}, {1}, {2}, {3}, {4}, " + (s.Aggr == -1 ? "null" : s.Aggr + "") + ", {5}, {6}, {7}, {8}, {9}, " + ((s.RateLimitInput == -1) ? "null" : (s.RateLimitInput + "")) + ", " + ((s.RateLimitOutput == -1) ? "null" : (s.RateLimitOutput + "")) + "," + ((s.Used == -1) ? "null" : (s.Used + "")) + ", {10})",
                        miid, nodeID, s.Name, s.Status, s.Protocol, s.Description, s.CircuitID, s.InterfaceType, s.IngressID, s.EgressID, s.Info));

                    if (batchlist1.Count >= 50)
                    {
                        newinterface += j.Execute(interfacesinsertsql + string.Join(",", batchlist1.ToArray())).AffectedRows;
                        batchlist1.Clear();
                    }
                }
                if (batchlist1.Count > 0)
                    newinterface += j.Execute(interfacesinsertsql + string.Join(",", batchlist1.ToArray())).AffectedRows;
                if (newinterface > 0)
                    Event(newinterface + " interface(s) added");

                // MODIFY       
                batchline = 0;
                batchlist1.Clear();
                batchstring.Clear();

                int modifiedinterface = 0;
                foreach (MEInterfaceToDatabase s in interfacesupdate)
                {
                    List<string> v = new List<string>();
                    if (s.UpdateDescription)
                    {
                        v.Add(j.Format("MI_Description = {0}", s.Description));
                        v.Add("MI_SE = null");
                        v.Add("MI_SE_Check = null");
                    }
                    if (s.UpdateStatus) v.Add("MI_Status = " + s.Status);
                    if (s.UpdateProtocol) v.Add("MI_Protocol = " + s.Protocol);
                    if (s.UpdateAggr)
                    {
                        if (s.Aggr == -1)
                            v.Add("MI_Aggregator = null");
                        else
                            v.Add("MI_Aggregator = " + s.Aggr);
                    }
                    if (s.UpdateCircuit) v.Add(j.Format("MI_MC = {0}", s.CircuitID));
                    if (s.UpdateInterfaceType) v.Add(j.Format("MI_Type = {0}", s.InterfaceType));
                    if (s.UpdateIngressID) v.Add(j.Format("MI_MQ_Input = {0}", s.IngressID));
                    if (s.UpdateEgressID) v.Add(j.Format("MI_MQ_Output = {0}", s.EgressID));
                    if (s.UpdateRateLimitInput)
                    {
                        if (s.RateLimitInput > -1) v.Add("MI_Rate_Input = " + s.RateLimitInput);
                        else v.Add("MI_Rate_Input = null");
                    }
                    if (s.UpdateRateLimitOutput)
                    {
                        if (s.RateLimitOutput > -1) v.Add("MI_Rate_Output = " + s.RateLimitOutput);
                        else v.Add("MI_Rate_Output = null");
                    }
                    if (s.UpdateUsed)
                    {
                        if (s.Used > -1) v.Add("MI_Used = " + s.Used);
                        else v.Add("MI_Used = null");
                    }
                    if (s.UpdateInfo) v.Add(j.Format("MI_Info = {0}", s.Info));

                    if (v.Count > 0)
                    {
                        string ustr = string.Join(",", v.ToArray());
                        string usql = j.Format("update MEInterface set " + StringHelper.EscapeFormat(ustr) + " where MI_ID = {0};", s.ID);
                        batchline++;
                        batchstring.AppendLine(usql);

                        if (batchline == 50)
                        {
                            modifiedinterface += j.Execute(batchstring.ToString()).AffectedRows;
                            batchstring.Clear();
                            batchline = 0;
                        }
                    }
                }
                if (batchline > 0)
                    modifiedinterface += j.Execute(batchstring.ToString()).AffectedRows;
                if (modifiedinterface > 0)
                    Event(modifiedinterface + " interface(s) modified");

                // DELETE
                batchline = 0;
                batchlist1.Clear();
                batchlist3.Clear();
                batchlist4.Clear();
                batchstring.Clear();

                int deletedinterface = 0;
                int removedrefinterface = 0;

                foreach (KeyValuePair<string, Row> pair in interfacedb)
                {
                    if (!interfaceslive.ContainsKey(pair.Key))
                    {
                        // DELETE
                        string miid = pair.Value["MI_ID"].ToString();
                        batchlist1.Add(j.Format("MI_ID = {0}", miid));
                        batchlist3.Add(j.Format("MI_MI = {0}", miid));
                        batchlist4.Add(j.Format("PI_TO_MI = {0}", miid));
                        Event("Interface DELETE: " + pair.Key);
                    }
                }

                foreach (string s in batchlist4)
                {
                    if (batchline % 20 == 0)
                    {
                        if (batchstring.Length > 0)
                        {
                            removedrefinterface += j.Execute(interfaceremoveref2sql + batchstring.ToString()).AffectedRows;
                            batchstring.Clear();
                        }
                        batchstring.Append(s);
                    }
                    else batchstring.Append(" or " + s);
                    batchline++;
                }
                if (batchstring.Length > 0)
                    removedrefinterface += j.Execute(interfaceremoveref2sql + batchstring.ToString()).AffectedRows;

                batchline = 0;
                batchstring.Clear();

                foreach (string s in batchlist3)
                {
                    if (batchline % 20 == 0)
                    {
                        if (batchstring.Length > 0)
                        {
                            removedrefinterface += j.Execute(interfaceremoveref1sql + batchstring.ToString()).AffectedRows;
                            batchstring.Clear();
                        }
                        batchstring.Append(s);
                    }
                    else batchstring.Append(" or " + s);
                    batchline++;
                }
                if (batchstring.Length > 0)
                    removedrefinterface += j.Execute(interfaceremoveref1sql + batchstring.ToString()).AffectedRows;

                batchline = 0;
                batchstring.Clear();

                foreach (string mii in batchlist1)
                {
                    if (batchline % 20 == 0)
                    {
                        if (batchstring.Length > 0)
                        {
                            deletedinterface += j.Execute(interfacesdeletesql + batchstring.ToString()).AffectedRows;
                            batchstring.Clear();
                        }
                        batchstring.Append(mii);
                    }
                    else batchstring.Append(" or " + mii);
                    batchline++;
                }
                if (batchstring.Length > 0)
                    deletedinterface += j.Execute(interfacesdeletesql + batchstring.ToString()).AffectedRows;

                if (deletedinterface > 0)
                    Event(deletedinterface + " interface(s) deleted");
                if (removedrefinterface > 0)
                    Event(removedrefinterface + " interface(s) reference updated");

                #endregion
            }
            
            #region Late Execute

            // DELETE PEER
            if (feature == null && feature == "peer")
            {
                batchline = 0;
                batchlist1.Clear();
                batchstring.Clear();
            }

            // DELETE SERVICE
            if (feature == null && feature == "service" && feature == "interface")
            {
                batchline = 0;
                batchlist1.Clear();
                batchlist2.Clear();
                batchstring.Clear();

                int deletedservice = 0;
                int removedrefservice = 0;

                foreach (KeyValuePair<string, Row> pair in circuitdb)
                {
                    if (!circuitlive.ContainsKey(pair.Key))
                    {
                        string mcid = pair.Value["MC_ID"].ToString();
                        batchlist1.Add(j.Format("MC_ID = {0}", mcid));
                        batchlist2.Add(j.Format("MP_TO_MC = {0}", mcid));
                        Event("Service DELETE: " + pair.Key);
                    }
                }

                foreach (string s in batchlist2)
                {
                    if (batchline % 20 == 0)
                    {
                        if (batchstring.Length > 0)
                        {
                            removedrefservice += j.Execute(circuitremoverefsql + batchstring.ToString()).AffectedRows;
                            batchstring.Clear();
                        }
                        batchstring.Append(s);
                    }
                    else batchstring.Append(" or " + s);
                    batchline++;
                }
                if (batchstring.Length > 0)
                    removedrefservice += j.Execute(circuitremoverefsql + batchstring.ToString()).AffectedRows;

                batchline = 0;
                batchstring.Clear();

                foreach (string s in batchlist1)
                {
                    if (batchline % 20 == 0)
                    {
                        if (batchstring.Length > 0)
                        {
                            deletedservice += j.Execute(circuitdeletesql + batchstring.ToString()).AffectedRows;
                            batchstring.Clear();
                        }
                        batchstring.Append(s);
                    }
                    else batchstring.Append(" or " + s);
                    batchline++;
                }
                if (batchstring.Length > 0)
                    deletedservice += j.Execute(circuitdeletesql + batchstring.ToString()).AffectedRows;

                if (deletedservice > 0)
                    Event(deletedservice + " service(s) deleted");
                if (removedrefservice > 0)
                    Event(removedrefservice + " service(s) reference updated");
            }

            // DELETE CUSTOMER
            if (feature == null || feature == "customer")
            {
                if (nodeManufacture == alu)
                {
                    batchline = 0;
                    batchlist1.Clear();
                    batchstring.Clear();

                    int deletedcustomer = 0;
                    foreach (KeyValuePair<string, string> pair in custdb)
                    {
                        if (!custlive.ContainsKey(pair.Key))
                        {
                            batchlist1.Add(j.Format("MU_ID = {0}", pair.Value));
                            Event("Customer DELETE: " + pair.Key);
                        }
                    }
                    if (batchlist1.Count > 0)
                    {
                        foreach (string c in batchlist1)
                        {
                            if (batchline % 20 == 0)
                            {
                                if (batchstring.Length > 0)
                                {
                                    deletedcustomer += j.Execute(custdeletesql + batchstring.ToString()).AffectedRows;
                                    batchstring.Clear();
                                }
                                batchstring.Append(c);
                            }
                            else batchstring.Append(" or " + c);
                            batchline++;
                        }
                        if (batchstring.Length > 0)
                            deletedcustomer += j.Execute(custdeletesql + batchstring.ToString()).AffectedRows;
                    }
                    if (deletedcustomer > 0)
                        Event(deletedcustomer + " customer(s) deleted");
                }
            }
            #endregion

            #endregion

            NodeSaveExit();

            return false;
        }
    }
}
