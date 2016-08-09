using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aphysoft.Common
{
    /// <summary>
    /// Boolean Extensions
    /// </summary>
    public static class BooleanExtensions
    {
        public static int ToInt(this bool value)
        {
            return value ? 1 : 0;
        }
    }
}
