
using Aphysoft.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Center
{
    internal static class NecrowVirtualization
    {
        #region Fields

        private static Necrow instance;

        private static bool ready = false;

        public static bool IsReady
        {
            get { return ready; }
        }

        internal static object PESync = new object();

        private static List<Tuple<string, List<Tuple<string, string, string, string, string, string>>>> pePhysicalInterfaces = null;

        /// <summary>
        /// 1 PI_Name 2 PI_Description 3 PI_ID 4 PI_Type 5 PI_PI 6 PI_TO_MI
        /// </summary>
        internal static List<Tuple<string, List<Tuple<string, string, string, string, string, string>>>> PEPhysicalInterfaces
        {
            get { return pePhysicalInterfaces; }
        }

        internal static object MESync = new object();

        private static List<Tuple<string, List<Tuple<string, string, string, string, string, string, string>>>> mePhysicalInterfaces = null;

        /// <summary>
        /// 1 MI_Name 2 MI_Description 3 MI_ID 4 MI_Type 5 MI_MI 6 MI_TO_MI 7 MI_TO_PI
        /// </summary>
        internal static List<Tuple<string, List<Tuple<string, string, string, string, string, string, string>>>> MEPhysicalInterfaces
        {
            get { return mePhysicalInterfaces; }
        }

        internal static object NNSync = new object();

        private static List<Tuple<string, List<Tuple<string, string>>>> nnPhysicalInterfaces = null;

        internal static List<Tuple<string, List<Tuple<string, string>>>> NNPhysicalInterfaces
        {
            get { return nnPhysicalInterfaces; }
        }

        private static Dictionary<string, string> nodeNeighbors = null;

        internal static Dictionary<string, string> NodeNeighbors
        {
            get { return nodeNeighbors; }
        }

        private static Dictionary<string, string> nnUnspecifiedInterfaces = null;

        internal static Dictionary<string, string> NNUnspecifiedInterfaces
        {
            get { return nnUnspecifiedInterfaces; }
        }

        private static Dictionary<string, List<string>> aliases = null;

        internal static Dictionary<string, List<string>> Aliases
        {
            get { return aliases; }
        }

        internal static object DACSync = new object();

        private static Dictionary<string, Tuple<string, string, string, string>> derivedAreaConnections = null;

        internal static Dictionary<string, Tuple<string, string, string, string>> DerivedAreaConnections
        {
            get { return derivedAreaConnections; }
        }

        #endregion

        #region Methods

        internal static void Load(Necrow instance)
        {
            NecrowVirtualization.instance = instance;

            Database jovice = Jovice.Database;
            Result result;
            Batch batch = jovice.Batch();

            string currentNode;
            int count;

            #region PE Interfaces

            lock (PESync)
            {
                result = jovice.Query(@"
select NO_Name, LEN(NO_Name) as NO_LEN, PI_Name, LEN(PI_Name) as PI_LEN, PI_Type, PI_ID, PI_Description, PI_PI, PI_TO_MI from
(select NO_Name, NO_ID from Node where NO_Type = 'P' and NO_Active = 1) n left join PEInterface on PI_NO = NO_ID and PI_Type in ('Hu', 'Te', 'Gi', 'Fa', 'Et')
order by NO_LEN desc, NO_Name, PI_LEN desc, PI_Name
");
                if (!result.OK) throw new Exception("Virtualization failed");

                pePhysicalInterfaces = new List<Tuple<string, List<Tuple<string, string, string, string, string, string>>>>();
                List<Tuple<string, string, string, string, string, string>> currentPEInterfaces = new List<Tuple<string, string, string, string, string, string>>();

                currentNode = null;
                count = 0;

                foreach (Row row in result)
                {
                    string node = row["NO_Name"].ToString();

                    if (currentNode != node)
                    {
                        if (currentNode != null)
                        {
                            pePhysicalInterfaces.Add(new Tuple<string, List<Tuple<string, string, string, string, string, string>>>(currentNode,
                                new List<Tuple<string, string, string, string, string, string>>(currentPEInterfaces)));
                            currentPEInterfaces.Clear();
                        }
                        currentNode = node;
                    }

                    string piName = row["PI_Name"].ToString();

                    if (piName != null)
                    {
                        currentPEInterfaces.Add(new Tuple<string, string, string, string, string, string>(
                            piName, row["PI_Description"].ToString(), row["PI_ID"].ToString(), row["PI_Type"].ToString(), row["PI_PI"].ToString(), row["PI_TO_MI"].ToString()));
                        count++;
                    }
                }
                pePhysicalInterfaces.Add(new Tuple<string, List<Tuple<string, string, string, string, string, string>>>(currentNode, currentPEInterfaces));
            }

            instance.Event("Loaded " + count + " PE physical interfaces");

            #endregion

            #region ME Interfaces

            lock (MESync)
            {
                result = jovice.Query(@"
select NO_Name, LEN(NO_Name) as NO_LEN, MI_Name, LEN(MI_Name) as MI_LEN, MI_Type, MI_ID, MI_Description, MI_MI, MI_TO_MI, MI_TO_PI from 
(select NO_Name, NO_ID from Node where NO_Type = 'M' and NO_Active = 1) n left join MEInterface on MI_NO = NO_ID and MI_Type in ('Hu', 'Te', 'Gi', 'Fa', 'Et')
order by NO_LEN desc, NO_Name, MI_LEN desc, MI_Name
");
                if (!result.OK) throw new Exception("Virtualization failed");

                mePhysicalInterfaces = new List<Tuple<string, List<Tuple<string, string, string, string, string, string, string>>>>();
                List<Tuple<string, string, string, string, string, string, string>> currentMEInterfaces = new List<Tuple<string, string, string, string, string, string, string>>();

                currentNode = null;
                count = 0;

                foreach (Row row in result)
                {
                    string node = row["NO_Name"].ToString();

                    if (currentNode != node)
                    {
                        if (currentNode != null)
                        {
                            mePhysicalInterfaces.Add(new Tuple<string, List<Tuple<string, string, string, string, string, string, string>>>(currentNode,
                                new List<Tuple<string, string, string, string, string, string, string>>(currentMEInterfaces)));
                            currentMEInterfaces.Clear();
                        }
                        currentNode = node;
                    }

                    string miName = row["MI_Name"].ToString();

                    if (miName != null)
                    {
                        currentMEInterfaces.Add(new Tuple<string, string, string, string, string, string, string>(
                            miName, row["MI_Description"].ToString(), row["MI_ID"].ToString(), row["MI_Type"].ToString(), row["MI_MI"].ToString(), row["MI_TO_MI"].ToString(), row["MI_TO_PI"].ToString()));
                        count++;
                    }
                }
                mePhysicalInterfaces.Add(new Tuple<string, List<Tuple<string, string, string, string, string, string, string>>>(currentNode, currentMEInterfaces));
            }

            instance.Event("Loaded " + count + " ME physical interfaces");

            #endregion

            #region Node Alias

            count = AliasLoad();

            instance.Event("Loaded " + count + " node aliases");

            #endregion

            #region Neighbor

            Tuple<int, int, int> counts = NeighborLoad();
            instance.Event("Loaded " + counts.Item1 + " neighbors");
            instance.Event("Loaded " + counts.Item2 + " neighbor interfaces");
            instance.Event("Loaded " + counts.Item3 + " neighbor references");

            #endregion

            #region Derived Area Connections

            result = jovice.Query("select * from DerivedAreaConnection");
            if (!result.OK) throw new Exception("Virtualization failed");

            derivedAreaConnections = new Dictionary<string, Tuple<string, string, string, string>>();
            
            foreach (Row row in result)
            {
                string id = row["DAC_ID"].ToString();
                string ar1 = row["DAC_AR_1"].ToString();
                string ar2 = row["DAC_AR_2"].ToString();
                string mi1 = row["DAC_MI_1"].ToString();
                string mi2 = row["DAC_MI_2"].ToString();

                derivedAreaConnections.Add(id, new Tuple<string, string, string, string>(ar1, ar2, mi1, mi2));
            }

            instance.Event("Loaded " + derivedAreaConnections.Count + " derived area connections");

            #endregion

            ready = true;
        }

        internal static void PEPhysicalInterfacesSort()
        {
            PEPhysicalInterfacesSort(false);
        }

        internal static void PEPhysicalInterfacesSort(bool onlyNodes)
        {
            lock (PESync)
            {
                pePhysicalInterfaces.Sort(delegate (
                    Tuple<string, List<Tuple<string, string, string, string, string, string>>> a,
                    Tuple<string, List<Tuple<string, string, string, string, string, string>>> b)
                {
                    string nodeA = a.Item1;
                    string nodeB = b.Item1;

                    // NO_LEN desc
                    if (nodeA.Length < nodeB.Length) return 1;
                    else if (nodeA.Length > nodeB.Length) return -1;
                    else return nodeA.CompareTo(nodeB); // NO_Name
                });

                if (!onlyNodes)
                {
                    foreach (Tuple<string, List<Tuple<string, string, string, string, string, string>>> c in pePhysicalInterfaces)
                    {
                        List<Tuple<string, string, string, string, string, string>> list = c.Item2;
                        PEPhysicalInterfacesSort(list);
                    }
                }
            }
        }

        internal static void PEPhysicalInterfacesSort(List<Tuple<string, string, string, string, string, string>> list)
        {
            lock (list)
            {
                list.Sort(delegate (
                        Tuple<string, string, string, string, string, string> a,
                        Tuple<string, string, string, string, string, string> b
                        )
                {
                    string nameA = a.Item1;
                    string nameB = b.Item1;

                    // Name_LEN desc
                    if (nameA.Length < nameB.Length) return 1;
                    else if (nameA.Length > nameB.Length) return -1;
                    else return nameA.CompareTo(nameB); // Name
                });
            }
        }

        internal static void MEPhysicalInterfacesSort()
        {
            MEPhysicalInterfacesSort(false);
        }

        internal static void MEPhysicalInterfacesSort(bool onlyNodes)
        {
            lock (MESync)
            {
                mePhysicalInterfaces.Sort(delegate (
                    Tuple<string, List<Tuple<string, string, string, string, string, string, string>>> a,
                    Tuple<string, List<Tuple<string, string, string, string, string, string, string>>> b)
                {
                    string nodeA = a.Item1;
                    string nodeB = b.Item1;

                    // NO_LEN desc
                    if (nodeA.Length < nodeB.Length) return 1;
                    else if (nodeA.Length > nodeB.Length) return -1;
                    else return nodeA.CompareTo(nodeB); // NO_Name
                });

                if (!onlyNodes)
                {
                    foreach (Tuple<string, List<Tuple<string, string, string, string, string, string, string>>> c in mePhysicalInterfaces)
                    {
                        List<Tuple<string, string, string, string, string, string, string>> list = c.Item2;
                        MEPhysicalInterfacesSort(list);
                    }
                }
            }
        }

        internal static void MEPhysicalInterfacesSort(List<Tuple<string, string, string, string, string, string, string>> list)
        {
            lock (list)
            {
                list.Sort(delegate (
                        Tuple<string, string, string, string, string, string, string> a,
                        Tuple<string, string, string, string, string, string, string> b
                        )
                {
                    string nameA = a.Item1;
                    string nameB = b.Item1;

                    // Name_LEN desc
                    if (nameA.Length < nameB.Length) return 1;
                    else if (nameA.Length > nameB.Length) return -1;
                    else return nameA.CompareTo(nameB); // Name
                });
            }
        }

        internal static void RemovePhysicalInterfacesByNode(string nodeName)
        {
            bool done = false;

            if (done == false)
            {
                lock (PESync)
                {
                    Tuple<string, List<Tuple<string, string, string, string, string, string>>> removeThis1 = null;

                    foreach (Tuple<string, List<Tuple<string, string, string, string, string, string>>> tuple in pePhysicalInterfaces)
                    {
                        if (tuple.Item1 == nodeName)
                        {
                            removeThis1 = tuple;
                            break;
                        }
                    }

                    if (removeThis1 != null)
                    {
                        pePhysicalInterfaces.Remove(removeThis1);
                        done = true;
                    }
                }
            }
            if (done == false)
            {
                lock (MESync)
                {
                    Tuple<string, List<Tuple<string, string, string, string, string, string, string>>> removeThis2 = null;

                    foreach (Tuple<string, List<Tuple<string, string, string, string, string, string, string>>> tuple in mePhysicalInterfaces)
                    {
                        if (tuple.Item1 == nodeName)
                        {
                            removeThis2 = tuple;
                            break;
                        }
                    }

                    if (removeThis2 != null)
                    {
                        mePhysicalInterfaces.Remove(removeThis2);
                        done = true;
                    }
                }
            }
        }

        internal static bool AliasExists(string name)
        {
            string node;
            return AliasExists(name, out node);
        }

        internal static bool AliasExists(string name, out string node)
        {
            bool exists = false;
            node = null;

            foreach (KeyValuePair<string, List<string>> pair in Aliases)
            {
                if (pair.Value.Contains(name))
                {
                    exists = true;
                    node = pair.Key;
                    break;
                }
            }
            return exists;
        }

        internal static int AliasLoad()
        {
            aliases = null;

            Result result = Jovice.Database.Query(@"
select NO_Name, NA_Name from Node, NodeAlias where NA_NO = NO_ID order by NA_Name
");
            if (!result.OK) throw new Exception("Virtualization failed");

            aliases = new Dictionary<string, List<string>>();

            int count = 0;

            foreach (Row row in result)
            {
                string noName = row["NO_Name"].ToString();

                if (!aliases.ContainsKey(noName))
                    aliases.Add(noName, new List<string>());

                aliases[noName].Add(row["NA_Name"].ToString());

                count++;
            }

            return count;
        }

        internal static Tuple<int, int, int> NeighborLoad()
        {
            Database jovice = Jovice.Database;
            Result result;
            Batch batch = jovice.Batch();
            Insert insert;

            string currentNode;
            int count1 = 0, count2 = 0, count3 = 0;

            lock (NNSync)
            {
                #region Node Neighbor

                result = jovice.Query("select * from NodeNeighbor");
                if (!result.OK) throw new Exception("Virtualization failed");

                nodeNeighbors = new Dictionary<string, string>();

                batch.Begin();
                foreach (Row row in result)
                {
                    string name = row["NN_Name"].ToString();
                    string id = row["NN_ID"].ToString();
                    if (!nodeNeighbors.ContainsKey(name))
                        nodeNeighbors.Add(name, id);
                    else
                    {
                        instance.Event("Duplicated NodeNeighbor " + name + " removed ID " + id);
                        batch.Execute("update PEInterface set PI_TO_NI = NULL where PI_TO_NI in (select NI_ID from NBInterface where NI_NN = {0})", id);
                        batch.Execute("update MEInterface set MI_TO_NI = NULL where MI_TO_NI in (select NI_ID from NBInterface where NI_NN = {0})", id);
                        batch.Execute("delete from NBInterface where NI_NN = {0}", id);
                        batch.Execute("delete from NodeNeighbor where NN_ID = {0}", id);
                    }
                }
                batch.Commit();

                count1 = nodeNeighbors.Count;

                #endregion

                #region Neighbor Interface

                result = jovice.Query(@"
select NN_Name, LEN(NN_Name) as NN_LEN, NI_Name, LEN(NI_Name) as NI_LEN, NI_ID from 
(select NN_Name, NN_ID from NodeNeighbor
) n left join NBInterface on NI_NN = NN_ID and NI_Name <> 'UNSPECIFIED'
order by NN_LEN desc, NN_Name, NI_LEN desc, NI_Name
");
                if (!result.OK) throw new Exception("Virtualization failed");

                nnPhysicalInterfaces = new List<Tuple<string, List<Tuple<string, string>>>>();
                List<Tuple<string, string>> currentNNInterfaces = new List<Tuple<string, string>>();

                currentNode = null;
                count2 = 0;

                foreach (Row row in result)
                {
                    string node = row["NN_Name"].ToString();

                    if (currentNode != node)
                    {
                        if (currentNode != null)
                        {
                            nnPhysicalInterfaces.Add(new Tuple<string, List<Tuple<string, string>>>(currentNode,
                                new List<Tuple<string, string>>(currentNNInterfaces)));
                            currentNNInterfaces.Clear();
                        }
                        currentNode = node;
                    }

                    string niName = row["NI_Name"].ToString();

                    if (niName != null)
                    {
                        currentNNInterfaces.Add(new Tuple<string, string>(
                            niName, row["NI_ID"].ToString()));
                        count2++;
                    }
                }
                nnPhysicalInterfaces.Add(new Tuple<string, List<Tuple<string, string>>>(currentNode, currentNNInterfaces));

                #endregion

                #region Neighbor Unspecified Reference

                result = jovice.Query(@"
select NN_ID, NN_Name, NI_ID from NodeNeighbor left join NBInterface on NI_NN = NN_ID and NI_Name = 'UNSPECIFIED'
");
                if (!result.OK) throw new Exception("Virtualization failed");

                batch.Begin();

                nnUnspecifiedInterfaces = new Dictionary<string, string>();

                foreach (Row row in result)
                {
                    string id = row["NN_ID"].ToString();
                    string node = row["NN_Name"].ToString();
                    string unid = row["NI_ID"].ToString();

                    if (unid != null)
                    {
                        if (!nnUnspecifiedInterfaces.ContainsKey(node))
                            nnUnspecifiedInterfaces.Add(node, unid);
                        else
                        {
                            batch.Execute("update PEInterface set PI_TO_NI = NULL where PI_TO_NI in (select NI_ID from NBInterface where NI_NN = {0})", id);
                            batch.Execute("update MEInterface set MI_TO_NI = NULL where PI_TO_NI in (select NI_ID from NBInterface where NI_NN = {0})", id);
                            batch.Execute("delete from NBInterface where NI_NN = {0}", id);
                            batch.Execute("delete from NodeNeighbor where NN_ID = {0}", id);
                            instance.Event("Removed duplicated neighbor key: " + node);
                        }
                    }
                    else
                    {
                        unid = Database.ID();

                        insert = jovice.Insert("NBInterface");
                        insert.Value("NI_ID", unid);
                        insert.Value("NI_NN", id);
                        insert.Value("NI_Name", "UNSPECIFIED");

                        batch.Execute(insert);
                        nnUnspecifiedInterfaces.Add(node, unid);

                        instance.Event("Added missing UNSPECIFIED interface to neighbor node " + node);
                    }
                }

                result = batch.Commit();
                if (!result.OK) throw new Exception("Virtualization failed");

                count3 = nnUnspecifiedInterfaces.Count;

                #endregion
            }
            
            return new Tuple<int, int, int>(count1, count2, count3);
        }

        #endregion
    }
}
