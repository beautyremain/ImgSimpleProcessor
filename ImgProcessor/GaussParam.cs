using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImgProcessor
{
    public class GaussParam
    {
        public double u;
        public double d;
        public double k;
        public GaussParam(double u,double d,double k)
        {
            this.u = u;
            this.k = k;
            this.d = d;
        }
    }
}
