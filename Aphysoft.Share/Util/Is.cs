using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    public static class Is
    {
        public static bool NullOrEmpty(params string[] values)
        {
            bool nullOrEmpty = false;

            foreach (string value in values)
            {
                if (string.IsNullOrEmpty(value))
                {
                    nullOrEmpty = true;
                    break;
                }
            }

            return nullOrEmpty;
        }
    }
}
