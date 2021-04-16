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

            if (user.UserState != UserStates.None)
                return StandRespone.FailResult("已经在房中 不能创建房间");

            DataCenter.Rooms.CreateRoom(Name, user, out int team,out int index);
            DataCenter.BroadcastQueue.Enqueue((BroadcastActions.CreateRoom, user.Room));
            DataCenter.BroadcastQueue.Enqueue((BroadcastActions.RoomChange, user.RoomDetail));
            return new StandRespone<(int, int)>(true, "创建成功", (team, index));
        }

        public StandRespone LeaveRoom()
        {
            var user = User as User;
            var suc = DataCenter.Rooms.LeaveRoom(user);
            DataCenter.BroadcastQueue.Enqueue((BroadcastActions.RoomChange, user.RoomDetail));
            DataCenter.BroadcastQueue.Enqueue((BroadcastActions.LeaveRoom, user.Room));
            return new StandRespone(suc);
        }

        public StandRespone JoinRoom(int roomid)
        {
            var user = User as User;

            if (user.UserState != UserStates.None)
                return StandRespone.FailResult("已经在房中 不能加入房间");

            var suc = DataCenter.Rooms.JoinRoom(roomid, user, out int team, out int index);
            DataCenter.BroadcastQueue.Enqueue((BroadcastActions.RoomChange,user.RoomDetail));
            DataCenter.BroadcastQueue.Enqueue((BroadcastActions.JoinRoom, user.Room));
            return new StandRespone<(int,int)>(suc,suc?"加入成功":"加入失败",(team,index));
        }

        public void Logout()
        {
            User.LoginOut();
        }
    }
}
