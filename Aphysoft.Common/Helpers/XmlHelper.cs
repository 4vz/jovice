using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aphysoft.Common
{
    public static class XmlHelper
    {
        public static T Serialize<T>(string xml) where T : class
        {
            var reader = new System.IO.StringReader(xml);
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));

            T oT = default(T);

            try
            {
                oT = serializer.Deserialize(reader) as T;
            }
            catch (System.Xml.XmlException xe)
            {
                throw xe;
            }
            finally
            {
                reader.Close();
            }

            return oT;
        }

        public static string Deserialize<T>(T oT) where T : class
        {
            var writer = new System.IO.StringWriter();
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(T));

            try
            {
                serializer.Serialize(writer, oT);
            }
            catch (System.Xml.XmlException xe)
            {
                throw xe;
            }
            finally
            {
                writer.Close();
            }

            return writer.ToString();
        }
    }
}
