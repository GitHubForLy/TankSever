﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using ServerCommon;

namespace TankSever.BLL
{
    public class User:AsyncUser
    {
        /// <summary>
        /// 战场信息
        /// </summary>
        public BattleInfos BattleInfo;

        /// <summary>
        /// 用户全局当前状态
        /// </summary>
        public UserStates UserState;


        public override void Login(string UserName)
        {
            base.Login(UserName);
            DataCenter.Instance.AddUser(this);
        }

        public override void LoginOut()
        {
            base.LoginOut();
            DataCenter.Instance.RemoveUser(UserName);
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

    /// <summary>
    /// 对战(开始战斗后)信息
    /// </summary>
    public class BattleInfos
    {
        public float Health;
        public Transform Trans;
    }
}
