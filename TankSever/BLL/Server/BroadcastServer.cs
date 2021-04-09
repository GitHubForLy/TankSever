using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 
using System.Threading;
using ServerCommon;
using DataModel;

namespace TankSever.BLL.Server
{
    class BroadcastServer :ServerBase
    {
        public override string ServerName => "广播服务";
        private static IDataFormatter _dataFormatter = DI.Instance.Resolve<IDataFormatter>();

        public BroadcastServer(INotifier notifier):base(notifier)
        {
            RunInterval = 100;
            UserCenter.Instance.OnUserLoginout += Instance_OnUserLoginout;
        }

        private void Instance_OnUserLoginout(string account)
        {
            BroadcastMessage("Loginout", account);
        }

        public override void Run()
        {
            try
            {
               // UpdateTransform();
            }
            catch(Exception e)
            {
                Notify(NotifyType.Error, "广播数据错误:" + e.Message, this);
            }
       
        }

        public static void BroadcastMessage(string action,object data)
        {
            Task.Run(() =>
            {
                Respone respone = new Respone()
                {
                    Controller = ControllerConst.Broad,
                    Action = action,
                    Data = data
                };
                var bytes= _dataFormatter.Serialize(respone);
                Program.NetServer.Broadcast(bytes);
            });
        }


        private void UpdateTransform()
        {
            var trans= DataCenter.Instance.GetTransforms();
            if(trans.Count>0)
            {
                Respone respone = new Respone()
                {
                    Controller = ControllerConst.Broad,
                    Action = nameof(UpdateTransform),
                    Data = trans
                };
                var data = _dataFormatter.Serialize(respone);
                Program.NetServer.Broadcast(data);
            }
        }
    }
}
