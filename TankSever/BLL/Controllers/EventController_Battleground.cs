using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankSever.BLL.Server;
using ProtobufProto.Model;
using TankSever.BLL.Protocol;

namespace TankSever.BLL.Controllers
{
    partial class EventController
    {
        [Request(TcpFlags.TcpDoStartFight)]
        public Respone  DoStartFight()
        {
            var room = _user.Room as Room;
            if (_user.RoomDetail.State == RoomUserStates.Ready && _user.RoomDetail.IsRoomOwner && room.StartFight())
            {
                (Program.BroadServer as BroadcastServer).BroadcastRoom((_user.Room.Info.RoomId, TcpFlags.TcpBdDoStartFight, null));
                (Program.BroadServer as BroadcastServer).BroadcastGlobal((TcpFlags.TcpRoomFight, room.Info));
                return new Respone() { IsSuccess = true, Message = "操作成功" };
            }
            else
            {
                return new Respone() { IsSuccess = false, Message = "操作失败" };
            }
        }
    }
}
