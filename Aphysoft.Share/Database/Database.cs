﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Oracle.DataAccess.Client;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;

namespace Aphysoft.Share
{
    public class DatabaseExceptionEventArgs : EventArgs
    {
        #region Fields

        private string sql;

        public string Sql
        {
            get { return sql; }
            set { sql = value; }
        }
        
        private string message;

        public string Message
        {
            get { return message; }
            set { message = value; }
        }

        private DatabaseException exception;

        public DatabaseException Exception
        {
            get { return exception; }
            set { exception = value; }
        }

        private bool noRetry = false;

        public bool NoRetry
        {
            get { return noRetry; }
            set { noRetry = value; }
        }

        #endregion

        #region Constructor

        public DatabaseExceptionEventArgs()
        {

        }

        #endregion
    }

    public delegate void DatabaseExceptionEventHandler(object sender, DatabaseExceptionEventArgs eventArgs);

    public delegate void DatabaseRetryEventHandler(object sender, DatabaseExceptionEventArgs eventArgs);

    public delegate string QueryDictionaryKeyCallback(Row row);

    public delegate void QueryDictionaryDuplicateCallback(Row row);

    public class Batch
    {
        #region Fields

        private List<string> lines = new List<string>();

        private Database database = null;

        public int Count
        {
            get { return lines.Count; }
        }

        #endregion

        #region Constructors

        internal Batch(Database database)
        {
            this.database = database;
        }

        #endregion

        #region Methods

        public void Begin()
        {
            lines.Clear();
        }

        public void Execute(string sql, params object[] args)
        {
            if (!string.IsNullOrEmpty(sql))
            {
                string line = database.Format(sql, args);
                lines.Add(line);
            }
        }

        public void Execute(Insert insert)
        {
            if (!insert.IsEmpty)
                lines.Add(insert.ToString());
        }

        public void Execute(Update update)
        {
            if (!update.IsEmpty)
                lines.Add(update.ToString());
        }

        public Result Commit()
        {
            int count = lines.Count;
            Result result = new Result(null);

            Stopwatch elapsed = new Stopwatch();
            elapsed.Restart();

            if (count > 0)
            {
                int index = 0;
                StringBuilder batch = new StringBuilder();

                bool ok = true;
                while (index < count)
                {
                    string line = lines[index];
                    batch.Append(line + ";\r\n");
                    index++;

                    if (index % 25 == 0)
                    {
                        Result currentResult = database.Execute(batch.ToString());
                        if (!currentResult.OK)
                        {
                            result.isExceptionThrown = true;
                            ok = false;
                            break;
                        }
                        else
                        {
                            batch.Clear();
                            result.AffectedRows = result.AffectedRows + currentResult.AffectedRows;
                        }
                    }
                }

                if (ok && batch.Length > 0)
                {
                    Result currentResult = database.Execute(batch.ToString());
                    if (!currentResult.OK)
                        result.isExceptionThrown = true;
                    else
                        result.AffectedRows = result.AffectedRows + currentResult.AffectedRows;
                }
            }

            result.ExecutionTime = elapsed.Elapsed;

            return result;
        }

        #endregion
    }

    public abstract class DMLBase
    {
        #region Fields

        protected Database database;

        protected string table;

        protected string where = null;

        protected Dictionary<string, object> objects = null;

        public bool IsEmpty
        {
            get { return objects == null || objects.Count == 0; }
        }

        #endregion

        #region Constructors

        public DMLBase(string table, Database database)
        {
            this.table = table;
            this.database = database;
            objects = new Dictionary<string, object>();
        }

        #endregion

        #region Methods

        protected void Object(string key, object value)
        {
            objects.Add(key, value);
        }

        #endregion
    }

    public class Insert : DMLBase
    {
        #region Constructors

        internal Insert(string table, Database database) : base(table, database)
        {
        }

        #endregion

        #region Methods

        public void Value(string key, object value)
        {
            Object(key, value);
        }

