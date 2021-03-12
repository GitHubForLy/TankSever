using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCommon.DB;
using System.Data.SqlClient;

namespace DBCore.SqlServer
{
    /// <summary>
    /// 可以创建<see cref="DBCore.SqlServer.SqlServerExecuter"/>的工厂
    /// </summary>
    public class SqlServerDBExecuterFactory : IDBExecuterFactory
    {
        private SqlConnectionStringBuilder connectionStringBuilder;

        public SqlServerDBExecuterFactory(string connectString):this(new SqlConnectionStringBuilder(connectString))
        {
        }
        public SqlServerDBExecuterFactory(SqlConnectionStringBuilder stringBuilder)
        {
            connectionStringBuilder = stringBuilder;
        }

        /// <summary>
        /// 创建一个新的Executer 
        /// </summary>
        public IDBExecuter CreateDBExecuter()
        {
            return new SqlServerExecuter(connectionStringBuilder.ConnectionString);
        }


        ///// <summary>
        ///// 创建一个新的Executer 
        ///// </summary>
        ///// <param name="connectString">连接字符</param>
        //public IDBExecuter CreateDBExecuter(string connectString)
        //{
        //    return new SqlServerExecuter(connectString);
        //}

        ///// <summary>
        ///// 创建一个新的Executer 
        ///// </summary>
        ///// <param name="connectString">连接字符对象</param>
        //public IDBExecuter CreateDBExecuter(DbConnectionStringBuilder stringBuilder)
        //{
        //    return new SqlServerExecuter(stringBuilder as SqlConnectionStringBuilder);
        //}
    }
}
