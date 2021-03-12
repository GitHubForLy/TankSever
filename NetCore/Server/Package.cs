using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetCore.Server
{
    /// <summary>
    /// 数据包管理类  （由于tcp的粘包和分包现象,所以要有此类来管理数据包:将输入的数据进行解析  头四个字节为当前包的长度 然后解析包）
    /// </summary>
    public class DataPackage
    {
        private List<byte> data;
        private int firstPackageLength;//第一个数据包的长度
        private const int packageHeaderLength= sizeof(int);


        public DataPackage()
        {
            data = new List<byte>();
        }

        /// <summary>
        /// 清除数据包
        /// </summary>
        public void Clear()
        {
            firstPackageLength = 0;
            data.Clear();
        }

        /// <summary>
        /// 加入数据段
        /// </summary>
        public void IncommingData(byte[] buffer)
        {
            IncommingData(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// 加入数据段
        /// </summary>
        public void IncommingData(byte[] buffer,int offset,int length)
        {
            if (buffer.Length <= 0)
                return;
          
            data.AddRange(buffer.Skip(offset).Take(length));
            if (firstPackageLength == 0)
            {
                if(data.Count< packageHeaderLength)
                    return;

                firstPackageLength = BitConverter.ToInt32(data.ToArray(), 0);
            }
        }

        /// <summary>
        /// 是否可以输出数据包
        /// </summary>
        public bool CanOutPackage()
        {
            if (firstPackageLength == 0)
                return false;
            if (data.Count < firstPackageLength)
                return false;

            return true;
        }

        /// <summary>
        /// 输出数据包 若不能输出包则返回null
        /// </summary>
        public byte[] OutPutPackage()
        {
            if (!CanOutPackage())
                return null;

            byte[] res = new byte[firstPackageLength - packageHeaderLength];
            data.CopyTo(packageHeaderLength, res, 0, firstPackageLength - packageHeaderLength);
            data.RemoveRange(0, firstPackageLength);
          

            if (data.Count < packageHeaderLength)
                firstPackageLength = 0;
            else
                firstPackageLength= BitConverter.ToInt32(data.ToArray(), 0);

            return res;
        }

        /// <summary>
        /// 输出数据包
        /// </summary>
        public bool OutPutPackage(out byte[] package)
        {
            package = OutPutPackage();
            return package != null;        
        }

    }
}
