using ServerCommon.NetServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace ServerCommon.Protocol
{
    /// <summary>
    /// 数据协议处理接口
    /// </summary>
    public interface IProtocolHandler
    {
        /// <summary>
        /// 处理数据
        /// </summary>
        /// <param name="data">请求数据</param>
        void DoRequest(byte[] req);

        /// <summary>
        /// 尝试获取响应数据
        /// </summary>
        /// <param name="res">响应数据</param>
        /// <returns>有响应返回true 没有返回false</returns>
        bool TryGetRespone(out byte[] res);
    }

    
}
