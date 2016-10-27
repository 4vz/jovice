﻿using Aphysoft.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jovice
{
    internal static class NecrowVirtualization
    {
        #region Fields

        private static List<Tuple<string, List<Tuple<string, string, string, string, string, string>>>> pePhysicalInterfaces = null;

        internal static List<Tuple<string, List<Tuple<string, string, string, string, string, string>>>> PEPhysicalInterfaces
        {
            get { return pePhysicalInterfaces; }
        }

        private static List<Tuple<string, List<Tuple<string, string, string, string>>>> mePhysicalInterfaces = null;

        internal static List<Tuple<string, List<Tuple<string, string, string, string>>>> MEPhysicalInterfaces
        {
            get { return mePhysicalInterfaces; }
        }

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

        #endregion

        #region Methods

        internal static void Load()
        {
            Database jovice = Necrow.Jovice;
            Result result;
            Batch batch = jovice.Batch();
            Insert insert;

            string currentNode;
            int count;

            result = jovice.Query(@"
select NO_Name, LEN(NO_Name) as NO_LEN, PI_Name, LEN(PI_Name) as PI_LEN, PI_Type, PI_ID, PI_Description, PI_PI, PI_TO_MI from
(select NO_Name, NO_ID from Node where NO_Type = 'P' and NO_Active = 1) n left join PEInterface on PI_NO = NO_ID and PI_Type in ('Hu', 'Te', 'Gi', 'Fa', 'Et')
order by NO_LEN desc, NO_Name, PI_LEN desc, PI_Name
");
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

            Necrow.Event("Loaded " + count + " PE physical interfaces");

            result = jovice.Query(@"
select NO_Name, LEN(NO_Name) as NO_LEN, MI_Name, LEN(MI_Name) as MI_LEN, MI_Type, MI_ID, MI_Description from 
(select NO_Name, NO_ID from Node where NO_Type = 'M' and NO_Active = 1) n left join MEInterface on MI_NO = NO_ID and MI_Type in ('Hu', 'Te', 'Gi', 'Fa', 'Et')
order by NO_LEN desc, NO_Name, MI_LEN desc, MI_Name
");

            mePhysicalInterfaces = new List<Tuple<string, List<Tuple<string, string, string, string>>>>();
            List<Tuple<string, string, string, string>> currentMEInterfaces = new List<Tuple<string, string, string, string>>();

            currentNode = null;
            count = 0;

            foreach (Row row in result)
            {
                string node = row["NO_Name"].ToString();

                if (currentNode != node)
                {
                    if (currentNode != null)
                    {
                        mePhysicalInterfaces.Add(new Tuple<string, List<Tuple<string, string, string, string>>>(currentNode,
                            new List<Tuple<string, string, string, string>>(currentMEInterfaces)));
                        currentMEInterfaces.Clear();
                    }
                    currentNode = node;
                }

                string miName = row["MI_Name"].ToString();

                if (miName != null)
                {
                    currentMEInterfaces.Add(new Tuple<string, string, string, string>(
                        miName, row["MI_Description"].ToString(), row["MI_ID"].ToString(), row["MI_Type"].ToString()));
                    count++;
                }
            }
            mePhysicalInterfaces.Add(new Tuple<string, List<Tuple<string, string, string, string>>>(currentNode, currentMEInterfaces));

            Necrow.Event("Loaded " + count + " ME physical interfaces");

            result = jovice.Query(@"
select NO_Name, NA_Name from Node, NodeAlias where NA_NO = NO_ID order by NA_Name
");
            aliases = new Dictionary<string, List<string>>();

            count = 0;

            foreach (Row row in result)
            {
                string noName = row["NO_Name"].ToString();

                if (!aliases.ContainsKey(noName))
                    aliases.Add(noName, new List<string>());

                aliases[noName].Add(row["NA_Name"].ToString());
                count++;
            }

            Necrow.Event("Loaded " + count + " node aliases");

            result = jovice.Query("select * from NodeNeighbor");

            nodeNeighbors = new Dictionary<string, string>();

            foreach (Row row in result)
            {
                nodeNeighbors.Add(row["NN_Name"].ToString(), row["NN_ID"].ToString());
            }

            Necrow.Event("Loaded " + nodeNeighbors.Count + " neighbors");

            result = jovice.Query(@"
select NN_Name, LEN(NN_Name) as NN_LEN, NI_Name, LEN(NI_Name) as NI_LEN, NI_ID from 
(select NN_Name, NN_ID from NodeNeighbor
) n left join NeighborInterface on NI_NN = NN_ID and NI_Name <> 'UNSPECIFIED'
order by NN_LEN desc, NN_Name, NI_LEN desc, NI_Name
");

            nnPhysicalInterfaces = new List<Tuple<string, List<Tuple<string, string>>>>();
            List<Tuple<string, string>> currentNNInterfaces = new List<Tuple<string, string>>();

            currentNode = null;
            count = 0;

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
                    count++;
                }
            }
            nnPhysicalInterfaces.Add(new Tuple<string, List<Tuple<string, string>>>(currentNode, currentNNInterfaces));

            Necrow.Event("Loaded " + count + " neighbor interfaces");

            result = jovice.Query(@"
select NN_ID, NN_Name, NI_ID from NodeNeighbor left join NeighborInterface on NI_NN = NN_ID and NI_Name = 'UNSPECIFIED'
");
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
                        batch.Execute("update PEInterface set PI_TO_NI = NULL where PI_TO_NI in (select NI_ID from NeighborInterface where NI_NN = {0})", id);
                        batch.Execute("update MEInterface set MI_TO_NI = NULL where PI_TO_NI in (select NI_ID from NeighborInterface where NI_NN = {0})", id);
                        batch.Execute("delete from NeighborInterface where NI_NN = {0}", id);
                        batch.Execute("delete from NodeNeighbor where NN_ID = {0}", id);
                        Necrow.Event("Removed duplicated neighbor key: " + node);
                    }
                }
                else
                {
                    unid = Database.ID();

                    insert = jovice.Insert("NeighborInterface");
                    insert.Value("NI_ID", unid);
                    insert.Value("NI_NN", id);
                    insert.Value("NI_Name", "UNSPECIFIED");

                    batch.Execute(insert);
                    nnUnspecifiedInterfaces.Add(node, unid);

                    Necrow.Event("Added missing UNSPECIFIED interface to neighbor node " + node);
                }
            }

            batch.Commit();

            Necrow.Event("Loaded " + nnUnspecifiedInterfaces.Count + " neighbor references");
        }

        internal static void PEPhysicalInterfacesSort()
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

            foreach (Tuple<string, List<Tuple<string, string, string, string, string, string>>> c in pePhysicalInterfaces)
            {
                List<Tuple<string, string, string, string, string, string>> list = c.Item2;

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
            mePhysicalInterfaces.Sort(delegate (
                Tuple<string, List<Tuple<string, string, string, string>>> a,
                Tuple<string, List<Tuple<string, string, string, string>>> b)
            {
                string nodeA = a.Item1;
                string nodeB = b.Item1;

                // NO_LEN desc
                if (nodeA.Length < nodeB.Length) return 1;
                else if (nodeA.Length > nodeB.Length) return -1;
                else return nodeA.CompareTo(nodeB); // NO_Name
            });

            foreach (Tuple<string, List<Tuple<string, string, string, string>>> c in mePhysicalInterfaces)
            {
                List<Tuple<string, string, string, string>> list = c.Item2;

                list.Sort(delegate (
                    Tuple<string, string, string, string> a,
                    Tuple<string, string, string, string> b
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

        internal static void AliasReload()
        {
            aliases = null;

            Result result = Necrow.Jovice.Query(@"
select NO_Name, NA_Name from Node, NodeAlias where NA_NO = NO_ID order by NA_Name
");
            aliases = new Dictionary<string, List<string>>();

            foreach (Row row in result)
            {
                string noName = row["NO_Name"].ToString();

                if (!aliases.ContainsKey(noName))
                    aliases.Add(noName, new List<string>());

                aliases[noName].Add(row["NA_Name"].ToString());
            }
        }

        internal static void FlushNeighborInterface()
        {
            Database jovice = Necrow.Jovice;

            // interfaces is interfaces that have been removed its references from mi_to_ni or pi_to_no
            // wat do?

            Result result = jovice.Query(@"
select NN_Name, NI_ID from NeighborInterface 
left join MEInterface on MI_TO_NI = NI_ID 
left join PEInterface on PI_TO_NI = NI_ID
left join NodeNeighbor on NN_ID = NI_NN
where NI_Name <> 'UNSPECIFIED' and MI_ID is null and PI_ID is null
");
            if (result.Count > 0)
            {
                Necrow.Event("Removing unused interfaces on Node Neighbors...");

                Batch batch = jovice.Batch();

                batch.Begin();
                foreach (Row row in result)
                {
                    string nn = row["NN_Name"].ToString();
                    string ni = row["NI_ID"].ToString();

                    batch.Execute("delete from NeighborInterface where NI_ID = {0}", ni);

                    foreach (Tuple<string, List<Tuple<string, string>>> entry in NecrowVirtualization.NNPhysicalInterfaces)
                    {
                        if (entry.Item1 == nn)
                        {
                            Tuple<string, string> nitoremove = null;
                            foreach (Tuple<string, string> nix in entry.Item2)
                            {
                                if (nix.Item2 == ni)
                                {
                                    nitoremove = nix;
                                    break;
                                }
                            }
                            if (nitoremove != null) entry.Item2.Remove(nitoremove);
                            break;
                        }
                    }
                }
                result = batch.Commit();

                Necrow.Event("Removed " + result.AffectedRows + " interfaces");
            }
        }

        #endregion
    }
}