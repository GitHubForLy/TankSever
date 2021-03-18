using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCommon.NetServer;

namespace ServerCommon.Protocol
{
    /// <summary>
    /// 执行上下文
    /// </summary>
    public class ExecuteContext
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        /// <summary>
        /// 用户连接对象
        /// </summary>
        public AsyncUserToken UserToken { get; set; }
    }
}
