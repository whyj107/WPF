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

        // 색 모음 파일 저장 경로
        private string _savePath = string.Empty;
        public string SavePath { get => _savePath; set { _savePath = value; OnPropertyChanged("SavePath"); } }

        // 색 추가로 추가된 색들의 리스트
        private ObservableCollection<Models.ColorInfo> _ListColors = new ObservableCollection<Models.ColorInfo>();
        public ObservableCollection<Models.ColorInfo> ListColors { get => _ListColors; set { _ListColors = value; OnPropertyChanged("ListColors"); } }
        #endregion

        #region [COMMAND]
        /// <summary>
        /// ListColor에서 색을 삭제하는 COMMAND입니다. 선택한 상태에서 'Delete' 버튼을 누르면 삭제됩니다.
        /// </summary>
        private RelayCommand _deleteListItemCmd;
        public ICommand Tab0DeleteListItemCmd { get { return _deleteListItemCmd ?? (_deleteListItemCmd = new RelayCommand(Tab0_DeleteListItem, CanExe)); } }
        private void Tab0_DeleteListItem(object sender)
        {
            deleting = true;
            if(SelectedItemsIdx.Count > 0)
            {
                foreach(int idx in SelectedItemsIdx)
                {
                    ListColors.RemoveAt(idx);
                }
                SelectedItemsIdx.Clear();
            }
            deleting = false;
        }

        /// <summary>
        /// ListColor에서 선택된 항목들을 선택해제하는 COMMAND입니다.
        /// </summary>
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
        
        /// <summary>
        /// ListColor에서 선택한 색들로 새로운 색 모음을 만드는 COMMAND입니다.
        /// </summary>
        private RelayCommand _makeColorChipCmd;
        public ICommand MakeColorChipCmd { get { return _makeColorChipCmd ?? (_makeColorChipCmd = new RelayCommand(MakeColorChip, CanExe)); } }
        private void MakeColorChip(object sender)
        {
            if(SelectedItemsIdx.Count == 0)
            {
                MessageBox.Show("선택된 항목이 없습니다. \n색을 선택해주세요.");
                return;
            }else if (_savePath.Equals(string.Empty))
            {
                MessageBox.Show("색 모음 이름을 설정해주세요.");
                return;
            }

            string DirPath = currentDirectoryPath + saveDirPath;

            DirectoryInfo di = new DirectoryInfo(DirPath);
            if (!di.Exists)
            {
                Directory.CreateDirectory(DirPath);
            }

            string FilePath = DirPath + @"\" + _savePath + ".txt";

            FileInfo fi;
            try
            {
                fi = new FileInfo(FilePath);
            }
            catch
            {
                MessageBox.Show("이름에는 다음 문자를 사용할 수 없습니다. \\ / : * ? < > | \"");
                return;
            }

            try
            {
                // 파일이 존재할 경우와 존재하지 않을 경우로 나누어서 진행
                if (fi.Exists)
                {
                    MessageBox.Show("같은 이름의 파일이 존재합니다.");
                }
                else
                {
                    using (StreamWriter sw = File.AppendText(FilePath))
                    {
                        if (SelectedItemsIdx.Count > 0)
                        {
                            SelectedItemsIdx.Reverse();
                            foreach (int idx in SelectedItemsIdx)
                            {
                                sw.WriteLine("#" + ListColors[idx].hex);
                            }
                            SelectedItemsIdx.Clear();
                        }
                        sw.Close();
                        MessageBox.Show("생성 성공했습니다!");
                    }
                }
            }
            catch
            {
                MessageBox.Show("생성 실패했습니다.");
            }

        }
        #endregion
    }
    #endregion


    #region [TAB2]
    public partial class MainViewModel : VMBase
    {
        #region [변수]
        // 투명도의 바탕에 사용되는 색
        private Models.ColorInfo _backgroundColor = new Models.ColorInfo() { solidColorbrush = new SolidColorBrush(Colors.White) };
        public Models.ColorInfo backgroundColor { get => _backgroundColor; set { _backgroundColor = value; OnPropertyChanged("backgroundColor"); } }

        // 투명도에 사용될 색
        private Models.ColorInfo[] _opacColorList = new Models.ColorInfo[9];
        public Models.ColorInfo[] OpacColorList { get => _opacColorList; set { OnPropertyChanged("OpacColorList"); } }

        // 원하는 투명도 설정 값
        private int _opacity = 0;
        public int opacity { get => _opacity; set { _opacity = value; OnPropertyChanged("opacity"); } }
        
        // 원하는 투명도 설정한 값 색
        private Models.ColorInfo _opacColor = new Models.ColorInfo();
        public Models.ColorInfo opacColor { get => _opacColor; set { _opacColor = value; OnPropertyChanged("opacColor"); } }
        #endregion

        #region [COMMAND]
        /// <summary>
        /// 배경색을 선택하는 COMMAND입니다. ColorDialog를 사용했습니다.
        /// </summary>
        private RelayCommand _backgroundColorPickCmd;
        public ICommand BackgroundColorPickCmd { get { return _backgroundColorPickCmd ?? (_backgroundColorPickCmd = new RelayCommand(BackgroundColorPick, CanExe)); } }
        private void BackgroundColorPick(object sender)
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
            ChangeColor(null);
        }

        /// <summary>
        /// 투명도를 설정한 색을 계산하여 _opacColor에 저장하는 COMMAND입니다.
        /// </summary>
        private RelayCommand _changeColorCmd;
        public ICommand ChangeColorCmd { get { return _changeColorCmd ?? (_changeColorCmd = new RelayCommand(ChangeColor, CanExe)); } }
        public void ChangeColor(object sender)
        {
            Color s = Color.FromRgb((byte)selectColor.r, (byte)selectColor.g, (byte)selectColor.b);
            Color b = Color.FromRgb((byte)backgroundColor.r, (byte)backgroundColor.g, (byte)backgroundColor.b);

            Color change = Blend(s, b, opacity * 0.01f);
            opacColor = new Models.ColorInfo()
            {
                solidColorbrush = new SolidColorBrush(change)
            };
        }

        /// <summary>
        /// 투명도 10 ~100% 사이의 색을 계산하여 _opacColorList에 저장하는 COMMAND입니다.
        /// </summary>
        private void CalcOpacityColor()
        {
            _opacColorList = new Models.ColorInfo[9];

            for(int i=1; i<10; i++)
            {
                Color s = Color.FromRgb((byte)selectColor.r, (byte)selectColor.g, (byte)selectColor.b);
                Color b = Color.FromRgb((byte)backgroundColor.r, (byte)backgroundColor.g, (byte)backgroundColor.b);

                Color tmp = Blend(s, b, i * 0.1f);

                _opacColorList[i - 1] = new Models.ColorInfo() { solidColorbrush = new SolidColorBrush(tmp)};
            }
            OpacColorList = _opacColorList;
        }

        /// <summary>
        /// 2가지의 색을 섞는 함수입니다. 
        /// </summary>
        /// <param name="color"></param>
        /// <param name="backColor"></param>
        /// <param name="alpha">color 색의 비율을 나타냅니다. 0~1 사이의 값으로 사용됩니다.</param>
        /// <returns></returns>
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
        #region [변수]
        // 색 모음의 리스트
        private ObservableCollection<Models.ColorChip> _colorchiplist = new ObservableCollection<Models.ColorChip>();
        public ObservableCollection<Models.ColorChip> ColorChipList { get => _colorchiplist; set { _colorchiplist = value; OnPropertyChanged("ColorChipList"); } }

        // 선택된 색 모음
        private Models.ColorChip _selectedChip = new Models.ColorChip();
        public Models.ColorChip SelectedChip { get => _selectedChip; set { _selectedChip = value; OnPropertyChanged("SelectedChip"); } }

        // AddColor Window
        Views.AddColor ac;
        #endregion

        #region [COMMAND]
        /// <summary>
        /// 색 모음이 저장된 폴더에서 색 모음 내용들을 읽어오는 함수입니다.
        /// </summary>
        private void LoadColorChip()
        {
            ColorChipList.Clear();
            string DirPath = currentDirectoryPath + saveDirPath;

            DirectoryInfo di = new DirectoryInfo(DirPath);
            if (!di.Exists)
            {
                Directory.CreateDirectory(DirPath);
            }

            foreach (FileInfo fi in di.GetFiles())
            {
                if (fi.Extension.ToLower().Equals(".txt"))
                {
                    string[] lines = File.ReadAllLines(fi.FullName);
                    ObservableCollection<Models.ColorInfo> colors = new ObservableCollection<Models.ColorInfo>();
                    foreach (string line in lines)
                    {
                        try
                        {
                            Color color = (Color)ColorConverter.ConvertFromString(line);

                            Models.ColorInfo ci = new Models.ColorInfo()
                            {
                                solidColorbrush = new SolidColorBrush(color)
                            };
                            colors.Add(ci);
                        }
                        catch
                        {
                            Log("Color Load Error!(" + fi.Name + ")\n");
                        }
                    }

                    Models.ColorChip cc = new Models.ColorChip()
                    {
                        name = fi.Name.Replace(fi.Extension, ""),
                        path = fi.FullName,
                        idxColor = colors[0],
                        colors = colors
                    };

                    ColorChipList.Add(cc);
                }
            }
            if (ColorChipList.Count > 0)
            {
                SelectedChip = ColorChipList[0];
            }
        }

        /// <summary>
        /// 선택된 색 모음(SelectedChip)을 삭제하는 COMMAND입니다. 선택한 상태에서 'Delete' 버튼을 누르면 삭제됩니다.
        /// </summary>
        private RelayCommand _tab2LdeleteListItemCmd;
        public ICommand Tab2L_DeleteListItemCmd { get { return _tab2LdeleteListItemCmd ?? (_tab2LdeleteListItemCmd = new RelayCommand(Tab2L_DeleteListItem, CanExe)); } }
        private void Tab2L_DeleteListItem(object sender)
        {
            if(MessageBox.Show("정말 삭제하시겠습니까?", "삭제", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                if (File.Exists(SelectedChip.path))
                {
                    try
                    {
                        File.Delete(SelectedChip.path);

                        ColorChipList.Remove(SelectedChip);

                        if(ColorChipList.Count > 0)
                        {
                            SelectedChip = ColorChipList[0];
                        }
                    }
                    catch
                    {
                        Log("Delete File Error!(" + SelectedChip.name + ")");
                        MessageBox.Show("삭제에 실패했습니다.");
                    }
                }
            }
        }

        /// <summary>
        /// SelectedChip.colors에서 색을 삭제하는 COMMAND입니다. 선택한 상태에서 'Delete' 버튼을 누르면 삭제됩니다.
        /// </summary>
        private RelayCommand _tab2RdeleteListItemCmd;
        public ICommand Tab2R_DeleteListItemCmd { get { return _tab2RdeleteListItemCmd ?? (_tab2RdeleteListItemCmd = new RelayCommand(Tab2R_DeleteListItem, CanExe)); } }
        private void Tab2R_DeleteListItem(object sender)
        {
            deleting = true;
            if (SelectedItemsIdx.Count > 0)
            {
                foreach (int idx in SelectedItemsIdx)
                {
                    _selectedChip.colors.RemoveAt(idx);
                }
                SelectedChip = _selectedChip;
            }
            deleting = false;
        }

        /// <summary>
        /// AddColor Window를 여는 COMMAND입니다.
        /// </summary>
        private RelayCommand _openaddColorCmd;
        public ICommand OpenAddColorCmd { get { return _openaddColorCmd ?? (_openaddColorCmd = new RelayCommand(OpenAddColor, CanExe)); } }
        private void OpenAddColor(object sender)
        {
            ac = new Views.AddColor();
            ac.Owner = Application.Current.MainWindow;
            ac.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            ac.DataContext = this;
            ac.ShowDialog();
        }

        /// <summary>
        /// AddColor Window를 닫는 COMMAND입니다. 이건 사실 ViewModel로 코드를 구성하지 않고 Views에서 구성해도 무관할 것 같습니다.
        /// </summary>
        private RelayCommand _closeAddColorCmd;
        public ICommand CloseAddColorCmd { get { return _closeAddColorCmd ?? (_closeAddColorCmd = new RelayCommand(CloseAddColor, CanExe)); } }
        private void CloseAddColor(object sender)
        {
            ac.Close();
        }

        /// <summary>
        /// AddColor Window에서 선택한 색들을 추가하는 COMMAND입니다.
        /// </summary>
        private RelayCommand _addColorCmd2;
        public ICommand AddColorCmd2 { get { return _addColorCmd2 ?? (_addColorCmd2 = new RelayCommand(AddColor2, CanExe)); } }
        private void AddColor2(object sender)
        {
            if (SelectedItemsIdx.Count > 0)
            {
                SelectedItemsIdx.Reverse();
                foreach (int idx in SelectedItemsIdx)
                {
                    Models.ColorInfo ci = ListColors[idx];
                    SelectedChip.colors.Add(ci);
                }
                SelectedItemsIdx.Clear();
            }
            ac.Close();
        }

        /// <summary>
        /// 선택된 색 모음에서 수정 버튼을 누를 때 이름 및 색을 다시 저장하는 COMMAND입니다.
        /// </summary>
        private RelayCommand _resetColorChipCmd;
        public ICommand ResetColorChipCmd { get { return _resetColorChipCmd ?? (_resetColorChipCmd = new RelayCommand(ResetColorChip, CanExe)); } }
        private void ResetColorChip(object sender)
        {
            FileInfo nowPath = new FileInfo(_selectedChip.path);
            FileInfo newPath = new FileInfo(nowPath.DirectoryName + @"\" + SelectedChip.name  +".txt");
            if (!nowPath.FullName.Equals(newPath.FullName))
            {
                if (newPath.Exists)
                {
                    MessageBox.Show("이미 다른 색 모음에서 사용중인 이름입니다. 이름을 변경해주세요.");
                    return;
                }
            }

            try{
                File.Move(nowPath.FullName, newPath.FullName);

                using (StreamWriter sw = new StreamWriter(newPath.FullName))
                {
                    sw.WriteLine("");
                    sw.Close();
                }

                using (StreamWriter sw = File.AppendText(newPath.FullName))
                {
                    if (_selectedChip.colors.Count > 0)
                    {
                        foreach (var item in _selectedChip.colors)
                        {
                            sw.WriteLine("#" + item.hex);
                        }
                    }
                    sw.Close();
                    MessageBox.Show("수정되었습니다.");
                }
            }
            catch
            {

            }
        }
        #endregion
    }
    #endregion


    #region [공통]
    public partial class MainViewModel : VMBase
    {
        #region [변수]
        // 색 모음 경로
        private string saveDirPath = @"\ColorChips";
        private string currentDirectoryPath = Environment.CurrentDirectory.ToString();

        // 이전 Tab Control Index
        private int pre_tabIdx = 0;

        // Tab Control 리스트뷰에서 선택된 아이템들의 인덱스 모음
        private List<int> SelectedItemsIdx = new List<int>();

        // 현재 삭제 중인지를 나타내는 변수
        private bool deleting = false;

        // 리스트뷰를 담기 위한 변수
        ListView lv;

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

        // 선택된 색
        private Models.ColorInfo _selectColor = new Models.ColorInfo();
        public Models.ColorInfo selectColor { get => _selectColor; set { _selectColor = value; CalcOpacityColor(); ChangeColor(null); OnPropertyChanged("selectColor"); } }

        // 색 모음 리스트의 Visibility
        private Visibility _leftLV = Visibility.Collapsed;
        public Visibility leftLV { get => _leftLV; set { _leftLV = value; OnPropertyChanged("leftLV"); } }
        #endregion

        #region [UI EVENT & COMMAND]
        /// <summary>
        /// TOGGLE BUTTON을 이용하여 FollowMouseWindow를 여는 이벤트 함수입니다. FollowMouseWindow를 사용할 때 후킹도 함께 사용해야하므로 timer도 같이 설정합니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OepnFMW(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = sender as ToggleButton;
            if (tb.IsChecked == true)
            {
                _listener = new LowLevelKeyboardListener();
                _listener.doEvent += PickColor;
                _listener.HookKeyboard();

                timer.Interval = TimeSpan.FromMilliseconds(1);
                timer.Tick += new EventHandler(doTimer);
                timer.Start();

                f = new Views.FollowMouseWindow();
                f.DataContext = this;
                f.Show();
            }
            else
            {
                f.Close();
                _listener.UnHookKeyboard();
                if (timer.IsEnabled)
                {
                    timer.Stop();
                }
            }
        }

        /// <summary>
        /// 현재 MainWindow를 닫을 경우 사용하는 이벤트 함수입니다. timer와 후킹을 중지한 뒤 종료합니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Slider의 값이 바뀔 때마다 선택된 색을 설정하도록 하는 이벤트 함수입니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// 0-9 숫자만 입력되도록하는 이벤트 함수입니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void NumPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        /// <summary>
        /// 0-9A-F 16진수만 입력되도록하는 이벤트 함수입니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// TabControl에서 Tab Index가 바뀔 때마다 사용되는 이벤트 함수입니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TabControl tc = sender as TabControl;
            leftLV = Visibility.Collapsed;

            if (tc.SelectedIndex == 1)
            {
                CalcOpacityColor();
                ChangeColor(null);
            }
            else if(tc.SelectedIndex == 2)
            {
                if(pre_tabIdx != 2)
                {
                    LoadColorChip();
                }
                leftLV = Visibility.Visible;
            }

            if(pre_tabIdx != tc.SelectedIndex)
            {
                SelectedItemsIdx.Clear(); 
                UnSelectAllItems(null);
            }

            pre_tabIdx = tc.SelectedIndex;
        }

        /// <summary>
        /// TabControl 안의 ListView에서 선택할때 사용되는 이벤트 함수입니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Tab_LV_SelectionChanged(object sender, RoutedEventArgs e)
        {
            if (!deleting)
            {
                lv = sender as ListView;
                SelectedItemsIdx.Clear();
                if (lv != null)
                {
                    foreach (Models.ColorInfo item in lv.SelectedItems)
                    {
                        int idx = lv.Items.IndexOf(item);
                        SelectedItemsIdx.Add(idx);
                        if(pre_tabIdx != 2)
                        {
                            selectColor = new Models.ColorInfo()
                            {
                                solidColorbrush = item.solidColorbrush
                            };
                        }
                    }
                    SelectedItemsIdx.Reverse();
                }
            }
        }

        /// <summary>
        /// 왼쪽 Border에 나타난 색(선택된 색, selectColor)을 ListColors에 추가하는 COMMAND입니다.
        /// </summary>
        private RelayCommand _addColorCmd;
        public ICommand AddColorCmd { get { return _addColorCmd ?? (_addColorCmd = new RelayCommand(AddColor, CanExe)); } }
        private void AddColor(object args)
        {
            Models.ColorInfo newColor = new Models.ColorInfo()
            {
                solidColorbrush = selectColor.solidColorbrush
            };
            ListColors.Add(newColor);
        }

        /// <summary>
        /// COMMAND에서 사용되는 공용 취소 COMMAND입니다.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private bool CanExe(object args)
        {
            return true;
        }
        #endregion

        #region [함수]
        public MainViewModel()
        {
            ListColors.Clear();
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
            string DirPath = Path.Combine(currentDirectoryPath, "Logs");
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
