using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Aphysoft.Share;

namespace Center
{
    class Program
    {
        static void Main(string[] args)
        {
            Apps.Service(new CenterService());
        }
    }

    [RunInstaller(true)]
    public class Installer : AppsInstaller
    {
        public Installer() : base("Center Service", "Center Service") { }
    }
}
