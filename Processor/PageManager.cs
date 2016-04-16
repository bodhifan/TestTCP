using Common.ImageUtils;
using Common.Log;
using Common.Management;
using Common.Mulation;
using Common.Utility;
using System;
using System.Threading;

namespace Common.Processor
{
    public class PageManager
    {
        private UIAccessProcessor processor;

        private string currentPhone = "";

        private string sucFilePath = @"成功.txt";
        private string failFilePath = @"失败.txt";
        private string phoneFilePath = @"手机号.txt";

        private Mulator mulator = null;
        Account curAccount;

        AccountFactory accountFactory;
        public PageManager()
        {
            processor = new UIAccessProcessor(null);
            accountFactory = new AccountFactory(phoneFilePath);
            processor.AddHandler(RestartMulator);
            processor.AddHandler(ClearAllAtFirst);
            processor.AddHandler(GotoRegisterPage);
            processor.AddHandler(GotoInputPhonePage);
            processor.AddHandler(GotoImageCodePage);
            processor.AddHandler(GotoSMSPage);
            processor.AddHandler(GotoAccountPage);
            processor.AddHandler(GotoUserNamePage);
            processor.AddHandler(GotoLastConfirm);

            processor.SetExcuteTimeOuts(Constants.WATTING_TIME+1000*20);

            // 当某一个元素的值超过 5次则执行 ClearAllAtFirst
            processor.EndExcute("ClearAllAtFirst", 5);

            processor.MakeCycle();

        }

        public void Start()
        {
            processor.Excute();
        }


        bool RestartMulator(Object sender, Object param)
        {
            MulatorManager mulatorManager = new MulatorManager();
            mulatorManager.Stop(mulator);

            mulator = mulatorManager.Setup("MEmu", true);
            return true;

        }

        bool ClearAllAtFirst(Object sender, Object param)
        {
            if(!accountFactory.HasNext())
            {
                LogHelper.Error("没有手机号");
                processor.Stop();
                return true;
            }

            curAccount = accountFactory.Next();

            Mulator.ClearBrowser();

            Thread.Sleep(1000);

            Mulator.ActiveHotkey();
            Thread.Sleep(1000);

            Mulator.SendCmm("1");
            return true;
        }

        bool GotoRegisterPage(Object sender, Object param)
        {
            curAccount.isSuc = false;

            Mulator.SendCmm("1");
            Mulator.SendCmm("2");
            Mulator.SendKey("https://login.m.taobao.com/login.htm?tpl_redirect_url=https%3A%2F%2Fh5.m.taobao.com%2Fmlapp%2Fmytaobao.html%23mlapp-mytaobao%5B1%5D",false,100);
            Mulator.SendCmm("{enter}");

            Thread.Sleep(500);
            Mulator.SendCmm("[");
            Mulator.SendCmm("]");

            Mulator.SendCmm("3");
            return true;
        }

        bool GotoInputPhonePage(Object sender, Object param)
        {
            if (!ImageValidation.IsStayAtPage(ImageSimilarityManager.INPUT_PHONE_PAGE))
            {
                curAccount.reason = "未能重新跳转到注册页面";
                processor.GotoHanlderUnit("GotoLastConfirm", 1000);
                return true;
            }

            // 打开手机输入框
            Mulator.SendCmm("4");
           
            Mulator.SendKey(curAccount.phone, true, 500);

            // 点击确定
            Mulator.SendCmm("5");

            // 一个次数限制
            int times = 3;
            while (times-- > 0)
            {
                LogHelper.Info("计算...");
                bool flag = ImageValidation.IsStayAtPage(ImageSimilarityManager.INPUT_PHONE_PAGE);
                if (!flag) break;
                LogHelper.Info("跳转到图片验证码失败，重新拖动验证条");
                Mulator.SendCmm("5");
            }

            if (times <= 0)
            {
                string msg = "[手机号码接收短信达到上限]OR[网络异常]...更换手机号码再试";
                LogHelper.Info(msg);
                curAccount.reason = msg;
                processor.GotoHanlderUnit("GotoLastConfirm", 1000);
                return true;
            }

            // 验证是否在接收短信处
            if (ImageValidation.IsStayAtPage(ImageSimilarityManager.INPUT_SMS_CODE))
            {
                LogHelper.Info("已经发送短信，跳转到短信接收处理器");
                processor.GotoHanlderUnit("GotoSMSPage", 1000);
            }
            return true;
        }

