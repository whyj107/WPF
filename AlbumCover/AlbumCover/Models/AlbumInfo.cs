using System.IO;
using System.Windows.Media.Imaging;
using TagLib;

namespace AlbumCover.Models
{
    public class AlbumInfo
    {
        // 폴더 위치
        private string _path;
        public string path { get => _path; set { _path = value; } }

        // 앨범 이름
        private string _album;
        public string album { get => _album; set { _album = value; } }

        // 아티스트 이름
        private string _albumArtist;
        public string albumArtist { get => _albumArtist; set { _albumArtist = value; } }

        // 트랙 수
        private int _trackCount;
        public int trackCount { get => _trackCount; set { _trackCount = value; } }

        // 앨범 커버 이미지 위치
        private string _coverArtpath;
        public string coverArtpath { get => _coverArtpath; set { _coverArtpath = value; } }

        // 앨범 커버 이미지
        private IPicture _coverArt;
        public IPicture covertArt {
            get { return _coverArt; }
            set {
                _coverArt = value;
                if (_coverArt != null)
                {
                    coverImg = changeImage(value);
                }
            }
        }

        private BitmapImage _coverImg;
        public BitmapImage coverImg { get => _coverImg; set { _coverImg = value; } }

        // IPicture -> BitmapImage
        private BitmapImage changeImage(IPicture picture)
        {
            MemoryStream ms = new MemoryStream(picture.Data.Data);
            ms.Seek(0, SeekOrigin.Begin);

            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = ms;
            bitmap.EndInit();

            return bitmap;
        }
    }
}
