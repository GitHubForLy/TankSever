using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommon
{
    public interface IDataFormatter
    {
        byte[] Serialize(object ojb);
        object Deserialize(byte[] data);
        T Deserialize<T>(byte[] data);
        IDynamicType DeserializeDynamic(byte[] data);
    }
}
