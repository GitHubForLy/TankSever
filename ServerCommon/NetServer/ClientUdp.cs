using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;

namespace ServerCommon.NetServer
{
    /// <summary>
    /// 客户udp
    /// </summary>
    public class ClientUdp
    {
        public IPEndPoint Point { get; }
        private bool isreceive = false;
        private UdpServer server;

        /// <summary>
        /// 自定义数据
        /// </summary>
        public object UserData { get; set; }

        internal ClientUdp(IPEndPoint pt,UdpServer server)
        {
            this.server = server;
            Point = pt;
        }

        public void SendMessage(byte[] data)
        {
            SendMessage(data, 0, data.Length);
        }
        public void SendMessage(byte[] data, int index, int length)
        {
            ////在没有接收到第一次数据之前 是不知道客户端的端口的 所以判断一下
            //if (!isreceive || Point == null)
            //    return;
            server.Send(data.Skip(index).ToArray(), Point);
        }

        public void Close()
        {
            server.CloseClient(this);
        }
    }
}
