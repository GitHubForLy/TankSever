using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public enum RoomState
    {
        Waiting,
        Fight
    }

    public class RoomInfo
    {
        public int RoomId;
        public RoomState State;
        public virtual int UserCount { get; set; }
        public virtual int MaxCount { get; set; }

        public virtual RoomSetting Setting { get; set; }

        public override bool Equals(object obj)
        {
            RoomInfo rom = obj as RoomInfo;
            if (rom == null)
                return false;
            return this.RoomId == rom.RoomId;
        }
        public override int GetHashCode()
        {
            return RoomId;
        }

        public static bool operator ==(RoomInfo rom1, RoomInfo rom2)
        {
            bool b1 = ReferenceEquals(rom1, null);
            bool b2 = ReferenceEquals(rom2, null);
            if (!b1 && !b2)
                return rom1.RoomId == rom2.RoomId;
            else if (b1 && b2)
                return true;
            else
                return false;
        }
        public static bool operator !=(RoomInfo rom1, RoomInfo rom2)
        {
            bool b1 = ReferenceEquals(rom1, null);
            bool b2 = ReferenceEquals(rom2, null);
            if (!b1 && !b2)
                return rom1.RoomId != rom2.RoomId;
            else if (b1 && b2)
                return false;
            else
                return true;
        }
    }
}
