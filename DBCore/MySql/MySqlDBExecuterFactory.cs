using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCommon.DB;
using MySql.Data.MySqlClient;

namespace DBCore.MySql
{
    /// <summary>
    /// 可以创建<see cref="DBCore.MySql.MySqlDBExecuter"/>的工厂
    /// </summary>
    public class MySqlDBExecuterFactory : IDBExecuterFactory
    {
        MySqlConnectionStringBuilder connectionStringBuilder;
        public MySqlDBExecuterFactory(string connectString) : this(new MySqlConnectionStringBuilder(connectString))
        {
        }
        public MySqlDBExecuterFactory(MySqlConnectionStringBuilder stringBuilder)
        {
            connectionStringBuilder = stringBuilder;
        }
        /// <summary>
        /// 创建一个MySql数据库帮助类
        /// </summary>
        /// <returns></returns>
        public IDBExecuter CreateDBExecuter()
        {
            return new MySqlDBExecuter(connectionStringBuilder);
        }

        //public IDBExecuter CreateDBExecuter(string connectString)
        //{
        //    return new MySqlDBExecuter(connectString);
        //}

        //public IDBExecuter CreateDBExecuter(DbConnectionStringBuilder connectStringBuilder)
        //{
        //    return new MySqlDBExecuter(connectStringBuilder as MySqlConnectionStringBuilder);
        //}
    }
}
