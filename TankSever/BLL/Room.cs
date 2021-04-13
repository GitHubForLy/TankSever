using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankSever.BLL
{
    class Room
    {
        public const int MaxUserCount = 10;
        private Dictionary<string, UserInfo> users = new Dictionary<string, UserInfo>();

        public Room(UserInfo Owner)
        {
            users.Add(Owner.Account,Owner);
        }

        public bool EnterRoom(UserInfo user)
        {
            lock(users)
            {
                if (users.ContainsKey(user.Account))
                    return false;
                if (users.Count >= MaxUserCount)
                    return false;
                if (user.UserState !=UserStates.None)
                    return false;
                users.Add(user.Account, user);
                return true;
            }
        }

        public bool LeaveRoom(UserInfo user)
        {
            lock(users)
            {
                if (!users.ContainsKey(user.Account))
                    return false;
                if (user.UserState != UserStates.Waiting)
                    return false;
                return users.Remove(user.Account);
            }
        }

    }
}
