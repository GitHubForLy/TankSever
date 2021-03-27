using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCommon.Protocol;

namespace TankSever.BLL.Protocol
{
    public class ProtocolHandlerFactory : ProtocolHandlerFactoryBase
    {
        public override ProtocolHandlerBase GetProtocolHandler()
        {
            return new ProtocolHandler();
        }
    }
}
