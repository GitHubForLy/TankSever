using ServerCommon;
using ServerCommon.NetServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankSever.BLL.Server
{
    public delegate void OnRequestEventHandle(string UserCode);
    /// <summary>
    /// 表示一个基本tcp服务
    /// </summary>
    public class StandTcpServer : AsyncSocketServerBase
    {
        public readonly byte[] heartbyte = Encoding.UTF8.GetBytes("heart");
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
            var token = new PackageUserToken(this, ReceiveBufferSize);
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

        /// <summary>
        /// 数据处理
        /// </summary>
        /// <returns>返回true代表内部已进行接收 否则没有进行接收需要调用方再进行接收</returns>
        private bool DataHandle(PackageUserToken token)
        {
            if (token.Pakcage.OutgoingPackage(out byte[] data))
            {
                if (CheckHeart(data))  //心跳包
                    return false;

                //var st=Stopwatch.StartNew();
                try
                {
                    token.ProtocolHandler.DoRequest(data);
                }
                catch (Exception err)
                {
                    System.Diagnostics.Debugger.Break();
                    Notify(NotifyType.Error, "处理数据出错:" + err.Message + ", " + (err.InnerException == null ? "" : err.InnerException.Message), this);
                    return false;
                }
                //var re = st.ElapsedMilliseconds;

                if (token.ProtocolHandler.TryGetRespone(out byte[] res))
                {
                    var senddata = DataPackage.PackData(res);
                    token.SendEventArgs.SetBuffer(senddata, 0, senddata.Length);
                    //System.Diagnostics.Debug.WriteLine("re:"+re + "  request data time:" + st.ElapsedMilliseconds);
                    //token.st = st;
                    SendAsync(token);    //发送数据
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 检查心跳包
        /// </summary>
        /// <returns>true是</returns>
        protected virtual bool CheckHeart(byte[] data)
        {
            if (data == null || data.Length != heartbyte.Length)
                return false;
            for (int i = 0; i < heartbyte.Length; i++)
            {
                if (data[i] != heartbyte[i])
                    return false;
            }
            return true;
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
        public override void Broadcast(byte[] data, bool isNeedLogin = true)
        {
            Broadcast(data, null, isNeedLogin);
        }

        public override void Broadcast(byte[] data, AsyncUser[] users, bool IsNeedLogin)
        {
            var packdata = DataPackage.PackData(data);
            base.Broadcast(packdata, users, IsNeedLogin);
        }
    }
}
