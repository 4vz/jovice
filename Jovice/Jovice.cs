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
        public static Database2 Database => Database2.Get("JOVICE");
    }
}

namespace Necrow
{
    public static class Jovice
    {
        public static Database2 Database => Database2.Get("JOVICE");
    }
}
