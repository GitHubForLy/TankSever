using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCommon.Protocol;
using ServerCommon.NetServer;
using ServerCommon;
using System.Net.Sockets;
using Unity;

namespace NetCore.Server
{
    public delegate void OnRequestEventHandle(string UserCode);
    /// <summary>
    /// 表示一个基本tcp服务
    /// </summary>
    public class StandTcpServer : AsyncSocketServerBase
    {
        [Dependency]
        public IProtocolHandler Handler { get; set; }
        [Dependency]
        public IActionExecuter ActionExecuter { get; set; }


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
        protected override AsyncUserToken CreateUserToken()=> new PackageUserToken(this, ReceiveBufferSize);


        /// <summary>
        /// 处理请求
        /// </summary>
        protected override bool HandleReceive(SocketAsyncEventArgs receivearg)
        {
            PackageUserToken token = receivearg.UserToken as PackageUserToken;

            try
            {
                token.Pakcage.IncommingData(receivearg.Buffer, receivearg.Offset, receivearg.BytesTransferred);
                while(token.Pakcage.OutgoingPackage(out byte[] data))
                {
                    //处理数据
                    if(Handler.DataHandle(token,ref data,ActionExecuter))
                        SendData(token, data);    //发送数据

                    Notify(NotifyType.RequsetLog, Encoding.UTF8.GetString(data), this);
                }
            }
            catch (Exception err)
            {
                Notify(NotifyType.RequsetErrorLog, err.Message + ", " + (err.InnerException == null ? "" : err.InnerException.Message), this);
                CloseClientSocket(token);
                throw new Exception(err.Message, err);
            }


            return true;  //断开连接
        }

        /// <summary>
        /// 关闭并回收连接对象
        /// </summary>
        public override void CloseClientSocket(AsyncUserToken userToken)
        {
            (userToken as PackageUserToken).Pakcage.Clear();

            base.CloseClientSocket(userToken);
        }
    }
}
