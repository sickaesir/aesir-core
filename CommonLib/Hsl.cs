using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib
{
    public class Hsl
    {
        public double h;
        public double s;
        public double l;

        public Hsl()
        {
            h = s = l = 0;
        }

        public Hsl(double _h, double _s, double _l)
        {
            h = _h;
            s = _s;
            l = _l;
        }
    }
}
