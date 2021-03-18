using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommon.Protocol
{
    /// <summary>
    /// 一个标准的以字典对象参数的执行上下文
    /// </summary>
    public class NormalExecuteContext : ExecuteContext
    {
        /// <summary>
        /// 参数集
        /// </summary>
        public Dictionary<string, object> Paramters { get; set; }

        public NormalExecuteContext()
        {
            Paramters = new Dictionary<string, object>();    
        }
    }
}