        public override string ToString()
        {
            if (objects.Count == 0) return "";

            StringBuilder sb = new StringBuilder();
            sb.Append("insert into ");
            sb.Append(table);
            sb.Append("(");
            int counter = 0;
            foreach (KeyValuePair<string, object> pair in objects)
            {
                sb.Append(pair.Key);                
                if (counter < objects.Count - 1) sb.Append(", ");
                counter++;
            }
            sb.Append(") values(");
            counter = 0;
            foreach (KeyValuePair<string, object> pair in objects)
            {
                sb.Append(database.Format("{0}", pair.Value));
                if (counter < objects.Count - 1) sb.Append(", ");
                counter++;
            }
            sb.Append(")");

            return sb.ToString();
        }

        public void Execute()
        {
            database.Execute(this);
        }
        

        #endregion
    }

    public class Update : DMLBase
    {
        #region Constructors

        internal Update(string table, Database database) : base(table, database)
        {
        }

        #endregion

        #region Methods

        public void Set(string key, object value)
        {
            Object(key, value);
        }

        public void Set(string key, object value, bool condition)
        {
            if (condition) Set(key, value);
        }

        public void Where(string key, object value)
        {
            where = database.Format(key + " = {0}", value);
        }

        public override string ToString()
        {
            if (objects.Count == 0) return "";

            StringBuilder sb = new StringBuilder();
            sb.Append("update ");
            sb.Append(table);
            sb.Append(" set ");
            int counter = 0;
            foreach (KeyValuePair<string, object> pair in objects)
            {
                sb.Append(pair.Key);
                sb.Append(" = ");
                sb.Append(database.Format("{0}", pair.Value));
                if (counter < objects.Count - 1) sb.Append(", ");
                counter++;
            }

            if (where != null)
            {
                sb.Append(" where ");
                sb.Append(where);
            }

            return sb.ToString();
        }

        public void Execute()
        {
            database.Execute(this);
        }


        #endregion
    }

    public class Where
    {
        #region Fields

        private string value = null;

        public string Value
        {
            get { return value; }
            set { this.value = value; }
        }

        #endregion

        #region Constructors

        public Where(string value)
        {
            this.value = value;
        }
        
        #endregion

        #region Methods

        public override string ToString()
        {
            return value;
        }

        public string Format(string before, string after)
        {
            if (value != null && value.Length > 0)
            {
                return before + value + after;
            }
            else return "";
        }

        public string Format(string before)
        {
            return Format(before, "");
        }

        public string Format()
        {
            return Format("", "");
        }

        #endregion
    }

    public sealed class Database
    {
        #region Fields

        private DatabaseConnection connection;

        private DatabaseType databaseType;

        private string connectionString;

        public string ConnectionString
        {
            get { return connectionString; }
        }

        private int queryAttempts = 1;

        /// <summary>
        /// Gets or sets how many query attempts shall be done until the current execution throws exception if none success.
        /// </summary>
        public int QueryAttempts
        {
            get { return queryAttempts; }
            set { queryAttempts = value; }
        }

        private int timeout = 0;

        public int Timeout
        {
            get { return timeout; }
            set { timeout = value; }
        }
       
        #endregion

        #region Events

        public event DatabaseExceptionEventHandler Exception;

        public event DatabaseRetryEventHandler Retry;

        #endregion

        #region Constructor

        public Database(string connectionString, DatabaseType type)
        {
            this.connectionString = connectionString;
            this.databaseType = type;

            if (databaseType == DatabaseType.SqlServer)
                connection = new SqlServerDatabaseConnection(this);
            else if (databaseType == DatabaseType.Oracle)
                connection = new OracleDatabaseConnection(this);
            else
                connection = new MySqlDatabaseConnection(this);
        }

        #endregion

        #region Methods

        internal void OnException(Exception e, string sql)
        {
            if (Exception != null)
            {
                DatabaseExceptionEventArgs eventArgs = new DatabaseExceptionEventArgs();

                eventArgs.Exception = connection.ParseMessage(e.Message);
                eventArgs.Message = e.Message;
                eventArgs.Sql = sql;

                Exception(this, eventArgs);
            }
        }

