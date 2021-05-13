using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBCore;
using ServerCommon;
using ServerCommon.DB;
using ServerCommon.NetServer;
using TankSever.BLL.Server;
using TankSever.BLL;
using AutoUpdate;

namespace TankSever
{

    static class Program
    {
        /// <summary>
        /// 主Tcp监听服务
        /// </summary>
        public static AsyncSocketServerBase NetServer { get;private set; }
        public static ServerBase BroadServer { get; private set; }
        /// <summary>
        /// 主窗口
        /// </summary>
        public static Form MainForm { get; private set; }
        /// <summary>
        /// 房间工作服务
        /// </summary>
        //public static ServerBase RoomServer { get; private set; }

        //udp服务
        public static UdpServer MainUdp { get; private set; }

        public static ServerBase UpdateServer { get; private set; }


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
            #region 注册类型（已改为在配置文件里注册类型）
            //IocContainer.RegisterType<IProtocolHandler, ProtobufHandler>();
            //IocContainer.RegisterType<IActionExecuter, ActionExecuter>();
            //IocContainer.RegisterType<AsyncSocketServerBase, StandTcpServer>("MainTcpServer");
            //IocContainer.RegisterType<IDBExecuterFactory, MySqlDBExecuterFactory>();
            #endregion

            DBServer.Instance.RegisterFactory(DI.Instance.Resolve<IDBExecuterFactory>());

            NetServer = DI.Instance.Resolve<AsyncSocketServerBase>("MainTcpServer", ("MaxConnections", 100), ("notifier", MainForm));
            NetServer.Init();
            NetServer.SocketTimeOutMS = 10 * 1000;

            BroadServer = new BroadcastServer(MainForm as MainForm);
            DataCenter.Init();
            MainUdp = new UdpServer(MainForm as MainForm);

            //RoomServer = new RoomWorker(MainForm as MainForm);
            UpdateServer = new UpdateServer(new System.Net.IPEndPoint(System.Net.IPAddress.Any,4788),MainForm as MainForm);
        }
    }
}
