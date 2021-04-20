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
        /// 所在的房间信息
        /// </summary>
        public RoomUser RoomDetail { get; set; } = new RoomUser();

        /// <summary>
        /// 所在的房间
        /// </summary>
        public RoomInfo Room { get; set; }

        public static event Action<User> OnUserLogout;

        /// <summary>
        /// 登录时间戳
        /// </summary>
        public string LoginTimestamp { get; private set; }

        public override void Login(string UserName)
        {
            base.Login(UserName);
            BattleInfo = new BattleInfo();
            RoomDetail.Account = UserName;
     
            if(DataCenter.Users.HasUser(UserName))
                OnUserLogout?.Invoke(DataCenter.Users[UserName]);

            DataCenter.Users.AddUser(this);
            LoginTimestamp = GetTimestamp();
        }

        public override void LoginOut()
        {
            OnUserLogout?.Invoke(this);
            DataCenter.Users.RemoveUser(UserName);
            BattleInfo = null;

            base.LoginOut();
        }

        private string GetTimestamp()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            var j = ts.TotalSeconds;
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

    }
   
}
