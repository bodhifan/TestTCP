using Common.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Common.Utility
{
    /// <summary>
    /// 短信验证码获取工具
    /// </summary>
   public class SmsReceiver
   {
        public static string TryGetSmsCode(string phone, int times = 5)
        {
            string url = ConfigFileManager.Instance().GetConfigFile().ReadString("注册信息", "SMS_URL", null);
            if (url == null || url.Equals(""))
            {
                string msg = "短信验证码URL为空";
                LogHelper.Error(msg);
                throw new Exception(msg);
            }
            url = string.Format(url, phone);
            LogHelper.Info("获取短信URL: " + url);

            Regex reg = new Regex("([\\d]{6}$)");
            string smsCode = null;
            int baseMills = 1000;

            int loop = times;
            while (loop-- > 0)
            {
                HttpTool tool = new HttpTool();
                smsCode = tool.HttpGet(url, null);
                LogHelper.Info("得到URL的Response：" + smsCode);
                Match m = reg.Match(smsCode);
                if (m.Success)
                {
                    smsCode = m.Groups[1].ToString();
                    break;
                }
                Thread.Sleep(baseMills);
                LogHelper.Info("未获取到短信验证码，重新获取...");
            }
            if(loop <= 0)
            {
                LogHelper.Info(string.Format("{0} 次获取短信验证码失败！",times));
                smsCode = null;
            }
            return smsCode;
        }
   }
}
