using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCommon.NetServer;

namespace ServerCommon.Protocol
{
    public class ExecuteContext
    {
        public ExecuteContext()
        {
            Paramters = new Dictionary<string, object>();
        }

        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public Dictionary<string, object> Paramters { get; set; }
        /// <summary>
        /// 用户连接对象
        /// </summary>
        public AsyncUserToken UserToken { get; set; }
    }
}
