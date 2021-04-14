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


        public static UserManager Users { get; } = new UserManager();

        public static RoomManager Rooms { get; } = new RoomManager();


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
