using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

namespace DigitalPalette.ViewModels
{
    #region [TAB1]
    public partial class MainViewModel : VMBase
    {
        #region [변수]
        private ObservableCollection<Models.ColorInfo> _ListColors = new ObservableCollection<Models.ColorInfo>();
        public ObservableCollection<Models.ColorInfo> ListColors { get => _ListColors; set { _ListColors = value; OnPropertyChanged("ListColors"); } }

        private List<int> SelectedItemsIdx = new List<int>();

        private bool deleting = false;

        ListView lv;
        #endregion

        #region [COMMAND]
        private RelayCommand _deleteListItemCmd;
        public ICommand DeleteListItemCmd { get { return _deleteListItemCmd ?? (_deleteListItemCmd = new RelayCommand(DeleteListItem, CanExe)); } }
        private void DeleteListItem(object sender)
        {
            deleting = true;
            if(SelectedItemsIdx.Count > 0)
            {
                foreach(int idx in SelectedItemsIdx)
                {
                    ListColors.RemoveAt(idx);
                }
            }
            deleting = false;
        }

        private RelayCommand _unSelectItemsCmd;
        public ICommand UnSelectItemsCmd { get { return _unSelectItemsCmd ?? (_unSelectItemsCmd = new RelayCommand(UnSelectAllItems, CanExe)); } }
        private void UnSelectAllItems(object sender)
        {
            if(lv!= null)
            {
                if (lv.SelectedItems.Count > 0)
                {
                    lv.UnselectAll();
                }
            }
        }

