using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace FileListening
{
    public partial class DBConfig : Form
    {
        public DBConfig()
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
        }

        private void SaveDBConfig(object sender, MouseEventArgs e)
        {
            string WCF = $"server={textBox1.Text};" +
                         $"database={textBox2.Text};" +
                         $"uid={textBox3.Text};" +
                         $"pwd={textBox4.Text};" +
                         $"TimeOut={textBox5.Text};";

            OrBitHelper.WriteIniData("DBConfig", "WCF", WCF);
            MessageBox.Show("保存成功!");
            Close();
        }

        private void Exit(object sender, MouseEventArgs e)
        {
            Close();
        }
    }
}
