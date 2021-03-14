﻿using System;
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
using Unity;
using ServerCommon.NetServer;
using ProtobufProto;
using ServerCommon.Protocol;
using TankSever.BLL;
using Unity.Resolution;

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
            Program.NetServer.Start(new IPEndPoint(IPAddress.Parse(txt_ip.Text),int.Parse(txt_port.Text)));
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
