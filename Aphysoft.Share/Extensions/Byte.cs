using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    public static class ByteExtensions
    {
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int memcmp(byte[] b1, byte[] b2, UIntPtr count);

        public static bool SequenceEqual(this byte[] b1, byte[] to)
        {
            if (b1 == to) return true; //reference equality check

            if (b1 == null || to == null || b1.Length != to.Length) return false;

            return memcmp(b1, to, new UIntPtr((uint)b1.Length)) == 0;
        }

        public static string ToHex(this byte[] value)
        {
            var hex = new StringBuilder(value.Length * 2);
            foreach (byte b in value)
            {
                hex.AppendFormat("{0:x2}", b);
            }

            return hex.ToString();
        }
    }
}
