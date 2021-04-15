using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace DataModel
{


    /// <summary>
    /// 标准请求响应
    /// </summary>
    public class StandRespone
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }

        public StandRespone()
        {
            IsSuccess = false;
        }

        public StandRespone(bool isSuccess) : this(isSuccess, "") { }

        public StandRespone(bool isSuccess, string message) => (IsSuccess, Message) = (isSuccess, message);


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



    /// <summary>
    /// 标准请求响应 泛型类型
    /// </summary>
    public class StandRespone<T>:StandRespone
    {
        public T Data { get; set; }
        
        public StandRespone()
        {
            IsSuccess = false;
        }

        public StandRespone(bool isSuccess) : this(isSuccess, "") { }

        public StandRespone(bool isSuccess, string message) : this(isSuccess, message, default(T)) { }

        public StandRespone(bool isSuccess, string message, T Data):base(isSuccess,message)
        {
            this.Data = Data;
        }

        /// <summary>
        /// 返回一个不安全的请求
        /// </summary>
        public static new StandRespone<T> UnSafeResult()
        {
            return new StandRespone<T>(false, "请求无效");
        }

        /// <summary>
        /// 失败的请求
        /// </summary>
        public static new StandRespone<T> FailResult(string message)
        {
            return new StandRespone<T>(false, message);
        }

        /// <summary>
        /// 成功的请求
        /// </summary>
        public static new StandRespone<T> SuccessResult(string messsage)
        {
            return new StandRespone<T>(true, messsage);
        }
    }
}
