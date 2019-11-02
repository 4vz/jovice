using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Aveezo;

namespace Aphysoft.Share
{
    public abstract class DatabaseConnection
    {
        #region Fields

        public Database database;

        protected Stopwatch stopwatch;

        protected bool cancelling = false;

        protected string ConnectionString { get; set; }

        #endregion

        #region Constructor

        public DatabaseConnection(string connectionString)
        {
            stopwatch = new Stopwatch();

            ConnectionString = connectionString;
        }

        #endregion

        #region Virtuals

        public virtual DatabaseExceptionType ParseMessage(string message) { return DatabaseExceptionType.None; }

        public virtual bool IsConnected() { throw new NotImplementedException(); }

        public virtual void InitializeDatabase() { throw new NotImplementedException(); }

        public virtual string Escape(string str) { throw new NotImplementedException(); }

        public virtual string Format(DateTime dateTime) { throw new NotImplementedException(); }

        public virtual Result Query(string sql) { throw new NotImplementedException(); }

        public virtual Column Scalar(string sql) { throw new NotImplementedException(); }

        protected virtual Result Execute(string sql, bool identity) { throw new NotImplementedException(); }

        public virtual int Cancel() { throw new NotImplementedException(); }

        #endregion

        #region Methods

        public Result Execute(string sql) { return Execute(sql, false); }

        public Result ExecuteIdentity(string sql) { return Execute(sql, true); }

        #endregion
    }

}
