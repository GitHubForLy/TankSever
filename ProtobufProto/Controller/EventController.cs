using DBCore;
using ProtobufProto.Model;
using ServerCommon.Protocol;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ProtobufProto.Controller
{
    public class EventController : StandController
    {
        //注册
        [AllowAnonymous]
        public Respone Register(RegistrRequest request)
        {
            //加密密码
            byte[] salt = new byte[20];
            new Random().NextBytes(salt);
            MD5Cng md5 = new MD5Cng();
            var saltpass = Encoding.UTF8.GetBytes(request.Password).Concat(salt).ToArray();
            var crpPass = md5.ComputeHash(saltpass);

            return StandResult(DBServer.Instance.Regeister(request.Account, Convert.ToBase64String(crpPass), Convert.ToBase64String(salt)));
        }

        //登录
        [AllowAnonymous]
        public Respone Login(LoginRequest request)
        {
            var res = DBServer.Instance.GetPassword(request.Account);
            if (res.IsSuccess)
            {
                var oldPass = res.Data.Rows[0]["password"].ToString();
                var salt = Convert.FromBase64String(res.Data.Rows[0]["salt"].ToString());
                var saltpass = Encoding.UTF8.GetBytes(request.Password).Concat(salt).ToArray();
                MD5Cng md5 = new MD5Cng();
                var crpPass = Convert.ToBase64String(md5.ComputeHash(saltpass));

                if (oldPass == crpPass)
                {
                    Context.Login(request.Account);
                    return StandResult(StandRespone.SuccessResult("登录成功"));
                }
                else
                {
                    return StandResult(StandRespone.FailResult("登录失败,密码错误"));
                }
            }
            else
                return StandResult(res);
        }

        public void LoginOut([Required] string Account)
        {
            Context.LoginOut();
        }

    }
}