using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using System.Threading;
using ServerCommon;
//using DataModel;
using ProtobufProto.Model;
using Google.Protobuf;

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
            BroadcastMessage(TcpFlags.TcpLogout, new LogoutInfo { UserAccount = user.UserAccount, Timestamp = user.LoginTimestamp });
            if (user.RoomDetail.State != RoomUserStates.None)
            {
                if (!DataCenter.Rooms.LeaveRoom(user))
                    throw new Exception("用户登出时离开房间失败");

                BroadcastMessage(TcpFlags.TcpBdLeaveRoom, user.Room.Info);
                BroadcastRoom((user.Room.Info.RoomId, TcpFlags.TcpRoomChange, user.RoomDetail));

                Notify(NotifyType.Message, user.UserAccount + " 用户登出 自动退出房间", this);
            }
            else
                Notify(NotifyType.Message, user.UserAccount + " 用户登出", this);
        }

        public override void Run()
        {
            Thread.CurrentThread.Priority = ThreadPriority.AboveNormal;
            try
            {
                foreach (var rom in DataCenter.Rooms.GetRoomList())
                {
                    if (rom.Info.State != RoomState.RoomFight)
                        continue;

                    PlayerTransformMap map = new PlayerTransformMap();
                    foreach(var user in  rom.GetUsers())
                    {
                        map.Transforms.Add(user.UserAccount, user.BattleInfo.Trans);
                    }
                    BroadcastRoom((rom.Info.RoomId, TcpFlags.TcpUpdateTransform, map));
                }

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

        public void BroadcastGlobal((TcpFlags action, ProtoBuf.IExtensible data) data)
        {
            BroadcastMessage(data.action, data.data);
        }

        public void BroadcastRoom((int roomid, TcpFlags action, ProtoBuf.IExtensible data) data)
        {
            var room = DataCenter.Rooms[data.roomid];
            if(room != null)
            {
                var users = room.GetUsers();
                BroadcastMessage(data.action, users, data.data);
            }
        }
        public void BroadcastTeam((int roomid,int teamid,TcpFlags action, ProtoBuf.IExtensible data) data)
        {
            var users = DataCenter.Rooms[data.roomid].GetUsers().Where(m=>m.RoomDetail.Team==data.teamid).ToArray();
            BroadcastMessage(data.action,users, data.data);
        }

        /// <summary>
        /// 广播消息
        /// </summary>
        public void BroadcastMessage<T>(TcpFlags action,T data) where T: ProtoBuf.IExtensible
        {
            BroadcastMessage(action, null, data);
        }

        /// <summary>
        /// 广播给指定的用户数组消息
        /// </summary>
        public void BroadcastMessage<T>(TcpFlags action,AsyncUser[] users, T data)
        {
            Task.Run(() =>
            {
                var bytes=    ProtobufDataPackage.PackageData(action, _dataFormatter.Serialize(data));

                Program.NetServer.Broadcast(bytes, users);
            });
        }


    }
}
