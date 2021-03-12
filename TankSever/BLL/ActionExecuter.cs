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
            foreach(var type in assmbly.GetTypes())
            {
                if (CheckController(type)) 

            }

        }
    

        protected virtual bool CheckController(Type type)
        {
            if (type.BaseType == typeof(IController) && type.i)
                return true;
            return false;
        }
    
    }

}
