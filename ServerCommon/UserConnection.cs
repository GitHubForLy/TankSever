using ServerCommon.NetServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommon
{
    public class UserConnection
    {
        private AsyncUserToken asyncUserToken;
        private bool isClosed;
        public string UserName { get; private set; }

        /// <summary>
        /// 是否登录
        /// </summary>
        public bool IsLogined=>UserCenter.Instance.CheckUser(asyncUserToken);


        public UserConnection(AsyncUserToken userToken)
        {
            asyncUserToken = userToken;
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="UserName"></param>
        public void Login(string UserName)
        {
            this.UserName = UserName;
            UserCenter.Instance.UserLogin(UserName,asyncUserToken);
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
                asyncUserToken.Server.CloseClientSocket(this.asyncUserToken);
                isClosed = true;
            }
        }
    }
}
