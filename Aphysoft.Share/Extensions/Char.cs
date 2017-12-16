using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    public static class CharExtensions
    {
        public static char? Nullable(this char value, char returnNullIf)
        {
            if (value == returnNullIf) return (char?)null;
            else return new char?(value);
        }
    }
}
