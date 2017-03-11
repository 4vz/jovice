using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Protected
{
    public partial class Project
    {
        public static string Database(string key)
        {
            if (key == "SHARE_DEBUG") return "";
            else if (key == "SHARE_RELEASE") return "";
            else if (key == "CENTER_DEBUG") return "";
            else if (key == "CENTER_RELEASE") return "";
            else if (key == "JOVICE_DEBUG") return "";
            else if (key == "JOVICE_RELEASE") return "";
            return null;
        }
    }
}
