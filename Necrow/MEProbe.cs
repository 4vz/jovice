﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Aphysoft.Share;
using System.Globalization;

using Jovice;
using System.Net;

namespace Necrow
{
    #region To Database

    class MECustomerToDatabase : Data2
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
        #region Topology
        
        private MEInterfaceToDatabase[] aggrChilds = null;

        public MEInterfaceToDatabase[] AggrChilds
        {
            get { return aggrChilds; }
            set { aggrChilds = value; }
        }

        private string neighborCheckPITOMI = null;

        public string NeighborCheckPITOMI
        {
            get { return neighborCheckPITOMI; }
            set { neighborCheckPITOMI = value; }
        }

        private bool updateNeighborCheckPITOMI = false;

        public bool UpdateNeighborCheckPITOMI
        {
            get { return updateNeighborCheckPITOMI; }
            set { updateNeighborCheckPITOMI = value; }
        }

        private string neighborCheckMITOMI = null;

        public string NeighborCheckMITOMI
        {
            get { return neighborCheckMITOMI; }
            set { neighborCheckMITOMI = value; }
        }

        private bool updateNeighborCheckMITOMI = false;

        public bool UpdateNeighborCheckMITOMI
        {
            get { return updateNeighborCheckMITOMI; }
            set { updateNeighborCheckMITOMI = value; }
        }

        #endregion

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

        public string IngressID { get; set; } = null;

        public bool UpdateIngressID { get; set; } = false;

        public string EgressID { get; set; } = null;

        public bool UpdateEgressID { get; set; } = false;

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

    class MEQOSPolicyToDatabase : Data2
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
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
        public string VCID { get; set; }

        public string Type { get; set; }

        public bool UpdateType { get; set; } = false;

        public string CustomerID { get; set; }

        public bool UpdateCustomer { get; set; } = false;

        public int AdmMTU { get; set; } = -1;

        public bool UpdateAdmMTU { get; set; } = false;

