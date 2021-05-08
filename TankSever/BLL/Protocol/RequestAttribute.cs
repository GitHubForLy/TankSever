using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtobufProto.Model;

namespace TankSever.BLL.Protocol
{
    /// <summary>
    /// 表示请求的方法
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class RequestAttribute:Attribute
    {
        public TcpFlags ReqestType { get; }
        public RequestAttribute(TcpFlags type)
        {
            ReqestType = type;
        }
    }
}
