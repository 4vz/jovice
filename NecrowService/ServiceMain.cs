using Center;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;

namespace NecrowService
{
    public partial class ServiceMain : ServiceBase
    {
        public ServiceMain()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            Necrow.Start();
        }

        protected override void OnStop()
        {
            Necrow.Stop();
        }
    }
}
