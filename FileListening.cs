using FileListening.Entity;
using IWshRuntimeLibrary;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using File = System.IO.File;

namespace FileListening
{

    public partial class FileListening : Form
    {
        /// <summary>
        /// 记录文件偏移字典
        /// </summary>
        private static Dictionary<string, long> fileDic = new Dictionary<string, long>();

        /// <summary>
        /// 多线程并发队列
        /// </summary>
        private static ConcurrentQueue<FileContent> logQueue = new ConcurrentQueue<FileContent>();

        /// <summary>
        /// 配置文件路径
        /// </summary>
        private readonly string path = AppDomain.CurrentDomain.BaseDirectory + "/config.ini";

        /// <summary>
        /// 监听对象
        /// </summary>
        private FileSystemWatcher watcher = new FileSystemWatcher();

        /// <summary>
        /// 记录监听文件的最后创建、修改时间用于下次做比较是否执行操作。
        /// </summary>
        DateTime lastRead = DateTime.MinValue;

        /// <summary>
        /// 自动伸缩窗体
        /// </summary>
        private static AutoAdaptWindowsSize autoAdaptSize;

        [DllImport("user32.dll", EntryPoint = "AnimateWindow")]
        private static extern bool AnimateWindow(IntPtr handle, int ms, int flags);

        /// <summary>
        /// 连续输入的键值
        /// </summary>
        private string inputKey = "";

        /// <summary>
        /// 监听开关
        /// </summary>
        private bool isOpen = false;

        /// <summary>
        /// 文件内容偏移量
        /// </summary>
        //private int offset = 0;

        /// <summary>
        /// 监听间隔时间（毫秒）
        /// </summary>
        //private int interval = 0;

        /// <summary>
        /// 监听文件类型
        /// </summary>
        //private string fileTypeStr = "";

        /// <summary>
        /// 监听路径
        /// </summary>
        //private string listeningPath = "";

        /// <summary>
        /// NG图片路径
        /// </summary>
        //private string NGImagePath = "";

        /// <summary>
        /// 整版图片路径
        /// </summary>
        //private string FullImagePath = "";

        /// <summary>
        /// 过滤方式 "*.*" 不过滤
        /// </summary>
        //private string filter = "";

        #region 判断文件是否呗占用
        [DllImport("kernel32.dll")]
        public static extern IntPtr _lopen(string lpPathName, int iReadWrite);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr hObject);

        public const int OF_READWRITE = 2;
        public const int OF_SHARE_DENY_NONE = 0x40;
        public readonly IntPtr HFILE_ERROR = new IntPtr(-1);
        #endregion

        /// <summary>
        /// 快捷方式名称-任意自定义
        /// </summary>
        private const string quickName = "FLTool";

        /// <summary>
        /// 获取系统自动启动目录
        /// </summary>
        private string SystemStartPath { get { return Environment.GetFolderPath(Environment.SpecialFolder.Startup); } }

        /// <summary>
        /// 获取程序完整路径
        /// </summary>
        private string AppAllPath { get { return Process.GetCurrentProcess().MainModule.FileName; } }

