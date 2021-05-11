using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using DataModel;
using TankSever.BLL.Server;
using ProtobufProto.Model;

namespace TankSever.BLL
{
    public class Room
    {
        private int teamUsesrCount;   //每个队伍最大数量
        SortedDictionary<int,User> users = new SortedDictionary<int, User>(); //(位置<=>玩家))
        private User Owner;
        private int teamCount;
        private DateTime startTime;
        private Dictionary<int, int> teamKillCount=new Dictionary<int, int>();
        private Battle battle;
        public RoomInfo Info { get; }

        ///// <summary>
        ///// 房间最大用户量
        ///// </summary>
        //public override int MaxCount => teamUsesrCount * teamCount;

        ///// <summary>
        ///// 当前用户数
        ///// </summary>
        //public override int UserCount
        //{
        //    get
        //    {
        //        lock(users)
        //        {
        //            return users.Count;
        //        }
        //    }    
        //}
        /// <summary>
        /// 是否已经满员
        /// </summary>
        public bool IsFull => Info.UserCount >=Info.MaxCount;

        /// <summary>
        /// 是否全员准备
        /// </summary>
        public bool IsFullReady => users.All(m => m.Value.RoomDetail.State == RoomUserStates.Ready);

        /// <summary>
        /// 创建房间
        /// </summary>
        /// <param name="roomid"></param>
        /// <param name="setting"></param>
        /// <param name="teamCount">队伍数</param>
        /// <param name="TeamMaxCount">单个队伍最大人数</param>
        public Room(int roomid, RoomSetting setting, int teamCount=2,int TeamMaxCount = 5)
        {
            Info = new RoomInfo()
            {
                RoomId = roomid,
                Setting = setting,
                State=RoomState.RoomWaiting,
                MaxCount=teamCount* TeamMaxCount
            };
            this.teamCount = teamCount;
            this.teamUsesrCount = TeamMaxCount;

            for(int i=0;i<teamCount;i++)
                teamKillCount.Add(i, 0);
        }

        /// <summary>
        /// 校验房间密码
        /// </summary>
        /// <param name="password"></param>
        public bool CheckPassword(string password)
        {
            if (!Info.Setting.HasPassword)
                return true;
            if (string.IsNullOrEmpty(password))
                return false;
            return password == Info.Setting.Password;
        }


        public bool EnterRoom(User user)
        {
            var team = -1;
            var index = -1;
            lock (users)
            {
                if (IsFull)
                    return false;

                if (user.RoomDetail.State != RoomUserStates.None)
                    return false;

                if (users.ContainsValue(user))
                    return false;
                if (Info.State != RoomState.RoomWaiting)
                    return false;

                FindTeamAndIndex(out team,out  index);
                users.Add(index,user);
                Info.UserCount = users.Count;

                if(users.Count==1)
                    UpdateOwner();

                // 设置用户信息
                user.RoomDetail.Team = team;
                user.RoomDetail.Index = index;
                user.RoomDetail.LastOpeartion = Owner==user? RoomUser.RoomOpeartion.Create:RoomUser.RoomOpeartion.Join;
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
                if (user.RoomDetail.State == RoomUserStates.None)
                    return false;

                foreach (var us in users)
                {

                    if(us.Value==user)
                    {
                        users.Remove(us.Key);
                        Info.UserCount = users.Count;

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
            if(Info.UserCount>=1)
            {
                Owner = users.First().Value;
                Owner.RoomDetail.IsRoomOwner = true;
                Owner.RoomDetail.State = RoomUserStates.Ready;
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
                if (Info.State != RoomState.RoomWaiting)
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
                if (Info.State != RoomState.RoomWaiting)
                    return false;
                user.RoomDetail.State = RoomUserStates.Waiting;
                user.RoomDetail.LastOpeartion = RoomUser.RoomOpeartion.CancelReady;
                return true;
            }
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
                if (index >= Info.MaxCount)
                    return false;

                users.Remove(roomUser.RoomDetail.Index);
                users.Add(index, roomUser);
                roomUser.RoomDetail.Team = (int)(index / teamUsesrCount);
                roomUser.RoomDetail.Index = index;
                roomUser.RoomDetail.LastOpeartion = RoomUser.RoomOpeartion.ChangeIndex;
                return true;
            }
        }

        public bool StartFight()
        {
            lock(users)
            {
                if (!IsFullReady)
                    return false;
                Info.State = RoomState.RoomFight;
                startTime = DateTime.Now;

                battle = new Battle(this);
                battle.StartBattle();

                return true;
            }
        }

        public void KillUser(User killer,User diead)
        {
            killer.BattleInfo.KillCount++;
            diead.BattleInfo.DeadCount++;

            lock(teamKillCount)
            {
                teamKillCount[killer.RoomDetail.Team]++;
            }
        }

        /// <summary>
        /// 检查游戏是否结束
        /// </summary>
        /// <param name="team">胜利队伍 平局返回-1</param>
        /// <returns></returns>
        public bool CheckGameFinished(out int team)
        {
            team = -1;
            lock (teamKillCount)
            {
                if (Info.Setting.Mode == FightMode.KillCount)
                {
                    foreach (var kv in teamKillCount)
                    {
                        if (kv.Value >=Info. Setting.TargetKillCount)
                        {
                            team = kv.Key;
                            return true;
                        }
                    }
                    var time = GetRemainingTime();
                    if (time <= 0)
                        return true;
                }
                else if (Info.Setting.Mode == FightMode.Time)
                {
                    var time = GetRemainingTime();
                    if (time <= 0)
                    {
                        int count = 0, n = 0;
                        foreach (var kv in teamKillCount)
                        {
                            if (kv.Value > count)
                            {
                                n = 1;
                                team = kv.Key;
                                count = kv.Value;
                            }
                            else if (kv.Value == count)
                                n++;
                        }
                        if (n > 1)              //平局（多个队伍的击杀数都是最高且一样）
                            team = -1;

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// 结束房间游戏 并初始化房间状态和玩家状态
        /// </summary>
        public void DoFinished()
        {
            Info.State = RoomState.RoomWaiting;

            lock(users)
            {
                foreach (var uer in users)
                    uer.Value.RoomDetail.State = uer.Value == Owner ? RoomUserStates.Ready : RoomUserStates.Waiting;
            }
            lock(teamKillCount)
            {
                for (int i=0;i<teamCount;i++)
                    teamKillCount[i] = 0;
            }
        }

        public int GetRemainingTime()
        {
            if (Info.State != RoomState.RoomFight)
                return -1;
            var remain = (DateTime.Now - startTime).TotalSeconds;
            return Info.Setting.MaxTime-(int)remain;
            //(Program.BroadServer as BroadcastServer).BroadcastRoom((RoomId, BroadcastActions.RemainingTime, remain));
        }
    }
}
