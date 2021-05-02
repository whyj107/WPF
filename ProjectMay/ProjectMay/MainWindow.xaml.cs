using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows;
using System.Windows.Controls;

namespace ProjectMay
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;

            if(b == select_b)
            {
                CommonOpenFileDialog cofd = new CommonOpenFileDialog();
                cofd.IsFolderPicker = true;
                if (cofd.ShowDialog() == CommonFileDialogResult.Ok)
                {
                    folder_tb.Text = cofd.FileName;
                }
            }
        }
    }
}
