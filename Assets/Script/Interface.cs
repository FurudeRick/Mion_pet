﻿using System;
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
using System.Threading;

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
            Thread thread = new Thread(() => RunPythonScript(listView1.SelectedItems[0].Text));
            thread.Start();
        }

        //运行python脚本
        public void RunPythonScript(string sArgName)
        {
            System.Diagnostics.ProcessStartInfo start = new System.Diagnostics.ProcessStartInfo();
            //python interprater location
            start.FileName = @"python.exe";
            //argument with file name and input parameters
            string path = sArgName;
             
            string cmd = fileRoute(path);
            string folder = cmd; //文件夹位置
            cmd = cmd + "cmd.xls";
            start.Arguments = string.Format("{0} {1} {2} {3}", sArgName, cmd, textBox2.Text, folder);
            start.UseShellExecute = false;// Do not use OS shell
            start.CreateNoWindow = true; // We don't need new window
            start.RedirectStandardOutput = true;// Any output, generated by application will be redirected back
            start.RedirectStandardError = true; // Any error in standard output will be redirected back (for example exceptions)
            start.LoadUserProfile = true;
            using (System.Diagnostics.Process process = System.Diagnostics.Process.Start(start))
            {
                //using (StreamReader reader = process.StandardOutput)
                //{
                //    string stderr = process.StandardError.ReadToEnd(); // Here are the exceptions from our Python script
                //    string result = reader.ReadToEnd(); // Here is the result of StdOut(for example: print "test")
                //    Console.WriteLine("From System Diagnostics");
                //    Console.WriteLine("5 + 10 = {0}", result);
                //}
            }

            //Console.ReadLine();
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
