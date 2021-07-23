using System;
using System.Windows.Media;

namespace DigitalPalette.Models
{
    public class ColorInfo
    {
        private SolidColorBrush _solidColorbrush = new SolidColorBrush(Colors.WhiteSmoke);
        public SolidColorBrush solidColorbrush { 
            get => _solidColorbrush; 
            set
            {
                r = value.Color.R;
                g = value.Color.G;
                b = value.Color.B;
                _solidColorbrush = value; 
            } 
        }

        private string hex_r = "F5";
        private int _r = 245;
        public int r { get => _r; set { _r = value; hex_r = IntToHexString(value); } }

        private string hex_g = "F5";
        private int _g = 245;
        public int g { get => _g; set { _g = value; hex_g = IntToHexString(value); } }

        private string hex_b = "F5";
        private int _b = 245;
        public int b { get => _b; set { _b = value; hex_b = IntToHexString(value); } }

        public string hex { 
            get => string.Format("{0}{1}{2}", hex_r, hex_g, hex_b);
            set { }
        }


        private string IntToHexString(int num)
        {
            byte[] bytes = BitConverter.GetBytes(num);
            return string.Concat(Array.ConvertAll(bytes, x => x.ToString("X2"))).Substring(0, 2);
        }

        public Color GetOppositeColor()
        {
            System.Drawing.Color color = System.Drawing.Color.FromArgb(_r, _g, _b);
            HSVColor hsv = GetHSV(color);
            hsv.Hue = (hsv.Hue + 0.5f) % 1.0f;
            return HSVtoColor(hsv);
        }

        public struct HSVColor
        {
            public double Hue;
            public double Saturation;
            public double Value;
        }

        private HSVColor GetHSV(System.Drawing.Color color)
        {
            HSVColor toReturn = new HSVColor();

            int max = Math.Max(color.R, Math.Max(color.G, color.B));
            int min = Math.Min(color.R, Math.Min(color.G, color.B));

            toReturn.Hue = Math.Round(color.GetHue(), 2);
            toReturn.Saturation = ((max == 0) ? 0 : 1d - (1d * min / max)) * 100;
            toReturn.Saturation = Math.Round(toReturn.Saturation, 2);
            toReturn.Value = Math.Round(((max / 255d) * 100), 2);

            return toReturn;
        }

        private Color HSVtoColor(HSVColor hSv)
        {
            int h = Convert.ToInt32(Math.Floor(hSv.Hue / 60)) % 6;
            double f = hSv.Hue / 60 - Math.Floor(hSv.Hue / 60);

            double value = hSv.Value * 255;
            int v = Convert.ToInt32(value);
            int p = Convert.ToInt32(value * (1 - hSv.Saturation));
            int q = Convert.ToInt32(value * (1 - f * hSv.Saturation));
            int t = Convert.ToInt32(value * (1 - (1 - f)*hSv.Saturation));


            Color result = Color.FromRgb((byte)v, (byte)p, (byte)q);

            switch (h){
                case 0:
                    result = Color.FromRgb((byte)v, (byte)t, (byte)p);
                    break;

                case 1:
                    result = Color.FromRgb((byte)q, (byte)v, (byte)p);
                    break;

                case 2:
                    result = Color.FromRgb((byte)p, (byte)v, (byte)t);
                    break;

                case 3:
                    result = Color.FromRgb((byte)p, (byte)q, (byte)v);
                    break;

                case 4:
                    result = Color.FromRgb((byte)t, (byte)p, (byte)v);
                    break;
            }

            return result;
        }

    }
}
