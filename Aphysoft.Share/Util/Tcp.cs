using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    public static class Tcp
    {
        public static bool IsPortListenAvailable(int port)
        {
            bool available = true;

            IPEndPoint[] tcpConnInfoArray = IPGlobalProperties.GetIPGlobalProperties().GetActiveTcpListeners();

            foreach (IPEndPoint tcpi in tcpConnInfoArray)
            {
                if (tcpi.Port == port)
                {
                    available = false;
                    break;
                }
            }

            return available;
        }
    }
}
