using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using AlbumCover.Models;
using TagLib;

namespace AlbumCover.ViewModels
{
    class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<AlbumInfo> _listItems = new ObservableCollection<AlbumInfo>();
        public ObservableCollection<AlbumInfo> ListItems
        {
            get => _listItems;
            set
            {
                _listItems = value;
                OnPropertyChanged("ListItems");
            }
        }
        private AlbumInfo _selectedItem = new AlbumInfo();
        public AlbumInfo SelectedItem {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");
            }
        }

        #region EVNET 방법 1 : 아직까지 안썼는데 잘 동작하므로 나둠
        private RelayCommand _dropCommand;
        public ICommand DropCommand {
            get
            {
                return this._dropCommand ?? (this._dropCommand = new RelayCommand(ExecuteDrop, CanExecuteDrop));
            }
        }
        private void ExecuteDrop(object args)
        {
            MessageBox.Show("들어왔으");
        }
        private bool CanExecuteDrop(object args)
        {
            return true;
        }
        #endregion

        #region [List Drag & Drop]
        public void DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }

        public void Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            string[] extensions = { ".aa", ".aax", ".aac", ".aiff", ".ape", ".dsf", ".flac", ".m4a",
                                    ".m4b", ".m4p", ".mp3", ".mpc", ".mpp", ".ogg", ".oga", ".wav",
                                    ".wma", ".wv", ".webm" };
            foreach (string file in files)
            {
                // 음원 확장자 확인
                if (Array.Exists(extensions, x => x == Path.GetExtension(file)))
                {
                    ListItems.Add(readAlbumInfo(file));
                }
            }
        }
        #endregion

        // 앨범 정보 추가
        private AlbumInfo readAlbumInfo(string file)
        {
            var tfile = TagLib.File.Create(file);

            IPicture picture = null;
            if(tfile.Tag.Pictures.Length > 0)
            {
                picture = tfile.Tag.Pictures[0];
            }

            return new AlbumInfo()
            {
                path = file,
                song = tfile.Tag.Title,
                artist = string.Join(", ", tfile.Tag.Artists),
                genre = string.Join(", ", tfile.Tag.Genres),
                trackNum = (int)tfile.Tag.Track,
                covertArt = picture
            };
        }
    }
}
