using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCommon.Protocol;

namespace TankSever.BLL.Controller
{
    public class AccountController:IController
    {
        public string ControllerName => "Account";

        public void Register(string Account,string Password)
        {

        }
    }
}
