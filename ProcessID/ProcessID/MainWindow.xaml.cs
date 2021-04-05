using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;

namespace ProcessID
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 변수 정의
        private Process[] processes;
        #endregion

        public MainWindow()
        {
            InitializeComponent();
        }

        #region UI EVENT
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if(b == close_B)
            {
                ComboBoxItem cbi = (ComboBoxItem)process_CB.SelectedItem;
                if(cbi != null)
                {
                    ProgramShutDown(processes[int.Parse(cbi.Tag.ToString())].MainWindowHandle);
                    info_TB.Clear();
                    process_CB.Items.Clear();
                }
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxItem cbi = (ComboBoxItem)(sender as ComboBox).SelectedItem;

            if (cbi != null)
            {
                info_TB.Clear();

                Process Info = processes[int.Parse(cbi.Tag.ToString())];
                info_TB.AppendText("Process : " + Info.ProcessName + Environment.NewLine);
                info_TB.AppendText("시작시간 : " + Info.StartTime + Environment.NewLine);
                info_TB.AppendText("프로세스 PID : " + Info.Id + Environment.NewLine);
                info_TB.AppendText("메모리 : " + Info.VirtualMemorySize + Environment.NewLine);
            }
        }

        private void ComboBox_DropDownOpened(object sender, EventArgs e)
        {
            GetAllProcessName();
        }
        #endregion

        private void GetAllProcessName()
        {
            processes = Process.GetProcesses();
            process_CB.Items.Clear();
            int idx = -1;
            foreach (Process process in processes)
            {
                idx++;
                if (process.MainWindowHandle == IntPtr.Zero)
                {
                    continue;
                }
                ComboBoxItem cbi = new ComboBoxItem()
                {
                    Tag = idx.ToString(),
                    Content = process.ProcessName
                };
                process_CB.Items.Add(cbi);
            }
        }

        [DllImport("user32.dll", SetLastError = true)]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);
        private void ProgramShutDown(IntPtr intPtr)
        {
            uint processId = 0;
            GetWindowThreadProcessId(intPtr, out processId);
            if (processId != 0)
            {
                Process process = Process.GetProcessById((int)processId);
                process.CloseMainWindow();
                process.Refresh();
                process.Kill();
                MessageBox.Show(process.ProcessName + " 끝냈습니다.");
            }
            else
            {
                MessageBox.Show("작업이 존재하지 않습니다.");
            }
        }


    }
}
