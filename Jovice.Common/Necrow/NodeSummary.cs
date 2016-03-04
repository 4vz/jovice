using Aphysoft.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Jovice
{
    internal static class Summary
    {
        #region Fields

        private static Thread mainLoop = null;

        private static bool stop = false;

        private static bool idle = false;

        #endregion

        #region Methods

        private static void Event(string message)
        {
            Necrow.Event(message, "SUMM");
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

        private delegate string SummaryCallback();
                 
        private static void MainLoop()
        {
            Culture.Default();

            Event("Started");

            Database j = Necrow.JoviceDatabase;

            while (!stop)
            {
                Result result;

                Batch batch = j.Batch();

                result = j.Query(@"
select NO_ID, NO_Type, NO_Name from Node where NO_Active = 1
");
                int commitAffected = 0;

                foreach (Row row in result)
                {
                    batch.Clear();

                    string noID = row["NO_ID"].ToString();
                    string noType = row["NO_Type"].ToString();

                    Result result2 = j.Query(@"
select NS_ID, NS_Key, NS_TimeStamp, NS_Value from NodeSummary where NS_NO = {0}
", noID);

                    //NSEvent("Summary process for: " + row["NO_Name"].ToString());

                    Dictionary<string, Tuple<DateTime, string, string>> current = new Dictionary<string, Tuple<DateTime, string, string>>();

                    foreach (Row row2 in result2)
                    {
                        string key = row2["NS_Key"].ToString();
                        if (!current.ContainsKey(key))
                            current.Add(key, new Tuple<DateTime, string, string>(
                                row2["NS_TimeStamp"].ToDateTime(),
                                row2["NS_Value"].ToString(),
                                row2["NS_ID"].ToString()));
                    }

                    if (noType == "P")
                    {
                        SummaryEntry("INTERFACE_COUNT", current, noID, batch, TimeSpan.FromMinutes(20), delegate() 
                        {
                            Result r = j.Query("select count(*) from PEInterface where PI_NO = {0} and PI_Name not like '%.%'", noID);
                            return r[0][0].ToInt() + "";
                        });
                        SummaryEntry("INTERFACE_COUNT_UP", current, noID, batch, TimeSpan.FromMinutes(20), delegate()
                        {
                            Result r = j.Query("select count(*) from PEInterface where PI_NO = {0} and PI_Name not like '%.%' and PI_Status = 1", noID);
                            return r[0][0].ToInt() + "";
                        });
                        SummaryEntry("INTERFACE_COUNT_HU", current, noID, batch, TimeSpan.FromMinutes(20), delegate()
                        {
                            Result r = j.Query("select count(*) from PEInterface where PI_NO = {0} and PI_Name not like '%.%' and PI_Name like 'Hu%'", noID);
                            return r[0][0].ToInt() + "";
                        });
                        SummaryEntry("INTERFACE_COUNT_HU_UP", current, noID, batch, TimeSpan.FromMinutes(20), delegate()
                        {
                            Result r = j.Query("select count(*) from PEInterface where PI_NO = {0} and PI_Name not like '%.%' and PI_Name like 'Hu%' and PI_Status = 1", noID);
                            return r[0][0].ToInt() + "";
                        });
                        SummaryEntry("INTERFACE_COUNT_TE", current, noID, batch, TimeSpan.FromMinutes(20), delegate()
                        {
                            Result r = j.Query("select count(*) from PEInterface where PI_NO = {0} and PI_Name not like '%.%' and PI_Name like 'Te%'", noID);
                            return r[0][0].ToInt() + "";
                        });
                        SummaryEntry("INTERFACE_COUNT_TE_UP", current, noID, batch, TimeSpan.FromMinutes(20), delegate()
                        {
                            Result r = j.Query("select count(*) from PEInterface where PI_NO = {0} and PI_Name not like '%.%' and PI_Name like 'Te%' and PI_Status = 1", noID);
                            return r[0][0].ToInt() + "";
                        });
                        SummaryEntry("INTERFACE_COUNT_GI", current, noID, batch, TimeSpan.FromMinutes(20), delegate()
                        {
                            Result r = j.Query("select count(*) from PEInterface where PI_NO = {0} and PI_Name not like '%.%' and PI_Name like 'Gi%'", noID);
                            return r[0][0].ToInt() + "";
                        });
                        SummaryEntry("INTERFACE_COUNT_GI_UP", current, noID, batch, TimeSpan.FromMinutes(20), delegate()
                        {
                            Result r = j.Query("select count(*) from PEInterface where PI_NO = {0} and PI_Name not like '%.%' and PI_Name like 'Gi%' and PI_Status = 1", noID);
                            return r[0][0].ToInt() + "";
                        });
                        SummaryEntry("INTERFACE_COUNT_FA", current, noID, batch, TimeSpan.FromMinutes(20), delegate()
                        {
                            Result r = j.Query("select count(*) from PEInterface where PI_NO = {0} and PI_Name not like '%.%' and PI_Name like 'Fa%'", noID);
                            return r[0][0].ToInt() + "";
                        });
                        SummaryEntry("INTERFACE_COUNT_FA_UP", current, noID, batch, TimeSpan.FromMinutes(20), delegate()
                        {
                            Result r = j.Query("select count(*) from PEInterface where PI_NO = {0} and PI_Name not like '%.%' and PI_Name like 'Fa%' and PI_Status = 1", noID);
                            return r[0][0].ToInt() + "";
                        });
                        SummaryEntry("INTERFACE_COUNT_SE", current, noID, batch, TimeSpan.FromMinutes(20), delegate()
                        {
                            Result r = j.Query("select count(*) from PEInterface where PI_NO = {0} and PI_Name not like '%.%' and PI_Name like 'Se%'", noID);
                            return r[0][0].ToInt() + "";
                        });
                        SummaryEntry("INTERFACE_COUNT_SE_UP", current, noID, batch, TimeSpan.FromMinutes(20), delegate()
                        {
                            Result r = j.Query("select count(*) from PEInterface where PI_NO = {0} and PI_Name not like '%.%' and PI_Name like 'Se%' and PI_Status = 1", noID);
                            return r[0][0].ToInt() + "";
                        });
                        SummaryEntry("SUBINTERFACE_COUNT", current, noID, batch, TimeSpan.FromMinutes(10), delegate()
                        {
                            Result r = j.Query("select count(*) from PEInterface where PI_NO = {0} and PI_Name like '%.%'", noID);
                            return r[0][0].ToInt() + "";
                        });
                        SummaryEntry("SUBINTERFACE_COUNT_UP", current, noID, batch, TimeSpan.FromMinutes(10), delegate()
                        {
                            Result r = j.Query("select count(*) from PEInterface where PI_NO = {0} and PI_Name like '%.%' and PI_Status = 1", noID);
                            return r[0][0].ToInt() + "";
                        });
                        SummaryEntry("SUBINTERFACE_COUNT_UP_UP", current, noID, batch, TimeSpan.FromMinutes(10), delegate()
                        {
                            Result r = j.Query("select count(*) from PEInterface where PI_NO = {0} and PI_Name like '%.%' and PI_Status = 1 and PI_Protocol = 1", noID);
                            return r[0][0].ToInt() + "";
                        });
                        SummaryEntry("SUBINTERFACE_COUNT_HU", current, noID, batch, TimeSpan.FromMinutes(10), delegate()
                        {
                            Result r = j.Query("select count(*) from PEInterface where PI_NO = {0} and PI_Name like 'Hu%.%'", noID);
                            return r[0][0].ToInt() + "";
                        });
                        SummaryEntry("SUBINTERFACE_COUNT_HU_UP", current, noID, batch, TimeSpan.FromMinutes(10), delegate()
                        {
                            Result r = j.Query("select count(*) from PEInterface where PI_NO = {0} and PI_Name like 'Hu%.%' and PI_Status = 1", noID);
                            return r[0][0].ToInt() + "";
                        });
                        SummaryEntry("SUBINTERFACE_COUNT_HU_UP_UP", current, noID, batch, TimeSpan.FromMinutes(10), delegate()
                        {
                            Result r = j.Query("select count(*) from PEInterface where PI_NO = {0} and PI_Name like 'Hu%.%' and PI_Status = 1 and PI_Protocol = 1", noID);
                            return r[0][0].ToInt() + "";
                        });
                        SummaryEntry("SUBINTERFACE_COUNT_TE", current, noID, batch, TimeSpan.FromMinutes(10), delegate()
                        {
                            Result r = j.Query("select count(*) from PEInterface where PI_NO = {0} and PI_Name like 'Te%.%'", noID);
                            return r[0][0].ToInt() + "";
                        });
                        SummaryEntry("SUBINTERFACE_COUNT_TE_UP", current, noID, batch, TimeSpan.FromMinutes(10), delegate()
                        {
                            Result r = j.Query("select count(*) from PEInterface where PI_NO = {0} and PI_Name like 'Te%.%' and PI_Status = 1", noID);
                            return r[0][0].ToInt() + "";
                        });
                        SummaryEntry("SUBINTERFACE_COUNT_TE_UP_UP", current, noID, batch, TimeSpan.FromMinutes(10), delegate()
                        {
                            Result r = j.Query("select count(*) from PEInterface where PI_NO = {0} and PI_Name like 'Te%.%' and PI_Status = 1 and PI_Protocol = 1", noID);
                            return r[0][0].ToInt() + "";
                        });
                        SummaryEntry("SUBINTERFACE_COUNT_GI", current, noID, batch, TimeSpan.FromMinutes(10), delegate()
                        {
                            Result r = j.Query("select count(*) from PEInterface where PI_NO = {0} and PI_Name like 'Gi%.%'", noID);
                            return r[0][0].ToInt() + "";
                        });
                        SummaryEntry("SUBINTERFACE_COUNT_GI_UP", current, noID, batch, TimeSpan.FromMinutes(10), delegate()
                        {
                            Result r = j.Query("select count(*) from PEInterface where PI_NO = {0} and PI_Name like 'Gi%.%' and PI_Status = 1", noID);
                            return r[0][0].ToInt() + "";
                        });
                        SummaryEntry("SUBINTERFACE_COUNT_GI_UP_UP", current, noID, batch, TimeSpan.FromMinutes(10), delegate()
                        {
                            Result r = j.Query("select count(*) from PEInterface where PI_NO = {0} and PI_Name like 'Gi%.%' and PI_Status = 1 and PI_Protocol = 1", noID);
                            return r[0][0].ToInt() + "";
                        });
                        SummaryEntry("SUBINTERFACE_COUNT_FA", current, noID, batch, TimeSpan.FromMinutes(10), delegate()
                        {
                            Result r = j.Query("select count(*) from PEInterface where PI_NO = {0} and PI_Name like 'Fa%.%'", noID);
                            return r[0][0].ToInt() + "";
                        });
                        SummaryEntry("SUBINTERFACE_COUNT_FA_UP", current, noID, batch, TimeSpan.FromMinutes(10), delegate()
                        {
                            Result r = j.Query("select count(*) from PEInterface where PI_NO = {0} and PI_Name like 'Fa%.%' and PI_Status = 1", noID);
                            return r[0][0].ToInt() + "";
                        });
                        SummaryEntry("SUBINTERFACE_COUNT_FA_UP_UP", current, noID, batch, TimeSpan.FromMinutes(10), delegate()
                        {
                            Result r = j.Query("select count(*) from PEInterface where PI_NO = {0} and PI_Name like 'Fa%.%' and PI_Status = 1 and PI_Protocol = 1", noID);
                            return r[0][0].ToInt() + "";
                        });
                        SummaryEntry("VRF_COUNT", current, noID, batch, TimeSpan.FromMinutes(60), delegate()
                        {
                            Result r = j.Query("select count(*) from PERouteName where PN_NO = {0}", noID);
                            return r[0][0].ToInt() + "";
                        });
                        SummaryEntry("QOS_COUNT", current, noID, batch, TimeSpan.FromMinutes(60), delegate()
                        {
                            Result r = j.Query("select count(*) from PEQOS where PQ_NO = {0}", noID);
                            return r[0][0].ToInt() + "";
                        });
                    }

                    commitAffected += batch.Commit().AffectedRows;

                    //Thread.Sleep(20);
                }

                if (commitAffected == 0)
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
                    Event("Updated " + commitAffected + " entries");
                    Thread.Sleep(5000);
                }
            }

            Event("Stopped");
        }

        private static void SummaryEntry(string key, Dictionary<string, Tuple<DateTime, string, string>> current, string nodeID, Batch batch, TimeSpan expireTime, SummaryCallback summary)
        {
            DateTime timestamp = DateTime.Now;

            if (current.ContainsKey(key))
            {
                Tuple<DateTime, string, string> item = current[key];
                DateTime oldtimestamp = item.Item1;

                if ((timestamp - oldtimestamp) > expireTime)
                {
                    string value = summary();
                    string oldvalue = item.Item2;
                    string id = item.Item3;

                    current[key] = new Tuple<DateTime, string, string>(timestamp, value, id);

                    if (oldvalue != value)
                    {
                        batch.Execute("update NodeSummary set NS_Value = {1}, NS_TimeStamp = {2} where NS_ID = {0}",
                            id, value, timestamp);
                    }
                    else
                    {
                        batch.Execute("update NodeSummary set NS_TimeStamp = {1} where NS_ID = {0}",
                            id, timestamp);
                    }

                }
            }
            else
            {
                string id = Database.ID();
                string value = summary();
                current.Add(key, new Tuple<DateTime, string, string>(timestamp, value, id));

                batch.Execute("insert into NodeSummary(NS_ID, NS_NO, NS_Key, NS_TimeStamp, NS_Value) values({0}, {1}, {2}, {3}, {4})",
                    id, nodeID, key, timestamp, value);
            }
        }

        #endregion
    }
}
