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
        public override string ServerName => "UpdateServer";

        public UpdateManager Manager => _manager;

        private UpdateManager _manager;
        private TcpListener tcpListener;
        private DataPackage package;

        public UpdateServer(IPEndPoint pt,INotifier notifier) : base(notifier)
        {
            package = new DataPackage();
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

        private byte[] ReadBuffer(NetworkStream stream)
        {
            byte[] buffer = new byte[1024];

            while(!package.CanOutPackage())
            {
                var count=stream.Read(buffer, 0, buffer.Length);
                package.IncommingData(buffer, 0, count);
            }
            return package.OutgoingPackage();
        }

        private void WriteBuffer(NetworkStream stream,byte[] data)
        {
            WriteBuffer(stream, data, 0, data.Length);
        }
        private void WriteBuffer(NetworkStream stream, byte[] data,int index,int length)
        {
            data = DataPackage.PackData(data.Skip(index).Take(length).ToArray());
            stream.Write(data, 0, data.Length);
        }

        private void OnAccept(TcpClient client)
        {
            client.ReceiveTimeout = 10*1000;
            var stream= client.GetStream();


            byte[] buffer;
            client.SendBufferSize = 1024*10;
            byte[] sendBuffer = new byte[client.SendBufferSize];
            int count;

            buffer= ReadBuffer(stream);
            var str= Encoding.UTF8.GetString(buffer, 0, buffer.Length);


            if (str[0] == 'S')
            {
                float version =float.Parse(str.Substring(1));
                float hiversion= _manager.GetHighVersion();
                if (version >= hiversion)
                {
                    client.Close();
                    return; 
                }

                _manager.GetDiffFiles(version, out (string, string)[] addfiles, out string[] delfiles, out long size);


                var filesAndSize = Encoding.UTF8.GetBytes(addfiles.Length + "|" + size+"|"+ hiversion);
                WriteBuffer(stream, filesAndSize);

                foreach (var file in addfiles)
                {
                    using (var fs = File.Open(file.Item1, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        //发送文件名和大小
                        var data = Encoding.UTF8.GetBytes(file.Item2 + "|" + fs.Length);
                        WriteBuffer(stream,data);

                        System.Diagnostics.Debug.WriteLine("file:" + file.Item1);

                        //读取该文件从多少字节开始发送
                        buffer =ReadBuffer(stream);
                        long pos = BitConverter.ToInt64(buffer, 0);
                        fs.Seek(pos, SeekOrigin.Begin);
                        System.Diagnostics.Debug.WriteLine("seek:" + pos);

                        //发送文件内容
                        while (!IsStop)
                        {
                            count = fs.Read(sendBuffer, 0, sendBuffer.Length);
                            if (count <= 0)
                                break;

                            WriteBuffer(stream, sendBuffer, 0, count);
                        }
                    }
                }

                //发送删除文件列表
                var dels =Encoding.UTF8.GetBytes(string.Join("|", delfiles));
                WriteBuffer(stream, dels);
                System.Diagnostics.Debug.WriteLine("完成");
            }
            client.Close();
        }


        public override void Run()
        {
            while(!IsStop)
            {
                TcpClient client=null;
                try
                {
                    client = tcpListener.AcceptTcpClient();
                }
                catch (SocketException) { continue; };
                
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
