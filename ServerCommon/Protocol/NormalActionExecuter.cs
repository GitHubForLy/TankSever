using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommon.Protocol
{
    /// <summary>
    /// 一个标准的action执行者
    /// </summary>
    public class NormalActionExecuter : ActionExecuter
    {
        public override object GetTargetParameterValue(ParameterInfo parameter, ExecuteContext executeContext)
        {
            NormalExecuteContext Context = executeContext as NormalExecuteContext;
            foreach (var dpar in Context.Paramters)
            {
                if (parameter.Name == dpar.Key)
                {
                    return dpar.Value;
                }
            }
            return null;
        }
    }
}
