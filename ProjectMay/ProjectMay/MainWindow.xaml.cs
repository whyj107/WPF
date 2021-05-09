using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace ProjectMay
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 00. 변수 정의
        private Dictionary<string, ListViewItem> files_list = new Dictionary<string, ListViewItem>();

        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;

            files_L.Items.Remove(files_list[b.Tag.ToString()]);
            files_list.Remove(b.Tag.ToString());
        }

        private void ListView_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effects = DragDropEffects.All;
            else
                e.Effects = DragDropEffects.None;
        }

        private void ListView_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            foreach(string file in files)
            {
                // 중복 확인
                if (CheckDuplicate(file))
                {
                    MessageBox.Show("이미 존재하는 폴더입니다.");
                    return;
                }

                // 경로가 디렉토리인지 파일인지 확인
                if (!IsDirectory(file))
                {
                    MessageBox.Show("디렉토리가 아닙니다!");
                    return;
                }


                ListViewItem lvi = new ListViewItem() { Tag = file };

                Grid g = new Grid();
                Label l = new Label()
                {
                    Content = file,
                    HorizontalAlignment = HorizontalAlignment.Left
                };

                Button b = new Button()
                {
                    Tag = file,
                    Width = 20,
                    Height = 20,
                    Content = "X",
                    HorizontalAlignment = HorizontalAlignment.Right
                };

                b.Click += Button_Click;

                g.Children.Add(l);
                g.Children.Add(b);

                lvi.Content = g;

                files_list.Add(file, lvi);
                files_L.Items.Add(lvi);

                CheckProcess();
            }
        }       

        /// <summary>
        /// 디렉토리인지 체크하는 함수입니다. 
        /// true : 디렉토리
        /// false : 디렉토리가 아님
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool IsDirectory(string path)
        {
            FileAttributes chk = File.GetAttributes(path);
            if((chk & FileAttributes.Directory) == FileAttributes.Directory)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 추가하려는 항목이 ListViewItem 중에서 중복인지 확인하는 함수입니다.
        /// true : 중복
        /// false : 중복 아님
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        private bool CheckDuplicate(string path)
        {
            foreach(string key in files_list.Keys)
            {
                if (path.Equals(key))
                    return true;
            }
            return false;
        }
        
        private void CheckProcess()
        {
            foreach(string key in files_list.Keys)
            {


            }
        }
    }
}
