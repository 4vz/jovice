using Aphysoft.Share;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Athena
{
    class Program
    {
        static void Main(string[] args)
        {
            Apps.Service(new Athena());
        }
    }

    [RunInstaller(true)]
    public class Installer : AppsInstaller
    {
        public Installer() : base("Aphysoft Athena", "Aphysoft Athena provides centralized management for all Aphysoft Apps services present in the network. If this service is stopped, most of Aphysoft Apps services functionality will not work.") { }
    }
}
