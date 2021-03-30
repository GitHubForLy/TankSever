using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using ServerCommon.Protocol;

namespace TankSever.BLL.Controllers
{
    class BroadcastController : Controller
    {
        public void UpdateTransform(PlayerTransform transform)
        {
            DataCenter.Instance.UpdateTrnasforms(transform);
        }
    }
}
