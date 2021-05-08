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
        private Dictionary<User, ClientUdp> clients;
        private Dictionary<User, bool> clientstates;
        private IDataFormatter formatter;
        private int currentFrameIndex=0;

        public Battle(Room room)
        {
            formatter = DI.Instance.Resolve<IDataFormatter>();
            _room = room;
            clients = new Dictionary<User, ClientUdp>();
            clientstates = new Dictionary<User, bool>();

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
                    while(true)
                    {
                        if (checkAllReady())
                            break;
                        await Task.Delay(30);
                    }


                    currentFrameIndex++;






                    await Task.Delay(GlobalConfig.FrameDuration);
                }
            });
        }

        private bool checkAllReady()
        {
            return clientstates.All(m => m.Value);
        }

        private void OnReceive(User user,byte[] data)
        {
            var req= ProtobufDataPackage.UnPackageData(data, out UdpFlags action);
            HandleRequest(user,action, req);
        }

        private void HandleRequest(User user,UdpFlags action,byte[] data)
        {
            switch(action)
            {
                case UdpFlags.UdpUpFrame:
                    OnUpFrame();
                    break;
                case UdpFlags.UdpGameReady:
                    OnGameReady(user);
                    break;


            }
        }

        private void OnGameReady(User user)
        {
            clientstates[user] = true;
        }

        private void OnUpFrame()
        {

        }

    }
}
