using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCommon;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JsonFormatter
{
    public class JsonDataFormatter : IDataFormatter
    {
        public Encoding FormatterEncoding { get; set; } = Encoding.UTF8;

        public object Deserialize(byte[] data)
        {
            var jsonstr = FormatterEncoding.GetString(data);
            return JsonConvert.DeserializeObject(jsonstr);
        }

        public T Deserialize<T>(byte[] data)
        {
            var jsonstr = FormatterEncoding.GetString(data);
            return JsonConvert.DeserializeObject<T>(jsonstr);
        }

        public IDynamicType DeserializeDynamic(byte[] data)
        {
            var jsonstr = FormatterEncoding.GetString(data);
            var jtoken= JsonConvert.DeserializeObject<JToken>(jsonstr);
            return new JsonDynamicType(jtoken);
        }

        public byte[] Serialize(object ojb)
        {
            string str= JsonConvert.SerializeObject(ojb);
            return FormatterEncoding.GetBytes(str);
        }
    }
}
