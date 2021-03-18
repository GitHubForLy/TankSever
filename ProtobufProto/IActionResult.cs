using ProtobufProto.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtobufProto
{
    /// <summary>
    /// 行为结果
    /// </summary>
    public interface IActionResult
    {
        /// <summary>
        /// 获取protobuf的标准响应消息
        /// </summary>
        /// <returns></returns>
        Respone GetRespone();    
    }
}
