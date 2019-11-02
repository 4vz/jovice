using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    public static class FloatExtensions
    {
        public static float? Nullable(this float value, float returnNullIf)
        {
            if (value == returnNullIf) return (float?)null;
            else return new float?(value);
        }

        public static int Round(this float value, MidpointRounding mode = MidpointRounding.AwayFromZero)
        {
            return (int)Math.Round(value, mode);
        }
    }
}
