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
using Unity;
using Unity.Resolution;
using static System.Net.Dns;

namespace TankSever
{
    public partial class MainForm : Form,INotifier
    {
        public MainForm()
        {
            InitializeComponent();          
        }

        private void btn_start_Click(object sender, EventArgs e)
        {          
            Program.NetServer.Start(new IPEndPoint(IPAddress.Any,int.Parse(txt_port.Text)));
            lbx_log.Items.Add("[服务已启动]");
            btn_start.Enabled = false;
            btn_stop.Enabled = true;
        }


        public void OnNotify(NotifyType type, string logInfo, IServer sender)
        {
            lbx_log.Invoke(new Action(() =>
            {
                lbx_log.Items.Add("[" + sender.ServerName + "]:" + logInfo);
            }));        
        }

        private void txt_ip_TextChanged(object sender, EventArgs e)
        {

        }

        private void btn_stop_Click(object sender, EventArgs e)
        {
            Program.NetServer.Close();
            lbx_log.Items.Add("[服务已停止]");
            btn_start.Enabled = true;
            btn_stop.Enabled = false;
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
    }
}
