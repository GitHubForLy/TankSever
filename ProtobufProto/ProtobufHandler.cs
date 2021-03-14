using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using System.Reflection;
using ServerCommon.Protocol;
using ProtobufProto.Model;

namespace ProtobufProto
{
    public class ProtobufHandler : IProtocolHandler
    {
        public void DataHandle(byte[] data, IActionExecuter actionExecuter)
        {
            Request request = Request.Parser.ParseFrom(data);

            ExecuteContext executeContext = new ExecuteContext();
            executeContext.ControllerName = request.Controller;
            executeContext.ActionName = request.Action;

            #region 子参数解析
            if (request.SubRequest != null)
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                foreach (var type in assembly.GetTypes())
                {
                    if (type.Name == request.SubName && typeof(IMessage).IsAssignableFrom(type))
                    {
                        var uppackMethod= request.SubRequest.GetType().GetMethod(nameof(request.SubRequest.Unpack)).MakeGenericMethod(type);
                        object subMessagae= uppackMethod.Invoke(request.SubRequest, null);

                        foreach(var property in subMessagae.GetType().GetProperties(BindingFlags.Public|BindingFlags.Instance))
                        {
                            executeContext.Paramters.Add(property.Name, property.GetValue(subMessagae));
                        }

                        break;
                    }
                }
            }
            #endregion

            actionExecuter.ExecuteAction(executeContext);
        }


        /// <summary>
        /// 在指定的程序集中获取指定类型的扩展方法
        /// </summary>
        /// <param name="assembly">查找的程序集</param>
        /// <param name="extendedType">扩展方法扩展的类类型</param>
        IEnumerable<MethodInfo> GetExtensionMethods(Assembly assembly, Type extendedType)
        {
            var query = from type in assembly.GetTypes()
                        where !type.IsGenericType && !type.IsNested
                        from method in type.GetMethods(BindingFlags.Static
                            | BindingFlags.Public | BindingFlags.NonPublic)
                        where method.IsDefined(typeof(System.Runtime.CompilerServices.ExtensionAttribute), false)
                        where method.GetParameters()[0].ParameterType == extendedType
                        select method;
            return query;

        }
    }
}
