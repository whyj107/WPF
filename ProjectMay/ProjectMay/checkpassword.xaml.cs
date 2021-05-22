using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ProjectMay
{
    /// <summary>
    /// checkpassword.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class checkpassword : Window
    {
        public string pass;
        public Action<bool> status;

        public checkpassword()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if(b == ok_B)
            {
                if (password_TB.Text.Equals(pass))
                {
                    status(true);
                    Close();
                }
                else
                {
                    MessageBox.Show("잘못된 비밀번호입니다.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    status(false);
                }
            }
            else if(b == cancle_B)
            {
                Close();
            }
        }        

    }
}
