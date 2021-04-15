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

    public class RoomChange
    { 
        public enum RoomOpeartion
        {
            Create,
            Join,
            Leave
        }
        public int RoomId;
        public RoomOpeartion Opeartion;
        public string Account;
        public int Team;
        public int Index;
    }

}
