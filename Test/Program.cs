using Aphysoft.Share;
using System.ComponentModel;
using System.IO;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {

            string afis = "AFIS";
            string anisa = "ANISA";

            string cinta = "CINTA";

            string result = string.Format($"{afis} {{0}} {anisa}", cinta);

            Apps.Console(result);
            Apps.ConsoleReadLine();

            //Apps.Service(new Test());
        }
    }

    [RunInstaller(true)]
    public class Installer : AppsInstaller
    {
        public Installer() : base("Test Service", "Test Service") { }
    }
}
