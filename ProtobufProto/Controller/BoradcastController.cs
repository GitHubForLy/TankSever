using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtobufProto.Data;
using ProtobufProto.Model;

namespace ProtobufProto.Controller
{
    public class BroadcastController:StandController
    {


        //private static Dictionary<string, Transform> Transforms { get; set; } = new Dictionary<string, Transform>();

        //private static void UpdateTransform(PlayerTransform transform)
        //{
        //    if (Transforms.ContainsKey(transform.Account))
        //        Transforms[transform.Account] = transform.UserTransform;
        //    else
        //        Transforms.Add(transform.Account, transform.UserTransform);
        //}


        public void UpdateTransform(PlayerTransform transform)
        {
            DataCenter.Instance.UpdateTransform(transform);

            Broadcast(DataCenter.Instance.GetTransformsCopy());
        }


    }
}
