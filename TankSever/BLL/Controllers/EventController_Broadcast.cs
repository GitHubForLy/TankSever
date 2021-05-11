using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCommon.Protocol;
using TankSever.BLL.Server;
using ProtobufProto.Model;

namespace TankSever.BLL.Controllers
{
    partial class EventController : Controller
    {
        //广播位置信息
        public void UpdateTransform(Transform transform)
        {
            _user.BattleInfo.Trans = transform;

            //(Program.BroadServer as BroadcastServer).BroadcastRoom((_user.Room.RoomId, BroadcastActions.UpdateTransform, data));

            //DataCenter.BroadcastRoomQueue.Enqueue((_user.Room.RoomId, BroadcastActions.UpdateTransform, data));
        }

        //public void UpdateTurretDirection(Vector3 TargetDirection)
        //{
        //    _user.BattleInfo.Taregtion = TargetDirection;
        //    (Program.BroadServer as BroadcastServer).BroadcastRoom((_user.Room.Info.RoomId, TcpFlags.t,(_user.UserAccount, TargetDirection)));
        //}

        //public void Fire()
        //{
        //    (Program.BroadServer as BroadcastServer).BroadcastRoom((_user.Room.RoomId, BroadcastActions.Fire,_user.UserAccount));
        //}

        //public void TakeDamage(float damage)
        //{         
        //    (Program.BroadServer as BroadcastServer).BroadcastRoom((_user.Room.RoomId, BroadcastActions.TakeDamage, (_user.UserAccount, damage)));
        //}


        //public void Die(string killAccount)
        //{
        //    (_user.Room as Room).KillUser(_user, DataCenter.Users[killAccount]);
        //    (Program.BroadServer as BroadcastServer).BroadcastRoom((_user.Room.RoomId, BroadcastActions.Die, (_user.UserAccount, killAccount)));
        //}



        ////广播方法调用
        //public void BroadcastMethod(BroadcastMethod methodinfo)
        //{
        //    switch(methodinfo.Flag)
        //    {
        //        case BroadcastFlag.Global:
        //            (Program.BroadServer as BroadcastServer).BroadcastGlobal((BroadcastActions.BroadcastMethod, methodinfo));
        //            //DataCenter.BroadcastGlobalQueue.Enqueue((BroadcastActions.BroadcastMethod, methodinfo));
        //            break;
        //        case BroadcastFlag.Room:
        //            (Program.BroadServer as BroadcastServer).BroadcastRoom((_user.Room.RoomId, BroadcastActions.BroadcastMethod, methodinfo));
        //            //DataCenter.BroadcastRoomQueue.Enqueue((_user.Room.RoomId, BroadcastActions.BroadcastMethod, methodinfo));
        //            break;
        //        case BroadcastFlag.Team:
        //            (Program.BroadServer as BroadcastServer).BroadcastTeam((_user.Room.RoomId, _user.RoomDetail.Team, BroadcastActions.BroadcastMethod, methodinfo));
        //            //DataCenter.BroadcastTeamQueue.Enqueue((_user.Room.RoomId, _user.RoomDetail.Team, BroadcastActions.BroadcastMethod, methodinfo));
        //            break;
        //    }

        //}

        ////广播字段复制
        //public void BroadcastField(string account)
        //{
        //    (Program.BroadServer as BroadcastServer).BroadcastGlobal((BroadcastActions.BroadcastField, account));
        //    //DataCenter.BroadcastGlobalQueue.Enqueue((BroadcastActions.BroadcastField, account));
        //}

        ////广播登录
        //public void Login(LoginInfo info)
        //{
        //    (Program.BroadServer as BroadcastServer).BroadcastGlobal((BroadcastActions.Login, info));
        //    //DataCenter.BroadcastGlobalQueue.Enqueue((BroadcastActions.Login, info));
        //}

    }
}
