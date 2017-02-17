using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Center;

namespace NecrowConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("NecrowConsole");

#if DEBUG
            if (Necrow.Debug())
            {
                //Necrow.Test("PE-D7-SRU");
#endif
                Necrow.Start();
#if DEBUG
            }
#endif

            Necrow.Console();
        }
    }
}
