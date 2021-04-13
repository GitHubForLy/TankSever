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
        private static Dictionary<string, AsyncUser> m_dictionary;

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
        /// 用户数组
        /// </summary>
        public AsyncUser[] Users => m_dictionary.Values.ToArray();


        /// <summary>
        /// 用户登录时
        /// </summary>
        public event Action<string> OnUserLogin;
        /// <summary>
        /// 用户登出
        /// </summary>
        public event Action<string,AsyncUser> OnUserLoginout;

        private UserCenter()
        {
            m_dictionary = new Dictionary<string, AsyncUser>();
        }

        /// <summary>
        /// 用户登出
        /// </summary>
        /// <param name="userName"></param>
        public void UserLogout(string userName)
        {
            lock (m_dictionary)
            {
                if(!string.IsNullOrEmpty(userName) && m_dictionary.ContainsKey(userName))
                {
                    var user = m_dictionary[userName];
                    m_dictionary.Remove(userName);
                    OnUserLoginout.Invoke(userName, user);
                }
            }
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        public void UserLogin(string userName, AsyncUser user)
        {
            lock (m_dictionary)
            {
                if (m_dictionary.ContainsKey(userName))
                {
                    OnUserLoginout.Invoke(userName,m_dictionary[userName]);
                    m_dictionary[userName] = user;
                }
                else
                    m_dictionary.Add(userName, user);
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


        public AsyncUser this[string account] 
        {
            get
            {
                if (string.IsNullOrEmpty(account))
                    return null;
                lock(m_dictionary)
                {
                    if (!m_dictionary.ContainsKey(account))
                        return null;
                    return m_dictionary[account];
                }
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
