using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Reflection;

using Aveezo;


namespace Aphysoft.Share
{
    public delegate void DatabaseRetryEventHandler(object sender, DatabaseExceptionEventArgs eventArgs);

    public delegate string QueryDictionaryKeyCallback(Row row);

    public delegate void QueryDictionaryDuplicateCallback(Row row);

    public sealed class Database
    {
        #region Fields

        private DatabaseConnection connection;

        public string TablePrefix { get; } = null;

        private string[] databaseTables = new string[] { };

        /// <summary>
        /// Gets or sets how many query attempts shall be done until the current execution throws exception if none success.
        /// </summary>
        public int QueryAttempts { get; set; } = 1;

        public int Timeout { get; set; } = 30;

        public DatabaseExceptionEventArgs LastException { get; private set; } = null;

        private DateTime lastConnectionCheck = DateTime.MinValue;

        private bool lastIsConnected = false;

        public bool IsConnected
        {
            get
            {
                TimeSpan x = DateTime.Now - lastConnectionCheck;
                
                if (x.TotalSeconds > 60)
                {
                    lastIsConnected = connection.IsConnected();
                    lastConnectionCheck = DateTime.Now;
                }

                return lastIsConnected;
            }
        }



        #endregion

        #region Events

        public event DatabaseRetryEventHandler Retry;

        #endregion

        #region Constructor

        public Database(DatabaseConnection connection, string tablePrefix)
        {
            this.connection = connection ?? throw new ArgumentNullException("connection");

            connection.database = this;

            TablePrefix = tablePrefix;

            connection.InitializeDatabase();
        }

        public Database(DatabaseConnection connection) : this(connection, null)
        {
        }

        public Database(string connectionString) : this(new SqlServerDatabaseConnection(connectionString), null)
        {
        }

        #endregion

        #region Methods

        public static implicit operator bool(Database database)
        {
            return database.IsConnected;
        }

        public OldTable<T> Virtualize<T>() where T : Data
        {
            object[] atable = typeof(T).GetCustomAttributes(false);

            if (atable.Length > 0)
            {
                OldTableAttribute atablea = atable[0] as OldTableAttribute;

                if (atablea != null)
                {
                    string tableName = atablea.Name;
                    string tableShort = atablea.ShortName;
                    string tableIdFormat = atablea.IdFormat;

                    if (tableName != null && tableShort != null && tableIdFormat != null)
                    {
                        string id = string.Format(tableIdFormat, tableShort);

                        Dictionary<string, string> propertyKeys = new Dictionary<string, string>();

                        foreach (PropertyInfo pi in typeof(T).GetProperties())
                        {
                            if (pi.Name == "Id")
                            {
                                propertyKeys.Add(pi.Name, id);
                            }
                            else
                            {
                                object[] api = pi.GetCustomAttributes(false);

                                if (api.Length > 0)
                                {
                                    OldColumnAttribute apia = api[0] as OldColumnAttribute;

                                    if (apia != null)
                                    {
                                        string piKey = apia.Name;
                                        propertyKeys.Add(pi.Name, string.Format(piKey, tableShort));
                                    }
                                }
                            }
                        }
                        
                        





                    }
                }
            }


            return null;





        }

        public void UpdateDatabaseTables(string[] tables)
        {
            if (TablePrefix == null)
            {
                databaseTables = tables;
            }
            else
            {
                List<string> tl = new List<string>();

                foreach (string table in tables)
                {
                    if (table.StartsWith(TablePrefix))
                    {
                        tl.Add(table.Substring(TablePrefix.Length));
                    }
                }

                databaseTables = tl.ToArray();
            }
        }

        public void OnException(Exception e, string sql)
        {
            DatabaseExceptionEventArgs eventArgs = new DatabaseExceptionEventArgs();

            eventArgs.Type = connection.ParseMessage(e.Message);
            eventArgs.Message = e.Message;
            eventArgs.Sql = sql;

            LastException = eventArgs;
        }

