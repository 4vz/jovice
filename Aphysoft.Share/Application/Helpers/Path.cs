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


        public static string Absolute(string virtualpath)
        {
            return VirtualPathUtility.ToAbsolute(virtualpath);
        }

        /// <summary>
        /// Convert specific application path to physical path, eg. ~/path/to/file.aspx to C:\app\path\to\file.aspx
        /// </summary>
        /// <returns></returns>
        public static string PhysicalPath(string virtualpath)
        {
            string path = string.Format("{0}{1}", HttpContext.Current.Request.PhysicalApplicationPath, Absolute(virtualpath).Substring(1).Replace('/', '\\'));
            
            return path;
        }
    }
}
