using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ServerCommon.NetServer
{
    public class ClientUdp
    {
        private UdpClient client;
        private IPEndPoint point;
        private bool isreceive = false;

        public event Action<byte[]> OnReceive;

        private ClientUdp(IPAddress clientIp,short recvPort)
        {
            client = new UdpClient(recvPort);
            point = new IPEndPoint(clientIp, recvPort);
            client.BeginReceive(OnReceiveMethod, null);
        }

        private void OnReceiveMethod(IAsyncResult ar)
        {
            byte[] data = client.EndReceive(ar, ref point);
            isreceive = true;
            OnReceive?.Invoke(data);
            client.BeginReceive(OnReceiveMethod, null);
        }

        public static ClientUdp CreateClient(AsyncUser user,short recvPort)
        {
            return new ClientUdp((user.UserToken.ConnectSocket.RemoteEndPoint as IPEndPoint).Address, recvPort);
        }

        public void SendMessage(byte[] data)
        {
            if (!isreceive)
                return;
            client.Send(data, data.Length, point);
        }
        public void SendMessage(byte[] data, int index, int length)
        {
            client.Send(data.Skip(index).ToArray(), length);
        }
    }
}
