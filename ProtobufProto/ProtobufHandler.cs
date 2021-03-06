using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using System.Reflection;
using ServerCommon.Protocol;
using ProtobufProto.Model;
using Google.Protobuf.WellKnownTypes;
using System.Data;
using ServerCommon.NetServer;

namespace ProtobufProto
{
    /// <summary>
    /// protobuf数据协议处理类
    /// </summary>
    public class ProtobufHandler : IProtocolHandler
    {
        /// <summary>
        /// 请求的数据处理
        /// </summary>
        /// <param name="userToken">用户连接对象</param>
        /// <param name="data">请求数据及响应数据</param>
        /// <param name="actionExecuter"></param>
        /// <returns>是否有数据响应 true代表有相应数据并由data传出 false代表没有相应</returns>
        public bool DataHandle(AsyncUserToken userToken,ref byte[] data, IActionExecuter actionExecuter)
        {
            Request request = Request.Parser.ParseFrom(data);

            ProtobufExecuteContext executeContext = new ProtobufExecuteContext();
            executeContext.ControllerName = request.Controller;
            executeContext.ActionName = request.Action;
            executeContext.UserToken = userToken;

            #region 子参数解析
            if (request.SubRequest != null)
            {
                Assembly assembly = Assembly.GetExecutingAssembly();
                var type=assembly.GetType(Any.GetTypeName(request.SubRequest.TypeUrl));
                if(type!=null /*&& request.SubRequest.Is((type as IMessage).Descriptor)*/)
                {
                    var uppackMethod = request.SubRequest.GetType().GetMethod(nameof(request.SubRequest.Unpack)).MakeGenericMethod(type);
                    IMessage subMessagae = uppackMethod.Invoke(request.SubRequest, null) as IMessage;
                    executeContext.SubMessage = subMessagae;
                }
            }
            #endregion

            var has= actionExecuter.TryExecuteAction(executeContext,out object obj);
            if(has)
            {
                if (obj == null)
                    return false;
            }
            else
            {
                //没有找到action
                obj = "无效的请求";
            }

            var respone = MakeRespone(request,obj);
            data = respone.ToByteArray();
            return true;
        }

        /// <summary>
        /// 构造返回消息
        /// </summary>
        /// <param name="request">请求消息</param>
        /// <param name="obj">响应对象</param>
        protected IMessage MakeRespone(Request request,object obj)
        {
            Respone respone = new Respone();
            respone.Controller = request.Controller;
            respone.Action = request.Action;
            respone.IsSuccess = false;

            if (obj == null)
                return respone;

            if (obj is Respone retRes)
            {
                respone.IsSuccess = retRes.IsSuccess;
                respone.Data = retRes.Data;
                respone.Message = retRes.Message;
            }
            else if(obj is IMessage megRes)
            {
                respone.Data = Any.Pack(megRes);
            }
            else
            {
                respone.IsSuccess = false;
                respone.Message = obj.ToString();
            }

            return respone;             
        }



        /// <summary>
        /// 在指定的程序集中获取指定类型的扩展方法
        /// </summary>
        /// <param name="assembly">查找的程序集</param>
        /// <param name="extendedType">扩展方法扩展的类类型</param>
        IEnumerable<MethodInfo> GetExtensionMethods(Assembly assembly, System.Type extendedType)
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
