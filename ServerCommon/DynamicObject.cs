using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommon
{
    /// <summary>
    /// 动态类型的数据
    /// </summary>
    public interface IDynamicType
    {
        /// <summary>
        /// 返回指定的类型值
        /// </summary>
        T GetValue<T>();
        /// <summary>
        /// 返回指定的类型值
        /// </summary>
        object GetValue(Type dataType);

        /// <summary>
        /// 获取子类型数组 如果有
        /// </summary>
        IDynamicType[] GetChids();

        /// <summary>
        /// 根据名字获取子类型 如果没有返回null
        /// </summary>
        IDynamicType GetChid(string name);
    }
}
