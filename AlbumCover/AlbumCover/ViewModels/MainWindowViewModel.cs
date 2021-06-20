using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
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
        #region [Variable]
        // 오디오 확장자
        string[] extensions = { ".aa", ".aax", ".aac", ".aiff", ".ape", ".dsf", ".flac", ".m4a",
                                ".m4b", ".m4p", ".mp3", ".mpc", ".mpp", ".ogg", ".oga", ".wav",
                                ".wma", ".wv", ".webm" };

        private SongInfo _selectedItem = new SongInfo() { song = "Title", artist = "Artist", album="Album" };
        public SongInfo SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }
    
        private ObservableCollection<SongInfo> _listItems = new ObservableCollection<SongInfo>();
        public ObservableCollection<SongInfo> ListItems
        {
            get => _listItems;
            set
            {
                _listItems = value;
                OnPropertyChanged("ListItems");
            }
        }

        private BitmapImage _showImg;
        public BitmapImage showImg { get => _showImg; set { _showImg = value; OnPropertyChanged("showImg"); } }
        #endregion

        #region 0. [Change Album Cover]
        #region [Variable]
        private string _musicDirPath = "Music Directory Path";
        public string musicDirPath{ get => _musicDirPath; set { _musicDirPath = value; OnPropertyChanged("musicDirPath"); } }
        #endregion

        #region [Button Command]
        #region 00. [Select Directory Button]
        private RelayCommand _selectBtnCmd;
        public ICommand SelectBtnCmd
        {
            get
            {
                return this._selectBtnCmd ?? (this._selectBtnCmd = new RelayCommand(ExeSelectBtn, CanExe));
            }
        }
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

        #region 01. [Save Information]
        private RelayCommand _saveAlbumBtnCmd;
        public ICommand SaveAlbumBtnCmd
        {
            get
            {
                return this._saveAlbumBtnCmd ?? (this._saveAlbumBtnCmd = new RelayCommand(ExeSaveAlbumBtn, CanExe));
            }
        }
        private void ExeSaveAlbumBtn(object args)
        {
            string tmp_path = Path.Combine(musicDirPath + "\\tmp.png");
            if (!System.IO.File.Exists(SelectedItem.coverArtpath))
            {
                BitmapEncoder encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(showImg));
                using (var fileStream = new FileStream(tmp_path, FileMode.Create))
                {
                    encoder.Save(fileStream);
                }
                SelectedItem.coverArtpath = tmp_path;
            }

            foreach (var file in ListItems)
            {
                var tfile = TagLib.File.Create(file.path);

                tfile.Tag.Title = file.song;

                string tmp_artist = SelectedItem.artist.Replace(", ", ",").Trim();
                tfile.Tag.Artists = tmp_artist.Split(',');

                tfile.Tag.Album = SelectedItem.album;

                if (System.IO.File.Exists(SelectedItem.coverArtpath) && showImg != null)
                {
                    var pic = new IPicture[1];
                    pic[0] = new Picture(SelectedItem.coverArtpath);
                    tfile.Tag.Pictures = pic;
                }

                tfile.Save();
                tfile.Dispose();
            }

            if (System.IO.File.Exists(tmp_path))
            {
                System.IO.File.Delete(tmp_path);
            }

            ReadDirectory(musicDirPath);
        }

        #endregion

        #endregion

        #region [기타]
        public void ReadDirectory(string path_)
        {
            musicDirPath = path_;

            ListItems.Clear();

            DirectoryInfo di = new DirectoryInfo(musicDirPath);
            foreach (var file in di.GetFiles())
            {
                AddSongInfo(new string[] { file.FullName });
            }
        }
        #endregion

        #endregion

        #region 1. [Change Song Information]
        #region [Button Command]
        #region 00. [+ Button]
        private RelayCommand _addBtnCmd;
        public ICommand AddBtnCmd
        {
            get
            {
                return this._addBtnCmd ?? (this._addBtnCmd = new RelayCommand(ExeAddBtn, CanExe));
            }
        }
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

        #region 01. [- Button]
        private RelayCommand _delBtnCmd;
        public ICommand DelBtnCmd {
            get
            {
                return this._delBtnCmd ?? (this._delBtnCmd = new RelayCommand(ExeDelBtn, CanExe));
            }
        }
        private void ExeDelBtn(object args)
        {
            ListItems.Remove(SelectedItem);
        }
        #endregion

        #region 02. [Save Information]
        private RelayCommand _saveInfoBtnCmd;
        public ICommand SaveInfoBtnCmd
        {
            get
            {
                return this._saveInfoBtnCmd ?? (this._saveInfoBtnCmd = new RelayCommand(ExeSaveInfoBtn, CanExe));
            }
        }
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
        }

        #endregion

        #endregion

        #region [ListView Command]
        public void DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }

        public void Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

            AddSongInfo(files);
        }

        public void SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            try
            {
                showImg = SelectedItem.coverImg;
            }
            catch (Exception ex)
            {
                showImg = null;
            }
        }

        #endregion

        #region [TextBox Command]
        // 숫자만 입력
        public void PreviewTextInputOnlyNumber(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        #endregion

        #endregion

        #region [COMMON COMMAND]

        #region 00. [Change Image Button]
        private RelayCommand _changeImgBtnCmd;
        public ICommand ChangeImgBtnCmd
        {
            get
            {
                return this._changeImgBtnCmd ?? (this._changeImgBtnCmd = new RelayCommand(ExeChangeImgBtn, CanExe));
            }
        }
        private void ExeChangeImgBtn(object args)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Image File|*.jpg;*.jpeg;*.png;*.gif;*.bmp";
            if (ofd.ShowDialog() == true)
            {
                SelectedItem.coverArtpath = ofd.FileName;

                BitmapImage tmp = new BitmapImage();
                tmp.BeginInit();
                tmp.UriSource = new Uri(ofd.FileName, UriKind.RelativeOrAbsolute);
                tmp.EndInit();

                showImg = tmp;
            }
        }
        #endregion

        #region 01. [Reset Image Button]
        private RelayCommand _resetImgBtnCmd;
        public ICommand ResetImgBtnCmd
        {
            get
            {
                return this._resetImgBtnCmd ?? (this._resetImgBtnCmd = new RelayCommand(ExeResetImgBtn, CanExe));
            }
        }
        private void ExeResetImgBtn(object args)
        {
            showImg = SelectedItem.coverImg;
        }
        #endregion

        private bool CanExe(object args)
        {
            return true;
        }
        #endregion

        #region [기타]
        // ListItems에 추가
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

        // 음악 정보 읽기
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
