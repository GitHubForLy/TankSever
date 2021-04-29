using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace AutoUpdate
{
    public class UpdateManager
    {
        public string UpdateDirectory;

        /// <summary>
        /// 获取当前最高版本
        /// </summary>
        /// <returns></returns>
        public float GetHighVersion()
        {
            if (!Directory.Exists(UpdateDirectory))
                return 0;

            var dires = Directory.GetDirectories(UpdateDirectory);
            float ver = 0;
            foreach (var dire in dires)
            {
                var name = Path.GetFileName(dire);
                if (float.TryParse(name, out float v))
                {
                    if (v > ver)
                        ver = v;
                }
            }
            return ver;
        }

        /// <summary>
        /// 获取指定版本的差异文件
        /// </summary>
        /// <param name="addFiles">旧版本没有的或要更新的文件 为相对于更新目录的路径</param>
        /// <param name="delFiles">旧版本有 新版本没有的文件 为相对于版本目录的路径</param>
        public void GetDiffFiles(float version,out (string upFile,string reFile)[] addFiles, out string[] delFiles,out long addSize)
        {
            addFiles =  new (string,string)[0];
            delFiles = new string[0];
            addSize = 0;

            var hiversion= GetHighVersion();
            if (version >= hiversion)
                return;

            var hiFiles = GetVersionFiles(hiversion);
            var qFiles = GetVersionFiles(version);

            List<(string,string)> addfiles = new List<(string,string)>();

            bool has;
            foreach(var file in hiFiles)
            {
                has = false;
                foreach (var cfile in qFiles)
                {
                    if(cfile.rfile==file.rfile)
                    {
                        has = true;

                        //创建一个哈希算法对象 
                        using (HashAlgorithm hash = HashAlgorithm.Create())
                        {
                            using (FileStream file1 = new FileStream(file.pfile, FileMode.Open, FileAccess.Read, FileShare.Read), file2 = new FileStream(cfile.pfile, FileMode.Open, FileAccess.Read, FileShare.Read))
                            {
                                byte[] hashByte1 = hash.ComputeHash(file1);//哈希算法根据文本得到哈希码的字节数组 
                                byte[] hashByte2 = hash.ComputeHash(file2);
                                string str1 = BitConverter.ToString(hashByte1);//将字节数组装换为字符串 
                                string str2 = BitConverter.ToString(hashByte2);

                                if (str1 != str2);//比较哈希码 
                                {
                                    addfiles.Add((file.pfile,file.rfile));
                                    addSize += file1.Length;
                                }
                            }
                        }
                        break;
                    }
                }
                if(!has)
                {
                    addfiles.Add((file.pfile,file.rfile));
                    using(var fs= File.Open(file.pfile, FileMode.Open, FileAccess.Read, FileShare.Read))
                        addSize+= fs.Length;
                }
            }

            delFiles = qFiles.Where(m => !hiFiles.Any(x => x.rfile == m.rfile)).Select(m => m.rfile).ToArray();
            addFiles = addfiles.ToArray();
        }


        public bool isValidFileContent(string filePath1, string filePath2)
        {
            //创建一个哈希算法对象 
            using (HashAlgorithm hash = HashAlgorithm.Create())
            {
                using (FileStream file1 = new FileStream(filePath1, FileMode.Open,FileAccess.Read,FileShare.Read), file2 = new FileStream(filePath2, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    byte[] hashByte1 = hash.ComputeHash(file1);//哈希算法根据文本得到哈希码的字节数组 
                    byte[] hashByte2 = hash.ComputeHash(file2);
                    string str1 = BitConverter.ToString(hashByte1);//将字节数组装换为字符串 
                    string str2 = BitConverter.ToString(hashByte2);
                    return (str1 == str2);//比较哈希码 
                }
            }
        }

        /// <summary>
        /// 返回该版本所有的文件
        /// </summary>
        /// <param name="version"></param>
        /// <returns>pfile为相对更新目录下的路径，rfile是相对版本目录下的路径</returns>
        public (string pfile,string rfile)[] GetVersionFiles(float version)
        {
            var dir = UpdateDirectory + "\\" + version;
            if (Directory.Exists(dir))
            {
                var files = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);          
                return files.Select(m=>(m,m.Replace(dir, "."))).ToArray();
            }
            return new (string,string)[0];
        }
    }
}
