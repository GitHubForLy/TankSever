using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TankSever.BLL.Server;
using DataModel;

namespace TankSever.BLL
{
    public class RoomManager
    {
        private SortedDictionary<int,Room> roomList = new SortedDictionary<int,Room>();

        /// <summary>
        /// 房间数量
        /// </summary>
        public int RoomCount => roomList.Count;

        public int CreateRoom(string Name,User user, out int team,out int index)
        {
            lock(roomList)
            {
                int id = FindMinEnableRoomId();
                var room = new Room(id,Name);
                roomList.Add(id,room);
                room.EnterRoom(user, out team,out index);
                return id;
            }
        }

        public bool LeaveRoom(User user)
        {
            lock (roomList)
            {
                if (user.RoomDetail.State == RoomUserStates.None)
                    return false;

                var id = user.Room.RoomId;
                if (roomList.ContainsKey(id))
                {
                    if ((user.Room as Room).LeaveRoom(user))
                    {
                        if (user.Room.UserCount <= 0)
                              roomList.Remove(id);
                        return true;
                    }
                    else
                        return false;
                }
            }     
            return false;
        }

        public bool JoinRoom(int RoomId, User user,out int team,out int index)
        {
            lock(roomList)
            {
                if(roomList.ContainsKey(RoomId))
                {
                    if (roomList[RoomId].EnterRoom(user, out team, out index))
                    {
                        return true;
                    }
                    return false;
                }
                index= team = -1;
                return false;
            }
        }


        /// <summary>
        /// 寻找一个最小的空闲的房间号
        /// </summary>
        /// <returns></returns>
        private int FindMinEnableRoomId()
        {
            int id = 0;
            foreach (var roid in roomList.Keys)
            {
                if (roid > id)
                    break;
                else
                    id = roid + 1;
            }
            return id;
        }


        public RoomInfo[] GetRoomList()
        {
            lock (roomList)
            {
                return roomList.Values.ToArray();
            }
        }

        public Room this [int roomid]
        {
            get
            {
                lock (roomList)
                {
                    if (!roomList.ContainsKey(roomid))
                        return null;
                    return roomList[roomid];
                }
            }
        } 

    }
}
