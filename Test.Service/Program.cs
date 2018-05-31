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
using System.ComponentModel;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Apps.Service(new Test());
        }
    }

    [RunInstaller(true)]
    public class Installer : AppsInstaller
    {
        public Installer() : base("Test Service", "Test Service") { }
    }


}
