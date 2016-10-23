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
            if (Necrow.Debug())
            {
                Necrow.Test("ME4-D5-RKT");
#endif
            Necrow.Start();
#if DEBUG
            }
#endif
            Necrow.Console();
        }
    }
}
