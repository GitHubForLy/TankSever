using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using DataModel;
using DBCore;
using ServerCommon;
using ProtobufProto.Model;

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
        public Room Room { get; set; }

        public static event Action<User> OnUserLogout;

        /// <summary>
        /// 登录时间戳
        /// </summary>
        public string LoginTimestamp { get; private set; }

        public override void Login(string account)
        {
            base.Login(account);
            BattleInfo = new BattleInfo();
            RoomDetail.Info = DBServer.Instance.GetUserInfo(account).Data.Unpack<UserInfo>();
            RoomDetail.Account = account;
     
            if(DataCenter.Users.HasUser(account))
                OnUserLogout?.Invoke(DataCenter.Users[account]);

            DataCenter.Users.AddUser(this);
            LoginTimestamp = GetTimestamp();
        }

        public override void LoginOut()
        {
            OnUserLogout?.Invoke(this);
            DataCenter.Users.RemoveUser(UserAccount);
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
