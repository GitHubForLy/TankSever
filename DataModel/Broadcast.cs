using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class Vector3
    {
        public float X;
        public float Y;
        public float Z;
    }


    public class Transform
    {
        public Vector3 Position;
        public Vector3 Rotation;
    }

    public class LoginInfo
    {
        public string Account;
        public int WaypointIndex;
    }

    public class SyncMethod
    {
        public string ClassFullName;
        public string MethodName;
        public object[] Parameters;
    }

    public class RoomUser
    {
        public int RoomId;
        public enum RoomOpeartion
        {
            Create,
            Join,
            Leave
        }
        /// <summary>
        /// 对所属房间的最后操作
        /// </summary>
        public RoomOpeartion LastOpeartion;
        public string Account;
        public int Team;
        public int Index;
    }

}
