using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using ServerCommon;

namespace ProtobufProto
{
    public class ProtobufFormatter : IDataFormatter
    {
        public object Deserialize(Type type,byte[] data)
        {
            using (var stream=new MemoryStream(data))
            {
                return  Serializer.Deserialize(type,stream);               
            }
        }

        public T Deserialize<T>(byte[] data)
        {
            using (var stream = new MemoryStream(data))
            {
                return Serializer.Deserialize<T>(stream);
            }
        }

        public byte[] Serialize(object ojb)
        {
            using (var stream = new MemoryStream())
            {
                Serializer.Serialize(stream, ojb);
                return stream.ToArray(); 
            }
        }
    }
}
