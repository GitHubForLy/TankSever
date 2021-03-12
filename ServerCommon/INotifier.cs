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
        /// 运行日志
        /// </summary>
        RunLog,
        /// <summary>
        /// 请求日志
        /// </summary>
        RequsetLog,
        /// <summary>
        /// 请求错误日志(既在前端列表输出也写入请求日志)
        /// </summary>
        RequsetErrorLog,
    }

    public interface INotifier
    {
        /// <summary>
        /// 当需要通知消息时
        /// </summary>
        void OnNotify(NotifyType type, string logInfo, IServer sender);
    }
}
