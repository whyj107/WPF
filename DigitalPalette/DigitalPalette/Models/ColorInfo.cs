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
            byte opp_r = (byte) (255 - _r);
            byte opp_g = (byte) (255 - _g);
            byte opp_b = (byte) (255 - _b);
            return Color.FromRgb(opp_r, opp_g, opp_b);
        }

        public Color BlackOrWhite()
        {
            return (_r * 0.299 + _g * 0.587 + _b * 0.114) > 186 ? Colors.Black : Colors.White;
        }

    }
}
