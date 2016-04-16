using System;
using System.Collections.Generic;
using System.Text;

namespace Common.ImageUtils
{
    class Similarity
    {
        public float similar(float[] source, float[] target)
        {
            float simi = 0;
            for (int i = 0; i < 72; i++)
            {
                simi += min(source[i], target[i]);
            }
            return simi;
        }
        private float min(float a, float b)
        {
            if (a < b)
                return a;
            else
                return b;
        }
    }
}
