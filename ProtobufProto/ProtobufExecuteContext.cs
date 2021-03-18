using ServerCommon.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;

namespace ProtobufProto
{
    public class ProtobufExecuteContext: ExecuteContext
    {
        /// <summary>
        /// 子请求消息
        /// </summary>
        public IMessage SubMessage { get; set; }
    }
}
