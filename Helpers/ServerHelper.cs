using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileListening
{
    public static class ServerHelper
    {
        /// <summary>
        /// 上传文件到指定的服务器共享文件夹中
        /// </summary>
        /// <param name="destinationFile">目标文件</param>
        /// <param name="folder">服务器文件夹</param>
        /// <param name="fileName">服务器文件名称（包含扩展名）</param>
        public static bool UploadFile(string destinationFile, string folder, string fileName)
        {
            string server = OrBitHelper.ReadIniData("ServerConfig", "server", "");
            string userName = OrBitHelper.ReadIniData("ServerConfig", "userName", "");
            string passWord = OrBitHelper.ReadIniData("ServerConfig", "passWord", "");
            // 将服务器地址和文件夹拼接
            string serverFolder = @"\\" + server + folder;
            // 将服务器地址、文件夹和文件名称拼接
            string fullServerFilePath = @"\\" + server + folder + fileName;
            // 尝试连接获取状态
            if (ConnectState(serverFolder, userName, passWord, out string ErrorMsg))
            {
                //执行方法   
                TransportRemoteToServer(fullServerFilePath, destinationFile);    //实现将远程服务器文件写入到本地  
                return true;
            }
            else
            {
                return false;
            }

        }

        /// <summary>  
        /// 从本地上传文件至服务器
        /// </summary>  
        /// <param name="src">远程服务器路径（共享文件夹路径）</param>  
        /// <param name="dst">本地文件夹路径</param>  
        /// <param name="fileName">上传至服务器上的文件名，包含扩展名</param>  
        public static void TransportRemoteToServer(string src, string dst)
        {
            // 如果要上传的本地文件不存在则新建一个
            if (!File.Exists(dst))
            {
                File.Create(dst).Dispose();
            }

            // 在服务器中创建需要被上传的文件流
            FileStream inFileStream = new FileStream(src, FileMode.OpenOrCreate);    //从远程服务器下载到本地的文件 

            // 打开本地文件流，后续将其上传到服务器的文件流中
            FileStream outFileStream = new FileStream(dst, FileMode.Open);    //远程服务器文件  此处假定远程服务器共享文件夹下确实包含本文件，否则程序报错  

            // 创建一个能容纳本地文件流大小的 byte 数组
            byte[] buf = new byte[outFileStream.Length];

            int byteCount;

            // 将存有本地文件内容的 byte 数组写入服务器中的文件流
            while ((byteCount = outFileStream.Read(buf, 0, buf.Length)) > 0)
            {

                inFileStream.Write(buf, 0, byteCount);

            }

            // 清理缓存区、释放文件流
            inFileStream.Flush();

            inFileStream.Close();

            outFileStream.Flush();

            outFileStream.Close();

        }

        /// <summary>  
        /// 连接远程共享文件夹  
        /// </summary>  
        /// <param name="path">远程共享文件夹的路径</param>  
        /// <param name="userName">用户名</param>  
        /// <param name="passWord">密码</param>  
        /// <returns></returns>  
        public static bool ConnectState(string path, string userName, string passWord, out string ErrorMsg)
        {
            bool flag = false;
            Process proc = new Process();
            try
            {
                proc.StartInfo.FileName = "cmd.exe";
                proc.StartInfo.UseShellExecute = false;
                proc.StartInfo.RedirectStandardInput = true;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.CreateNoWindow = true;
                proc.Start();
                proc.StandardInput.WriteLine("net use * /del /y");
                string dosLine = "net use " + path + " " + passWord + " /user:" + userName;
                proc.StandardInput.WriteLine(dosLine);
                proc.StandardInput.WriteLine("exit");
                while (!proc.HasExited)
                {
                    proc.WaitForExit(1000);
                }
                string errormsg = proc.StandardError.ReadToEnd();
                proc.StandardError.Close();
                if (string.IsNullOrEmpty(errormsg))
                {
                    ErrorMsg = "";
                    flag = true;
                }
                else
                {
                    ErrorMsg = errormsg;
                }
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
            }
            finally
            {
                proc.Close();
                proc.Dispose();
            }
            return flag;
        }
    }
}
