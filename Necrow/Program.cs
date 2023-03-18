using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



using System.IO;

using Aphysoft.Share;
using System.ComponentModel;
using System.Diagnostics;

namespace Necrow
{
    class Program
    {
        static void Main(string[] args)
        {
#if DEBUG
            //Apps.Service(new Necrow());
            // Deep crawl
            Apps.Service(new Necrow("ME9-D1-MDC-3", ProbeTypes.Deep));
#else
            Apps.Service(new Necrow());
#endif

        }
    }
    
    [RunInstaller(true)]
    public class Installer : AppsInstaller
    {
        public Installer() : base("Necrow Service", "Necrow Service") { }
    }
}
