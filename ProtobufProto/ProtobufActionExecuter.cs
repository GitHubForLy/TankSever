using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ServerCommon.Protocol;

namespace ProtobufProto
{
    /// <summary>
    /// Protobuf协议的action执行者
    /// </summary>
    public class ProtobufActionExecuter : ActionExecuter
    {
        protected override Assembly GetActionAssembly()
        {
            return Assembly.GetExecutingAssembly();
        }
        public override object GetTargetParameterValue(ParameterInfo parameter, ExecuteContext executeContext)
        {
            ProtobufExecuteContext context = executeContext as ProtobufExecuteContext;
            if (parameter.ParameterType.IsAssignableFrom(context.SubMessage.GetType()))
                return context.SubMessage;

            if (parameter.HasDefaultValue)
                return parameter.DefaultValue;
            else if (parameter.ParameterType.IsValueType)
                return parameter.RawDefaultValue;
            return null;
        }

        bool IsNullableType(Type theType)
        {
            return (theType.IsGenericType && theType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)));
        }
    }
}
