using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCommon.NetServer;

namespace ServerCommon
{
    public class UserCenter
    {
        private static readonly object lockobj = new object();
        private static Dictionary<string, AsyncUserContext> m_dictionary;

        public int Count { get {return m_dictionary.Count;  } }

        private static UserCenter instance;
        /// <summary>
        /// 获取用户中心的唯一实例
        /// </summary>
        public static UserCenter Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockobj)
                    {
                        if (instance == null)
                            instance = new UserCenter();
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// 用户登录时
        /// </summary>
        public event Action<string> OnUserLogin;
        /// <summary>
        /// 用户登出
        /// </summary>
        public event Action<string,object> OnUserLoginout;

        private UserCenter()
        {
            m_dictionary = new Dictionary<string, AsyncUserContext>();
        }


        public void UserLogout(string userName)
        {
            lock (m_dictionary)
            {
                if(!string.IsNullOrEmpty(userName) && m_dictionary.ContainsKey(userName))
                {
                    var data = m_dictionary[userName].UserData;
                    m_dictionary.Remove(userName);
                    OnUserLoginout.Invoke(userName, data);
                }
            }
        }

        public void UserLogin(string userName, AsyncUserContext loginContext)
        {
            lock (m_dictionary)
            {
                if (m_dictionary.ContainsKey(userName))
                {
                    OnUserLoginout.Invoke(userName,m_dictionary[userName].UserData);
                    //m_dictionary[userName].Server.CloseClientSocket(m_dictionary[userName]);
                    m_dictionary[userName] = loginContext;
                }
                else
                    m_dictionary.Add(userName, loginContext);
            }

            OnUserLogin?.Invoke(userName);
        }

        public bool HasUser(string userName)
        {
            if (string.IsNullOrEmpty(userName))
                return false;

            lock (m_dictionary)
            {
                return m_dictionary.ContainsKey(userName);
            }
        }


        /// <summary>
        /// 检测用户登录
        /// </summary>
        /// <param name="loginContext">登录上下文</param>
        public bool CheckUser(AsyncUserToken loginContext)
        {
            lock (m_dictionary)
            {
                return m_dictionary.Values.Any(m=>m.UserToken==loginContext);
            }
        }

        ///// <summary>
        ///// 更新用户状态
        ///// </summary>
        ///// <param name="handleThread">调用线程</param>
        //public void DetectionUserHandle(Thread handleThread)
        //{
        //    LoginContext[] array = m_dictionary.Values.ToArray<LoginContext>();
        //    for (int i = 0; i < array.Length; i++)
        //    {
        //        if (!handleThread.IsAlive)
        //            break;
        //        if ((DateTime.Now - array[i].ActiveDT).TotalMilliseconds >= 60 * 1000)
        //        {
        //            UserLogout(array[i].UserName);
        //        }
        //    }
        //}
    }
}
