using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jovice;
using System.IO;

namespace NecrowConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("NecrowConsole");

#if DEBUG
            Necrow.Test("pe2-d5-kbl-transit");
#endif
            Necrow.Start();
            Necrow.Console();
        }
    }
}
