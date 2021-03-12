using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBCore
{
    /// <summary>
    /// 定义一个数据库服务执行后返回的结果
    /// </summary>
    public struct DBResultEx
    {
        public DBResultEx(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
            Datas = new DataSet();
        }

        /// <summary>
        /// 返回一个不安全的请求
        /// </summary>
        public static DBResultEx UnSafeResult()
        {
            return new DBResultEx(false, "请求无效");
        }

        /// <summary>
        /// 失败的请求
        /// </summary>
        public static DBResultEx FailResult(string message)
        {
            return new DBResultEx(false, message);
        }

        /// <summary>
        /// 成功的请求
        /// </summary>
        public static DBResultEx SuccessResult(string messsage)
        {
            return new DBResultEx(true, messsage);
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
        /// 数据集
        /// </summary>
        public DataSet Datas { get; set; }
    }
}
