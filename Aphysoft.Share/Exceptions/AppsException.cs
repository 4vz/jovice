using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    [Serializable]
    public class AppsException : Exception
    {
        public AppsException() { }
        public AppsException(string message) : base(message) { }
        public AppsException(string message, Exception inner) : base(message, inner) { }
        protected AppsException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
