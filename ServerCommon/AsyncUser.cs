using ServerCommon.NetServer;
using ServerCommon.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommon
{
    public class AsyncUser
    {
        private AsyncUserToken _userToken;
        private bool _isClosed = false;

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; private set; }

        /// <summary>
        /// 是否登录
        /// </summary>
        public bool IsLogined => UserCenter.Instance.HasUser(UserName);



        public AsyncUser(AsyncUserToken Token)
        {
            _userToken = Token;
        }


        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="UserName"></param>
        public void Login(string UserName)
        {
            this.UserName = UserName;
            UserCenter.Instance.UserLogin(UserName, _userToken);
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
            if (!_isClosed)
            {
                _userToken.Server.CloseClientSocket(_userToken);
                _isClosed = true;
            }
        }


    }
}
