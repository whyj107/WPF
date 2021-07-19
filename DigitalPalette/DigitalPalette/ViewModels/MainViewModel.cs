using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace DigitalPalette.ViewModels
{
    #region [TAB1]
    public partial class MainViewModel : VMBase
    {
        #region [Variable]
        private ObservableCollection<Models.ColorInfo> _ListColors = new ObservableCollection<Models.ColorInfo>();
        public ObservableCollection<Models.ColorInfo> ListColors { get => _ListColors; set { _ListColors = value; OnPropertyChanged("ListColors"); } }
        #endregion

        #region [COMMAND]        
        private bool CanExe(object args)
        {
            return true;
        }
        #endregion

    }
    #endregion


    #region [TAB2]
    public partial class MainViewModel : VMBase
    {

    }
    #endregion


    #region [TAB 3]
    public partial class MainViewModel : VMBase
    {

    }
    #endregion

    #region [COMMON]
    public partial class MainViewModel : VMBase
    {
        #region [Variable]
        // 포인터 위치를 나타내기 위한 타이머
        private DispatcherTimer timer = new DispatcherTimer();

        // 색 저장을 위한 후킹 변수
        private LowLevelKeyboardListener _listener;

        // 현재 마우스 포인터 위치
        public static Point nowPoint;

        // FollowMouseWindow 화면 창
        Views.FollowMouseWindow f;
        // FollowMouseWindow 화면이 존재 및 CheckBox Check 확인 변수
        private bool _isFollowWindowActive = false;
        public bool isFollowWindowActive { get => _isFollowWindowActive; set { _isFollowWindowActive = value; OnPropertyChanged("isFollowWindowActive"); } }

        // 포인터 위치의 색
        private Models.ColorInfo _nowColor = new Models.ColorInfo();
        public Models.ColorInfo nowColor { get => _nowColor; set { _nowColor = value; OnPropertyChanged("nowColor"); } }
        #endregion

        #region [COMMAND]
        public void OepnFMW(object sender, RoutedEventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            if (cb.IsChecked == true)
            {
                _listener = new LowLevelKeyboardListener();
                _listener.doEvent += SaveColor;
                _listener.HookKeyboard();

                f = new Views.FollowMouseWindow();
                f.Show();
            }
            else
            {
                f.Close();
                _listener.UnHookKeyboard();
            }
        }

        /// <summary>
        /// 마우스 후킹에서 진행되는 이벤트 함수입니다.
        /// </summary>
        private void SaveColor()
        {
            ListColors.Add(nowColor);
        }

        public void Window_Closed(object sender, EventArgs e)
        {
            if (timer.IsEnabled)
            {
                timer.Stop();
            }

            if (_listener != null)
            {
                _listener.UnHookKeyboard();
            }

            Application.Current.Shutdown();
        }
        #endregion

        public MainViewModel()
        {
            ListColors.Clear();

            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += new EventHandler(MousePointerColor);
            timer.Start();
        }

        private String _left = "0";
        public String left { get => _left; set { _left = value; OnPropertyChanged("left"); } }

        private String _top = "0";
        public String top { get => _top; set { _top = value; OnPropertyChanged("top"); } }
        private void Test()
        {
            int height = 100;
            int width = 150;
            double _x = SystemParameters.VirtualScreenWidth / (System.Windows.Forms.Screen.AllScreens).Length;
            double _y = SystemParameters.VirtualScreenHeight;

            int margin = 10;

            var pointToScreen = System.Windows.Forms.Control.MousePosition;
            nowPoint = new Point(pointToScreen.X, pointToScreen.Y);

            double x = pointToScreen.X + 1.5 * margin;
            double y = pointToScreen.Y + margin;

            if (pointToScreen.Y > _y - 1.5 * height)
            {
                y = pointToScreen.Y - height - margin;
            }

            if ((pointToScreen.X > 0) && (pointToScreen.X > _x - 1.5 * width))
            {
                x = pointToScreen.X - width - margin;
            }

            // Console.WriteLine(string.Format("{0}, {1}", pointToScreen.X, pointToScreen.Y));

            left = x.ToString();
            top = y.ToString();
        }

        /// <summary>
        /// nowPoint의 위치의 색을 nowColor 변수에 저장하는 함수입니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void MousePointerColor(object sender, EventArgs e)
        {
            Test();
            // Console.WriteLine(string.Format("{0}, {1}", nowPoint.X, nowPoint.Y));

            System.Drawing.Color tmp = ScreenColor((int)nowPoint.X, (int)nowPoint.Y);
            nowColor = new Models.ColorInfo()
            {
                solidColorbrush = new SolidColorBrush(Color.FromRgb(tmp.R, tmp.G, tmp.B))
            };
        }

        #region 함수
        /// <summary>
        /// 포인터의 색 추출하는 함수입니다.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        private static System.Drawing.Color ScreenColor(int x, int y)
        {
            System.Drawing.Size sz = new System.Drawing.Size(1, 1);
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(1, 1);
            System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp);
            g.CopyFromScreen(x, y, 0, 0, sz);
            return bmp.GetPixel(0, 0);
        }

        /// <summary>
        /// 메모장에 로그를 남기는 함수입니다.
        /// [yyyy-MM-dd HH:mm:ss.fff:    str]
        /// </summary>
        /// <param name="str"></param>
        private void Log(string str)
        {
            string currentDirectoryPath = Environment.CurrentDirectory.ToString();
            string DirPath = System.IO.Path.Combine(currentDirectoryPath, "Logs");
            string FilePath = DirPath + @"\Log_" + DateTime.Today.ToString("yyyyMMdd") + ".log";

            DirectoryInfo di = new DirectoryInfo(DirPath);
            FileInfo fi = new FileInfo(FilePath);

            try
            {
                // Logs 디렉토리가 없을 경우 생성
                if (!di.Exists) Directory.CreateDirectory(DirPath);

                // 오류 메세지 생성
                string error_string = string.Format("{0}: \t{1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), str);
                Console.WriteLine(error_string);

                // Log 파일이 존재할 경우와 존재하지 않을 경우로 나누어서 진행
                if (!fi.Exists)
                {
                    using (StreamWriter sw = new StreamWriter(FilePath))
                    {
                        sw.WriteLine(error_string);
                        sw.Close();
                    }
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(FilePath))
                    {
                        sw.WriteLine(error_string);
                        sw.Close();
                    }
                }
            }
            catch
            {

            }
        }
        #endregion
    }
    #endregion
}
