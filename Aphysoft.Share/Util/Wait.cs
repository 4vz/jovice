using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    public static class Wait
    {
        public delegate bool UntilEventHandler();

        public static void Until(UntilEventHandler e, int secondsTimeout)
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            while (true)
            {
                if (e())
                {
                    break;
                }
                else
                {
                    if (sw.Elapsed.Seconds > secondsTimeout)
                    {
                        break;
                    }
                }

                Thread.Sleep(1000);
            }
        }

        public static void Until(UntilEventHandler e)
        {
            Until(e, int.MaxValue);
        }
    }
}
