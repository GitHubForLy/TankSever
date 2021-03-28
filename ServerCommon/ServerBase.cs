using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServerCommon
{
    public abstract class ServerBase:NotifyServer
    {
        private Thread _thread;

        /// <summary>
        /// 执行间隔
        /// </summary>
        protected int RunInterval { get; set; } = 1000;
        public bool IsStop { get; private set; }

        public ServerBase(INotifier notifier) : base(notifier) 
        {
        }


        public void Start()
        {
            _thread = new Thread(ThreadStart);
            IsStop = false;
            _thread.Start();
        }

        public void Stop()
        {
            IsStop = true;
        }

        private void ThreadStart()
        {
            while(!IsStop)
            {
                Run();

                DateTime now = DateTime.Now;
                while((DateTime.Now-now).TotalMilliseconds<RunInterval)
                {
                    if(RunInterval<100)
                        Thread.Sleep(RunInterval);
                    else
                        Thread.Sleep(100);
                    if (IsStop)
                        return;
                }
            }
        }

        public abstract void Run();
    }
}
