using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using ServerCommon;

namespace TankSever.BLL
{
    public class User : AsyncUser
    {
        /// <summary>
        /// 战场信息
        /// </summary>
        public BattleInfo BattleInfo { get; private set; }

        /// <summary>
        /// 用户全局当前状态
        /// </summary>
        public UserStates UserState = UserStates.None;

        /// <summary>
        /// 登录时间戳
        /// </summary>
        public string LoginTimestamp { get; private set; }

        public override void Login(string UserName)
        {
            base.Login(UserName);

            BattleInfo = new BattleInfo();
            DataCenter.Users.AddUser(this);
            LoginTimestamp = GetTimestamp();
        }

        public override void LoginOut()
        {
            DataCenter.Users.RemoveUser(UserName);
            BattleInfo = null;

            base.LoginOut();
        }

        private string GetTimestamp()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

    }

    public enum UserStates
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

}
