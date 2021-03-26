using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Reflection;
using ServerCommon.NetServer;

namespace ServerCommon.Protocol
{
    public class ModelStates
    {
        public string InvalidModelName { get; set; }
        public object InvalidModelValue { get; set; }
        public string ErrorMessage { get; set; }
        public  bool IsValid { get; set; }
    }

    

    /// <summary>
    /// 基本的控制器 控制器的基类
    /// </summary>
    public abstract class Controller:IController
    {
        /// <summary>
        /// 用户连接对象
        /// </summary>
        public AsyncUser User { get; set; }

        public ModelStates ModelState { get; set; }

    }
}
