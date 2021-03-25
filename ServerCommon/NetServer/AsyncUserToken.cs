using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;

namespace ServerCommon.NetServer
{
    public class AsyncUserToken : IDisposable
    {
        private bool m_disposed;
        private Socket m_connectSocket;
        private byte[] m_asyncReceiveBuffer;

        public AutoResetEvent SendEvent {get;}
        public AutoResetEvent RecvEvent { get; }
        /// <summary>
        /// 所属用户名
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 是否处于激活状态(可用状态)
        /// </summary>
        public bool IsActive { get; set; }
        /// <summary>
        /// 是否登录
        /// </summary>
        public bool IsLogined => UserCenter.Instance.CheckUser(this);

        public Socket ConnectSocket
        {
            set
            {
                m_connectSocket = value;
                //清理缓存
                SendEventArgs.AcceptSocket = m_connectSocket;
                ReceiveEventArgs.AcceptSocket = m_connectSocket;
            }
            get { return m_connectSocket; }
        }

        public DateTime ConnectDateTime { set; get; }
        public DateTime ActiveDateTime { set; get; }

        /// <summary>
        /// 表示接收的SocketAsyncEventArgs
        /// </summary>
        public SocketAsyncEventArgs ReceiveEventArgs { get;}

        /// <summary>
        /// 表示发送的SocketAsyncEventArgs
        /// </summary>
        public SocketAsyncEventArgs SendEventArgs { get; }

        /// <summary>
        /// 所属net服务
        /// </summary>
        public AsyncSocketServerBase Server { get;}



        public AsyncUserToken(AsyncSocketServerBase server,int asyncReceiveBufferSize)
        {
            Server = server;
            m_connectSocket = null;

            m_asyncReceiveBuffer = new byte[asyncReceiveBufferSize];
            ReceiveEventArgs = new SocketAsyncEventArgs();
            ReceiveEventArgs.SetBuffer(m_asyncReceiveBuffer, 0, asyncReceiveBufferSize);
            ReceiveEventArgs.UserToken = this;

            SendEventArgs = new SocketAsyncEventArgs();
            SendEventArgs.UserToken = this;

            m_disposed = false;
            SendEvent = new AutoResetEvent(true);
            RecvEvent = new AutoResetEvent(true);
        }

        ~AsyncUserToken()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {           
            if (m_disposed) return;
            if (disposing)
            {
                //释放托管资源
                m_connectSocket = null;
                m_asyncReceiveBuffer = null;
                ReceiveEventArgs.UserToken = null;
                SendEventArgs.UserToken = null;
            }
            //释放非托管资源            
            ReceiveEventArgs.Dispose();
            SendEventArgs.Dispose();
            SendEvent.Dispose();
            m_disposed = true;
        }


    }
}
