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

    public class PlayerTransform
    {
        public string Accout { get; set; }
        public Transform Trans { get; set; }
    }
}
