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
        private Dictionary<string,UserInfo> Players = new Dictionary<string, UserInfo>();

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
            UserCenter.Instance.OnUserLoginout += Instance_OnUserLoginout;
        }

        private void Instance_OnUserLoginout(string account,object userdata)
        {
            lock(Players)
            {
                if (Players.ContainsKey(account))
                    Players.Remove(account);
            }
        }

        public void UpdateTrnasforms(string user,Transform transform)
        {
            
            lock(Players)
            {
                if (Players.ContainsKey(user))
                    Players[user].transform = transform;
                else
                    Players.Add(user, new UserInfo { transform=transform});
            }
        }

        public List<(string account, Transform trans)> GetTransforms()
        {
            List<(string,Transform)> res = new List<(string, Transform)>();
            lock(Players)
            {
                foreach (var tran in Players)
                {
                    res.Add((tran.Key, tran.Value.transform));
                }
            }
            return res;
        }

    }
}
