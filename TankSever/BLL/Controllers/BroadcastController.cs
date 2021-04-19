using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using ServerCommon.Protocol;
using TankSever.BLL.Server;

namespace TankSever.BLL.Controllers
{
    class BroadcastController : Controller
    {
        private User _user => User as User;
        //广播位置信息
        public void UpdateTransform((string account,Transform transform) data)
        {
            DataCenter.BroadcastRoomQueue.Enqueue((_user.Room.RoomId, BroadcastActions.UpdateTransform, data));
        }

        //广播方法调用
        public void BroadcastMethod(BroadcastMethod methodinfo)
        {
            switch(methodinfo.Flag)
            {
                case BroadcastFlag.Global:
                    DataCenter.BroadcastGlobalQueue.Enqueue((BroadcastActions.BroadcastMethod, methodinfo));
                    break;
                case BroadcastFlag.Room:
                    DataCenter.BroadcastRoomQueue.Enqueue((_user.Room.RoomId, BroadcastActions.BroadcastMethod, methodinfo));
                    break;
                case BroadcastFlag.Team:
                    DataCenter.BroadcastTeamQueue.Enqueue((_user.Room.RoomId, _user.RoomDetail.Team, BroadcastActions.BroadcastMethod, methodinfo));
                    break;
            }

        }

        //广播字段复制
        public void BroadcastField(string account)
        {
            DataCenter.BroadcastGlobalQueue.Enqueue((BroadcastActions.BroadcastField, account));
        }

        //广播登录
        public void Login(LoginInfo info)
        {
            DataCenter.BroadcastGlobalQueue.Enqueue((BroadcastActions.Login, info));
        }

        public void BroadcastRoomMsg(string message)
        {
            DataCenter.BroadcastRoomQueue.Enqueue((_user.Room.RoomId, BroadcastActions.BroadcastRoomMsg, (_user.UserName,message)));
        }
    }
}
