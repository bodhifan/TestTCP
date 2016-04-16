using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using log4net;
using Common.Log;
using System.Threading;

namespace Common.Utility
{
    public class KeyboardUtility
    {
        // 键盘值与驱动代码的映射
        Dictionary<string, int> mapping = new Dictionary<string, int>() {
            { "[",311}, { "]",312},  {"'",411}, { "esc",100}, {"-" ,211},{"=" ,212},
            { "1",201}, { "2",202},{ "3",203},{ "4",204},{ "5",205},{ "6",206},{ "7",207},{ "8",208},{ "9",209},{ "0",210},
            { "q",301}, { "w",302},{ "e",303},{ "r",304},{ "t",305},{ "y",306},{ "u",307},{ "i",308},{ "o",309},{ "p",310},
            { "a",401}, { "s",402},{ "d",403},{ "f",404},{ "g",405},{ "h",406},{ "j",407},{ "k",408},{ "l",409},{";",410},
            { "z",501}, { "x",502},{ "c",503},{ "v",504},{ "b",505},{ "n",506},{ "m",507},{ ",",508},{ ".",509},{ "/",510}
        };

        // 需要加SHIFT才能敲出来的键
        Dictionary<char, char> key2keyMap = new Dictionary<char, char>() { {'_','-' }, {':',';' },{'#','3' }, { '?','/'}, { '%','5'}, { '+','='}
            };

        // 键盘上的特殊按键对应的code
        Dictionary<string, int> keyStr2Codemap = new Dictionary<string, int>() { { "{enter}", 815 } };
        
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
            // string dllfile = System.AppDomain.CurrentDomain.BaseDirectory + "DD64.dll";
            // string dllfile = @"C:\迅雷下载\dd62562\dd62562\DD64调用.dll";
            string dllfile = ReadDataFromReg();
            LogHelper.Info("dllfile lcoation:" + dllfile);
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

        public void SendKey(string key,int maxMills = 500)
        {
            if(keyStr2Codemap.ContainsKey(key))
            {
                int ch = keyStr2Codemap[key];
                SendKey(ch);
                return;
            }
            char[] list = key.ToCharArray();
            foreach(char ch in list)
            {
                // 如果是大写先按下SHIFT按键
                bool isShiftDown = false;
                char chKey = ch;
                if (ch >= 65 && ch <= 90)
                {
                    isShiftDown = true;
                    chKey =(char)(chKey + 32);
                }
                // 需要辅助按键SHIFT
                if(key2keyMap.ContainsKey(chKey))
                {
                    isShiftDown = true;
                    chKey = key2keyMap[chKey];
                }
                if (isShiftDown)
                    dd.key(500, 1);
                 SendKey(chKey);

                if (isShiftDown)
                    dd.key(500, 2);

                int sleepMills = new Random().Next(maxMills);
                if(sleepMills < 100)
                   sleepMills = 100;
                if(maxMills != 0)
                    Thread.Sleep(sleepMills);
            }
        }

        public void SendKey(char key)
        {
            string str = key.ToString();

            if (!mapping.ContainsKey(str))
            {
                LogHelper.Error(string.Format("未包含按键{0}对应的代码", str));
                return;
            }
            int code = mapping[str];
            dd.key(code, 1);
            dd.key(code, 2);
        }

        public void SendKey(int code,int type)
        {
            dd.key(code, type);
        }

        public void SendKey(int code)
        {
            dd.key(code, 1);
            dd.key(code, 2);
        }

        public void SendButton(MouseButton code)
        {
            int cc = (int)code;
            LogHelper.Debug("点击鼠标按钮：" + cc);
            dd.btn(cc);
            Thread.Sleep(500);
            dd.btn(cc*2);
        }

        public void MouseMove(int x, int y)
        {
            LogHelper.Debug(string.Format("鼠标移动X:{0} Y:{1}",x,y));
            dd.mov(x, y);
        }



    }

    public enum MouseButton
    {
        Left = 1,
        Right = 4
    }
}
