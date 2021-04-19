using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;

namespace TankSever.BLL
{
    public class Room:RoomInfo
    {
        private int teamUsesrCount;   //每个队伍最大数量
        SortedDictionary<int,User> users = new SortedDictionary<int, User>(); //(位置<=>玩家))
        private User Owner;
        private int teamCount;

        /// <summary>
        /// 房间最大用户量
        /// </summary>
        public override int MaxCount => teamUsesrCount * teamCount;

        /// <summary>
        /// 当前用户数
        /// </summary>
        public override int UserCount
        {
            get
            {
                lock(users)
                {
                    return users.Count;
                }
            }    
        }
        /// <summary>
        /// 是否已经满员
        /// </summary>
        public bool IsFull => UserCount >= MaxCount;

        public Room(int roomid,string Name,int teamCount=2,int TeamMaxCount = 5)
        {
            this.RoomId = roomid;
            this.Name = Name;
            this.teamCount = teamCount;
            this.teamUsesrCount = TeamMaxCount;

            State = RoomState.Waiting;
        }

        public bool EnterRoom(User user,out int team,out int index)
        {
            team = -1;
            index = -1;
            lock (users)
            {
                if (IsFull)
                    return false;

                if (user.RoomDetail.State != RoomUserStates.None)
                    return false;

                if (users.ContainsValue(user))
                    return false;

                FindTeamAndIndex(out team,out  index);
                users.Add(index,user);

                UpdateOwner();

                // 设置用户信息
                user.RoomDetail.Team = team;
                user.RoomDetail.Index = index;
                user.RoomDetail.LastOpeartion = Owner==user?RoomUser.RoomOpeartion.Create:RoomUser.RoomOpeartion.Join;
                user.RoomDetail.State = Owner == user ? RoomUserStates.Ready : RoomUserStates.Waiting;
                user.RoomDetail.IsRoomOwner = Owner == user;
                user.Room = this;

                return true;
            }
        }

        public bool LeaveRoom(User user)
        {
            lock(users)
            {
                if (user.RoomDetail.State != RoomUserStates.Waiting && user!=Owner)
                    return false;

                foreach (var us in users)
                {

                    if(us.Value==user)
                    {
                        users.Remove(us.Key);

                        user.RoomDetail.LastOpeartion = RoomUser.RoomOpeartion.Leave;
                        user.RoomDetail.State = RoomUserStates.None;
                        user.RoomDetail.IsRoomOwner = false;

                        UpdateOwner();
                        return true;
                    }                         
                    
                }
                return false;
            }
        }


        /// <summary>
        /// 加入房间时找到合适team和位置
        /// </summary>
        /// <returns></returns>
        private bool FindTeamAndIndex(out int team,out int index)
        {
            int n = 99999;
            int tmp;
            team = -1;
            index = -1;
            for(int i=0;i<teamCount;i++)
            {
                tmp = users.Count(u => u.Value.RoomDetail.Team == i);
                if (tmp < n)
                {
                    n = tmp;
                    team = i;
                }
            }
            for(int i=team*teamUsesrCount;i< team * teamUsesrCount+teamUsesrCount; i++)
            {             
                if (!users.ContainsKey(i))               
                {
                    index = i;
                    return true;
                }
            }
            return false;
        }

        private void UpdateOwner()
        {
            if(UserCount==1)
            {
                Owner = users.First().Value;
            }
        }

        /// <summary>
        /// 获取房间内所有用户
        /// </summary>
        public User[] GetUsers()
        {
            return users.Values.ToArray();
        }

        /// <summary>
        /// 准备
        /// </summary>
        public bool RoomReady(RoomUser user)
        {
            lock(users)
            {
                if (user.State != RoomUserStates.Waiting)
                    return false;
                user.State = RoomUserStates.Ready;
                user.LastOpeartion = RoomUser.RoomOpeartion.Ready;
                return true;
            }
        }

        /// <summary>
        /// 取消准备
        /// </summary>
        public bool RoomCancelReady(User user)
        {
            lock (users)
            {
                if (user == Owner)
                    return false;
                if (user.RoomDetail.State != RoomUserStates.Ready)
                    return false;
                user.RoomDetail.State = RoomUserStates.Waiting;
                user.RoomDetail.LastOpeartion = RoomUser.RoomOpeartion.CancelReady;
                return true;
            }
        }

        /// <summary>
        /// 是否都准备好了
        /// </summary>
        public bool IsFullReady()
        {
            return !users.Any(m => m.Value.RoomDetail.State != RoomUserStates.Ready);
        }

        /// <summary>
        /// 更改位置
        /// </summary>
        public bool ChangeIndex(User roomUser,int index)
        {
            lock(users)
            {
                if (roomUser.RoomDetail.State != RoomUserStates.Waiting && roomUser != Owner)
                    return false;
                if (users.ContainsKey(index))
                    return false;
                if (index >= MaxCount)
                    return false;

                users.Remove(roomUser.RoomDetail.Index);
                users.Add(index, roomUser);
                roomUser.RoomDetail.Team = (int)(index / teamUsesrCount);
                roomUser.RoomDetail.Index = index;
                roomUser.RoomDetail.LastOpeartion = RoomUser.RoomOpeartion.ChangeIndex;
                return true;
            }
        }
    }
}
