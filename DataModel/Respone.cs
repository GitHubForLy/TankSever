using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace DataModel
{
    public class Respone
    {
        public int RequestId { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }

    }
    public class Respone<T>:Respone
    {
        public T Data {get;set;}

        public static object Create(Type DataType,int RequestId,string Controller,string Action,object Data)
        {
            var resType = typeof(Respone<>).MakeGenericType(DataType);
            var resp = resType.GetConstructor(Type.EmptyTypes).Invoke(null);
            resType.GetField("RequestId").SetValue(resp, RequestId);
            resType.GetField("Controller").SetValue(resp, Controller);
            resType.GetField("Action").SetValue(resp, Action);
            resType.GetField("Data").SetValue(resp, Data);
            return resp;
        }

    }

   
}
