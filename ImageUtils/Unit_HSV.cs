using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ImageUtils
{
    public class Unit_HSV
    {
        float H;
        float S;
        float V;

        public Unit_HSV(float h, float s, float v)
        {
            H = h;
            S = s;
            V = v;
        }

        public float getH()
        {
            return H;
        }
        public void setH(float h)
        {
            H = h;
        }
        public float getS()
        {
            return S;
        }
        public void setS(float s)
        {
            S = s;
        }
        public float getV()
        {
            return V;
        }
        public void setV(float v)
        {
            V = v;
        }

    };
}
