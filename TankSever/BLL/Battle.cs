using ServerCommon.NetServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCommon;

namespace TankSever.BLL
{
    /// <summary>
    /// 战场管理
    /// </summary>
    public class Battle
    {
        private Room _room;
        private Dictionary<User, ClientUdp> clients;
        private IDataFormatter formatter;

        public Battle(Room room)
        {
            formatter = DI.Instance.Resolve<IDataFormatter>();
            _room = room;
            clients = new Dictionary<User, ClientUdp>();
            ClientUdp udp;
            foreach(var user in _room.GetUsers())
            {
                udp = ClientUdp.CreateClient(user, (short)GlobalConfig.UdpPort);
                clients.Add(user, udp);
                udp.OnReceive += data =>
                {
                    OnReceive(user, data);
                };
            }
        }


        public void StartBattle()
        {
            
        }


        public void OnReceive(User user,byte[] data)
        {
            var req= formatter.DeserializeDynamic(data);
            req.GetChid(nameof(DataModel.Request.Controller))
        
        }
    }
}
