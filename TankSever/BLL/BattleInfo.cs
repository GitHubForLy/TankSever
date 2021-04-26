using DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankSever.BLL
{
    public class BattleInfo
    {
        public float Health;
        public Transform Trans;
        public double transTime;
        public Vector3 velocity;
        public Vector3 Taregtion;

        public int KillCount;
        public int DeadCount;
    }
}
