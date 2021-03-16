using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCommon.NetServer;

namespace NetCore.Server
{
    /// <summary>
    /// 使用完整数据包的UserToken
    /// </summary>
    public class PackageUserToken:AsyncUserToken
    {
        /// <summary>
        /// 数据包
        /// </summary>
        public DataPackage Pakcage { get; set; }

        public PackageUserToken(AsyncSocketServerBase Server,int receiveBufferSize):base(Server, receiveBufferSize)
        {
            Pakcage = new DataPackage();
        }
    }
}
