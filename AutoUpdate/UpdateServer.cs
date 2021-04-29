using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServerCommon.NetServer;
using System.IO;
using System.Net;
using System.Net.Sockets;
using ServerCommon;

namespace AutoUpdate
{
    
    public class UpdateServer:ServerBase
    {
        public string UpdateDirectory = ".\\Update";
        public override string ServerName => throw new NotImplementedException();

        public UpdateManager Manager => _manager;

        private UpdateManager _manager;
        private TcpListener tcpListener;

        public UpdateServer(IPEndPoint pt,INotifier notifier) : base(notifier)
        {
            _manager = new UpdateManager();
            _manager.UpdateDirectory = UpdateDirectory;

            tcpListener = new TcpListener(pt);       
        }

         
        public override void Start()
        {
            tcpListener.Start();
            base.Start();

        }

        public override void Stop()
        {
            tcpListener.Stop();
            base.Stop();
        }

        private void OnAccept(TcpClient client)
        {
            client.ReceiveTimeout = 10*1000;
            var stream= client.GetStream();


            byte[] buffer=new byte[20];
            client.SendBufferSize = 1024;
            byte[] sendBuffer = new byte[client.SendBufferSize];
            int count;

            stream.Read(buffer, 0, buffer.Length);
            var str= Encoding.UTF8.GetString(buffer, 0, buffer.Length);


            if (str[0] == 'S')
            {
                float version =float.Parse(str.Substring(1));
                _manager.GetDiffFiles(version, out (string, string)[] addfiles, out string[] delfiles, out long size);

                var filesAndSize = Encoding.UTF8.GetBytes(addfiles.Length + "|" + size);
                stream.Write(filesAndSize, 0, filesAndSize.Length);

                foreach (var file in addfiles)
                {
                    using (var fs = File.Open(file.Item1, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        //发送文件名和大小
                        var data = Encoding.UTF8.GetBytes(file.Item2 + "|" + fs.Length);
                        stream.Write(data, 0, data.Length);

                        //读取该文件从多少字节开始发送
                        count= stream.Read(buffer, 0, buffer.Length);
                        int pos = BitConverter.ToInt32(buffer, 0);
                        fs.Seek(pos, SeekOrigin.Begin);

                        //发送文件内容
                        while (!IsStop)
                        {
                            count = fs.Read(sendBuffer, 0, sendBuffer.Length);
                            if (count <= 0)
                                break;
                            stream.Write(sendBuffer, 0, count);
                        }
                    }
                }

                //发送删除文件列表
                var dels =Encoding.UTF8.GetBytes(string.Join("|", delfiles));
                stream.Write(dels, 0, dels.Length);
            }
            client.Close();
        }


        public override void Run()
        {
            while(!IsStop)
            {
                var client=tcpListener.AcceptTcpClient();
                Task.Run(() =>
                {
                    var ce = client;
                    try
                    {
                        OnAccept(ce);
                    }
                    catch(IOException e)
                    {
                        ce.Close();
                        Notify(NotifyType.Warning, "文件更新错误:" + e.Message, this);
                    }
                });
            }
        }
    }
}
