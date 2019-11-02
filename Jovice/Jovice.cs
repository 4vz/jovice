using Aphysoft.Share;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Center
{
    public static class Jovice
    {
        public static Database Database => Database.Get("JOVICE");
    }
}

namespace Necrow
{
    public static class Jovice
    {
        public static Database Database => Database.Get("JOVICE");
    }
}
