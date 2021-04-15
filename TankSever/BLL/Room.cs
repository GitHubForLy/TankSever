using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankSever.BLL
{
    public enum RoomState
    {
        Waiting,
        Fight
    }
    class Room
    {
        private int TeamMaxCount;   //每个队伍最大数量
        private Dictionary<int, Dictionary<int, User>> teams = new Dictionary<int, Dictionary<int, User>>(); //  (队伍 <=> (位置<=>玩家))
        private User Owner;

        /// <summary>
        /// 房间名称
        /// </summary>
        public string Name { get; }
        public RoomState State { get; private set; }
        /// <summary>
        /// 房间最大用户量
        /// </summary>
        public int MaxCount => TeamMaxCount * teams.Count;

        /// <summary>
        /// 当前用户数
        /// </summary>
        public int UserCount
        {
            get
            {
                lock(teams)
                {
                    int count = 0;
                    foreach (var team in teams.Values)
                    {
                        count += team.Count;
                    }
                    return count;
                }
            }    
        }
        /// <summary>
        /// 是否已经满员
        /// </summary>
        public bool IsFull => UserCount >= MaxCount;

        public Room(string Name,int teamCount=2,int TeamMaxCount = 5)
        {
            this.Name = Name;
            State = RoomState.Waiting;
            this.TeamMaxCount = TeamMaxCount;
            for (int i=0;i<teamCount;i++)
            {
                teams.Add(i, new Dictionary<int, User>());
            }
        }

        public bool EnterRoom(User user,out int team,out int index)
        {
            team = -1;
            index = -1;
            lock (teams)
            {
                if (IsFull)
                    return false;

                if (user.UserState != UserStates.None)
                    return false;

                FindTeamAndIndex(out team,out  index);

                teams[team].Add(index, user);
                UpdateOwner();
                return true;
            }
        }

        public bool LeaveRoom(User user)
        {
            lock(teams)
            {
                if (user.UserState != UserStates.Waiting)
                    return false;

                foreach (var team in teams)
                {
                    foreach(var us in team.Value)
                    {
                        if(us.Value==user)
                        {
                            team.Value.Remove(us.Key);
                            UpdateOwner();
                            return true;
                        }                         
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
            team = -1;
            index = -1;
            for(int i=0;i<teams.Count;i++)
            {
                if (teams[i].Count < n)
                {
                    n = teams[i].Count;
                    team = i;
                }
            }
            for(int i=0;i<TeamMaxCount;i++)
            {
                if (!teams[team].ContainsKey(i))               
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
                foreach(var team in teams)
                {
                    if(team.Value.Count>0)
                    {
                        Owner = team.Value.First().Value;
                        return;
                    }    
                }
            }
        }
    }
}
