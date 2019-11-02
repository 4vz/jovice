using System;
using System.Collections.Generic;
using Oracle.ManagedDataAccess.Client;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Aphysoft.Share;
using System.Data;

using Aveezo;

namespace Aphysoft.Share
{
    public class OracleDatabaseConnection : DatabaseConnection
    {
        #region Fields

        private List<OracleCommand> commands = new List<OracleCommand>();

        #endregion

        #region Constructor

        public OracleDatabaseConnection(string connectionString) : base(connectionString)
        {
        }

        #endregion

        #region Methods

        public override bool IsConnected()
        {
            bool connected = false;

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                try
                {
                    connection.Open();

                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                        connection.Dispose();
                        connected = true;
                    }
                }
                catch (Exception ex)
                {
                    database.OnException(ex, null);
                }
            }

            return connected;
        }

        public override void InitializeDatabase()
        {
            Result r = Query("SELECT table_name FROM all_tables");

            List<string> tables = new List<string>();

            foreach (Row row in r)
            {
                tables.Add(row["TABLE_NAME"].ToString());
            }

            database.UpdateDatabaseTables(tables.ToArray());
        }

        public override DatabaseExceptionType ParseMessage(string message)
        {
            //if (message.IndexOf("login failed") > -1) return DatabaseException.LoginFailed;
            //else if (message.IndexOf("timeout period elapsed") > -1) return DatabaseException.Timeout;
            return DatabaseExceptionType.None;
        }

        public override string Escape(string str)
        {
            return str.Replace("'", @"\'");
        }

        public override string Format(DateTime dateTime)
        {
            return dateTime.ToString("yyyy/MM/dd HH:mm:ss.fff");
        }

        private OracleCommand Begin(string sql, OracleConnection connection)
        {
            OracleCommand command = null;

            try
            {
                connection.Open();
                command = new OracleCommand(sql, connection);
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

        private void End(OracleConnection connection, OracleCommand command)
        {
            connection.Close();
            connection.Dispose();
            command.Dispose();
        }

        public override Result Query(string sql)
        {
            Result result = new Result(sql);
            int attempts = database.QueryAttempts;

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                OracleCommand command = Begin(sql, connection);

                if (command != null)
                {

                    lock (commands)
                    {
                        commands.Add(command);
                    }

                    bool doBreak = false;
                    for (int attempt = 0; attempt < attempts; attempt++)
                    {
                        OracleDataReader reader = null;
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
                                Row row = new Row();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    string name = names[i];
                                    bool isNull = reader.IsDBNull(i);
                                    object value = reader.GetValue(i);
                                    row.Add(name, new Column(value, isNull));
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

        public override Column Scalar(string sql)
        {
            Column column = null;
            int attempts = database.QueryAttempts;

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                OracleCommand command = Begin(sql, connection);

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
                            if (data != null) column = new Column(data, false);
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

        protected override Result Execute(string sql, bool identity)
        {
            Result result = new Result(sql);
            int attempts = database.QueryAttempts;

            using (OracleConnection connection = new OracleConnection(ConnectionString))
            {
                OracleCommand command = Begin(sql, connection);

                if (command != null)
                {
                    lock (commands)
                    {
                        commands.Add(command);
                    }

                    bool doBreak = false;
                    for (int attempt = 0; attempt < attempts; attempt++)
                    {
                        OracleCommand identityCommand = null;
                        try
                        {
                            stopwatch.Restart();
                            result.AffectedRows = command.ExecuteNonQuery();
                            stopwatch.Stop();
                            result.ExecutionTime = stopwatch.Elapsed;

                            if (identity)
                            {
                                // not implemented

                                //identityCommand = new OracleCommand("select cast(SCOPE_IDENTITY() as bigint)", connection);
                                //lock (commands)
                                //{
                                //    commands.Add(identityCommand);
                                //}

                                //result.Identity = (Int64)identityCommand.ExecuteScalar();
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
                foreach (OracleCommand command in commands)
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
