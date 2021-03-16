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
    public abstract class ControllerBase:IController
    {
        /// <summary>
        /// 用户连接对象
        /// </summary>
        public UserConnection UserConnect { get; set; }

        public ModelStates ModelState { get; set; }

        /// <summary>
        /// 成功并带有信息的响应
        /// </summary>
        protected StandRespone SuccessMessage(string message) => new StandRespone(true, message);

        /// <summary>
        /// 失败并带有信息的响应
        /// </summary>
        protected StandRespone FailMessage(string message)=>new StandRespone(false, message);

        /// <summary>
        /// 成功并带有信息和表的响应
        /// </summary>
        protected StandRespone TableMessage(string message,DataTable table)
        {
            StandRespone respone = new StandRespone(true,message)
            {
                Data = table
            };
            return respone;
        }
    }
}
