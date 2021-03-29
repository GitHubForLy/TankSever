using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class Request
    {
        public int RequestId { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public object Data { get; set; }
    }


    public class LoginRequest 
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class RegisterRequest 
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class  PlayerTransformRequest
    {
        public PlayerTransform Trans { get; set; }
    }
}
