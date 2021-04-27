using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DBCore;
using ServerCommon.Protocol;
using DataModel;
using TankSever.BLL.Server;

namespace TankSever.BLL.Controllers
{
    partial class EventController:Controller 
    {
        private User _user => User as User;
        //注册
        [AllowAnonymous]
        public StandRespone Register(RegisterRequest request)
        {
            //加密密码
            byte[] salt = new byte[20];
            new Random().NextBytes(salt);
            MD5Cng md5 = new MD5Cng();
            var saltpass = Encoding.UTF8.GetBytes(request.Password).Concat(salt).ToArray();
            var crpPass = md5.ComputeHash(saltpass);

            return DBServer.Instance.Regeister(request.UserName, Convert.ToBase64String(crpPass), Convert.ToBase64String(salt));
        }

        //登录
        [AllowAnonymous]
        public StandRespone Login(LoginRequest request)
        {
            var res = DBServer.Instance.GetPassword(request.UserName);
            if (res.IsSuccess)
            {
                var oldPass = res.Data.pwd;
                var salt = Convert.FromBase64String(res.Data.salt);
                var saltpass = Encoding.UTF8.GetBytes(request.Password).Concat(salt).ToArray();
                MD5Cng md5 = new MD5Cng();
                var crpPass = Convert.ToBase64String(md5.ComputeHash(saltpass));

                if (oldPass == crpPass)
                {
                    User.Login(request.UserName);

                    return new StandRespone<string> { IsSuccess = true, Message = "登录成功",Data= (User as User).LoginTimestamp };
                }
                else
                {
                    return StandRespone.FailResult("登录失败,密码错误");
                }
            }
            else
                return res;
        }

        public void Logout()
        {
            User.LoginOut();
        }


        //public List<(string,Transform)> GetPlayerTransforms()
        //{
        //    return DataCenter.Instance.GetTransforms();
        //}


        public RoomInfo[] RoomList()
        {
            return DataCenter.Rooms.GetRoomList();
        }


        public StandRespone CreateRoom(RoomSetting setting)
        {
            var user = User as User;

            if (user.RoomDetail.State != RoomUserStates.None)
                return StandRespone.FailResult("已经在房中 不能创建房间");

            var room= DataCenter.Rooms.CreateRoom(user, setting);
            (Program.BroadServer as BroadcastServer).BroadcastGlobal((BroadcastActions.CreateRoom, user.Room));
            (Program.BroadServer as BroadcastServer).BroadcastRoom((room.RoomId, BroadcastActions.RoomChange, user.RoomDetail));
            //DataCenter.BroadcastGlobalQueue.Enqueue((BroadcastActions.CreateRoom, user.Room));
            //DataCenter.BroadcastRoomQueue.Enqueue((romid,BroadcastActions.RoomChange, user.RoomDetail));
            return new StandRespone<RoomInfo>(true, "创建成功", room);
        }

        public StandRespone LeaveRoom()
        {
            var user = User as User;
            var suc = DataCenter.Rooms.LeaveRoom(user);
            (Program.BroadServer as BroadcastServer).BroadcastGlobal((BroadcastActions.LeaveRoom, user.Room));
            (Program.BroadServer as BroadcastServer).BroadcastRoom((user.Room.RoomId, BroadcastActions.RoomChange, user.RoomDetail));
            //DataCenter.BroadcastRoomQueue.Enqueue((user.Room.RoomId,BroadcastActions.RoomChange, user.RoomDetail));
            //DataCenter.BroadcastGlobalQueue.Enqueue((BroadcastActions.LeaveRoom, user.Room));
            return new StandRespone(suc);
        }

        public StandRespone JoinRoom((int roomid,string password)data)
        {
            var user = User as User;

            if (user.RoomDetail.State != RoomUserStates.None)
                return StandRespone.FailResult("已经在房中 不能加入房间");

            if (!(DataCenter.Rooms[data.roomid]?.CheckPassword(data.password) ?? false))
                return StandRespone.FailResult("房间密码错误");

            var suc = DataCenter.Rooms.JoinRoom(data.roomid, user,out Room room);
            (Program.BroadServer as BroadcastServer).BroadcastGlobal((BroadcastActions.JoinRoom, user.Room));
            (Program.BroadServer as BroadcastServer).BroadcastRoom((user.Room.RoomId, BroadcastActions.RoomChange, user.RoomDetail));
            //DataCenter.BroadcastRoomQueue.Enqueue((roomid,BroadcastActions.RoomChange,user.RoomDetail));
            //DataCenter.BroadcastGlobalQueue.Enqueue((BroadcastActions.JoinRoom, user.Room));
            return new StandRespone<RoomInfo>(suc,suc?"加入成功":"加入失败", room);
        }

        public RoomUser[] GetRoomUsers(int roomid)
        {
            return DataCenter.Rooms[roomid]?.GetUsers().Select(m=>m.RoomDetail).ToArray()??new RoomUser[0];
        }

        public StandRespone RoomReady()
        {
            var user = User as User;
            if ((user.Room as Room).RoomReady(user.RoomDetail))
            {
                (Program.BroadServer as BroadcastServer).BroadcastRoom((user.Room.RoomId, BroadcastActions.RoomChange, user.RoomDetail));
                //DataCenter.BroadcastRoomQueue.Enqueue((user.Room.RoomId,BroadcastActions.RoomChange, user.RoomDetail));
                return StandRespone.SuccessResult("准备成功");
            }
            else
                return StandRespone.FailResult("准备失败");
        }

        public StandRespone RoomCancelReady()
        {
            var user = User as User;
            if ((user.Room as Room).RoomCancelReady(user))
            {
                (Program.BroadServer as BroadcastServer).BroadcastRoom((user.Room.RoomId, BroadcastActions.RoomChange, user.RoomDetail));
                //DataCenter.BroadcastRoomQueue.Enqueue((user.Room.RoomId,BroadcastActions.RoomChange, user.RoomDetail));
                return StandRespone.SuccessResult("取消准备成功");
            }
            else
                return StandRespone.FailResult("取消准备失败");
        }

        public StandRespone RoomChangeIndex(int index)
        {
            var user = User as User;
            if ((user.Room as Room).ChangeIndex(user,index))
            {
                (Program.BroadServer as BroadcastServer).BroadcastRoom((user.Room.RoomId, BroadcastActions.RoomChange, user.RoomDetail));
                //DataCenter.BroadcastRoomQueue.Enqueue((user.Room.RoomId,BroadcastActions.RoomChange, user.RoomDetail));
                return StandRespone.SuccessResult("操作成功");
            }
            else
                return StandRespone.FailResult("操作失败");
        }



        [AllowAnonymous]
        public double CheckTime()
        {
            return Sys.GetTime();
        }
    }
}
