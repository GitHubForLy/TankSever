using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommon
{
    public interface IDataFormatter
    {
        /// <summary>
        /// 序列化对象
        /// </summary>
        byte[] Serialize(object ojb);
        /// <summary>
        /// 反序列化
        /// </summary>
        object Deserialize(byte[] data);
        /// <summary>
        /// 反序列化
        /// </summary>
        T Deserialize<T>(byte[] data);
        /// <summary>
        /// 反序列化为 动态类型
        /// </summary>
        IDynamicType DeserializeDynamic(byte[] data);
    }
}
