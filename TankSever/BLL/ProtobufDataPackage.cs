using ProtobufProto.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TankSever.BLL
{
    class ProtobufDataPackage
    {
        public static byte[] PackageData(TcpFlags type, byte[] data)
        {
            var typebits = BitConverter.GetBytes((short)type);
            return typebits.Concat(data).ToArray();
        }

        public static byte[] UnPackageData(byte[] data, out TcpFlags type)
        {
            type = (TcpFlags)BitConverter.ToInt16(data, 0);
            return data.Skip(2).ToArray();
        }

        public static byte[] PackageData(UdpFlags type, byte[] data)
        {
            var typebits = BitConverter.GetBytes((short)type);
            return typebits.Concat(data).ToArray();
        }

        public static byte[] UnPackageData(byte[] data, out UdpFlags type)
        {
            type = (UdpFlags)BitConverter.ToInt16(data, 0);
            return data.Skip(2).ToArray();
        }
    }
}
