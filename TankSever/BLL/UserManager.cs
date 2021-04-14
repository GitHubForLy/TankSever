﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankSever.BLL
{
    public class UserManager
    {
        private Dictionary<string, User> Users = new Dictionary<string, User>();

        /// <summary>
        /// 用户当前数量
        /// </summary>
        public int UserCount => Users.Count;

        /// <summary>
        /// 用户登出事件 
        /// </summary>
        public event Action<string, User> OnUserLoginout;


        public void AddUser(User user)
        {
            lock (Users)
            {
                if (Users.ContainsKey(user.UserName))
                    Users[user.UserName] = user;
                else
                    Users.Add(user.UserName, user);
            }
        }

        public bool HasUser(string UserName)
        {
            return Users.ContainsKey(UserName);
        }

        public bool RemoveUser(string userName)
        {
            lock (Users)
            {
                if (Users.ContainsKey(userName))
                {
                    OnUserLoginout?.Invoke(userName, Users[userName]);
                    return Users.Remove(userName);
                }
                return false;
            }
        }
    }
}
