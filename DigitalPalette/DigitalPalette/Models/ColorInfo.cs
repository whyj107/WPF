using System;
using System.Windows.Media;

namespace DigitalPalette.Models
{
    public class ColorInfo
    {
        // SolidColorBrush로 색 저장
        private SolidColorBrush _solidColorbrush = new SolidColorBrush(Colors.WhiteSmoke);
        public SolidColorBrush solidColorbrush { 
            get => _solidColorbrush; 
            set
            {
                r = value.Color.R;
                g = value.Color.G;
                b = value.Color.B;
                _solidColorbrush = value;
                foreGroundColor = value;
            } 
        }

        // r 값(hex string & int)
        private string hex_r = "F5";

        private int _r = 245;
        public int r { get => _r; set { _r = value; hex_r = IntToHexString(value); } }

        // g값(hex string & int)
        private string hex_g = "F5";
        private int _g = 245;
        public int g { get => _g; set { _g = value; hex_g = IntToHexString(value); } }

        // b값(hex string & int)
        private string hex_b = "F5";
        private int _b = 245;
        public int b { get => _b; set { _b = value; hex_b = IntToHexString(value); } }

        // 색의 hex 값
        public string hex { 
            get => string.Format("{0}{1}{2}", hex_r, hex_g, hex_b);
            set { }
        }

        // 내부에 사용될 글자색
        private SolidColorBrush _foreGroundColor = new SolidColorBrush();
        public SolidColorBrush foreGroundColor
        {
            get => _foreGroundColor;
            set
            {
                _foreGroundColor = new SolidColorBrush(BlackOrWhite());
            }
        }

        /// <summary>
        /// Int 값을 Hex String으로 변경해주는 함수값입니다. 사실 이렇게 사용안하고 ColorConverter를 사용해도 됩니다.
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private string IntToHexString(int num)
        {
            byte[] bytes = BitConverter.GetBytes(num);
            return string.Concat(Array.ConvertAll(bytes, x => x.ToString("X2"))).Substring(0, 2);
        }

        /// <summary>
        /// 흰색과 검정색 중 잘 보이는 색으로 색을 설정하는 함수입니다. 아래의 사이트에서 보고 사용하였습니다.
        /// https://stackoverflow.com/questions/3942878/how-to-decide-font-color-in-white-or-black-depending-on-background-color
        /// </summary>
        /// <returns></returns>
        public Color BlackOrWhite()
        {
            return (_r * 0.299 + _g * 0.587 + _b * 0.114) > 186 ? Colors.Black : Colors.White;
        }

        /// <summary>
        /// 처음에 글자색을 잘 보이게하고 싶어서 반대색을 사용하였는데 생각보다 글자색이 보이지 않아서 사용하지 않게된 함수입니다.
        /// </summary>
        /// <returns></returns>
        public Color GetOppositeColor()
        {
            byte opp_r = (byte)(255 - _r);
            byte opp_g = (byte)(255 - _g);
            byte opp_b = (byte)(255 - _b);
            return Color.FromRgb(opp_r, opp_g, opp_b);
        }

    }
}
