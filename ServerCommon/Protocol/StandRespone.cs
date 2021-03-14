using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace ServerCommon.Protocol
{
    /// <summary>
    /// 标准请求响应
    /// </summary>
    public class StandRespone
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public DataTable Data { get; set; }

        public StandRespone(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
            Data = new DataTable();
        }


        /// <summary>
        /// 返回一个不安全的请求
        /// </summary>
        public static StandRespone UnSafeResult()
        {
            return new StandRespone(false, "请求无效");
        }

        /// <summary>
        /// 失败的请求
        /// </summary>
        public static StandRespone FailResult(string message)
        {
            return new StandRespone(false, message);
        }

        /// <summary>
        /// 成功的请求
        /// </summary>
        public static StandRespone SuccessResult(string messsage)
        {
            return new StandRespone(true, messsage);
        }
    }
}
