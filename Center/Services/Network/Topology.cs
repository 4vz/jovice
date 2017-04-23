using Aphysoft.Share;
using Center.Providers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;

namespace Center
{
    public enum TopologyDiscoveryTypes
    {
        ServiceID,
        PEInterface,
        MEInterface
    }


    public static class Topology
    {
        public static void Discovery(List<object> objects, TopologyDiscoveryTypes discovery, string id, ref string serviceType, ref string serviceSubType)
        {
            Database jovice = Jovice.Database;

            string piMatch = null;
            string miMatch = null;
            string mcMatch = null;

            if (discovery == TopologyDiscoveryTypes.ServiceID)
            {
                piMatch = jovice.Format("PI_SE = {0}", id);
                miMatch = jovice.Format("MI_SE = {0}", id);
                mcMatch = jovice.Format("MC_SE = {0}", id);
            }
            else if (discovery == TopologyDiscoveryTypes.PEInterface)
            {
                piMatch = jovice.Format("PI_ID = {0}", id);
            }
            else if (discovery == TopologyDiscoveryTypes.MEInterface)
            {
                miMatch = jovice.Format("MI_ID = {0}", id);
            }

            List<object> topology = new List<object>();
            List<object> topologyPurpose = new List<object>();
            List<object> vrf = new List<object>();
            List<object> rateInput = new List<object>();
            List<object> inputLimiter = new List<object>();
            List<object> rateOutput = new List<object>();
            List<object> outputLimiter = new List<object>();
            List<object> ip = new List<object>();
            List<object> vcid = new List<object>();
            List<object> nodeInfo = new List<object>();
            List<object> localacc = new List<object>();
            List<object> routeType = new List<object>();
            List<object> ipd = new List<object>();

            Result r = Result.Null, r2 = Result.Null, r3 = Result.Null;
            string piIndex0MI1 = null;

            if (piMatch != null)
            {
                r = jovice.Query(@"
select 
a.PI_ID, a.PI_Name, a.PI_Description, n.NO_Name, n.NO_Manufacture, n.NO_Model, n.NO_Version, n.NO_TimeStamp, n.NO_AR, a.PI_Status, a.PI_Protocol, rn.PN_Name,
qosa.PQ_Bandwidth as QInput, qosa.PQ_Name as QInputName, qosa.PQ_Package as QInputPackage,
qosb.PQ_Bandwidth as QOutput, qosb.PQ_Name as QOutputName, qosb.PQ_Package as QOutputPackage,
a.PI_Rate_Input, a.PI_Rate_Output,
a.PI_TO_MI, b.PI_Description as PI2_Description, b.PI_Status as PI2_Status, b.PI_Protocol as PI2_Protocol, b.PI_TO_MI as PI2_TO_MI, 
d.NO_Name as NO2_Name, c.MI_Name, d.NO_Manufacture as NO2_Manufacture, d.NO_Model as NO2_Model, d.NO_Version as NO2_Version, d.NO_TimeStamp as NO2_TimeStamp,
e.PP_IP,
NULL as XPI_Name, NULL as XNO_Name,
NULL as MI_MC, NULL as MC_VCID, NULL as MC_Description, NULL as MC_MTU, NULL as MC_Status, NULL as MC_Protocol, NULL as MC_Type, NULL as MI_ID,
nn.NN_Name, ni.NI_Name
from Node n, PEInterface a
left join PERouteName rn on a.PI_PN = rn.PN_ID
left join PEQOS qosa on a.PI_PQ_Input = qosa.PQ_ID
left join PEQOS qosb on a.PI_PQ_Output = qosb.PQ_ID
left join PEInterface b on a.PI_PI = b.PI_ID
left join MEInterface c on b.PI_TO_MI = c.MI_ID
left join Node d on c.MI_NO = d.NO_ID
left join PEInterfaceIP e on a.PI_ID = e.PP_PI and e.PP_Order = 1
left join NeighborInterface ni on b.PI_TO_NI = ni.NI_ID
left join NodeNeighbor nn on ni.NI_NN = nn.NN_ID
where a." + piMatch + @" and a.PI_NO = n.NO_ID
order by n.NO_Active desc, a.PI_Status desc, a.PI_Protocol desc");
            }

            if (r.Count > 0 || serviceType == "VP" || serviceType == "AS" || serviceType == "AB")
            {
                #region VPNIP / ASTINET / ASTINET BEDA BANDWIDTH

                if (r.Count == 0)
                {
                    if (miMatch != null)
                    {
                        r = jovice.Query(@"
select
a.PI_ID, a.PI_Name, a.PI_Description, n.NO_Name, n.NO_Manufacture, n.NO_Model, n.NO_Version, n.NO_TimeStamp, n.NO_AR, a.PI_Status, a.PI_Protocol, rn.PN_Name,
qosa.PQ_Bandwidth as QInput, qosa.PQ_Name as QInputName, qosa.PQ_Package as QInputPackage,
qosb.PQ_Bandwidth as QOutput, qosb.PQ_Name as QOutputName, qosb.PQ_Package as QOutputPackage,
a.PI_Rate_Input, a.PI_Rate_Output,
a.PI_TO_MI, b.PI_Description as PI2_Description, b.PI_Status as PI2_Status, b.PI_Protocol as PI2_Protocol, b.PI_TO_MI as PI2_TO_MI, 
d.NO_Name as NO2_Name, c.MI_Name, d.NO_Manufacture as NO2_Manufacture, d.NO_Model as NO2_Model, d.NO_Version as NO2_Version, d.NO_TimeStamp as NO2_TimeStamp,
e.PP_IP,
xpi.PI_Name as XPI_Name, xpino.NO_Name as XNO_Name, xpino.NO_TimeStamp as XNO_TimeStamp,
c.MI_MC, cmc.MC_VCID, cmc.MC_Description, cmc.MC_MTU, cmc.MC_Status, cmc.MC_Protocol, cmc.MC_Type, c.MI_ID,
NULL as PU_Type,
nn.NN_Name, ni.NI_Name
from Node d, MEInterface c
left join PEInterface a on c.MI_TO_PI = a.PI_ID
left join Node n on a.PI_NO = n.NO_ID
left join PERouteName rn on a.PI_PN = rn.PN_ID
left join PEQOS qosa on a.PI_PQ_Input = qosa.PQ_ID
left join PEQOS qosb on a.PI_PQ_Output = qosb.PQ_ID
left join PEInterface b on a.PI_PI = b.PI_ID
left join PEInterfaceIP e on a.PI_ID = e.PP_PI and e.PP_Order = 1
left join MEInterface mc on c.MI_MI = mc.MI_ID
left join PEInterface xpi on xpi.PI_ID = mc.MI_TO_PI
left join Node xpino on xpino.NO_ID = xpi.PI_NO
left join MECircuit cmc on c.MI_MC = cmc.MC_ID
left join NeighborInterface ni on mc.MI_TO_NI = ni.NI_ID
left join NodeNeighbor nn on ni.NI_NN = nn.NN_ID
where c." + miMatch + @" and c.MI_NO = d.NO_ID
order by d.NO_Active desc, XPI_Name desc, c.MI_Status desc, c.MI_Protocol desc
");
                    }

                    if (r.Count == 0)
                    {
                        if (mcMatch != null)
                        {
                            r = jovice.Query(@"
select
a.PI_ID, a.PI_Name, a.PI_Description, n.NO_Name, n.NO_Manufacture, n.NO_Model, n.NO_Version, n.NO_TimeStamp, n.NO_AR, a.PI_Status, a.PI_Protocol, rn.PN_Name,
qosa.PQ_Bandwidth as QInput, qosa.PQ_Name as QInputName, qosa.PQ_Package as QInputPackage,
qosb.PQ_Bandwidth as QOutput, qosb.PQ_Name as QOutputName, qosb.PQ_Package as QOutputPackage,
a.PI_Rate_Input, a.PI_Rate_Output,
a.PI_TO_MI, b.PI_Description as PI2_Description, b.PI_Status as PI2_Status, b.PI_Protocol as PI2_Protocol, b.PI_TO_MI as PI2_TO_MI, 
d.NO_Name as NO2_Name, c.MI_Name, d.NO_Manufacture as NO2_Manufacture, d.NO_Model as NO2_Model, d.NO_Version as NO2_Version, d.NO_TimeStamp as NO2_TimeStamp,
e.PP_IP,
xpi.PI_Name as XPI_Name, xpino.NO_Name as XNO_Name, xpino.NO_TimeStamp as XNO_TimeStamp,
c.MI_MC, cmc.MC_VCID, cmc.MC_Description, cmc.MC_MTU, cmc.MC_Status, cmc.MC_Protocol, cmc.MC_Type, c.MI_ID,
NULL as PU_Type,
nn.NN_Name, ni.NI_Name
from Node d, MECircuit cmc
left join MEInterface c on c.MI_MC = cmc.MC_ID
left join PEInterface a on c.MI_TO_PI = a.PI_ID
left join Node n on a.PI_NO = n.NO_ID
left join PERouteName rn on a.PI_PN = rn.PN_ID
left join PEQOS qosa on a.PI_PQ_Input = qosa.PQ_ID
left join PEQOS qosb on a.PI_PQ_Output = qosb.PQ_ID
left join PEInterface b on a.PI_PI = b.PI_ID
left join PEInterfaceIP e on a.PI_ID = e.PP_PI and e.PP_Order = 1
left join MEInterface mc on c.MI_MI = mc.MI_ID
left join PEInterface xpi on xpi.PI_ID = mc.MI_TO_PI
left join Node xpino on xpino.NO_ID = xpi.PI_NO
left join NeighborInterface ni on mc.MI_TO_NI = ni.NI_ID
left join NodeNeighbor nn on ni.NI_NN = nn.NN_ID
where cmc." + mcMatch + @" and cmc.MC_NO = d.NO_ID
order by d.NO_Active desc, XPI_Name desc, cmc.MC_Status desc, cmc.MC_Protocol desc
");
                        }

                    }

                }

                int piIndex = 0;
                foreach (Row row in r)
                {
                    if (piIndex == 0 || piIndex == 1)
                    {
                        List<object> topologyCurrent = new List<object>();
                        List<object> nodeInfoTopologyCurrent = new List<object>();

                        string topologyVRF = null;
                        string topologyPurposeCurrent = null;
                        int topologyRateInput = 0;
                        string topologyInputLimiter = null;
                        int topologyRateOutput = 0;
                        string topologyOutputLimiter = null;
                        string topologyCurrentIP = null;
                        string topologyCurrentVCID = null;
                        string topologyLocalAccess = null;

                        List<object> topologyElementCurrent;
                        List<object> nodeInfoNodeCurrent;

                        string piname = null;
                        string piToMi = null;
                        string miMC = null;
                        string miID = null;
                        string remoteMC = null;
                        string remoteMCNOName = null;
                        string mi1Name = null;
                        int qinput = 0;
                        int qoutput = 0;
                        int rinput = 0;
                        int routput = 0;

                        object[] topologyRoute = null;

                        #region PI

                        piname = row["PI_Name"].ToString();

                        if (piname != null)
                        {
                            topologyLocalAccess = row["NO_AR"].ToString();

                            topologyElementCurrent = new List<object>();
                            topologyElementCurrent.Add("PI"); //0

                            topologyElementCurrent.Add(row["NO_Name"].ToString());//1
                            topologyElementCurrent.Add(row["NO_Manufacture"].ToString());//2
                            topologyElementCurrent.Add(row["NO_Model"].ToString());//3
                            topologyElementCurrent.Add(row["NO_Version"].ToString());//4

                            nodeInfoNodeCurrent = new List<object>();
                            nodeInfoNodeCurrent.Add(row["NO_Name"].ToString());
                            nodeInfoNodeCurrent.Add(ProcessDateTime(row["NO_TimeStamp"].ToDateTime()));
                            nodeInfoTopologyCurrent.Add(nodeInfoNodeCurrent.ToArray());

                            //--
                            string pi2desc = row["PI2_Description"].ToString();
                            topologyElementCurrent.Add(pi2desc);//5
                            topologyElementCurrent.Add(row["PI2_Status"].ToBool());//6
                            topologyElementCurrent.Add(row["PI2_Protocol"].ToBool());//7
                            //--

                            topologyElementCurrent.Add(piname);//8
                            string pidesc = row["PI_Description"].ToString();
                            topologyElementCurrent.Add(pidesc);//9
                            topologyElementCurrent.Add(row["PI_Status"].ToBool());//10
                            topologyElementCurrent.Add(row["PI_Protocol"].ToBool());//11    

                            qinput = row["QInput"].ToInt();
                            topologyElementCurrent.Add(qinput);//12
                            qoutput = row["QOutput"].ToInt();
                            topologyElementCurrent.Add(qoutput);//13
                            rinput = row["PI_Rate_Input"].ToInt();
                            topologyElementCurrent.Add(rinput);//14
                            routput = row["PI_Rate_Output"].ToInt();
                            topologyElementCurrent.Add(routput);//15

                            topologyElementCurrent.Add(row["QInputName"].ToString());//16
                            topologyElementCurrent.Add(row["QOutputName"].ToString());//17
                            //--
                            topologyElementCurrent.Add(row["PN_Name"].ToString());//18
                            piToMi = row["PI_TO_MI"].ToString();
                            topologyVRF = row["PN_Name"].ToString();
                            topologyRateInput = qinput > 0 && qinput > rinput ? qinput : rinput > 0 && rinput > qinput ? rinput : 0;
                            topologyRateOutput = qoutput > 0 && qoutput > routput ? qoutput : routput > 0 && routput > qoutput ? routput : 0;
                            if (topologyRateInput > 0) topologyInputLimiter = row["NO_Name"].ToString();
                            if (topologyRateOutput > 0) topologyOutputLimiter = row["NO_Name"].ToString();

                            string piToMi2 = row["PI2_TO_MI"].ToString();

                            if (piToMi2 == null)
                            {
                                topologyElementCurrent.Add(null);//19
                                topologyElementCurrent.Add(row["NI_Name"].ToString());//20
                                topologyElementCurrent.Add(row["NN_Name"].ToString());//21
                                topologyElementCurrent.Add(null);//22
                                topologyElementCurrent.Add(null);//23
                                topologyElementCurrent.Add(null);//24
                            }
                            else
                            {
                                topologyElementCurrent.Add("EX");//19                                
                                topologyElementCurrent.Add(row["MI_Name"].ToString() + "." + piname.Split('.')[1]); //20
                                topologyElementCurrent.Add(row["NO2_Name"].ToString()); //21
                                topologyElementCurrent.Add(row["NO2_Manufacture"].ToString()); //22
                                topologyElementCurrent.Add(row["NO2_Model"].ToString()); //23
                                topologyElementCurrent.Add(row["NO2_Version"].ToString()); //24

                                nodeInfoNodeCurrent = new List<object>();
                                nodeInfoNodeCurrent.Add(row["NO2_Name"].ToString());
                                nodeInfoNodeCurrent.Add(ProcessDateTime(row["NO2_TimeStamp"].ToDateTime()));
                                nodeInfoTopologyCurrent.Add(nodeInfoNodeCurrent.ToArray());
                            }

                            topologyElementCurrent.Add(row["QInputPackage"].ToString());//25
                            topologyElementCurrent.Add(row["QOutputPackage"].ToString());//26

                            string piid = row["PI_ID"].ToString();
                            topologyElementCurrent.Add(piid == null ? null : Base64.Encode(piid)); //27


                            topologyCurrentIP = row["PP_IP"].ToString();
                            topologyCurrent.Add(topologyElementCurrent.ToArray());

                            if (serviceType == "AB" && pidesc != null)
                            {
                                string pidescUpper = pidesc.ToUpper();

                                if (StringHelper.Find(pidescUpper, "(DOM", "DOMESTIK", " DOM", "DOM ")) topologyPurposeCurrent = "DOMESTIK";
                                else topologyPurposeCurrent = "GLOBAL";
                            }

                            // routing
                            r2 = jovice.Query(@"
select PU_Type
from PERouteUse
where PU_PI = {0}
", piid);
                            if (r2.Count > 0)
                            {
                                List<object> routes = new List<object>();

                                foreach (Row row2 in r2)
                                {
                                    string type = row2["PU_Type"].ToString();
                                    routes.Add(type);
                                }


                                topologyRoute = routes.ToArray();
                            }
                        }
                        else
                        {
                            string xpiname = row["XPI_Name"].ToString();

                            if (xpiname != null)
                            {
                                //Service.Event("HERE: " + objects[3]);
                                string miName = row["MI_Name"].ToString();

                                string dotend = "";
                                if (miName != null)
                                {
                                    dotend = "." + miName.Split('.')[1];
                                }

                                topologyElementCurrent = new List<object>();
                                topologyElementCurrent.Add("XPI"); //0
                                topologyElementCurrent.Add(row["XNO_Name"].ToString());//1

                                nodeInfoNodeCurrent = new List<object>();
                                nodeInfoNodeCurrent.Add(row["XNO_Name"].ToString());
                                nodeInfoNodeCurrent.Add(ProcessDateTime(row["XNO_TimeStamp"].ToDateTime()));
                                nodeInfoTopologyCurrent.Add(nodeInfoNodeCurrent.ToArray());

                                topologyElementCurrent.Add(xpiname + dotend);//2

                                topologyCurrent.Add(topologyElementCurrent.ToArray());

                                piToMi = row["MI_ID"].ToString();
                            }
                            else
                            {
                                #region XMI2

                                topologyElementCurrent = new List<object>();
                                topologyElementCurrent.Add("XMI2"); //0

                                topologyCurrent.Add(topologyElementCurrent.ToArray());

                                #endregion

                                #region MC

                                topologyElementCurrent = new List<object>();
                                topologyElementCurrent.Add("MC"); //0
                                topologyElementCurrent.Add(row["NO2_Name"].ToString());//1
                                topologyElementCurrent.Add(row["NO2_Manufacture"].ToString());//2
                                topologyElementCurrent.Add(row["NO2_Model"].ToString());//3
                                topologyElementCurrent.Add(row["NO2_Version"].ToString());//4

                                nodeInfoNodeCurrent = new List<object>();
                                nodeInfoNodeCurrent.Add(row["NO2_Name"].ToString());
                                nodeInfoNodeCurrent.Add(ProcessDateTime(row["NO2_TimeStamp"].ToDateTime()));
                                nodeInfoTopologyCurrent.Add(nodeInfoNodeCurrent.ToArray());

                                //--
                                topologyElementCurrent.Add(null);//5
                                topologyElementCurrent.Add(false);//6
                                topologyElementCurrent.Add(null);//7
                                //--
                                topologyElementCurrent.Add(null);//8
                                topologyElementCurrent.Add(false);//9
                                topologyElementCurrent.Add(null);//10
                                //--
                                topologyCurrentVCID = row["MC_VCID"].ToString();
                                topologyElementCurrent.Add(topologyCurrentVCID);//11
                                topologyElementCurrent.Add(row["MC_Description"].ToString());//12
                                topologyElementCurrent.Add(row["MC_MTU"].ToIntShort());//13
                                topologyElementCurrent.Add(row["MC_Status"].ToBool());//14
                                topologyElementCurrent.Add(row["MC_Protocol"].ToBool());//15
                                topologyElementCurrent.Add(row["MC_Type"].ToString());//16

                                //--
                                topologyElementCurrent.Add(null);//17

                                topologyCurrent.Add(topologyElementCurrent.ToArray());

                                #endregion

                                remoteMC = row["MI_MC"].ToString();
                            }
                        }

                        #endregion

                        #region MI2

                        if (piToMi != null)
                        {
                            r2 = jovice.Query(@"
select 
a.MI_Name, a.MI_Description, NO_Name, NO_Manufacture, NO_Model, NO_Version, NO_TimeStamp, NO_AR, a.MI_Status, a.MI_Protocol,
qosa.MQ_Bandwidth as QInput, qosa.MQ_Name as QInputName,
qosb.MQ_Bandwidth as QOutput, qosb.MQ_Name as QOutputName,
a.MI_Rate_Input, a.MI_Rate_Output,
b.MI_Description as MI2_Description, b.MI_Status as MI2_Status, b.MI_Protocol as MI2_Protocol,
MC_ID, MC_VCID, MC_Description, MC_MTU, MC_Status, MC_Protocol, MC_Type
from Node, MEInterface a
left join MEQOS qosa on a.MI_MQ_Input = qosa.MQ_ID
left join MEQOS qosb on a.MI_MQ_Output = qosb.MQ_ID
left join MEInterface b on a.MI_MI = b.MI_ID
left join MECircuit on a.MI_MC = MC_ID
where a.MI_ID = {0} and a.MI_NO = NO_ID
", piToMi);
                            if (r2.Count > 0)
                            {
                                Row row2 = r2[0];
                                topologyLocalAccess = row2["NO_AR"].ToString();
                            }
                            if (r2.Count == 1)
                            {
                                Row row2 = r2[0];

                                topologyLocalAccess = null;

                                topologyElementCurrent = new List<object>();
                                topologyElementCurrent.Add("MI2"); //0
                                topologyElementCurrent.Add(row2["NO_Name"].ToString());//1
                                topologyElementCurrent.Add(row2["NO_Manufacture"].ToString());//2
                                topologyElementCurrent.Add(row2["NO_Model"].ToString());//3
                                topologyElementCurrent.Add(row2["NO_Version"].ToString());//4

                                nodeInfoNodeCurrent = new List<object>();
                                nodeInfoNodeCurrent.Add(row2["NO_Name"].ToString());
                                nodeInfoNodeCurrent.Add(ProcessDateTime(row2["NO_TimeStamp"].ToDateTime()));
                                nodeInfoTopologyCurrent.Add(nodeInfoNodeCurrent.ToArray());

                                //--
                                topologyElementCurrent.Add(row2["MI_Name"].ToString());//5
                                topologyElementCurrent.Add(row2["MI_Description"].ToString());//6
                                topologyElementCurrent.Add(row2["MI_Status"].ToBool());//7
                                topologyElementCurrent.Add(row2["MI_Protocol"].ToBool());//8

                                qinput = row2["QInput"].ToInt();
                                topologyElementCurrent.Add(qinput);//9
                                qoutput = row2["QOutput"].ToInt();
                                topologyElementCurrent.Add(qoutput);//10
                                rinput = row2["MI_Rate_Input"].ToInt();
                                topologyElementCurrent.Add(rinput);//11
                                routput = row2["MI_Rate_Output"].ToInt();
                                topologyElementCurrent.Add(routput);//12

                                int tOutput = qinput > 0 && qinput > rinput ? qinput : rinput > 0 && rinput > qinput ? rinput : 0;
                                int tInput = qoutput > 0 && qoutput > routput ? qoutput : routput > 0 && routput > qoutput ? routput : 0;

                                if (tInput > 0)
                                {
                                    topologyRateInput = tInput < topologyRateInput ? tInput : topologyRateInput;
                                    topologyInputLimiter = row2["NO_Name"].ToString();
                                }
                                if (tOutput > 0)
                                {
                                    topologyRateOutput = tOutput < topologyRateOutput ? tOutput : topologyRateOutput;
                                    topologyOutputLimiter = row2["NO_Name"].ToString();
                                }

                                topologyElementCurrent.Add(row2["QInputName"].ToString());//13
                                topologyElementCurrent.Add(row2["QOutputName"].ToString());//14
                                //--
                                topologyElementCurrent.Add(row2["MI2_Description"].ToString());//15
                                topologyElementCurrent.Add(row2["MI2_Status"].ToBool());//16
                                topologyElementCurrent.Add(row2["MI2_Protocol"].ToBool());//17
                                //--
                                topologyCurrentVCID = row2["MC_VCID"].ToString();
                                topologyElementCurrent.Add(topologyCurrentVCID);//18
                                topologyElementCurrent.Add(row2["MC_Description"].ToString());//19
                                topologyElementCurrent.Add(row2["MC_MTU"].ToIntShort());//20
                                topologyElementCurrent.Add(row2["MC_Status"].ToBool());//21
                                topologyElementCurrent.Add(row2["MC_Protocol"].ToBool());//22
                                topologyElementCurrent.Add(row2["MC_Type"].ToString());//23

                                //--
                                topologyElementCurrent.Add(null);//24

                                topologyCurrent.Add(topologyElementCurrent.ToArray());

                                miMC = row2["MC_ID"].ToString();
                                miID = piToMi;
                            }

                        }
                        else
                        {
                        }

                        #endregion

                        #region MC

                        if (miMC != null)
                        {
                            r2 = jovice.Query(@"
select a.MP_TO_MC, a.MP_VCID, a.MP_Protocol, a.MP_Type,
NO_Name, NO_Manufacture, NO_Model, NO_Version, NO_TimeStamp, NO_AR, MC_ID, MC_VCID, MC_Description, MC_MTU, MC_Status, MC_Protocol, MC_Type,
b.MP_VCID as MP2_VCID, b.MP_Protocol as MP2_Protocol, b.MP_Type as MP2_Type, a.MP_MS
from MEPeer a
left join MECircuit on a.MP_TO_MC = MC_ID
left join Node on MC_NO = NO_ID
left join MEPeer b on MC_ID = b.MP_MC and b.MP_TO_MC = a.MP_MC
where a.MP_MC = {0} order by a.MP_TO_MC desc
", miMC);
                            if (r2.Count > 0)
                            {
                                Row row2 = r2[0];

                                string mpToMC = row2["MP_TO_MC"].ToString();

                                if (mpToMC != null)
                                {
                                    topologyElementCurrent = new List<object>();
                                    topologyElementCurrent.Add("MC"); //0
                                    remoteMCNOName = row2["NO_Name"].ToString();
                                    topologyElementCurrent.Add(remoteMCNOName);//1
                                    topologyElementCurrent.Add(row2["NO_Manufacture"].ToString());//2
                                    topologyElementCurrent.Add(row2["NO_Model"].ToString());//3
                                    topologyElementCurrent.Add(row2["NO_Version"].ToString());//4

                                    nodeInfoNodeCurrent = new List<object>();
                                    nodeInfoNodeCurrent.Add(row2["NO_Name"].ToString());
                                    nodeInfoNodeCurrent.Add(ProcessDateTime(row2["NO_TimeStamp"].ToDateTime()));
                                    nodeInfoTopologyCurrent.Add(nodeInfoNodeCurrent.ToArray());

                                    //--
                                    topologyElementCurrent.Add(row2["MP_VCID"].ToString());//5
                                    topologyElementCurrent.Add(row2["MP_Protocol"].ToBool());//6
                                    topologyElementCurrent.Add(row2["MP_Type"].ToString());//7
                                    //--
                                    topologyElementCurrent.Add(row2["MP2_VCID"].ToString());//8
                                    topologyElementCurrent.Add(row2["MP2_Protocol"].ToBool());//9
                                    topologyElementCurrent.Add(row2["MP2_Type"].ToString());//10
                                    //--
                                    topologyElementCurrent.Add(row2["MC_VCID"].ToString());//11
                                    topologyElementCurrent.Add(row2["MC_Description"].ToString());//12
                                    topologyElementCurrent.Add(row2["MC_MTU"].ToIntShort());//13
                                    topologyElementCurrent.Add(row2["MC_Status"].ToBool());//14
                                    topologyElementCurrent.Add(row2["MC_Protocol"].ToBool());//15
                                    topologyElementCurrent.Add(row2["MC_Type"].ToString());//16

                                    //--
                                    topologyElementCurrent.Add(null);//17

                                    //--
                                    topologyElementCurrent.Add(r2.Count); //18

                                    topologyCurrent.Add(topologyElementCurrent.ToArray());

                                    remoteMC = mpToMC;
                                    topologyLocalAccess = row2["NO_AR"].ToString();
                                }
                                else
                                {
                                    topologyLocalAccess = null;

                                    r2 = jovice.Query("select NO_Name, NO_Manufacture, NO_Model, NO_Version, NO_TimeStamp from Node, MESDP where NO_IP = MS_IP and MS_ID = {0}", row2["MP_MS"].ToString());

                                    if (r2.Count == 1)
                                    {
                                        #region XMC

                                        row2 = r2[0];

                                        topologyElementCurrent = new List<object>();
                                        topologyElementCurrent.Add("XMC"); //0
                                        topologyElementCurrent.Add(row2["NO_Name"].ToString());
                                        topologyElementCurrent.Add(row2["NO_Manufacture"].ToString());
                                        topologyElementCurrent.Add(row2["NO_Model"].ToString());
                                        topologyElementCurrent.Add(row2["NO_Version"].ToString());

                                        nodeInfoNodeCurrent = new List<object>();
                                        nodeInfoNodeCurrent.Add(row2["NO_Name"].ToString());
                                        nodeInfoNodeCurrent.Add(ProcessDateTime(row2["NO_TimeStamp"].ToDateTime()));
                                        nodeInfoTopologyCurrent.Add(nodeInfoNodeCurrent.ToArray());

                                        topologyCurrent.Add(topologyElementCurrent.ToArray());

                                        #endregion
                                    }
                                }
                            }
                            else
                            {
                                r2 = jovice.Query(@"
select a.MI_Name, a.MI_Description, a.MI_Status, a.MI_Protocol, n.NO_AR, n.NO_Name,
qosa.MQ_Bandwidth as QInput, qosa.MQ_Name as QInputName,
qosb.MQ_Bandwidth as QOutput, qosb.MQ_Name as QOutputName,
a.MI_Rate_Input, a.MI_Rate_Output,
b.MI_ID as MI2_ID, b.MI_Description as MI2_Description, b.MI_Status as MI2_Status, b.MI_Protocol as MI2_Protocol,
c.NI_Name, d.NN_Name
from MEInterface a
left join Node n on n.NO_ID = a.MI_NO
left join MEQOS qosa on a.MI_MQ_Input = qosa.MQ_ID
left join MEQOS qosb on a.MI_MQ_Output = qosb.MQ_ID
left join MEInterface b on a.MI_MI = b.MI_ID
left join NeighborInterface c on b.MI_TO_NI = c.NI_ID
left join NodeNeighbor d on c.NI_NN = d.NN_ID
where a.MI_MC = {0} and a.MI_ID <> {1}
", miMC, miID);

                                if (r2.Count > 0)
                                {
                                    remoteMC = "STRAIGHTFROMMI2";
                                    topologyLocalAccess = r2[0]["NO_AR"].ToString();
                                    //Service.Event("STRAIGHT: " + objects[3]);
                                }

                            }
                        }

                        #endregion

                        #region MI1

                        if (remoteMC != null)
                        {

                            if (remoteMC != "STRAIGHTFROMMI2")
                            {
                                r2 = jovice.Query(@"
select a.MI_Name, a.MI_Description, a.MI_Status, a.MI_Protocol, n.NO_Name,
qosa.MQ_Bandwidth as QInput, qosa.MQ_Name as QInputName,
qosb.MQ_Bandwidth as QOutput, qosb.MQ_Name as QOutputName,
a.MI_Rate_Input, a.MI_Rate_Output,
b.MI_ID as MI2_ID, b.MI_Description as MI2_Description, b.MI_Status as MI2_Status, b.MI_Protocol as MI2_Protocol,
c.NI_Name, d.NN_Name
from Node n, MEInterface a
left join MEQOS qosa on a.MI_MQ_Input = qosa.MQ_ID
left join MEQOS qosb on a.MI_MQ_Output = qosb.MQ_ID
left join MEInterface b on a.MI_MI = b.MI_ID
left join NeighborInterface c on b.MI_TO_NI = c.NI_ID
left join NodeNeighbor d on c.NI_NN = d.NN_ID
where a.MI_MC = {0} and a.MI_NO = n.NO_ID
", remoteMC);
                            }

                            if (r2.Count > 0)
                            {
                                Row row2 = r2[0];
                                topologyElementCurrent = new List<object>();
                                topologyElementCurrent.Add("MI1");//0
                                //--
                                mi1Name = row2["MI_Name"].ToString();
                                topologyElementCurrent.Add(mi1Name);//1
                                topologyElementCurrent.Add(row2["MI_Description"].ToString());//2
                                topologyElementCurrent.Add(row2["MI_Status"].ToBool());//3
                                topologyElementCurrent.Add(row2["MI_Protocol"].ToBool());//4

                                qinput = row2["QInput"].ToInt();
                                topologyElementCurrent.Add(qinput);//5
                                qoutput = row2["QOutput"].ToInt();
                                topologyElementCurrent.Add(qoutput);//6
                                rinput = row2["MI_Rate_Input"].ToInt();
                                topologyElementCurrent.Add(rinput);//7
                                routput = row2["MI_Rate_Output"].ToInt();
                                topologyElementCurrent.Add(routput);//8

                                int tInput = qinput > 0 && qinput > rinput ? qinput : rinput > 0 && rinput > qinput ? rinput : 0;
                                int tOutput = qoutput > 0 && qoutput > routput ? qoutput : routput > 0 && routput > qoutput ? routput : 0;

                                if (tInput > 0)
                                {
                                    topologyRateInput = tInput < topologyRateInput ? tInput : topologyRateInput;
                                    topologyInputLimiter = row2["NO_Name"].ToString();
                                }
                                if (tOutput > 0)
                                {
                                    topologyRateOutput = tOutput < topologyRateOutput ? tOutput : topologyRateOutput;
                                    topologyOutputLimiter = row2["NO_Name"].ToString();
                                }

                                topologyElementCurrent.Add(row2["QInputName"].ToString());//9
                                topologyElementCurrent.Add(row2["QOutputName"].ToString());//10

                                if (mi1Name.StartsWith("Ag"))
                                {
                                    r3 = jovice.Query(@"
select m.MI_Name, m.MI_Description, m.MI_Status, m.MI_Protocol,
a.NI_Name, b.NN_Name
from MEInterface m
left join NeighborInterface a on m.MI_TO_NI = a.NI_ID
left join NodeNeighbor b on a.NI_NN = b.NN_ID
where m.MI_MI = {0} and m.MI_Aggregator is not null", row2["MI2_ID"].ToString());

                                    List<object> io11 = new List<object>();
                                    List<object> io12 = new List<object>();
                                    List<object> io14 = new List<object>();
                                    List<object> io15 = new List<object>();

                                    List<object> io13 = new List<object>();
                                    List<object> io17 = new List<object>();

                                    foreach (Row row3 in r3)
                                    {
                                        io11.Add(row3["MI_Name"].ToString());
                                        io12.Add(row3["MI_Description"].ToString());
                                        io14.Add(row3["MI_Status"].ToBool());
                                        io15.Add(row3["MI_Protocol"].ToBool());

                                        io17.Add(row3["NI_Name"].ToString());
                                        io13.Add(row3["NN_Name"].ToString());
                                    }

                                    topologyElementCurrent.Add(io11.ToArray());//11
                                    topologyElementCurrent.Add(io12.ToArray());//12

                                    string nnName = row2["NN_Name"].ToString();
                                    string niName = row2["NI_Name"].ToString();

                                    if (nnName == null)
                                    {
                                        int i = 0;
                                        foreach (string innname in io13)
                                        {
                                            if (nnName == null)
                                            {
                                                nnName = innname;
                                                niName = (string)io17[i];
                                            }
                                            else if (niName != (string)io17[i])
                                            {
                                                niName = niName + "," + (string)io17[i];
                                            }
                                            else if (nnName != innname)
                                            {
                                                nnName = null;
                                                niName = null;
                                                break;
                                            }

                                            i++;
                                        }
                                    }

                                    topologyElementCurrent.Add(nnName); //13
                                    topologyElementCurrent.Add(io14.ToArray());//14
                                    topologyElementCurrent.Add(io15.ToArray());//15
                                    topologyElementCurrent.Add(null);
                                    topologyElementCurrent.Add(niName); // 17
                                }
                                else
                                {
                                    string miDesc = row2["MI2_Description"].ToString();
                                    topologyElementCurrent.Add(null);//11
                                    topologyElementCurrent.Add(miDesc);//12
                                    topologyElementCurrent.Add(row2["NN_Name"].ToString());// );//13
                                    topologyElementCurrent.Add(row2["MI2_Status"].ToBool());//14
                                    topologyElementCurrent.Add(row2["MI2_Protocol"].ToBool());//15
                                    topologyElementCurrent.Add(null); //16
                                    topologyElementCurrent.Add(row2["NI_Name"].ToString()); //17
                                }



                                topologyCurrent.Add(topologyElementCurrent.ToArray());
                            }
                        }
                        else
                        {


                        }

                        #endregion

                        if (serviceType == "VP" || serviceType == "AS")
                        {
                            string nomiEnd1 = null;
                            if (remoteMCNOName != null && mi1Name != null)
                                nomiEnd1 = remoteMCNOName + ":" + mi1Name;
                            if (piIndex == 0)
                            {
                                piIndex0MI1 = nomiEnd1;
                                topologyPurposeCurrent = "MAIN";
                            }
                            else
                            {
                                if (piIndex0MI1 != nomiEnd1 || piIndex0MI1 == null || nomiEnd1 == null) break;
                                else topologyPurposeCurrent = "BACKUP";
                            }
                        }

                        topology.Add(topologyCurrent.ToArray());
                        vrf.Add(topologyVRF);
                        rateInput.Add(topologyRateInput);
                        inputLimiter.Add(topologyInputLimiter);
                        rateOutput.Add(topologyRateOutput);
                        outputLimiter.Add(topologyOutputLimiter);
                        topologyPurpose.Add(topologyPurposeCurrent);

                        ip.Add(topologyCurrentIP);

                        if (topologyCurrentIP != null)
                        {

                            string iplocal = topologyCurrentIP.Split(new char[] { '/' })[0];
                            IPNetwork ipnetwork = IPNetwork.Parse(topologyCurrentIP);

                            if (ipnetwork.Usable >= 2) // using usable address
                            {
                                IPAddressCollection addresses = IPNetwork.ListIPAddress(ipnetwork);

                                bool inside = false;
                                foreach (IPAddress address in addresses)
                                {
                                    string addressx = address.ToString();
                                    if (addressx == ipnetwork.FirstUsable.ToString()) inside = true;

                                    if (inside)
                                    {
                                        if (addressx != iplocal)
                                        {
                                            ipd.Add(address.ToString());
                                            break;
                                        }
                                    }
                                }
                            }
                            else // 31 or // 32
                            {
                                if (ipnetwork.Cidr == 32)
                                {
                                    ipd.Add(iplocal);
                                }
                                else
                                {
                                    IPAddressCollection addresses = IPNetwork.ListIPAddress(ipnetwork);

                                    foreach (IPAddress address in addresses)
                                    {
                                        string addressx = address.ToString();
                                        if (addressx != iplocal)
                                        {
                                            ipd.Add(address.ToString());
                                            break;
                                        }
                                    }
                                }
                            }
                        }



                        vcid.Add(topologyCurrentVCID);
                        nodeInfo.Add(nodeInfoTopologyCurrent.ToArray());

                        if (topologyLocalAccess != null)
                        {
                            r = jovice.Query(@"
select AR_Name, AW_Name, AG_Name
from Area
left join AreaWitel on AR_AW = AW_ID
left join AreaGroup on AW_AG = AG_ID
where AR_ID = {0}
", topologyLocalAccess);
                            Row rowla = r[0];

                            topologyLocalAccess = "STO " + rowla["AR_Name"].ToString().Trim() + ", WITEL " + rowla["AW_Name"].ToString().Trim() + ", " + rowla["AG_Name"].ToString().Trim();
                        }

                        localacc.Add(topologyLocalAccess);

                        if (topologyVRF != null && serviceType == null)
                        {
                            serviceType = "VP";
                        }

                        routeType.Add(topologyRoute);
                    }
                    else break;

                    piIndex++;
                }

                #endregion
            }
            else
            {
                #region METRO

                if (miMatch != null)
                {
                    r = jovice.Query(@"
select
i.MI_ID, i.MI_Name, i.MI_Description, i.MI_Status, i.MI_Protocol,
qosa.MQ_Bandwidth as QInput, qosa.MQ_Name as QInputName,
qosb.MQ_Bandwidth as QOutput, qosb.MQ_Name as QOutputName,
i.MI_Rate_Input, i.MI_Rate_Output,
i2.MI_ID as MI2_ID, i2.MI_Description as MI2_Description, i2.MI_Status as MI2_Status, i2.MI_Protocol as MI2_Protocol,
n.NO_Name, n.NO_Manufacture, n.NO_Model, n.NO_Version, n.NO_TimeStamp,
c.MC_ID, c.MC_VCID, c.MC_Description, c.MC_MTU, c.MC_Status, c.MC_Protocol, c.MC_Type,
aw.AW_AG, se.SE_SID,
ni.NI_Name, nn.NN_Name
from 
Node n, Area ar, AreaWitel aw, MEInterface i
left join Service se on se.SE_ID = i.MI_SE
left join MECircuit c on c.MC_ID = i.MI_MC
left join MEQOS qosa on i.MI_MQ_Input = qosa.MQ_ID
left join MEQOS qosb on i.MI_MQ_Output = qosb.MQ_ID
left join MEInterface i2 on i.MI_MI = i2.MI_ID
left join MEInterface ci on ci.MI_MC = c.MC_ID
left join MEPeer cp on cp.MP_MC = c.MC_ID
left join NeighborInterface ni on i2.MI_TO_NI = ni.NI_ID
left join NodeNeighbor nn on ni.NI_NN = nn.NN_ID
where
i." + miMatch + @" and i.MI_TO_PI is null and i.MI_NO = n.NO_ID and
n.NO_AR = ar.AR_ID and ar.AR_AW = aw.AW_ID
order by n.NO_Active desc, c.MC_ID desc
");
                }

                if (r.Count == 0)
                {

                    if (mcMatch != null)
                    {
                        r = jovice.Query(@"
select
i.MI_ID, i.MI_Name, i.MI_Description, i.MI_Status, i.MI_Protocol,
qosa.MQ_Bandwidth as QInput, qosa.MQ_Name as QInputName,
qosb.MQ_Bandwidth as QOutput, qosb.MQ_Name as QOutputName,
i.MI_Rate_Input, i.MI_Rate_Output,
i2.MI_ID as MI2_ID, i2.MI_Description as MI2_Description, i2.MI_Status as MI2_Status, i2.MI_Protocol as MI2_Protocol,
n.NO_Name, n.NO_Manufacture, n.NO_Model, n.NO_Version, n.NO_TimeStamp,
c.MC_ID, c.MC_VCID, c.MC_Description, c.MC_MTU, c.MC_Status, c.MC_Protocol, c.MC_Type,
aw.AW_AG, se.SE_SID,
ni.NI_Name, nn.NN_Name
from 
Node n, Area ar, AreaWitel aw, MECircuit c
left join Service se on se.SE_ID = c.MC_SE
left join MEInterface i on i.MI_MC = c.MC_ID
left join MEQOS qosa on i.MI_MQ_Input = qosa.MQ_ID
left join MEQOS qosb on i.MI_MQ_Output = qosb.MQ_ID
left join MEInterface i2 on i.MI_MI = i2.MI_ID
left join NeighborInterface ni on i2.MI_TO_NI = ni.NI_ID
left join NodeNeighbor nn on ni.NI_NN = nn.NN_ID
where 
c." + mcMatch + @" and c.MC_NO = n.NO_ID and
n.NO_AR = ar.AR_ID and ar.AR_AW = aw.AW_ID
");
                    }

                }

                if (r.Count > 0)
                {
                    List<object> topologyCurrent = new List<object>();
                    List<object> nodeInfoTopologyCurrent = new List<object>();

                    string topologyCurrentVCID = null;
                    int topologyRateInput = 0;
                    string topologyInputLimiter = null;
                    int topologyRateOutput = 0;
                    string topologyOutputLimiter = null;
                    string topologyLocalAccess = null;

                    List<object> topologyElementCurrent;
                    List<object> nodeInfoNodeCurrent;
                    string mcID;
                    string remoteMC;
                    string localSID;
                    int qinput = 0;
                    int qoutput = 0;
                    int rinput = 0;
                    int routput = 0;
                    Row row = r[0];
                    mcID = row["MC_ID"].ToString();
                    localSID = row["SE_SID"].ToString();

                    Column c;
                    c = jovice.Scalar("select count(MI_ID) from MEInterface where MI_MC = {0}", mcID);
                    int cint = c.ToInt();
                    c = jovice.Scalar("select count(MP_ID) from MEPeer where MP_MC = {0}", mcID);
                    int cpeer = c.ToInt();

                    if (serviceType == null) serviceType = "ME";

                    if (mcID != null)
                    {
                        if ((cint == 1 && cpeer == 1) || (cint == 2 && cpeer == 0))
                        {
                            if (serviceType == "ME") serviceSubType = "PP";
                        }

                        string milName = row["MI_Name"].ToString();

                        if (cint > 0)
                        {
                            #region MIL

                            topologyElementCurrent = new List<object>();
                            topologyElementCurrent.Add("MIL");//0
                            //--
                            string milDesc = row["MI2_Description"].ToString();

                            if (milName.StartsWith("Ag"))
                            {
                                r2 = jovice.Query(@"
select m.MI_Name, m.MI_Description, m.MI_Status, m.MI_Protocol,
a.NI_Name, b.NN_Name 
from MEInterface m
left join NeighborInterface a on m.MI_TO_NI = a.NI_ID
left join NodeNeighbor b on a.NI_NN = b.NN_ID
where 
m.MI_MI = {0} and m.MI_Aggregator is not null", row["MI2_ID"].ToString());

                                List<object> iolName = new List<object>();
                                List<object> iolDesc = new List<object>();
                                List<object> iolStat = new List<object>();
                                List<object> iolProt = new List<object>();

                                List<object> iolNNName = new List<object>();
                                List<object> iolNIName = new List<object>();

                                foreach (Row row2 in r2)
                                {
                                    iolName.Add(row2["MI_Name"].ToString());
                                    iolDesc.Add(row2["MI_Description"].ToString());
                                    iolStat.Add(row2["MI_Status"].ToBool());
                                    iolProt.Add(row2["MI_Protocol"].ToBool());

                                    iolNNName.Add(row2["NN_Name"].ToString());
                                    iolNIName.Add(row2["NI_Name"].ToString());
                                }

                                string nnName = row["NN_Name"].ToString();
                                string niName = row["NI_Name"].ToString();
                                if (nnName == null)
                                {
                                    int i = 0;
                                    foreach (string innname in iolNNName)
                                    {
                                        if (nnName == null)
                                        {
                                            nnName = innname;
                                            niName = (string)iolNIName[i];
                                        }
                                        else if (nnName != innname || niName != (string)iolNIName[i])
                                        {
                                            nnName = null;
                                            niName = null;
                                            break;
                                        }

                                        i++;
                                    }
                                }

                                topologyElementCurrent.Add(nnName); //1
                                topologyElementCurrent.Add(niName); //2
                            }
                            else
                            {
                                topologyElementCurrent.Add(row["NN_Name"].ToString());
                                topologyElementCurrent.Add(row["NI_Name"].ToString());
                            }

                            topologyCurrent.Add(topologyElementCurrent.ToArray());
                            #endregion
                        }

                        #region MI2

                        topologyElementCurrent = new List<object>();
                        topologyElementCurrent.Add("MI2"); //0
                        topologyElementCurrent.Add(row["NO_Name"].ToString());//1
                        topologyElementCurrent.Add(row["NO_Manufacture"].ToString());//2
                        topologyElementCurrent.Add(row["NO_Model"].ToString());//3
                        topologyElementCurrent.Add(row["NO_Version"].ToString());//4

                        nodeInfoNodeCurrent = new List<object>();
                        nodeInfoNodeCurrent.Add(row["NO_Name"].ToString());
                        nodeInfoNodeCurrent.Add(ProcessDateTime(row["NO_TimeStamp"].ToDateTime()));
                        nodeInfoTopologyCurrent.Add(nodeInfoNodeCurrent.ToArray());

                        //--
                        topologyElementCurrent.Add(row["MI_Name"].ToString());//5
                        topologyElementCurrent.Add(row["MI_Description"].ToString());//6
                        topologyElementCurrent.Add(row["MI_Status"].ToBool());//7
                        topologyElementCurrent.Add(row["MI_Protocol"].ToBool());//8

                        qinput = row["QInput"].ToInt();
                        topologyElementCurrent.Add(qinput); //9
                        qoutput = row["QOutput"].ToInt();
                        topologyElementCurrent.Add(qoutput); //10
                        rinput = row["MI_Rate_Input"].ToInt();
                        topologyElementCurrent.Add(rinput); //11
                        routput = row["MI_Rate_Output"].ToInt();
                        topologyElementCurrent.Add(routput); //12

                        topologyElementCurrent.Add(row["QInputName"].ToString()); //13
                        topologyElementCurrent.Add(row["QOutputName"].ToString()); //14
                        //--
                        topologyElementCurrent.Add(row["MI2_Description"].ToString()); //15
                        topologyElementCurrent.Add(row["MI2_Status"].ToBool()); //16
                        topologyElementCurrent.Add(row["MI2_Protocol"].ToBool()); //17
                        //--
                        topologyCurrentVCID = row["MC_VCID"].ToString();
                        topologyElementCurrent.Add(topologyCurrentVCID); //18
                        topologyElementCurrent.Add(row["MC_Description"].ToString()); //19
                        topologyElementCurrent.Add(row["MC_MTU"].ToIntShort()); //20
                        topologyElementCurrent.Add(row["MC_Status"].ToBool()); //21
                        topologyElementCurrent.Add(row["MC_Protocol"].ToBool()); //22
                        topologyElementCurrent.Add(row["MC_Type"].ToString()); //23

                        topologyRateInput = qinput > 0 && qinput > rinput ? qinput : rinput > 0 && rinput > qinput ? rinput : 0;
                        topologyRateOutput = qoutput > 0 && qoutput > routput ? qoutput : routput > 0 && routput > qoutput ? routput : 0;

                        if (topologyRateInput > 0) topologyInputLimiter = row["NO_Name"].ToString();
                        if (topologyRateOutput > 0) topologyOutputLimiter = row["NO_Name"].ToString();

                        //--
                        if (cint == 2 && cpeer == 0)
                            topologyElementCurrent.Add(null);//24
                        else
                            topologyElementCurrent.Add(cint);//24

                        topologyCurrent.Add(topologyElementCurrent.ToArray());

                        #endregion

                        if (cpeer == 1)
                        {
                            #region MC

                            r2 = jovice.Query(@"
select p.MP_TO_MC, p.MP_VCID, p.MP_Protocol, p.MP_Type,
n.NO_Name, n.NO_Manufacture, n.NO_Model, n.NO_Version, n.NO_TimeStamp, c.MC_ID, c.MC_VCID, c.MC_Description, c.MC_MTU, c.MC_Status, c.MC_Protocol, c.MC_Type,
p2.MP_VCID as MP2_VCID, p2.MP_Protocol as MP2_Protocol, p2.MP_Type as MP2_Type, aw.AW_AG, se.SE_SID, p.MP_MS
from MEPeer p
left join MECircuit c on p.MP_TO_MC = c.MC_ID
left join Service se on c.MC_SE = se.SE_ID
left join Node n on c.MC_NO = n.NO_ID
left join Area ar on n.NO_AR = ar.AR_ID
left join AreaWitel aw on ar.AR_AW = aw.AW_ID
left join MEPeer p2 on c.MC_ID = p2.MP_MC
where p.MP_MC = {0}
order by p2.MP_TO_MC desc
", mcID);
                            Row row2 = r2[0];

                            if (r2.Count > 1)
                            {
                                if (serviceType == "ME") serviceSubType = "PM";
                            }

                            string mpToMC = row2["MP_TO_MC"].ToString();
                            remoteMC = null;

                            if (mpToMC != null)
                            {

                                topologyElementCurrent = new List<object>();
                                topologyElementCurrent.Add("MC"); //0
                                topologyElementCurrent.Add(row2["NO_Name"].ToString());//1
                                topologyElementCurrent.Add(row2["NO_Manufacture"].ToString());//2
                                topologyElementCurrent.Add(row2["NO_Model"].ToString());//3
                                topologyElementCurrent.Add(row2["NO_Version"].ToString());//4

                                nodeInfoNodeCurrent = new List<object>();
                                nodeInfoNodeCurrent.Add(row2["NO_Name"].ToString());
                                nodeInfoNodeCurrent.Add(ProcessDateTime(row2["NO_TimeStamp"].ToDateTime()));
                                nodeInfoTopologyCurrent.Add(nodeInfoNodeCurrent.ToArray());

                                //--
                                topologyElementCurrent.Add(row2["MP_VCID"].ToString());//5
                                topologyElementCurrent.Add(row2["MP_Protocol"].ToBool());//6
                                topologyElementCurrent.Add(row2["MP_Type"].ToString());//7
                                //--
                                topologyElementCurrent.Add(row2["MP2_VCID"].ToString());//8
                                topologyElementCurrent.Add(row2["MP2_Protocol"].ToBool());//9
                                topologyElementCurrent.Add(row2["MP2_Type"].ToString());//10
                                //--
                                topologyElementCurrent.Add(row2["MC_VCID"].ToString());//11
                                topologyElementCurrent.Add(row2["MC_Description"].ToString());//12
                                topologyElementCurrent.Add(row2["MC_MTU"].ToIntShort());//13
                                topologyElementCurrent.Add(row2["MC_Status"].ToBool());//14
                                topologyElementCurrent.Add(row2["MC_Protocol"].ToBool());//15
                                topologyElementCurrent.Add(row2["MC_Type"].ToString());//16

                                //--
                                string remoteSID = row2["SE_SID"].ToString();

                                topologyElementCurrent.Add(remoteSID != localSID ? remoteSID : null);//17

                                remoteMC = mpToMC;

                                string localAG = row["AW_AG"].ToString();
                                string remoteAG = row2["AW_AG"].ToString();

                                if (localAG != remoteAG)
                                {
                                    if (serviceSubType == "PP") serviceSubType = "IP";
                                }

                                //--
                                topologyElementCurrent.Add(r2.Count); // 18

                                topologyCurrent.Add(topologyElementCurrent.ToArray());
                            }
                            else
                            {
                                r2 = jovice.Query("select NO_Name, NO_Manufacture, NO_Model, NO_Version, NO_TimeStamp from Node, MESDP where NO_IP = MS_IP and MS_ID = {0}", row2["MP_MS"].ToString());

                                if (r2.Count == 1)
                                {
                                    #region XMC

                                    row2 = r2[0];

                                    topologyElementCurrent = new List<object>();
                                    topologyElementCurrent.Add("XMC"); //0
                                    topologyElementCurrent.Add(row2["NO_Name"].ToString());
                                    topologyElementCurrent.Add(row2["NO_Manufacture"].ToString());
                                    topologyElementCurrent.Add(row2["NO_Model"].ToString());
                                    topologyElementCurrent.Add(row2["NO_Version"].ToString());

                                    nodeInfoNodeCurrent = new List<object>();
                                    nodeInfoNodeCurrent.Add(row2["NO_Name"].ToString());
                                    nodeInfoNodeCurrent.Add(ProcessDateTime(row2["NO_TimeStamp"].ToDateTime()));
                                    nodeInfoTopologyCurrent.Add(nodeInfoNodeCurrent.ToArray());

                                    topologyCurrent.Add(topologyElementCurrent.ToArray());

                                    #endregion
                                }
                            }

                            #endregion

                            #region MI1

                            if (remoteMC != null)
                            {
                                r2 = jovice.Query(@"
select a.MI_Name, a.MI_Description, a.MI_Status, a.MI_Protocol,
qosa.MQ_Bandwidth as QInput, qosa.MQ_Name as QInputName,
qosb.MQ_Bandwidth as QOutput, qosb.MQ_Name as QOutputName,
a.MI_Rate_Input, a.MI_Rate_Output,
b.MI_ID as MI2_ID, b.MI_Description as MI2_Description, b.MI_Status as MI2_Status, b.MI_Protocol as MI2_Protocol,
ni.NI_Name, nn.NN_Name
from MEInterface a
left join MEQOS qosa on a.MI_MQ_Input = qosa.MQ_ID
left join MEQOS qosb on a.MI_MQ_Output = qosb.MQ_ID
left join MEInterface b on a.MI_MI = b.MI_ID
left join NeighborInterface ni on b.MI_TO_NI = ni.NI_ID
left join NodeNeighbor nn on ni.NI_NN = nn.NN_ID
where a.MI_MC = {0}
", remoteMC);

                                if (r2.Count > 0)
                                {
                                    row2 = r2[0];
                                    topologyElementCurrent = new List<object>();
                                    topologyElementCurrent.Add("MI1");//0
                                    //--
                                    string mi1Name = row2["MI_Name"].ToString();
                                    topologyElementCurrent.Add(row2["MI_Name"].ToString());//1
                                    topologyElementCurrent.Add(row2["MI_Description"].ToString());//2
                                    topologyElementCurrent.Add(row2["MI_Status"].ToBool());//3
                                    topologyElementCurrent.Add(row2["MI_Protocol"].ToBool());//4

                                    qinput = row2["QInput"].ToInt();
                                    topologyElementCurrent.Add(qinput);//5
                                    qoutput = row2["QOutput"].ToInt();
                                    topologyElementCurrent.Add(qoutput);//6
                                    rinput = row2["MI_Rate_Input"].ToInt();
                                    topologyElementCurrent.Add(rinput);//7
                                    routput = row2["MI_Rate_Output"].ToInt();
                                    topologyElementCurrent.Add(routput);//8

                                    int tOutput = qinput > 0 && qinput > rinput ? qinput : rinput > 0 && rinput > qinput ? rinput : 0;
                                    int tInput = qoutput > 0 && qoutput > routput ? qoutput : routput > 0 && routput > qoutput ? routput : 0;

                                    if (tInput > 0)
                                    {
                                        topologyRateInput = tInput < topologyRateInput ? tInput : topologyRateInput;
                                        topologyInputLimiter = row["NO_Name"].ToString();
                                    }
                                    if (tOutput > 0)
                                    {
                                        topologyRateOutput = tOutput < topologyRateOutput ? tOutput : topologyRateOutput;
                                        topologyOutputLimiter = row["NO_Name"].ToString();
                                    }


                                    topologyElementCurrent.Add(row2["QInputName"].ToString());//9
                                    topologyElementCurrent.Add(row2["QOutputName"].ToString());//10

                                    if (mi1Name.StartsWith("Ag"))
                                    {
                                        r3 = jovice.Query(@"
select m.MI_Name, m.MI_Description, m.MI_Status, m.MI_Protocol,
a.NI_Name, b.NN_Name 
from MEInterface m
left join NeighborInterface a on m.MI_TO_NI = a.NI_ID
left join NodeNeighbor b on a.NI_NN = b.NN_ID
where m.MI_MI = {0} and m.MI_Aggregator is not null", row2["MI2_ID"].ToString());

                                        List<object> io1Name = new List<object>();
                                        List<object> io1Desc = new List<object>();
                                        List<object> io1Stat = new List<object>();
                                        List<object> io1Prot = new List<object>();

                                        List<object> iolNNName = new List<object>();
                                        List<object> iolNIName = new List<object>();

                                        foreach (Row row3 in r3)
                                        {
                                            io1Name.Add(row3["MI_Name"].ToString());
                                            io1Desc.Add(row3["MI_Description"].ToString());
                                            io1Stat.Add(row3["MI_Status"].ToBool());
                                            io1Prot.Add(row3["MI_Protocol"].ToBool());

                                            iolNNName.Add(row3["NN_Name"].ToString());
                                            iolNIName.Add(row3["NI_Name"].ToString());
                                        }

                                        topologyElementCurrent.Add(io1Name.ToArray());//11
                                        topologyElementCurrent.Add(io1Desc.ToArray());//12

                                        string nnName = row2["NN_Name"].ToString();
                                        string niName = row2["NI_Name"].ToString();
                                        if (nnName == null)
                                        {
                                            int i = 0;
                                            foreach (string innname in iolNNName)
                                            {
                                                if (nnName == null)
                                                {
                                                    nnName = innname;
                                                    niName = (string)iolNIName[i];
                                                }
                                                else if (nnName != innname || niName != (string)iolNIName[i])
                                                {
                                                    nnName = null;
                                                    niName = null;
                                                    break;
                                                }

                                                i++;
                                            }
                                        }

                                        topologyElementCurrent.Add(nnName);
                                        topologyElementCurrent.Add(io1Stat.ToArray());//14
                                        topologyElementCurrent.Add(io1Prot.ToArray());//15
                                        topologyElementCurrent.Add(null); //16
                                        topologyElementCurrent.Add(niName);
                                    }
                                    else
                                    {
                                        topologyElementCurrent.Add(null);//11
                                        topologyElementCurrent.Add(row2["MI2_Description"].ToString());//12
                                        topologyElementCurrent.Add(row2["NN_Name"].ToString());//13
                                        topologyElementCurrent.Add(row2["MI2_Status"].ToBool());//14
                                        topologyElementCurrent.Add(row2["MI2_Protocol"].ToBool());//15
                                        topologyElementCurrent.Add(null); //16
                                        topologyElementCurrent.Add(row2["NI_Name"].ToString());
                                    }



                                    topologyCurrent.Add(topologyElementCurrent.ToArray());
                                }
                            }
                            else
                            {
                            }

                            #endregion
                        }
                        else if (cpeer > 1)
                        {
                            if (serviceType == "ME") serviceSubType = "PM";

                            #region MX
                            topologyElementCurrent = new List<object>();
                            topologyElementCurrent.Add("MX");//0
                            topologyElementCurrent.Add(cpeer);
                            topologyCurrent.Add(topologyElementCurrent.ToArray());
                            #endregion
                        }
                        else // if 0
                        {
                            if (cint == 2)
                            {
                                r2 = jovice.Query(@"
select a.MI_Name, a.MI_Description, a.MI_Status, a.MI_Protocol,
qosa.MQ_Bandwidth as QInput, qosa.MQ_Name as QInputName,
qosb.MQ_Bandwidth as QOutput, qosb.MQ_Name as QOutputName,
a.MI_Rate_Input, a.MI_Rate_Output,
b.MI_ID as MI2_ID, b.MI_Description as MI2_Description, b.MI_Status as MI2_Status, b.MI_Protocol as MI2_Protocol,
se.SE_SID,
c.NI_Name, d.NN_Name
from MEInterface a
left join MEQOS qosa on a.MI_MQ_Input = qosa.MQ_ID
left join MEQOS qosb on a.MI_MQ_Output = qosb.MQ_ID
left join MEInterface b on a.MI_MI = b.MI_ID
left join Service se on a.MI_SE = se.SE_ID
left join NeighborInterface c on b.MI_TO_NI = c.NI_ID
left join NodeNeighbor d on c.NI_NN = d.NN_ID
where a.MI_MC = {0} and a.MI_ID <> {1}
", mcID, row["MI_ID"].ToString());

                                if (r2.Count > 0)
                                {
                                    #region MI1

                                    Row row2 = r2[0];
                                    topologyElementCurrent = new List<object>();
                                    topologyElementCurrent.Add("MI1");//0
                                    //--
                                    string mi1Name = row2["MI_Name"].ToString();
                                    topologyElementCurrent.Add(row2["MI_Name"].ToString());//1
                                    topologyElementCurrent.Add(row2["MI_Description"].ToString());//2
                                    topologyElementCurrent.Add(row2["MI_Status"].ToBool());//3
                                    topologyElementCurrent.Add(row2["MI_Protocol"].ToBool());//4

                                    qinput = row2["QInput"].ToInt();
                                    topologyElementCurrent.Add(qinput);//5
                                    qoutput = row2["QOutput"].ToInt();
                                    topologyElementCurrent.Add(qoutput);//6
                                    rinput = row2["MI_Rate_Input"].ToInt();
                                    topologyElementCurrent.Add(rinput);//7
                                    routput = row2["MI_Rate_Output"].ToInt();
                                    topologyElementCurrent.Add(routput);//8

                                    int tOutput = qinput > 0 && qinput > rinput ? qinput : rinput > 0 && rinput > qinput ? rinput : 0;
                                    int tInput = qoutput > 0 && qoutput > routput ? qoutput : routput > 0 && routput > qoutput ? routput : 0;

                                    if (tInput > 0)
                                    {
                                        topologyRateInput = tInput < topologyRateInput ? tInput : topologyRateInput;
                                        topologyInputLimiter = row["NO_Name"].ToString();
                                    }
                                    if (tOutput > 0)
                                    {
                                        topologyRateOutput = tOutput < topologyRateOutput ? tOutput : topologyRateOutput;
                                        topologyOutputLimiter = row["NO_Name"].ToString();
                                    }

                                    topologyElementCurrent.Add(row2["QInputName"].ToString());//9
                                    topologyElementCurrent.Add(row2["QOutputName"].ToString());//10

                                    if (mi1Name.StartsWith("Ag"))
                                    {
                                        r3 = jovice.Query(@"
select m.MI_Name, m.MI_Description, m.MI_Status, m.MI_Protocol,
a.NI_Name, b.NN_Name
from MEInterface m
left join NeighborInterface a on m.MI_TO_NI = a.NI_ID
left join NodeNeighbor b on a.NI_NN = b.NN_ID
where m.MI_MI = {0} and m.MI_Aggregator is not null", row2["MI2_ID"].ToString());

                                        List<object> io1Name = new List<object>();
                                        List<object> io1Desc = new List<object>();
                                        List<object> io1Stat = new List<object>();
                                        List<object> io1Prot = new List<object>();

                                        List<object> io1NN = new List<object>();
                                        List<object> io1NI = new List<object>();

                                        foreach (Row row3 in r3)
                                        {
                                            io1Name.Add(row3["MI_Name"].ToString());
                                            io1Desc.Add(row3["MI_Description"].ToString());
                                            io1Stat.Add(row3["MI_Status"].ToBool());
                                            io1Prot.Add(row3["MI_Protocol"].ToBool());

                                            io1NI.Add(row3["NI_Name"].ToString());
                                            io1NN.Add(row3["NN_Name"].ToString());
                                        }

                                        topologyElementCurrent.Add(io1Name.ToArray());//11
                                        topologyElementCurrent.Add(io1Desc.ToArray());//12

                                        string nnName = row2["NN_Name"].ToString();
                                        string niName = row2["NI_Name"].ToString();
                                                                                
                                        if (nnName == null)
                                        {
                                            int i = 0;
                                            foreach (string innname in io1NN)
                                            {
                                                if (nnName == null)
                                                {
                                                    nnName = innname;
                                                    niName = (string)io1NI[i];
                                                }
                                                else if (nnName != innname || niName != (string)io1NI[i])
                                                {
                                                    nnName = null;
                                                    niName = null;
                                                    break;
                                                }

                                                i++;
                                            }
                                        }

                                        topologyElementCurrent.Add(nnName); //13
                                        topologyElementCurrent.Add(io1Stat.ToArray());//14
                                        topologyElementCurrent.Add(io1Prot.ToArray());//15

                                        string remoteSID = row2["SE_SID"].ToString();
                                        topologyElementCurrent.Add(remoteSID != localSID ? remoteSID : null);//16

                                        topologyElementCurrent.Add(niName); // 17
                                    }
                                    else
                                    {
                                        string miDesc = row2["MI2_Description"].ToString();
                                        topologyElementCurrent.Add(null);//11
                                        topologyElementCurrent.Add(miDesc);//12
                                        topologyElementCurrent.Add(row2["NN_Name"].ToString());// );//13
                                        topologyElementCurrent.Add(row2["MI2_Status"].ToBool());//14
                                        topologyElementCurrent.Add(row2["MI2_Protocol"].ToBool());//15

                                        string remoteSID = row2["SE_SID"].ToString();
                                        topologyElementCurrent.Add(remoteSID != localSID ? remoteSID : null);//16

                                        topologyElementCurrent.Add(row2["NI_Name"].ToString()); //17
                                    }




                                    topologyCurrent.Add(topologyElementCurrent.ToArray());

                                    #endregion
                                }
                            }
                            else
                            {
                                if (serviceType == "ME") serviceSubType = null;
                            }
                        }
                    }
                    else
                    {
                        #region MID
                        serviceType = "ID";
                        serviceSubType = null;

                        // cuma interface description
                        topologyElementCurrent = new List<object>();
                        topologyElementCurrent.Add("MID");//0
                        topologyElementCurrent.Add(row["NO_Name"].ToString());//1
                        topologyElementCurrent.Add(row["NO_Manufacture"].ToString());//2
                        topologyElementCurrent.Add(row["NO_Model"].ToString());//3
                        topologyElementCurrent.Add(row["NO_Version"].ToString());//4

                        nodeInfoNodeCurrent = new List<object>();
                        nodeInfoNodeCurrent.Add(row["NO_Name"].ToString());
                        nodeInfoNodeCurrent.Add(ProcessDateTime(row["NO_TimeStamp"].ToDateTime()));
                        nodeInfoTopologyCurrent.Add(nodeInfoNodeCurrent.ToArray());

                        //--
                        topologyElementCurrent.Add(row["MI_Name"].ToString());//5
                        topologyElementCurrent.Add(row["MI_Description"].ToString());//6
                        topologyElementCurrent.Add(row["MI_Status"].ToBool());//7
                        topologyElementCurrent.Add(row["MI_Protocol"].ToBool());//8
                        topologyCurrent.Add(topologyElementCurrent.ToArray());

                        #endregion
                    }

                    topology.Add(topologyCurrent.ToArray());
                    vrf.Add(null);
                    rateInput.Add(topologyRateInput);
                    rateOutput.Add(topologyRateOutput);
                    topologyPurpose.Add(null);
                    ip.Add(null);
                    ipd.Add(null);
                    vcid.Add(topologyCurrentVCID);
                    nodeInfo.Add(nodeInfoTopologyCurrent.ToArray());
                    inputLimiter.Add(topologyInputLimiter);
                    outputLimiter.Add(topologyOutputLimiter);

                    if (topologyLocalAccess != null)
                    {
                        r = jovice.Query(@"
select AR_Name, AW_Name, AG_Name
from Area
left join AreaWitel on AR_AW = AW_ID
left join AreaGroup on AW_AG = AG_ID
where AR_ID = {0}
", topologyLocalAccess);
                        Row rowla = r[0];

                        topologyLocalAccess = "STO " + rowla["AR_Name"].ToString().Trim() + ", WITEL " + rowla["AW_Name"].ToString().Trim() + ", " + rowla["AG_Name"].ToString().Trim();
                    }

                    localacc.Add(topologyLocalAccess);
                }

                #endregion
            }

            objects.Add(topology.ToArray()); // 9
            objects.Add(vrf.ToArray()); // 10
            objects.Add(rateInput.ToArray()); // 11
            objects.Add(inputLimiter.ToArray()); // 12
            objects.Add(rateOutput.ToArray()); // 13
            objects.Add(outputLimiter.ToArray()); // 14
            objects.Add(topologyPurpose.ToArray()); // 15
            objects.Add(ip.ToArray()); // 16
            objects.Add(vcid.ToArray()); // 17
            objects.Add(nodeInfo.ToArray()); // 18
            objects.Add(localacc.ToArray()); // 19
            objects.Add(routeType.ToArray()); // 20
        }
        
        public static void Prepare(SearchMatchResult matchResult)
        {
            matchResult.AddColumn("Topology"); // 9
            matchResult.AddColumn("Vrf"); // 10
            matchResult.AddColumn("RateInput"); // 11
            matchResult.AddColumn("InputLimiter"); // 12
            matchResult.AddColumn("RateOutput"); // 13
            matchResult.AddColumn("OutputLimiter"); // 14
            matchResult.AddColumn("Purpose"); // 15
            matchResult.AddColumn("IP"); // 16
            matchResult.AddColumn("VCID"); // 17
            matchResult.AddColumn("NodeInfo"); // 18
            matchResult.AddColumn("LocalAccess"); // 19
            matchResult.AddColumn("RouteType"); // 20
        }

        private static DateTime? ProcessDateTime(DateTime fromdb)
        {
            if (fromdb > DateTime.MinValue)
            {
                return fromdb.ConvertOffset(7);
            }
            else return null;
        }

    }
}