using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankSever.BLL.Server;

namespace TankSever.BLL.Controllers
{
    partial class EventController
    {
        public StandRespone DoStartFight()
        {
            var room = _user.Room as Room;
            if (_user.RoomDetail.State == RoomUserStates.Ready && _user.RoomDetail.IsRoomOwner && room.StartFight())
            {
                (Program.BroadServer as BroadcastServer).BroadcastRoom((_user.Room.RoomId, BroadcastActions.DoStartFight, null));
                //DataCenter.BroadcastRoomQueue.Enqueue((_user.Room.RoomId, BroadcastActions.DoStartFight, null));
                return StandRespone.SuccessResult("操作成功");
            }
            else
            {
                return StandRespone.FailResult("操作失败");
            }
        }
    }
}
