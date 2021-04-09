using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ServerCommon;

namespace ServerCommon.NetServer
{
    public class DaemonThread //: NotifyServer
    {
        protected Thread m_thread;
        AsyncSocketServerBase m_asyncSocketServer;

        public DaemonThread(AsyncSocketServerBase asyncSocketServer, INotifier notifier) //: base(notifier)
        {
            m_asyncSocketServer = asyncSocketServer;
            m_thread = new Thread(DaemonThreadStart);
            m_thread.IsBackground = true;
            m_thread.Name = "daemonThread";
            m_thread.Start();
        }

        protected virtual void DaemonThreadStart()
        {
            try
            {
                while (m_thread.IsAlive)
                {
                    m_asyncSocketServer.DetectionUserHandle(new Func<bool>(() => !m_thread.IsAlive));
                    //每2秒检测一次
                    for (int i = 0; i < 2 * 1000 / 10; i++)
                    {
                        if (!m_thread.IsAlive)
                            break;
                        Thread.Sleep(10);
                    }
                }
            }
            catch(ThreadAbortException)
            {
            }
            catch(Exception e)
            {
                m_asyncSocketServer.Notify(NotifyType.Error, "守护线程出错:" + e.Message, m_asyncSocketServer);
            }
        }

        public void Close()
        {
            m_thread.Abort();
            m_thread.Join();
        }
    }
}