        internal bool OnRetry(Exception e, string sql)
        {
            if (Retry != null)
            {
                DatabaseExceptionEventArgs eventArgs = new DatabaseExceptionEventArgs();

                eventArgs.Exception = connection.ParseMessage(e.Message);
                eventArgs.Message = e.Message;
                eventArgs.Sql = sql;

                Retry(this, eventArgs);

                if (eventArgs.NoRetry)
                {
                    return true;
                }
            }
            return false;
        }

        public Batch Batch()
        {
            return new Batch(this);
        }        

        public Insert Insert(string table)
        {
            return new Insert(table, this);
        }

        public Update Update(string table)
        {
            return new Update(table, this);
        }

        public static bool IsNumber(object value)
        {
            return value is sbyte
                    || value is byte
                    || value is short
                    || value is ushort
                    || value is int
                    || value is uint
                    || value is long
                    || value is ulong
                    || value is float
                    || value is double
                    || value is decimal;
        }

        public string Escape(string sql)
        {
            return connection.Escape(sql);
        }

        public bool Test()
        {
            return connection.Test();
        }

        public string DateTime(DateTime dateTime)
        {
            return connection.DateTime(dateTime);
        }

        public string Format(string sql, params object[] args)
        {
            if (sql == null) return null;
            if (args == null) return string.Format(sql, "NULL");
            else if (args.Length > 0)
            {
                List<string> nargs = new List<string>();

                foreach (object a in args)
                {
                    string atrs = null;

                    if (a == null) atrs = "NULL";
                    else if (a is bool) atrs = (bool)a ? "1" : "0";
                    else if (a is DateTime) atrs = "'" + DateTime((DateTime)a) + "'";
                    else if (IsNumber(a)) atrs = "" + a.ToString() + "";
                    else if (a is List<string> || a is string[])
                    {
                        StringBuilder sb = new StringBuilder();
                        
                        string[] ls;
                        if (a is string[]) ls = (string[])a;
                        else ls = ((List<string>)a).ToArray();

                        sb.Append("(");
                        if (ls.Length == 0) sb.Append("NULL");
                        else
                        {
                            int lsi = 0;
                            foreach (string lss in ls)
                            {
                                if (lsi > 0) sb.Append(", ");
                                if (lss == null) sb.Append("NULL");
                                else
                                {
                                    sb.Append("'");
                                    sb.Append(Escape(lss));
                                    sb.Append("'");
                                }
                                lsi++;
                            }
                        }
                        sb.Append(")");
                        atrs = sb.ToString();
                    }
                    else atrs = "'" + Escape(a.ToString()) + "'";
                    
                    nargs.Add(atrs);
                }

                return string.Format(sql, nargs.ToArray());
            }
            else return sql;

        }

        public Result Query(string sql, params object[] args)
        {
            string fsql = Format(sql, args);
            return connection.Query(fsql);
        }

        public Dictionary<string, Row> QueryDictionary(string sql, string key, params object[] args)
        {
            return QueryDictionary(sql, key, delegate (Row row) { }, args);
        }
        
        public Dictionary<string, Row> QueryDictionary(string sql, string key, QueryDictionaryDuplicateCallback duplicate, params object[] args)
        {
            Result result = Query(sql, args);

            if (result.OK)
            {
                Dictionary<string, Row> dictionary = new Dictionary<string, Row>();
                foreach (Row row in result)
                {
                    string dictkey = row[key].ToString();
                    if (!dictionary.ContainsKey(dictkey))
                        dictionary.Add(dictkey, row);
                    else
                        duplicate(row);
                }
                return dictionary;
            }
            else
                return null;
        }

        public Dictionary<string, Row> QueryDictionary(string sql, QueryDictionaryKeyCallback keyCreate, params object[] args)
        {
            return QueryDictionary(sql, keyCreate, delegate (Row row) { }, args);
        }

