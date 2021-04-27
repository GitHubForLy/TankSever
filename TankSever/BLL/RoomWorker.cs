using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using ServerCommon;
using TankSever.BLL.Server;

namespace TankSever.BLL
{
    class RoomWorker : ServerBase
    {
        public override string ServerName => "房间处理服务";

        public RoomWorker(INotifier notifier):base(notifier)
        {
            this.RunInterval = 1000;
        }

        public override void Run()
        {
            Room[] rooms= (Room[])DataCenter.Rooms.GetRoomList();
            foreach(var room in rooms)
            {
                if(room.State== DataModel.RoomState.Fight)
                {
                    //广播游戏时间
                    var remain = room.GetRemainingTime();
                    (Program.BroadServer as BroadcastServer).BroadcastRoom((room.RoomId, BroadcastActions.RemainingTime, remain));

                    //判断游戏结束 并广播
                    if(room.CheckGameFinished(out int team))
                    {
                        room.DoFinished();
                        (Program.BroadServer as BroadcastServer).BroadcastGlobal((BroadcastActions.RoomWaitting, room));
                        (Program.BroadServer as BroadcastServer).BroadcastRoom((room.RoomId, BroadcastActions.GameFinished, team));
                    }
                }
            }
        }
    }

}
