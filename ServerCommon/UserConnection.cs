using ServerCommon.NetServer;
using ServerCommon.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommon
{
    public class AsyncContext
    {
        private bool isClosed;
        private ExecuteContext executeContext;
        private AsyncUserToken UserToken => executeContext.UserToken;

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; private set; }
        /// <summary>
        /// 所属服务
        /// </summary>
        public AsyncSocketServerBase NetServer => UserToken.Server;
        /// <summary>
        /// 是否登录
        /// </summary>
        public bool IsLogined=>UserCenter.Instance.CheckUser(UserToken);

        public string Controller => executeContext.ControllerName;
        public string Action => executeContext.ActionName;


        public AsyncContext(ExecuteContext Context)
        {
            executeContext = Context;
        }


        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="UserName"></param>
        public void Login(string UserName)
        {
            this.UserName = UserName;
            UserCenter.Instance.UserLogin(UserName, UserToken);
        }

        /// <summary>
        /// 登出
        /// </summary>
        public void LoginOut()
        {
            UserCenter.Instance.UserLogout(UserName);
            this.UserName = null;
            Close();
        }

        private void Close()
        {
            if (!isClosed)
            {
                UserToken.Server.CloseClientSocket(UserToken);
                isClosed = true;
            }
        }


    }
}
