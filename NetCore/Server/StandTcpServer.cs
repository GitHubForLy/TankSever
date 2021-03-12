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
        public IProtocolHandler protocolHandler { get; set; }

        public override string ServerName => "StandTcpServer";

        /// <summary>
        /// 处理请求时
        /// </summary>
        public event OnRequestEventHandle OnRequest;


        public StandTcpServer(int MaxConnections, /*NetDBServer dBServer, */INotifier notifier) : base(MaxConnections, notifier)
        {
            //protocolHandler = new TcpProtocolHandler(dBServer, this);
        }

        /// <summary>
        /// 构造UserToken
        /// </summary>
        /// <returns></returns>
        protected override AsyncUserToken CreateUserToken()
        {
            return new PackageUserToken(ReceiveBufferSize);
        }


        /// <summary>
        /// 处理请求
        /// </summary>
        protected override bool HandleReceive(SocketAsyncEventArgs receivearg)
        {
            //byte[] resbuf;
            PackageUserToken token = receivearg.UserToken as PackageUserToken;

            try
            {
                token.Pakcage.IncommingData(receivearg.Buffer, receivearg.Offset, receivearg.BytesTransferred);
                while(token.Pakcage.OutPutPackage(out byte[] data))
                {
                    //处理数据
                    protocolHandler.Deserialize(data);



                    Notify(NotifyType.RequsetLog, Encoding.UTF8.GetString(data), this);
                }

                //TxRequest request = ResolveRequest(userToken.ReceiveEventArgs.Buffer, userToken.ReceiveEventArgs.Offset, userToken.ReceiveEventArgs.BytesTransferred);
                //OnRequest?.Invoke(request.UserCode.ToString(), request.Opcode);

                //FuyooRespone respone = protocolHandler.HandleRequest(request);
                //resbuf = SerializeRespone(respone);
            }
            catch (Exception err)
            {
                Notify(NotifyType.RequsetErrorLog, err.Message + ", " + (err.InnerException == null ? "" : err.InnerException.Message), this);
                CloseClientSocket(token);
                throw new Exception(err.Message, err);
            }

            //SendData(userToken, resbuf);    //发送数据

            return true;  //断开连接
        }

        /// <summary>
        /// 解析参数
        /// </summary>
        //protected virtual TxRequest ResolveRequest(byte[] buffer, int offset, int count)
        //{
        //    TxRequest rt = new TxRequest(TxRequsetType.Tcp);
        //    string Request = Encoding.Default.GetString(buffer, offset, count);
        //    try
        //    {
        //        var Dics = JsonConvert.DeserializeObject<Dictionary<string, string>>(Request);
        //        rt.Opcode = (OpCode)int.Parse(Dics[BLLParamConst.PM_OpCode]);
        //        rt.UserCode = int.Parse(Dics[BLLParamConst.PM_ClientNO]);
        //        rt.Password = Dics[BLLParamConst.PM_Password];
        //        rt.Params = Dics;
        //    }
        //    catch
        //    {
        //        //无效的参数类型
        //    }
        //    return rt;
        //}


        /// <summary>
        /// 反序列化响应结果
        /// </summary>
        //protected virtual byte[] SerializeRespone(FuyooRespone respone)
        //{
        //    string json = JsonConvert.SerializeObject(respone);
        //    return Encoding.Default.GetBytes(json);
        //}


        protected override void CloseClientSocket(AsyncUserToken userToken)
        {
            (userToken as PackageUserToken).Pakcage.Clear();

            base.CloseClientSocket(userToken);
        }
    }
}
