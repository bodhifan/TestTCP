using Common.Log;
using Common.Management;
using Common.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Win32Api;

namespace Common.Mulation
{
    /// <summary>
    /// 通过图片进行验证
    /// </summary>
    public class ImageValidation
    {

        /// <summary>
        /// 判断当前流程是否停留在 name 页面
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsStayAtPage(string name)
        {
            Bitmap bitmap = GetTitleImage();
            Bitmap reszieMap = new Bitmap(bitmap,256,256);
            bool flag = IsSimilar(name, reszieMap, 0.2f);

            bitmap.Save("dump.png");
            LogHelper.Debug(string.Format("{0} 是否相似 {1}", name,flag));
            return flag;
        }

        /// <summary>
        /// 判断快捷键是否开启
        /// </summary>
        /// <returns></returns>
        public static bool IsHotKeyStarting()
        {
            Bitmap hotkey = GetHotKeyImage();
            Bitmap resizeMap = new Bitmap(hotkey, 64, 64);
            hotkey.Save("hotkey.png");
            return IsSimilar(ImageSimilarityManager.ACTIVE_HOT_KEY, resizeMap,0.1f);
        }
        private static bool IsSimilar(string name, Bitmap bitmap,float gapValue = 0.05f)
        {
            float result = ImageSimilarityManager.Instance().Compare(name, bitmap);
            LogHelper.Debug(string.Format("{0}相似度计算结果：{1}",name,result));
            float gap = Math.Abs((float)(result - 1.0f));
            LogHelper.Debug(string.Format("{0}相似度计算差值：{1}",name, gap));
            if (gap < gapValue)
            {
                return true;
            }
            return false;
        }



        public static Bitmap GetHotKeyImage()
        {
            IntPtr handler = Mulator.ActiveMulator();
            int offsetX = ConfigFileManager.Instance().GetConfigFile().ReadInteger("HOTKEY验证码", "OFFSET_L", 640);
            int offsetY = ConfigFileManager.Instance().GetConfigFile().ReadInteger("HOTKEY验证码", "OFFSET_T", 85);
            int offsetW = ConfigFileManager.Instance().GetConfigFile().ReadInteger("HOTKEY验证码", "OFFSET_R", -11);
            int offsetH = ConfigFileManager.Instance().GetConfigFile().ReadInteger("HOTKEY验证码", "OFFSET_B", -1037);

            return DumpImage(offsetX, offsetY, offsetW, offsetH);

        }



        /// <summary>
        /// 输出图像
        /// </summary>
        /// <param name="offsetX"></param>
        /// <param name="offsetY"></param>
        /// <param name="offsetW"></param>
        /// <param name="offsetH"></param>
        /// <returns></returns>
        public static Bitmap DumpImage(int offsetX, int offsetY, int offsetW, int offsetH)
        {
            LogHelper.Debug(string.Format("OFFSET_X:{0} OFFSET_Y:{1} OFFSET_R:{2} OFFSET_B:{3}", offsetX, offsetY, offsetW, offsetH));
            RECT rect = GetRaidoRect();

            if (rect.Left < 0 || rect.Top < 0 || rect.Right < 0 || rect.Bottom < 0)
            {
                LogHelper.Error(string.Format("获取边框失败"));
                rect = GetRaidoRect();
            }
            int left = rect.Left + offsetX;
            int top = rect.Top + offsetY;

            int right = rect.Right + offsetW;
            int buttom = rect.Bottom + offsetH;

            int width = right - left;
            int height = buttom - top;

            LogHelper.Info(string.Format("LEFT:{0} TOP:{1} WIDTH:{2} HEIGHT:{3}", left, top, width, height));
            Bitmap bitmap = ImageUtility.CaptureScreen(left, top, width, height);

            return bitmap;

        }

        public static RECT GetRaidoRect()
        {
            IntPtr handler = Mulator.GetMonitorWindowHandler();
            return GetRaidoRect(handler);
        }

        public static RECT GetRaidoRect(IntPtr handler)
        {
            RECT rect = new RECT();
            Win32API.GetWindowRect(handler, ref rect);

            string radio1 = ConfigFileManager.Instance().GetConfigFile().ReadString("PIXELRADIO", "radio", "1.25");
            if (!radio1.Equals("1"))
            {
                float radio = (float)Convert.ToDouble(radio1);
                rect.Left = (int)(rect.Left * radio);
                rect.Right = (int)(rect.Right * radio);
                rect.Top = (int)(rect.Top * radio);
                rect.Bottom = (int)(rect.Bottom * radio);
            }
            LogHelper.Debug(string.Format("rect-left：{0} rect-top：{1} rect-right：{2} rect-bottom：{3}", rect.Left, rect.Top, rect.Right, rect.Bottom));
            return rect;
        }

        /// <summary>
        /// 导出图片验证码
        /// </summary>
        public static void DumpImageCode()
        {
            Mulator.ActiveMulator();
            int offsetX = ConfigFileManager.Instance().GetConfigFile().ReadInteger("图片验证码", "OFFSET_L", 470);
            int offsetY = ConfigFileManager.Instance().GetConfigFile().ReadInteger("图片验证码", "OFFSET_T", 173);
            int offsetW = ConfigFileManager.Instance().GetConfigFile().ReadInteger("图片验证码", "OFFSET_R", -95);
            int offsetH = ConfigFileManager.Instance().GetConfigFile().ReadInteger("图片验证码", "OFFSET_B", -919);

            Bitmap bitmap = DumpImage(offsetX, offsetY, offsetW, offsetH);
            bitmap.Save(@"dump.png");
        }
        /// <summary>
        /// 导出常规图像
        /// </summary>
        public static Bitmap GetTitleImage()
        {
            Mulator.ActiveMulator();
            int offsetX = ConfigFileManager.Instance().GetConfigFile().ReadInteger("TITLE图片", "OFFSET_L", 470);
            int offsetY = ConfigFileManager.Instance().GetConfigFile().ReadInteger("TITLE图片", "OFFSET_T", 173);
            int offsetW = ConfigFileManager.Instance().GetConfigFile().ReadInteger("TITLE图片", "OFFSET_R", -95);
            int offsetH = ConfigFileManager.Instance().GetConfigFile().ReadInteger("TITLE图片", "OFFSET_B", -919);
            return DumpImage(offsetX, offsetY, offsetW, offsetH);
        }
    }



}
