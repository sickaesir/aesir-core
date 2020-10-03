using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib
{
    [Serializable]
    public class Rgb
    {
        public byte r;
        public byte g;
        public byte b;

        public Rgb()
        {
            r = 0;
            g = 0;
            b = 0;
        }

        public Rgb(byte _r, byte _g, byte _b)
        {
            r = _r;
            g = _g;
            b = _b;
        }

        public override string ToString()
        {
            return $"RGB (R: {r}, G: {g}, B: {b})";
        }

        public static Rgb FromHsv(double _h, double _s, double _v)
        {

            int hi = Convert.ToInt32(Math.Floor(_h / 60)) % 6;
            double f = _h / 60 - Math.Floor(_h / 60);

            _v = _v * 255;
            int v = Convert.ToInt32(_v);
            int p = Convert.ToInt32(v * (1 - _s));
            int q = Convert.ToInt32(v * (1 - f * _s));
            int t = Convert.ToInt32(v * (1 - (1 - f) * _s));


            if (hi == 0)
                return new Rgb((byte)v, (byte)t, (byte)p);
            else if (hi == 1)
                return new Rgb((byte)q, (byte)v, (byte)p);
            else if (hi == 2)
                return new Rgb((byte)p, (byte)v, (byte)t);
            else if (hi == 3)
                return new Rgb((byte)p, (byte)q, (byte)v);
            else if (hi == 4)
                return new Rgb((byte)t, (byte)p, (byte)v);
            else
                return new Rgb((byte)v, (byte)p, (byte)q);
        }

        public static Rgb FromHsl(double h, double s, double l)
        {
            return FromHsl(new Hsl(h, s, l));
        }

        public static Rgb FromHsl(Hsl hsl)
        {
            double v;

            double r, g, b;



            r = hsl.l;   // default to gray

            g = hsl.l;

            b = hsl.l;

            v = (hsl.l <= 0.5) ? (hsl.l * (1.0 + hsl.s)) : (hsl.l + hsl.s - hsl.l * hsl.s);

            if (v > 0)

            {

                double m;

                double sv;

                int sextant;

                double fract, vsf, mid1, mid2;



                m = hsl.l + hsl.l - v;

                sv = (v - m) / v;

                hsl.h *= 6.0;

                sextant = (int)hsl.h;

                fract = hsl.h - sextant;

                vsf = v * sv * fract;

                mid1 = m + vsf;

                mid2 = v - vsf;

                switch (sextant)

                {

                    case 0:

                        r = v;

                        g = mid1;

                        b = m;

                        break;

                    case 1:

                        r = mid2;

                        g = v;

                        b = m;

                        break;

                    case 2:

                        r = m;

                        g = v;

                        b = mid1;

                        break;

                    case 3:

                        r = m;

                        g = mid2;

                        b = v;

                        break;

                    case 4:

                        r = mid1;

                        g = m;

                        b = v;

                        break;

                    case 5:

                        r = v;

                        g = m;

                        b = mid2;

                        break;

                }

            }

            return new Rgb(Convert.ToByte(r * 255.0f), Convert.ToByte(g * 255.0f), Convert.ToByte(b * 255.0f));
        }
    }
}
