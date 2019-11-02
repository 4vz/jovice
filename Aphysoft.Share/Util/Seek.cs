using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    public static class Seek
    {
        public delegate int SizeHandler(int index);
        public delegate object[] BufferRetrieve();
        public delegate void GroupCallback(int index, object[] data);

        public static void Variable(int groupCount, SizeHandler groupSize, BufferRetrieve retrieve, GroupCallback callback)
        {
            int order = 0;
            int orderSeek = 0;

            object[] orderBuffer = null;

            while (true)
            {
                object[] buffer = retrieve();

                int available = buffer.Length;

                if (available > 0)
                {
                    int bufferSeek = 0;

                    while (available > 0)
                    {
                        int orderCountRead = 0;

                        if (orderSeek == 0)
                        {
                            int orderSize = groupSize(order);

                            if (orderSize > 0)
                            {
                                orderCountRead = orderSize;

                                if (orderCountRead > 100000000) throw new OverflowException();
                                orderBuffer = new object[orderCountRead];
                            }
                            else
                            {
                                order++;
                            }
                        }

                        if (orderSeek < orderCountRead)
                        {
                            int orderLeft = orderCountRead - orderSeek;
                            int retrieved = (available < orderLeft) ? available : orderLeft;

                            Array.Copy(buffer, bufferSeek, orderBuffer, orderSeek, retrieved);

                            available -= retrieved;
                            orderSeek += retrieved;
                            bufferSeek += retrieved;

                            if (orderSeek == orderCountRead)
                            {
                                callback(order, orderBuffer);

                                order++;
                                orderSeek = 0;
                            }
                        }

                        if (order >= groupCount)
                        {
                            order = 0;
                        }
                    }
                }
                else
                {
                    break;
                }


            }
        }
    }
}
