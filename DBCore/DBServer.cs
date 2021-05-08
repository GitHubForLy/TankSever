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
using ProtobufProto.Model;
using Google.Protobuf.WellKnownTypes;

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

        public Respone Regeister(string userName,string userAccount,string password,string salt)
        {
            if (!userAccount.IsDBSafe() || !password.IsDBSafe() || !userName.IsDBSafe())
                return new Respone() { IsSuccess = false, Message = "请求无效" };

            var executer=DBExecuterFactory.CreateDBExecuter();

            if(!System.Text.RegularExpressions.Regex.IsMatch(userName, @"^[0-9a-zA-Z_]{1,}$"))
                return new Respone() { IsSuccess = false, Message = "用户账号只能保护数字字母下划线" };

            try
            {
                var queryExitUser = $"select* from userinfo where account = '{userAccount}'";
                if (executer.ExecuteNonQuery(queryExitUser) > 0)
                    return new Respone() { IsSuccess = false, Message = "用户已存在" };

                var trans= executer.Connection.BeginTransaction();
                var insertUser = $"insert into userinfo (account,create_date,status,user_name) " +
                    $"values('{userAccount}',now(),'{UserStatus.Normal}','{userName}');"+

                    $"insert into user_password(user_id,password,salt) " +
                    $"values(@@identity,'{password}','{salt}')";
                executer.ExecuteNonQuery(insertUser);
                trans.Commit();

                return new Respone() { IsSuccess = true, Message = "注册成功" };
            }
            catch(Exception e)
            {
                executer.Close();
                return new Respone() { IsSuccess = false, Message = "发生异常" };
            }
            finally
            {
                executer.Close();
            }
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        public Respone GetUserInfo(string account)
        {
            if (!account.IsDBSafe())
                return new Respone() { IsSuccess = false, Message = "请求无效" };

            using (var executer = DBExecuterFactory.CreateDBExecuter())
            {
                var cmd = $"select user_name,account from userinfo " +      
                    $"where account='{account}'";

                var reader= executer.ExecuteReader(cmd);
                reader.Read();
                if(!reader.HasRows)
                    return new Respone() { IsSuccess = false, Message = "没有该账号信息" };

                UserInfo userInfo = new UserInfo
                {
                    UserName = reader["user_name"].ToString()
                };
                return new Respone() { IsSuccess = true, Data=Any.Pack(userInfo) };
            }
        }


        /// <summary>
        /// 查询密码
        /// </summary>
        /// <param name="userAccount"></param>
        /// <param name="password"></param>
        public Respone GetPassword(string userAccount,out (string salt,string pwd) resdata)
        {
            resdata = ("", "");
            if (!userAccount.IsDBSafe())
                return new Respone() { IsSuccess = false, Message = "请求无效" };

            var executer = DBExecuterFactory.CreateDBExecuter();
            try
            {
                var cmd = $"select a.salt,a.password from user_password a " +
                    $"inner join userinfo b on a.user_id=b.user_id " +
                    $"where b.account='{userAccount}'";
                var data=executer.ExecuteToTable(cmd);
                if (data.Rows.Count > 0)
                {
                    resdata.salt = data.Rows[0]["salt"].ToString();
                    resdata.pwd = data.Rows[0]["password"].ToString();
                    return new Respone() { IsSuccess = true, Message = "该账号不存在" };
                }
                else
                    return new Respone() { IsSuccess = false, Message = "该账号不存在" };
            }
            catch(Exception e)
            {
                executer.Close();
                return new Respone() { IsSuccess = false, Message = "发生异常" };
            }
            finally
            {
                executer.Close();
            }
        }

        
    }
}
