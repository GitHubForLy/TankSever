using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCommon.Protocol;
using ServerCommon.NetServer;
using ServerCommon;
using System.Net.Sockets;

namespace NetCore.Server
{
    public delegate void OnRequestEventHandle(string UserCode);
    /// <summary>
    /// 表示一个基本tcp服务
    /// </summary>
    public class StandTcpServer : AsyncSocketServerBase
    {
        public override string ServerName => "StandTcpServer";

        /// <summary>
        /// 处理请求时
        /// </summary>
        public event OnRequestEventHandle OnRequest;


        public StandTcpServer(int MaxConnections, INotifier notifier) : base(MaxConnections, notifier)
        {
        }

        /// <summary>
        /// 构造UserToken
        /// </summary>
        protected override AsyncUserToken CreateUserToken()
        {
            var token =new PackageUserToken(this, ReceiveBufferSize);
            token.ProtocolHandler = ProtoFactory.CreateProtocolHandler(token);
            return token;
        }


        /// <summary>
        /// 处理请求
        /// </summary>
        protected override void OnReceiveSuccess(AsyncUserToken usertoken)
        {
            base.OnReceiveSuccess(usertoken);
            PackageUserToken token = usertoken as PackageUserToken;
            try
            {
                token.Pakcage.IncommingData(token.ReceiveEventArgs.Buffer, token.ReceiveEventArgs.Offset, token.ReceiveEventArgs.BytesTransferred);
                if (!DataHandle(token))
                    ReceiveAsync(token);
            }
            catch (Exception err)
            {
                Notify(NotifyType.Error, err.Message + ", " + (err.InnerException == null ? "" : err.InnerException.Message), this);
            }
        }


        private bool DataHandle(PackageUserToken token)
        {
            if(token.Pakcage.OutgoingPackage(out byte[] data))
            {
                token.ProtocolHandler.DoRequest(data);
                if(token.ProtocolHandler.TryGetRespone(out byte[] res))
                {
                    var senddata = DataPackage.PackData(res);
                    token.SendEventArgs.SetBuffer(senddata, 0, senddata.Length);
                    SendAsync(token);    //发送数据
                    return true;
                }
            }
            return false;
        }

        
        /// <summary>
        /// 关闭并回收连接对象
        /// </summary>
        public override void CloseClientSocket(AsyncUserToken userToken)
        {
            (userToken as PackageUserToken).Pakcage.Clear();

            base.CloseClientSocket(userToken);
        }

        public override void OnSendSuccess(AsyncUserToken token)
        {
            base.OnSendSuccess(token);

            if (!DataHandle(token as PackageUserToken))
                ReceiveAsync(token);
        }


        /// <summary>
        /// 广播数据
        /// </summary>
        /// <param name="data"></param>
        public override void Broadcast(byte[] data,bool isNeedLogin=true)
        {
            var packdata = DataPackage.PackData(data);
            base.Broadcast(packdata);
        }
    }
}
