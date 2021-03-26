using Microsoft.Practices.Unity.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using Unity.Resolution;

namespace ServerCommon
{
    public class DI
    {
        private static DI instance;
        private static readonly object lockobj = new object();
        private UnityContainer IocContainer;

        public static DI Instance
        {
            get
            {
                if(instance == null)
                {
                    lock (lockobj)
                    {
                        if (instance == null)
                            instance = new DI();
                    }
                }
                return instance;
            }
        }

        private DI()
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            UnityConfigurationSection section = (UnityConfigurationSection)configuration.GetSection(UnityConfigurationSection.SectionName);
            IocContainer = new UnityContainer();
            section.Configure(IocContainer, "Server");
        }


        public T Resolve<T>()
        {
            return IocContainer.Resolve<T>();
        }

        public T Resolve<T>(string name,params (string name,object value)[] paramterOverrides)
        {
            ParameterOverride[] overrides = new ParameterOverride[paramterOverrides.Length];
            for (int i= 0;i<overrides.Length;i++)
            {
                overrides[i] = new ParameterOverride(paramterOverrides[i].name, paramterOverrides[i].value);
            }
            return IocContainer.Resolve<T>(name, overrides);
        }

        public void Regiseter<TFrom,TTo>()
        {
            IocContainer.RegisterType(typeof(TFrom),typeof(TTo));
        }
    }
}
