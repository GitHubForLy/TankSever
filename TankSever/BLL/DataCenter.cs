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
        //private static DataCenter instance;
        //private static readonly object lockobj = new object();

        ///// <summary>
        ///// 唯一实例
        ///// </summary>
        //public static DataCenter Instance
        //{
        //    get
        //    {
        //        if(instance==null)
        //        {
        //            lock(lockobj)
        //            {
        //                if (instance == null)
        //                    instance = new DataCenter();
        //            }
        //        }
        //        return instance;
        //    }
        //}


        public static UserManager Users { get; private set; } = new UserManager();

        public static RoomManager Rooms { get; private set; } = new RoomManager();

        public static SemaphoreQueue<(string action, object data)> BroadcastQueue { get; private set; } 

        /// <summary>
        /// 初始化数据
        /// </summary>
        public static void Init()
        {
            Users= new UserManager();
            Rooms = new RoomManager();
            BroadcastQueue = new SemaphoreQueue<(string action, object data)>(1000);
        }


        //public List<(string account, Transform trans)> GetTransforms()
        //{
        //    List<(string, Transform)> res = new List<(string, Transform)>();
        //    lock (Users)
        //    {
        //        foreach (var tran in Users)
        //        {
        //            res.Add((tran.Key, tran.Value.BattleInfo.Trans));
        //        }
        //    }
        //    return res;
        //}

    }
}