        public Dictionary<string, Row> QueryDictionary(string sql, QueryDictionaryKeyCallback keyCreate, QueryDictionaryDuplicateCallback duplicate, params object[] args)
        {
            Result result = Query(sql, args);

            if (result.OK)
            {
                Dictionary<string, Row> dictionary = new Dictionary<string, Row>();
                foreach (Row row in result)
                {
                    string key = keyCreate(row);
                    if (!dictionary.ContainsKey(key))
                        dictionary.Add(key, row);
                    else
                        duplicate(row);
                }
                return dictionary;
            }
            else
                return null;
        }

        public List<string> QueryList(string sql, string key, params object[] args)
        {
            Result result = Query(sql, args);

            if (result.OK)
            {
                List<string> list = new List<string>();
                foreach (Row row in result) list.Add(row[key].ToString());
                return list;
            }
            else
                return null;
        }

        public Column Scalar(string sql, params object[] args)
        {
            string fsql = Format(sql, args);
            return connection.Scalar(fsql);
        }

        public Result Execute(string sql, params object[] args)
        {
            string fsql = Format(sql, args);
            return connection.Execute(fsql);
        }

        public Result Execute(Insert insert)
        {
            return connection.Execute(insert.ToString());
        }

        public Result Execute(Update update)
        {
            return connection.Execute(update.ToString());
        }

        public Result ExecuteIdentity(string sql, params object[] args)
        {
            string fsql = Format(sql, args);
            return connection.ExecuteIdentity(fsql);
        }

        /// <summary>
        /// Cancels all current query/transactions.
        /// </summary>
        public int Cancel()
        {
            return connection.Cancel();
        }

