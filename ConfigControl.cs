using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace FileListening
{
    public partial class ConfigControl : Form
    {
        public ConfigControl()
        {
            InitializeComponent();
        }

        private void DBConfig_Load(object sender, EventArgs e)
        {
            string WCF = OrBitHelper.ReadIniData("DBConfig", "WCF", "");
            if (!string.IsNullOrEmpty(WCF))
            {
                Dictionary<string, string> DBConfigDict = WCF.Split(new char[] { ';' }, options: StringSplitOptions.RemoveEmptyEntries)
                                                .ToDictionary(key => key.Split('=')[0], value => value.Split('=')[1]);

                textBox1.Text = DBConfigDict["server"];
                textBox2.Text = DBConfigDict["database"];
                textBox3.Text = DBConfigDict["uid"];
                textBox4.Text = DBConfigDict["pwd"];
                textBox5.Text = DBConfigDict["TimeOut"];
            }

            textBox8.Text = OrBitHelper.ReadIniData("ServerConfig", "server", "");
            textBox7.Text = OrBitHelper.ReadIniData("ServerConfig", "userName", "");
            textBox6.Text = OrBitHelper.ReadIniData("ServerConfig", "passWord", "");

            textBox11.Text = OrBitHelper.ReadIniData("FtpConfig", "server", "");
            textBox10.Text = OrBitHelper.ReadIniData("FtpConfig", "userName", "");
            textBox9.Text  = OrBitHelper.ReadIniData("FtpConfig", "passWord", "");
        }

        private void SaveDBConfig(object sender, MouseEventArgs e)
        {
            string WCF = $"server={textBox1.Text};" +
                         $"database={textBox2.Text};" +
                         $"uid={textBox3.Text};" +
                         $"pwd={textBox4.Text};" +
                         $"TimeOut={textBox5.Text};";

            OrBitHelper.WriteIniData("DBConfig", "WCF", WCF);


            OrBitHelper.WriteIniData("ServerConfig", "server", textBox8.Text);
            OrBitHelper.WriteIniData("ServerConfig", "userName", textBox7.Text);
            OrBitHelper.WriteIniData("ServerConfig", "passWord", textBox6.Text);

            OrBitHelper.WriteIniData("FtpConfig", "server", textBox11.Text);
            OrBitHelper.WriteIniData("FtpConfig", "userName", textBox10.Text);
            OrBitHelper.WriteIniData("FtpConfig", "passWord", textBox9.Text);
            MessageBox.Show("保存成功!");
            Close();
        }

        private void Exit(object sender, MouseEventArgs e)
        {
            Close();
        }

    }
}
