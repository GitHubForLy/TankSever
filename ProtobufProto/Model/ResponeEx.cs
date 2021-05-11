using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;

namespace ProtobufProto.Model
{
    [ProtoInclude(1,typeof(Respone<>))]
    public partial class Respone
    {

    }

    [ProtoContract]
    public class Respone<T>
    {
        [ProtoMember(1)]
        public bool IsSuccess { get; set; } = false;

        [ProtoMember(2)]
        public string Message { get; set; } = "";

        [ProtoMember(5)]
        public T Data { get; set; }
    }
}
