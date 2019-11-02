using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Aphysoft.Share
{
    [Serializable]
    public abstract class BaseMessage
    {
        #region Constructors

        public BaseMessage()
        {
        }

        #endregion

        #region Serialize Deserialize

        internal byte[] Serialize()
        {
            byte[] bytes = null;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                BinaryFormatter bin = new BinaryFormatter();
                try
                {
                    bin.Serialize(memoryStream, this);
                    bytes = memoryStream.ToArray();
                }
                catch (Exception ex)
                {
                }
            }

            return bytes;
        }

        internal static BaseMessage Deserialize(byte[] buffer)
        {
            BaseMessage obj = null;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                memoryStream.Write(buffer, 0, buffer.Length);
                memoryStream.Position = 0;

                BinaryFormatter bin = new BinaryFormatter();

                try
                {
                    obj = (BaseMessage)bin.Deserialize(memoryStream);
                }
                catch (Exception ex)
                {
                }
            }

            return obj;
        }

        #endregion
    }


    internal sealed class OmitResponseMessage : BaseMessage
    {
        #region Constructors

        public OmitResponseMessage()
        {

        }

        #endregion
    }
}
