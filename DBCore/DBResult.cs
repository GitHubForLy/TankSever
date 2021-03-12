using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace DBCore
{
    /// <summary>
    /// 定义一个数据库服务执行后返回的结果
    /// </summary>
    public struct DBResult
    {
        public DBResult(bool isSuccess,string message)
        {
            IsSuccess = isSuccess;
            Message = message;
            Data = new DataTable();
        }

        /// <summary>
        /// 返回一个不安全的请求
        /// </summary>
        public static DBResult UnSafeResult()
        {
            return new DBResult(false, "请求无效");
        }

        /// <summary>
        /// 失败的请求
        /// </summary>
        public static DBResult FailResult(string message)
        {
            return new DBResult(false, message);
        }

        /// <summary>
        /// 成功的请求
        /// </summary>
        public static DBResult SuccessResult(string messsage)
        {
            return new DBResult(true, messsage);
        }


        /// <summary>
        /// 是否成功
        /// </summary>
        public bool IsSuccess { get; set; }
        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 数据表
        /// </summary>
        public DataTable Data { get; set; } 
    }
}