        public bool OnRetry(Exception e, string sql)
        {
            if (Retry != null)
            {
                DatabaseExceptionEventArgs eventArgs = new DatabaseExceptionEventArgs();

                eventArgs.Type = connection.ParseMessage(e.Message);
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

        public string Format(DateTime dateTime)
        {
            return connection.Format(dateTime);
        }

        public string Format(string sql, params object[] args)
        {
            if (sql == null) return null;


            string fsql = sql;

            if (TablePrefix != null)
            {
                foreach (string dt in databaseTables)
                {
                    fsql = fsql.Replace("[" + dt + "]", TablePrefix + dt);
                }
            }

            if (args == null) return string.Format(fsql, "NULL");
            else if (args.Length > 0)
            {
                List<string> nargs = new List<string>();

                foreach (object a in args)
                {
                    string atrs = null;

                    if (a == null) atrs = "NULL";
                    else if (a is bool) atrs = (bool)a ? "1" : "0";
                    else if (a is DateTime) atrs = "'" + Format((DateTime)a) + "'";
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
                    else
                    {
                        string toEscape = a.ToString();

                        if (toEscape == null)
                            atrs = "NULL";
                        else
                            atrs = "'" + Escape(toEscape) + "'";
                    }

                    nargs.Add(atrs);
                }

                return string.Format(fsql, nargs.ToArray());
            }
            else return fsql;

        }

        public Result Query(string sql, params object[] args)
        {
            string fsql = Format(sql, args);
            return connection.Query(fsql);
        }

        public bool Query(out Result result, string sql, params object[] args)
        {
            result = Query(sql, args);
            return result && result > 0;
        }

        public bool Query(out Row row, string sql, params object[] args)
        {
            bool ok = Query(out Result result, sql, args);

            row = null;
            if (ok)
            {
                if (result > 0)
                    row = result[0];
                else
                    ok = false;
            }

            return ok;
        }

        public Dictionary<string, Row> QueryDictionary(string sql, string key, params object[] args)
        {
            return QueryDictionary(sql, key, delegate (Row row) { }, args);
        }

        public Dictionary<string, Row> QueryDictionary(string sql, string key, QueryDictionaryDuplicateCallback duplicate, params object[] args)
        {
            Result result = Query(sql, args);

            if (result)
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

            if (result)
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

            if (result)
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
            string sql = insert.ToString();

            Result executionResult;

            if (!string.IsNullOrEmpty(sql))
                executionResult = connection.Execute(sql);
            else
                executionResult = Result.Null;

            return executionResult;
        }

        public Result Execute(Update update)
        {
            string sql = update.ToString();

            Result executionResult;

            if (!string.IsNullOrEmpty(sql))
                executionResult = connection.Execute(sql);
            else
                executionResult = Result.Null;

            return executionResult;
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
                char r = idChars[Rnd.Int(idCharsLen)];
                sb.Append(r);
            }

            return sb.ToString();
        }

        #endregion

        #region Instancing

        private static Dictionary<string, Database> instances = new Dictionary<string, Database>();

        private static string system = null;

        public static Database Get(string name)
        {
            if (instances.ContainsKey(name))
            {
                return instances[name];
            }
            else
            {
                string config = Apps.Config(name);

                if (config != null)
                {
                    string databaseType = Apps.Config(name + "_DATABASE_TYPE");

                    Database database = null;

                    if (databaseType == null || databaseType == "SQLSERVER")
                    {
                        database = new Database(new SqlServerDatabaseConnection(config));
                    }
                    else if (databaseType == "ORACLE")
                    {
                        Type type = null;

                        try
                        {
                            type = Type.GetType("Aphysoft.Share.OracleDatabaseConnection, aphysoft.share.oracle");
                        }
                        catch (Exception ex)
                        {

                        }

                        if (type != null)
                        {
                            ConstructorInfo ctor = type.GetConstructor(new[] { typeof(string) });
                            database = new Database((DatabaseConnection)ctor.Invoke(new object[] { config }));
                        }
                    }
                    else if (databaseType == "MYSQL")
                    {
                        Type type = null;

                        try
                        {
                            type = Type.GetType("Aphysoft.Share.MySqlDatabaseConnection, aphysoft.share.mysql");
                        }
                        catch (Exception ex)
                        {

                        }

                        if (type != null)
                        {
                            ConstructorInfo ctor = type.GetConstructor(new[] { typeof(string) });
                            database = new Database((DatabaseConnection)ctor.Invoke(new object[] { config }));
                        }
                    }
                    else
                    {
                        throw new Exception("Database type is not specified");
                    }

                    lock (instances)
                    {
                        instances.Add(name, database);
                    }

                    return database;
                }
                else
                    return null;
            }
        }

        public static Database Web()
        {
            if (system == null)
            {
                system = Apps.Config("WEB");
                if (system == null) system = "";
            }
            

            if (system == "")
            {
                return null;
            }
            else
            {
                return Get(system);
            }
        }

        #endregion
    }
}
