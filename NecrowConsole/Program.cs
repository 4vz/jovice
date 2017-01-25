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
                //Necrow.Test("me-d1-pgki-tsel");
                Necrow.Test("me4-d2-sm1");
                //Necrow.Test("ME8-D5-KBL");

                //Necrow.Test("PE-D5-KLM-INET");
                //Necrow.Test("ME-D5-KLM");

                //Necrow.Test("PE2-D2-JT2-TRANSIT");
#endif
                Necrow.Start();
#if DEBUG
            }
#endif

            Necrow.Console();
        }
    }
}
