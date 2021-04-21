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
        public void UpdateTransform((string account,double sendtime,Transform transform,Vector3 velocity) data)
        {
            _user.BattleInfo.Trans = data.transform;
            _user.BattleInfo.velocity = data.velocity;
            _user.BattleInfo.transTime = data.sendtime;
            //(Program.BroadServer as BroadcastServer).BroadcastRoom((_user.Room.RoomId, BroadcastActions.UpdateTransform, data));
            //DataCenter.BroadcastRoomQueue.Enqueue((_user.Room.RoomId, BroadcastActions.UpdateTransform, data));
        }

        public void UpdateTurretDirection(Vector3 TargetDirection)
        {
            _user.BattleInfo.Taregtion = TargetDirection;
            (Program.BroadServer as BroadcastServer).BroadcastRoom((_user.Room.RoomId, BroadcastActions.UpdateTurretDirection,(_user.UserName,TargetDirection)));
        }

        public void Fire()
        {
            (Program.BroadServer as BroadcastServer).BroadcastRoom((_user.Room.RoomId, BroadcastActions.Fire,_user.UserName));
        }








        //广播方法调用
        public void BroadcastMethod(BroadcastMethod methodinfo)
        {
            switch(methodinfo.Flag)
            {
                case BroadcastFlag.Global:
                    (Program.BroadServer as BroadcastServer).BroadcastGlobal((BroadcastActions.BroadcastMethod, methodinfo));
                    //DataCenter.BroadcastGlobalQueue.Enqueue((BroadcastActions.BroadcastMethod, methodinfo));
                    break;
                case BroadcastFlag.Room:
                    (Program.BroadServer as BroadcastServer).BroadcastRoom((_user.Room.RoomId, BroadcastActions.BroadcastMethod, methodinfo));
                    //DataCenter.BroadcastRoomQueue.Enqueue((_user.Room.RoomId, BroadcastActions.BroadcastMethod, methodinfo));
                    break;
                case BroadcastFlag.Team:
                    (Program.BroadServer as BroadcastServer).BroadcastTeam((_user.Room.RoomId, _user.RoomDetail.Team, BroadcastActions.BroadcastMethod, methodinfo));
                    //DataCenter.BroadcastTeamQueue.Enqueue((_user.Room.RoomId, _user.RoomDetail.Team, BroadcastActions.BroadcastMethod, methodinfo));
                    break;
            }

        }

        //广播字段复制
        public void BroadcastField(string account)
        {
            (Program.BroadServer as BroadcastServer).BroadcastGlobal((BroadcastActions.BroadcastField, account));
            //DataCenter.BroadcastGlobalQueue.Enqueue((BroadcastActions.BroadcastField, account));
        }

        //广播登录
        public void Login(LoginInfo info)
        {
            (Program.BroadServer as BroadcastServer).BroadcastGlobal((BroadcastActions.Login, info));
            //DataCenter.BroadcastGlobalQueue.Enqueue((BroadcastActions.Login, info));
        }

        public void BroadcastRoomMsg(string message)
        {
            (Program.BroadServer as BroadcastServer).BroadcastRoom((_user.Room.RoomId, BroadcastActions.BroadcastRoomMsg, (_user.UserName, message)));
            //DataCenter.BroadcastRoomQueue.Enqueue((_user.Room.RoomId, BroadcastActions.BroadcastRoomMsg, (_user.UserName,message)));
        }
    }
}
