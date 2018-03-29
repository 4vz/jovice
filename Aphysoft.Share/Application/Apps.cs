using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Reflection;
using System.Configuration;
using System.Diagnostics;

namespace Aphysoft.Share
{
    public static class Apps
    {
        public delegate void StartCallback();

        private static bool active = false;

        public static bool Active { get => active; }

        public static void Console(StartCallback callback)
        {
            if ((string)AppDomain.CurrentDomain.GetData("data") == null)
            {
                Assembly caller = Assembly.GetCallingAssembly();
                AssemblyName callerName = caller.GetName();

                Process process = new Process();
                process.EnableRaisingEvents = true;

                process.StartInfo = new ProcessStartInfo();
                process.StartInfo.Arguments = caller.Location + " " + callerName.Name + "/" + callerName.Version.ToString() + "/" + callerName.ProcessorArchitecture.ToString();
                process.StartInfo.FileName = "aphysoft.apps.console.exe";
                process.StartInfo.WorkingDirectory = Directory.GetCurrentDirectory();

                process.Start();
            }
            else
            {
                Environment.ExitCode = 0;
                active = true;

                System.Console.WriteLine((string)AppDomain.CurrentDomain.GetData("data"));

                callback();
            }
        }

        public static void Restart()
        {
            Environment.ExitCode = 100;
        }
    }
}
