using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Aphysoft.Share;

namespace Aphysoft.Share
{
    public class MySqlDatabaseConnection : DatabaseConnection2
    {
        #region Fields

        private List<MySqlCommand> commands = new List<MySqlCommand>();

        #endregion

        #region Constructor

        public MySqlDatabaseConnection(string connectionString) : base(connectionString)
        {
        }

        #endregion

        #region Methods

        public override void InitializeDatabase()
        {
            Result2 r = Query("select table_name FROM information_schema.tables where table_type = 'BASE TABLE' and table_schema = database()");

            List<string> tables = new List<string>();

            foreach (Row2 row in r)
            {
                tables.Add(row["table_name"].ToString());
            }

            database.UpdateDatabaseTables(tables.ToArray());
        }

        public override DatabaseExceptionType2 ParseMessage(string message)
        {
            //if (message.IndexOf("login failed") > -1) return DatabaseException.LoginFailed;
            //else if (message.IndexOf("timeout period elapsed") > -1) return DatabaseException.Timeout;
            return DatabaseExceptionType2.None;
        }


        public override string Escape(string str)
        {
            return str.Replace("'", "\'");
        }

        public override string Format(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        private MySqlCommand Begin(string sql, MySqlConnection connection)
        {
            MySqlCommand command = null;

            try
            {
                connection.Open();
                command = new MySqlCommand(sql, connection);
                if (database.Timeout > -1)
                    command.CommandTimeout = database.Timeout;
            }
            catch (Exception ex)
            {
                database.OnException(ex, sql);
                command = null;
            }

            return command;
        }

        private void End(MySqlConnection connection, MySqlCommand command)
        {
            connection.Close();
            connection.Dispose();
            command.Dispose();
        }

        public override Result2 Query(string sql)
        {
            Result2 result = new Result2(sql);
            int attempts = database.QueryAttempts;

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                MySqlCommand command = Begin(sql, connection);

                if (command != null)
                {

                    lock (commands)
                    {
                        commands.Add(command);
                    }

                    bool doBreak = false;
                    for (int attempt = 0; attempt < attempts; attempt++)
                    {
                        MySqlDataReader reader = null;
                        try
                        {
                            stopwatch.Restart();
                            reader = command.ExecuteReader();
                            stopwatch.Stop();
                            result.ExecutionTime = stopwatch.Elapsed;

                            List<string> names = new List<string>();
                            for (int i = 0; i < reader.FieldCount; i++) names.Add(reader.GetName(i));
                            result.ColumnNames = names.ToArray();

                            result.Clear();
                            while (reader.Read())
                            {
                                Row2 row = new Row2();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    string name = names[i];
                                    bool isNull = reader.IsDBNull(i);
                                    object value = reader.GetValue(i);
                                    row.Add(name, new Column2(value, isNull));
                                }
                                result.Add(row);
                                if (result.Count >= 200000) break;
                            }

                            doBreak = true;
                        }
                        catch (Exception e)
                        {
                            if (!cancelling)
                            {
                                if (attempt == attempts - 1)
                                {
                                    result.IsExceptionThrown = true;
                                    database.OnException(e, sql);
                                }
                                else if (database.OnRetry(e, sql))
                                {
                                    result.IsExceptionThrown = true;
                                    database.OnException(e, sql);
                                    doBreak = true;
                                    break;
                                }

                            }
                            else doBreak = true;
                        }
                        finally { if (reader != null) reader.Close(); }
                        if (doBreak) break;
                    }

                    End(connection, command);
                    lock (commands)
                    {
                        if (commands.Contains(command)) commands.Remove(command);
                    }

                }
                else
                    result.IsExceptionThrown = true;
            }
            return result;
        }

        public override Column2 Scalar(string sql)
        {
            Column2 column = null;
            int attempts = database.QueryAttempts;

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                MySqlCommand command = Begin(sql, connection);

                if (command != null)
                {

                    lock (commands)
                    {
                        commands.Add(command);
                    }

                    bool doBreak = false;
                    for (int attempt = 0; attempt < attempts; attempt++)
                    {
                        try
                        {
                            object data = command.ExecuteScalar();
                            if (data != null) column = new Column2(data, false);
                            doBreak = true;
                        }
                        catch (Exception e)
                        {
                            if (!cancelling)
                            {
                                if (attempt == attempts - 1)
                                {
                                    database.OnException(e, sql);
                                }
                                else if (database.OnRetry(e, sql))
                                {
                                    database.OnException(e, sql);
                                    doBreak = true;
                                    break;
                                }
                            }
                            else doBreak = true;
                        }
                        if (doBreak) break;
                    }

                    End(connection, command);
                    lock (commands)
                    {
                        if (commands.Contains(command)) commands.Remove(command);
                    }
                }
                else
                { }
            }
            return column;
        }

        protected override Result2 Execute(string sql, bool identity)
        {
            Result2 result = new Result2(sql);
            int attempts = database.QueryAttempts;

            using (MySqlConnection connection = new MySqlConnection(ConnectionString))
            {
                MySqlCommand command = Begin(sql, connection);

                if (command != null)
                {
                    lock (commands)
                    {
                        commands.Add(command);
                    }

                    bool doBreak = false;
                    for (int attempt = 0; attempt < attempts; attempt++)
                    {
                        MySqlCommand identityCommand = null;
                        try
                        {
                            stopwatch.Restart();
                            result.AffectedRows = command.ExecuteNonQuery();
                            stopwatch.Stop();
                            result.ExecutionTime = stopwatch.Elapsed;

                            if (identity)
                            {
                                identityCommand = new MySqlCommand("SELECT LAST_INSERT_ID()", connection);
                                lock (commands)
                                {
                                    commands.Add(identityCommand);
                                }

                                result.Identity = (Int64)identityCommand.ExecuteScalar();
                            }
                            doBreak = true;
                        }
                        catch (Exception e)
                        {
                            if (!cancelling)
                            {
                                if (attempt == attempts - 1)
                                {
                                    result.IsExceptionThrown = true;
                                    database.OnException(e, sql);
                                }
                                else if (database.OnRetry(e, sql))
                                {
                                    result.IsExceptionThrown = true;
                                    database.OnException(e, sql);
                                    doBreak = true;
                                    break;
                                }
                            }
                            else doBreak = true;
                        }
                        finally
                        {
                            if (identityCommand != null)
                            {
                                identityCommand.Dispose();
                                lock (commands)
                                {
                                    if (commands.Contains(command)) commands.Remove(identityCommand);
                                }
                            }
                        }
                        if (doBreak) break;
                    }

                    End(connection, command);
                    lock (commands)
                    {
                        if (commands.Contains(command)) commands.Remove(command);
                    }
                }
                else
                    result.IsExceptionThrown = true;
            }

            return result;
        }

        public override int Cancel()
        {
            cancelling = true;

            int nc = commands.Count;

            lock (commands)
            {
                foreach (MySqlCommand command in commands)
                {
                    command.Cancel();
                }
                commands.Clear();
            }

            cancelling = false;

            return nc;
        }

        #endregion
    }
}
