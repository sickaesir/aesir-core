using CommonLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS4Library
{
    public struct DS4Color
    {
        public byte red;
        public byte green;
        public byte blue;
        public DS4Color(Rgb c)
        {
            red = c.r;
            green = c.g;
            blue = c.b;
        }
        public DS4Color(byte r, byte g, byte b)
        {
            red = r;
            green = g;
            blue = b;
        }
        public override bool Equals(object obj)
        {
            if (obj is DS4Color)
            {
                DS4Color dsc = ((DS4Color)obj);
                return (this.red == dsc.red && this.green == dsc.green && this.blue == dsc.blue);
            }
            else
                return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public Rgb ToColor => new Rgb(red, green, blue);
        //public Color ToColorA
        //{
        //    get
        //    {
        //        byte alphacolor = Math.Max(red, Math.Max(green, blue));
        //        Color reg = Color.FromArgb(red, green, blue);
        //        Color full = HuetoRGB(reg.GetHue(), reg.GetBrightness(), reg);
        //        return Color.FromArgb((alphacolor > 205 ? 255 : (alphacolor + 50)), full);
        //    }
        //}

        private Rgb HuetoRGB(float hue, float light, Rgb rgb)
        {
            float L = (float)Math.Max(.5, light);
            float C = (1 - Math.Abs(2 * L - 1));
            float X = (C * (1 - Math.Abs((hue / 60) % 2 - 1)));
            float m = L - C / 2;
            float R = 0, G = 0, B = 0;
            if (light == 1) return new Rgb(0xff, 0xff, 0xff);
            else if (rgb.r == rgb.g && rgb.g == rgb.b) return new Rgb(0xff, 0xff, 0xff);
            else if (0 <= hue && hue < 60) { R = C; G = X; }
            else if (60 <= hue && hue < 120) { R = X; G = C; }
            else if (120 <= hue && hue < 180) { G = C; B = X; }
            else if (180 <= hue && hue < 240) { G = X; B = C; }
            else if (240 <= hue && hue < 300) { R = X; B = C; }
            else if (300 <= hue && hue < 360) { R = C; B = X; }
            return new Rgb((byte)((R + m) * 255), (byte)((G + m) * 255), (byte)((B + m) * 255));
        }

        public static bool TryParse(string value, ref DS4Color ds4color)
        {
            try
            {
                string[] ss = value.Split(',');
                return byte.TryParse(ss[0], out ds4color.red) && byte.TryParse(ss[1], out ds4color.green) && byte.TryParse(ss[2], out ds4color.blue);
            }
            catch { return false; }
        }
        public override string ToString() => $"Red: {red} Green: {green} Blue: {blue}";
    }
}
