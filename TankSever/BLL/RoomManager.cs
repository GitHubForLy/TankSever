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
            var room = new Room(Name);
            lock(roomList)
            {
                int id = FindMinEnableRoomId();
                roomList.Add(id,room);
                room.EnterRoom(user, out team,out index);
                return id;
            }
        }

        public bool LeaveRoom(int RoomId,User user)
        {
            lock (roomList)
            {
                if (roomList.ContainsKey(RoomId))
                {
                    roomList[RoomId].LeaveRoom(user);
                    if(roomList.Count<=0)

                    return roomList.Remove(RoomId);
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
                    return roomList[RoomId].EnterRoom(user,out team,out index);
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


        public List<RoomInfo> GetRoomList()
        {
            var res = new List<RoomInfo>();
            lock(roomList)
            {
                foreach (var rom in roomList)
                {
                    res.Add(new RoomInfo()
                    {
                        State = rom.Value.State == RoomState.Waiting ? 0 : 1,
                        RoomId = rom.Key,
                        Name = rom.Value.Name,
                        Count = rom.Value.UserCount,
                        MaxCount = rom.Value.MaxCount,
                    });
                }
            }
            return res;
        }




    }
}
