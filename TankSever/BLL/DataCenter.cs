using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using ServerCommon;
using System.Threading;

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

        public static SemaphoreQueue<(string action, object data)> BroadcastGlobalQueue { get; private set; }
        public static SemaphoreQueue<(int roomid,string action, object data)> BroadcastRoomQueue { get; private set; }
        public static SemaphoreQueue<(int roomid,int teamid,string action, object data)> BroadcastTeamQueue { get; private set; }

        public static WaitHandle[] BroadcastWaitHandles { get; private set; }

        /// <summary>
        /// 初始化数据
        /// </summary>
        public static void Init()
        {
            Users= new UserManager();
            Rooms = new RoomManager();
            BroadcastGlobalQueue = new SemaphoreQueue<(string action, object data)>(1000);
            BroadcastRoomQueue = new SemaphoreQueue<(int roomid, string action, object data)>(1000);
            BroadcastTeamQueue = new SemaphoreQueue<(int roomid, int teamid, string action, object data)>(100);
            BroadcastWaitHandles = new WaitHandle[] { BroadcastGlobalQueue.Semaphore, BroadcastRoomQueue.Semaphore, BroadcastTeamQueue.Semaphore };
        }

    }
}