        private const string idChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz`1234567890~!@#$^*()_+-=[]{}|;:,./<>?";
        private const int idCharsLen = 89;

        public static string ID()
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < 20; i++)
            {
                char r = idChars[RandomHelper.Next(idCharsLen)];
                sb.Append(r);
            }

            return sb.ToString();
        }

        #endregion
    }

    [Serializable]
    public enum DatabaseType
    {
        SqlServer,
        Oracle,
        MySql
    }

    public enum DatabaseException
    {
        Other,
        LoginFailed,
        Timeout
    }
    
    internal abstract class DatabaseConnection
    {
        #region Fields

        protected Database database;

        protected Stopwatch stopwatch;

        protected bool cancelling = false;

        #endregion

        #region Constructor

        public DatabaseConnection(Database database)
        {
            this.database = database;
            
            stopwatch = new Stopwatch();
        }

        #endregion

        #region Methods

        public virtual DatabaseException ParseMessage(string message) { return DatabaseException.Other; }

        public virtual bool Test() { return false; }

        public virtual string Escape(string str) { return str; }

        public virtual string DateTime(DateTime dateTime) { return null; }

        public virtual Result Query(string sql) { return new Result(sql); }

        public virtual Column Scalar(string sql) { return null; }

        public virtual Result Execute(string sql) { return new Result(sql); }

        public virtual Result ExecuteIdentity(string sql) { return new Result(sql); }

        public virtual int Cancel() { return 0; }

        #endregion
    }

    internal class SqlServerDatabaseConnection : DatabaseConnection
    {
        #region Fields

        private List<SqlCommand> commands = new List<SqlCommand>();

        #endregion

        #region Constructor

        public SqlServerDatabaseConnection(Database database) : base(database)
        {
        }

        #endregion

        #region Methods

        public override DatabaseException ParseMessage(string message)
        {
            if (message.IndexOf("login failed") > -1) return DatabaseException.LoginFailed;
            else if (message.IndexOf("timeout period elapsed") > -1) return DatabaseException.Timeout;
            else return DatabaseException.Other;
        }

        public override bool Test()
        {
            Result result = Query("SELECT 1");

            if (result.Count == 1 && result[0][0].ToInt() == 1)
                return true;
            else
                return false;
        }

        public override string Escape(string str)
        {
            return str.Replace("'", "''");
        }

        public override string DateTime(DateTime dateTime)
        {
            return dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        private SqlCommand Begin(string sql, SqlConnection connection)
        {
            try
            {
                connection.Open();
            }
            catch (Exception ex)
            {
                database.OnException(ex, sql);
            }

            SqlCommand command = new SqlCommand(sql, connection);
            if (database.Timeout > 0)
                command.CommandTimeout = database.Timeout;

            return command;
        }

        private void End(SqlConnection connection, SqlCommand command)
        {
            connection.Close();
            connection.Dispose();
            command.Dispose();
        }

        public override Result Query(string sql)
        {
            Result result = new Result(sql);
            int attempts = database.QueryAttempts;

            using (SqlConnection connection = new SqlConnection(database.ConnectionString))
            {
                SqlCommand command = Begin(sql, connection);
                lock (commands)
                {
                    commands.Add(command);
                }

                bool doBreak = false;
                for (int attempt = 0; attempt < attempts; attempt++)
                {
                    SqlDataReader reader = null;
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
                                row.Add(name, new Column(name, value, isNull));
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
                                result.isExceptionThrown = true;
                                database.OnException(e, sql);
                            }
                            else if (database.OnRetry(e, sql))
                            {                                
                                result.isExceptionThrown = true;
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
            return result;
        }

        public override Column Scalar(string sql)
        {
            Column column = null;
            int attempts = database.QueryAttempts;

            using (SqlConnection connection = new SqlConnection(database.ConnectionString))
            {
                SqlCommand command = Begin(sql, connection);
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
                        if (data != null) column = new Column(null, data, false);
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
            return column;
        }

        private Result Execute(string sql, bool returnIdentity)
        {
            Result result = new Result(sql);
            int attempts = database.QueryAttempts;

            using (SqlConnection connection = new SqlConnection(database.ConnectionString))
            {
                SqlCommand command = Begin(sql, connection);
                lock (commands)
                {
                    commands.Add(command);
                }

                bool doBreak = false;
                for (int attempt = 0; attempt < attempts; attempt++)
                {
                    SqlCommand identityCommand = null;
                    try
                    {
                        stopwatch.Restart();
                        result.AffectedRows = command.ExecuteNonQuery();
                        stopwatch.Stop();
                        result.ExecutionTime = stopwatch.Elapsed;

                        if (returnIdentity)
                        {
                            identityCommand = new SqlCommand("select cast(SCOPE_IDENTITY() as bigint)", connection);
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
                                result.isExceptionThrown = true;
                                database.OnException(e, sql);
                            }
                            else if (database.OnRetry(e, sql))
                            {
                                result.isExceptionThrown = true;
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

            return result;
        }

        public override Result Execute(string sql)
        {
            return Execute(sql, false);
        }

        public override Result ExecuteIdentity(string sql)
        {
            return Execute(sql, true);
        }

        public override int Cancel()
        {
            cancelling = true;

            int nc = commands.Count;

            lock (commands)
            {
                foreach (SqlCommand command in commands)
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

    internal class OracleDatabaseConnection : DatabaseConnection
    {
        #region Constructor

        public OracleDatabaseConnection(Database database) : base(database)
        {
        }

        #endregion

        #region Methods

        public override string Escape(string str)
        {
            return str.Replace("'", @"\'");
        }

        public override Result Query(string sql)
        {
            Result result = new Result(sql);

            using (OracleConnection connection = new OracleConnection(database.ConnectionString))
            {
                try
                {
                    connection.Open();
                    stopwatch.Restart();

                    OracleCommand command = new OracleCommand(sql, connection);

                    command.CommandTimeout = 120;

                    OracleDataReader reader = command.ExecuteReader();

                    stopwatch.Stop();

                    result.ExecutionTime = stopwatch.Elapsed;

                    command.Dispose();

                    while (reader.Read())
                    {
                        Row row = new Row();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string name = reader.GetName(i);
                            bool isNull = reader.IsDBNull(i);
                            object value = reader.GetValue(i);
                            row.Add(name, new Column(name, value, isNull));
                        }
                        result.Add(row);

                        if (result.Count >= 50000) break;
                    }

                    reader.Close();
                }
                catch (Exception e)
                {
                    result.isExceptionThrown = true;
                }
                finally
                {
                    if (connection != null) connection.Close();
                }
            }

            return result;
        }

        public override Column Scalar(string sql)
        {
            Column column = null;

            using (OracleConnection connection = new OracleConnection(database.ConnectionString))
            {
                try
                {
                    connection.Open();

                    OracleCommand command = new OracleCommand(sql, connection);
                    object reader = command.ExecuteScalar();

                    command.Dispose();

                    if (reader != null)
                        column = new Column(null, reader, false);
                }
                catch (Exception e)
                {
                }
                finally
                {
                    if (connection != null) connection.Close();
                }
            }

            return column;
        }

        public override Result Execute(string sql)
        {
            Result result = new Result(sql);

            using (OracleConnection connection = new OracleConnection(database.ConnectionString))
            {
                try
                {
                    connection.Open();

                    stopwatch.Restart();

                    OracleCommand command = new OracleCommand(sql, connection);
                    result.AffectedRows = command.ExecuteNonQuery();

                    stopwatch.Stop();

                    result.ExecutionTime = stopwatch.Elapsed;

                    command.Dispose();
                }
                catch (Exception e)
                {
                    result.isExceptionThrown = true;
                }
                finally
                {
                    if (connection != null) connection.Close();
                }
            }

            return result;
        }

        #endregion
    }

    internal class MySqlDatabaseConnection : DatabaseConnection
    {
        #region Constructor

        public MySqlDatabaseConnection(Database database) : base(database)
        {
        }

        #endregion

        #region Methods

        public void Exception(Exception e, string sql)
        {
            string message = e.Message;
        }

        public override string Escape(string str)
        {
            return str.Replace("'", "\'");
        }

        public override Result Query(string sql)
        {
            Result result = new Result(sql);

            using (MySqlConnection connection = new MySqlConnection(database.ConnectionString))
            {
                try
                {
                    connection.Open();
                    stopwatch.Restart();

                    MySqlCommand command = new MySqlCommand(sql, connection);

                    command.CommandTimeout = 120;

                    MySqlDataReader reader = command.ExecuteReader();

                    stopwatch.Stop();
                    
                    result.ExecutionTime = stopwatch.Elapsed;

                    command.Dispose();

                    while (reader.Read())
                    {
                        Row row = new Row();
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            string name = reader.GetName(i);
                            bool isNull = reader.IsDBNull(i);
                            object value = reader.GetValue(i);
                            row.Add(name, new Column(name, value, isNull));
                        }
                        result.Add(row);

                        if (result.Count >= 50000) break;
                    }

                    reader.Close();
                }
                catch (Exception e)
                {
                    result.isExceptionThrown = true;
                    Exception(e, sql);
                }
                finally
                {
                    if (connection != null) connection.Close();
                }
            }

            return result;
        }

        public override Column Scalar(string sql)
        {
            Column column = null;

            using (MySqlConnection connection = new MySqlConnection(database.ConnectionString))
            {
                try
                {
                    connection.Open();

                    MySqlCommand command = new MySqlCommand(sql, connection);
                    object reader = command.ExecuteScalar();

                    command.Dispose();

                    if (reader != null)
                        column = new Column(null, reader, false);
                }
                catch (Exception e)
                {
                    Exception(e, sql);
                }
                finally
                {
                    if (connection != null) connection.Close();
                }
            }

            return column;
        }

        public override Result Execute(string sql)
        {
            Result result = new Result(sql);

            using (MySqlConnection connection = new MySqlConnection(database.ConnectionString))
            {
                try
                {
                    connection.Open();

                    stopwatch.Restart();

                    MySqlCommand command = new MySqlCommand(sql, connection);
                    result.AffectedRows = command.ExecuteNonQuery();

                    stopwatch.Stop();

                    result.ExecutionTime = stopwatch.Elapsed;

                    command.Dispose();
                }
                catch (Exception e)
                {
                    result.isExceptionThrown = true;
                    Exception(e, sql);
                }
                finally
                {
                    if (connection != null) connection.Close();
                }
            }

            return result;
        }

        #endregion
    }

    [Serializable]
    public sealed class Result : IList<Row>
    {
        #region Fields

        internal bool isExceptionThrown = false;

        public bool OK
        {
            get { return !isExceptionThrown; }
        }

        private List<Row> rows;

        private TimeSpan executionTime;

        public TimeSpan ExecutionTime
        {
            get { return executionTime; }
            internal set { executionTime = value; }
        }

        private int affectedRows = 0;

        public int AffectedRows
        {
            get { return affectedRows; }
            internal set { affectedRows = value; }
        }

        private Int64 identity;

        public Int64 Identity
        {
            get { return identity; }
            internal set { identity = value; }
        }

        private string[] columnNames;

        public string[] ColumnNames
        {
            get { return columnNames; }
            internal set { columnNames = value; }
        }

        private string sql;

        public string Sql
        {
            get { return sql; }
            internal set { sql = value; }
        }

        #endregion

        #region Constructor

        public Result(string sql)
        {
            rows = new List<Row>();
            this.sql = sql;
        }

        #endregion

        #region Methods

        public int IndexOf(Row item)
        {
            return rows.IndexOf(item);
        }

        public void Insert(int index, Row item)
        {
            rows.Insert(index, item);
        }

        public void RemoveAt(int index)
        {
            rows.RemoveAt(index);
        }

        public Row this[int index]
        {
            get
            {
                if (isExceptionThrown)
                {
                    throw new Exception("Warning: This result contain exceptions");
                }
                return rows[index];
            }
            set { }
        }

        public void Add(Row item)
        {
            rows.Add(item);
        }

        public void Clear()
        {
            rows.Clear();
        }

        public bool Contains(Row item)
        {
            return rows.Contains(item);
        }

        public void CopyTo(Row[] array, int arrayIndex)
        {
            rows.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return rows.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(Row item)
        {
            return rows.Remove(item);
        }

        public IEnumerator<Row> GetEnumerator()
        {
            return rows.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)rows.GetEnumerator();
        }

        #endregion
    }

    [Serializable]
    public sealed class Row : IDictionary<string, Column>
    {
        #region Fields

        private Dictionary<string, Column> columns;

        #endregion

        #region Constructor

        //internal
        public
        Row()
        {
            columns = new Dictionary<string, Column>();
        }

        #endregion

        #region Methods

        public void Add(string key, Column value)
        {
            columns.Add(key, value);
        }

        public bool ContainsKey(string key)
        {
            return columns.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return columns.Keys; }
        }

        public bool Remove(string key)
        {
            return columns.Remove(key);
        }

        public bool TryGetValue(string key, out Column value)
        {
            return columns.TryGetValue(key, out value);
        }

        public ICollection<Column> Values
        {
            get { return columns.Values; }
        }

        public Column this[int index]
        {
            get
            {
                if (index >= 0 && index < columns.Count)
                {
                    int i = 0;
                    foreach (KeyValuePair<string, Column> kvpc in columns)
                    {
                        if (i == index) return kvpc.Value;
                        i++;
                    }
                    return null;
                }
                else return null;
            }
            set { }
        }

        public Column this[string key]
        {
            get
            {
                return columns[key];
            }
            set { }
        }

        public void Add(KeyValuePair<string, Column> item)
        {
            columns.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            columns.Clear();
        }

        public bool Contains(KeyValuePair<string, Column> item)
        {
            return columns.ContainsKey(item.Key);
        }

        public void CopyTo(KeyValuePair<string, Column>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { return columns.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<string, Column> item)
        {
            return columns.Remove(item.Key);
        }

        public IEnumerator<KeyValuePair<string, Column>> GetEnumerator()
        {
            return columns.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)columns.GetEnumerator();
        }

        #endregion
    }

    [Serializable]
    public sealed class Column
    {
        #region Fields

        private string name;

        private object value;

        private bool isNull;

        public bool IsNull
        {
            get { return isNull; }
        }

        #endregion

        #region Constructor

        // internal
        public Column(string name, object value, bool isNull)
        {
            this.name = name;
            this.value = value;
            this.isNull = isNull;
        }

        #endregion

        #region Methods

        private T GetValue<T>(T ifNull)
        {
            if (IsNull) return ifNull;
            else
            {
                if (value.GetType() == typeof(T)) return (T)value;
                else return default(T);
            }
        }

        private T GetValue<T>()
        {
            return GetValue<T>(default(T));
        }

        public object ToObject()
        {
            return ToObject(null);
        }

        public object ToObject(object ifNull)
        {
            if (IsNull) return ifNull;
            else return value;
        }

        public override string ToString()
        {
            return GetValue<string>();
        }

        public string ToString(string ifNull)
        {
            if (IsNull) return ifNull;
            else return ToString();
        }

        public long ToLong()
        {
            if (value.GetType() == typeof(long))
                return GetValue<long>();
            else if (value.GetType() == typeof(int))
                return GetValue<int>();
            else if (value.GetType() == typeof(short))
                return GetValue<short>();
            else if (value.GetType() == typeof(decimal))
                return (int)GetValue<decimal>();
            else
                return 0;
        }

        public long ToLong(long ifNull)
        {
            if (IsNull) return ifNull;
            else return ToLong();
        }

        public int ToInt()
        {
            if (value.GetType() == typeof(int))
                return GetValue<int>();
            else if (value.GetType() == typeof(short))
                return GetValue<short>();
            else if (value.GetType() == typeof(byte))
                return (int)GetValue<byte>();
            else if (value.GetType() == typeof(decimal))
                return (int)GetValue<decimal>();
            else
                return 0;
        }

        public int ToInt(int ifNull)
        {
            if (IsNull) return ifNull;
            else return ToInt();
        }

        public int ToIntShort()
        {
            if (value.GetType() == typeof(short))
                return GetValue<short>();
            else if (value.GetType() == typeof(decimal))
                return (int)GetValue<decimal>();
            else
                return 0;
        }

        public int ToIntShort(int ifNull)
        {
            if (IsNull) return ifNull;
            else return ToIntShort();
        }

        public int ToIntByte()
        {
            if (value.GetType() == typeof(byte))
                return (int)GetValue<byte>();
            else
                return 0;
        }

        public int ToIntByte(int ifNull)
        {
            if (IsNull) return ifNull;
            else return ToIntByte();
        }

        public double ToDouble()
        {
            if (value.GetType() == typeof(decimal))
                return (double)GetValue<decimal>();
            else
                return 0;
        }

        public double ToDouble(double ifNull)
        {
            if (IsNull) return ifNull;
            else return ToDouble();
        }

        public bool? ToNullableBool()
        {
            if (IsNull) return null;
            else return new bool?(ToBool());
        }

        public bool ToBool()
        {
            if (value.GetType() == typeof(bool))
                return GetValue<bool>();
            else
                return false;
        }

        public bool ToBool(bool ifNull)
        {
            if (IsNull) return ifNull;
            else return ToBool();
        }

        public DateTime ToDateTime()
        {
            return GetValue<DateTime>();
        }

        public DateTime ToDateTime(DateTime ifNull)
        {
            if (IsNull) return ifNull;
            else return ToDateTime();
        }

        public DateTime? ToNullabelDateTime()
        {
            if (IsNull) return null;
            else return new DateTime?(ToDateTime());
        }

        public TimeSpan ToTimeSpan()
        {
            return GetValue<TimeSpan>();
        }

        public TimeSpan ToTimeSpan(TimeSpan ifNull)
        {
            if (IsNull) return ifNull;
            else return ToTimeSpan();
        }

        #endregion
    }
}