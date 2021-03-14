using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankSever.BLL
{
    public class UserCenter
    {
        private static readonly object lockobj = new object();
        private static Dictionary<string, LoginContext> m_dictionary;

        public int Count { get { return m_dictionary.Count; } }

        private static UserCenter instance;
        /// <summary>
        /// 获取用户中心的唯一实例
        /// </summary>
        public static UserCenter Instance
        {
            get
            {
                if(instance==null)
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

        private UserCenter()
        {
            m_dictionary = new Dictionary<string, LoginContext>();
        }


        public void UserLogout(string userName)
        {
            Remove(userName);
        }

        public void UserLogin(string userName, string userPwd)
        {
            if (HasUser(userName))
            {//已登录
                Remove(userName);
            }
            LoginContext loginedContext = new LoginContext();
            loginedContext.UserName = userName;
            loginedContext.UserPwd = userPwd;
            loginedContext.LoginedDT = DateTime.Now;
            loginedContext.ActiveDT = DateTime.Now;
            Add(loginedContext.UserName, loginedContext);

            OnUserLogin?.Invoke(userName);
        }

        /// <summary>
        /// 是否存在用户
        /// </summary>
        public bool HasUser(string useernam)
        {
            return m_dictionary.ContainsKey(useernam);
        }

        /// <summary>
        /// 检测用户登录
        /// </summary>
        public bool CheckUser(string userName, string userPwd)
        {
            //用户没登录，返回false
            if (!HasUser(userName))
                return false;

            LoginContext loginedContext = TryGetContext(userName);
            loginedContext.ActiveDT = DateTime.Now;
            return loginedContext.UserPwd.Equals(userPwd);
        }

        /// <summary>
        /// 更新用户状态
        /// </summary>
        /// <param name="handleThread">调用线程</param>
        public void DetectionUserHandle(Thread handleThread)
        {
            LoginContext[] array = m_dictionary.Values.ToArray<LoginContext>();
            for (int i = 0; i < array.Length; i++)
            {
                if (!handleThread.IsAlive)
                    break;
                if ((DateTime.Now - array[i].ActiveDT).TotalMilliseconds >= 60 * 1000)
                {
                    UserLogout(array[i].UserName);
                }
            }
        }

        private void Add(string key, LoginContext context)
        {
            if (null == context)
                throw new ArgumentException("context added to a AsyncSocketLoginedList cannot be null");
            lock (m_dictionary)
            {
                m_dictionary.Add(key, context);
            }
        }

        private void Remove(string key)
        {
            lock (m_dictionary)
            {
                m_dictionary.Remove(key);
            }
        }



        private bool ContainsContext(LoginContext context)
        {
            return m_dictionary.ContainsValue(context);
        }

        private LoginContext TryGetContext(string key)
        {
            return m_dictionary[key];
        }

    }
}
