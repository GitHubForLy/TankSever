using ServerCommon.NetServer;
using ServerCommon.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommon
{
    /// <summary>
    /// 表示一个异步网络用户
    /// </summary>
    public class AsyncUser
    {
        internal AsyncUserToken UserToken { get; set; }
        private bool _isClosed = false;

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// 是否登录
        /// </summary>
        //public bool IsLogined => UserCenter.Instance.CheckUser(UserToken);
        public bool IsLogined { get; private set; }



        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="UserName"></param>
        public virtual void Login(string UserName)
        {
            this.UserName = UserName;
            //UserCenter.Instance.UserLogin(UserName, this);
            IsLogined = true;
        }

        /// <summary>
        /// 登出
        /// </summary>
        public virtual void LoginOut()
        {
            IsLogined = false;
            //UserCenter.Instance.UserLogout(UserName);
            //UserName = null;
            //CloseConnect();
        }

        /// <summary>
        /// 向当前用户发送数据
        /// </summary>
        public void SendMessage(byte[] data)
        {
            UserToken.Server.Broadcast(data, new AsyncUserToken[] { UserToken }, false);
        }

        private void CloseConnect()
        {
            if (!_isClosed)
            {
                UserToken.Server.CloseClientSocket(UserToken);
                _isClosed = true;
            }
        }


    }
}
