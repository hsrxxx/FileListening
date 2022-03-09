using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FileListening
{
    class FtpHelper
    {
        /// <summary>
        /// 上传文件至FTP服务器
        /// </summary>
        /// <param name="destinationFile">本地文件路径</param>
        /// <param name="ftpFolder">Ftp文件路径（包含扩展名）</param>
        /// <returns></returns>
        public static bool UploadFile(string destinationFile, string ftpFolder)
        {
            string server = OrBitHelper.ReadIniData("FtpConfig", "server", "");
            string userName = OrBitHelper.ReadIniData("FtpConfig", "userName", "");
            string passWord = OrBitHelper.ReadIniData("FtpConfig", "passWord", "");

            Uri ftpUri = new Uri("ftp://" + server + ftpFolder);
            
            if (ConnectState(server, out string ErrorMsg))
            {
                UploadFile(ftpUri, userName, passWord, destinationFile);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断文件夹路径是否存在，不存在则创建
        /// </summary>
        /// <param name="ftpUri"></param>
        /// <param name="userName"></param>
        /// <param name="passWord"></param>
        /// <param name="ErrorMsg"></param>
        /// <returns></returns>
        public static bool MakeDirectory(Uri ftpUri, string userName, string passWord, out string ErrorMsg)
        {
            bool flag = false;
            ErrorMsg = "";
            // 检查URI是否符合FTP协议
            if (ftpUri.Scheme != Uri.UriSchemeFtp)
            {
                ErrorMsg = "Uri路径不符合FTP协议！";
                return flag;
            }
            if (!Directory.Exists(ftpUri.ToString()))
            {
                try
                {
                    FtpWebRequest ftp = (FtpWebRequest)WebRequest.Create(ftpUri);
                    ftp.Credentials = new NetworkCredential(userName, passWord);
                    ftp.Method = WebRequestMethods.Ftp.MakeDirectory;
                    FtpWebResponse ftpResponse = ftp.GetResponse() as FtpWebResponse;
                    string code = ftpResponse.StatusCode.ToString();
                    ftpResponse.Close();
                    Console.WriteLine(code);
                }
                catch (Exception ex)
                {
                    ErrorMsg = ex.Message;
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 从本地上传文件到Ftp
        /// </summary>
        /// <param name="ftpUri">完整上传路径（包括扩展名）</param>
        /// <param name="userName">用户名</param>
        /// <param name="passWord">密码</param>
        /// <param name="destinationFile">本地文件路径</param>
        public static void UploadFile(Uri ftpUri, string userName, string passWord, string destinationFile)
        {
            FtpWebRequest ftp = (FtpWebRequest)WebRequest.Create(ftpUri);
            ftp.Credentials = new NetworkCredential(userName, passWord);
            ftp.Method = WebRequestMethods.Ftp.UploadFile;
            ftp.UseBinary = true;
            ftp.UsePassive = true;
            ftp.KeepAlive = false;

            // 获取ftp文件流
            Stream ftpstream = ftp.GetRequestStream();
            // 获取本地文件流
            FileStream fs = File.OpenRead(destinationFile);

            byte[] buffer = new byte[fs.Length];

            // 将本地文件流写入缓冲区
            fs.Read(buffer, 0, buffer.Length);
            // 将缓冲区文件写入ftp文件流
            ftpstream.Write(buffer, 0, buffer.Length);

            // 关闭连接
            fs.Close();
            ftpstream.Close();
        }

        /// <summary>
        /// 连接FTP服务测试
        /// </summary>
        /// <param name="URI">FTP地址</param>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public static bool ConnectState(string server, out string ErrorMsg)
        {
            bool flag = false;
            ErrorMsg = "";
            try
            {
                // 根据服务器信息FtpWebRequest创建类的对象
                // FtpWebRequest ftp = (FtpWebRequest)WebRequest.Create(URI);
                // ftp.Credentials = new NetworkCredential(username, password);
                // ftp.Method = WebRequestMethods.Ftp.ListDirectory;
                // FtpWebResponse ftpResponse = ftp.GetResponse() as FtpWebResponse;
                // string code = ftpResponse.StatusCode.ToString();
                // ftpResponse.Close();
                // flag = true;

                System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient(server, 21);
                flag = client.Connected;
                client.Close();
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
            }
            return flag;
        }

    }
}
