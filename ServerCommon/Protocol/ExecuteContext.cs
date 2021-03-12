using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommon.Protocol
{
    public class ExecuteContext
    {
        string ControllerName { get; set; }
        string ActionName { get; set; }
        Dictionary<string, object> Paramters { get; set; }
    }
}
