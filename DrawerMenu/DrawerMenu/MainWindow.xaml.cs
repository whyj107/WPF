using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DrawerMenu
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

            if(b == BtnOpen)
            {
                btn_SP.IsEnabled = false;
            }
            else if(b == BtnClose){
                btn_SP.IsEnabled = true;
            }
            else
            {
                MessageBox.Show(b.Name.ToString());
            }

        }

        private void ListViewItem_PreviewMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            ListViewItem lvi = sender as ListViewItem;
            for (int i=0; i< LV.Items.Count; i++)
            {
                var lvi_d = LV.ItemContainerGenerator.ContainerFromIndex(i) as ListViewItem;
                if (lvi_d != lvi)
                {
                    lvi_d.Background = Brushes.White;
                }
            }
        }
    }
}
