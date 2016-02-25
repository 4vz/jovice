using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Jovice;
using System.IO;

namespace NecrowConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("NecrowConsole v2.0.1");
            FileInfo fi = new FileInfo("Necrow.ini");
            if (fi.Exists)
            {
                Console.Write("Reading Necrow.ini... ");
                string[] clines = File.ReadAllLines(fi.FullName);

                ProbeProperties properties = new ProbeProperties();

                foreach (string cline in clines)
                {
                    if (cline.Length > 0)
                    {
                        int eqin = cline.IndexOf('=');
                        string key = cline.Substring(0, eqin);
                        string value = cline.Substring(eqin + 1);

                        if (key == "SSHServerAddress") properties.SSHServerAddress = value;
                        else if (key == "SSHUser") properties.SSHUser = value;
                        else if (key == "SSHPassword") properties.SSHPassword = value;
                        else if (key == "SSHTerminal") properties.SSHTerminal = value;
                        else if (key == "TacacUser") properties.TacacUser = value;
                        else if (key == "TacacPassword") properties.TacacPassword = value;
                    }
                }

                Console.WriteLine("Done");

                if (properties.SSHServerAddress != null && properties.SSHUser != null && properties.SSHPassword != null && properties.SSHTerminal != null &&
                    properties.TacacPassword != null && properties.TacacPassword != null)
                {
                    
                    Necrow.Set(properties);
                    Necrow.Start(NecrowServices.Probe | NecrowServices.ServiceFinder | NecrowServices.TopologyFinder | NecrowServices.Summary);
                    //Necrow.Start(NecrowServices.None);
                    Necrow.Console();
                }
                else
                {
                    Console.WriteLine("Incomplete configuration. Please check the configuration file.");
                    Console.ReadLine();
                }
                
            }
            else
            {
                Console.WriteLine("Cannot find Necrow.ini configuration file. Make sure the configuration file exists in " + fi.Directory.FullName + ".");
                Console.ReadLine();
            }
        }
    }
}
