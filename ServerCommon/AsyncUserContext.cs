using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCommon.NetServer;

namespace ServerCommon
{
    public class AsyncUserContext
    {
        public AsyncUserContext(AsyncUserToken token) => UserToken = token;
        public AsyncUserToken UserToken { get; }
        public object UserData { get; set; }
    }
}
