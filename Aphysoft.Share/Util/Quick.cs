using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    public static class Quick
    {
        public static void Log(string log)
        {
            StreamWriter sw = new StreamWriter("D:\\log.txt", true);
            sw.WriteLine($"{DateTime.Now.ToString("yyyy/MM/dd:HH:mm:ss.fff")}|{log}");
            sw.Close();
        }
    }
}
