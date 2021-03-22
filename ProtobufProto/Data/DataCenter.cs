using ProtobufProto.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProtobufProto.Data
{
    public static class DataCenter
    {
        private static Dictionary<string, Transform> Transforms { get; set; } = new Dictionary<string, Transform>();


        public static void UpdateTransform(PlayerTransform transform)
        {
            if (Transforms.ContainsKey(transform.Account))
                Transforms[transform.Account] = transform.UserTransform;
            else
                Transforms.Add(transform.Account, transform.UserTransform);
        }
    }
}
