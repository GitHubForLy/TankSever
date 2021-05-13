using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace ServerCommon.NetServer
{
    public class UdpServer : ServerBase
    {
        private UdpClient client;
        Dictionary<IPEndPoint, ClientUdp> dict = new Dictionary<IPEndPoint, ClientUdp>();

        public override string ServerName => "UdpServer";
        public static event Action<ClientUdp,byte[]> OnReceive;

        public UdpServer(INotifier notifier):base(notifier)
        {
        }

        public void Start(IPEndPoint pt)
        {
            client = new UdpClient(pt);
            base.Start();
        }


        public override void Run()
        {
            IPEndPoint pt = new IPEndPoint(IPAddress.Any, 0);
            while (!IsStop)
            {
                byte[] data = client.Receive(ref pt);

                var ct = GetClient(pt);
                Task.Run(() =>
                {
                    OnReceive?.Invoke(ct,  data);
                });
            }
        }

        internal int Send(byte[] data,IPEndPoint pt)
        {
            return client.Send(data, data.Length, pt);
        }


        private ClientUdp GetClient(IPEndPoint pt)
        {
            if (dict.ContainsKey(pt))
                return dict[pt];

            var ct = new ClientUdp(pt, this);
            dict.Add(pt, ct);
            return ct;
        }

        /// <summary>
        /// 关闭该用户
        /// </summary>
        /// <param name="client"></param>
        public void CloseClient(ClientUdp client)
        {
            if (dict.ContainsKey(client.Point))
                dict.Remove(client.Point);
        }
    }
}
