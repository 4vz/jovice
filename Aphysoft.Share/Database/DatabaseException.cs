using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    public enum DatabaseExceptionType
    {
        None,
        LoginFailed,
        Timeout
    }

    public class DatabaseExceptionEventArgs : EventArgs
    {
        #region Fields

        public string Sql { get; internal set; }

        public string Message { get; internal set; }

        public DatabaseExceptionType Type { get; internal set; }

        public bool NoRetry { get; set; } = false;

        #endregion

        #region Constructor

        public DatabaseExceptionEventArgs()
        {

        }

        #endregion
    }

    public delegate void DatabaseExceptionEventHandler(object sender, DatabaseExceptionEventArgs eventArgs);
}
