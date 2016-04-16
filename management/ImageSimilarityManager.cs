using Common.ImageUtils;
using Common.Log;
using Common.Utility;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Common.Management
{

    /// <summary>
    /// 图像相似度管理
    /// </summary>
    public class ImageSimilarityManager
    {
        // key->value: 图像名称->文件名称
        Dictionary<string, string> name2File = new Dictionary<string, string>() {
            { "active_hot_key", "activinghotkey.png" },
            { "input_phone", "input_phone.png" },
            { "image_code", "image_code.png" },
            { "input_smscode", "input_smscode.png" },
            { "input_password", "input_password.png" },
            { "input_username", "input_username.png" },
            { "last_confirm", "last_confirm.png" }};

        // 开启热键 input_username
        public static string ACTIVE_HOT_KEY = "active_hot_key";

        float[] hotKeyBuffer;

        // 输入手机号码页面
        public static string INPUT_PHONE_PAGE = "input_phone";

        // 图片验证码页面
        public static string IMAGE_CODE = "image_code";

        // 输入短信验证码页面
        public static string INPUT_SMS_CODE = "input_smscode";

        // 输入账户密码
        public static string INPUT_PASSWORD = "input_password";

        // 输入账户名
        public static string INPUT_USERNAME = "input_username";

        // 最后的页面
        public static string LAST_CONFIRM = "last_confirm";

        private static ImageSimilarityManager instance = null;
        public static ImageSimilarityManager Instance()
        {
            if (null == instance)
                instance = new ImageSimilarityManager();
            return instance;
        }
        //缓存已知图像直方图数据
        Dictionary<string, ImageData> image2Data = new Dictionary<string, ImageData>();
        string IMAGE_PATH = AppDomain.CurrentDomain.BaseDirectory+ @"images\";
        private ImageSimilarityManager()
        {
            LoadImage();
        }

        /// <summary>
        /// 加载所有已知的图像
        /// </summary>
        private void LoadImage()
        {
            Dictionary<string, string>.Enumerator itor = name2File.GetEnumerator();
            while(itor.MoveNext())
            {
                Bitmap bitmap = new Bitmap(IMAGE_PATH + itor.Current.Value);
                ImageData activinghotkeyData = new ImageData();

                // 对热键进行颜色特征的相似度处理
                if (itor.Current.Key.Equals("active_hot_key"))
                {
                    Bitmap resizeMap = new Bitmap(bitmap, 64, 64);
                    activinghotkeyData.colorData = CalSimilarityForImage.GetFeature(resizeMap);
                }
                else
                {
                    Bitmap resizeMap = new Bitmap(bitmap, 256, 256);
                    activinghotkeyData.data = ImageSimilarity.GetHisogram(resizeMap);
                }
                image2Data.Add(itor.Current.Key, activinghotkeyData);

            }

        }

        /// <summary>
        /// 计算target 与 缓存中name的图像的相似度
        /// </summary>
        /// <param name="name"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public float Compare(string name, Bitmap target)
        {
            if(!image2Data.ContainsKey(name))
            {
                string msg = "缓存中未包含" + name + "的图像数据";
                LogHelper.Error(msg);
                throw new Exception(msg);
            }

            float rtn;
            if (name.Equals("active_hot_key"))
                rtn= CalSimilarityForImage.GetSimilarity(image2Data[name].colorData, target);
            else
                rtn = ImageSimilarity.GetResult(image2Data[name].data, target);

            return rtn;
        }
    }
    class ImageData
    {
       public int[] data;
       public float[] colorData;
    }
}
