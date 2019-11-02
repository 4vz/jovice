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

        public static bool StartsWith(this byte[] b1, byte[] with)
        {
            byte[] compare = new byte[with.Length];
            Buffer.BlockCopy(b1, 0, compare, 0, with.Length);

            return compare.SequenceEqual(with);
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

        public static void Random(this byte[] b)
        {
            Rnd.Bytes(b);
        }        

        public static byte[] ToArray(this byte b)
        {
            return new[] { b };
        }
    }

    public static class Bytes
    {
        public static readonly byte[] Zero = new byte[] { 0 };

        public static byte[] GetZero(int n)
        {
            if (n < 1) return null;

            byte[] b = new byte[n];

            for (int i = 0; i < n; i++)
            {
                b[i] = 0;
            }

            return b;
        }

        public static readonly byte[] One = new byte[] { 1 };

        public static byte[] BlockCopy(byte[] source, int offset, int length)
        {
            byte[] destination = new byte[length];

            Buffer.BlockCopy(source, offset, destination, 0, length);

            return destination;
        }
    }
}
