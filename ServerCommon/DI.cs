using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Resolution;

namespace ServerCommon
{
    public class DI
    {
        private UnityContainer container;
        private static DI instance;
        public static DI Instance
        {
            get
            {
                if (instance == null)
                    instance = new DI();
                return instance;
            }
        }

        private DI()
        {
            container = new UnityContainer();
        }



    }
}
