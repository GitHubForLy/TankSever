using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCommon.DB;
using System.Data.Common;
using MySql.Data.MySqlClient;
using System.Data;

namespace DBCore.MySql
{
    /// <summary>
    /// mysql数据库帮助方法
    /// </summary>
    public class MySqlDBExecuter:IDBExecuter
    {
        private MySqlConnectionStringBuilder connectionStringBuilder;

        public DbConnection Connection { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public MySqlDBExecuter(string connectionString) : this(new MySqlConnectionStringBuilder(connectionString))
        {
        }

        public MySqlDBExecuter(MySqlConnectionStringBuilder connectionStringBuilder)
        {
            this.connectionStringBuilder = connectionStringBuilder;
            Connection = new MySqlConnection(connectionStringBuilder.ConnectionString);
            Connection.Open();
        }

        public int ExecuteNonQuery(string cmd)
        {
            DbCommand sqlCommand = new MySqlCommand(cmd, Connection as MySqlConnection);
            return sqlCommand.ExecuteNonQuery();
        }

        public DbDataReader ExecuteReader(string cmd)
        {
            DbCommand sqlCommand = new MySqlCommand(cmd, Connection as MySqlConnection);
            return sqlCommand.ExecuteReader();
        }

        public object ExecuteScalar(string cmd)
        {
            DbCommand sqlCommand = new MySqlCommand(cmd, Connection as MySqlConnection);
            return sqlCommand.ExecuteScalar();
        }

        /// <summary>
        /// 执行指定sql语句 并返回查询的第一个数据表
        /// </summary>
        public DataTable ExecuteToTable(string cmd)
        {
            MySqlCommand sqlCommand = new MySqlCommand(cmd, Connection as MySqlConnection);
            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(sqlCommand);
            var datatable = new DataTable();
            dataAdapter.Fill(datatable);
            return datatable;
        }

        /// <summary>
        /// 执行指定sql语句 并返回查询的数据集
        /// </summary>
        public DataSet ExecuteToDataSet(string cmd)
        {
            MySqlCommand sqlCommand = new MySqlCommand(cmd, Connection as MySqlConnection);
            MySqlDataAdapter dataAdapter = new MySqlDataAdapter(sqlCommand);
            var dataset = new DataSet();
            dataAdapter.Fill(dataset);
            return dataset;
        }


        /// <summary>
        /// 关闭并释放资源
        /// </summary>
        public void Close()
        {
            if (Connection != null && Connection.State != System.Data.ConnectionState.Closed)
                Connection.Close();
        }

        /// <summary>
        /// 关闭并释放资源
        /// </summary>
        public void Dispose()
        {
            Close();
        }
    }
}
