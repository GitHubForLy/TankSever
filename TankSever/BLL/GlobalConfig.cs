using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankSever.BLL
{
   public class GlobalConfig
    {
        public static int TcpPort { get; set; }
        public static int UdpPort { get; set; }

        /// <summary>
        /// 帧同步延时毫秒
        /// </summary>
        public static int FrameDuration { get; set; } = 100;
    }
}
