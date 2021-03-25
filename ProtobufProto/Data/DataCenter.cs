using ProtobufProto.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtobufProto.Model;

namespace ProtobufProto.Data
{
    public class DataCenter
    {
        PlayerTransformMap playerTransforms { get; set; } = new PlayerTransformMap();
        //private Dictionary<string, Transform> Transforms { get; set; } = new Dictionary<string, Transform>();

        private static DataCenter instance;
        private static readonly object obj = new object();
        public static DataCenter Instance
        {
            get
            {
                if(instance == null)
                {
                    lock(obj)
                    {
                        if (instance == null)
                            instance = new DataCenter();
                    }
                }
                return instance;
            }
        }

        private DataCenter(){}

        public void UpdateTransform(PlayerTransform transform)
        {
            lock(playerTransforms)
            {
                if (playerTransforms.Transforms.ContainsKey(transform.Account))
                    playerTransforms.Transforms[transform.Account] = transform.Trans;
                else
                    playerTransforms.Transforms.Add(transform.Account, transform.Trans);
            }
        }

        public PlayerTransformMap GetTransformsCopy()
        {
            lock(playerTransforms)
            {
                return playerTransforms.Clone();
            }
        }
    }
}
