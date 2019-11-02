using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    public class Attempt
    {
        public delegate void AttemptCallback();

        public static void Try(int count, AttemptCallback attempt)
        {
            Try(count, 0, attempt);
        }

        public static void Try(int count, int delay, AttemptCallback attempt)
        {
            if (count > 0)
            {
                int i = 0;
                Exception throwdis = null;
                for (i = 0; i < count; i++)
                {
                    try
                    {
                        attempt();
                        break;
                    }
                    catch (Exception ex)
                    {
                        throwdis = ex;
                    }

                    if (delay > 0)
                        Thread.Sleep(delay);
                }

                if (i == count)
                {
                    throw throwdis;
                }
            }
        }
    }
}
