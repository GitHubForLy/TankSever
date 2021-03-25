using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommon
{
    public enum NotifyType
    {
        /// <summary>
        /// 警告
        /// </summary>
        Warning,
        /// <summary>
        /// 信息
        /// </summary>
        Message,
        /// <summary>
        /// 错误
        /// </summary>
        Error,
    }

    public interface INotifier
    {
        /// <summary>
        /// 当需要通知消息时
        /// </summary>
        void OnNotify(NotifyType type, string logInfo, IServer sender);
    }
}
