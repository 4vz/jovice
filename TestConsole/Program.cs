using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using System.Xml.Serialization;
using System.Diagnostics;
using Aphysoft.Share;
using System.Net;
using Aphysoft.Test;

namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Test t = new Test();
            t.Start(true);
        }
    }


}
