using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;

namespace ServerCommon.DB
{
    /// <summary>
    /// 定义一个可以创建<see cref="ServerCommon.DB.IDBExecuter"/>的工厂接口
    /// </summary>
    public interface IDBExecuterFactory
    {
        /// <summary>
        /// 创建一个数据库帮助类
        /// </summary>
        IDBExecuter CreateDBExecuter();

        //IDBExecuter CreateDBExecuter(string connectString);
        //IDBExecuter CreateDBExecuter(DbConnectionStringBuilder connectStringBuilder);
    }
}
