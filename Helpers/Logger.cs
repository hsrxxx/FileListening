using System;
using System.IO;
using System.Linq;

namespace FileListening
{
    public class Logger
    {
        public string logContent;
        public string LogFolderName = "FileListeningLogs";

        /// <summary>
        /// 追加日志信息
        /// </summary>
        /// <param name="msg">信息</param>
        public void SetLogContent(string msg)
        {
            logContent += $"时间[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}] 信息[{msg}]" + Environment.NewLine;
        }

        /// <summary>
        /// 保存日志信息
        /// </summary>
        /// <param name="FolderName">类型文件夹名称</param>
        /// <param name="LogName">日志名称</param>
        /// <param name="ErrorMsg">错误返回信息</param>
        /// <returns></returns>
        public bool SaveLog(string FolderName, string LogName, out string ErrorMsg)
        {
            try
            {
                //读取配置的盘符
                string LogPath = OrBitHelper.ReadIniData("config", "LogPath", "");

                // 配置路径盘符存在则使用，不存在则默认第一个盘符路径
                if (LogPath == "")
                {
                    // 获取最后一个盘符的路径
                    LogPath = Directory.GetLogicalDrives()[0] + LogFolderName;
                }
                else
                {
                    // 路径是否存在
                    if (!Directory.Exists(LogPath))
                    {
                        // 盘符是否存在
                        if (!Directory.GetLogicalDrives().Contains(LogPath.Substring(0, 1) + @":\"))
                        {
                            // 获取第一个盘符的路径
                            LogPath = Directory.GetLogicalDrives()[0] + LogFolderName;
                        }
                    }
                }

                //全地址
                LogPath = @LogPath + @"\" + FolderName;

                if (logContent == "")
                {
                    ErrorMsg = "日志内容为空!";
                    return false;
                }

                if (!Directory.Exists(LogPath))
                {
                    Directory.CreateDirectory(LogPath);
                }

                LogPath = LogPath + @"\" + DateTime.Now.ToString("yyyyMMdd");
                if (!Directory.Exists(LogPath))
                {
                    Directory.CreateDirectory(LogPath);
                }
                //一个小时产生一个日志
                string RealPath = LogPath + @"\" + LogName + "-" + DateTime.Now.ToString("yyyyMMdd-HH") + @".txt";
                StreamWriter Stw = null;
                try
                {
                    if (File.Exists(RealPath)) //追加文件内容
                    {
                        Stw = File.AppendText(RealPath);
                        Stw.WriteLine(logContent);
                        Stw.Close();
                        Stw.Dispose();
                    }
                    else//新建文件 并写第一行数据
                    {
                        Stw = new StreamWriter(RealPath);
                        Stw.WriteLine(logContent);
                        Stw.Close();
                        Stw.Dispose();
                    }
                    ErrorMsg = "";
                    return true;
                }
                catch (Exception ex)
                {
                    ErrorMsg = "Log记录异常-1:" + ex.Message;
                    return false;
                }
                finally
                {
                    if (Stw != null)
                    {
                        Stw.Close();
                        Stw.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMsg = "Log记录异常-2:" + ex.Message;
                return false;
            }
        }
    }
}