﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCommon.Protocol;
using TankSever.BLL.Model;
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
            _dynamicData = data;
            _req = _dynamicData.GetValue<Request>();
            switch(_req.Controller)
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
                return true;

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