using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommon.Protocol
{
    /// <summary>
    /// action执行者
    /// </summary>
    public interface IActionExecuter
    {
        /// <summary>
        /// 执行行为
        /// </summary>
        /// <param name="ControllerName">控制器名称</param>
        /// <param name="ActionName">行为名称</param>
        object ExecuteAction(ExecuteContext executeContext);
    }
}
