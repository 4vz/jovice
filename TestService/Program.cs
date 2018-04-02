using Aphysoft.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Aphysoft.Test;

namespace TestService
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            Test test = new Test();

            Apps.Service("TestService", delegate ()
            {
                test.Start();
            }, 
            delegate ()
            {
                test.Stop();
            });
        }
    }
}
