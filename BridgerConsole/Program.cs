using Aphysoft.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BridgerConsole
{
    class Program
    {
        static void Main(string[] args)
        {

#if !DEBUG
            Apps.Console(delegate ()
            {
#endif
            Bridger bridger = new Bridger();

            bridger.Start(true);


#if !DEBUG
            });
#endif
        }
    }
}
