using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ServerCommon;

namespace ServerCommon.NetServer
{

    /// <summary>
    /// 表示一个异步套接字服务
    /// </summary>
    public abstract class AsyncSocketServerBase : NotifyServer, IDisposable
    {
        private Socket m_listenSocket;
        private AsyncUserTokenPool m_asyncUserTokenPool;
        private AsyncUserTokenList m_asyncUserTokenList;
        private Semaphore m_maxNumberAcceptClients;  //限制访问接收的连接的线程数，控制最大并发数
        private DaemonThread m_daemonThread;        //守护线程 负责心跳检查
        private int m_totalSerial;
        private int m_totalReceiveBytes;
        private int m_totalSendBytes;
        private bool m_disposed;
        private bool m_isInit = false;


        protected int NumMaxConnctions;

        /// <summary>
        /// 已连接的所有客户
        /// </summary>
        public AsyncUserTokenList AsyncSocketUserTokenList { get { return m_asyncUserTokenList; } }

        /// <summary>
        /// 连接超时时间，单位MS 默认10秒
        /// </summary>
        public int SocketTimeOutMS { set; get; } = 1000 * 10;
        /// <summary>
        /// 接收缓冲区大小 默认1kb
        /// </summary>
        public int ReceiveBufferSize { get; set; } = 1024;

        public int TotalSerial { get { return m_totalSerial; } }
        public int TotalReceiveBytes { get { return m_totalReceiveBytes; } }
        public int TotalSendBytes { get { return m_totalSendBytes; } }

        //指示服务是否关闭
        public bool IsClosed { set; get; }


        public AsyncSocketServerBase() : this(100) { }

        public AsyncSocketServerBase(int MaxConnections):this(MaxConnections,null){}

        public AsyncSocketServerBase(int MaxConnections, INotifier notifier) : base(notifier)
        {
            NumMaxConnctions = MaxConnections;
            m_maxNumberAcceptClients = new Semaphore(NumMaxConnctions, NumMaxConnctions);

            m_asyncUserTokenList = new AsyncUserTokenList();
            m_asyncUserTokenPool = new AsyncUserTokenPool(NumMaxConnctions);

            m_totalSerial = 0;
            m_totalReceiveBytes = 0;
            m_totalSendBytes = 0;
            m_disposed = false;
            IsClosed = true;
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
                m_asyncUserTokenList = null;
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
            return new AsyncUserToken(this,ReceiveBufferSize);
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
                Notify(NotifyType.RequsetErrorLog, "服务未初始化,无法启动",this);
                return;
            }

            IsClosed = false;
            m_listenSocket = new Socket(localEP.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            m_listenSocket.Bind(localEP);
            m_listenSocket.Listen(NumMaxConnctions);
            Notifier?.OnNotify(NotifyType.RunLog, string.Format("开始监听端口：{0}", localEP.ToString()), this);

            StartAccept(null);
            m_daemonThread = new DaemonThread(this, Notifier);
        }

        public void Close()
        {
            AsyncUserToken[] array = null;
            m_asyncUserTokenList.CopyList(ref array);

            for (int i = 0; i < array.Length; i++)
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
                Notifier?.OnNotify(NotifyType.RunLog, string.Format("关闭网络服务发生错误：{0}", e.Message), this);
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
                Notifier?.OnNotify(NotifyType.RunLog, string.Format("等待用户连接时发生错误 : {0}", ex.Message), this);
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
                Notifier?.OnNotify(NotifyType.RunLog, string.Format("接受用户{0}连接时发生错误: {1}", e.AcceptSocket, ex.Message), this);
            }
        }

        private void ProcessAccept(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success)
            {
                Notifier?.OnNotify(NotifyType.RunLog, string.Format("成功与用户建立连接，本地地址: {0}, 远程地址: {1}",
                     e.AcceptSocket.LocalEndPoint, e.AcceptSocket.RemoteEndPoint), this);

                AsyncUserToken userToken = m_asyncUserTokenPool.Pop();
                userToken.ConnectSocket = e.AcceptSocket;
                userToken.ConnectDateTime = DateTime.Now;
                m_asyncUserTokenList.Add(userToken);

                try
                {
                    bool willRaiseEvent = userToken.ConnectSocket.ReceiveAsync(userToken.ReceiveEventArgs);
                    if (!willRaiseEvent)
                    {
                        ProcessReceive(userToken.ReceiveEventArgs);
                    }
                }
                catch (Exception ex)
                {
                    Notifier?.OnNotify(NotifyType.RunLog, string.Format("接受用户{0}的数据时发生错误: {1}", userToken.ConnectSocket, ex.Message), this);
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
                Notifier?.OnNotify(NotifyType.RunLog, string.Format("用户{0}读取或发送时发生错误：{1}", userToken.ConnectSocket, ex.Message), this);
            }
        }

