using Aphysoft.Share;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Test
{
    public class Test : Node
    {
        #region Constructors

        public Test() : base("TEST")
        {
        }

        #endregion

        protected override void OnEvent(string message)
        {
            if (Apps.IsConsole)
                Console.WriteLine(message);
        }

        protected override void OnStart()
        {
            while (IsRunning)
            {
                Thread.Sleep(1000);
            }
        }        

        protected override void OnStop()
        {
        }


    }
}
