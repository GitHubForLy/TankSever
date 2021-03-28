using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;

namespace TankSever.BLL
{
    public class DataCenter
    {
        private static DataCenter instance;
        private static readonly object lockobj = new object();
        private Dictionary<string,Transform> playerTransforms = new Dictionary<string, Transform>();

        /// <summary>
        /// 唯一实例
        /// </summary>
        public static DataCenter Instance
        {
            get
            {
                if(instance==null)
                {
                    lock(lockobj)
                    {
                        if (instance == null)
                            instance = new DataCenter();
                    }
                }
                return instance;
            }
        }



        public void UpdateTrnasforms(PlayerTransform transform)
        {
            lock(playerTransforms)
            {
                if (playerTransforms.ContainsKey(transform.Accout))
                    playerTransforms[transform.Accout] = transform.Trans;
                else
                    playerTransforms.Add(transform.Accout, transform.Trans);
            }
        }

        public List<PlayerTransform> GetTransforms()
        {
            List<PlayerTransform> res = new List<PlayerTransform>();
            lock(playerTransforms)
            {
                foreach (var tran in playerTransforms)
                {
                    res.Add(new PlayerTransform
                    {
                        Accout = tran.Key,
                        Trans = tran.Value
                    });
                }
            }
            return res;
        }

    }
}
