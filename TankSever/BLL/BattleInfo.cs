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
        public Transform Trans=new Transform();
        public double transTime;
        public Vector3 velocity=new Vector3();
        public Vector3 Taregtion=new Vector3();

        public int KillCount;
        public int DeadCount;
    }
}