        /// <summary>
        /// 获取桌面目录
        /// </summary>
        private string DesktopPath { get { return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory); } }

        public FileListening()
        {
            InitializeComponent();

            #region 窗体缩放
            autoAdaptSize = new AutoAdaptWindowsSize(this);
            autoAdaptSize.InitControlsInfo(this.Controls[0]);
            #endregion
        }

        /// <summary>
        /// 初始化加载信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileListening_Load(object sender, EventArgs e)
        {
            // 使其不会对访问错误线程的错误捕捉：指监听文件线程对主线程控件的访问操作属于错误线程，不捕捉则不会报错。
            CheckForIllegalCrossThreadCalls = false;

            BackColor = Color.FromArgb(243, 243, 243);

            //KeyPreview = true;
            //windowState.Items.Add("N/A");
            //windowState.Items.Add("托盘");
            //windowState.Items.Add("最小化");
            //runingState.Items.Add("N/A");
            //runingState.Items.Add("开机自启");
            //fileType.Items.Add("N/A");
            //fileType.Items.Add("AOI-JZ");
            //fileType.Items.Add("SPI-STK");
            //fileType.Items.Add("SPI-HTGD");

            listeningPathBox.Text = OrBitHelper.ReadIniData("basic", "listeningPath", "");
            filterBox.Text = OrBitHelper.ReadIniData("basic", "filter", "");
            intervalBox.Text = OrBitHelper.ReadIniData("basic", "interval", "");
            offsetBox.Text = OrBitHelper.ReadIniData("basic", "offset", "");
            windowState.Text = OrBitHelper.ReadIniData("basic", "windowState", "N/A");
            runingState.Text = OrBitHelper.ReadIniData("basic", "runingState", "N/A");
            fileType.Text = OrBitHelper.ReadIniData("basic", "fileType", "请选择类型!");
            IsFileMove.Checked = Convert.ToBoolean(OrBitHelper.ReadIniData("basic", "IsFileMove", ""));
            FileMovePathBox.Text = OrBitHelper.ReadIniData("basic", "FileMovePath", "");
            NGImagePathBox.Text = OrBitHelper.ReadIniData("JutzeAOI", "NGImagePath", "");
            FullImagePathBox.Text = OrBitHelper.ReadIniData("JutzeAOI", "FullImagePath", "");
        }

        /// <summary>
        /// 返回信息模板
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="type">操作</param>
        /// <param name="fileName">文件名</param>
        /// <param name="content">内容</param>
        /// <returns></returns>
        private string MessageTemp(string time, string type, string fileName, string content)
        {
            return $"时间[{time}] 操作[{type}] 文件[{fileName}] 内容[{content}]";
        }

        /// <summary>
        /// 选择文件路径（通用方法）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChoosePath(object sender, EventArgs e)
        {
            if (isOpen)
            {
                MessageBox.Show("文件监听已开启！请停止监听才能修改！");
                return;
            }
            DialogResult dr = folderBrowserDialog1.ShowDialog();
            //获取所打开文件的文件名
            string filename = folderBrowserDialog1.SelectedPath;
            if (dr == DialogResult.OK && !string.IsNullOrEmpty(filename))
            {
                Button thisClickedButton = sender as Button;
                if(thisClickedButton.Name == "ListeningPathButton")
                {
                    listeningPathBox.Text = filename;
                }else if (thisClickedButton.Name == "NGImagePathButton")
                {
                    NGImagePathBox.Text = filename;
                }
                else if (thisClickedButton.Name == "FullImagePathButton")
                {
                    FullImagePathBox.Text = filename;
                }
                else if (thisClickedButton.Name == "FileMovePathButton")
                {
                    FileMovePathBox.Text = filename;
                }
            }
        }

        /// <summary>
        /// 保存配置按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveButton(object sender, EventArgs e)
        {
            if (SaveData())
            {
                MessageBox.Show("保存成功！");
            }
        }

        /// <summary>
        /// 保存配置信息
        /// </summary>
        private bool SaveData()
        {
            if (!CheckData())
            {
                return false;
            }

            if (runingState.Text == "开机自启")
            {
                return SetMeAutoStart(true);
            }
            else
            {
                SetMeAutoStart(false);
            }

            OrBitHelper.WriteIniData("basic", "listeningPath", listeningPathBox.Text);
            OrBitHelper.WriteIniData("basic", "filter", filterBox.Text);
            OrBitHelper.WriteIniData("basic", "interval", intervalBox.Text);
            OrBitHelper.WriteIniData("basic", "offset", offsetBox.Text);
            OrBitHelper.WriteIniData("basic", "windowState", windowState.Text);
            OrBitHelper.WriteIniData("basic", "runingState", runingState.Text);
            OrBitHelper.WriteIniData("basic", "fileType", fileType.Text);
            OrBitHelper.WriteIniData("basic", "IsFileMove", IsFileMove.Checked.ToString());
            OrBitHelper.WriteIniData("basic", "FileMovePath", FileMovePathBox.Text);
            OrBitHelper.WriteIniData("JutzeAOI", "NGImagePath", NGImagePathBox.Text);
            OrBitHelper.WriteIniData("JutzeAOI", "FullImagePath", FullImagePathBox.Text);

            return true;
        }

        /// <summary>
        /// 校验配置数据是否正确
        /// </summary>
        /// <returns></returns>
        private bool CheckData()
        {
            // 判断监听路径是否存在
            if (!Directory.Exists(listeningPathBox.Text))
            {
                MessageBox.Show("监听路径不存在，请重新选择！");
                return false;
            }
            // 文件是否转移
            if (IsFileMove.Checked)
            {
                // 判断文件转移路径是否存在
                if (!Directory.Exists(FileMovePathBox.Text))
                {
                    MessageBox.Show("文件转移路径不存在，请重新选择！");
                    return false;
                }
            }
            // 判断监听类型的配置是否正确
            if (fileType.Text == "AOI-JZ")
            {
                if (!Directory.Exists(NGImagePathBox.Text))
                {
                    MessageBox.Show("AOI-JZ：NG图片路径不存在，请重新选择！");
                    return false;
                }
                if (!Directory.Exists(FullImagePathBox.Text))
                {
                    MessageBox.Show("AOI-JZ：整版路径不存在，请重新选择！");
                    return false;
                }
            }
            // 判断监听间隔是否正确
            if (!IsInt(intervalBox.Text))
            {
                MessageBox.Show("请输入正确的间隔毫秒数！");
                return false;
            }
            // 判断偏移量是否正确
            if (!IsInt(offsetBox.Text))
            {
                MessageBox.Show("请输入正确的偏移量！");
                return false;
            }
            return true;
        }

        /// <summary>
        /// 检查不同的监听文件类型的其他配置信息
        /// </summary>
        /// <returns></returns>
        private bool CheckTypeConfig()
        {
            if (fileType.Text == "N/A")
            {
                MessageBox.Show("请选择监听类型!");
                return false;
            }
            else if (fileType.Text == "AOI-JZ")
            {
                if (NGImagePathBox.Text == "")
                {
                    MessageBox.Show("AOI-JZ：请选择NG图片路径!");
                    return false;
                }
                if (FullImagePathBox.Text == "")
                {
                    MessageBox.Show("AOI-JZ：请选择整版图片路径!");
                    return false;
                }
                if (!Directory.Exists(NGImagePathBox.Text))
                {
                    MessageBox.Show("AOI-JZ：NG图片路径不存在或无效!");
                    return false;
                }
                if (!Directory.Exists(FullImagePathBox.Text))
                {
                    MessageBox.Show("AOI-JZ：整版图片路径不存在或无效!");
                    return false;
                }
            }
            else if (fileType.Text == "SPI-STK")
            {
            }
            return true;
        }

        /// <summary>
        /// 开启/停止监听按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListeningSwitch(object sender, EventArgs e)
        {
            // 检查监听类型配置信息
            if (!CheckTypeConfig())
            {
                return;
            }

            // 测试连接是否成功
            if (!DBConn())
            {
                return;
            }

            // 检验配置是否正确
            if (!SaveData())
            {
                return;
            }

            isOpen = !isOpen;
            if (isOpen)
            {
                // 界面变化
                ListeningSwitchButton.Text = "停止监听";
                ListeningSwitchButton.BackColor = Color.Gray;
                listeningPathBox.ReadOnly = true;
                FileMovePathBox.ReadOnly = true;
                NGImagePathBox.ReadOnly = true;
                FullImagePathBox.ReadOnly = true;
                filterBox.ReadOnly = true;
                intervalBox.ReadOnly = true;
                offsetBox.ReadOnly = true;

                IsFileMove.Enabled = false;
                windowState.Enabled = false;
                runingState.Enabled = false;
                fileType.Enabled = false;
                // 监听配置赋值
                // NGImagePath = NGImagePathBox.Text;
                // FullImagePath = FullImagePathBox.Text;
                // fileTypeStr = fileType.Text;
                // listeningPath = listeningPathBox.Text;
                // filter = filterBox.Text;
                // interval = int.Parse(intervalBox.Text);
                // offset = int.Parse(offsetBox.Text);

                // 窗体状态
                if (windowState.Text == "最小化")
                {
                    WindowState = FormWindowState.Minimized;
                }

                // 托盘
                if (windowState.Text == "托盘")
                {
                    Visible = false;
                    NotifyIcon.Visible = true;
                }

                // 初始化监听的数据内容
                if (fileType.Text == "AOI-JZ")
                {
                    InitFileDic();
                }
                // 开启监听
                WatcherStart();
                // 开启定时器
                Timer.Interval = int.Parse(intervalBox.Text);
                Timer.Start();

                return;
            }
            else
            {
                // 结束监听
                WatcherEnd();
                // 定时器停止
                Timer.Stop();
                // 界面变化
                ListeningSwitchButton.Text = "开启监听";
                ListeningSwitchButton.BackColor = Color.Gainsboro;
                listeningPathBox.ReadOnly = false;
                FileMovePathBox.ReadOnly = false;
                NGImagePathBox.ReadOnly = false;
                FullImagePathBox.ReadOnly = false;
                filterBox.ReadOnly = false;
                intervalBox.ReadOnly = false;
                offsetBox.ReadOnly = false;

                IsFileMove.Enabled = true;
                windowState.Enabled = true;
                runingState.Enabled = true;
                fileType.Enabled = true;
            }
        }

        public static void Delay(int milliSecond)
        {
            int start = Environment.TickCount;
            while (Math.Abs(Environment.TickCount - start) < milliSecond)//毫秒
            {
                Application.DoEvents();//可执行某无聊的操作
            }
        }

        /// <summary>
        /// 定时获取监听内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListeningTimer(object sender, EventArgs e)
        {
            // 执行前停止定时任务以防止为中心完成再次执行
            try
            {
                Timer.Stop();
                while (logQueue.Count > 0)
                {
                    //DateTime StartTime = DateTime.Now;
                    //DateTime EndTime;
                    //double Spn;

                    FileContent result;
                    logQueue.TryDequeue(out result);

                    // 获取 textBox5 的所有行数，没有则为 -1
                    // int count = textBox5.Lines.GetUpperBound(0);
                    // 输出追加的内容
                    string logStr = MessageTemp(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"),
                                                result.ListeningType,
                                                result.FileName,
                                                (result.Content + result.ContentItem).Length.ToString());//.Length.ToString());

                    textBox5.Text += logStr + Environment.NewLine;

                    Logger logger = new Logger();
                    logger.SetLogContent(logStr);
                    logger.SaveLog(fileType.Text, result.FileName.Split('\\')[0], out string ErrorMsg);
                    if (ErrorMsg != "")
                    {
                        textBox5.Text += "本地日志保存错误：" + ErrorMsg + Environment.NewLine;
                    }

                    // 每次添加内容自动滚到最底部
                    textBox5.SelectionStart = textBox5.TextLength; 
                    textBox5.ScrollToCaret();

                    // 类型、方式正确才会进行数据插入操作
                    // 创建的时候也会进行解析判定数据符合在进行下一步
                    // 监听到文件的改变或创建，且不报错时进行处理
                    if ((result.ListeningType == "Changed" || result.ListeningType == "Created") && !result.Content.Contains("错误："))
                    {
                        // 处理数据
                        _ = InsertData(result.Content, result.ContentItem, result.FileName);
                    }

                    //EndTime = DateTime.Now;
                    //Spn = EndTime.Subtract(StartTime).TotalMilliseconds;
                    //textBox5.Text +="开始时间:" + StartTime.ToString("yyyy-MM-dd HH:mm:ss.fff") +
                    //                "结束时间:" + EndTime.ToString("yyyy-MM-dd HH:mm:ss.fff") +
                    //                "总耗时(毫秒):" + Spn.ToString() + Environment.NewLine;
                }
            }
            catch(Exception ex)
            {
                textBox5.Text += "定时数据处理异常：" + ex.Message + Environment.NewLine;
            }
            finally
            {
                Timer.Start();
            }
        }

        /// <summary>
        /// 将被监听的文件处理完成后转移
        /// </summary>
        /// <param name="sourceFilePath">监听路径：E:\特发东智\监听测试\csv</param>
        /// <param name="ListeningFilePath">被监听文件路径：线体1\xxx.csv</param>
        private void FileMove(string sourceFilePath, string ListeningFilePath)
        {
            string sourceFileName = sourceFilePath + "\\" + ListeningFilePath;
            string destFileName = FileMovePathBox.Text + "\\" + Path.GetFileNameWithoutExtension(sourceFilePath) + "\\" + ListeningFilePath;
            // 去掉文件名路径后的文件夹目录是否存在
            if (!Directory.Exists(Path.GetDirectoryName(destFileName)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(destFileName));
            }
            try
            {
                File.Move(sourceFileName, destFileName);
                textBox5.Text += "文件成功转移：" + sourceFileName + destFileName + Environment.NewLine;
            }
            catch (Exception ex)
            {
                textBox5.Text += "文件转移失败：" + ex.Message + Environment.NewLine;
            }
        }

        /// <summary>
        /// 将条码的过站数据插入数据库
        /// </summary>
        /// <param name="data">过站数据</param>
        private async Task InsertData(string data, string dataItem, string filePath)
        {
            await Task.Factory.StartNew(() =>
            {
                // string[] ItemList = Regex.Split(data, "\r\n", RegexOptions.IgnoreCase);
                string MachineName = Environment.MachineName;
                if (fileType.Text == "AOI-JZ")
                {
                    JutzeAOI jutzeAOI = OrBitHelper.JsonToObject<JutzeAOI>(data);

                    string FullImageFilePath = "";
                    string NGImageFilePath = "";

                    #region 获取关联的整版图片路径
                    foreach (string dir in Directory.GetDirectories(FullImagePathBox.Text, "*" + jutzeAOI.TagCode + "*", SearchOption.AllDirectories))
                    {
                        foreach (string file in Directory.GetFiles(dir, "*.jpg", SearchOption.AllDirectories))
                        {
                            FullImageFilePath = file;
                            textBox5.Text += "整版图片路径：" + file + Environment.NewLine;
                        }
                    }
                    //ServerHelper.UploadFile(FullImageFilePath, @"\新建文件夹\MES\Test", @"\Test1.jpg");
                    //FtpHelper.UploadFile(FullImageFilePath, @"/Test1.jpg");
                    #endregion

                    #region 获取关联的NG图片路径
                    if (jutzeAOI.Image == "OK.jpg")
                    {
                        textBox5.Text += "NG图片路径：" + "无NG图片" + Environment.NewLine;
                    }
                    else
                    {
                        foreach (string file in Directory.GetFiles(NGImagePathBox.Text, jutzeAOI.Image, SearchOption.AllDirectories))
                        {
                            NGImageFilePath = file;
                            textBox5.Text += "NG图片路径：" + file + Environment.NewLine;
                        }
                        //ServerHelper.UploadFile(NGImageFilePath, @"\新建文件夹\MES\Test", @"\Test2.jpg");
                        //FtpHelper.UploadFile(NGImageFilePath, @"/Test2.jpg");
                    }
                    #endregion

                    DBHelper db = new DBHelper();

                    // WCF地址
                    string WCFAddress = OrBitHelper.ReadIniData("DBConfig", "WCF", "");

                    //执行存储过程
                    string SqlStr = @"  DECLARE	@return_value int,
                                                @I_ReturnMessage nvarchar(max),
                                                @HandleResult nvarchar(max)

                                        EXEC	@return_value = [dbo].[MESAPI_FileListeningTest_DoMethod]
                                                @I_ReturnMessage = @I_ReturnMessage OUTPUT,
		                                        @FileName = '" + filePath + @"',
		                                        @Data = '" + data + @"'

                                        SELECT	@return_value as ReturnValue,@I_ReturnMessage as I_ReturnMessage";

                    DataSet ds = null;
                    ds = db.SelectDataSet(SqlStr, out string ErrorMsg, WCFAddress);

                    // 检查数据库连接是否成功
                    if (ds == null || ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                    {
                        textBox5.Text += "数据库连接失败：" + ErrorMsg + Environment.NewLine;
                    }
                    else
                    {
                        // 检查返回值
                        int LastIndex = ds.Tables.Count - 1;
                        if (ds.Tables[LastIndex].Rows[0]["ReturnValue"].ToString() == "-1")
                        {
                            textBox5.Text += "调用成功！反馈失败：" + ds.Tables[LastIndex].Rows[0]["I_ReturnMessage"].ToString() + Environment.NewLine;
                        }
                        else
                        {
                            textBox5.Text += "调用成功！反馈成功：" + ds.Tables[LastIndex].Rows[0]["I_ReturnMessage"].ToString() + Environment.NewLine;
                        }
                    }
                }
                else if (fileType.Text == "SPI-STK")
                {
                    DBHelper db = new DBHelper();

                    // WCF地址
                    string WCFAddress = OrBitHelper.ReadIniData("DBConfig", "WCF", "");

                    //执行存储过程
                    string SqlStr = @"  DECLARE	@return_value int,
                                                @I_ReturnMessage nvarchar(max),
                                                @HandleResult nvarchar(max)

                                        EXEC	@return_value = [dbo].[MESAPI_FileListeningTest_DoMethod]
                                                @I_ReturnMessage = @I_ReturnMessage OUTPUT,
		                                        @FileName = '" + filePath + @"',
		                                        @Data = '" + data + @"'

                                        SELECT	@return_value as ReturnValue,@I_ReturnMessage as I_ReturnMessage";

                    DataSet ds = null;
                    ds = db.SelectDataSet(SqlStr, out string ErrorMsg, WCFAddress);

                    // 检查数据库连接是否成功
                    if (ds == null || ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                    {
                        textBox5.Text += "数据库连接失败：" + ErrorMsg + Environment.NewLine;
                    }
                    else
                    {
                        // 检查返回值
                        int LastIndex = ds.Tables.Count - 1;
                        if (ds.Tables[LastIndex].Rows[0]["ReturnValue"].ToString() == "-1")
                        {
                            textBox5.Text += "调用成功！反馈失败：" + ds.Tables[LastIndex].Rows[0]["I_ReturnMessage"].ToString() + Environment.NewLine;
                        }
                        else
                        {
                            textBox5.Text += "调用成功！反馈成功：" + ds.Tables[LastIndex].Rows[0]["I_ReturnMessage"].ToString() + Environment.NewLine;
                        }
                    }
                    // 文件是否转移
                    if (IsFileMove.Checked)
                    {
                        FileMove(listeningPathBox.Text, filePath);
                    }
                    // Console.WriteLine("数据处理完成！线程ID:" + Thread.CurrentThread.ManagedThreadId);
                }
                else if (fileType.Text == "SPI-HTGD")
                {
                    DBHelper db = new DBHelper();

                    // WCF地址
                    string WCFAddress = OrBitHelper.ReadIniData("DBConfig", "WCF", "");

                    //执行存储过程
                    string SqlStr = @"  DECLARE	@return_value int,
                                                @I_ReturnMessage nvarchar(max),
                                                @HandleResult nvarchar(max)

                                        EXEC	@return_value = [dbo].[MESAPI_FileListeningTest_DoMethod]
                                                @I_ReturnMessage = @I_ReturnMessage OUTPUT,
		                                        @FileName = '" + filePath + @"',
		                                        @Data = '" + data + "|" + dataItem + @"'

                                        SELECT	@return_value as ReturnValue,@I_ReturnMessage as I_ReturnMessage";

                    DataSet ds = null;
                    ds = db.SelectDataSet(SqlStr, out string ErrorMsg, WCFAddress);

                    // 检查数据库连接是否成功
                    if (ds == null || ds.Tables.Count <= 0 || ds.Tables[0].Rows.Count <= 0)
                    {
                        textBox5.Text += "数据库连接失败：" + ErrorMsg + Environment.NewLine;
                    }
                    else
                    {
                        // 检查返回值
                        int LastIndex = ds.Tables.Count - 1;
                        if (ds.Tables[LastIndex].Rows[0]["ReturnValue"].ToString() == "-1")
                        {
                            textBox5.Text += "调用成功！反馈失败：" + ds.Tables[LastIndex].Rows[0]["I_ReturnMessage"].ToString() + Environment.NewLine;
                        }
                        else
                        {
                            textBox5.Text += "调用成功！反馈成功：" + ds.Tables[LastIndex].Rows[0]["I_ReturnMessage"].ToString() + Environment.NewLine;
                        }
                    }
                    // 文件是否转移
                    if (IsFileMove.Checked)
                    {
                        FileMove(listeningPathBox.Text, filePath);
                    }
                    // Console.WriteLine("数据处理完成！线程ID:" + Thread.CurrentThread.ManagedThreadId);
                }
            });
        }

        #region 异步测试
        /// <summary>
        /// async 将函数内的 await 函数进行异步处理 本身的代码还是同步如 Thread.Sleep(1000)
        /// 在这个函数内执行2个需要 await 的函数
        /// </summary>
        /// <returns></returns>
        private async Task AsyncMethod()
        {
            Thread.Sleep(1000);
            Console.WriteLine("1000");
            string Result = await TimeConsumingMethod() + " + AsyncMethod. My Thread ID is :" + Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine(Result);
            Result = await TimeConsumingMethod1() + " + AsyncMethod. My Thread ID is :" + Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine(Result);
            //返回值是Task的函数可以不用return
        }

        /// <summary>
        /// 这个函数就是一个耗时函数，可能是IO操作，也可能是cpu密集型工作。
        /// </summary>
        /// <returns></returns>
        private Task<string> TimeConsumingMethod()
        {
            var task = Task.Factory.StartNew(() =>
            {
                Console.WriteLine("异步函数1. My Thread ID is :" + Thread.CurrentThread.ManagedThreadId);
                Thread.Sleep(5000);
                Console.WriteLine("异步函数1 + 5000. My Thread ID is :" + Thread.CurrentThread.ManagedThreadId);
                return "异步函数1";
            });
            return task;
        }

        /// <summary>
        /// 这个函数就是一个耗时函数，可能是IO操作，也可能是cpu密集型工作。
        /// </summary>
        /// <returns></returns>
        private Task<string> TimeConsumingMethod1()
        {
            var task = Task.Factory.StartNew(() =>
            {
                Thread.Sleep(2000);
                Console.WriteLine("异步函数2 + 2000. My Thread ID is :" + Thread.CurrentThread.ManagedThreadId);
                return "异步函数2";
            });
            return task;
        }

        /// <summary>
        /// 测试异步方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            Console.WriteLine("非异步. My Thread ID is :" + Thread.CurrentThread.ManagedThreadId);
            _ = AsyncMethod();
            Console.WriteLine("非异步. My Thread ID is :" + Thread.CurrentThread.ManagedThreadId);
        }
        #endregion

        #region 文件监听相关操作
        /// <summary>
        /// 开启监听
        /// </summary>
        private void WatcherStart()
        {
            // 监听路径
            watcher.Path = listeningPathBox.Text;

            // 监听触发条件
            watcher.NotifyFilter = // NotifyFilters.Attributes
                                   // | NotifyFilters.DirectoryName 
                                   // | NotifyFilters.FileName 
                                   // | NotifyFilters.LastAccess
                                   // | NotifyFilters.Security 
                                   // | NotifyFilters.CreationTime
                                 //NotifyFilters.LastWrite;
                                 NotifyFilters.Size;

            // 过滤 "*.*" 为监听全部
            watcher.Filter = filterBox.Text;

            // 监听执行函数
            watcher.Changed += new FileSystemEventHandler(Watcher_Changed);
            // watcher.Created += new FileSystemEventHandler(Watcher_Created);
            // watcher.Deleted += new FileSystemEventHandler(Watcher_Deleted);
            // watcher.Renamed += new RenamedEventHandler(Watcher_Renamed);

            // 是否监听子目录
            watcher.IncludeSubdirectories = true;

            // 开启监听
            watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// 停止监听
        /// </summary>
        private void WatcherEnd()
        {
            watcher.Changed -= new FileSystemEventHandler(Watcher_Changed);
            // watcher.Created -= new FileSystemEventHandler(Watcher_Created);
            // watcher.Deleted -= new FileSystemEventHandler(Watcher_Deleted);
            // watcher.Renamed -= new RenamedEventHandler(Watcher_Renamed);
            watcher.EnableRaisingEvents = false;
        }

        /// <summary>
        /// 对文件的内容偏移进行初始化
        /// </summary>
        private void InitFileDic()
        {
            FileInfo[] files = new FileInfo[0];
            fileDic.Clear();
            if (string.IsNullOrEmpty(listeningPathBox.Text) == false)
            {
                files = new DirectoryInfo(listeningPathBox.Text).GetFiles(filterBox.Text, SearchOption.AllDirectories);
            }
            for (int i = 0; i < files.Length; i++)
            {
                string filePath = files[i].FullName;
                if (int.Parse(offsetBox.Text) == 0)
                {
                    // 记录文件路径和文件偏移
                    fileDic.Add(filePath, files[i].Length);
                }
                else
                {
                    // 如果默认偏移超出原文件则使用原文件大小偏移
                    if (int.Parse(offsetBox.Text) > files[i].Length)//防止超出
                    {
                        fileDic.Add(filePath, files[i].Length);
                    }
                    else
                    {
                        fileDic.Add(filePath, int.Parse(offsetBox.Text));
                    }
                }
            }
        }

        /// <summary>
        /// 创建内存映射文件，获取文件新增的内容
        /// </summary>
        /// <param name="offset">偏移量</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="name">文件名</param>
        /// <param name="ListeningType">监听类型</param>
        private void AppendContentToCosole(long offset, string filePath, string name, string ListeningType)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) 
                    {
                        if (fs.CanSeek)
                        {
                            fs.Seek(offset, SeekOrigin.Begin);
                            fileDic[filePath] = fs.Length;
                            if (offset < fs.Length)//防止期间文件删除后创建导致偏移变化
                            {
                                // 按行读取
                                if(fileType.Text == "AOI-JZ")
                                {
                                    using (StreamReader sr = new StreamReader(fs, Encoding.Default))
                                    {
                                        string tmp = "";
                                        while ((tmp = sr.ReadLine()?.Trim()) != null)
                                        {
                                            if (tmp == string.Empty)
                                            {
                                                continue;
                                            }

                                            string[] ItemList = tmp.Split(new char[] { ',' }, options: StringSplitOptions.RemoveEmptyEntries);
                                            if (ItemList[0] == "BoardIndex")
                                            {
                                                continue;
                                            }
                                            JutzeAOI jutzeAOI = new JutzeAOI
                                            {
                                                BoardIndex = ItemList[0],
                                                GroupName = ItemList[1],
                                                BoardName = ItemList[2],
                                                LotNumber = ItemList[3],
                                                TagCode = ItemList[4],
                                                InspectionTime = ItemList[5],
                                                RepairedTime = ItemList[6],
                                                Operator = ItemList[7],
                                                Shift = ItemList[8],
                                                Result = ItemList[9],
                                                ReferenceName = ItemList[10],
                                                PartCode = ItemList[11],
                                                BlockNum = ItemList[12],
                                                Layer = ItemList[13],
                                                WindowNumber = ItemList[14],
                                                Method = ItemList[15],
                                                Sample = ItemList[16],
                                                NGName = ItemList[17],
                                                Image = ItemList[18]
                                            };

                                            FileContent _q = new FileContent() { FileName = name, Content = OrBitHelper.ObjectToJson(jutzeAOI), ListeningType = ListeningType };
                                            logQueue.Enqueue(_q);
                                        }
                                    }
                                }
                                // 一次性读取
                                else if(fileType.Text == "SPI-STK")
                                {
                                    #region 双表类型
                                    //DataTable SinitTekSPI = new DataTable("SinitTekSPI");
                                    //DataTable SinitTekSPIItem = new DataTable("SinitTekSPIItem");
                                    //// 记录每行记录中的各字段内容
                                    //string[] aryLine = null;
                                    //// 表头
                                    //string[] tableHead = null;
                                    //// 标示列数
                                    //int columnCount = 0;
                                    //// 标示是否是读取标题
                                    //bool IsFirst = true;
                                    //// 是否为主表
                                    //bool IsSinitTekSPI = true;
                                    //using (StreamReader sr = new StreamReader(fs, Encoding.Default))
                                    //{
                                    //    string tmp = "";
                                    //    while ((tmp = sr.ReadLine()?.Trim()) != null)
                                    //    {
                                    //        // 判断为空 读取完成主表后读取子表
                                    //        if (tmp == string.Empty)
                                    //        {
                                    //            IsFirst = true;
                                    //            IsSinitTekSPI = false;
                                    //            continue;
                                    //        }

                                    //        if (IsSinitTekSPI)
                                    //        {
                                    //            // 获取表头行信息
                                    //            if (IsFirst)
                                    //            {
                                    //                tmp = "PCBId,TestTime,TestTakes,TestResults,QrCode,PCBName,ProgramPath,TestBoxCount,AvgHeight,AvgArea,AvgVolume,AvgXOffset,AvgYOffset";
                                    //                tableHead = tmp.Split(',');
                                    //                columnCount = tableHead.Length;
                                    //                //创建列
                                    //                for (int i = 0; i < columnCount; i++)
                                    //                {
                                    //                    DataColumn dc = new DataColumn(tableHead[i]);
                                    //                    SinitTekSPI.Columns.Add(dc);
                                    //                }
                                    //                IsFirst = false;
                                    //            }
                                    //            else
                                    //            {
                                    //                aryLine = tmp.Split(',');
                                    //                DataRow dr = SinitTekSPI.NewRow();
                                    //                for (int j = 0; j < columnCount; j++)
                                    //                {
                                    //                    dr[j] = aryLine[j];
                                    //                }
                                    //                SinitTekSPI.Rows.Add(dr);
                                    //            }
                                    //        }
                                    //        else
                                    //        {
                                    //            // 获取表头行信息
                                    //            if (IsFirst)
                                    //            {
                                    //                tmp = "TestBoxId,TestResults,TestResultsType,Height,Area,Volume,Xoffset,Yoffset,CADName,Makeup,Image2D";
                                    //                tableHead = tmp.Split(',');
                                    //                columnCount = tableHead.Length;
                                    //                //创建列
                                    //                for (int i = 0; i < columnCount; i++)
                                    //                {
                                    //                    DataColumn dc = new DataColumn(tableHead[i]);
                                    //                    SinitTekSPIItem.Columns.Add(dc);
                                    //                }
                                    //                IsFirst = false;
                                    //            }
                                    //            else
                                    //            {
                                    //                // 获取当前行数据 循环将每列数据添加
                                    //                aryLine = tmp.Split(',');
                                    //                DataRow dr = SinitTekSPIItem.NewRow();
                                    //                for (int j = 0; j < columnCount; j++)
                                    //                {
                                    //                    dr[j] = aryLine[j];
                                    //                }
                                    //                SinitTekSPIItem.Rows.Add(dr);
                                    //            }
                                    //        }
                                    //    }
                                    //}
                                    //FileContent _q = new FileContent() 
                                    //{    
                                    //    FileName = name,
                                    //    Content = "主表：SinitTekSPI" + Environment.NewLine +
                                    //                DataTableToJson(SinitTekSPI) + Environment.NewLine +
                                    //                "明细：SinitTekSPIItem" + Environment.NewLine +
                                    //                DataTableToJson(SinitTekSPIItem),
                                    //    // Content = "SinitTekSPIData",
                                    //    ListeningType = ListeningType 
                                    //};
                                    //logQueue.Enqueue(_q);
                                    #endregion

                                    #region 单表类型
                                    DataTable SinitTekSPI = new DataTable("SinitTekSPI");
                                    // 记录每行记录中的各字段内容
                                    string[] aryLine = null;
                                    // 表头
                                    string[] tableHead = null;
                                    // 表头列表
                                    string tableHeadStr = "";
                                    // 标示列数
                                    int columnCount = 0;
                                    // 标示是否是读取标题
                                    bool IsFirst = false;
                                    // 标示是否跳过
                                    bool IsSkip = true;

                                    tableHeadStr = "PadID,ComponentID,Type,AreaPercent,Height,VolumePercent,XOffset,YOffset,PadSizeX,PadSizeY,Area,HeightPercent,Volume,Result,Errcode,PinNum,Barcode,Date,Time,ArrayID";
                                    tableHead = tableHeadStr.Split(',');
                                    columnCount = tableHead.Length;
                                    //创建列
                                    for (int i = 0; i < columnCount; i++)
                                    {
                                        DataColumn dc = new DataColumn(tableHead[i]);
                                        SinitTekSPI.Columns.Add(dc);
                                    }

                                    using (StreamReader sr = new StreamReader(fs, Encoding.Default))
                                    {
                                        string tmp = "";
                                        while ((tmp = sr.ReadLine()?.Trim()) != null)
                                        {
                                            // 判断为空跳过
                                            if (tmp == string.Empty)
                                            {
                                                continue;
                                            }

                                            // 找到表头位置再次跳过
                                            if (tmp.Contains("PadID"))
                                            {
                                                IsFirst = true;
                                                IsSkip = false;
                                                continue;
                                            }

                                            // 开关跳过
                                            if (IsSkip)
                                            {
                                                continue;
                                            }

                                            aryLine = tmp.Split(',');
                                            DataRow dr = SinitTekSPI.NewRow();
                                            for (int j = 0; j < columnCount; j++)
                                            {
                                                dr[j] = aryLine[j];
                                            }
                                            SinitTekSPI.Rows.Add(dr);
                                        }
                                    }
                                    FileContent _q = new FileContent()
                                    {
                                        FileName = name,
                                        Content = DataTableToJson(SinitTekSPI),
                                        ListeningType = ListeningType
                                    };
                                    logQueue.Enqueue(_q);
                                    #endregion

                                }
                                // 一次性读取
                                else if (fileType.Text == "SPI-HTGD")
                                {
                                    #region 双表类型
                                    DataTable HTGDSPI = new DataTable("HTGDSPI");
                                    DataTable HTGDSPIItem = new DataTable("HTGDSPIItem");

                                    // 记录每行记录中的各字段内容
                                    string[] aryLine = null;
                                    // 表头列表
                                    string[] tableHead = null;
                                    // 表头列表
                                    string tableHeadStr = "";
                                    // 标示列数
                                    int columnCount = 0;
                                    // 计数器
                                    int counter = 0;
                                    // 行数
                                    int rowCount = 0;

                                    // 主表添加表头
                                    tableHeadStr = "ModelName,LotSN,Line,Field1,EquipmentModel,Order,TestTimeYmd,TestTimeHms,Result,Layer,TestNumber,NGCount,Id";
                                    tableHead = tableHeadStr.Split(',');
                                    columnCount = tableHead.Length;
                                    for (int i = 0; i < columnCount; i++)
                                    {
                                        DataColumn dc = new DataColumn(tableHead[i]);
                                        HTGDSPI.Columns.Add(dc);
                                    }
                                    // 从表添加表头
                                    tableHeadStr = "ComponentName_BlockNumber,PADNumbers,ProductName,NGType,Field1,Field2,Field3,Id,ParentId";
                                    tableHead = tableHeadStr.Split(',');
                                    columnCount = tableHead.Length;
                                    for (int i = 0; i < columnCount; i++)
                                    {
                                        DataColumn dc = new DataColumn(tableHead[i]);
                                        HTGDSPIItem.Columns.Add(dc);
                                    }

                                    using (StreamReader sr = new StreamReader(fs, Encoding.Default))
                                    {
                                        string tmp = "";
                                        while ((tmp = sr.ReadLine()?.Trim()) != null)
                                        {
                                            // 为空时重置counter
                                            if (tmp == string.Empty)
                                            {
                                                counter = 0;
                                                rowCount += 1;
                                                continue;
                                            }

                                            // 为0时添加主表空行
                                            if (counter == 0)
                                            {
                                                DataRow dr = HTGDSPI.NewRow();
                                                HTGDSPI.Rows.Add(dr);
                                            }

                                            // 小于12时填充主表空行数据
                                            // 不小于12时添加NG从表行数据
                                            if (counter < 12)
                                            {
                                                if (counter == 11)
                                                {
                                                    HTGDSPI.Rows[rowCount][counter + 1] = rowCount + 1;
                                                }
                                                HTGDSPI.Rows[rowCount][counter] = tmp;
                                                counter += 1;
                                            }
                                            else
                                            {
                                                aryLine = tmp.Split(';');
                                                columnCount = aryLine.Length - 1;
                                                DataRow dr = HTGDSPIItem.NewRow();
                                                for (int j = 0; j < columnCount; j++)
                                                {
                                                    if (j == columnCount - 1)
                                                    {
                                                        dr[j + 1] = counter - 11;
                                                        dr[j + 2] = rowCount + 1;
                                                    }
                                                    dr[j] = aryLine[j];
                                                }
                                                HTGDSPIItem.Rows.Add(dr);
                                                counter += 1;
                                            }
                                        }
                                    }
                                    FileContent _q = new FileContent()
                                    {
                                        FileName = name,
                                        Content = DataTableToJson(HTGDSPI),
                                        ContentItem = DataTableToJson(HTGDSPIItem),
                                        ListeningType = ListeningType
                                    };
                                    logQueue.Enqueue(_q);
                                    #endregion

                                }
                            }
                            else
                            {
                                FileContent _q = new FileContent() { FileName = name, Content = "错误：文件内容被删除或不存在变更!", ListeningType = ListeningType };
                                logQueue.Enqueue(_q);
                            }
                        }
                        else
                        {
                            FileContent _q = new FileContent() { FileName = name, Content = "错误：当前流不支持查找！", ListeningType = ListeningType };
                            logQueue.Enqueue(_q);
                        }
                    }
                }
                catch (Exception ex)
                {
                    FileContent _q = new FileContent() { FileName = name, Content = "错误：" + ex.Message, ListeningType = ListeningType };
                    logQueue.Enqueue(_q);
                }
            }
        }

        /// <summary>
        /// 判断文件是否被占用
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool IsFileInUse(string fileName)
        {
            bool inUse = true;
            FileStream fs = null;
            try
            {
                fs = new FileStream(fileName, FileMode.Open, FileAccess.Read,FileShare.None);
                inUse = false;
            }
            catch
            {
            }
            finally
            {
                if (fs != null) fs.Close();
            }
            return inUse;//true表示正在使用,false没有使用
        }

        /// <summary>
        /// 监听修改文件操作    
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            //DateTime lastWriteTime = File.GetLastWriteTime(e.FullPath);
            //if (lastWriteTime.Ticks - lastRead.Ticks > 100000)
            //{
            string ListeningType = e.ChangeType.ToString();

            fileDic.TryGetValue(e.FullPath, out long temp);
            if (temp == 0)
            {
                fileDic[e.FullPath] = 0;
            }

            #region 文件是否被占用
            //IntPtr vHandle = _lopen(e.FullPath, OF_READWRITE | OF_SHARE_DENY_NONE);
            //if (vHandle == HFILE_ERROR)
            //{
            //    FileContent _q = new FileContent() { FileName = e.Name, Content = "错误：文件[" + e.FullPath + "]被占用，最后写入时间：" + lastWriteTime.ToString(), ListeningType = e.ChangeType.ToString() };
            //    logQueue.Enqueue(_q);
            //    return;
            //}
            //CloseHandle(vHandle);

            //if (IsFileInUse(e.FullPath))
            //{
            //    FileContent _q = new FileContent() { FileName = e.Name, Content = "错误：文件[" + e.FullPath + "]被占用，最后写入时间：" + lastWriteTime.ToString(), ListeningType = e.ChangeType.ToString() };
            //    logQueue.Enqueue(_q);
            //    return;
            //}
            #endregion

            // 如果文件被占用则等待结束占用
            while (true)
            {
                try
                {
                    using (Stream stream = File.Open(e.FullPath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                    {
                        if (stream != null)
                            break;
                    }
                }
                catch (Exception ex)
                {
                    FileContent _q = new FileContent() { FileName = e.Name, Content = "错误：" + ex.Message + " 等待结束占用，时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), ListeningType = e.ChangeType.ToString() };
                    logQueue.Enqueue(_q);
                }
                Thread.Sleep(100);
            }
            AppendContentToCosole(fileDic[e.FullPath], e.FullPath, e.Name, ListeningType);
            //    lastRead = lastWriteTime;
            //}
        }

        /// <summary>
        /// 监听创建文件操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            // lastRead = File.GetLastWriteTime(e.FullPath);
            if (File.Exists(e.FullPath))
            {
                fileDic.TryGetValue(e.FullPath, out long temp);
                if (temp == 0)
                {
                    fileDic[e.FullPath] = 0;
                }
                AppendContentToCosole(fileDic[e.FullPath], e.FullPath, e.Name, e.ChangeType.ToString());
            }
            else
            {
                FileContent _q = new FileContent() { FileName = e.Name, Content = "错误：创建后文件[" + e.FullPath + "]已不存在", ListeningType = e.ChangeType.ToString() };
                logQueue.Enqueue(_q);
            }
        }

        /// <summary>
        /// 监听删除文件操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            if (File.Exists(e.FullPath) == false)
            {
                fileDic.Remove(e.FullPath);
                FileContent _q = new FileContent() { FileName = e.Name, Content = e.FullPath, ListeningType = e.ChangeType.ToString() };
                logQueue.Enqueue(_q);
            }
            else
            {
                FileContent _q = new FileContent() { FileName = e.Name, Content = "错误：删除后文件[" + e.FullPath + "]仍然存在", ListeningType = e.ChangeType.ToString() };
                logQueue.Enqueue(_q);
            }
        }

        /// <summary>
        /// 监听重命名文件操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            if (File.Exists(e.FullPath))
            {
                FileInfo _info = new FileInfo(e.FullPath);
                fileDic.Add(e.FullPath, _info.Length);
                fileDic.Remove(e.OldFullPath);
                FileContent _q1 = new FileContent() { FileName = e.OldName + " -> " + e.Name, Content = e.OldFullPath + " -> " + e.FullPath, ListeningType = e.ChangeType.ToString() };
                logQueue.Enqueue(_q1);
            }
            else
            {
                FileContent _q = new FileContent() { FileName = e.Name, Content = "错误：重命名后的文件[" + e.FullPath + "]已不存在", ListeningType = e.ChangeType.ToString() };
                logQueue.Enqueue(_q);
            }
        }
        #endregion

        #region 工具方法
        /// <summary>
        /// 判断是否为带符号的小数点数值
        /// </summary>
        /// <param name="value">需要判断的数值</param>
        /// <returns></returns>
        private static bool IsNumeric(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
        }

        /// <summary>
        /// 判断是否为整数数值
        /// </summary>
        /// <param name="value">需要判断的数值</param>
        /// <returns></returns>
        private static bool IsInt(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*$");
        }

        /// <summary>
        /// 判断是否为带小数点数值
        /// </summary>
        /// <param name="value">需要判断的数值</param>
        /// <returns></returns>
        private static bool IsUnsign(string value)
        {
            return Regex.IsMatch(value, @"^\d*[.]?\d*$");
        }

        /// <summary>
        /// 将 DataTable 转为 Json 格式
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        private static string DataTableToJson(DataTable dt)
        {
            List<Dictionary<string, object>> list = new List<Dictionary<string, object>>();
            foreach (DataRow dr in dt.Rows)
            {
                Dictionary<string, object> result = new Dictionary<string, object>();
                foreach (DataColumn dc in dt.Columns)
                {
                    result.Add(dc.ColumnName, dr[dc]);
                }
                list.Add(result);
            }
            int recursionLimit = 100;
            JavaScriptSerializer serialize = new JavaScriptSerializer();
            serialize.RecursionLimit = recursionLimit;
            serialize.MaxJsonLength = Int32.MaxValue;
            return serialize.Serialize(list);
        }

        /// <summary>
        /// 将 DataTable 转为 XML 格式
        /// </summary>
        /// <param name="xmlDS"></param>
        /// <returns></returns>
        private static string DataTableToXML(DataTable dt)
        {
            StringBuilder sb = new StringBuilder();
            XmlWriter writer = XmlWriter.Create(sb);
            XmlSerializer serializer = new XmlSerializer(typeof(DataTable));
            serializer.Serialize(writer, dt);
            writer.Close();
            return sb.ToString();
        }
        #endregion

        #region 开机自启相关方法
        /// <summary>
        /// 设置开机自动启动-只需要调用改方法就可以了参数里面的bool变量是控制开机启动的开关的
        /// </summary>
        /// <param name="onOff">自启开关</param>
        private bool SetMeAutoStart(bool onOff)
        {
            if (onOff)//开机启动
            {
                //获取启动路径应用程序快捷方式的路径集合
                List<string> shortcutPaths = GetQuickFromFolder(SystemStartPath, AppAllPath);
                //存在2个以快捷方式则保留一个快捷方式-避免重复多于
                if (shortcutPaths.Count >= 2)
                {
                    for (int i = 1; i < shortcutPaths.Count; i++)
                    {
                        DeleteFile(shortcutPaths[i]);
                    }
                }
                else if (shortcutPaths.Count < 1)//不存在则创建快捷方式
                {
                    if(!CreateShortcut(SystemStartPath, quickName, AppAllPath, "文本监听工具"))
                    {
                        return false;
                    }
                }
            }
            else//开机不启动
            {
                //获取启动路径应用程序快捷方式的路径集合
                List<string> shortcutPaths = GetQuickFromFolder(SystemStartPath, AppAllPath);
                //存在快捷方式则遍历全部删除
                if (shortcutPaths.Count > 0)
                {
                    for (int i = 0; i < shortcutPaths.Count; i++)
                    {
                        DeleteFile(shortcutPaths[i]);
                    }
                }
            }
            //创建桌面快捷方式-如果需要可以取消注释
            //CreateDesktopQuick(desktopPath, quickName, AppAllPath);
            return true;
        }

        /// <summary>
        ///  向目标路径创建指定文件的快捷方式
        /// </summary>
        /// <param name="directory">目标目录</param>
        /// <param name="shortcutName">快捷方式名字</param>
        /// <param name="targetPath">文件完全路径</param>
        /// <param name="description">描述</param>
        /// <param name="iconLocation">图标地址</param>
        /// <returns>成功或失败</returns>
        private bool CreateShortcut(string directory, string shortcutName, string targetPath, string description = null, string iconLocation = null)
        {
            try
            {
                if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);                         //目录不存在则创建
                //添加引用 Com 中搜索 Windows Script Host Object Model
                string shortcutPath = Path.Combine(directory, string.Format("{0}.lnk", shortcutName));          //合成路径
                WshShell shell = new IWshRuntimeLibrary.WshShell();
                IWshShortcut shortcut = (IWshRuntimeLibrary.IWshShortcut)shell.CreateShortcut(shortcutPath);    //创建快捷方式对象
                shortcut.TargetPath = targetPath;                                                               //指定目标路径
                shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);                                  //设置起始位置
                shortcut.WindowStyle = 1;                                                                       //设置运行方式，默认为常规窗口
                shortcut.Description = description;                                                             //设置备注
                shortcut.IconLocation = string.IsNullOrWhiteSpace(iconLocation) ? targetPath : iconLocation;    //设置图标路径
                shortcut.Save();                                                                                //保存快捷方式
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("设置开机启动失败！请检查是否有权限！异常信息：" + ex.Message + Environment.NewLine + 
                    "可能原因：路径 " + directory + " 没有写入权限！");
            }
            return false;
        }

        /// <summary>
        /// 获取指定文件夹下指定应用程序的快捷方式路径集合
        /// </summary>
        /// <param name="directory">文件夹</param>
        /// <param name="targetPath">目标应用程序路径</param>
        /// <returns>目标应用程序的快捷方式</returns>
        private List<string> GetQuickFromFolder(string directory, string targetPath)
        {
            List<string> tempStrs = new List<string>();
            tempStrs.Clear();
            string tempStr;
            string[] files = Directory.GetFiles(directory, "*.lnk");
            if (files == null || files.Length < 1)
            {
                return tempStrs;
            }
            for (int i = 0; i < files.Length; i++)
            {
                //files[i] = string.Format("{0}\\{1}", directory, files[i]);
                tempStr = GetAppPathFromQuick(files[i]);
                if (tempStr == targetPath)
                {
                    tempStrs.Add(files[i]);
                }
            }
            return tempStrs;
        }

        /// <summary>
        /// 获取快捷方式的目标文件路径-用于判断是否已经开启了自动启动
        /// </summary>
        /// <param name="shortcutPath"></param>
        /// <returns></returns>
        private string GetAppPathFromQuick(string shortcutPath)
        {
            //快捷方式文件的路径 = @"d:\Test.lnk";
            if (File.Exists(shortcutPath))
            {
                WshShell shell = new WshShell();
                IWshShortcut shortct = (IWshShortcut)shell.CreateShortcut(shortcutPath);
                //快捷方式文件指向的路径.Text = 当前快捷方式文件IWshShortcut类.TargetPath;
                //快捷方式文件指向的目标目录.Text = 当前快捷方式文件IWshShortcut类.WorkingDirectory;
                return shortct.TargetPath;
            }
            else
            {
                return "";
            }
        }

        /// <summary>
        /// 根据路径删除文件-用于取消自启时从计算机自启目录删除程序的快捷方式
        /// </summary>
        /// <param name="path">路径</param>
        private void DeleteFile(string path)
        {
            FileAttributes attr = File.GetAttributes(path);
            if (attr == FileAttributes.Directory)
            {
                Directory.Delete(path, true);
            }
            else
            {
                System.IO.File.Delete(path);
            }
        }

        /// <summary>
        /// 在桌面上创建快捷方式-如果需要可以调用
        /// </summary>
        /// <param name="desktopPath">桌面地址</param>
        /// <param name="appPath">应用路径</param>
        private void CreateDesktopQuick(string desktopPath = "", string quickName = "", string appPath = "")
        {
            List<string> shortcutPaths = GetQuickFromFolder(desktopPath, appPath);
            //如果没有则创建
            if (shortcutPaths.Count < 1)
            {
                CreateShortcut(desktopPath, quickName, appPath, "文本监听工具");
            }
        }
        #endregion

        #region 连接测试相关方法
        /// <summary>
        /// 打开数据库配置窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfigControlFrom(object sender, EventArgs e)
        {
            ConfigControl configControl = new ConfigControl();
            configControl.ShowDialog();
        }

        /// <summary>
        /// 连接数据库测试
        /// </summary>
        /// <returns></returns>
        private bool DBConn()
        {
            string ErrorMsg;
            string WCFAddress = OrBitHelper.ReadIniData("DBConfig", "WCF", "");

            //获取地址
            if (WCFAddress == "")
            {
                MessageBox.Show("请配置数据库信息!");
                return false;
            }

            DBHelper db = new DBHelper();

            //测试连接
            if (db.SelectDataSet("select 1 ", out ErrorMsg, WCFAddress) == null)
            {
                MessageBox.Show("连接数据库失败! /(ㄒoㄒ)/~~" + Environment.NewLine + "信息:" + ErrorMsg);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 连接服务器测试
        /// </summary>
        /// <returns></returns>
        private bool ServerConn()
        {
            string server = OrBitHelper.ReadIniData("ServerConfig", "server", "");
            string userName = OrBitHelper.ReadIniData("ServerConfig", "userName", "");
            string passWord = OrBitHelper.ReadIniData("ServerConfig", "passWord", "");

            string serverFolder = @"\\" + server + @"\新建文件夹\MES\Test";
            if (!ServerHelper.ConnectState(serverFolder, userName, passWord, out string ErrorMsg))
            {
                MessageBox.Show("连接服务器失败! /(ㄒoㄒ)/~~" + serverFolder + "|" + userName + "|"  + passWord + Environment.NewLine + "信息:" + ErrorMsg);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 连接服务器测试
        /// </summary>
        /// <returns></returns>
        private bool FtpConn()
        {
            string server = OrBitHelper.ReadIniData("FtpConfig", "server", "");

            if (!FtpHelper.ConnectState(server, out string ErrorMsg))
            {
                MessageBox.Show("连接FTP服务器失败! /(ㄒoㄒ)/~~" + Environment.NewLine + "信息:" + ErrorMsg);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 测试连接按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TestDBConn(object sender, EventArgs e)
        {
            if (ServerConn() && DBConn() && FtpConn())
            {
                MessageBox.Show("数据库、服务器、FTP服务器连接成功！o(*￣▽￣*)ブ");
            }
        }

        #endregion

        #region 其他方法
        /// <summary>
        /// 清空监听框信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClearListeningInfo(object sender, EventArgs e)
        {
            textBox5.Text = "";
        }

        /// <summary>
        /// 托盘图标双击显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void NotifyIcon_DoubleClick(object sender, EventArgs e)
        {
            Visible = true;
            NotifyIcon.Visible = false;
        }

        /// <summary>
        /// 获取当前系统信息
        /// </summary>
        /// <returns></returns>
        private int GetPhicnalInfo()
        {
            string MachineName = Environment.MachineName;
            textBox5.Text += MachineName + "\r\n";
            ManagementClass osClass = new ManagementClass("Win32_Processor");//后面几种可以试一下，会有意外的收获//Win32_PhysicalMemory/Win32_Keyboard/Win32_ComputerSystem/Win32_OperatingSystem
            foreach (ManagementObject obj in osClass.GetInstances())
            {
                PropertyDataCollection pdc = obj.Properties;
                foreach (PropertyData pd in pdc)
                {
                    textBox5.Text += string.Format("{0}： {1}{2}", pd.Name, pd.Value, "\r\n");
                }
            }
            return 0;
        }
        /// <summary>
        /// 开启敏感配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheatCode(object sender, KeyEventArgs e)
        {
            // 上 38 下 40 左 37 右 39 B  66 A  65
            // 38 38 40 40 37 39 37 39 66 65 66 65
            string cheatCodeString = "383840403739373966656665";
            inputKey += e.KeyValue.ToString();
            if (cheatCodeString.StartsWith(inputKey))
            {
                if (e.KeyValue == 38)
                {
                    Location = new Point(Location.X, Location.Y - 20);
                    Thread.Sleep(100);
                    Location = new Point(Location.X, Location.Y + 20);
                }
                if (e.KeyValue == 40)
                {
                    Location = new Point(Location.X, Location.Y + 20);
                    Thread.Sleep(100);
                    Location = new Point(Location.X, Location.Y - 20);
                }
                if (e.KeyValue == 37)
                {
                    Location = new Point(Location.X - 20, Location.Y);
                    Thread.Sleep(100);
                    Location = new Point(Location.X + 20, Location.Y);
                }
                if (e.KeyValue == 39)
                {
                    Location = new Point(Location.X + 20, Location.Y);
                    Thread.Sleep(100);
                    Location = new Point(Location.X - 20, Location.Y);
                }
                if (e.KeyValue == 66)
                {
                    Location = new Point(Location.X + 20, Location.Y + 20);
                    Size = new Size(Size.Width - 40, Size.Height - 40);
                    Thread.Sleep(100);
                    Location = new Point(Location.X - 20, Location.Y - 20);
                    Size = new Size(Size.Width + 40, Size.Height + 40);
                }
                if (e.KeyValue == 65)
                {
                    Location = new Point(Location.X - 20, Location.Y - 20);
                    Size = new Size(Size.Width + 40, Size.Height + 40);
                    Thread.Sleep(100);
                    Location = new Point(Location.X + 20, Location.Y + 20);
                    Size = new Size(Size.Width - 40, Size.Height - 40);
                }
            }
            else
            {
                inputKey = "";
            }

            if (inputKey == cheatCodeString)
            {

                // do something...
            }
        }

        /// <summary>
        /// 窗体尺寸变化自适应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileListening_SizeChanged(object sender, EventArgs e)
        {
            if (autoAdaptSize != null)
            {
                autoAdaptSize.FormSizeChanged();
            }
        }
        #endregion
    }
}
