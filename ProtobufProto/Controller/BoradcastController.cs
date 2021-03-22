using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtobufProto.Data;
using ProtobufProto.Model;

namespace ProtobufProto.Controller
{
    public class BoradcastController:StandController
    {
        public void Transform(PlayerTransform transforms)
        {
            DataCenter.UpdateTransform(transforms);
        }


    }
}
