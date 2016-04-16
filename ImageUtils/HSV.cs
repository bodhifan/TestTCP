using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Data;

namespace Common.ImageUtils
{
    public class HSV
    {

        //HSV 类 用于：
        //1.获得图片的RGB值；
        //2.将RGB值转化为HSV值；
        //3.计算每个像素点的颜色特征值
        public float[] readRGB(string picFilePath, Bitmap mybmp)
        {
            //获取RGB首地址

            //获取图像的BitmapData对像 
            BitmapData data = mybmp.LockBits(new Rectangle(0, 0, mybmp.Width, mybmp.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);

            //Scan0相当于图片的第一行的首地址，将其赋值给ptr
            IntPtr ptr = data.Scan0;
            int bytes = mybmp.Width * mybmp.Height * 3;//总共有Width * Height 个像素点，每个像素点需要3个值来描述其RGB值

            //循环处理
            byte[] source = new byte[bytes];
            float[] rgbvalues = new float[bytes];

            // 将图片RGB值存入byte型数组中
            System.Runtime.InteropServices.Marshal.Copy(ptr, source, 0, bytes);

            for (int i = 0; i < bytes; i++)//将RGB信息从byte型数组转移到float型数组中
            {
                rgbvalues[i] = source[i];
            }

            mybmp.UnlockBits(data);//解除锁定
            return rgbvalues;
        }

        public void RGB2HSV(ref float[] rgbValues)//将图片数据由RGB空间描述，转化为HSV空间描述
        {
            //转化
            float r;
            float g;
            float b;

            float h = 0;
            float s = 0;
            float v = 0;

            for (int i = 0; i < rgbValues.GetLength(0) - 2; i += 3)//循环处理，每次从rgbValues数组中取出三个值（B,G,R），将其转化为(H,S,V)
            {
                b = rgbValues[i];
                g = rgbValues[i + 1];
                r = rgbValues[i + 2];

                Unit_RGB u_RGB = new Unit_RGB(b, g, r);
                Unit_HSV u_HSV = new Unit_HSV(h, s, v);

                u_HSV = Caculate_HSV(u_RGB);//Caculate_HSV 用于完成转化

                h = u_HSV.getH();
                s = u_HSV.getS();
                v = u_HSV.getV();

                rgbValues[i] = h;
                rgbValues[i + 1] = s;
                rgbValues[i + 2] = v;//把原来的三元组(B,G,R)替换为新的三元组(H,S,V)

            }
        }
        private Unit_HSV Caculate_HSV(Unit_RGB rgb)
        {
            //计算
            float H = 0;
            float S;
            float V;

            float _max;
            float _min;
            float delt;

            float r;
            float g;
            float b;

            r = rgb.getR() / 255;
            g = rgb.getG() / 255;
            b = rgb.getB() / 255;

            _max = max(b, g, r);
            _min = min(b, g, r);
            delt = _max - _min;

            //求V
            V = _max;

            if (delt != 0)
            {
                //求S
                S = delt / _max;
            }
            else
            {
                // r = g = b = 0 // s = 0, v is undefined
                S = 0;
                H = 0;

                Unit_HSV hsv0 = new Unit_HSV(H, S, V);
                return hsv0;
            }

            //求H
            if (r == _max)
            {
                H = (g - b) / (_max - _min);
            }
            if (g == _max)
            {
                H = 2 + (b - r) / (_max - _min);
            }
            if (b == _max)
            {
                H = 4 + (r - g) / (_max - _min);
            }

            H = H * 60;
            if (H < 0)
            {
                H = H + 360;
            }

            Unit_HSV hsv = new Unit_HSV(H, S, V);
            return hsv;//计算完毕后，返回一个包含(H, S, V)信息的元组
        }

        private float max(float a, float b, float c)//求最大值
        {
            if (a > b && a > c)
                return a;
            else if (b > c)
                return b;
            else
                return c;
        }
        private float min(float a, float b, float c)//求最小值
        {
            if (a < b && a < c)
                return a;
            else if (b < c)
                return b;
            else
                return c;
        }

        public float Caculate_feature(Unit_HSV _hsv)//计算HSV空间中的一个像素点的颜色特征值
        {
            float h, s, v;
            h = _hsv.getH();
            //MessageBox.Show("H = " + Convert.ToString(h)+"\r\n");
            s = _hsv.getS();
            v = _hsv.getV();

            float res1, res2, res3;
            res1 = rankH(h);
            res2 = rankS(s);
            res3 = rankV(v);

            float I_value;
            float Qs = 3, Qv = 3;//S,V的量化级数（共分为多少级?）
            //HSV空间中的一个像素点的颜色特征值：I_value = Qs * Qv * H +Qv *S + V;
            I_value = Qs * Qv * res1 + Qv * res2 + res3;
            return I_value;
        }
        private float rankH(float h)//对H值进行分级
        {
            float result = 0;
            if (h > 315 && h <= 360 || h >= 0 && h <= 20)
            {
                result = 0;
                return result;
            }
            if (h > 20 && h <= 40)
            {
                result = 1;
                return result;
            }
            if (h > 40 && h <= 75)
            {
                result = 2;
                return result;
            }
            if (h > 75 && h <= 155)
            {
                result = 3;
                return result;
            }
            if (h > 155 && h <= 190)
            {
                result = 4;
                return result;
            }
            if (h > 190 && h <= 270)
            {
                result = 5;
                return result;
            }
            if (h > 270 && h <= 295)
            {
                result = 6;
                return result;
            }
            if (h > 295 && h <= 315)
            {
                result = 7;
                return result;
            }
            else
            {
                //MessageBox.Show("rankH = " + Convert.ToString(h) +"\r\n");
                MessageBox.Show("H 值有误！");
                return result;
            }

        }
        private float rankS(float s)//对S值进行分级
        {
            float result = 0;
            if (s >= 0 && s <= 0.2)
            {
                result = 0;
                return result;
            }
            if (s > 0.2 && s <= 0.7)
            {
                result = 1;
                return result;
            }
            if (s > 0.7 && s <= 1)
            {
                result = 2;
                return result;
            }
            else
            {
                MessageBox.Show("S 值有误！");
                return result;
            }

        }
        private float rankV(float v)//对V值进行分级
        {
            float result = 0;
            if (v >= 0 && v <= 0.2)
            {
                result = 0;
                return result;
            }
            if (v > 0.2 && v <= 0.7)
            {
                result = 1;
                return result;
            }
            if (v > 0.7 && v <= 1)
            {
                result = 2;
                return result;
            }
            else
            {
                MessageBox.Show("S 值有误！");
                return result;
            }
        }
        public void HSV_Featurize(float[] hsvValues, ref float[] counter)//计算整张图片的颜色特征值，存入一个72维的数组中
        {
            float h, s, v;
            float feature = 0;
            int totalPix = hsvValues.GetLength(0) / 3;//图片共有totalPix个像素点

            for (int i = 0; i < hsvValues.GetLength(0) - 2; i += 3)
            {
                h = hsvValues[i];
                s = hsvValues[i + 1];
                v = hsvValues[i + 2];

                Unit_HSV _hsv = new Unit_HSV(h, s, v);//利用读到的H,S,V值，创建一个Unit_HSV对象，用于计算其颜色特征值
                feature = Caculate_feature(_hsv);
                counter[Convert.ToInt16(feature)] += 1;//为counter数组中特征值为feature0的元素的值加1，表示检测到一个特征值为feature0的像素点

            }//循环，直至把所有的像素点都处理完毕

            //接下来对counter数组做归一化处理

            for (int j = 0; j < 72; j++)//图片共有hsvValues/3个像素点
            {
                counter[j] = counter[j] / totalPix;//每一个counter[i]值都是[0,1]之间的小数
                //其物理意义为直方图的高度，亦即各种颜色在整张图片中所占比例
            }
        }


    };
}
