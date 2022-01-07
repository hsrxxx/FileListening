using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using IWshRuntimeLibrary;
using System.Windows.Forms;
using File = System.IO.File;
using FileListening.Entity;

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
        /// 监听开关
        /// </summary>
        private bool isOpen = false;

        /// <summary>
        /// 文件内容偏移量
        /// </summary>
        private int offset = 0;

        /// <summary>
        /// 监听间隔
        /// </summary>
        private int interval = 0;

        /// <summary>
        /// 监听路径
        /// </summary>
        private string listeningPath = "";

        /// <summary>
        /// 过滤方式
        /// </summary>
        private string filter = "";

        /// <summary>
        /// 快捷方式名称-任意自定义
        /// </summary>
        private const string QuickName = "FLTool";

        /// <summary>
        /// 自动获取系统自动启动目录
        /// </summary>
        private string SystemStartPath { get { return Environment.GetFolderPath(Environment.SpecialFolder.Startup); } }

        /// <summary>
        /// 自动获取程序完整路径
        /// </summary>
        private string appAllPath { get { return Process.GetCurrentProcess().MainModule.FileName; } }

        /// <summary>
        /// 自动获取桌面目录
        /// </summary>
        private string desktopPath { get { return Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory); } }

        public FileListening()
        {
            InitializeComponent();
        }

        // 加载初始化
        private void FileListening_Load(object sender, EventArgs e)
        {
            BackColor = Color.FromArgb(243, 243, 243);
            windowState.Items.Add("N/A");
            windowState.Items.Add("托盘");
            windowState.Items.Add("最小化");
            runingState.Items.Add("N/A");
            runingState.Items.Add("开机自启");
            fileType.Items.Add("N/A");
            fileType.Items.Add("炬子AOI");
            fileType.Items.Add("斯泰克SPI");
            listeningPathBox.Text = OrBitHelper.ReadIniData("basic", "listeningPath", "");
            filterBox.Text = OrBitHelper.ReadIniData("basic", "filter", "");
            intervalBox.Text = OrBitHelper.ReadIniData("basic", "interval", "");
            offsetBox.Text = OrBitHelper.ReadIniData("basic", "offset", "");
            windowState.Text = OrBitHelper.ReadIniData("basic", "windowState", "N/A");
            runingState.Text = OrBitHelper.ReadIniData("basic", "runingState", "N/A");
            fileType.Text = OrBitHelper.ReadIniData("basic", "fileType", "请选择类型!");
            NGImagePathBox.Text = OrBitHelper.ReadIniData("JutzeAOI", "NGImagePath", "");
            FullImagePathBox.Text = OrBitHelper.ReadIniData("JutzeAOI", "FullImagePath", "");
        }

        /// <summary>
        /// 窗体尺寸变化自适应
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FileListening_SizeChanged(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// 返回信息模板
        /// </summary>
        /// <param name="time">时间</param>
        /// <param name="type">操作</param>
        /// <param name="fileName">文件名</param>
        /// <param name="content">内容</param>
        /// <returns></returns>
        public string MessageTemp(string time, string type, string fileName, string content)
        {
            return $"时间[{time}] 操作[{type}] 文件[{fileName}] 内容[{content}]";
        }

        /// <summary>
        /// 选择监听文件路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChooseListeningPath(object sender, EventArgs e)
        {
            DialogResult dr = folderBrowserDialog1.ShowDialog();
            //获取所打开文件的文件名
            string filename = folderBrowserDialog1.SelectedPath;
            if (dr == DialogResult.OK && !string.IsNullOrEmpty(filename))
            {
                listeningPathBox.Text = filename;
            }
        }

        /// <summary>
        /// 选择NG图片文件路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChooseNGImagePath(object sender, EventArgs e)
        {
            DialogResult dr = folderBrowserDialog1.ShowDialog();
            //获取所打开文件的文件名
            string filename = folderBrowserDialog1.SelectedPath;
            if (dr == DialogResult.OK && !string.IsNullOrEmpty(filename))
            {
                NGImagePathBox.Text = filename;
            }
        }

        /// <summary>
        /// 选择整版图片文件路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChooseFullImagePath(object sender, EventArgs e)
        {
            DialogResult dr = folderBrowserDialog1.ShowDialog();
            //获取所打开文件的文件名
            string filename = folderBrowserDialog1.SelectedPath;
            if (dr == DialogResult.OK && !string.IsNullOrEmpty(filename))
            {
                FullImagePathBox.Text = filename;
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
        public bool SaveData()
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
            OrBitHelper.WriteIniData("JutzeAOI", "NGImagePath", NGImagePathBox.Text);
            OrBitHelper.WriteIniData("JutzeAOI", "FullImagePath", FullImagePathBox.Text);

            return true;
        }

        public bool CheckData()
        {
            if (!System.IO.Directory.Exists(listeningPathBox.Text))
            {
                MessageBox.Show("监听路径不存在，请重新选择！");
                return false;
            }
            if (!IsInt(intervalBox.Text))
            {
                MessageBox.Show("请输入正确的毫秒数！");
                return false;
            }
            if (!IsInt(offsetBox.Text))
            {
                MessageBox.Show("请输入正确的偏移量！");
                return false;
            }
            return true;
        }

        // 开启/关闭监听
        private void ListeningSwitch(object sender, EventArgs e)
        {
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
                filterBox.ReadOnly = true;
                intervalBox.ReadOnly = true;
                offsetBox.ReadOnly = true;
                windowState.Enabled = false;
                runingState.Enabled = false;
                fileType.Enabled = false;
                // 监听配置赋值
                listeningPath = listeningPathBox.Text;
                filter = filterBox.Text;
                interval = int.Parse(intervalBox.Text);
                offset = int.Parse(offsetBox.Text);

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
                initFileDic();
                // 开启监听
                watcherStart();
                // 开启定时器
                Timer.Interval = interval;
                Timer.Start();

                return;
            }
            else
            {
                // 结束监听
                watcherEnd();
                // 定时器停止
                Timer.Stop();
                // 界面变化
                ListeningSwitchButton.Text = "开启监听";
                ListeningSwitchButton.BackColor = Color.Gainsboro;
                listeningPathBox.ReadOnly = false;
                filterBox.ReadOnly = false;
                intervalBox.ReadOnly = false;
                offsetBox.ReadOnly = false;
                windowState.Enabled = true;
                runingState.Enabled = true;
                fileType.Enabled = true;
            }
        }

        /// <summary>
        /// 定时获取监听内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListeningTimer(object sender, EventArgs e)
        {
            while (logQueue.Count > 0)
            {
                DateTime StartTime = DateTime.Now;
                DateTime EndTime;
                double Spn;

                FileContent result = new FileContent();
                logQueue.TryDequeue(out result);
                // 获取 textBox5 的所有行数，没有则为 -1 
                int count = textBox5.Lines.GetUpperBound(0);
                // 输出追加的内容
                textBox5.Text +=MessageTemp(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), 
                                            result.ListeningType, 
                                            result.FileName,
                                            result.Content.Replace("\r\n", "")) + Environment.NewLine;
                // 每次添加内容自动滚到最底部
                textBox5.SelectionStart = textBox5.Text.Length; 
                textBox5.ScrollToCaret();
                // 处理数据
                InsertData(result.Content);

                EndTime = DateTime.Now;
                Spn = EndTime.Subtract(StartTime).TotalMilliseconds;
                textBox5.Text +="开始时间:" + StartTime.ToString("yyyy-MM-dd HH:mm:ss.fff") +
                                "结束时间:" + EndTime.ToString("yyyy-MM-dd HH:mm:ss.fff") +
                                "总耗时(毫秒):" + Spn.ToString() + Environment.NewLine;
            }
        }

        /// <summary>
        /// 将条码的过站数据插入数据库
        /// </summary>
        /// <param name="data">过站数据</param>
        public void InsertData(string data)
        {
            // string[] ItemList = Regex.Split(data, "\r\n", RegexOptions.IgnoreCase);
            string[] ItemList = data.Split(new char[] { ',' }, options: StringSplitOptions.RemoveEmptyEntries);
            if (fileType.Text == "炬子AOI")
            {
                JutzeAOI jutzeAOI = new JutzeAOI();
                jutzeAOI.BoardIndex = ItemList[0];
                jutzeAOI.GroupName = ItemList[1];
                jutzeAOI.BoardName = ItemList[2];
                jutzeAOI.LotNumber = ItemList[3];
                jutzeAOI.TagCode = ItemList[4];
                jutzeAOI.InspectionTime = ItemList[5];
                jutzeAOI.RepairedTime = ItemList[6];
                jutzeAOI.Operator = ItemList[7];
                jutzeAOI.Shift = ItemList[8];
                jutzeAOI.Result = ItemList[9];
                jutzeAOI.ReferenceName = ItemList[10];
                jutzeAOI.PartCode = ItemList[11];
                jutzeAOI.BlockNum = ItemList[12];
                jutzeAOI.Layer = ItemList[13];
                jutzeAOI.WindowNumber = ItemList[14];
                jutzeAOI.Method = ItemList[15];
                jutzeAOI.Sample = ItemList[16];
                jutzeAOI.NGName = ItemList[17];
                jutzeAOI.Image = ItemList[18];
                textBox5.Text += "json格式：" + Environment.NewLine + OrBitHelper.ObjectToJson(jutzeAOI) + Environment.NewLine;

                // 获取关联的整版图片路径
                foreach (string dir in Directory.GetDirectories(FullImagePathBox.Text, "*" + jutzeAOI.TagCode + "*", SearchOption.AllDirectories))
                {
                    foreach (string file in Directory.GetFiles(dir, "*.jpg", SearchOption.AllDirectories))
                    {
                        textBox5.Text += "整版图片路径：" + Environment.NewLine + file + Environment.NewLine;
                    }
                }

                // 获取关联的NG图片路径
                if (jutzeAOI.Image == "OK.jpg")
                {
                    textBox5.Text += "NG图片路径：" + Environment.NewLine + "无NG图片" + Environment.NewLine;
                }
                else
                {
                    foreach (string file in Directory.GetFiles(NGImagePathBox.Text, jutzeAOI.Image, SearchOption.AllDirectories))
                    {
                        textBox5.Text += "NG图片路径：" + Environment.NewLine + file + Environment.NewLine;
                    }
                }
            }
            else if (fileType.Text == "斯泰克SPI")
            {
                SinitTekSPI sinitTekSPI = new SinitTekSPI();
            }
        }

        /// <summary>
        /// 开启监听
        /// </summary>
        public void watcherStart()
        {
            watcher.Path = listeningPath;
            watcher.NotifyFilter = NotifyFilters.Attributes 
                                 | NotifyFilters.CreationTime 
                                 | NotifyFilters.DirectoryName 
                                 | NotifyFilters.FileName 
                                 | NotifyFilters.LastAccess
                                 | NotifyFilters.LastWrite 
                                 | NotifyFilters.Security 
                                 | NotifyFilters.Size;
            watcher.Filter = filter;
            watcher.Changed += new FileSystemEventHandler(watcher_Changed);
            watcher.Created += new FileSystemEventHandler(watcher_Created);
            watcher.Deleted += new FileSystemEventHandler(watcher_Deleted);
            watcher.Renamed += new RenamedEventHandler(watcher_Renamed);
            watcher.EnableRaisingEvents = true;
        }

        /// <summary>
        /// 停止监听
        /// </summary>
        public void watcherEnd()
        {
            watcher.Changed -= new FileSystemEventHandler(watcher_Changed);
            watcher.Created -= new FileSystemEventHandler(watcher_Created);
            watcher.Deleted -= new FileSystemEventHandler(watcher_Deleted);
            watcher.Renamed -= new RenamedEventHandler(watcher_Renamed);
            watcher.EnableRaisingEvents = false;
        }

        /// <summary>
        /// 对文件的内容偏移进行初始化
        /// </summary>
        public void initFileDic()
        {
            FileInfo[] files = new FileInfo[0];
            fileDic.Clear();
            if (string.IsNullOrEmpty(listeningPath) == false)
            {
                files = new DirectoryInfo(listeningPath).GetFiles(filter, SearchOption.TopDirectoryOnly);
            }
            for (int i = 0; i < files.Length; i++)
            {
                string filePath = files[i].FullName;
                if (offset == 0)
                {
                    // 记录文件路径和文件偏移
                    fileDic.Add(filePath, files[i].Length);
                }
                else
                {
                    // 如果默认偏移超出原文件则使用原文件大小偏移
                    if (offset > files[i].Length)//防止超出
                    {
                        fileDic.Add(filePath, files[i].Length);
                    }
                    else
                    {
                        fileDic.Add(filePath, offset);
                    }
                }
            }
        }

        /// <summary>
        /// 创建内存映射文件
        /// </summary>
        private static void AppendContentToCosole(long offset, string filePath, string name, string ListeningType)
        {
            string line = string.Empty;
            if (File.Exists(filePath))
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    if (fs.CanSeek)
                    {
                        fs.Seek(offset, SeekOrigin.Begin);
                        fileDic[filePath] = fs.Length;
                        //Encoding EncodingType;
                        //if (filePath.Contains(".csv"))
                        //{
                        //    EncodingType = Encoding.Default;
                        //}
                        //else
                        //{
                        //    EncodingType = Encoding.UTF8;
                        //}
                        if (offset < fs.Length)//防止期间文件删除后创建导致偏移变化
                        {
                            using (StreamReader sr = new StreamReader(fs, Encoding.Default))
                            {
                                string tmp = "";
                                // 有值则循环 ReadLine:读取log文件写入时需要在最后换行  ReadToEnd:全部一起读
                                while (string.IsNullOrEmpty(tmp = sr.ReadLine()) != true)
                                {
                                    FileContent _q = new FileContent() { FileName = name, Content = tmp, ListeningType = ListeningType };
                                    logQueue.Enqueue(_q);
                                }
                            }
                        }
                        else
                        {
                            FileContent _q = new FileContent() { FileName = name, Content = "文件内容被删除!", ListeningType = ListeningType };
                            logQueue.Enqueue(_q);
                        }
                    }
                    else
                    {
                        FileContent _q = new FileContent() { FileName = name, Content = "当前流不支持查找！", ListeningType = ListeningType };
                        logQueue.Enqueue(_q);
                    }
                }
            }
        }

        private static void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            string ListeningType = e.ChangeType.ToString();
            // 以防不存在字典中
            long temp = 0;
            fileDic.TryGetValue(e.FullPath, out temp);
            if (temp == 0)
            {
                fileDic[e.FullPath] = 0;
            }
            AppendContentToCosole(fileDic[e.FullPath], e.FullPath, e.Name, ListeningType);
        }

        private static void watcher_Created(object sender, FileSystemEventArgs e)
        {
            if (File.Exists(e.FullPath))
            {
                FileInfo _info = new FileInfo(e.FullPath);
                fileDic.Add(e.FullPath, _info.Length);
                FileContent _q = new FileContent() { FileName = e.Name, Content = e.FullPath, ListeningType = e.ChangeType.ToString() };
                logQueue.Enqueue(_q);
            }
            else
            {
                FileContent _q = new FileContent() { FileName = e.Name, Content = "创建后文件[" + e.FullPath + "]已不存在", ListeningType = e.ChangeType.ToString() };
                logQueue.Enqueue(_q);
            }
        }

        private static void watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            if (File.Exists(e.FullPath) == false)
            {
                fileDic.Remove(e.FullPath);
                FileContent _q = new FileContent() { FileName = e.Name, Content = e.FullPath, ListeningType = e.ChangeType.ToString() };
                logQueue.Enqueue(_q);
            }
            else
            {
                FileContent _q = new FileContent() { FileName = e.Name, Content = "删除后文件[" + e.FullPath + "]仍然存在", ListeningType = e.ChangeType.ToString() };
                logQueue.Enqueue(_q);
            }
        }

        private static void watcher_Renamed(object sender, RenamedEventArgs e)
        {
            if (File.Exists(e.FullPath))
            {
                FileInfo _info = new FileInfo(e.FullPath);
                fileDic.Add(e.FullPath, _info.Length);
                FileContent _q1 = new FileContent() { FileName = e.OldName + " -> " + e.Name, Content = e.OldFullPath + " -> " + e.FullPath, ListeningType = e.ChangeType.ToString() };
                logQueue.Enqueue(_q1);
            }
            else
            {
                FileContent _q = new FileContent() { FileName = e.Name, Content = "重命名后的文件[" + e.FullPath + "]已不存在", ListeningType = e.ChangeType.ToString() };
                logQueue.Enqueue(_q);
            }
        }

        private void notifyIcon1_DoubleClick(object sender, EventArgs e)
        {
            Visible = true;
            NotifyIcon.Visible = false;
        }

        // 带符号的小数点数值
        public static bool IsNumeric(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*[.]?\d*$");
        }

        // 整数数值
        public static bool IsInt(string value)
        {
            return Regex.IsMatch(value, @"^[+-]?\d*$");
        }

        // 带小数点数值
        public static bool IsUnsign(string value)
        {
            return Regex.IsMatch(value, @"^\d*[.]?\d*$");
        }

        /// <summary>
        /// 设置开机自动启动-只需要调用改方法就可以了参数里面的bool变量是控制开机启动的开关的
        /// </summary>
        /// <param name="onOff">自启开关</param>
        public bool SetMeAutoStart(bool onOff)
        {
            if (onOff)//开机启动
            {
                //获取启动路径应用程序快捷方式的路径集合
                List<string> shortcutPaths = GetQuickFromFolder(SystemStartPath, appAllPath);
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
                    if(!CreateShortcut(SystemStartPath, QuickName, appAllPath, "文本监听工具"))
                    {
                        return false;
                    }
                }
            }
            else//开机不启动
            {
                //获取启动路径应用程序快捷方式的路径集合
                List<string> shortcutPaths = GetQuickFromFolder(SystemStartPath, appAllPath);
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
            //CreateDesktopQuick(desktopPath, QuickName, appAllPath);
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
            string tempStr = null;
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
        public void CreateDesktopQuick(string desktopPath = "", string quickName = "", string appPath = "")
        {
            List<string> shortcutPaths = GetQuickFromFolder(desktopPath, appPath);
            //如果没有则创建
            if (shortcutPaths.Count < 1)
            {
                CreateShortcut(desktopPath, quickName, appPath, "文本监听工具");
            }
        }

        /// <summary>
        /// 连接数据库测试
        /// </summary>
        /// <returns></returns>
        public bool DBConn()
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
        /// 测试数据库连接是否联通
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TestDBConn(object sender, EventArgs e)
        {
            if (DBConn())
            {
                MessageBox.Show("数据库连接成功！o(*￣▽￣*)ブ");
            }
        }

        /// <summary>
        /// 打开数据库配置窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DBConfigFrom(object sender, EventArgs e)
        {
            DBConfig dBConfig = new DBConfig();
            dBConfig.ShowDialog();
            dBConfig.StartPosition = FormStartPosition.Manual;
        }

        /// <summary>
        /// 检查不同的监听文件类型的其他配置信息
        /// </summary>
        /// <returns></returns>
        public bool CheckTypeConfig()
        {
            if (fileType.Text == "N/A")
            {
                MessageBox.Show("请选择监听类型!");
                return false;
            }
            else if (fileType.Text == "炬子AOI")
            {
                if (NGImagePathBox.Text == "")
                {
                    MessageBox.Show("炬子AOI：请选择NG图片路径!");
                    return false;
                }
                if (FullImagePathBox.Text == "")
                {
                    MessageBox.Show("炬子AOI：请选择整版图片路径!");
                    return false;
                }
                if (!Directory.Exists(NGImagePathBox.Text))
                {
                    MessageBox.Show("炬子AOI：NG图片路径不存在或无效!");
                    return false;
                }
                if (!Directory.Exists(FullImagePathBox.Text))
                {
                    MessageBox.Show("炬子AOI：整版图片路径不存在或无效!");
                    return false;
                }
            }else if (fileType.Text == "斯泰克SPI")
            {
                MessageBox.Show("斯泰克SPI开发中...");
                return true;
            }
            return true;
        }
    }

    /// <summary>
    /// 记录文件新增内容
    /// </summary>
    public class FileContent
    {
        public string FileName { get; set; }
        public string Content { get; set; }
        public string ListeningType { get; set; }
    }
}
