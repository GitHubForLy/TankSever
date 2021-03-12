using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ServerCommon;
using DBCore;
using NetCore.Server;
using System.Net;

namespace TankSever
{
    public partial class MainForm : Form,INotifier
    {
        StandTcpServer NetServer;

        public MainForm()
        {
            InitializeComponent();
            NetServer = new StandTcpServer(100, this);
            NetServer.Init();
        }

        private void btn_start_Click(object sender, EventArgs e)
        {
            NetServer.Start(new IPEndPoint(IPAddress.Parse(txt_ip.Text),int.Parse(txt_port.Text)));
            lbx_log.Items.Add("[服务已启动]");
        }


        public void OnNotify(NotifyType type, string logInfo, IServer sender)
        {
            lbx_log.Invoke(new Action(() =>
            {
                lbx_log.Items.Add("[" + sender.ServerName + "]:" + logInfo);
            }));        
        }
    }
}
