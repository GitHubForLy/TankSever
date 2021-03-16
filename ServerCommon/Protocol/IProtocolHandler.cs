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
        /// <param name="actionExecuter"></param>
       bool DataHandle(AsyncUserToken userToken, ref byte[] data, IActionExecuter actionExecuter);
    }

    
}
