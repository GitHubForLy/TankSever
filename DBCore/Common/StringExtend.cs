using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DBCore.Common
{
    public static class StringExtend
    {
        /// <summary>
        /// 检查该字符是否数据库安全（防止注入攻击）
        /// </summary>
        public static bool IsDBSafe(this string text)
        {
            if (string.IsNullOrEmpty(text))
                return true;
            return Regex.IsMatch(text, @"\w*((\％27)|(\'))((\％6F)|o|(\％4F))((\％72)|r|(\％52))");            
        }
    }
}
