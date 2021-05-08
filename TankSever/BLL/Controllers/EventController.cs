using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DBCore;
using ServerCommon.Protocol;
//using DataModel;
using TankSever.BLL.Server;
using AutoUpdate;
using ProtobufProto;
using ProtobufProto.Model;

namespace TankSever.BLL.Controllers
{
    partial class EventController:Controller 
    {
        private User _user => User as User;
        //注册
        [AllowAnonymous]
        public Respone Register(RegistrRequest request)
        {
            //加密密码
            byte[] salt = new byte[20];
            new Random().NextBytes(salt);
            MD5Cng md5 = new MD5Cng();
            var saltpass = Encoding.UTF8.GetBytes(request.Password).Concat(salt).ToArray();
            var crpPass = md5.ComputeHash(saltpass);

            return DBServer.Instance.Regeister(request.UserName,request.Account, Convert.ToBase64String(crpPass), Convert.ToBase64String(salt));
        }

        //登录
        [AllowAnonymous]
        public Respone Login(LoginRequest request)
        {
            var res = DBServer.Instance.GetPassword(request.Account,out (string salt,string pwd)data);
            if (res.IsSuccess)
            {
                var oldPass = data.pwd;
                var salt = Convert.FromBase64String(data.salt);
                var saltpass = Encoding.UTF8.GetBytes(request.Password).Concat(salt).ToArray();
                MD5Cng md5 = new MD5Cng();
                var crpPass = Convert.ToBase64String(md5.ComputeHash(saltpass));

                if (oldPass == crpPass)
                {
                    User.Login(request.Account);

                    return ToolHelp.CreateRespone(true, "登录成功", new SingleString() { Data = _user.LoginTimestamp });
                }
                else
                {
                    return ToolHelp.CreateRespone(false, "登录失败,密码错误");
                }
            }
            else
                return res;
        }

        //获取用户信息
        public Respone GetUserInfo()
        {
            var res = DBServer.Instance.GetUserInfo(_user.UserAccount);
            return res;
        }

        public void Logout()
        {
            User.LoginOut();
        }

        [AllowAnonymous]
        public Respone CheckVersion(float version)
        {
            float highVer= (Program.UpdateServer as UpdateServer).Manager.GetHighVersion();

            if(version< highVer)
            {
                (Program.UpdateServer as UpdateServer).Manager.GetDiffFiles(version, out _, out _, out long size);
                return ToolHelp.CreateRespone(false, new VersionInfo { HighVersion = highVer, SumSize = size });
            }
            return ToolHelp.CreateRespone(true, "最新版本");
        }




        public RoomInfos RoomList()
        {
            var infos = new Google.Protobuf.Collections.RepeatedField<RoomInfo>();
            infos.AddRange(DataCenter.Rooms.GetRoomList().Select(m=>m.Info));
            var roominfos = new RoomInfos();
            roominfos.Infos.AddRange(infos);
            return roominfos;
        }


        public Respone CreateRoom(RoomSetting setting)
        {
            var user = User as User;

            if (user.RoomDetail.State != RoomUserStates.None)
                return ToolHelp.CreateRespone(false, "已经在房中 不能创建房间");

            var room= DataCenter.Rooms.CreateRoom(user, setting);
            (Program.BroadServer as BroadcastServer).BroadcastGlobal((TcpFlags.TcpCreateRoom, user.Room));
            (Program.BroadServer as BroadcastServer).BroadcastRoom((room.Info.RoomId, TcpFlags.TcpRoomChange, user.RoomDetail));
            return ToolHelp.CreateRespone(true, "创建成功",room.Info);
        }

        public Respone LeaveRoom()
        {
            var user = User as User;
            var suc = DataCenter.Rooms.LeaveRoom(user);
            (Program.BroadServer as BroadcastServer).BroadcastGlobal((TcpFlags.TcpLeaveRoom, user.Room));
            (Program.BroadServer as BroadcastServer).BroadcastRoom((user.Room.Info.RoomId, TcpFlags.TcpRoomChange, user.RoomDetail));
            return ToolHelp.CreateRespone(suc);
        }

        public Respone JoinRoom((int roomid,string password)data)
        {
            var user = User as User;

            if (user.RoomDetail.State != RoomUserStates.None)
                return ToolHelp.CreateRespone(false, "已经在房中 不能加入房间");

            if (!(DataCenter.Rooms[data.roomid]?.CheckPassword(data.password) ?? false))
                return ToolHelp.CreateRespone(false, "房间密码错误");

            var suc = DataCenter.Rooms.JoinRoom(data.roomid, user,out Room room);
            (Program.BroadServer as BroadcastServer).BroadcastGlobal((TcpFlags.TcpJoinRoom, user.Room));
            (Program.BroadServer as BroadcastServer).BroadcastRoom((user.Room.Info.RoomId, TcpFlags.TcpRoomChange, user.RoomDetail));

            return ToolHelp.CreateRespone(suc, suc ? "加入成功" : "加入失败", room.Info);
        }

        public RoomUser[] GetRoomUsers(int roomid)
        {
            return DataCenter.Rooms[roomid]?.GetUsers().Select(m=>m.RoomDetail).ToArray()??new RoomUser[0];
        }

        public Respone RoomReady()
        {
            var user = User as User;
            if ((user.Room as Room).RoomReady(user.RoomDetail))
            {
                (Program.BroadServer as BroadcastServer).BroadcastRoom((user.Room.Info.RoomId, TcpFlags.TcpRoomChange, user.RoomDetail));;
                return ToolHelp.CreateRespone(true, "准备成功");
            }
            else
                return ToolHelp.CreateRespone(false, "准备失败");
        }

        public Respone RoomCancelReady()
        {
            var user = User as User;
            if ((user.Room as Room).RoomCancelReady(user))
            {
                (Program.BroadServer as BroadcastServer).BroadcastRoom((user.Room.Info.RoomId, TcpFlags.TcpRoomChange, user.RoomDetail));
                return ToolHelp.CreateRespone(true, "取消准备成功");
            }
            else
                return ToolHelp.CreateRespone(false, "取消准备失败");
        }

        public Respone RoomChangeIndex(int index)
        {
            var user = User as User;
            if ((user.Room as Room).ChangeIndex(user,index))
            {
                (Program.BroadServer as BroadcastServer).BroadcastRoom((user.Room.Info.RoomId, TcpFlags.TcpRoomChange, user.RoomDetail));
                return ToolHelp.CreateRespone(true, "操作成功");
            }
            else
                return ToolHelp.CreateRespone(false, "操作失败");
        }



        [AllowAnonymous]
        public double CheckTime()
        {
            return Sys.GetTime();
        }
    }
}
