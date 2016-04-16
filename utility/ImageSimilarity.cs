using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace Common.Utility
{
    /// <summary>
    /// 图像相似度计算
    /// </summary>
    public class ImageSimilarity
    {
        /// <summary>
        /// 将图像imageFile转为256*256大小
        /// </summary>
        /// <param name="imageFile"></param>
        /// <param name="newImageFile"></param>
        /// <returns></returns>
        public static Bitmap Resize(string imageFile, string newImageFile)
        {

            Image img = Image.FromFile(imageFile);

            Bitmap imgOutput = new Bitmap(img, 256, 256);

            imgOutput.Save(newImageFile, System.Drawing.Imaging.ImageFormat.Jpeg);

            imgOutput.Dispose();

            return (Bitmap)Image.FromFile(newImageFile);

        }


        public static int[] GetHisogram(Bitmap img)
        {

            BitmapData data = img.LockBits(new System.Drawing.Rectangle(0, 0, img.Width, img.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            int[] histogram = new int[256];

            unsafe
            {

                byte* ptr = (byte*)data.Scan0;

                int remain = data.Stride - data.Width * 3;

                for (int i = 0; i < histogram.Length; i++)

                    histogram[i] = 0;

                for (int i = 0; i < data.Height; i++)

                {

                    for (int j = 0; j < data.Width; j++)

                    {

                        int mean = ptr[0] + ptr[1] + ptr[2];

                        mean /= 3;

                        histogram[mean]++;

                        ptr += 3;

                    }

                    ptr += remain;

                }

            }

            img.UnlockBits(data);
            return histogram;

        }

        private static float GetAbs(int firstNum, int secondNum)
        {

            float abs = Math.Abs((float)firstNum - (float)secondNum);

            float result = Math.Max(firstNum, secondNum);

            if (result == 0)

                result = 1;

            return abs / result;

        }

        public static float GetResult(int[] firstNum, Bitmap bitmap2)
        {
            int[] secondNum = GetHisogram(bitmap2);
            return GetResult(firstNum, secondNum);
        }

        public static float GetResult(Bitmap bitmap1, Bitmap bitmap2)
        {
            int[] firstNum = GetHisogram(bitmap1);
            int[] secondNum = GetHisogram(bitmap2);
            return GetResult(firstNum, secondNum);
        }


        public static float GetResult(int[] firstNum, int[] scondNum)
        {

            if (firstNum.Length != scondNum.Length)
            {

                return 0;

            }
            else
            {

                float result = 0;

                int j = firstNum.Length;

                for (int i = 0; i < j; i++)

                {

                    result += 1 - GetAbs(firstNum[i], scondNum[i]);

                }

                return result / j;

            }

        }
    }
}
