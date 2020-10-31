using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace ForU_Excel
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



        /************************
         * UI 관련 이벤트 함수
         * **********************/

        // 숫자만 입력하도록 설정
        private void AR_TB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        // 알파벳만 입력하도록 설정
        private void RC_TB_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^a-zA-Z]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        // 버튼 클릭
        private void Btn1_Click(object sender, RoutedEventArgs e)
        {
            Mecro_TB.Text = "";

            if (!AR_TB.Text.Equals(""))
            {
                int row = int.Parse(AR_TB.Text) + 1;

                string R_start = "Sub row_fun()";
                string R_change = string.Format("    Rows({0}).Insert", row.ToString());
                string R_end = "End Sub";

                Mecro_TB.AppendText("x번째에 빈행 추가 시작--------------\n");
                Mecro_TB.AppendText(R_start);
                Mecro_TB.AppendText("\n");
                Mecro_TB.AppendText(R_change);
                Mecro_TB.AppendText("\n");
                Mecro_TB.AppendText(R_end);
                Mecro_TB.AppendText("\nx번째에 빈행 추가 끝--------------\n");
            }

            if (!RC_TB1.Text.Equals("") && !RC_TB2.Text.Equals(""))
            {
                if (FD_TB.Text.Equals(""))
                {
                    MessageBox.Show("찾을 값을 입력해주세요.");
                }
                else
                {
                    string F = string.Format(
                        "Sub find_func()\n" +
                        "    Dim strAddr As String\n" +
                        "    Dim find_data As Range\n" +
                        "    With ActiveSheet.UsedRange\n" +
                        "        Set find_data = Range(\"{0}:{1}\").Find(What:=\"{2}\", Lookat:=xlPart)\n" +
                        "        If Not find_data Is Nothing Then\n" +
                        "            strAddr = find_data.Address\n" +
                        "            Do\n" +
                        "                Rows(find_data.Row + 1).Insert\n" +
                        "                Set find_data = Range(\"{0}:{1}\").FindNext(find_data)\n" +
                        "            Loop While Not find_data Is Nothing And find_data.Address <> strAddr\n" +
                        "        End If\n" +
                        "    End With\n" +
                        "End Sub",
                        RC_TB1.Text.ToUpper(), RC_TB2.Text.ToUpper(), FD_TB.Text);

                    Mecro_TB.AppendText("특정값 빈행 추가 시작--------------\n");
                    Mecro_TB.AppendText(F);
                    Mecro_TB.AppendText("\n특정값 빈행 추가 끝--------------\n");
                }
            }
        }

    }
}
