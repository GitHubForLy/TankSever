using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public enum BroadcastFlag
    {
        Global,
        Room,
        Team,
        User
    }
    public class BroadcastMethod
    {
        public BroadcastFlag Flag = BroadcastFlag.Global;
        public string ClassFullName;
        public string MethodName;
        public object[] Parameters;
    }
}
