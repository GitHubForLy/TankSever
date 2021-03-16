using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using ServerCommon.Protocol;
using System.Security.Cryptography;
using DBCore;

namespace TankSever.BLL.Controller
{
    public class AccountController:ControllerBase
    {
        public StandRespone Register([Required]string Account,[Required]string Password)
        {
            if (!ModelState.IsValid)
                return FailMessage(ModelState.ErrorMessage);

            //加密密码
            byte[] salt = new byte[20];
            new Random().NextBytes(salt);
            MD5Cng md5 = new MD5Cng();
            var saltpass = Encoding.UTF8.GetBytes(Password).Concat(salt).ToArray();
            var crpPass = md5.ComputeHash(saltpass);      

            return Program.DbServer.Regeister(Account, Convert.ToBase64String(crpPass), Convert.ToBase64String(salt));
        }

        public StandRespone Login([Required] string Account, [Required] string Password)
        {
            if (!ModelState.IsValid)
                return FailMessage(ModelState.ErrorMessage);

            var res= Program.DbServer.GetPassword(Account);
            if(res.IsSuccess)
            {
                var oldPass = res.Data.Rows[0]["password"].ToString();
                var salt = Convert.FromBase64String(res.Data.Rows[0]["salt"].ToString());
                var saltpass = Encoding.UTF8.GetBytes(Password).Concat(salt).ToArray();
                MD5Cng md5 = new MD5Cng();
                var crpPass =Convert.ToBase64String(md5.ComputeHash(saltpass));
             
                if(oldPass==crpPass)
                {
                    UserConnect.Login(Account);
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

        public void LoginOut([Required] string Account)
        {
            UserConnect.LoginOut();
        }


    }
}
