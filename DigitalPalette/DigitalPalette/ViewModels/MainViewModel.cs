using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace DigitalPalette.ViewModels
{
    #region [TAB1]
    public partial class MainViewModel : VMBase
    {
        #region [Variable]
        // 현재 마우스 포인터 위치
        public static Point nowPoint;

        #region 마우스 포인터 위치 DLL
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };
        #endregion

        private Models.ColorInfo _nowColor = new Models.ColorInfo();
        public Models.ColorInfo nowColor { get => _nowColor; set { _nowColor = value; OnPropertyChanged("nowColor"); } }
        #endregion

        #region [COMMAND]

        private bool CanExe(object args)
        {
            return true;
        }
        #endregion

        public MainViewModel()
        {
            DispatcherTimer timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += new EventHandler(MousePointerColor);
            timer.Start();
        }

        /// <summary>
        /// 마우스 포인터의 현재 위치를 Win32를 이용하여 되돌려주는 함수입니다.
        /// </summary>
        /// <returns></returns>
        public static Point GetMousePosition()
        {
            var w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);
            nowPoint = new Point(w32Mouse.X, w32Mouse.Y);
            return nowPoint;
        }

        public void MousePointerColor(object sender, EventArgs e)
        {
            // Console.WriteLine(string.Format("{0}, {1}", nowPoint.X, nowPoint.Y));

            System.Drawing.Color tmp = ScreenColor((int) nowPoint.X, (int) nowPoint.Y);
            nowColor = new Models.ColorInfo()
            {
                solidColorbrush = new SolidColorBrush(Color.FromRgb(tmp.R, tmp.G, tmp.B)),
                r = tmp.R,
                g = tmp.G,
                b = tmp.B
            };
        }

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

    #region [ETC]
    public partial class MainViewModel : VMBase
    {
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
    }
    #endregion
}
