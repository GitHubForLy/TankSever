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
        public Vector3 Position=new Vector3();
        public Vector3 Rotation=new Vector3();
    }

    public class LoginInfo
    {
        public string Account;
        public int WaypointIndex;
    }



}
