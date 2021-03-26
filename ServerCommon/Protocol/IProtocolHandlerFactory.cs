using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCommon.NetServer;

namespace ServerCommon.Protocol
{
    public interface IProtocolHandlerFactory
    {
         IProtocolHandler CreateProtocolHandler(AsyncUserToken token);
    }
}
