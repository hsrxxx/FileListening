
namespace FileListening
{
    partial class ConfigControl
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox6 = new System.Windows.Forms.TextBox();
            this.textBox7 = new System.Windows.Forms.TextBox();
            this.textBox8 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.textBox9 = new System.Windows.Forms.TextBox();
            this.textBox10 = new System.Windows.Forms.TextBox();
            this.textBox11 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.iniConfigList = new System.Windows.Forms.TabControl();
            this.DBConfigPage = new System.Windows.Forms.TabPage();
            this.ServerConfigPage = new System.Windows.Forms.TabPage();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.iniConfigList.SuspendLayout();
            this.DBConfigPage.SuspendLayout();
            this.ServerConfigPage.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 19);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "数据库地址";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 15);
            this.label2.TabIndex = 1;
            this.label2.Text = "数据库名称";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(20, 96);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "数据库用户";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(20, 133);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(82, 15);
            this.label4.TabIndex = 3;
            this.label4.Text = "数据库密码";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(9, 171);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(98, 15);
            this.label5.TabIndex = 4;
            this.label5.Text = "连接超时(秒)";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(124, 17);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(186, 25);
            this.textBox1.TabIndex = 5;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(124, 54);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(186, 25);
            this.textBox2.TabIndex = 6;
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(124, 92);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(186, 25);
            this.textBox3.TabIndex = 7;
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(124, 131);
            this.textBox4.Name = "textBox4";
            this.textBox4.PasswordChar = '*';
            this.textBox4.Size = new System.Drawing.Size(186, 25);
            this.textBox4.TabIndex = 8;
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(124, 168);
            this.textBox5.Name = "textBox5";
            this.textBox5.Size = new System.Drawing.Size(186, 25);
            this.textBox5.TabIndex = 9;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(11, 244);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(100, 40);
            this.button1.TabIndex = 10;
            this.button1.Text = "保存设置";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.MouseClick += new System.Windows.Forms.MouseEventHandler(this.SaveDBConfig);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(247, 244);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(100, 40);
            this.button2.TabIndex = 11;
            this.button2.Text = "退出";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.MouseClick += new System.Windows.Forms.MouseEventHandler(this.Exit);
            // 
            // textBox6
            // 
            this.textBox6.Location = new System.Drawing.Point(124, 92);
            this.textBox6.Name = "textBox6";
            this.textBox6.PasswordChar = '*';
            this.textBox6.Size = new System.Drawing.Size(186, 25);
            this.textBox6.TabIndex = 17;
            // 
            // textBox7
            // 
            this.textBox7.Location = new System.Drawing.Point(124, 54);
            this.textBox7.Name = "textBox7";
            this.textBox7.Size = new System.Drawing.Size(186, 25);
            this.textBox7.TabIndex = 16;
            // 
            // textBox8
            // 
            this.textBox8.Location = new System.Drawing.Point(124, 17);
            this.textBox8.Name = "textBox8";
            this.textBox8.Size = new System.Drawing.Size(186, 25);
            this.textBox8.TabIndex = 15;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(66, 96);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 15);
            this.label6.TabIndex = 14;
            this.label6.Text = "密码";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(51, 57);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(52, 15);
            this.label7.TabIndex = 13;
            this.label7.Text = "用户名";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(20, 19);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(82, 15);
            this.label8.TabIndex = 12;
            this.label8.Text = "服务器地址";
            // 
            // textBox9
            // 
            this.textBox9.Location = new System.Drawing.Point(124, 92);
            this.textBox9.Name = "textBox9";
            this.textBox9.PasswordChar = '*';
            this.textBox9.Size = new System.Drawing.Size(186, 25);
            this.textBox9.TabIndex = 23;
            // 
            // textBox10
            // 
            this.textBox10.Location = new System.Drawing.Point(124, 54);
            this.textBox10.Name = "textBox10";
            this.textBox10.Size = new System.Drawing.Size(186, 25);
            this.textBox10.TabIndex = 22;
            // 
            // textBox11
            // 
            this.textBox11.Location = new System.Drawing.Point(124, 17);
            this.textBox11.Name = "textBox11";
            this.textBox11.Size = new System.Drawing.Size(186, 25);
            this.textBox11.TabIndex = 21;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(66, 96);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(37, 15);
            this.label9.TabIndex = 20;
            this.label9.Text = "密码";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(51, 57);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(52, 15);
            this.label10.TabIndex = 19;
            this.label10.Text = "用户名";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(39, 17);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(61, 15);
            this.label11.TabIndex = 18;
            this.label11.Text = "Ftp地址";
            // 
            // iniConfigList
            // 
            this.iniConfigList.Controls.Add(this.DBConfigPage);
            this.iniConfigList.Controls.Add(this.ServerConfigPage);
            this.iniConfigList.Controls.Add(this.tabPage1);
            this.iniConfigList.ItemSize = new System.Drawing.Size(80, 20);
            this.iniConfigList.Location = new System.Drawing.Point(11, 7);
            this.iniConfigList.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.iniConfigList.Multiline = true;
            this.iniConfigList.Name = "iniConfigList";
            this.iniConfigList.SelectedIndex = 0;
            this.iniConfigList.Size = new System.Drawing.Size(340, 231);
            this.iniConfigList.TabIndex = 24;
            // 
            // DBConfigPage
            // 
            this.DBConfigPage.Controls.Add(this.textBox1);
            this.DBConfigPage.Controls.Add(this.label1);
            this.DBConfigPage.Controls.Add(this.label2);
            this.DBConfigPage.Controls.Add(this.label3);
            this.DBConfigPage.Controls.Add(this.label4);
            this.DBConfigPage.Controls.Add(this.label5);
            this.DBConfigPage.Controls.Add(this.textBox2);
            this.DBConfigPage.Controls.Add(this.textBox3);
            this.DBConfigPage.Controls.Add(this.textBox4);
            this.DBConfigPage.Controls.Add(this.textBox5);
            this.DBConfigPage.Location = new System.Drawing.Point(4, 24);
            this.DBConfigPage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.DBConfigPage.Name = "DBConfigPage";
            this.DBConfigPage.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.DBConfigPage.Size = new System.Drawing.Size(332, 203);
            this.DBConfigPage.TabIndex = 0;
            this.DBConfigPage.Text = "数据库配置";
            this.DBConfigPage.UseVisualStyleBackColor = true;
            // 
            // ServerConfigPage
            // 
            this.ServerConfigPage.Controls.Add(this.label8);
            this.ServerConfigPage.Controls.Add(this.label7);
            this.ServerConfigPage.Controls.Add(this.label6);
            this.ServerConfigPage.Controls.Add(this.textBox8);
            this.ServerConfigPage.Controls.Add(this.textBox7);
            this.ServerConfigPage.Controls.Add(this.textBox6);
            this.ServerConfigPage.Location = new System.Drawing.Point(4, 24);
            this.ServerConfigPage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ServerConfigPage.Name = "ServerConfigPage";
            this.ServerConfigPage.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.ServerConfigPage.Size = new System.Drawing.Size(332, 203);
            this.ServerConfigPage.TabIndex = 1;
            this.ServerConfigPage.Text = "服务器配置";
            this.ServerConfigPage.UseVisualStyleBackColor = true;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.textBox9);
            this.tabPage1.Controls.Add(this.label11);
            this.tabPage1.Controls.Add(this.textBox10);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.textBox11);
            this.tabPage1.Controls.Add(this.label9);
            this.tabPage1.Location = new System.Drawing.Point(4, 24);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.tabPage1.Size = new System.Drawing.Size(332, 203);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "FTP服务";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // ConfigControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(360, 291);
            this.Controls.Add(this.iniConfigList);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "ConfigControl";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "高级配置";
            this.Load += new System.EventHandler(this.DBConfig_Load);
            this.iniConfigList.ResumeLayout(false);
            this.DBConfigPage.ResumeLayout(false);
            this.DBConfigPage.PerformLayout();
            this.ServerConfigPage.ResumeLayout(false);
            this.ServerConfigPage.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.TextBox textBox5;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox6;
        private System.Windows.Forms.TextBox textBox7;
        private System.Windows.Forms.TextBox textBox8;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBox9;
        private System.Windows.Forms.TextBox textBox10;
        private System.Windows.Forms.TextBox textBox11;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TabControl iniConfigList;
        private System.Windows.Forms.TabPage DBConfigPage;
        private System.Windows.Forms.TabPage ServerConfigPage;
        private System.Windows.Forms.TabPage tabPage1;
    }
}