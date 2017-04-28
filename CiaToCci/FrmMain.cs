using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace CiaToCci
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var openFileDialog1 = new OpenFileDialog
            {
                Filter = @"(*.cia)|*.cia",
                FilterIndex = 2,
                RestoreDirectory = true
            };
            if (openFileDialog1.ShowDialog() != DialogResult.OK || string.IsNullOrEmpty(openFileDialog1.FileName)) return;
            var tool = Path.Combine(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Resouce\To ol\makerom-x86_64.exe"));
            var file = openFileDialog1.FileName;
            var dosCommand = $"{Path.GetPathRoot(tool)}\"{Path.GetFullPath(tool).Remove(0, 3)}\" -ciatocci \"{file}\"";
            try
            {
                Execute(dosCommand, 0);
                MessageBox.Show(@"操作成功！");
            }
            catch (Exception ex)
            {
#if DEBUG
                MessageBox.Show(ex.ToString());
#else
                MessageBox.Show(@"操作失败！");
#endif
            }
        }

        // 执行DOS命令，返回DOS命令的输出
        /// 
        /// dos命令
        /// 等待命令执行的时间（单位：毫秒），如果设定为0，则无限等待
        /// 返回输出，如果发生异常，返回空字符串
        public static string Execute(string dosCommand, int milliseconds)
        {
            var output = string.Empty;     //输出字符串
            if (string.IsNullOrEmpty(dosCommand)) return output;
            var process = new Process();     //创建进程对象
            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = "/C " + dosCommand,
                UseShellExecute = false,    //关闭Shell的使用
                RedirectStandardInput = false,  //重定向标准输入
                RedirectStandardOutput = true,  //重定向标准输出
                RedirectStandardError = true,   //重定向错误输出
                CreateNoWindow = true   //设置不显示窗口
            };
            //设定需要执行的命令
            //设定参数，其中的“/C”表示执行完命令后马上退出
            //不使用系统外壳程序启动
            //不重定向输入
            //重定向输出
            //不创建窗口
            process.StartInfo = startInfo;
            try
            {
                if (process.Start())       //开始进程
                {
                    if (milliseconds == 0)
                    {
                        process.WaitForExit();//这里无限等待进程结束
                    }
                    else
                    {
                        process.WaitForExit(milliseconds);  //这里等待进程结束，等待时间为指定的毫秒
                    }
                    output = process.StandardOutput.ReadToEnd();//读取进程的输出
                }
            }
            finally
            {
                process.Close();
            }
            return output;
        }
    }
}
