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
        /// <param name="executeContext">执行上下文</param>
        object ExecuteAction(ExecuteContext executeContext);

        /// <summary>
        /// 尝试执行行为 若没有找到返回false
        /// </summary>
        /// <param name="executeContext">执行上下文</param>
        /// <param name="result">若找到返回结果</param>
        bool TryExecuteAction(ExecuteContext executeContext,out object result);
    }
}
