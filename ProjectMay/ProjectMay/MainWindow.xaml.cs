using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace ProjectMay
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        // https://www.codeproject.com/Articles/20880/Folder-protection-for-Windows-using-Csharp-and-con

        #region 00. 변수 정의
        public string status;
        string[] arr;

        private string _pathkey;
        public string pathkey
        {
            get { return _pathkey; }
            set { _pathkey = value; }
        }

        CommonOpenFileDialog cofd = new CommonOpenFileDialog();
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            status = string.Empty;
            arr = new string[6];
            arr[0] = ".{2559a1f2-21d7-11d4-bdaf-00c04f60b9f0}";
            arr[1] = ".{21EC2020-3AEA-1069-A2DD-08002B30309D}";
            arr[2] = ".{2559a1f4-21d7-11d4-bdaf-00c04f60b9f0}";
            arr[3] = ".{645FF040-5081-101B-9F08-00AA002F954E}";
            arr[4] = ".{2559a1f1-21d7-11d4-bdaf-00c04f60b9f0}";
            arr[5] = ".{7007ACC7-3202-11D1-AAD2-00805FC1270E}";

            cofd.IsFolderPicker = true;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (rWLock.IsChecked == true)
            {
                status = arr[0];
            }

            if(cofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                DirectoryInfo d = new DirectoryInfo(cofd.FileName);
                string selectedpath = d.Parent.FullName + d.Name;
                
            }

        }

        private bool checkPassword()
        {
            XmlTextReader read;
            if(pathkey == null)
            {
                //read = new XmlTextReader();
            }
            else
            {
                read = new XmlTextReader(pathkey + "\\p.xml");
            }

            return false;
        }
    }
}
