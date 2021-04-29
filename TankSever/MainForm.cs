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
using System.Net;
using System.Threading;
using TankSever.BLL;

namespace TankSever
{
    public partial class MainForm : Form,INotifier
    {
        private int PrevTotaiReiveBytes;
        private int PrevTotaiSendBytes;
        private SynchronizationContext synchronizationContext;

        public MainForm()
        {
            synchronizationContext = SynchronizationContext.Current;
            InitializeComponent();          
        }

        private void btn_start_Click(object sender, EventArgs e)
        {          
            Program.NetServer.Start(new IPEndPoint(IPAddress.Any,int.Parse(txt_port.Text)));
            Program.BroadServer.Start();
            Program.RoomServer.Start();
            Program.UpdateServer.Start();

            lbx_log.Items.Add("[服务已启动]");
            btn_start.Enabled = false;
            btn_stop.Enabled = true;
            netTimer.Enabled = true;
        }


        public void OnNotify(NotifyType type, string logInfo, IServer sender)
        {
            synchronizationContext.Post((state) =>
            {
                switch (type)
                {
                    case NotifyType.Error:
                        lbx_log.Items.Add(new ListBoxItem { Text = "[" + sender.ServerName + "]异常:" + logInfo, TextColor = Color.Red });
                        break;
                    case NotifyType.Message:
                        lbx_log.Items.Add(new ListBoxItem { Text = "[" + sender.ServerName + "]:" + logInfo, TextColor = Color.Black });
                        break;
                    case NotifyType.Warning:
                        lbx_log.Items.Add(new ListBoxItem { Text = "[" + sender.ServerName + "]警告:" + logInfo, TextColor = Color.Yellow });
                        break;
                }
            },null);          
        }

        private void btn_stop_Click(object sender, EventArgs e)
        {
            Program.NetServer.Close();
            Program.BroadServer.Stop();
            Program.RoomServer.Stop();
            Program.UpdateServer.Stop();
            lbx_log.Items.Add("[服务已停止]");
            btn_start.Enabled = true;
            btn_stop.Enabled = false;
            netTimer.Enabled = false;
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(!Program.NetServer.IsClosed)
            {
                if(DialogResult.Yes==MessageBox.Show("服务正在运行,是否关闭服务并退出？","询问",MessageBoxButtons.YesNo))
                {
                    Program.NetServer.Close();
                    e.Cancel = false;
                }
                else
                {
                    e.Cancel = true;
                }

            }
        }

        private void netTimer_Tick(object sender, EventArgs e)
        {

            float krbytes = (float)Math.Round((Program.NetServer.TotalReceiveBytes - PrevTotaiReiveBytes) / 1024f, 2);
            float ksbytes = (float)Math.Round((Program.NetServer.TotalSendBytes - PrevTotaiSendBytes) / 1024f,2);
            tssl_request.Text = $"{krbytes}kb/s";
            tssl_resbytes.Text = $"{ksbytes}kb/s";
            tssl_conncount.Text = Program.NetServer.ConnectedCount.ToString();
            tssl_usercount.Text = DataCenter.Users.UserCount.ToString();

            PrevTotaiReiveBytes = Program.NetServer.TotalReceiveBytes;
            PrevTotaiSendBytes = Program.NetServer.TotalSendBytes;
        }
    }
}
