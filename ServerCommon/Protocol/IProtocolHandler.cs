using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommon.Protocol
{
    /// <summary>
    /// 定义一个协议处理(例如json和protocolbuffer)的接口
    /// </summary>
    public interface IProtocolHandler
    {
        /// <summary>
        /// 反序列化指定对象
        /// </summary>
        object Deserialize(byte[] data);

        /// <summary>
        /// 序列化指定对象
        /// </summary>
        byte[] Serialize(object obj);


        /// <summary>
        /// 反序列化指定对象
        /// </summary>
        T Deserialize<T>(byte[] data);


        /// <summary>
        /// 序列化指定对象
        /// </summary>
        byte[] Serialize<T>(T obj);
    }
}
