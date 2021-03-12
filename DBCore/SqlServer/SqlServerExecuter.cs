using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data.Common;
using ServerCommon.DB;
using System.Data;

namespace DBCore.SqlServer
{
    /// <summary>
    /// sqlserver 执行帮助方法
    /// </summary>
    public class SqlServerExecuter : IDBExecuter
    {
        private SqlConnectionStringBuilder connectionStringBuilder;

        public DbConnection Connection { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public SqlServerExecuter(string connectionString) : this(new SqlConnectionStringBuilder(connectionString))
        {
        }

        public SqlServerExecuter(SqlConnectionStringBuilder connectionStringBuilder)
        {
            this.connectionStringBuilder = connectionStringBuilder;
            Connection = new SqlConnection(connectionStringBuilder.ConnectionString);
            Connection.Open();
        }

        /// <summary>
        /// 获取并打开一个新的数据库连接  用完一定要及时关闭
        /// </summary>
        public DbConnection GetConnection()
        {
            SqlConnection connection = new SqlConnection(connectionStringBuilder.ConnectionString);
            connection.Open();
            return connection;
        }

        public int ExecuteNonQuery(string cmd)
        {
            DbCommand sqlCommand = new SqlCommand(cmd, Connection as SqlConnection);
            return sqlCommand.ExecuteNonQuery();
        }

        public DbDataReader ExecuteReader(string cmd)
        {
            DbCommand sqlCommand = new SqlCommand(cmd, Connection as SqlConnection);
            return sqlCommand.ExecuteReader();
        }

        public object ExecuteScalar(string cmd)
        {
            DbCommand sqlCommand = new SqlCommand(cmd, Connection as SqlConnection);
            return sqlCommand.ExecuteScalar();
        }

        /// <summary>
        /// 执行指定sql语句 并返回查询的第一个数据表
        /// </summary>
        public DataTable ExecuteToTable(string cmd)
        {
            SqlCommand sqlCommand = new SqlCommand(cmd, Connection as SqlConnection);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlCommand);
            var datatable = new DataTable();
            dataAdapter.Fill(datatable);
            return datatable;
        }

        /// <summary>
        /// 执行指定sql语句 并返回查询的数据集
        /// </summary>
        public DataSet ExecuteToDataSet(string cmd)
        {
            SqlCommand sqlCommand = new SqlCommand(cmd, Connection as SqlConnection);
            SqlDataAdapter dataAdapter = new SqlDataAdapter(sqlCommand);
            var dataset = new DataSet();
            dataAdapter.Fill(dataset);
            return dataset;
        }

        public void Close()
        {
            if (Connection != null && Connection.State != System.Data.ConnectionState.Closed)
                Connection.Close();
        }

        public void Dispose()
        {
            Close();
        }
    }
}
