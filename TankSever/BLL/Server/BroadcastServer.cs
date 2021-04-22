﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using System.Threading;
using ServerCommon;
using DataModel;

namespace TankSever.BLL.Server
{
    class BroadcastServer :ServerBase
    {
        public override string ServerName => "广播服务";
        private static IDataFormatter _dataFormatter = DI.Instance.Resolve<IDataFormatter>();

        public BroadcastServer(INotifier notifier):base(notifier)
        {
            RunInterval = 200;
            User.OnUserLogout += Instance_OnUserLoginout;
        }

        private void Instance_OnUserLoginout(User user)
        {
            BroadcastMessage(BroadcastActions.Loginout, (user.UserName,user.LoginTimestamp));
            if (user.RoomDetail.State != RoomUserStates.None)
            {
                if (!DataCenter.Rooms.LeaveRoom(user))
                    throw new Exception("用户登出时离开房间失败");
                BroadcastMessage(BroadcastActions.LeaveRoom, user.Room);
                BroadcastRoom((user.Room.RoomId,BroadcastActions.RoomChange, user.RoomDetail));

                Notify(NotifyType.Message, user.UserName + " 用户登出 自动退出房间", this);
            }
            else
                Notify(NotifyType.Message, user.UserName + " 用户登出", this);
        }

        public override void Run()
        {
            //Thread.CurrentThread.Priority = ThreadPriority.Highest;
            try
            {
                //foreach(var rom in DataCenter.Rooms.GetRoomList())
                //{
                //    if (rom.State != RoomState.Fight)
                //        continue;
                //    foreach(var user in (rom as Room).GetUsers())
                //    {
                //        if(user.BattleInfo.Trans!=null && user.BattleInfo.velocity!=null)
                //        {
                //            BroadcastRoom((rom.RoomId, BroadcastActions.UpdateTransform,
                //                 (user.UserName, user.BattleInfo.transTime, user.BattleInfo.Trans, user.BattleInfo.velocity)));
                //        }
                //    }
                //}

                //do
                //{
                //    int index= WaitHandle.WaitAny(DataCenter.BroadcastWaitHandles,RunInterval);
                //    System.Diagnostics.Debug.WriteLine("index:"+index);
                //    if (index!=WaitHandle.WaitTimeout)
                //    {
                //        if(index==0)
                //        {
                //            var data = DataCenter.BroadcastGlobalQueue.DequeueNoWait();
                //            BroadcastGlobal(data);
                //        }
                //        else if (index == 1)
                //        {
                //            var data = DataCenter.BroadcastRoomQueue.DequeueNoWait();
                //            BroadcastRoom(data);
                //        }
                //        else if (index == 2)
                //        {
                //            var data = DataCenter.BroadcastTeamQueue.DequeueNoWait();
                //            BroadcastTeam(data);
                //        }
                //    }
                //}
                //while (IsStop);
            }
            catch(Exception e)
            {
                Notify(NotifyType.Error, "广播数据错误:" + e.Message, this);
            }
       
        }

        public void BroadcastGlobal((string action, object data) data)
        {
            BroadcastMessage(data.action, data.data);
        }

        public void BroadcastRoom((int roomid,string action, object data) data)
        {
            var room = DataCenter.Rooms[data.roomid];
            if(room != null)
            {
                var users = room.GetUsers();
                BroadcastMessage(data.action, users, data.data);
            }
        }
        public void BroadcastTeam((int roomid,int teamid,string action, object data) data)
        {
            var users = DataCenter.Rooms[data.roomid].GetUsers().Where(m=>m.RoomDetail.Team==data.teamid).ToArray();
            BroadcastMessage(data.action,users, data.data);
        }

        /// <summary>
        /// 广播消息
        /// </summary>
        public void BroadcastMessage<T>(string action,T data)
        {
            BroadcastMessage(action, null, data);
        }

        /// <summary>
        /// 广播给指定的用户数组消息
        /// </summary>
        public void BroadcastMessage<T>(string action,AsyncUser[] users, T data)
        {
            Task.Run(() =>
            {
                Respone<T> respone = new Respone<T>()
            {
                Controller = ControllerConst.Broad,
                Action = action,
                Data = data
            };
            var bytes = _dataFormatter.Serialize(respone);
            Program.NetServer.Broadcast(bytes, users);
            });
        }


    }
}
