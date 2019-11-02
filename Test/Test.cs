using Aphysoft.Share;
using System;
using System.IO.Pipes;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading;

namespace Test
{
    public class Test : Node
    {
        #region Constructors

        public Test() : base("TEST")
        {
        }

        #endregion

        protected override void OnStart()
        {
            BeginEdge(IPAddress.Loopback, "ATHENA");

            StandBy();
        }        

        protected override void OnStop()
        {
        }
    }
}
