using Newtonsoft.Json;
using System;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace FileListening
{
    public static class OrBitHelper
    {
        /// <summary>
        /// ini配置文件写入方法，返回0表示失败，非0为成功
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        /// <summary>
        /// ini配置文件读取方法，返回取得字符串缓冲区的长度
        /// </summary>
        /// <param name="section"></param>
        /// <param name="key"></param>
        /// <param name="def"></param>
        /// <param name="retVal"></param>
        /// <param name="size"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        [DllImport("kernel32")]
        private static extern long GetPrivateProfileString(string section, string key, string def,
            StringBuilder retVal, int size, string filePath);

        /// <summary>
        /// 读取Ini配置文件信息
        /// </summary>
        /// <param name="Section">区间</param>
        /// <param name="Key">键值</param>
        /// <param name="NoText">默认值</param>
        /// <returns></returns>
        public static string ReadIniData(string Section, string Key, string NoText)
        {
            string path = System.AppDomain.CurrentDomain.BaseDirectory + @"/config.ini";
            if (File.Exists(path))
            {
                StringBuilder temp = new StringBuilder(1024);
                GetPrivateProfileString(Section, Key, NoText, temp, 1024, path);
                return temp.ToString();
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary>
        /// 写入Ini配置文件信息，成功返回true，失败返回false
        /// </summary>
        /// <param name="Section">区间</param>
        /// <param name="Key">键值</param>
        /// <param name="Value">值</param>
        /// <returns></returns>
        public static bool WriteIniData(string Section, string Key, string Value)
        {
            string path = System.AppDomain.CurrentDomain.BaseDirectory + @"/config.ini";
            if (File.Exists(path))
            {
                long OpStation = WritePrivateProfileString(Section, Key, Value, path);
                if (OpStation == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 测试WCF是否联通
        /// </summary>
        /// <param name="WCFAddress"></param>
        /// <returns></returns>
        public static bool TestConnectMES(string WCFAddress)
        {
            string Sql = @"select 1";
            OrBitADCService.ADCService ADC = new OrBitADCService.ADCService();
            DataSet ds = new DataSet();
            ds = ADC.GetDataSetWithSQLString(WCFAddress, Sql);
            if (ds == null || ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取WCF测试地址
        /// </summary>
        /// <param name="StrType"></param>
        /// <param name="flag"></param>
        /// <returns></returns>
        public static string GetWCFAddress(string StrType, out bool flag)
        {
            string WCFAddress = "";
            string TxtPath = System.AppDomain.CurrentDomain.BaseDirectory + @"WCFAddress.txt";

            if (!File.Exists(TxtPath))
            {
                flag = false;
                return "";
            }
            else
            {
                //获取地址
                try
                {
                    StreamReader sr = new StreamReader(TxtPath, Encoding.Default);
                    String line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line.ToString().Contains(StrType))
                        {
                            WCFAddress = line.ToString().Replace(StrType, "").TrimStart().TrimEnd();
                        }

                    }
                    sr.Close();
                    sr.Dispose();
                    flag = true;
                    return WCFAddress;
                }
                catch (IOException e)
                {
                    flag = false;
                    return WCFAddress;
                }
            }


        }

        /// <summary>
        /// 对象转Json
        /// </summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        public static string ObjectToJson(object o)
        {
            string JsonStr = "";
            try
            {
                JsonStr = JsonConvert.SerializeObject(o);
                return JsonStr;
            }
            catch (Exception ex)
            {
                JsonStr = ex.Message;
                return JsonStr;
            }
        }

        /// <summary>
        /// Json转对象
        /// </summary>
        /// <param name="ob"></param>
        /// <returns></returns>
        public static T JsonToObject<T>(string json)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception)
            {
                return default(T);
            }
        }
    }

}