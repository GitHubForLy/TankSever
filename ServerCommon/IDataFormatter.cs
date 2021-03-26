using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerCommon
{
    interface IDataFormatter
    {
        byte[] Serialize(object ojb);
        object Deserialize(byte[] data);
    }
}
