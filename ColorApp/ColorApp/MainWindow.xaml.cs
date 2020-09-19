using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ColorApp
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /*********************
        * 10진수 입력
        **********************/
        private void TextBox10_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!Char.IsDigit((char)KeyInterop.VirtualKeyFromKey(e.Key)) & e.Key != Key.Back & e.Key != Key.Tab | e.Key == Key.Space)
            {
                e.Handled = true;
            }
        }

        /*********************
        * 16진수 입력
        **********************/
        private void TextBox16_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!Char.IsLetterOrDigit((char)KeyInterop.VirtualKeyFromKey(e.Key)) & e.Key != Key.Back | e.Key == Key.Space)
            {
                e.Handled = true;
            }

            string[] hex_alpha = { "A", "B", "C", "D", "E", "F" };
            int position = Array.IndexOf(hex_alpha, e.Key.ToString());
            if (position == -1)
            {
                e.Handled = true;
            }
        }

        /*********************
        * 변환 버튼 클릭
        **********************/
        private void Btn_trans_Click(object sender, RoutedEventArgs e)
        {
            int r_10 = CheckNumber(textBoxR10.Text);
            int g_10 = CheckNumber(textBoxG10.Text);
            int b_10 = CheckNumber(textBoxB10.Text);

            LabelR16.Content = "0x" + ToHexString(r_10).Substring(0, 2);
            LabelG16.Content = "0x" + ToHexString(g_10).Substring(0, 2);
            LabelB16.Content = "0x" + ToHexString(b_10).Substring(0, 2);

            Color1.Background = new SolidColorBrush(Color.FromArgb(255, (byte)r_10, (byte)g_10, (byte)b_10));
        }

        /*********************
        * 10진수 16진수로 변경
        **********************/
        public string ToHexString(int int10)
        {
            byte[] bytes = BitConverter.GetBytes(int10);
            string hexString = String.Empty;
            for (int i = 0; i < bytes.Length; i++)
            {
                hexString += bytes[i].ToString("X2");
            }
            return hexString;
        }

        /*********************
        * 10진수 입력 확인
        **********************/
        public int CheckNumber(string input_num)
        {
            int output_num = 0;
            if (!input_num.Equals(""))
            {
                if (int.Parse(input_num) > 255)
                {
                    output_num = 255;
                }
                else if (int.Parse(input_num) > 0)
                {
                    output_num = int.Parse(input_num);
                }
            }

            return output_num;
        }

        /*********************
        * 10진수 포커스
        **********************/
        private void TextBox10_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox tb = (sender as TextBox);
            if(tb != null)
            {
                tb.SelectAll();
            }
        }
    }
}
