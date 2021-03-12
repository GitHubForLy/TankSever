using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using ServerCommon;
using System.Security.Cryptography;
using DBCore.Common;
using System.Data.Common;
using Unity;
using ServerCommon.DB;

namespace DBCore
{

    /// <summary>
    /// <B>MySql数据库服务</B>
    ///  <br/><br/>
    /// xml注释讲解： <a href="https://www.cnblogs.com/mq0036/p/6540444.html">这里</a>
    /// <br/>
    /// <see cref="P:MySql.Data.MySqlClient.MySqlConnection.Database" />
    /// <code lang="C#">
    /// MySqlDBServer s=new MySqlDBServer();
    /// </code>
    ///</summary>

    public sealed class DBServer:IServer
    {
        private DbConnectionStringBuilder connectionStringBuilder;

        [Dependency]
        public IDBExecuterFactory DBExecuterFactory { get; set; }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServerName { get => "MySqlDBServer"; } 


        public DBServer(string connectionString):this(new DbConnectionStringBuilder() { ConnectionString =connectionString })
        {
        }

        public DBServer(DbConnectionStringBuilder connectionStringBuilder)
        {
            this.connectionStringBuilder = connectionStringBuilder;
        }


        /// <summary>
        /// 用户注册
        /// </summary>
        /// <param name="userAccount">用户账号</param>
        /// <param name="password">密码</param>
        /// <param name="salt">盐</param>
        /// <example>
        ///     <code lang="C#">
        ///         //加密密码
        ///         byte[] salt = new byte[20];
        ///         new Random().NextBytes(salt);
        ///         MD5Cng md5 = new MD5Cng();
        ///         var saltpass = Encoding.UTF8.GetBytes(password).Concat(salt).ToArray();
        ///         var crpPass = md5.ComputeHash(saltpass);
        ///     </code>
        /// </example>

        public DBResult Regeister(string userAccount,string password,string salt)
        {
            if (!userAccount.IsDBSafe() || !password.IsDBSafe())
                return DBResult.UnSafeResult();

            var executer=DBExecuterFactory.CreateDBExecuter();

            try
            {
                var queryExitUser = $"select* from userinfo where account = '{userAccount}'";
                if (executer.ExecuteNonQuery(queryExitUser) > 0)
                    return new DBResult(false, "用户已存在!");
                
                var trans= executer.Connection.BeginTransaction();
                var insertUser = $"insert into userinfo (account,create_date,status) " +
                    $"values('{userAccount}',now(),'{UserStatus.Normal}');"+
                    $"insert into user_password(user_id,password,salt) " +
                    $"values(@@identity,'{password}','{salt}')";
                executer.ExecuteNonQuery(insertUser);
                trans.Commit();

                return DBResult.SuccessResult("注册成功");
            }
            catch(Exception e)
            {
                executer.Close();
                return DBResult.FailResult("发生异常:" + e.Message);
            }
            finally
            {
                executer.Close();
            }
        }


        /// <summary>
        /// 查询密码
        /// </summary>
        /// <param name="userAccount"></param>
        /// <param name="password"></param>
        public DBResult GetPassword(string userAccount)
        {
            if (!userAccount.IsDBSafe())
                return DBResult.UnSafeResult();

            var executer = DBExecuterFactory.CreateDBExecuter();
            try
            {
                var cmd = $"select a.salt,a.password from user_password a " +
                    $"inner join userinfo b on a.user_id=b.user_id " +
                    $"where b.account='{userAccount}'";
                var data=executer.ExecuteToTable(cmd);
                return new DBResult() { IsSuccess = true, Message = "查询成功", Data = data };
            }
            catch(Exception e)
            {
                executer.Close();
                return DBResult.FailResult("发生异常:" + e.Message);
            }
            finally
            {
                executer.Close();
            }
        }

        
    }
}
