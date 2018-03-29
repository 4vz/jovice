using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    public class Bridger : Node
    {
        #region Constructors

        public Bridger() : base("Bridger")
        {
            
        }

        #endregion

        protected override void OnEvent(string message)
        {
#if DEBUG
            Console.WriteLine(message);
#endif
        }

        protected override void OnStart()
        {
            Console.WriteLine("Yesss im starting");

            BeginAcceptEdges();

            while (IsRunning)
            {
                if (!IsRunning)
                {
                    Console.WriteLine("Bye! :(");
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
