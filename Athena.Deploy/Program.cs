using Aphysoft.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Athena.Deploy
{
    class Program
    {
        static void Main(string[] args)
        {
            (new AthenaDeploy(@"D:\Projects\Jovice\deploy.txt")).Start();
        }
    }
}
