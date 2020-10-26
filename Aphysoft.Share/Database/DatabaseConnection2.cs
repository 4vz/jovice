using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Aphysoft.Share
{
    public abstract class DatabaseConnection2
    {
        #region Fields

        public Database2 database;

        protected Stopwatch stopwatch;

        protected bool cancelling = false;

        public string ConnectionString { get; set; }

        #endregion

        #region Constructor

        public DatabaseConnection2(string connectionString)
        {
            stopwatch = new Stopwatch();

            ConnectionString = connectionString;
        }

        #endregion

        #region Virtuals

        public virtual DatabaseExceptionType2 ParseMessage(string message) { return DatabaseExceptionType2.None; }

        public virtual bool IsConnected() { throw new NotImplementedException(); }

        public virtual void InitializeDatabase() { throw new NotImplementedException(); }

        public virtual string Escape(string str) { throw new NotImplementedException(); }

        public virtual string Format(DateTime dateTime) { throw new NotImplementedException(); }

        public virtual Result2 Query(string sql) { throw new NotImplementedException(); }

        public virtual Column2 Scalar(string sql) { throw new NotImplementedException(); }

        protected virtual Result2 Execute(string sql, bool identity) { throw new NotImplementedException(); }

        public virtual int Cancel() { throw new NotImplementedException(); }

        #endregion

        #region Methods

        public Result2 Execute(string sql) { return Execute(sql, false); }

        public Result2 ExecuteIdentity(string sql) { return Execute(sql, true); }

        #endregion
    }

}