        bool GotoImageCodePage(Object sender, Object param)
        {
            ImageValidation.DumpImageCode();
            Thread.Sleep(1000);
            string type = ConfigFileManager.Instance().GetConfigFile().ReadString("图片验证码", "IMAGE_TYPE", "RK");
            string name = ConfigFileManager.Instance().GetConfigFile().ReadString("图片验证码", "IMAGE_NAME", "ruokuai2spy");
            string pwd = ConfigFileManager.Instance().GetConfigFile().ReadString("图片验证码", "IMAGE_PWD", "spy2ruokuai");
            CheckImage checkImage = CheckImageFactory.GetCheckImage(type, name, pwd);
            string imageCode = checkImage.RecognizeByCodeTypeAndPath(@"dump.png", 3040);
            LogHelper.Info("============================");
            LogHelper.Info("图片验证码：" + imageCode);
            Mulator.LostFocus();

            // 打开图片验证码输入框
            Mulator.SendCmm("6");
            Mulator.SendKey(imageCode, true);
            Thread.Sleep(1000);

            // 点击确定
            Mulator.SendCmm("7");

            // 检查是否还在改页面，如果在，说明图片验证码失败
            if (ImageValidation.IsStayAtPage(ImageSimilarityManager.IMAGE_CODE))
            {
                Mulator.LostFocus();
               
                // 删除原来的验证码
                Mulator.SendCmm("-");
                return false;
            }

            return true;
        }
        bool GotoSMSPage(Object sender, Object param)
        {
            // 判断是否回到了手机号码输入页面
            if (ImageValidation.IsStayAtPage(ImageSimilarityManager.INPUT_PHONE_PAGE))
            {
                string msg = "[手机号码接收短信达到上限]OR[网络异常]...更换手机号码再试";
                // 杀死浏览器进程，重头开始
                LogHelper.Info(msg);
                curAccount.reason = msg;
                processor.GotoHanlderUnit("GotoLastConfirm", 1000);
                return true;
            }

            string smsCode = SmsReceiver.TryGetSmsCode(currentPhone);

            if (smsCode == null)
            {
                string msg = "获取短信验证码失败";
                LogHelper.Info(msg);
                curAccount.reason = msg;
                processor.GotoHanlderUnit("GotoLastConfirm", 1000);
                return true;
            }

            LogHelper.Info("获取短信验证码:" + smsCode);

            Mulator.SendCmm("8");
            // 获取短信验证码
            Mulator.SendKey(smsCode, true);
            Thread.Sleep(1000);
            Mulator.SendCmm("9");

            return true;
        }

        bool GotoAccountPage(Object sender, Object param)
        {
            // 判断是否回到了手机号码输入页面
            if (ImageValidation.IsStayAtPage(ImageSimilarityManager.INPUT_PHONE_PAGE))
            {
                string msg = "短信验证码错误";
                // 杀死浏览器进程，重头开始
                LogHelper.Info(msg);
                curAccount.reason = msg;
                processor.GotoHanlderUnit("GotoLastConfirm", 1000);
                return true;
            }

            Mulator.SendCmm("a");

            curAccount.password = "flag123456";

            LogHelper.Info("密码为：" + curAccount.password);
            // 输入密码
            Mulator.SendKey(curAccount.password,true);

            // 确定
            Mulator.SendCmm("b");

            return true;
        }

        bool GotoLastConfirm(Object sender, Object param)
        {
            // 都算对
            if (curAccount.isSuc)
            {
                LogHelper.Info(string.Format("注册成功：{0}{1}{2}", curAccount.userName, curAccount.password, currentPhone));
                AppendToSucFile(curAccount.ToString());
            }
            else
            {
                LogHelper.Error(string.Format("注册失败：{0}{1}{2}", curAccount.userName, curAccount.password, currentPhone));
                AppendToFailFile(curAccount.ToString());
            }
            return true;
        }

        bool GotoUserNamePage(Object sender, Object param)
        {
            // 判断是否回到了手机号码输入页面
            if (!ImageValidation.IsStayAtPage(ImageSimilarityManager.INPUT_USERNAME))
            {
                LogHelper.Info("等待进入输入账户名页面...");
                return false;
            }

            Mulator.SendCmm("c");

            // 输入密码
            curAccount.userName = RandomGenerator.getRandomMixEnAndNum(5, 7);
            LogHelper.Info("账户名为：" + curAccount.userName);
            Mulator.SendKey(curAccount.userName,true);

            // 确定
            Mulator.SendCmm("d");


            // 跳出账户名只能设置一次的提示,点击确定
            Mulator.SendCmm("e");

            Thread.Sleep(2000);
            // 判断是否还在改页面，如果在说明账户名重复
            if (ImageValidation.IsStayAtPage(ImageSimilarityManager.INPUT_USERNAME))
            {
                LogHelper.Info("账户名" + curAccount.userName + "重复");
                Mulator.SendCmm("c");

                // 点击删除按钮
                Mulator.SendCmm("=");
                return false;
            }

            curAccount.isSuc = true;
            return true;
        }

        bool MyLogHandler(Object sender, string param)
        {

            LogHelper.Debug(param);
            return true;
        }

        void AppendToSucFile(string content)
        {
            FileUtils.WriteContentToFile(content, sucFilePath);
        }

        void AppendToFailFile(string content)
        {
            FileUtils.WriteContentToFile(content, failFilePath);
        }
    }
}
