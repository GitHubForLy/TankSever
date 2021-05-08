using ServerCommon.NetServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommon.Protocol
{
    public abstract class ProtocolHandlerBase : IProtocolHandler
    {
        private bool _hasRespone;
        private byte[] _respone;

        internal AsyncUserToken AsyncToken;

        //执行请求
        public void DoRequest(byte[] req)
        {
            var contro= CreateController(req);
            if(contro==null)
            {
                _hasRespone = false;
                return;
            }

            ControllerPropertyInit(contro);
            _hasRespone= TryExecuteAction(contro, out _respone);
            ReleaseController(contro);
        }

        /// <summary>
        /// 尝试获取响应
        /// </summary>
        /// <param name="res"></param>
        /// <returns>有相应则返回true否则返回true</returns>
        public virtual bool TryGetRespone(out byte[] res)
        {
            if (_hasRespone)
                res = _respone;
            else
                res = null;
            return _hasRespone;
        }


        public virtual void ControllerPropertyInit(IController controller)
        {
            controller.User = AsyncToken.User;
        }

        /// <summary>
        /// 创建控制器
        /// </summary>
        /// <param name="RequestObj">请求对象</param>
        public abstract IController CreateController(byte[] requestData);

        public abstract bool TryExecuteAction(IController controller, out byte[] res);

        /// <summary>
        /// 释放控制器
        /// </summary>
        /// <param name="controller"></param>
        public virtual void ReleaseController(IController controller)
        {            
        }

        protected void Notify(NotifyType notifyType,string message)
        {
            AsyncToken.Server.Notify(notifyType, message, AsyncToken.Server);
        }
    }
}
