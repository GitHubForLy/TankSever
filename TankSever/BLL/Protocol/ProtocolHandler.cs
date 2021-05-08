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
using ProtobufProto.Model;

namespace TankSever.BLL.Protocol
{
    public sealed class ProtocolHandler : ProtocolHandlerBase
    {
        //private IDynamicType _dynamicData;
        //private Request _req;
        private  byte[] reqData;
        private TcpFlags action;
        private IDataFormatter formatter;

        public override IController CreateController(byte[] data)
        {
            reqData = ProtobufDataPackage.UnPackageData(data, out action);
            formatter = DI.Instance.Resolve<IDataFormatter>();
            //简单一点先直接返回这个控制器   之后复杂一点可以反射遍历所有控制器然后创建
            return new EventController();
        }

        public override bool TryExecuteAction(IController controller, out byte[] res)
        {
            res = null;
            object resobj;
            List<object> pendingPars = new List<object>();

            var methods = controller.GetType().GetMethods(BindingFlags.Public|BindingFlags.Instance);
            MethodInfo method=null;
            foreach (var md in methods)
            {
                if (md.IsDefined(typeof(RequestAttribute)))
                {
                    var attr= md.GetCustomAttribute<RequestAttribute>();
                    if(attr.ReqestType==action)
                    {
                        method = md;
                        break;
                    }
                }
            }


            if(method==null)
                return false;

            if (!Auth(controller,method))
            {
                resobj = StandRespone.FailResult("请求未授权");
                if (method.ReturnType == typeof(void))
                    return false;
                else
                {
                    res = ProtobufDataPackage.PackageData(action, formatter.Serialize(resobj));
                    return true;
                }
            }

            Notify(NotifyType.Message, "请求:["+ controller.User.UserAccount + "][" + action.ToString() + "]");

            var pars = method.GetParameters();
            if(pars.Length>1)
            {
                //控制器方法参数个数大于1 暂时不处理
                Notify(NotifyType.Warning, "请求的方法参数大于1，请求将被忽略,方法名:" + method.Name);
                return false;
            }
            else if(pars.Length==1)
            {
                var data = formatter.Deserialize(pars[0].ParameterType, reqData);
                pendingPars.Add(data);
            }
    
            resobj=method.Invoke(controller, pendingPars.ToArray());

            if (method.ReturnType == typeof(void))
                return false;

            res=ProtobufDataPackage.PackageData(action, formatter.Serialize(resobj));
            return true;
        }

        /// <summary>
        /// 登陆验证    暂时写成这样 之后可以添加 action过滤器来实现
        /// </summary>
        private bool Auth(IController controller,MethodInfo Action)
        {
            if (controller.GetType().IsDefined(typeof(AllowAnonymousAttribute))
                || Action.IsDefined(typeof(AllowAnonymousAttribute)))
                return true;

            if (!controller.User.IsLogined)
            {
                return false;
            }

            return true;
        }
    }
}
