using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    /// <summary>
    /// 对战模式
    /// </summary>
    public enum FightMode
    {
        KillCount,
        Time
    }

    /// <summary>
    /// 房间设置
    /// </summary>
    public class RoomSetting
    {
        public string RoomName;
        public FightMode Mode;
        public int MaxTime;
        public int TargetKillCount;

        public bool HasPassword;
        public string Password;
    }
}
