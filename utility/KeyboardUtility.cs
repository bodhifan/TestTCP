using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Common.Log;

namespace Common.Utility
{
    public class KeyboardUtility
    {
        // 键盘值与驱动代码的映射
        Dictionary<string, int> mapping = new Dictionary<string, int>() {
            { "a",401}, { "b",505} };
        private static KeyboardUtility instance;
        private CDD dd;
        public static KeyboardUtility Instance()
        {
            if (instance == null)
            {
                instance = new KeyboardUtility();
            }
            return instance;
        }

        private KeyboardUtility()
        {
            dd = new CDD();
            string dllfile = ReadDataFromReg();
            System.IO.FileInfo fi = new System.IO.FileInfo(dllfile);
            if (!fi.Exists)
            {
                LogHelper.Error(string.Format("文件{0}不存在",dllfile));
                return;
            }
            int ret = dd.Load(dllfile);
            if (ret == -2) { LogHelper.Error("装载库时发生错误"); return; }
            if (ret == -1) { LogHelper.Error("取函数地址时发生错误"); return; }
            if (ret == 0) { LogHelper.Info("非增强模块"); }
        }

        private string ReadDataFromReg()
        {
            Microsoft.Win32.RegistryKey key;
            key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\\DD XOFT\\", false);
            if (key != null)
            {
                foreach (string vname in key.GetValueNames())
                {
                    if ("path" == vname.ToLower())
                    {
                        return key.GetValue(vname, "").ToString();
                    }
                }
            }
            return "";
        }

        public void SendKey(string key)
        {
            key = key.ToLower();
            if (!mapping.ContainsKey(key))
            {
                LogHelper.Error(string.Format("未包含按键{0}对应的代码", key));
                return;
            }
            int code = mapping[key];
            dd.key(code, 1);
            dd.key(code, 2);
        }



    }
}
