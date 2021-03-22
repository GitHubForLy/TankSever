using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommon.Protocol
{
    public abstract class ActionExecuter : IActionExecuter
    {
        /// <summary>
        /// 调用行为
        /// </summary>
        /// <param name="executeContext">调用上下文</param>
        public object ExecuteAction(ExecuteContext executeContext)
        {
            var assmbly = GetActionAssembly();
            MethodInfo methodInfo;
            foreach (var type in assmbly.GetTypes())
            {
                if (typeof(ControllerBase).IsAssignableFrom(type) && type.Name == executeContext.ControllerName + "Controller")
                {
                    if ((methodInfo = type.GetMethod(executeContext.ActionName)) != null)
                    {
                        return InvokeMethod(type, executeContext, methodInfo);
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 尝试执行行为
        /// </summary>
        /// <param name="executeContext"></param>
        /// <param name="result"></param>
        /// <returns>返回ture找到了acion false反之</returns>
        public bool TryExecuteAction(ExecuteContext executeContext, out object result)
        {
            var assmbly = GetActionAssembly();
            MethodInfo methodInfo;
            foreach (var type in assmbly.GetTypes())
            {
                if (typeof(ControllerBase).IsAssignableFrom(type) && type.Name == executeContext.ControllerName + "Controller")
                {
                    if ((methodInfo = type.GetMethod(executeContext.ActionName)) != null)
                    {
                        result= InvokeMethod(type, executeContext, methodInfo);
                        return true;
                    }
                }
            }

            result = null;
            return false;
        }

        /// <summary>
        /// 调用action方法
        /// </summary>
        /// <param name="controller">控制器类型</param>
        /// <param name="executeContext">调用上下文</param>
        /// <param name="method">调用的action方法</param>
        public object InvokeMethod(Type controller, ExecuteContext executeContext, MethodInfo method)
        {
            List<object> parms = new List<object>();
            var methodPars = method.GetParameters();
            object parValue;
            ModelStates modelStates = new ModelStates();
            modelStates.IsValid = true;

            foreach (var par in methodPars)
            {
                //获取对应方法参数的值
                parValue = GetTargetParameterValue(par,executeContext);
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

            var instance = controller.GetConstructor(Type.EmptyTypes).Invoke(new object[0]);
            if (instance is ControllerBase controllerBase)
            {
                controllerBase.ModelState = modelStates;
                controllerBase.UserConnect = new UserConnection(executeContext.UserToken);
            }

            return method.Invoke(instance, parms.ToArray());
        }


        protected virtual Assembly GetActionAssembly()
        {
            return Assembly.GetExecutingAssembly();
        }

        /// <summary>
        /// 根据指定的参数信息和执行上下文 获取参数值
        /// </summary>
        /// <param name="parameter">目标参数信息</param>
        /// <param name="executeContext">执行上下文</param>
        /// <returns>参数的值 可以返回null</returns>
        public abstract object GetTargetParameterValue(ParameterInfo parameter, ExecuteContext executeContext);

    }
}
