using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankSever.BLL
{
    public  enum BroadcastFlag
    {
        Global,
        Room,
        Team,
        User
    }
    public class BroadcastData
    {
        public BroadcastFlag Flag= BroadcastFlag.Global;
        public string Action;
        public object Data;
    }
}
