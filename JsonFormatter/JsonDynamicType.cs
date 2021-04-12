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

            return new JsonDynamicType(jtoken);
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
            try
            {
                return _jToken.ToObject<T>();
            }
            catch
            {
                    return default(T);
            }
        }


        public object GetValue(Type dataType)
        {
            try
            {
                return _jToken.ToObject(dataType);
            }
            catch
            {
                if (dataType.IsValueType)
                    return Activator.CreateInstance(dataType);
                return null;
            }
        }
    }
}
