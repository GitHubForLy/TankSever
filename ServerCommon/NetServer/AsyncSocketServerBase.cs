using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ServerCommon;
using ServerCommon.Protocol;

namespace ServerCommon.NetServer
{
    /// <summary>
    /// 表示一个异步套接字服务
    /// </summary>
    public abstract class AsyncSocketServerBase : NotifyServer, IDisposable
    {
        private Socket m_listenSocket;
        private AsyncUserTokenPool m_asyncUserTokenPool;
        private Semaphore m_maxNumberAcceptClients;  //限制访问接收的连接的线程数，控制最大并发数
        private DaemonThread m_daemonThread;        //守护线程 负责心跳检查
        private int m_totalSerial;
        private int m_totalReceiveBytes;
        private int m_totalSendBytes;
        private bool m_disposed;
        private bool m_isInit = false;

        /// <summary>
        /// 用户连接列表
        /// </summary>
        protected AsyncUserTokenList ConnectionList;

        protected int NumMaxConnctions;

        protected IProtocolHandlerFactory ProtoFactory { get; }

        /// <summary>
        /// 连接超时时间，单位MS 默认10秒
        /// </summary>
        public int SocketTimeOutMS { set; get; } = 1000 * 10;
        /// <summary>
        /// 接收缓冲区大小 默认1kb
        /// </summary>
        public int ReceiveBufferSize { get; set; } = 1024;

        public int TotalSerial => m_totalSerial; 
        public int TotalReceiveBytes =>m_totalReceiveBytes;
        public int TotalSendBytes => m_totalSendBytes; 
        public int ConnectedCount => ConnectionList.Count;

        //指示服务是否关闭
        public bool IsClosed { set; get; }


        public AsyncSocketServerBase() : this(100) { }

        public AsyncSocketServerBase(int MaxConnections):this(MaxConnections,null){}

        public AsyncSocketServerBase(int MaxConnections, INotifier notifier) : base(notifier)
        {
            NumMaxConnctions = MaxConnections;
            m_maxNumberAcceptClients = new Semaphore(NumMaxConnctions, NumMaxConnctions);

            ConnectionList = new AsyncUserTokenList();
            m_asyncUserTokenPool = new AsyncUserTokenPool(NumMaxConnctions);

            m_totalSerial = 0;
            m_totalReceiveBytes = 0;
            m_totalSendBytes = 0;
            m_disposed = false;
            IsClosed = true;
            ProtoFactory = DI.Instance.Resolve<IProtocolHandlerFactory>();
        }

        ~AsyncSocketServerBase()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (m_disposed) return;
            if (disposing)
            {
                //释放托管资源
                ConnectionList = null;
                m_asyncUserTokenPool = null;
            }
            //释放非托管资源
            m_maxNumberAcceptClients.Dispose();
            if (m_listenSocket != null)
            {
                m_listenSocket.Dispose();
            }
            m_disposed = true;
        }

