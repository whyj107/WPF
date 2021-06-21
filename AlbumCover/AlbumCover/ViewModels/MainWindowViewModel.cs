using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using AlbumCover.Models;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Dialogs;
using TagLib;

namespace AlbumCover.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        #region [공통 변수]
        // taglib에서 허용되는 오디오 확장자 모음입니다. 몇가지만 주로 사용될 것 같습니다.
        string[] extensions = { ".aa", ".aax", ".aac", ".aiff", ".ape", ".dsf", ".flac", ".m4a",
                                ".m4b", ".m4p", ".mp3", ".mpc", ".mpp", ".ogg", ".oga", ".wav",
                                ".wma", ".wv", ".webm" };

        // [앨범 커버 변경]과 [음원 정보 변경] 두 부분에서 사용되는 리스트 아이템입니다.
        private ObservableCollection<SongInfo> _listItems = new ObservableCollection<SongInfo>();
        public ObservableCollection<SongInfo> ListItems{ get => _listItems; set { _listItems = value; OnPropertyChanged("ListItems"); } }

        // [앨범 커버 변경]과 [음원 정보 변경] 두 부분에서 보여지는 앨범 커버 이미지입니다.
        private BitmapImage _showImg;
        public BitmapImage showImg { get => _showImg; set { _showImg = value; OnPropertyChanged("showImg"); } }
        #endregion

        #region 00. [앨범 커버 변경]
        #region [변수]
        // 앨범 정보입니다.
        private AlbumInfo _album = new AlbumInfo() { album="Album", albumArtist="Artist" };
        public AlbumInfo Album { get => _album; set { _album = value; OnPropertyChanged("Album"); } }

        // 앨범 디렉토리 경로입니다.
        private string _musicDirPath = "앨범 경로";
        public string musicDirPath{ get => _musicDirPath; set { _musicDirPath = value; OnPropertyChanged("musicDirPath"); } }

        // [BACKGROUNDWORKER] : 앨범 변경에서 시간이 오래 소요될 경우를 대비하여 백그라운드 워커와 로딩창을 준비했습니다.
        BackgroundWorker bw = new BackgroundWorker();
        Views.LoadingWindow l = new Views.LoadingWindow();

        // 로딩 페이지 사용 시 뒤에 깔아 둘 배경의 VISIBILITY 입니다.
        private Visibility _loading_back = Visibility.Collapsed;
        public Visibility loading_back { get => _loading_back; set { _loading_back = value; OnPropertyChanged("loading_back"); } }
        #endregion

        #region [버튼 클릭 이벤트(커맨드)]
        #region 00. [디렉토리 선택 버튼]
        // COMMAND BINDING을 위한 부분입니다.
        private RelayCommand _selectBtnCmd;
        public ICommand SelectBtnCmd { get { return this._selectBtnCmd ?? (this._selectBtnCmd = new RelayCommand(ExeSelectBtn, CanExe)); } }
        
        /// <summary>
        /// COMMON OPEN FILE DIALOG를 이용하여 디렉토리 선택하는 함수입니다.(디렉토리 선택 시 예뻐서 사용한 것이므로 다른 방법으로 변경해도 무관합니다.)
        /// </summary>
        /// <param name="args"></param>
        private void ExeSelectBtn(object args)
        {
            CommonOpenFileDialog cofd = new CommonOpenFileDialog();
            cofd.IsFolderPicker = true;

            if(cofd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                ReadDirectory(cofd.FileName);
            }
        }
        #endregion

        #region 01. [저장 버튼]
        // COMMAND BINDING을 위한 부부입니다.
        private RelayCommand _saveAlbumBtnCmd;
        public ICommand SaveAlbumBtnCmd { get { return this._saveAlbumBtnCmd ?? (this._saveAlbumBtnCmd = new RelayCommand(ExeSaveAlbumBtn, CanExe)); } }

        /// <summary>
        /// LISTVIEW에 있는 모든 음악들의 앨범커버, 앨범이름과 아티스트 정보를 바꾸는 함수입니다.
        /// 작업은 BACKGROUNDWORKER를 사용하여 진행하고 진행하는동안 로딩 페이지를 띄웁니다.
        /// </summary>
        /// <param name="args"></param>
        private void ExeSaveAlbumBtn(object args)
        {
            bw.DoWork += new DoWorkEventHandler(DoWork);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(RunWorkerCompleted);

            bw.RunWorkerAsync();

            l = new Views.LoadingWindow();
            l.Owner = Application.Current.MainWindow;
            l.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            
            loading_back = Visibility.Visible;

            l.ShowDialog();
        }
        #endregion
        #endregion

        #region [BACKGROUNDWORKER]
        /// <summary>
        /// 백그라운드에서 작업을 진행하는 함순입니다.
        /// LISTVIEW에 있는 음원들의 앨범 커버, 앨범 이름과 아티스트 이름을 변경합니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DoWork(object sender, DoWorkEventArgs e)
        {
            // 선택한 음원의 이미지로 변경할 경우 tmp.png로 저장한 뒤 저장한 이미지로 앨범 커버를 적용합니다.
            string tmp_path = Path.Combine(musicDirPath + "\\tmp.png");
            if (!System.IO.File.Exists(Album.coverArtpath))
            {
                DirectoryInfo di = new DirectoryInfo(musicDirPath);
                if (di.Exists)
                {
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(showImg));

                    using (var fileStream = new FileStream(tmp_path, FileMode.Create))
                    {
                        encoder.Save(fileStream);
                    }
                    Album.coverArtpath = tmp_path;
                }
            }

            // LISTVIEW에 있는 음원들의 정보를 변경하는 부분입니다.
            foreach (var file in ListItems)
            {
                var tfile = TagLib.File.Create(file.path);

                tfile.Tag.Title = file.song;

                string tmp_artist = Album.albumArtist.Replace(", ", ",").Trim();
                tfile.Tag.Artists = tmp_artist.Split(',');

                tfile.Tag.Album = Album.album;

                if (System.IO.File.Exists(Album.coverArtpath) && showImg != null)
                {
                    var pic = new IPicture[1];
                    pic[0] = new Picture(Album.coverArtpath);
                    tfile.Tag.Pictures = pic;
                }

                tfile.Save();
                tfile.Dispose();
            }

            // 사용했던 tmp.png 이미지를 삭제합니다.
            if (System.IO.File.Exists(tmp_path))
            {
                System.IO.File.Delete(tmp_path);
            }
        }

        /// <summary>
        /// 백그라운드의 작업이 완료되면 진행하는 함수입니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (l.ShowActivated == true)
            {
                l.Close();
            }
            string message = "저장되었습니다.";
            if (ListItems.Count < 1)
            {
                message = "저장할 음원을 추가해주세요.";
            }

            MessageBox.Show(message);

            ReadDirectory(musicDirPath);

            loading_back = Visibility.Collapsed;
        }
        #endregion

        #region [기타]
        /// <summary>
        /// 디렉토리 경로에서 음원 파일을 읽어서 LISTITEMS(LISTVIEW와 연결된 변수)에 저장하는 함수입니다.
        /// </summary>
        /// <param name="path_"></param>
        public void ReadDirectory(string path_)
        {
            musicDirPath = path_;

            DirectoryInfo di = new DirectoryInfo(musicDirPath);
            if (di.Exists)
            {
                ListItems.Clear();
                foreach (var file in di.GetFiles())
                {
                    AddSongInfo(new string[] { file.FullName });
                }
            }
        }
        #endregion
        #endregion

        #region 01. [음원 정보 변경]
        #region [변수]
        // 선택된 음원 정보입니다.
        private SongInfo _selectedItem = new SongInfo() { song = "Title", artist = "Artist", album = "Album" };
        public SongInfo SelectedItem { get => _selectedItem; set { _selectedItem = value; OnPropertyChanged("SelectedItem"); } }
        #endregion

        #region [버튼 클릭 이벤트(커맨드)]
        #region 00. [음원 추가 버튼]
        // COMMAND BINDING을 위한 부분입니다.
        private RelayCommand _addBtnCmd;
        public ICommand AddBtnCmd { get { return this._addBtnCmd ?? (this._addBtnCmd = new RelayCommand(ExeAddBtn, CanExe)); } }

        /// <summary>
        /// OPEN FILE DIALOG 이용하여 음원을 추가하는 함수입니다.
        /// </summary>
        /// <param name="args"></param>
        private void ExeAddBtn(object args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Multiselect = true;
            ofd.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic);
            if(ofd.ShowDialog() == true)
            {
                AddSongInfo(ofd.FileNames);
            }
        }
        #endregion

        #region 01. [음원 삭제 버튼]
        // COMMAND BINDING을 위한 부분입니다.
        private RelayCommand _delBtnCmd;
        public ICommand DelBtnCmd { get { return this._delBtnCmd ?? (this._delBtnCmd = new RelayCommand(ExeDelBtn, CanExe)); } }

        /// <summary>
        /// 선택된 음원(SELECTEDITEM)을 LISTITEMS(LISTVIEW)에서 삭제하는 함수입니다.
        /// </summary>
        /// <param name="args"></param>
        private void ExeDelBtn(object args)
        {
            ListItems.Remove(SelectedItem);
        }
        #endregion

        #region 02. [저장 버튼]
        // COMMAND BINDING을 위한 부분입니다.
        private RelayCommand _saveInfoBtnCmd;
        public ICommand SaveInfoBtnCmd { get { return this._saveInfoBtnCmd ?? (this._saveInfoBtnCmd = new RelayCommand(ExeSaveInfoBtn, CanExe)); } }

        /// <summary>
        /// 선택된 음원(SELECTEDITEM)의 정보들(음원명, 아티스트명, 앨범명 등)을 저장하는 함수입니다.
        /// </summary>
        /// <param name="args"></param>
        private void ExeSaveInfoBtn(object args)
        {
            var tfile = TagLib.File.Create(SelectedItem.path);

            tfile.Tag.Title = SelectedItem.song;

            string tmp_artist = SelectedItem.artist.Replace(", ", ",").Trim();
            tfile.Tag.Artists = tmp_artist.Split(',');

            tfile.Tag.Album = SelectedItem.album;

            string tmp_genre = SelectedItem.genre.Replace(", ", ",").Trim();
            tfile.Tag.Genres = tmp_genre.Split(',');

            tfile.Tag.Year = SelectedItem.year;

            tfile.Tag.Track = SelectedItem.trackNum;

            if (System.IO.File.Exists(SelectedItem.coverArtpath) && showImg != null)
            {
                var pic = new IPicture[1];
                pic[0] = new Picture(SelectedItem.coverArtpath);
                tfile.Tag.Pictures = pic;
                SelectedItem.covertArt = tfile.Tag.Pictures[0];
            }

            tfile.Save();
            tfile.Dispose();
            MessageBox.Show("저장되었습니다.");
        }
        #endregion
        #endregion

        #region [LISTVIEW 이벤트]
        /// <summary>
        /// 드레그하여 LISTVIEW 안으로 들어올 떄 발생하는 함수입니다. 외부 디렉토리에서 음원 파일을 추가하기 위하여 만든 이벤트입니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }

        /// <summary>
        /// LISTVIEW에서 드레그 드랍했을 때 발생하는 함수입니다. 외부 디렉토리에서 음원 파일을 추가하기 위하여 만든 이벤트입니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            AddSongInfo(files);
        }

        /// <summary>
        /// LISTVIEW에서 아이템 선택이 바뀔 경우 발생하는 함수입니다. 미리 SELECTITEM과 선택된 리스트를 연결해 두었기 때문에 앨범 커버만 여기서 바꿔줍니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void LV_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                showImg = SelectedItem.coverImg;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                showImg = null;
            }
        }
        #endregion

        #region [TEXTBOX 이벤트]
        /// <summary>
        /// TEXTBOX에 입력시 정규표현식을 사용하여 숫자만 입력되도록 하는 함수입니다.
        /// YEAR와 TRACK 항목에 사용됩니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void PreviewTextInputOnlyNumber(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        #endregion
        #endregion

        #region [공용 이벤트(커맨드)]
        #region [이미지 변경 버튼]
        // COMMAND BINDING을 위한 부분입니다.
        private RelayCommand _changeImgBtnCmd;
        public ICommand ChangeImgBtnCmd { get { return this._changeImgBtnCmd ?? (this._changeImgBtnCmd = new RelayCommand(ExeChangeImgBtn, CanExe)); } }

        /// <summary>
        /// OPEN FILE DIALOG를 이용하여 이미지를 추가하는 함수입니다.
        /// </summary>
        /// <param name="args"></param>
        private void ExeChangeImgBtn(object args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image File|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
            if (ofd.ShowDialog() == true)
            {
                if(SelectedItem == null)
                {
                    SelectedItem = new SongInfo();
                }

                SelectedItem.coverArtpath = ofd.FileName;

                BitmapImage tmp = new BitmapImage();
                tmp.BeginInit();
                tmp.UriSource = new Uri(ofd.FileName, UriKind.RelativeOrAbsolute);
                tmp.EndInit();

                showImg = tmp;
            }
        }
        #endregion

        #region [이미지 초기화 버튼]
        // COMMAND BINDING을 위한 부분입니다.
        private RelayCommand _resetImgBtnCmd;
        public ICommand ResetImgBtnCmd { get { return this._resetImgBtnCmd ?? (this._resetImgBtnCmd = new RelayCommand(ExeResetImgBtn, CanExe)); } }

        /// <summary>
        /// 원래 이미지로 변경하는 함수입니다.
        /// </summary>
        /// <param name="args"></param>
        private void ExeResetImgBtn(object args)
        {
            if (SelectedItem != null)
            {
                showImg = SelectedItem.coverImg;
            }
        }
        #endregion

        #region [TABCONTROL 이벤트]
        /// <summary>
        /// TABCONTROL에서 다른 TAB을 선택할 떄 발생하는 함수입니다. [음원 정보 변경]->[앨범 커버 변경]으로 이동할 때 리스트를 REFRESH하기 위하여 만든 이벤트입니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void TC_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var tc = sender as TabControl;

            if(tc.SelectedIndex == 0)
            {
                try
                {
                    ReadDirectory(musicDirPath);
                }catch(Exception ex)
                {
                    Console.WriteLine("TC_SelectionChnaged Error");
                    Console.WriteLine(ex.Message);
                }
            }
        }
        #endregion

        #region [WINDOW 이벤튼]
        /// <summary>
        /// 윈도우를 닫을 때 완전히 종료하도록하는 함수입니다.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Window_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }
        #endregion

        // COMMAND BINDING 사용 시 필요한 함수입니다.
        private bool CanExe(object args)
        {
            return true;
        }
        #endregion

        #region [기타]
        /// <summary>
        /// LISTITEMS(LISTVIEW)에 음원 정보를 추가하는 함수입니다.
        /// </summary>
        /// <param name="files"></param>
        private void AddSongInfo(string[] files)
        {
            foreach (string file in files)
            {
                // 음원 확장자 확인
                if (Array.Exists(extensions, x => x == Path.GetExtension(file)))
                {
                    // path으로 중복 확인
                    if (ListItems.Any(x => x.path == file) == false)
                    {
                        ListItems.Add(ReadSongInfo(file));
                    }
                }
            }
        }

        /// <summary>
        /// 음원 파일에서 음원을 객체에 저장하는 함수입니다.
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private SongInfo ReadSongInfo(string file)
        {
            var tfile = TagLib.File.Create(file);

            IPicture picture = null;
            if (tfile.Tag.Pictures.Length > 0)
            {
                picture = tfile.Tag.Pictures[0];
            }

            SongInfo si = new SongInfo()
            {
                song = tfile.Tag.Title,
                artist = string.Join(", ", tfile.Tag.Artists),
                album = tfile.Tag.Album,
                albumArtist = string.Join(", ", tfile.Tag.AlbumArtists),
                genre = string.Join(", ", tfile.Tag.Genres),
                year = tfile.Tag.Year,
                trackNum = tfile.Tag.Track,
                path = file,
                covertArt = picture
            };

            tfile.Dispose();

            return si;
        }
        #endregion
    }
}
