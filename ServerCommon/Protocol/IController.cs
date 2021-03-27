using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommon.Protocol
{
    public interface IController
    {
        AsyncUser User { get; set; }
    }
}
