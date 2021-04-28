using System;
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

        public void AddUser(User user)
        {
            lock (Users)
            {
                if (Users.ContainsKey(user.UserAccount))
                {
                    Users[user.UserAccount] = user;
                }
                else
                    Users.Add(user.UserAccount, user);
            }
        }

        public bool HasUser(string UserAccount)
        {
            return Users.ContainsKey(UserAccount);
        }

        public bool RemoveUser(string UserAccount)
        {
            lock (Users)
            {
                if (Users.ContainsKey(UserAccount))
                {
                    return Users.Remove(UserAccount);
                }
                return false;
            }
        }

        public User this[string UserAccount]
        {
            get
            {
                lock(Users)
                {
                    if (!Users.ContainsKey(UserAccount))
                        return null;
                    return Users[UserAccount];
                }
            }
        }
    }
}
