using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using ServerCommon.Protocol;
using System.ComponentModel.DataAnnotations;
using TankSever.BLL.Controller;

namespace TankSever.BLL
{
    public class ActionExecuter : IActionExecuter
    {
        /// <summary>
        /// 调用行为
        /// </summary>
        /// <param name="executeContext"></param>
        public object ExecuteAction(ExecuteContext executeContext)
        {
            var assmbly= Assembly.GetExecutingAssembly();
            MethodInfo methodInfo;
            foreach(var type in assmbly.GetTypes())
            {
                if (typeof(ControllerBase).IsAssignableFrom(type) && type.Name == executeContext.ControllerName + "Controller")
                {
                    if((methodInfo = type.GetMethod(executeContext.ActionName) )!= null)
                    {
                        return InvokeMethod(type, executeContext, methodInfo);
                    }
                }                    
            }

            return null;
        }

        
        /// <summary>
        /// 调用action方法
        /// </summary>
        /// <param name="controller">控制器类型</param>
        /// <param name="executeContext">调用上下文</param>
        /// <param name="method">调用的action方法</param>
        private object InvokeMethod(Type controller,ExecuteContext executeContext,MethodInfo method)
        {
            List<object> parms = new List<object>();
            var methodPars= method.GetParameters();
            object parValue;
            ModelStates modelStates = new ModelStates();
            modelStates.IsValid = true;

            foreach(var par in methodPars)
            {
                parValue = null;
                foreach (var dpar in executeContext.Paramters)
                {
                    if (par.Name == dpar.Key)
                    {
                        parValue = dpar.Value;
                        break;
                    }
                }
                parms.Add(parValue);

                #region 根据特性验证参数
                foreach (var attr in par.GetCustomAttributes<ValidationAttribute>(true))
                {
                    if (!attr.IsValid(parValue))
                    {
                        modelStates.IsValid = false;
                        modelStates.InvalidModelName = par.Name;
                        modelStates.ErrorMessage = attr.ErrorMessage;
                        modelStates.InvalidModelValue = parValue;
                        break;
                    }
                }
                #endregion
            }

            var instance= controller.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
            if(instance is ControllerBase)
                (instance as ControllerBase).ModelState = modelStates;

            return method.Invoke(instance, parms.ToArray());
        }
        
    }

}
