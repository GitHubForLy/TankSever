using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DBCore;
using ServerCommon.Protocol;
using DataModel;

namespace TankSever.BLL.Controllers
{
    class EventController:Controller 
    {
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

                    return new StandRespone { IsSuccess = true, Message = "登录成功|" + (User as User).LoginTimestamp };
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


        public StandRespone CreateRoom(string Name)
        {
            var user = User as User;

            if (user.RoomDetail.State != RoomUserStates.None)
                return StandRespone.FailResult("已经在房中 不能创建房间");

            var romid= DataCenter.Rooms.CreateRoom(Name, user, out int team,out int index);
            DataCenter.BroadcastGlobalQueue.Enqueue((BroadcastActions.CreateRoom, user.Room));
            DataCenter.BroadcastRoomQueue.Enqueue((romid,BroadcastActions.RoomChange, user.RoomDetail));
            return new StandRespone<int>(true, "创建成功", romid);
        }

        public StandRespone LeaveRoom()
        {
            var user = User as User;
            var suc = DataCenter.Rooms.LeaveRoom(user);
            DataCenter.BroadcastRoomQueue.Enqueue((user.Room.RoomId,BroadcastActions.RoomChange, user.RoomDetail));
            DataCenter.BroadcastGlobalQueue.Enqueue((BroadcastActions.LeaveRoom, user.Room));
            return new StandRespone(suc);
        }

        public StandRespone JoinRoom(int roomid)
        {
            var user = User as User;

            if (user.RoomDetail.State != RoomUserStates.None)
                return StandRespone.FailResult("已经在房中 不能加入房间");

            var suc = DataCenter.Rooms.JoinRoom(roomid, user, out int team, out int index);
            DataCenter.BroadcastRoomQueue.Enqueue((roomid,BroadcastActions.RoomChange,user.RoomDetail));
            DataCenter.BroadcastGlobalQueue.Enqueue((BroadcastActions.JoinRoom, user.Room));
            return new StandRespone<int>(suc,suc?"加入成功":"加入失败", roomid);
        }

        public RoomUser[] GetRoomUsers(int roomid)
        {
            return DataCenter.Rooms[roomid]?.GetUsers().Select(m=>m.RoomDetail).ToArray();
        }

        public StandRespone RoomReady()
        {
            var user = User as User;
            if ((user.Room as Room).RoomReady(user.RoomDetail))
            {
                DataCenter.BroadcastRoomQueue.Enqueue((user.Room.RoomId,BroadcastActions.RoomChange, user.RoomDetail));
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
                DataCenter.BroadcastRoomQueue.Enqueue((user.Room.RoomId,BroadcastActions.RoomChange, user.RoomDetail));
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
                DataCenter.BroadcastRoomQueue.Enqueue((user.Room.RoomId,BroadcastActions.RoomChange, user.RoomDetail));
                return StandRespone.SuccessResult("操作成功");
            }
            else
                return StandRespone.FailResult("操作失败");
        }
    }
}
