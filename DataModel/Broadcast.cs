using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    public class Vector3
    {
        public float X { get; set; }
        public float Y { get; set; }
        public float Z { get; set; }
    }


    public class Transform
    {
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
    }

    public class LoginInfo
    {
        public string Account { get; set; }
        public int WaypointIndex { get; set; }
    }

    public class SyncMethod
    {
        public string ClassFullName { get; set; }
        public string MethodName { get; set; }
        public object[] Parameters { get; set; }
    }

}
