using ServerCommon.NetServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommon.Protocol
{
    public abstract class ProtocolHandlerFactoryBase : IProtocolHandlerFactory
    {
        private AsyncUserToken _token;
        public IProtocolHandler CreateProtocolHandler(AsyncUserToken token)
        {
            _token = token;
            var proto= GetProtocolHandler();
            PropertyInit(proto);
            return proto;
        }

        public abstract ProtocolHandlerBase GetProtocolHandler();

        protected virtual void PropertyInit(ProtocolHandlerBase protocol)
        {
            protocol.AsyncToken = _token;
        }
    }
}
