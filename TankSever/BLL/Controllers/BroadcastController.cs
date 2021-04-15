using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using ServerCommon.Protocol;
using TankSever.BLL.Server;

namespace TankSever.BLL.Controllers
{
    class BroadcastController : Controller
    {
        //广播位置信息
        public void UpdateTransform((string account,Transform transform) data)
        {
            DataCenter.BroadcastQueue.Enqueue((BroadcastActions.UpdateTransform, data));
        }

        //广播方法调用
        public void BroadcastMethod((string account,SyncMethod info)data)
        {
            DataCenter.BroadcastQueue.Enqueue((BroadcastActions.BroadcastMethod, data));
        }

        //广播字段复制
        public void BroadcastField(string account)
        {
            DataCenter.BroadcastQueue.Enqueue((BroadcastActions.BroadcastField, account));
        }

        //广播登录
        public void Login(LoginInfo info)
        {
            DataCenter.BroadcastQueue.Enqueue((BroadcastActions.Login, info));
        }
    }
}
