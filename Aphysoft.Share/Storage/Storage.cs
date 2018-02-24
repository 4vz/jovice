using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Security.AccessControl;
using System.Security.Permissions;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Share
{ 
    public static class IO
    {
        public static bool HasWriteAccessToDirectory(string folderPath)
        {
            try
            {
                // TODO

                System.Security.AccessControl.DirectorySecurity ds = Directory.GetAccessControl(folderPath);
                return true;

            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }
    }
}
