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

            UserCenter.Instance.
        }

    }
}
