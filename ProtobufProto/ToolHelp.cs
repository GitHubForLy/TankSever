using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtobufProto.Model;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;

namespace ProtobufProto
{
    /// <summary>
    /// 工具类
    /// </summary>
    public static class ToolHelp
    {
        public static Respone CreateRespone(bool IsSuccess)
        {
            return new Respone { IsSuccess = IsSuccess };
        }
        public static Respone CreateRespone(bool IsSuccess, string message)
        {
            return new Respone { IsSuccess = IsSuccess, Message=message };
        }
        public static Respone CreateRespone(bool IsSuccess, IMessage data)
        {
            return new Respone { IsSuccess = IsSuccess, Data=Any.Pack(data) };
        }
        public static Respone CreateRespone(bool IsSuccess, string Message, IMessage data)
        {
            return new Respone { IsSuccess = IsSuccess, Message=Message,Data = Any.Pack(data) };
        }
    }
}