        public List<string> AdjacentPeers { get; set; } = null;
    }

    class MEPeerToDatabase : StatusToDatabase
    {
        private string circuitID;

        public string CircuitID
        {
            get { return circuitID; }
            set { circuitID = value; }
        }

        private bool updateCircuitID = false;

        public bool UpdateCircuitID
        {
            get { return updateCircuitID; }
            set { updateCircuitID = value; }
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

    public sealed partial class Probe
    {
        private ProbeProcessResult MEProcess()
        {
            ProbeProcessResult probe = new ProbeProcessResult();

            string[] lines = null;
            Batch batch = j.Batch();
            Result2 result;

            #region ALU-CUSTOMER

            Dictionary<string, MECustomerToDatabase> alucustlive = new Dictionary<string, MECustomerToDatabase>();
            Dictionary<string, Row2> alucustdb = null;
            List<MECustomerToDatabase> alucustinsert = new List<MECustomerToDatabase>();
            List<MECustomerToDatabase> alucustupdate = new List<MECustomerToDatabase>();

            if (nodeManufacture == alu)
            {
                Event("Checking Circuit Customer");

                alucustdb = j.QueryDictionary("select * from MECustomer where MU_NO = {0}", "MU_UID", nodeID);
                if (alucustdb == null) return DatabaseFailure(probe);

                #region Live

                if (Request("show service customer | match \"Customer-ID\"", out lines, probe)) return probe;

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
                        li.Id = Database2.ID();
                        alucustinsert.Add(li);
                    }
                }

                #endregion

                #region Execute

                // ADD
                batch.Begin();
                foreach (MECustomerToDatabase s in alucustinsert)
                {
                    Insert insert = j.Insert("MECustomer");
                    insert.Value("MU_ID", s.Id);
                    insert.Value("MU_NO", nodeID);
                    insert.Value("MU_UID", s.CustomerID);
                    batch.Add(insert);
                }
                result = batch.Commit();
                if (!result) return DatabaseFailure(probe);
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
                if (!result) return DatabaseFailure(probe);
                Event(result, EventActions.Update, EventElements.ALUCustomer, false);

                #endregion

                alucustdb = j.QueryDictionary("select * from MECustomer where MU_NO = {0}", "MU_UID", nodeID);
                if (alucustdb == null) return DatabaseFailure(probe);
            }

            #endregion

            #region QOS

            Event("Checking QOS");

            Dictionary<string, MEQOSToDatabase> qoslive = new Dictionary<string, MEQOSToDatabase>();
            Dictionary<string, Row2> qosdb = j.QueryDictionary("select * from MEQOS where MQ_NO = {0}", delegate (Row2 row) { return (row["MQ_Type"].ToBool() ? "1" : "0") + "_" + row["MQ_Name"].ToString(); }, nodeID);
            if (qosdb == null) return DatabaseFailure(probe);
            List<MEQOSToDatabase> qosinsert = new List<MEQOSToDatabase>();
            List<MEQOSToDatabase> qosupdate = new List<MEQOSToDatabase>();

            Dictionary<string, MEQOSPolicyToDatabase> policylive = new Dictionary<string, MEQOSPolicyToDatabase>();
            Dictionary<string, Row2> policydb = j.QueryDictionary("select * from MEQOSPolicy where MQP_NO = {0}", "MQP_Name", nodeID);
            if (policydb == null) return DatabaseFailure(probe);
            List<MEQOSPolicyToDatabase> policyinsert = new List<MEQOSPolicyToDatabase>();

            #region Live

            if (nodeManufacture == alu)
            {
                #region alu

                if (Request("show qos sap-ingress", out lines, probe)) return probe;

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

                if (Request("show qos sap-egress", out lines, probe)) return probe;

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
                    AlcatelLucentQOS ni = AlcatelLucentQOS.Parse(li.Name);
                    li.Bandwidth = ni.Bandwidth;
                }

                if (Request("show qos scheduler-policy", out lines, probe)) return probe;

                bool capture = false;
                foreach (string line in lines)
                {
                    if (capture)
                    {
                        var st = line.Trim();

                        if (line.StartsWith("==="))
                        {
                            break;
                        }

                        MEQOSPolicyToDatabase qp = new MEQOSPolicyToDatabase();
                        qp.Name = st;
                        policylive.Add(st, qp);

                    }
                    else if (line.StartsWith("---"))
                    {
                        capture = true;
                    }
                }

                #endregion
            }
            else if (nodeManufacture == hwe)
            {
                #region hwe

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
                    HuaweiQOS ni = HuaweiQOS.Parse(li.Name);
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
                    li.Id = Database2.ID();
                    qosinsert.Add(li);
                }
                else
                {
                    Row2 db = qosdb[pair.Key];

                    MEQOSToDatabase u = new MEQOSToDatabase();
                    u.Id = db["MQ_ID"].ToString();

                    bool update = false;
                    StringBuilder updateinfo = new StringBuilder();

                    if (db["MQ_Bandwidth"].ToInt(-1) != li.Bandwidth)
                    {
                        update = true;
                        u.UpdateBandwidth = true;
                        u.Bandwidth = li.Bandwidth;
                        UpdateInfo(updateinfo, "bandwidth", db["MQ_Bandwidth"].ToInt(-1).NullableInfo("{0}K"), li.Bandwidth.NullableInfo("{0}K"));
                    }
                    if (update)
                    {
                        Event("QOS UPDATE: " + pair.Key + " " + updateinfo.ToString());
                        qosupdate.Add(u);
                    }
                }
            }

            foreach (KeyValuePair<string, MEQOSPolicyToDatabase> pair in policylive)
            {
                MEQOSPolicyToDatabase li = pair.Value;
                if (!policydb.ContainsKey(pair.Key))
                {
                    Event("QOS Policy ADD: " + pair.Key);
                    li.Id = Database2.ID();
                    policyinsert.Add(li);
                }
                else
                {
                }
            }


            #endregion

            #region Execute

            // ADD
            batch.Begin();
            foreach (MEQOSToDatabase s in qosinsert)
            {
                Insert insert = j.Insert("MEQOS");
                insert.Value("MQ_ID", s.Id);
                insert.Value("MQ_NO", nodeID);
                insert.Value("MQ_Name", s.Name);
                insert.Value("MQ_Type", s.Type.Nullable(-1));
                insert.Value("MQ_Bandwidth", s.Bandwidth.Nullable(-1));
                batch.Add(insert);
            }
            result = batch.Commit();
            if (!result) return DatabaseFailure(probe);
            Event(result, EventActions.Add, EventElements.QOS, false);

            // UPDATE
            batch.Begin();
            foreach (MEQOSToDatabase s in qosupdate)
            {
                Update update = j.Update("MEQOS");
                update.Set("MQ_Bandwidth", s.Bandwidth.Nullable(-1), s.UpdateBandwidth);
                update.Where("MQ_ID", s.Id);
                batch.Add(update);
            }
            result = batch.Commit();
            if (!result) return DatabaseFailure(probe);
            Event(result, EventActions.Update, EventElements.QOS, false);

            #endregion

            qosdb = j.QueryDictionary("select * from MEQOS where MQ_NO = {0}", delegate (Row2 row) { return (row["MQ_Type"].ToBool() ? "1" : "0") + "_" + row["MQ_Name"].ToString(); }, nodeID);
            if (qosdb == null) return DatabaseFailure(probe);


            // ADD
            batch.Begin();
            foreach (MEQOSPolicyToDatabase s in policyinsert)
            {
                Insert insert = j.Insert("MEQOSPolicy");
                insert.Value("MQP_ID", s.Id);
                insert.Value("MQP_NO", nodeID);
                insert.Value("MQP_Name", s.Name);
                batch.Add(insert);
            }
            result = batch.Commit();
            if (!result) return DatabaseFailure(probe);
            Event(result, EventActions.Add, EventElements.QOS, false);

            #endregion

            #region SDP

            Event("Checking SDP");

            Dictionary<string, MESDPToDatabase> sdplive = new Dictionary<string, MESDPToDatabase>();
            Dictionary<string, Row2> sdpdb = j.QueryDictionary("select * from MESDP where MS_NO = {0}", "MS_SDP", nodeID);
            if (sdpdb == null) return DatabaseFailure(probe);
            Dictionary<string, Row2> ipnodedb = j.QueryDictionary("select NO_IP, NO_ID from Node where NO_IP is not null", "NO_IP");
            if (ipnodedb == null) return DatabaseFailure(probe);
            List<MESDPToDatabase> sdpinsert = new List<MESDPToDatabase>();
            List<MESDPToDatabase> sdpupdate = new List<MESDPToDatabase>();

            #region Live

            if (nodeManufacture == alu)
            {
                #region alu

                if (Request("show service sdp", out lines, probe)) return probe;

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
                            if (int.TryParse(amtu, out iamtu)) d.AdmMTU = iamtu == 0 ? -1 : iamtu;
                            else d.AdmMTU = -1;
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
                if (Request("display mpls ldp remote-peer", out lines, probe)) return probe;

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
                            d.AdmMTU = -1;
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
                if (Request("display vsi verbose | in Peer Router ID", out lines, probe)) return probe;

                foreach (string line in lines)
                {
                    string lineTrim = line.Trim();
                    if (lineTrim.Length > 0)
                    {
                        string[] linex = lineTrim.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);
                        if (linex.Length == 2)
                        {
                            farend = linex[1].Trim();

                            bool isip = IPAddress.TryParse(farend, out IPAddress farendipaddress);

                            if (!sdplive.ContainsKey(farend) && isip)
                            {
                                MESDPToDatabase d = new MESDPToDatabase();
                                d.SDP = farend;
                                d.AdmMTU = -1;
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
                if (Request("display mpls l2vc | in destination", out lines, probe)) return probe;

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
                                d.AdmMTU = -1;
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
                    li.Id = Database2.ID();
                    sdpinsert.Add(li);
                }
                else
                {
                    Row2 db = sdpdb[pair.Key];

                    MESDPToDatabase u = new MESDPToDatabase();
                    u.Id = db["MS_ID"].ToString();

                    bool update = false;
                    StringBuilder updateinfo = new StringBuilder();

                    if (db["MS_Status"].ToBool() != li.Status)
                    {
                        update = true;
                        u.UpdateStatus = true;
                        u.Status = li.Status;
                        UpdateInfo(updateinfo, "status", db["MS_Status"].ToBool().DescribeUpDown(), li.Status.DescribeUpDown());
                    }
                    if (db["MS_Protocol"].ToBool() != li.Protocol)
                    {
                        update = true;
                        u.UpdateProtocol = true;
                        u.Protocol = li.Protocol;
                        UpdateInfo(updateinfo, "protocol", db["MS_Protocol"].ToBool().DescribeUpDown(), li.Protocol.DescribeUpDown());
                    }
                    if (db["MS_Type"].ToString() != li.Type)
                    {
                        update = true;
                        u.UpdateType = true;
                        u.Type = li.Type;
                        UpdateInfo(updateinfo, "type", db["MS_Type"].ToString(), li.Type);
                    }
                    if (db["MS_LSP"].ToString() != li.LSP)
                    {
                        update = true;
                        u.UpdateLSP = true;
                        u.LSP = li.LSP;
                        UpdateInfo(updateinfo, "lsp", db["MS_LSP"].ToString(), li.LSP);
                    }
                    if (db["MS_MTU"].ToIntShort(-1) != li.AdmMTU)
                    {
                        update = true;
                        u.UpdateAdmMTU = true;
                        u.AdmMTU = li.AdmMTU;
                        UpdateInfo(updateinfo, "mtu", db["MS_MTU"].ToIntShort(-1).NullableInfo(), li.AdmMTU.NullableInfo());
                    }
                    if (db["MS_IP"].ToString() != li.FarEnd)
                    {
                        update = true;
                        u.UpdateFarEnd = true;
                        u.FarEnd = li.FarEnd;
                        UpdateInfo(updateinfo, "remote-ip", db["MS_IP"].ToString(), li.FarEnd);
                    }
                    if (db["MS_TO_NO"].ToString() != li.FarEndNodeID)
                    {
                        update = true;
                        u.UpdateFarEndNodeID = true;
                        u.FarEndNodeID = li.FarEndNodeID;
                        UpdateInfo(updateinfo, "remote-ip-node", db["MS_TO_NO"].ToString(), li.FarEndNodeID, true);
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
                Insert insert = j.Insert("MESDP");
                insert.Value("MS_ID", s.Id);
                insert.Value("MS_NO", nodeID);
                insert.Value("MS_SDP", s.SDP);
                insert.Value("MS_Status", s.Status);
                insert.Value("MS_Protocol", s.Protocol);
                insert.Value("MS_IP", s.FarEnd);
                insert.Value("MS_MTU", s.AdmMTU.Nullable(-1));
                insert.Value("MS_Type", s.Type);
                insert.Value("MS_LSP", s.LSP);
                insert.Value("MS_TO_NO", s.FarEndNodeID);
                batch.Add(insert);
            }
            result = batch.Commit();
            if (!result) return DatabaseFailure(probe);
            Event(result, EventActions.Add, EventElements.SDP, false);

            // UPDATE
            batch.Begin();
            foreach (MESDPToDatabase s in sdpupdate)
            {
                Update update = j.Update("MESDP");
                update.Set("MS_Status", s.Status, s.UpdateStatus);
                update.Set("MS_Protocol", s.Protocol, s.UpdateProtocol);
                update.Set("MS_Type", s.Type, s.UpdateType);
                update.Set("MS_LSP", s.LSP, s.UpdateLSP);
                update.Set("MS_MTU", s.AdmMTU.Nullable(-1), s.UpdateAdmMTU);
                update.Set("MS_IP", s.FarEnd, s.UpdateFarEnd);
                update.Set("MS_TO_NO", s.FarEndNodeID, s.UpdateFarEndNodeID);
                update.Where("MS_ID", s.Id);
                batch.Add(update);
            }
            result = batch.Commit();
            if (!result) return DatabaseFailure(probe);
            Event(result, EventActions.Update, EventElements.SDP, false);

            #endregion

            sdpdb = j.QueryDictionary("select * from MESDP where MS_NO = {0}", "MS_SDP", nodeID);
            if (sdpdb == null) return DatabaseFailure(probe);

            #endregion

            #region CIRCUIT

            Event("Checking Circuit");

            SortedDictionary<string, MECircuitToDatabase> circuitlive = new SortedDictionary<string, MECircuitToDatabase>();
            Dictionary<string, Row2> circuitdb = null;
            List<MECircuitToDatabase> circuitinsert = new List<MECircuitToDatabase>();
            List<MECircuitToDatabase> circuitupdate = new List<MECircuitToDatabase>();
            List<string[]> hweCircuitMplsL2vc = null;
            List<string[]> hweCircuitVllCcc = null;

            //circuitdb = QueryDictionary("select * from MECircuit where MC_NO = {0}", "MC_VCID", nodeID);
            //goto debug3;

            #region Live

            if (nodeManufacture == alu)
            {
                #region alu

                // PRESTEP, fix duplicated vcid in alu metro that might happen if probe fail sometime ago.
                result = j.Query("select MC_VCID from (select MC_VCID, COUNT(MC_VCID) as c from MECircuit where MC_NO = {0} group by MC_VCID) a where c >= 2", nodeID);
                if (result.Count > 0)
                {
                    Event(result.Count + " circuit(s) are found duplicated, began deleting...");
                    List<string> duplicatedvcids = new List<string>();
                    foreach (Row2 row in result) duplicatedvcids.Add(row["MC_VCID"].ToString());
                    string duplicatedvcidstr = "'" + string.Join("', '", duplicatedvcids.ToArray()) + "'";
                    j.Execute("update MEInterface set MI_MC = NULL where MI_MC in (select MC_ID from MECircuit where MC_VCID in (" + duplicatedvcidstr + ") and MC_NO = {0})", nodeID);
                    j.Execute("update MEPeer set MP_TO_MC = NULL where MP_TO_MC in (select MC_ID from MECircuit where MC_VCID in (" + duplicatedvcidstr + ") and MC_NO = {0})", nodeID);
                    result = j.Execute("delete from MEPeer where MP_MC in (select MC_ID from MECircuit where MC_VCID in (" + duplicatedvcidstr + ") and MC_NO = {0})", nodeID);
                    Event(result.AffectedRows + " peer(s) have been deleted");
                    result = j.Execute("delete from MECircuit where MC_ID in (select MC_ID from MECircuit where MC_VCID in (" + duplicatedvcidstr + ") and MC_NO = {0})", nodeID);
                    Event(result.AffectedRows + " circuits(s) have been deleted");
                }

                circuitdb = j.QueryDictionary("select * from MECircuit where MC_NO = {0}", "MC_VCID", nodeID);
                if (circuitdb == null) return DatabaseFailure(probe);

                //goto debug3;

                // STEP 1, dari display config untuk epipe dan vpls, biar dapet mtu dan deskripsinya
                if (Request("admin display-config | match customer context children | match \"epipe|vpls|description|service-mtu\" expression", out lines, probe)) return probe;

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
                            if (!circuitlive.ContainsKey(cservice.VCID))
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
                            if (int.TryParse(mtu, out amtu)) cservice.AdmMTU = amtu == 0 ? -1 : amtu;
                            else cservice.AdmMTU = -1;
                        }
                    }
                }
                if (cservice != null)
                {
                    if (!circuitlive.ContainsKey(cservice.VCID))
                        circuitlive.Add(cservice.VCID, cservice);
                    cservice = null;
                }

                // STEP 2, dari service-using, sisanya
                if (Request("show service service-using", out lines, probe)) return probe;

                foreach (string line in lines)
                {
                    if (line.Length > 0 && char.IsDigit(line[0]))
                    {
                        string[] linex = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        if (linex.Length >= 5)
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

                                if (!circuitlive.ContainsKey(linex[0]))
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

                circuitdb = j.QueryDictionary("select * from MECircuit where MC_NO = {0}", "MC_Description", nodeID);
                if (circuitdb == null) return DatabaseFailure(probe);

                // display vsi verbose | in VSI
                // display mpls l2vc brief

                // STEP 1, VSI Name dan VSI ID
                if (Request("display vsi verbose | in VSI Name|VSI ID", out lines, probe)) return probe;

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
                if (Request("display vsi", out lines, probe)) return probe;

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
                                else cu.AdmMTU = -1;
                            }
                        }
                    }
                }

                // STEP 3, dari MPLS L2VC
                hweCircuitMplsL2vc = new List<string[]>();

                if (Request("display mpls l2vc | in client interface|VC ID|local VC MTU|destination|primary or secondary", out lines, probe)) return probe;

                string cinterface = null;
                string cinterfaceVCID = null;
                string cinterfaceSDP = null;
                bool cinterfacestate = false;
                int cmtu = -1;
                bool cinterfacewithstate = false;
                bool cinterfaceprimary = true;

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
                                    string vcidname = cinterface + "_GROUP";

                                    if (!circuitlive.ContainsKey(vcidname))
                                    {
                                        MECircuitToDatabase cu = new MECircuitToDatabase();
                                        cu.Type = "E";
                                        cu.Description = vcidname;
                                        cu.VCID = cinterfaceprimary ? cinterfaceVCID : null;
                                        cu.CustomerID = null;
                                        cu.Status = cinterfacestate;
                                        cu.Protocol = cinterfacestate;
                                        cu.AdmMTU = cmtu;

                                        circuitlive.Add(vcidname, cu);
                                    }
                                    else
                                    {
                                        MECircuitToDatabase cu = circuitlive[vcidname];
                                        if (cinterfaceprimary) cu.VCID = cinterfaceVCID;
                                    }

                                    hweCircuitMplsL2vc.Add(new string[] { cinterface, cinterfaceSDP, cinterfacestate.ToString(), vcidname, cinterfaceVCID, cinterfacewithstate.ToString() });

                                    cinterface = null;
                                    cinterfaceVCID = null;
                                    cinterfaceSDP = null;
                                    cinterfacestate = false;
                                    cinterfacewithstate = false;
                                    cmtu = -1;
                                }

                                if (lineValue.Length > 0)
                                {
                                    string[] linex2 = lineValue.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                    NetworkInterface inf = NetworkInterface.Parse(linex2[0]);
                                    if (inf != null) cinterface = inf.Name;

                                    if (linex2.Length >= 3)
                                    {
                                        cinterfacewithstate = true;
                                        if (linex2[2] == "up") cinterfacestate = true;
                                    }
                                    else
                                    {
                                        // we dont know how to determine the cinterfacestate yet... so cinterfacewithstate = false
                                        // lets assume true
                                        cinterfacestate = true;
                                    }
                                }
                            }
                            else if (lineKey == "VC ID") cinterfaceVCID = lineValue;
                            else if (lineKey == "destination") cinterfaceSDP = lineValue;
                            else if (lineKey == "primary or secondary")
                            {
                                if (lineValue == "primary") cinterfaceprimary = true;
                                else cinterfaceprimary = false;
                            }
                            else if (lineKey == "local VC MTU")
                            {
                                if (lineValue.Length > 0)
                                {
                                    string[] linex2 = lineValue.Trim().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                                    int amtu;
                                    if (int.TryParse(linex2[0], out amtu)) cmtu = amtu;
                                    else cmtu = -1;
                                }
                            }
                        }
                    }
                }
                if (cinterface != null)
                {
                    string vcidname = cinterface + "_GROUP";

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
                    else
                    {
                        MECircuitToDatabase cu = circuitlive[vcidname];
                        if (cinterfaceprimary) cu.VCID = cinterfaceVCID;
                    }

                    hweCircuitMplsL2vc.Add(new string[] { cinterface, cinterfaceSDP, cinterfacestate.ToString(), vcidname, cinterfaceVCID, cinterfacewithstate.ToString() });
                }

                // STEP 4, dari VLL CCC
                hweCircuitVllCcc = new List<string[]>();


                /*
012345678901234567890123456789012345678901234567890123456789
name: MULIA-CEMERLANG, type: local, state: up,
intf1: GigabitEthernet7/1/10.2463 (up), access-port: false

intf2: GigabitEthernet8/0/3.2463 (up), access-port: false
                */
                if (Request("display vll ccc", out lines, probe)) return probe;

                string cccname = null;
                string cccif1 = null;
                string cccif2 = null;
                string cccstatus = null;

                foreach (string line in lines)
                {
                    string lineTrim = line.Trim();

                    if (lineTrim.Length > 0)
                    {
                        if (lineTrim.StartsWith("name: "))
                        {
                            if (cccname != null)
                            {
                                string vcidname = cccname + "_CCC";

                                bool custatus = false;
                                if (cccstatus != null && cccstatus.ToLower().StartsWith("up")) custatus = true;

                                if (!circuitlive.ContainsKey(vcidname))
                                {
                                    MECircuitToDatabase cu = new MECircuitToDatabase();
                                    cu.Type = "E";
                                    cu.Description = vcidname;
                                    cu.VCID = null;
                                    cu.CustomerID = null;
                                    cu.Status = custatus;
                                    cu.Protocol = custatus;
                                    cu.AdmMTU = -1;

                                    circuitlive.Add(vcidname, cu);
                                }

                                hweCircuitVllCcc.Add(new string[] { cccname, cccif1, cccif2, vcidname });
                                cccif1 = null;
                                cccif2 = null;
                            }

                            cccname = lineTrim.Substring(6, lineTrim.IndexOf(',') - 6);
                            cccstatus = lineTrim.Substring(lineTrim.IndexOf("state: ") + 7);
                        }
                        else if (cccname != null)
                        {
                            if (lineTrim.StartsWith("intf"))
                            {
                                string ifname = lineTrim.Substring(7, lineTrim.IndexOf(' ', 7) - 7);
                                if (lineTrim.StartsWith("intf1")) cccif1 = ifname;
                                else cccif2 = ifname;
                            }
                        }
                    }
                }
                if (cccname != null)
                {
                    string vcidname = cccname + "_CCC";

                    bool custatus = false;
                    if (cccstatus != null && cccstatus.ToLower().StartsWith("up")) custatus = true;

                    if (!circuitlive.ContainsKey(vcidname))
                    {
                        MECircuitToDatabase cu = new MECircuitToDatabase();
                        cu.Type = "E";
                        cu.Description = vcidname;
                        cu.VCID = null;
                        cu.CustomerID = null;
                        cu.Status = custatus;
                        cu.Protocol = custatus;
                        cu.AdmMTU = -1;

                        circuitlive.Add(vcidname, cu);
                    }

                    hweCircuitVllCcc.Add(new string[] { cccname, cccif1, cccif2, vcidname });
                }

                #endregion
            }

            #endregion

            #region Check

            Dictionary<string, List<string>> adjacentPeers = null;

            ServiceImmediateDiscovery(circuitlive);

            foreach (KeyValuePair<string, MECircuitToDatabase> pair in circuitlive)
            {
                MECircuitToDatabase li = pair.Value;

                if (!circuitdb.ContainsKey(pair.Key))
                {
                    if (adjacentPeers == null)
                    {
                        result = j.Query("select MP_VCID, MP_ID from MEPeer, MESDP, Node where MP_MS = MS_ID and MS_IP = NO_IP and NO_ID = {0}", nodeID);
                        if (!result) return DatabaseFailure(probe);
                        adjacentPeers = new Dictionary<string, List<string>>();
                        if (result)
                        {
                            foreach (Row2 row in result)
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
                    li.Id = Database2.ID();

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
                    Row2 db = circuitdb[pair.Key];

                    MECircuitToDatabase u = new MECircuitToDatabase();
                    u.Id = db["MC_ID"].ToString();

                    bool update = false;
                    StringBuilder updateinfo = new StringBuilder();

                    if (db["MC_Status"].ToBool() != li.Status)
                    {
                        update = true;
                        u.UpdateStatus = true;
                        u.Status = li.Status;
                        UpdateInfo(updateinfo, "status", db["MC_Status"].ToBool().DescribeUpDown(), li.Status.DescribeUpDown());
                    }
                    if (db["MC_Protocol"].ToBool() != li.Protocol)
                    {
                        update = true;
                        u.UpdateProtocol = true;
                        u.Protocol = li.Protocol;
                        UpdateInfo(updateinfo, "protocol", db["MC_Protocol"].ToBool().DescribeUpDown(), li.Protocol.DescribeUpDown());
                    }
                    if (db["MC_Type"] != li.Type)
                    {
                        update = true;
                        u.UpdateType = true;
                        u.Type = li.Type;
                        UpdateInfo(updateinfo, "type", db["MC_Type"].ToString(), li.Type);
                    }
                    if (db["MC_MU"] != li.CustomerID)
                    {
                        update = true;
                        u.UpdateCustomer = true;
                        u.CustomerID = li.CustomerID;
                        UpdateInfo(updateinfo, "customer", db["MC_MU"], li.CustomerID, true);
                    }
                    if (db["MC_Description"] != li.Description)
                    {
                        update = true;
                        u.UpdateDescription = true;
                        u.Description = li.Description;
                        UpdateInfo(updateinfo, "description", db["MC_Description"], li.Description, true);
                    }
                    if (db["MC_SI"] != li.ServiceImmediateID)
                    {
                        update = true;
                        u.UpdateServiceImmediateID = true;
                        u.ServiceImmediateID = li.ServiceImmediateID;
                        UpdateInfo(updateinfo, "service-immediate", "UPDATING");
                    }
                    if (db["MC_MTU"].ToIntShort(-1) != li.AdmMTU)
                    {
                        update = true;
                        u.UpdateAdmMTU = true;
                        u.AdmMTU = li.AdmMTU;
                        UpdateInfo(updateinfo, "mtu", db["MC_MTU"].ToIntShort(-1).NullableInfo(), li.AdmMTU.NullableInfo());
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

            // ADD
            batch.Begin();
            foreach (MECircuitToDatabase s in circuitinsert)
            {
                Insert insert = j.Insert("MECircuit");
                insert.Value("MC_ID", s.Id);
                insert.Value("MC_NO", nodeID);
                insert.Value("MC_VCID", s.VCID);
                insert.Value("MC_Type", s.Type);
                insert.Value("MC_Status", s.Status);
                insert.Value("MC_Protocol", s.Protocol);
                insert.Value("MC_MU", s.CustomerID);
                insert.Value("MC_Description", s.Description);
                insert.Value("MC_MTU", s.AdmMTU.Nullable(0));
                insert.Value("MC_SI", s.ServiceImmediateID);
                batch.Add(insert);
            }
            result = batch.Commit();
            if (!result) return DatabaseFailure(probe);
            Event(result, EventActions.Add, EventElements.Circuit, false);

            batch.Begin();
            foreach (MECircuitToDatabase s in circuitinsert)
            {
                if (s.AdjacentPeers != null)
                {
                    string[] peers = s.AdjacentPeers.ToArray();
                    foreach (string peer in peers) batch.Add("update MEPeer set MP_TO_MC = {0} where MP_ID = {1}", s.Id, peer);
                }
            }
            result = batch.Commit();
            if (!result) return DatabaseFailure(probe);
            Event(result, EventActions.Update, EventElements.CircuitReference, false);

            // UPDATE
            batch.Begin();
            foreach (MECircuitToDatabase s in circuitupdate)
            {
                Update update = j.Update("MECircuit");
                update.Set("MC_Status", s.Status, s.UpdateStatus);
                update.Set("MC_Protocol", s.Protocol, s.UpdateProtocol);
                update.Set("MC_Type", s.Type, s.UpdateType);
                update.Set("MC_Description", s.Description, s.UpdateDescription);
                update.Set("MC_SI", s.ServiceImmediateID, s.UpdateServiceImmediateID);
                update.Set("MC_MTU", s.AdmMTU.Nullable(0), s.UpdateAdmMTU);
                update.Set("MC_MU", s.CustomerID, s.UpdateCustomer);
                update.Where("MC_ID", s.Id);
                batch.Add(update);
            }
            result = batch.Commit();
            if (!result) return DatabaseFailure(probe);
            Event(result, EventActions.Update, EventElements.Circuit, false);

            #endregion

            if (nodeManufacture == alu) circuitdb = j.QueryDictionary("select * from MECircuit where MC_NO = {0}", "MC_VCID", nodeID);
            else if (nodeManufacture == hwe) circuitdb = j.QueryDictionary("select * from MECircuit where MC_NO = {0}", "MC_Description", nodeID);
            if (circuitdb == null) return DatabaseFailure(probe);

            #endregion

            #region PEER

            Event("Checking Peer");

            Dictionary<string, MEPeerToDatabase> peerlive = new Dictionary<string, MEPeerToDatabase>();
            List<string> duplicatedpeers = new List<string>();
            Dictionary<string, Row2> peerdb = j.QueryDictionary("select * from MEPeer, MECircuit, MESDP where MP_MC = MC_ID and MP_MS = MS_ID and MC_NO = {0}", delegate (Row2 row) {
                return row["MS_SDP"].ToString() + ":" + row["MP_VCID"].ToString();
            }, delegate (Row2 row) { duplicatedpeers.Add(row["MP_ID"].ToString()); }, nodeID);
            if (peerdb == null) return DatabaseFailure(probe);
            List<MEPeerToDatabase> peerinsert = new List<MEPeerToDatabase>();
            List<MEPeerToDatabase> peerupdate = new List<MEPeerToDatabase>();

            if (duplicatedpeers.Count > 0)
            {
                Event(duplicatedpeers.Count + " peer-per-circuit(s) are found duplicated, began deleting...");
                string duplicatedpeerstr = "'" + string.Join("', '", duplicatedpeers.ToArray()) + "'";
                result = j.Execute("delete from MEPeer where MP_ID in (" + duplicatedpeerstr + ")");
                if (!result) return DatabaseFailure(probe);
                Event(result, EventActions.Delete, EventElements.Peer, true);
            }

            #region Live

            if (nodeManufacture == alu)
            {
                #region alu

                if (Request("show service sdp-using", out lines, probe)) return probe;

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
                if (Request("display vsi peer-info", out lines, probe)) return probe;

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
                foreach (string[] strs in hweCircuitMplsL2vc)
                {
                    // cinterface, cinterfaceSDP, cinterfacestate.ToString(), vcidname, cinterfaceVCID, cinterfacewithstate
                    //  0            1              2                          3         4              5

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

                        if (strs[5] == "True")
                            c.Protocol = strs[2] == "True";
                        else
                        {
                            // cannot determine the state without checking the interface status first
                            // assume true
                            c.Protocol = true;
                        }

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

                result = j.Query("select MC_ID from MESDP, MECircuit where MS_TO_NO = MC_NO and MS_ID = {0} and MC_VCID = {1}", li.SDPID, li.VCID);
                if (!result) return DatabaseFailure(probe);

                if (result.Count > 0) li.ToCircuitID = result[0]["MC_ID"].ToString();

                if (!peerdb.ContainsKey(pair.Key))
                {
                    Event("Peer ADD: " + pair.Key);
                    li.Id = Database2.ID();
                    peerinsert.Add(li);
                }
                else
                {
                    Row2 db = peerdb[pair.Key];

                    MEPeerToDatabase u = new MEPeerToDatabase();
                    u.Id = db["MP_ID"].ToString();
                    li.Id = u.Id;

                    bool update = false;
                    StringBuilder updateinfo = new StringBuilder();

                    if (db["MP_MC"].ToString() != li.CircuitID)
                    {
                        update = true;
                        u.UpdateCircuitID = true;
                        u.CircuitID = li.CircuitID;
                        UpdateInfo(updateinfo, "circuit", null);
                    }
                    if (db["MP_Protocol"].ToBool() != li.Protocol)
                    {
                        update = true;
                        u.UpdateProtocol = true;
                        u.Protocol = li.Protocol;
                        UpdateInfo(updateinfo, "protocol", db["MP_Protocol"].ToBool().DescribeUpDown(), li.Protocol.DescribeUpDown());
                    }
                    if (db["MP_Type"].ToString() != li.Type)
                    {
                        update = true;
                        u.UpdateType = true;
                        u.Type = li.Type;
                        UpdateInfo(updateinfo, "type", db["MP_Type"].ToString(), li.Type);
                    }
                    if (db["MP_TO_MC"].ToString() != li.ToCircuitID)
                    {
                        update = true;
                        u.UpdateToCircuitID = true;
                        u.ToCircuitID = li.ToCircuitID;
                        UpdateInfo(updateinfo, "target-circuit", db["MP_TO_MC"].ToString(), li.ToCircuitID, true);
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
                Insert insert = j.Insert("MEPeer");
                insert.Value("MP_ID", s.Id);
                insert.Value("MP_MC", s.CircuitID);
                insert.Value("MP_MS", s.SDPID);
                insert.Value("MP_VCID", s.VCID);
                insert.Value("MP_Protocol", s.Protocol);
                insert.Value("MP_Type", s.Type);
                insert.Value("MP_TO_MC", s.ToCircuitID);
                batch.Add(insert);
            }
            result = batch.Commit();
            if (!result) return DatabaseFailure(probe);
            Event(result, EventActions.Add, EventElements.Peer, false);

            // UPDATE
            batch.Begin();
            foreach (MEPeerToDatabase s in peerupdate)
            {
                Update update = j.Update("MEPeer");
                update.Set("MP_Protocol", s.Protocol, s.UpdateProtocol);
                update.Set("MP_Type", s.Type, s.UpdateType);
                update.Set("MP_TO_MC", s.ToCircuitID, s.UpdateToCircuitID);
                update.Where("MP_ID", s.Id);
                batch.Add(update);
            }
            result = batch.Commit();
            if (!result) return DatabaseFailure(probe);
            Event(result, EventActions.Update, EventElements.Peer, false);

            // DELETE
            List<string> peerdelete = new List<string>();

            batch.Begin();
            foreach (KeyValuePair<string, Row2> pair in peerdb)
            {
                if (!peerlive.ContainsKey(pair.Key) || pair.Value["MP_MC"].ToString() != peerlive[pair.Key].CircuitID)
                {
                    Event("Peer DELETE: " + pair.Key);

                    string mpID = pair.Value["MP_ID"].ToString();
                    peerdelete.Add(mpID);
                    batch.Add("update MEMac set MA_MP = NULL where MA_MP = {0}", mpID);
                }
            }
            result = batch.Commit();
            if (!result) return DatabaseFailure(probe);

            batch.Begin();
            foreach (string id in peerdelete)
            {
                batch.Add("delete from MEPeer where MP_ID = {0}", id);
            }
            result = batch.Commit();
            if (!result) return DatabaseFailure(probe);
            Event(result, EventActions.Delete, EventElements.Peer, false);

            #endregion

            #endregion

            #region INTERFACE

            Event("Checking Interface");

            SortedDictionary<string, MEInterfaceToDatabase> interfacelive = new SortedDictionary<string, MEInterfaceToDatabase>();
            List<string> duplicatedinterfaces = new List<string>();
            Dictionary<string, Row2> interfacedb = j.QueryDictionary("select * from MEInterface where MI_NO = {0}", "MI_Name", delegate (Row2 row) { duplicatedinterfaces.Add(row["MI_ID"].ToString()); }, nodeID);
            if (interfacedb == null) return DatabaseFailure(probe);
            SortedDictionary<string, MEInterfaceToDatabase> interfaceinsert = new SortedDictionary<string, MEInterfaceToDatabase>();
            List<MEInterfaceToDatabase> interfaceupdate = new List<MEInterfaceToDatabase>();

            if (duplicatedinterfaces.Count > 0)
            {
                Event(duplicatedinterfaces.Count + " interface(s) are found duplicated, began deleting...");
                string duplicatedinterfacestr = "'" + string.Join("', '", duplicatedinterfaces.ToArray()) + "'";
                result = j.Execute("update PEInterface set PI_TO_MI = NULL where PI_TO_MI in (" + duplicatedinterfacestr + ")");
                if (!result) return DatabaseFailure(probe);
                result = j.Execute("update MEInterface set MI_MI = NULL where MI_MI in (" + duplicatedinterfacestr + ")");
                if (!result) return DatabaseFailure(probe);
                result = j.Execute("update MEInterface set MI_TO_MI = NULL where MI_TO_MI in (" + duplicatedinterfacestr + ")");
                if (!result) return DatabaseFailure(probe);
                result = j.Execute("delete from DerivedAreaConnection where DAC_MI_1 in (" + duplicatedinterfacestr + ") or DAC_MI_2 in (" + duplicatedinterfacestr + ")");
                if (!result) return DatabaseFailure(probe);
                result = j.Execute("delete from MEMac where MA_MI in (" + duplicatedinterfacestr + ")");
                if (!result) return DatabaseFailure(probe);
                result = j.Execute("delete from MEInterface where MI_ID in (" + duplicatedinterfacestr + ")");
                if (!result) return DatabaseFailure(probe);
                Event(result, EventActions.Delete, EventElements.Interface, true);
            }

            #region Live

            if (nodeManufacture == alu)
            {
                #region alu
                if (Request("show port description", out lines, probe)) return probe;

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

                if (Request("show port", out lines, probe)) return probe;


                //4/2/11      Up    Yes  Up      9212 9212    - accs null xcme   GIGE-LX  10KM
                //0           1     2    3       4    5       6 7    8    9      10       11
                //4/2/12      Up    Yes  Link Up 9212 9212    2 netw null xcme   GIGE-LX  10KM
                //0           1     2    3    4  5    6       7 8    9    10     11       12

                List<Tuple<MEInterfaceToDatabase, string, int>> monitorPort = new List<Tuple<MEInterfaceToDatabase, string, int>>();

                foreach (string line in lines)
                {
                    if (line.Length > 0)
                    {
                        if (char.IsDigit(line[0]))
                        {
                            string[] tokens = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                            if (tokens.Length >= 4)
                            {
                                string portdet = tokens[0].Trim();
                                string portex = "Ex" + portdet;
                                portex = portex.Replace('.', ':');

                                if (interfacelive.ContainsKey(portex))
                                {
                                    interfacelive[portex].Enable = tokens[1].Trim() == "Up";
                                    interfacelive[portex].Protocol = tokens[2].Trim() == "Yes";

                                    string il3 = tokens[3].Trim();
                                    int tokenShift = 0;

                                    if (il3 == "Link")
                                    {
                                        il3 = "Up";
                                        tokenShift = 1;
                                    }

                                    interfacelive[portex].Status = il3 == "Up";

                                    if (tokens.Length >= (6 + tokenShift))
                                    {
                                        // mtu
                                        string cmtu = tokens[4 + tokenShift];
                                        string omtu = tokens[5 + tokenShift];

                                        if (int.TryParse(cmtu, out int icmtu)) interfacelive[portex].AdmMTU = icmtu == 0 ? -1 : icmtu;
                                        else interfacelive[portex].AdmMTU = -1;

                                        if (int.TryParse(omtu, out int iomtu)) interfacelive[portex].RunMTU = iomtu == 0 ? -1 : iomtu;
                                        else interfacelive[portex].RunMTU = -1;
                                    }

                                    if (tokens.Length >= (7 + tokenShift))
                                    {
                                        string agr = tokens[6 + tokenShift].Trim();
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
                                                mid.InterfaceType = "Ag";
                                                interfacelive.Add("Ag" + agr, mid);
                                            }
                                        }
                                    }

                                    if (tokens.Length >= (9 + tokenShift))
                                    {
                                        // mode and encap
                                        string mode = tokens[7 + tokenShift].Trim().ToUpper();
                                        string encp = tokens[8 + tokenShift].Trim().ToUpper();

                                        if (mode.Length > 0) interfacelive[portex].Mode = mode[0];
                                        if (encp.Length > 0) interfacelive[portex].Encapsulation = encp == "NULL" ? '-' : encp[0];

                                        if (interfacelive[portex].AdmMTU == interfacelive[portex].RunMTU)
                                        {
                                            char enc = interfacelive[portex].Encapsulation;
                                            int cmtu = interfacelive[portex].AdmMTU;

                                            if ((enc == 'D' && cmtu == 1518) ||
                                                (enc == 'Q' && cmtu == 1522) ||
                                                (enc == '-' && cmtu == 1514)
                                                )
                                            {
                                                interfacelive[portex].AdmMTU = -1;
                                            }
                                        }
                                    }

                                    if (tokens.Length >= (10 + tokenShift))
                                    {
                                        string typ = tokens[9 + tokenShift].Trim().ToLower();

                                        string ity = null;
                                        if (typ == "faste") ity = "Fa";
                                        else if (typ == "xcme") ity = "Gi";
                                        else if (typ == "xgige") ity = "Te";
                                        else if (typ == "cgige") ity = "Hu";
                                        else if (typ == "tdm") ity = "Se";

                                        if (ity != null)
                                        {
                                            interfacelive[portex].InterfaceType = ity;
                                        }

                                        //3/1/1       Up    Yes  Up      9212 9212    - accs dotq xcme   GIGE-LX  10KM
                                        //4/2/12      Up    Yes  Link Up 9212 9212    2 netw null xcme   GIGE-LX  10KM

                                        //1234567890123456789012345678901234567890123456789012345678901234567890123456789
                                        //         1         2         3         4         5         6   

                                        if (line.Length >= 64)
                                        {
                                            string endinfo = line.Substring(63).Trim();

                                            if (endinfo.Length > 0)
                                                interfacelive[portex].Info = endinfo;
                                        }
                                    }

                                    if (interfacelive[portex].Status == false)
                                    {
                                        string[] portlines;
                                        if (Request("show port " + portdet + " | match \"Last State Change\"", out portlines, probe)) return probe;

                                        foreach (string portline in portlines)
                                        {
                                            string portlinetrim = portline.Trim();
                                            //Last State Change  : 09/27/2016 02:59:23        Hold time down   : 0 seconds
                                            //01234567890123456789012345678901234567890123456789
                                            //                     
                                            if (portlinetrim.StartsWith("Last State Change") && portlinetrim.Length >= 40)
                                            {
                                                string dtim = portlinetrim.Substring(21, 19);
                                                DateTime lstch;
                                                if (!DateTime.TryParse(dtim, out lstch)) lstch = DateTime.MinValue;
                                                if (lstch > DateTime.MinValue) interfacelive[portex].LastDown = lstch - nodeTimeOffset;
                                            }
                                        }

                                        interfacelive[portex].TrafficInput = 0;
                                        interfacelive[portex].TrafficOutput = 0;
                                    }
                                    else
                                    {
                                        interfacelive[portex].LastDown = null;

                                        if (interfacelive[portex].InterfaceType != null)
                                        {
                                            int typerate = -1;

                                            if (interfacelive[portex].InterfaceType == "Hu") typerate = 104857600;
                                            else if (interfacelive[portex].InterfaceType == "Te") typerate = 10485760;
                                            else if (interfacelive[portex].InterfaceType == "Gi") typerate = 1048576;
                                            else if (interfacelive[portex].InterfaceType == "Fa") typerate = 102400;
                                            else if (interfacelive[portex].InterfaceType == "Et") typerate = 10240;

                                            if (typerate > -1)
                                            {
                                                monitorPort.Add(new Tuple<MEInterfaceToDatabase, string, int>(interfacelive[portex], portdet, typerate));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                // monitor port

                foreach (List<Tuple<MEInterfaceToDatabase, string, int>> group in monitorPort.Group(5))
                {
                    List<string> portJoinedList = new List<string>();
                    Dictionary<string, Tuple<MEInterfaceToDatabase, string, int>> dict = new Dictionary<string, Tuple<MEInterfaceToDatabase, string, int>>();
                    foreach (Tuple<MEInterfaceToDatabase, string, int> item in group)
                    {
                        portJoinedList.Add(item.Item2);
                        dict.Add(item.Item2, item);
                    }
                    string portJoinedString = string.Join(" ", portJoinedList.ToArray());

                    if (Request("monitor port " + portJoinedString + " interval 3 repeat 1", out string[] monitorPortLines, probe)) return probe;

                    bool begin = false;
                    MEInterfaceToDatabase currentIf = null;
                    int currentTypeRate = -1;
                    foreach (string line in monitorPortLines)
                    {
                        if (line.IndexOf("(Mode: Delta)") > -1) begin = true;
                        else if (begin)
                        {
                            string lineTrim = line.Trim();
                            if (lineTrim.StartsWith("Port "))
                            {
                                string refdet = lineTrim.Substring(5);
                                currentIf = dict[refdet].Item1;
                                currentTypeRate = dict[refdet].Item3;
                            }
                            else if (lineTrim.StartsWith("Octets ") && currentIf != null)
                            {
                                string[] tokens = lineTrim.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);
                                if (tokens.Length == 3)
                                {
                                    string sin = tokens[1];
                                    string sout = tokens[2];

                                    if (!long.TryParse(sin, out long inputRate)) inputRate = -1;
                                    if (!long.TryParse(sout, out long outputRate)) outputRate = -1;

                                    if (inputRate > -1)
                                    {
                                        double kbps = Math.Round((double)inputRate / 1024 / 3);
                                        currentIf.TrafficInput = (float)Math.Round(kbps / (double)currentTypeRate * 100, 2);

                                        if (outputRate == -1) currentIf.TrafficOutput = 0;
                                    }
                                    if (outputRate > -1)
                                    {
                                        double kbps = Math.Round((double)outputRate / 1024 / 3);
                                        currentIf.TrafficOutput = (float)Math.Round(kbps / (double)currentTypeRate * 100, 2);

                                        if (inputRate == -1) currentIf.TrafficInput = 0;
                                    }
                                }
                                currentIf = null;
                            }
                        }
                    }
                }

                if (nodeVersion.StartsWith("TiMOS-B")) // sementara TiMOS-B ga bisa dapet deskripsi
                {
                    if (Request("show service sap-using", out lines, probe)) return probe;

                    foreach (string line in lines)
                    {
                        if (line.Length > 0)
                        {
                            if (char.IsDigit(line[0]) || line.StartsWith("lag"))
                            {
                                string[] linex = line.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);

                                string mainname = null, vlan = null;
                                string thisport = null;
                                string circuitID = null;

                                string[] portx = linex[0].Split(new char[] { ':' });
                                if (portx[0].StartsWith("lag-")) mainname = "Ag" + portx[0].Substring(4);
                                else mainname = "Ex" + portx[0];

                                mainname = mainname.Replace('.', ':');

                                if (portx.Length > 1) vlan = portx[1];
                                if (vlan == null) thisport = mainname + ".DIRECT";
                                else thisport = mainname + "." + vlan;

                                if (mainname.StartsWith("Ag"))
                                {
                                    if (!interfacelive.ContainsKey(mainname))
                                    {
                                        MEInterfaceToDatabase mid = new MEInterfaceToDatabase();
                                        mid.Name = mainname;
                                        mid.Status = true;
                                        mid.Protocol = true;
                                        mid.Enable = true;
                                        interfacelive.Add(mainname, mid);
                                    }
                                }

                                if (circuitdb.ContainsKey(linex[1])) circuitID = circuitdb[linex[1]]["MC_ID"].ToString();
                                else circuitID = null;

                                string adm, opr;
                                string ingressID = null;
                                string egressID = null;
                                if (nodeVersion.StartsWith("TiMOS-B-8") || nodeVersion.StartsWith("TiMOS-B-7"))
                                {
                                    adm = linex[5];
                                    opr = linex[6];
                                    if (qosdb.ContainsKey("0_" + linex[2])) ingressID = qosdb["0_" + linex[2]]["MQ_ID"].ToString();
                                }
                                else
                                {
                                    adm = linex[6];
                                    opr = linex[7];
                                    if (qosdb.ContainsKey("0_" + linex[2])) ingressID = qosdb["0_" + linex[2]]["MQ_ID"].ToString();
                                    if (qosdb.ContainsKey("1_" + linex[4])) egressID = qosdb["1_" + linex[4]]["MQ_ID"].ToString();
                                }

                                if (!interfacelive.ContainsKey(thisport))
                                {
                                    MEInterfaceToDatabase mid = new MEInterfaceToDatabase();
                                    mid.Name = thisport;
                                    mid.Status = opr == "Up";
                                    mid.Protocol = opr == "Up";
                                    mid.Enable = adm == "Up";
                                    mid.CircuitID = circuitID;
                                    mid.IngressID = ingressID;
                                    mid.EgressID = egressID;

                                    interfacelive.Add(thisport, mid);
                                }
                            }
                        }
                    }

                }
                else
                {
                    // make lag list
                    if (Request("show lag description", out lines, probe)) return probe;

                    /* lag
                /*
===============================================================================
Lag-id Port-id   Adm   Act/Stdby Opr   Description
-------------------------------------------------------------------------------
01234567890123456789012345678901234567890123456789
            1         2         3         4
16(e)            up              up    AKSES_to_RAN-PAG-TLT.1_5x1G
       9/1/10    up    active    up    AKSES_BACKHAUL_RAN-TSEL_ME-D2-KB#9/1/
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
                                if (description != null && current != null)
                                    current.Description = description.ToString();
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
                                        bool cadm = line.Substring(17, 2) == "up" ? true : false;
                                        bool copr = line.Substring(33, 2) == "up" ? true : false;
                                        current.Status = copr;
                                        current.Enable = cadm;
                                        current.Protocol = copr;
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
                                if (line.Substring(0, 39).Trim() == "")
                                    description.Append(line.Substring(39));
                                else if (current != null)
                                {
                                    current.Description = description.ToString();
                                    description = null;
                                    current = null;
                                }
                            }
                        }
                    }

                    if (description != null && current != null) current.Description = description.ToString();

                    if (Request("show service sap-using description", out lines, probe)) return probe;

                    port = null;
                    description = new StringBuilder();
                    string adm = null;
                    string opr = null;
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
                                        mid.Status = opr == "Up";
                                        mid.Protocol = opr == "Up";
                                        mid.Enable = adm == "Up";
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
                                        MEInterfaceToDatabase mid = new MEInterfaceToDatabase
                                        {
                                            Name = name,
                                            Status = true,
                                            Protocol = true,
                                            Enable = true
                                        };
                                        interfacelive.Add(name, mid);
                                    }
                                }

                                if (circuitdb.ContainsKey(linex[1])) circuitID = circuitdb[linex[1]]["MC_ID"].ToString();
                                else circuitID = null;

                                adm = linex[2];
                                opr = linex[3];

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
                                        mid.Status = opr == "Up";
                                        mid.Protocol = opr == "Up";
                                        mid.Enable = adm == "Up";
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
                            mid.Status = opr == "Up";
                            mid.Protocol = opr == "Up";
                            mid.Enable = adm == "Up";
                            mid.CircuitID = circuitID;
                            mid.Dot1Q = dot1q;

                            interfacelive.Add(port, mid);
                        }
                    }

                    if (Request("show service sap-using", out lines, probe)) return probe;

                    foreach (string line in lines)
                    {
                        if (line.Length > 0)
                        {
                            if (char.IsDigit(line[0]) || line.StartsWith("lag"))
                            {
                                string[] linex = line.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);

                                string mainname = null, vlan = null;
                                string thisport = null;

                                string[] portx = linex[0].Split(new char[] { ':' });
                                if (portx[0].StartsWith("lag-")) mainname = "Ag" + portx[0].Substring(4);
                                else mainname = "Ex" + portx[0];

                                mainname = mainname.Replace('.', ':');

                                if (portx.Length > 1) vlan = portx[1];
                                if (vlan == null) thisport = mainname + ".DIRECT";
                                else thisport = mainname + "." + vlan;

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
                                }
                            }
                        }
                    }

                    //if (Request("show router interface detail", out lines, probe)) return probe;
                }

                Dictionary<string, string> routerinterfaces = new Dictionary<string, string>();


                // additional data
                if (Request("show router interface exclude-services | match Network", out lines, probe)) return probe;
                /*
0123456789012345678901234567890123456789012345678901234567890123456789
          1         2         3         4         5         6    
dummy-to-bks9                    Up          Down/--     Network n/a
system                           Up          Up/--       Network system
to-me-d2-spu                     Up          Up/--       Network 4/2/1
to-me9-d2-bks                    Up          Up/--       Network lag-30
to-me9-d2-jt                     Up          Up/--       Network lag-32

                 */
                foreach (var line in lines)
                {
                    if (line.Length > 0)
                    {
                        string[] tokens = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        if (tokens.Length >= 5)
                        {
                            routerinterfaces.Add(tokens[0], tokens[4]);
                        }
                    }
                }

                if (Request("show router ospf neighbor", out lines, probe)) return probe;

                foreach (var line in lines)
                {
                    if (line.Length > 0)
                    {
                        string[] tokens = line.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                        if (tokens.Length == 6)
                        {
                            if (tokens[0].Length > 0)
                            {
                                if (routerinterfaces.ContainsKey(tokens[0]))
                                {
                                    var stport = routerinterfaces[tokens[0]];

                                    string mainname = null;

                                    if (stport.Length > 4 && stport.StartsWith("lag-")) mainname = "Ag" + stport.Substring(4);
                                    else if (char.IsDigit(stport[0])) mainname = "Ex" + stport;

                                    if (mainname != null && interfacelive.ContainsKey(mainname))
                                    {
                                        if (ipnodedb.ContainsKey(tokens[1]))
                                            interfacelive[mainname].TopologyNOID = ipnodedb[tokens[1]]["NO_ID"].ToString();
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

                if (Request("display interface description", out lines, probe)) return probe;

                bool begin = false;
                string port = null;
                StringBuilder description = new StringBuilder();

                if (nodeVersion == "5.70" || nodeVersion == "5.90")
                {
                    // 5.90
                    //GigabitEthernet0/0/0          H
                    //0123456789012345678901234567890
                    //          1         2         3

                    // 5.70
                    //Eth-Trunk1.110              IPTV_Multicast
                    //0123456789012345678901234567890
                    //          1         2         3

                    foreach (string line in lines)
                    {
                        if (line.Length > 0)
                        {
                            if (begin)
                            {
                                if (line[0] != ' ' && line.Length >= (nodeVersion == "5.70" ? 29 : 31))
                                {
                                    if (port != null)
                                    {
                                        if (!interfacelive.ContainsKey(port))
                                        {
                                            MEInterfaceToDatabase mid = new MEInterfaceToDatabase()
                                            {
                                                Name = port,
                                                Description = description.Length > 0 ? FixDescription(description.ToString()) : null
                                            };
                                            interfacelive.Add(port, mid);
                                        }

                                        description.Clear();
                                        port = null;
                                    }

                                    string inf = line.Substring(0, (nodeVersion == "5.70" ? 28 : 30)).Trim();

                                    NetworkInterface nif = NetworkInterface.Parse(inf);
                                    if (nif != null)
                                    {
                                        if (nif.Type == "Hu" || nif.Type == "Te") port = "Gi" + nif.PortName;
                                        else port = nif.Name;
                                    }

                                    if (port != null)
                                    {
                                        string descarea = null;
                                        if (nodeVersion == "5.70")
                                            descarea = line.Substring(28);
                                        else
                                            descarea = line.Substring(30);

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
                            mid.Description = description.Length > 0 ? FixDescription(description.ToString()) : null;
                            interfacelive.Add(port, mid);
                        }
                    }
                }
                else
                {
                    string status = null;
                    string protocol = null;

                    // 5.120
                    //Aux0/0/1                      *down   down     H
                    //012345678901234567890123456789012345678901234567
                    //          1         2         3         4

                    // 5.160
                    //Aux0/0/1                      down    down     HUAWEI, Aux0/0/1 Interface

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
                                            mid.Description = description.Length > 0 ? FixDescription(description.ToString()) : null;
                                            mid.Status = (status == "up" || status == "up(s)");
                                            mid.Protocol = (status == "up" || status == "up(s)");
                                            mid.Enable = (status != "*down");
                                            interfacelive.Add(port, mid);
                                        }

                                        description.Clear();
                                        port = null;
                                        status = null;
                                        protocol = null;
                                    }

                                    string inf = line.Substring(0, 30).Trim();
                                    status = line.Substring(30, 7).Trim();
                                    protocol = line.Substring(38, 7).Trim();

                                    NetworkInterface nif = NetworkInterface.Parse(inf);
                                    if (nif != null)
                                    {
                                        if (nif.Type == "Hu" || nif.Type == "Te") port = "Gi" + nif.PortName;
                                        else port = nif.Name;
                                    }

                                    if (port != null)
                                    {
                                        string descarea = line.Substring(47).TrimStart();
                                        description.Append(descarea);
                                    }
                                }
                                else if (port != null && line.Length >= 48)
                                    description.Append(line.Substring(47));
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
                            mid.Description = description.Length > 0 ? FixDescription(description.ToString()) : null;
                            mid.Status = (status == "up" || status == "up(s)");
                            mid.Protocol = (status == "up" || status == "up(s)");
                            mid.Enable = (status != "*down");
                            interfacelive.Add(port, mid);
                        }
                    }
                }

                //main interface
                if (nodeVersion == "5.90" || nodeVersion == "5.70")
                {
                    if (Request("display interface | in current state|down time|BW|bandwidth utilization|Maximum Transmit Unit", out lines, probe)) return probe;
                }
                else
                {
                    if (Request("display interface main | in current state|down time|BW|bandwidth utilization|Maximum Transmit Unit", out lines, probe)) return probe;
                }

                MEInterfaceToDatabase currentInterfaceToDatabase = null;

                foreach (string line in lines)
                {
                    string lineTrim = line.Trim();
                    if (line.IndexOf("current state") > -1 && !line.StartsWith("Line protocol"))
                    {
                        currentInterfaceToDatabase = null;
                        string[] splits = line.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);
                        NetworkInterface nif = NetworkInterface.Parse(splits[0]);
                        if (nif != null && !nif.IsSubInterface)
                        {
                            string name = NetworkInterface.HuaweiName(nif);

                            if (interfacelive.ContainsKey(name))
                            {
                                currentInterfaceToDatabase = interfacelive[name];
                                currentInterfaceToDatabase.InterfaceType = nif.Type; // default
                                currentInterfaceToDatabase.Mode = 'A';
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
                            if (int.TryParse(dtim.Substring(0, 4), out int year) &&
                                int.TryParse(dtim.Substring(5, 2), out int month) &&
                                int.TryParse(dtim.Substring(8, 2), out int day) &&
                                int.TryParse(dtim.Substring(11, 2), out int hour) &&
                                int.TryParse(dtim.Substring(14, 2), out int min) &&
                                int.TryParse(dtim.Substring(17, 2), out int sec))
                            {
                                currentInterfaceToDatabase.LastDown = (new DateTime(year, month, day, hour, min, sec)) - nodeTimeOffset;
                            }
                        }
                        else
                            currentInterfaceToDatabase.LastDown = null;

                    }
                    else if (currentInterfaceToDatabase != null && currentInterfaceToDatabase.InterfaceType != null && lineTrim.StartsWith("Input bandwidth utilization"))
                    {
                        string[] tokens = lineTrim.Split(new char[] { ':' });
                        if (tokens.Length == 2)
                        {
                            string per = tokens[1].Trim();
                            if (per.EndsWith("%"))
                            {
                                string pertrim = per.TrimEnd('%');
                                if (float.TryParse(pertrim, out float pertrimf)) currentInterfaceToDatabase.TrafficInput = pertrimf;
                            }
                        }
                    }
                    else if (currentInterfaceToDatabase != null && currentInterfaceToDatabase.InterfaceType != null && lineTrim.StartsWith("Output bandwidth utilization"))
                    {
                        string[] tokens = lineTrim.Split(new char[] { ':' });
                        if (tokens.Length == 2)
                        {
                            string per = tokens[1].Trim();
                            if (per.EndsWith("%"))
                            {
                                string pertrim = per.TrimEnd('%');
                                if (float.TryParse(pertrim, out float pertrimf)) currentInterfaceToDatabase.TrafficOutput = pertrimf;
                            }
                        }

                        currentInterfaceToDatabase = null;
                    }

                    if (currentInterfaceToDatabase != null && line.IndexOf("Maximum Transmit Unit") > -1)
                    {
                        //,The Maximum Transmit Unit is 1600
                        //     012345678901234567890123456789
                        //               1         2
                        int idx = line.IndexOf("Maximum Transmit");
                        string cot = line.Substring(idx + 25).Split(new char[] { ' ', ',', '.' })[0].Trim();

                        if (int.TryParse(cot, out int rmtu))
                        {
                            currentInterfaceToDatabase.RunMTU = rmtu;
                        }
                    }

                    if (currentInterfaceToDatabase != null && line.StartsWith("Switch Port"))
                    {
                        currentInterfaceToDatabase.Mode = 'N';
                    }
                }

                if (Request("display interface brief", out lines, probe)) return probe;

                begin = false;
                int aggr = -1;

                foreach (string line in lines)
                {
                    if (begin)
                    {
                        string lineTrim = line.Trim();
                        string[] tokens = lineTrim.Split(StringSplitTypes.Space, StringSplitOptions.RemoveEmptyEntries);

                        string poe = tokens[0].Trim();
                        NetworkInterface inf = NetworkInterface.Parse(poe);

                        if (aggr > -1 && line.StartsWith(" "))
                        {
                            if (!poe.StartsWith("Eth-Trunk"))
                            {
                                if (inf != null)
                                {
                                    string normal = NetworkInterface.HuaweiName(inf);

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

                        // untuk huawei 5.7 dan 5.9
                        if (nodeVersion == "5.70" || nodeVersion == "5.90")
                        {
                            // 0 => interface, 1 => status, 2 => protocol
                            if (tokens.Length >= 3)
                            {
                                if (inf != null)
                                {
                                    string ifname = NetworkInterface.HuaweiName(inf);

                                    if (interfacelive.ContainsKey(ifname))
                                    {
                                        interfacelive[ifname].Status = tokens[1] == "up";
                                        interfacelive[ifname].Protocol = tokens[2] == "up";
                                        interfacelive[ifname].Enable = tokens[1] != "*down";
                                    }
                                }
                            }
                        }

                        // fix interface type yg ga kebaca klo status protocol down
                        if (inf != null)
                        {
                            string ifname = NetworkInterface.HuaweiName(inf);

                            if (interfacelive.ContainsKey(ifname) && !inf.IsSubInterface)
                            {
                                MEInterfaceToDatabase ive = interfacelive[ifname];

                                if (ive.Status == false && inf.Type != ive.InterfaceType)
                                {
                                    ive.InterfaceType = inf.Type;
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
                foreach (string[] strs in hweCircuitMplsL2vc)
                {
                    // cinterface, cinterfaceSDP, cinterfacestate.ToString(), vcidname, cinterfaceVCID, cinterfacewithstate
                    //  0            1              2                          3         4              5

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

                // vll ccc ke port
                foreach (string[] strs in hweCircuitVllCcc)
                {
                    /// cccname, cccif1, cccif2, vcidname
                    string vcidname = strs[3];
                    string inf1 = strs[1];
                    string inf2 = strs[2];

                    if (circuitdb.ContainsKey(vcidname))
                    {
                        NetworkInterface nif1 = NetworkInterface.Parse(inf1);

                        if (nif1 != null)
                        {
                            string inf = nif1.Name;
                            if (interfacelive.ContainsKey(inf))
                            {
                                string cid = circuitdb[vcidname]["MC_ID"].ToString();
                                interfacelive[inf].CircuitID = cid;
                            }
                        }

                        NetworkInterface nif2 = NetworkInterface.Parse(inf2);

                        if (nif2 != null)
                        {
                            string inf = nif2.Name;
                            if (interfacelive.ContainsKey(inf))
                            {
                                string cid = circuitdb[vcidname]["MC_ID"].ToString();
                                interfacelive[inf].CircuitID = cid;
                            }
                        }
                    }
                }

                // vsi ke port (l2 binding vsi)
                if (Request("display vsi services all", out lines, probe)) return probe;

                //GigabitEthernet7/0/3.2999           "ZZZ ZZZ"                       up
                //GigabitEthernet7/0/10.20            OAMN-MSAN-PWT02
                //0123456789012345678901234567890123456789012345678901234567890123456789
                //          1         2         3         4         5         6
                //                                    1234567890123456789012345678901
                //Eth-Trunk2.50                       Speedy_Management_Punung_50

                foreach (string line in lines)
                {
                    if (line.Length >= 68)
                    {
                        string se = line.Substring(0, 36).Trim();
                        NetworkInterface nif = NetworkInterface.Parse(se);

                        if (nif != null)
                        {
                            string portnif = nif.Name;
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
                if (Request("display cur int | in interface |qos-profile |qos\\ car\\ cir|user-queue |vlan-type\\ dot1q|mtu ", out lines, probe)) return probe;

                string currentInterface = null;
                string currentParent = null;
                int typerate = -1;
                int currentKBPS = -1;

                foreach (string line in lines)
                {
                    string lineTrim = line.Trim();

                    if (lineTrim.StartsWith("interface "))
                    {
                        currentInterface = null;
                        currentParent = null;
                        typerate = -1;
                        currentKBPS = -1;

                        string nifc = lineTrim.Substring(10);
                        NetworkInterface nif = NetworkInterface.Parse(nifc);
                        if (nif != null)
                        {
                            string portnif = nif.Name;
                            if (interfacelive.ContainsKey(portnif))
                            {
                                currentInterface = portnif;
                                if (nif.IsSubInterface)
                                {
                                    string bport = nif.BaseName;
                                    if (interfacelive.ContainsKey(bport))
                                    {
                                        currentParent = bport;
                                        typerate = nif.TypeRate;
                                    }
                                }
                            }
                        }
                    }
                    else if (currentInterface != null)
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

                                    if (qosDir == "inbound") interfacelive[currentInterface].IngressID = qosID;
                                    else if (qosDir == "outbound") interfacelive[currentInterface].EgressID = qosID;
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

                                if (rDir == "inbound")
                                {
                                    interfacelive[currentInterface].RateInput = size;

                                    if (currentParent != null)
                                    {
                                        if (size > 0)
                                        {
                                            int cur = interfacelive[currentParent].CirConfigTotalInput;
                                            if (cur == -1) cur = 0;
                                            interfacelive[currentParent].CirConfigTotalInput = cur + size;

                                            long curR = interfacelive[currentParent].CirTotalInput;
                                            if (curR == -1) curR = 0;
                                            interfacelive[currentParent].CirTotalInput = curR + size;
                                        }
                                        else if (typerate > -1)
                                        {
                                            long curR = interfacelive[currentParent].CirTotalInput;
                                            if (curR == -1) curR = 0;
                                            interfacelive[currentParent].CirTotalInput = curR + typerate;
                                        }
                                    }
                                }
                                else if (rDir == "outbound")
                                {
                                    interfacelive[currentInterface].RateOutput = size;

                                    if (currentParent != null)
                                    {
                                        if (size > 0)
                                        {
                                            int cur = interfacelive[currentParent].CirConfigTotalOutput;
                                            if (cur == -1) cur = 0;
                                            interfacelive[currentParent].CirConfigTotalOutput = cur + size;

                                            long curR = interfacelive[currentParent].CirTotalOutput;
                                            if (curR == -1) curR = 0;
                                            interfacelive[currentParent].CirTotalOutput = curR + size;
                                        }
                                        else if (typerate > -1)
                                        {
                                            long curR = interfacelive[currentParent].CirTotalOutput;
                                            if (curR == -1) curR = 0;
                                            interfacelive[currentParent].CirTotalOutput = curR + typerate;
                                        }
                                    }
                                }
                            }
                        }
                        else if (lineTrim.StartsWith("vlan-type"))
                        {
                            //vlan-type dot1q 110
                            //012345678901234567890123456789
                            int dot1q;
                            if (int.TryParse(lineTrim.Substring(16), out dot1q)) interfacelive[currentInterface].Dot1Q = dot1q;
                        }
                        else if (currentKBPS != -1 && (lineTrim.EndsWith("inbound") || lineTrim.EndsWith("outbound")))
                        {
                            bool inbound = false;
                            if (lineTrim.EndsWith("inbound")) inbound = true;

                            int kbps = currentKBPS;

                            if (inbound) interfacelive[currentInterface].RateInput = kbps;
                            else interfacelive[currentInterface].RateOutput = kbps;

                            if (currentParent != null)
                            {
                                if (kbps > 0)
                                {
                                    if (inbound)
                                    {
                                        int cur = interfacelive[currentParent].CirConfigTotalInput;
                                        if (cur == -1) cur = 0;
                                        interfacelive[currentParent].CirConfigTotalInput = cur + kbps;

                                        long curR = interfacelive[currentParent].CirTotalInput;
                                        if (curR == -1) curR = 0;
                                        interfacelive[currentParent].CirTotalInput = curR + kbps;
                                    }
                                    else
                                    {
                                        int cur = interfacelive[currentParent].CirConfigTotalOutput;
                                        if (cur == -1) cur = 0;
                                        interfacelive[currentParent].CirConfigTotalOutput = cur + kbps;

                                        long curR = interfacelive[currentParent].CirTotalOutput;
                                        if (curR == -1) curR = 0;
                                        interfacelive[currentParent].CirTotalOutput = curR + kbps;
                                    }
                                }
                                else if (typerate > -1)
                                {
                                    if (inbound)
                                    {
                                        long curR = interfacelive[currentParent].CirTotalInput;
                                        if (curR == -1) curR = 0;
                                        interfacelive[currentParent].CirTotalInput = curR + typerate;
                                    }
                                    else
                                    {
                                        long curR = interfacelive[currentParent].CirTotalOutput;
                                        if (curR == -1) curR = 0;
                                        interfacelive[currentParent].CirTotalOutput = curR + typerate;
                                    }
                                }
                            }
                        }
                        else if (lineTrim.StartsWith("qos car cir"))
                        {
                            currentKBPS = -1;

                            string[] splits = lineTrim.Split(StringSplitTypes.Space);
                            if (splits.Length > 3)
                            {
                                int kbps;
                                if (int.TryParse(splits[3], out kbps))
                                {
                                    int direction = 0;
                                    if (lineTrim.EndsWith("inbound")) direction = -1;
                                    else if (lineTrim.EndsWith("outbound")) direction = 1;

                                    if (direction != 0)
                                    {
                                        bool inbound = direction == -1;

                                        if (inbound) interfacelive[currentInterface].RateInput = kbps;
                                        else interfacelive[currentInterface].RateOutput = kbps;

                                        if (currentParent != null)
                                        {
                                            if (kbps > 0)
                                            {
                                                if (inbound)
                                                {
                                                    int cur = interfacelive[currentParent].CirConfigTotalInput;
                                                    if (cur == -1) cur = 0;
                                                    interfacelive[currentParent].CirConfigTotalInput = cur + kbps;

                                                    long curR = interfacelive[currentParent].CirTotalInput;
                                                    if (curR == -1) curR = 0;
                                                    interfacelive[currentParent].CirTotalInput = curR + kbps;
                                                }
                                                else
                                                {
                                                    int cur = interfacelive[currentParent].CirConfigTotalOutput;
                                                    if (cur == -1) cur = 0;
                                                    interfacelive[currentParent].CirConfigTotalOutput = cur + kbps;

                                                    long curR = interfacelive[currentParent].CirTotalOutput;
                                                    if (curR == -1) curR = 0;
                                                    interfacelive[currentParent].CirTotalOutput = curR + kbps;
                                                }
                                            }
                                            else if (typerate > -1)
                                            {
                                                if (inbound)
                                                {
                                                    long curR = interfacelive[currentParent].CirTotalInput;
                                                    if (curR == -1) curR = 0;
                                                    interfacelive[currentParent].CirTotalInput = curR + typerate;
                                                }
                                                else
                                                {
                                                    long curR = interfacelive[currentParent].CirTotalOutput;
                                                    if (curR == -1) curR = 0;
                                                    interfacelive[currentParent].CirTotalOutput = curR + typerate;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        currentKBPS = kbps;
                                    }
                                }
                            }
                        }
                        else if (lineTrim.StartsWith("mtu"))
                        {
                            //mtu 1600
                            //01234
                            if (currentParent == null)
                            {
                                if (int.TryParse(lineTrim.Substring(4).Trim(), out int cmtu))
                                {
                                    interfacelive[currentInterface].AdmMTU = cmtu;
                                }
                            }
                        }
                    }
                }


                // additional data
                if (Request("display ospf peer | in Router ID|interface", out lines, probe)) return probe;


 //               Area 0.0.0.80 interface 172.31.43.17 (Eth-Trunk209)'s neighbors
 //Router ID: 172.31.63.136        Address: 172.31.43.18      GR State: Normal
 //012345678901

                string einf = null;
                string target = null;
                
                foreach (var line in lines)
                {
                    var linet = line.Trim();

                    if (linet.Length > 0)
                    {
                        if (linet.StartsWith("Area "))
                        {
                            var xda = linet.IndexOf("(");
                            if (xda > -1)
                            {
                                var yda = linet.IndexOf(")", xda);
                                var ifname = linet.Substring(xda + 1, yda - xda - 1);

                                NetworkInterface nif = NetworkInterface.Parse(ifname);

                                if (nif != null && interfacelive.ContainsKey(nif.Name))
                                {
                                    einf = nif.Name;
                                }
                            }
                        }
                        else if (einf != null && linet.StartsWith("Router ID:"))
                        {
                            if (linet.Length > 13)
                            {
                                var kx = linet.Substring(11);
                                var spaceafter = kx.IndexOf(' ');
                                var ip = kx.Substring(0, spaceafter);


                                if (ipnodedb.ContainsKey(ip))
                                    interfacelive[einf].TopologyNOID = ipnodedb[ip]["NO_ID"].ToString();
                            }

                            einf = null;
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

                            if (nodeManufacture == hwe && interfacelive[parent].Encapsulation == '-')
                            {
                                interfacelive[parent].Encapsulation = 'D';
                                interfacelive[parent].Mode = 'A';
                            }
                        }
                    }
                    else
                    {
                        li.SubInterfaceCount = 0;
                    }
                }
            }

            List<Tuple<string, string, string, string, string, string, string>> vMEPhysicalInterfaces = null;
            bool vExists = false;
            lock (NecrowVirtualization.MESync)
            {
                foreach (Tuple<string, List<Tuple<string, string, string, string, string, string, string>>> v in NecrowVirtualization.MEPhysicalInterfaces)
                {
                    if (v.Item1 == NodeName)
                    {
                        vMEPhysicalInterfaces = v.Item2;
                        vExists = true;
                        break;
                    }
                }
                if (!vExists)
                {
                    vMEPhysicalInterfaces = new List<Tuple<string, string, string, string, string, string, string>>();
                    NecrowVirtualization.MEPhysicalInterfaces.Add(new Tuple<string, List<Tuple<string, string, string, string, string, string, string>>>(NodeName, vMEPhysicalInterfaces));
                    NecrowVirtualization.MEPhysicalInterfacesSort(true);
                }
            }

            int sinf = 0, sinfup = 0, sinfag = 0, sinfhu = 0, sinfhuup = 0, sinfte = 0, sinfteup = 0, sinfgi = 0, sinfgiup = 0, sinffa = 0, sinffaup = 0, sinfet = 0, sinfetup = 0,
                ssubinf = 0, ssubinfup = 0, ssubinfupup = 0, ssubinfag = 0, ssubinfagup = 0, ssubinfagupup = 0, ssubinfhu = 0, ssubinfhuup = 0, ssubinfhuupup = 0, ssubinfte = 0, ssubinfteup = 0, ssubinfteupup = 0, ssubinfgi = 0, ssubinfgiup = 0, ssubinfgiupup = 0, ssubinffa = 0, ssubinffaup = 0, ssubinffaupup = 0, ssubinfet = 0, ssubinfetup = 0, ssubinfetupup = 0;

            ServiceImmediateDiscovery(interfacelive);

            foreach (KeyValuePair<string, MEInterfaceToDatabase> pair in interfacelive)
            {
                MEInterfaceToDatabase li = pair.Value;

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
                    if (inftype == "Ex") inftype = li.InterfaceType;

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
                            li.ParentID = interfacedb[parentPort]["MI_ID"].ToString();
                        else if (interfaceinsert.ContainsKey(parentPort)) // cek di interface yg baru
                            li.ParentID = interfaceinsert[parentPort].Id;
                    }

                    if (!inf.IsSubInterface)
                    {
                        if (inftype == "Ag")
                        {
                            // we need support with child
                            if (int.TryParse(inf.Interface, out int myaggr))
                            {
                                // cari child di interfacelive yg aggr-nya myaggr
                                List<MEInterfaceToDatabase> agPhysicals = new List<MEInterfaceToDatabase>();
                                foreach (KeyValuePair<string, MEInterfaceToDatabase> spair in interfacelive)
                                {
                                    if (spair.Value.Aggr == myaggr)
                                        agPhysicals.Add(spair.Value);
                                }
                                li.AggrChilds = agPhysicals.ToArray();

                                // cari neighbor per anak
                                // jika setiap anak memiliki neighbor YANG PARENT IDNYA TIDAK NULL DAN SAMA, maka set ke adjacentParentID
                                string childNeighborParentID = null;
                                int neighborType = 0; // 1: PE, 2: ME

                                foreach (MEInterfaceToDatabase mi in li.AggrChilds)
                                {
                                    FindPhysicalNeighbor(mi); // find topology anaknya dulu

                                    if (mi.AggrNeighborParentID == null)
                                    {
                                        childNeighborParentID = null;
                                        break;
                                    }
                                    else if (childNeighborParentID == null)
                                    {
                                        childNeighborParentID = mi.AggrNeighborParentID;

                                        if (mi.TopologyPEInterfaceID != null) neighborType = 1;
                                        else if (mi.TopologyMEInterfaceID != null) neighborType = 2;
                                    }
                                    else if (childNeighborParentID != mi.AggrNeighborParentID)
                                    {
                                        childNeighborParentID = null;
                                        break;
                                    }
                                }

                                // jika child ga nyambung, coba orang tua
                                //if (childNeighborParentID == null)
                                //{
                                //    FindPhysicalNeighbor(li);
                                //}

                                // jika ME, gunakan MI_TO_NO
                                if (childNeighborParentID == null)
                                {
                                    if (li.TopologyNOID != null)
                                    {
                                        result = j.Query("select MI_ID, MI_Description from MEInterface where MI_NO = {0} and MI_TO_NO = {1}", li.TopologyNOID, nodeID);

                                        if (result.Count == 1)
                                        {
                                            li.TopologyMEInterfaceID = result[0]["MI_ID"].ToString();
                                        }
                                        else if (result.Count > 1)
                                        {
                                            foreach (var row in result)
                                            {
                                                var idesc = row["MI_Description"].ToString();
                                                var iid = row["MI_ID"].ToString();

                                                //string cdesc = CleanDescription(idesc, neighborMEName);
                                            }
                                        }
                                    }
                                }

                                // adjacentParentID adalah parentID (aggr) dari lawannya interface aggr ini di PE.
                                if (childNeighborParentID != null)
                                {
                                    // if topology to PE
                                    if (neighborType == 1)
                                    {
                                        li.TopologyPEInterfaceID = childNeighborParentID;

                                        // query lawan
                                        li.ChildrenNeighbor = new Dictionary<int, Tuple<string, string, string>>();
                                        result = j.Query("select PI_ID, PI_DOT1Q, PI_TO_MI, PI_TO_PI from PEInterface where PI_PI = {0}", li.TopologyPEInterfaceID);
                                        if (!result) return DatabaseFailure(probe);
                                        foreach (Row2 row in result)
                                        {
                                            if (!row["PI_DOT1Q"].IsNull)
                                            {
                                                int dot1q = row["PI_DOT1Q"].ToIntShort();
                                                if (!li.ChildrenNeighbor.ContainsKey(dot1q)) li.ChildrenNeighbor.Add(dot1q,
                                                    new Tuple<string, string, string>(row["PI_ID"].ToString(), row["PI_TO_MI"].ToString(), row["PI_TO_PI"].ToString()));
                                            }
                                        }
                                    }
                                    else if (neighborType == 2) // to ME
                                    {
                                        li.TopologyMEInterfaceID = childNeighborParentID;

                                        // query lawan
                                        li.ChildrenNeighbor = new Dictionary<int, Tuple<string, string, string>>();
                                        result = j.Query("select MI_ID, MI_DOT1Q, MI_TO_MI, MI_TO_PI from MEInterface where MI_MI = {0}", li.TopologyMEInterfaceID);
                                        if (!result) return DatabaseFailure(probe);
                                        foreach (Row2 row in result)
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
                        }
                        else
                        {
                            FindPhysicalNeighbor(li);

                        }
                    }
                    else if (li.ParentID != null) // subif dan anak aggregate
                    {
                        if (interfacelive.ContainsKey(parentPort))
                        {
                            MEInterfaceToDatabase parent = interfacelive[parentPort];

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
                                        if (parent.TopologyPEInterfaceID != null) // berarti lawannya PE
                                        {
                                            li.TopologyPEInterfaceID = parentChildrenNeighbor.Item1;
                                            li.NeighborCheckPITOMI = parentChildrenNeighbor.Item2;
                                        }
                                        else // lawannya ME
                                        {
                                            li.TopologyMEInterfaceID = parentChildrenNeighbor.Item1;
                                            li.NeighborCheckMITOMI = parentChildrenNeighbor.Item2;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                li.EquipmentName = NetworkInterface.EquipmentName(nodeManufacture + (nodeManufacture == cso && nodeVersion == xr ? "XR" : ""), li.Name);

                if (!interfacedb.ContainsKey(pair.Key))
                {
                    Event("Interface ADD: " + pair.Key);

                    li.Id = Database2.ID();
                    interfaceinsert.Add(li.Name, li);
                }
                else
                {
                    Row2 db = interfacedb[pair.Key];

                    MEInterfaceToDatabase u = new MEInterfaceToDatabase();
                    u.Id = db["MI_ID"].ToString();
                    li.Id = u.Id;

                    bool update = false;
                    StringBuilder updateinfo = new StringBuilder();

                    if (db["MI_MI"].ToString() != li.ParentID)
                    {
                        update = true;
                        u.UpdateParentID = true;
                        u.ParentID = li.ParentID;
                        UpdateInfo(updateinfo, "parent", db["MI_MI"].ToString(), li.ParentID, true);
                    }
                    if (db["MI_TO_PI"].ToString() != li.TopologyPEInterfaceID)
                    {
                        update = true;
                        u.UpdateTopologyPEInterfaceID = true;
                        u.TopologyPEInterfaceID = li.TopologyPEInterfaceID;
                        UpdateInfo(updateinfo, "mi-to-pi", db["MI_TO_PI"].ToString(), li.TopologyPEInterfaceID, true);
                    }
                    else if (li.TopologyPEInterfaceID != null && li.NeighborCheckPITOMI != u.Id)
                    {
                        update = true;
                        u.UpdateNeighborCheckPITOMI = true;
                        u.TopologyPEInterfaceID = li.TopologyPEInterfaceID;
                        UpdateInfo(updateinfo, "neighbor-pi-to-mi", li.NeighborCheckPITOMI, u.Id, true);
                    }
                    if (db["MI_TO_MI"].ToString() != li.TopologyMEInterfaceID)
                    {
                        update = true;
                        u.UpdateTopologyMEInterfaceID = true;
                        u.TopologyMEInterfaceID = li.TopologyMEInterfaceID;
                        UpdateInfo(updateinfo, "mi-to-mi", db["MI_TO_MI"].ToString(), li.TopologyMEInterfaceID, true);
                    }
                    else if (li.TopologyMEInterfaceID != null && li.NeighborCheckMITOMI != u.Id)
                    {
                        update = true;
                        u.UpdateNeighborCheckMITOMI = true;
                        u.TopologyMEInterfaceID = li.TopologyMEInterfaceID;
                        UpdateInfo(updateinfo, "neighbor-mi-to-mi", li.NeighborCheckMITOMI, u.Id, true);
                    }
                    if (db["MI_TO_NI"].ToString() != li.TopologyNBInterfaceID)
                    {
                        update = true;
                        u.UpdateTopologyNBInterfaceID = true;
                        u.TopologyNBInterfaceID = li.TopologyNBInterfaceID;
                        UpdateInfo(updateinfo, "mi-to-ni", db["MI_TO_NI"].ToString(), li.TopologyNBInterfaceID, true);
                    }
                    if (db["MI_TO_NO"].ToString() != li.TopologyNOID)
                    {
                        update = true;
                        u.UpdateTopologyNOID = true;
                        u.TopologyNOID = li.TopologyNOID;
                        UpdateInfo(updateinfo, "mi-to-no", db["MI_TO_NO"].ToString(), li.TopologyNOID, true);
                    }
                    if (db["MI_Description"].ToString() != li.Description)
                    {
                        update = true;
                        u.UpdateDescription = true;
                        u.Description = li.Description;
                        UpdateInfo(updateinfo, "description", db["MI_Description"].ToString(), li.Description, true);
                    }
                    if (db["MI_SI"] != li.ServiceImmediateID)
                    {
                        update = true;
                        u.UpdateServiceImmediateID = true;
                        u.ServiceImmediateID = li.ServiceImmediateID;
                        UpdateInfo(updateinfo, "service-immediate", "UPDATING");
                    }
                    if (db["MI_Status"].ToBool() != li.Status)
                    {
                        update = true;
                        u.UpdateStatus = true;
                        u.Status = li.Status;
                        UpdateInfo(updateinfo, "status", db["MI_Status"].ToBool().DescribeUpDown(), li.Status.DescribeUpDown());
                    }
                    if (db["MI_Protocol"].ToBool() != li.Protocol)
                    {
                        update = true;
                        u.UpdateProtocol = true;
                        u.Protocol = li.Protocol;
                        UpdateInfo(updateinfo, "protocol", db["MI_Protocol"].ToBool().DescribeUpDown(), li.Protocol.DescribeUpDown());
                    }
                    if (db["MI_Enable"].ToBool() != li.Enable)
                    {
                        update = true;
                        u.UpdateEnable = true;
                        u.Enable = li.Enable;
                        UpdateInfo(updateinfo, "enable", db["MI_Enable"].ToBool().DescribeTrueFalse(), li.Enable.DescribeTrueFalse());
                    }
                    if (db["MI_EquipmentName"].ToString() != li.EquipmentName)
                    {
                        update = true;
                        u.UpdateEquipmentName = true;
                        u.EquipmentName = li.EquipmentName;
                        UpdateInfo(updateinfo, "equipment-name", db["MI_EquipmentName"].ToString(), li.EquipmentName);
                    }
                    if (db["MI_DOT1Q"].ToIntShort(-1) != li.Dot1Q)
                    {
                        update = true;
                        u.UpdateDot1Q = true;
                        u.Dot1Q = li.Dot1Q;
                        UpdateInfo(updateinfo, "dot1q", db["MI_DOT1Q"].ToIntShort(-1).NullableInfo(), li.Dot1Q.NullableInfo());
                    }
                    if (db["MI_Aggregator"].ToIntShort(-1) != li.Aggr)
                    {
                        update = true;
                        u.UpdateAggr = true;
                        u.Aggr = li.Aggr;
                        UpdateInfo(updateinfo, "aggr", db["MI_Aggregator"].ToIntShort(-1).NullableInfo(), li.Aggr.NullableInfo());
                    }
                    if (db["MI_MC"].ToString() != li.CircuitID)
                    {
                        update = true;
                        u.UpdateCircuit = true;
                        u.CircuitID = li.CircuitID;
                        UpdateInfo(updateinfo, "circuit", db["MI_MC"].ToString(), li.CircuitID, true);
                    }
                    if (db["MI_Mode"].ToChar('-') != li.Mode)
                    {
                        update = true;
                        u.UpdateMode = true;
                        u.Mode = li.Mode;
                        UpdateInfo(updateinfo, "mode", db["MI_Mode"].ToChar('-').NullableInfo(), li.Mode.NullableInfo());
                    }
                    if (db["MI_Encapsulation"].ToChar('-') != li.Encapsulation)
                    {
                        update = true;
                        u.UpdateEncapsulation = true;
                        u.Encapsulation = li.Encapsulation;
                        UpdateInfo(updateinfo, "encapsulation", db["MI_Encapsulation"].ToChar('-').NullableInfo(), li.Mode.NullableInfo());
                    }
                    if (db["MI_MTU"].ToIntShort(-1) != li.RunMTU)
                    {
                        update = true;
                        u.UpdateRunMTU = true;
                        u.RunMTU = li.RunMTU;
                        UpdateInfo(updateinfo, "operational-mtu", db["MI_MTU"].ToIntShort(-1).NullableInfo(), li.RunMTU.NullableInfo());
                    }
                    if (db["MI_MTU_Config"].ToIntShort(-1) != li.AdmMTU)
                    {
                        update = true;
                        u.UpdateAdmMTU = true;
                        u.AdmMTU = li.AdmMTU;
                        UpdateInfo(updateinfo, "config-mtu", db["MI_MTU_Config"].ToIntShort(-1).NullableInfo(), li.AdmMTU.NullableInfo());
                    }
                    if (db["MI_Type"].ToString() != li.InterfaceType)
                    {
                        update = true;
                        u.UpdateInterfaceType = true;
                        u.InterfaceType = li.InterfaceType;
                        UpdateInfo(updateinfo, "type", db["MI_Type"].ToString(), li.InterfaceType);
                    }
                    if (db["MI_MQ_Input"].ToString() != li.IngressID)
                    {
                        update = true;
                        u.UpdateIngressID = true;
                        u.IngressID = li.IngressID;
                        UpdateInfo(updateinfo, "qos-input", db["MI_MQ_Input"].ToString(), li.IngressID, true);
                    }
                    if (db["MI_MQ_Output"].ToString() != li.EgressID)
                    {
                        update = true;
                        u.UpdateEgressID = true;
                        u.EgressID = li.EgressID;
                        UpdateInfo(updateinfo, "qos-output", db["MI_MQ_Output"].ToString(), li.EgressID, true);
                    }
                    if (db["MI_Rate_Input"].ToInt(-1) != li.RateInput)
                    {
                        update = true;
                        u.UpdateRateInput = true;
                        u.RateInput = li.RateInput;
                        UpdateInfo(updateinfo, "rate-input", db["MI_Rate_Input"].ToInt(-1).NullableInfo(), li.RateInput.NullableInfo());
                    }
                    if (db["MI_Rate_Output"].ToInt(-1) != li.RateOutput)
                    {
                        update = true;
                        u.UpdateRateOutput = true;
                        u.RateOutput = li.RateOutput;
                        UpdateInfo(updateinfo, "rate-output", db["MI_Rate_Output"].ToInt(-1).NullableInfo(), li.RateOutput.NullableInfo());
                    }
                    if (db["MI_Info"].ToString() != li.Info)
                    {
                        update = true;
                        u.UpdateInfo = true;
                        u.Info = li.Info;
                        UpdateInfo(updateinfo, "info", db["MI_Info"].ToString(), li.Info);
                    }
                    if (db["MI_LastDown"].ToNullableDateTime() != li.LastDown)
                    {
                        update = true;
                        u.UpdateLastDown = true;
                        u.LastDown = li.LastDown;
                        UpdateInfo(updateinfo, "lastdown", db["MI_LastDown"].ToNullableDateTime().ToString(), li.LastDown.ToString(), true);
                    }
                    if (db["MI_Percentage_TrafficInput"].ToFloat(-1) != li.TrafficInput)
                    {
                        update = true;
                        u.UpdateTrafficInput = true;
                        u.TrafficInput = li.TrafficInput;
                        UpdateInfo(updateinfo, "traffic-input", db["MI_Percentage_TrafficInput"].ToFloat(-1).NullableInfo(), li.TrafficInput.NullableInfo(), true);
                    }
                    if (db["MI_Percentage_TrafficOutput"].ToFloat(-1) != li.TrafficOutput)
                    {
                        update = true;
                        u.UpdateTrafficOutput = true;
                        u.TrafficOutput = li.TrafficOutput;
                        UpdateInfo(updateinfo, "traffic-output", db["MI_Percentage_TrafficOutput"].ToFloat(-1).NullableInfo(), li.TrafficOutput.NullableInfo(), true);
                    }
                    if (db["MI_Summary_CIRConfigTotalInput"].ToInt(-1) != li.CirConfigTotalInput)
                    {
                        update = true;
                        u.UpdateCirConfigTotalInput = true;
                        u.CirConfigTotalInput = li.CirConfigTotalInput;
                        UpdateInfo(updateinfo, "circonf-input", db["MI_Summary_CIRConfigTotalInput"].ToInt(-1).NullableInfo(), li.CirConfigTotalInput.NullableInfo());
                    }
                    if (db["MI_Summary_CIRConfigTotalOutput"].ToInt(-1) != li.CirConfigTotalOutput)
                    {
                        update = true;
                        u.UpdateCirConfigTotalOutput = true;
                        u.CirConfigTotalOutput = li.CirConfigTotalOutput;
                        UpdateInfo(updateinfo, "circonf-output", db["MI_Summary_CIRConfigTotalOutput"].ToInt(-1).NullableInfo(), li.CirConfigTotalOutput.NullableInfo());
                    }
                    if (db["MI_Summary_CIRTotalInput"].ToLong(-1) != li.CirTotalInput)
                    {
                        update = true;
                        u.UpdateCirTotalInput = true;
                        u.CirTotalInput = li.CirTotalInput;
                        UpdateInfo(updateinfo, "cir-input", db["MI_Summary_CIRTotalInput"].ToLong(-1).NullableInfo(), li.CirTotalInput.NullableInfo());
                    }
                    if (db["MI_Summary_CIRTotalOutput"].ToLong(-1) != li.CirTotalOutput)
                    {
                        update = true;
                        u.UpdateCirTotalOutput = true;
                        u.CirTotalOutput = li.CirTotalOutput;
                        UpdateInfo(updateinfo, "cir-output", db["MI_Summary_CIRTotalOutput"].ToLong(-1).NullableInfo(), li.CirTotalOutput.NullableInfo());
                    }
                    if (db["MI_Summary_SubInterfaceCount"].ToIntShort(-1) != li.SubInterfaceCount)
                    {
                        update = true;
                        u.UpdateSubInterfaceCount = true;
                        u.SubInterfaceCount = li.SubInterfaceCount;
                        UpdateInfo(updateinfo, "subif-count", db["MI_Summary_SubInterfaceCount"].ToIntShort(-1).NullableInfo(), li.SubInterfaceCount.NullableInfo());
                    }

                    if (update)
                    {
                        Event("Interface UPDATE: " + pair.Key + " " + updateinfo.ToString());
                        interfaceupdate.Add(u);
                    }
                }
            }

            // NECROW 3.0
            /*
            foreach (KeyValuePair<string, MEInterfaceToDatabase> pair in interfacelive)
            {
                MEInterfaceToDatabase li = pair.Value;

                NetworkInterface inf = NetworkInterface.Parse(li.Name);

                // OPTIMUM RATE
                int ori = 0, oro = 0;

                int cr = -1;
                if (inf != null)
                {
                    if (inf.Type == "Ag")
                    {
                        MEInterfaceToDatabase aggrInterface = null;

                        if (inf.IsSubInterface)
                        {
                            if (interfacelive.ContainsKey(inf.BaseName))
                            {
                                aggrInterface = interfacelive[inf.BaseName];
                            }
                        }
                        else
                        {
                            aggrInterface = li;
                        }

                        if (aggrInterface != null)
                        {
                            cr = 0;
                            foreach (MEInterfaceToDatabase pix in aggrInterface.AggrChilds)
                            {
                                string ty = pix.InterfaceType;

                                int typerate = 0;
                                if (ty == "Hu") typerate = 104857600;
                                else if (ty == "Te") typerate = 10485760;
                                else if (ty == "Gi") typerate = 1048576;
                                else if (ty == "Fa") typerate = 102400;
                                else if (ty == "Et") typerate = 10240;
                                else if (ty == "Se") typerate = 0;

                                cr += typerate;
                            }
                        }
                    }
                    else
                    {
                        if (inf.IsSubInterface)
                        {
                            if (interfacelive.ContainsKey(inf.BaseName))
                            {
                                MEInterfaceToDatabase par = interfacelive[inf.BaseName];

                                string ty = par.InterfaceType;

                                int typerate = 0;
                                if (ty == "Hu") typerate = 104857600;
                                else if (ty == "Te") typerate = 10485760;
                                else if (ty == "Gi") typerate = 1048576;
                                else if (ty == "Fa") typerate = 102400;
                                else if (ty == "Et") typerate = 10240;
                                else if (ty == "Se") typerate = 0;

                                cr = typerate;
                            }
                        }
                        else
                        {
                            string ty = li.InterfaceType;

                            int typerate = 0;
                            if (ty == "Hu") typerate = 104857600;
                            else if (ty == "Te") typerate = 10485760;
                            else if (ty == "Gi") typerate = 1048576;
                            else if (ty == "Fa") typerate = 102400;
                            else if (ty == "Et") typerate = 10240;
                            else if (ty == "Se") typerate = 0;

                            cr = typerate;
                        }
                    }
                }

                if (cr != -1)
                {
                    int dri = li.RateInput == -1 ? int.MaxValue : li.RateInput;
                    int dro = li.RateOutput == -1 ? int.MaxValue : li.RateOutput;

                    int qri = int.MaxValue, qro = int.MaxValue;
                    foreach (KeyValuePair<string, Row2> qpair in qosdb)
                    {
                        string rid = qpair.Value["MQ_ID"];
                        bool rdi = qpair.Value["MQ_Type"];

                        if (rid == li.IngressID && !rdi)
                            qri = qpair.Value["MQ_Bandwidth"].ToInt(int.MaxValue);
                        if (rid == li.EgressID && rdi)
                            qro = qpair.Value["MQ_Bandwidth"].ToInt(int.MaxValue);
                    }

                    ori = int.MaxValue;
                    if (dri < ori) ori = dri;
                    if (qri < ori) ori = qri;
                    if (cr < ori) ori = cr;

                    oro = int.MaxValue;
                    if (dro < oro) oro = dro;
                    if (qro < oro) oro = qro;
                    if (cr < oro) oro = cr;
                }

                // INTERFACE
                Result2 ifr = j.Query("select IF_ID, IF_Entry_State, IF_OptimumRate_Input, IF_OptimumRate_Output from Interface where IF_NO = {0} and IF_EquipmentName = {1}", nodeID, li.EquipmentName);

                if (ifr == 0)
                {
                    Insert ifi = j.Insert("Interface");
                    li.InterfaceID = ifi.Key("IF_ID");
                    ifi.Value("IF_Entry_State", 1);
                    ifi.Value("IF_Entry_LastToggle", DateTime.UtcNow);
                    ifi.Value("IF_NO", nodeID);
                    ifi.Value("IF_EquipmentName", li.EquipmentName);
                    ifi.Value("IF_OptimumRate_Input", ori);
                    ifi.Value("IF_OptimumRate_Output", oro);
                    ifi.Execute();
                }
                else
                {
                    li.InterfaceID = ifr[0]["IF_ID"];
                    bool est = ifr[0]["IF_Entry_State"];
                    int dori = ifr[0]["IF_OptimumRate_Input"].ToInt(-1);
                    int doro = ifr[0]["IF_OptimumRate_Output"].ToInt(-1);

                    Update ifu = j.Update("Interface");
                    ifu.Set("IF_Entry_State", 1, est == false);
                    ifu.Set("IF_OptimumRate_Input", ori, dori != ori);
                    ifu.Set("IF_OptimumRate_Output", oro, doro != oro);

                    if (!ifu.IsEmpty)
                    {
                        ifu.Where("IF_ID", li.InterfaceID);
                        ifu.Execute();
                    }

                    if (interfacedb.ContainsKey(pair.Key))
                    {
                        string dbid = interfacedb[pair.Key]["MI_ID"];
                        string dbif = interfacedb[pair.Key]["MI_IF"];

                        bool found = false;

                        if (dbif != li.InterfaceID)
                        {
                            foreach (MEInterfaceToDatabase uix in interfaceupdate)
                            {
                                if (uix.Id == dbid)
                                {
                                    uix.InterfaceID = dbif;
                                    uix.UpdateInterfaceID = true;
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            // langsung update aja
                            j.Execute("update MEInterface set MI_IF = {0} where MI_ID = {1}", li.InterfaceID, dbid);
                        }
                    }
                }
            }*/

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

            // ADD
            batch.Begin();

            List<Tuple<string, string>> interfaceTopologyPIUpdate = new List<Tuple<string, string>>();
            List<Tuple<string, string>> interfaceTopologyMIUpdate = new List<Tuple<string, string>>();
            List<string> dacRemove = new List<string>();

            foreach (KeyValuePair<string, MEInterfaceToDatabase> pair in interfaceinsert)
            {
                MEInterfaceToDatabase s = pair.Value;

                Insert insert = j.Insert("MEInterface");
                insert.Value("MI_ID", s.Id);
                insert.Value("MI_IF", s.InterfaceID);
                insert.Value("MI_NO", nodeID);
                insert.Value("MI_Name", s.Name);
                insert.Value("MI_Status", s.Status);
                insert.Value("MI_Protocol", s.Protocol);
                insert.Value("MI_Enable", s.Enable);
                insert.Value("MI_EquipmentName", s.EquipmentName);
                insert.Value("MI_DOT1Q", s.Dot1Q.Nullable(-1));
                insert.Value("MI_Aggregator", s.Aggr.Nullable(-1));
                insert.Value("MI_Description", s.Description);
                insert.Value("MI_MC", s.CircuitID);
                insert.Value("MI_Type", s.InterfaceType);
                insert.Value("MI_MQ_Input", s.IngressID);
                insert.Value("MI_MQ_Output", s.EgressID);
                insert.Value("MI_Rate_Input", s.RateInput.Nullable(-1));
                insert.Value("MI_Rate_Output", s.RateOutput.Nullable(-1));
                insert.Value("MI_MTU", s.RunMTU.Nullable(-1));
                insert.Value("MI_MTU_Config", s.AdmMTU.Nullable(-1));
                insert.Value("MI_Mode", s.Mode.Nullable('-'));
                insert.Value("MI_Encapsulation", s.Mode.Nullable('-'));
                insert.Value("MI_Info", s.Info);
                insert.Value("MI_SI", s.ServiceImmediateID);
                insert.Value("MI_MI", s.ParentID);
                insert.Value("MI_TO_PI", s.TopologyPEInterfaceID);
                insert.Value("MI_TO_MI", s.TopologyMEInterfaceID);
                insert.Value("MI_TO_NI", s.TopologyNBInterfaceID);
                insert.Value("MI_TO_NO", s.TopologyNOID);
                insert.Value("MI_LastDown", s.LastDown);
                insert.Value("MI_Percentage_TrafficInput", s.TrafficInput.Nullable(-1));
                insert.Value("MI_Percentage_TrafficOutput", s.TrafficInput.Nullable(-1));
                insert.Value("MI_Summary_CIRConfigTotalInput", s.CirConfigTotalInput.Nullable(-1));
                insert.Value("MI_Summary_CIRConfigTotalOutput", s.CirConfigTotalOutput.Nullable(-1));
                insert.Value("MI_Summary_CIRTotalInput", s.CirTotalInput.Nullable(-1));
                insert.Value("MI_Summary_CIRTotalOutput", s.CirTotalOutput.Nullable(-1));
                insert.Value("MI_Summary_SubInterfaceCount", s.SubInterfaceCount.Nullable(-1));
                batch.Add(insert);

                interfaceTopologyPIUpdate.Add(new Tuple<string, string>(s.TopologyPEInterfaceID, s.Id));
                interfaceTopologyMIUpdate.Add(new Tuple<string, string>(s.TopologyMEInterfaceID, s.Id));
            }
            result = batch.Commit();
            if (!result) return DatabaseFailure(probe);
            Event(result, EventActions.Add, EventElements.Interface, false);

            // UPDATE       
            batch.Begin();
            foreach (MEInterfaceToDatabase s in interfaceupdate)
            {
                Update update = j.Update("MEInterface");
                update.Set("MI_MI", s.ParentID, s.UpdateParentID);

                if (s.UpdateTopologyPEInterfaceID)
                {
                    update.Set("MI_TO_PI", s.TopologyPEInterfaceID);
                    interfaceTopologyPIUpdate.Add(new Tuple<string, string>(s.TopologyPEInterfaceID, s.Id));
                }
                else if (s.UpdateNeighborCheckPITOMI)
                {
                    interfaceTopologyPIUpdate.Add(new Tuple<string, string>(s.TopologyPEInterfaceID, s.Id));
                }
                if (s.UpdateTopologyMEInterfaceID)
                {
                    update.Set("MI_TO_MI", s.TopologyMEInterfaceID);
                    interfaceTopologyMIUpdate.Add(new Tuple<string, string>(s.TopologyMEInterfaceID, s.Id));
                }
                else if (s.UpdateNeighborCheckMITOMI)
                {
                    interfaceTopologyMIUpdate.Add(new Tuple<string, string>(s.TopologyMEInterfaceID, s.Id));
                }
                update.Set("MI_IF", s.InterfaceID, s.UpdateInterfaceID);
                update.Set("MI_TO_NI", s.TopologyNBInterfaceID, s.UpdateTopologyNBInterfaceID);
                update.Set("MI_Description", s.Description, s.UpdateDescription);
                update.Set("MI_SI", s.ServiceImmediateID, s.UpdateServiceImmediateID);
                update.Set("MI_Status", s.Status, s.UpdateStatus);
                update.Set("MI_Protocol", s.Protocol, s.UpdateProtocol);
                update.Set("MI_Enable", s.Enable, s.UpdateEnable);
                update.Set("MI_DOT1Q", s.Dot1Q.Nullable(-1), s.UpdateDot1Q);
                update.Set("MI_Aggregator", s.Aggr.Nullable(-1), s.UpdateAggr);
                update.Set("MI_MC", s.CircuitID, s.UpdateCircuit);
                update.Set("MI_Type", s.InterfaceType, s.UpdateInterfaceType);
                update.Set("MI_EquipmentName", s.EquipmentName, s.UpdateEquipmentName);
                update.Set("MI_MQ_Input", s.IngressID, s.UpdateIngressID);
                update.Set("MI_MQ_Output", s.EgressID, s.UpdateEgressID);
                update.Set("MI_Rate_Input", s.RateInput.Nullable(-1), s.UpdateRateInput);
                update.Set("MI_Rate_Output", s.RateOutput.Nullable(-1), s.UpdateRateOutput);
                update.Set("MI_MTU", s.RunMTU.Nullable(-1), s.UpdateRunMTU);
                update.Set("MI_MTU_Config", s.AdmMTU.Nullable(-1), s.UpdateAdmMTU);
                update.Set("MI_Mode", s.Mode.Nullable('-'), s.UpdateMode);
                update.Set("MI_Encapsulation", s.Encapsulation.Nullable('-'), s.UpdateEncapsulation);
                update.Set("MI_Info", s.Info, s.UpdateInfo);
                update.Set("MI_LastDown", s.LastDown, s.UpdateLastDown);
                update.Set("MI_TO_NO", s.TopologyNOID, s.UpdateTopologyNOID);
                update.Set("MI_Percentage_TrafficInput", s.TrafficInput.Nullable(-1), s.UpdateTrafficInput);
                update.Set("MI_Percentage_TrafficOutput", s.TrafficOutput.Nullable(-1), s.UpdateTrafficOutput);
                update.Set("MI_Summary_CIRConfigTotalInput", s.CirConfigTotalInput.Nullable(-1), s.UpdateCirConfigTotalInput);
                update.Set("MI_Summary_CIRConfigTotalOutput", s.CirConfigTotalOutput.Nullable(-1), s.UpdateCirConfigTotalOutput);
                update.Set("MI_Summary_CIRTotalInput", s.CirTotalInput.Nullable(-1), s.UpdateCirTotalInput);
                update.Set("MI_Summary_CIRTotalOutput", s.CirTotalOutput.Nullable(-1), s.UpdateCirTotalOutput);
                update.Set("MI_Summary_SubInterfaceCount", s.SubInterfaceCount, s.UpdateSubInterfaceCount);
                update.Where("MI_ID", s.Id);
                batch.Add(update);
            }
            result = batch.Commit();
            if (!result) return DatabaseFailure(probe);
            Event(result, EventActions.Update, EventElements.Interface, false);

            batch.Begin();
            foreach (Tuple<string, string> tuple in interfaceTopologyPIUpdate)
            {
                if (tuple.Item1 != null) batch.Add("update PEInterface set PI_TO_MI = {0} where PI_ID = {1}", tuple.Item2, tuple.Item1);
                else batch.Add("update PEInterface set PI_TO_MI = NULL where PI_TO_MI = {0}", tuple.Item2);
            }
            foreach (Tuple<string, string> tuple in interfaceTopologyMIUpdate)
            {
                if (tuple.Item1 != null) batch.Add("update MEInterface set MI_TO_MI = {0} where MI_ID = {1}", tuple.Item2, tuple.Item1);
                else
                {
                    batch.Add("update MEInterface set MI_TO_MI = NULL where MI_TO_MI = {0}", tuple.Item2);

                    // remove dac from virtualization
                    foreach (KeyValuePair<string, Tuple<string, string, string, string>> entry in NecrowVirtualization.DerivedAreaConnections)
                    {
                        if (entry.Value.Item3 == tuple.Item2 || entry.Value.Item4 == tuple.Item2)
                        {
                            dacRemove.Add(entry.Key);
                            break;
                        }
                    }
                }
            }
            result = batch.Commit();
            if (!result) return DatabaseFailure(probe);
            Event(result, EventActions.Update, EventElements.NBInterface, false);

            // DELETE
            batch.Begin();
            List<string> interfacedelete = new List<string>();
            foreach (KeyValuePair<string, Row2> pair in interfacedb)
            {
                if (!interfacelive.ContainsKey(pair.Key))
                {
                    Event("Interface DELETE: " + pair.Key);
                    string ifid = pair.Value["MI_IF"];
                    string id = pair.Value["MI_ID"].ToString();

                    batch.Add("update PEInterface set PI_TO_MI = NULL where PI_TO_MI = {0}", id);
                    batch.Add("update MEInterface set MI_TO_MI = NULL where MI_TO_MI = {0}", id);
                    batch.Add("update MEInterface set MI_MI = NULL where MI_MI = {0}", id);
                    batch.Add("update MEMac set MA_MI = NULL where MA_MI = {0}", id);

                    if (ifid != null)
                        batch.Add("update Interface set IF_Entry_State = 0, IF_Entry_LastToggle = {0} where IF_ID = {1}", DateTime.UtcNow, ifid);

                    interfacedelete.Add(id);

                    // remove dac from virtualization
                    foreach (KeyValuePair<string, Tuple<string, string, string, string>> entry in NecrowVirtualization.DerivedAreaConnections)
                    {
                        if (entry.Value.Item3 == id || entry.Value.Item4 == id)
                        {
                            dacRemove.Add(entry.Key);
                            break;
                        }
                    }
                }
            }
            result = batch.Commit();
            if (!result) return DatabaseFailure(probe);
            Event(result, EventActions.Delete, EventElements.Interface, false);

            // AREA CONNECTIONS
            if (nodeAreaID != null)
            {
                lock (NecrowVirtualization.DACSync)
                {
                    batch.Begin();

                    List<string> remove = new List<string>();
                    List<Tuple<string, Tuple<string, string, string, string>>> add = new List<Tuple<string, Tuple<string, string, string, string>>>();

                    foreach (KeyValuePair<string, MEInterfaceToDatabase> pair in interfacelive)
                    {
                        MEInterfaceToDatabase li = pair.Value;

                        if (((li.ParentID != null && li.Aggr != -1) || li.ParentID == null) && li.TopologyMEInterfaceID != null)
                        {
                            // fisik (biasa atau anak aggregator)
                            string interfaceID = li.Id;
                            bool existInVir = false;

                            foreach (KeyValuePair<string, Tuple<string, string, string, string>> pair2 in NecrowVirtualization.DerivedAreaConnections)
                            {
                                Tuple<string, string, string, string> dacEntry = pair2.Value;

                                if (dacEntry.Item3 == interfaceID)
                                {
                                    existInVir = true;
                                    if (dacEntry.Item4 != li.TopologyMEInterfaceID)
                                    {
                                        // gak bener ini, update!
                                        remove.Add(pair2.Key);
                                        result = j.Query("select NO_AR from Node, MEInterface where MI_NO = NO_ID and MI_ID = {0} and NO_AR is not null", li.TopologyMEInterfaceID);
                                        if (!result) return DatabaseFailure(probe);
                                        if (result.Count == 1)
                                        {
                                            string topologyAreaID = result[0]["NO_AR"].ToString();
                                            add.Add(new Tuple<string, Tuple<string, string, string, string>>(pair2.Key, new Tuple<string, string, string, string>(nodeAreaID, topologyAreaID, interfaceID, li.TopologyMEInterfaceID)));
                                            Update update = j.Update("DerivedAreaConnection");
                                            update.Where("DAC_ID", pair2.Key);
                                            update.Set("DAC_AR_2", topologyAreaID);
                                            update.Set("DAC_MI_2", li.TopologyMEInterfaceID);
                                            batch.Add(update);
                                        }
                                    }
                                    break;
                                }
                                else if (dacEntry.Item4 == interfaceID)
                                {
                                    existInVir = true;
                                    if (dacEntry.Item3 != li.TopologyMEInterfaceID)
                                    {
                                        // gak bener ini, update!
                                        remove.Add(pair2.Key);
                                        result = j.Query("select NO_AR from Node, MEInterface where MI_NO = NO_ID and MI_ID = {0} and NO_AR is not null", li.TopologyMEInterfaceID);
                                        if (!result) return DatabaseFailure(probe);
                                        if (result.Count == 1)
                                        {
                                            string topologyAreaID = result[0]["NO_AR"].ToString();
                                            add.Add(new Tuple<string, Tuple<string, string, string, string>>(pair2.Key, new Tuple<string, string, string, string>(topologyAreaID, nodeAreaID, li.TopologyMEInterfaceID, interfaceID)));
                                            Update update = j.Update("DerivedAreaConnection");
                                            update.Where("DAC_ID", pair2.Key);
                                            update.Set("DAC_AR_1", topologyAreaID);
                                            update.Set("DAC_MI_1", li.TopologyMEInterfaceID);
                                            batch.Add(update);
                                        }
                                    }
                                    break;
                                }
                            }

                            // gak eksis
                            if (!existInVir)
                            {
                                // add
                                string dacID = Database2.ID();
                                result = j.Query("select NO_AR from Node, MEInterface where MI_NO = NO_ID and MI_ID = {0} and NO_AR is not null", li.TopologyMEInterfaceID);
                                if (!result) return DatabaseFailure(probe);
                                if (result.Count == 1)
                                {
                                    string topologyAreaID = result[0]["NO_AR"].ToString();
                                    add.Add(new Tuple<string, Tuple<string, string, string, string>>(dacID, new Tuple<string, string, string, string>(nodeAreaID, topologyAreaID, interfaceID, li.TopologyMEInterfaceID)));
                                    Insert insert = j.Insert("DerivedAreaConnection");
                                    insert.Value("DAC_ID", dacID);
                                    insert.Value("DAC_AR_1", nodeAreaID);
                                    insert.Value("DAC_AR_2", topologyAreaID);
                                    insert.Value("DAC_MI_1", interfaceID);
                                    insert.Value("DAC_MI_2", li.TopologyMEInterfaceID);
                                    batch.Add(insert);
                                }
                            }
                        }
                    }

                    result = batch.Commit();
                    if (!result) return DatabaseFailure(probe);

                    // modify virtualizations
                    // remove
                    foreach (string dacID in remove)
                    {
                        NecrowVirtualization.DerivedAreaConnections.Remove(dacID);
                    }

                    // add
                    foreach (Tuple<string, Tuple<string, string, string, string>> entry in add)
                    {
                        NecrowVirtualization.DerivedAreaConnections.Add(entry.Item1, entry.Item2);
                    }

                    // remove dac
                    batch.Begin();
                    foreach (string dacID in dacRemove)
                    {
                        NecrowVirtualization.DerivedAreaConnections.Remove(dacID);
                        batch.Add("delete from DerivedAreaConnection where DAC_ID = {0}", dacID);
                    }
                    result = batch.Commit();
                    if (!result) return DatabaseFailure(probe);
                }
            }

            // redone vMEPhysicalInterfaces
            vMEPhysicalInterfaces.Clear();
            foreach (KeyValuePair<string, MEInterfaceToDatabase> pair in interfacelive)
            {
                MEInterfaceToDatabase li = pair.Value;
                vMEPhysicalInterfaces.Add(new Tuple<string, string, string, string, string, string, string>(li.Name, li.Description, li.Id, li.InterfaceType, li.ParentID, li.TopologyMEInterfaceID, li.TopologyPEInterfaceID));
            }
            NecrowVirtualization.MEPhysicalInterfacesSort(vMEPhysicalInterfaces);

            batch.Begin();
            foreach (string id in interfacedelete)
            {
                batch.Add("delete from MEInterface where MI_ID = {0}", id);
            }
            result = batch.Commit();
            if (!result) return DatabaseFailure(probe);
            Event(result, EventActions.Delete, EventElements.Interface, false);

            // RESERVES
            //batch.Begin();
            //foreach (KeyValuePair<string, MEInterfaceToDatabase> pair in interfacelive)
            //{
            //    foreach (KeyValuePair<string, Row> pair2 in reserves)
            //    {
            //        string key2 = pair2.Key;
            //        if (key2.StartsWith(pair.Value.Name + "=") || key2.EndsWith("=" + pair.Value.ServiceVID))
            //        {
            //            batch.Add("delete from Reserve where RE_ID = {0}", pair2.Value["RE_ID"].ToString());
            //        }
            //    }
            //}
            //result = batch.Commit();
            //if (!result) return DatabaseFailure(probe);
            //if (result.AffectedRows > 0) Event(result.AffectedRows + " reserved entr" + (result.AffectedRows > 1 ? "ies have " : "y has ") + " been found");

            #endregion

            #endregion

            #region LATE DELETE

            // CIRCUIT DELETE
            batch.Begin();
            List<string> circuitdelete = new List<string>();
            foreach (KeyValuePair<string, Row2> pair in circuitdb)
            {
                if (!circuitlive.ContainsKey(pair.Key))
                {
                    Event("Circuit DELETE: " + pair.Key);
                    string id = pair.Value["MC_ID"].ToString();
                    batch.Add("update MEInterface set MI_MC = NULL where MI_MC = {0}", id);
                    batch.Add("update MEPeer set MP_TO_MC = NULL where MP_TO_MC = {0}", id);
                    batch.Add("delete from MEMac where MA_MC = {0}", id);
                    circuitdelete.Add(id);
                }
            }
            result = batch.Commit();
            if (!result) return DatabaseFailure(probe);
            batch.Begin();
            foreach (string id in circuitdelete)
            {
                batch.Add("delete from MECircuit where MC_ID = {0}", id);
            }
            result = batch.Commit();
            if (!result) return DatabaseFailure(probe);
            Event(result, EventActions.Delete, EventElements.Circuit, false);

            // SDP DELETE
            batch.Begin();
            foreach (KeyValuePair<string, Row2> pair in sdpdb)
            {
                if (!sdplive.ContainsKey(pair.Key))
                {
                    Event("SDP DELETE: " + pair.Key);
                    batch.Add("delete from MESDP where MS_ID = {0}", pair.Value["MS_ID"].ToString());
                }
            }
            result = batch.Commit();
            if (!result) return DatabaseFailure(probe);
            Event(result, EventActions.Delete, EventElements.SDP, false);

            // ALU CUSTOMER DELETE            
            if (nodeManufacture == alu)
            {
                batch.Begin();
                foreach (KeyValuePair<string, Row2> pair in alucustdb)
                {
                    Row2 row = pair.Value;
                    if (!alucustlive.ContainsKey(pair.Key))
                    {
                        Event("ALU-Customer DELETE: " + pair.Key);
                        batch.Add("update MECircuit set MC_MU = NULL where MC_MU = {0}", row["MU_ID"].ToString()); // in case theres stupid circuit left around
                        batch.Add("delete from MECustomer where MU_ID = {0}", row["MU_ID"].ToString());
                    }
                }
                result = batch.Commit();
                if (!result) return DatabaseFailure(probe);
                Event(result, EventActions.Delete, EventElements.ALUCustomer, false);
            }

            // QOS DELETE
            batch.Begin();
            foreach (KeyValuePair<string, Row2> pair in qosdb)
            {
                Row2 row = pair.Value;
                if (!qoslive.ContainsKey(pair.Key))
                {
                    Event("QOS DELETE: " + pair.Key);
                    string mqid = row["MQ_ID"].ToString();
                    batch.Add("update MEInterface set MI_MQ_Input = NULL where MI_MQ_Input = {0}", mqid);
                    batch.Add("update MEInterface set MI_MQ_Output = NULL where MI_MQ_Output = {0}", mqid);
                    batch.Add("delete from MEQOS where MQ_ID = {0}", mqid);
                }
            }
            result = batch.Commit();
            if (!result) return DatabaseFailure(probe);
            Event(result, EventActions.Delete, EventElements.QOS, false);

            // QOS POLICY DELETE
            batch.Begin();
            foreach (KeyValuePair<string, Row2> pair in policydb)
            {
                Row2 row = pair.Value;
                if (!policylive.ContainsKey(pair.Key))
                {
                    Event("QOS POLICY DELETE: " + pair.Key);
                    string mqid = row["MQP_ID"].ToString();
                    batch.Add("delete from MEQOSPolicy where MQP_ID = {0}", mqid);
                }
            }
            result = batch.Commit();
            if (!result) return DatabaseFailure(probe);
            Event(result, EventActions.Delete, EventElements.QOS, false);


            #endregion

            return probe;
        }
    }
}
