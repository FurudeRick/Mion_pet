using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Assets.Script
{
    class RunScript
    {
        public static void RunPythonScript()
        {
            Process p = new Process();
            p.StartInfo.FileName = @"D:\mydocument\project\unity\desk_pet\Mion_pet\Build\PyRPA.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.CreateNoWindow = true;
            p.OutputDataReceived += new DataReceivedEventHandler(OutputHandler);
            p.ErrorDataReceived += new DataReceivedEventHandler(OutputHandler);
            p.Start();
            p.BeginOutputReadLine();
            p.WaitForExit();
            p.Close();
        }
        static void OutputHandler(object sendingProcess, DataReceivedEventArgs outLine)
        {
            Console.WriteLine(outLine.Data);
        }
    }
}