        public void Tab1_LV_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (!deleting)
            {
                lv = sender as ListView;
                SelectedItemsIdx.Clear();
                if (lv != null)
                {
                    foreach (Models.ColorInfo item in lv.SelectedItems)
                    {
                        SelectedItemsIdx.Add(lv.Items.IndexOf(item));
                        selectColor = item;
                    }
                    SelectedItemsIdx.Reverse();
                }
            }
        }
        #endregion

    }
    #endregion


    #region [TAB2]
    public partial class MainViewModel : VMBase
    {
        #region [변수]
        private Models.ColorInfo _backgroundColor = new Models.ColorInfo() { solidColorbrush = new SolidColorBrush(Colors.White) };
        public Models.ColorInfo backgroundColor { get => _backgroundColor; set { _backgroundColor = value; OnPropertyChanged("backgroundColor"); } }

        private Models.ColorInfo[] _opacColor = new Models.ColorInfo[9];
        public Models.ColorInfo[] OpacColor { get => _opacColor; set { OnPropertyChanged("OpacColor"); } }

        private SolidColorBrush[] _opacForeColor = new SolidColorBrush[10];
        public SolidColorBrush[] OpacForeColor { get => _opacForeColor; set { OnPropertyChanged("OpacForeColor"); } }
        #endregion

        #region [COMMAND]
        private RelayCommand _colorPickCmd;
        public ICommand ColorPickCmd { get { return _colorPickCmd ?? (_colorPickCmd = new RelayCommand(ColorPick, CanExe)); } }
        private void ColorPick(object sender)
        {
            System.Windows.Forms.ColorDialog cd = new System.Windows.Forms.ColorDialog();
            if(cd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                backgroundColor = new Models.ColorInfo()
                {
                    solidColorbrush = new SolidColorBrush(Color.FromRgb(cd.Color.R, cd.Color.G, cd.Color.B))
                };
            }
            CalcOpacityColor();
        }

        private void CalcOpacityColor()
        {
            _opacColor = new Models.ColorInfo[9];

            for(int i=1; i<10; i++)
            {
                Color s = Color.FromRgb((byte)selectColor.r, (byte)selectColor.g, (byte)selectColor.b);
                Color b = Color.FromRgb((byte)backgroundColor.r, (byte)backgroundColor.g, (byte)backgroundColor.b);
                Color tmp = Blend(s, b, i * 0.1f);

                _opacColor[i - 1] = new Models.ColorInfo() { solidColorbrush = new SolidColorBrush(tmp)};
                _opacForeColor[i - 1] = new SolidColorBrush(_opacColor[i-1].BlackOrWhite());
            }

            _opacForeColor[9] = new SolidColorBrush(selectColor.BlackOrWhite());

            OpacColor = _opacColor;
            OpacForeColor = _opacForeColor;
        }

        private Color Blend(Color color, Color backColor, float alpha)
        {
            byte r = (byte)((color.R * alpha) + (backColor.R * (1 - alpha)));
            byte g = (byte)((color.G * alpha) + (backColor.G * (1 - alpha)));
            byte b = (byte)((color.B * alpha) + (backColor.B * (1 - alpha)));
            return Color.FromRgb(r, g, b);
        }
        #endregion
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
        #region [변수]
        // 포인터 위치를 나타내기 위한 타이머
        private DispatcherTimer timer = new DispatcherTimer();

        // 색 저장을 위한 후킹 변수
        private LowLevelKeyboardListener _listener;

        // 현재 마우스 포인터 위치
        public static Point nowPoint;

        // FollowMouseWindow 화면 창
        Views.FollowMouseWindow f;
        // FollowMouseWindow 화면 창의 왼쪽과 위쪽
        private String _left = "0";
        public String left { get => _left; set { _left = value; OnPropertyChanged("left"); } }

        private String _top = "0";
        public String top { get => _top; set { _top = value; OnPropertyChanged("top"); } }
        // FollowMouseWindow 화면이 존재 및 CheckBox Check 확인 변수
        private bool _isFollowWindowActive = false;
        public bool isFollowWindowActive { get => _isFollowWindowActive; set { _isFollowWindowActive = value; OnPropertyChanged("isFollowWindowActive"); } }

        // 포인터 위치의 색
        private Models.ColorInfo _nowColor = new Models.ColorInfo();
        public Models.ColorInfo nowColor { get => _nowColor; set { _nowColor = value; OnPropertyChanged("nowColor"); } }

        private Models.ColorInfo _selectColor = new Models.ColorInfo();
        public Models.ColorInfo selectColor { get => _selectColor; set { _selectColor = value; CalcOpacityColor(); OnPropertyChanged("selectColor"); } }
        #endregion

        #region [COMMAND]
        public void OepnFMW(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = sender as ToggleButton;
            if (tb.IsChecked == true)
            {
                _listener = new LowLevelKeyboardListener();
                _listener.doEvent += PickColor;
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

        public void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider s = sender as Slider;
            if (s.Name.Equals("red_S"))
            {
                selectColor = new Models.ColorInfo()
                {
                    solidColorbrush = new SolidColorBrush(Color.FromRgb((byte)s.Value, (byte)selectColor.g, (byte)selectColor.b))
                };
            }
            else if (s.Name.Equals("green_S"))
            {
                selectColor = new Models.ColorInfo()
                {
                    solidColorbrush = new SolidColorBrush(Color.FromRgb((byte)selectColor.r, (byte)s.Value, (byte)selectColor.b))
                };
            }
            else if (s.Name.Equals("blue_S"))
            {
                selectColor = new Models.ColorInfo()
                {
                    solidColorbrush = new SolidColorBrush(Color.FromRgb((byte)selectColor.r, (byte)selectColor.g, (byte)s.Value))
                };
            }
        }

        private RelayCommand _addColorCmd;
        public ICommand AddColorCmd { get { return _addColorCmd ?? (_addColorCmd = new RelayCommand(AddColor, CanExe)); } }
        private void AddColor(object args)
        {
            ListColors.Add(selectColor);
        }

        public void NumPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        public void HexNumPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9a-fA-F]+");
            e.Handled = regex.IsMatch(e.Text);

            TextBox tb = sender as TextBox;
            if(tb.Text.Length == 6)
            {
                byte[] rgb = ConvertHexStringToByte(tb.Text);
                selectColor = new Models.ColorInfo()
                {
                    solidColorbrush = new SolidColorBrush(Color.FromRgb(rgb[0], rgb[1], rgb[2]))
                };
            }
            tb.Select(tb.Text.Length, 0);
        }
        public void TabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            TabControl tc = sender as TabControl;
            
            if(tc.SelectedIndex == 1)
            {
                CalcOpacityColor();
            }
        }
        private bool CanExe(object args)
        {
            return true;
        }
        #endregion

        #region [함수]
        public MainViewModel()
        {
            ListColors.Clear();

            timer.Interval = TimeSpan.FromMilliseconds(1);
            timer.Tick += new EventHandler(doTimer);
            timer.Start();
        }

        /// <summary>
        /// 16진수문자열을 byte[]로 변경하는 함수입니다.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] ConvertHexStringToByte(string str)
        {
            byte[] result = new byte[str.Length / 2];

            for(int i=0; i<result.Length; i++)
            {
                result[i] = Convert.ToByte(str.Substring(i * 2, 2), 16);
            }
            return result;
        }

        /// <summary>
        /// 마우스 후킹에서 진행되는 이벤트 함수입니다.
        /// </summary>
        private void PickColor()
        {
            ListColors.Add(nowColor);
        }

        /// <summary>
        /// 타이머에서 동작시킬 이벤트 함수입니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void doTimer(object sender, EventArgs e)
        {
            SetFollowMouseWindowPosition();
            MousePointerColor();
        }

        /// <summary>
        /// FollowMouseWindow의 위치 설정하는 함수입니다.
        /// </summary>
        private void SetFollowMouseWindowPosition()
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
        private void MousePointerColor()
        {
            // Console.WriteLine(string.Format("{0}, {1}", nowPoint.X, nowPoint.Y));

            System.Drawing.Color tmp = ScreenColor((int)nowPoint.X, (int)nowPoint.Y);
            nowColor = new Models.ColorInfo()
            {
                solidColorbrush = new SolidColorBrush(Color.FromRgb(tmp.R, tmp.G, tmp.B))
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
