using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Common.ImageUtils
{
    public class CalSimilarityForImage
    {
        public static float[] GetFeature(Bitmap bmp2search)
        {
            HSV _hsv = new HSV();
            
            int pixSize = 0;
            pixSize = bmp2search.Height * bmp2search.Width * 3;

            float[] rgbvalues = new float[pixSize];

           rgbvalues = _hsv.readRGB("", bmp2search);
            _hsv.RGB2HSV(ref rgbvalues);//将图片的RGB值转化为HSV值

            float[] feature = new float[72];

            for (int i = 0; i < 72; i++)//初始化为0
            {
                feature[i] = 0;
            }
            _hsv.HSV_Featurize(rgbvalues, ref feature);//得到颜色特征值，存放在feature数组里

            return feature;
        }

        public static float GetSimilarity(float[] firstFeats, Bitmap bitmap2)
        {
            float[] second = CalSimilarityForImage.GetFeature(bitmap2);

            Similarity sim = new Similarity();
            float simValue = sim.similar(firstFeats, second);

            return simValue;
        }
    }
}
