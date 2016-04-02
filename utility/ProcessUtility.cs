using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Utility
{
    public class ProcessUtility
    {
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
        /// 
        /// </summary>
        /// <param name="exePath"></param>
        /// <param name="cmdLines"></param>
        /// <returns></returns>
        public static string ExecAndWait(string exePath, string cmdLines)
        {

            Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = exePath;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
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
            return output;
        }
    }
}
