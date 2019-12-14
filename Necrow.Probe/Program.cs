using System;
using Aveezo;

namespace Necrow.Probe
{
    class Program
    {
        static void Main(string[] args)
        {
            Database d = new Database("Data Source=localhost;Initial Catalog=jovice;User ID=development;Password=development;async=true", DatabaseType.SqlServer);

            
       
            while (d.Query("select * from Node", out string de))
            {

            }



            



            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
    }
}
