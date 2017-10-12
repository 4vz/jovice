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
        private Necrow necrow;

        public ServiceMain()
        {
            InitializeComponent();

            necrow = new Necrow();
        }

        protected override void OnStart(string[] args)
        {
            necrow.Start();
        }

        protected override void OnStop()
        {
            necrow.Stop();
        }
    }
}