        /// <summary>
        /// 构造UserToken
        /// </summary>
        /// <returns></returns>
        protected virtual AsyncUserToken CreateUserToken()
        {
            var token=new AsyncUserToken(this,ReceiveBufferSize);
            token.ProtocolHandler = ProtoFactory.CreateProtocolHandler(token);
            return token;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            //预分配上下文对象
            AsyncUserToken userToken;
            for (int i = 0; i < NumMaxConnctions; i++)
            {
                userToken = CreateUserToken();
                userToken.ReceiveEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                userToken.SendEventArgs.Completed += new EventHandler<SocketAsyncEventArgs>(IO_Completed);
                m_asyncUserTokenPool.Push(userToken);
            }
            m_isInit = true;
        }
        public void Start(IPEndPoint localEP)
        {
            if(!m_isInit)
            {
                Notify(NotifyType.Error, "服务未初始化,无法启动",this);
                return;
            }

            IsClosed = false;
            m_listenSocket = new Socket(localEP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            m_listenSocket.Bind(localEP);
            m_listenSocket.Listen(NumMaxConnctions);
            Notifier?.OnNotify(NotifyType.Message, string.Format("开始监听端口：{0}", localEP.ToString()), this);

            StartAccept(null);
            m_daemonThread = new DaemonThread(this, Notifier);
        }

        public void Close()
        {
            AsyncUserToken[] array = null;
            ConnectionList.CopyList(ref array);

            for (int i = array.Length - 1; i >= 0; i--)
            {
                CloseClientSocket(array[i]);
            }
            try
            {
                if (m_listenSocket.Connected)
                    m_listenSocket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception e)
            {
                Notifier?.OnNotify(NotifyType.Error, string.Format("关闭网络服务发生错误：{0}", e.Message), this);
            }
            m_listenSocket.Close();
            m_listenSocket = null;

            if (m_daemonThread != null)
                m_daemonThread.Close();
            IsClosed = true;
                 
        }

        private void StartAccept(SocketAsyncEventArgs e)
        {
            if (null == e)
            {
                e = new SocketAsyncEventArgs();
                e.Completed += new EventHandler<SocketAsyncEventArgs>(OnAcceptCompleted);
            }
            else
            {
                e.AcceptSocket = null;
            }
            m_maxNumberAcceptClients.WaitOne();

            try
            {
                bool willRaiseEvent = m_listenSocket.AcceptAsync(e);
                if (!willRaiseEvent)
                {
                    ProcessAccept(e);
                }
            }
            catch (Exception ex)
            {
                Notifier?.OnNotify(NotifyType.Error, string.Format("等待用户连接时发生错误 : {0}", ex.Message), this);
            }
        }

        private void OnAcceptCompleted(object sender, SocketAsyncEventArgs e)
        {
            try
            {
                if (e.SocketError != SocketError.OperationAborted)
                {
                    ProcessAccept(e);
                }
            }
            catch (Exception ex)
            {
                Notifier?.OnNotify(NotifyType.Error, string.Format("接受用户{0}连接时发生错误: {1}", e.AcceptSocket, ex.Message), this);
            }
        }

        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                Notifier?.OnNotify(NotifyType.Message, string.Format("成功与用户建立连接，本地地址: {0}, 远程地址: {1}",
                     e.AcceptSocket.LocalEndPoint, e.AcceptSocket.RemoteEndPoint), this);

                AsyncUserToken userToken = m_asyncUserTokenPool.Pop();
                userToken.ConnectSocket = e.AcceptSocket;
                userToken.ConnectDateTime = DateTime.Now;
                userToken.ActiveDateTime = DateTime.Now;
                userToken.IsActive = true;
                ConnectionList.Add(userToken);

                try
                {
                    ReceiveAsync(userToken);
                }
                catch (Exception ex)
                {
                    Notifier?.OnNotify(NotifyType.Error, string.Format("接受用户{0}的数据时发生错误: {1}", userToken.ConnectSocket, ex.Message), this);
                }
            }
            //递归接收下一次请求
            StartAccept(e);
        }

        protected void IO_Completed(object sender, SocketAsyncEventArgs e)
        {
            AsyncUserToken userToken = e.UserToken as AsyncUserToken;
            userToken.ActiveDateTime = DateTime.Now;

            try
            {
                if (e.LastOperation == SocketAsyncOperation.Receive)
                    ProcessReceive(e);
                else if (e.LastOperation == SocketAsyncOperation.Send)
                    ProcessSend(e);
                else
                    throw new ArgumentException("The last operation completed on the socket was not a receive or send");
            }
            catch (Exception ex)
            {
                Notifier?.OnNotify(NotifyType.Error, string.Format("用户{0}读取或发送时发生错误：{1}", userToken.ConnectSocket, ex.Message), this);
            }
        }

        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            AsyncUserToken userToken = e.UserToken as AsyncUserToken;
            lock (userToken)
            {
                if (!userToken.IsActive)
                    return;
                userToken.RecvEvent.Set();

                int count = userToken.ReceiveEventArgs.BytesTransferred;
                if (count > 0 && userToken.ReceiveEventArgs.SocketError == SocketError.Success)
                {
                    OnReceiveSuccess(userToken);
                }
                else
                {
                    CloseClientSocket(userToken);
                }
            }           
        }

        /// <summary>
        /// 异步接收数据
        /// </summary>
        protected void ReceiveAsync(AsyncUserToken token)
        {
            lock(token)
            {
                token.RecvEvent.WaitOne();
                if (!token.ConnectSocket.ReceiveAsync(token.ReceiveEventArgs))
                    ProcessReceive(token.ReceiveEventArgs);
            }
        }


        /// <summary>
        /// 处理回应逻辑
        /// </summary>
        /// <returns></returns>
        protected virtual void OnReceiveSuccess(AsyncUserToken token)
        {
            Interlocked.Increment(ref m_totalSerial);
            Interlocked.Add(ref m_totalReceiveBytes, token.ReceiveEventArgs.BytesTransferred);
            token.ActiveDateTime = DateTime.Now;
        }


        /// <summary>
        /// 发送数据
        /// </summary>
        protected void SendAsync(AsyncUserToken userToken)
        {
            lock(userToken)
            {
                userToken.SendEvent.WaitOne();
                if (!userToken.ConnectSocket.SendAsync(userToken.SendEventArgs))
                    ProcessSend(userToken.SendEventArgs);
            }       
        }

        private void ProcessSend(SocketAsyncEventArgs e)
        {
            AsyncUserToken userToken = e.UserToken as AsyncUserToken;
            lock(userToken)
            {
                if (!userToken.IsActive)
                    return;

                userToken.SendEvent.Set();
                if (e.SocketError == SocketError.Success)
                {
                    OnSendSuccess(userToken);
                }
                else
                {
                    CloseClientSocket(userToken);
                }
            }
        }

        public virtual void OnSendSuccess(AsyncUserToken token)
        {
            Interlocked.Add(ref m_totalSendBytes, token.SendEventArgs.BytesTransferred);
            token.ActiveDateTime = DateTime.Now;
        }

        /// <summary>
        /// 关闭并释放用户连接
        /// </summary>
        public virtual void CloseClientSocket(AsyncUserToken userToken)
        {
            System.Diagnostics.Debug.WriteLine("closeClientsocket");

            lock (userToken)
            {
                if (!userToken.IsActive) return;
                //if (null == userToken.ConnectSocket) return;

                string socketInfo = string.Format("本地地址 : {0} 远程地址 : {1}", userToken.ConnectSocket.LocalEndPoint,
                    userToken.ConnectSocket.RemoteEndPoint);
                try
                {
                    userToken.ConnectSocket.Shutdown(SocketShutdown.Both);
                }
                catch (SocketException ex)
                {
                    Notifier?.OnNotify(NotifyType.Error, string.Format("断开连接 {0} 时发生错误: {1}", socketInfo, ex.Message), this);
                }

                if(userToken.User.IsLogined)
                    userToken.User.LoginOut();
                userToken.ConnectSocket.Close();
                userToken.ConnectSocket = null;
                userToken.SendEvent.Set();
                userToken.RecvEvent.Set();
                userToken.IsActive = false;

                m_asyncUserTokenPool.Push(userToken);
                ConnectionList.Remove(userToken);
                m_maxNumberAcceptClients.Release();
            }


        }

        /// <summary>
        /// 检测连接对象状态
        /// </summary>
        /// <param name="checkBreak">判断是否中断的回调</param>
        public void DetectionUserHandle(Func<bool> checkBreak = null)
        {
            AsyncUserToken[] tokens=null;
            ConnectionList.CopyList(ref tokens);
            for (int i = 0; i < tokens.Length; i++)
            {
                if (checkBreak != null && checkBreak())
                    break;
                try
                {
                    if ((DateTime.Now - tokens[i].ActiveDateTime).TotalMilliseconds >= SocketTimeOutMS )
                    {
                        CloseClientSocket(tokens[i]);
                    }
                }
                catch (Exception e)
                {
                    Notifier.OnNotify(NotifyType.Error, string.Format("守护线程关闭套接字时超时，错误信息: {0}", e.Message), this);
                }
            }
        }

        /// <summary>
        /// 广播数据  （！调用此方法的上层不能锁某个<see cref="ServerCommon.NetServer.AsyncUserToken"/>，否则就在新线程里调用该方法）
        /// </summary>
        /// <param name="data">数据</param>
        /// <param name="isNeedLogin">广播给是否需要登录的人</param>
        public virtual void Broadcast(byte[] data,bool isNeedLogin=true)
        {
            AsyncUserToken[] tokens = null;
            ConnectionList.CopyList(ref tokens);

            for (int i = tokens.Length - 1; i >= 0; i--)
            {
                lock(tokens[i])
                {
                    //目前取消登录后 不会断开连接 所以要靠这里进行心跳判断
                    if (!tokens[i].IsActive || (isNeedLogin && !tokens[i].IsLogined))
                        continue;
                    tokens[i].ConnectSocket.BeginSend(data, 0, data.Length, SocketFlags.None,
                        SendCompletd, (tokens[i],tokens[i].ConnectSocket));
                }    
            }
        }

        private void SendCompletd(IAsyncResult result)
        {
            (AsyncUserToken token,Socket st)  = ((AsyncUserToken token, Socket st))result.AsyncState;

            lock (token)
            {
                //因为在到这一步（lock）之前 可能会发生该token被关闭并push到栈里 然后又有人连接又被pop出来 然后又把新的socket赋值个这个token的ConnectSocket
                //所以 要判断一下这里的socket是不是原来BeginSend的socket
                if (token.ConnectSocket != st)
                    return;
                if (!token.IsActive)
                    return;

                int bytes = token.ConnectSocket.EndSend(result, out SocketError error);
                if (error != SocketError.Success)
                {
                    CloseClientSocket(token);
                }
                else
                {
                    token.ActiveDateTime = DateTime.Now;
                    Interlocked.Add(ref m_totalSendBytes, bytes);
                }
            }
        }
    }
}