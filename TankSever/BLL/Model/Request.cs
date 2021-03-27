﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankSever.BLL.Model
{
    public class Request
    {
        public string Controller { get; set; }
        public string Action { get; set; }
    }


    public class LoginRequest : Request
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class RegisterRequest : Request
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
}
