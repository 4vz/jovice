using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aphysoft.Share;
using Jovice;
using Aphysoft.Common;

namespace ServiceConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Service.Server(ServiceOutputTypes.Default);
            Server.Init();
        }
    }
}
