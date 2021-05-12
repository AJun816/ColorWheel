using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Palette.Common
{
    public static class ColorConverter
    {
        public static ColorHSV RgbToHsv(Color rgb)
        {
            int min, max; float H, S, V;
            min = Math.Min(Math.Min(rgb.R, rgb.G), rgb.B);
            max = Math.Max(Math.Max(rgb.R, rgb.G), rgb.B);

            H = 0;
            if (max == min)
            {
                H = 0;
            }
            else if (max == rgb.R && rgb.G > rgb.B)
            {
                H = (int)(60f * (rgb.G - rgb.B) / (max - min) + 0);
            }
            else if (max == rgb.R && rgb.G < rgb.B)
            {
                H = (int)(60f * (rgb.G - rgb.B) / (max - min) + 360);
            }
            else if (max == rgb.G)
            {
                H = (int)(60f * (rgb.B - rgb.R) / (max - min) + 120);
            }
            else if (max == rgb.B)
            {
                H = (int)(60f * (rgb.R - rgb.G) / (max - min) + 240);
            }

            S = max != 0 ? (max - min) * 1.0f / max : 0;
            V = max / 255;
            return new ColorHSV(H, S, V);
        }

        public static Color HsvToRgb(ColorHSV hsv)
        {
            int hi = Convert.ToInt32(Math.Floor(hsv.H / 60f)) % 6;
            double f = hsv.H / 60f - Math.Floor(hsv.H / 60f);

            int v = Convert.ToInt32(hsv.V);
            int p = Convert.ToInt32(hsv.V * (1 - hsv.S));
            int q = Convert.ToInt32(hsv.V * (1 - f * hsv.S));
            int t = Convert.ToInt32(hsv.V * (1 - (1 - f) * hsv.S));

            if (hi == 0)
                return Color.FromArgb(v, t, p);
            else if (hi == 1)
                return Color.FromArgb(q, v, p);
            else if (hi == 2)
                return Color.FromArgb(p, v, t);
            else if (hi == 3)
                return Color.FromArgb(p, q, v);
            else if (hi == 4)
                return Color.FromArgb(t, p, v);
            else
                return Color.FromArgb(v, p, q);


        }

        public static ColorHSL RgbToHsl(Color rgb)
        {
            return new ColorHSL(rgb.GetHue(), rgb.GetSaturation(), rgb.GetBrightness());
        }

        public static Color HslToRgb(ColorHSL hsl)
        {
            int R = 0, G = 0, B = 0;
            if (hsl.S == 0) // 灰色
            {
                R = Convert.ToInt32(255 * hsl.L);
                G = R;
                B = R;
            }
            else
            {
                float p, q, t_R, t_G, t_B;
                if (hsl.L < 0.5f)
                {
                    q = hsl.L * (1 + hsl.S);
                }
                else
                {
                    q = hsl.L + hsl.S - hsl.L * hsl.S;
                }
                p = 2 * hsl.L - q;

                float h_k = hsl.H / 360;
                t_R = h_k + 1f / 3;
                t_G = h_k;
                t_B = h_k - 1f / 3;
                R = HueToRgb(p, q, t_R);
                G = HueToRgb(p, q, t_G);
                B = HueToRgb(p, q, t_R);
            }
            return Color.FromArgb(R, G, B);

        }

        internal static int HueToRgb(float p, float q, float t_C)
        {
            float Color_C;
            if (t_C < 0) t_C += 1;
            if (t_C > 1) t_C -= 1;

            if (t_C < 1 / 6)
                Color_C = p + (q - p) * 6 * t_C;
            else if (t_C < 1 / 2)
                Color_C = q;
            else if (t_C < 2 / 3)
                Color_C = p + (q - p) * (2 / 3 - t_C) * 6;
            else
                Color_C = p;
            return Convert.ToInt32(255 * Color_C);
        }

    }

    public class ColorHSL
    {
        public ColorHSL(float h, float s, float l)
        {
            this._h = h;
            this._s = s;
            this._l = l;
        }

        private float _h;
        private float _s;
        private float _l;

        public float H
        {
            get { return this._h; }
            set
            {
                this._h = value;
                this._h = this._h > 360 ? 360 : this._h;
                this._h = this._h < 0 ? 0 : this._h;
            }
        }

        public float S
        {
            get { return this._s; }
            set
            {
                this._s = value;
                this._s = this._s > 1 ? 1 : this._s;
                this._s = this._s < 0 ? 0 : this._s;
            }

        }

        public float L
        {
            get { return this._l; }
            set
            {
                this._l = value;
                this._l = this._l > 1 ? 1 : this._l;
                this._l = this._l < 0 ? 0 : this._l;
            }
        }

    }

    public class ColorHSV
    {

        public ColorHSV(float h, float s, float v)
        {
            this._h = h;
            this._s = s;
            this._v = v;
        }

        private float _h;
        private float _s;
        private float _v;


        public float H
        {
            get { return this._h; }
            set
            {
                this._h = value;
                this._h = this._h > 360 ? 360 : this._h;
                this._h = this._h < 0 ? 0 : this._h;
            }
        }

        public float S
        {
            get { return this._s; }
            set
            {
                this._s = value;
                this._s = this._s > 1 ? 1 : this._s;
                this._s = this._s < 0 ? 0 : this._s;
            }
        }

        public float V
        {
            get { return this._v; }
            set
            {
                this._v = value;
                this._v = this._v > 1 ? 1 : this._v;
                this._v = this._v < 0 ? 0 : this._v;
            }
        }

    }

}
