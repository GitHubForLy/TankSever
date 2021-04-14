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
        private Dictionary<string, User> users = new Dictionary<string, User>();

        public Room(User Owner)
        {
            users.Add(Owner.UserName,Owner);
        }

        public bool EnterRoom(User user)
        {
            lock(users)
            {
                if (users.ContainsKey(user.UserName))
                    return false;
                if (users.Count >= MaxUserCount)
                    return false;
                if (user.UserState !=UserStates.None)
                    return false;
                users.Add(user.UserName, user);
                return true;
            }
        }

        public bool LeaveRoom(User user)
        {
            lock(users)
            {
                if (!users.ContainsKey(user.UserName))
                    return false;
                if (user.UserState != UserStates.Waiting)
                    return false;
                return users.Remove(user.UserName);
            }
        }

    }
}
