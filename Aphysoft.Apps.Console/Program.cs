using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Apps.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            string assembly = args[0];

            if (assembly != null)
            {
                string assemblyData = args[1];
                string name = Path.GetFileNameWithoutExtension(assembly);

                AppDomainSetup setup = new AppDomainSetup { ApplicationName = name, ShadowCopyFiles = "true", ConfigurationFile = assembly + ".config" };

                while (true)
                {
                    AppDomain domain = AppDomain.CreateDomain(name, AppDomain.CurrentDomain.Evidence, setup);

                    if (assemblyData == null) assemblyData = "";
                    domain.SetData("data", assemblyData);

                    try
                    {
                        domain.ExecuteAssembly(assembly);
                        int exitcode = Environment.ExitCode;
                        AppDomain.Unload(domain);
                        if (exitcode == 0) break;
                    }
                    catch (Exception ex)
                    {
                        System.Console.WriteLine("Exception: " + ex.Message);
                        System.Console.ReadLine();
                        break;
                    }
                }
            }
        }
    }
}
