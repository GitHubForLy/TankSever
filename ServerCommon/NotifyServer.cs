using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommon
{
    /// <summary>
    /// 声明一个通知服务
    /// </summary>
    public abstract class NotifyServer : IServer
    {
        private INotifier notifier;
        /// <summary>
        /// 获取通知者对象
        /// </summary>
        public INotifier Notifier { get { return notifier; } }

        public abstract string ServerName{ get; }

        public NotifyServer(INotifier notifier)
        {
            this.notifier = notifier;
        }

        /// <summary>
        /// 通知消息
        /// </summary>
        public void Notify(NotifyType notifyType, string msg, IServer sender)
        {
            notifier?.OnNotify(notifyType, msg, sender);
        }
    }


}
