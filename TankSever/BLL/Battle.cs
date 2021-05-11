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
        private Dictionary<User, bool> clientstates=new Dictionary<User, bool>();       //用户状态 (是否上传第一次操作)
        private Dictionary<User, int> clientUpId = new Dictionary<User, int>(); //用户上传序号
        private Dictionary<int, PlayerOperations> hisFrames=new Dictionary<int, PlayerOperations>();  //历史帧
        private IDataFormatter formatter;
        private int currentFrameIndex = 0;
        private Dictionary<User,PlayerOperation> currentOperations=new Dictionary<User, PlayerOperation>();

        public Battle(Room room)
        {
            formatter = DI.Instance.Resolve<IDataFormatter>();
            _room = room;

            ClientUdp udp;
            foreach(var user in _room.GetUsers())
            {
                udp = ClientUdp.CreateClient(user, (short)GlobalConfig.UdpPort);
                clients.Add(user, udp);
                clientstates.Add(user, false);

                udp.OnReceive += data =>
                {
                    OnReceive(user, data);
                };
            }
        }


        public void StartBattle()
        {
            Task.Factory.StartNew(async () =>
            {
                while(true)
                {
                    //等待所有玩家发来第一次操作
                    while(true)
                    {
                        if (checkAllReady())
                            break;
                        await Task.Delay(30);
                    }


                    currentFrameIndex++;

                    DownOperations frame = new DownOperations();
                    frame.FrameIndex = currentFrameIndex;
                    frame.Operations = new PlayerOperations();
                    frame.Operations.Operations.AddRange(currentOperations.Values);
                    broadcastFrame(frame);


                    hisFrames[currentFrameIndex] = frame.Operations;
                    await Task.Delay(GlobalConfig.FrameDuration);
                }
            });
        }

        //广播帧数据
        private void broadcastFrame(DownOperations frame)
        {
            byte[] data;
            foreach(var client in clients)
            {
                data= formatter.Serialize(frame);
                data=ProtobufDataPackage.PackageData(UdpFlags.UdpDownFrame, data);
                client.Value.SendMessage(data);
            }
        }

        private bool checkAllReady()
        {
            return clientstates.All(m => m.Value);
        }

        //接收到消息
        private void OnReceive(User user,byte[] data)
        {
            var req= ProtobufDataPackage.UnPackageData(data, out UdpFlags action);
            HandleRequest(user,action, req);
        }

        //处理消息
        private void HandleRequest(User user,UdpFlags action,byte[] data)
        {
            switch(action)
            {
                case UdpFlags.UdpUpFrame:
                {
                    var frame = formatter.Deserialize<UpFrame>(data);
                    OnUpFrame(user, frame);
                    break;
                }
                case UdpFlags.UdpGameReady:
                    OnGameReady(user);
                    break;
                case UdpFlags.UdpReqFrame:
                {
                    var index = formatter.Deserialize<SingleInt>(data);
                    OnReqFrame(user, index);
                    break;
                }

            }
        }

        //请求补帧
        private void OnReqFrame(User user, SingleInt index)
        {
            if(index.Data<currentFrameIndex)
            {
                DownOperations opers= new DownOperations();
                opers.FrameIndex = index.Data;
                opers.Operations= hisFrames[index.Data];

                var data = formatter.Serialize(opers);
                data=ProtobufDataPackage.PackageData(UdpFlags.UdpDownFrame, data);
                clients[user].SendMessage(data);
            }
        }

        private void OnGameReady(User user)
        {
            clientstates[user] = true;
        }

        //上传帧操作
        private void OnUpFrame(User user,UpFrame upFrame)
        {
            if(upFrame.UpIndex>clientUpId[user])
            {
                clientUpId[user] = upFrame.UpIndex;
                currentOperations[user] = upFrame.Oper;
            }
        }

    }
}
