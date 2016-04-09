using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using log4net;

namespace Common.Utility
{
    public class ProcessUtility
    {
        static ILog log = LogManager.GetLogger(typeof(ProcessUtility));


        public static Dictionary<string, Process> nameProcessMap = new Dictionary<string, Process>();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public static void KillProcess(string name)
        {
            string killCmm = "tasklist | findstr \"" + name + "\"";
            string allListenning = ExecAndWait("cmd.exe", killCmm);
            string[] lines = allListenning.Split(new string[] { "\r\n" }, StringSplitOptions.None);

            for (int i = 1; i < lines.Length; i++)
            {
                if (lines[i].Contains(".exe"))
                {
                    string[] cols = lines[i].Split(new string[] { "\t", "      ", "     ", "    ", "   ", "  ", " " }, StringSplitOptions.None);

                    string pid = "";
                    int cnt = 0;
                    foreach (string str in cols)
                    {
                        if (str != "")
                        {
                            cnt++;
                            if (cnt == 2)
                            {
                                pid = str;
                            }
                        }
                    }
                    ExecAndWait("cmd.exe", "taskkill /f /pid " + pid);
                }
            }
        }


        /// <summary>
        /// 运行一个外部程序，并等待其退出
        /// </summary>
        /// <param name="exePath"></param>
        /// <param name="cmdLines"></param>
        /// <returns></returns>
        public static string ExecAndWait(string exePath, string cmdLines)
        {
            log.Debug("执行命令：" + cmdLines);
            Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = exePath;
            process.StartInfo.UseShellExecute = false;
        //    process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            process.StandardInput.WriteLine(cmdLines);
            process.StandardInput.WriteLine("exit");
            process.StandardInput.AutoFlush = true;
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process.Close();

            log.Debug(output);

            return output;
        }


        /// <summary>
        /// 运行一个外部程序
        /// </summary>
        /// <param name="exePath"></param>
        /// <param name="cmdLines"></param>
        /// <returns></returns>
        public static void ExecAync(string exePath, string cmdLines,string name)
        {
            Thread thread = new Thread(new AyncSetupProcess(exePath, cmdLines,name).Setup);
            thread.Start();
        }

        /// <summary>
        /// 运行一个外部程序
        /// </summary>
        /// <param name="exePath"></param>
        /// <param name="cmdLines"></param>
        /// <returns></returns>
        public static void Exec(string exePath, string cmdLines,string name="default")
        {
            Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = exePath;
            process.StartInfo.Arguments = cmdLines;
            process.Start();

            nameProcessMap.Add(name, process);
        }

        /// <summary>
        /// 执行外部程序，并监听其输出
        /// </summary>
        /// <param name="exePath"></param>
        /// <param name="cmdLines"></param>
        /// <param name="receviedMsgHandler"></param>
        /// <param name="errorMsgHandler"></param>
        /// <param name="exitHandler"></param>
        /// <returns></returns>
        public static Process Exec(string exePath, string[] cmdLines,DataReceivedEventHandler receviedMsgHandler, DataReceivedEventHandler errorMsgHandler,EventHandler exitHandler)
        {
            Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = exePath;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            process.BeginOutputReadLine();
            foreach (string cmdline in cmdLines)
            {
                process.StandardInput.WriteLine(cmdline);
            }

            process.EnableRaisingEvents = true;

            if (receviedMsgHandler != null)
            {
                process.OutputDataReceived += new DataReceivedEventHandler(receviedMsgHandler);
            }
            if (errorMsgHandler != null)
            {
                process.ErrorDataReceived += new DataReceivedEventHandler(errorMsgHandler);
            }
            if (exitHandler != null)
            {
                process.Exited += new EventHandler(exitHandler);
            }
            return process;
        }
    }

    class AyncSetupProcess
    {
        private string exePath;
        private string cmdLines;
        private string name;
        public AyncSetupProcess(string exePath, string cmdLines,string name)
        {
            this.exePath = exePath;
            this.cmdLines = cmdLines;
            this.name = name;
        }
        public void Setup()
        {
            ProcessUtility.Exec(exePath, cmdLines,name);
        }
    } 
}
