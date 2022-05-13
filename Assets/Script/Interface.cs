using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace Assets.Script
{
    public partial class Interface : Form
    {
        public Interface()
        {
            InitializeComponent();
            listView1.View = View.Details;
            this.listView1.Columns.Add("文件列表", -2, HorizontalAlignment.Left);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && File.Exists(textBox1.Text))
            {
                System.Diagnostics.Process.Start(textBox1.Text);
                listView1.Items.Add(textBox1.Text);
            }
            else
            {
                MessageBox.Show("文件不存在");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = false;   //是否允许多选
            dialog.Title = "请选择要处理的文件";  //窗口title
            dialog.Filter = "所有文件(*.*) | *.* ";   //可选择的文件类型
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string path = dialog.FileName;  //获取文件路径
                textBox1.Text = path;
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        //移除
        private void button3_Click(object sender, EventArgs e)
        {
            int index = listView1.SelectedItems[0].Index;
            listView1.Items.RemoveAt(index);
        }

        //listView双击功能
        private void listView1_DoubleClick(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && File.Exists(textBox1.Text))
            {
                System.Diagnostics.Process.Start(textBox1.Text);
            }
            else
            {
                int index = listView1.SelectedItems[0].Index;
                listView1.Items.RemoveAt(index);
                MessageBox.Show("文件不存在");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string[] strArr = new string[1];//参数列表
            strArr[0] = textBox2.Text;
            RunPythonScript(listView1.SelectedItems[0].Text, "", strArr);
        }

        //运行python脚本
        public static void RunPythonScript(string sArgName, string args = "", params string[] teps)
        {
            Process p = new Process();
            string path = sArgName;//(因为我没放debug下，所以直接写的绝对路径,替换掉上面的路径了)
            p.StartInfo.FileName = @"python.exe";//没有配环境变量的话，可以像我这样写python.exe的绝对路径。如果配了，直接写"python.exe"即可
            string sArguments = path;
            string cmd = fileRoute(path);
            cmd = cmd + "cmd.xls";
            sArguments += " " + cmd;
            foreach (string sigstr in teps)
            {
                sArguments += " " + sigstr;//传递参数
            }

            sArguments += " " + args;

            p.StartInfo.Arguments = sArguments;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardError = true;
            p.StartInfo.CreateNoWindow = true;

            p.Start();
            p.BeginOutputReadLine();
            //p.OutputDataReceived += new DataReceivedEventHandler(p_OutputDataReceived);
            //Console.ReadLine();
            p.WaitForExit();
        }
        //输出python的print消息
        static void p_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Data))
            {
                AppendText(e.Data + Environment.NewLine);
            }
        }
        public static void AppendText(string text)
        {
            MessageBox.Show(text);     //此处在控制台输出.py文件print的结果
        }
        private void button5_Click(object sender, EventArgs e)
        {
            string route = fileRoute(listView1.SelectedItems[0].Text);
            System.Diagnostics.Process.Start(route);
        }
        public static string fileRoute(string file)
        {
            string route = file;
            for (int i = file.Length - 1; route[i] != '\\'; i--)
            {
                route = file.Remove(i);
            }
            return route;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (textBox1.Text != "" && File.Exists(textBox1.Text))
            {
                listView1.Items.Add(textBox1.Text);
            }
            else
            {
                MessageBox.Show("文件不存在");
            }
        }
    }
}
