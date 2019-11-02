using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    public static class Rnd
    {
        [ThreadStatic]
        private static System.Random random;

        private static System.Random Seed
        {
            get { return random ?? (random = new System.Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
        }

        public static int Int()
        {            

            return Seed.Next();
        }

        public static int Natural()
        {
            return Seed.Next(0, int.MaxValue);
        }

        public static int Int(int maxValue)
        {
            return Seed.Next(maxValue);
        }

        public static int Int(int minValue, int maxValue)
        {
            return Seed.Next(minValue, maxValue);
        }

        public static double Double()
        {
            return Seed.NextDouble();
        }

        public static void Bytes(byte[] buffer)
        {
            Seed.NextBytes(buffer);
        }
    }
}
