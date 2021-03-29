using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ServerCommon;
using DataModel;

namespace TankSever.BLL.Server
{
    class BroadcastServer :ServerBase
    {
        public override string ServerName => "广播服务";
        private IDataFormatter _dataFormatter;

        public BroadcastServer(INotifier notifier):base(notifier)
        {
            RunInterval = 100;
            _dataFormatter = DI.Instance.Resolve<IDataFormatter>();
        }

        public override void Run()
        {
            try
            {
                UpadateTransform();
            }
            catch(Exception e)
            {
                Notify(NotifyType.Error, "广播数据错误:" + e.Message, this);
            }
       
        }

        private void UpadateTransform()
        {
            var trans= DataCenter.Instance.GetTransforms();
            if(trans.Count>0)
            {
                Respone respone = new Respone()
                {
                    Controller = ControllerConst.Broad,
                    Action = nameof(UpadateTransform),
                    Data = trans
                };
                var data = _dataFormatter.Serialize(respone);

                Program.NetServer.Broadcast(data);
            }
        }
    }
}
