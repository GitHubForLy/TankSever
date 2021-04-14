using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using ServerCommon;

namespace TankSever.BLL
{
    public class DataCenter
    {
        private static DataCenter instance;
        private static readonly object lockobj = new object();

        private Dictionary<string,User> Users = new Dictionary<string, User>();
        private List<Room> roomList = new List<Room>();

        /// <summary>
        /// 用户当前数量
        /// </summary>
        public int UserCount => Users.Count;
        /// <summary>
        /// 房间数量
        /// </summary>
        public int RoomCount => roomList.Count;

        /// <summary>
        /// 用户登出事件 
        /// </summary>
        public event Action<string, AsyncUser> OnUserLoginout;

        /// <summary>
        /// 唯一实例
        /// </summary>
        public static DataCenter Instance
        {
            get
            {
                if(instance==null)
                {
                    lock(lockobj)
                    {
                        if (instance == null)
                            instance = new DataCenter();
                    }
                }
                return instance;
            }
        }


        private DataCenter()
        {
        }


        public void AddUser(User user)
        {
            lock(Users)
            {
                if (Users.ContainsKey(user.UserName))
                    Users[user.UserName] = user;
                else
                    Users.Add(user.UserName, user);
            }
        }

        public bool RemoveUser(string userName)
        {
            lock(Users)
            {
                if (Users.ContainsKey(userName))
                {
                    OnUserLoginout?.Invoke(userName, Users[userName]);
                    return Users.Remove(userName);
                }
                return false;
            }
        }

        public void UpdateTrnasforms(string user,Transform transform)
        {           
            lock(Users)
            {
                if (Users.ContainsKey(user))
                    Users[user].BattleInfo.Trans = transform;
            }
        }

        public List<(string account, Transform trans)> GetTransforms()
        {
            List<(string,Transform)> res = new List<(string, Transform)>();
            lock(Users)
            {
                foreach (var tran in Users)
                {
                    res.Add((tran.Key, tran.Value.BattleInfo.Trans));
                }
            }
            return res;
        }

    }
}