        private void ProcessReceive(SocketAsyncEventArgs e)
        {
            AsyncUserToken userToken = e.UserToken as AsyncUserToken;
            if (null == userToken.ConnectSocket)
                return;

            userToken.ActiveDateTime = DateTime.Now;
            int offset = userToken.ReceiveEventArgs.Offset;
            int count = userToken.ReceiveEventArgs.BytesTransferred;

            if (count > 0 && userToken.ReceiveEventArgs.SocketError == SocketError.Success)
            {
                Interlocked.Increment(ref m_totalSerial);
                Interlocked.Add(ref m_totalReceiveBytes, count);

                bool isClose = true;
                try
                {
                    isClose = !HandleReceive(e);
                }
                catch (Exception err)
                {
                    CloseClientSocket(userToken);
                    throw new Exception(err.Message, err);
                }

                if (isClose) //是否需要短连接
                {
                    CloseClientSocket(userToken);
                }
                else
                {
                    //接收下一次数据
                    bool willRaiseEvent = userToken.ConnectSocket.ReceiveAsync(userToken.ReceiveEventArgs);
                    if (!willRaiseEvent)
                    {
                        ProcessReceive(userToken.ReceiveEventArgs);
                    }
                }
            }
            else
            {
                CloseClientSocket(userToken);
            }
        }

        /// <summary>
        /// 处理回应逻辑
        /// </summary>
        /// <returns></returns>
        protected abstract bool HandleReceive(SocketAsyncEventArgs receiveArg);


        /// <summary>
        /// 发送数据
        /// </summary>
        protected void SendData(AsyncUserToken userToken, byte[] data)
        {
            if (userToken.IsSending)
                throw new Exception("已经在发送中");
            userToken.SendEventArgs.SetBuffer(data, 0, data.Length);
            userToken.IsSending = true;
            if (!userToken.ConnectSocket.SendAsync(userToken.SendEventArgs))
                ProcessSend(userToken.SendEventArgs);
        }

        private void ProcessSend(SocketAsyncEventArgs e)
        {
            AsyncUserToken userToken = e.UserToken as AsyncUserToken;
            userToken.IsSending = false;

            if (e.SocketError == SocketError.Success)
            {
                Interlocked.Add(ref m_totalSendBytes, e.BytesTransferred);
                userToken.ActiveDateTime = DateTime.Now;
            }
            else
            {
                CloseClientSocket(userToken);
            }
        }

        /// <summary>
        /// 关闭并释放用户连接
        /// </summary>
        public virtual void CloseClientSocket(AsyncUserToken userToken)
        {
            if (null == userToken.ConnectSocket) return;

            string socketInfo = string.Format("本地地址 : {0} 远程地址 : {1}", userToken.ConnectSocket.LocalEndPoint,
                userToken.ConnectSocket.RemoteEndPoint);
            try
            {
                userToken.ConnectSocket.Shutdown(SocketShutdown.Both);
            }
            catch (SocketException ex)
            {
                Notifier?.OnNotify(NotifyType.RunLog, string.Format("断开连接 {0} 时发生错误: {1}", socketInfo, ex.Message), this);
            }
            userToken.ConnectSocket.Close();
            userToken.ConnectSocket = null;

            m_asyncUserTokenPool.Push(userToken);
            m_asyncUserTokenList.Remove(userToken);
            m_maxNumberAcceptClients.Release();
        }

        /// <summary>
        /// 检测连接对象状态
        /// </summary>
        /// <param name="checkBreak">判断是否中断的回调</param>
        public void DetectionUserHandle(Func<bool> checkBreak = null)
        {

            AsyncUserToken[] userTokenArray = null;
            AsyncSocketUserTokenList.CopyList(ref userTokenArray);
            for (int i = 0; i < userTokenArray.Length; i++)
            {
                if (checkBreak != null && checkBreak())
                    break;
                try
                {
                    if ((DateTime.Now - userTokenArray[i].ActiveDateTime).TotalMilliseconds >= SocketTimeOutMS)
                    {
                        lock (userTokenArray[i])
                            CloseClientSocket(userTokenArray[i]);
                    }
                }
                catch (Exception e)
                {
                    Notifier.OnNotify(NotifyType.RunLog, string.Format("守护线程关闭套接字时超时，错误信息: {0}", e.Message), this);
                }
            }
        }
    }
}