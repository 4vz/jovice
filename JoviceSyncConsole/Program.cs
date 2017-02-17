using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Center;
using Aphysoft.Share;
using System.Threading;

namespace JoviceSyncConsole
{
    class Program
    {
        private static void Event(string message)
        {
            Console.WriteLine(message);
        }
        static void Main(string[] args)
        {
            Event("JoviceSyncConsole");

            Database local = Jovice.Database;
            local.Exception += Local_Exception;
            Database remote = new Database("Data Source=telkom.center;Initial Catalog=jovice;User ID=telkom.center;Password=t3lk0mdotc3nt3r;async=true", DatabaseType.SqlServer);

            bool ok = true;
            if (local.Test()) Event("Local database OK");
            else
            {
                Event("Local database Not OK");
                ok = false;
            }
            if (remote.Test()) Event("Remote database OK");
            else
            {
                Event("Remote database Not OK");
                ok = false;
            }


            Result result;
            Batch batch = local.Batch();

            while (ok)
            {
                Console.Write("Enter the table you want to sync (exit to end application): ");
                string table = Console.ReadLine();

                if (table != "exit")
                {
                    result = local.Query("select top 1 * from " + table);
                    if (result.Count == 0) result = remote.Query("select top 1 * from " + table);
                    if (result.Count > 0)
                    {
                        Console.Write("Enter the column name of the primary key: ");
                        string pk = Console.ReadLine();

                        Event("Synchronizing...");

                        Dictionary<string, Row> locald = local.QueryDictionary("select * from " + table, pk);
                        Dictionary<string, Row> remoted = remote.QueryDictionary("select * from " + table, pk);
                        
                        batch.Begin();
                        foreach (KeyValuePair<string, Row> pair in remoted)
                        {
                            if (!locald.ContainsKey(pair.Key))
                            {
                                // new
                                Insert insert = local.Insert(table);
                                foreach (KeyValuePair<string, Column> pair2 in pair.Value)
                                {
                                    insert.Value(pair2.Key, pair2.Value.ToObject());
                                }
                                batch.Execute(insert);
                            }
                            else
                            {
                                // updated
                                Row localrow = locald[pair.Key];
                                Update update = local.Update(table);
                                update.Where(pk, pair.Key);
                                foreach (KeyValuePair<string, Column> pair2 in pair.Value)
                                {
                                    if (pair2.Key != pk)
                                    {
                                        if (pair2.Value.ToObject() != localrow[pair2.Key].ToObject())
                                        {
                                            update.Set(pair2.Key, pair2.Value.ToObject());
                                        }
                                    }
                                }
                                batch.Execute(update);
                            }
                        }
                        foreach (KeyValuePair<string, Row> pair in locald)
                        {
                            if (!remoted.ContainsKey(pair.Key))
                            {
                                batch.Execute("delete from " + table + " where " + pk + " = {0}", pair.Key);
                            }
                        }
                        batch.Commit();

                        Event("Synched");

                    }
                    else Event("Local and remote table of " + table + " are empty.");
                }
                else break;                
            }

            Event("End of Application");
            Thread.Sleep(2000);
        }

        private static void Local_Exception(object sender, DatabaseExceptionEventArgs eventArgs)
        {
            Event(eventArgs.Message + " SQL:" + eventArgs.Sql);
        }
    }
}
