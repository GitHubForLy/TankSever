using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using ServerCommon.Protocol;
using TankSever.BLL.Controller;

namespace TankSever.BLL
{
    public class ActionExecuter : IActionExecuter
    {
        public void ExecuteAction(ExecuteContext executeContext)
        {
            var assmbly= Assembly.GetExecutingAssembly();
            MethodInfo methodInfo;
            foreach(var type in assmbly.GetTypes())
            {
                if (typeof(IController).IsAssignableFrom(type) && type.Name == executeContext.ControllerName + "Controller")
                {
                    if((methodInfo = type.GetMethod(executeContext.ActionName) )!= null)
                    {
                        InvokeMethod(type, executeContext, methodInfo);
                    }
                    break;
                }
                    
            }
        }



        protected void InvokeMethod(Type controller,ExecuteContext executeContext,MethodInfo method)
        {
            List<object> parms = new List<object>();
            var s= method.GetParameters();
            bool hasP;
            foreach(var par in s)
            {
                hasP = false;
                foreach(var dpar in executeContext.Paramters)
                {
                    if (par.Name == dpar.Key)
                    {
                        hasP = true;
                        parms.Add(dpar.Value);
                        break;
                    }
                }
                if (!hasP)//不存在该参数 传递null
                    parms.Add(null);
            }

            var instance= controller.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
            method.Invoke(instance, parms.ToArray());
        }
        
    }

}
