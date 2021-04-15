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
using ServerCommon.Protocol;
using DataModel;

namespace DBCore
{

    /// <summary>
    /// <B>MySql数据库服务</B>
    /// xml注释讲解： <a href="https://www.cnblogs.com/mq0036/p/6540444.html">这里</a>
    ///</summary>

    public sealed class DBServer : IServer
    {
        private static DBServer instance;

        public IDBExecuterFactory DBExecuterFactory { get; private set; }

        /// <summary>
        /// 服务名称
        /// </summary>
        public string ServerName { get => "MySqlDBServer"; }

        /// <summary>
        /// 当前数据库服务唯一实例
        /// </summary>
        public static DBServer Instance 
        { 
            get
            {
                if (instance == null)
                    instance= new DBServer();
                return instance;
            } 
        }

        private DBServer(){}

        
        [InjectionMethod]
        public void RegisterFactory(IDBExecuterFactory DBExecuterFactory)
        {
            this.DBExecuterFactory = DBExecuterFactory;
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

        public StandRespone Regeister(string userAccount,string password,string salt)
        {
            if (!userAccount.IsDBSafe() || !password.IsDBSafe())
                return StandRespone.UnSafeResult();

            var executer=DBExecuterFactory.CreateDBExecuter();

            try
            {
                var queryExitUser = $"select* from userinfo where account = '{userAccount}'";
                if (executer.ExecuteNonQuery(queryExitUser) > 0)
                    return new StandRespone(false, "用户已存在!");
                
                var trans= executer.Connection.BeginTransaction();
                var insertUser = $"insert into userinfo (account,create_date,status) " +
                    $"values('{userAccount}',now(),'{UserStatus.Normal}');"+
                    $"insert into user_password(user_id,password,salt) " +
                    $"values(@@identity,'{password}','{salt}')";
                executer.ExecuteNonQuery(insertUser);
                trans.Commit();

                return StandRespone.SuccessResult("注册成功");
            }
            catch(Exception e)
            {
                executer.Close();
                return StandRespone.FailResult("发生异常:" + e.Message);
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
        public StandRespone<(string salt,string pwd)> GetPassword(string userAccount)
        {
            if (!userAccount.IsDBSafe())
                return StandRespone<(string,string)>.UnSafeResult();

            var executer = DBExecuterFactory.CreateDBExecuter();
            try
            {
                var cmd = $"select a.salt,a.password from user_password a " +
                    $"inner join userinfo b on a.user_id=b.user_id " +
                    $"where b.account='{userAccount}'";
                var data=executer.ExecuteToTable(cmd);
                if (data.Rows.Count > 0)
                    //return new StandRespone(true, "查询成功") { Data = data };
                    return new StandRespone<(string, string)>(true, "查询成功", (data.Rows[0]["salt"].ToString(), data.Rows[0]["password"].ToString()));
                else
                    return new StandRespone<(string,string)>(false, "该账号不存在");
            }
            catch(Exception e)
            {
                executer.Close();
                return StandRespone<(string, string)>.FailResult("发生异常:" + e.Message);
            }
            finally
            {
                executer.Close();
            }
        }

        
    }
}
