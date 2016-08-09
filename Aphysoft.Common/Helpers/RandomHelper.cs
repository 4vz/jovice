using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Aphysoft.Common
{
    public static class RandomHelper
    {
        [ThreadStatic]
        private static Random random;

        private static Random Random
        {
            get { return random ?? (random = new Random(unchecked(Environment.TickCount * 31 + Thread.CurrentThread.ManagedThreadId))); }
        }

        public static int Next()
        {
            return Random.Next();
        }

        public static int Next(int maxValue)
        {
            return Random.Next(maxValue);
        }

        public static int Next(int minValue, int maxValue)
        {
            return Random.Next(minValue, maxValue);
        }

        public static double NextDouble()
        {
            return Random.NextDouble();
        }

        public static void NextBytes(byte[] buffer)
        {
            Random.NextBytes(buffer);
        }
    }
}
