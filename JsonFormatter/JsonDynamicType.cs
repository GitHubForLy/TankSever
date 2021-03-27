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
    /// <summary>
    /// json动态类型
    /// </summary>
    class JsonDynamicType : IDynamicType
    {
        private JToken _jToken;
        public JsonDynamicType(JToken jToken)
        {
            _jToken = jToken;
        }


        public IDynamicType GetChid(string name)
        {
            if (_jToken.Count()<=0)
                return null;
            var jtoken = _jToken[name];
            if (jtoken == null)
                return null;

            return new JsonDynamicType(_jToken[name] as JObject);
        }

        public IDynamicType[] GetChids()
        {
            IDynamicType[] res = new IDynamicType[_jToken.Count()];
            for(int i=0; i<res.Length; i++)
            {
                res[i] = new JsonDynamicType(_jToken.ElementAt(i));
            }
            return res;
        }

        public T GetValue<T>()
        {
            return _jToken.ToObject<T>();
        }

        public object GetValue(Type dataType)
        {
            return _jToken.ToObject(dataType);
        }
    }
}
