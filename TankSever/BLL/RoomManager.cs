using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankSever.BLL
{
    public class RoomManager
    {
        private List<Room> roomList = new List<Room>();

        /// <summary>
        /// 房间数量
        /// </summary>
        public int RoomCount => roomList.Count;

        public int CreateRoom(User user)
        {
            var room = new Room(user);
            lock(roomList)
            {
                roomList.Add(room);
                return roomList.Count-1;
            }
        }

        public bool LeaveRoom(User user)
        {
            return true;
        }

    }
}
