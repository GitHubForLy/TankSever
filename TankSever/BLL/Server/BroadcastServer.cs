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
            RunInterval = 200;
            DataCenter.Users.OnUserLoginout += Instance_OnUserLoginout;
        }

        private void Instance_OnUserLoginout(string account,User user)
        {
            BroadcastMessage(BroadcastActions.Loginout, (account, user.LoginTimestamp));
        }

        public override void Run()
        {
            try
            {
                bool has;
                do
                {
                    has = DataCenter.BroadcastQueue.Dequeue(RunInterval, out (string action, object data) msg);
                    if(has)
                    {
                        BroadcastMessage(msg.action, msg.data);
                    }
                }
                while (IsStop);
            }
            catch(Exception e)
            {
                Notify(NotifyType.Error, "广播数据错误:" + e.Message, this);
            }
       
        }

        public  void BroadcastMessage<T>(string action,T data)
        {
            Task.Run(() =>
            {
                Respone<T> respone = new Respone<T>()
                {
                    Controller = ControllerConst.Broad,
                    Action = action,
                    Data = data
                };
                var bytes= _dataFormatter.Serialize(respone);
                Program.NetServer.Broadcast(bytes);
            });
        }


        //private void UpdateTransform()
        //{
        //    var trans= DataCenter.Instance.GetTransforms();
        //    if(trans.Count>0)
        //    {
        //        Respone respone = new Respone()
        //        {
        //            Controller = ControllerConst.Broad,
        //            Action = nameof(UpdateTransform),
        //            Data = trans
        //        };
        //        var data = _dataFormatter.Serialize(respone);
        //        Program.NetServer.Broadcast(data);
        //    }
        //}
    }
}
