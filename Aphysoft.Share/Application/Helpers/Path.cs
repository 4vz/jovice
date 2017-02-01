using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Aphysoft.Share
{
    /// <summary>
    /// Provides a set of static methods for path operations. 
    /// </summary>
    public static class Path
    {
        /// <summary>
        /// Get base path of current context.
        /// </summary>
        public static string Base()
        {
            return HttpContext.Current.Request.Path;
        }
        
        // C:\a\b\c  = 4
        // ../../afis/lima.txt

        /// <summary>
        /// Convert specific application path to physical path, eg. ~/path/to/file.aspx to C:\app\path\to\file.aspx
        /// </summary>
        /// <returns></returns>
        public static string PhysicalPath(string virtualpath)
        {
            if (virtualpath.StartsWith("../"))
            {
                string[] appPaths = HttpContext.Current.Request.PhysicalApplicationPath.Split(new char[] { '\\' }, StringSplitOptions.RemoveEmptyEntries);
                string[] vPaths = virtualpath.Split(StringSplitTypes.Slash, StringSplitOptions.RemoveEmptyEntries);

                int appPathCount = appPaths.Length;
                int vPathCount = 0;
                foreach (string vp in vPaths)
                {
                    if (vp == "..")
                    {
                        appPathCount--;
                        vPathCount++;
                    }
                    else break;
                }
                return string.Join("\\", appPaths, 0, appPathCount) + "\\" + string.Join("\\", vPaths, vPathCount, vPaths.Length - vPathCount);
            }
            else
            {
                return HttpContext.Current.Server.MapPath(virtualpath);
            }

            //string path = string.Format("{0}{1}", HttpContext.Current.Request.PhysicalApplicationPath, Absolute(virtualpath).Substring(1).Replace('/', '\\'));
            
            //return path;
        }
    }
}
