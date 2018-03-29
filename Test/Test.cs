using Aphysoft.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Test
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
            if (IsConsole)
                Console.WriteLine(message);
        }

        protected override void OnStart()
        {
            Event("Test");

            BeginEdge(IPAddress.Loopback);

            while (IsRunning)
            {
                if (!IsRunning)
                {
                    Event("Byte");
                    break;
                }
            }
        }

        protected override void OnStop()
        {
            Console.WriteLine("Stop is called");


        }


    }
}
