
namespace FileListening
{
    partial class FileListening
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FileListening));
            this.ListeningPathButton = new System.Windows.Forms.Button();
            this.listeningPathBox = new System.Windows.Forms.TextBox();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.filterBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.intervalBox = new System.Windows.Forms.TextBox();
            this.windowState = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.runingState = new System.Windows.Forms.ComboBox();
            this.ListeningSwitchButton = new System.Windows.Forms.Button();
            this.SaveConfigButton = new System.Windows.Forms.Button();
            this.Timer = new System.Windows.Forms.Timer(this.components);
            this.NotifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.offsetBox = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.fileType = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.DBConnButton = new System.Windows.Forms.Button();
            this.label9 = new System.Windows.Forms.Label();
            this.ConfigControlButton = new System.Windows.Forms.Button();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.BasicConfig = new System.Windows.Forms.TabPage();
            this.IsFileMove = new System.Windows.Forms.CheckBox();
            this.FileMovePathButton = new System.Windows.Forms.Button();
            this.FileMovePathBox = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.JutzeAOI = new System.Windows.Forms.TabPage();
            this.FullImagePathButton = new System.Windows.Forms.Button();
            this.FullImagePathBox = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.NGImagePathButton = new System.Windows.Forms.Button();
            this.NGImagePathBox = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.SinitTekSPI = new System.Windows.Forms.TabPage();
            this.ClearListeningInfoButton = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.HTGD = new System.Windows.Forms.TabPage();
            this.tabControl1.SuspendLayout();
            this.BasicConfig.SuspendLayout();
            this.JutzeAOI.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ListeningPathButton
            // 
            this.ListeningPathButton.Location = new System.Drawing.Point(390, 129);
            this.ListeningPathButton.Name = "ListeningPathButton";
            this.ListeningPathButton.Size = new System.Drawing.Size(100, 25);
            this.ListeningPathButton.TabIndex = 0;
            this.ListeningPathButton.Text = "选择路径";
            this.ListeningPathButton.UseVisualStyleBackColor = true;
            this.ListeningPathButton.Click += new System.EventHandler(this.ChoosePath);
            // 
            // listeningPathBox
            // 
            this.listeningPathBox.Location = new System.Drawing.Point(12, 129);
            this.listeningPathBox.Name = "listeningPathBox";
            this.listeningPathBox.Size = new System.Drawing.Size(372, 25);
            this.listeningPathBox.TabIndex = 2;
            // 
            // filterBox
            // 
            this.filterBox.Location = new System.Drawing.Point(12, 78);
            this.filterBox.Name = "filterBox";
            this.filterBox.Size = new System.Drawing.Size(120, 25);
            this.filterBox.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 111);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 15);
            this.label1.TabIndex = 4;
            this.label1.Text = "监听路径";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 15);
            this.label2.TabIndex = 5;
            this.label2.Text = "过滤方案";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 232);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 15);
            this.label3.TabIndex = 7;
            this.label3.Text = "监听信息";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(138, 59);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(113, 15);
            this.label4.TabIndex = 9;
            this.label4.Text = "监听间隔(毫秒)";
            // 
            // intervalBox
            // 
            this.intervalBox.Location = new System.Drawing.Point(138, 78);
            this.intervalBox.Name = "intervalBox";
            this.intervalBox.Size = new System.Drawing.Size(120, 25);
            this.intervalBox.TabIndex = 8;
            // 
            // windowState
            // 
            this.windowState.FormattingEnabled = true;
            this.windowState.Items.AddRange(new object[] {
            "N/A",
            "托盘",
            "最小化"});
            this.windowState.Location = new System.Drawing.Point(12, 30);
            this.windowState.Name = "windowState";
            this.windowState.Size = new System.Drawing.Size(120, 23);
            this.windowState.TabIndex = 12;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 12);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(67, 15);
            this.label6.TabIndex = 13;
            this.label6.Text = "窗体状态";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(138, 12);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(67, 15);
            this.label7.TabIndex = 14;
            this.label7.Text = "运行状态";
            // 
            // runingState
            // 
            this.runingState.FormattingEnabled = true;
            this.runingState.Items.AddRange(new object[] {
            "N/A",
            "开机自启"});
            this.runingState.Location = new System.Drawing.Point(138, 30);
            this.runingState.Name = "runingState";
            this.runingState.Size = new System.Drawing.Size(120, 23);
            this.runingState.TabIndex = 15;
            // 
            // ListeningSwitchButton
            // 
            this.ListeningSwitchButton.Location = new System.Drawing.Point(11, 292);
            this.ListeningSwitchButton.Name = "ListeningSwitchButton";
            this.ListeningSwitchButton.Size = new System.Drawing.Size(100, 249);
            this.ListeningSwitchButton.TabIndex = 17;
            this.ListeningSwitchButton.Text = "开启监听";
            this.ListeningSwitchButton.UseVisualStyleBackColor = true;
            this.ListeningSwitchButton.Click += new System.EventHandler(this.ListeningSwitch);
            // 
            // SaveConfigButton
            // 
            this.SaveConfigButton.Location = new System.Drawing.Point(11, 59);
            this.SaveConfigButton.Name = "SaveConfigButton";
            this.SaveConfigButton.Size = new System.Drawing.Size(100, 40);
            this.SaveConfigButton.TabIndex = 16;
            this.SaveConfigButton.Text = "保存配置";
            this.SaveConfigButton.UseVisualStyleBackColor = true;
            this.SaveConfigButton.Click += new System.EventHandler(this.SaveButton);
            // 
            // Timer
            // 
            this.Timer.Tick += new System.EventHandler(this.ListeningTimer);
            // 
            // NotifyIcon
            // 
            this.NotifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("NotifyIcon.Icon")));
            this.NotifyIcon.Text = "文本监听工具";
            this.NotifyIcon.Visible = true;
            this.NotifyIcon.DoubleClick += new System.EventHandler(this.NotifyIcon_DoubleClick);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(264, 59);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 15);
            this.label5.TabIndex = 20;
            this.label5.Text = "初始偏移";
            // 
            // offsetBox
            // 
            this.offsetBox.Location = new System.Drawing.Point(264, 78);
            this.offsetBox.Name = "offsetBox";
            this.offsetBox.Size = new System.Drawing.Size(120, 25);
            this.offsetBox.TabIndex = 19;
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(12, 255);
            this.textBox5.Multiline = true;
            this.textBox5.Name = "textBox5";
            this.textBox5.ReadOnly = true;
            this.textBox5.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.textBox5.Size = new System.Drawing.Size(618, 235);
            this.textBox5.TabIndex = 21;
            this.textBox5.WordWrap = false;
            // 
            // fileType
            // 
            this.fileType.FormattingEnabled = true;
            this.fileType.Items.AddRange(new object[] {
            "N/A",
            "AOI-JZ",
            "SPI-STK",
            "SPI-HTGD"});
            this.fileType.Location = new System.Drawing.Point(264, 30);
            this.fileType.Name = "fileType";
            this.fileType.Size = new System.Drawing.Size(120, 23);
            this.fileType.TabIndex = 23;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(264, 12);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(67, 15);
            this.label8.TabIndex = 22;
            this.label8.Text = "文件类型";
            // 
            // DBConnButton
            // 
            this.DBConnButton.Location = new System.Drawing.Point(11, 13);
            this.DBConnButton.Name = "DBConnButton";
            this.DBConnButton.Size = new System.Drawing.Size(100, 40);
            this.DBConnButton.TabIndex = 24;
            this.DBConnButton.Text = "测试连接";
            this.DBConnButton.UseVisualStyleBackColor = true;
            this.DBConnButton.Click += new System.EventHandler(this.TestDBConn);
            // 
            // label9
            // 
            this.label9.BackColor = System.Drawing.SystemColors.ButtonShadow;
            this.label9.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.label9.Location = new System.Drawing.Point(119, 13);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(2, 528);
            this.label9.TabIndex = 25;
            this.label9.Text = "label9";
            // 
            // ConfigControlButton
            // 
            this.ConfigControlButton.Location = new System.Drawing.Point(11, 105);
            this.ConfigControlButton.Name = "ConfigControlButton";
            this.ConfigControlButton.Size = new System.Drawing.Size(100, 40);
            this.ConfigControlButton.TabIndex = 26;
            this.ConfigControlButton.Text = "高级配置";
            this.ConfigControlButton.UseVisualStyleBackColor = true;
            this.ConfigControlButton.Click += new System.EventHandler(this.ConfigControlFrom);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.BasicConfig);
            this.tabControl1.Controls.Add(this.JutzeAOI);
            this.tabControl1.Controls.Add(this.SinitTekSPI);
            this.tabControl1.Controls.Add(this.HTGD);
            this.tabControl1.Location = new System.Drawing.Point(130, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(650, 529);
            this.tabControl1.TabIndex = 27;
            // 
            // BasicConfig
            // 
            this.BasicConfig.BackColor = System.Drawing.Color.White;
            this.BasicConfig.Controls.Add(this.textBox5);
            this.BasicConfig.Controls.Add(this.IsFileMove);
            this.BasicConfig.Controls.Add(this.ClearListeningInfoButton);
            this.BasicConfig.Controls.Add(this.FileMovePathButton);
            this.BasicConfig.Controls.Add(this.FileMovePathBox);
            this.BasicConfig.Controls.Add(this.label3);
            this.BasicConfig.Controls.Add(this.label12);
            this.BasicConfig.Controls.Add(this.ListeningPathButton);
            this.BasicConfig.Controls.Add(this.listeningPathBox);
            this.BasicConfig.Controls.Add(this.filterBox);
            this.BasicConfig.Controls.Add(this.fileType);
            this.BasicConfig.Controls.Add(this.label1);
            this.BasicConfig.Controls.Add(this.label8);
            this.BasicConfig.Controls.Add(this.label2);
            this.BasicConfig.Controls.Add(this.intervalBox);
            this.BasicConfig.Controls.Add(this.label5);
            this.BasicConfig.Controls.Add(this.label4);
            this.BasicConfig.Controls.Add(this.offsetBox);
            this.BasicConfig.Controls.Add(this.windowState);
            this.BasicConfig.Controls.Add(this.label6);
            this.BasicConfig.Controls.Add(this.label7);
            this.BasicConfig.Controls.Add(this.runingState);
            this.BasicConfig.Location = new System.Drawing.Point(4, 25);
            this.BasicConfig.Name = "BasicConfig";
            this.BasicConfig.Padding = new System.Windows.Forms.Padding(3);
            this.BasicConfig.Size = new System.Drawing.Size(642, 500);
            this.BasicConfig.TabIndex = 1;
            this.BasicConfig.Text = "基本设置";
            // 
            // IsFileMove
            // 
            this.IsFileMove.AutoSize = true;
            this.IsFileMove.Location = new System.Drawing.Point(526, 182);
            this.IsFileMove.Name = "IsFileMove";
            this.IsFileMove.Size = new System.Drawing.Size(89, 19);
            this.IsFileMove.TabIndex = 34;
            this.IsFileMove.Text = "文件转移";
            this.IsFileMove.UseVisualStyleBackColor = true;
            // 
            // FileMovePathButton
            // 
            this.FileMovePathButton.Location = new System.Drawing.Point(390, 178);
            this.FileMovePathButton.Name = "FileMovePathButton";
            this.FileMovePathButton.Size = new System.Drawing.Size(100, 25);
            this.FileMovePathButton.TabIndex = 31;
            this.FileMovePathButton.Text = "选择路径";
            this.FileMovePathButton.UseVisualStyleBackColor = true;
            this.FileMovePathButton.Click += new System.EventHandler(this.ChoosePath);
            // 
            // FileMovePathBox
            // 
            this.FileMovePathBox.Location = new System.Drawing.Point(12, 178);
            this.FileMovePathBox.Name = "FileMovePathBox";
            this.FileMovePathBox.Size = new System.Drawing.Size(372, 25);
            this.FileMovePathBox.TabIndex = 32;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(12, 160);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(97, 15);
            this.label12.TabIndex = 33;
            this.label12.Text = "文件转移路径";
            // 
            // JutzeAOI
            // 
            this.JutzeAOI.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.JutzeAOI.Controls.Add(this.FullImagePathButton);
            this.JutzeAOI.Controls.Add(this.FullImagePathBox);
            this.JutzeAOI.Controls.Add(this.label11);
            this.JutzeAOI.Controls.Add(this.NGImagePathButton);
            this.JutzeAOI.Controls.Add(this.NGImagePathBox);
            this.JutzeAOI.Controls.Add(this.label10);
            this.JutzeAOI.Location = new System.Drawing.Point(4, 25);
            this.JutzeAOI.Name = "JutzeAOI";
            this.JutzeAOI.Padding = new System.Windows.Forms.Padding(3);
            this.JutzeAOI.Size = new System.Drawing.Size(642, 500);
            this.JutzeAOI.TabIndex = 0;
            this.JutzeAOI.Text = "AOI-JZ";
            // 
            // FullImagePathButton
            // 
            this.FullImagePathButton.Location = new System.Drawing.Point(424, 79);
            this.FullImagePathButton.Name = "FullImagePathButton";
            this.FullImagePathButton.Size = new System.Drawing.Size(100, 25);
            this.FullImagePathButton.TabIndex = 8;
            this.FullImagePathButton.Text = "选择路径";
            this.FullImagePathButton.UseVisualStyleBackColor = true;
            this.FullImagePathButton.Click += new System.EventHandler(this.ChoosePath);
            // 
            // FullImagePathBox
            // 
            this.FullImagePathBox.Location = new System.Drawing.Point(12, 79);
            this.FullImagePathBox.Name = "FullImagePathBox";
            this.FullImagePathBox.Size = new System.Drawing.Size(392, 25);
            this.FullImagePathBox.TabIndex = 9;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(12, 61);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(97, 15);
            this.label11.TabIndex = 10;
            this.label11.Text = "整版图片路径";
            // 
            // NGImagePathButton
            // 
            this.NGImagePathButton.Location = new System.Drawing.Point(424, 30);
            this.NGImagePathButton.Name = "NGImagePathButton";
            this.NGImagePathButton.Size = new System.Drawing.Size(100, 25);
            this.NGImagePathButton.TabIndex = 5;
            this.NGImagePathButton.Text = "选择路径";
            this.NGImagePathButton.UseVisualStyleBackColor = true;
            this.NGImagePathButton.Click += new System.EventHandler(this.ChoosePath);
            // 
            // NGImagePathBox
            // 
            this.NGImagePathBox.Location = new System.Drawing.Point(12, 30);
            this.NGImagePathBox.Name = "NGImagePathBox";
            this.NGImagePathBox.Size = new System.Drawing.Size(392, 25);
            this.NGImagePathBox.TabIndex = 6;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(12, 12);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(83, 15);
            this.label10.TabIndex = 7;
            this.label10.Text = "NG图片路径";
            // 
            // SinitTekSPI
            // 
            this.SinitTekSPI.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.SinitTekSPI.Location = new System.Drawing.Point(4, 25);
            this.SinitTekSPI.Name = "SinitTekSPI";
            this.SinitTekSPI.Size = new System.Drawing.Size(642, 214);
            this.SinitTekSPI.TabIndex = 2;
            this.SinitTekSPI.Text = "SPI-STK";
            // 
            // ClearListeningInfoButton
            // 
            this.ClearListeningInfoButton.Location = new System.Drawing.Point(530, 222);
            this.ClearListeningInfoButton.Name = "ClearListeningInfoButton";
            this.ClearListeningInfoButton.Size = new System.Drawing.Size(100, 25);
            this.ClearListeningInfoButton.TabIndex = 24;
            this.ClearListeningInfoButton.Text = "清空信息";
            this.ClearListeningInfoButton.UseVisualStyleBackColor = true;
            this.ClearListeningInfoButton.Click += new System.EventHandler(this.ClearListeningInfo);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.tabControl1);
            this.panel1.Controls.Add(this.ConfigControlButton);
            this.panel1.Controls.Add(this.label9);
            this.panel1.Controls.Add(this.SaveConfigButton);
            this.panel1.Controls.Add(this.DBConnButton);
            this.panel1.Controls.Add(this.ListeningSwitchButton);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(782, 553);
            this.panel1.TabIndex = 28;
            // 
            // HTGD
            // 
            this.HTGD.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.HTGD.Location = new System.Drawing.Point(4, 25);
            this.HTGD.Name = "HTGD";
            this.HTGD.Padding = new System.Windows.Forms.Padding(3);
            this.HTGD.Size = new System.Drawing.Size(642, 214);
            this.HTGD.TabIndex = 3;
            this.HTGD.Text = "SPI-HTGD";
            // 
            // FileListening
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 553);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FileListening";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "文本监听工具";
            this.Load += new System.EventHandler(this.FileListening_Load);
            this.SizeChanged += new System.EventHandler(this.FileListening_SizeChanged);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.CheatCode);
            this.tabControl1.ResumeLayout(false);
            this.BasicConfig.ResumeLayout(false);
            this.BasicConfig.PerformLayout();
            this.JutzeAOI.ResumeLayout(false);
            this.JutzeAOI.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button ListeningPathButton;
        private System.Windows.Forms.TextBox listeningPathBox;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TextBox filterBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox intervalBox;
        private System.Windows.Forms.ComboBox windowState;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox runingState;
        private System.Windows.Forms.Button ListeningSwitchButton;
        private System.Windows.Forms.Button SaveConfigButton;
        private System.Windows.Forms.Timer Timer;
        private System.Windows.Forms.NotifyIcon NotifyIcon;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox offsetBox;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.ComboBox fileType;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button DBConnButton;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button ConfigControlButton;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage BasicConfig;
        private System.Windows.Forms.TabPage JutzeAOI;
        private System.Windows.Forms.TabPage SinitTekSPI;
        private System.Windows.Forms.Button FullImagePathButton;
        private System.Windows.Forms.TextBox FullImagePathBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Button NGImagePathButton;
        private System.Windows.Forms.TextBox NGImagePathBox;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button ClearListeningInfoButton;
        private System.Windows.Forms.CheckBox IsFileMove;
        private System.Windows.Forms.Button FileMovePathButton;
        private System.Windows.Forms.TextBox FileMovePathBox;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabPage HTGD;
    }
}