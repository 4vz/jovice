using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    public enum DatabaseExceptionType2
    {
        None,
        LoginFailed,
        Timeout
    }

    public class DatabaseExceptionEventArgs2 : EventArgs
    {
        #region Fields

        public string Sql { get; internal set; }

        public string Message { get; internal set; }

        public DatabaseExceptionType2 Type { get; internal set; }

        public bool NoRetry { get; set; } = false;

        #endregion

        #region Constructor

        public DatabaseExceptionEventArgs2()
        {

        }

        #endregion
    }

    public delegate void DatabaseExceptionEventHandler2(object sender, DatabaseExceptionEventArgs2 eventArgs);
}
