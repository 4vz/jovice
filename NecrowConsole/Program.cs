using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Center;

using Aphysoft.Share;

namespace NecrowConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("NecrowConsole");
            
            Necrow necrow = new Necrow();
#if DEBUG
            Console.WriteLine("DEBUG MODE");
            necrow.Test("PE-D2-ELK");
#endif
            necrow.Start(true);
        }
    }
}
