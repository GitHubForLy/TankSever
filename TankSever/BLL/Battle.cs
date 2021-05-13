using ServerCommon.NetServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCommon;
using ProtobufProto.Model;

namespace TankSever.BLL
{
    /// <summary>
    /// 战场管理
    /// </summary>
    public class Battle
    {
        private Room _room;
        private Dictionary<User, ClientUdp> clients=new Dictionary<User, ClientUdp>();  //用户和udp
        private Dictionary<ClientUdp, int> clientUpId = new Dictionary<ClientUdp, int>(); //用户上传序号
        private Dictionary<int, PlayerOperations> hisFrames=new Dictionary<int, PlayerOperations>();  //历史帧
        private IDataFormatter formatter;
        private int currentFrameIndex = 0;
        private Dictionary<ClientUdp    ,PlayerOperation> currentOperations=new Dictionary<ClientUdp, PlayerOperation>();
        private bool noOver = true;
        private User[] users;

        public Battle(Room room)
        {
            formatter = DI.Instance.Resolve<IDataFormatter>();
            _room = room;
            users = room.GetUsers();

            UdpServer.OnReceive += OnUdpServerReceive;

            ClientUdp udp;
            //foreach(var user in _room.GetUsers())
            //{
            //    udp = new ClientUdp(user);
            //    clients.Add(user, udp);
            //    clientUpId[user] = -1;

            //    udp.OnReceive += data =>
            //    {
            //        OnReceive(user, data);
            //    };
            //}
        }


        public void StartBattle()
        {
            Task.Factory.StartNew(async () =>
            {
                //等待所有玩家准备完成
                while (noOver)
                {
                    if (checkAllReady())
                        break;
                    await Task.Delay(200);
                }
                //等待所有玩家发来第一次操作
                while (noOver)
                {
                    SendBattleStart();
                    if (checkAllFirstOper())
                        break;
                    await Task.Delay(200);
                }

                //循环发送帧操作
                while (noOver)
                {                             
                    currentFrameIndex++;

                    DownOperations frame = new DownOperations();
                    frame.FrameIndex = currentFrameIndex;
                    frame.Operations = new PlayerOperations();
                    frame.Operations.Operations.AddRange(currentOperations.Values);
                    BroadcastMessage(UdpFlags.UdpDownFrame,frame);


                    hisFrames[currentFrameIndex] = frame.Operations;
                    await Task.Delay(GlobalConfig.FrameDuration);
                }
            });
        }

        //发送战斗开始
        private void SendBattleStart()
        {
            BroadcastMessage(UdpFlags.UdpBattleStart, null);
        }

        public void StopFight()
        {
            System.Diagnostics.Debug.WriteLine("清理战场");
            noOver = false;
            lock(clients)
            {
                foreach (var ct in clients)
                {
                    ct.Value.Close();
                }
            }
        }

        //广播消息
        private void BroadcastMessage(UdpFlags flag,object data)
        {
            byte[] buffer;
            buffer = formatter.Serialize(data);
            buffer = ProtobufDataPackage.PackageData(flag, buffer);

            lock(clients)
            {
                foreach (var client in clients)
                {
                    client.Value.SendMessage(buffer);
                }
            }
        }


        //检查是否都发来了操作信息
        private bool checkAllReady()
        {
            lock(clients)
            {
                return clients.Count >= users.Length;
            }
        }


        //检查是否都发来了操作信息
        private bool checkAllFirstOper()
        {
            lock(clients)
            {
                return currentOperations.Count >= clients.Count;
            }
        }

        //接收到消息
        private void OnUdpServerReceive(ClientUdp user,byte[] data)
        {
            var req= ProtobufDataPackage.UnPackageData(data, out UdpFlags action);
            HandleRequest(user,action, req);
        }

        //处理消息
        private void HandleRequest(ClientUdp user,UdpFlags action,byte[] data)
        {
            switch(action)
            {
                case UdpFlags.UdpGameReady:
                    OnGameReady(user, formatter.Deserialize<SingleString>(data));
                    break;
                case UdpFlags.UdpUpFrame:
                {
                    var frame = formatter.Deserialize<UpFrame>(data);
                    OnUpFrame(user, frame);
                    break;
                }
                case UdpFlags.UdpReqFrame:
                {
                    var req = formatter.Deserialize<ReqLackFrame>(data);
                    OnReqFrame(user, req);
                    break;
                }

            }
        }

        //用户准备
        private void OnGameReady(ClientUdp user, SingleString data)
        {
            string account = data.Data;
            lock(clients)
            {
                if (!clients.ContainsValue(user))
                {
                    var au = users.First(m => m.UserAccount == account);
                    clients.Add(au, user);
                    clientUpId[user] = -1;

                    user.UserData = au;
                }
            }
        }


        //请求补帧
        private void OnReqFrame(ClientUdp user, ReqLackFrame req)
        {
            DownLackFrame frames = new DownLackFrame();

            foreach(var i in req.Indexes)
            {
                DownOperations opers = new DownOperations();
                opers.FrameIndex = i;
                opers.Operations=hisFrames[i];
                frames.Frames.Add(opers);
            }

            var data = formatter.Serialize(frames);
            data = ProtobufDataPackage.PackageData(UdpFlags.UdpDownLackFrame, data);
            user.SendMessage(data);
        }


        //上传帧操作
        private void OnUpFrame(ClientUdp user,UpFrame upFrame)
        {
            if(upFrame.UpIndex>clientUpId[user])
            {
                clientUpId[user] = upFrame.UpIndex;
                currentOperations[user] = upFrame.Oper;
            }
        }

    }
}
