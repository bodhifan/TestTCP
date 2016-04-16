using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ImageUtils
{

    public class Unit_RGB
    {
        float B;
        float G;
        float R;

        public Unit_RGB(float b, float g, float r)
        {
            R = r;
            G = g;
            B = b;
        }
        public float getB()
        {
            return B;
        }
        public void setB(float b)
        {
            B = b;
        }
        public float getR()
        {
            return R;
        }
        public void setR(float r)
        {
            R = r;
        }
        public float getG()
        {
            return G;
        }
        public void setG(float g)
        {
            G = g;
        }

    };//注意顺序
}

