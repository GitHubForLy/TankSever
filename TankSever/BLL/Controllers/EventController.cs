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
using TankSever.BLL.Protocol;

namespace TankSever.BLL.Controllers
{
    partial class EventController:Controller 
    {
        private User _user => User as User;
        //注册
        [AllowAnonymous,Request(TcpFlags.TcpRegester)]
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
        [AllowAnonymous, Request(TcpFlags.TcpLogin)]
        public Respone<SingleString> Login(LoginRequest request)
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

                    return ToolHelp.CreateRespone(true, "登录成功", new SingleString { Data = _user.LoginTimestamp });
                }
                else
                {
                    return ToolHelp.CreateRespone(false, "登录失败,密码错误",new SingleString() {  Data=""});
                }
            }
            else
                return ToolHelp.CreateRespone(false, res.Message, new SingleString() { Data = "" });
        }

        //获取用户信息
        [Request(TcpFlags.TcpGetUserInfo)]
        public Respone<UserInfo> GetUserInfo()
        {
            var res = DBServer.Instance.GetUserInfo(_user.UserAccount);
            return res;
        }

        [Request(TcpFlags.TcpLogout)]
        public void Logout()
        {
            User.LoginOut();
        }

        [AllowAnonymous,  Request(TcpFlags.TcpCheckVersion)]
        public Respone<VersionInfo> CheckVersion(float version)
        {
            float highVer= (Program.UpdateServer as UpdateServer).Manager.GetHighVersion();

            if(version< highVer)
            {
                (Program.UpdateServer as UpdateServer).Manager.GetDiffFiles(version, out _, out _, out long size);
                return ToolHelp.CreateRespone(false, new VersionInfo { HighVersion = highVer, SumSize = size });
            }
            return ToolHelp.CreateRespone<VersionInfo>(true, "最新版本",null);
        }

        [Request(TcpFlags.TcpGetRoomList)]
        public RoomInfos RoomList()
        {
            var infos = new Google.Protobuf.Collections.RepeatedField<RoomInfo>();
            infos.AddRange(DataCenter.Rooms.GetRoomList().Select(m=>m.Info));
            var roominfos = new RoomInfos();
            roominfos.Infos.AddRange(infos);
            return roominfos;
        }

        [Request(TcpFlags.TcpCreateRoom)]
        public Respone<RoomInfo> CreateRoom(RoomSetting setting)
        {
            var user = User as User;

            if (user.RoomDetail.State != RoomUserStates.None)
                return ToolHelp.CreateRespone<RoomInfo>(false, "已经在房中 不能创建房间",null);

            var room= DataCenter.Rooms.CreateRoom(user, setting);
            (Program.BroadServer as BroadcastServer).BroadcastGlobal((TcpFlags.TcpBdCreateRoom, user.Room.Info));
            (Program.BroadServer as BroadcastServer).BroadcastRoom((room.Info.RoomId, TcpFlags.TcpRoomChange, user.RoomDetail));
            return ToolHelp.CreateRespone(true, "创建成功",room.Info);
        }

        [Request(TcpFlags.TcpLeaveRoom)]
        public Respone LeaveRoom()
        {
            var user = User as User;
            var suc = DataCenter.Rooms.LeaveRoom(user);
            (Program.BroadServer as BroadcastServer).BroadcastGlobal((TcpFlags.TcpBdLeaveRoom, user.Room.Info));
            (Program.BroadServer as BroadcastServer).BroadcastRoom((user.Room.Info.RoomId, TcpFlags.TcpRoomChange, user.RoomDetail));
            return ToolHelp.CreateRespone(suc);
        }

        [Request(TcpFlags.TcpJoinRoom)]
        public Respone<RoomInfo> JoinRoom((int roomid,string password)data)
        {
            var user = User as User;

            if (user.RoomDetail.State != RoomUserStates.None)
                return ToolHelp.CreateRespone<RoomInfo>(false, "已经在房中 不能加入房间",null);

            if (!(DataCenter.Rooms[data.roomid]?.CheckPassword(data.password) ?? false))
                return ToolHelp.CreateRespone<RoomInfo>(false, "房间密码错误",null);

            var suc = DataCenter.Rooms.JoinRoom(data.roomid, user,out Room room);
            (Program.BroadServer as BroadcastServer).BroadcastGlobal((TcpFlags.TcpBdJoinRoom, user.Room.Info));
            (Program.BroadServer as BroadcastServer).BroadcastRoom((user.Room.Info.RoomId, TcpFlags.TcpRoomChange, user.RoomDetail));

            return ToolHelp.CreateRespone(suc, suc ? "加入成功" : "加入失败", room.Info);
        }

        [Request(TcpFlags.TcpGetRoomUsers)]
        public RoomUsers GetRoomUsers(int roomid)
        {
            RoomUsers users = new RoomUsers();
            users.Users.AddRange(DataCenter.Rooms[roomid]?.GetUsers().Select(m => m.RoomDetail).ToArray() ?? new RoomUser[0]);
            return users;
        }

        [Request(TcpFlags.TcpRoomReady)]
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

        [Request(TcpFlags.TcpRoomCancelReady)]
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

        [Request(TcpFlags.TcpRoomChangeIndex)]
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

        [Request(TcpFlags.TcpRoomMessage)]
        public void BroadcastRoomMsg(RoomMessage message)
        {
            (Program.BroadServer as BroadcastServer).BroadcastRoom((_user.Room.Info.RoomId, TcpFlags.TcpRoomMessage, message));
            //DataCenter.BroadcastRoomQueue.Enqueue((_user.Room.RoomId, BroadcastActions.BroadcastRoomMsg, (_user.UserName,message)));
        }

        [AllowAnonymous,Request(TcpFlags.TcpCheckTime)]
        public SingleDouble CheckTime()
        {
            return new SingleDouble { Data = Sys.GetTime() };
        }
    }
}
