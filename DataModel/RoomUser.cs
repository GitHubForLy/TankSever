using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    /// <summary>
    /// 房间用户状态
    /// </summary>
    public enum RoomUserStates
    {
        /// <summary>
        /// 在房间外
        /// </summary>
        None,
        /// <summary>
        /// 加入房间但还没有准备
        /// </summary>
        Waiting,
        /// <summary>
        /// 加入房间已准备
        /// </summary>
        Ready,
        /// <summary>
        /// 已开始加入战斗
        /// </summary>
        Fight
    }


    /// <summary>
    /// 房间用户
    /// </summary>
    public class RoomUser
    {
        //public int RoomId;
        public string Account;
        public enum RoomOpeartion
        {
            Create,
            Join,
            Leave,
            Ready,
            CancelReady,
            ChangeIndex
        }

        /// <summary>
        /// 对所属房间的最后操作
        /// </summary>
        public RoomOpeartion LastOpeartion;
        /// <summary>
        /// 状态
        /// </summary>
        public RoomUserStates State;
        /// <summary>
        /// 是否房主
        /// </summary>
        public bool IsRoomOwner;

        public int Team;
        public int Index;

        public override bool Equals(object obj)
        {
            RoomUser user = obj as RoomUser;
            if (user == null)
                return false;
            return user.Account == Account;
        }
        public override int GetHashCode()
        {
            return Account.GetHashCode();
        }

        public static bool operator==(RoomUser user1,RoomUser user2)
        {
            bool b1 = ReferenceEquals(user1, null);
            bool b2 = ReferenceEquals(user2, null);
            if (!b1 && !b2)
                return user1.Account == user2.Account;
            else if (b1 && b2)
                return true;
            else
                return false;
        }
        public static bool operator !=(RoomUser user1, RoomUser user2)
        {
            bool b1 = ReferenceEquals(user1, null);
            bool b2 = ReferenceEquals(user2, null);
            if (!b1 && !b2)
                return user1.Account != user2.Account;
            else if (b1 && b2)
                return false;
            else
                return true;
        }
    }
}
