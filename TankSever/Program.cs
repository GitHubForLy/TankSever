using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBCore;
using DBCore.MySql;
using Microsoft.Practices.Unity.Configuration;
using ServerCommon.DB;
using ServerCommon.NetServer;
using Unity;
using Unity.Resolution;

namespace TankSever
{

    static class Program
    {
        /// <summary>
        /// 全局IOC容器
        /// </summary>
        public static UnityContainer IocContainer { get;private set; }
        /// <summary>
        /// 主Tcp监听服务
        /// </summary>
        public static AsyncSocketServerBase NetServer { get;private set; }
        /// <summary>
        /// 主窗口
        /// </summary>
        public static Form MainForm { get; private set; }


        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            MainForm = new MainForm();
            Init();

            Application.Run(MainForm);
        }


        private static void Init()
        {
            Configuration configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            UnityConfigurationSection section = (UnityConfigurationSection)configuration.GetSection(UnityConfigurationSection.SectionName);
            IocContainer = new UnityContainer();
            section.Configure(IocContainer, "Server");

            #region 注册类型（已改为在配置文件里注册类型）
            //IocContainer.RegisterType<IProtocolHandler, ProtobufHandler>();
            //IocContainer.RegisterType<IActionExecuter, ActionExecuter>();
            //IocContainer.RegisterType<AsyncSocketServerBase, StandTcpServer>("MainTcpServer");
            //IocContainer.RegisterType<IDBExecuterFactory, MySqlDBExecuterFactory>();
            #endregion

            DBServer.Instance.RegisterFactory(IocContainer.Resolve<IDBExecuterFactory>());

            ParameterOverride maxConnections = new ParameterOverride("MaxConnections", 100);
            ParameterOverride notifier = new ParameterOverride("notifier", MainForm);
            NetServer = IocContainer.Resolve<AsyncSocketServerBase>("MainTcpServer", maxConnections, notifier);
            NetServer.Init();
            NetServer.SocketTimeOutMS = 10 * 1000;
        }
    }
}
