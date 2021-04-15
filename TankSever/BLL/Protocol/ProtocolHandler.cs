using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCommon.Protocol;
using DataModel;
using ServerCommon;
using TankSever.BLL.Controllers;
using System.Reflection;

namespace TankSever.BLL.Protocol
{
    public sealed class ProtocolHandler : ProtocolHandlerBase
    {
        private IDynamicType _dynamicData;
        private Request _req;


        public override IController CreateController(IDynamicType data)
        {
            _req = data.GetValue<Request>();
            _dynamicData= data.GetChid(nameof(_req.Data));
            switch (_req.Controller)
            {
                case ControllerConst.Event:
                    return new EventController();
                case ControllerConst.Broad:
                    return new BroadcastController();
            }
            return null;
        }

        public override bool TryExecuteAction(IController controller, out object res)
        {
            res = null;
            List<object> pendingPars = new List<object>();
            var method= controller.GetType().GetMethod(_req.Action);
            if(method==null)
                return false;

            if (!Auth(controller,method, out res))
            {
                if (method.ReturnType == typeof(void))
                    return false;
                else
                    return true;
            }

            var pars = method.GetParameters();
            if(pars.Length>1)
            {
                foreach (var par in pars)
                {
                    var data = _dynamicData.GetChid(par.Name)?.GetValue(par.ParameterType);
                    pendingPars.Add(data);
                }
            }
            else if(pars.Length==1)
            {
                var data = _dynamicData.GetValue(pars[0].ParameterType);
                pendingPars.Add(data);
            }

    
            res=method.Invoke(controller, pendingPars.ToArray());

            if (method.ReturnType == typeof(void))
                return false;


            //var resType = typeof(Respone<>).MakeGenericType(res.GetType());
            //var resp = resType.GetConstructor(Type.EmptyTypes).Invoke(null);
            //resType.GetField("RequestId").SetValue(resp, _req.RequestId);
            //resType.GetField("Controller").SetValue(resp, _req.Controller);
            //resType.GetField("Action").SetValue(resp, _req.Action);
            //resType.GetField("Data").SetValue(resp, res);
            var respone = new Respone<object>
            {
                RequestId = _req.RequestId,
                Controller = _req.Controller,
                Action = _req.Action,
                Data = res
            };

            res = respone;
            return true;
        }

        /// <summary>
        /// 登陆验证    暂时写成这样 之后可以添加 action过滤器来实现
        /// </summary>
        private bool Auth(IController controller,MethodInfo Action,out object UnAuthRes)
        {
            UnAuthRes = null;

            if (controller.GetType().IsDefined(typeof(AllowAnonymousAttribute))
                || Action.IsDefined(typeof(AllowAnonymousAttribute)))
                return true;

            if (!controller.User.IsLogined)
            {
                UnAuthRes = StandRespone.FailResult("请求未授权");
                return false;
            }

            return true;
        }
    }
}
