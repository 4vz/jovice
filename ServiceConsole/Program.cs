using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aphysoft.Share;

using Center;

namespace ServiceConsole
{
    class Program
    {
        static void Main(string[] args)
        {        
            Service.Server(ServiceTraceLevels.Default);
            Server.Init();
        }
    }
}
