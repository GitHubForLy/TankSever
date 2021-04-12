using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using DBCore;
using ServerCommon.Protocol;
using DataModel;

namespace TankSever.BLL.Controllers
{
    class EventController:Controller 
    {
        //注册
        [AllowAnonymous]
        public StandRespone Register(RegisterRequest request)
        {
            //加密密码
            byte[] salt = new byte[20];
            new Random().NextBytes(salt);
            MD5Cng md5 = new MD5Cng();
            var saltpass = Encoding.UTF8.GetBytes(request.Password).Concat(salt).ToArray();
            var crpPass = md5.ComputeHash(saltpass);

            return DBServer.Instance.Regeister(request.UserName, Convert.ToBase64String(crpPass), Convert.ToBase64String(salt));
        }

        //登录
        [AllowAnonymous]
        public StandRespone Login(LoginRequest request)
        {
            var res = DBServer.Instance.GetPassword(request.UserName);
            if (res.IsSuccess)
            {
                var oldPass = res.Data.Rows[0]["password"].ToString();
                var salt = Convert.FromBase64String(res.Data.Rows[0]["salt"].ToString());
                var saltpass = Encoding.UTF8.GetBytes(request.Password).Concat(salt).ToArray();
                MD5Cng md5 = new MD5Cng();
                var crpPass = Convert.ToBase64String(md5.ComputeHash(saltpass));

                if (oldPass == crpPass)
                {
                    User.Login(request.UserName,request.Timestamp);

                    return StandRespone.SuccessResult("登录成功");
                }
                else
                {
                    return StandRespone.FailResult("登录失败,密码错误");
                }
            }
            else
                return res;
        }


        public List<(string,Transform)> GetPlayerTransforms()
        {
            return DataCenter.Instance.GetTransforms();
        }


        public void Logout()
        {
            User.LoginOut();
        }
    }
}
