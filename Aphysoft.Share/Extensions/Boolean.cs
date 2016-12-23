using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aphysoft.Share
{
    /// <summary>
    /// Boolean Extensions
    /// </summary>
    public static class BoolExtensions
    {
        public static int ToInt(this bool value)
        {
            return value ? 1 : 0;
        }

        public static string Describe(this bool value, string ifTrue, string ifFalse)
        {
            return value ? ifTrue : ifFalse;
        }

        public static string DescribeTrueFalse(this bool value)
        {
            return value.Describe("True", "False");
        }

        public static string DescribeUpDown(this bool value)
        {
            return value.Describe("Up", "Down");
        }

        public static string DescribeYesNo(this bool value)
        {
            return value.Describe("Yes", "No");
        }
    }
}
