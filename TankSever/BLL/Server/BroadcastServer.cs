using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ServerCommon;

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
                BoradcastTransform();
            }
            catch(Exception e)
            {
                Notify(NotifyType.Error, "广播数据错误:" + e.Message, this);
            }
       
        }

        private void BoradcastTransform()
        {
            var trans= DataCenter.Instance.GetTransforms();
            if(trans.Count>0)
            {
                var data = _dataFormatter.Serialize(trans);

                Program.NetServer.Broadcast(data);
            }
        }
    }
}
