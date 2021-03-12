using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Common;
using System.Data;


namespace ServerCommon.DB
{
    /// <summary>
    /// 定义数据库基本操作的类  并且该类推荐即用即毁
    /// </summary>
    public interface IDBExecuter:IDisposable
    {
        /// <summary>
        /// 连接对象
        /// </summary>
        DbConnection Connection { get; set; }

        int ExecuteNonQuery(string cmd);

        DbDataReader ExecuteReader(string cmd);

        object ExecuteScalar(string cmd);

        /// <summary>
        /// 执行指定sql语句 并返回查询的第一个数据表
        /// </summary>
        DataTable ExecuteToTable(string cmd);

        /// <summary>
        /// 执行指定sql语句 并返回查询的数据集
        /// </summary>
        DataSet ExecuteToDataSet(string cmd);

        /// <summary>
        /// 关闭连接 释放资源
        /// </summary>
        void Close();
    }
}
